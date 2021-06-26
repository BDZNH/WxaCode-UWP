using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WxaCode
{
    class WxaCodeParams
    {
        private static WxaCodeParams instance;
        private WxaCodeParams()
        {
            
            scene = (string)LocalSettings.Get("scene", "");
            page = (string)LocalSettings.Get("page", "");
            width = (int)LocalSettings.Get("width", 430);
            line_color = new LineColor();
            auto_color = (bool)LocalSettings.Get("auto_color", false);
            line_color.r = (int)LocalSettings.Get("red", 0);
            line_color.g = (int)LocalSettings.Get("green", 0);
            line_color.b = (int)LocalSettings.Get("blue", 0);
            is_hyaline = (bool)LocalSettings.Get("is_hyaline", false);
        }

        public static WxaCodeParams GetInstance()
        {
            if(instance == null)
            {
                instance = new WxaCodeParams();
            }
            return instance;
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

        public void Save()
        {
            if(LocalSettings.IfSaveWxacodeParam())
            {
                LocalSettings.Set("scene", scene);
                LocalSettings.Set("page", page);
                LocalSettings.Set("width", width);
                LocalSettings.Set("auto_color", auto_color);
                LocalSettings.Set("red", line_color.r);
                LocalSettings.Set("green", line_color.g);
                LocalSettings.Set("blue", line_color.b);
                LocalSettings.Set("is_hyaline", is_hyaline);
            }
            else
            {
                LocalSettings.Set("scene", "");
                LocalSettings.Set("page", "");
                LocalSettings.Set("width", 430);
                LocalSettings.Set("auto_color", false);
                LocalSettings.Set("red", 0);
                LocalSettings.Set("green", 0);
                LocalSettings.Set("blue", 0);
                LocalSettings.Set("is_hyaline", false);
            }
        }
    }
}
