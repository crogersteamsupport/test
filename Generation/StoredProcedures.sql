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
  @TFSProjectName nvarchar(MAX),
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
  @TFSProjectName nvarchar(MAX)
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
  @TFSProjectName nvarchar(MAX),
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
  @TFSProjectName nvarchar(MAX)
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
  @TFSProjectName nvarchar(MAX),
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
  @TFSProjectName nvarchar(MAX)
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
  @TFSProjectName nvarchar(MAX),
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
  @TFSProjectName nvarchar(MAX)
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
  @TFSProjectName nvarchar(MAX),
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
  @TFSProjectName nvarchar(MAX)
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


