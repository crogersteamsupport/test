using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace TeamSupport.CDI.linq
{
    [Table(Name = "Users")]
    class User
    {
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
        int _userID;
        [Column(Storage = "_userID", DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int UserID { get { return _userID; } }

        [Column]
        public int OrganizationID;
#pragma warning restore CS0649
    }
}
