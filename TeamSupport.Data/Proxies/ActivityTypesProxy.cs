using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
    public partial class ActivityType : BaseItem
    {
        public ActivityTypeProxy GetProxy()
        {
            ActivityTypeProxy result = new ActivityTypeProxy();

            result.ModifierID = this.ModifierID;
            result.CreatorID = this.CreatorID;
            result.OrganizationID = this.OrganizationID;
            result.Position = this.Position;
            result.Description = (this.Description);
            result.Name = (this.Name);
            result.ActivityTypeID = this.ActivityTypeID;

            result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
            result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);


            return result;
        }
    }
}
