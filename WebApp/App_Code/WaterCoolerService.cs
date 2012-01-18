using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using System.Collections;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Text;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Runtime.Serialization;

namespace TSWebServices
{
  [ScriptService]
  [WebService(Namespace = "http://teamsupport.com/")]
  [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
  public class WaterCoolerService : System.Web.Services.WebService
  {

    public WaterCoolerService() { }

    /// <summary>
    /// Gets the top 25 wc messages plus their replies
    /// </summary>
    /// <returns></returns>

    [WebMethod]
    public WaterCoolerThread[] GetThreads()
    {
      List<WaterCoolerThread> result = new List<WaterCoolerThread>();
      //This reads the WaterCoolerView because it has UserNames and Groups.  This is readonly
      WaterCoolerView wc = new WaterCoolerView(TSAuthentication.GetLoginUser());
      wc.LoadTop25Threads();


      foreach (WaterCoolerViewItem item in wc)
      {
        WaterCoolerThread thread = new WaterCoolerThread();
        thread.Message = item.GetProxy();

        WaterCoolerView replies = new WaterCoolerView(wc.LoginUser);
        replies.LoadReplies(item.MessageID);

        thread.Replies = replies.GetWaterCoolerViewItemProxies();
        result.Add(thread);
      }

      return result.ToArray();
    }


    /// <summary>
    /// Saves a message to the watercooler DB from javascript
    /// </summary>
    /// <param name="messageID"></param>
    /// <param name="message"></param>
    /// <returns></returns>

    [WebMethod]
    public WaterCoolerViewItemProxy AddMessage(int? messageID, string message)
    {
      // DON'T user the view to save to the DB.  User the WaterCooler table.  The views are read only
      WaterCoolerItem wc = (new WaterCooler(TSAuthentication.GetLoginUser())).AddNewWaterCoolerItem();

      wc.GroupFor = null;
      wc.Message = message;
      wc.MessageType = messageID == null ? "Comment" : "Reply";
      wc.OrganizationID = TSAuthentication.OrganizationID;
      wc.ReplyTo = messageID;
      wc.TimeStamp = DateTime.UtcNow;
      wc.UserID = TSAuthentication.UserID;
      wc.Collection.Save();

      return WaterCoolerView.GetWaterCoolerViewItem(wc.Collection.LoginUser, wc.MessageID).GetProxy();
    }

    /// <summary>
    /// Deletes a WC message or reply
    /// </summary>
    /// <param name="messageID"></param>
    /// <returns></returns>
    [WebMethod]
    public bool DeleteMessage(int messageID)
    {
      // DON'T user the view to save to the DB.  User the WaterCooler table.  The views are read only
      WaterCoolerItem wc = WaterCooler.GetWaterCoolerItem(TSAuthentication.GetLoginUser(), messageID);
      if (wc.OrganizationID != TSAuthentication.OrganizationID) return false;
      if (wc.UserID != TSAuthentication.UserID && !TSAuthentication.IsSystemAdmin) return false;
      wc.Delete();
      wc.Collection.Save();
      return true;
    }
  }



  [DataContract]
  public class WaterCoolerThread
  {
    [DataMember] public WaterCoolerViewItemProxy Message { get; set; }
    [DataMember] public WaterCoolerViewItemProxy[] Replies { get; set; }
  }




}