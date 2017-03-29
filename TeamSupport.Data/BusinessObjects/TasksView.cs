using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
    public partial class TasksViewItem
    {
    }

    public partial class TasksView
    {
        public void LoadByParentID(int ParentID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM TasksView WHERE ParentID = @ParentID ORDER BY DueDate";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ParentID", ParentID);
                Fill(command);
            }
        }
    }

}
