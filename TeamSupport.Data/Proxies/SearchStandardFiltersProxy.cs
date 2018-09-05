using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
    public partial class SearchStandardFilter : BaseItem
    {
        public SearchStandardFilterProxy GetProxy()
        {
            SearchStandardFilterProxy result = new SearchStandardFilterProxy();
            result.Tasks = this.Tasks;
            result.WaterCooler = this.WaterCooler;
            result.ProductVersions = this.ProductVersions;
            result.Notes = this.Notes;
            result.Wikis = this.Wikis;
            result.KnowledgeBase = this.KnowledgeBase;
            result.Tickets = this.Tickets;
            result.UserID = this.UserID;
            result.StandardFilterID = this.StandardFilterID;



            return result;
        }
    }
}
