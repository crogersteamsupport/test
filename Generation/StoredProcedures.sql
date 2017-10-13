IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectProductFamily' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectProductFamily
GO

CREATE PROCEDURE dbo.uspGeneratedSelectProductFamily

(
  @ProductFamilyID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ProductFamilyID],
    [OrganizationID],
    [Name],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [ImportID],
    [ImportFileID]
  FROM [dbo].[ProductFamilies]
  WHERE ([ProductFamilyID] = @ProductFamilyID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertProductFamily' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertProductFamily
GO

CREATE PROCEDURE dbo.uspGeneratedInsertProductFamily

(
  @OrganizationID int,
  @Name nvarchar(MAX),
  @Description nvarchar(MAX),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @NeedsIndexing int,
  @ImportID varchar(500),
  @ImportFileID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ProductFamilies]
  (
    [OrganizationID],
    [Name],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [ImportID],
    [ImportFileID])
  VALUES (
    @OrganizationID,
    @Name,
    @Description,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @NeedsIndexing,
    @ImportID,
    @ImportFileID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateProductFamily' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateProductFamily
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateProductFamily

(
  @ProductFamilyID int,
  @OrganizationID int,
  @Name nvarchar(MAX),
  @Description nvarchar(MAX),
  @DateModified datetime,
  @ModifierID int,
  @NeedsIndexing int,
  @ImportID varchar(500),
  @ImportFileID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ProductFamilies]
  SET
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [Description] = @Description,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [NeedsIndexing] = @NeedsIndexing,
    [ImportID] = @ImportID,
    [ImportFileID] = @ImportFileID
  WHERE ([ProductFamilyID] = @ProductFamilyID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteProductFamily' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteProductFamily
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteProductFamily

(
  @ProductFamilyID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ProductFamilies]
  WHERE ([ProductFamilyID] = @ProductFamilyID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectProductFamily' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectProductFamily
GO

CREATE PROCEDURE dbo.uspGeneratedSelectProductFamily

(
  @ProductFamilyID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ProductFamilyID],
    [OrganizationID],
    [Name],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [ImportID],
    [ImportFileID]
  FROM [dbo].[ProductFamilies]
  WHERE ([ProductFamilyID] = @ProductFamilyID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertProductFamily' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertProductFamily
GO

CREATE PROCEDURE dbo.uspGeneratedInsertProductFamily

(
  @OrganizationID int,
  @Name nvarchar(MAX),
  @Description nvarchar(MAX),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @NeedsIndexing int,
  @ImportID varchar(500),
  @ImportFileID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ProductFamilies]
  (
    [OrganizationID],
    [Name],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [ImportID],
    [ImportFileID])
  VALUES (
    @OrganizationID,
    @Name,
    @Description,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @NeedsIndexing,
    @ImportID,
    @ImportFileID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateProductFamily' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateProductFamily
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateProductFamily

(
  @ProductFamilyID int,
  @OrganizationID int,
  @Name nvarchar(MAX),
  @Description nvarchar(MAX),
  @DateModified datetime,
  @ModifierID int,
  @NeedsIndexing int,
  @ImportID varchar(500),
  @ImportFileID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ProductFamilies]
  SET
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [Description] = @Description,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [NeedsIndexing] = @NeedsIndexing,
    [ImportID] = @ImportID,
    [ImportFileID] = @ImportFileID
  WHERE ([ProductFamilyID] = @ProductFamilyID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteProductFamily' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteProductFamily
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteProductFamily

(
  @ProductFamilyID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ProductFamilies]
  WHERE ([ProductFamilyID] = @ProductFamilyID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectProductFamily' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectProductFamily
GO

CREATE PROCEDURE dbo.uspGeneratedSelectProductFamily

(
  @ProductFamilyID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ProductFamilyID],
    [OrganizationID],
    [Name],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [ImportID],
    [ImportFileID]
  FROM [dbo].[ProductFamilies]
  WHERE ([ProductFamilyID] = @ProductFamilyID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertProductFamily' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertProductFamily
GO

CREATE PROCEDURE dbo.uspGeneratedInsertProductFamily

(
  @OrganizationID int,
  @Name nvarchar(MAX),
  @Description nvarchar(MAX),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @NeedsIndexing int,
  @ImportID varchar(500),
  @ImportFileID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ProductFamilies]
  (
    [OrganizationID],
    [Name],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [ImportID],
    [ImportFileID])
  VALUES (
    @OrganizationID,
    @Name,
    @Description,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @NeedsIndexing,
    @ImportID,
    @ImportFileID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateProductFamily' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateProductFamily
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateProductFamily

(
  @ProductFamilyID int,
  @OrganizationID int,
  @Name nvarchar(MAX),
  @Description nvarchar(MAX),
  @DateModified datetime,
  @ModifierID int,
  @NeedsIndexing int,
  @ImportID varchar(500),
  @ImportFileID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ProductFamilies]
  SET
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [Description] = @Description,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [NeedsIndexing] = @NeedsIndexing,
    [ImportID] = @ImportID,
    [ImportFileID] = @ImportFileID
  WHERE ([ProductFamilyID] = @ProductFamilyID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteProductFamily' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteProductFamily
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteProductFamily

(
  @ProductFamilyID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ProductFamilies]
  WHERE ([ProductFamilyID] = @ProductFamilyID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectProductFamily' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectProductFamily
GO

CREATE PROCEDURE dbo.uspGeneratedSelectProductFamily

(
  @ProductFamilyID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ProductFamilyID],
    [OrganizationID],
    [Name],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [ImportID],
    [ImportFileID]
  FROM [dbo].[ProductFamilies]
  WHERE ([ProductFamilyID] = @ProductFamilyID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertProductFamily' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertProductFamily
GO

CREATE PROCEDURE dbo.uspGeneratedInsertProductFamily

(
  @OrganizationID int,
  @Name nvarchar(MAX),
  @Description nvarchar(MAX),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @NeedsIndexing int,
  @ImportID varchar(500),
  @ImportFileID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ProductFamilies]
  (
    [OrganizationID],
    [Name],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [ImportID],
    [ImportFileID])
  VALUES (
    @OrganizationID,
    @Name,
    @Description,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @NeedsIndexing,
    @ImportID,
    @ImportFileID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateProductFamily' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateProductFamily
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateProductFamily

(
  @ProductFamilyID int,
  @OrganizationID int,
  @Name nvarchar(MAX),
  @Description nvarchar(MAX),
  @DateModified datetime,
  @ModifierID int,
  @NeedsIndexing int,
  @ImportID varchar(500),
  @ImportFileID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ProductFamilies]
  SET
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [Description] = @Description,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [NeedsIndexing] = @NeedsIndexing,
    [ImportID] = @ImportID,
    [ImportFileID] = @ImportFileID
  WHERE ([ProductFamilyID] = @ProductFamilyID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteProductFamily' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteProductFamily
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteProductFamily

(
  @ProductFamilyID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ProductFamilies]
  WHERE ([ProductFamilyID] = @ProductFamilyID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectProductFamily' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectProductFamily
GO

CREATE PROCEDURE dbo.uspGeneratedSelectProductFamily

(
  @ProductFamilyID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ProductFamilyID],
    [OrganizationID],
    [Name],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [ImportID],
    [ImportFileID]
  FROM [dbo].[ProductFamilies]
  WHERE ([ProductFamilyID] = @ProductFamilyID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertProductFamily' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertProductFamily
GO

CREATE PROCEDURE dbo.uspGeneratedInsertProductFamily

(
  @OrganizationID int,
  @Name nvarchar(MAX),
  @Description nvarchar(MAX),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @NeedsIndexing int,
  @ImportID varchar(500),
  @ImportFileID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ProductFamilies]
  (
    [OrganizationID],
    [Name],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [NeedsIndexing],
    [ImportID],
    [ImportFileID])
  VALUES (
    @OrganizationID,
    @Name,
    @Description,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @NeedsIndexing,
    @ImportID,
    @ImportFileID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateProductFamily' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateProductFamily
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateProductFamily

(
  @ProductFamilyID int,
  @OrganizationID int,
  @Name nvarchar(MAX),
  @Description nvarchar(MAX),
  @DateModified datetime,
  @ModifierID int,
  @NeedsIndexing int,
  @ImportID varchar(500),
  @ImportFileID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ProductFamilies]
  SET
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [Description] = @Description,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [NeedsIndexing] = @NeedsIndexing,
    [ImportID] = @ImportID,
    [ImportFileID] = @ImportFileID
  WHERE ([ProductFamilyID] = @ProductFamilyID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteProductFamily' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteProductFamily
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteProductFamily

(
  @ProductFamilyID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ProductFamilies]
  WHERE ([ProductFamilyID] = @ProductFamilyID)
GO


