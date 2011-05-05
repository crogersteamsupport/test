using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TeamSupport.Data;
using TeamSupport.ServiceLibrary;
using Microsoft.Win32;
using System.Net.Mail;
using System.IO;

namespace TeamSupport.ServiceTestApplication
{
  public partial class Form1 : Form
  {
    private LoginUser _loginUser;
    private string _templatePath;
    EmailProcessor _emailProcessor2;

    EmailProcessor _emailProcessor;
    EmailSender _emailSender;
    SlaProcessor _slaProcessor;
    IisDbLogger _iisDbLogger;
    MurocUpdater _murocUpdater;


    public Form1()
    {
      InitializeComponent();
      System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.BelowNormal;

      _emailProcessor = new EmailProcessor();
      _emailSender = new EmailSender();
      _slaProcessor = new SlaProcessor();
      _iisDbLogger = new IisDbLogger();
      _murocUpdater = new MurocUpdater();

    }

    private string GetConnectionString()
    {
      return Utils.GetSettingString("ConnectionString", "Data Source=localhost;Initial Catalog=TeamSupport;Persist Security Info=True;User ID=sa;Password=muroc");
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
      Properties.Settings.Default.Save();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      _loginUser = new LoginUser(GetConnectionString(), -1, -1, null);
    }


    private void button12_Click(object sender, EventArgs e)
    {
      if (!timerEmails.Enabled)
      {
        timerEmails.Interval = 1000;
        _emailProcessor2 = new EmailProcessor();
      }

      timerEmails.Enabled = !timerEmails.Enabled;
      button12.Text = timerEmails.Enabled ? "Stop Emails" : "Start Emails";
    }
    private void timerEmails_Tick(object sender, EventArgs e)
    {
      if (_emailProcessor2 != null)
      {
        _emailProcessor2.Run();
      }

    }


    private void button1_Click(object sender, EventArgs e)
    {
      MurocUpdater updater = new MurocUpdater();
      updater.Run();
    }

    private void btnSLA_Click(object sender, EventArgs e)
    {
      if (Utils.GetSettingInt("SlaProcessEnabled") > 0 && Utils.GetSettingInt("SlaProcessInterval") > 0)
      {
        SlaProcessor processor = new SlaProcessor();
        processor.Run();
      }
    }


    private void btnOnStart_Click(object sender, EventArgs e)
    {
      _emailProcessor.Start("EmailEnabled", "EmailInterval", 10);
      _emailSender.Start("EmailEnabled", "EmailInterval", 10);
      _slaProcessor.Start("SlaProcessEnabled", "SlaProcessInterval", 300);
      _iisDbLogger.Start("IisLogEnabled", "IisLogInterval", 300);
      _murocUpdater.Start("MurocUpdateEnabled", "MurocUpdateInterval", 300);

      MessageBox.Show("Started");

    }

    private void btnOnStop_Click(object sender, EventArgs e)
    {
      _emailProcessor.Stop();
      _emailSender.Stop();
      _slaProcessor.Stop();
      _iisDbLogger.Stop();
      _murocUpdater.Stop();
      MessageBox.Show("Stopped");

    }

  }
}
