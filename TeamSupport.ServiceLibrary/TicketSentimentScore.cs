using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace WatsonToneAnalyzer
{
    [Table(Name = "TicketSentimentScores")]
    class TicketSentimentScoreLinq
    {
        int _ticketSentimentScoreID;
        [Column(Storage = "_ticketSentimentScoreID", DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int TicketSentimentScoreID { get { return _ticketSentimentScoreID; } }

        [Column]
        public int TicketID;
        [Column]
        public bool IsAgent;
        [Column]
        public int TicketSentimentScore;
        [Column]
        public bool Sad;
        [Column]
        public bool Frustrated;
        [Column]
        public bool Satisfied;
        [Column]
        public bool Excited;
        [Column]
        public bool Polite;
        [Column]
        public bool Impolite;
        [Column]
        public bool Sympathetic;

        public void SetSentimentID(int sentimentID)
        {
            switch (sentimentID)
            {
                case 1:
                    Sad = true;
                    break;
                case 2:
                    Frustrated = true;
                    break;
                case 3:
                    Satisfied = true;
                    break;
                case 4:
                    Excited = true;
                    break;
                case 5:
                    Polite = true;
                    break;
                case 6:
                    Impolite = true;
                    break;
                case 7:
                    Sympathetic = true;
                    break;
            }
        }
    }
}
