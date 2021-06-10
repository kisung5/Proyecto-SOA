using System;
using Sentiment_service.Services;

namespace JsonTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("[STARTING]  Sentiment analysis service is starting ...");

            SentimentAnalysisService service = new SentimentAnalysisService();
            service.StartConsuming();

            Console.ReadLine();
        }
    }
}
