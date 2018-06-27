using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Diagnostics;
using System.Threading;

namespace WatsonToneAnalyzer
{
    class WatsonMessage
    {
        WatsonPostContent _postContent;
        List<ActionToAnalyze> _utteranceActions;

        public WatsonMessage()
        {
            _postContent = new WatsonPostContent();
            _utteranceActions = new List<ActionToAnalyze>();
        }

        public bool Empty {  get { return _utteranceActions.Count == 0; } }

        public int ActionCount { get; private set; }

        void check()
        {
            if (_utteranceActions.Count != _postContent.utterances.Count)
                Debugger.Break();
        }

        static int WatsonUtterancePerAPICall = Int32.Parse(ConfigurationManager.AppSettings.Get("WatsonUtterancePerAPICall"));

        public bool TryAdd(ActionToAnalyze actionToAnalyze, Utterance utterance)
        {
            // Max 50 per call
            if (_utteranceActions.Count >= WatsonUtterancePerAPICall)
                return false;

            _utteranceActions.Add(actionToAnalyze);
            _postContent.Add(utterance);
            check();
            ++ActionCount;
            return true;
        }

        public bool TryAdd(ActionToAnalyze actionToAnalyze, List<Utterance> utterances)
        {
            // Max 50 per call
            if (_utteranceActions.Count + utterances.Count > WatsonUtterancePerAPICall)
                return false;

            _postContent.Add(utterances);
            for (int i = 0; i < utterances.Count; ++i)
                _utteranceActions.Add(actionToAnalyze);  // same action for multiple utterances
            check();
            ++ActionCount;
            return true;
        }

        /// <summary>serialize to JSON</summary>
        public string ToJSON()
        {
            return _postContent.ToJSON(); // JSON string
        }

        public void PublishWatsonResponse(UtteranceToneList watsonResponse)
        {
            foreach (UtteranceResponse utterance in watsonResponse.utterances_tone)
                PublishActionSentiment(_utteranceActions[utterance.utterance_id], utterance);
        }

        /// <summary>
        /// Async callback from HTTP_POST to put the watson response into the db
        /// </summary>
        /// <param name="result">Watson results</param>
        /// <param name="actionToAnalyze">ActionToAnalyze record we are processing</param>
        static Mutex _singleThreadedTransactions = new Mutex(false);

        static void PublishActionSentiment(ActionToAnalyze actionToAnalyze, UtteranceResponse result)
        {
            // Process The ActionToAnalyze results
            WatsonTransaction transaction = null;  // Transaction that can be rolled back
            try
            {
                // 1. Insert ActionSentiment and ActionSentimentScores
                // 2. run the TicketSentimentStrategy to create TicketSentimentScore
                // 3. delete the ActionToAnalyze
                _singleThreadedTransactions.WaitOne();  // connection does not support parallel transactions
                transaction = new WatsonTransaction();
                transaction.RecordWatsonResults(result, actionToAnalyze);
                transaction.Commit();
            }
            catch (Exception e2)
            {
                if (transaction != null)
                    transaction.Rollback();
                WatsonEventLog.WriteEntry("Watson analysis failed - system will retry", e2);
                Console.WriteLine(e2.ToString());
            }
            finally
            {
                if (transaction != null)
                    transaction.Dispose();
                _singleThreadedTransactions.ReleaseMutex();
            }
        }

    }
}
