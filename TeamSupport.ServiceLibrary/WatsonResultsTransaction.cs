using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Configuration;
using System.Diagnostics;

namespace WatsonToneAnalyzer
{
    /// <summary>
    /// Transaction to insert watson results into the DB
    /// </summary>
    class WatsonResultsTransaction : IDisposable
    {
        const string EVENT_SOURCE = "Application";

        SqlConnection _connection;
        SqlTransaction _transaction;
        DataContext _db;

        /// <summary>
        /// Constructor to wrap the "using" of SqlConnection, SqlTransaction, and DataContext
        /// </summary>
        /// <param name="tones"></param>
        /// <param name="actionToAnalyze"></param>
        public WatsonResultsTransaction(Utterance utterance, ActionToAnalyze actionToAnalyze)
        {
            List<Tones> tones = utterance.tones;
            if (tones == null)
                return; // ?

            // open the connection
            string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
            _connection = new SqlConnection(connectionString);  // using
            _connection.Open(); // connection must be open to begin transaction

            // start the transaction
            _transaction = _connection.BeginTransaction();  // using

            // create a data context
            _db = new DataContext(_connection); // using
            _db.Transaction = _transaction;

            try
            {
                // insert ActionSentiment
                ActionSentiment sentiment = InsertActionSentiment(_db, actionToAnalyze);
                _db.SubmitChanges();    // get the DB generated ID
                int actionSentimentID = sentiment.ActionSentimentID;

                // insert child records - ActionSentimentScore(s)
                InsertSentimentScores(tones, _db, actionSentimentID);

                // Delete ActionToAnalyze
                actionToAnalyze.DeleteOnSubmit(_db);
                _db.SubmitChanges();

                // Success!
                _transaction.Commit();
            }
            catch (Exception e)
            {
                _transaction.Rollback();

                EventLog.WriteEntry(EVENT_SOURCE, "********************PublishToTable SubmitTransaction: Exception " + e.Message + " ### " + e.Source + " ### " + e.StackTrace.ToString());
                Console.WriteLine("Exception at insert into Action Sentiment:" + e.Message + "###" + e.Source + " ----- STACK: " + e.StackTrace.ToString());
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            if (_db != null)
                _db.Dispose();

            if (_transaction != null)
                _transaction.Dispose();

            if (_connection != null)
                _connection.Dispose();
        }

        /// <summary>
        /// Create the ActionSentiment for the ActionID
        /// </summary>
        /// <param name="db">context for insert/submit</param>
        /// <param name="a">action to analyze</param>
        /// <returns></returns>
        static ActionSentiment InsertActionSentiment(DataContext db, ActionToAnalyze a)
        {
            // already exists?
            Table<ActionSentiment> table = db.GetTable<ActionSentiment>();
            if (table.Where(u => u.ActionID == a.ActionID).Any())
                throw new Exception("Error: ActionSentiment already exists?");

            // Insert
            ActionSentiment sentiment = new ActionSentiment
            {
                ActionID = a.ActionID,
                TicketID = a.TicketID,
                UserID = a.UserID,
                OrganizationID = a.OrganizationID,
                IsAgent = a.IsAgent,
                DateCreated = DateTime.Now
            };
            table.InsertOnSubmit(sentiment);
            return sentiment;
        }

        /// <summary>
        /// Insert the detected sentiment tones, or a default if not found
        /// </summary>
        /// <param name="tones">tones from IBM Watson - may be empty</param>
        /// <param name="db">data context</param>
        /// <param name="actionSentimentID">Parent record ID</param>
        private static void InsertSentimentScores(List<Tones> tones, DataContext db, int actionSentimentID)
        {
            // tones detected?
            Table<ActionSentimentScore> actionSentimentScoreTable = db.GetTable<ActionSentimentScore>();
            if ((tones == null) || !tones.Any())
            {
                // no tones detected
                ActionSentimentScore score = new ActionSentimentScore
                {
                    ActionSentimentID = actionSentimentID,
                    SentimentID = 0,    // no sentiment detected
                    SentimentScore = 0
                };
                actionSentimentScoreTable.InsertOnSubmit(score);
                return;
            }

            // insert tone sentiment scores
            foreach (Tones tone in tones)
            {
                ActionSentimentScore score = new ActionSentimentScore
                {
                    ActionSentimentID = actionSentimentID,
                    SentimentID = (int)FindSentimentID(tone.tone_id),
                    SentimentScore = Convert.ToDecimal(tone.score)
                };
                actionSentimentScoreTable.InsertOnSubmit(score);
            }
        }

        /// <summary> See dbo.ToneSentiment </summary>
        enum ESentiment
        {
            none = 0,
            sad = 1,
            frustrated = 2,
            satisfied = 3,
            excited = 4,
            polite = 5,
            impolite = 6,
            sympathetic = 7
        }

        /// <summary> Find the sentiment enum from tone id text </summary>
        static ESentiment FindSentimentID(string Sentiment)
        {
            ESentiment result;
            if (!Enum.TryParse<ESentiment>(Sentiment, false, out result))
                return ESentiment.none;
            return result;
        }
    }
}
