using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Offensive_service.Models;

namespace Offensive_service.Services
{
    class OffensiveAnalysisService
    {
        private readonly CloudStorageService _AZURE_STORAGE;
        private readonly NLPService _NLP;

        private readonly string _RABBITMQ_HOSTNAME;
        private readonly string _RABBITMQ_USERNAME;
        private readonly string _RABBITMQ_PASSWORD;
        private readonly int _RABBITMQ_PORT;
        private readonly string _PUB_EXCHANGE_NAME;
        private readonly string _SUB_EXCHANGE_NAME;
        private readonly string _QUEUE_NAME;

        private IConnection _connection;
        private IModel _channel;

        public OffensiveAnalysisService()
        {
            _AZURE_STORAGE = new CloudStorageService();
            _NLP = new NLPService();

            //StreamReader file = File.OpenText(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\appsettings.json");
            StreamReader file = File.OpenText(Path.Combine(Environment.CurrentDirectory,"appsettings.json"));
            JsonTextReader reader = new JsonTextReader(file);

            JObject configFile = (JObject)JToken.ReadFrom(reader);

            JToken rabbitmqConfig = configFile.GetValue("RabbitMQConnection");
            _RABBITMQ_HOSTNAME = (string)rabbitmqConfig["hostName"];
            _RABBITMQ_USERNAME = (string)rabbitmqConfig["userName"];
            _RABBITMQ_PASSWORD = (string)rabbitmqConfig["password"];
            _RABBITMQ_PORT = (int)rabbitmqConfig["port"];
            _PUB_EXCHANGE_NAME = (string)rabbitmqConfig["pub_exchange"];
            _SUB_EXCHANGE_NAME = (string)rabbitmqConfig["sub_exchange"];
            _QUEUE_NAME = (string)rabbitmqConfig["queue"];

            InitService();
        }

        private void InitService()
        {
            _connection = CreateConnetion();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: _SUB_EXCHANGE_NAME,
                                     type: ExchangeType.Topic);

            _channel.QueueDeclare(queue: _QUEUE_NAME,
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            _channel.QueueBind(queue: _QUEUE_NAME,
                               exchange: _SUB_EXCHANGE_NAME,
                               routingKey: "offensive.analysis");

            _channel.QueueBind(queue: _QUEUE_NAME,
                               exchange: _SUB_EXCHANGE_NAME,
                               routingKey: "general.analysis");

            /*channel.BasicQos(prefetchSize: 0,
                  prefetchCount: 10, 
                  global: false);*/
        }

        private IConnection CreateConnetion()
        {
            var factory = new ConnectionFactory
            {
                HostName = _RABBITMQ_HOSTNAME,
                UserName = _RABBITMQ_USERNAME,
                Password = _RABBITMQ_PASSWORD,
                Port = _RABBITMQ_PORT
            };

            return factory.CreateConnection();
        }

        public void StartConsuming()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (sender, e) => {

                var body = e.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);

                Console.WriteLine(message);

                ProcessMessage(message);
            };

            _channel.BasicConsume(queue: _QUEUE_NAME,
                                 autoAck: true,
                                 consumer: consumer);
        }

        private async void ProcessMessage(string message)
        {
            AnalysisRequest request = JsonConvert.DeserializeObject<AnalysisRequest>(message);

            // 1) Downloads the document
            await _AZURE_STORAGE.DownloadDocumentAsync(request.FileName);

            // 2) Analyzes the document
            float result = _NLP.offensive_analysis(request.FileName);

            // 3) Creates new document object
            var analysisResult = new
            {
                FileName = request.FileName,
                OffensivePercentage = result
            };

            // 4) Publishes the analysis result
            Dictionary<string, object> attributes = new Dictionary<string, object>();
            attributes.Add("type", 2);

            Publish(message: JsonConvert.SerializeObject(analysisResult),
                    routingKey: "analysis.results",
                    messageAttributes: attributes);

            Console.WriteLine(string.Format("File {0} have an offensive content percentage of {1}", request.FileName, result));
        }

        private void Publish(string message, string routingKey, IDictionary<string, object> messageAttributes, string timeToLive = "30000")
        {
            var body = Encoding.UTF8.GetBytes(message);
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.Headers = messageAttributes;
            properties.Expiration = timeToLive;

            _channel.BasicPublish(_PUB_EXCHANGE_NAME, routingKey, properties, body);
        }

    }
}
