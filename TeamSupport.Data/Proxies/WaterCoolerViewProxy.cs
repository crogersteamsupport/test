using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(WaterCoolerViewItemProxy))]
  public class WaterCoolerViewItemProxy
  {
    public WaterCoolerViewItemProxy() {}
    [DataMember] public int MessageID { get; set; }
    [DataMember] public int UserID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string TimeStamp { get; set; }
    [DataMember] public string Message { get; set; }
    [DataMember] public int? MessageParent { get; set; }
    [DataMember] public bool IsDeleted { get; set; }
    [DataMember] public DateTime LastModified { get; set; }
    [DataMember] public string UserName { get; set; }     
  }
  
  public partial class WaterCoolerViewItem : BaseItem
  {
    public WaterCoolerViewItemProxy GetProxy()
    {
      WaterCoolerViewItemProxy result = new WaterCoolerViewItemProxy();
      result.IsDeleted = this.IsDeleted;
      result.MessageParent = this.MessageParent;
      result.Message = MakeLink(this.Message);
      result.OrganizationID = this.OrganizationID;
      result.UserID = this.UserID;
      result.MessageID = this.MessageID;
      result.UserName = Users.GetUserFullName(BaseCollection.LoginUser, this.UserID);
       
      result.TimeStamp = DateTime.SpecifyKind(this.TimeStampUtc, DateTimeKind.Utc).ToString("o");
      result.LastModified = DateTime.SpecifyKind(this.LastModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }

    public string MakeLink(string txt)
    {
        //Regex regx = new Regex("http://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?", RegexOptions.IgnoreCase);
        //(http://|)(www\.)?([^\.]+)\.(\w{2})$
        //(?<http>(http:[/][/]|www.)([a-z]|[A-Z]|[0-9]|[/.]|[~])*)
        Regex regx = new Regex(@"((([A-Za-z]{3,9}:(?:\/\/)?)(?:[-;:&=\+\$,\w]+@)?[A-Za-z0-9.-]+|(?:www.|[-;:&=\+\$,\w]+@)[A-Za-z0-9.-]+)((?:\/[\+~%\/.\w-_]*)?\??(?:[-\+=&;%@.\w_]*)#?(?:[.\!\/\\w]*))?)", RegexOptions.IgnoreCase);
        MatchCollection mactches = regx.Matches(txt);

        foreach (Match match in mactches)
        {
            txt = txt.Replace(match.Value, "<a target='_blank' class='ts-link ui-state-default' href='" + (match.Value.StartsWith("http://") ? match.Value : "http://" + match.Value) + "'>" + match.Value + "</a>");
        }

        return GenerateTicketLink(txt);
    }

    public string GenerateTicketLink(string txt)
    {
        Regex regx = new Regex(@"&ticket\d+", RegexOptions.IgnoreCase);
        MatchCollection mactches = regx.Matches(txt);
        int ticketnum;
        Tickets tix = new Tickets(BaseCollection.LoginUser);
        
        foreach (Match match in mactches)
        {
            ticketnum = Convert.ToInt32(Regex.Replace(match.Value, "[^0-9]+", string.Empty));
            tix.LoadByTicketNumber(BaseCollection.LoginUser.OrganizationID, ticketnum);
            if (!tix.IsEmpty)
            {
                if (tix[0].OrganizationID == BaseCollection.LoginUser.OrganizationID)
                {
                    txt = txt.Replace(match.Value, "<a class='ts-link ui-state-default' href='#' onclick='top.Ts.MainPage.openTicket(" + ticketnum + "); return false;'>ticket " + ticketnum + "</a>");
                }
            }
        }

        return txt;
    }
  }
}
