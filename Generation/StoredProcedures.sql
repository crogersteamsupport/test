IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCRMLinkTableItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCRMLinkTableItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCRMLinkTableItem

(
  @CRMLinkID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [CRMLinkID],
    [OrganizationID],
    [Active],
    [CRMType],
    [Username],
    [Password],
    [SecurityToken],
    [TypeFieldMatch],
    [LastLink],
    [SendBackTicketData],
    [LastProcessed],
    [LastTicketID],
    [AllowPortalAccess],
    [SendWelcomeEmail],
    [DefaultSlaLevelID],
    [PushTicketsAsCases],
    [PullCasesAsTickets],
    [PullCustomerProducts],
    [ActionTypeIDToPush],
    [HostName],
    [DefaultProject],
    [UpdateStatus],
    [MatchAccountsByName],
    [UseSandBoxServer],
    [AlwaysUseDefaultProjectKey]
  FROM [dbo].[CRMLinkTable]
  WHERE ([CRMLinkID] = @CRMLinkID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCRMLinkTableItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCRMLinkTableItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCRMLinkTableItem

(
  @OrganizationID int,
  @Active bit,
  @CRMType varchar(100),
  @Username varchar(100),
  @Password varchar(100),
  @SecurityToken varchar(1000),
  @TypeFieldMatch varchar(500),
  @LastLink datetime,
  @SendBackTicketData bit,
  @LastProcessed datetime,
  @LastTicketID int,
  @AllowPortalAccess bit,
  @SendWelcomeEmail bit,
  @DefaultSlaLevelID int,
  @PushTicketsAsCases bit,
  @PullCasesAsTickets bit,
  @PullCustomerProducts bit,
  @ActionTypeIDToPush int,
  @HostName varchar(8000),
  @DefaultProject varchar(8000),
  @UpdateStatus bit,
  @MatchAccountsByName bit,
  @UseSandBoxServer bit,
  @AlwaysUseDefaultProjectKey bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CRMLinkTable]
  (
    [OrganizationID],
    [Active],
    [CRMType],
    [Username],
    [Password],
    [SecurityToken],
    [TypeFieldMatch],
    [LastLink],
    [SendBackTicketData],
    [LastProcessed],
    [LastTicketID],
    [AllowPortalAccess],
    [SendWelcomeEmail],
    [DefaultSlaLevelID],
    [PushTicketsAsCases],
    [PullCasesAsTickets],
    [PullCustomerProducts],
    [ActionTypeIDToPush],
    [HostName],
    [DefaultProject],
    [UpdateStatus],
    [MatchAccountsByName],
    [UseSandBoxServer],
    [AlwaysUseDefaultProjectKey])
  VALUES (
    @OrganizationID,
    @Active,
    @CRMType,
    @Username,
    @Password,
    @SecurityToken,
    @TypeFieldMatch,
    @LastLink,
    @SendBackTicketData,
    @LastProcessed,
    @LastTicketID,
    @AllowPortalAccess,
    @SendWelcomeEmail,
    @DefaultSlaLevelID,
    @PushTicketsAsCases,
    @PullCasesAsTickets,
    @PullCustomerProducts,
    @ActionTypeIDToPush,
    @HostName,
    @DefaultProject,
    @UpdateStatus,
    @MatchAccountsByName,
    @UseSandBoxServer,
    @AlwaysUseDefaultProjectKey)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCRMLinkTableItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCRMLinkTableItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCRMLinkTableItem

(
  @CRMLinkID int,
  @OrganizationID int,
  @Active bit,
  @CRMType varchar(100),
  @Username varchar(100),
  @Password varchar(100),
  @SecurityToken varchar(1000),
  @TypeFieldMatch varchar(500),
  @LastLink datetime,
  @SendBackTicketData bit,
  @LastProcessed datetime,
  @LastTicketID int,
  @AllowPortalAccess bit,
  @SendWelcomeEmail bit,
  @DefaultSlaLevelID int,
  @PushTicketsAsCases bit,
  @PullCasesAsTickets bit,
  @PullCustomerProducts bit,
  @ActionTypeIDToPush int,
  @HostName varchar(8000),
  @DefaultProject varchar(8000),
  @UpdateStatus bit,
  @MatchAccountsByName bit,
  @UseSandBoxServer bit,
  @AlwaysUseDefaultProjectKey bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CRMLinkTable]
  SET
    [OrganizationID] = @OrganizationID,
    [Active] = @Active,
    [CRMType] = @CRMType,
    [Username] = @Username,
    [Password] = @Password,
    [SecurityToken] = @SecurityToken,
    [TypeFieldMatch] = @TypeFieldMatch,
    [LastLink] = @LastLink,
    [SendBackTicketData] = @SendBackTicketData,
    [LastProcessed] = @LastProcessed,
    [LastTicketID] = @LastTicketID,
    [AllowPortalAccess] = @AllowPortalAccess,
    [SendWelcomeEmail] = @SendWelcomeEmail,
    [DefaultSlaLevelID] = @DefaultSlaLevelID,
    [PushTicketsAsCases] = @PushTicketsAsCases,
    [PullCasesAsTickets] = @PullCasesAsTickets,
    [PullCustomerProducts] = @PullCustomerProducts,
    [ActionTypeIDToPush] = @ActionTypeIDToPush,
    [HostName] = @HostName,
    [DefaultProject] = @DefaultProject,
    [UpdateStatus] = @UpdateStatus,
    [MatchAccountsByName] = @MatchAccountsByName,
    [UseSandBoxServer] = @UseSandBoxServer,
    [AlwaysUseDefaultProjectKey] = @AlwaysUseDefaultProjectKey
  WHERE ([CRMLinkID] = @CRMLinkID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCRMLinkTableItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCRMLinkTableItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCRMLinkTableItem

(
  @CRMLinkID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CRMLinkTable]
  WHERE ([CRMLinkID] = @CRMLinkID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCRMLinkTableItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCRMLinkTableItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCRMLinkTableItem

(
  @CRMLinkID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [CRMLinkID],
    [OrganizationID],
    [Active],
    [CRMType],
    [Username],
    [Password],
    [SecurityToken],
    [TypeFieldMatch],
    [LastLink],
    [SendBackTicketData],
    [LastProcessed],
    [LastTicketID],
    [AllowPortalAccess],
    [SendWelcomeEmail],
    [DefaultSlaLevelID],
    [PushTicketsAsCases],
    [PullCasesAsTickets],
    [PullCustomerProducts],
    [ActionTypeIDToPush],
    [HostName],
    [DefaultProject],
    [UpdateStatus],
    [MatchAccountsByName],
    [UseSandBoxServer],
    [AlwaysUseDefaultProjectKey]
  FROM [dbo].[CRMLinkTable]
  WHERE ([CRMLinkID] = @CRMLinkID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCRMLinkTableItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCRMLinkTableItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCRMLinkTableItem

(
  @OrganizationID int,
  @Active bit,
  @CRMType varchar(100),
  @Username varchar(100),
  @Password varchar(100),
  @SecurityToken varchar(1000),
  @TypeFieldMatch varchar(500),
  @LastLink datetime,
  @SendBackTicketData bit,
  @LastProcessed datetime,
  @LastTicketID int,
  @AllowPortalAccess bit,
  @SendWelcomeEmail bit,
  @DefaultSlaLevelID int,
  @PushTicketsAsCases bit,
  @PullCasesAsTickets bit,
  @PullCustomerProducts bit,
  @ActionTypeIDToPush int,
  @HostName varchar(8000),
  @DefaultProject varchar(8000),
  @UpdateStatus bit,
  @MatchAccountsByName bit,
  @UseSandBoxServer bit,
  @AlwaysUseDefaultProjectKey bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CRMLinkTable]
  (
    [OrganizationID],
    [Active],
    [CRMType],
    [Username],
    [Password],
    [SecurityToken],
    [TypeFieldMatch],
    [LastLink],
    [SendBackTicketData],
    [LastProcessed],
    [LastTicketID],
    [AllowPortalAccess],
    [SendWelcomeEmail],
    [DefaultSlaLevelID],
    [PushTicketsAsCases],
    [PullCasesAsTickets],
    [PullCustomerProducts],
    [ActionTypeIDToPush],
    [HostName],
    [DefaultProject],
    [UpdateStatus],
    [MatchAccountsByName],
    [UseSandBoxServer],
    [AlwaysUseDefaultProjectKey])
  VALUES (
    @OrganizationID,
    @Active,
    @CRMType,
    @Username,
    @Password,
    @SecurityToken,
    @TypeFieldMatch,
    @LastLink,
    @SendBackTicketData,
    @LastProcessed,
    @LastTicketID,
    @AllowPortalAccess,
    @SendWelcomeEmail,
    @DefaultSlaLevelID,
    @PushTicketsAsCases,
    @PullCasesAsTickets,
    @PullCustomerProducts,
    @ActionTypeIDToPush,
    @HostName,
    @DefaultProject,
    @UpdateStatus,
    @MatchAccountsByName,
    @UseSandBoxServer,
    @AlwaysUseDefaultProjectKey)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCRMLinkTableItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCRMLinkTableItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCRMLinkTableItem

(
  @CRMLinkID int,
  @OrganizationID int,
  @Active bit,
  @CRMType varchar(100),
  @Username varchar(100),
  @Password varchar(100),
  @SecurityToken varchar(1000),
  @TypeFieldMatch varchar(500),
  @LastLink datetime,
  @SendBackTicketData bit,
  @LastProcessed datetime,
  @LastTicketID int,
  @AllowPortalAccess bit,
  @SendWelcomeEmail bit,
  @DefaultSlaLevelID int,
  @PushTicketsAsCases bit,
  @PullCasesAsTickets bit,
  @PullCustomerProducts bit,
  @ActionTypeIDToPush int,
  @HostName varchar(8000),
  @DefaultProject varchar(8000),
  @UpdateStatus bit,
  @MatchAccountsByName bit,
  @UseSandBoxServer bit,
  @AlwaysUseDefaultProjectKey bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CRMLinkTable]
  SET
    [OrganizationID] = @OrganizationID,
    [Active] = @Active,
    [CRMType] = @CRMType,
    [Username] = @Username,
    [Password] = @Password,
    [SecurityToken] = @SecurityToken,
    [TypeFieldMatch] = @TypeFieldMatch,
    [LastLink] = @LastLink,
    [SendBackTicketData] = @SendBackTicketData,
    [LastProcessed] = @LastProcessed,
    [LastTicketID] = @LastTicketID,
    [AllowPortalAccess] = @AllowPortalAccess,
    [SendWelcomeEmail] = @SendWelcomeEmail,
    [DefaultSlaLevelID] = @DefaultSlaLevelID,
    [PushTicketsAsCases] = @PushTicketsAsCases,
    [PullCasesAsTickets] = @PullCasesAsTickets,
    [PullCustomerProducts] = @PullCustomerProducts,
    [ActionTypeIDToPush] = @ActionTypeIDToPush,
    [HostName] = @HostName,
    [DefaultProject] = @DefaultProject,
    [UpdateStatus] = @UpdateStatus,
    [MatchAccountsByName] = @MatchAccountsByName,
    [UseSandBoxServer] = @UseSandBoxServer,
    [AlwaysUseDefaultProjectKey] = @AlwaysUseDefaultProjectKey
  WHERE ([CRMLinkID] = @CRMLinkID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCRMLinkTableItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCRMLinkTableItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCRMLinkTableItem

(
  @CRMLinkID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CRMLinkTable]
  WHERE ([CRMLinkID] = @CRMLinkID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCRMLinkTableItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCRMLinkTableItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCRMLinkTableItem

(
  @CRMLinkID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [CRMLinkID],
    [OrganizationID],
    [Active],
    [CRMType],
    [Username],
    [Password],
    [SecurityToken],
    [TypeFieldMatch],
    [LastLink],
    [SendBackTicketData],
    [LastProcessed],
    [LastTicketID],
    [AllowPortalAccess],
    [SendWelcomeEmail],
    [DefaultSlaLevelID],
    [PushTicketsAsCases],
    [PullCasesAsTickets],
    [PullCustomerProducts],
    [ActionTypeIDToPush],
    [HostName],
    [DefaultProject],
    [UpdateStatus],
    [MatchAccountsByName],
    [UseSandBoxServer],
    [AlwaysUseDefaultProjectKey]
  FROM [dbo].[CRMLinkTable]
  WHERE ([CRMLinkID] = @CRMLinkID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCRMLinkTableItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCRMLinkTableItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCRMLinkTableItem

(
  @OrganizationID int,
  @Active bit,
  @CRMType varchar(100),
  @Username varchar(100),
  @Password varchar(100),
  @SecurityToken varchar(1000),
  @TypeFieldMatch varchar(500),
  @LastLink datetime,
  @SendBackTicketData bit,
  @LastProcessed datetime,
  @LastTicketID int,
  @AllowPortalAccess bit,
  @SendWelcomeEmail bit,
  @DefaultSlaLevelID int,
  @PushTicketsAsCases bit,
  @PullCasesAsTickets bit,
  @PullCustomerProducts bit,
  @ActionTypeIDToPush int,
  @HostName varchar(8000),
  @DefaultProject varchar(8000),
  @UpdateStatus bit,
  @MatchAccountsByName bit,
  @UseSandBoxServer bit,
  @AlwaysUseDefaultProjectKey bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CRMLinkTable]
  (
    [OrganizationID],
    [Active],
    [CRMType],
    [Username],
    [Password],
    [SecurityToken],
    [TypeFieldMatch],
    [LastLink],
    [SendBackTicketData],
    [LastProcessed],
    [LastTicketID],
    [AllowPortalAccess],
    [SendWelcomeEmail],
    [DefaultSlaLevelID],
    [PushTicketsAsCases],
    [PullCasesAsTickets],
    [PullCustomerProducts],
    [ActionTypeIDToPush],
    [HostName],
    [DefaultProject],
    [UpdateStatus],
    [MatchAccountsByName],
    [UseSandBoxServer],
    [AlwaysUseDefaultProjectKey])
  VALUES (
    @OrganizationID,
    @Active,
    @CRMType,
    @Username,
    @Password,
    @SecurityToken,
    @TypeFieldMatch,
    @LastLink,
    @SendBackTicketData,
    @LastProcessed,
    @LastTicketID,
    @AllowPortalAccess,
    @SendWelcomeEmail,
    @DefaultSlaLevelID,
    @PushTicketsAsCases,
    @PullCasesAsTickets,
    @PullCustomerProducts,
    @ActionTypeIDToPush,
    @HostName,
    @DefaultProject,
    @UpdateStatus,
    @MatchAccountsByName,
    @UseSandBoxServer,
    @AlwaysUseDefaultProjectKey)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCRMLinkTableItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCRMLinkTableItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCRMLinkTableItem

(
  @CRMLinkID int,
  @OrganizationID int,
  @Active bit,
  @CRMType varchar(100),
  @Username varchar(100),
  @Password varchar(100),
  @SecurityToken varchar(1000),
  @TypeFieldMatch varchar(500),
  @LastLink datetime,
  @SendBackTicketData bit,
  @LastProcessed datetime,
  @LastTicketID int,
  @AllowPortalAccess bit,
  @SendWelcomeEmail bit,
  @DefaultSlaLevelID int,
  @PushTicketsAsCases bit,
  @PullCasesAsTickets bit,
  @PullCustomerProducts bit,
  @ActionTypeIDToPush int,
  @HostName varchar(8000),
  @DefaultProject varchar(8000),
  @UpdateStatus bit,
  @MatchAccountsByName bit,
  @UseSandBoxServer bit,
  @AlwaysUseDefaultProjectKey bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CRMLinkTable]
  SET
    [OrganizationID] = @OrganizationID,
    [Active] = @Active,
    [CRMType] = @CRMType,
    [Username] = @Username,
    [Password] = @Password,
    [SecurityToken] = @SecurityToken,
    [TypeFieldMatch] = @TypeFieldMatch,
    [LastLink] = @LastLink,
    [SendBackTicketData] = @SendBackTicketData,
    [LastProcessed] = @LastProcessed,
    [LastTicketID] = @LastTicketID,
    [AllowPortalAccess] = @AllowPortalAccess,
    [SendWelcomeEmail] = @SendWelcomeEmail,
    [DefaultSlaLevelID] = @DefaultSlaLevelID,
    [PushTicketsAsCases] = @PushTicketsAsCases,
    [PullCasesAsTickets] = @PullCasesAsTickets,
    [PullCustomerProducts] = @PullCustomerProducts,
    [ActionTypeIDToPush] = @ActionTypeIDToPush,
    [HostName] = @HostName,
    [DefaultProject] = @DefaultProject,
    [UpdateStatus] = @UpdateStatus,
    [MatchAccountsByName] = @MatchAccountsByName,
    [UseSandBoxServer] = @UseSandBoxServer,
    [AlwaysUseDefaultProjectKey] = @AlwaysUseDefaultProjectKey
  WHERE ([CRMLinkID] = @CRMLinkID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCRMLinkTableItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCRMLinkTableItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCRMLinkTableItem

(
  @CRMLinkID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CRMLinkTable]
  WHERE ([CRMLinkID] = @CRMLinkID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCRMLinkTableItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCRMLinkTableItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCRMLinkTableItem

(
  @CRMLinkID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [CRMLinkID],
    [OrganizationID],
    [Active],
    [CRMType],
    [Username],
    [Password],
    [SecurityToken],
    [TypeFieldMatch],
    [LastLink],
    [SendBackTicketData],
    [LastProcessed],
    [LastTicketID],
    [AllowPortalAccess],
    [SendWelcomeEmail],
    [DefaultSlaLevelID],
    [PushTicketsAsCases],
    [PullCasesAsTickets],
    [PullCustomerProducts],
    [ActionTypeIDToPush],
    [HostName],
    [DefaultProject],
    [UpdateStatus],
    [MatchAccountsByName],
    [UseSandBoxServer],
    [AlwaysUseDefaultProjectKey]
  FROM [dbo].[CRMLinkTable]
  WHERE ([CRMLinkID] = @CRMLinkID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCRMLinkTableItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCRMLinkTableItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCRMLinkTableItem

(
  @OrganizationID int,
  @Active bit,
  @CRMType varchar(100),
  @Username varchar(100),
  @Password varchar(100),
  @SecurityToken varchar(1000),
  @TypeFieldMatch varchar(500),
  @LastLink datetime,
  @SendBackTicketData bit,
  @LastProcessed datetime,
  @LastTicketID int,
  @AllowPortalAccess bit,
  @SendWelcomeEmail bit,
  @DefaultSlaLevelID int,
  @PushTicketsAsCases bit,
  @PullCasesAsTickets bit,
  @PullCustomerProducts bit,
  @ActionTypeIDToPush int,
  @HostName varchar(8000),
  @DefaultProject varchar(8000),
  @UpdateStatus bit,
  @MatchAccountsByName bit,
  @UseSandBoxServer bit,
  @AlwaysUseDefaultProjectKey bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CRMLinkTable]
  (
    [OrganizationID],
    [Active],
    [CRMType],
    [Username],
    [Password],
    [SecurityToken],
    [TypeFieldMatch],
    [LastLink],
    [SendBackTicketData],
    [LastProcessed],
    [LastTicketID],
    [AllowPortalAccess],
    [SendWelcomeEmail],
    [DefaultSlaLevelID],
    [PushTicketsAsCases],
    [PullCasesAsTickets],
    [PullCustomerProducts],
    [ActionTypeIDToPush],
    [HostName],
    [DefaultProject],
    [UpdateStatus],
    [MatchAccountsByName],
    [UseSandBoxServer],
    [AlwaysUseDefaultProjectKey])
  VALUES (
    @OrganizationID,
    @Active,
    @CRMType,
    @Username,
    @Password,
    @SecurityToken,
    @TypeFieldMatch,
    @LastLink,
    @SendBackTicketData,
    @LastProcessed,
    @LastTicketID,
    @AllowPortalAccess,
    @SendWelcomeEmail,
    @DefaultSlaLevelID,
    @PushTicketsAsCases,
    @PullCasesAsTickets,
    @PullCustomerProducts,
    @ActionTypeIDToPush,
    @HostName,
    @DefaultProject,
    @UpdateStatus,
    @MatchAccountsByName,
    @UseSandBoxServer,
    @AlwaysUseDefaultProjectKey)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCRMLinkTableItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCRMLinkTableItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCRMLinkTableItem

(
  @CRMLinkID int,
  @OrganizationID int,
  @Active bit,
  @CRMType varchar(100),
  @Username varchar(100),
  @Password varchar(100),
  @SecurityToken varchar(1000),
  @TypeFieldMatch varchar(500),
  @LastLink datetime,
  @SendBackTicketData bit,
  @LastProcessed datetime,
  @LastTicketID int,
  @AllowPortalAccess bit,
  @SendWelcomeEmail bit,
  @DefaultSlaLevelID int,
  @PushTicketsAsCases bit,
  @PullCasesAsTickets bit,
  @PullCustomerProducts bit,
  @ActionTypeIDToPush int,
  @HostName varchar(8000),
  @DefaultProject varchar(8000),
  @UpdateStatus bit,
  @MatchAccountsByName bit,
  @UseSandBoxServer bit,
  @AlwaysUseDefaultProjectKey bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CRMLinkTable]
  SET
    [OrganizationID] = @OrganizationID,
    [Active] = @Active,
    [CRMType] = @CRMType,
    [Username] = @Username,
    [Password] = @Password,
    [SecurityToken] = @SecurityToken,
    [TypeFieldMatch] = @TypeFieldMatch,
    [LastLink] = @LastLink,
    [SendBackTicketData] = @SendBackTicketData,
    [LastProcessed] = @LastProcessed,
    [LastTicketID] = @LastTicketID,
    [AllowPortalAccess] = @AllowPortalAccess,
    [SendWelcomeEmail] = @SendWelcomeEmail,
    [DefaultSlaLevelID] = @DefaultSlaLevelID,
    [PushTicketsAsCases] = @PushTicketsAsCases,
    [PullCasesAsTickets] = @PullCasesAsTickets,
    [PullCustomerProducts] = @PullCustomerProducts,
    [ActionTypeIDToPush] = @ActionTypeIDToPush,
    [HostName] = @HostName,
    [DefaultProject] = @DefaultProject,
    [UpdateStatus] = @UpdateStatus,
    [MatchAccountsByName] = @MatchAccountsByName,
    [UseSandBoxServer] = @UseSandBoxServer,
    [AlwaysUseDefaultProjectKey] = @AlwaysUseDefaultProjectKey
  WHERE ([CRMLinkID] = @CRMLinkID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCRMLinkTableItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCRMLinkTableItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCRMLinkTableItem

(
  @CRMLinkID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CRMLinkTable]
  WHERE ([CRMLinkID] = @CRMLinkID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCRMLinkTableItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCRMLinkTableItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCRMLinkTableItem

(
  @CRMLinkID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [CRMLinkID],
    [OrganizationID],
    [Active],
    [CRMType],
    [Username],
    [Password],
    [SecurityToken],
    [TypeFieldMatch],
    [LastLink],
    [SendBackTicketData],
    [LastProcessed],
    [LastTicketID],
    [AllowPortalAccess],
    [SendWelcomeEmail],
    [DefaultSlaLevelID],
    [PushTicketsAsCases],
    [PullCasesAsTickets],
    [PullCustomerProducts],
    [ActionTypeIDToPush],
    [HostName],
    [DefaultProject],
    [UpdateStatus],
    [MatchAccountsByName],
    [UseSandBoxServer],
    [AlwaysUseDefaultProjectKey]
  FROM [dbo].[CRMLinkTable]
  WHERE ([CRMLinkID] = @CRMLinkID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCRMLinkTableItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCRMLinkTableItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCRMLinkTableItem

(
  @OrganizationID int,
  @Active bit,
  @CRMType varchar(100),
  @Username varchar(100),
  @Password varchar(100),
  @SecurityToken varchar(1000),
  @TypeFieldMatch varchar(500),
  @LastLink datetime,
  @SendBackTicketData bit,
  @LastProcessed datetime,
  @LastTicketID int,
  @AllowPortalAccess bit,
  @SendWelcomeEmail bit,
  @DefaultSlaLevelID int,
  @PushTicketsAsCases bit,
  @PullCasesAsTickets bit,
  @PullCustomerProducts bit,
  @ActionTypeIDToPush int,
  @HostName varchar(8000),
  @DefaultProject varchar(8000),
  @UpdateStatus bit,
  @MatchAccountsByName bit,
  @UseSandBoxServer bit,
  @AlwaysUseDefaultProjectKey bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CRMLinkTable]
  (
    [OrganizationID],
    [Active],
    [CRMType],
    [Username],
    [Password],
    [SecurityToken],
    [TypeFieldMatch],
    [LastLink],
    [SendBackTicketData],
    [LastProcessed],
    [LastTicketID],
    [AllowPortalAccess],
    [SendWelcomeEmail],
    [DefaultSlaLevelID],
    [PushTicketsAsCases],
    [PullCasesAsTickets],
    [PullCustomerProducts],
    [ActionTypeIDToPush],
    [HostName],
    [DefaultProject],
    [UpdateStatus],
    [MatchAccountsByName],
    [UseSandBoxServer],
    [AlwaysUseDefaultProjectKey])
  VALUES (
    @OrganizationID,
    @Active,
    @CRMType,
    @Username,
    @Password,
    @SecurityToken,
    @TypeFieldMatch,
    @LastLink,
    @SendBackTicketData,
    @LastProcessed,
    @LastTicketID,
    @AllowPortalAccess,
    @SendWelcomeEmail,
    @DefaultSlaLevelID,
    @PushTicketsAsCases,
    @PullCasesAsTickets,
    @PullCustomerProducts,
    @ActionTypeIDToPush,
    @HostName,
    @DefaultProject,
    @UpdateStatus,
    @MatchAccountsByName,
    @UseSandBoxServer,
    @AlwaysUseDefaultProjectKey)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCRMLinkTableItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCRMLinkTableItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCRMLinkTableItem

(
  @CRMLinkID int,
  @OrganizationID int,
  @Active bit,
  @CRMType varchar(100),
  @Username varchar(100),
  @Password varchar(100),
  @SecurityToken varchar(1000),
  @TypeFieldMatch varchar(500),
  @LastLink datetime,
  @SendBackTicketData bit,
  @LastProcessed datetime,
  @LastTicketID int,
  @AllowPortalAccess bit,
  @SendWelcomeEmail bit,
  @DefaultSlaLevelID int,
  @PushTicketsAsCases bit,
  @PullCasesAsTickets bit,
  @PullCustomerProducts bit,
  @ActionTypeIDToPush int,
  @HostName varchar(8000),
  @DefaultProject varchar(8000),
  @UpdateStatus bit,
  @MatchAccountsByName bit,
  @UseSandBoxServer bit,
  @AlwaysUseDefaultProjectKey bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CRMLinkTable]
  SET
    [OrganizationID] = @OrganizationID,
    [Active] = @Active,
    [CRMType] = @CRMType,
    [Username] = @Username,
    [Password] = @Password,
    [SecurityToken] = @SecurityToken,
    [TypeFieldMatch] = @TypeFieldMatch,
    [LastLink] = @LastLink,
    [SendBackTicketData] = @SendBackTicketData,
    [LastProcessed] = @LastProcessed,
    [LastTicketID] = @LastTicketID,
    [AllowPortalAccess] = @AllowPortalAccess,
    [SendWelcomeEmail] = @SendWelcomeEmail,
    [DefaultSlaLevelID] = @DefaultSlaLevelID,
    [PushTicketsAsCases] = @PushTicketsAsCases,
    [PullCasesAsTickets] = @PullCasesAsTickets,
    [PullCustomerProducts] = @PullCustomerProducts,
    [ActionTypeIDToPush] = @ActionTypeIDToPush,
    [HostName] = @HostName,
    [DefaultProject] = @DefaultProject,
    [UpdateStatus] = @UpdateStatus,
    [MatchAccountsByName] = @MatchAccountsByName,
    [UseSandBoxServer] = @UseSandBoxServer,
    [AlwaysUseDefaultProjectKey] = @AlwaysUseDefaultProjectKey
  WHERE ([CRMLinkID] = @CRMLinkID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCRMLinkTableItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCRMLinkTableItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCRMLinkTableItem

(
  @CRMLinkID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CRMLinkTable]
  WHERE ([CRMLinkID] = @CRMLinkID)
GO


