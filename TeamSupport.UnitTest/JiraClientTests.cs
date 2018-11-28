using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using TeamSupport.JIRA;
using TeamSupport.JIRA.JiraJSONSerializedModels;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace TeamSupport.UnitTest
{
    [TestFixture]
    [Category("Jira Client Integration")]
    public class JiraClientTests
    {
        string token, baseURL;
        IJiraClient jiraClient;
        IEnumerable<Issue<IssueFields>> projectKeys;

        [SetUp]
        public void Init()
        {
            //Base64 encoded username:password pattern gets back single-string token
            token = @"amlyYXRlc3RAdGVhbXN1cHBvcnQuY29tOk11cm9jMjAwOCE=";
            baseURL = @"https://teamsupportio.atlassian.net";
            jiraClient = new JiraClient(baseURL, "jiratest@teamsupport.com", token);
            projectKeys = jiraClient.GetAllIssues();
        }

        [DllImport(@"urlmon.dll", CharSet = CharSet.Auto)]
        private extern static uint FindMimeFromData(uint pBC,
        [MarshalAs(UnmanagedType.LPStr)] string pwzUrl,
        [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer,
        uint cbSize,
        [MarshalAs(UnmanagedType.LPStr)] string pwzMimeProposed,
        uint dwMimeFlags,
        out uint ppwzMimeOut,
        uint dwReserverd);


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
            Assert.Throws<NullReferenceException>(() => new JiraClient(null, null, null));
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
        public void JiraClient_ShouldBeAbleToGetIssueViaProjectKey_Successfully()
        {
            //Act
            var response = jiraClient.GetIssuesViaProjectKey(projectKeys.LastOrDefault().key);

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
            if (projectKeys.Any())
            {
                var actual = jiraClient.UpdateIssueViaProjectKey(projectKeys.LastOrDefault().key, updateObject);
                var expected = HttpStatusCode.NoContent;

                //Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void JiraClient_UpdateIssueViaProjectKeyShouldThrowJiraClientException_WithInvalidParams()
        {
            Assert.Throws<JiraClientException>(() => jiraClient.UpdateIssueViaProjectKey(null, null));
            Assert.Throws<JiraClientException>(() => jiraClient.UpdateIssueViaProjectKey(string.Empty, null));
            Assert.Throws<JiraClientException>(() => jiraClient.UpdateIssueViaProjectKey(string.Empty, new UpdateObject()));
        }

        [Test]
        public void JiraClient_DeleteIssueViaProjectKeyShouldThrowJiraClientException_WithInvalidParams()
        {
            Assert.Throws<JiraClientException>(() => jiraClient.DeleteIssueViaProjectKey(null));
            Assert.Throws<JiraClientException>(() => jiraClient.DeleteIssueViaProjectKey(string.Empty));
        }

        //204 No Content = successful
        //404 Not Found = unsuccessful
        [Test]
        public void JiraClient_DeleteIssueViaProjectKeyShouldReturnHttpStatusCodeNotFound_WhenDeletingNonExistentIssue()
        {
            //Arrange
            var projectKeyToDeleteBy = string.Empty;
            HttpStatusCode expected = HttpStatusCode.NotFound;
            if (projectKeys.Any())
            {
                var splitString = projectKeys.FirstOrDefault().key.Split('-');
                var keyAsInt = int.Parse(splitString[1]);
                keyAsInt++;
                projectKeyToDeleteBy = splitString[0] + "-" + keyAsInt.ToString();
            }

            //Act
            var actual = jiraClient.DeleteIssueViaProjectKey(projectKeyToDeleteBy);

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void JiraClient_DeleteIssueViaProjectKeyShouldReturnHttpStatusCodeNoContent_WhenValidParams()
        {
            //Arrange
            var projectKeyToDeleteBy = string.Empty;
            HttpStatusCode expected = HttpStatusCode.Unused;
            if (projectKeys.Any())
            {
                projectKeyToDeleteBy = projectKeys.FirstOrDefault().key;
                expected = HttpStatusCode.NoContent;
            }
            else
            {
                expected = HttpStatusCode.NotFound;
            }

            //Act
            var actual = jiraClient.DeleteIssueViaProjectKey(projectKeyToDeleteBy);


            //Assert
            Assert.AreEqual(expected, actual);
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
            string projectKey = string.Empty;
            HttpStatusCode expected = HttpStatusCode.Unused;
            if (projectKeys.Any())
            {
                projectKey = projectKeys.FirstOrDefault().key;
                expected = HttpStatusCode.Created;
            }
            else
            {
                expected = HttpStatusCode.NotFound;
            }
            string comment = "Some comment to update via integration test!";

            //Act
            var actual = jiraClient.CreateCommentViaProjectKey(projectKey, comment);

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase("expected comment")]
        [TestCase("-999")]
        [TestCase("*")]
        [TestCase("ftDqFzzAd2SU76fkc2zN")]
        [TestCase("Bc5EOnBR7hFWX9rCKt5D")]
        public void JiraClient_GetCommentViaProjectKeyShouldBeSuccessful_WithValidProjectKey(string expectedComment)
        {
            //Arrange
            HttpStatusCode createdComment = HttpStatusCode.Unused;
            if (projectKeys.Any())
            {
                createdComment = jiraClient.CreateCommentViaProjectKey(projectKeys.FirstOrDefault().key, expectedComment);
            }

            if (createdComment == HttpStatusCode.Created)
            {
                //Act
                var actual = jiraClient.GetCommentsViaProjectKey(projectKeys.FirstOrDefault().key);

                Assert.IsNotNull(actual.LastOrDefault());
                Assert.AreEqual(actual.LastOrDefault().body, expectedComment);
            }
        }

        [TestCase("SomeProjectKeyThatDoesNotExist")]
        [TestCase("ftDqFzzAd2SU76fkc2zN")]
        [TestCase("*")]
        [TestCase("-999")]
        public void JiraClient_GetCommentViaProjectKeyShouldReturnEmptyIEnumerableComment_WithInvalidProjectKey(string projectKey)
        {
            //Act
            var actual = jiraClient.GetCommentsViaProjectKey(projectKey);

            //Assert
            Assert.IsNull(actual.FirstOrDefault());
        }

        [Test]
        public void JiraClient_GetCommentViaProjectKeyShouldReturnEmptyIEnumerableComment_WithValidProjectKeyWithNoComments()
        {
            //Arrange
            string issueToGetEmptyComments = string.Empty;
            if (projectKeys.Any())
            {
                foreach (var currentKey in projectKeys)
                {
                    var issueComments = jiraClient.GetCommentsViaProjectKey(currentKey.key);
                    if (!issueComments.Any())
                    {
                        issueToGetEmptyComments = currentKey.key;
                        break;
                    }
                }
            }

            //Act
            var actual = jiraClient.GetCommentsViaProjectKey(issueToGetEmptyComments);

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
        [Test, Order(2)]
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
        [Test, Order(1)]
        public void JiraClient_UpdateCommentViaProjectKeyShouldBeSuccessfull_WithValidParams()
        {
            var projectKeys = jiraClient.GetAllIssues();
            IEnumerable<Comment> comments;
            int commentId = 0;
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
        //Create
        [Test]
        public void JiraClient_ShouldCreateRemoteIssueLink_WithValidParams()
        {
            //Arrange
            string projectKey = string.Empty;
            HttpStatusCode expected = HttpStatusCode.Unused;
            if (projectKeys.Any())
            {
                projectKey = projectKeys.FirstOrDefault().key;
                expected = HttpStatusCode.Created;
            }
            else
            {
                expected = HttpStatusCode.NotFound;
            }
            var remoteLink = new RemoteLinkAbbreviated();
            remoteLink.Object = new Dictionary<string, string>();
            remoteLink.Object.Add("url", "https://host.domain");
            remoteLink.Object.Add("title", "Crazy customer support issue");

            //Act
            var actual = jiraClient.CreateRemoteLinkViaProjectKey(projectKey, remoteLink);

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void JiraClient_ShouldThrowJiraClientExceptionToCreateRemoteIssueLink_WithInvalidParams()
        {
            //Assert
            Assert.Throws<JiraClientException>(() => jiraClient.CreateRemoteLinkViaProjectKey(null, null));
        }


        [Test]
        public void JiraClient_ShouldFailToCreateRemoteIssueLink_WithForNonExistentIssue()
        {
            //Arrange
            var remoteLink = new RemoteLinkAbbreviated();
            remoteLink.Object = new Dictionary<string, string>();
            remoteLink.Object.Add("url", "https://host.domain");
            remoteLink.Object.Add("title", "Crazy customer support issue");

            //Act
            var actual = jiraClient.CreateRemoteLinkViaProjectKey("SomeNonExistentIssue", remoteLink);

            //Assert
            Assert.AreEqual(HttpStatusCode.NotFound, actual);
        }

        [TestCase("*")]
        [TestCase("")]
        [TestCase("-999")]
        [TestCase("SomeTitle")]
        public void JiraClient_ShouldFailToCreateRemoteIssueLink_WithInvalidMediaType(string title)
        {
            //Arrange            
            string projectKey = string.Empty;
            HttpStatusCode expected = HttpStatusCode.Unused;
            if (projectKeys.Any())
            {
                projectKey = projectKeys.FirstOrDefault().key;
                expected = HttpStatusCode.BadRequest;
            }
            else
            {
                expected = HttpStatusCode.NotFound;
            }
            var remoteLink = new RemoteLinkAbbreviated();
            remoteLink.Object = new Dictionary<string, string>();
            remoteLink.Object.Add("url", "Invalid media type");
            remoteLink.Object.Add("title", title);

            //Act
            var actual = jiraClient.CreateRemoteLinkViaProjectKey(projectKey, remoteLink);

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, actual);
        }

        //Get Remote Link(s)
        [Test]
        public void JiraClient_GetRemoteLinkViaProjectKeyShouldGetRemoteLink_WithValidParams()
        {
            //Arrange            
            string projectKey = string.Empty;
            if (projectKeys.Any())
            {
                projectKey = projectKeys.FirstOrDefault().key;
            }

            //Act
            var actual = jiraClient.GetRemoteLinkViaProjectKey(projectKey);

            //Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void JiraClient_GetRemoteLinkViaProjectKeyShouldThrowJiraClientException_WithNullParams()
        {
            Assert.Throws<JiraClientException>(() => jiraClient.GetRemoteLinkViaProjectKey(null));
        }

        [Test]
        public void JiraClient_ShouldFailToUpdateRemoteLinkViaProjectKey_WithInvalidRemoteLinkId()
        {
            //Arrange            
            if (projectKeys.Any())
            {
                foreach (var currentKey in projectKeys)
                {
                    var existingRemoteLink = jiraClient.GetRemoteLinkViaProjectKey(currentKey.key);
                    if (existingRemoteLink.Any())
                    {
                        var actual = jiraClient.UpdateRemoteLinkViaProjectKeyAndRemoteLinkId(currentKey.key, int.MaxValue, new RemoteLinkAbbreviated());
                        Assert.AreEqual(HttpStatusCode.NotFound, actual);
                    }
                }
            }
        }

        //Update
        [Test]
        public void JiraClient_ShouldUpdateRemoteIssueLink_WithValidParams()
        {
            //Arrange            
            if (projectKeys.Any())
            {
                foreach (var currentKey in projectKeys)
                {
                    var existingRemoteLink = jiraClient.GetRemoteLinkViaProjectKey(currentKey.key);
                    if (existingRemoteLink.Any())
                    {
                        var remoteLink = new RemoteLinkAbbreviated();
                        remoteLink.Object = new Dictionary<string, string>();
                        remoteLink.Object.Add("url", @"https://host.domain");
                        remoteLink.Object.Add("title", "some title");

                        var actual = jiraClient.UpdateRemoteLinkViaProjectKeyAndRemoteLinkId(currentKey.key, existingRemoteLink.FirstOrDefault().id, remoteLink);
                        Assert.AreEqual(HttpStatusCode.NoContent, actual);
                    }
                }
            }
        }

        [TestCase("*")]
        [TestCase("")]
        [TestCase("-999")]
        [TestCase("SomeTitle")]
        public void JiraClient_ShouldReturnBadRequestFromUpdateRemoteIssueLink_WithIncorrectRequestParams(string title)
        {
            //Arrange            
            if (projectKeys.Any())
            {
                foreach (var currentKey in projectKeys)
                {
                    var existingRemoteLink = jiraClient.GetRemoteLinkViaProjectKey(currentKey.key);
                    if (existingRemoteLink.Any())
                    {
                        var remoteLink = new RemoteLinkAbbreviated();
                        remoteLink.Object = new Dictionary<string, string>();
                        remoteLink.Object.Add("url", @"Non-existent URL creates bad request");
                        remoteLink.Object.Add("title", title);

                        var actual = jiraClient.UpdateRemoteLinkViaProjectKeyAndRemoteLinkId(currentKey.key, existingRemoteLink.FirstOrDefault().id, remoteLink);
                        Assert.AreEqual(HttpStatusCode.BadRequest, actual);
                    }
                }
            }
        }

        //Delete
        //204 if successful
        //404 if unsuccessful
        [Test]
        public void JiraClient_ShouldDeleteRemoteIssueLinkViaProjectKey_WithValidParams()
        {
            string projectKey = string.Empty;
            HttpStatusCode expected = HttpStatusCode.Unused;
            HttpStatusCode actual = HttpStatusCode.Unused;
            if (projectKeys.Any())
            {
                projectKey = projectKeys.FirstOrDefault().key;
                expected = HttpStatusCode.NoContent;
            }
            else
            {
                expected = HttpStatusCode.NotFound;
            }

            //Act
            JiraClient_ShouldCreateRemoteIssueLink_WithValidParams();
            var internalIdList = jiraClient.GetRemoteLinkViaProjectKey(projectKey);

            foreach (var id in internalIdList)
            {
                actual = jiraClient.DeleteRemoteLinkViaInternalId(projectKey, id.id);
            }

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void JiraClient_DeleteRemoteIssueLinkViaInternalIdShouldThrowJiraClientException_WithNullParams()
        {
            //Assert
            Assert.Throws<JiraClientException>(() => jiraClient.DeleteRemoteLinkViaInternalId(null, 0));
        }

        #endregion

        #region Attachments
        //Create
        //Average size of all attachments is 246KB
        //Average size of Jira-specific attachments is 336KB
        //Largest jira-specific non-TOKService attachment as of 11/21/2018: 69.291297MB
        [Test]
        public void JiraClient_ShouldCreateAnAttachmentForExistingIssue_WithValidParams()
        {
            if (projectKeys.Any())
            {
                //50kb image
                #region base64String
                string base64String = @"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
/9k=";
                #endregion
                byte[] octetStream = Convert.FromBase64String(base64String);
                HttpStatusCode expected = HttpStatusCode.OK;
                //Act
                var actual = jiraClient.CreateAttachmentViaProjectKey(projectKeys.LastOrDefault().key, octetStream);
                //Assert
                Assert.AreEqual(expected, actual);
            }
        }

       

        [Test]
        public void JiraClient_ShouldCreateAnAttachmentForExistingIssueWithLargeFile_WithValidParams()
        {
            //Arrange
            var base64String = string.Empty;
            var myBuiltString = new StringBuilder();
            if (projectKeys.Any())
            {
                //Going to need a buffer to read in
                //// pathSource = @"C:\images\SomeDoc.txt";
               var pathSource = @"C:\images\largeVideo.mp4";
                IntPtr mimeTypePtr = new IntPtr();
                try
                {
                    //Act
                    mimeTypePtr = GetFileAndPerformanceTest( pathSource);
                }
                catch (Exception ex)
                {
                    Marshal.FreeCoTaskMem(mimeTypePtr);
                    Console.WriteLine(ex.Message);
                }

            }
        }

        private IntPtr GetFileAndPerformanceTest(string pathSource)
        {
            //Arrange
            IntPtr mimeTypePtr;
            using (FileStream fsSource = new FileStream(pathSource,
                FileMode.Open, FileAccess.Read))
            {
                byte[] bytes = new byte[fsSource.Length];
                uint mimetype;
                FindMimeFromData(0, null, bytes, (uint)fsSource.Length, null, 0, out mimetype, 0);
                mimeTypePtr = new IntPtr(mimetype);
                string mime = Marshal.PtrToStringUni(mimeTypePtr);
                Marshal.FreeCoTaskMem(mimeTypePtr);
               
                HttpStatusCode expected = HttpStatusCode.OK;
                byte[] octetStream = bytes;
                var actual = jiraClient.CreateAttachmentViaProjectKey(projectKeys.LastOrDefault().key, octetStream);
                //Assert
                Assert.AreEqual(expected, actual);
            }

            return mimeTypePtr;
        }

        [Test]
        public void JiraClient_ShouldCreateAnAttachmentForExistingIssue_WithTerseOctectStream()
        {
            if (projectKeys.Any())
            {
                byte[] octetStream = new byte['a'];
                HttpStatusCode expected = HttpStatusCode.OK;

                var actual = jiraClient.CreateAttachmentViaProjectKey(projectKeys.LastOrDefault().key, octetStream);

                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void JiraClient_CreateAnAttachmentForExistingIssueShouldThrowJiraClientException_WithInvalidParams()
        {
            Assert.Throws<JiraClientException>(() => jiraClient.CreateAttachmentViaProjectKey(null, null));
            if (projectKeys.Any())
                Assert.Throws<JiraClientException>(() => jiraClient.CreateAttachmentViaProjectKey(projectKeys.LastOrDefault().key, null));
        }

        //Get
        [Test]
        public void JiraClient_GetAnAttachmentShouldReturnContents_WithValidParams()
        {
            if (projectKeys.Any())
            {
                //Arrange
                HttpStatusCode expected = HttpStatusCode.OK;

                var actual = jiraClient.GetAttachmentsViaProjectKey(projectKeys.LastOrDefault().key);

                //Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void JiraClient_GetAnAttachmentFail_WithInvalidParams()
        {

        }

        //Update
        [Test]
        public void JiraClient_UpdateAnAttachmentShouldReturnContents_WithValidParams()
        {

        }

        [Test]
        public void JiraClient_UpdateAnAttachmentFail_WithInvalidParams()
        {

        }
               
        //Delete
        [Test]
        public void JiraClient_DeleteAnAttachmentShouldReturnContents_WithValidParams()
        {

        }

        [Test]
        public void JiraClient_DeleteAnAttachmentFail_WithInvalidParams()
        {

        }
        #endregion
    }
}
