using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WxaCode
{
    class WxaCode
    {
        public string appid;
        public string appsecret;
        private string accessToken;
        public string lasstErrorMsg;
        public WxaCode()
        {
        }

        public string refreshAccessToken()
        {
            accessToken = null;
            StringBuilder url = new StringBuilder(512);
            url.Append(@"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=");
            url.Append(appid);
            url.Append(@"&secret=");
            url.Append(appsecret);

            List<Byte> data = HandlerHttpsCallBack(url.ToString(), false, null);
            if(data!=null)
            {
                byte[] response = data.ToArray();
                string tmpresponse = System.Text.Encoding.ASCII.GetString(response);
                if (tmpresponse.Contains("access_token"))
                {
                    JObject jsondata = JsonConvert.DeserializeObject<JObject>(tmpresponse);
                    accessToken = jsondata.GetValue("access_token").ToString();
                    return accessToken;
                }
                else
                {
                    lasstErrorMsg = tmpresponse;
                }
            }
            else
            {
                lasstErrorMsg = "maybe there are net work error";
            }

            return null;
        }

        public bool IsNeedRefreshAccesstoken(string _appid, string _appsecret)
        {
            if(appid is null || appsecret is null)
            {
                appid = _appid;
                appsecret = _appsecret;
                return true;
            }
            if(_appid != appid || _appsecret != appsecret)
            {
                appid = _appid;
                appsecret = _appsecret;
                return true;
            }
            return false;
        }

        public byte[] GetWxaCodeUnlimited(string wxacodeparams,bool refreshAccesstoken)
        {
            if(refreshAccesstoken)
            {
                accessToken = refreshAccessToken();
                if(accessToken==null)
                {
                    // maybe network error or appid/appsecret error
                    return null;
                }
            }string url = @"https://api.weixin.qq.com/wxa/getwxacodeunlimit?access_token=";
            url += accessToken;
            List<byte> data = HandlerHttpsCallBack(url, true, wxacodeparams);
            if(data!=null)
            {
                byte[] response = data.ToArray();
                if(response.Length>1024)
                {
                    return response;
                }
                else
                {
                    lasstErrorMsg = System.Text.Encoding.ASCII.GetString(response);
                }
            }
            return null;
        }

        private List<byte> HandlerHttpsCallBack(string url,bool post,string wxacodeparams)
        {
            var data = new List<byte>();
            var request = (HttpWebRequest)WebRequest.Create(url);
            if (post)
            {
                request.ContentType = "application/json";
                request.Method = "POST";
                using(var writer = new System.IO.StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(wxacodeparams);
                    writer.Flush();
                    writer.Close();
                }
            }
            else
            {
                request.Method = "GET";
            }

            using (var response = request.GetResponse().GetResponseStream())
            {
                {
                    var buffer = new byte[1024];

                    long totalbytes = 0;
                    int readbytes = 0;
                    while ((readbytes = response.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        totalbytes += readbytes;
                        for (int i = 0; i < readbytes; i++)
                        {
                            data.Add(buffer[i]);
                        }
                    }
                }
                return data;
            }
        }
    }
}
