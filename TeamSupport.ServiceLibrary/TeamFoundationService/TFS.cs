using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace TeamSupport.ServiceLibrary
{
    public class TFS
    {
        public static string GetProjects(string hostname, string accessToken)
        {
            string responseBody = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", accessToken))));

                    using (HttpResponseMessage response = client.GetAsync(string.Format("{0}/DefaultCollection/_apis/projects?api-version=1.0", hostname)).Result) //"https://vavc.visualstudio.com/DefaultCollection/_apis/projects?api-version=1.0").Result)
                    {
                        response.EnsureSuccessStatusCode();

                        if (response.StatusCode.ToString().ToLower() == "ok")
                        {
                            responseBody = response.Content.ReadAsStringAsync().Result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //vv
            }

            return responseBody;
        }
    }
}
