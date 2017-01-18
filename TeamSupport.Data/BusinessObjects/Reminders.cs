using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
    public partial class Reminder
    {
    }

    public partial class Reminders
    {

        public void LoadByUser(int userID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Reminders WHERE (UserID = @UserID) AND (IsDismissed = 0) ORDER BY DueDate";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", userID);
                Fill(command);
            }
        }

        public void LoadByTicketID(int ticketID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"SELECT * 
                                        FROM Reminders as R
                                        INNER JOIN TaskAssociations as TA ON R.ReminderID = TA.ReminderID
                                        WHERE TA.RefType = 17 AND TA.RefID = @TicketID  ORDER BY DueDate";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketID", ticketID);
                Fill(command);
            }

        }

        public void LoadByUserMonth(DateTime date, int userID, string Type, string ID)
        {
            string additional = "";
            string userStr = "(UserID = @UserID)";

            if (Type == "9")
            {
                additional = "and (refid in (select TicketID from OrganizationTickets where OrganizationID = @id) and RefType = 17) or (RefType = @type and RefID = @ID)";
                userStr = "1=1";
            }

            if (Type == "4")
            {
                additional = "and (refid in (select TicketID from Tickets where GroupID = @id) and RefType = 17)";
                userStr = "1=1";
            }

            if (Type == "0")
            {
                additional = "AND (Reftype = @type) AND (RefID = @id)";
                userStr = "1=1";
            }


            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = string.Format("SELECT * FROM Reminders WHERE {0} AND (IsDismissed = 0) AND (Month(Duedate) = @month) AND (Year(Duedate) = @year) {1} ORDER BY DueDate", userStr, additional);
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@month", date.Month);
                command.Parameters.AddWithValue("@year", date.Year);
                command.Parameters.AddWithValue("@type", Type);
                command.Parameters.AddWithValue("@id", ID);
                Fill(command);
            }
        }

        public void LoadByItem(ReferenceType refType, int refID, int? userID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Reminders WHERE (UserID = @UserID OR @UserID < 0) AND (IsDismissed = 0) AND (RefType = @RefType) AND (RefID = @RefID) ORDER BY DueDate";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", userID ?? -1);
                command.Parameters.AddWithValue("@RefType", refType);
                command.Parameters.AddWithValue("@RefID", refID);
                Fill(command);
            }
        }

        public void LoadByItemAll(ReferenceType refType, int refID, int? userID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Reminders WHERE (IsDismissed = 0) AND (RefType = @RefType) AND (RefID = @RefID) ORDER BY DueDate";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", userID ?? -1);
                command.Parameters.AddWithValue("@RefType", refType);
                command.Parameters.AddWithValue("@RefID", refID);
                Fill(command);
            }
        }

        public void LoadForEmail()
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
            SELECT 
                *
            FROM 
                Reminders 
            WHERE 
                HasEmailSent = 0
                AND IsDismissed = 0
                AND UserID IS NOT NULL
                AND DueDate <= GETUTCDATE()
            ORDER BY
                DueDate";
                command.CommandType = CommandType.Text;
                Fill(command);
            }
        }

        //Tasks
        //Inspired by SearchService.GetAllCompaniesAndContacts
        public void LoadCreatedByUser(int from, int count, int userID, bool searchPending, bool searchComplete)
        {
            //Remember this will change once we add the isComplete field
            string pendingQuery = @"
            SELECT 
                *
            FROM
                Reminders
            WHERE
                CreatorID = @UserID 
                AND UserID <> @UserID
                AND TaskIsComplete = 0";

            string completeQuery = @"
            SELECT 
                *
            FROM
                Reminders
            WHERE
                CreatorID = @UserID 
                AND UserID <> @UserID
                AND TaskIsComplete = 1 ";

            string pageQuery = @"
            WITH 
                q AS ({0}),
                r AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY CASE WHEN TaskDueDate IS NULL THEN 1 ELSE 0 END, TaskIsComplete ASC, TaskDateCompleted DESC, TaskDueDate, DueDate) AS 'RowNum' FROM q)
            SELECT
                ReminderID
                , OrganizationID
                , RefType
                , RefID
                , Description
                , DueDate
                , UserID
                , IsDismissed
                , HasEmailSent
                , CreatorID
                , DateCreated
                , TaskName
                , TaskDueDate
                , TaskIsComplete
                , TaskDateCompleted
                , TaskParentID
            FROM 
                r
            WHERE
                RowNum BETWEEN @From AND @To";

            //User user = Users.GetUser(loginUser, loginUser.UserID);
            //if (user.TicketRights == TicketRightType.Customers)
            //{
            //    companyQuery = companyQuery + " AND o.OrganizationID IN (SELECT OrganizationID FROM UserRightsOrganizations WHERE UserID = " + user.UserID.ToString() + ")";
            //    contactQuery = contactQuery + " AND u.OrganizationID IN (SELECT OrganizationID FROM UserRightsOrganizations WHERE UserID = " + user.UserID.ToString() + ")";
            //}

            //if (active != null)
            //{
            //    companyQuery = companyQuery + " AND o.IsActive = @IsActive";
            //    contactQuery = contactQuery + " AND u.IsActive = @IsActive";
            //    command.Parameters.AddWithValue("@IsActive", (bool)active);
            //}

            StringBuilder query;

            query = new StringBuilder(string.Format(pageQuery, pendingQuery));

            //    if (searchPending && searchComplete)
            //{
            //    query = new StringBuilder(string.Format(pageQuery, pendingQuery + " UNION ALL " + completeQuery));
            //}
            //else if (searchPending)
            //{
            //    query = new StringBuilder(string.Format(pageQuery, pendingQuery));
            //}
            //else
            //{
            //    query = new StringBuilder(string.Format(pageQuery, completeQuery));
            //}

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

        public void LoadAssignedToUser(int from, int count, int userID, bool searchPending, bool searchComplete)
        {
            //Remember this will change once we add the isComplete field
            string pendingQuery = @"
            SELECT 
                *
            FROM
                Reminders
            WHERE
                UserID = @UserID
                AND TaskIsComplete = 0";

            string completeQuery = @"
            SELECT 
                *
            FROM
                Reminders
            WHERE
                UserID = @UserID
                AND TaskIsComplete = 1 ";

            string pageQuery = @"
            WITH 
                q AS ({0}),
                r AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY CASE WHEN TaskDueDate IS NULL THEN 1 ELSE 0 END, TaskIsComplete ASC, TaskDueDate ASC) AS 'RowNum' FROM q)
            SELECT
                ReminderID
                , OrganizationID
                , RefType
                , RefID
                , Description
                , DueDate
                , UserID
                , IsDismissed
                , HasEmailSent
                , CreatorID
                , DateCreated
                , TaskName
                , TaskDueDate
                , TaskIsComplete
                , TaskDateCompleted
                , TaskParentID
            FROM 
                r
            WHERE
                RowNum BETWEEN @From AND @To";

            //User user = Users.GetUser(loginUser, loginUser.UserID);
            //if (user.TicketRights == TicketRightType.Customers)
            //{
            //    companyQuery = companyQuery + " AND o.OrganizationID IN (SELECT OrganizationID FROM UserRightsOrganizations WHERE UserID = " + user.UserID.ToString() + ")";
            //    contactQuery = contactQuery + " AND u.OrganizationID IN (SELECT OrganizationID FROM UserRightsOrganizations WHERE UserID = " + user.UserID.ToString() + ")";
            //}

            //if (active != null)
            //{
            //    companyQuery = companyQuery + " AND o.IsActive = @IsActive";
            //    contactQuery = contactQuery + " AND u.IsActive = @IsActive";
            //    command.Parameters.AddWithValue("@IsActive", (bool)active);
            //}

            StringBuilder query;

            query = new StringBuilder(string.Format(pageQuery, pendingQuery));

            //    if (searchPending && searchComplete)
            //{
            //    query = new StringBuilder(string.Format(pageQuery, pendingQuery + " UNION ALL " + completeQuery));
            //}
            //else if (searchPending)
            //{
            //    query = new StringBuilder(string.Format(pageQuery, pendingQuery));
            //}
            //else
            //{
            //    query = new StringBuilder(string.Format(pageQuery, completeQuery));
            //}

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
            //Remember this will change once we add the isComplete field
            //string pendingQuery = @"
            //SELECT 
            //    *
            //FROM
            //    Reminders
            //WHERE
            //    CreatorID = @UserID 
            //    OR UserID <> @UserID
            //    AND TaskIsComplete = 1 ";

            string completeQuery = @"
            SELECT 
                *
            FROM
                Reminders
            WHERE
                (CreatorID = @UserID 
                OR UserID = @UserID)
                AND TaskIsComplete = 1 ";

            string pageQuery = @"
            WITH 
                q AS ({0}),
                r AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY TaskIsComplete ASC, TaskDateCompleted DESC, TaskDueDate, DueDate) AS 'RowNum' FROM q)
            SELECT
                ReminderID
                , OrganizationID
                , RefType
                , RefID
                , Description
                , DueDate
                , UserID
                , IsDismissed
                , HasEmailSent
                , CreatorID
                , DateCreated
                , TaskName
                , TaskDueDate
                , TaskIsComplete
                , TaskDateCompleted
                , TaskParentID
            FROM 
                r
            WHERE
                RowNum BETWEEN @From AND @To";

            //User user = Users.GetUser(loginUser, loginUser.UserID);
            //if (user.TicketRights == TicketRightType.Customers)
            //{
            //    companyQuery = companyQuery + " AND o.OrganizationID IN (SELECT OrganizationID FROM UserRightsOrganizations WHERE UserID = " + user.UserID.ToString() + ")";
            //    contactQuery = contactQuery + " AND u.OrganizationID IN (SELECT OrganizationID FROM UserRightsOrganizations WHERE UserID = " + user.UserID.ToString() + ")";
            //}

            //if (active != null)
            //{
            //    companyQuery = companyQuery + " AND o.IsActive = @IsActive";
            //    contactQuery = contactQuery + " AND u.IsActive = @IsActive";
            //    command.Parameters.AddWithValue("@IsActive", (bool)active);
            //}

            StringBuilder query;

            //if (searchPending && searchComplete)
            //{
            //    query = new StringBuilder(string.Format(pageQuery, pendingQuery + " UNION ALL " + completeQuery));
            //}
            //else if (searchPending)
            //{
            //    query = new StringBuilder(string.Format(pageQuery, pendingQuery));
            //}
            //else
            //{
            query = new StringBuilder(string.Format(pageQuery, completeQuery));
            //}

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

        public void LoadIncompleteByParentID(int parentID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Reminders WHERE TaskParentID = @ParentID AND TaskIsComplete = 0";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ParentID", parentID);
                Fill(command);
            }
        }

        public void LoadIncompleteAssociatedToCompany(int from, int count, int organizationID)
        {
            string completeQuery = @"
            SELECT 
                rem.*
            FROM
                Reminders rem
                JOIN TaskAssociations ta
                    ON rem.ReminderID = ta.ReminderID
            WHERE
                rem.TaskIsComplete = 0
                AND ta.RefType = 9
                AND ta.RefID = @organizationID";

            string pageQuery = @"
            WITH 
                q AS ({0}),
                r AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY TaskDueDate, DueDate) AS 'RowNum' FROM q)
            SELECT
                ReminderID
                , OrganizationID
                , RefType
                , RefID
                , Description
                , DueDate
                , UserID
                , IsDismissed
                , HasEmailSent
                , CreatorID
                , DateCreated
                , TaskName
                , TaskDueDate
                , TaskIsComplete
                , TaskDateCompleted
                , TaskParentID
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
                command.Parameters.AddWithValue("@organizationID", organizationID);
                command.Parameters.AddWithValue("@From", from + 1);
                command.Parameters.AddWithValue("@To", from + count);
                Fill(command);
            }
        }

        public void LoadCompleteAssociatedToCompany(int from, int count, int organizationID)
        {
            string completeQuery = @"
            SELECT 
                rem.*
            FROM
                Reminders rem
                JOIN TaskAssociations ta
                    ON rem.ReminderID = ta.ReminderID
            WHERE
                rem.TaskIsComplete = 1
                AND ta.RefType = 9
                AND ta.RefID = @organizationID";

            string pageQuery = @"
            WITH 
                q AS ({0}),
                r AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY TaskDueDate, DueDate) AS 'RowNum' FROM q)
            SELECT
                ReminderID
                , OrganizationID
                , RefType
                , RefID
                , Description
                , DueDate
                , UserID
                , IsDismissed
                , HasEmailSent
                , CreatorID
                , DateCreated
                , TaskName
                , TaskDueDate
                , TaskIsComplete
                , TaskDateCompleted
                , TaskParentID
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
                rem.*
            FROM
                Reminders rem
                JOIN TaskAssociations ta
                    ON rem.ReminderID = ta.ReminderID
            WHERE
                (rem.CreatorID = @UserID OR rem.UserID = @UserID)
                AND ta.RefType = 9
                AND ta.RefID = @organizationID";

            string pageQuery = @"
            WITH 
                q AS ({0}),
                r AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY TaskDueDate, DueDate) AS 'RowNum' FROM q)
            SELECT
                ReminderID
                , OrganizationID
                , RefType
                , RefID
                , Description
                , DueDate
                , UserID
                , IsDismissed
                , HasEmailSent
                , CreatorID
                , DateCreated
                , TaskName
                , TaskDueDate
                , TaskIsComplete
                , TaskDateCompleted
                , TaskParentID
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
                command.Parameters.AddWithValue("@organizationID", organizationID);
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@From", from + 1);
                command.Parameters.AddWithValue("@To", from + count);
                Fill(command);
            }
        }
    }

}
