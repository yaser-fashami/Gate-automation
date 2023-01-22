using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpmcoGateAutomation.Contract
{
    public class ConsigneeGateInPrinterRequestDto
    {
        public string yardOperatorName { get; set; }
        public string equipmentName { get; set; }
        public string TruckNo { get; set; }
        public string CntrNo { get; set; }
        public string GateNo { get; set; }
        public string GateOperator { get; set; }
        public string CntrLocation { get; set; }
        public string SizeType { get; set; }
        public string DeliveryPermissionNo { get; set; }
        public string GateDate { get; set; }
        public string Code { get; set; }
        public string GateLogNo { get; set; }
        public string P1 { get { return TruckNo.Substring(0, 2); } }
        public string P2 { get { return TruckNo.Substring(2, 1); } }
        public string P3 { get { return TruckNo.Substring(3, 3); } }
        public string P4 { get { return TruckNo.Substring(6, 2); } }
    }
}
