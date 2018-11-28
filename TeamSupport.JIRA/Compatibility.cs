﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using TeamSupport.JIRA.JiraJSONSerializedModels;

namespace TeamSupport.JIRA
{
    public interface IJiraClient
    {
		/// <summary>Returns the metadata for the given project+issuetype</summary>
		IssueMetaData.RootObject GetIssueMetaData(string projectKey, string issueType);

		/// <summary>Returns all issues for the given project</summary>
		IEnumerable<Issue> GetIssuesViaProjectKey(String projectKey);
        /// <summary>Returns all issues of the specified type for the given project</summary>
        IEnumerable<Issue> GetIssues(String projectKey, String issueType);
        /// <summary>Enumerates through all issues for the given project</summary>
        IEnumerable<Issue> EnumerateIssues(String projectKey);
        /// <summary>Enumerates through all issues of the specified type for the given project</summary>
        IEnumerable<Issue> EnumerateIssues(String projectKey, String issueType);

        /// <summary>Returns the issue identified by the given ref</summary>
        Issue LoadIssue(String issueRef);
        /// <summary>Returns the issue identified by the given ref</summary>
        Issue LoadIssue(IssueRef issueRef);
        IssueRef CreateIssueViaRestClient(String projectKey, String issueType, IssueFields issueFields);
        /// <summary>Creates an issue of the specified type for the given project</summary>
        Issue CreateIssue(String projectKey, String issueType, String summary);
        /// <summary>Creates an issue of the specified type for the given project</summary>
        Issue CreateIssue(String projectKey, String issueType, IssueFields issueFields);
        Comment UpdateCommentViaProjectKey(string projectKey, int commentId, String comment);
        /// <summary>Updates the given issue on the remote system</summary>
        Issue UpdateIssue(Issue issue);
		/// <summary>Updates the specific field on the given issue (by id)</summary>
		bool UpdateIssueField(int issueId, string fieldName, string fieldValue);
		/// <summary>Updates the specific field on the given issue (by id) with the json value already</summary>
		bool UpdateIssueFieldByParameter(int issueId, string jsonBody);
		/// <summary>Deletes the given issue from the remote system</summary>
		void DeleteIssue(IssueRef issue);

        /// <summary>Returns all transitions avilable to the given issue</summary>
        IEnumerable<Transition> GetTransitions(IssueRef issue);
        /// <summary>Changes the state of the given issue as described by the transition</summary>
        Issue TransitionIssue(IssueRef issue, Transition transition);

        /// <summary>Returns all watchers for the given issue</summary>
        IEnumerable<JiraUser> GetWatchers(IssueRef issue);

        /// <summary>Returns all comments for the given issue</summary>
        IEnumerable<Comment> GetComments(IssueRef issue);
        IEnumerable<Issue<IssueFields>> GetAllIssues();

        /// <summary>Adds a comment to the given issue</summary>
        Comment CreateComment(IssueRef issue, String comment);
        HttpStatusCode CreateCommentViaProjectKey(string projectKey, string comment);
        /// <summary>Deletes the given comment</summary>
        void DeleteComment(IssueRef issue, Comment comment);

        /// <summary>Return all attachments for the given issue</summary>
        IEnumerable<Attachment> GetAttachments(IssueRef issue);
        /// <summary>Creates an attachment to the given issue</summary>
        Attachment CreateAttachment(IssueRef issue, Stream stream, String fileName);
        /// <summary>Deletes the given attachment</summary>
        void DeleteAttachment(Attachment attachment);

        /// <summary>Returns all links for the given issue</summary>
        IEnumerable<IssueLink> GetIssueLinks(IssueRef issue);
        HttpStatusCode UpdateIssueViaProjectKey(string v, UpdateObject updateObject);

        /// <summary>Returns the link between two issues of the given relation</summary>
        IssueLink LoadIssueLink(IssueRef parent, IssueRef child, String relationship);
        /// <summary>Creates a link between two issues with the given relation</summary>
        IssueLink CreateIssueLink(IssueRef parent, IssueRef child, String relationship);
        /// <summary>Removes the given link of two issues</summary>
        void DeleteIssueLink(IssueLink link);

        /// <summary>Returns all remote links (attached urls) for the given issue</summary>
        IEnumerable<RemoteLink> GetRemoteLinks(IssueRef issue);
        /// <summary>Creates a remote link (attached url) for the given issue</summary>
        RemoteLink CreateRemoteLink(IssueRef issue, RemoteLink remoteLink, string globalId);
        HttpStatusCode DeleteIssueViaProjectKey(string projectKey);

        /// <summary>Updates the given remote link (attached url) of the specified issue</summary>
        RemoteLink UpdateRemoteLink(IssueRef issue, RemoteLink remoteLink);
        /// <summary>Removes the given remote link (attached url) of the specified issue</summary>
        void DeleteRemoteLink(IssueRef issue, RemoteLink remoteLink);

        /// <summary>Returns all issue types</summary>
        IEnumerable<IssueType> GetIssueTypes();

        ///<summary>Returns all projects</summary>
        IEnumerable<Project> GetProjects();

        /// <summary>Returns information about the JIRA server</summary>
        ServerInfo GetServerInfo();
        IEnumerable<Comment> GetCommentsViaProjectKey(string projectKey);
        HttpStatusCode DeleteCommentViaProjectKey(string projectKey, int commentId);
        HttpStatusCode CreateRemoteLinkViaProjectKey(string projectKey, RemoteLinkAbbreviated remoteLinkObject);
        IEnumerable<RemoteLinkRoot> GetRemoteLinkViaProjectKey(string projectKey);
        HttpStatusCode UpdateRemoteLinkViaProjectKeyAndRemoteLinkId(string projectKey, int internalId, RemoteLinkAbbreviated remoteLink);
        HttpStatusCode DeleteRemoteLinkViaInternalId(string projectKey, int internalId);
        HttpStatusCode CreateAttachmentViaProjectKey(string projectKey, byte[] octetStream);
<<<<<<< HEAD
        IEnumerable<Attachment> GetAttachmentsViaProjectKey(string key);
=======
>>>>>>> c364012ad236c68b013e5deb1570f4103397046d
    }

    public class JiraClient : IJiraClient
    {
        private readonly IJiraClient<IssueFields> client;
        public JiraClient(string baseUrl, string username, string token)
        {
            client = new JiraClient<IssueFields>(baseUrl, username, token);
        }

		public JiraClient(string baseUrl, string username, string token, string apiPath)
		{
            client = new JiraClient<IssueFields>(baseUrl, username, token);
		}

		public IssueMetaData.RootObject GetIssueMetaData(string projectKey, string issueType)
		{
			return client.GetIssueMetaData(projectKey, issueType);
		}

        public IEnumerable<Issue> GetIssuesViaProjectKey(String projectKey)
        {
            return client.GetIssuesViaProjectKey(projectKey).Select(Issue.From).ToArray();
        }

        public IEnumerable<Issue> GetIssues(String projectKey, String issueType)
        {
            return client.GetIssues(projectKey, issueType).Select(Issue.From).ToArray();
        }

        public IEnumerable<Issue> EnumerateIssues(String projectKey)
        {
            return client.EnumerateIssues(projectKey).Select(Issue.From);
        }

        public IEnumerable<Issue> EnumerateIssues(String projectKey, String issueType)
        {
            return client.EnumerateIssues(projectKey, issueType).Select(Issue.From);
        }

        public Issue LoadIssue(String issueRef)
        {
            return Issue.From(client.LoadIssue(issueRef));
        }

        public Issue LoadIssue(IssueRef issueRef)
        {
            return Issue.From(client.LoadIssue(issueRef));
        }

        public Issue CreateIssue(String projectKey, String issueType, String summary)
        {
            return Issue.From(client.CreateIssue(projectKey, issueType, summary));
        }

        public Issue CreateIssue(String projectKey, String issueType, IssueFields issueFields)
        {
            return Issue.From(client.CreateIssue(projectKey, issueType, issueFields));
        }

        public Issue UpdateIssue(Issue issue)
        {
            return Issue.From(client.UpdateIssue(issue));
        }

		public bool UpdateIssueField(int issueId, string fieldName, string fieldValue)
		{
			return client.UpdateIssueField(issueId, fieldName, fieldValue);
		}

		public bool UpdateIssueFieldByParameter(int issueId, string jsonBody)
		{
			return client.UpdateIssueFieldByParameter(issueId, jsonBody);
		}

		public bool UpdateIssueFields(int issueId, Dictionary<string, string> updateFields)
		{
			return client.UpdateIssueFields(issueId, updateFields);
		}

		public void DeleteIssue(IssueRef issue)
        {
            client.DeleteIssue(issue);
        }

        public IEnumerable<Transition> GetTransitions(IssueRef issue)
        {
            return client.GetTransitions(issue);
        }

        public Issue TransitionIssue(IssueRef issue, Transition transition)
        {
            return Issue.From(client.TransitionIssue(issue, transition));
        }

        public IEnumerable<JiraUser> GetWatchers(IssueRef issue)
        {
            return client.GetWatchers(issue);
        }

        public IEnumerable<Comment> GetComments(IssueRef issue)
        {
            return client.GetComments(issue);
        }

        public Comment CreateComment(IssueRef issue, string comment)
        {
            return client.CreateComment(issue, comment);
        }

        public HttpStatusCode CreateCommentViaProjectKey(string projectKey, string comment)
        {
            return client.CreateCommentViaProjectKey(projectKey, comment);
        }

		public Comment UpdateComment(IssueRef issue, int commentId, string comment)
		{
			return client.UpdateComment(issue, commentId, comment);
		}

		public void DeleteComment(IssueRef issue, Comment comment)
        {
            client.DeleteComment(issue, comment);
        }

        public IEnumerable<Attachment> GetAttachments(IssueRef issue)
        {
            return client.GetAttachments(issue);
        }

        public Attachment CreateAttachment(IssueRef issue, Stream stream, string fileName)
        {
            return client.CreateAttachment(issue, stream, fileName);
        }

        public void DeleteAttachment(Attachment attachment)
        {
            client.DeleteAttachment(attachment);
        }

        public IEnumerable<IssueLink> GetIssueLinks(IssueRef issue)
        {
            return client.GetIssueLinks(issue);
        }

        public IssueLink LoadIssueLink(IssueRef parent, IssueRef child, string relationship)
        {
            return client.LoadIssueLink(parent, child, relationship);
        }

        public IssueLink CreateIssueLink(IssueRef parent, IssueRef child, string relationship)
        {
            return client.CreateIssueLink(parent, child, relationship);
        }

        public void DeleteIssueLink(IssueLink link)
        {
            client.DeleteIssueLink(link);
        }

        public IEnumerable<RemoteLink> GetRemoteLinks(IssueRef issue)
        {
            return client.GetRemoteLinks(issue);
        }

        public RemoteLink CreateRemoteLink(IssueRef issue, RemoteLink remoteLink, string globalId)
        {
            return client.CreateRemoteLink(issue, remoteLink, globalId);
        }

        public RemoteLink UpdateRemoteLink(IssueRef issue, RemoteLink remoteLink)
        {
            return client.UpdateRemoteLink(issue, remoteLink);
        }

        public void DeleteRemoteLink(IssueRef issue, RemoteLink remoteLink)
        {
            client.DeleteRemoteLink(issue, remoteLink);
        }

        public IEnumerable<IssueType> GetIssueTypes()
        {
            return client.GetIssueTypes();
        }

        public IEnumerable<Project> GetProjects()
        {
          return client.GetProjects();
        }

		public Sprint GetSprintById(int id)
		{
			return client.GetSprintById(id);
        }

        public IEnumerable<Sprint> GetSprintsByBoardId(int boardId)
		{
			return client.GetSprintsByBoardId(boardId);
		}

		public IEnumerable<Board> GetBoards()
		{
			return client.GetBoards();
        }

		public ServerInfo GetServerInfo()
        {
            return client.GetServerInfo();
        }

        public IssueRef CreateIssueViaRestClient(string v1, string v2, IssueFields issueFields)
        {
            return client.CreateIssueViaRestClient(v1, v2, issueFields);
        }

        public IEnumerable<Comment> GetCommentsViaProjectKey(string v)
        {
            return client.GetCommentsViaProjectKey(v);
        }

        public Comment UpdateCommentViaProjectKey(string v1, int x, string v2  )
        {
            return client.UpdateCommentViaProjectKey(v1, x, v2);
        }

        public HttpStatusCode UpdateIssueViaProjectKey(string v, UpdateObject updateObject)
        {
            return client.UpdateIssueViaProjectKey(v, updateObject);
        }

        public HttpStatusCode DeleteCommentViaProjectKey(string projectKey, int commentId)
        {
            return client.DeleteCommentViaProjectKey(projectKey, commentId);
        }

       public IEnumerable<Issue<IssueFields>> GetAllIssues()
        {
            return client.GetAllIssues();
        }

        public HttpStatusCode DeleteIssueViaProjectKey(string projectKey)
        {
            return client.DeleteIssueViaProjectKey(projectKey);
        }

        public HttpStatusCode CreateRemoteLinkViaProjectKey(string projectKey, RemoteLinkAbbreviated remoteLink)
        {
            return client.CreateRemoteLinkViaProjectKey(projectKey, remoteLink);
        }

        public IEnumerable<RemoteLinkRoot> GetRemoteLinkViaProjectKey(string projectKey)
        {
            return client.GetRemoteLinkViaProjectKey(projectKey);
        }

        public HttpStatusCode UpdateRemoteLinkViaProjectKeyAndRemoteLinkId(string projectKey, int internalId, RemoteLinkAbbreviated remoteLink)
        {
            return client.UpdateRemoteLinkViaProjectKeyAndRemoteLinkId(projectKey, internalId, remoteLink);
        }

        public HttpStatusCode DeleteRemoteLinkViaInternalId(string projectKey, int internalId)
        {
            return client.DeleteRemoteLinkViaInternalId(projectKey, internalId);
        }

        public HttpStatusCode CreateAttachmentViaProjectKey(string projectKey, byte[] octetStream)
        {
            return client.CreateAttachmentViaProjectKey(projectKey, octetStream);
        }
<<<<<<< HEAD

        public IEnumerable<Attachment> GetAttachmentsViaProjectKey(string projectKey)
        {
            return client.GetAttachmentsViaProjectKey(projectKey);
        }
=======
>>>>>>> c364012ad236c68b013e5deb1570f4103397046d
    }

    public class Issue : Issue<IssueFields>
    {
        internal static Issue From(Issue<IssueFields> other)
        {
            if (other == null)
                return null;

            return new Issue
            {
                expand = other.expand,
                id = other.id,
                key = other.key,
                self = other.self,
                fields = other.fields,
            };
        }
    }
}
