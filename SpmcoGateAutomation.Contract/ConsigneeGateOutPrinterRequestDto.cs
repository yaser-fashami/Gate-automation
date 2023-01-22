using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpmcoGateAutomation.Contract
{
    public class ConsigneeGateOutPrinterRequestDto
    {
        public string TruckNo { get; set; }
        public string CntrNo { get; set; }
        public string SizeType { get; set; }
        public string DeliveryPermissionNo { get; set; }
        public string Code { get; set; }
        public string GateInTime { get; set; }
        public string GateOutTime { get; set; }
        public string ExitPermitNo { get; set; }
        public string ExitPermitDate { get; set; }
        public string ReferenceNo { get; set; }
        public string P1 { get { return TruckNo.Substring(0, 2); } }
        public string P2 { get { return TruckNo.Substring(2, 1); } }
        public string P3 { get { return TruckNo.Substring(3, 3); } }
        public string P4 { get { return TruckNo.Substring(6, 2); } }
    }
}
