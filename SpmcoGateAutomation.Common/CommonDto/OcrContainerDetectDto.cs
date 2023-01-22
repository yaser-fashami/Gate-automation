using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpmcoGateAutomation.Common.CommonDto
{
    public class OcrContainerDetectDto
    {
        public string CntrNo { get; set; }
        public string IsoCode { get; set; }
        public byte CntrSize { get; internal set; }
        public string CntrType { get; internal set; }
        public byte CheckSum { get; set; }
        public int Confidence { get; set; }
        public byte CalculatedCheckSum { get; set; }
        public bool IsValid { get; set; }
        public long ElapsedTime { get; set; }
    }
}
