using System;
using Microsoft.AspNetCore.Mvc;
using Plain.RabbitMQ;
using RabbitMQ.Client;
using API_service.Models;
using Newtonsoft.Json;

namespace API_service.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AnalysisController : ControllerBase
    {
        private readonly IPublisher _publisher;

        public AnalysisController(IPublisher publisher)
        {
            _publisher = publisher;
        }

        [HttpPost]
        public IActionResult PostAnalysis([FromBody] AnalysisRequest analysisRequest)
        {
            string routingKey;
            switch (analysisRequest.Type)
            {
                case 0:
                    routingKey = "entity.analysis";
                    break;
                case 1:
                    routingKey = "sentiment.analysis";
                    break;
                case 2:
                    routingKey = "offensive.analysis";
                    break;
                default:
                    routingKey = "general.analysis";
                    break;
            }

            _publisher.Publish(message: JsonConvert.SerializeObject(analysisRequest),
                               routingKey: routingKey,
                               messageAttributes: null);

            return Ok();
        }

    }
}
