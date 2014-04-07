IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectUser' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectUser
GO

CREATE PROCEDURE dbo.uspGeneratedSelectUser

(
  @UserID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [UserID],
    [Email],
    [FirstName],
    [MiddleName],
    [LastName],
    [Title],
    [CryptedPassword],
    [IsActive],
    [MarkDeleted],
    [TimeZoneID],
    [CultureName],
    [LastLogin],
    [LastActivity],
    [LastPing],
    [LastWaterCoolerID],
    [IsSystemAdmin],
    [IsFinanceAdmin],
    [IsPasswordExpired],
    [IsPortalUser],
    [IsChatUser],
    [PrimaryGroupID],
    [InOffice],
    [InOfficeComment],
    [ReceiveTicketNotifications],
    [ReceiveAllGroupNotifications],
    [SubscribeToNewTickets],
    [ActivatedOn],
    [DeactivatedOn],
    [OrganizationID],
    [LastVersion],
    [SessionID],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [OrgsUserCanSeeOnPortal],
    [DoNotAutoSubscribe],
    [IsClassicView],
    [SubscribeToNewActions],
    [ApprovedTerms],
    [ShowWelcomePage],
    [UserInformation],
    [PortalAutoReg],
    [AppChatID],
    [AppChatStatus],
    [MenuItems],
    [TicketRights],
    [Signature],
    [LinkedIn],
    [OnlyEmailAfterHours],
    [BlockInboundEmail],
    [SalesForceID],
    [ChangeTicketVisibility],
    [ChangeKBVisibility],
    [EnforceSingleSession],
    [NeedsIndexing],
    [AllowAnyTicketCustomer],
    [FontFamily],
    [FontSize],
    [CanCreateCompany],
    [CanEditCompany],
    [CanCreateContact],
    [CanEditContact],
    [RestrictUserFromEditingAnyActions],
    [AllowUserToEditAnyAction],
    [UserCanPinAction]
  FROM [dbo].[Users]
  WHERE ([UserID] = @UserID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertUser' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertUser
GO

CREATE PROCEDURE dbo.uspGeneratedInsertUser

(
  @Email varchar(1024),
  @FirstName varchar(100),
  @MiddleName varchar(100),
  @LastName varchar(100),
  @Title varchar(100),
  @CryptedPassword varchar(255),
  @IsActive bit,
  @MarkDeleted bit,
  @TimeZoneID varchar(300),
  @CultureName varchar(50),
  @LastLogin datetime,
  @LastActivity datetime,
  @LastPing datetime,
  @LastWaterCoolerID int,
  @IsSystemAdmin bit,
  @IsFinanceAdmin bit,
  @IsPasswordExpired bit,
  @IsPortalUser bit,
  @IsChatUser bit,
  @PrimaryGroupID int,
  @InOffice bit,
  @InOfficeComment varchar(200),
  @ReceiveTicketNotifications bit,
  @ReceiveAllGroupNotifications bit,
  @SubscribeToNewTickets bit,
  @ActivatedOn datetime,
  @DeactivatedOn datetime,
  @OrganizationID int,
  @LastVersion varchar(50),
  @SessionID uniqueidentifier,
  @ImportID varchar(500),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @OrgsUserCanSeeOnPortal varchar(200),
  @DoNotAutoSubscribe bit,
  @IsClassicView bit,
  @SubscribeToNewActions bit,
  @ApprovedTerms bit,
  @ShowWelcomePage bit,
  @UserInformation varchar(MAX),
  @PortalAutoReg bit,
  @AppChatID varchar(200),
  @AppChatStatus bit,
  @MenuItems varchar(1000),
  @TicketRights int,
  @Signature varchar(MAX),
  @LinkedIn varchar(200),
  @OnlyEmailAfterHours bit,
  @BlockInboundEmail bit,
  @SalesForceID varchar(8000),
  @ChangeTicketVisibility bit,
  @ChangeKBVisibility bit,
  @EnforceSingleSession bit,
  @NeedsIndexing bit,
  @AllowAnyTicketCustomer bit,
  @FontFamily int,
  @FontSize int,
  @CanCreateCompany bit,
  @CanEditCompany bit,
  @CanCreateContact bit,
  @CanEditContact bit,
  @RestrictUserFromEditingAnyActions bit,
  @AllowUserToEditAnyAction bit,
  @UserCanPinAction bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Users]
  (
    [Email],
    [FirstName],
    [MiddleName],
    [LastName],
    [Title],
    [CryptedPassword],
    [IsActive],
    [MarkDeleted],
    [TimeZoneID],
    [CultureName],
    [LastLogin],
    [LastActivity],
    [LastPing],
    [LastWaterCoolerID],
    [IsSystemAdmin],
    [IsFinanceAdmin],
    [IsPasswordExpired],
    [IsPortalUser],
    [IsChatUser],
    [PrimaryGroupID],
    [InOffice],
    [InOfficeComment],
    [ReceiveTicketNotifications],
    [ReceiveAllGroupNotifications],
    [SubscribeToNewTickets],
    [ActivatedOn],
    [DeactivatedOn],
    [OrganizationID],
    [LastVersion],
    [SessionID],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [OrgsUserCanSeeOnPortal],
    [DoNotAutoSubscribe],
    [IsClassicView],
    [SubscribeToNewActions],
    [ApprovedTerms],
    [ShowWelcomePage],
    [UserInformation],
    [PortalAutoReg],
    [AppChatID],
    [AppChatStatus],
    [MenuItems],
    [TicketRights],
    [Signature],
    [LinkedIn],
    [OnlyEmailAfterHours],
    [BlockInboundEmail],
    [SalesForceID],
    [ChangeTicketVisibility],
    [ChangeKBVisibility],
    [EnforceSingleSession],
    [NeedsIndexing],
    [AllowAnyTicketCustomer],
    [FontFamily],
    [FontSize],
    [CanCreateCompany],
    [CanEditCompany],
    [CanCreateContact],
    [CanEditContact],
    [RestrictUserFromEditingAnyActions],
    [AllowUserToEditAnyAction],
    [UserCanPinAction])
  VALUES (
    @Email,
    @FirstName,
    @MiddleName,
    @LastName,
    @Title,
    @CryptedPassword,
    @IsActive,
    @MarkDeleted,
    @TimeZoneID,
    @CultureName,
    @LastLogin,
    @LastActivity,
    @LastPing,
    @LastWaterCoolerID,
    @IsSystemAdmin,
    @IsFinanceAdmin,
    @IsPasswordExpired,
    @IsPortalUser,
    @IsChatUser,
    @PrimaryGroupID,
    @InOffice,
    @InOfficeComment,
    @ReceiveTicketNotifications,
    @ReceiveAllGroupNotifications,
    @SubscribeToNewTickets,
    @ActivatedOn,
    @DeactivatedOn,
    @OrganizationID,
    @LastVersion,
    @SessionID,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @OrgsUserCanSeeOnPortal,
    @DoNotAutoSubscribe,
    @IsClassicView,
    @SubscribeToNewActions,
    @ApprovedTerms,
    @ShowWelcomePage,
    @UserInformation,
    @PortalAutoReg,
    @AppChatID,
    @AppChatStatus,
    @MenuItems,
    @TicketRights,
    @Signature,
    @LinkedIn,
    @OnlyEmailAfterHours,
    @BlockInboundEmail,
    @SalesForceID,
    @ChangeTicketVisibility,
    @ChangeKBVisibility,
    @EnforceSingleSession,
    @NeedsIndexing,
    @AllowAnyTicketCustomer,
    @FontFamily,
    @FontSize,
    @CanCreateCompany,
    @CanEditCompany,
    @CanCreateContact,
    @CanEditContact,
    @RestrictUserFromEditingAnyActions,
    @AllowUserToEditAnyAction,
    @UserCanPinAction)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateUser' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateUser
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateUser

(
  @UserID int,
  @Email varchar(1024),
  @FirstName varchar(100),
  @MiddleName varchar(100),
  @LastName varchar(100),
  @Title varchar(100),
  @CryptedPassword varchar(255),
  @IsActive bit,
  @MarkDeleted bit,
  @TimeZoneID varchar(300),
  @CultureName varchar(50),
  @LastLogin datetime,
  @LastActivity datetime,
  @LastPing datetime,
  @LastWaterCoolerID int,
  @IsSystemAdmin bit,
  @IsFinanceAdmin bit,
  @IsPasswordExpired bit,
  @IsPortalUser bit,
  @IsChatUser bit,
  @PrimaryGroupID int,
  @InOffice bit,
  @InOfficeComment varchar(200),
  @ReceiveTicketNotifications bit,
  @ReceiveAllGroupNotifications bit,
  @SubscribeToNewTickets bit,
  @ActivatedOn datetime,
  @DeactivatedOn datetime,
  @OrganizationID int,
  @LastVersion varchar(50),
  @SessionID uniqueidentifier,
  @ImportID varchar(500),
  @DateModified datetime,
  @ModifierID int,
  @OrgsUserCanSeeOnPortal varchar(200),
  @DoNotAutoSubscribe bit,
  @IsClassicView bit,
  @SubscribeToNewActions bit,
  @ApprovedTerms bit,
  @ShowWelcomePage bit,
  @UserInformation varchar(MAX),
  @PortalAutoReg bit,
  @AppChatID varchar(200),
  @AppChatStatus bit,
  @MenuItems varchar(1000),
  @TicketRights int,
  @Signature varchar(MAX),
  @LinkedIn varchar(200),
  @OnlyEmailAfterHours bit,
  @BlockInboundEmail bit,
  @SalesForceID varchar(8000),
  @ChangeTicketVisibility bit,
  @ChangeKBVisibility bit,
  @EnforceSingleSession bit,
  @NeedsIndexing bit,
  @AllowAnyTicketCustomer bit,
  @FontFamily int,
  @FontSize int,
  @CanCreateCompany bit,
  @CanEditCompany bit,
  @CanCreateContact bit,
  @CanEditContact bit,
  @RestrictUserFromEditingAnyActions bit,
  @AllowUserToEditAnyAction bit,
  @UserCanPinAction bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Users]
  SET
    [Email] = @Email,
    [FirstName] = @FirstName,
    [MiddleName] = @MiddleName,
    [LastName] = @LastName,
    [Title] = @Title,
    [CryptedPassword] = @CryptedPassword,
    [IsActive] = @IsActive,
    [MarkDeleted] = @MarkDeleted,
    [TimeZoneID] = @TimeZoneID,
    [CultureName] = @CultureName,
    [LastLogin] = @LastLogin,
    [LastActivity] = @LastActivity,
    [LastPing] = @LastPing,
    [LastWaterCoolerID] = @LastWaterCoolerID,
    [IsSystemAdmin] = @IsSystemAdmin,
    [IsFinanceAdmin] = @IsFinanceAdmin,
    [IsPasswordExpired] = @IsPasswordExpired,
    [IsPortalUser] = @IsPortalUser,
    [IsChatUser] = @IsChatUser,
    [PrimaryGroupID] = @PrimaryGroupID,
    [InOffice] = @InOffice,
    [InOfficeComment] = @InOfficeComment,
    [ReceiveTicketNotifications] = @ReceiveTicketNotifications,
    [ReceiveAllGroupNotifications] = @ReceiveAllGroupNotifications,
    [SubscribeToNewTickets] = @SubscribeToNewTickets,
    [ActivatedOn] = @ActivatedOn,
    [DeactivatedOn] = @DeactivatedOn,
    [OrganizationID] = @OrganizationID,
    [LastVersion] = @LastVersion,
    [SessionID] = @SessionID,
    [ImportID] = @ImportID,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [OrgsUserCanSeeOnPortal] = @OrgsUserCanSeeOnPortal,
    [DoNotAutoSubscribe] = @DoNotAutoSubscribe,
    [IsClassicView] = @IsClassicView,
    [SubscribeToNewActions] = @SubscribeToNewActions,
    [ApprovedTerms] = @ApprovedTerms,
    [ShowWelcomePage] = @ShowWelcomePage,
    [UserInformation] = @UserInformation,
    [PortalAutoReg] = @PortalAutoReg,
    [AppChatID] = @AppChatID,
    [AppChatStatus] = @AppChatStatus,
    [MenuItems] = @MenuItems,
    [TicketRights] = @TicketRights,
    [Signature] = @Signature,
    [LinkedIn] = @LinkedIn,
    [OnlyEmailAfterHours] = @OnlyEmailAfterHours,
    [BlockInboundEmail] = @BlockInboundEmail,
    [SalesForceID] = @SalesForceID,
    [ChangeTicketVisibility] = @ChangeTicketVisibility,
    [ChangeKBVisibility] = @ChangeKBVisibility,
    [EnforceSingleSession] = @EnforceSingleSession,
    [NeedsIndexing] = @NeedsIndexing,
    [AllowAnyTicketCustomer] = @AllowAnyTicketCustomer,
    [FontFamily] = @FontFamily,
    [FontSize] = @FontSize,
    [CanCreateCompany] = @CanCreateCompany,
    [CanEditCompany] = @CanEditCompany,
    [CanCreateContact] = @CanCreateContact,
    [CanEditContact] = @CanEditContact,
    [RestrictUserFromEditingAnyActions] = @RestrictUserFromEditingAnyActions,
    [AllowUserToEditAnyAction] = @AllowUserToEditAnyAction,
    [UserCanPinAction] = @UserCanPinAction
  WHERE ([UserID] = @UserID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteUser' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteUser
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteUser

(
  @UserID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Users]
  WHERE ([UserID] = @UserID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAction' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAction
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAction

(
  @ActionID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ActionID],
    [ActionTypeID],
    [SystemActionTypeID],
    [Name],
    [Description],
    [TimeSpent],
    [DateStarted],
    [IsVisibleOnPortal],
    [IsKnowledgeBase],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [TicketID],
    [ActionSource],
    [SalesForceID],
    [DateModifiedBySalesForceSync],
    [Pinned]
  FROM [dbo].[Actions]
  WHERE ([ActionID] = @ActionID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAction' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAction
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAction

(
  @ActionTypeID int,
  @SystemActionTypeID int,
  @Name varchar(500),
  @Description varchar(MAX),
  @TimeSpent int,
  @DateStarted datetime,
  @IsVisibleOnPortal bit,
  @IsKnowledgeBase bit,
  @ImportID varchar(50),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @TicketID int,
  @ActionSource varchar(50),
  @SalesForceID varchar(8000),
  @DateModifiedBySalesForceSync datetime,
  @Pinned bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Actions]
  (
    [ActionTypeID],
    [SystemActionTypeID],
    [Name],
    [Description],
    [TimeSpent],
    [DateStarted],
    [IsVisibleOnPortal],
    [IsKnowledgeBase],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [TicketID],
    [ActionSource],
    [SalesForceID],
    [DateModifiedBySalesForceSync],
    [Pinned])
  VALUES (
    @ActionTypeID,
    @SystemActionTypeID,
    @Name,
    @Description,
    @TimeSpent,
    @DateStarted,
    @IsVisibleOnPortal,
    @IsKnowledgeBase,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @TicketID,
    @ActionSource,
    @SalesForceID,
    @DateModifiedBySalesForceSync,
    @Pinned)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAction' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAction
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAction

(
  @ActionID int,
  @ActionTypeID int,
  @SystemActionTypeID int,
  @Name varchar(500),
  @Description varchar(MAX),
  @TimeSpent int,
  @DateStarted datetime,
  @IsVisibleOnPortal bit,
  @IsKnowledgeBase bit,
  @ImportID varchar(50),
  @DateModified datetime,
  @ModifierID int,
  @TicketID int,
  @ActionSource varchar(50),
  @SalesForceID varchar(8000),
  @DateModifiedBySalesForceSync datetime,
  @Pinned bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Actions]
  SET
    [ActionTypeID] = @ActionTypeID,
    [SystemActionTypeID] = @SystemActionTypeID,
    [Name] = @Name,
    [Description] = @Description,
    [TimeSpent] = @TimeSpent,
    [DateStarted] = @DateStarted,
    [IsVisibleOnPortal] = @IsVisibleOnPortal,
    [IsKnowledgeBase] = @IsKnowledgeBase,
    [ImportID] = @ImportID,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [TicketID] = @TicketID,
    [ActionSource] = @ActionSource,
    [SalesForceID] = @SalesForceID,
    [DateModifiedBySalesForceSync] = @DateModifiedBySalesForceSync,
    [Pinned] = @Pinned
  WHERE ([ActionID] = @ActionID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAction' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAction
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAction

(
  @ActionID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Actions]
  WHERE ([ActionID] = @ActionID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectUser' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectUser
GO

CREATE PROCEDURE dbo.uspGeneratedSelectUser

(
  @UserID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [UserID],
    [Email],
    [FirstName],
    [MiddleName],
    [LastName],
    [Title],
    [CryptedPassword],
    [IsActive],
    [MarkDeleted],
    [TimeZoneID],
    [CultureName],
    [LastLogin],
    [LastActivity],
    [LastPing],
    [LastWaterCoolerID],
    [IsSystemAdmin],
    [IsFinanceAdmin],
    [IsPasswordExpired],
    [IsPortalUser],
    [IsChatUser],
    [PrimaryGroupID],
    [InOffice],
    [InOfficeComment],
    [ReceiveTicketNotifications],
    [ReceiveAllGroupNotifications],
    [SubscribeToNewTickets],
    [ActivatedOn],
    [DeactivatedOn],
    [OrganizationID],
    [LastVersion],
    [SessionID],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [OrgsUserCanSeeOnPortal],
    [DoNotAutoSubscribe],
    [IsClassicView],
    [SubscribeToNewActions],
    [ApprovedTerms],
    [ShowWelcomePage],
    [UserInformation],
    [PortalAutoReg],
    [AppChatID],
    [AppChatStatus],
    [MenuItems],
    [TicketRights],
    [Signature],
    [LinkedIn],
    [OnlyEmailAfterHours],
    [BlockInboundEmail],
    [SalesForceID],
    [ChangeTicketVisibility],
    [ChangeKBVisibility],
    [EnforceSingleSession],
    [NeedsIndexing],
    [AllowAnyTicketCustomer],
    [FontFamily],
    [FontSize],
    [CanCreateCompany],
    [CanEditCompany],
    [CanCreateContact],
    [CanEditContact],
    [RestrictUserFromEditingAnyActions],
    [AllowUserToEditAnyAction],
    [UserCanPinAction]
  FROM [dbo].[Users]
  WHERE ([UserID] = @UserID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertUser' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertUser
GO

CREATE PROCEDURE dbo.uspGeneratedInsertUser

(
  @Email varchar(1024),
  @FirstName varchar(100),
  @MiddleName varchar(100),
  @LastName varchar(100),
  @Title varchar(100),
  @CryptedPassword varchar(255),
  @IsActive bit,
  @MarkDeleted bit,
  @TimeZoneID varchar(300),
  @CultureName varchar(50),
  @LastLogin datetime,
  @LastActivity datetime,
  @LastPing datetime,
  @LastWaterCoolerID int,
  @IsSystemAdmin bit,
  @IsFinanceAdmin bit,
  @IsPasswordExpired bit,
  @IsPortalUser bit,
  @IsChatUser bit,
  @PrimaryGroupID int,
  @InOffice bit,
  @InOfficeComment varchar(200),
  @ReceiveTicketNotifications bit,
  @ReceiveAllGroupNotifications bit,
  @SubscribeToNewTickets bit,
  @ActivatedOn datetime,
  @DeactivatedOn datetime,
  @OrganizationID int,
  @LastVersion varchar(50),
  @SessionID uniqueidentifier,
  @ImportID varchar(500),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @OrgsUserCanSeeOnPortal varchar(200),
  @DoNotAutoSubscribe bit,
  @IsClassicView bit,
  @SubscribeToNewActions bit,
  @ApprovedTerms bit,
  @ShowWelcomePage bit,
  @UserInformation varchar(MAX),
  @PortalAutoReg bit,
  @AppChatID varchar(200),
  @AppChatStatus bit,
  @MenuItems varchar(1000),
  @TicketRights int,
  @Signature varchar(MAX),
  @LinkedIn varchar(200),
  @OnlyEmailAfterHours bit,
  @BlockInboundEmail bit,
  @SalesForceID varchar(8000),
  @ChangeTicketVisibility bit,
  @ChangeKBVisibility bit,
  @EnforceSingleSession bit,
  @NeedsIndexing bit,
  @AllowAnyTicketCustomer bit,
  @FontFamily int,
  @FontSize int,
  @CanCreateCompany bit,
  @CanEditCompany bit,
  @CanCreateContact bit,
  @CanEditContact bit,
  @RestrictUserFromEditingAnyActions bit,
  @AllowUserToEditAnyAction bit,
  @UserCanPinAction bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Users]
  (
    [Email],
    [FirstName],
    [MiddleName],
    [LastName],
    [Title],
    [CryptedPassword],
    [IsActive],
    [MarkDeleted],
    [TimeZoneID],
    [CultureName],
    [LastLogin],
    [LastActivity],
    [LastPing],
    [LastWaterCoolerID],
    [IsSystemAdmin],
    [IsFinanceAdmin],
    [IsPasswordExpired],
    [IsPortalUser],
    [IsChatUser],
    [PrimaryGroupID],
    [InOffice],
    [InOfficeComment],
    [ReceiveTicketNotifications],
    [ReceiveAllGroupNotifications],
    [SubscribeToNewTickets],
    [ActivatedOn],
    [DeactivatedOn],
    [OrganizationID],
    [LastVersion],
    [SessionID],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [OrgsUserCanSeeOnPortal],
    [DoNotAutoSubscribe],
    [IsClassicView],
    [SubscribeToNewActions],
    [ApprovedTerms],
    [ShowWelcomePage],
    [UserInformation],
    [PortalAutoReg],
    [AppChatID],
    [AppChatStatus],
    [MenuItems],
    [TicketRights],
    [Signature],
    [LinkedIn],
    [OnlyEmailAfterHours],
    [BlockInboundEmail],
    [SalesForceID],
    [ChangeTicketVisibility],
    [ChangeKBVisibility],
    [EnforceSingleSession],
    [NeedsIndexing],
    [AllowAnyTicketCustomer],
    [FontFamily],
    [FontSize],
    [CanCreateCompany],
    [CanEditCompany],
    [CanCreateContact],
    [CanEditContact],
    [RestrictUserFromEditingAnyActions],
    [AllowUserToEditAnyAction],
    [UserCanPinAction])
  VALUES (
    @Email,
    @FirstName,
    @MiddleName,
    @LastName,
    @Title,
    @CryptedPassword,
    @IsActive,
    @MarkDeleted,
    @TimeZoneID,
    @CultureName,
    @LastLogin,
    @LastActivity,
    @LastPing,
    @LastWaterCoolerID,
    @IsSystemAdmin,
    @IsFinanceAdmin,
    @IsPasswordExpired,
    @IsPortalUser,
    @IsChatUser,
    @PrimaryGroupID,
    @InOffice,
    @InOfficeComment,
    @ReceiveTicketNotifications,
    @ReceiveAllGroupNotifications,
    @SubscribeToNewTickets,
    @ActivatedOn,
    @DeactivatedOn,
    @OrganizationID,
    @LastVersion,
    @SessionID,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @OrgsUserCanSeeOnPortal,
    @DoNotAutoSubscribe,
    @IsClassicView,
    @SubscribeToNewActions,
    @ApprovedTerms,
    @ShowWelcomePage,
    @UserInformation,
    @PortalAutoReg,
    @AppChatID,
    @AppChatStatus,
    @MenuItems,
    @TicketRights,
    @Signature,
    @LinkedIn,
    @OnlyEmailAfterHours,
    @BlockInboundEmail,
    @SalesForceID,
    @ChangeTicketVisibility,
    @ChangeKBVisibility,
    @EnforceSingleSession,
    @NeedsIndexing,
    @AllowAnyTicketCustomer,
    @FontFamily,
    @FontSize,
    @CanCreateCompany,
    @CanEditCompany,
    @CanCreateContact,
    @CanEditContact,
    @RestrictUserFromEditingAnyActions,
    @AllowUserToEditAnyAction,
    @UserCanPinAction)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateUser' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateUser
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateUser

(
  @UserID int,
  @Email varchar(1024),
  @FirstName varchar(100),
  @MiddleName varchar(100),
  @LastName varchar(100),
  @Title varchar(100),
  @CryptedPassword varchar(255),
  @IsActive bit,
  @MarkDeleted bit,
  @TimeZoneID varchar(300),
  @CultureName varchar(50),
  @LastLogin datetime,
  @LastActivity datetime,
  @LastPing datetime,
  @LastWaterCoolerID int,
  @IsSystemAdmin bit,
  @IsFinanceAdmin bit,
  @IsPasswordExpired bit,
  @IsPortalUser bit,
  @IsChatUser bit,
  @PrimaryGroupID int,
  @InOffice bit,
  @InOfficeComment varchar(200),
  @ReceiveTicketNotifications bit,
  @ReceiveAllGroupNotifications bit,
  @SubscribeToNewTickets bit,
  @ActivatedOn datetime,
  @DeactivatedOn datetime,
  @OrganizationID int,
  @LastVersion varchar(50),
  @SessionID uniqueidentifier,
  @ImportID varchar(500),
  @DateModified datetime,
  @ModifierID int,
  @OrgsUserCanSeeOnPortal varchar(200),
  @DoNotAutoSubscribe bit,
  @IsClassicView bit,
  @SubscribeToNewActions bit,
  @ApprovedTerms bit,
  @ShowWelcomePage bit,
  @UserInformation varchar(MAX),
  @PortalAutoReg bit,
  @AppChatID varchar(200),
  @AppChatStatus bit,
  @MenuItems varchar(1000),
  @TicketRights int,
  @Signature varchar(MAX),
  @LinkedIn varchar(200),
  @OnlyEmailAfterHours bit,
  @BlockInboundEmail bit,
  @SalesForceID varchar(8000),
  @ChangeTicketVisibility bit,
  @ChangeKBVisibility bit,
  @EnforceSingleSession bit,
  @NeedsIndexing bit,
  @AllowAnyTicketCustomer bit,
  @FontFamily int,
  @FontSize int,
  @CanCreateCompany bit,
  @CanEditCompany bit,
  @CanCreateContact bit,
  @CanEditContact bit,
  @RestrictUserFromEditingAnyActions bit,
  @AllowUserToEditAnyAction bit,
  @UserCanPinAction bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Users]
  SET
    [Email] = @Email,
    [FirstName] = @FirstName,
    [MiddleName] = @MiddleName,
    [LastName] = @LastName,
    [Title] = @Title,
    [CryptedPassword] = @CryptedPassword,
    [IsActive] = @IsActive,
    [MarkDeleted] = @MarkDeleted,
    [TimeZoneID] = @TimeZoneID,
    [CultureName] = @CultureName,
    [LastLogin] = @LastLogin,
    [LastActivity] = @LastActivity,
    [LastPing] = @LastPing,
    [LastWaterCoolerID] = @LastWaterCoolerID,
    [IsSystemAdmin] = @IsSystemAdmin,
    [IsFinanceAdmin] = @IsFinanceAdmin,
    [IsPasswordExpired] = @IsPasswordExpired,
    [IsPortalUser] = @IsPortalUser,
    [IsChatUser] = @IsChatUser,
    [PrimaryGroupID] = @PrimaryGroupID,
    [InOffice] = @InOffice,
    [InOfficeComment] = @InOfficeComment,
    [ReceiveTicketNotifications] = @ReceiveTicketNotifications,
    [ReceiveAllGroupNotifications] = @ReceiveAllGroupNotifications,
    [SubscribeToNewTickets] = @SubscribeToNewTickets,
    [ActivatedOn] = @ActivatedOn,
    [DeactivatedOn] = @DeactivatedOn,
    [OrganizationID] = @OrganizationID,
    [LastVersion] = @LastVersion,
    [SessionID] = @SessionID,
    [ImportID] = @ImportID,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [OrgsUserCanSeeOnPortal] = @OrgsUserCanSeeOnPortal,
    [DoNotAutoSubscribe] = @DoNotAutoSubscribe,
    [IsClassicView] = @IsClassicView,
    [SubscribeToNewActions] = @SubscribeToNewActions,
    [ApprovedTerms] = @ApprovedTerms,
    [ShowWelcomePage] = @ShowWelcomePage,
    [UserInformation] = @UserInformation,
    [PortalAutoReg] = @PortalAutoReg,
    [AppChatID] = @AppChatID,
    [AppChatStatus] = @AppChatStatus,
    [MenuItems] = @MenuItems,
    [TicketRights] = @TicketRights,
    [Signature] = @Signature,
    [LinkedIn] = @LinkedIn,
    [OnlyEmailAfterHours] = @OnlyEmailAfterHours,
    [BlockInboundEmail] = @BlockInboundEmail,
    [SalesForceID] = @SalesForceID,
    [ChangeTicketVisibility] = @ChangeTicketVisibility,
    [ChangeKBVisibility] = @ChangeKBVisibility,
    [EnforceSingleSession] = @EnforceSingleSession,
    [NeedsIndexing] = @NeedsIndexing,
    [AllowAnyTicketCustomer] = @AllowAnyTicketCustomer,
    [FontFamily] = @FontFamily,
    [FontSize] = @FontSize,
    [CanCreateCompany] = @CanCreateCompany,
    [CanEditCompany] = @CanEditCompany,
    [CanCreateContact] = @CanCreateContact,
    [CanEditContact] = @CanEditContact,
    [RestrictUserFromEditingAnyActions] = @RestrictUserFromEditingAnyActions,
    [AllowUserToEditAnyAction] = @AllowUserToEditAnyAction,
    [UserCanPinAction] = @UserCanPinAction
  WHERE ([UserID] = @UserID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteUser' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteUser
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteUser

(
  @UserID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Users]
  WHERE ([UserID] = @UserID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAction' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAction
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAction

(
  @ActionID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ActionID],
    [ActionTypeID],
    [SystemActionTypeID],
    [Name],
    [Description],
    [TimeSpent],
    [DateStarted],
    [IsVisibleOnPortal],
    [IsKnowledgeBase],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [TicketID],
    [ActionSource],
    [SalesForceID],
    [DateModifiedBySalesForceSync],
    [Pinned]
  FROM [dbo].[Actions]
  WHERE ([ActionID] = @ActionID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAction' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAction
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAction

(
  @ActionTypeID int,
  @SystemActionTypeID int,
  @Name varchar(500),
  @Description varchar(MAX),
  @TimeSpent int,
  @DateStarted datetime,
  @IsVisibleOnPortal bit,
  @IsKnowledgeBase bit,
  @ImportID varchar(50),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @TicketID int,
  @ActionSource varchar(50),
  @SalesForceID varchar(8000),
  @DateModifiedBySalesForceSync datetime,
  @Pinned bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Actions]
  (
    [ActionTypeID],
    [SystemActionTypeID],
    [Name],
    [Description],
    [TimeSpent],
    [DateStarted],
    [IsVisibleOnPortal],
    [IsKnowledgeBase],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [TicketID],
    [ActionSource],
    [SalesForceID],
    [DateModifiedBySalesForceSync],
    [Pinned])
  VALUES (
    @ActionTypeID,
    @SystemActionTypeID,
    @Name,
    @Description,
    @TimeSpent,
    @DateStarted,
    @IsVisibleOnPortal,
    @IsKnowledgeBase,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @TicketID,
    @ActionSource,
    @SalesForceID,
    @DateModifiedBySalesForceSync,
    @Pinned)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAction' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAction
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAction

(
  @ActionID int,
  @ActionTypeID int,
  @SystemActionTypeID int,
  @Name varchar(500),
  @Description varchar(MAX),
  @TimeSpent int,
  @DateStarted datetime,
  @IsVisibleOnPortal bit,
  @IsKnowledgeBase bit,
  @ImportID varchar(50),
  @DateModified datetime,
  @ModifierID int,
  @TicketID int,
  @ActionSource varchar(50),
  @SalesForceID varchar(8000),
  @DateModifiedBySalesForceSync datetime,
  @Pinned bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Actions]
  SET
    [ActionTypeID] = @ActionTypeID,
    [SystemActionTypeID] = @SystemActionTypeID,
    [Name] = @Name,
    [Description] = @Description,
    [TimeSpent] = @TimeSpent,
    [DateStarted] = @DateStarted,
    [IsVisibleOnPortal] = @IsVisibleOnPortal,
    [IsKnowledgeBase] = @IsKnowledgeBase,
    [ImportID] = @ImportID,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [TicketID] = @TicketID,
    [ActionSource] = @ActionSource,
    [SalesForceID] = @SalesForceID,
    [DateModifiedBySalesForceSync] = @DateModifiedBySalesForceSync,
    [Pinned] = @Pinned
  WHERE ([ActionID] = @ActionID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAction' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAction
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAction

(
  @ActionID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Actions]
  WHERE ([ActionID] = @ActionID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectUser' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectUser
GO

CREATE PROCEDURE dbo.uspGeneratedSelectUser

(
  @UserID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [UserID],
    [Email],
    [FirstName],
    [MiddleName],
    [LastName],
    [Title],
    [CryptedPassword],
    [IsActive],
    [MarkDeleted],
    [TimeZoneID],
    [CultureName],
    [LastLogin],
    [LastActivity],
    [LastPing],
    [LastWaterCoolerID],
    [IsSystemAdmin],
    [IsFinanceAdmin],
    [IsPasswordExpired],
    [IsPortalUser],
    [IsChatUser],
    [PrimaryGroupID],
    [InOffice],
    [InOfficeComment],
    [ReceiveTicketNotifications],
    [ReceiveAllGroupNotifications],
    [SubscribeToNewTickets],
    [ActivatedOn],
    [DeactivatedOn],
    [OrganizationID],
    [LastVersion],
    [SessionID],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [OrgsUserCanSeeOnPortal],
    [DoNotAutoSubscribe],
    [IsClassicView],
    [SubscribeToNewActions],
    [ApprovedTerms],
    [ShowWelcomePage],
    [UserInformation],
    [PortalAutoReg],
    [AppChatID],
    [AppChatStatus],
    [MenuItems],
    [TicketRights],
    [Signature],
    [LinkedIn],
    [OnlyEmailAfterHours],
    [BlockInboundEmail],
    [SalesForceID],
    [ChangeTicketVisibility],
    [ChangeKBVisibility],
    [EnforceSingleSession],
    [NeedsIndexing],
    [AllowAnyTicketCustomer],
    [FontFamily],
    [FontSize],
    [CanCreateCompany],
    [CanEditCompany],
    [CanCreateContact],
    [CanEditContact],
    [RestrictUserFromEditingAnyActions],
    [AllowUserToEditAnyAction],
    [UserCanPinAction]
  FROM [dbo].[Users]
  WHERE ([UserID] = @UserID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertUser' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertUser
GO

CREATE PROCEDURE dbo.uspGeneratedInsertUser

(
  @Email varchar(1024),
  @FirstName varchar(100),
  @MiddleName varchar(100),
  @LastName varchar(100),
  @Title varchar(100),
  @CryptedPassword varchar(255),
  @IsActive bit,
  @MarkDeleted bit,
  @TimeZoneID varchar(300),
  @CultureName varchar(50),
  @LastLogin datetime,
  @LastActivity datetime,
  @LastPing datetime,
  @LastWaterCoolerID int,
  @IsSystemAdmin bit,
  @IsFinanceAdmin bit,
  @IsPasswordExpired bit,
  @IsPortalUser bit,
  @IsChatUser bit,
  @PrimaryGroupID int,
  @InOffice bit,
  @InOfficeComment varchar(200),
  @ReceiveTicketNotifications bit,
  @ReceiveAllGroupNotifications bit,
  @SubscribeToNewTickets bit,
  @ActivatedOn datetime,
  @DeactivatedOn datetime,
  @OrganizationID int,
  @LastVersion varchar(50),
  @SessionID uniqueidentifier,
  @ImportID varchar(500),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @OrgsUserCanSeeOnPortal varchar(200),
  @DoNotAutoSubscribe bit,
  @IsClassicView bit,
  @SubscribeToNewActions bit,
  @ApprovedTerms bit,
  @ShowWelcomePage bit,
  @UserInformation varchar(MAX),
  @PortalAutoReg bit,
  @AppChatID varchar(200),
  @AppChatStatus bit,
  @MenuItems varchar(1000),
  @TicketRights int,
  @Signature varchar(MAX),
  @LinkedIn varchar(200),
  @OnlyEmailAfterHours bit,
  @BlockInboundEmail bit,
  @SalesForceID varchar(8000),
  @ChangeTicketVisibility bit,
  @ChangeKBVisibility bit,
  @EnforceSingleSession bit,
  @NeedsIndexing bit,
  @AllowAnyTicketCustomer bit,
  @FontFamily int,
  @FontSize int,
  @CanCreateCompany bit,
  @CanEditCompany bit,
  @CanCreateContact bit,
  @CanEditContact bit,
  @RestrictUserFromEditingAnyActions bit,
  @AllowUserToEditAnyAction bit,
  @UserCanPinAction bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Users]
  (
    [Email],
    [FirstName],
    [MiddleName],
    [LastName],
    [Title],
    [CryptedPassword],
    [IsActive],
    [MarkDeleted],
    [TimeZoneID],
    [CultureName],
    [LastLogin],
    [LastActivity],
    [LastPing],
    [LastWaterCoolerID],
    [IsSystemAdmin],
    [IsFinanceAdmin],
    [IsPasswordExpired],
    [IsPortalUser],
    [IsChatUser],
    [PrimaryGroupID],
    [InOffice],
    [InOfficeComment],
    [ReceiveTicketNotifications],
    [ReceiveAllGroupNotifications],
    [SubscribeToNewTickets],
    [ActivatedOn],
    [DeactivatedOn],
    [OrganizationID],
    [LastVersion],
    [SessionID],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [OrgsUserCanSeeOnPortal],
    [DoNotAutoSubscribe],
    [IsClassicView],
    [SubscribeToNewActions],
    [ApprovedTerms],
    [ShowWelcomePage],
    [UserInformation],
    [PortalAutoReg],
    [AppChatID],
    [AppChatStatus],
    [MenuItems],
    [TicketRights],
    [Signature],
    [LinkedIn],
    [OnlyEmailAfterHours],
    [BlockInboundEmail],
    [SalesForceID],
    [ChangeTicketVisibility],
    [ChangeKBVisibility],
    [EnforceSingleSession],
    [NeedsIndexing],
    [AllowAnyTicketCustomer],
    [FontFamily],
    [FontSize],
    [CanCreateCompany],
    [CanEditCompany],
    [CanCreateContact],
    [CanEditContact],
    [RestrictUserFromEditingAnyActions],
    [AllowUserToEditAnyAction],
    [UserCanPinAction])
  VALUES (
    @Email,
    @FirstName,
    @MiddleName,
    @LastName,
    @Title,
    @CryptedPassword,
    @IsActive,
    @MarkDeleted,
    @TimeZoneID,
    @CultureName,
    @LastLogin,
    @LastActivity,
    @LastPing,
    @LastWaterCoolerID,
    @IsSystemAdmin,
    @IsFinanceAdmin,
    @IsPasswordExpired,
    @IsPortalUser,
    @IsChatUser,
    @PrimaryGroupID,
    @InOffice,
    @InOfficeComment,
    @ReceiveTicketNotifications,
    @ReceiveAllGroupNotifications,
    @SubscribeToNewTickets,
    @ActivatedOn,
    @DeactivatedOn,
    @OrganizationID,
    @LastVersion,
    @SessionID,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @OrgsUserCanSeeOnPortal,
    @DoNotAutoSubscribe,
    @IsClassicView,
    @SubscribeToNewActions,
    @ApprovedTerms,
    @ShowWelcomePage,
    @UserInformation,
    @PortalAutoReg,
    @AppChatID,
    @AppChatStatus,
    @MenuItems,
    @TicketRights,
    @Signature,
    @LinkedIn,
    @OnlyEmailAfterHours,
    @BlockInboundEmail,
    @SalesForceID,
    @ChangeTicketVisibility,
    @ChangeKBVisibility,
    @EnforceSingleSession,
    @NeedsIndexing,
    @AllowAnyTicketCustomer,
    @FontFamily,
    @FontSize,
    @CanCreateCompany,
    @CanEditCompany,
    @CanCreateContact,
    @CanEditContact,
    @RestrictUserFromEditingAnyActions,
    @AllowUserToEditAnyAction,
    @UserCanPinAction)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateUser' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateUser
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateUser

(
  @UserID int,
  @Email varchar(1024),
  @FirstName varchar(100),
  @MiddleName varchar(100),
  @LastName varchar(100),
  @Title varchar(100),
  @CryptedPassword varchar(255),
  @IsActive bit,
  @MarkDeleted bit,
  @TimeZoneID varchar(300),
  @CultureName varchar(50),
  @LastLogin datetime,
  @LastActivity datetime,
  @LastPing datetime,
  @LastWaterCoolerID int,
  @IsSystemAdmin bit,
  @IsFinanceAdmin bit,
  @IsPasswordExpired bit,
  @IsPortalUser bit,
  @IsChatUser bit,
  @PrimaryGroupID int,
  @InOffice bit,
  @InOfficeComment varchar(200),
  @ReceiveTicketNotifications bit,
  @ReceiveAllGroupNotifications bit,
  @SubscribeToNewTickets bit,
  @ActivatedOn datetime,
  @DeactivatedOn datetime,
  @OrganizationID int,
  @LastVersion varchar(50),
  @SessionID uniqueidentifier,
  @ImportID varchar(500),
  @DateModified datetime,
  @ModifierID int,
  @OrgsUserCanSeeOnPortal varchar(200),
  @DoNotAutoSubscribe bit,
  @IsClassicView bit,
  @SubscribeToNewActions bit,
  @ApprovedTerms bit,
  @ShowWelcomePage bit,
  @UserInformation varchar(MAX),
  @PortalAutoReg bit,
  @AppChatID varchar(200),
  @AppChatStatus bit,
  @MenuItems varchar(1000),
  @TicketRights int,
  @Signature varchar(MAX),
  @LinkedIn varchar(200),
  @OnlyEmailAfterHours bit,
  @BlockInboundEmail bit,
  @SalesForceID varchar(8000),
  @ChangeTicketVisibility bit,
  @ChangeKBVisibility bit,
  @EnforceSingleSession bit,
  @NeedsIndexing bit,
  @AllowAnyTicketCustomer bit,
  @FontFamily int,
  @FontSize int,
  @CanCreateCompany bit,
  @CanEditCompany bit,
  @CanCreateContact bit,
  @CanEditContact bit,
  @RestrictUserFromEditingAnyActions bit,
  @AllowUserToEditAnyAction bit,
  @UserCanPinAction bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Users]
  SET
    [Email] = @Email,
    [FirstName] = @FirstName,
    [MiddleName] = @MiddleName,
    [LastName] = @LastName,
    [Title] = @Title,
    [CryptedPassword] = @CryptedPassword,
    [IsActive] = @IsActive,
    [MarkDeleted] = @MarkDeleted,
    [TimeZoneID] = @TimeZoneID,
    [CultureName] = @CultureName,
    [LastLogin] = @LastLogin,
    [LastActivity] = @LastActivity,
    [LastPing] = @LastPing,
    [LastWaterCoolerID] = @LastWaterCoolerID,
    [IsSystemAdmin] = @IsSystemAdmin,
    [IsFinanceAdmin] = @IsFinanceAdmin,
    [IsPasswordExpired] = @IsPasswordExpired,
    [IsPortalUser] = @IsPortalUser,
    [IsChatUser] = @IsChatUser,
    [PrimaryGroupID] = @PrimaryGroupID,
    [InOffice] = @InOffice,
    [InOfficeComment] = @InOfficeComment,
    [ReceiveTicketNotifications] = @ReceiveTicketNotifications,
    [ReceiveAllGroupNotifications] = @ReceiveAllGroupNotifications,
    [SubscribeToNewTickets] = @SubscribeToNewTickets,
    [ActivatedOn] = @ActivatedOn,
    [DeactivatedOn] = @DeactivatedOn,
    [OrganizationID] = @OrganizationID,
    [LastVersion] = @LastVersion,
    [SessionID] = @SessionID,
    [ImportID] = @ImportID,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [OrgsUserCanSeeOnPortal] = @OrgsUserCanSeeOnPortal,
    [DoNotAutoSubscribe] = @DoNotAutoSubscribe,
    [IsClassicView] = @IsClassicView,
    [SubscribeToNewActions] = @SubscribeToNewActions,
    [ApprovedTerms] = @ApprovedTerms,
    [ShowWelcomePage] = @ShowWelcomePage,
    [UserInformation] = @UserInformation,
    [PortalAutoReg] = @PortalAutoReg,
    [AppChatID] = @AppChatID,
    [AppChatStatus] = @AppChatStatus,
    [MenuItems] = @MenuItems,
    [TicketRights] = @TicketRights,
    [Signature] = @Signature,
    [LinkedIn] = @LinkedIn,
    [OnlyEmailAfterHours] = @OnlyEmailAfterHours,
    [BlockInboundEmail] = @BlockInboundEmail,
    [SalesForceID] = @SalesForceID,
    [ChangeTicketVisibility] = @ChangeTicketVisibility,
    [ChangeKBVisibility] = @ChangeKBVisibility,
    [EnforceSingleSession] = @EnforceSingleSession,
    [NeedsIndexing] = @NeedsIndexing,
    [AllowAnyTicketCustomer] = @AllowAnyTicketCustomer,
    [FontFamily] = @FontFamily,
    [FontSize] = @FontSize,
    [CanCreateCompany] = @CanCreateCompany,
    [CanEditCompany] = @CanEditCompany,
    [CanCreateContact] = @CanCreateContact,
    [CanEditContact] = @CanEditContact,
    [RestrictUserFromEditingAnyActions] = @RestrictUserFromEditingAnyActions,
    [AllowUserToEditAnyAction] = @AllowUserToEditAnyAction,
    [UserCanPinAction] = @UserCanPinAction
  WHERE ([UserID] = @UserID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteUser' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteUser
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteUser

(
  @UserID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Users]
  WHERE ([UserID] = @UserID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAction' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAction
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAction

(
  @ActionID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ActionID],
    [ActionTypeID],
    [SystemActionTypeID],
    [Name],
    [Description],
    [TimeSpent],
    [DateStarted],
    [IsVisibleOnPortal],
    [IsKnowledgeBase],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [TicketID],
    [ActionSource],
    [SalesForceID],
    [DateModifiedBySalesForceSync],
    [Pinned]
  FROM [dbo].[Actions]
  WHERE ([ActionID] = @ActionID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAction' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAction
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAction

(
  @ActionTypeID int,
  @SystemActionTypeID int,
  @Name varchar(500),
  @Description varchar(MAX),
  @TimeSpent int,
  @DateStarted datetime,
  @IsVisibleOnPortal bit,
  @IsKnowledgeBase bit,
  @ImportID varchar(50),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @TicketID int,
  @ActionSource varchar(50),
  @SalesForceID varchar(8000),
  @DateModifiedBySalesForceSync datetime,
  @Pinned bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Actions]
  (
    [ActionTypeID],
    [SystemActionTypeID],
    [Name],
    [Description],
    [TimeSpent],
    [DateStarted],
    [IsVisibleOnPortal],
    [IsKnowledgeBase],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [TicketID],
    [ActionSource],
    [SalesForceID],
    [DateModifiedBySalesForceSync],
    [Pinned])
  VALUES (
    @ActionTypeID,
    @SystemActionTypeID,
    @Name,
    @Description,
    @TimeSpent,
    @DateStarted,
    @IsVisibleOnPortal,
    @IsKnowledgeBase,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @TicketID,
    @ActionSource,
    @SalesForceID,
    @DateModifiedBySalesForceSync,
    @Pinned)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAction' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAction
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAction

(
  @ActionID int,
  @ActionTypeID int,
  @SystemActionTypeID int,
  @Name varchar(500),
  @Description varchar(MAX),
  @TimeSpent int,
  @DateStarted datetime,
  @IsVisibleOnPortal bit,
  @IsKnowledgeBase bit,
  @ImportID varchar(50),
  @DateModified datetime,
  @ModifierID int,
  @TicketID int,
  @ActionSource varchar(50),
  @SalesForceID varchar(8000),
  @DateModifiedBySalesForceSync datetime,
  @Pinned bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Actions]
  SET
    [ActionTypeID] = @ActionTypeID,
    [SystemActionTypeID] = @SystemActionTypeID,
    [Name] = @Name,
    [Description] = @Description,
    [TimeSpent] = @TimeSpent,
    [DateStarted] = @DateStarted,
    [IsVisibleOnPortal] = @IsVisibleOnPortal,
    [IsKnowledgeBase] = @IsKnowledgeBase,
    [ImportID] = @ImportID,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [TicketID] = @TicketID,
    [ActionSource] = @ActionSource,
    [SalesForceID] = @SalesForceID,
    [DateModifiedBySalesForceSync] = @DateModifiedBySalesForceSync,
    [Pinned] = @Pinned
  WHERE ([ActionID] = @ActionID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAction' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAction
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAction

(
  @ActionID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Actions]
  WHERE ([ActionID] = @ActionID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectUser' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectUser
GO

CREATE PROCEDURE dbo.uspGeneratedSelectUser

(
  @UserID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [UserID],
    [Email],
    [FirstName],
    [MiddleName],
    [LastName],
    [Title],
    [CryptedPassword],
    [IsActive],
    [MarkDeleted],
    [TimeZoneID],
    [CultureName],
    [LastLogin],
    [LastActivity],
    [LastPing],
    [LastWaterCoolerID],
    [IsSystemAdmin],
    [IsFinanceAdmin],
    [IsPasswordExpired],
    [IsPortalUser],
    [IsChatUser],
    [PrimaryGroupID],
    [InOffice],
    [InOfficeComment],
    [ReceiveTicketNotifications],
    [ReceiveAllGroupNotifications],
    [SubscribeToNewTickets],
    [ActivatedOn],
    [DeactivatedOn],
    [OrganizationID],
    [LastVersion],
    [SessionID],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [OrgsUserCanSeeOnPortal],
    [DoNotAutoSubscribe],
    [IsClassicView],
    [SubscribeToNewActions],
    [ApprovedTerms],
    [ShowWelcomePage],
    [UserInformation],
    [PortalAutoReg],
    [AppChatID],
    [AppChatStatus],
    [MenuItems],
    [TicketRights],
    [Signature],
    [LinkedIn],
    [OnlyEmailAfterHours],
    [BlockInboundEmail],
    [SalesForceID],
    [ChangeTicketVisibility],
    [ChangeKBVisibility],
    [EnforceSingleSession],
    [NeedsIndexing],
    [AllowAnyTicketCustomer],
    [FontFamily],
    [FontSize],
    [CanCreateCompany],
    [CanEditCompany],
    [CanCreateContact],
    [CanEditContact],
    [RestrictUserFromEditingAnyActions],
    [AllowUserToEditAnyAction],
    [UserCanPinAction]
  FROM [dbo].[Users]
  WHERE ([UserID] = @UserID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertUser' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertUser
GO

CREATE PROCEDURE dbo.uspGeneratedInsertUser

(
  @Email varchar(1024),
  @FirstName varchar(100),
  @MiddleName varchar(100),
  @LastName varchar(100),
  @Title varchar(100),
  @CryptedPassword varchar(255),
  @IsActive bit,
  @MarkDeleted bit,
  @TimeZoneID varchar(300),
  @CultureName varchar(50),
  @LastLogin datetime,
  @LastActivity datetime,
  @LastPing datetime,
  @LastWaterCoolerID int,
  @IsSystemAdmin bit,
  @IsFinanceAdmin bit,
  @IsPasswordExpired bit,
  @IsPortalUser bit,
  @IsChatUser bit,
  @PrimaryGroupID int,
  @InOffice bit,
  @InOfficeComment varchar(200),
  @ReceiveTicketNotifications bit,
  @ReceiveAllGroupNotifications bit,
  @SubscribeToNewTickets bit,
  @ActivatedOn datetime,
  @DeactivatedOn datetime,
  @OrganizationID int,
  @LastVersion varchar(50),
  @SessionID uniqueidentifier,
  @ImportID varchar(500),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @OrgsUserCanSeeOnPortal varchar(200),
  @DoNotAutoSubscribe bit,
  @IsClassicView bit,
  @SubscribeToNewActions bit,
  @ApprovedTerms bit,
  @ShowWelcomePage bit,
  @UserInformation varchar(MAX),
  @PortalAutoReg bit,
  @AppChatID varchar(200),
  @AppChatStatus bit,
  @MenuItems varchar(1000),
  @TicketRights int,
  @Signature varchar(MAX),
  @LinkedIn varchar(200),
  @OnlyEmailAfterHours bit,
  @BlockInboundEmail bit,
  @SalesForceID varchar(8000),
  @ChangeTicketVisibility bit,
  @ChangeKBVisibility bit,
  @EnforceSingleSession bit,
  @NeedsIndexing bit,
  @AllowAnyTicketCustomer bit,
  @FontFamily int,
  @FontSize int,
  @CanCreateCompany bit,
  @CanEditCompany bit,
  @CanCreateContact bit,
  @CanEditContact bit,
  @RestrictUserFromEditingAnyActions bit,
  @AllowUserToEditAnyAction bit,
  @UserCanPinAction bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Users]
  (
    [Email],
    [FirstName],
    [MiddleName],
    [LastName],
    [Title],
    [CryptedPassword],
    [IsActive],
    [MarkDeleted],
    [TimeZoneID],
    [CultureName],
    [LastLogin],
    [LastActivity],
    [LastPing],
    [LastWaterCoolerID],
    [IsSystemAdmin],
    [IsFinanceAdmin],
    [IsPasswordExpired],
    [IsPortalUser],
    [IsChatUser],
    [PrimaryGroupID],
    [InOffice],
    [InOfficeComment],
    [ReceiveTicketNotifications],
    [ReceiveAllGroupNotifications],
    [SubscribeToNewTickets],
    [ActivatedOn],
    [DeactivatedOn],
    [OrganizationID],
    [LastVersion],
    [SessionID],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [OrgsUserCanSeeOnPortal],
    [DoNotAutoSubscribe],
    [IsClassicView],
    [SubscribeToNewActions],
    [ApprovedTerms],
    [ShowWelcomePage],
    [UserInformation],
    [PortalAutoReg],
    [AppChatID],
    [AppChatStatus],
    [MenuItems],
    [TicketRights],
    [Signature],
    [LinkedIn],
    [OnlyEmailAfterHours],
    [BlockInboundEmail],
    [SalesForceID],
    [ChangeTicketVisibility],
    [ChangeKBVisibility],
    [EnforceSingleSession],
    [NeedsIndexing],
    [AllowAnyTicketCustomer],
    [FontFamily],
    [FontSize],
    [CanCreateCompany],
    [CanEditCompany],
    [CanCreateContact],
    [CanEditContact],
    [RestrictUserFromEditingAnyActions],
    [AllowUserToEditAnyAction],
    [UserCanPinAction])
  VALUES (
    @Email,
    @FirstName,
    @MiddleName,
    @LastName,
    @Title,
    @CryptedPassword,
    @IsActive,
    @MarkDeleted,
    @TimeZoneID,
    @CultureName,
    @LastLogin,
    @LastActivity,
    @LastPing,
    @LastWaterCoolerID,
    @IsSystemAdmin,
    @IsFinanceAdmin,
    @IsPasswordExpired,
    @IsPortalUser,
    @IsChatUser,
    @PrimaryGroupID,
    @InOffice,
    @InOfficeComment,
    @ReceiveTicketNotifications,
    @ReceiveAllGroupNotifications,
    @SubscribeToNewTickets,
    @ActivatedOn,
    @DeactivatedOn,
    @OrganizationID,
    @LastVersion,
    @SessionID,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @OrgsUserCanSeeOnPortal,
    @DoNotAutoSubscribe,
    @IsClassicView,
    @SubscribeToNewActions,
    @ApprovedTerms,
    @ShowWelcomePage,
    @UserInformation,
    @PortalAutoReg,
    @AppChatID,
    @AppChatStatus,
    @MenuItems,
    @TicketRights,
    @Signature,
    @LinkedIn,
    @OnlyEmailAfterHours,
    @BlockInboundEmail,
    @SalesForceID,
    @ChangeTicketVisibility,
    @ChangeKBVisibility,
    @EnforceSingleSession,
    @NeedsIndexing,
    @AllowAnyTicketCustomer,
    @FontFamily,
    @FontSize,
    @CanCreateCompany,
    @CanEditCompany,
    @CanCreateContact,
    @CanEditContact,
    @RestrictUserFromEditingAnyActions,
    @AllowUserToEditAnyAction,
    @UserCanPinAction)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateUser' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateUser
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateUser

(
  @UserID int,
  @Email varchar(1024),
  @FirstName varchar(100),
  @MiddleName varchar(100),
  @LastName varchar(100),
  @Title varchar(100),
  @CryptedPassword varchar(255),
  @IsActive bit,
  @MarkDeleted bit,
  @TimeZoneID varchar(300),
  @CultureName varchar(50),
  @LastLogin datetime,
  @LastActivity datetime,
  @LastPing datetime,
  @LastWaterCoolerID int,
  @IsSystemAdmin bit,
  @IsFinanceAdmin bit,
  @IsPasswordExpired bit,
  @IsPortalUser bit,
  @IsChatUser bit,
  @PrimaryGroupID int,
  @InOffice bit,
  @InOfficeComment varchar(200),
  @ReceiveTicketNotifications bit,
  @ReceiveAllGroupNotifications bit,
  @SubscribeToNewTickets bit,
  @ActivatedOn datetime,
  @DeactivatedOn datetime,
  @OrganizationID int,
  @LastVersion varchar(50),
  @SessionID uniqueidentifier,
  @ImportID varchar(500),
  @DateModified datetime,
  @ModifierID int,
  @OrgsUserCanSeeOnPortal varchar(200),
  @DoNotAutoSubscribe bit,
  @IsClassicView bit,
  @SubscribeToNewActions bit,
  @ApprovedTerms bit,
  @ShowWelcomePage bit,
  @UserInformation varchar(MAX),
  @PortalAutoReg bit,
  @AppChatID varchar(200),
  @AppChatStatus bit,
  @MenuItems varchar(1000),
  @TicketRights int,
  @Signature varchar(MAX),
  @LinkedIn varchar(200),
  @OnlyEmailAfterHours bit,
  @BlockInboundEmail bit,
  @SalesForceID varchar(8000),
  @ChangeTicketVisibility bit,
  @ChangeKBVisibility bit,
  @EnforceSingleSession bit,
  @NeedsIndexing bit,
  @AllowAnyTicketCustomer bit,
  @FontFamily int,
  @FontSize int,
  @CanCreateCompany bit,
  @CanEditCompany bit,
  @CanCreateContact bit,
  @CanEditContact bit,
  @RestrictUserFromEditingAnyActions bit,
  @AllowUserToEditAnyAction bit,
  @UserCanPinAction bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Users]
  SET
    [Email] = @Email,
    [FirstName] = @FirstName,
    [MiddleName] = @MiddleName,
    [LastName] = @LastName,
    [Title] = @Title,
    [CryptedPassword] = @CryptedPassword,
    [IsActive] = @IsActive,
    [MarkDeleted] = @MarkDeleted,
    [TimeZoneID] = @TimeZoneID,
    [CultureName] = @CultureName,
    [LastLogin] = @LastLogin,
    [LastActivity] = @LastActivity,
    [LastPing] = @LastPing,
    [LastWaterCoolerID] = @LastWaterCoolerID,
    [IsSystemAdmin] = @IsSystemAdmin,
    [IsFinanceAdmin] = @IsFinanceAdmin,
    [IsPasswordExpired] = @IsPasswordExpired,
    [IsPortalUser] = @IsPortalUser,
    [IsChatUser] = @IsChatUser,
    [PrimaryGroupID] = @PrimaryGroupID,
    [InOffice] = @InOffice,
    [InOfficeComment] = @InOfficeComment,
    [ReceiveTicketNotifications] = @ReceiveTicketNotifications,
    [ReceiveAllGroupNotifications] = @ReceiveAllGroupNotifications,
    [SubscribeToNewTickets] = @SubscribeToNewTickets,
    [ActivatedOn] = @ActivatedOn,
    [DeactivatedOn] = @DeactivatedOn,
    [OrganizationID] = @OrganizationID,
    [LastVersion] = @LastVersion,
    [SessionID] = @SessionID,
    [ImportID] = @ImportID,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [OrgsUserCanSeeOnPortal] = @OrgsUserCanSeeOnPortal,
    [DoNotAutoSubscribe] = @DoNotAutoSubscribe,
    [IsClassicView] = @IsClassicView,
    [SubscribeToNewActions] = @SubscribeToNewActions,
    [ApprovedTerms] = @ApprovedTerms,
    [ShowWelcomePage] = @ShowWelcomePage,
    [UserInformation] = @UserInformation,
    [PortalAutoReg] = @PortalAutoReg,
    [AppChatID] = @AppChatID,
    [AppChatStatus] = @AppChatStatus,
    [MenuItems] = @MenuItems,
    [TicketRights] = @TicketRights,
    [Signature] = @Signature,
    [LinkedIn] = @LinkedIn,
    [OnlyEmailAfterHours] = @OnlyEmailAfterHours,
    [BlockInboundEmail] = @BlockInboundEmail,
    [SalesForceID] = @SalesForceID,
    [ChangeTicketVisibility] = @ChangeTicketVisibility,
    [ChangeKBVisibility] = @ChangeKBVisibility,
    [EnforceSingleSession] = @EnforceSingleSession,
    [NeedsIndexing] = @NeedsIndexing,
    [AllowAnyTicketCustomer] = @AllowAnyTicketCustomer,
    [FontFamily] = @FontFamily,
    [FontSize] = @FontSize,
    [CanCreateCompany] = @CanCreateCompany,
    [CanEditCompany] = @CanEditCompany,
    [CanCreateContact] = @CanCreateContact,
    [CanEditContact] = @CanEditContact,
    [RestrictUserFromEditingAnyActions] = @RestrictUserFromEditingAnyActions,
    [AllowUserToEditAnyAction] = @AllowUserToEditAnyAction,
    [UserCanPinAction] = @UserCanPinAction
  WHERE ([UserID] = @UserID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteUser' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteUser
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteUser

(
  @UserID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Users]
  WHERE ([UserID] = @UserID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAction' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAction
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAction

(
  @ActionID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ActionID],
    [ActionTypeID],
    [SystemActionTypeID],
    [Name],
    [Description],
    [TimeSpent],
    [DateStarted],
    [IsVisibleOnPortal],
    [IsKnowledgeBase],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [TicketID],
    [ActionSource],
    [SalesForceID],
    [DateModifiedBySalesForceSync],
    [Pinned]
  FROM [dbo].[Actions]
  WHERE ([ActionID] = @ActionID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAction' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAction
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAction

(
  @ActionTypeID int,
  @SystemActionTypeID int,
  @Name varchar(500),
  @Description varchar(MAX),
  @TimeSpent int,
  @DateStarted datetime,
  @IsVisibleOnPortal bit,
  @IsKnowledgeBase bit,
  @ImportID varchar(50),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @TicketID int,
  @ActionSource varchar(50),
  @SalesForceID varchar(8000),
  @DateModifiedBySalesForceSync datetime,
  @Pinned bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Actions]
  (
    [ActionTypeID],
    [SystemActionTypeID],
    [Name],
    [Description],
    [TimeSpent],
    [DateStarted],
    [IsVisibleOnPortal],
    [IsKnowledgeBase],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [TicketID],
    [ActionSource],
    [SalesForceID],
    [DateModifiedBySalesForceSync],
    [Pinned])
  VALUES (
    @ActionTypeID,
    @SystemActionTypeID,
    @Name,
    @Description,
    @TimeSpent,
    @DateStarted,
    @IsVisibleOnPortal,
    @IsKnowledgeBase,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @TicketID,
    @ActionSource,
    @SalesForceID,
    @DateModifiedBySalesForceSync,
    @Pinned)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAction' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAction
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAction

(
  @ActionID int,
  @ActionTypeID int,
  @SystemActionTypeID int,
  @Name varchar(500),
  @Description varchar(MAX),
  @TimeSpent int,
  @DateStarted datetime,
  @IsVisibleOnPortal bit,
  @IsKnowledgeBase bit,
  @ImportID varchar(50),
  @DateModified datetime,
  @ModifierID int,
  @TicketID int,
  @ActionSource varchar(50),
  @SalesForceID varchar(8000),
  @DateModifiedBySalesForceSync datetime,
  @Pinned bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Actions]
  SET
    [ActionTypeID] = @ActionTypeID,
    [SystemActionTypeID] = @SystemActionTypeID,
    [Name] = @Name,
    [Description] = @Description,
    [TimeSpent] = @TimeSpent,
    [DateStarted] = @DateStarted,
    [IsVisibleOnPortal] = @IsVisibleOnPortal,
    [IsKnowledgeBase] = @IsKnowledgeBase,
    [ImportID] = @ImportID,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [TicketID] = @TicketID,
    [ActionSource] = @ActionSource,
    [SalesForceID] = @SalesForceID,
    [DateModifiedBySalesForceSync] = @DateModifiedBySalesForceSync,
    [Pinned] = @Pinned
  WHERE ([ActionID] = @ActionID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAction' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAction
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAction

(
  @ActionID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Actions]
  WHERE ([ActionID] = @ActionID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectUser' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectUser
GO

CREATE PROCEDURE dbo.uspGeneratedSelectUser

(
  @UserID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [UserID],
    [Email],
    [FirstName],
    [MiddleName],
    [LastName],
    [Title],
    [CryptedPassword],
    [IsActive],
    [MarkDeleted],
    [TimeZoneID],
    [CultureName],
    [LastLogin],
    [LastActivity],
    [LastPing],
    [LastWaterCoolerID],
    [IsSystemAdmin],
    [IsFinanceAdmin],
    [IsPasswordExpired],
    [IsPortalUser],
    [IsChatUser],
    [PrimaryGroupID],
    [InOffice],
    [InOfficeComment],
    [ReceiveTicketNotifications],
    [ReceiveAllGroupNotifications],
    [SubscribeToNewTickets],
    [ActivatedOn],
    [DeactivatedOn],
    [OrganizationID],
    [LastVersion],
    [SessionID],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [OrgsUserCanSeeOnPortal],
    [DoNotAutoSubscribe],
    [IsClassicView],
    [SubscribeToNewActions],
    [ApprovedTerms],
    [ShowWelcomePage],
    [UserInformation],
    [PortalAutoReg],
    [AppChatID],
    [AppChatStatus],
    [MenuItems],
    [TicketRights],
    [Signature],
    [LinkedIn],
    [OnlyEmailAfterHours],
    [BlockInboundEmail],
    [SalesForceID],
    [ChangeTicketVisibility],
    [ChangeKBVisibility],
    [EnforceSingleSession],
    [NeedsIndexing],
    [AllowAnyTicketCustomer],
    [FontFamily],
    [FontSize],
    [CanCreateCompany],
    [CanEditCompany],
    [CanCreateContact],
    [CanEditContact],
    [RestrictUserFromEditingAnyActions],
    [AllowUserToEditAnyAction],
    [UserCanPinAction]
  FROM [dbo].[Users]
  WHERE ([UserID] = @UserID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertUser' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertUser
GO

CREATE PROCEDURE dbo.uspGeneratedInsertUser

(
  @Email varchar(1024),
  @FirstName varchar(100),
  @MiddleName varchar(100),
  @LastName varchar(100),
  @Title varchar(100),
  @CryptedPassword varchar(255),
  @IsActive bit,
  @MarkDeleted bit,
  @TimeZoneID varchar(300),
  @CultureName varchar(50),
  @LastLogin datetime,
  @LastActivity datetime,
  @LastPing datetime,
  @LastWaterCoolerID int,
  @IsSystemAdmin bit,
  @IsFinanceAdmin bit,
  @IsPasswordExpired bit,
  @IsPortalUser bit,
  @IsChatUser bit,
  @PrimaryGroupID int,
  @InOffice bit,
  @InOfficeComment varchar(200),
  @ReceiveTicketNotifications bit,
  @ReceiveAllGroupNotifications bit,
  @SubscribeToNewTickets bit,
  @ActivatedOn datetime,
  @DeactivatedOn datetime,
  @OrganizationID int,
  @LastVersion varchar(50),
  @SessionID uniqueidentifier,
  @ImportID varchar(500),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @OrgsUserCanSeeOnPortal varchar(200),
  @DoNotAutoSubscribe bit,
  @IsClassicView bit,
  @SubscribeToNewActions bit,
  @ApprovedTerms bit,
  @ShowWelcomePage bit,
  @UserInformation varchar(MAX),
  @PortalAutoReg bit,
  @AppChatID varchar(200),
  @AppChatStatus bit,
  @MenuItems varchar(1000),
  @TicketRights int,
  @Signature varchar(MAX),
  @LinkedIn varchar(200),
  @OnlyEmailAfterHours bit,
  @BlockInboundEmail bit,
  @SalesForceID varchar(8000),
  @ChangeTicketVisibility bit,
  @ChangeKBVisibility bit,
  @EnforceSingleSession bit,
  @NeedsIndexing bit,
  @AllowAnyTicketCustomer bit,
  @FontFamily int,
  @FontSize int,
  @CanCreateCompany bit,
  @CanEditCompany bit,
  @CanCreateContact bit,
  @CanEditContact bit,
  @RestrictUserFromEditingAnyActions bit,
  @AllowUserToEditAnyAction bit,
  @UserCanPinAction bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Users]
  (
    [Email],
    [FirstName],
    [MiddleName],
    [LastName],
    [Title],
    [CryptedPassword],
    [IsActive],
    [MarkDeleted],
    [TimeZoneID],
    [CultureName],
    [LastLogin],
    [LastActivity],
    [LastPing],
    [LastWaterCoolerID],
    [IsSystemAdmin],
    [IsFinanceAdmin],
    [IsPasswordExpired],
    [IsPortalUser],
    [IsChatUser],
    [PrimaryGroupID],
    [InOffice],
    [InOfficeComment],
    [ReceiveTicketNotifications],
    [ReceiveAllGroupNotifications],
    [SubscribeToNewTickets],
    [ActivatedOn],
    [DeactivatedOn],
    [OrganizationID],
    [LastVersion],
    [SessionID],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [OrgsUserCanSeeOnPortal],
    [DoNotAutoSubscribe],
    [IsClassicView],
    [SubscribeToNewActions],
    [ApprovedTerms],
    [ShowWelcomePage],
    [UserInformation],
    [PortalAutoReg],
    [AppChatID],
    [AppChatStatus],
    [MenuItems],
    [TicketRights],
    [Signature],
    [LinkedIn],
    [OnlyEmailAfterHours],
    [BlockInboundEmail],
    [SalesForceID],
    [ChangeTicketVisibility],
    [ChangeKBVisibility],
    [EnforceSingleSession],
    [NeedsIndexing],
    [AllowAnyTicketCustomer],
    [FontFamily],
    [FontSize],
    [CanCreateCompany],
    [CanEditCompany],
    [CanCreateContact],
    [CanEditContact],
    [RestrictUserFromEditingAnyActions],
    [AllowUserToEditAnyAction],
    [UserCanPinAction])
  VALUES (
    @Email,
    @FirstName,
    @MiddleName,
    @LastName,
    @Title,
    @CryptedPassword,
    @IsActive,
    @MarkDeleted,
    @TimeZoneID,
    @CultureName,
    @LastLogin,
    @LastActivity,
    @LastPing,
    @LastWaterCoolerID,
    @IsSystemAdmin,
    @IsFinanceAdmin,
    @IsPasswordExpired,
    @IsPortalUser,
    @IsChatUser,
    @PrimaryGroupID,
    @InOffice,
    @InOfficeComment,
    @ReceiveTicketNotifications,
    @ReceiveAllGroupNotifications,
    @SubscribeToNewTickets,
    @ActivatedOn,
    @DeactivatedOn,
    @OrganizationID,
    @LastVersion,
    @SessionID,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @OrgsUserCanSeeOnPortal,
    @DoNotAutoSubscribe,
    @IsClassicView,
    @SubscribeToNewActions,
    @ApprovedTerms,
    @ShowWelcomePage,
    @UserInformation,
    @PortalAutoReg,
    @AppChatID,
    @AppChatStatus,
    @MenuItems,
    @TicketRights,
    @Signature,
    @LinkedIn,
    @OnlyEmailAfterHours,
    @BlockInboundEmail,
    @SalesForceID,
    @ChangeTicketVisibility,
    @ChangeKBVisibility,
    @EnforceSingleSession,
    @NeedsIndexing,
    @AllowAnyTicketCustomer,
    @FontFamily,
    @FontSize,
    @CanCreateCompany,
    @CanEditCompany,
    @CanCreateContact,
    @CanEditContact,
    @RestrictUserFromEditingAnyActions,
    @AllowUserToEditAnyAction,
    @UserCanPinAction)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateUser' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateUser
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateUser

(
  @UserID int,
  @Email varchar(1024),
  @FirstName varchar(100),
  @MiddleName varchar(100),
  @LastName varchar(100),
  @Title varchar(100),
  @CryptedPassword varchar(255),
  @IsActive bit,
  @MarkDeleted bit,
  @TimeZoneID varchar(300),
  @CultureName varchar(50),
  @LastLogin datetime,
  @LastActivity datetime,
  @LastPing datetime,
  @LastWaterCoolerID int,
  @IsSystemAdmin bit,
  @IsFinanceAdmin bit,
  @IsPasswordExpired bit,
  @IsPortalUser bit,
  @IsChatUser bit,
  @PrimaryGroupID int,
  @InOffice bit,
  @InOfficeComment varchar(200),
  @ReceiveTicketNotifications bit,
  @ReceiveAllGroupNotifications bit,
  @SubscribeToNewTickets bit,
  @ActivatedOn datetime,
  @DeactivatedOn datetime,
  @OrganizationID int,
  @LastVersion varchar(50),
  @SessionID uniqueidentifier,
  @ImportID varchar(500),
  @DateModified datetime,
  @ModifierID int,
  @OrgsUserCanSeeOnPortal varchar(200),
  @DoNotAutoSubscribe bit,
  @IsClassicView bit,
  @SubscribeToNewActions bit,
  @ApprovedTerms bit,
  @ShowWelcomePage bit,
  @UserInformation varchar(MAX),
  @PortalAutoReg bit,
  @AppChatID varchar(200),
  @AppChatStatus bit,
  @MenuItems varchar(1000),
  @TicketRights int,
  @Signature varchar(MAX),
  @LinkedIn varchar(200),
  @OnlyEmailAfterHours bit,
  @BlockInboundEmail bit,
  @SalesForceID varchar(8000),
  @ChangeTicketVisibility bit,
  @ChangeKBVisibility bit,
  @EnforceSingleSession bit,
  @NeedsIndexing bit,
  @AllowAnyTicketCustomer bit,
  @FontFamily int,
  @FontSize int,
  @CanCreateCompany bit,
  @CanEditCompany bit,
  @CanCreateContact bit,
  @CanEditContact bit,
  @RestrictUserFromEditingAnyActions bit,
  @AllowUserToEditAnyAction bit,
  @UserCanPinAction bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Users]
  SET
    [Email] = @Email,
    [FirstName] = @FirstName,
    [MiddleName] = @MiddleName,
    [LastName] = @LastName,
    [Title] = @Title,
    [CryptedPassword] = @CryptedPassword,
    [IsActive] = @IsActive,
    [MarkDeleted] = @MarkDeleted,
    [TimeZoneID] = @TimeZoneID,
    [CultureName] = @CultureName,
    [LastLogin] = @LastLogin,
    [LastActivity] = @LastActivity,
    [LastPing] = @LastPing,
    [LastWaterCoolerID] = @LastWaterCoolerID,
    [IsSystemAdmin] = @IsSystemAdmin,
    [IsFinanceAdmin] = @IsFinanceAdmin,
    [IsPasswordExpired] = @IsPasswordExpired,
    [IsPortalUser] = @IsPortalUser,
    [IsChatUser] = @IsChatUser,
    [PrimaryGroupID] = @PrimaryGroupID,
    [InOffice] = @InOffice,
    [InOfficeComment] = @InOfficeComment,
    [ReceiveTicketNotifications] = @ReceiveTicketNotifications,
    [ReceiveAllGroupNotifications] = @ReceiveAllGroupNotifications,
    [SubscribeToNewTickets] = @SubscribeToNewTickets,
    [ActivatedOn] = @ActivatedOn,
    [DeactivatedOn] = @DeactivatedOn,
    [OrganizationID] = @OrganizationID,
    [LastVersion] = @LastVersion,
    [SessionID] = @SessionID,
    [ImportID] = @ImportID,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [OrgsUserCanSeeOnPortal] = @OrgsUserCanSeeOnPortal,
    [DoNotAutoSubscribe] = @DoNotAutoSubscribe,
    [IsClassicView] = @IsClassicView,
    [SubscribeToNewActions] = @SubscribeToNewActions,
    [ApprovedTerms] = @ApprovedTerms,
    [ShowWelcomePage] = @ShowWelcomePage,
    [UserInformation] = @UserInformation,
    [PortalAutoReg] = @PortalAutoReg,
    [AppChatID] = @AppChatID,
    [AppChatStatus] = @AppChatStatus,
    [MenuItems] = @MenuItems,
    [TicketRights] = @TicketRights,
    [Signature] = @Signature,
    [LinkedIn] = @LinkedIn,
    [OnlyEmailAfterHours] = @OnlyEmailAfterHours,
    [BlockInboundEmail] = @BlockInboundEmail,
    [SalesForceID] = @SalesForceID,
    [ChangeTicketVisibility] = @ChangeTicketVisibility,
    [ChangeKBVisibility] = @ChangeKBVisibility,
    [EnforceSingleSession] = @EnforceSingleSession,
    [NeedsIndexing] = @NeedsIndexing,
    [AllowAnyTicketCustomer] = @AllowAnyTicketCustomer,
    [FontFamily] = @FontFamily,
    [FontSize] = @FontSize,
    [CanCreateCompany] = @CanCreateCompany,
    [CanEditCompany] = @CanEditCompany,
    [CanCreateContact] = @CanCreateContact,
    [CanEditContact] = @CanEditContact,
    [RestrictUserFromEditingAnyActions] = @RestrictUserFromEditingAnyActions,
    [AllowUserToEditAnyAction] = @AllowUserToEditAnyAction,
    [UserCanPinAction] = @UserCanPinAction
  WHERE ([UserID] = @UserID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteUser' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteUser
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteUser

(
  @UserID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Users]
  WHERE ([UserID] = @UserID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAction' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAction
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAction

(
  @ActionID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ActionID],
    [ActionTypeID],
    [SystemActionTypeID],
    [Name],
    [Description],
    [TimeSpent],
    [DateStarted],
    [IsVisibleOnPortal],
    [IsKnowledgeBase],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [TicketID],
    [ActionSource],
    [SalesForceID],
    [DateModifiedBySalesForceSync],
    [Pinned]
  FROM [dbo].[Actions]
  WHERE ([ActionID] = @ActionID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAction' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAction
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAction

(
  @ActionTypeID int,
  @SystemActionTypeID int,
  @Name varchar(500),
  @Description varchar(MAX),
  @TimeSpent int,
  @DateStarted datetime,
  @IsVisibleOnPortal bit,
  @IsKnowledgeBase bit,
  @ImportID varchar(50),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @TicketID int,
  @ActionSource varchar(50),
  @SalesForceID varchar(8000),
  @DateModifiedBySalesForceSync datetime,
  @Pinned bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Actions]
  (
    [ActionTypeID],
    [SystemActionTypeID],
    [Name],
    [Description],
    [TimeSpent],
    [DateStarted],
    [IsVisibleOnPortal],
    [IsKnowledgeBase],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [TicketID],
    [ActionSource],
    [SalesForceID],
    [DateModifiedBySalesForceSync],
    [Pinned])
  VALUES (
    @ActionTypeID,
    @SystemActionTypeID,
    @Name,
    @Description,
    @TimeSpent,
    @DateStarted,
    @IsVisibleOnPortal,
    @IsKnowledgeBase,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @TicketID,
    @ActionSource,
    @SalesForceID,
    @DateModifiedBySalesForceSync,
    @Pinned)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAction' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAction
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAction

(
  @ActionID int,
  @ActionTypeID int,
  @SystemActionTypeID int,
  @Name varchar(500),
  @Description varchar(MAX),
  @TimeSpent int,
  @DateStarted datetime,
  @IsVisibleOnPortal bit,
  @IsKnowledgeBase bit,
  @ImportID varchar(50),
  @DateModified datetime,
  @ModifierID int,
  @TicketID int,
  @ActionSource varchar(50),
  @SalesForceID varchar(8000),
  @DateModifiedBySalesForceSync datetime,
  @Pinned bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Actions]
  SET
    [ActionTypeID] = @ActionTypeID,
    [SystemActionTypeID] = @SystemActionTypeID,
    [Name] = @Name,
    [Description] = @Description,
    [TimeSpent] = @TimeSpent,
    [DateStarted] = @DateStarted,
    [IsVisibleOnPortal] = @IsVisibleOnPortal,
    [IsKnowledgeBase] = @IsKnowledgeBase,
    [ImportID] = @ImportID,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [TicketID] = @TicketID,
    [ActionSource] = @ActionSource,
    [SalesForceID] = @SalesForceID,
    [DateModifiedBySalesForceSync] = @DateModifiedBySalesForceSync,
    [Pinned] = @Pinned
  WHERE ([ActionID] = @ActionID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAction' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAction
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAction

(
  @ActionID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Actions]
  WHERE ([ActionID] = @ActionID)
GO


