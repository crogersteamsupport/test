IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectContactsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectContactsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectContactsViewItem

(
  @UserID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [Email],
    [FirstName],
    [UserID],
    [Name],
    [MiddleName],
    [LastName],
    [Title],
    [IsActive],
    [MarkDeleted],
    [LastLogin],
    [LastActivity],
    [LastPing],
    [IsSystemAdmin],
    [IsFinanceAdmin],
    [IsPasswordExpired],
    [IsPortalUser],
    [PrimaryGroupID],
    [InOffice],
    [InOfficeComment],
    [ActivatedOn],
    [DeactivatedOn],
    [OrganizationID],
    [Organization],
    [LastVersion],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [OrganizationParentID],
    [CryptedPassword],
    [SalesForceID],
    [NeedsIndexing],
    [OrganizationActive],
    [OrganizationSAExpirationDate],
    [PortalLimitOrgTickets]
  FROM [dbo].[ContactsView]
  WHERE ([UserID] = @UserID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertContactsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertContactsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertContactsViewItem

(
  @Email varchar(1024),
  @FirstName varchar(100),
  @UserID int,
  @Name varchar(201),
  @MiddleName varchar(100),
  @LastName varchar(100),
  @Title varchar(100),
  @IsActive bit,
  @MarkDeleted bit,
  @LastLogin datetime,
  @LastActivity datetime,
  @LastPing datetime,
  @IsSystemAdmin bit,
  @IsFinanceAdmin bit,
  @IsPasswordExpired bit,
  @IsPortalUser bit,
  @PrimaryGroupID int,
  @InOffice bit,
  @InOfficeComment varchar(200),
  @ActivatedOn datetime,
  @DeactivatedOn datetime,
  @OrganizationID int,
  @Organization varchar(255),
  @LastVersion varchar(50),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @OrganizationParentID int,
  @CryptedPassword varchar(255),
  @SalesForceID varchar(8000),
  @NeedsIndexing bit,
  @OrganizationActive bit,
  @OrganizationSAExpirationDate datetime,
  @PortalLimitOrgTickets bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ContactsView]
  (
    [Email],
    [FirstName],
    [UserID],
    [Name],
    [MiddleName],
    [LastName],
    [Title],
    [IsActive],
    [MarkDeleted],
    [LastLogin],
    [LastActivity],
    [LastPing],
    [IsSystemAdmin],
    [IsFinanceAdmin],
    [IsPasswordExpired],
    [IsPortalUser],
    [PrimaryGroupID],
    [InOffice],
    [InOfficeComment],
    [ActivatedOn],
    [DeactivatedOn],
    [OrganizationID],
    [Organization],
    [LastVersion],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [OrganizationParentID],
    [CryptedPassword],
    [SalesForceID],
    [NeedsIndexing],
    [OrganizationActive],
    [OrganizationSAExpirationDate],
    [PortalLimitOrgTickets])
  VALUES (
    @Email,
    @FirstName,
    @UserID,
    @Name,
    @MiddleName,
    @LastName,
    @Title,
    @IsActive,
    @MarkDeleted,
    @LastLogin,
    @LastActivity,
    @LastPing,
    @IsSystemAdmin,
    @IsFinanceAdmin,
    @IsPasswordExpired,
    @IsPortalUser,
    @PrimaryGroupID,
    @InOffice,
    @InOfficeComment,
    @ActivatedOn,
    @DeactivatedOn,
    @OrganizationID,
    @Organization,
    @LastVersion,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @OrganizationParentID,
    @CryptedPassword,
    @SalesForceID,
    @NeedsIndexing,
    @OrganizationActive,
    @OrganizationSAExpirationDate,
    @PortalLimitOrgTickets)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateContactsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateContactsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateContactsViewItem

(
  @Email varchar(1024),
  @FirstName varchar(100),
  @UserID int,
  @Name varchar(201),
  @MiddleName varchar(100),
  @LastName varchar(100),
  @Title varchar(100),
  @IsActive bit,
  @MarkDeleted bit,
  @LastLogin datetime,
  @LastActivity datetime,
  @LastPing datetime,
  @IsSystemAdmin bit,
  @IsFinanceAdmin bit,
  @IsPasswordExpired bit,
  @IsPortalUser bit,
  @PrimaryGroupID int,
  @InOffice bit,
  @InOfficeComment varchar(200),
  @ActivatedOn datetime,
  @DeactivatedOn datetime,
  @OrganizationID int,
  @Organization varchar(255),
  @LastVersion varchar(50),
  @DateModified datetime,
  @ModifierID int,
  @OrganizationParentID int,
  @CryptedPassword varchar(255),
  @SalesForceID varchar(8000),
  @NeedsIndexing bit,
  @OrganizationActive bit,
  @OrganizationSAExpirationDate datetime,
  @PortalLimitOrgTickets bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ContactsView]
  SET
    [Email] = @Email,
    [FirstName] = @FirstName,
    [Name] = @Name,
    [MiddleName] = @MiddleName,
    [LastName] = @LastName,
    [Title] = @Title,
    [IsActive] = @IsActive,
    [MarkDeleted] = @MarkDeleted,
    [LastLogin] = @LastLogin,
    [LastActivity] = @LastActivity,
    [LastPing] = @LastPing,
    [IsSystemAdmin] = @IsSystemAdmin,
    [IsFinanceAdmin] = @IsFinanceAdmin,
    [IsPasswordExpired] = @IsPasswordExpired,
    [IsPortalUser] = @IsPortalUser,
    [PrimaryGroupID] = @PrimaryGroupID,
    [InOffice] = @InOffice,
    [InOfficeComment] = @InOfficeComment,
    [ActivatedOn] = @ActivatedOn,
    [DeactivatedOn] = @DeactivatedOn,
    [OrganizationID] = @OrganizationID,
    [Organization] = @Organization,
    [LastVersion] = @LastVersion,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [OrganizationParentID] = @OrganizationParentID,
    [CryptedPassword] = @CryptedPassword,
    [SalesForceID] = @SalesForceID,
    [NeedsIndexing] = @NeedsIndexing,
    [OrganizationActive] = @OrganizationActive,
    [OrganizationSAExpirationDate] = @OrganizationSAExpirationDate,
    [PortalLimitOrgTickets] = @PortalLimitOrgTickets
  WHERE ([UserID] = @UserID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteContactsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteContactsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteContactsViewItem

(
  @UserID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ContactsView]
  WHERE ([UserID] = @UserID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectContactsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectContactsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectContactsViewItem

(
  @UserID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [Email],
    [FirstName],
    [UserID],
    [Name],
    [MiddleName],
    [LastName],
    [Title],
    [IsActive],
    [MarkDeleted],
    [LastLogin],
    [LastActivity],
    [LastPing],
    [IsSystemAdmin],
    [IsFinanceAdmin],
    [IsPasswordExpired],
    [IsPortalUser],
    [PrimaryGroupID],
    [InOffice],
    [InOfficeComment],
    [ActivatedOn],
    [DeactivatedOn],
    [OrganizationID],
    [Organization],
    [LastVersion],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [OrganizationParentID],
    [CryptedPassword],
    [SalesForceID],
    [NeedsIndexing],
    [OrganizationActive],
    [OrganizationSAExpirationDate],
    [PortalLimitOrgTickets]
  FROM [dbo].[ContactsView]
  WHERE ([UserID] = @UserID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertContactsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertContactsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertContactsViewItem

(
  @Email varchar(1024),
  @FirstName varchar(100),
  @UserID int,
  @Name varchar(201),
  @MiddleName varchar(100),
  @LastName varchar(100),
  @Title varchar(100),
  @IsActive bit,
  @MarkDeleted bit,
  @LastLogin datetime,
  @LastActivity datetime,
  @LastPing datetime,
  @IsSystemAdmin bit,
  @IsFinanceAdmin bit,
  @IsPasswordExpired bit,
  @IsPortalUser bit,
  @PrimaryGroupID int,
  @InOffice bit,
  @InOfficeComment varchar(200),
  @ActivatedOn datetime,
  @DeactivatedOn datetime,
  @OrganizationID int,
  @Organization varchar(255),
  @LastVersion varchar(50),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @OrganizationParentID int,
  @CryptedPassword varchar(255),
  @SalesForceID varchar(8000),
  @NeedsIndexing bit,
  @OrganizationActive bit,
  @OrganizationSAExpirationDate datetime,
  @PortalLimitOrgTickets bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ContactsView]
  (
    [Email],
    [FirstName],
    [UserID],
    [Name],
    [MiddleName],
    [LastName],
    [Title],
    [IsActive],
    [MarkDeleted],
    [LastLogin],
    [LastActivity],
    [LastPing],
    [IsSystemAdmin],
    [IsFinanceAdmin],
    [IsPasswordExpired],
    [IsPortalUser],
    [PrimaryGroupID],
    [InOffice],
    [InOfficeComment],
    [ActivatedOn],
    [DeactivatedOn],
    [OrganizationID],
    [Organization],
    [LastVersion],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [OrganizationParentID],
    [CryptedPassword],
    [SalesForceID],
    [NeedsIndexing],
    [OrganizationActive],
    [OrganizationSAExpirationDate],
    [PortalLimitOrgTickets])
  VALUES (
    @Email,
    @FirstName,
    @UserID,
    @Name,
    @MiddleName,
    @LastName,
    @Title,
    @IsActive,
    @MarkDeleted,
    @LastLogin,
    @LastActivity,
    @LastPing,
    @IsSystemAdmin,
    @IsFinanceAdmin,
    @IsPasswordExpired,
    @IsPortalUser,
    @PrimaryGroupID,
    @InOffice,
    @InOfficeComment,
    @ActivatedOn,
    @DeactivatedOn,
    @OrganizationID,
    @Organization,
    @LastVersion,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @OrganizationParentID,
    @CryptedPassword,
    @SalesForceID,
    @NeedsIndexing,
    @OrganizationActive,
    @OrganizationSAExpirationDate,
    @PortalLimitOrgTickets)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateContactsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateContactsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateContactsViewItem

(
  @Email varchar(1024),
  @FirstName varchar(100),
  @UserID int,
  @Name varchar(201),
  @MiddleName varchar(100),
  @LastName varchar(100),
  @Title varchar(100),
  @IsActive bit,
  @MarkDeleted bit,
  @LastLogin datetime,
  @LastActivity datetime,
  @LastPing datetime,
  @IsSystemAdmin bit,
  @IsFinanceAdmin bit,
  @IsPasswordExpired bit,
  @IsPortalUser bit,
  @PrimaryGroupID int,
  @InOffice bit,
  @InOfficeComment varchar(200),
  @ActivatedOn datetime,
  @DeactivatedOn datetime,
  @OrganizationID int,
  @Organization varchar(255),
  @LastVersion varchar(50),
  @DateModified datetime,
  @ModifierID int,
  @OrganizationParentID int,
  @CryptedPassword varchar(255),
  @SalesForceID varchar(8000),
  @NeedsIndexing bit,
  @OrganizationActive bit,
  @OrganizationSAExpirationDate datetime,
  @PortalLimitOrgTickets bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ContactsView]
  SET
    [Email] = @Email,
    [FirstName] = @FirstName,
    [Name] = @Name,
    [MiddleName] = @MiddleName,
    [LastName] = @LastName,
    [Title] = @Title,
    [IsActive] = @IsActive,
    [MarkDeleted] = @MarkDeleted,
    [LastLogin] = @LastLogin,
    [LastActivity] = @LastActivity,
    [LastPing] = @LastPing,
    [IsSystemAdmin] = @IsSystemAdmin,
    [IsFinanceAdmin] = @IsFinanceAdmin,
    [IsPasswordExpired] = @IsPasswordExpired,
    [IsPortalUser] = @IsPortalUser,
    [PrimaryGroupID] = @PrimaryGroupID,
    [InOffice] = @InOffice,
    [InOfficeComment] = @InOfficeComment,
    [ActivatedOn] = @ActivatedOn,
    [DeactivatedOn] = @DeactivatedOn,
    [OrganizationID] = @OrganizationID,
    [Organization] = @Organization,
    [LastVersion] = @LastVersion,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [OrganizationParentID] = @OrganizationParentID,
    [CryptedPassword] = @CryptedPassword,
    [SalesForceID] = @SalesForceID,
    [NeedsIndexing] = @NeedsIndexing,
    [OrganizationActive] = @OrganizationActive,
    [OrganizationSAExpirationDate] = @OrganizationSAExpirationDate,
    [PortalLimitOrgTickets] = @PortalLimitOrgTickets
  WHERE ([UserID] = @UserID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteContactsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteContactsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteContactsViewItem

(
  @UserID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ContactsView]
  WHERE ([UserID] = @UserID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectContactsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectContactsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectContactsViewItem

(
  @UserID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [Email],
    [FirstName],
    [UserID],
    [Name],
    [MiddleName],
    [LastName],
    [Title],
    [IsActive],
    [MarkDeleted],
    [LastLogin],
    [LastActivity],
    [LastPing],
    [IsSystemAdmin],
    [IsFinanceAdmin],
    [IsPasswordExpired],
    [IsPortalUser],
    [PrimaryGroupID],
    [InOffice],
    [InOfficeComment],
    [ActivatedOn],
    [DeactivatedOn],
    [OrganizationID],
    [Organization],
    [LastVersion],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [OrganizationParentID],
    [CryptedPassword],
    [SalesForceID],
    [NeedsIndexing],
    [OrganizationActive],
    [OrganizationSAExpirationDate],
    [PortalLimitOrgTickets]
  FROM [dbo].[ContactsView]
  WHERE ([UserID] = @UserID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertContactsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertContactsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertContactsViewItem

(
  @Email varchar(1024),
  @FirstName varchar(100),
  @UserID int,
  @Name varchar(201),
  @MiddleName varchar(100),
  @LastName varchar(100),
  @Title varchar(100),
  @IsActive bit,
  @MarkDeleted bit,
  @LastLogin datetime,
  @LastActivity datetime,
  @LastPing datetime,
  @IsSystemAdmin bit,
  @IsFinanceAdmin bit,
  @IsPasswordExpired bit,
  @IsPortalUser bit,
  @PrimaryGroupID int,
  @InOffice bit,
  @InOfficeComment varchar(200),
  @ActivatedOn datetime,
  @DeactivatedOn datetime,
  @OrganizationID int,
  @Organization varchar(255),
  @LastVersion varchar(50),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @OrganizationParentID int,
  @CryptedPassword varchar(255),
  @SalesForceID varchar(8000),
  @NeedsIndexing bit,
  @OrganizationActive bit,
  @OrganizationSAExpirationDate datetime,
  @PortalLimitOrgTickets bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ContactsView]
  (
    [Email],
    [FirstName],
    [UserID],
    [Name],
    [MiddleName],
    [LastName],
    [Title],
    [IsActive],
    [MarkDeleted],
    [LastLogin],
    [LastActivity],
    [LastPing],
    [IsSystemAdmin],
    [IsFinanceAdmin],
    [IsPasswordExpired],
    [IsPortalUser],
    [PrimaryGroupID],
    [InOffice],
    [InOfficeComment],
    [ActivatedOn],
    [DeactivatedOn],
    [OrganizationID],
    [Organization],
    [LastVersion],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [OrganizationParentID],
    [CryptedPassword],
    [SalesForceID],
    [NeedsIndexing],
    [OrganizationActive],
    [OrganizationSAExpirationDate],
    [PortalLimitOrgTickets])
  VALUES (
    @Email,
    @FirstName,
    @UserID,
    @Name,
    @MiddleName,
    @LastName,
    @Title,
    @IsActive,
    @MarkDeleted,
    @LastLogin,
    @LastActivity,
    @LastPing,
    @IsSystemAdmin,
    @IsFinanceAdmin,
    @IsPasswordExpired,
    @IsPortalUser,
    @PrimaryGroupID,
    @InOffice,
    @InOfficeComment,
    @ActivatedOn,
    @DeactivatedOn,
    @OrganizationID,
    @Organization,
    @LastVersion,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @OrganizationParentID,
    @CryptedPassword,
    @SalesForceID,
    @NeedsIndexing,
    @OrganizationActive,
    @OrganizationSAExpirationDate,
    @PortalLimitOrgTickets)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateContactsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateContactsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateContactsViewItem

(
  @Email varchar(1024),
  @FirstName varchar(100),
  @UserID int,
  @Name varchar(201),
  @MiddleName varchar(100),
  @LastName varchar(100),
  @Title varchar(100),
  @IsActive bit,
  @MarkDeleted bit,
  @LastLogin datetime,
  @LastActivity datetime,
  @LastPing datetime,
  @IsSystemAdmin bit,
  @IsFinanceAdmin bit,
  @IsPasswordExpired bit,
  @IsPortalUser bit,
  @PrimaryGroupID int,
  @InOffice bit,
  @InOfficeComment varchar(200),
  @ActivatedOn datetime,
  @DeactivatedOn datetime,
  @OrganizationID int,
  @Organization varchar(255),
  @LastVersion varchar(50),
  @DateModified datetime,
  @ModifierID int,
  @OrganizationParentID int,
  @CryptedPassword varchar(255),
  @SalesForceID varchar(8000),
  @NeedsIndexing bit,
  @OrganizationActive bit,
  @OrganizationSAExpirationDate datetime,
  @PortalLimitOrgTickets bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ContactsView]
  SET
    [Email] = @Email,
    [FirstName] = @FirstName,
    [Name] = @Name,
    [MiddleName] = @MiddleName,
    [LastName] = @LastName,
    [Title] = @Title,
    [IsActive] = @IsActive,
    [MarkDeleted] = @MarkDeleted,
    [LastLogin] = @LastLogin,
    [LastActivity] = @LastActivity,
    [LastPing] = @LastPing,
    [IsSystemAdmin] = @IsSystemAdmin,
    [IsFinanceAdmin] = @IsFinanceAdmin,
    [IsPasswordExpired] = @IsPasswordExpired,
    [IsPortalUser] = @IsPortalUser,
    [PrimaryGroupID] = @PrimaryGroupID,
    [InOffice] = @InOffice,
    [InOfficeComment] = @InOfficeComment,
    [ActivatedOn] = @ActivatedOn,
    [DeactivatedOn] = @DeactivatedOn,
    [OrganizationID] = @OrganizationID,
    [Organization] = @Organization,
    [LastVersion] = @LastVersion,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [OrganizationParentID] = @OrganizationParentID,
    [CryptedPassword] = @CryptedPassword,
    [SalesForceID] = @SalesForceID,
    [NeedsIndexing] = @NeedsIndexing,
    [OrganizationActive] = @OrganizationActive,
    [OrganizationSAExpirationDate] = @OrganizationSAExpirationDate,
    [PortalLimitOrgTickets] = @PortalLimitOrgTickets
  WHERE ([UserID] = @UserID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteContactsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteContactsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteContactsViewItem

(
  @UserID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ContactsView]
  WHERE ([UserID] = @UserID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectContactsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectContactsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectContactsViewItem

(
  @UserID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [Email],
    [FirstName],
    [UserID],
    [Name],
    [MiddleName],
    [LastName],
    [Title],
    [IsActive],
    [MarkDeleted],
    [LastLogin],
    [LastActivity],
    [LastPing],
    [IsSystemAdmin],
    [IsFinanceAdmin],
    [IsPasswordExpired],
    [IsPortalUser],
    [PrimaryGroupID],
    [InOffice],
    [InOfficeComment],
    [ActivatedOn],
    [DeactivatedOn],
    [OrganizationID],
    [Organization],
    [LastVersion],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [OrganizationParentID],
    [CryptedPassword],
    [SalesForceID],
    [NeedsIndexing],
    [OrganizationActive],
    [OrganizationSAExpirationDate],
    [PortalLimitOrgTickets]
  FROM [dbo].[ContactsView]
  WHERE ([UserID] = @UserID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertContactsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertContactsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertContactsViewItem

(
  @Email varchar(1024),
  @FirstName varchar(100),
  @UserID int,
  @Name varchar(201),
  @MiddleName varchar(100),
  @LastName varchar(100),
  @Title varchar(100),
  @IsActive bit,
  @MarkDeleted bit,
  @LastLogin datetime,
  @LastActivity datetime,
  @LastPing datetime,
  @IsSystemAdmin bit,
  @IsFinanceAdmin bit,
  @IsPasswordExpired bit,
  @IsPortalUser bit,
  @PrimaryGroupID int,
  @InOffice bit,
  @InOfficeComment varchar(200),
  @ActivatedOn datetime,
  @DeactivatedOn datetime,
  @OrganizationID int,
  @Organization varchar(255),
  @LastVersion varchar(50),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @OrganizationParentID int,
  @CryptedPassword varchar(255),
  @SalesForceID varchar(8000),
  @NeedsIndexing bit,
  @OrganizationActive bit,
  @OrganizationSAExpirationDate datetime,
  @PortalLimitOrgTickets bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ContactsView]
  (
    [Email],
    [FirstName],
    [UserID],
    [Name],
    [MiddleName],
    [LastName],
    [Title],
    [IsActive],
    [MarkDeleted],
    [LastLogin],
    [LastActivity],
    [LastPing],
    [IsSystemAdmin],
    [IsFinanceAdmin],
    [IsPasswordExpired],
    [IsPortalUser],
    [PrimaryGroupID],
    [InOffice],
    [InOfficeComment],
    [ActivatedOn],
    [DeactivatedOn],
    [OrganizationID],
    [Organization],
    [LastVersion],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [OrganizationParentID],
    [CryptedPassword],
    [SalesForceID],
    [NeedsIndexing],
    [OrganizationActive],
    [OrganizationSAExpirationDate],
    [PortalLimitOrgTickets])
  VALUES (
    @Email,
    @FirstName,
    @UserID,
    @Name,
    @MiddleName,
    @LastName,
    @Title,
    @IsActive,
    @MarkDeleted,
    @LastLogin,
    @LastActivity,
    @LastPing,
    @IsSystemAdmin,
    @IsFinanceAdmin,
    @IsPasswordExpired,
    @IsPortalUser,
    @PrimaryGroupID,
    @InOffice,
    @InOfficeComment,
    @ActivatedOn,
    @DeactivatedOn,
    @OrganizationID,
    @Organization,
    @LastVersion,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @OrganizationParentID,
    @CryptedPassword,
    @SalesForceID,
    @NeedsIndexing,
    @OrganizationActive,
    @OrganizationSAExpirationDate,
    @PortalLimitOrgTickets)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateContactsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateContactsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateContactsViewItem

(
  @Email varchar(1024),
  @FirstName varchar(100),
  @UserID int,
  @Name varchar(201),
  @MiddleName varchar(100),
  @LastName varchar(100),
  @Title varchar(100),
  @IsActive bit,
  @MarkDeleted bit,
  @LastLogin datetime,
  @LastActivity datetime,
  @LastPing datetime,
  @IsSystemAdmin bit,
  @IsFinanceAdmin bit,
  @IsPasswordExpired bit,
  @IsPortalUser bit,
  @PrimaryGroupID int,
  @InOffice bit,
  @InOfficeComment varchar(200),
  @ActivatedOn datetime,
  @DeactivatedOn datetime,
  @OrganizationID int,
  @Organization varchar(255),
  @LastVersion varchar(50),
  @DateModified datetime,
  @ModifierID int,
  @OrganizationParentID int,
  @CryptedPassword varchar(255),
  @SalesForceID varchar(8000),
  @NeedsIndexing bit,
  @OrganizationActive bit,
  @OrganizationSAExpirationDate datetime,
  @PortalLimitOrgTickets bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ContactsView]
  SET
    [Email] = @Email,
    [FirstName] = @FirstName,
    [Name] = @Name,
    [MiddleName] = @MiddleName,
    [LastName] = @LastName,
    [Title] = @Title,
    [IsActive] = @IsActive,
    [MarkDeleted] = @MarkDeleted,
    [LastLogin] = @LastLogin,
    [LastActivity] = @LastActivity,
    [LastPing] = @LastPing,
    [IsSystemAdmin] = @IsSystemAdmin,
    [IsFinanceAdmin] = @IsFinanceAdmin,
    [IsPasswordExpired] = @IsPasswordExpired,
    [IsPortalUser] = @IsPortalUser,
    [PrimaryGroupID] = @PrimaryGroupID,
    [InOffice] = @InOffice,
    [InOfficeComment] = @InOfficeComment,
    [ActivatedOn] = @ActivatedOn,
    [DeactivatedOn] = @DeactivatedOn,
    [OrganizationID] = @OrganizationID,
    [Organization] = @Organization,
    [LastVersion] = @LastVersion,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [OrganizationParentID] = @OrganizationParentID,
    [CryptedPassword] = @CryptedPassword,
    [SalesForceID] = @SalesForceID,
    [NeedsIndexing] = @NeedsIndexing,
    [OrganizationActive] = @OrganizationActive,
    [OrganizationSAExpirationDate] = @OrganizationSAExpirationDate,
    [PortalLimitOrgTickets] = @PortalLimitOrgTickets
  WHERE ([UserID] = @UserID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteContactsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteContactsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteContactsViewItem

(
  @UserID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ContactsView]
  WHERE ([UserID] = @UserID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectContactsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectContactsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectContactsViewItem

(
  @UserID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [Email],
    [FirstName],
    [UserID],
    [Name],
    [MiddleName],
    [LastName],
    [Title],
    [IsActive],
    [MarkDeleted],
    [LastLogin],
    [LastActivity],
    [LastPing],
    [IsSystemAdmin],
    [IsFinanceAdmin],
    [IsPasswordExpired],
    [IsPortalUser],
    [PrimaryGroupID],
    [InOffice],
    [InOfficeComment],
    [ActivatedOn],
    [DeactivatedOn],
    [OrganizationID],
    [Organization],
    [LastVersion],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [OrganizationParentID],
    [CryptedPassword],
    [SalesForceID],
    [NeedsIndexing],
    [OrganizationActive],
    [OrganizationSAExpirationDate],
    [PortalLimitOrgTickets]
  FROM [dbo].[ContactsView]
  WHERE ([UserID] = @UserID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertContactsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertContactsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertContactsViewItem

(
  @Email varchar(1024),
  @FirstName varchar(100),
  @UserID int,
  @Name varchar(201),
  @MiddleName varchar(100),
  @LastName varchar(100),
  @Title varchar(100),
  @IsActive bit,
  @MarkDeleted bit,
  @LastLogin datetime,
  @LastActivity datetime,
  @LastPing datetime,
  @IsSystemAdmin bit,
  @IsFinanceAdmin bit,
  @IsPasswordExpired bit,
  @IsPortalUser bit,
  @PrimaryGroupID int,
  @InOffice bit,
  @InOfficeComment varchar(200),
  @ActivatedOn datetime,
  @DeactivatedOn datetime,
  @OrganizationID int,
  @Organization varchar(255),
  @LastVersion varchar(50),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @OrganizationParentID int,
  @CryptedPassword varchar(255),
  @SalesForceID varchar(8000),
  @NeedsIndexing bit,
  @OrganizationActive bit,
  @OrganizationSAExpirationDate datetime,
  @PortalLimitOrgTickets bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ContactsView]
  (
    [Email],
    [FirstName],
    [UserID],
    [Name],
    [MiddleName],
    [LastName],
    [Title],
    [IsActive],
    [MarkDeleted],
    [LastLogin],
    [LastActivity],
    [LastPing],
    [IsSystemAdmin],
    [IsFinanceAdmin],
    [IsPasswordExpired],
    [IsPortalUser],
    [PrimaryGroupID],
    [InOffice],
    [InOfficeComment],
    [ActivatedOn],
    [DeactivatedOn],
    [OrganizationID],
    [Organization],
    [LastVersion],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [OrganizationParentID],
    [CryptedPassword],
    [SalesForceID],
    [NeedsIndexing],
    [OrganizationActive],
    [OrganizationSAExpirationDate],
    [PortalLimitOrgTickets])
  VALUES (
    @Email,
    @FirstName,
    @UserID,
    @Name,
    @MiddleName,
    @LastName,
    @Title,
    @IsActive,
    @MarkDeleted,
    @LastLogin,
    @LastActivity,
    @LastPing,
    @IsSystemAdmin,
    @IsFinanceAdmin,
    @IsPasswordExpired,
    @IsPortalUser,
    @PrimaryGroupID,
    @InOffice,
    @InOfficeComment,
    @ActivatedOn,
    @DeactivatedOn,
    @OrganizationID,
    @Organization,
    @LastVersion,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @OrganizationParentID,
    @CryptedPassword,
    @SalesForceID,
    @NeedsIndexing,
    @OrganizationActive,
    @OrganizationSAExpirationDate,
    @PortalLimitOrgTickets)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateContactsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateContactsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateContactsViewItem

(
  @Email varchar(1024),
  @FirstName varchar(100),
  @UserID int,
  @Name varchar(201),
  @MiddleName varchar(100),
  @LastName varchar(100),
  @Title varchar(100),
  @IsActive bit,
  @MarkDeleted bit,
  @LastLogin datetime,
  @LastActivity datetime,
  @LastPing datetime,
  @IsSystemAdmin bit,
  @IsFinanceAdmin bit,
  @IsPasswordExpired bit,
  @IsPortalUser bit,
  @PrimaryGroupID int,
  @InOffice bit,
  @InOfficeComment varchar(200),
  @ActivatedOn datetime,
  @DeactivatedOn datetime,
  @OrganizationID int,
  @Organization varchar(255),
  @LastVersion varchar(50),
  @DateModified datetime,
  @ModifierID int,
  @OrganizationParentID int,
  @CryptedPassword varchar(255),
  @SalesForceID varchar(8000),
  @NeedsIndexing bit,
  @OrganizationActive bit,
  @OrganizationSAExpirationDate datetime,
  @PortalLimitOrgTickets bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ContactsView]
  SET
    [Email] = @Email,
    [FirstName] = @FirstName,
    [Name] = @Name,
    [MiddleName] = @MiddleName,
    [LastName] = @LastName,
    [Title] = @Title,
    [IsActive] = @IsActive,
    [MarkDeleted] = @MarkDeleted,
    [LastLogin] = @LastLogin,
    [LastActivity] = @LastActivity,
    [LastPing] = @LastPing,
    [IsSystemAdmin] = @IsSystemAdmin,
    [IsFinanceAdmin] = @IsFinanceAdmin,
    [IsPasswordExpired] = @IsPasswordExpired,
    [IsPortalUser] = @IsPortalUser,
    [PrimaryGroupID] = @PrimaryGroupID,
    [InOffice] = @InOffice,
    [InOfficeComment] = @InOfficeComment,
    [ActivatedOn] = @ActivatedOn,
    [DeactivatedOn] = @DeactivatedOn,
    [OrganizationID] = @OrganizationID,
    [Organization] = @Organization,
    [LastVersion] = @LastVersion,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [OrganizationParentID] = @OrganizationParentID,
    [CryptedPassword] = @CryptedPassword,
    [SalesForceID] = @SalesForceID,
    [NeedsIndexing] = @NeedsIndexing,
    [OrganizationActive] = @OrganizationActive,
    [OrganizationSAExpirationDate] = @OrganizationSAExpirationDate,
    [PortalLimitOrgTickets] = @PortalLimitOrgTickets
  WHERE ([UserID] = @UserID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteContactsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteContactsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteContactsViewItem

(
  @UserID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ContactsView]
  WHERE ([UserID] = @UserID)
GO


