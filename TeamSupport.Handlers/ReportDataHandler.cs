using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using System.Net;
using System.Web.SessionState;
using System.Drawing;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.IO;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.IO.Compression;
using ICSharpCode.SharpZipLib.Zip;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Security;

namespace TeamSupport.Handlers
{
  public class ReportDataHandler : IHttpHandler
  {
    public bool IsReusable
    {
      get { return false; }
    }
    

    public void ProcessRequest(HttpContext context)
    {
      context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      context.Response.AddHeader("Expires", "-1");
      context.Response.AddHeader("Pragma", "no-cache");

      using (Stream receiveStream = context.Request.InputStream)
      {
        using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
        {
          string requestContent = readStream.ReadToEnd();
          try
          {

            List<string> segments = new List<string>();
            bool flag = false;
            for (int i = 0; i < context.Request.Url.Segments.Length; i++)
            {
              string s = context.Request.Url.Segments[i].ToLower().Trim().Replace("/", "");
              if (flag) segments.Add(s);
              if (s == "reportdata") flag = true;
            }


            JObject args = JObject.Parse(requestContent);

            string content = "";
            switch (segments[0])
            {
              case "table":
                content = GetReportTableData(args);
                break;
              case "chart":
                content = GetReportChartData(args);
                break;
              default:
                context.Response.ContentType = "text/html";
                context.Response.Write("Undefined report data type");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.End();
                break;
            }
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.Write(content);

          }
          catch (Exception ex)
          {
            ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), ex, "ReportDataHander", requestContent);
            context.Response.ContentType = "text/html";
            context.Response.Write("There was an error getting your report data.");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
          }
        }
      }
    }

    private static string GetReportTableData(JObject args)
    {
      GridResult result = Reports.GetReportData(TSAuthentication.GetLoginUser(), (int)args["reportID"], (int)args["from"], (int)args["to"], (string)args["sortField"], (bool)args["isDesc"], (bool)args["useUserFilter"]);
      return JsonConvert.SerializeObject(result);
    }

    private static string GetReportChartData(JObject args)
    {
      LoginUser loginUser = TSAuthentication.GetLoginUser();
      Report report = Reports.GetReport(loginUser, (int)args["reportID"], TSAuthentication.UserID);
      Reports.UpdateReportView(loginUser, report.ReportID);
      SummaryReport summaryReport = JsonConvert.DeserializeObject<SummaryReport>(report.ReportDef);
      DataTable table = Reports.GetSummaryData(TSAuthentication.GetLoginUser(), summaryReport, true, report);
      return Reports.BuildChartData(loginUser, table, summaryReport);
    }

  }
}
