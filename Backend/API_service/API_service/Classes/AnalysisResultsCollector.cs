using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using Plain.RabbitMQ;
using API_service.Models;
using API_service.Services;

namespace API_service.Classes
{
    public class AnalysisResultsCollector : IHostedService
    {
        private readonly ISubscriber _subscriber;
        private readonly DocumentService _documentService;
        private static WebSocket _webSocket;
        private static TaskCompletionSource<object> _socketFinishedTcs;

        public AnalysisResultsCollector(ISubscriber subscriber, DocumentService documentService)
        {
            _subscriber = subscriber;
            _documentService = documentService;
            _webSocket = null;
            _socketFinishedTcs = null;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriber.Subscribe(ProcessMessage);
            return Task.CompletedTask;
        }

        private bool ProcessMessage(string message, IDictionary<string, object> headers)
        {
            int analysisType = (int)headers["type"];

            JObject analysisResult = JObject.Parse(message);

            Document documentdb = _documentService.GetByName((string)analysisResult["FileName"]);
            Document document;

            // The analyzed document already exists in the database
            if (documentdb != null)
                document = documentdb;
            // The analyzed document doesn't exists in the database
            else
            {
                document = new Document();
                document.Name = (string)analysisResult["FileName"];
            }

            switch (analysisType)
            {
                // Entity analysis
                case 0:
                    break;
                // Sentiment analysis
                case 1:
                    document.SentimentPercentage = (double)analysisResult["SentimentPercentage"];
                    break;
                // Offensive analysis
                case 2:
                    document.OffensivePercentage = (double)analysisResult["OffensivePercentage"];
                    break;
                default:
                    break;
            }

            // Updates the document because it already exists in the database
            if (documentdb != null)
                _documentService.Update(document.Id, document);
            // Creates a new document in the database
            else
                _documentService.Create(document);

            SendMessageWebSocket(document);

            return true;
        }

        private static async void SendMessageWebSocket(Document document)
        {
            var message = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(document));

            if (_webSocket != null && _webSocket.State == WebSocketState.Open)
            {
                await _webSocket.SendAsync(message, WebSocketMessageType.Text, true, CancellationToken.None);
                _socketFinishedTcs.TrySetResult(null);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public static void AddSocket(WebSocket webSocket, TaskCompletionSource<object> socketFinishedTcs)
        {
            _webSocket = webSocket;
            _socketFinishedTcs = socketFinishedTcs;
        }
    }
}
