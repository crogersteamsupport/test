using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;



namespace TeamSupport.Handlers.Routes
{
    public class BaseRoute
    {
        protected static void WriteJson(HttpContext context, object payload)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.AddHeader("Expires", "-1");
            context.Response.AddHeader("Pragma", "no-cache");
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.Write(JsonConvert.SerializeObject(payload));
        }

        protected static JObject ReadData(HttpContext context)
        {
            string result;
            using (Stream receiveStream = context.Request.InputStream)
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    result = readStream.ReadToEnd();
                }
            }
            return JObject.Parse(result);
        }

        public static string[] GetSegments(HttpContext context)
        {
            List<string> segments = new List<string>();
            bool routeFlag = false;
            for (int i = 0; i < context.Request.Url.Segments.Length; i++)
            {
                if (context.Request.Url.Segments[i].TrimEnd('/') == "rt")
                {
                    routeFlag = true;
                }
                else if (routeFlag)
                {
                    segments.Add(context.Request.Url.Segments[i].TrimEnd('/'));
                }
            }
            return segments.ToArray();
        }
    }
}
