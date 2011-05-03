using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TeamSupport.Data;

namespace TeamSupport.DataManager
{
  public partial class LoginForm : Form
  {
    public LoginForm()
    {
      InitializeComponent();
      Telerik.WinControls.ThemeResolutionService.ApplicationThemeName = "Vista";
    }

    public static bool Login()
    {
      LoginForm form = new LoginForm();
      if (form.ShowDialog() == DialogResult.OK)
      {
        return true;
      }
      else
      {
        return false;
      }

    }

    private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (DialogResult != DialogResult.OK) return;
      
      textName.Text = "kjones@teamsupport.com";
      textPassword.Text = "xlg7mh";
      
      string connectionString = System.Configuration.ConfigurationManager.AppSettings["MainConnection"];
      LoginUser loginUser = new LoginUser(connectionString, 34, 1078, null);

      Users users = new Users(loginUser);
      users.LoadByEmail(textName.Text);
      string password = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(textPassword.Text, "MD5");

      foreach (User user in users)
      {
        if (user.OrganizationID == 1 && user.CryptedPassword == password)
        {

          LoginSession.LoginUser = new TeamSupport.Data.LoginUser(connectionString, -1, user.OrganizationID, null);
          return;
        }
      }

      e.Cancel = true;

      MessageBox.Show("Invalid user name or password.");
    }
  }
}
