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
    [JiraProjectKey]
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
    [JiraProjectKey])
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
    @JiraProjectKey)

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
  @JiraProjectKey varchar(250)
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
    [JiraProjectKey] = @JiraProjectKey
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
    [JiraProjectKey]
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
    [JiraProjectKey])
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
    @JiraProjectKey)

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
  @JiraProjectKey varchar(250)
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
    [JiraProjectKey] = @JiraProjectKey
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
    [JiraProjectKey]
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
    [JiraProjectKey])
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
    @JiraProjectKey)

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
  @JiraProjectKey varchar(250)
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
    [JiraProjectKey] = @JiraProjectKey
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
    [JiraProjectKey]
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
    [JiraProjectKey])
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
    @JiraProjectKey)

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
  @JiraProjectKey varchar(250)
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
    [JiraProjectKey] = @JiraProjectKey
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
    [JiraProjectKey]
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
    [JiraProjectKey])
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
    @JiraProjectKey)

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
  @JiraProjectKey varchar(250)
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
    [JiraProjectKey] = @JiraProjectKey
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
    [JiraProjectKey]
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
    [JiraProjectKey])
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
    @JiraProjectKey)

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
  @JiraProjectKey varchar(250)
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
    [JiraProjectKey] = @JiraProjectKey
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
    [JiraProjectKey]
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
    [JiraProjectKey])
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
    @JiraProjectKey)

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
  @JiraProjectKey varchar(250)
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
    [JiraProjectKey] = @JiraProjectKey
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
    [JiraProjectKey]
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
    [JiraProjectKey])
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
    @JiraProjectKey)

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
  @JiraProjectKey varchar(250)
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
    [JiraProjectKey] = @JiraProjectKey
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
    [JiraProjectKey]
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
    [JiraProjectKey])
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
    @JiraProjectKey)

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
  @JiraProjectKey varchar(250)
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
    [JiraProjectKey] = @JiraProjectKey
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


