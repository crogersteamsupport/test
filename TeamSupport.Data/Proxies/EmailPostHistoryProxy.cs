using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class EmailPostHistoryItem : BaseItem
  {
    public EmailPostHistoryItemProxy GetProxy()
    {
      EmailPostHistoryItemProxy result = new EmailPostHistoryItemProxy();
      result.Text3 = this.Text3;
      result.Text2 = this.Text2;
      result.Text1 = this.Text1;
      result.Param10 = this.Param10;
      result.Param9 = this.Param9;
      result.Param8 = this.Param8;
      result.Param7 = this.Param7;
      result.Param6 = this.Param6;
      result.Param5 = this.Param5;
      result.Param4 = this.Param4;
      result.Param3 = this.Param3;
      result.Param2 = this.Param2;
      result.Param1 = this.Param1;
      result.CreatorID = this.CreatorID;
      result.HoldTime = this.HoldTime;
      result.EmailPostType = this.EmailPostType;
      result.EmailPostID = this.EmailPostID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
