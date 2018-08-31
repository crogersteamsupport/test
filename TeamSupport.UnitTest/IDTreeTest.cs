﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamSupport.IDTree;
using TeamSupport.Data;
using TeamSupport.Proxy;
using TeamSupport.DataAPI;
using System.Configuration;
using System.Web.Configuration;
using System.Data.Linq;

namespace TeamSupport.UnitTest
{
    [TestClass]
    public class IDTreeTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            AuthenticationModel authentication = AuthenticationModel.AuthenticationModelTest("4787299|1078|0|51dab274-5d73-4f56-8df9-da97d20cdc5b|1",
                "Application Name=App;Data Source=dev-sql.corp.teamsupport.com; Initial Catalog=TeamSupportNightly;Persist Security Info=True;User ID=Dev-Sql-WebApp;Password=TeamSupportDev;Connect Timeout=500;");

            using (ConnectionContext connection = new ConnectionContext(authentication))
            {
                // validated user on organization
                OrganizationModel organization = connection.Organization;
                UserModel user = connection.User;

                for (int i = 0; i < 10; ++i)
                {
                    // new ticket
                    TicketProxy inTicketProxy = EmptyTicket();
                    TicketModel ticket = Data_API.Create(user, inTicketProxy) as TicketModel;  // user created a ticketD
                    TicketProxy outTicketProxy = Data_API.Read<TicketProxy>(ticket);
                    Assert.AreEqual(inTicketProxy, outTicketProxy);

                    for (int j = 0; j < 10; ++j)
                    {
                        // new action
                        ActionProxy inActionProxy = EmptyAction();
                        ActionModel action = Data_API.Create(ticket, inActionProxy) as ActionModel;
                        ActionProxy outActionProxy = Data_API.Read<ActionProxy>(action);
                        Assert.AreEqual(inActionProxy, outActionProxy);
                    }
                }
            }
        }

        public static TicketProxy EmptyTicket()
        {
            return new TicketProxy()
            {
                //TicketID
                ImportFileID = null,
                //EmailReplyToAddress = null,
                //JiraLinkURL = null,
                //JiraKey = null,
                //JiraID = null,
                //SyncWithJira = null,
                //DateModifiedByJiraSync = null,
                //JiraStatus = null,
                SalesForceID = null,
                DateModifiedBySalesForceSync = null,
                KnowledgeBaseCategoryID = null,
                //DueDate = null,
                ModifierID = 4787299,
                CreatorID = 4787299,
                DateModified = DateTime.UtcNow,
                DateCreated = DateTime.UtcNow,
                DocID = null,
                NeedsIndexing = false,
                SlaWarningInitialResponse = null,
                SlaWarningLastAction = null,
                SlaWarningTimeClosed = null,
                SlaViolationInitialResponse = null,
                SlaViolationLastAction = null,
                SlaViolationTimeClosed = null,
                PortalEmail = null,
                TicketSource = "Agent",
                LastWarningTime = null,
                LastViolationTime = null,
                ImportID = null,
                CloserID = null,
                DateClosed = null,
                IsKnowledgeBase = false,
                IsVisibleOnPortal = false,
                //TicketNumber = 56523,
                ParentID = null,
                Name = "sdfg",
                OrganizationID = 1078,
                TicketSeverityID = 12539,
                TicketTypeID = 1,
                TicketStatusID = 13757,
                UserID = 4787299,
                GroupID = 2079,
                ProductID = 243901,
                SolvedVersionID = null,
                ReportedVersionID = null,
                //Identity = p48 output
            };
        }

        public static ActionProxy EmptyAction()
        {
            return new ActionProxy()
            {
                ImportFileID = null,
                //IsClean = 0,
                Description = "<p style=\"font - size: 12pt; font - family: helvetica; \">asdfasdf</p>",
                Pinned = false,
                //JiraID = null,
                //DateModifiedByJiraSync = null,
                SalesForceID = null,
                DateModifiedBySalesForceSync = null,
                ActionSource = null,
                TicketID = -1,
                ModifierID = 4787299,
                CreatorID = 4787299,
                DateModified = DateTime.UtcNow,
                DateCreated = DateTime.UtcNow,
                ImportID = null,
                IsKnowledgeBase = false,
                IsVisibleOnPortal = true,
                DateStarted = DateTime.UtcNow,
                TimeSpent = 0,
                Name = "",
                SystemActionTypeID = 0,
                ActionTypeID = 2119
            };
        }
    }
}
