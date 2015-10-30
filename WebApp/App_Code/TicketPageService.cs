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
using System.Diagnostics;

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

            if (info.Ticket.CategoryName != null && info.Ticket.ForumCategory != null)
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
        public TicketPageUser[] GetTicketUsers(int ticketID)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Organization organization = TSAuthentication.GetOrganization(loginUser);

            List<TicketPageUser> result = new List<TicketPageUser>();
            Ticket ticket = Tickets.GetTicket(loginUser, ticketID);

            UsersView sender = new UsersView(TSAuthentication.GetLoginUser());
            sender.LoadLastSenderByTicketNumber(TSAuthentication.OrganizationID, ticketID);

            List<User> ticketUsers = new List<TeamSupport.Data.User>();
            bool hasSenderBeenAdded = false;

            if (ticket.GroupID != null && organization.ShowGroupMembersFirstInTicketAssignmentList)
            {
                Users groupUsers = new Users(loginUser);
                groupUsers.LoadByGroupID((int)ticket.GroupID);
                ticketUsers.AddRange(groupUsers);
            }

            Users users = new Users(TSAuthentication.GetLoginUser());
            users.LoadByOrganizationID(TSAuthentication.OrganizationID, true);

            ticketUsers.AddRange(users.Where(p => !ticketUsers.Any(p2 => p2.UserID == p.UserID)).OrderBy(tu => tu.DisplayName));

            foreach (User user in ticketUsers)
            {
              TicketPageUser selectUser = new TicketPageUser();
              selectUser.ID = user.UserID;
              selectUser.Name = user.DisplayName;

              if (!string.IsNullOrEmpty(user.InOfficeComment)) selectUser.InOfficeMessage = user.InOfficeComment;
              if (sender.Count > 0 && sender[0].UserID == user.UserID) selectUser.IsSender = true;
              if (ticket.CreatorID == user.UserID) selectUser.IsCreator = true;
              if (ticket.UserID == user.UserID) selectUser.IsSelected = true;

              if (selectUser.IsSender)
              {
                result.Insert(0, selectUser);
                hasSenderBeenAdded = true;
              }
              else if (selectUser.IsCreator)
              {
                if (hasSenderBeenAdded)
                {
                  result.Insert(1, selectUser);
                }
                else
                  result.Insert(0, selectUser);
              }
              else
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
                    IsSelected = (ticket[0].GroupID == g.GroupID)
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
                    timeLineItem.item.Message = SanitizeMessage(timeLineItem.item.Message, loginUser);
                    timeLineItem.Attachments = attachments.GetAttachmentProxies();

                    timeLineItems.Add(timeLineItem);
                }
                else
                {
                    TimeLineItem wcItem = new TimeLineItem();
                    wcItem.item = viewItem.GetProxy();

                    WaterCoolerView wc = new WaterCoolerView(TSAuthentication.GetLoginUser());
						  wc.LoadMoreThreadsNoCountFilter(0, (int)viewItem.TicketNumber);

                    if(wc.Any(d=>d.MessageID == viewItem.RefID))
                    {
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
		public TimeLineItem EmailTicket(int ticketID, string addresses, string introduction)
		{
			addresses = addresses.Length > 200 ? addresses.Substring(0, 200) : addresses;
			EmailPosts posts = new EmailPosts(TSAuthentication.GetLoginUser());
			EmailPost post = posts.AddNewEmailPost();
			post.EmailPostType = EmailPostType.TicketSendEmail;
			post.HoldTime = 0;

			post.Param1 = TSAuthentication.UserID.ToString();
			post.Param2 = ticketID.ToString();
			post.Param3 = addresses;
			post.Text1 = introduction;
			posts.Save();

			string actionText = string.Format("<i>Action added via introduction e-mail to: {0} </i> <br><br>{1}", addresses, introduction);
      string logPost = string.Format("{0} sent a email introduction to {1}", TSAuthentication.GetLoginUser().GetUserFullName(), addresses);
			return LogAction(ticketID, SystemActionType.Email, "Email Introduction", actionText, logPost);
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
          string defaultOrder = "[{'CatID':'AssignedTo','CatName':'Assigned To','Disabled':'false'},{'CatID':'Group','CatName':'Group','Disabled':'false'},{'CatID':'hr','CatName':'Line Break','Disabled':'false'},{'CatID':'Type','CatName':'Type','Disabled':'false'},{'CatID':'Status','CatName':'Status','Disabled':'false'},{'CatID':'Severity','CatName':'Severity','Disabled':'false'},{'CatID':'hr','CatName':'Line Break','Disabled':'false'},{'CatID':'SLAStatus','CatName':'SLA Status','Disabled':'false'},{'CatID':'VisibleToCustomers','CatName':'Visible To Customers','Disabled':'false'},{'CatID':'KnowledgeBase','CatName':'Knowledge Base','Disabled':'false'},{'CatID':'Community','CatName':'Community','Disabled':'false'},{'CatID':'DaysOpened','CatName':'Days Opened','Disabled':'false'},{'CatID':'TotalTimeSpent','CatName':'Total Time Spent','Disabled':'false'},{'CatID':'DueDate','CatName':'Due Date','Disabled':'false'},{'CatID':'hr','CatName':'Line Break','Disabled':'false'},{'CatID':'Customers','CatName':'Customers','Disabled':'false'},{'CatID':'CustomFields','CatName':'Custom Fields','Disabled':'false'},{'CatID':'hr','CatName':'Line Break','Disabled':'false'},{'CatID':'Product','CatName':'Product','Disabled':'false'},{'CatID':'Reported','CatName':'Reported','Disabled':'false'},{'CatID':'Resolved','CatName':'Resolved','Disabled':'false'},{'CatID':'hr','CatName':'Line Break','Disabled':'false'},{'CatID':'Inventory','CatName':'Inventory','Disabled':'false'},{'CatID':'hr','CatName':'Line Break','Disabled':'false'},{'CatID':'Tags','CatName':'Tags','Disabled':'false'},{'CatID':'hr','CatName':'Line Break','Disabled':'false'},{'CatID':'Reminders','CatName':'Reminders','Disabled':'false'},{'CatID':'hr','CatName':'Line Break','Disabled':'false'},{'CatID':'AssociatedTickets','CatName':'Associated Tickets','Disabled':'false'},{'CatID':'hr','CatName':'Line Break','Disabled':'false'},{'CatID':'UserQueue','CatName':'User Queue','Disabled':'false'},{'CatID':'hr','CatName':'Line Break','Disabled':'false'},{'CatID':'SubscribedUsers','CatName':'Subscribed Users','Disabled':'false'},{'CatID':'hr','CatName':'Line Break','Disabled':'true'},{'CatID':'Jira','CatName':'Jira','Disabled':'false'}]";
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
                if (!string.IsNullOrWhiteSpace(user.Signature) && proxy.IsVisibleOnPortal && !proxy.IsKnowledgeBase)
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
              if (proxy.IsVisibleOnPortal && !proxy.IsKnowledgeBase)
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


        [WebMethod]
        public bool SetActionPortal(int actionID, bool isVisibleOnPortal)
        {
          TeamSupport.Data.Action action = Actions.GetAction(TSAuthentication.GetLoginUser(), actionID);
          User user = TSAuthentication.GetUser(TSAuthentication.GetLoginUser());
          User author = Users.GetUser(TSAuthentication.GetLoginUser(), action.CreatorID);
          bool isKB = action.IsKnowledgeBase;
          if (CanEditAction(action) || user.ChangeTicketVisibility)
          {
            if (author != null)
            {
              if (isVisibleOnPortal && !isKB)
              {
                if (!string.IsNullOrWhiteSpace(author.Signature))
                {
                  if (!action.Description.Contains(author.Signature.Replace(" />", ">")))
                    action.Description = action.Description + "<br/><br/>" + author.Signature;
                }
              }
              else
              {
                if (!string.IsNullOrWhiteSpace(author.Signature))
                {
                  if (action.Description.Contains(author.Signature.Replace(" />", ">")))
                  {
                    action.Description = action.Description.Replace("<p><br><br></p>\n" + author.Signature.Replace(" />", ">"), "").Replace("<br><br>" + author.Signature.Replace(" />", ">"), "");
                    //action.Description = action.Description.Replace("<br><br>" + author.Signature.Replace(" />", ">"), "");
                  }
                }
              }
            }
            action.IsVisibleOnPortal = isVisibleOnPortal;
            action.Collection.Save();
          }
          return action.IsVisibleOnPortal;
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
                    label = String.Format("<strong>{0} {1}</strong></br>{2}", users[i].FirstName, users[i].LastName, users[i].Organization),
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

            //result.Add(new AutocompleteItem("Unassigned", "-1"));
            for (int i = 0; i < users.Count(); i++)
            {
                result.Add(new AutocompleteItem(users[i].DisplayName, users[i].UserID.ToString()));
            }
            return result.ToArray();
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

        [WebMethod]
        public object[] GetKBTicketAndActions(int ticketID)
        {
          TicketsViewItem ticket = TicketsView.GetTicketsViewItem(TSAuthentication.GetLoginUser(), ticketID);
          ActionsView view = new ActionsView(TSAuthentication.GetLoginUser());
          view.LoadKBByTicketID(ticketID);

          List<object> result = new List<object>();
          result.Add(ticket.GetProxy());
          result.Add(view.GetActionsViewItemProxies());
          result.Add(GetActionAttachments(view[0].ActionID));
          return result.ToArray();
        }


        [WebMethod]
        public TimeLineItem NewWCPost(string data)
        {
          WatercoolerJsonInfo info = Newtonsoft.Json.JsonConvert.DeserializeObject<WatercoolerJsonInfo>(data);
          WatercoolerMsgItem wc = (new WatercoolerMsg(TSAuthentication.GetLoginUser())).AddNewWatercoolerMsgItem();
          int parentmsgid = info.ParentTicketID;
          int pageType = info.PageType;
          int pageID = info.PageID;

          wc.UserID = TSAuthentication.UserID;
          wc.OrganizationID = TSAuthentication.OrganizationID;
          wc.TimeStamp = DateTime.UtcNow;
          wc.LastModified = DateTime.UtcNow;
          wc.Message = info.Description;
          wc.MessageParent = parentmsgid;

          wc.Collection.Save();

          if (info.ParentTicketID != -1)
          {
            WatercoolerMsg wcm = new WatercoolerMsg(TSAuthentication.GetLoginUser());
            wcm.LoadByMessageID(parentmsgid);

            wcm[0].LastModified = DateTime.UtcNow;
            wcm[0].Collection.Save();
          }

          if (wc.MessageParent == -1 && info.PageType == 0)
            addWCAttachment((int)wc.MessageID, info.PageID, WaterCoolerAttachmentType.Ticket);
          foreach (int ticketID in info.Tickets)
          {
            addWCAttachment(wc.MessageID, ticketID, WaterCoolerAttachmentType.Ticket);
            if (wc.MessageParent != -1)
              addWCAttachment((int)wc.MessageParent, ticketID, WaterCoolerAttachmentType.Ticket);
          }

          if (wc.MessageParent == -1 && info.PageType == 1)
            addWCAttachment((int)wc.MessageID, info.PageID, WaterCoolerAttachmentType.Product);
          foreach (int productID in info.Products)
          {
            addWCAttachment(wc.MessageID, productID, WaterCoolerAttachmentType.Product);
            if (wc.MessageParent != -1)
              addWCAttachment((int)wc.MessageParent, productID, WaterCoolerAttachmentType.Product);
          }

          if (wc.MessageParent == -1 && info.PageType == 2)
            addWCAttachment((int)wc.MessageID, info.PageID, WaterCoolerAttachmentType.Company);
          foreach (int CompanyID in info.Company)
          {
            addWCAttachment(wc.MessageID, CompanyID, WaterCoolerAttachmentType.Company);
            if (wc.MessageParent != -1)
              addWCAttachment((int)wc.MessageParent, CompanyID, WaterCoolerAttachmentType.Company);
          }

          if (wc.MessageParent == -1 && info.PageType == 4)
            addWCAttachment((int)wc.MessageID, info.PageID, WaterCoolerAttachmentType.Group);
          foreach (int groupID in info.Groups)
          {
            addWCAttachment(wc.MessageID, groupID, WaterCoolerAttachmentType.Group);
            if (wc.MessageParent != -1)
              addWCAttachment((int)wc.MessageParent, groupID, WaterCoolerAttachmentType.Group);
          }

          foreach (int UserID in info.User)
          {
            addWCAttachment(wc.MessageID, UserID, WaterCoolerAttachmentType.User);
            if (wc.MessageParent != -1)
              addWCAttachment((int)wc.MessageParent, UserID, WaterCoolerAttachmentType.User);
          }

          //return wc.MessageID;
          return GetWCLineItem(wc);
        }

        [WebMethod]
        public object[] SetTicketType(int ticketID, int ticketTypeID)
        {
          TicketService ts = new TicketService();
          TransferCustomValues(ticketID, ticketTypeID);
          Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
          int statusId = ticket.TicketStatusID;
          if (ticketTypeID == ticket.TicketTypeID) return null;
          //if (!CanEditTicket(ticket)) return null;
          TicketType ticketType = TicketTypes.GetTicketType(ticket.Collection.LoginUser, ticketTypeID);
          if (ticketType.OrganizationID != TSAuthentication.OrganizationID) return null;
          ticket.TicketTypeID = ticketTypeID;

          TicketStatuses statuses = new TicketStatuses(ticket.Collection.LoginUser);
          statuses.LoadAvailableTicketStatuses(ticketTypeID, null);
          ticket.TicketStatusID = statuses[0].TicketStatusID;
          TicketStatus newStatus = null;
          try
          {
            TicketStatus currStatus = TicketStatuses.GetTicketStatus(ticket.Collection.LoginUser, statusId);
            newStatus = statuses.Where(s => s.Name == currStatus.Name).FirstOrDefault();
            ticket.TicketStatusID = newStatus.TicketStatusID;
          }
          catch (Exception)
          {
          }
          ticket.Collection.Save();
          List<object> result = new List<object>();
          result.Add((newStatus != null) ? newStatus.GetProxy() : statuses[0].GetProxy());
          result.Add(ts.GetParentCustomValues(ticketID));
          TicketsViewItem tv = TicketsView.GetTicketsViewItemByNumber(TSAuthentication.GetLoginUser(), ticket.TicketNumber);
          result.Add(tv.GetProxy());
          return result.ToArray();
        }

        [WebMethod]
        public TimeLineItem ConvertActionItem(int actionID)
        {
          LoginUser loginUser = TSAuthentication.GetLoginUser();
          TeamSupport.Data.Actions actions = new Actions(loginUser);
          actions.LoadByActionID(actionID);
          return GetActionTimelineItem(actions[0]);
        }


        [WebMethod]
        public bool SetActionPinned(int ticketID, int actionID, bool pinned)
        {
          // When an action is pinned, all other actions need to be unpinned.
          if (pinned)
          {
            Actions actions = new Actions(TSAuthentication.GetLoginUser());
            actions.LoadByTicketID(ticketID);
            foreach (TeamSupport.Data.Action action in actions)
            {
              if (action.ActionID == actionID)
              {
                action.Pinned = true;
              }
              else if(action.Pinned)
              {
                action.Pinned = false;
              }
            }
            actions.Save();
          }
          // When unpin we only need to update the requested action.
          else
          {
            TeamSupport.Data.Action action = Actions.GetAction(TSAuthentication.GetLoginUser(), actionID);
            action.Pinned = false;
            action.Collection.Save();
          }
          return pinned;
        }


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
        public class TicketPageUser : TicketPropertySelectField
        {
          [DataMember]
          public string InOfficeMessage { get; set; }
          [DataMember]
          public bool IsCreator { get; set; }
          [DataMember]
          public bool IsSender { get; set; }
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
            [DataMember]
            public string Disabled { get; set; }
        }

        //Private Methods

        private void TransferCustomValues(int ticketID, int ticketTypeID)
        {
          TicketService ts = new TicketService();
          Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
          CustomValueProxy[] oldValues = ts.GetCustomValues(ticketID);
          DateTime dateValue;
          CustomValues values = new CustomValues(ticket.Collection.LoginUser);
          values.LoadByReferenceType(TSAuthentication.OrganizationID, ReferenceType.Tickets, ticketTypeID, ticketID);
          CustomValueProxy[] newValues = values.GetCustomValueProxies();

          foreach (CustomValueProxy newCustVal in newValues)
          {
            foreach (CustomValueProxy oldCustVal in oldValues)
            {
              if (newCustVal.Name == oldCustVal.Name && newCustVal.FieldType == oldCustVal.FieldType)
              {
                CustomValue customValue = CustomValues.GetValue(TSAuthentication.GetLoginUser(), newCustVal.CustomFieldID, ticketID);
                if (oldCustVal.Value == null)
                {
                  customValue.Value = "";
                  customValue.Collection.Save();
                }

                if (customValue.FieldType == CustomFieldType.DateTime || customValue.FieldType == CustomFieldType.Date || customValue.FieldType == CustomFieldType.Time)
                {
                  if (oldCustVal.Value != null)
                  {
                    if (DateTime.TryParse(oldCustVal.Value.ToString(), out dateValue))
                      customValue.Value = dateValue.ToString();
                  }
                }
                else
                {
                  customValue.Value = oldCustVal.Value.ToString();
                }

                customValue.Collection.Save();
              }
            }

          }
        }

        private string CleanMessage(string message,  LoginUser loginUser)
        {
          return HTMLSanitizeMessage(HtmlUtility.CheckScreenR(loginUser, message));
        }

        private string HTMLSanitizeMessage(string message)
        {
          return HtmlUtility.Sanitize(message);
        }

        private string SanitizeMessage(string message, LoginUser loginUser)
        {
          //return TagMessage(AddScreenrToMessage(message, loginUser), loginUser)
          return AddScreenrToMessage(message, loginUser);
        }

        private string TagMessage(string message, LoginUser loginUser)
        {
          return HtmlUtility.TagHtml(loginUser, message);
        }

        private string AddScreenrToMessage(string message, LoginUser loginUser)
        {
          return HtmlUtility.CheckScreenR(loginUser, message);
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
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), action.CreatorID);
            TimeLineItem item = new TimeLineItem();

          string displayName = "";

          if(user != null) 
            displayName = user.DisplayName;
          else if (Enum.IsDefined(typeof(SystemUser), action.CreatorID)) 
            displayName = ((SystemUser)action.CreatorID).ToString();

            TicketTimeLineViewItemProxy itemProxy = new TicketTimeLineViewItemProxy
            {
                TicketID = action.TicketID,
                RefID = action.ActionID,
                IsWC = false,
                MessageType = action.ActionTypeName,
                ActionTypeID = action.ActionTypeID,
                Message = SanitizeMessage(action.Description, loginUser),
                DateCreated = DateTime.SpecifyKind(action.DateCreatedUtc, DateTimeKind.Utc),
					 DateStarted = action.DateStartedUtc != null ? DateTime.SpecifyKind((DateTime)action.DateStartedUtc, DateTimeKind.Utc) : action.DateStartedUtc,
                OrganizationID = TSAuthentication.OrganizationID,
                CreatorID = action.CreatorID,
                CreatorName = displayName,
                IsKnowledgeBase = action.IsKnowledgeBase,
                IsVisibleOnPortal = action.IsVisibleOnPortal,
                IsPinned = action.Pinned,
                TimeSpent = action.TimeSpent,
                WCUserID = null
            };

            item.item = itemProxy;

            Attachments attachments = new Attachments(loginUser);
            attachments.LoadByActionID(item.item.RefID);
            item.Attachments = GetActionAttachments(action.ActionID, loginUser);

            return item;
        }

        private TimeLineItem GetWCLineItem(TeamSupport.Data.WatercoolerMsgItem wc)
        {
          TicketTimeLineView TimeLineView = new TicketTimeLineView(TSAuthentication.GetLoginUser());
          TimeLineView.LoadByRefIDAndType(wc.MessageID, true);

          TimeLineItem wcItem = new TimeLineItem();
          wcItem.item = TimeLineView[0].GetProxy();
          wcItem.Likes = 0;
          wcItem.DidLike = false;

          List<WaterCoolerReply> wcReplies = new List<WaterCoolerReply>();
          wcItem.WaterCoolerReplies = wcReplies.ToArray();

          return wcItem;
        }

        private AttachmentProxy[] GetActionAttachments(int actionID, LoginUser loginUser)
        {
            Attachments attachments = new Attachments(loginUser);
            attachments.LoadByActionID(actionID);
            return attachments.GetAttachmentProxies();
        }

        private void addWCAttachment(int messageID, int attachmentID, WaterCoolerAttachmentType attachmentType)
        {
          try
          {
            WatercoolerAttachment ticketAttachment = (new WatercoolerAttachments(TSAuthentication.GetLoginUser()).AddNewWatercoolerAttachment());
            ticketAttachment.MessageID = messageID;
            ticketAttachment.AttachmentID = attachmentID;
            ticketAttachment.RefType = attachmentType;
            ticketAttachment.DateCreated = DateTime.UtcNow;
            ticketAttachment.CreatorID = TSAuthentication.GetLoginUser().UserID;
            ticketAttachment.Collection.Save();
          }
          catch (Exception e)
          {

          }
        }

		private TimeLineItem LogAction(int ticketID, SystemActionType actionType, string name, string actionText, string actionLog)
		{
			TicketsViewItem ticket = TicketsView.GetTicketsViewItem(TSAuthentication.GetLoginUser(), ticketID);
			User user = TSAuthentication.GetUser(TSAuthentication.GetLoginUser());
			TeamSupport.Data.Action action = (new Actions(TSAuthentication.GetLoginUser())).AddNewAction();
			action.ActionTypeID = null;
			action.Name = name;
			action.ActionSource = name;
			action.SystemActionTypeID = actionType;
			action.Description = actionText;
			action.IsVisibleOnPortal = false;
			action.IsKnowledgeBase = false;
			action.TicketID = ticket.TicketID;
			action.CreatorID = TSAuthentication.UserID;
			action.Collection.Save();

			ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, actionLog);

			return GetActionTimelineItem(action);
		}
	}
}