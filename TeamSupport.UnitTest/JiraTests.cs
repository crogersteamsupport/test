using NUnit.Framework;
using System;
using System.Linq;
using TeamSupport.EFData;
using TeamSupport.EFData.Models;
using TeamSupport.JIRA;

namespace TeamSupport.UnitTest
{
    [TestFixture, Category("Jira Integration")]
    public class JiraTests
    {
        IJiraRepository jiraRepository;
        [SetUp]
        public void InitJiraRepo()
        {
            jiraRepository = new JiraRepository(new GenericRepository<TicketLinkToJira>());
        }

        [Test]
        public void JiraClient_ConstructorShouldThrowUriFormatException_WithEmptyStringParams()
        {
            //Assert
             Assert.Throws<UriFormatException>(() => new JiraClient(string.Empty, string.Empty, string.Empty));
        }

        [Test]
        public void GetTicketsToPushAsIssues_ShouldGetAllSyncedJiraTickets_WhenValid()
        {
            //Act
            var jiraTickets = jiraRepository.GetAllSyncedJiraTickets().ToList();
            var syncedTickets = jiraTickets.Where(a => a.SyncWithJira == true);

            //Assert
            Assert.IsNotNull(jiraTickets);
            Assert.AreEqual(jiraTickets.Count, syncedTickets.Count());//Tests to make sure expression executed as expected
        }

        [Test]
        public void GetTicketsToPushAsIssues_ShouldGetAllSyncedTickets_WithNoParams()
        {
            //Arrange
            var jiraTickets = new JiraTicketsService(jiraRepository).GetTicketsToPushAsIssues();

            //Assert
            Assert.IsNotNull(jiraTickets);
        }
    }
}
