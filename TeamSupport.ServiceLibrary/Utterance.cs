using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Text.RegularExpressions;

namespace WatsonToneAnalyzer
{
    public enum User
    {
        customer,
        agent
    }

    public class Utterance
    {
        public string user;  // do not rename - must match IBM spec "user"
        public string text;  // do not rename - must match IBM spec "text"

        public Utterance(bool isAgent, string inputText)
        {
            user = (isAgent ? User.agent : User.customer).ToString();
            text = inputText;
            if (inputText.Length > MaxUtteranceLength)
                throw new Exception(String.Format("utterance text overflow: {0}", inputText));
        }

        static int MaxUtteranceLength = Int32.Parse(ConfigurationManager.AppSettings.Get("MaxUtteranceLength"));

        public static List<Utterance> ParseToUtteranceRequest(bool isAgent, string text)
        {
            List<Utterance> results = new List<Utterance>();
            try
            {
                // action fit in utterance?
                if (text.Length <= MaxUtteranceLength)
                {
                    results.Add(new Utterance(isAgent, text));
                    return results;
                }

                // pack sentences into utterances
                StringBuilder utteranceText = new StringBuilder();
                string[] sentences = Regex.Split(text, @"(?<=[.?!,;:])");
                foreach (string sentence in sentences)
                {
                    // sentence fit in utterance
                    if (utteranceText.Length + sentence.Length <= MaxUtteranceLength)
                    {
                        utteranceText.Append(sentence);
                        continue;
                    }

                    // utterance full
                    if (sentence.Length < MaxUtteranceLength)
                    {
                        results.Add(new Utterance(isAgent, utteranceText.ToString()));
                        utteranceText.Clear();
                        utteranceText.Append(sentence);
                        continue;
                    }

                    // sentence length overflow
                    int offset = 0;
                    while (sentence.Length - offset > MaxUtteranceLength)
                    {
                        int appendLength = MaxUtteranceLength - utteranceText.Length;
                        utteranceText.Append(sentence.Substring(offset, appendLength));
                        results.Add(new Utterance(isAgent, utteranceText.ToString()));
                        utteranceText.Clear();
                        offset += appendLength;
                    }
                    utteranceText.Append(sentence.Substring(offset));
                }
                results.Add(new Utterance(isAgent, utteranceText.ToString()));
            }
            catch (Exception e)
            {
                WatsonEventLog.WriteEntry("Exception while reading from action table:", e);
                Console.WriteLine(e.ToString());
            }

            return results;
        }

    }


}
