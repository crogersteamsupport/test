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
    [NeedsIndexing]
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
    [NeedsIndexing])
  VALUES (
    @OrganizationID,
    @Name,
    @Description,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @NeedsIndexing)

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
  @NeedsIndexing bit
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
    [NeedsIndexing] = @NeedsIndexing
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
    [NeedsIndexing]
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
    [NeedsIndexing])
  VALUES (
    @OrganizationID,
    @Name,
    @Description,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @NeedsIndexing)

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
  @NeedsIndexing bit
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
    [NeedsIndexing] = @NeedsIndexing
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
    [NeedsIndexing]
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
    [NeedsIndexing])
  VALUES (
    @OrganizationID,
    @Name,
    @Description,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @NeedsIndexing)

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
  @NeedsIndexing bit
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
    [NeedsIndexing] = @NeedsIndexing
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
    [NeedsIndexing]
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
    [NeedsIndexing])
  VALUES (
    @OrganizationID,
    @Name,
    @Description,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @NeedsIndexing)

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
  @NeedsIndexing bit
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
    [NeedsIndexing] = @NeedsIndexing
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
    [NeedsIndexing]
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
    [NeedsIndexing])
  VALUES (
    @OrganizationID,
    @Name,
    @Description,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @NeedsIndexing)

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
  @NeedsIndexing bit
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
    [NeedsIndexing] = @NeedsIndexing
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


