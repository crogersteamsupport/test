using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TeamSupport.DataManager
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      if (LoginForm.Login())
       Application.Run(new MainForm());
    }
  }
}
