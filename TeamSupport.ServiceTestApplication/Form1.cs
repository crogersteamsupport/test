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
using TeamSupport.CrmIntegration;
using Microsoft.Win32;
using System.Net.Mail;
using System.IO;

namespace TeamSupport.ServiceTestApplication
{
  public partial class Form1 : Form
  {
    ServiceThreadPool<EmailProcessor> _emailProcessor;
    ServiceThreadPool<EmailSender> _emailSender;
    SlaProcessor _slaProcessor;
    ServiceThreadPool<Indexer> _indexer;
    CrmPool _crmPool;
    ReminderProcessor _reminderProcessor;
    ImportProcessor _importProcessor;
    WebHooks _webhooks;
    
    public Form1()
    {
      InitializeComponent();
      System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.BelowNormal;
      /*
      Settings settings = new Settings(ServiceThread.GetLoginUser("Service Test App"), "EmailSender");
      settings.WriteBool("Debug", true);
      settings = new Settings(ServiceThread.GetLoginUser("Service Test App"), "EmailProcessor");
      settings.WriteBool("Debug", true);
      */
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
      Properties.Settings.Default.Save();
      StopAll();
    }

    private void StopAll()
    {
      ServiceThread.ServiceStopped = true;
      if (_emailProcessor != null) _emailProcessor.Stop();
      if (_emailSender != null) _emailSender.Stop();
      if (_slaProcessor != null) _slaProcessor.Stop();
      if (_indexer != null) _indexer.Stop();
      if (_crmPool != null) _crmPool.Stop();
      if (_reminderProcessor != null) _reminderProcessor.Stop();
      if (_importProcessor != null) _importProcessor.Stop();
      if (_webhooks != null) _webhooks.Stop();
    }

    private void StartProcess(ServiceThread thread, Button button)
    {
      button.Text = button.Text.Replace("Start ", "Stop ");
      button.ForeColor = Color.Red;
      try
      {
        button.Enabled = false;
        thread.Start();
      }
      finally
      {
        button.Enabled = true;
      }
    
    }

    private void StopProcess(ServiceThread thread, Button button)
    {
      try
      {
        button.Enabled = false;
        thread.Stop();
        thread = null;
      }
      finally
      {
        button.Enabled = true;
        button.Text = button.Text.Replace("Stop ", "Start ");
        button.ForeColor = Color.Green;
      }
    }

    private void btnEmailProcessor_Click(object sender, EventArgs e)
    {
      if (_emailProcessor == null || _emailProcessor.IsStopped) StartProcess(_emailProcessor = new ServiceThreadPool<EmailProcessor>("EmailProcessor"), sender as Button); else StopProcess(_emailProcessor, sender as Button);
    }

    private void btnEmailSender_Click(object sender, EventArgs e)
    {
      if (_emailSender == null || _emailSender.IsStopped) StartProcess(_emailSender = new ServiceThreadPool<EmailSender>("EmailSender"), sender as Button); else StopProcess(_emailSender, sender as Button);
    }

    private void btnSlaProcessor_Click(object sender, EventArgs e)
    {
      if (_slaProcessor == null || _slaProcessor.IsStopped) StartProcess(_slaProcessor = new SlaProcessor(), sender as Button); else StopProcess(_slaProcessor, sender as Button);
    }

    private void btnIndexer_Click(object sender, EventArgs e)
    {
      if (_indexer == null || _indexer.IsStopped) StartProcess(_indexer = new ServiceThreadPool<Indexer>("Indexer"), sender as Button); else StopProcess(_indexer, sender as Button);
    }

    private void btnCrmPool_Click(object sender, EventArgs e)
    {
      if (_crmPool == null || _crmPool.IsStopped) StartProcess(_crmPool = new CrmPool(), sender as Button); else StopProcess(_crmPool, sender as Button);
    }

    private void btnReminders_Click(object sender, EventArgs e)
    {
      if (_reminderProcessor == null || _reminderProcessor.IsStopped) StartProcess(_reminderProcessor = new ReminderProcessor(), sender as Button); else StopProcess(_reminderProcessor, sender as Button);

    }

    private void button1_Click(object sender, EventArgs e)
    {
      if (_webhooks == null || _webhooks.IsStopped) StartProcess(_webhooks = new WebHooks(), sender as Button); else StopProcess(_webhooks, sender as Button);
    }

    private void button2_Click(object sender, EventArgs e)
    {
      if (_importProcessor == null || _importProcessor.IsStopped) StartProcess(_importProcessor = new ImportProcessor(), sender as Button); else StopProcess(_importProcessor, sender as Button);
    }

    private void btnTestEmail_Click(object sender, EventArgs e)
    {
      try
      {
        using (SmtpClient client = new SmtpClient("smtp.socketlabs.com", 25))
        {
          client.Credentials = new System.Net.NetworkCredential("MurocSystems", "k3C5Wtb8ZYs7");
          client.Timeout = 20000;
          client.EnableSsl = true; //does not work on socket labs
          MailMessage message = new MailMessage("kjones@teamsupport.com", "kjones@teamsupport.com", "SMTP TEST", "THIS IS JUST A TEST");
          //message.Headers.Add("X-xsMessageId", email.OrganizationID.ToString());
          //message.Headers.Add("X-xsMailingId", email.EmailID.ToString());
          client.Send(message);
          MessageBox.Show("Message Sent");
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("There was an error sending mail\n\n" + ex.Message);
      }
    }


  }
}
