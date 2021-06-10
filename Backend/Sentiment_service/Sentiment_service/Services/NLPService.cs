using System;
using System.IO;
using Microsoft.ML;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Sentiment_service.Models;

namespace Sentiment_service.Services
{
    class NLPService
    {
        private readonly string _FILES_DIR;

        public NLPService()
        {
            _FILES_DIR = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\Data\\";
        }

        public float sentiment_analysis(string fileName)
        {
            MLContext context = new MLContext();

            var data = context.Data.LoadFromTextFile<SentimentData>(path: _FILES_DIR + "\\stock_data.csv", 
                                                                    hasHeader: true, 
                                                                    separatorChar: ',', 
                                                                    allowQuoting: true);

            var pipeline = context.Transforms.Expression("Label", "(x) => x == 1 ? true : false", "Sentiment")
                .Append(context.Transforms.Text.FeaturizeText("Features", nameof(SentimentData.Text)))
                .Append(context.BinaryClassification.Trainers.SdcaLogisticRegression());

            var model = pipeline.Fit(data);
            var predictionEngine = context.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
            string text = pdf_to_Text(_FILES_DIR + fileName);
            var Prediction = predictionEngine.Predict(new SentimentData { Text = text });

            return Prediction.Probability;
        }

        private static string pdf_to_Text(string path)
        {
            PdfReader reader = new PdfReader(path);
            string text = string.Empty;
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                text += PdfTextExtractor.GetTextFromPage(reader, page);
            }
            reader.Close();
            return text;
        }

    }
}
