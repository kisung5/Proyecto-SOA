using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment_service.Models
{
    class AnalysisRequest
    {
        public int Type { get; set; }
        public string FileName { get; set; }
    }
}
