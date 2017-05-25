using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace TeamSupport.ServiceLibrary
{
    public class TFS
    {
        private static string _hostname;
        private static string _accessToken;
        public TFS()
        {
        }

        public TFS(string hostname, string accessToken)
        {
            _hostname = hostname;
            _accessToken = accessToken;
        }
        public string GetProjects()
        {
            string responseBody = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", AccessToken))));

                    using (HttpResponseMessage response = client.GetAsync(string.Format("{0}/DefaultCollection/_apis/projects?api-version=1.0", HostName)).Result)
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

        public string GetWorkItemsBy(List<int> workItemIds, DateTime? lastLink)
        {
            var result = "";
            string credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", AccessToken)));
            string idsCommaSeparated = string.Join(",", workItemIds);
            string dateOnly = "1900-01-01";
            
            if (lastLink.HasValue)
            {
                //vv We substract one day because of the GreaterThan '>' logic in the query below. wiql does not allow time precision for dates. (it is possible with other library which we are not using now)
                dateOnly = lastLink.Value.AddDays(-1).ToString("yyyy-MM-dd");
            }

            //create wiql object
            var wiql = new
            {
                query = "Select [id] " +
                        "From WorkItems " +
                        "Where " +
                        "[System.ChangedDate] > '" + dateOnly + "' " +
                        "And [id] IN (" + idsCommaSeparated + ") " +
                        "Order By [Changed Date] Desc"
            };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(HostName);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                //serialize the wiql object into a json string. MediaType needs to be application/json for a post call
                var postValue = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(wiql), Encoding.UTF8, "application/json");
                var method = new HttpMethod("POST");

                //ToDo //vv The example I found for this uses the api version 2.2, the other examples uses 1.0. Check which one should we use and where to check the api versions.
                var httpRequestMessage = new HttpRequestMessage(method, string.Format("{0}/_apis/wit/wiql?api-version=2.2", HostName)) { Content = postValue };
                var httpResponseMessage = client.SendAsync(httpRequestMessage).Result;

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    WorkItemQueryResult workItemQueryResult = httpResponseMessage.Content.ReadAsAsync<WorkItemQueryResult>().Result;

                    //now that we have a bunch of work items, build a list of id's so we can get details
                    var builder = new StringBuilder();

                    foreach (var item in workItemQueryResult.WorkItems)
                    {
                        builder.Append(item.Id.ToString()).Append(",");
                    }

                    string ids = builder.ToString().TrimEnd(new char[] { ',' });

                    //vv we could just bring the specific fields if needed, I don't see this happening now.
                    //string fieldsCommaSeparated = string.Join(",", fields);
                    //string queryString = string.Format("_apis/wit/workitems?ids={0}&fields={1}&asOf={2}&api-version=2.2", ids, fieldsCommaSeparated, workItemQueryResult.AsOf);

                    //ToDo //vv The 'asOf' should be the last processed timestamp
                    //ToDo //vv we probably should have a 'max' of ids here because there is a limit in the query string size. We'll need to loop if needed. https://stackoverflow.com/questions/812925/what-is-the-maximum-possible-length-of-a-query-string

                    string queryString = string.Format("_apis/wit/workitems?ids={0}&asOf={1}&api-version=2.2", ids, workItemQueryResult.AsOf);
                    HttpResponseMessage getWorkItemsHttpResponse = client.GetAsync(queryString).Result;

                    if (getWorkItemsHttpResponse.IsSuccessStatusCode)
                    {
                        result = getWorkItemsHttpResponse.Content.ReadAsStringAsync().Result;
                    }
                    else
                    {
                        //ToDo //vv we have to return the error somehow..
                    }
                }
                else
                {
                    //ToDo //vv we have to return the error somehow..
                }
            }

            return result;
        }

        public void GetWorkItemsFields()
        {

        }

        #region Properties
        public string HostName
        {
            get
            {
                return _hostname;
            }
            set
            {
                _hostname = value;
            }
        }

        public string AccessToken
        {
            get
            {
                return _accessToken;
            }
            set
            {
                _accessToken = value;
            }
        }
        #endregion
    }
}
