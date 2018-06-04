﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace TeamSupport.CDI.linq
{
    [Table(Name = "TicketSentiments")]
    class TicketSentiment
    {
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
        int _ticketSentimentID;
        [Column(Storage = "_ticketSentimentID", DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int TicketSentimentID { get { return _ticketSentimentID; } }

        [Column]
        public int TicketID;
        [Column]
        public double AverageActionSentiment;
#pragma warning restore CS0649
    }
}