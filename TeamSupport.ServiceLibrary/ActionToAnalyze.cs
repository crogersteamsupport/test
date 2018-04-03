using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace WatsonToneAnalyzer
{
    /// <summary>
    /// Linq class to integrate with ActionToAnalyze Table
    /// </summary>
    [Table(Name = "ActionToAnalyze")]
    class ActionToAnalyze
    {
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

        /// <summary>remove HTML, whitespace, email addresses...</summary>
        public string WatsonText() { return CleanStringV3(ActionDescription); }

        /// <summary> see ts-app\TeamSupport.Data\BusinessObjects\ActionToAnalyze.cs CleanStringV2 </summary>
        public static string CleanStringV3(string RawHtml)
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

        public static String CleanStringV2(String RawHtml)
        {
            String Html = Regex.Replace(RawHtml, @"<[^>]*>", String.Empty); //removes html tags
            Html = Regex.Replace(Html, "nbsp;", " "); //removes strange nbsp tags
            Html = Regex.Replace(Html, @"[\d-]", " "); //removes all digits
            Html = Regex.Replace(Html, @"[\w\d]+\@[\w\d]+\.com", " "); //removes email adresses
            StringBuilder sb = new StringBuilder();
            char previous = ' ';
            foreach (char c in Html)  //goes through every charecter in the string and only passes on valid charecters to the new string
            {
                if ((c== '!') || (c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' && previous != '.' || c == ' ' && previous != ' ' || c == '?')
                {
                    sb.Append(c);
                }
                previous = c;
            }

            string text = sb.ToString();
            if (text.Length > 500)
                text = text.Substring(0, 499);
            return text;
        }

        /// <summary> Delete ActionToAnalyze </summary>
        public void DeleteOnSubmit(DataContext db)
        {
            Table<ActionToAnalyze> table = db.GetTable<ActionToAnalyze>();
            table.Attach(this);
            table.DeleteOnSubmit(this);
        }

        // MQ message
        public static ActionToAnalyze Factory(string message)
        {
            return JsonConvert.DeserializeObject<ActionToAnalyze>(message);
        }

    }
}
