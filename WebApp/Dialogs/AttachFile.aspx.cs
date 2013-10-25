using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using TeamSupport.WebUtils;
using TeamSupport.Data;
using Telerik.Web.UI;
using System.IO;

public partial class Dialogs_AttachFile : BaseDialogPage
{
  private int _refID = -1;
  private ReferenceType _refType;

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);

    _manager.AjaxSettings.Clear();

    _refID = int.Parse(Request["RefID"]);
    _refType = (ReferenceType)int.Parse(Request["RefType"]);

  }

  public override bool Save()
  {
    int used = Organizations.GetStorageUsed(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);
    int allowed = Organizations.GetTotalStorageAllowed(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);
    /*
    if (used > allowed)
    {
      if (UserSession.CurrentUser.IsFinanceAdmin)
      {
        _manager.Alert("You have exceeded your allocated storage capacity.  If you would like to add additional storage, please contact our sales team at 800.596.2820 x806, or send an email to sales@teamsupport.com");
      }
      else
      {
        Users users = new Users(UserSession.LoginUser);
        users.LoadFinanceAdmins(UserSession.LoginUser.OrganizationID);
        if (users.IsEmpty)
        {
          _manager.Alert("Please ask your billing administrator to purchase additional storage to add additional attachments.");
        }
        else
        {
          _manager.Alert("Please ask your billing administrator (" + users[0].FirstLastName + ") to purchase additional storage to add additional attachments.");
        }
      }


      return false;
    }
    */

    Attachments attachments = new Attachments(UserSession.LoginUser);

    string folderName = "Unknown";

    switch (_refType)
    {
      case ReferenceType.Organizations:
        folderName = "OrganizationAttachments";
        break;
      default:
        folderName = "Unknown";
        break;
    }

    foreach (UploadedFile file in ulFile.UploadedFiles)
    {
      string directory = TSUtils.GetAttachmentPath(folderName, _refID);
      string fileName = file.GetName();

      fileName = DataUtils.VerifyUniqueFileName(directory, fileName);

      Attachment attachment = attachments.AddNewAttachment();
      attachment.RefType = _refType;
      attachment.RefID = _refID;
      attachment.OrganizationID = UserSession.LoginUser.OrganizationID;
      attachment.FileName = fileName;
      attachment.Path = Path.Combine(directory, fileName);
      attachment.FileType = string.IsNullOrEmpty(file.ContentType) ? "application/octet-stream" : file.ContentType;
      attachment.FileSize = file.ContentLength;
      attachment.Description = textDescription.Text;

      Directory.CreateDirectory(directory);
      file.SaveAs(attachment.Path, true);
      attachments.Save();
    }

    return true;
  }

}
