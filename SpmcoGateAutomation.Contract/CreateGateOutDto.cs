using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpmcoGateAutomation.Contract
{
    public class CreateGateOutDto
    {
        public string PlateNo { get; set; }
        public List<string> CntrList { get; set; }
        public byte LogType { get; set; }
        public string GateOutNo { get; set; }
    }
}
