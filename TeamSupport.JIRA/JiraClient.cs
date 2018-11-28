using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Deserializers;
using TeamSupport.JIRA.JiraJSONSerializedModels;

namespace TeamSupport.JIRA
{
	//JIRA REST API documentation: https://docs.atlassian.com/jira/REST/latest

	public class JiraClient<TIssueFields> : IJiraClient<TIssueFields> where TIssueFields : IssueFields, new()
	{
		private readonly string username;
        private readonly string token;
		private readonly RestClient client;
		private readonly JsonDeserializer deserializer;
		public JiraClient(string baseUrl, string username, string token)
		{
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
			this.username = username;
            this.token = token;
			deserializer = new JsonDeserializer();
			client = new RestClient { BaseUrl = new System.Uri(baseUrl + (baseUrl.EndsWith("/") ? "" : "/") + "rest/api/latest/") };
		}

		public JiraClient(string baseUrl, string username, string token, string apiPath)
		{
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            this.username = username;
            this.token = token;
			deserializer = new JsonDeserializer();
			client = new RestClient { BaseUrl = new System.Uri(baseUrl + (baseUrl.EndsWith("/") ? "" : "/") + "rest/" + apiPath + "/1.0/") };
		}

		private RestRequest CreateRequest(Method method, String path)
		{
			var request = new RestRequest { Method = method, Resource = path, RequestFormat = DataFormat.Json };
			request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(String.Format("{0}:{1}", username, token))));
			return request;
		}

		private void AssertStatus(IRestResponse response, HttpStatusCode status)
		{
			if (response.ErrorException != null)
				throw new JiraClientException("Transport level error: " + response.ErrorMessage, response.ErrorException);
			if (response.StatusCode != status)
				throw new JiraClientException("JIRA returned wrong status: " + response.StatusDescription, response.Content);
		}

		public IssueMetaData.RootObject GetIssueMetaData(string projectKey, string issueType)
		{
			IssueMetaData.RootObject issueMetaData = new IssueMetaData.RootObject();

			try
			{
				var path = string.Format("issue/createmeta?projectKeys={0}&issuetypeNames={1}&expand=projects.issuetypes.fields", projectKey, issueType);
				var request = CreateRequest(Method.GET, path);

				var response = client.Execute(request);
				AssertStatus(response, HttpStatusCode.OK);

				issueMetaData = deserializer.Deserialize<IssueMetaData.RootObject>(response);

				if (!string.IsNullOrEmpty(response.Content))
				{
					string responseContent = response.Content;
					JObject jObject = JObject.Parse(responseContent);

					if (issueMetaData != null
						&& issueMetaData.projects != null
						&& issueMetaData.projects.Any()
						&& issueMetaData.projects[0].issuetypes != null
						&& issueMetaData.projects[0].issuetypes.Any())
						issueMetaData.projects[0].issuetypes[0].fields.customFields = IssueMetaData.GetCustomFields(jObject.ToString());
				}
			}
			catch (Exception ex)
			{
				Trace.TraceError("GetIssueMetaData() error: {0}", ex);
				throw new JiraClientException("Could not get issue metadata", ex);
			}

			return issueMetaData;
		}

        public IEnumerable<Issue<TIssueFields>> GetAllIssues()
        {
            try
            {
                var baseURL = @"https://teamsupportio.atlassian.net";
                var client = new RestClient(baseURL);
                var request = new RestRequest("/rest/api/latest/search", Method.GET);
                request.AddHeader("Authorization", "Basic " + token);
                var response = client.Execute(request);
                var data = deserializer.Deserialize<IssueContainer<TIssueFields>>(response);
                var issues = data.issues ?? Enumerable.Empty<Issue<TIssueFields>>();
                return issues;
            }
            catch (Exception ex)
            {
                Trace.TraceError("GetAllIssues() error: {0}", ex);
                throw new JiraClientException("Could not get all issues", ex);
            }
        }

        public IEnumerable<Issue<TIssueFields>> GetIssues(String projectKey)
		{
			return EnumerateIssues(projectKey, null).ToArray();
		}

		public IEnumerable<Issue<TIssueFields>> GetIssues(String projectKey, String issueType)
		{
			return EnumerateIssues(projectKey, issueType).ToArray();
		}

		public IEnumerable<Issue<TIssueFields>> EnumerateIssues(String projectKey)
		{
			return EnumerateIssues(projectKey, null);
		}

		public IEnumerable<Issue<TIssueFields>> EnumerateIssues(String projectKey, String issueType)
		{
			try
			{
				return EnumerateIssuesInternal(projectKey, issueType);
			}
			catch (Exception ex)
			{
				Trace.TraceError("EnumerateIssues(projectKey, issueType) error: {0}", ex);
				throw new JiraClientException("Could not load issues", ex);
			}
		}

		private IEnumerable<Issue<TIssueFields>> EnumerateIssuesInternal(String projectKey, String issueType)
		{
			var queryCount = 50;
			var resultCount = 0;
			while (true)
			{
				var jql = String.Format("project={0}", Uri.EscapeUriString(projectKey));
				if (!String.IsNullOrEmpty(issueType))
					jql += String.Format("+AND+issueType={0}", Uri.EscapeUriString(issueType));
				var path = String.Format("search?jql={0}&startAt={1}&maxResults={2}", jql, resultCount, queryCount);
				var request = CreateRequest(Method.GET, path);

				var response = client.Execute(request);
				AssertStatus(response, HttpStatusCode.OK);

				var data = deserializer.Deserialize<IssueContainer<TIssueFields>>(response);
				var issues = data.issues ?? Enumerable.Empty<Issue<TIssueFields>>();

				foreach (var item in issues) yield return item;
				resultCount += issues.Count();

				if (resultCount < data.total) continue;
				else /* all issues received */ break;
			}
		}

		public Issue<TIssueFields> LoadIssue(IssueRef issueRef)
		{
			if (String.IsNullOrEmpty(issueRef.id))
				return LoadIssue(issueRef.key);
			else /* we have an id */
				return LoadIssue(issueRef.id);
		}

		public Issue<TIssueFields> LoadIssue(String issueRef)
		{
			try
			{
				var path = String.Format("issue/{0}", issueRef);
				var request = CreateRequest(Method.GET, path);

				var response = client.Execute(request);
				AssertStatus(response, HttpStatusCode.OK);

				var issue = deserializer.Deserialize<Issue<TIssueFields>>(response);
				issue.fields.comments = GetComments(issue).ToList();
				issue.fields.watchers = GetWatchers(issue).ToList();
				Issue.ExpandLinks(issue);

				if (!string.IsNullOrEmpty(response.Content))
				{
					issue.fields.SetCustomFields(response.Content);
				}

				return issue;
			}
			catch (Exception ex)
			{
				Trace.TraceError("GetIssue(issueRef) error: {0}", ex);
				throw new JiraClientException("Could not load issue", ex);
			}
		}

		public Issue<TIssueFields> CreateIssue(String projectKey, String issueType, String summary)
		{
			return CreateIssue(projectKey, issueType, new TIssueFields { summary = summary });
		}

        public Issue<TIssueFields> CreateIssue(String projectKey, String issueType, TIssueFields issueFields)
		{
            try { 
                var request = CreateRequest(Method.POST, "issue");
                request.AddHeader("ContentType", "application/json");

                var issueData = new Dictionary<string, object>();
				issueData.Add("project", new { key = projectKey });
				issueData.Add("issuetype", new { name = issueType });

				if (issueFields.summary != null)
					issueData.Add("summary", issueFields.summary);
				if (issueFields.description != null)
					issueData.Add("description", issueFields.description);
				if (issueFields.labels != null)
					issueData.Add("labels", issueFields.labels);
				if (issueFields.timetracking != null)
					issueData.Add("timetracking", new { originalEstimate = issueFields.timetracking.originalEstimate });

				var propertyList = typeof(TIssueFields).GetProperties().Where(p => p.Name.StartsWith("customfield_"));
				foreach (var property in propertyList)
				{
					var value = property.GetValue(issueFields, null);
					if (value != null) issueData.Add(property.Name, value);
				}

				var response = client.Execute(request);
                AssertStatus(response, HttpStatusCode.Created);

                var issueRef = deserializer.Deserialize<IssueRef>(response);
                return LoadIssue(issueRef);
            }
			catch (Exception ex)
			{
				Trace.TraceError("CreateIssue(projectKey, typeCode) error: {0}", ex);
				throw new JiraClientException("Could not create issue", ex);
			}
		}

<<<<<<< HEAD
        //public IssueRef CreateIssueViaRestClient(String projectKey, String issueType, TIssueFields issueFields)
        //{
        //    try
        //    {
        //        var baseURL = @"https://teamsupportio.atlassian.net";
        //        var client = new RestClient(baseURL);
        //        var request = new RestRequest("rest/api/2/issue/", Method.POST);

        //        var issueData = new Dictionary<string, object>();
        //        issueData.Add("project", new { key = projectKey });
        //        issueData.Add("issuetype", new { name = issueType });

        //        if (issueFields.summary != null)
        //            issueData.Add("summary", issueFields.summary);
        //        if (issueFields.description != null)
        //            issueData.Add("description", issueFields.description);
        //        var project = new JIRA.JiraJSONSerializedModels.Project() { key = projectKey.ToString() };
        //        var ticketToSendToJira = new BaseIssue()
        //        {
        //            fields = new Fields()
        //            {
        //                project = project,//This is the rootObject.fields.project.key
        //                summary = issueFields.summary,
        //                description = issueFields.description,
        //                issuetype = new Issuetype()
        //                {
        //                    name = issueType.ToString()
        //                }
        //            }
        //        };
        //        var ticketToSendToJiraSerialized = JsonConvert.SerializeObject(ticketToSendToJira);
        //        request.AddParameter("application/json", ticketToSendToJiraSerialized, ParameterType.RequestBody);
        //        request.AddHeader("Authorization", "Basic " + token);

        //        var response = client.Execute(request);
        //        return deserializer.Deserialize<IssueRef>(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.TraceError("CreateIssue(projectKey, typeCode) error: {0}", ex);
        //        throw new JiraClientException("Could not create issue", ex);
        //    }
        //}
=======
        public IssueRef CreateIssueViaRestClient(String projectKey, String issueType, TIssueFields issueFields)
        {
            try
            {
                var baseURL = @"https://teamsupportio.atlassian.net";
                var client = new RestClient(baseURL);
                var request = new RestRequest("rest/api/2/issue/", Method.POST);

                var issueData = new Dictionary<string, object>();
                issueData.Add("project", new { key = projectKey });
                issueData.Add("issuetype", new { name = issueType });

                if (issueFields.summary != null)
                    issueData.Add("summary", issueFields.summary);
                if (issueFields.description != null)
                    issueData.Add("description", issueFields.description);
                var project = new JIRA.JiraJSONSerializedModels.Project() { key = projectKey.ToString() };
                var ticketToSendToJira = new RootObject()
                {
                    fields = new Fields()
                    {
                        project = project,//This is the rootObject.fields.project.key
                        summary = issueFields.summary,
                        description = issueFields.description,
                        issuetype = new Issuetype()
                        {
                            name = issueType.ToString()
                        }
                    }
                };
                var ticketToSendToJiraSerialized = JsonConvert.SerializeObject(ticketToSendToJira);
                request.AddParameter("application/json", ticketToSendToJiraSerialized, ParameterType.RequestBody);
                request.AddHeader("Authorization", "Basic " + token);

                var response = client.Execute(request);
                return deserializer.Deserialize<IssueRef>(response);
            }
            catch (Exception ex)
            {
                Trace.TraceError("CreateIssue(projectKey, typeCode) error: {0}", ex);
                throw new JiraClientException("Could not create issue", ex);
            }
        }
>>>>>>> c364012ad236c68b013e5deb1570f4103397046d

        public Issue<TIssueFields> UpdateIssue(Issue<TIssueFields> issue)
		{
			try
			{
				var path = String.Format("issue/{0}", issue.id);
				var request = CreateRequest(Method.PUT, path);
				request.AddHeader("ContentType", "application/json");

				var updateData = new Dictionary<string, object>();
				if (issue.fields.summary != null)
					updateData.Add("summary", new[] { new { set = issue.fields.summary } });
				if (issue.fields.description != null)
					updateData.Add("description", new[] { new { set = issue.fields.description } });
				if (issue.fields.labels != null)
					updateData.Add("labels", new[] { new { set = issue.fields.labels } });
				if (issue.fields.timetracking != null)
					updateData.Add("timetracking", new[] { new { set = new { originalEstimate = issue.fields.timetracking.originalEstimate } } });

				//var propertyList = typeof(TIssueFields).GetProperties().Where(p => p.Name.StartsWith("customfield_"));
				//foreach (var property in propertyList)
				//{
				//    var value = property.GetValue(issue.fields, null);
				//    if (value != null) updateData.Add(property.Name, new[] { new { set = value } });
				//}

				foreach (KeyValuePair<string, CustomField> customField in issue.fields.customFields)
				{
					updateData.Add(customField.Key, new[] { new { set = customField.Value.Value } });
				}

				request.AddBody(new { update = updateData });

				var response = client.Execute(request);
				AssertStatus(response, HttpStatusCode.NoContent);

				return LoadIssue(issue);
			}
			catch (JiraClientException jiraEx)
			{
				Trace.TraceError("UpdateIssue(issue) error: {0}", jiraEx);
				throw new JiraClientException("Could not update issue", jiraEx);
			}
			catch (Exception ex)
			{
				throw new Exception("Exception caught when updating issue", ex);
			}
		}

		public bool UpdateIssueField(int issueId, string fieldName, string fieldValue)
		{
			bool isSuccessful = false;

			try
			{
				bool isTimeTracking = false;
				fieldName = fieldName.ToLower();
				var path = String.Format("issue/{0}", issueId);
				var request = CreateRequest(Method.PUT, path);
				request.AddHeader("ContentType", "application/json");

				var updateData = new Dictionary<string, object>();

				if (fieldName == "summary")
				{
					updateData.Add(fieldName, fieldValue);
				}

				if (fieldName == "priority")
				{
					updateData.Add(fieldName, new[] { new { set = new { name = fieldValue } } });
				}

				if (fieldName.StartsWith("customfield_"))
				{
					updateData.Add(fieldName, new[] { new { set = new { value = fieldValue } } });
				}

				if (fieldName == "originalestimate")
				{
					updateData.Add("timetracking", new { originalEstimate = fieldValue });
					isTimeTracking = true;
				}

				if (fieldName == "remainingestimate")
				{
					updateData.Add("timetracking", new { remainingEstimate = fieldValue });
					isTimeTracking = true;
				}

				if (!isTimeTracking)
				{
					request.AddBody(new { update = updateData });
				}
				else
				{
					request.AddBody(new { fields = updateData });
				}

				var response = client.Execute(request);
				AssertStatus(response, HttpStatusCode.NoContent);

				isSuccessful = true;
			}
			catch (JiraClientException jiraEx)
			{
				Trace.TraceError("UpdateIssue(issue) error: {0}", jiraEx);
				throw new JiraClientException("Could not update issue field " + fieldName + " with: " + fieldValue, jiraEx);
			}
			catch (Exception ex)
			{
				throw new Exception("Exception caught when updating issue field " + fieldName + " with: " + fieldValue, ex);
			}

			return isSuccessful;
		}

		public bool UpdateIssueFieldByParameter(int issueId, string jsonBody)
		{
			bool isSuccessful = false;

			try
			{
				var path = string.Format("issue/{0}", issueId);
				var request = CreateRequest(Method.PUT, path);
				request.AddHeader("ContentType", "application/json");
				request.AddParameter("application/json", jsonBody, ParameterType.RequestBody);

				var response = client.Execute(request);
				AssertStatus(response, HttpStatusCode.NoContent);
			}
			catch (JiraClientException jiraEx)
			{
				Trace.TraceError("UpdateIssues(issue, updatefields) error: {0}", jiraEx);
				throw new JiraClientException("Could not update issue field with: " + jsonBody, jiraEx);
			}
			catch (Exception ex)
			{
				throw new Exception("Exception caught when updating issue field with: " + jsonBody, ex);
			}

			return isSuccessful;
		}

		public bool UpdateIssueFields(int issueId, Dictionary<string, string> updateFields)
		{
			bool isSuccessful = false;

			try
			{
				var updateData = new Dictionary<string, object>();

				foreach (KeyValuePair<string, string> updateField in updateFields)
				{
					string fieldName = updateField.Key.ToLower();
					string fieldValue = updateField.Value;

					if (fieldName == "summary")
					{
						updateData.Add(fieldName, fieldValue);
					}

					if (fieldName == "priority")
					{
						updateData.Add(fieldName, new[] { new { set = new { name = fieldValue } } });
					}

					if (fieldName.StartsWith("customfield_"))
					{
						updateData.Add(fieldName, new[] { new { set = new { value = fieldValue } } });
					}

					if (fieldName == "originalestimate")
					{
						updateData.Add("timetracking", new[] { new { set = new { originalestimate = fieldValue } } });
					}

					if (fieldName == "remainingestimate")
					{
						updateData.Add("timetracking", new[] { new { set = new { remainingestimate = fieldValue } } });
					}
				}

				if (updateData != null && updateData.Count > 0 && updateData.Any())
				{
					var path = String.Format("issue/{0}", issueId);
					var request = CreateRequest(Method.PUT, path);
					request.AddHeader("ContentType", "application/json");
					request.AddBody(new { update = updateData });

					var response = client.Execute(request);
					AssertStatus(response, HttpStatusCode.NoContent);
				}

				isSuccessful = true;
			}
			catch (JiraClientException jiraEx)
			{
				Trace.TraceError("UpdateIssues(issue, updatefields) error: {0}", jiraEx);
				string s = string.Join(";", updateFields.Select(x => x.Key + "=" + x.Value).ToArray());
				throw new JiraClientException("Could not update issue fields " + s, jiraEx);
			}
			catch (Exception ex)
			{
				string s = string.Join(";", updateFields.Select(x => x.Key + "=" + x.Value).ToArray());
				throw new Exception("Exception caught when updating issue fields " + s, ex);
			}

			return isSuccessful;
		}

		public void DeleteIssue(IssueRef issue)
		{
			try
			{
				var path = String.Format("issue/{0}?deleteSubtasks=true", issue.id);
				var request = CreateRequest(Method.DELETE, path);

				var response = client.Execute(request);
				AssertStatus(response, HttpStatusCode.NoContent);
			}
			catch (Exception ex)
			{
				Trace.TraceError("DeleteIssue(issue) error: {0}", ex);
				throw new JiraClientException("Could not delete issue", ex);
			}
		}

		public IEnumerable<Transition> GetTransitions(IssueRef issue)
		{
			try
			{
				var path = String.Format("issue/{0}/transitions?expand=transitions.fields", issue.id);
				var request = CreateRequest(Method.GET, path);

				var response = client.Execute(request);
				AssertStatus(response, HttpStatusCode.OK);

				var data = deserializer.Deserialize<TransitionsContainer>(response);
				return data.transitions;
			}
			catch (Exception ex)
			{
				Trace.TraceError("GetTransitions(issue) error: {0}", ex);
				throw new JiraClientException("Could not load issue transitions", ex);
			}
		}

		public Issue<TIssueFields> TransitionIssue(IssueRef issue, Transition transition)
		{
			try
			{
				var path = String.Format("issue/{0}/transitions", issue.id);
				var request = CreateRequest(Method.POST, path);
				request.AddHeader("ContentType", "application/json");

				var update = new Dictionary<string, object>();
				update.Add("transition", new { id = transition.id });
				if (transition.fields != null)
					update.Add("fields", transition.fields);

				request.AddBody(update);

				var response = client.Execute(request);
				AssertStatus(response, HttpStatusCode.NoContent);

				return LoadIssue(issue);
			}
			catch (Exception ex)
			{
				Trace.TraceError("TransitionIssue(issue, transition) error: {0}", ex);
				throw new JiraClientException("Could not transition issue state", ex);
			}
		}


		public IEnumerable<JiraUser> GetWatchers(IssueRef issue)
		{
			try
			{
				var path = String.Format("issue/{0}/watchers", issue.id);
				var request = CreateRequest(Method.GET, path);

				var response = client.Execute(request);
				AssertStatus(response, HttpStatusCode.OK);

				return deserializer.Deserialize<WatchersContainer>(response).watchers;
			}
			catch (Exception ex)
			{
				Trace.TraceError("GetWatchers(issue) error: {0}", ex);
				throw new JiraClientException("Could not load watchers", ex);
			}
		}


        public IEnumerable<Comment> GetComments(IssueRef issue)
        {
            try
            {
                var path = String.Format("issue/{0}/comment", issue.id);
                var request = CreateRequest(Method.GET, path);

                var response = client.Execute(request);
                AssertStatus(response, HttpStatusCode.OK);

                var data = deserializer.Deserialize<CommentsContainer>(response);
                return data.comments ?? Enumerable.Empty<Comment>();
            }
            catch (Exception ex)
            {
                Trace.TraceError("GetComments(issue) error: {0}", ex);
                throw new JiraClientException("Could not load comments", ex);
            }
        }

        public IEnumerable<Comment> GetCommentsViaProjectKey(string projectKey)
        {
            if(string.IsNullOrEmpty(projectKey))
                throw new JiraClientException("Could not get comments via project key");
            try
            {
                var baseURL = @"https://teamsupportio.atlassian.net";
                var client = new RestClient(baseURL);
                var request = new RestRequest("rest/api/latest/issue/" + projectKey + "/comment", Method.GET);
                request.AddHeader("Authorization", "Basic " + token);
                var response = client.Execute(request);

                var data = deserializer.Deserialize<CommentsContainer>(response);
                return data.comments ?? Enumerable.Empty<Comment>();
            }
            catch (Exception ex)
            {
                Trace.TraceError("GetCommentsViaProjectKey(projectKey) error: {0}", ex);
                throw new JiraClientException("Could not load comments", ex);
            }
        }

        public HttpStatusCode CreateCommentViaProjectKey(string projectKey, string comment)
        {
            if(string.IsNullOrEmpty(projectKey) || string.IsNullOrEmpty(comment))
                throw new JiraClientException("Project key or comment were empty");

            try
            {
                var baseURL = @"https://teamsupportio.atlassian.net";
                var client = new RestClient(baseURL);
                var request = new RestRequest("rest/api/latest/issue/" + projectKey +  "/comment", Method.POST);
                var tmp = new Comment { body = comment };
              //  var testSerialization = JsonConvert.SerializeObject(tmp);
                request.AddParameter("application/json", JsonConvert.SerializeObject(tmp), ParameterType.RequestBody);
                request.AddHeader("Authorization", "Basic " + token);
                var response = client.Execute(request);
                return response.StatusCode;
            }
            catch (Exception ex)
            {
                throw new JiraClientException("Could not create comment: ", ex.Message);
            }
        }

        public Comment CreateComment(IssueRef issue, String comment)
		{
			try
			{
				var path = String.Format("issue/{0}/comment", issue.id);
				var request = CreateRequest(Method.POST, path);
				request.AddHeader("ContentType", "application/json");
				request.AddBody(new Comment { body = comment });

				var response = client.Execute(request);
				AssertStatus(response, HttpStatusCode.Created);

				return deserializer.Deserialize<Comment>(response);
			}
			catch (Exception ex)
			{
				Trace.TraceError("CreateComment(issue, comment) error: {0}", ex);
				throw new JiraClientException("Could not create comment", ex);
			}
		}

        //Ex.) /rest/api/latest/issue/SSP-79/comment/12450
        public Comment UpdateCommentViaProjectKey(string projectKey, int commentId, String comment)
        {
            if (string.IsNullOrEmpty(projectKey))
                throw new JiraClientException("Project key was empty");
            try
            {
                var baseURL = @"https://teamsupportio.atlassian.net";
                var client = new RestClient(baseURL);
                var request = new RestRequest("/rest/api/latest/issue/" + projectKey + "/comment/" + commentId, Method.PUT);
                var tmp = new Comment { body = comment };
                var testSerialization = JsonConvert.SerializeObject(tmp);
                request.AddParameter("application/json", JsonConvert.SerializeObject(tmp), ParameterType.RequestBody);
                request.AddHeader("Authorization", "Basic " + token);
                var response = client.Execute(request);

                return deserializer.Deserialize<Comment>(response);
            }
            catch (Exception ex)
            {
                Trace.TraceError("UpdateCommentViaProjectKey(projectKey, commentId, comment) error: {0}", ex);
                throw new JiraClientException("Could not update comment", ex);
            }
        }

        public Comment UpdateComment(IssueRef issue, int commentId, String comment)
		{
			try
			{
				var path = String.Format("issue/{0}/comment/{1}", issue.id, commentId);
				var request = CreateRequest(Method.PUT, path);
				request.AddHeader("ContentType", "application/json");
				request.AddBody(new Comment { body = comment });

				var response = client.Execute(request);
				AssertStatus(response, HttpStatusCode.OK);

				return deserializer.Deserialize<Comment>(response);
			}
			catch (Exception ex)
			{
				Trace.TraceError("CreateComment(issue, comment) error: {0}", ex);
				throw new JiraClientException("Could not create comment", ex);
			}
		}

		public void DeleteComment(IssueRef issue, Comment comment)
		{
			try
			{
				var path = String.Format("issue/{0}/comment/{1}", issue.id, comment.id);
				var request = CreateRequest(Method.DELETE, path);

				var response = client.Execute(request);
				AssertStatus(response, HttpStatusCode.NoContent);
			}
			catch (Exception ex)
			{
				Trace.TraceError("DeleteComment(issue, comment) error: {0}", ex);
				throw new JiraClientException("Could not delete comment", ex);
			}
		}

		public IEnumerable<Attachment> GetAttachments(IssueRef issue)
		{
			return LoadIssue(issue).fields.attachment;
		}

		public Attachment CreateAttachment(IssueRef issue, Stream fileStream, String fileName)
		{
			try
			{
				Byte[] byteData = UTF8Encoding.UTF8.GetBytes(fileName);

				var path = String.Format("issue/{0}/attachments", issue.id);
				var request = CreateRequest(Method.POST, path);
				request.AddHeader("X-Atlassian-Token", "nocheck");
				request.AddHeader("ContentType", "multipart/form-data");
				request.AddFile("file", @"\\dev-sql\TSData\Organizations\13679\Actions\10198538\19604636971407100.docx");
				request.AddHeader("ContentLength", 10485760.ToString());
				request.AddFile("file", stream => fileStream.CopyTo(stream), fileName);

				var response = client.Execute(request);
				AssertStatus(response, HttpStatusCode.OK);

				return deserializer.Deserialize<List<Attachment>>(response).Single();
			}
			catch (Exception ex)
			{
				Trace.TraceError("CreateAttachment(issue, fileStream, fileName) error: {0}", ex);
				throw new JiraClientException("Could not create attachment", ex);
			}
		}

		public void DeleteAttachment(Attachment attachment)
		{
			try
			{
				var path = String.Format("attachment/{0}", attachment.id);
				var request = CreateRequest(Method.DELETE, path);

				var response = client.Execute(request);
				AssertStatus(response, HttpStatusCode.NoContent);
			}
			catch (Exception ex)
			{
				Trace.TraceError("DeleteAttachment(attachment) error: {0}", ex);
				throw new JiraClientException("Could not delete attachment", ex);
			}
		}

		public IEnumerable<IssueLink> GetIssueLinks(IssueRef issue)
		{
			return LoadIssue(issue).fields.issuelinks;
		}

		public IssueLink LoadIssueLink(IssueRef parent, IssueRef child, String relationship)
		{
			try
			{
				var issue = LoadIssue(parent);
				var links = issue.fields.issuelinks
					.Where(l => l.type.name == relationship)
					.Where(l => l.inwardIssue.id == parent.id)
					.Where(l => l.outwardIssue.id == child.id)
					.ToArray();

				if (links.Length > 1)
					throw new JiraClientException("Ambiguous issue link");
				return links.SingleOrDefault();
			}
			catch (Exception ex)
			{
				Trace.TraceError("LoadIssueLink(parent, child, relationship) error: {0}", ex);
				throw new JiraClientException("Could not load issue link", ex);
			}
		}

		public IssueLink CreateIssueLink(IssueRef parent, IssueRef child, String relationship)
		{
			try
			{
				var request = CreateRequest(Method.POST, "issueLink");
				request.AddHeader("ContentType", "application/json");
				request.AddBody(new
				{
					type = new { name = relationship },
					inwardIssue = new { id = parent.id },
					outwardIssue = new { id = child.id }
				});

				var response = client.Execute(request);
				AssertStatus(response, HttpStatusCode.Created);

				return LoadIssueLink(parent, child, relationship);
			}
			catch (Exception ex)
			{
				Trace.TraceError("CreateIssueLink(parent, child, relationship) error: {0}", ex);
				throw new JiraClientException("Could not link issues", ex);
			}
		}

		public void DeleteIssueLink(IssueLink link)
		{
			try
			{
				var path = String.Format("issueLink/{0}", link.id);
				var request = CreateRequest(Method.DELETE, path);

				var response = client.Execute(request);
				AssertStatus(response, HttpStatusCode.NoContent);
			}
			catch (Exception ex)
			{
				Trace.TraceError("DeleteIssueLink(link) error: {0}", ex);
				throw new JiraClientException("Could not delete issue link", ex);
			}
		}

        /// <summary>
        /// Deletes single issue for a given project per user credentials
        /// </summary>
        /// <param name="projectKey"></param>
        /// <returns></returns>
        public HttpStatusCode DeleteIssueViaProjectKey(string projectKey)
        {
            if (string.IsNullOrEmpty(projectKey))
                throw new JiraClientException("Project key was empty or null");

            try
            {
                var baseURL = @"https://teamsupportio.atlassian.net";
                var client = new RestClient(baseURL);
                var request = new RestRequest("/rest/api/latest/issue/" + projectKey, Method.DELETE);
                request.AddHeader("Authorization", "Basic " + token);
                var response = client.Execute(request);
                return response.StatusCode;
            }
            catch (Exception ex)
            {
                Trace.TraceError("DeleteIssueViaProjectKey(projectKey) error: {0}", ex.Message);
                throw new JiraClientException("Could not delete issue", ex.Message);
            }
        }


        public IEnumerable<RemoteLink> GetRemoteLinks(IssueRef issue)
		{
			try
			{
				var path = string.Format("issue/{0}/remotelink", issue.id);
				var request = CreateRequest(Method.GET, path);
				request.AddHeader("ContentType", "application/json");

				var response = client.Execute(request);
				AssertStatus(response, HttpStatusCode.OK);

				return deserializer.Deserialize<List<RemoteLinkResult>>(response)
					.Select(RemoteLink.Convert).ToList();
			}
			catch (Exception ex)
			{
				Trace.TraceError("GetRemoteLinks(issue) error: {0}", ex);
				throw new JiraClientException("Could not load external links for issue", ex);
			}
		}

        public RemoteLink CreateRemoteLink(IssueRef issue, RemoteLink remoteLink, string globalId)
		{
			try
			{
				var path = string.Format("issue/{0}/remotelink", issue.id);
				var request = CreateRequest(Method.POST, path);
				request.AddHeader("ContentType", "application/json");
				request.AddBody(new
				{
					application = new
					{
						//type = "TechTalk.JiraRestClient",
						name = "Team Support"
					},
					@object = new
					{
						url = remoteLink.url,
						title = remoteLink.title,
						summary = remoteLink.summary,
						icon = remoteLink.icon
					},
					@globalId = globalId
				});

				var response = client.Execute(request);
				AssertStatus(response, HttpStatusCode.Created);

				//returns: { "id": <id>, "self": <url> }
				var linkId = deserializer.Deserialize<RemoteLink>(response).id;
				return GetRemoteLinks(issue).Single(rl => rl.id == linkId);
			}
			catch (Exception ex)
			{
				Trace.TraceError("CreateRemoteLink(issue, remoteLink) error: {0}", ex);
				throw new JiraClientException("Could not create external link for issue", ex);
			}
		}

		public RemoteLink UpdateRemoteLink(IssueRef issue, RemoteLink remoteLink)
		{
			try
			{
				var path = string.Format("issue/{0}/remotelink/{1}", issue.id, remoteLink.id);
				var request = CreateRequest(Method.PUT, path);
				request.AddHeader("ContentType", "application/json");

				var updateData = new Dictionary<string, object>();
				if (remoteLink.url != null) updateData.Add("url", remoteLink.url);
				if (remoteLink.title != null) updateData.Add("title", remoteLink.title);
				if (remoteLink.summary != null) updateData.Add("summary", remoteLink.summary);
				request.AddBody(new { @object = updateData });

				var response = client.Execute(request);
				AssertStatus(response, HttpStatusCode.NoContent);

				return GetRemoteLinks(issue).Single(rl => rl.id == remoteLink.id);
			}
			catch (Exception ex)
			{
				Trace.TraceError("UpdateRemoteLink(issue, remoteLink) error: {0}", ex);
				throw new JiraClientException("Could not update external link for issue", ex);
			}
		}

		public void DeleteRemoteLink(IssueRef issue, RemoteLink remoteLink)
		{
			try
			{
				var path = string.Format("issue/{0}/remotelink/{1}", issue.id, remoteLink.id);
				var request = CreateRequest(Method.DELETE, path);
				request.AddHeader("ContentType", "application/json");

				var response = client.Execute(request);
				AssertStatus(response, HttpStatusCode.NoContent);
			}
			catch (Exception ex)
			{
				Trace.TraceError("DeleteRemoteLink(issue, remoteLink) error: {0}", ex);
				throw new JiraClientException("Could not delete external link for issue", ex);
			}
		}

		public IEnumerable<IssueType> GetIssueTypes()
		{
			try
			{
				var request = CreateRequest(Method.GET, "issuetype");
				request.AddHeader("ContentType", "application/json");

                var response = client.Execute(request);
				AssertStatus(response, HttpStatusCode.OK);

				var data = deserializer.Deserialize<List<IssueType>>(response);
				return data;

			}
			catch (Exception ex)
			{
				Trace.TraceError("GetIssueTypes() error: {0}", ex);
				throw new JiraClientException("Could not load issue types", ex);
			}
		}

		public IEnumerable<Project> GetProjects()
		{
			try
			{
				var request = CreateRequest(Method.GET, "project");
				request.AddHeader("ContentType", "application/json");

				var response = client.Execute(request);
				AssertStatus(response, HttpStatusCode.OK);

				var data = deserializer.Deserialize<List<Project>>(response);
				return data;

			}
			catch (Exception ex)
			{
				Trace.TraceError("GetProjects() error: {0}", ex);
				throw new JiraClientException("Could not load projects", ex);
			}
		}

		public Sprint GetSprintById(int id)
		{
			try
			{
				var path = string.Format("sprint/{0}", id);
				var request = CreateRequest(Method.GET, path);
				request.AddHeader("ContentType", "application/json");

				var response = client.Execute(request);
				AssertStatus(response, HttpStatusCode.OK);

				var data = deserializer.Deserialize<Sprint>(response);
				return data;

			}
			catch (JiraClientException jiraEx)
			{
				Trace.TraceError("GetSprintById(id) error: {0}", jiraEx);
				throw new JiraClientException("Could not sprint with id: " + id.ToString(), jiraEx);
			}
			catch (Exception ex)
			{
				Trace.TraceError("GetSprintById() error: {0}", ex);
				throw new JiraClientException("Could not load sprint", ex);
			}
		}

		public IEnumerable<Board> GetBoards()
		{
			try
			{
				var request = CreateRequest(Method.GET, "board");
				request.AddHeader("ContentType", "application/json");

				var response = client.Execute(request);
				AssertStatus(response, HttpStatusCode.OK);

				var data = deserializer.Deserialize<BoardsContainer>(response);
				return data.values;
			}
			catch (JiraClientException jiraEx)
			{
				Trace.TraceError("GetBoards() error: {0}", jiraEx);
				throw new JiraClientException("Could not get boards.", jiraEx);
			}
			catch (Exception ex)
			{
				Trace.TraceError("GetBoards() error: {0}", ex);
				throw new JiraClientException("Could not load boards", ex);
			}
		}

		public IEnumerable<Sprint> GetSprintsByBoardId(int boardId)
		{
			try
			{
				var path = string.Format("board/{0}/sprint", boardId);
				var request = CreateRequest(Method.GET, path);
				request.AddHeader("ContentType", "application/json");

				var response = client.Execute(request);
				AssertStatus(response, HttpStatusCode.OK);

				var data = deserializer.Deserialize<SprintsContainer>(response);
				return data.values;

			}
			catch (JiraClientException jiraEx)
			{
				Trace.TraceError("GetSprintsByBoardId(boardId) error: {0}", jiraEx);
				throw new JiraClientException("Could not get sprints for board id: " + boardId.ToString(), jiraEx);
			}
			catch (Exception ex)
			{
				Trace.TraceError("GetSprintsByBoardId(boardId) error: {0}", ex);
				throw new JiraClientException("Could not load sprint", ex);
			}
		}

		public ServerInfo GetServerInfo()
		{
			try
			{
				var request = CreateRequest(Method.GET, "serverInfo");
				request.AddHeader("ContentType", "application/json");

				var response = client.Execute(request);
				AssertStatus(response, HttpStatusCode.OK);

				return deserializer.Deserialize<ServerInfo>(response);
			}
			catch (Exception ex)
			{
				Trace.TraceError("GetServerInfo() error: {0}", ex);
				throw new JiraClientException("Could not retrieve server information", ex);
			}
		}

        public HttpStatusCode UpdateIssueViaProjectKey(string projectKey, UpdateObject updateObject)
        {
            if (string.IsNullOrEmpty(projectKey) || updateObject == null)
                throw new JiraClientException("Project key or update object was empty or null");

            try
            {
                var baseURL = @"https://teamsupportio.atlassian.net";
                var client = new RestClient(baseURL);
                var request = new RestRequest("/rest/api/latest/issue/" + projectKey, Method.PUT);
                //var test = JsonConvert.SerializeObject(updateObject);
                request.AddParameter("application/json", JsonConvert.SerializeObject(updateObject), ParameterType.RequestBody);
                request.AddHeader("Authorization", "Basic " + token);
                var response = client.Execute(request);
                return response.StatusCode;
            }
            catch (Exception ex)
            {
                Trace.TraceError("UpdateIssueViaProjectKey(projectKey, updateObject) error: {0}", ex.Message);
                throw new JiraClientException("Could not update issue", ex.Message);
            }
        }

        public HttpStatusCode DeleteCommentViaProjectKey(string projectKey, int commentId)
        {
            if (string.IsNullOrEmpty(projectKey))
                throw new JiraClientException("Project key was empty or null");

            try
            {
                var baseURL = @"https://teamsupportio.atlassian.net";
                var client = new RestClient(baseURL);
                var request = new RestRequest("/rest/api/latest/issue/" + projectKey + "/comment/" + commentId, Method.DELETE);
                request.AddHeader("Authorization", "Basic " + token);
                var response = client.Execute(request);
                return response.StatusCode;
            }
            catch (Exception ex)
            {
                Trace.TraceError("DeleteCommentViaProjectKey(projectKey, commentId) error: {0}", ex.Message);
                throw new JiraClientException("Could not delete comment", ex.Message);
            }
        }

        public HttpStatusCode CreateRemoteLinkViaProjectKey(string projectKey, RemoteLinkAbbreviated remoteLink)
        {
            if (string.IsNullOrEmpty(projectKey) || remoteLink == null)
                throw new JiraClientException("Project key or remote link was empty or null");

            try
            {
                var baseURL = @"https://teamsupportio.atlassian.net";
                var client = new RestClient(baseURL);
                var request = new RestRequest("rest/api/latest/issue/" + projectKey + "/remotelink", Method.POST);
                //  var serializerSettings = new JsonSerializerSettings();
                //  serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

                //request.AddParameter("application/json", JsonConvert.SerializeObject(remoteLink, serializerSettings), ParameterType.RequestBody);
                request.AddParameter("application/json", JsonConvert.SerializeObject(remoteLink), ParameterType.RequestBody);
                request.AddHeader("Authorization", "Basic " + token);

                var response = client.Execute(request);
               // var linkId = deserializer.Deserialize<RemoteLink>(response).id;
                return response.StatusCode;
            }
            catch (Exception ex)
            {
                Trace.TraceError("CreateRemoteLink(issue, remoteLink) error: {0}", ex.Message);
                throw new JiraClientException("Could not create external link for issue", ex.Message);
            }
        }

        public IEnumerable<RemoteLinkRoot> GetRemoteLinkViaProjectKey(string projectKey)
        {
            if (string.IsNullOrEmpty(projectKey))
                throw new JiraClientException("Project key or remote link was empty or null");

            try
            {
                var baseURL = @"https://teamsupportio.atlassian.net";
                var client = new RestClient(baseURL);
                var request = new RestRequest("rest/api/latest/issue/" + projectKey + "/remotelink", Method.GET);
                request.AddHeader("Authorization", "Basic " + token);

                var response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var remoteLink = deserializer.Deserialize<List<RemoteLinkRoot>>(response);
                   // var linkId = deserializer.Deserialize<RemoteLink>(response).id;
                    return remoteLink;
                }
                else
                {
                    return new List<RemoteLinkRoot>();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("CreateRemoteLink(issue, remoteLink) error: {0}", ex.Message);
                throw new JiraClientException("Could not create external link for issue", ex.Message);
            }
        }

        public HttpStatusCode UpdateRemoteLinkViaProjectKeyAndRemoteLinkId(string projectKey, int internalId, RemoteLinkAbbreviated updateObject)
        {
            if (string.IsNullOrEmpty(projectKey))
                throw new JiraClientException("Project key was empty or null");
            if(internalId < 1)
                throw new JiraClientException("Internal Id for remote link was zero or negative");

            try
            {
                var baseURL = @"https://teamsupportio.atlassian.net";
                var client = new RestClient(baseURL);
                var request = new RestRequest("rest/api/latest/issue/" + projectKey + "/remotelink/" + internalId, Method.PUT);//POST will not work with current version of Jira API (v7)
                request.AddParameter("application/json", JsonConvert.SerializeObject(updateObject), ParameterType.RequestBody);
                request.AddHeader("Authorization", "Basic " + token);

                var response = client.Execute(request);
                return response.StatusCode;
            }
            catch (Exception ex)
            {
                Trace.TraceError("UpdateRemoteLinkViaProjectKeyAndRemoteLinkId(projectKey, internalId) error: {0}", ex.Message);
                throw new JiraClientException("Could not update external link for issue", ex.Message);
            }
        }

        public HttpStatusCode DeleteRemoteLinkViaInternalId(string projectKey, int internalId)
        {
            if (string.IsNullOrEmpty(projectKey))
                throw new JiraClientException("Project key was empty or null");
            if (internalId < 1)
                throw new JiraClientException("Internal Id for remote link was zero or negative");

            try
            {
                var baseURL = @"https://teamsupportio.atlassian.net";
                var client = new RestClient(baseURL);
                var request = new RestRequest("rest/api/latest/issue/" + projectKey + "/remotelink/" + internalId, Method.DELETE);
                request.AddHeader("Authorization", "Basic " + token);

                var response = client.Execute(request);
                return response.StatusCode;
            }
            catch (Exception ex)
            {
                Trace.TraceError("DeleteRemoteLinkViaInternalId(projectKey, internalId) error: {0}", ex.Message);
                throw new JiraClientException("Could not delete external link for issue", ex.Message);
            }
        }


        private string DetermineMimeTypeFromOctetStream(byte[] octetStream)
        {

            uint mimetype;
            IntPtr mimeTypePtr = new IntPtr();
            var mime = string.Empty;
<<<<<<< HEAD
=======
            //Reads first 2048 bytes of file to determine mime-type
            //reads entirety of file to make certain of what it is...inefficient!
>>>>>>> c364012ad236c68b013e5deb1570f4103397046d
            try
            {
                Mimes.FindMimeFromData(0, null, octetStream, (uint)octetStream.Length, null, 0, out mimetype, 0);
                mimeTypePtr = new IntPtr(mimetype);
                mime = Marshal.PtrToStringUni(mimeTypePtr);
                Marshal.FreeCoTaskMem(mimeTypePtr);
            }
            catch(Exception ex)
            {
                Marshal.FreeCoTaskMem(mimeTypePtr);
                Trace.TraceError("DetermineMimeTypeFromOctetStream(octetStream) error: {0}", ex.Message);
                throw new JiraClientException("Failed Interop marshalling during Mime Type Sniffing operation", ex.Message);
            }
            return mime;
        }

        public HttpStatusCode CreateAttachmentViaProjectKey(string projectKey, byte[] octetStream)
        {

            var mimeType = DetermineMimeTypeFromOctetStream(octetStream);

            if (string.IsNullOrEmpty(projectKey))
                throw new JiraClientException("Project key was empty or null");
            if (octetStream == null)
                throw new JiraClientException("Attachment was empty or null");

            try
            {
<<<<<<< HEAD
                var fileName = DateTime.Now.ToLocalTime().ToString();
=======
>>>>>>> c364012ad236c68b013e5deb1570f4103397046d
                var baseURL = @"https://teamsupportio.atlassian.net";
                var client = new RestClient(baseURL);
                var request = new RestRequest("rest/api/latest/issue/" + projectKey + "/attachments", Method.POST);
                request.AddHeader("X-Atlassian-Token", "nocheck");
<<<<<<< HEAD
                request.AddFileBytes("file", octetStream, fileName, mimeType);
                request.AddHeader("Authorization", "Basic " + token);

                var response = client.Execute(request);

=======
                request.AddFileBytes("file", octetStream, projectKey + "_" + DateTime.Now, mimeType);
                request.AddHeader("Authorization", "Basic " + token);

                var response = client.Execute(request);
>>>>>>> c364012ad236c68b013e5deb1570f4103397046d
                return response.StatusCode;
            }
            catch (Exception ex)
            {
                Trace.TraceError("DeleteRemoteLinkViaInternalId(projectKey, internalId) error: {0}", ex.Message);
                throw new JiraClientException("Could not delete external link for issue", ex.Message);
            }

        }
<<<<<<< HEAD

        //rest/api/latest/issue/{attachmentId}?fields=attachment
        //public byte[] GetSingleAttachmentViaAttachmentId(string attachmentId)
        //{
        //    //IEnumerable<Attachment> 
        //}


        /// <summary>
        /// Attachment Id is retreived by doing a Get issue(s), first
        /// </summary>
        /// <param name="attachmentId"></param>
        /// <returns>Returns single attachment</returns>
        public IEnumerable<Attachment> GetAttachmentsViaProjectKey(string attachmentId)
        {

            if (string.IsNullOrEmpty(attachmentId))
                throw new JiraClientException("Project key was empty or null");

            try
            {
                var baseURL = @"https://teamsupportio.atlassian.net";
                var client = new RestClient(baseURL);
                var request = new RestRequest("rest/api/latest/issue/attachment/" + attachmentId , Method.GET);
                request.AddHeader("Authorization", "Basic " + token);

                var response = client.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var attachments = deserializer.Deserialize<List<Attachment>>(response);
                    return attachments;
                }
                else
                {
                    return new List<Attachment>();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("DeleteRemoteLinkViaInternalId(projectKey, internalId) error: {0}", ex.Message);
                throw new JiraClientException("Could not delete external link for issue", ex.Message);
            }
        }

        public IEnumerable<Issue> GetIssuesViaProjectKey(string projectKey)
        {
                if (string.IsNullOrEmpty(projectKey))
                throw new JiraClientException("Project key was empty or null");

            try
            {
                var baseURL = @"https://teamsupportio.atlassian.net";
                TechTalk.JiraRestClient.IssueRef testRef = new TechTalk.JiraRestClient.IssueRef();
                testRef.key = "SSP-80";
                TechTalk.JiraRestClient.JiraClient currentClient = new TechTalk.JiraRestClient.JiraClient(baseURL, "jiratest@teamsupport.com", "Muroc2008!");
                var test = currentClient.GetAttachments(testRef);
                
                var myContent = test.LastOrDefault().content;
                var client = new RestClient(baseURL);
                var request = new RestRequest("rest/api/latest/issue/" + projectKey, Method.GET);
                request.AddHeader("Authorization", "Basic " + token);

                var response = client.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var issues = deserializer.Deserialize<List<Issue>>(response);
                    return issues;
                }
                else
                {
                    return new List<Issue>();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("DeleteRemoteLinkViaInternalId(projectKey, internalId) error: {0}", ex.Message);
                throw new JiraClientException("Could not delete external link for issue", ex.Message);
            }
        }

        public IssueRef CreateIssueViaRestClient(string projectKey, string issueType, TIssueFields issueFields)
        {
            throw new NotImplementedException();
        }
=======
>>>>>>> c364012ad236c68b013e5deb1570f4103397046d
    }
}
