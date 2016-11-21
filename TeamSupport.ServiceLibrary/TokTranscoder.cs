using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamSupport.Data;
using Microsoft.Win32;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.ComponentModel;



namespace TeamSupport.ServiceLibrary
{
    [Serializable]
    public class TokTranscoder : ServiceThreadPoolProcess
    {
        private static object _staticLock = new object();

        public override void ReleaseAllLocks()
        {
            //You need to lock this, if you plan on multiple threads
        }

        public override void Run()
        {
            Logs.WriteEvent("Starting Run");

        }


    }
}
