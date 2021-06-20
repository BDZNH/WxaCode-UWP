using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WxaCode
{
    class WxaCodeParams
    {
        public WxaCodeParams()
        {
            line_color = new LineColor();
        }
        public string scene { get; set; }
        public string page { get; set; }
        public int width { get; set; }
        public bool auto_color { get; set; }
        public LineColor line_color { get; set; }
        public bool is_hyaline { get; set; }

        public class LineColor
        {
            public int r { get; set; }
            public int g { get; set; }
            public int b { get; set; }
        }
    }
}
