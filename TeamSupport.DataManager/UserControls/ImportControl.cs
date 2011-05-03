using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using TeamSupport.Data;
using System.Data.OleDb;

namespace TeamSupport.DataManager.UserControls
{
  public partial class ImportControl : BaseOrganizationUserControl
  {
    private DataSet _dataSet;
    private Importer _importer;
    
    public ImportControl()
    {
      InitializeComponent();
    }
    
    protected override void LoadOrganization(Organization organization)
    {
    }

    private void btnOpen_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.FileName = Properties.Settings.Default.SpreadSheetSourceFileName;
      dialog.Filter = "Excel Files (*.xls, *.xlsx)|*.xls;*.xlsx|All Files (*.*)|*.*";
      if (dialog.ShowDialog() == DialogResult.OK)
      {
        lblFileName.Text = dialog.FileName;
        Properties.Settings.Default.SpreadSheetSourceFileName = dialog.FileName;
        Properties.Settings.Default.Save();
        btnImport.Enabled = true;

        try
        {
          Cursor.Current = Cursors.WaitCursor;
          _importer = new Importer(new LoginUser(LoginSession.LoginUser.ConnectionString, 34, 1078, null), dialog.FileName);
        }
        catch (Exception ex)
        {
          MessageBox.Show("There was an error opening the file.\n\n" + ex.Message);
          return;
        }
        finally
        {
          Cursor.Current = Cursors.Default;
        }

      }
    }   
    
    private void btnImport_Click(object sender, EventArgs e)
    {
      if (_importer == null) return;
      try
      {
        Cursor.Current = Cursors.WaitCursor;
        
        if (cbNewOrg.Checked)
          MessageBox.Show(_importer.Import(textOrganizationName.Text));
        else
          MessageBox.Show(_importer.Import(OrganizationID));

      }
      finally
      {
        Cursor.Current = Cursors.Default;
      }
      
    }

    private void cbNewOrg_CheckedChanged(object sender, EventArgs e)
    {
      textOrganizationName.Enabled = cbNewOrg.Checked;
    }
  }
}
