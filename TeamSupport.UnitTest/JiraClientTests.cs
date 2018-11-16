using Moq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using TeamSupport.EFData;
using TeamSupport.EFData.Models;
using TeamSupport.JIRA;
using TeamSupport.JIRA.JiraJSONSerializedModels;

namespace TeamSupport.UnitTest
{
    [TestFixture]
    [Category("Jira Client Integration")]
    public class JiraClientTests
    {
        string token, baseURL;
        IJiraClient jiraClient;

        [SetUp]
        public void Init()
        {
            token = @"amlyYXRlc3RAdGVhbXN1cHBvcnQuY29tOk11cm9jMjAwOCE=";//Base64 encoded username:password pattern gets back single-string token
            baseURL = @"https://teamsupportio.atlassian.net";
            jiraClient = new JiraClient(baseURL, "jiratest@teamsupport.com", token);
        }

        [Test]
        public void JiraClient_ShouldBeInstantiatable_WithValidParams()
        {
            //Assert
            Assert.IsNotNull(jiraClient);
            Assert.DoesNotThrow(() => new JiraClient(baseURL, "jiratest@teamsupport.com", token));
        }

        [Test]
        public void JiraClient_ShouldThrowNullReferenceException_WithNullParams()
        {
            //Assert
            Assert.Throws<NullReferenceException>(()=> new JiraClient(null, null, null));
        }

        #region Issues
        [Test]
        public void JiraClient_CreateIssueViaRestClientShouldThrowJiraClientException_WithInvalidParams()
        {
            Assert.Throws<JiraClientException>(() => jiraClient.CreateIssueViaRestClient(null, null, new IssueFields()));
        }

        [TestCase(null, null, null)]
        [TestCase(null, "", null)]
        [TestCase("", null, null)]
        [TestCase("", "", null)]
        public void JiraClient_CreateIssueViaRestClientShouldThrowJiraClientException_WithInvalidParams(string projectKey, string issueType, IssueFields issueFields)
        {
            Assert.Throws<JiraClientException>(() => jiraClient.CreateIssueViaRestClient(projectKey, issueType, issueFields));
        }


        [Test]
        public void JiraClient_ShouldBeAbleToCreateIssueTask_Successfully()
        {
            var issueFields = new IssueFields() { summary = "test summary", description = "some descr" };
            //Act
            var response = jiraClient.CreateIssueViaRestClient("SSP", "Task", issueFields);

            Assert.IsNotNull(response.id);
            Assert.IsNotNull(response.key);
        }

        [Test]
        public void JiraClient_ShouldBeAbleToCreateIssueStory_Successfully()
        {
            var issueFields = new IssueFields() { summary = "test summary", description = "some descr" };
            //Act
            var response = jiraClient.CreateIssueViaRestClient("SSP", "Story", issueFields);

            Assert.IsNotNull(response.id);
            Assert.IsNotNull(response.key);
        }

        [Test]
        public void JiraClient_ShouldBeAbleToGetAllIssues_Successfully()
        {
            //Act
            var response = jiraClient.GetAllIssues();

            Assert.IsNotNull(response);
        }
        
        [Test]
        public void JiraClient_UpdateIssueViaProjectKeyShouldBeAbleToUpdateIssue_Successfully()
        {
            //Arrange
            var updateObject = new UpdateObject() { update = new Update() {
                Description = new List<Description>(), Summary = new List<Summary>()
            } };

            var description = new Description() { Set = "Updating description..." };
            updateObject.update.Description.Add(description);
            var summary = new Summary() { Set = "Updating summary..." };
            updateObject.update.Summary.Add(summary);
            //Act
            var actual = jiraClient.UpdateIssueViaProjectKey("SSP-79", updateObject);
            var expected = HttpStatusCode.NoContent;

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void JiraClient_UpdateIssueViaProjectKeyShouldThrowJiraClientException_WithInvalidParams()
        {
            Assert.Throws<JiraClientException>(() => jiraClient.UpdateIssueViaProjectKey(null, null));
            Assert.Throws<JiraClientException>(() => jiraClient.UpdateIssueViaProjectKey(string.Empty, null));
            Assert.Throws<JiraClientException>(() => jiraClient.UpdateIssueViaProjectKey(string.Empty, new UpdateObject()));
        }

        #endregion

        #region Comments   

        [TestCase(null, null)]
        [TestCase(null, "")]
        [TestCase("", null)]
        [TestCase("", "")]
        public void JiraClient_CreateCommentViaProjectKeyShouldThrowJiraClientException_WithInvalidParams(string projectKey, string comment)
        {
            Assert.Throws<JiraClientException>(() => jiraClient.CreateCommentViaProjectKey(projectKey, comment));
        }

   
        [Test]
        public void JiraClient_CreateCommentViaProjectKey_ShouldBeSuccessful()
        {
            //Arrange
            string projectKey = "SSP-79";
            string comment = "Some crazy comment to update via integration test!";

            //Act
            var actual = jiraClient.CreateCommentViaProjectKey(projectKey, comment);
            var expected = HttpStatusCode.Created;

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void JiraClient_GetCommentViaProjectKeyShouldBeSuccessful_WithValidProjectKey()
        {
            //Arrange
            var projectKey = "SSP-79";

            //Act
            var actual = jiraClient.GetCommentsViaProjectKey(projectKey);

            Assert.IsNotNull(actual.FirstOrDefault());
        }

        [TestCase("SomeProjectKeyThatDoesNotExist")]
        [TestCase("SSP-93")]
        public void JiraClient_GetCommentViaProjectKeyShouldReturnEmptyIEnumerableComment_WithInvalidProjectKeyOrValidProjectKeyWithNoComments(string projectKey)
        {
            //Act
            var actual = jiraClient.GetCommentsViaProjectKey(projectKey);

            //Assert
            Assert.IsNull(actual.FirstOrDefault());
        }

        [Test]
        public void JiraClient_GetCommentViaProjectKeyShouldThrowException_WithInvalidParams()
        {
            Assert.Throws<JiraClientException>(() => jiraClient.GetCommentsViaProjectKey(null));
        }

        [Test]
        public void JiraClient_DeleteCommentViaProjectKeyShouldThrowException_WithInvalidParams()
        {
            Assert.Throws<JiraClientException>(() => jiraClient.DeleteCommentViaProjectKey(null, 0));
        }

        /// <summary>
        /// This will delete all comments for all project keys for a given user credential
        /// </summary>
        [Test]
        public void JiraClient_DeleteCommentViaProjectKeyShouldBeSuccessful_WithValidParams()
        {
            //Arrange
            var projectKeys = jiraClient.GetAllIssues();
            IEnumerable<Comment> comments;
            int commentId = 0;
            HttpStatusCode actual = HttpStatusCode.Unused;
            //Act

            foreach (var current in projectKeys)
            {
                comments = jiraClient.GetCommentsViaProjectKey(current.key);
                if (comments.Count() > 0)
                {
                    foreach (var comment in comments)
                    {
                        commentId = int.Parse(comment.id);
                        actual = jiraClient.DeleteCommentViaProjectKey(current.key, commentId);
                    }
                }
            }

            var expected = HttpStatusCode.NoContent;

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-999)]
        [TestCase(int.MinValue)]
        [TestCase(int.MaxValue)]
        public void JiraClient_DeleteCommentViaProjectKeyShouldBeUnsuccessful_WithInvalidCommentId(int commentId)
        {
            //Act
            var response = jiraClient.GetAllIssues();

            var actual = jiraClient.DeleteCommentViaProjectKey(response.FirstOrDefault().key, commentId);
            var expected = HttpStatusCode.NotFound;

            //Assert
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// This will update all existing comments for all project keys
        /// </summary>
        [Test]
        public void JiraClient_UpdateCommentViaProjectKeyShouldBeSuccessfull_WithValidParams()
        {
            var projectKeys = jiraClient.GetAllIssues();
            IEnumerable<Comment> comments;
            int commentId = 0;
            HttpStatusCode actual = HttpStatusCode.Unused;
            Comment response;
            foreach (var current in projectKeys)
            {
                comments = jiraClient.GetCommentsViaProjectKey(current.key);
                if (comments.Count() > 0)
                {
                    foreach (var comment in comments.Reverse())
                    {
                        commentId = int.Parse(comment.id);
                        response = jiraClient.UpdateCommentViaProjectKey(current.key, commentId, "comment updated!");
                        Assert.NotNull(response);
                    }
                }
            }           
        }

        [Test]
        public void JiraClient_UpdateCommentViaProjectKeyShouldThrowException_WithEmptyProjectKey()
        {
            Assert.Throws<JiraClientException>(() => jiraClient.UpdateCommentViaProjectKey(string.Empty, -1, string.Empty));
        }

        #endregion

        #region Remote Link

        #endregion
    }
}
