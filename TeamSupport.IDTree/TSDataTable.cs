using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Data.Linq;

namespace TeamSupport.IDTree
{
    public class TSDataTable : IDisposable
    {
        public DataTable _table { get; private set; }
        Proxy.AuthenticationModel _authentication;
        SqlConnection _connection;
        SqlTransaction _transaction;
        SqlCommand _command;
        SqlDataAdapter _adapter;

        //const string ActionQuery = "SET NOCOUNT OFF; SELECT [ActionID], [ActionTypeID], [SystemActionTypeID], [Name], [TimeSpent], [DateStarted], [IsVisibleOnPortal], [IsKnowledgeBase], [ImportID], [DateCreated], [DateModified], [CreatorID], [ModifierID], [TicketID], [ActionSource], [DateModifiedBySalesForceSync], [SalesForceID], [DateModifiedByJiraSync], [JiraID], [Pinned], [Description], [IsClean], [ImportFileID] FROM [dbo].[Actions] WHERE ([ActionID] = @t0);";
        //const string UserQuery = "SET NOCOUNT OFF; SELECT [UserID], [Email], [FirstName], [MiddleName], [LastName], [Title], [CryptedPassword], [IsActive], [MarkDeleted], [TimeZoneID], [CultureName], [LastLogin], [LastActivity], [LastPing], [LastWaterCoolerID], [IsSystemAdmin], [IsFinanceAdmin], [IsPasswordExpired], [IsPortalUser], [IsChatUser], [PrimaryGroupID], [InOffice], [InOfficeComment], [ReceiveTicketNotifications], [ReceiveAllGroupNotifications], [SubscribeToNewTickets], [ActivatedOn], [DeactivatedOn], [OrganizationID], [LastVersion], [SessionID], [ImportID], [DateCreated], [DateModified], [CreatorID], [ModifierID], [OrgsUserCanSeeOnPortal], [DoNotAutoSubscribe], [IsClassicView], [SubscribeToNewActions], [ApprovedTerms], [ShowWelcomePage], [UserInformation], [PortalAutoReg], [AppChatID], [AppChatStatus], [MenuItems], [TicketRights], [Signature], [LinkedIn], [OnlyEmailAfterHours], [BlockInboundEmail], [SalesForceID], [ChangeTicketVisibility], [ChangeKBVisibility], [EnforceSingleSession], [NeedsIndexing], [AllowAnyTicketCustomer], [FontFamily], [FontSize], [CanCreateCompany], [CanEditCompany], [CanCreateContact], [CanEditContact], [RestrictUserFromEditingAnyActions], [AllowUserToEditAnyAction], [UserCanPinAction], [PortalLimitOrgTickets], [CanCreateAsset], [CanEditAsset], [CanChangeCommunityVisibility], [FilterInactive], [DisableExporting], [DisablePublic], [CanCreateProducts], [CanEditProducts], [CanCreateVersions], [CanEditVersions], [ReceiveUnassignedGroupEmails], [ProductFamiliesRights], [BlockEmailFromCreatingOnly], [CalGUID], [PortalViewOnly], [verificationPhoneNumber], [verificationCode], [verificationCodeExpiration], [PasswordCreatedUtc], [ImportFileID], [PortalLimitOrgChildrenTickets], [CanBulkMerge] FROM [dbo].[Users] WHERE ([UserID] = @t0);";

        public TSDataTable(string query, params object[] args)
        {
            _authentication = new Proxy.AuthenticationModel();
            _connection = new SqlConnection(_authentication.ConnectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction(IsolationLevel.ReadUncommitted);
            _command = new SqlCommand()
            {
                Connection = _connection,
                Transaction = _transaction,
                CommandText = query,
                CommandType = CommandType.Text,
            };
            for (int i = 0; i < args.Length; ++i)
                _command.Parameters.AddWithValue($"t{i}", args[0]);

            _table = new DataTable();
            try
            {
                _adapter = new SqlDataAdapter(_command);
                _adapter.FillSchema(_table, SchemaType.Source);
                _adapter.Fill(_table);
                _transaction.Commit();
            }
            catch (Exception e)
            {
                _transaction.Rollback();
            }
            _connection.Close();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            if (_adapter != null)
                _adapter.Dispose();
            if (_command != null)
                _command.Dispose();
            if (_table != null)
                _table.Dispose();
            if (_transaction != null)
                _transaction.Dispose();
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
            }
        }
    }

}
