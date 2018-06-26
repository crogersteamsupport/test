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

        static int MaxUtteranceLength = Int32.Parse(ConfigurationManager.AppSettings.Get("MaxUtteranceLength"));

        public void AddToJson(ActionToAnalyze actionToAnalyze, List<ActionToAnalyze> actionsForAPICall)
        {
            try
            {
                // action fit in utterance?
                string text = actionToAnalyze.WatsonText();
                if (text.Length <= MaxUtteranceLength)
                {
                    Add(actionToAnalyze.ActionID.ToString(), text);
                    actionsForAPICall.Add(actionToAnalyze);
                    return;
                }

                // pack sentences into utterances
                StringBuilder utterance = new StringBuilder();
                string[] sentences = Regex.Split(text, @"(?<=[.?!,;:])");
                foreach (string sentence in sentences)
                {
                    // sentence fit in utterance
                    if (utterance.Length + sentence.Length <= MaxUtteranceLength)
                    {
                        utterance.Append(sentence);
                        continue;
                    }

                    // utterance full
                    if (sentence.Length < MaxUtteranceLength)
                    {
                        Add(actionToAnalyze.ActionID.ToString(), utterance.ToString());
                        actionsForAPICall.Add(actionToAnalyze);
                        utterance.Clear();
                        continue;
                    }

                    // sentence length overflow
                    int offset = 0;
                    while (sentence.Length - offset > MaxUtteranceLength)
                    {
                        int appendLength = MaxUtteranceLength - utterance.Length;
                        utterance.Append(sentence.Substring(offset, appendLength));
                        Add(actionToAnalyze.ActionID.ToString(), utterance.ToString());
                        actionsForAPICall.Add(actionToAnalyze);
                        utterance.Clear();
                        offset += appendLength;
                    }
                    utterance.Append(sentence.Substring(offset));
                }
                Add(actionToAnalyze.ActionID.ToString(), utterance.ToString());  // extract the raw text
                actionsForAPICall.Add(actionToAnalyze);
            }
            catch (Exception e)
            {
                WatsonEventLog.WriteEntry("Exception while reading from action table:", e);
                Console.WriteLine(e.ToString());
            }
        }

        static int WatsonUtterancePerAPICall = Int32.Parse(ConfigurationManager.AppSettings.Get("WatsonUtterancePerAPICall"));

        // move overflow to next WatsonPostContent
        public void TakeRemainder(WatsonPostContent from)
        {
            if (from.utterances.Count <= WatsonUtterancePerAPICall)
                return;

            List<Utterance> source = from.utterances;
            int remainder = source.Count - WatsonUtterancePerAPICall;
            for (int i = 0; i < remainder; ++i)
            {
                utterances.Add(source.Last());
                source.RemoveAt(source.Count - 1);
            }
        }

    }
}
