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
  public class UserService : System.Web.Services.WebService
  {

    public UserService() { }

    [WebMethod]
    public UserProxy UpdateUserStatusComment(string comment)
    {
      LoginUser loginUser = TSAuthentication.GetLoginUser();
      User user = TSAuthentication.GetUser(loginUser);
      user.InOfficeComment = comment;
      user.Collection.Save();


      WaterCooler watercooler = new WaterCooler(loginUser);
      WaterCoolerItem item = watercooler.AddNewWaterCoolerItem();
      item.Message = string.Format("<strong>{0} - </strong>{1}", user.FirstLastName, user.InOfficeComment);
      item.OrganizationID = user.OrganizationID;
      item.TimeStamp = DateTime.UtcNow;
      item.UserID = user.UserID;
      watercooler.Save();

      return user.GetProxy();
    }
    
    [WebMethod]
    public UserProxy ToggleUserStatus()
    {
      LoginUser loginUser = TSAuthentication.GetLoginUser();
      User user = TSAuthentication.GetUser(loginUser);
      user.InOffice = !user.InOffice;
      user.Collection.Save();

      WaterCooler watercooler = new WaterCooler(loginUser);
      WaterCoolerItem item = watercooler.AddNewWaterCoolerItem();
      item.Message = string.Format("<strong>{0}</strong> {1}", user.FirstLastName, user.InOffice ? "is now in the office." : "has left the office.");
      item.OrganizationID = user.OrganizationID;
      item.TimeStamp = DateTime.UtcNow;
      item.UserID = user.UserID;
      //watercooler.Save();
      return user.GetProxy();
    }

    [WebMethod]
    public ChatUserSettingProxy ToggleUserChatStatus()
    {
      ChatUserSetting setting = ChatUserSettings.GetChatUserSetting(TSAuthentication.GetLoginUser(), TSAuthentication.UserID);
      setting.IsAvailable = !setting.IsAvailable;
      setting.Collection.Save();
      return setting.GetProxy();
    }

    [WebMethod]
    public BasicUser[] GetUsers()
    {
      Users users = new Users(TSAuthentication.GetLoginUser());
      users.LoadByOrganizationID(TSAuthentication.OrganizationID, false);
      List<BasicUser> result = new List<BasicUser>();

      foreach (User user in users)
      {
        BasicUser basic = new BasicUser();
        basic.Name = user.FirstName + " " + user.LastName;
        basic.UserID = user.UserID;
        result.Add(basic);
      }
      return result.ToArray();
    }

    [WebMethod]
    public GroupProxy[] GetGroups()
    {
      Groups groups = new Groups(TSAuthentication.GetLoginUser());
      groups.LoadByOrganizationID(TSAuthentication.OrganizationID);
      return groups.GetGroupProxies();
    }

    [WebMethod]
    public AutocompleteItem[] SearchUsers(string searchTerm)
    {
      UsersView users = new UsersView(TSAuthentication.GetLoginUser());
      users.LoadByTerm(TSAuthentication.OrganizationID, searchTerm, 15);

      List<AutocompleteItem> list = new List<AutocompleteItem>();
      foreach (UsersViewItem user in users)
      {
        list.Add(new AutocompleteItem(user.FirstName + " " + user.LastName, user.UserID.ToString()));
      }

      return list.ToArray();
    }

    [DataContract]
    public class BasicUser
    {
      [DataMember] public string Name { get; set; }
      [DataMember] public int UserID { get; set; }
    }


  }
}