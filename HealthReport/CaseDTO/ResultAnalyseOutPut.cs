using System;
using System.Collections.Generic;
using System.Text;

namespace HealthReport.CaseDTO
{
   public class ResultAnalyseOutPut
    {
        public Dictionary<string, int> TivaluePairs { get; set; }
        public Dictionary<string, int> XinvaluePairs { get; set; }
        public Dictionary<string, int> CaiGanvaluePairs { get; set; }
        public Dictionary<string, int> CaiNiaovaluePairs { get; set; }
        public Dictionary<string, int> XShevaluePairs { get; set; }
        public Dictionary<string, int> TingvaluePairs { get; set; }
        public Dictionary<string, int> FeivaluePairs { get; set; }
        public Dictionary<string, int> FangvaluePairs { get; set; }
    }
}
