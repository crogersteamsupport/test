﻿using System;
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
    #region DTO Classes

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
        public DateTime? ReminderDueDate { get; set; }
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
        public string AssignedTo { get; set; }
        [DataMember]
        public int ModifierID { get; set; }
        [DataMember]
        public DateTime DateModified { get; set; }
        [DataMember]
        public int? ReminderID { get; set; }
        [DataMember]
        public List<TaskDTO> SubTasks { get; set; }
        [DataMember]
        public TaskAssociationsViewItemProxy[] Associations { get; set; }
    }

    #endregion
    public partial class Tasks
    {
        public List<TaskDTO> LoadByTicketID(int ticketID)
        {
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

        public List<TaskDTO> LoadAssignedTasks(int from, int count, int userID, bool searchPending, bool searchComplete)
        {
            List<TaskDTO> result = new List<TaskDTO>();

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

            using (IDbConnection db = new SqlConnection(LoginUser.ConnectionString))
            {
                result = (List<TaskDTO>)db.Query<TaskDTO>(query.ToString(), new { @UserID = userID, @From = from + 1, @To = from + count });
            }

            return result;
        }

        public List<TaskDTO> LoadMyTasks(int from, int count, int userID, bool searchPending, bool searchComplete)
        {
            List<TaskDTO> result = new List<TaskDTO>();

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

            using (IDbConnection db = new SqlConnection(LoginUser.ConnectionString))
            {
                result = (List<TaskDTO>)db.Query<TaskDTO>(query.ToString(), new { @UserID = userID, @From = from + 1, @To = from + count });
            }

            return result;
        }

        public List<TaskDTO> LoadCompleted(int from, int count, int userID, bool searchPending, bool searchComplete)
        {
            List<TaskDTO> result = new List<TaskDTO>();

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

            using (IDbConnection db = new SqlConnection(LoginUser.ConnectionString))
            {
                result = (List<TaskDTO>)db.Query<TaskDTO>(query.ToString(), new { @UserID = userID, @From = from + 1, @To = from + count });
            }

            return result;
        }

        #region Associations

     //   public void LoadByCompany(int from, int count, int organizationID)
     //   {
     //       //Paging implemented but currently excluded

     //       string completeQuery = @"
     //           SELECT
     //               ST.CreatorID,
     //               ST.DateCreated,
     //               ST.Description,
     //               ST.DueDate,
     //               R.DueDate AS 'ReminderDueDate',
     //               R.IsDismissed,
     //               ST.OrganizationID,
     //               ST.TaskID,
     //               ST.DateCompleted,
     //               ST.IsComplete,
     //               ST.ParentID,
     //               CASE WHEN T.Name is not null THEN T.Name + ' > ' + ST.Name
     //                   ELSE ST.Name END AS Name,
     //               ST.UserID,
     //               ST.ModifierID,
					//ST.DateModified,
     //               R.ReminderID
     //           FROM
     //               Tasks ST
     //               LEFT JOIN Tasks T 
     //                   ON ST.ParentID = T.TaskID
     //               LEFT JOIN Reminders R
     //                   ON ST.TaskID = R.RefID
     //                   AND R.RefType = 61
     //               JOIN TaskAssociations ta
     //                   ON ST.TaskID = ta.TaskID
     //           WHERE
     //               ta.RefType = 9
     //               AND ta.RefID = @organizationID";

     //       string pageQuery = @"
     //       WITH 
     //           q AS ({0}),
     //           t AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY IsComplete asc, CASE WHEN DueDate IS NULL THEN 1 ELSE 0 END, IsComplete ASC, DueDate, ReminderDueDate) AS 'RowNum' FROM q)
     //       SELECT
     //           TaskID
     //           , OrganizationID
     //           , Description
     //           , DueDate
     //           , UserID
     //           , IsDismissed
     //           , CreatorID
     //           , DateCreated
     //           , Name
     //           , ReminderDueDate
     //           , IsComplete
     //           , DateCompleted
     //           , ParentID
     //           , ModifierID
     //           , DateModified
     //           , ReminderID
     //       FROM 
     //           t";
     //       //WHERE
     //       //    RowNum BETWEEN @From AND @To";

     //       StringBuilder query;

     //       query = new StringBuilder(string.Format(pageQuery, completeQuery));

     //       using (SqlCommand command = new SqlCommand())
     //       {
     //           command.CommandText = query.ToString();
     //           command.CommandType = CommandType.Text;
     //           command.Parameters.AddWithValue("@organizationID", organizationID);
     //           //command.Parameters.AddWithValue("@From", from + 1);
     //           //command.Parameters.AddWithValue("@To", from + count);
     //           Fill(command);
     //       }
     //   }

     //   public void LoadByContact(int from, int count, int contactID)
     //   {
     //       //Paging has been written for but is currently excluded.

     //       string completeQuery = @"
     //           SELECT
     //               ST.CreatorID,
     //               ST.DateCreated,
     //               ST.Description,
     //               ST.DueDate,
     //               R.DueDate AS 'ReminderDueDate',
     //               R.IsDismissed,
     //               ST.OrganizationID,
     //               ST.TaskID,
     //               ST.DateCompleted,
     //               ST.IsComplete,
     //               ST.ParentID,
     //               CASE WHEN T.Name is not null THEN T.Name + ' > ' + ST.Name
     //                   ELSE ST.Name END AS Name,
     //               ST.UserID,
     //               ST.ModifierID,
					//ST.DateModified,
     //               R.ReminderID
     //           FROM
     //               Tasks ST
     //               LEFT JOIN Tasks T 
     //                   ON ST.ParentID = T.TaskID
     //               LEFT JOIN Reminders R
     //                   ON ST.TaskID = R.RefID
     //                   AND R.RefType = 61
     //               JOIN TaskAssociations ta
     //                   ON ST.TaskID = ta.TaskID
     //           WHERE
     //               ta.RefType = 32
     //               AND ta.RefID = @contactID";

     //       string pageQuery = @"
     //       WITH 
     //           q AS ({0}),
     //           t AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY IsComplete asc, CASE WHEN DueDate IS NULL THEN 1 ELSE 0 END, IsComplete ASC, DueDate, ReminderDueDate) AS 'RowNum' FROM q)
     //       SELECT
     //           TaskID
     //           , OrganizationID
     //           , Description
     //           , DueDate
     //           , UserID
     //           , IsDismissed
     //           , CreatorID
     //           , DateCreated
     //           , Name
     //           , ReminderDueDate
     //           , IsComplete
     //           , DateCompleted
     //           , ParentID
     //           , ModifierID
     //           , DateModified
     //           , ReminderID
     //       FROM 
     //           t";
     //       //WHERE
     //       //    RowNum BETWEEN @From AND @To;

     //       StringBuilder query;

     //       query = new StringBuilder(string.Format(pageQuery, completeQuery));

     //       using (SqlCommand command = new SqlCommand())
     //       {
     //           command.CommandText = query.ToString();
     //           command.CommandType = CommandType.Text;
     //           command.Parameters.AddWithValue("@contactID", contactID);
     //           //command.Parameters.AddWithValue("@From", from + 1);
     //           //command.Parameters.AddWithValue("@To", from + count);
     //           Fill(command);
     //       }
     //   }

     //   public void LoadByUser(int from, int count, int userID)
     //   {
     //       //Paging has been written for but is currently excluded.

     //       string completeQuery = @"
     //           SELECT
     //               ST.CreatorID,
     //               ST.DateCreated,
     //               ST.Description,
     //               ST.DueDate,
     //               R.DueDate AS 'ReminderDueDate',
     //               R.IsDismissed,
     //               ST.OrganizationID,
     //               ST.TaskID,
     //               ST.DateCompleted,
     //               ST.IsComplete,
     //               ST.ParentID,
     //               CASE WHEN T.Name is not null THEN T.Name + ' > ' + ST.Name
     //                   ELSE ST.Name END AS Name,
     //               ST.UserID,
     //               ST.ModifierID,
					//ST.DateModified,
     //               R.ReminderID
     //           FROM
     //               Tasks ST
     //               LEFT JOIN Tasks T 
     //                   ON ST.ParentID = T.TaskID
     //               LEFT JOIN Reminders R
     //                   ON ST.TaskID = R.RefID
     //                   AND R.RefType = 61
     //               JOIN TaskAssociations ta
     //                   ON ST.TaskID = ta.TaskID
     //           WHERE
     //               ta.RefType = 22
     //               AND ta.RefID = @userID";

     //       string pageQuery = @"
     //       WITH 
     //           q AS ({0}),
     //           t AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY IsComplete asc, CASE WHEN DueDate IS NULL THEN 1 ELSE 0 END, IsComplete ASC, DueDate, ReminderDueDate) AS 'RowNum' FROM q)
     //       SELECT
     //           TaskID
     //           , OrganizationID
     //           , Description
     //           , DueDate
     //           , UserID
     //           , IsDismissed
     //           , CreatorID
     //           , DateCreated
     //           , Name
     //           , ReminderDueDate
     //           , IsComplete
     //           , DateCompleted
     //           , ParentID
     //           , ModifierID
     //           , DateModified
     //           , ReminderID
     //       FROM 
     //           t";
     //       //WHERE
     //       //    RowNum BETWEEN @From AND @To;

     //       StringBuilder query;

     //       query = new StringBuilder(string.Format(pageQuery, completeQuery));

     //       using (SqlCommand command = new SqlCommand())
     //       {
     //           command.CommandText = query.ToString();
     //           command.CommandType = CommandType.Text;
     //           command.Parameters.AddWithValue("@userID", userID);
     //           //command.Parameters.AddWithValue("@From", from + 1);
     //           //command.Parameters.AddWithValue("@To", from + count);
     //           Fill(command);
     //       }
     //   }

        #endregion
    }
}
