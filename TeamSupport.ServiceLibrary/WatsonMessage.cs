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
        List<ActionToAnalyze> _actions;

        static int totalActionCount = 0;
        static int totalUtteranceCount = 0;

        public WatsonMessage()
        {
            _postContent = new WatsonPostContent();
            _utteranceActions = new List<ActionToAnalyze>();
            _actions = new List<ActionToAnalyze>();
        }

        public bool Empty {  get { return !_utteranceActions.Any(); } }
        public int ActionCount { get { return _actions.Count; } }
        public int UtteranceCount { get { return _utteranceActions.Count; } }

        public override string ToString()
        {
            return String.Format("Actions {0} Utterances {1}", ActionCount, UtteranceCount);
        }

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

            // empty so it fit and is done
            if (String.IsNullOrEmpty(utterance.text))
                return true;

            _postContent.Add(utterance);
            _actions.Add(actionToAnalyze);
            _utteranceActions.Add(actionToAnalyze);
            check();
            return true;
        }

        public bool TryAdd(ActionToAnalyze actionToAnalyze, List<Utterance> utterances)
        {
            // Max 50 per call
            if (_utteranceActions.Count + utterances.Count > WatsonUtterancePerAPICall)
                return false;

            _postContent.Add(utterances);
            _actions.Add(actionToAnalyze);
            for (int i = 0; i < utterances.Count; ++i)
                _utteranceActions.Add(actionToAnalyze);  // same action for multiple utterances
            check();
            return true;
        }

        /// <summary>serialize to JSON</summary>
        public string ToJSON()
        {
            return _postContent.ToJSON(); // JSON string
        }

        /// <summary> Write the message results back to the database </summary>
        public void PublishWatsonResponse(UtteranceToneList watsonResponse)
        {
            totalActionCount += _actions.Count;
            totalUtteranceCount += _utteranceActions.Count;

            // update the actions with the corresponding utterances
            foreach (UtteranceResponse utterance in watsonResponse.utterances_tone)
                _utteranceActions[utterance.utterance_id].AddSentiment(utterance);

            // update the action with the accumulated results
            foreach(ActionToAnalyze action in _actions)
                PublishActionSentiment(action);
        }

        /// <summary>
        /// Async callback from HTTP_POST to put the watson response into the db
        /// </summary>
        /// <param name="result">Watson results</param>
        /// <param name="actionToAnalyze">ActionToAnalyze record we are processing</param>
        static Mutex _singleThreadedTransactions = new Mutex(false);

        static void PublishActionSentiment(ActionToAnalyze actionToAnalyze)
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
                transaction.RecordWatsonResults(actionToAnalyze);
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
