using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamSupport.IDTree;
using TeamSupport.Data;
using TeamSupport.DataAPI;
using TeamSupport.ModelAPI;

namespace TeamSupport.UnitTest
{
    [TestClass]
    public class AttachmentTest
    {
        public const string _userScot = "4787299|1078|0|51dab274-5d73-4f56-8df9-da97d20cdc5b|1";
        public const string _connectionString = "Application Name=App;Data Source=dev-sql.corp.teamsupport.com; Initial Catalog=TeamSupportNightly;Persist Security Info=True;User ID=Dev-Sql-WebApp;Password=TeamSupportDev;Connect Timeout=500;";

        [TestMethod]
        public void ActionAttachments()
        {
            string userData = _userScot;
            AuthenticationModel authentication = AuthenticationModel.AuthenticationModelTest(userData, _connectionString);
            using (ConnectionContext connection = new ConnectionContext(authentication))
            {
                // user ticket
                TicketProxy ticketProxy = IDTreeTest.EmptyTicket(); // from front end
                TicketModel ticketModel = (TicketModel)Data_API.Create(connection.User, ticketProxy);  // dbo.Tickets

                // ticket action
                ActionProxy actionProxy = IDTreeTest.EmptyAction(); // from front end
                ActionModel actionModel = (ActionModel)Data_API.Create(ticketModel, actionProxy);  // dbo.Actions

                // action attachment
                ActionAttachmentProxy attachmentProxy = (ActionAttachmentProxy)IDTreeTest.CreateAttachment(connection.OrganizationID, 
                    AttachmentProxy.References.Actions, actionModel.ActionID, actionModel.AttachmentPath);
                AttachmentModel attachmentModel = (AttachmentModel)Data_API.Create(actionModel, attachmentProxy);

                // read back attachment
                AttachmentProxy read = Data_API.ReadRefTypeProxy<AttachmentProxy>(connection, attachmentProxy.AttachmentID);
                switch (read)
                {
                    case ActionAttachmentProxy output:  // Action attachment
                        Assert.AreEqual(attachmentProxy, output);
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            }
        }
    }
}
