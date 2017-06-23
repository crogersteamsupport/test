IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectProduct
GO

CREATE PROCEDURE dbo.uspGeneratedSelectProduct

(
  @ProductID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ProductID],
    [OrganizationID],
    [Name],
    [Description],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [ProductFamilyID],
    [JiraProjectKey],
    [ImportFileID],
    [EmailReplyToAddress],
    [SlaLevelID],
    [TFSProjectName]
  FROM [dbo].[Products]
  WHERE ([ProductID] = @ProductID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertProduct
GO

CREATE PROCEDURE dbo.uspGeneratedInsertProduct

(
  @OrganizationID int,
  @Name varchar(255),
  @Description varchar(1024),
  @ImportID varchar(500),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @NeedsIndexing bit,
  @ProductFamilyID int,
  @JiraProjectKey varchar(250),
  @ImportFileID int,
  @EmailReplyToAddress varchar(500),
  @SlaLevelID int,
  @TFSProjectName nvarchar(256),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Products]
  (
    [OrganizationID],
    [Name],
    [Description],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [ProductFamilyID],
    [JiraProjectKey],
    [ImportFileID],
    [EmailReplyToAddress],
    [SlaLevelID],
    [TFSProjectName])
  VALUES (
    @OrganizationID,
    @Name,
    @Description,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @NeedsIndexing,
    @ProductFamilyID,
    @JiraProjectKey,
    @ImportFileID,
    @EmailReplyToAddress,
    @SlaLevelID,
    @TFSProjectName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateProduct
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateProduct

(
  @ProductID int,
  @OrganizationID int,
  @Name varchar(255),
  @Description varchar(1024),
  @ImportID varchar(500),
  @DateModified datetime,
  @ModifierID int,
  @NeedsIndexing bit,
  @ProductFamilyID int,
  @JiraProjectKey varchar(250),
  @ImportFileID int,
  @EmailReplyToAddress varchar(500),
  @SlaLevelID int,
  @TFSProjectName nvarchar(256)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Products]
  SET
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [Description] = @Description,
    [ImportID] = @ImportID,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [NeedsIndexing] = @NeedsIndexing,
    [ProductFamilyID] = @ProductFamilyID,
    [JiraProjectKey] = @JiraProjectKey,
    [ImportFileID] = @ImportFileID,
    [EmailReplyToAddress] = @EmailReplyToAddress,
    [SlaLevelID] = @SlaLevelID,
    [TFSProjectName] = @TFSProjectName
  WHERE ([ProductID] = @ProductID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteProduct
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteProduct

(
  @ProductID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Products]
  WHERE ([ProductID] = @ProductID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectActionLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [id],
    [ActionID],
    [DateModifiedByTFSSync],
    [TFSID]
  FROM [dbo].[ActionLinkToTFS]
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertActionLinkToTFSItem

(
  @ActionID int,
  @DateModifiedByTFSSync datetime,
  @TFSID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ActionLinkToTFS]
  (
    [ActionID],
    [DateModifiedByTFSSync],
    [TFSID])
  VALUES (
    @ActionID,
    @DateModifiedByTFSSync,
    @TFSID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateActionLinkToTFSItem

(
  @id int,
  @ActionID int,
  @DateModifiedByTFSSync datetime,
  @TFSID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ActionLinkToTFS]
  SET
    [ActionID] = @ActionID,
    [DateModifiedByTFSSync] = @DateModifiedByTFSSync,
    [TFSID] = @TFSID
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteActionLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ActionLinkToTFS]
  WHERE ([id] = @id)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTicketLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [id],
    [TicketID],
    [DateModifiedByTFSSync],
    [SyncWithTFS],
    [TFSID],
    [TFSTitle],
    [TFSURL],
    [TFSState],
    [CreatorID],
    [CrmLinkID]
  FROM [dbo].[TicketLinkToTFS]
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTicketLinkToTFSItem

(
  @TicketID int,
  @DateModifiedByTFSSync datetime,
  @SyncWithTFS bit,
  @TFSID int,
  @TFSTitle nvarchar(256),
  @TFSURL nvarchar(256),
  @TFSState nvarchar(256),
  @CreatorID int,
  @CrmLinkID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TicketLinkToTFS]
  (
    [TicketID],
    [DateModifiedByTFSSync],
    [SyncWithTFS],
    [TFSID],
    [TFSTitle],
    [TFSURL],
    [TFSState],
    [CreatorID],
    [CrmLinkID])
  VALUES (
    @TicketID,
    @DateModifiedByTFSSync,
    @SyncWithTFS,
    @TFSID,
    @TFSTitle,
    @TFSURL,
    @TFSState,
    @CreatorID,
    @CrmLinkID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTicketLinkToTFSItem

(
  @id int,
  @TicketID int,
  @DateModifiedByTFSSync datetime,
  @SyncWithTFS bit,
  @TFSID int,
  @TFSTitle nvarchar(256),
  @TFSURL nvarchar(256),
  @TFSState nvarchar(256),
  @CrmLinkID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TicketLinkToTFS]
  SET
    [TicketID] = @TicketID,
    [DateModifiedByTFSSync] = @DateModifiedByTFSSync,
    [SyncWithTFS] = @SyncWithTFS,
    [TFSID] = @TFSID,
    [TFSTitle] = @TFSTitle,
    [TFSURL] = @TFSURL,
    [TFSState] = @TFSState,
    [CrmLinkID] = @CrmLinkID
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTicketLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TicketLinkToTFS]
  WHERE ([id] = @id)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectProductVersion' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectProductVersion
GO

CREATE PROCEDURE dbo.uspGeneratedSelectProductVersion

(
  @ProductVersionID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ProductVersionID],
    [ProductID],
    [ProductVersionStatusID],
    [VersionNumber],
    [ReleaseDate],
    [IsReleased],
    [Description],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [JiraProjectKey],
    [ImportFileID],
    [TFSProjectName]
  FROM [dbo].[ProductVersions]
  WHERE ([ProductVersionID] = @ProductVersionID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertProductVersion' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertProductVersion
GO

CREATE PROCEDURE dbo.uspGeneratedInsertProductVersion

(
  @ProductID int,
  @ProductVersionStatusID int,
  @VersionNumber varchar(50),
  @ReleaseDate datetime,
  @IsReleased bit,
  @Description varchar(MAX),
  @ImportID varchar(50),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @NeedsIndexing bit,
  @JiraProjectKey varchar(250),
  @ImportFileID int,
  @TFSProjectName nvarchar(256),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ProductVersions]
  (
    [ProductID],
    [ProductVersionStatusID],
    [VersionNumber],
    [ReleaseDate],
    [IsReleased],
    [Description],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [JiraProjectKey],
    [ImportFileID],
    [TFSProjectName])
  VALUES (
    @ProductID,
    @ProductVersionStatusID,
    @VersionNumber,
    @ReleaseDate,
    @IsReleased,
    @Description,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @NeedsIndexing,
    @JiraProjectKey,
    @ImportFileID,
    @TFSProjectName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateProductVersion' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateProductVersion
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateProductVersion

(
  @ProductVersionID int,
  @ProductID int,
  @ProductVersionStatusID int,
  @VersionNumber varchar(50),
  @ReleaseDate datetime,
  @IsReleased bit,
  @Description varchar(MAX),
  @ImportID varchar(50),
  @DateModified datetime,
  @ModifierID int,
  @NeedsIndexing bit,
  @JiraProjectKey varchar(250),
  @ImportFileID int,
  @TFSProjectName nvarchar(256)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ProductVersions]
  SET
    [ProductID] = @ProductID,
    [ProductVersionStatusID] = @ProductVersionStatusID,
    [VersionNumber] = @VersionNumber,
    [ReleaseDate] = @ReleaseDate,
    [IsReleased] = @IsReleased,
    [Description] = @Description,
    [ImportID] = @ImportID,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [NeedsIndexing] = @NeedsIndexing,
    [JiraProjectKey] = @JiraProjectKey,
    [ImportFileID] = @ImportFileID,
    [TFSProjectName] = @TFSProjectName
  WHERE ([ProductVersionID] = @ProductVersionID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteProductVersion' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteProductVersion
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteProductVersion

(
  @ProductVersionID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ProductVersions]
  WHERE ([ProductVersionID] = @ProductVersionID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAttachment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAttachment
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAttachment

(
  @AttachmentID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [AttachmentID],
    [OrganizationID],
    [FileName],
    [FileType],
    [FileSize],
    [Path],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [RefType],
    [RefID],
    [SentToJira],
    [AttachmentGUID],
    [ProductFamilyID],
    [SentToTFS]
  FROM [dbo].[Attachments]
  WHERE ([AttachmentID] = @AttachmentID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAttachment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAttachment
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAttachment

(
  @OrganizationID int,
  @FileName nvarchar(1000),
  @FileType varchar(255),
  @FileSize bigint,
  @Path nvarchar(1000),
  @Description varchar(2000),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @RefType int,
  @RefID int,
  @SentToJira bit,
  @AttachmentGUID uniqueidentifier,
  @ProductFamilyID int,
  @SentToTFS bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Attachments]
  (
    [OrganizationID],
    [FileName],
    [FileType],
    [FileSize],
    [Path],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [RefType],
    [RefID],
    [SentToJira],
    [AttachmentGUID],
    [ProductFamilyID],
    [SentToTFS])
  VALUES (
    @OrganizationID,
    @FileName,
    @FileType,
    @FileSize,
    @Path,
    @Description,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @RefType,
    @RefID,
    @SentToJira,
    @AttachmentGUID,
    @ProductFamilyID,
    @SentToTFS)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAttachment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAttachment
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAttachment

(
  @AttachmentID int,
  @OrganizationID int,
  @FileName nvarchar(1000),
  @FileType varchar(255),
  @FileSize bigint,
  @Path nvarchar(1000),
  @Description varchar(2000),
  @DateModified datetime,
  @ModifierID int,
  @RefType int,
  @RefID int,
  @SentToJira bit,
  @AttachmentGUID uniqueidentifier,
  @ProductFamilyID int,
  @SentToTFS bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Attachments]
  SET
    [OrganizationID] = @OrganizationID,
    [FileName] = @FileName,
    [FileType] = @FileType,
    [FileSize] = @FileSize,
    [Path] = @Path,
    [Description] = @Description,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [RefType] = @RefType,
    [RefID] = @RefID,
    [SentToJira] = @SentToJira,
    [AttachmentGUID] = @AttachmentGUID,
    [ProductFamilyID] = @ProductFamilyID,
    [SentToTFS] = @SentToTFS
  WHERE ([AttachmentID] = @AttachmentID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAttachment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAttachment
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAttachment

(
  @AttachmentID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Attachments]
  WHERE ([AttachmentID] = @AttachmentID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectProductVersionsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectProductVersionsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectProductVersionsViewItem

(
  @ProductVersionID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ProductVersionID],
    [ProductID],
    [ProductVersionStatusID],
    [VersionNumber],
    [ReleaseDate],
    [IsReleased],
    [Description],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [VersionStatus],
    [ProductName],
    [OrganizationID],
    [ProductFamilyID],
    [JiraProjectKey],
    [TFSProjectName]
  FROM [dbo].[ProductVersionsView]
  WHERE ([ProductVersionID] = @ProductVersionID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertProductVersionsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertProductVersionsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertProductVersionsViewItem

(
  @ProductVersionID int,
  @ProductID int,
  @ProductVersionStatusID int,
  @VersionNumber varchar(50),
  @ReleaseDate datetime,
  @IsReleased bit,
  @Description varchar(MAX),
  @ImportID varchar(50),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @NeedsIndexing bit,
  @VersionStatus varchar(255),
  @ProductName varchar(255),
  @OrganizationID int,
  @ProductFamilyID int,
  @JiraProjectKey varchar(250),
  @TFSProjectName nvarchar(256),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ProductVersionsView]
  (
    [ProductVersionID],
    [ProductID],
    [ProductVersionStatusID],
    [VersionNumber],
    [ReleaseDate],
    [IsReleased],
    [Description],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [VersionStatus],
    [ProductName],
    [OrganizationID],
    [ProductFamilyID],
    [JiraProjectKey],
    [TFSProjectName])
  VALUES (
    @ProductVersionID,
    @ProductID,
    @ProductVersionStatusID,
    @VersionNumber,
    @ReleaseDate,
    @IsReleased,
    @Description,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @NeedsIndexing,
    @VersionStatus,
    @ProductName,
    @OrganizationID,
    @ProductFamilyID,
    @JiraProjectKey,
    @TFSProjectName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateProductVersionsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateProductVersionsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateProductVersionsViewItem

(
  @ProductVersionID int,
  @ProductID int,
  @ProductVersionStatusID int,
  @VersionNumber varchar(50),
  @ReleaseDate datetime,
  @IsReleased bit,
  @Description varchar(MAX),
  @ImportID varchar(50),
  @DateModified datetime,
  @ModifierID int,
  @NeedsIndexing bit,
  @VersionStatus varchar(255),
  @ProductName varchar(255),
  @OrganizationID int,
  @ProductFamilyID int,
  @JiraProjectKey varchar(250),
  @TFSProjectName nvarchar(256)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ProductVersionsView]
  SET
    [ProductID] = @ProductID,
    [ProductVersionStatusID] = @ProductVersionStatusID,
    [VersionNumber] = @VersionNumber,
    [ReleaseDate] = @ReleaseDate,
    [IsReleased] = @IsReleased,
    [Description] = @Description,
    [ImportID] = @ImportID,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [NeedsIndexing] = @NeedsIndexing,
    [VersionStatus] = @VersionStatus,
    [ProductName] = @ProductName,
    [OrganizationID] = @OrganizationID,
    [ProductFamilyID] = @ProductFamilyID,
    [JiraProjectKey] = @JiraProjectKey,
    [TFSProjectName] = @TFSProjectName
  WHERE ([ProductVersionID] = @ProductVersionID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteProductVersionsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteProductVersionsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteProductVersionsViewItem

(
  @ProductVersionID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ProductVersionsView]
  WHERE ([ProductVersionID] = @ProductVersionID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectProduct
GO

CREATE PROCEDURE dbo.uspGeneratedSelectProduct

(
  @ProductID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ProductID],
    [OrganizationID],
    [Name],
    [Description],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [ProductFamilyID],
    [JiraProjectKey],
    [ImportFileID],
    [EmailReplyToAddress],
    [SlaLevelID],
    [TFSProjectName]
  FROM [dbo].[Products]
  WHERE ([ProductID] = @ProductID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertProduct
GO

CREATE PROCEDURE dbo.uspGeneratedInsertProduct

(
  @OrganizationID int,
  @Name varchar(255),
  @Description varchar(1024),
  @ImportID varchar(500),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @NeedsIndexing bit,
  @ProductFamilyID int,
  @JiraProjectKey varchar(250),
  @ImportFileID int,
  @EmailReplyToAddress varchar(500),
  @SlaLevelID int,
  @TFSProjectName nvarchar(256),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Products]
  (
    [OrganizationID],
    [Name],
    [Description],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [ProductFamilyID],
    [JiraProjectKey],
    [ImportFileID],
    [EmailReplyToAddress],
    [SlaLevelID],
    [TFSProjectName])
  VALUES (
    @OrganizationID,
    @Name,
    @Description,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @NeedsIndexing,
    @ProductFamilyID,
    @JiraProjectKey,
    @ImportFileID,
    @EmailReplyToAddress,
    @SlaLevelID,
    @TFSProjectName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateProduct
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateProduct

(
  @ProductID int,
  @OrganizationID int,
  @Name varchar(255),
  @Description varchar(1024),
  @ImportID varchar(500),
  @DateModified datetime,
  @ModifierID int,
  @NeedsIndexing bit,
  @ProductFamilyID int,
  @JiraProjectKey varchar(250),
  @ImportFileID int,
  @EmailReplyToAddress varchar(500),
  @SlaLevelID int,
  @TFSProjectName nvarchar(256)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Products]
  SET
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [Description] = @Description,
    [ImportID] = @ImportID,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [NeedsIndexing] = @NeedsIndexing,
    [ProductFamilyID] = @ProductFamilyID,
    [JiraProjectKey] = @JiraProjectKey,
    [ImportFileID] = @ImportFileID,
    [EmailReplyToAddress] = @EmailReplyToAddress,
    [SlaLevelID] = @SlaLevelID,
    [TFSProjectName] = @TFSProjectName
  WHERE ([ProductID] = @ProductID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteProduct
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteProduct

(
  @ProductID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Products]
  WHERE ([ProductID] = @ProductID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectActionLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [id],
    [ActionID],
    [DateModifiedByTFSSync],
    [TFSID]
  FROM [dbo].[ActionLinkToTFS]
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertActionLinkToTFSItem

(
  @ActionID int,
  @DateModifiedByTFSSync datetime,
  @TFSID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ActionLinkToTFS]
  (
    [ActionID],
    [DateModifiedByTFSSync],
    [TFSID])
  VALUES (
    @ActionID,
    @DateModifiedByTFSSync,
    @TFSID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateActionLinkToTFSItem

(
  @id int,
  @ActionID int,
  @DateModifiedByTFSSync datetime,
  @TFSID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ActionLinkToTFS]
  SET
    [ActionID] = @ActionID,
    [DateModifiedByTFSSync] = @DateModifiedByTFSSync,
    [TFSID] = @TFSID
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteActionLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ActionLinkToTFS]
  WHERE ([id] = @id)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTicketLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [id],
    [TicketID],
    [DateModifiedByTFSSync],
    [SyncWithTFS],
    [TFSID],
    [TFSTitle],
    [TFSURL],
    [TFSState],
    [CreatorID],
    [CrmLinkID]
  FROM [dbo].[TicketLinkToTFS]
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTicketLinkToTFSItem

(
  @TicketID int,
  @DateModifiedByTFSSync datetime,
  @SyncWithTFS bit,
  @TFSID int,
  @TFSTitle nvarchar(256),
  @TFSURL nvarchar(256),
  @TFSState nvarchar(256),
  @CreatorID int,
  @CrmLinkID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TicketLinkToTFS]
  (
    [TicketID],
    [DateModifiedByTFSSync],
    [SyncWithTFS],
    [TFSID],
    [TFSTitle],
    [TFSURL],
    [TFSState],
    [CreatorID],
    [CrmLinkID])
  VALUES (
    @TicketID,
    @DateModifiedByTFSSync,
    @SyncWithTFS,
    @TFSID,
    @TFSTitle,
    @TFSURL,
    @TFSState,
    @CreatorID,
    @CrmLinkID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTicketLinkToTFSItem

(
  @id int,
  @TicketID int,
  @DateModifiedByTFSSync datetime,
  @SyncWithTFS bit,
  @TFSID int,
  @TFSTitle nvarchar(256),
  @TFSURL nvarchar(256),
  @TFSState nvarchar(256),
  @CrmLinkID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TicketLinkToTFS]
  SET
    [TicketID] = @TicketID,
    [DateModifiedByTFSSync] = @DateModifiedByTFSSync,
    [SyncWithTFS] = @SyncWithTFS,
    [TFSID] = @TFSID,
    [TFSTitle] = @TFSTitle,
    [TFSURL] = @TFSURL,
    [TFSState] = @TFSState,
    [CrmLinkID] = @CrmLinkID
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTicketLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TicketLinkToTFS]
  WHERE ([id] = @id)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectProductVersion' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectProductVersion
GO

CREATE PROCEDURE dbo.uspGeneratedSelectProductVersion

(
  @ProductVersionID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ProductVersionID],
    [ProductID],
    [ProductVersionStatusID],
    [VersionNumber],
    [ReleaseDate],
    [IsReleased],
    [Description],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [JiraProjectKey],
    [ImportFileID],
    [TFSProjectName]
  FROM [dbo].[ProductVersions]
  WHERE ([ProductVersionID] = @ProductVersionID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertProductVersion' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertProductVersion
GO

CREATE PROCEDURE dbo.uspGeneratedInsertProductVersion

(
  @ProductID int,
  @ProductVersionStatusID int,
  @VersionNumber varchar(50),
  @ReleaseDate datetime,
  @IsReleased bit,
  @Description varchar(MAX),
  @ImportID varchar(50),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @NeedsIndexing bit,
  @JiraProjectKey varchar(250),
  @ImportFileID int,
  @TFSProjectName nvarchar(256),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ProductVersions]
  (
    [ProductID],
    [ProductVersionStatusID],
    [VersionNumber],
    [ReleaseDate],
    [IsReleased],
    [Description],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [JiraProjectKey],
    [ImportFileID],
    [TFSProjectName])
  VALUES (
    @ProductID,
    @ProductVersionStatusID,
    @VersionNumber,
    @ReleaseDate,
    @IsReleased,
    @Description,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @NeedsIndexing,
    @JiraProjectKey,
    @ImportFileID,
    @TFSProjectName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateProductVersion' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateProductVersion
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateProductVersion

(
  @ProductVersionID int,
  @ProductID int,
  @ProductVersionStatusID int,
  @VersionNumber varchar(50),
  @ReleaseDate datetime,
  @IsReleased bit,
  @Description varchar(MAX),
  @ImportID varchar(50),
  @DateModified datetime,
  @ModifierID int,
  @NeedsIndexing bit,
  @JiraProjectKey varchar(250),
  @ImportFileID int,
  @TFSProjectName nvarchar(256)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ProductVersions]
  SET
    [ProductID] = @ProductID,
    [ProductVersionStatusID] = @ProductVersionStatusID,
    [VersionNumber] = @VersionNumber,
    [ReleaseDate] = @ReleaseDate,
    [IsReleased] = @IsReleased,
    [Description] = @Description,
    [ImportID] = @ImportID,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [NeedsIndexing] = @NeedsIndexing,
    [JiraProjectKey] = @JiraProjectKey,
    [ImportFileID] = @ImportFileID,
    [TFSProjectName] = @TFSProjectName
  WHERE ([ProductVersionID] = @ProductVersionID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteProductVersion' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteProductVersion
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteProductVersion

(
  @ProductVersionID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ProductVersions]
  WHERE ([ProductVersionID] = @ProductVersionID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAttachment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAttachment
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAttachment

(
  @AttachmentID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [AttachmentID],
    [OrganizationID],
    [FileName],
    [FileType],
    [FileSize],
    [Path],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [RefType],
    [RefID],
    [SentToJira],
    [AttachmentGUID],
    [ProductFamilyID],
    [SentToTFS]
  FROM [dbo].[Attachments]
  WHERE ([AttachmentID] = @AttachmentID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAttachment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAttachment
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAttachment

(
  @OrganizationID int,
  @FileName nvarchar(1000),
  @FileType varchar(255),
  @FileSize bigint,
  @Path nvarchar(1000),
  @Description varchar(2000),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @RefType int,
  @RefID int,
  @SentToJira bit,
  @AttachmentGUID uniqueidentifier,
  @ProductFamilyID int,
  @SentToTFS bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Attachments]
  (
    [OrganizationID],
    [FileName],
    [FileType],
    [FileSize],
    [Path],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [RefType],
    [RefID],
    [SentToJira],
    [AttachmentGUID],
    [ProductFamilyID],
    [SentToTFS])
  VALUES (
    @OrganizationID,
    @FileName,
    @FileType,
    @FileSize,
    @Path,
    @Description,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @RefType,
    @RefID,
    @SentToJira,
    @AttachmentGUID,
    @ProductFamilyID,
    @SentToTFS)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAttachment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAttachment
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAttachment

(
  @AttachmentID int,
  @OrganizationID int,
  @FileName nvarchar(1000),
  @FileType varchar(255),
  @FileSize bigint,
  @Path nvarchar(1000),
  @Description varchar(2000),
  @DateModified datetime,
  @ModifierID int,
  @RefType int,
  @RefID int,
  @SentToJira bit,
  @AttachmentGUID uniqueidentifier,
  @ProductFamilyID int,
  @SentToTFS bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Attachments]
  SET
    [OrganizationID] = @OrganizationID,
    [FileName] = @FileName,
    [FileType] = @FileType,
    [FileSize] = @FileSize,
    [Path] = @Path,
    [Description] = @Description,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [RefType] = @RefType,
    [RefID] = @RefID,
    [SentToJira] = @SentToJira,
    [AttachmentGUID] = @AttachmentGUID,
    [ProductFamilyID] = @ProductFamilyID,
    [SentToTFS] = @SentToTFS
  WHERE ([AttachmentID] = @AttachmentID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAttachment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAttachment
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAttachment

(
  @AttachmentID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Attachments]
  WHERE ([AttachmentID] = @AttachmentID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectProductVersionsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectProductVersionsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectProductVersionsViewItem

(
  @ProductVersionID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ProductVersionID],
    [ProductID],
    [ProductVersionStatusID],
    [VersionNumber],
    [ReleaseDate],
    [IsReleased],
    [Description],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [VersionStatus],
    [ProductName],
    [OrganizationID],
    [ProductFamilyID],
    [JiraProjectKey],
    [TFSProjectName]
  FROM [dbo].[ProductVersionsView]
  WHERE ([ProductVersionID] = @ProductVersionID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertProductVersionsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertProductVersionsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertProductVersionsViewItem

(
  @ProductVersionID int,
  @ProductID int,
  @ProductVersionStatusID int,
  @VersionNumber varchar(50),
  @ReleaseDate datetime,
  @IsReleased bit,
  @Description varchar(MAX),
  @ImportID varchar(50),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @NeedsIndexing bit,
  @VersionStatus varchar(255),
  @ProductName varchar(255),
  @OrganizationID int,
  @ProductFamilyID int,
  @JiraProjectKey varchar(250),
  @TFSProjectName nvarchar(256),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ProductVersionsView]
  (
    [ProductVersionID],
    [ProductID],
    [ProductVersionStatusID],
    [VersionNumber],
    [ReleaseDate],
    [IsReleased],
    [Description],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [VersionStatus],
    [ProductName],
    [OrganizationID],
    [ProductFamilyID],
    [JiraProjectKey],
    [TFSProjectName])
  VALUES (
    @ProductVersionID,
    @ProductID,
    @ProductVersionStatusID,
    @VersionNumber,
    @ReleaseDate,
    @IsReleased,
    @Description,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @NeedsIndexing,
    @VersionStatus,
    @ProductName,
    @OrganizationID,
    @ProductFamilyID,
    @JiraProjectKey,
    @TFSProjectName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateProductVersionsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateProductVersionsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateProductVersionsViewItem

(
  @ProductVersionID int,
  @ProductID int,
  @ProductVersionStatusID int,
  @VersionNumber varchar(50),
  @ReleaseDate datetime,
  @IsReleased bit,
  @Description varchar(MAX),
  @ImportID varchar(50),
  @DateModified datetime,
  @ModifierID int,
  @NeedsIndexing bit,
  @VersionStatus varchar(255),
  @ProductName varchar(255),
  @OrganizationID int,
  @ProductFamilyID int,
  @JiraProjectKey varchar(250),
  @TFSProjectName nvarchar(256)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ProductVersionsView]
  SET
    [ProductID] = @ProductID,
    [ProductVersionStatusID] = @ProductVersionStatusID,
    [VersionNumber] = @VersionNumber,
    [ReleaseDate] = @ReleaseDate,
    [IsReleased] = @IsReleased,
    [Description] = @Description,
    [ImportID] = @ImportID,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [NeedsIndexing] = @NeedsIndexing,
    [VersionStatus] = @VersionStatus,
    [ProductName] = @ProductName,
    [OrganizationID] = @OrganizationID,
    [ProductFamilyID] = @ProductFamilyID,
    [JiraProjectKey] = @JiraProjectKey,
    [TFSProjectName] = @TFSProjectName
  WHERE ([ProductVersionID] = @ProductVersionID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteProductVersionsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteProductVersionsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteProductVersionsViewItem

(
  @ProductVersionID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ProductVersionsView]
  WHERE ([ProductVersionID] = @ProductVersionID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectProduct
GO

CREATE PROCEDURE dbo.uspGeneratedSelectProduct

(
  @ProductID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ProductID],
    [OrganizationID],
    [Name],
    [Description],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [ProductFamilyID],
    [JiraProjectKey],
    [ImportFileID],
    [EmailReplyToAddress],
    [SlaLevelID],
    [TFSProjectName]
  FROM [dbo].[Products]
  WHERE ([ProductID] = @ProductID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertProduct
GO

CREATE PROCEDURE dbo.uspGeneratedInsertProduct

(
  @OrganizationID int,
  @Name varchar(255),
  @Description varchar(1024),
  @ImportID varchar(500),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @NeedsIndexing bit,
  @ProductFamilyID int,
  @JiraProjectKey varchar(250),
  @ImportFileID int,
  @EmailReplyToAddress varchar(500),
  @SlaLevelID int,
  @TFSProjectName nvarchar(256),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Products]
  (
    [OrganizationID],
    [Name],
    [Description],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [ProductFamilyID],
    [JiraProjectKey],
    [ImportFileID],
    [EmailReplyToAddress],
    [SlaLevelID],
    [TFSProjectName])
  VALUES (
    @OrganizationID,
    @Name,
    @Description,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @NeedsIndexing,
    @ProductFamilyID,
    @JiraProjectKey,
    @ImportFileID,
    @EmailReplyToAddress,
    @SlaLevelID,
    @TFSProjectName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateProduct
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateProduct

(
  @ProductID int,
  @OrganizationID int,
  @Name varchar(255),
  @Description varchar(1024),
  @ImportID varchar(500),
  @DateModified datetime,
  @ModifierID int,
  @NeedsIndexing bit,
  @ProductFamilyID int,
  @JiraProjectKey varchar(250),
  @ImportFileID int,
  @EmailReplyToAddress varchar(500),
  @SlaLevelID int,
  @TFSProjectName nvarchar(256)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Products]
  SET
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [Description] = @Description,
    [ImportID] = @ImportID,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [NeedsIndexing] = @NeedsIndexing,
    [ProductFamilyID] = @ProductFamilyID,
    [JiraProjectKey] = @JiraProjectKey,
    [ImportFileID] = @ImportFileID,
    [EmailReplyToAddress] = @EmailReplyToAddress,
    [SlaLevelID] = @SlaLevelID,
    [TFSProjectName] = @TFSProjectName
  WHERE ([ProductID] = @ProductID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteProduct
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteProduct

(
  @ProductID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Products]
  WHERE ([ProductID] = @ProductID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectActionLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [id],
    [ActionID],
    [DateModifiedByTFSSync],
    [TFSID]
  FROM [dbo].[ActionLinkToTFS]
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertActionLinkToTFSItem

(
  @ActionID int,
  @DateModifiedByTFSSync datetime,
  @TFSID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ActionLinkToTFS]
  (
    [ActionID],
    [DateModifiedByTFSSync],
    [TFSID])
  VALUES (
    @ActionID,
    @DateModifiedByTFSSync,
    @TFSID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateActionLinkToTFSItem

(
  @id int,
  @ActionID int,
  @DateModifiedByTFSSync datetime,
  @TFSID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ActionLinkToTFS]
  SET
    [ActionID] = @ActionID,
    [DateModifiedByTFSSync] = @DateModifiedByTFSSync,
    [TFSID] = @TFSID
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteActionLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ActionLinkToTFS]
  WHERE ([id] = @id)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTicketLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [id],
    [TicketID],
    [DateModifiedByTFSSync],
    [SyncWithTFS],
    [TFSID],
    [TFSTitle],
    [TFSURL],
    [TFSState],
    [CreatorID],
    [CrmLinkID]
  FROM [dbo].[TicketLinkToTFS]
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTicketLinkToTFSItem

(
  @TicketID int,
  @DateModifiedByTFSSync datetime,
  @SyncWithTFS bit,
  @TFSID int,
  @TFSTitle nvarchar(256),
  @TFSURL nvarchar(256),
  @TFSState nvarchar(256),
  @CreatorID int,
  @CrmLinkID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TicketLinkToTFS]
  (
    [TicketID],
    [DateModifiedByTFSSync],
    [SyncWithTFS],
    [TFSID],
    [TFSTitle],
    [TFSURL],
    [TFSState],
    [CreatorID],
    [CrmLinkID])
  VALUES (
    @TicketID,
    @DateModifiedByTFSSync,
    @SyncWithTFS,
    @TFSID,
    @TFSTitle,
    @TFSURL,
    @TFSState,
    @CreatorID,
    @CrmLinkID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTicketLinkToTFSItem

(
  @id int,
  @TicketID int,
  @DateModifiedByTFSSync datetime,
  @SyncWithTFS bit,
  @TFSID int,
  @TFSTitle nvarchar(256),
  @TFSURL nvarchar(256),
  @TFSState nvarchar(256),
  @CrmLinkID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TicketLinkToTFS]
  SET
    [TicketID] = @TicketID,
    [DateModifiedByTFSSync] = @DateModifiedByTFSSync,
    [SyncWithTFS] = @SyncWithTFS,
    [TFSID] = @TFSID,
    [TFSTitle] = @TFSTitle,
    [TFSURL] = @TFSURL,
    [TFSState] = @TFSState,
    [CrmLinkID] = @CrmLinkID
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTicketLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TicketLinkToTFS]
  WHERE ([id] = @id)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectProductVersion' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectProductVersion
GO

CREATE PROCEDURE dbo.uspGeneratedSelectProductVersion

(
  @ProductVersionID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ProductVersionID],
    [ProductID],
    [ProductVersionStatusID],
    [VersionNumber],
    [ReleaseDate],
    [IsReleased],
    [Description],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [JiraProjectKey],
    [ImportFileID],
    [TFSProjectName]
  FROM [dbo].[ProductVersions]
  WHERE ([ProductVersionID] = @ProductVersionID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertProductVersion' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertProductVersion
GO

CREATE PROCEDURE dbo.uspGeneratedInsertProductVersion

(
  @ProductID int,
  @ProductVersionStatusID int,
  @VersionNumber varchar(50),
  @ReleaseDate datetime,
  @IsReleased bit,
  @Description varchar(MAX),
  @ImportID varchar(50),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @NeedsIndexing bit,
  @JiraProjectKey varchar(250),
  @ImportFileID int,
  @TFSProjectName nvarchar(256),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ProductVersions]
  (
    [ProductID],
    [ProductVersionStatusID],
    [VersionNumber],
    [ReleaseDate],
    [IsReleased],
    [Description],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [JiraProjectKey],
    [ImportFileID],
    [TFSProjectName])
  VALUES (
    @ProductID,
    @ProductVersionStatusID,
    @VersionNumber,
    @ReleaseDate,
    @IsReleased,
    @Description,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @NeedsIndexing,
    @JiraProjectKey,
    @ImportFileID,
    @TFSProjectName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateProductVersion' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateProductVersion
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateProductVersion

(
  @ProductVersionID int,
  @ProductID int,
  @ProductVersionStatusID int,
  @VersionNumber varchar(50),
  @ReleaseDate datetime,
  @IsReleased bit,
  @Description varchar(MAX),
  @ImportID varchar(50),
  @DateModified datetime,
  @ModifierID int,
  @NeedsIndexing bit,
  @JiraProjectKey varchar(250),
  @ImportFileID int,
  @TFSProjectName nvarchar(256)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ProductVersions]
  SET
    [ProductID] = @ProductID,
    [ProductVersionStatusID] = @ProductVersionStatusID,
    [VersionNumber] = @VersionNumber,
    [ReleaseDate] = @ReleaseDate,
    [IsReleased] = @IsReleased,
    [Description] = @Description,
    [ImportID] = @ImportID,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [NeedsIndexing] = @NeedsIndexing,
    [JiraProjectKey] = @JiraProjectKey,
    [ImportFileID] = @ImportFileID,
    [TFSProjectName] = @TFSProjectName
  WHERE ([ProductVersionID] = @ProductVersionID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteProductVersion' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteProductVersion
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteProductVersion

(
  @ProductVersionID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ProductVersions]
  WHERE ([ProductVersionID] = @ProductVersionID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAttachment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAttachment
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAttachment

(
  @AttachmentID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [AttachmentID],
    [OrganizationID],
    [FileName],
    [FileType],
    [FileSize],
    [Path],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [RefType],
    [RefID],
    [SentToJira],
    [AttachmentGUID],
    [ProductFamilyID],
    [SentToTFS]
  FROM [dbo].[Attachments]
  WHERE ([AttachmentID] = @AttachmentID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAttachment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAttachment
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAttachment

(
  @OrganizationID int,
  @FileName nvarchar(1000),
  @FileType varchar(255),
  @FileSize bigint,
  @Path nvarchar(1000),
  @Description varchar(2000),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @RefType int,
  @RefID int,
  @SentToJira bit,
  @AttachmentGUID uniqueidentifier,
  @ProductFamilyID int,
  @SentToTFS bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Attachments]
  (
    [OrganizationID],
    [FileName],
    [FileType],
    [FileSize],
    [Path],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [RefType],
    [RefID],
    [SentToJira],
    [AttachmentGUID],
    [ProductFamilyID],
    [SentToTFS])
  VALUES (
    @OrganizationID,
    @FileName,
    @FileType,
    @FileSize,
    @Path,
    @Description,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @RefType,
    @RefID,
    @SentToJira,
    @AttachmentGUID,
    @ProductFamilyID,
    @SentToTFS)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAttachment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAttachment
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAttachment

(
  @AttachmentID int,
  @OrganizationID int,
  @FileName nvarchar(1000),
  @FileType varchar(255),
  @FileSize bigint,
  @Path nvarchar(1000),
  @Description varchar(2000),
  @DateModified datetime,
  @ModifierID int,
  @RefType int,
  @RefID int,
  @SentToJira bit,
  @AttachmentGUID uniqueidentifier,
  @ProductFamilyID int,
  @SentToTFS bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Attachments]
  SET
    [OrganizationID] = @OrganizationID,
    [FileName] = @FileName,
    [FileType] = @FileType,
    [FileSize] = @FileSize,
    [Path] = @Path,
    [Description] = @Description,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [RefType] = @RefType,
    [RefID] = @RefID,
    [SentToJira] = @SentToJira,
    [AttachmentGUID] = @AttachmentGUID,
    [ProductFamilyID] = @ProductFamilyID,
    [SentToTFS] = @SentToTFS
  WHERE ([AttachmentID] = @AttachmentID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAttachment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAttachment
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAttachment

(
  @AttachmentID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Attachments]
  WHERE ([AttachmentID] = @AttachmentID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectProductVersionsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectProductVersionsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectProductVersionsViewItem

(
  @ProductVersionID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ProductVersionID],
    [ProductID],
    [ProductVersionStatusID],
    [VersionNumber],
    [ReleaseDate],
    [IsReleased],
    [Description],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [VersionStatus],
    [ProductName],
    [OrganizationID],
    [ProductFamilyID],
    [JiraProjectKey],
    [TFSProjectName]
  FROM [dbo].[ProductVersionsView]
  WHERE ([ProductVersionID] = @ProductVersionID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertProductVersionsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertProductVersionsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertProductVersionsViewItem

(
  @ProductVersionID int,
  @ProductID int,
  @ProductVersionStatusID int,
  @VersionNumber varchar(50),
  @ReleaseDate datetime,
  @IsReleased bit,
  @Description varchar(MAX),
  @ImportID varchar(50),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @NeedsIndexing bit,
  @VersionStatus varchar(255),
  @ProductName varchar(255),
  @OrganizationID int,
  @ProductFamilyID int,
  @JiraProjectKey varchar(250),
  @TFSProjectName nvarchar(256),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ProductVersionsView]
  (
    [ProductVersionID],
    [ProductID],
    [ProductVersionStatusID],
    [VersionNumber],
    [ReleaseDate],
    [IsReleased],
    [Description],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [VersionStatus],
    [ProductName],
    [OrganizationID],
    [ProductFamilyID],
    [JiraProjectKey],
    [TFSProjectName])
  VALUES (
    @ProductVersionID,
    @ProductID,
    @ProductVersionStatusID,
    @VersionNumber,
    @ReleaseDate,
    @IsReleased,
    @Description,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @NeedsIndexing,
    @VersionStatus,
    @ProductName,
    @OrganizationID,
    @ProductFamilyID,
    @JiraProjectKey,
    @TFSProjectName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateProductVersionsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateProductVersionsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateProductVersionsViewItem

(
  @ProductVersionID int,
  @ProductID int,
  @ProductVersionStatusID int,
  @VersionNumber varchar(50),
  @ReleaseDate datetime,
  @IsReleased bit,
  @Description varchar(MAX),
  @ImportID varchar(50),
  @DateModified datetime,
  @ModifierID int,
  @NeedsIndexing bit,
  @VersionStatus varchar(255),
  @ProductName varchar(255),
  @OrganizationID int,
  @ProductFamilyID int,
  @JiraProjectKey varchar(250),
  @TFSProjectName nvarchar(256)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ProductVersionsView]
  SET
    [ProductID] = @ProductID,
    [ProductVersionStatusID] = @ProductVersionStatusID,
    [VersionNumber] = @VersionNumber,
    [ReleaseDate] = @ReleaseDate,
    [IsReleased] = @IsReleased,
    [Description] = @Description,
    [ImportID] = @ImportID,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [NeedsIndexing] = @NeedsIndexing,
    [VersionStatus] = @VersionStatus,
    [ProductName] = @ProductName,
    [OrganizationID] = @OrganizationID,
    [ProductFamilyID] = @ProductFamilyID,
    [JiraProjectKey] = @JiraProjectKey,
    [TFSProjectName] = @TFSProjectName
  WHERE ([ProductVersionID] = @ProductVersionID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteProductVersionsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteProductVersionsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteProductVersionsViewItem

(
  @ProductVersionID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ProductVersionsView]
  WHERE ([ProductVersionID] = @ProductVersionID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectProduct
GO

CREATE PROCEDURE dbo.uspGeneratedSelectProduct

(
  @ProductID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ProductID],
    [OrganizationID],
    [Name],
    [Description],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [ProductFamilyID],
    [JiraProjectKey],
    [ImportFileID],
    [EmailReplyToAddress],
    [SlaLevelID],
    [TFSProjectName]
  FROM [dbo].[Products]
  WHERE ([ProductID] = @ProductID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertProduct
GO

CREATE PROCEDURE dbo.uspGeneratedInsertProduct

(
  @OrganizationID int,
  @Name varchar(255),
  @Description varchar(1024),
  @ImportID varchar(500),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @NeedsIndexing bit,
  @ProductFamilyID int,
  @JiraProjectKey varchar(250),
  @ImportFileID int,
  @EmailReplyToAddress varchar(500),
  @SlaLevelID int,
  @TFSProjectName nvarchar(256),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Products]
  (
    [OrganizationID],
    [Name],
    [Description],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [ProductFamilyID],
    [JiraProjectKey],
    [ImportFileID],
    [EmailReplyToAddress],
    [SlaLevelID],
    [TFSProjectName])
  VALUES (
    @OrganizationID,
    @Name,
    @Description,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @NeedsIndexing,
    @ProductFamilyID,
    @JiraProjectKey,
    @ImportFileID,
    @EmailReplyToAddress,
    @SlaLevelID,
    @TFSProjectName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateProduct
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateProduct

(
  @ProductID int,
  @OrganizationID int,
  @Name varchar(255),
  @Description varchar(1024),
  @ImportID varchar(500),
  @DateModified datetime,
  @ModifierID int,
  @NeedsIndexing bit,
  @ProductFamilyID int,
  @JiraProjectKey varchar(250),
  @ImportFileID int,
  @EmailReplyToAddress varchar(500),
  @SlaLevelID int,
  @TFSProjectName nvarchar(256)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Products]
  SET
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [Description] = @Description,
    [ImportID] = @ImportID,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [NeedsIndexing] = @NeedsIndexing,
    [ProductFamilyID] = @ProductFamilyID,
    [JiraProjectKey] = @JiraProjectKey,
    [ImportFileID] = @ImportFileID,
    [EmailReplyToAddress] = @EmailReplyToAddress,
    [SlaLevelID] = @SlaLevelID,
    [TFSProjectName] = @TFSProjectName
  WHERE ([ProductID] = @ProductID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteProduct
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteProduct

(
  @ProductID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Products]
  WHERE ([ProductID] = @ProductID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectActionLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [id],
    [ActionID],
    [DateModifiedByTFSSync],
    [TFSID]
  FROM [dbo].[ActionLinkToTFS]
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertActionLinkToTFSItem

(
  @ActionID int,
  @DateModifiedByTFSSync datetime,
  @TFSID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ActionLinkToTFS]
  (
    [ActionID],
    [DateModifiedByTFSSync],
    [TFSID])
  VALUES (
    @ActionID,
    @DateModifiedByTFSSync,
    @TFSID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateActionLinkToTFSItem

(
  @id int,
  @ActionID int,
  @DateModifiedByTFSSync datetime,
  @TFSID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ActionLinkToTFS]
  SET
    [ActionID] = @ActionID,
    [DateModifiedByTFSSync] = @DateModifiedByTFSSync,
    [TFSID] = @TFSID
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteActionLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ActionLinkToTFS]
  WHERE ([id] = @id)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTicketLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [id],
    [TicketID],
    [DateModifiedByTFSSync],
    [SyncWithTFS],
    [TFSID],
    [TFSTitle],
    [TFSURL],
    [TFSState],
    [CreatorID],
    [CrmLinkID]
  FROM [dbo].[TicketLinkToTFS]
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTicketLinkToTFSItem

(
  @TicketID int,
  @DateModifiedByTFSSync datetime,
  @SyncWithTFS bit,
  @TFSID int,
  @TFSTitle nvarchar(256),
  @TFSURL nvarchar(256),
  @TFSState nvarchar(256),
  @CreatorID int,
  @CrmLinkID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TicketLinkToTFS]
  (
    [TicketID],
    [DateModifiedByTFSSync],
    [SyncWithTFS],
    [TFSID],
    [TFSTitle],
    [TFSURL],
    [TFSState],
    [CreatorID],
    [CrmLinkID])
  VALUES (
    @TicketID,
    @DateModifiedByTFSSync,
    @SyncWithTFS,
    @TFSID,
    @TFSTitle,
    @TFSURL,
    @TFSState,
    @CreatorID,
    @CrmLinkID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTicketLinkToTFSItem

(
  @id int,
  @TicketID int,
  @DateModifiedByTFSSync datetime,
  @SyncWithTFS bit,
  @TFSID int,
  @TFSTitle nvarchar(256),
  @TFSURL nvarchar(256),
  @TFSState nvarchar(256),
  @CrmLinkID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TicketLinkToTFS]
  SET
    [TicketID] = @TicketID,
    [DateModifiedByTFSSync] = @DateModifiedByTFSSync,
    [SyncWithTFS] = @SyncWithTFS,
    [TFSID] = @TFSID,
    [TFSTitle] = @TFSTitle,
    [TFSURL] = @TFSURL,
    [TFSState] = @TFSState,
    [CrmLinkID] = @CrmLinkID
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTicketLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TicketLinkToTFS]
  WHERE ([id] = @id)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectProductVersion' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectProductVersion
GO

CREATE PROCEDURE dbo.uspGeneratedSelectProductVersion

(
  @ProductVersionID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ProductVersionID],
    [ProductID],
    [ProductVersionStatusID],
    [VersionNumber],
    [ReleaseDate],
    [IsReleased],
    [Description],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [JiraProjectKey],
    [ImportFileID],
    [TFSProjectName]
  FROM [dbo].[ProductVersions]
  WHERE ([ProductVersionID] = @ProductVersionID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertProductVersion' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertProductVersion
GO

CREATE PROCEDURE dbo.uspGeneratedInsertProductVersion

(
  @ProductID int,
  @ProductVersionStatusID int,
  @VersionNumber varchar(50),
  @ReleaseDate datetime,
  @IsReleased bit,
  @Description varchar(MAX),
  @ImportID varchar(50),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @NeedsIndexing bit,
  @JiraProjectKey varchar(250),
  @ImportFileID int,
  @TFSProjectName nvarchar(256),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ProductVersions]
  (
    [ProductID],
    [ProductVersionStatusID],
    [VersionNumber],
    [ReleaseDate],
    [IsReleased],
    [Description],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [JiraProjectKey],
    [ImportFileID],
    [TFSProjectName])
  VALUES (
    @ProductID,
    @ProductVersionStatusID,
    @VersionNumber,
    @ReleaseDate,
    @IsReleased,
    @Description,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @NeedsIndexing,
    @JiraProjectKey,
    @ImportFileID,
    @TFSProjectName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateProductVersion' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateProductVersion
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateProductVersion

(
  @ProductVersionID int,
  @ProductID int,
  @ProductVersionStatusID int,
  @VersionNumber varchar(50),
  @ReleaseDate datetime,
  @IsReleased bit,
  @Description varchar(MAX),
  @ImportID varchar(50),
  @DateModified datetime,
  @ModifierID int,
  @NeedsIndexing bit,
  @JiraProjectKey varchar(250),
  @ImportFileID int,
  @TFSProjectName nvarchar(256)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ProductVersions]
  SET
    [ProductID] = @ProductID,
    [ProductVersionStatusID] = @ProductVersionStatusID,
    [VersionNumber] = @VersionNumber,
    [ReleaseDate] = @ReleaseDate,
    [IsReleased] = @IsReleased,
    [Description] = @Description,
    [ImportID] = @ImportID,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [NeedsIndexing] = @NeedsIndexing,
    [JiraProjectKey] = @JiraProjectKey,
    [ImportFileID] = @ImportFileID,
    [TFSProjectName] = @TFSProjectName
  WHERE ([ProductVersionID] = @ProductVersionID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteProductVersion' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteProductVersion
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteProductVersion

(
  @ProductVersionID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ProductVersions]
  WHERE ([ProductVersionID] = @ProductVersionID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAttachment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAttachment
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAttachment

(
  @AttachmentID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [AttachmentID],
    [OrganizationID],
    [FileName],
    [FileType],
    [FileSize],
    [Path],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [RefType],
    [RefID],
    [SentToJira],
    [AttachmentGUID],
    [ProductFamilyID],
    [SentToTFS]
  FROM [dbo].[Attachments]
  WHERE ([AttachmentID] = @AttachmentID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAttachment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAttachment
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAttachment

(
  @OrganizationID int,
  @FileName nvarchar(1000),
  @FileType varchar(255),
  @FileSize bigint,
  @Path nvarchar(1000),
  @Description varchar(2000),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @RefType int,
  @RefID int,
  @SentToJira bit,
  @AttachmentGUID uniqueidentifier,
  @ProductFamilyID int,
  @SentToTFS bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Attachments]
  (
    [OrganizationID],
    [FileName],
    [FileType],
    [FileSize],
    [Path],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [RefType],
    [RefID],
    [SentToJira],
    [AttachmentGUID],
    [ProductFamilyID],
    [SentToTFS])
  VALUES (
    @OrganizationID,
    @FileName,
    @FileType,
    @FileSize,
    @Path,
    @Description,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @RefType,
    @RefID,
    @SentToJira,
    @AttachmentGUID,
    @ProductFamilyID,
    @SentToTFS)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAttachment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAttachment
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAttachment

(
  @AttachmentID int,
  @OrganizationID int,
  @FileName nvarchar(1000),
  @FileType varchar(255),
  @FileSize bigint,
  @Path nvarchar(1000),
  @Description varchar(2000),
  @DateModified datetime,
  @ModifierID int,
  @RefType int,
  @RefID int,
  @SentToJira bit,
  @AttachmentGUID uniqueidentifier,
  @ProductFamilyID int,
  @SentToTFS bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Attachments]
  SET
    [OrganizationID] = @OrganizationID,
    [FileName] = @FileName,
    [FileType] = @FileType,
    [FileSize] = @FileSize,
    [Path] = @Path,
    [Description] = @Description,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [RefType] = @RefType,
    [RefID] = @RefID,
    [SentToJira] = @SentToJira,
    [AttachmentGUID] = @AttachmentGUID,
    [ProductFamilyID] = @ProductFamilyID,
    [SentToTFS] = @SentToTFS
  WHERE ([AttachmentID] = @AttachmentID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAttachment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAttachment
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAttachment

(
  @AttachmentID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Attachments]
  WHERE ([AttachmentID] = @AttachmentID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectProductVersionsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectProductVersionsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectProductVersionsViewItem

(
  @ProductVersionID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ProductVersionID],
    [ProductID],
    [ProductVersionStatusID],
    [VersionNumber],
    [ReleaseDate],
    [IsReleased],
    [Description],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [VersionStatus],
    [ProductName],
    [OrganizationID],
    [ProductFamilyID],
    [JiraProjectKey],
    [TFSProjectName]
  FROM [dbo].[ProductVersionsView]
  WHERE ([ProductVersionID] = @ProductVersionID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertProductVersionsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertProductVersionsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertProductVersionsViewItem

(
  @ProductVersionID int,
  @ProductID int,
  @ProductVersionStatusID int,
  @VersionNumber varchar(50),
  @ReleaseDate datetime,
  @IsReleased bit,
  @Description varchar(MAX),
  @ImportID varchar(50),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @NeedsIndexing bit,
  @VersionStatus varchar(255),
  @ProductName varchar(255),
  @OrganizationID int,
  @ProductFamilyID int,
  @JiraProjectKey varchar(250),
  @TFSProjectName nvarchar(256),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ProductVersionsView]
  (
    [ProductVersionID],
    [ProductID],
    [ProductVersionStatusID],
    [VersionNumber],
    [ReleaseDate],
    [IsReleased],
    [Description],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [VersionStatus],
    [ProductName],
    [OrganizationID],
    [ProductFamilyID],
    [JiraProjectKey],
    [TFSProjectName])
  VALUES (
    @ProductVersionID,
    @ProductID,
    @ProductVersionStatusID,
    @VersionNumber,
    @ReleaseDate,
    @IsReleased,
    @Description,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @NeedsIndexing,
    @VersionStatus,
    @ProductName,
    @OrganizationID,
    @ProductFamilyID,
    @JiraProjectKey,
    @TFSProjectName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateProductVersionsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateProductVersionsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateProductVersionsViewItem

(
  @ProductVersionID int,
  @ProductID int,
  @ProductVersionStatusID int,
  @VersionNumber varchar(50),
  @ReleaseDate datetime,
  @IsReleased bit,
  @Description varchar(MAX),
  @ImportID varchar(50),
  @DateModified datetime,
  @ModifierID int,
  @NeedsIndexing bit,
  @VersionStatus varchar(255),
  @ProductName varchar(255),
  @OrganizationID int,
  @ProductFamilyID int,
  @JiraProjectKey varchar(250),
  @TFSProjectName nvarchar(256)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ProductVersionsView]
  SET
    [ProductID] = @ProductID,
    [ProductVersionStatusID] = @ProductVersionStatusID,
    [VersionNumber] = @VersionNumber,
    [ReleaseDate] = @ReleaseDate,
    [IsReleased] = @IsReleased,
    [Description] = @Description,
    [ImportID] = @ImportID,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [NeedsIndexing] = @NeedsIndexing,
    [VersionStatus] = @VersionStatus,
    [ProductName] = @ProductName,
    [OrganizationID] = @OrganizationID,
    [ProductFamilyID] = @ProductFamilyID,
    [JiraProjectKey] = @JiraProjectKey,
    [TFSProjectName] = @TFSProjectName
  WHERE ([ProductVersionID] = @ProductVersionID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteProductVersionsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteProductVersionsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteProductVersionsViewItem

(
  @ProductVersionID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ProductVersionsView]
  WHERE ([ProductVersionID] = @ProductVersionID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectProduct
GO

CREATE PROCEDURE dbo.uspGeneratedSelectProduct

(
  @ProductID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ProductID],
    [OrganizationID],
    [Name],
    [Description],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [ProductFamilyID],
    [JiraProjectKey],
    [ImportFileID],
    [EmailReplyToAddress],
    [SlaLevelID],
    [TFSProjectName]
  FROM [dbo].[Products]
  WHERE ([ProductID] = @ProductID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertProduct
GO

CREATE PROCEDURE dbo.uspGeneratedInsertProduct

(
  @OrganizationID int,
  @Name varchar(255),
  @Description varchar(1024),
  @ImportID varchar(500),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @NeedsIndexing bit,
  @ProductFamilyID int,
  @JiraProjectKey varchar(250),
  @ImportFileID int,
  @EmailReplyToAddress varchar(500),
  @SlaLevelID int,
  @TFSProjectName nvarchar(256),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Products]
  (
    [OrganizationID],
    [Name],
    [Description],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [ProductFamilyID],
    [JiraProjectKey],
    [ImportFileID],
    [EmailReplyToAddress],
    [SlaLevelID],
    [TFSProjectName])
  VALUES (
    @OrganizationID,
    @Name,
    @Description,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @NeedsIndexing,
    @ProductFamilyID,
    @JiraProjectKey,
    @ImportFileID,
    @EmailReplyToAddress,
    @SlaLevelID,
    @TFSProjectName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateProduct
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateProduct

(
  @ProductID int,
  @OrganizationID int,
  @Name varchar(255),
  @Description varchar(1024),
  @ImportID varchar(500),
  @DateModified datetime,
  @ModifierID int,
  @NeedsIndexing bit,
  @ProductFamilyID int,
  @JiraProjectKey varchar(250),
  @ImportFileID int,
  @EmailReplyToAddress varchar(500),
  @SlaLevelID int,
  @TFSProjectName nvarchar(256)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Products]
  SET
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [Description] = @Description,
    [ImportID] = @ImportID,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [NeedsIndexing] = @NeedsIndexing,
    [ProductFamilyID] = @ProductFamilyID,
    [JiraProjectKey] = @JiraProjectKey,
    [ImportFileID] = @ImportFileID,
    [EmailReplyToAddress] = @EmailReplyToAddress,
    [SlaLevelID] = @SlaLevelID,
    [TFSProjectName] = @TFSProjectName
  WHERE ([ProductID] = @ProductID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteProduct
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteProduct

(
  @ProductID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Products]
  WHERE ([ProductID] = @ProductID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectActionLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [id],
    [ActionID],
    [DateModifiedByTFSSync],
    [TFSID]
  FROM [dbo].[ActionLinkToTFS]
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertActionLinkToTFSItem

(
  @ActionID int,
  @DateModifiedByTFSSync datetime,
  @TFSID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ActionLinkToTFS]
  (
    [ActionID],
    [DateModifiedByTFSSync],
    [TFSID])
  VALUES (
    @ActionID,
    @DateModifiedByTFSSync,
    @TFSID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateActionLinkToTFSItem

(
  @id int,
  @ActionID int,
  @DateModifiedByTFSSync datetime,
  @TFSID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ActionLinkToTFS]
  SET
    [ActionID] = @ActionID,
    [DateModifiedByTFSSync] = @DateModifiedByTFSSync,
    [TFSID] = @TFSID
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteActionLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ActionLinkToTFS]
  WHERE ([id] = @id)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTicketLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [id],
    [TicketID],
    [DateModifiedByTFSSync],
    [SyncWithTFS],
    [TFSID],
    [TFSTitle],
    [TFSURL],
    [TFSState],
    [CreatorID],
    [CrmLinkID]
  FROM [dbo].[TicketLinkToTFS]
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTicketLinkToTFSItem

(
  @TicketID int,
  @DateModifiedByTFSSync datetime,
  @SyncWithTFS bit,
  @TFSID int,
  @TFSTitle nvarchar(256),
  @TFSURL nvarchar(256),
  @TFSState nvarchar(256),
  @CreatorID int,
  @CrmLinkID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TicketLinkToTFS]
  (
    [TicketID],
    [DateModifiedByTFSSync],
    [SyncWithTFS],
    [TFSID],
    [TFSTitle],
    [TFSURL],
    [TFSState],
    [CreatorID],
    [CrmLinkID])
  VALUES (
    @TicketID,
    @DateModifiedByTFSSync,
    @SyncWithTFS,
    @TFSID,
    @TFSTitle,
    @TFSURL,
    @TFSState,
    @CreatorID,
    @CrmLinkID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTicketLinkToTFSItem

(
  @id int,
  @TicketID int,
  @DateModifiedByTFSSync datetime,
  @SyncWithTFS bit,
  @TFSID int,
  @TFSTitle nvarchar(256),
  @TFSURL nvarchar(256),
  @TFSState nvarchar(256),
  @CrmLinkID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TicketLinkToTFS]
  SET
    [TicketID] = @TicketID,
    [DateModifiedByTFSSync] = @DateModifiedByTFSSync,
    [SyncWithTFS] = @SyncWithTFS,
    [TFSID] = @TFSID,
    [TFSTitle] = @TFSTitle,
    [TFSURL] = @TFSURL,
    [TFSState] = @TFSState,
    [CrmLinkID] = @CrmLinkID
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTicketLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TicketLinkToTFS]
  WHERE ([id] = @id)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectProductVersion' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectProductVersion
GO

CREATE PROCEDURE dbo.uspGeneratedSelectProductVersion

(
  @ProductVersionID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ProductVersionID],
    [ProductID],
    [ProductVersionStatusID],
    [VersionNumber],
    [ReleaseDate],
    [IsReleased],
    [Description],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [JiraProjectKey],
    [ImportFileID],
    [TFSProjectName]
  FROM [dbo].[ProductVersions]
  WHERE ([ProductVersionID] = @ProductVersionID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertProductVersion' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertProductVersion
GO

CREATE PROCEDURE dbo.uspGeneratedInsertProductVersion

(
  @ProductID int,
  @ProductVersionStatusID int,
  @VersionNumber varchar(50),
  @ReleaseDate datetime,
  @IsReleased bit,
  @Description varchar(MAX),
  @ImportID varchar(50),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @NeedsIndexing bit,
  @JiraProjectKey varchar(250),
  @ImportFileID int,
  @TFSProjectName nvarchar(256),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ProductVersions]
  (
    [ProductID],
    [ProductVersionStatusID],
    [VersionNumber],
    [ReleaseDate],
    [IsReleased],
    [Description],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [JiraProjectKey],
    [ImportFileID],
    [TFSProjectName])
  VALUES (
    @ProductID,
    @ProductVersionStatusID,
    @VersionNumber,
    @ReleaseDate,
    @IsReleased,
    @Description,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @NeedsIndexing,
    @JiraProjectKey,
    @ImportFileID,
    @TFSProjectName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateProductVersion' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateProductVersion
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateProductVersion

(
  @ProductVersionID int,
  @ProductID int,
  @ProductVersionStatusID int,
  @VersionNumber varchar(50),
  @ReleaseDate datetime,
  @IsReleased bit,
  @Description varchar(MAX),
  @ImportID varchar(50),
  @DateModified datetime,
  @ModifierID int,
  @NeedsIndexing bit,
  @JiraProjectKey varchar(250),
  @ImportFileID int,
  @TFSProjectName nvarchar(256)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ProductVersions]
  SET
    [ProductID] = @ProductID,
    [ProductVersionStatusID] = @ProductVersionStatusID,
    [VersionNumber] = @VersionNumber,
    [ReleaseDate] = @ReleaseDate,
    [IsReleased] = @IsReleased,
    [Description] = @Description,
    [ImportID] = @ImportID,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [NeedsIndexing] = @NeedsIndexing,
    [JiraProjectKey] = @JiraProjectKey,
    [ImportFileID] = @ImportFileID,
    [TFSProjectName] = @TFSProjectName
  WHERE ([ProductVersionID] = @ProductVersionID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteProductVersion' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteProductVersion
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteProductVersion

(
  @ProductVersionID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ProductVersions]
  WHERE ([ProductVersionID] = @ProductVersionID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAttachment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAttachment
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAttachment

(
  @AttachmentID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [AttachmentID],
    [OrganizationID],
    [FileName],
    [FileType],
    [FileSize],
    [Path],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [RefType],
    [RefID],
    [SentToJira],
    [AttachmentGUID],
    [ProductFamilyID],
    [SentToTFS]
  FROM [dbo].[Attachments]
  WHERE ([AttachmentID] = @AttachmentID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAttachment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAttachment
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAttachment

(
  @OrganizationID int,
  @FileName nvarchar(1000),
  @FileType varchar(255),
  @FileSize bigint,
  @Path nvarchar(1000),
  @Description varchar(2000),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @RefType int,
  @RefID int,
  @SentToJira bit,
  @AttachmentGUID uniqueidentifier,
  @ProductFamilyID int,
  @SentToTFS bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Attachments]
  (
    [OrganizationID],
    [FileName],
    [FileType],
    [FileSize],
    [Path],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [RefType],
    [RefID],
    [SentToJira],
    [AttachmentGUID],
    [ProductFamilyID],
    [SentToTFS])
  VALUES (
    @OrganizationID,
    @FileName,
    @FileType,
    @FileSize,
    @Path,
    @Description,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @RefType,
    @RefID,
    @SentToJira,
    @AttachmentGUID,
    @ProductFamilyID,
    @SentToTFS)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAttachment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAttachment
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAttachment

(
  @AttachmentID int,
  @OrganizationID int,
  @FileName nvarchar(1000),
  @FileType varchar(255),
  @FileSize bigint,
  @Path nvarchar(1000),
  @Description varchar(2000),
  @DateModified datetime,
  @ModifierID int,
  @RefType int,
  @RefID int,
  @SentToJira bit,
  @AttachmentGUID uniqueidentifier,
  @ProductFamilyID int,
  @SentToTFS bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Attachments]
  SET
    [OrganizationID] = @OrganizationID,
    [FileName] = @FileName,
    [FileType] = @FileType,
    [FileSize] = @FileSize,
    [Path] = @Path,
    [Description] = @Description,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [RefType] = @RefType,
    [RefID] = @RefID,
    [SentToJira] = @SentToJira,
    [AttachmentGUID] = @AttachmentGUID,
    [ProductFamilyID] = @ProductFamilyID,
    [SentToTFS] = @SentToTFS
  WHERE ([AttachmentID] = @AttachmentID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAttachment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAttachment
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAttachment

(
  @AttachmentID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Attachments]
  WHERE ([AttachmentID] = @AttachmentID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectProductVersionsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectProductVersionsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectProductVersionsViewItem

(
  @ProductVersionID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ProductVersionID],
    [ProductID],
    [ProductVersionStatusID],
    [VersionNumber],
    [ReleaseDate],
    [IsReleased],
    [Description],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [VersionStatus],
    [ProductName],
    [OrganizationID],
    [ProductFamilyID],
    [JiraProjectKey],
    [TFSProjectName]
  FROM [dbo].[ProductVersionsView]
  WHERE ([ProductVersionID] = @ProductVersionID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertProductVersionsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertProductVersionsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertProductVersionsViewItem

(
  @ProductVersionID int,
  @ProductID int,
  @ProductVersionStatusID int,
  @VersionNumber varchar(50),
  @ReleaseDate datetime,
  @IsReleased bit,
  @Description varchar(MAX),
  @ImportID varchar(50),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @NeedsIndexing bit,
  @VersionStatus varchar(255),
  @ProductName varchar(255),
  @OrganizationID int,
  @ProductFamilyID int,
  @JiraProjectKey varchar(250),
  @TFSProjectName nvarchar(256),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ProductVersionsView]
  (
    [ProductVersionID],
    [ProductID],
    [ProductVersionStatusID],
    [VersionNumber],
    [ReleaseDate],
    [IsReleased],
    [Description],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [VersionStatus],
    [ProductName],
    [OrganizationID],
    [ProductFamilyID],
    [JiraProjectKey],
    [TFSProjectName])
  VALUES (
    @ProductVersionID,
    @ProductID,
    @ProductVersionStatusID,
    @VersionNumber,
    @ReleaseDate,
    @IsReleased,
    @Description,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @NeedsIndexing,
    @VersionStatus,
    @ProductName,
    @OrganizationID,
    @ProductFamilyID,
    @JiraProjectKey,
    @TFSProjectName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateProductVersionsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateProductVersionsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateProductVersionsViewItem

(
  @ProductVersionID int,
  @ProductID int,
  @ProductVersionStatusID int,
  @VersionNumber varchar(50),
  @ReleaseDate datetime,
  @IsReleased bit,
  @Description varchar(MAX),
  @ImportID varchar(50),
  @DateModified datetime,
  @ModifierID int,
  @NeedsIndexing bit,
  @VersionStatus varchar(255),
  @ProductName varchar(255),
  @OrganizationID int,
  @ProductFamilyID int,
  @JiraProjectKey varchar(250),
  @TFSProjectName nvarchar(256)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ProductVersionsView]
  SET
    [ProductID] = @ProductID,
    [ProductVersionStatusID] = @ProductVersionStatusID,
    [VersionNumber] = @VersionNumber,
    [ReleaseDate] = @ReleaseDate,
    [IsReleased] = @IsReleased,
    [Description] = @Description,
    [ImportID] = @ImportID,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [NeedsIndexing] = @NeedsIndexing,
    [VersionStatus] = @VersionStatus,
    [ProductName] = @ProductName,
    [OrganizationID] = @OrganizationID,
    [ProductFamilyID] = @ProductFamilyID,
    [JiraProjectKey] = @JiraProjectKey,
    [TFSProjectName] = @TFSProjectName
  WHERE ([ProductVersionID] = @ProductVersionID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteProductVersionsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteProductVersionsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteProductVersionsViewItem

(
  @ProductVersionID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ProductVersionsView]
  WHERE ([ProductVersionID] = @ProductVersionID)
GO


