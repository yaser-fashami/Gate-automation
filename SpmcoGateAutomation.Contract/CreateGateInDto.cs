using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpmcoGateAutomation.Contract
{
    public class CreateGateInDto
    {
        public long DeliveryPermissionCntrId { get; set; }
        public string PlateNo { get; set; }
        public string CntrNo { get; set; }
        public string ImageName { get; set; }
        public string GateInId { get; set; }
        public byte LogType { get; set; }
        public string GateInNo { get; set; }
    }
}
