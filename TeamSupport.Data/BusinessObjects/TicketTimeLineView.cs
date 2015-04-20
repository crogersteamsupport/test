using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
    public partial class TicketTimeLineViewItem
    {
    }

    public partial class TicketTimeLineView
    {
        public void LoadByRange(int ticketID, int start, int end)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"WITH BaseQuery AS(
                                        SELECT *, ROW_NUMBER() OVER (ORDER BY DateCreated DESC) AS 'RowNum'
	                                    FROM TicketTimelineView
	                                    WHERE TicketID = @TicketID AND (WCUserID IS NULL OR  WCUserID = @UserID)
                                    )

                                    SELECT *
                                    FROM BaseQuery
                                    WHERE RowNum BETWEEN @Start AND @End
                                    ORDER BY BaseQuery.RowNum
                                    ";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketID", ticketID);
                command.Parameters.AddWithValue("@UserID", LoginUser.UserID);
                command.Parameters.AddWithValue("@Start", start);
                command.Parameters.AddWithValue("@End", end);
                Fill(command);
            }
        }
    }

}
