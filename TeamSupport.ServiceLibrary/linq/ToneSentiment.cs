using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace WatsonToneAnalyzer
{
    [Table(Name = "ToneSentiments")]
    class ToneSentiment
    {
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
        int _sentimentID;
        [Column(Storage = "_sentimentID", DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int SentimentID { get { return _sentimentID; } }

        [Column]
        public string SentimentName;
        [Column]
        public decimal? SentimentMultiplier;
#pragma warning restore CS0649
    }
}
