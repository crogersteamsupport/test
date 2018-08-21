using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;

namespace TeamSupport.Model
{
    /// <summary>
    /// ultralight read-only interface to database for verifying primary keys
    /// </summary>
    static class DBReader
    {
        // Verify correctness as we traverse the layers of the logical model
        public static void VerifyOrganization(DataContext db, int organizationID) { Verify(db, $"SELECT OrganizationID FROM Organizations WITH (NOLOCK) WHERE OrganizationID={organizationID}"); }
        public static void VerifyUser(DataContext db, int organizationID, int userID) { Verify(db, $"SELECT UserID FROM Users WITH (NOLOCK) WHERE UserID={userID} AND OrganizationID={organizationID}"); }
        public static void VerifyTicket(DataContext db, int organizationID, int ticketID) { Verify(db, $"SELECT TicketID FROM Tickets WITH (NOLOCK) WHERE TicketID={ticketID} AND OrganizationID={organizationID}"); }
        public static void VerifyAction(DataContext db, int organizationID, int ticketID, int actionID) { Verify(db, $"SELECT ActionID FROM Actions WITH (NOLOCK) WHERE ActionID={actionID} AND TicketID={ticketID}"); }
        public static void VerifyActionAttachment(DataContext db, int organizationID, int ticketID, int actionID, int actionAttachmentID) { Verify(db, $"SELECT ActionAttachmentID FROM ActionAttachments WITH (NOLOCK) WHERE ActionAttachmentID={actionAttachmentID} AND ActionID={actionID} AND OrganizationID={organizationID}"); }
        
        static void Verify(DataContext db, string query)
        {
            if (!db.ExecuteQuery<int>(query).Any()) // valid ID found?
                throw new Exception(String.Format($"{query} not found"));
        }

        public static bool UserAllowUserToEditAnyAction(DataContext db, int userID)
        {
            return db.ExecuteQuery<bool>($"SELECT AllowUserToEditAnyAction FROM Users WITH (NOLOCK) WHERE UserID={userID}").First();
        }

        public static string AttachmentPath(DataContext db, int id)
        {
            return db.ExecuteQuery<string>($"SELECT[Value] FROM FilePaths WITH(NOLOCK) WHERE ID = {id}").FirstOrDefault();
        }

        public static int CreatorID(DataContext db, int actionID)
        {
            return db.ExecuteQuery<int>($"SELECT CreatorID FROM Actions WITH (NOLOCK) WHERE ActionID={actionID}").Min();
        }

        public static int AttachmentStorageUsed(DataContext db, int organizationID)
        {
            return db.ExecuteQuery<int>($"SELECT SUM(a.FileSize) FROM Attachments a WITH (NOLOCK) WHERE (a.OrganizationID = {organizationID}").Min();
        }
        public static int TicketNumber(DataContext db, int id)
        {
            return db.ExecuteQuery<int>($"SELECT TicketNumber FROM Tickets WITH(NOLOCK) WHERE TicketId = {id}").First();
        }

    }
    }
