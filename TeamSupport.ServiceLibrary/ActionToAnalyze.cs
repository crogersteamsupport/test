﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Diagnostics;

namespace WatsonToneAnalyzer
{
    /// <summary>
    /// Linq class to integrate with ActionToAnalyze Table
    /// </summary>
    [Table(Name = "ActionToAnalyze")]
    class ActionToAnalyze
    {
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
        // note Int64 - different ID data type from other ID's
        private Int64 _actionToAnalyzeID;
        [Column(Storage="_actionToAnalyzeID", IsPrimaryKey = true, IsDbGenerated = true)]
        public Int64 ActionToAnalyzeID {  get { return _actionToAnalyzeID; } }

        [Column]
        public int ActionID;
        [Column]
        public int TicketID;
        [Column]
        public int UserID;
        [Column]
        public int OrganizationID;
        [Column]
        public bool IsAgent;
        [Column]
        public DateTime DateCreated;
        [Column]
        public string ActionDescription;
#pragma warning restore CS0649

        /// <summary>remove HTML, whitespace, email addresses...</summary>
        public string WatsonText() { return CleanString(ActionDescription); }

        public static string CleanString(string RawHtml)
        {
            String text = Regex.Replace(RawHtml, @"<[^>]*>", String.Empty); //remove html tags
            text = Regex.Replace(text, "&nbsp;", " "); //remove HTML space
            text = Regex.Replace(text, @"[\d-]", " "); //removes all digits [0-9]
            text = Regex.Replace(text, @"[\w\d]+\@[\w\d]+\.com", " "); //removes email adresses
            text = Regex.Replace(text, @"\s+", " ");   // remove whitespace
            if (text.Length > 500)
                text = text.Substring(0, 499);
            return text;
        }

        /// <summary> Delete ActionToAnalyze </summary>
        public void DeleteOnSubmit(DataContext db)
        {
            Table<ActionToAnalyze> table = db.GetTable<ActionToAnalyze>();
            try
            {
                // linq classes have an attach state to the DB table row
                if(table.GetOriginalEntityState(this) == null)
                    table.Attach(this); // must be attached to delete
            }
            catch (Exception e2)
            {
                WatsonEventLog.WriteEntry("Exception with table.Attach - ", e2);
                Console.WriteLine(e2.ToString());
            }

            table.DeleteOnSubmit(this);
        }

        // MQ message
        public static ActionToAnalyze Factory(string message)
        {
            return JsonConvert.DeserializeObject<ActionToAnalyze>(message);
        }

    }
}
