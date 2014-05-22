IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAssetsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAssetsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAssetsViewItem

(
  @AssetID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [AssetID],
    [ProductID],
    [ProductName],
    [ProductVersionID],
    [ProductVersionNumber],
    [OrganizationID],
    [SerialNumber],
    [Name],
    [Location],
    [Notes],
    [WarrantyExpiration],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [CreatorName],
    [ModifierID],
    [ModifierName],
    [SubPartOf],
    [Status],
    [ImportID]
  FROM [dbo].[AssetsView]
  WHERE ([AssetID] = @AssetID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAssetsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAssetsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAssetsViewItem

(
  @AssetID int,
  @ProductID int,
  @ProductName varchar(255),
  @ProductVersionID int,
  @ProductVersionNumber varchar(50),
  @OrganizationID int,
  @SerialNumber varchar(500),
  @Name varchar(500),
  @Location varchar(500),
  @Notes text,
  @WarrantyExpiration datetime,
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @CreatorName varchar(201),
  @ModifierID int,
  @ModifierName varchar(201),
  @SubPartOf int,
  @Status varchar(500),
  @ImportID varchar(500),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[AssetsView]
  (
    [AssetID],
    [ProductID],
    [ProductName],
    [ProductVersionID],
    [ProductVersionNumber],
    [OrganizationID],
    [SerialNumber],
    [Name],
    [Location],
    [Notes],
    [WarrantyExpiration],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [CreatorName],
    [ModifierID],
    [ModifierName],
    [SubPartOf],
    [Status],
    [ImportID])
  VALUES (
    @AssetID,
    @ProductID,
    @ProductName,
    @ProductVersionID,
    @ProductVersionNumber,
    @OrganizationID,
    @SerialNumber,
    @Name,
    @Location,
    @Notes,
    @WarrantyExpiration,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @CreatorName,
    @ModifierID,
    @ModifierName,
    @SubPartOf,
    @Status,
    @ImportID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAssetsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAssetsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAssetsViewItem

(
  @AssetID int,
  @ProductID int,
  @ProductName varchar(255),
  @ProductVersionID int,
  @ProductVersionNumber varchar(50),
  @OrganizationID int,
  @SerialNumber varchar(500),
  @Name varchar(500),
  @Location varchar(500),
  @Notes text,
  @WarrantyExpiration datetime,
  @DateModified datetime,
  @CreatorName varchar(201),
  @ModifierID int,
  @ModifierName varchar(201),
  @SubPartOf int,
  @Status varchar(500),
  @ImportID varchar(500)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[AssetsView]
  SET
    [ProductID] = @ProductID,
    [ProductName] = @ProductName,
    [ProductVersionID] = @ProductVersionID,
    [ProductVersionNumber] = @ProductVersionNumber,
    [OrganizationID] = @OrganizationID,
    [SerialNumber] = @SerialNumber,
    [Name] = @Name,
    [Location] = @Location,
    [Notes] = @Notes,
    [WarrantyExpiration] = @WarrantyExpiration,
    [DateModified] = @DateModified,
    [CreatorName] = @CreatorName,
    [ModifierID] = @ModifierID,
    [ModifierName] = @ModifierName,
    [SubPartOf] = @SubPartOf,
    [Status] = @Status,
    [ImportID] = @ImportID
  WHERE ([AssetID] = @AssetID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAssetsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAssetsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAssetsViewItem

(
  @AssetID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[AssetsView]
  WHERE ([AssetID] = @AssetID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAssetsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAssetsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAssetsViewItem

(
  @AssetID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [AssetID],
    [ProductID],
    [ProductName],
    [ProductVersionID],
    [ProductVersionNumber],
    [OrganizationID],
    [SerialNumber],
    [Name],
    [Location],
    [Notes],
    [WarrantyExpiration],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [CreatorName],
    [ModifierID],
    [ModifierName],
    [SubPartOf],
    [Status],
    [ImportID]
  FROM [dbo].[AssetsView]
  WHERE ([AssetID] = @AssetID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAssetsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAssetsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAssetsViewItem

(
  @AssetID int,
  @ProductID int,
  @ProductName varchar(255),
  @ProductVersionID int,
  @ProductVersionNumber varchar(50),
  @OrganizationID int,
  @SerialNumber varchar(500),
  @Name varchar(500),
  @Location varchar(500),
  @Notes text,
  @WarrantyExpiration datetime,
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @CreatorName varchar(201),
  @ModifierID int,
  @ModifierName varchar(201),
  @SubPartOf int,
  @Status varchar(500),
  @ImportID varchar(500),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[AssetsView]
  (
    [AssetID],
    [ProductID],
    [ProductName],
    [ProductVersionID],
    [ProductVersionNumber],
    [OrganizationID],
    [SerialNumber],
    [Name],
    [Location],
    [Notes],
    [WarrantyExpiration],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [CreatorName],
    [ModifierID],
    [ModifierName],
    [SubPartOf],
    [Status],
    [ImportID])
  VALUES (
    @AssetID,
    @ProductID,
    @ProductName,
    @ProductVersionID,
    @ProductVersionNumber,
    @OrganizationID,
    @SerialNumber,
    @Name,
    @Location,
    @Notes,
    @WarrantyExpiration,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @CreatorName,
    @ModifierID,
    @ModifierName,
    @SubPartOf,
    @Status,
    @ImportID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAssetsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAssetsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAssetsViewItem

(
  @AssetID int,
  @ProductID int,
  @ProductName varchar(255),
  @ProductVersionID int,
  @ProductVersionNumber varchar(50),
  @OrganizationID int,
  @SerialNumber varchar(500),
  @Name varchar(500),
  @Location varchar(500),
  @Notes text,
  @WarrantyExpiration datetime,
  @DateModified datetime,
  @CreatorName varchar(201),
  @ModifierID int,
  @ModifierName varchar(201),
  @SubPartOf int,
  @Status varchar(500),
  @ImportID varchar(500)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[AssetsView]
  SET
    [ProductID] = @ProductID,
    [ProductName] = @ProductName,
    [ProductVersionID] = @ProductVersionID,
    [ProductVersionNumber] = @ProductVersionNumber,
    [OrganizationID] = @OrganizationID,
    [SerialNumber] = @SerialNumber,
    [Name] = @Name,
    [Location] = @Location,
    [Notes] = @Notes,
    [WarrantyExpiration] = @WarrantyExpiration,
    [DateModified] = @DateModified,
    [CreatorName] = @CreatorName,
    [ModifierID] = @ModifierID,
    [ModifierName] = @ModifierName,
    [SubPartOf] = @SubPartOf,
    [Status] = @Status,
    [ImportID] = @ImportID
  WHERE ([AssetID] = @AssetID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAssetsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAssetsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAssetsViewItem

(
  @AssetID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[AssetsView]
  WHERE ([AssetID] = @AssetID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAssetsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAssetsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAssetsViewItem

(
  @AssetID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [AssetID],
    [ProductID],
    [ProductName],
    [ProductVersionID],
    [ProductVersionNumber],
    [OrganizationID],
    [SerialNumber],
    [Name],
    [Location],
    [Notes],
    [WarrantyExpiration],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [CreatorName],
    [ModifierID],
    [ModifierName],
    [SubPartOf],
    [Status],
    [ImportID]
  FROM [dbo].[AssetsView]
  WHERE ([AssetID] = @AssetID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAssetsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAssetsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAssetsViewItem

(
  @AssetID int,
  @ProductID int,
  @ProductName varchar(255),
  @ProductVersionID int,
  @ProductVersionNumber varchar(50),
  @OrganizationID int,
  @SerialNumber varchar(500),
  @Name varchar(500),
  @Location varchar(500),
  @Notes text,
  @WarrantyExpiration datetime,
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @CreatorName varchar(201),
  @ModifierID int,
  @ModifierName varchar(201),
  @SubPartOf int,
  @Status varchar(500),
  @ImportID varchar(500),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[AssetsView]
  (
    [AssetID],
    [ProductID],
    [ProductName],
    [ProductVersionID],
    [ProductVersionNumber],
    [OrganizationID],
    [SerialNumber],
    [Name],
    [Location],
    [Notes],
    [WarrantyExpiration],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [CreatorName],
    [ModifierID],
    [ModifierName],
    [SubPartOf],
    [Status],
    [ImportID])
  VALUES (
    @AssetID,
    @ProductID,
    @ProductName,
    @ProductVersionID,
    @ProductVersionNumber,
    @OrganizationID,
    @SerialNumber,
    @Name,
    @Location,
    @Notes,
    @WarrantyExpiration,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @CreatorName,
    @ModifierID,
    @ModifierName,
    @SubPartOf,
    @Status,
    @ImportID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAssetsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAssetsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAssetsViewItem

(
  @AssetID int,
  @ProductID int,
  @ProductName varchar(255),
  @ProductVersionID int,
  @ProductVersionNumber varchar(50),
  @OrganizationID int,
  @SerialNumber varchar(500),
  @Name varchar(500),
  @Location varchar(500),
  @Notes text,
  @WarrantyExpiration datetime,
  @DateModified datetime,
  @CreatorName varchar(201),
  @ModifierID int,
  @ModifierName varchar(201),
  @SubPartOf int,
  @Status varchar(500),
  @ImportID varchar(500)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[AssetsView]
  SET
    [ProductID] = @ProductID,
    [ProductName] = @ProductName,
    [ProductVersionID] = @ProductVersionID,
    [ProductVersionNumber] = @ProductVersionNumber,
    [OrganizationID] = @OrganizationID,
    [SerialNumber] = @SerialNumber,
    [Name] = @Name,
    [Location] = @Location,
    [Notes] = @Notes,
    [WarrantyExpiration] = @WarrantyExpiration,
    [DateModified] = @DateModified,
    [CreatorName] = @CreatorName,
    [ModifierID] = @ModifierID,
    [ModifierName] = @ModifierName,
    [SubPartOf] = @SubPartOf,
    [Status] = @Status,
    [ImportID] = @ImportID
  WHERE ([AssetID] = @AssetID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAssetsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAssetsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAssetsViewItem

(
  @AssetID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[AssetsView]
  WHERE ([AssetID] = @AssetID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAssetsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAssetsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAssetsViewItem

(
  @AssetID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [AssetID],
    [ProductID],
    [ProductName],
    [ProductVersionID],
    [ProductVersionNumber],
    [OrganizationID],
    [SerialNumber],
    [Name],
    [Location],
    [Notes],
    [WarrantyExpiration],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [CreatorName],
    [ModifierID],
    [ModifierName],
    [SubPartOf],
    [Status],
    [ImportID]
  FROM [dbo].[AssetsView]
  WHERE ([AssetID] = @AssetID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAssetsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAssetsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAssetsViewItem

(
  @AssetID int,
  @ProductID int,
  @ProductName varchar(255),
  @ProductVersionID int,
  @ProductVersionNumber varchar(50),
  @OrganizationID int,
  @SerialNumber varchar(500),
  @Name varchar(500),
  @Location varchar(500),
  @Notes text,
  @WarrantyExpiration datetime,
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @CreatorName varchar(201),
  @ModifierID int,
  @ModifierName varchar(201),
  @SubPartOf int,
  @Status varchar(500),
  @ImportID varchar(500),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[AssetsView]
  (
    [AssetID],
    [ProductID],
    [ProductName],
    [ProductVersionID],
    [ProductVersionNumber],
    [OrganizationID],
    [SerialNumber],
    [Name],
    [Location],
    [Notes],
    [WarrantyExpiration],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [CreatorName],
    [ModifierID],
    [ModifierName],
    [SubPartOf],
    [Status],
    [ImportID])
  VALUES (
    @AssetID,
    @ProductID,
    @ProductName,
    @ProductVersionID,
    @ProductVersionNumber,
    @OrganizationID,
    @SerialNumber,
    @Name,
    @Location,
    @Notes,
    @WarrantyExpiration,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @CreatorName,
    @ModifierID,
    @ModifierName,
    @SubPartOf,
    @Status,
    @ImportID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAssetsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAssetsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAssetsViewItem

(
  @AssetID int,
  @ProductID int,
  @ProductName varchar(255),
  @ProductVersionID int,
  @ProductVersionNumber varchar(50),
  @OrganizationID int,
  @SerialNumber varchar(500),
  @Name varchar(500),
  @Location varchar(500),
  @Notes text,
  @WarrantyExpiration datetime,
  @DateModified datetime,
  @CreatorName varchar(201),
  @ModifierID int,
  @ModifierName varchar(201),
  @SubPartOf int,
  @Status varchar(500),
  @ImportID varchar(500)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[AssetsView]
  SET
    [ProductID] = @ProductID,
    [ProductName] = @ProductName,
    [ProductVersionID] = @ProductVersionID,
    [ProductVersionNumber] = @ProductVersionNumber,
    [OrganizationID] = @OrganizationID,
    [SerialNumber] = @SerialNumber,
    [Name] = @Name,
    [Location] = @Location,
    [Notes] = @Notes,
    [WarrantyExpiration] = @WarrantyExpiration,
    [DateModified] = @DateModified,
    [CreatorName] = @CreatorName,
    [ModifierID] = @ModifierID,
    [ModifierName] = @ModifierName,
    [SubPartOf] = @SubPartOf,
    [Status] = @Status,
    [ImportID] = @ImportID
  WHERE ([AssetID] = @AssetID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAssetsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAssetsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAssetsViewItem

(
  @AssetID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[AssetsView]
  WHERE ([AssetID] = @AssetID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAssetsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAssetsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAssetsViewItem

(
  @AssetID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [AssetID],
    [ProductID],
    [ProductName],
    [ProductVersionID],
    [ProductVersionNumber],
    [OrganizationID],
    [SerialNumber],
    [Name],
    [Location],
    [Notes],
    [WarrantyExpiration],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [CreatorName],
    [ModifierID],
    [ModifierName],
    [SubPartOf],
    [Status],
    [ImportID]
  FROM [dbo].[AssetsView]
  WHERE ([AssetID] = @AssetID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAssetsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAssetsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAssetsViewItem

(
  @AssetID int,
  @ProductID int,
  @ProductName varchar(255),
  @ProductVersionID int,
  @ProductVersionNumber varchar(50),
  @OrganizationID int,
  @SerialNumber varchar(500),
  @Name varchar(500),
  @Location varchar(500),
  @Notes text,
  @WarrantyExpiration datetime,
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @CreatorName varchar(201),
  @ModifierID int,
  @ModifierName varchar(201),
  @SubPartOf int,
  @Status varchar(500),
  @ImportID varchar(500),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[AssetsView]
  (
    [AssetID],
    [ProductID],
    [ProductName],
    [ProductVersionID],
    [ProductVersionNumber],
    [OrganizationID],
    [SerialNumber],
    [Name],
    [Location],
    [Notes],
    [WarrantyExpiration],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [CreatorName],
    [ModifierID],
    [ModifierName],
    [SubPartOf],
    [Status],
    [ImportID])
  VALUES (
    @AssetID,
    @ProductID,
    @ProductName,
    @ProductVersionID,
    @ProductVersionNumber,
    @OrganizationID,
    @SerialNumber,
    @Name,
    @Location,
    @Notes,
    @WarrantyExpiration,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @CreatorName,
    @ModifierID,
    @ModifierName,
    @SubPartOf,
    @Status,
    @ImportID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAssetsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAssetsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAssetsViewItem

(
  @AssetID int,
  @ProductID int,
  @ProductName varchar(255),
  @ProductVersionID int,
  @ProductVersionNumber varchar(50),
  @OrganizationID int,
  @SerialNumber varchar(500),
  @Name varchar(500),
  @Location varchar(500),
  @Notes text,
  @WarrantyExpiration datetime,
  @DateModified datetime,
  @CreatorName varchar(201),
  @ModifierID int,
  @ModifierName varchar(201),
  @SubPartOf int,
  @Status varchar(500),
  @ImportID varchar(500)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[AssetsView]
  SET
    [ProductID] = @ProductID,
    [ProductName] = @ProductName,
    [ProductVersionID] = @ProductVersionID,
    [ProductVersionNumber] = @ProductVersionNumber,
    [OrganizationID] = @OrganizationID,
    [SerialNumber] = @SerialNumber,
    [Name] = @Name,
    [Location] = @Location,
    [Notes] = @Notes,
    [WarrantyExpiration] = @WarrantyExpiration,
    [DateModified] = @DateModified,
    [CreatorName] = @CreatorName,
    [ModifierID] = @ModifierID,
    [ModifierName] = @ModifierName,
    [SubPartOf] = @SubPartOf,
    [Status] = @Status,
    [ImportID] = @ImportID
  WHERE ([AssetID] = @AssetID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAssetsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAssetsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAssetsViewItem

(
  @AssetID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[AssetsView]
  WHERE ([AssetID] = @AssetID)
GO


