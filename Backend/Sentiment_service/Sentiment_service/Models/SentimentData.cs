using Microsoft.ML.Data;

namespace Sentiment_service.Models
{
    class SentimentData
    {
        [LoadColumn(0)]
        public string Text { get; set; }

        [LoadColumn(1)]
        public float Sentiment { get; set; }
    }
}
