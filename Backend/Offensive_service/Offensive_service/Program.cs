using System;
using Offensive_service.Services;

namespace Offensive_service
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("[STARTING] Offensive content analysis service is starting ...");

            OffensiveAnalysisService service = new OffensiveAnalysisService();
            service.StartConsuming();

            Console.ReadLine();
        }
    }
}
