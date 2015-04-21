using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using System.Collections;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Text;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Runtime.Serialization;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace TSWebServices
{
    [ScriptService]
    [WebService(Namespace = "http://teamsupport.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class TicketPageService : System.Web.Services.WebService
    {

        public TicketPageService() { }

        [WebMethod]
        public TicketPageInfo GetTicketInfo(int ticketNumber)
        {
            TicketsViewItem ticket = TicketsView.GetTicketsViewItemByNumber(TSAuthentication.GetLoginUser(), ticketNumber);
            if (ticket == null) return null;
            if (ticket.OrganizationID != TSAuthentication.OrganizationID) return null;

            User user = Users.GetUser(ticket.Collection.LoginUser, TSAuthentication.UserID);
            if (!ticket.UserHasRights(user)) return null;

            TicketService ticketService = new TicketService();
            ticketService.MarkTicketAsRead(ticket.TicketID);

            TicketPageInfo info = new TicketPageInfo();
            info.Ticket = ticket.GetProxy();

            if (info.Ticket.Name.ToLower() == "<no subject>")
                info.Ticket.Name = "";

            if (info.Ticket.CategoryName != null)
                info.Ticket.CategoryDisplayString = ForumCategories.GetCategoryDisplayString(TSAuthentication.GetLoginUser(), (int)info.Ticket.ForumCategory);
            if (info.Ticket.KnowledgeBaseCategoryName != null)
                info.Ticket.KnowledgeBaseCategoryDisplayString = KnowledgeBaseCategories.GetKnowledgeBaseCategoryDisplayString(TSAuthentication.GetLoginUser(), (int)info.Ticket.KnowledgeBaseCategoryID);
            info.Customers = ticketService.GetTicketCustomers(ticket.TicketID);
            info.Related = ticketService.GetRelatedTickets(ticket.TicketID);
            info.Tags = ticketService.GetTicketTags(ticket.TicketID);
            info.CustomValues = ticketService.GetParentCustomValues(ticket.TicketID);
            info.Subscribers = GetSubscribers(ticket);
            info.Queuers = GetQueuers(ticket);

            Reminders reminders = new Reminders(ticket.Collection.LoginUser);
            reminders.LoadByItem(ReferenceType.Tickets, ticket.TicketID, TSAuthentication.UserID);
            info.Reminders = reminders.GetReminderProxies();

            Assets assets = new Assets(ticket.Collection.LoginUser);
            assets.LoadByTicketID(ticket.TicketID);
            info.Assets = assets.GetAssetProxies();

            info.LinkToJira = GetLinkToJira(ticket.TicketID);

            return info;
        }

        [WebMethod]
        public TicketPropertySelectField[] GetTicketUsers(int ticketID)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Organization organization = TSAuthentication.GetOrganization(loginUser);

            List<TicketPropertySelectField> result = new List<TicketPropertySelectField>();
            Ticket ticket = Tickets.GetTicket(loginUser, ticketID);

            UsersView sender = new UsersView(TSAuthentication.GetLoginUser());
            sender.LoadLastSenderByTicketNumber(TSAuthentication.OrganizationID, ticketID);

            List<User> ticketUsers = new List<TeamSupport.Data.User>();

            if (ticket.GroupID != null && organization.ShowGroupMembersFirstInTicketAssignmentList)
            {
                Users groupUsers = new Users(loginUser);
                groupUsers.LoadByGroupID((int)ticket.GroupID);
                ticketUsers.AddRange(groupUsers);
            }

            Users users = new Users(TSAuthentication.GetLoginUser());
            users.LoadByOrganizationID(TSAuthentication.OrganizationID, true);

            ticketUsers.AddRange(users.Where(p => !ticketUsers.Any(p2 => p2.UserID == p.UserID)));

            foreach (User user in ticketUsers)
            {
                TicketPropertySelectField selectUser = new TicketPropertySelectField();
                selectUser.ID = user.UserID;

                string displayName = user.DisplayName + " ";

                if (!string.IsNullOrEmpty(user.InOfficeComment))
                {
                    displayName = string.Format("{0} - {1} ", displayName, user.InOfficeComment);
                }

                string postLine = "";

                if (ticket.CreatorID == user.UserID)
                {
                    postLine = "(Sender";
                }

                if (sender.Count > 0 && sender[0].UserID == user.UserID)
                {
                    if (postLine.Length > 0)
                    {
                        postLine += " and Creator";
                    }
                    else
                    {
                        postLine = "(Creator";
                    }
                }

                if (postLine.Length > 0)
                {
                    postLine += ")";
                }

                selectUser.Name = displayName + postLine;

                if (ticket.UserID == user.UserID)
                {
                    selectUser.IsSelected = true;
                }

                result.Add(selectUser);
            }

            return result.ToArray();
        }

        [WebMethod]
        public TicketPropertySelectField[] GetTicketGroups(int ticketID)
        {
            Tickets ticket = new Tickets(TSAuthentication.GetLoginUser());
            ticket.LoadByTicketID(ticketID);

            if (!ticket.IsEmpty && ticket[0].UserID != null)
            {
                int userID = (int)ticket[0].UserID;
                List<GroupProxy> groupProxies = new List<GroupProxy>();

                Groups orgGroups = new Groups(TSAuthentication.GetLoginUser());
                orgGroups.LoadByNotUserID(userID, TSAuthentication.OrganizationID);

                Groups userGroups = new Groups(TSAuthentication.GetLoginUser());
                userGroups.LoadByUserID(userID);

                groupProxies.AddRange(userGroups.GetGroupProxies().OrderBy(ug => ug.Name));
                groupProxies.AddRange(orgGroups.GetGroupProxies().OrderBy(og => og.Name));

                return groupProxies.Select(g => new TicketPropertySelectField
                {
                    ID = g.GroupID,
                    Name = g.Name,
                    IsSelected = (g.GroupID == ticket[0].GroupID)
                }).ToArray();
            }
            else
            {
                Groups groups = new Groups(TSAuthentication.GetLoginUser());
                groups.LoadByOrganizationID(TSAuthentication.OrganizationID);
                return groups.Select(g => new TicketPropertySelectField
                {
                    ID = g.GroupID,
                    Name = g.Name,
                    IsSelected = false
                }).ToArray();
            }
        }

        [WebMethod]
        public TimeLineItem[] GetTimeLineItems(int ticketID, int from)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            List<TimeLineItem> timeLineItems = new List<TimeLineItem>();

            TicketTimeLineView TimeLineView = new TicketTimeLineView(loginUser);
            TimeLineView.LoadByRange(ticketID, from, from + 9);

            foreach (TicketTimeLineViewItem viewItem in TimeLineView)
            {
                if (!viewItem.IsWC)
                {
                    Attachments attachments = new Attachments(loginUser);
                    attachments.LoadByActionID(viewItem.RefID);

                    TimeLineItem timeLineItem = new TimeLineItem();
                    timeLineItem.item = viewItem.GetProxy();
                    timeLineItem.item.Message = SanitizeMessage(timeLineItem.item.Message);
                    timeLineItem.Attachments = attachments.GetAttachmentProxies();

                    timeLineItems.Add(timeLineItem);
                }
                else
                {
                    TimeLineItem wcItem = new TimeLineItem();
                    wcItem.item = viewItem.GetProxy();

                    WatercoolerMsg replies = new WatercoolerMsg(loginUser);
                    replies.LoadReplies(viewItem.RefID);

                    WatercoolerLikes likes = new WatercoolerLikes(loginUser);
                    likes.LoadByMessageID(viewItem.RefID);

                    wcItem.Likes = likes.Count();

                    wcItem.DidLike = (likes.Where(l => l.UserID == loginUser.UserID).Count() > 0);

                    List<WaterCoolerReply> wcReplies = new List<WaterCoolerReply>();

                    foreach (WatercoolerMsgItem reply in replies)
                    {
                        WaterCoolerReply replyItem = new WaterCoolerReply();
                        replyItem.WaterCoolerReplyProxy = reply.GetProxy();

                        WatercoolerLikes replyLikes = new WatercoolerLikes(loginUser);
                        replyLikes.LoadByMessageID(reply.MessageID);

                        replyItem.Likes = replyLikes.Count();

                        replyItem.DidLike = (replyLikes.Where(rl => rl.UserID == loginUser.UserID).Count() > 0);

                        wcReplies.Add(replyItem);
                    }

                    wcItem.WaterCoolerReplies = wcReplies.ToArray();
                    timeLineItems.Add(wcItem);
                }
            }

            return timeLineItems.ToArray();
        }

        [WebMethod]
        public TimeLineItem RequestUpdate(int ticketID)
        {
          TicketsViewItem ticket = TicketsView.GetTicketsViewItem(TSAuthentication.GetLoginUser(), ticketID);
          if (ticket == null) return null;
          EmailPosts.SendTicketUpdateRequest(TSAuthentication.GetLoginUser(), ticketID);
          User user = TSAuthentication.GetUser(TSAuthentication.GetLoginUser());
          TeamSupport.Data.Action action = (new Actions(TSAuthentication.GetLoginUser())).AddNewAction();
          action.ActionTypeID = null;
          action.Name = "Update Requested";
          action.ActionSource = "UpdateRequest";
          action.SystemActionTypeID = SystemActionType.UpdateRequest;
          action.Description = String.Format("<p>{0} requested an update for this ticket.</p>", user.FirstName);
          action.IsVisibleOnPortal = false;
          action.IsKnowledgeBase = false;
          action.TicketID = ticket.TicketID;
          action.CreatorID = TSAuthentication.UserID;
          action.Collection.Save();


          string description = String.Format("{0} requested an update from {1} for {2}", user.FirstLastName, ticket.UserName, Tickets.GetTicketLink(TSAuthentication.GetLoginUser(), ticketID));
          ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);

          return GetActionTimelineItem(action);
        }

        [WebMethod]
        public int GetActionCount(int ticketID)
        {
            return Actions.GetTicketActionCount(TSAuthentication.GetLoginUser(), ticketID);
        }

        [WebMethod]
        public void SaveTicketPageOrder(string keyname, string data)
        {
            Settings.OrganizationDB.WriteString(keyname, data);
        }

        [WebMethod]
        public TicketCategoryOrder[] GetTicketPageOrder(string KeyName)
        {
            string defaultOrder = "[{\"CatID\":\"AssignedTo\",\"CatName\":\"Assigned To\"},{\"CatID\":\"Group\",\"CatName\":\"Group\"},{\"CatID\":\"Type\",\"CatName\":\"Type\"},{\"CatID\":\"Status\",\"CatName\":\"Status\"},{\"CatID\":\"Severity\",\"CatName\":\"Severity\"},{\"CatID\":\"SLAStatus\",\"CatName\":\"SLA Status\"},{\"CatID\":\"VisibleToCustomers\",\"CatName\":\"Visible To Customers\"},{\"CatID\":\"KnowledgeBase\",\"CatName\":\"Knowledge Base\"},{\"CatID\":\"Community\",\"CatName\":\"Community\"},{\"CatID\":\"DaysOpened\",\"CatName\":\"Days Opened\"},{\"CatID\":\"TotalTimeSpent\",\"CatName\":\"Total Time Spent\"},{\"CatID\":\"DueDate\",\"CatName\":\"Due Date\"},{\"CatID\":\"Customers\",\"CatName\":\"Customers\"},{\"CatID\":\"CustomFields\",\"CatName\":\"Custom Fields\"},{\"CatID\":\"Product\",\"CatName\":\"Product\"},{\"CatID\":\"Inventory\",\"CatName\":\"Inventory\"},{\"CatID\":\"Tags\",\"CatName\":\"Tags\"},{\"CatID\":\"Reminders\",\"CatName\":\"Reminders\"},{\"CatID\":\"AssociatedTickets\",\"CatName\":\"Associated Tickets\"},{\"CatID\":\"UserQueue\",\"CatName\":\"User Queue\"},{\"CatID\":\"SubscribedUsers\",\"CatName\":\"Subscribed Users\"}]";
            List<TicketCategoryOrder> items = JsonConvert.DeserializeObject<List<TicketCategoryOrder>>(Settings.OrganizationDB.ReadString(KeyName, defaultOrder));

            return items.ToArray();
        }

        [WebMethod]
        public AutocompleteItem[] GetUserOrOrganizationForTicket(string searchTerm)
        {
            User user = TSAuthentication.GetUser(TSAuthentication.GetLoginUser());
            return GetUserOrOrganizationFiltered(searchTerm, !user.AllowAnyTicketCustomer);
        }

        [WebMethod]
        public TimeLineItem UpdateAction(ActionProxy proxy)
        {
            TeamSupport.Data.Action action = Actions.GetActionByID(TSAuthentication.GetLoginUser(), proxy.ActionID);
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), TSAuthentication.UserID);

            if (action == null)
            {
                action = (new Actions(TSAuthentication.GetLoginUser())).AddNewAction();
                action.TicketID = proxy.TicketID;
                action.CreatorID = TSAuthentication.UserID;
                if (!string.IsNullOrWhiteSpace(user.Signature) && proxy.IsVisibleOnPortal)
                {
                    if (!proxy.Description.Contains(user.Signature))
                    {
                        action.Description = proxy.Description + "<br/><br/>" + user.Signature;
                    }
                    else
                    {
                        action.Description = proxy.Description;
                    }
                }
                else
                {
                    action.Description = proxy.Description;
                }
            }
            else
            {
                if (proxy.IsVisibleOnPortal)
                {
                    if (!string.IsNullOrWhiteSpace(user.Signature))
                    {
                        if (!action.Description.Contains(user.Signature.Replace(" />", ">")))
                        {
                            action.Description = proxy.Description + "<br/><br/>" + user.Signature;
                        }
                        else
                        {
                            action.Description = proxy.Description;
                        }
                    }
                    else
                    {
                        action.Description = proxy.Description;
                    }
                }
                else
                {
                    action.Description = proxy.Description;
                }
            }

            if (!CanEditAction(action)) return null;


            action.ActionTypeID = proxy.ActionTypeID;
            action.DateStarted = proxy.DateStarted;
            action.TimeSpent = proxy.TimeSpent;
            action.IsKnowledgeBase = proxy.IsKnowledgeBase;
            action.IsVisibleOnPortal = proxy.IsVisibleOnPortal;
            action.Collection.Save();

            return GetActionTimelineItem(action);
        }

        public AutocompleteItem[] GetUserOrOrganizationFiltered(string searchTerm, bool filterByUserRights)
        {
            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
            organizations.LoadByLikeOrganizationName(TSAuthentication.OrganizationID, searchTerm, true, 50, filterByUserRights);

            UsersView users = new UsersView(organizations.LoginUser);
            users.LoadByLikeName(TSAuthentication.OrganizationID, searchTerm, 50, true, true);

            List<AutocompleteItem> list = new List<AutocompleteItem>();
            for (int i = 0; i < organizations.Count; i++)
            {
                list.Add(new AutocompleteItem(organizations[i].Name, organizations[i].OrganizationID.ToString(), "o"));
            }

            for (int i = 0; i < users.Count; i++)
            {
                list.Add(new AutocompleteItem
                {
                    label = String.Format("<strong>{0} {1}</strong></br>{1}", users[i].FirstName, users[i].LastName, users[i].Organization),
                    id = users[i].UserID.ToString(),
                    data = "u",
                    value = users[i].FirstName + " " + users[i].LastName
                });
            }

            return list.ToArray();
        }

        [WebMethod]
        public AttachmentProxy[] GetActionAttachments(int ActionID)
        {
            return GetActionAttachments(ActionID, TSAuthentication.GetLoginUser());
        }

        [WebMethod]
        public AutocompleteItem[] SearchUsers(string term)
        {
            List<AutocompleteItem> result = new List<AutocompleteItem>();
            Users users = new Users(TSAuthentication.GetLoginUser());
            users.LoadByName(term, TSAuthentication.OrganizationID, true, false, false);

            for (int i = 0; i < users.Count(); i++)
            {
                result.Add(new AutocompleteItem(users[i].DisplayName, users[i].UserID.ToString()));
            }
            return result.OrderBy(r => r.label).ToArray();
        }
        [WebMethod]
        public ProductProxy[] GetTicketCustomerProducts(int ticketID)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            Organizations organizations = new Organizations(ticket.Collection.LoginUser);
            organizations.LoadByTicketID(ticketID);
            if (organizations.IsEmpty) return null;

            List<int> organizationIDs = new List<int>();
            foreach (Organization organization in organizations)
            {
                organizationIDs.Add(organization.OrganizationID);
            }
            Products products = new Products(ticket.Collection.LoginUser);
            products.LoadByCustomerIDs(organizationIDs.ToArray());

            if (products.IsEmpty) return null;

            return products.GetProductProxies();
        }

        [WebMethod]
        public string GetActionTicketTemplate(int actionTypeId)
        {
            TicketTemplate template = TicketTemplates.GetByActionType(TSAuthentication.GetLoginUser(), actionTypeId);
            if (template == null) return "";
            if (template.OrganizationID != TSAuthentication.OrganizationID) return "";
            return template.TemplateText;
        }


        //TODO Move this down

        [DataContract]
        public class TicketPageInfo
        {
            [DataMember]
            public TicketsViewItemProxy Ticket { get; set; }
            [DataMember]
            public TicketCustomer[] Customers { get; set; }
            [DataMember]
            public RelatedTicket[] Related { get; set; }
            [DataMember]
            public TagProxy[] Tags { get; set; }
            [DataMember]
            public CustomValueProxy[] CustomValues { get; set; }
            [DataMember]
            public UserInfo[] Subscribers { get; set; }
            [DataMember]
            public UserInfo[] Queuers { get; set; }
            [DataMember]
            public ReminderProxy[] Reminders { get; set; }
            [DataMember]
            public AssetProxy[] Assets { get; set; }
            [DataMember]
            public TicketLinkToJiraItemProxy LinkToJira { get; set; }
        }

        [DataContract]
        public class TicketPropertySelectField
        {
            [DataMember]
            public int ID { get; set; }
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public bool IsSelected { get; set; }
        }

        [DataContract]
        public class TimeLineItem
        {
            [DataMember]
            public TicketTimeLineViewItemProxy item { get; set; }
            [DataMember]
            public int? Likes { get; set; }
            [DataMember]
            public bool DidLike { get; set; }
            [DataMember]
            public AttachmentProxy[] Attachments { get; set; }
            [DataMember]
            public WaterCoolerReply[] WaterCoolerReplies { get; set; }
        }

        [DataContract]
        public class WaterCoolerReply
        {
            [DataMember]
            public WatercoolerMsgItemProxy WaterCoolerReplyProxy { get; set; }
            [DataMember]
            public int Likes { get; set; }
            [DataMember]
            public bool DidLike { get; set; }
        }


        [DataContract(Namespace = "http://teamsupport.com/")]
        public class TicketCategoryOrder
        {
            [DataMember]
            public string CatID { get; set; }
            [DataMember]
            public string CatName { get; set; }
        }

        //Private Methods

        private string SanitizeMessage(string message)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            return HtmlUtility.TagHtml(loginUser, HtmlUtility.Sanitize(HtmlUtility.CheckScreenR(loginUser, message)));
        }

        private bool CanEditAction(TeamSupport.Data.Action action)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), action.TicketID);
            User user = TSAuthentication.GetUser(TSAuthentication.GetLoginUser());
            return (ticket.OrganizationID == TSAuthentication.OrganizationID && (action.CreatorID == TSAuthentication.UserID || TSAuthentication.IsSystemAdmin || user.AllowUserToEditAnyAction));
        }

        private TicketLinkToJiraItemProxy GetLinkToJira(int ticketID)
        {
            TicketLinkToJiraItemProxy result = null;
            TicketLinkToJira linkToJira = new TicketLinkToJira(TSAuthentication.GetLoginUser());
            linkToJira.LoadByTicketID(ticketID);
            if (linkToJira.Count > 0)
            {
                result = linkToJira[0].GetProxy();
            }
            return result;
        }

        private UserInfo[] GetSubscribers(TicketsViewItem ticket)
        {
            UsersView users = new UsersView(ticket.Collection.LoginUser);
            users.LoadBySubscription(ticket.TicketID, ReferenceType.Tickets);
            List<UserInfo> result = new List<UserInfo>();
            foreach (UsersViewItem user in users)
            {
                result.Add(new UserInfo(user));
            }
            return result.ToArray();
        }

        private UserInfo[] GetQueuers(TicketsViewItem ticket)
        {
            UsersView users = new UsersView(ticket.Collection.LoginUser);
            users.LoadByTicketQueue(ticket.TicketID);
            List<UserInfo> result = new List<UserInfo>();
            foreach (UsersViewItem user in users)
            {
                result.Add(new UserInfo(user));
            }
            return result.ToArray();
        }

        private TimeLineItem GetActionTimelineItem(TeamSupport.Data.Action action)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            TimeLineItem item = new TimeLineItem();
            TicketTimeLineViewItemProxy itemProxy = new TicketTimeLineViewItemProxy
            {
                TicketID = action.TicketID,
                RefID = action.ActionID,
                IsWC = false,
                MessageType = action.ActionTypeName,
                Message = SanitizeMessage(action.Description),
                DateCreated = action.DateCreated,
                OrganizationID = TSAuthentication.OrganizationID,
                CreatorID = action.CreatorID,
                CreatorName = loginUser.GetUserFullName(),
                IsKnowledgeBase = action.IsKnowledgeBase,
                IsVisibleOnPortal = action.IsVisibleOnPortal,
                IsPinned = action.Pinned,
                WCUserID = null
            };

            item.item = itemProxy;

            Attachments attachments = new Attachments(loginUser);
            attachments.LoadByActionID(item.item.RefID);
            item.Attachments = GetActionAttachments(action.ActionID, loginUser);

            return item;
        }

        private AttachmentProxy[] GetActionAttachments(int actionID, LoginUser loginUser)
        {
            Attachments attachments = new Attachments(loginUser);
            attachments.LoadByActionID(actionID);
            return attachments.GetAttachmentProxies();
        }
    }
}