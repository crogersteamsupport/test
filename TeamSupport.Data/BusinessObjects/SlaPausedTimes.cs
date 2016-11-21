using System;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
    public partial class SlaPausedTimes
    {
        public void LoadByTicketId(int ticketId)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"SELECT * FROM SlaPausedTimes WHERE TicketId = @ticketId AND ResumedOn IS NOT NULL";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ticketId", ticketId);
                Fill(command);
            }
        }
    }
}
