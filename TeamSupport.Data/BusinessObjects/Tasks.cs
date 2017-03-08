using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Configuration;
using System.Runtime.Serialization;


namespace TeamSupport.Data
{
    [DataContract(Namespace = "http://teamsupport.com/")]
    public class TaskDTO
    {
        public TaskDTO() { }
        [DataMember]
        public int CreatorID { get; set; }
        [DataMember]
        public DateTime DateCreated { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public DateTime? DueDate { get; set; }
        [DataMember]
        public DateTime? ReminderDate { get; set; }
        [DataMember]
        public bool IsDismissed { get; set; }
        [DataMember]
        public int OrganizationID { get; set; }
        [DataMember]
        public int TaskID { get; set; }
        [DataMember]
        public DateTime? DateCompleted { get; set; }
        [DataMember]
        public bool IsComplete { get; set; }
        [DataMember]
        public int? ParentID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int? UserID { get; set; }
        [DataMember]
        public int ModifierID { get; set; }
        [DataMember]
        public DateTime DateModified { get; set; }
        [DataMember]
        public int? ReminderID { get; set; }
    }


    public partial class Task
    {


        public bool IsDismissed
        {
            get
            {
                if (Row.Table.Columns.Contains("IsDismissed") && Row["IsDismissed"] != DBNull.Value)
                {
                    return (bool)Row["IsDismissed"];
                }
                else return false;
            }
        }

        public DateTime? ReminderDueDate
        {
            get
            {
                if (Row.Table.Columns.Contains("ReminderDueDate") && Row["ReminderDueDate"] != DBNull.Value)
                {
                    return (DateTime)Row["ReminderDueDate"];
                }
                else return null;
            }
        }
    }

    public partial class Tasks
    {
        public List<TaskDTO> LoadByTicketID(int ticketID)
        {
            ticketID = 3888454;

            List<TaskDTO> result = new List<TaskDTO>();

            string query = @"
                SELECT
                    ST.CreatorID,
                    ST.DateCreated,
                    ST.Description,
                    ST.DueDate,
                    R.DueDate AS 'ReminderDueDate',
                    R.IsDismissed,
                    ST.OrganizationID,
                    ST.TaskID,
                    ST.DateCompleted,
                    ST.IsComplete,
                    ST.ParentID,
                    CASE WHEN T.Name is not null THEN T.Name + ' > ' + ST.Name
                        ELSE ST.Name END AS Name,
                    ST.UserID,
                    ST.ModifierID,
					ST.DateModified,
                    R.ReminderID
                FROM
                    Tasks ST
                    LEFT JOIN Tasks T
                        ON ST.ParentID = T.TaskID
                    JOIN TaskAssociations TA
                        ON ST.TaskID = ta.TaskID
                    LEFT JOIN Reminders R
                        ON T.TaskID = R.RefID
                        AND R.RefType = 61
                WHERE
                    TA.RefType = 17 
                    AND TA.RefID = @TicketID";

            using (IDbConnection db = new SqlConnection(LoginUser.ConnectionString))
            {
                result = (List<TaskDTO>)db.Query<TaskDTO>(query, new { @TicketID = ticketID });
            }

            return result;
        }

        public void LoadByTicketID2(int ticketID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
                SELECT
                    ST.CreatorID,
                    ST.DateCreated,
                    ST.Description,
                    ST.DueDate,
                    R.DueDate AS 'ReminderDueDate',
                    R.IsDismissed,
                    ST.OrganizationID,
                    ST.TaskID,
                    ST.DateCompleted,
                    ST.IsComplete,
                    ST.ParentID,
                    CASE WHEN T.Name is not null THEN T.Name + ' > ' + ST.Name
                        ELSE ST.Name END AS Name,
                    ST.UserID,
                    ST.ModifierID,
					ST.DateModified,
                    R.ReminderID
                FROM
                    Tasks ST
                    LEFT JOIN Tasks C
                        ON ST.ParentID = T.TaskID
                    JOIN TaskAssociations TA
                        ON ST.TaskID = ta.TaskID
                    LEFT JOIN Reminders R
                        ON T.TaskID = R.RefID
                        AND R.RefType = 61
                WHERE
                    TA.RefType = 17 
                    AND TA.RefID = @TicketID";


                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketID", ticketID);
                Fill(command);
            }
        }

        public void LoadAssignedTasks(int from, int count, int userID, bool searchPending, bool searchComplete)
        {
            string pendingQuery = @"
                SELECT
                    ST.CreatorID,
                    ST.DateCreated,
                    ST.Description,
                    ST.DueDate,
                    R.DueDate AS 'ReminderDueDate',
                    R.IsDismissed,
                    ST.OrganizationID,
                    ST.TaskID,
                    ST.DateCompleted,
                    ST.IsComplete,
                    ST.ParentID,
                    CASE WHEN T.Name is not null THEN T.Name + ' > ' + ST.Name
                        ELSE ST.Name END AS Name,
                    ST.UserID,
                    ST.ModifierID,
					ST.DateModified,
                    R.ReminderID
                FROM
                    Tasks ST
                    LEFT JOIN Tasks T 
                        ON ST.ParentID = T.TaskID
                    LEFT JOIN Reminders R
                        ON ST.TaskID = R.RefID
                        AND R.RefType = 61
                WHERE
                    ST.CreatorID = @UserID
                    AND ST.UserID <> @UserID
                    AND ST.IsComplete = 0";

            string pageQuery = @"
            WITH 
                q AS ({0}),
                r AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY CASE WHEN DueDate IS NULL THEN 1 ELSE 0 END, IsComplete ASC, DateCompleted DESC, DueDate, ReminderDueDate) AS 'RowNum' FROM q)
            SELECT
                TaskID
                , OrganizationID
                , Description
                , DueDate
                , UserID
                , IsDismissed
                , CreatorID
                , DateCreated
                , Name
                , ReminderDueDate
                , IsComplete
                , DateCompleted
                , ParentID
                , ModifierID
                , DateModified
                , ReminderID
            FROM 
                r
            WHERE
                RowNum BETWEEN @From AND @To";

            StringBuilder query;
            query = new StringBuilder(string.Format(pageQuery, pendingQuery));

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = query.ToString();
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@From", from + 1);
                command.Parameters.AddWithValue("@To", from + count);
                Fill(command);
            }
        }

        public void LoadMyTasks(int from, int count, int userID, bool searchPending, bool searchComplete)
        {
            string pendingQuery =
                @"SELECT
                    ST.CreatorID,
                    ST.DateCreated,
                    ST.Description,
                    ST.DueDate,
                    R.DueDate AS 'ReminderDueDate',
                    R.IsDismissed,
                    ST.OrganizationID,
                    ST.TaskID,
                    ST.DateCompleted,
                    ST.IsComplete,
                    ST.ParentID,
                    CASE WHEN T.Name is not null THEN T.Name + ' > ' + ST.Name
                        ELSE ST.Name END AS Name,
                    ST.UserID,
                    ST.ModifierID,
					ST.DateModified,
                    R.ReminderID
                FROM 
                    Tasks ST
                    LEFT JOIN Tasks T 
                        ON ST.ParentID = T.TaskID
                    LEFT JOIN Reminders R
                        ON ST.TaskID = R.RefID
                        AND R.RefType = 61
                WHERE
                    ST.UserID = @UserID
                    AND ST.IsComplete = 0";

            string pageQuery = @"
            WITH 
                q AS ({0}),
                r AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY CASE WHEN DueDate IS NULL THEN 1 ELSE 0 END, IsComplete ASC, DueDate ASC) AS 'RowNum' FROM q)
            SELECT
                TaskID
                , OrganizationID
                , Description
                , DueDate
                , UserID
                , IsDismissed
                , CreatorID
                , DateCreated
                , Name
                , DueDate
                , IsComplete
                , DateCompleted
                , ParentID
                , ReminderID
                , ModifierID
                , DateModified
            FROM 
                r
            WHERE
                RowNum BETWEEN @From AND @To";

            StringBuilder query;

            query = new StringBuilder(string.Format(pageQuery, pendingQuery));

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = query.ToString();
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@From", from + 1);
                command.Parameters.AddWithValue("@To", from + count);
                Fill(command);
            }
        }

        public void LoadCompleted(int from, int count, int userID, bool searchPending, bool searchComplete)
        {
            string completeQuery =
                 @"SELECT
                    ST.CreatorID,
                    ST.DateCreated,
                    ST.Description,
                    ST.DueDate,
                    R.DueDate AS 'ReminderDueDate',
                    R.IsDismissed,
                    ST.OrganizationID,
                    ST.TaskID,
                    ST.DateCompleted,
                    ST.IsComplete,
                    ST.ParentID,
                    CASE WHEN T.Name is not null THEN T.Name + ' > ' + ST.Name
                        ELSE ST.Name END AS Name,
                    ST.UserID,
                    ST.ModifierID,
					ST.DateModified,
                    R.ReminderID
                FROM
                    Tasks ST
                    LEFT JOIN Tasks T 
                        ON ST.ParentID = T.TaskID
                    LEFT JOIN Reminders R
                        ON ST.TaskID = R.RefID
                        AND R.RefType = 61
                WHERE
                    (
                        ST.CreatorID = @UserID
                        OR ST.UserID = @UserID
                    )
                    AND ST.IsComplete = 1";

            string pageQuery = @"
            WITH 
                q AS ({0}),
                r AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY IsComplete ASC, DateCompleted DESC, DueDate, ReminderDueDate) AS 'RowNum' FROM q)
            SELECT
                TaskID
                , OrganizationID
                , Description
                , DueDate
                , UserID
                , IsDismissed
                , CreatorID
                , DateCreated
                , Name
                , ReminderDueDate
                , IsComplete
                , DateCompleted
                , ParentID
                , ModifierID
                , DateModified
                , ReminderID
            FROM 
                r
            WHERE
                RowNum BETWEEN @From AND @To";

            StringBuilder query;
            query = new StringBuilder(string.Format(pageQuery, completeQuery));

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = query.ToString();
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@From", from + 1);
                command.Parameters.AddWithValue("@To", from + count);
                Fill(command);
            }
        }

        public void LoadByParentID(int parentID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Tasks WHERE ParentID = @ParentID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ParentID", parentID);
                Fill(command);
            }
        }

        public void LoadIncompleteByParentID(int parentID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Tasks WHERE ParentID = @ParentID AND IsComplete = 0";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ParentID", parentID);
                Fill(command);
            }
        }

        public void LoadByCompany(int from, int count, int organizationID)
        {
            //Paging implemented but currently excluded

            string completeQuery = @"
                SELECT
                    ST.CreatorID,
                    ST.DateCreated,
                    ST.Description,
                    ST.DueDate,
                    R.DueDate AS 'ReminderDueDate',
                    R.IsDismissed,
                    ST.OrganizationID,
                    ST.TaskID,
                    ST.DateCompleted,
                    ST.IsComplete,
                    ST.ParentID,
                    CASE WHEN T.Name is not null THEN T.Name + ' > ' + ST.Name
                        ELSE ST.Name END AS Name,
                    ST.UserID,
                    ST.ModifierID,
					ST.DateModified,
                    R.ReminderID
                FROM
                    Tasks ST
                    LEFT JOIN Tasks T 
                        ON ST.ParentID = T.TaskID
                    LEFT JOIN Reminders R
                        ON ST.TaskID = R.RefID
                        AND R.RefType = 61
                    JOIN TaskAssociations ta
                        ON ST.TaskID = ta.TaskID
                WHERE
                    ta.RefType = 9
                    AND ta.RefID = @organizationID";

            string pageQuery = @"
            WITH 
                q AS ({0}),
                t AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY IsComplete asc, CASE WHEN DueDate IS NULL THEN 1 ELSE 0 END, IsComplete ASC, DueDate, ReminderDueDate) AS 'RowNum' FROM q)
            SELECT
                TaskID
                , OrganizationID
                , Description
                , DueDate
                , UserID
                , IsDismissed
                , CreatorID
                , DateCreated
                , Name
                , ReminderDueDate
                , IsComplete
                , DateCompleted
                , ParentID
                , ModifierID
                , DateModified
                , ReminderID
            FROM 
                t";
            //WHERE
            //    RowNum BETWEEN @From AND @To";

            StringBuilder query;

            query = new StringBuilder(string.Format(pageQuery, completeQuery));

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = query.ToString();
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@organizationID", organizationID);
                //command.Parameters.AddWithValue("@From", from + 1);
                //command.Parameters.AddWithValue("@To", from + count);
                Fill(command);
            }
        }

        public void LoadByContact(int from, int count, int contactID)
        {
            //Paging has been written for but is currently excluded.

            string completeQuery = @"
                SELECT
                    ST.CreatorID,
                    ST.DateCreated,
                    ST.Description,
                    ST.DueDate,
                    R.DueDate AS 'ReminderDueDate',
                    R.IsDismissed,
                    ST.OrganizationID,
                    ST.TaskID,
                    ST.DateCompleted,
                    ST.IsComplete,
                    ST.ParentID,
                    CASE WHEN T.Name is not null THEN T.Name + ' > ' + ST.Name
                        ELSE ST.Name END AS Name,
                    ST.UserID,
                    ST.ModifierID,
					ST.DateModified,
                    R.ReminderID
                FROM
                    Tasks ST
                    LEFT JOIN Tasks T 
                        ON ST.ParentID = T.TaskID
                    LEFT JOIN Reminders R
                        ON ST.TaskID = R.RefID
                        AND R.RefType = 61
                    JOIN TaskAssociations ta
                        ON ST.TaskID = ta.TaskID
                WHERE
                    ta.RefType = 32
                    AND ta.RefID = @contactID";

            string pageQuery = @"
            WITH 
                q AS ({0}),
                t AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY IsComplete asc, CASE WHEN DueDate IS NULL THEN 1 ELSE 0 END, IsComplete ASC, DueDate, ReminderDueDate) AS 'RowNum' FROM q)
            SELECT
                TaskID
                , OrganizationID
                , Description
                , DueDate
                , UserID
                , IsDismissed
                , CreatorID
                , DateCreated
                , Name
                , ReminderDueDate
                , IsComplete
                , DateCompleted
                , ParentID
                , ModifierID
                , DateModified
                , ReminderID
            FROM 
                t";
            //WHERE
            //    RowNum BETWEEN @From AND @To;

            StringBuilder query;

            query = new StringBuilder(string.Format(pageQuery, completeQuery));

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = query.ToString();
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@contactID", contactID);
                //command.Parameters.AddWithValue("@From", from + 1);
                //command.Parameters.AddWithValue("@To", from + count);
                Fill(command);
            }
        }

        public void LoadByUser(int from, int count, int userID)
        {
            //Paging has been written for but is currently excluded.

            string completeQuery = @"
                SELECT
                    ST.CreatorID,
                    ST.DateCreated,
                    ST.Description,
                    ST.DueDate,
                    R.DueDate AS 'ReminderDueDate',
                    R.IsDismissed,
                    ST.OrganizationID,
                    ST.TaskID,
                    ST.DateCompleted,
                    ST.IsComplete,
                    ST.ParentID,
                    CASE WHEN T.Name is not null THEN T.Name + ' > ' + ST.Name
                        ELSE ST.Name END AS Name,
                    ST.UserID,
                    ST.ModifierID,
					ST.DateModified,
                    R.ReminderID
                FROM
                    Tasks ST
                    LEFT JOIN Tasks T 
                        ON ST.ParentID = T.TaskID
                    LEFT JOIN Reminders R
                        ON ST.TaskID = R.RefID
                        AND R.RefType = 61
                    JOIN TaskAssociations ta
                        ON ST.TaskID = ta.TaskID
                WHERE
                    ta.RefType = 22
                    AND ta.RefID = @userID";

            string pageQuery = @"
            WITH 
                q AS ({0}),
                t AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY IsComplete asc, CASE WHEN DueDate IS NULL THEN 1 ELSE 0 END, IsComplete ASC, DueDate, ReminderDueDate) AS 'RowNum' FROM q)
            SELECT
                TaskID
                , OrganizationID
                , Description
                , DueDate
                , UserID
                , IsDismissed
                , CreatorID
                , DateCreated
                , Name
                , ReminderDueDate
                , IsComplete
                , DateCompleted
                , ParentID
                , ModifierID
                , DateModified
                , ReminderID
            FROM 
                t";
            //WHERE
            //    RowNum BETWEEN @From AND @To;

            StringBuilder query;

            query = new StringBuilder(string.Format(pageQuery, completeQuery));

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = query.ToString();
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@userID", userID);
                //command.Parameters.AddWithValue("@From", from + 1);
                //command.Parameters.AddWithValue("@To", from + count);
                Fill(command);
            }
        }

        public void LoadCompleteAssociatedToCompany(int from, int count, int organizationID)
        {
            string completeQuery = @"
            SELECT 
                t.*,
                r.IsDismissed,
                r.DueDate AS 'ReminderDueDate'
            FROM
                Tasks t
                JOIN TaskAssociations ta
                    ON t.TaskID = ta.TaskID
                LEFT JOIN Reminders r
                    ON t.TaskID = r.RefID
                    AND r.RefType = 61
            WHERE
                t.IsComplete = 1
                AND ta.RefType = 9
                AND ta.RefID = @organizationID";

            string pageQuery = @"
            WITH 
                q AS ({0}),
                t AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY DueDate, ReminderDueDate) AS 'RowNum' FROM q)
            SELECT
                TaskID
                , OrganizationID
                , Description
                , DueDate
                , UserID
                , IsDismissed
                , CreatorID
                , DateCreated
                , Name
                , ReminderDueDate
                , IsComplete
                , DateCompleted
                , ParentID
                , ModifierID
                , DateModified
                , ReminderID
            FROM 
                t
            WHERE
                RowNum BETWEEN @From AND @To";

            StringBuilder query;

            query = new StringBuilder(string.Format(pageQuery, completeQuery));

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = query.ToString();
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@organizationID", organizationID);
                command.Parameters.AddWithValue("@From", from + 1);
                command.Parameters.AddWithValue("@To", from + count);
                Fill(command);
            }
        }

        public void LoadByUserAndAssociatedToCompany(int from, int count, int organizationID, int userID)
        {
            string completeQuery = @"
            SELECT 
                t.*,
                r.IsDismissed,
                r.DueDate AS 'ReminderDueDate'
            FROM
                Tasks t
                JOIN TaskAssociations ta
                    ON t.TaskID = ta.TaskID
                LEFT JOIN Reminders r
                    ON t.TaskID = r.RefID
                    AND r.RefType = 61
            WHERE
                (t.CreatorID = @UserID OR t.UserID = @UserID)
                AND ta.RefType = 9
                AND ta.RefID = @organizationID";

            string pageQuery = @"
            WITH 
                q AS ({0}),
                t AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY DueDate, ReminderDueDate) AS 'RowNum' FROM q)
            SELECT
                TaskID
                , OrganizationID
                , Description
                , DueDate
                , UserID
                , IsDismissed
                , CreatorID
                , DateCreated
                , Name
                , ReminderDueDate
                , IsComplete
                , DateCompleted
                , ParentID
                , ModifierID
                , DateModified
                , ReminderID
            FROM 
                t
            WHERE
                RowNum BETWEEN @From AND @To";

            StringBuilder query;

            query = new StringBuilder(string.Format(pageQuery, completeQuery));

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = query.ToString();
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@organizationID", organizationID);
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@From", from + 1);
                command.Parameters.AddWithValue("@To", from + count);
                Fill(command);
            }
        }
    }
}
