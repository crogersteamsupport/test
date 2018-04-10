using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WatsonToneAnalyzer
{
    /// <summary>
    /// Convert the action text and users into utterances for the post to watson
    /// </summary>
    class WatsonPostContent
    {
        public class Utterance
        {
            public string user;
            public string text;
        }

        public List<Utterance> utterances;

        /// <summary>default constructor</summary>
        public WatsonPostContent()
        {
            utterances = new List<Utterance>();
        }

        /// <summary>insert an utterance</summary>
        public void Add(string userID, string inputText)
        {
            utterances.Add(new WatsonPostContent.Utterance()
            {
                user = userID,
                text = inputText
            });
        }

        /// <summary>serialize to JSON</summary>
        public override string ToString()
        {
            // escapes \b, \f, \n, \r, \t, ", and \
            return JsonConvert.SerializeObject(this);
        }
    }
}
