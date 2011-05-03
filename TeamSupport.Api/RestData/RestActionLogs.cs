using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Data;
using TeamSupport.Data;
using System.Net;

namespace TeamSupport.Api
{
  public class RestActionLogs
  {

    public static string GetItem(RestCommand command, ReferenceType refType, int refID, int id)
    {
      ActionLogsViewItem item = ActionLogsView.GetActionLogsViewItem(command.LoginUser, id);
      if (item.OrganizationID != command.LoginUser.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return item.GetXml("ActionItem", true);
    }


    public static string GetItems(RestCommand command, ReferenceType refType, int refID)
    {
      ActionLogsView items = new ActionLogsView(command.LoginUser);
      items.LoadByReference(refID, refType);
      return items.GetXml("History", "ActionItem", true, command.Filters);
    }

    public static string AddNote(RestCommand command, ReferenceType refType, int refID)
    {
      if (!DataUtils.IsReferenceValid(command.LoginUser, refType, refID)) throw new RestException(HttpStatusCode.Unauthorized);
      Notes items = new Notes(command.LoginUser);
      Note item = items.AddNewNote();
      item.ReadFromXml(command.Data, true);
      item.RefType = refType;
      item.RefID = refID;
      item.Collection.Save();
      return item.GetXml("Note", true);
    }

    public static string RemoveNote(RestCommand command, ReferenceType refType, int refID, int noteID)
    {
      Note item = Notes.GetNote(command.LoginUser, noteID);
      if (item.RefType != refType && item.RefID != refID) throw new RestException(HttpStatusCode.Unauthorized);
      if (!DataUtils.IsReferenceValid(command.LoginUser, refType, refID)) throw new RestException(HttpStatusCode.Unauthorized);
      item.Delete();
      item.Collection.Save();
      return "";
    }
  }
}
