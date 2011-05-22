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
    EmailProcessor _emailProcessor;
    EmailSender _emailSender;
    SlaProcessor _slaProcessor;
    Indexer _indexer;

    public Form1()
    {
      InitializeComponent();
      System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.BelowNormal;

      _emailProcessor = new EmailProcessor();
      _emailSender = new EmailSender();
      _slaProcessor = new SlaProcessor();
      _indexer = new Indexer();

    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
      Properties.Settings.Default.Save();
      Stop();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
    }



    private void btnOnStart_Click(object sender, EventArgs e)
    {

      if (btnOnStart.Text == "Start")
      {
        Start();
      }
      else
      {
        Stop();
      }

    }

    private void Start()
    {
      btnOnStart.Text = "Stop";
      btnOnStart.ForeColor = Color.Red;
      try
      {
        btnOnStart.Enabled = false;
        //_emailProcessor.Start("EmailEnabled", "EmailInterval", 10);
        //_emailSender.Start("EmailEnabled", "EmailInterval", 10);
        //_slaProcessor.Start("SlaProcessEnabled", "SlaProcessInterval", 300);
        _indexer.Start("IndexerEnabled", "IndexerInterval", 60);
      }
      finally
      {

        btnOnStart.Enabled = true;
      }
    }

    private void Stop()
    {
      try
      {
        btnOnStart.Enabled = false;
        _emailProcessor.Stop();
        _emailSender.Stop();
        _slaProcessor.Stop();
        _indexer.Stop();
      }
      finally
      {
        btnOnStart.Enabled = true;
        btnOnStart.Text = "Start";
        btnOnStart.ForeColor = Color.Green;
      }
    }


  }
}
