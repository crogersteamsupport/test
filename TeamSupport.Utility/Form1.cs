using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace TeamSupport.Utility
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e)
    {
      string root = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "data");
      string outName = Path.Combine(root, "out.log");
      File.Delete(outName);
      using (StreamWriter writer = File.CreateText(outName))
      {
        List<string> fileNames = new List<string>(Directory.EnumerateFiles(root, "*.txt"));
        foreach (string fileName in fileNames)
        {
          using (StreamReader reader = new StreamReader(fileName))
          { 
            string line;
            int id = 0;
            while ((line = reader.ReadLine()) != null)
            {
              int i = line.IndexOf("EmailPostID: \"");
              if (i > -1)
              {
                
                id = int.Parse(line.Substring(i + 14, line.IndexOf("\"", i + 14) - i - 14));
              }
              else if (line.IndexOf("Invalid column name 'PortalLimitOrgTickets'.") > -1)
              {
                writer.WriteLine(id.ToString()+",");
              }
            }
          
          }
        }

      }

      MessageBox.Show("done");
    }
  }
}
