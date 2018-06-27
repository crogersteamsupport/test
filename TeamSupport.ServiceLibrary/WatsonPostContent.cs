using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Configuration;
using System.Text.RegularExpressions;

namespace WatsonToneAnalyzer
{
    /// <summary>
    /// Convert the action text and users into utterances for the post to watson
    /// </summary>
    class WatsonPostContent
    {
        public List<Utterance> utterances;

        /// <summary>default constructor</summary>
        public WatsonPostContent()
        {
            utterances = new List<Utterance>();
        }

        /// <summary>serialize to JSON</summary>
        public override string ToString()
        {
            // escapes \b, \f, \n, \r, \t, ", and \
            return JsonConvert.SerializeObject(this);
        }

        public void Add(Utterance utterance)
        {
            utterances.Add(utterance);
            if (utterance.text.Length > 500)
                System.Diagnostics.Debugger.Break();
        }
        public void Add(List<Utterance> utterance) { utterances.AddRange(utterance); }
    }
}
