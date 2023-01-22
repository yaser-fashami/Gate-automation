using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spmco.Common.Log
{
    public class MassTransitLog
    {
        public Guid TransactionId { get; set; }
        public string Sender { get; set; }
        public string LogData { get; set; }
        public string LogDate { get; set; }
        public string LogType { get; set; }
    }
}
