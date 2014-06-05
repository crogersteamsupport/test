IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAsset' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAsset
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAsset

(
  @AssetID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [AssetID],
    [OrganizationID],
    [SerialNumber],
    [Name],
    [Location],
    [Notes],
    [ProductID],
    [WarrantyExpiration],
    [AssignedTo],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [SubPartOf],
    [Status],
    [ImportID],
    [ProductVersionID],
    [NeedsIndexing]
  FROM [dbo].[Assets]
  WHERE ([AssetID] = @AssetID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAsset' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAsset
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAsset

(
  @OrganizationID int,
  @SerialNumber varchar(500),
  @Name varchar(500),
  @Location varchar(500),
  @Notes text,
  @ProductID int,
  @WarrantyExpiration datetime,
  @AssignedTo int,
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @SubPartOf int,
  @Status varchar(500),
  @ImportID varchar(500),
  @ProductVersionID int,
  @NeedsIndexing bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Assets]
  (
    [OrganizationID],
    [SerialNumber],
    [Name],
    [Location],
    [Notes],
    [ProductID],
    [WarrantyExpiration],
    [AssignedTo],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [SubPartOf],
    [Status],
    [ImportID],
    [ProductVersionID],
    [NeedsIndexing])
  VALUES (
    @OrganizationID,
    @SerialNumber,
    @Name,
    @Location,
    @Notes,
    @ProductID,
    @WarrantyExpiration,
    @AssignedTo,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @SubPartOf,
    @Status,
    @ImportID,
    @ProductVersionID,
    @NeedsIndexing)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAsset' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAsset
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAsset

(
  @AssetID int,
  @OrganizationID int,
  @SerialNumber varchar(500),
  @Name varchar(500),
  @Location varchar(500),
  @Notes text,
  @ProductID int,
  @WarrantyExpiration datetime,
  @AssignedTo int,
  @DateModified datetime,
  @ModifierID int,
  @SubPartOf int,
  @Status varchar(500),
  @ImportID varchar(500),
  @ProductVersionID int,
  @NeedsIndexing bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Assets]
  SET
    [OrganizationID] = @OrganizationID,
    [SerialNumber] = @SerialNumber,
    [Name] = @Name,
    [Location] = @Location,
    [Notes] = @Notes,
    [ProductID] = @ProductID,
    [WarrantyExpiration] = @WarrantyExpiration,
    [AssignedTo] = @AssignedTo,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [SubPartOf] = @SubPartOf,
    [Status] = @Status,
    [ImportID] = @ImportID,
    [ProductVersionID] = @ProductVersionID,
    [NeedsIndexing] = @NeedsIndexing
  WHERE ([AssetID] = @AssetID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAsset' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAsset
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAsset

(
  @AssetID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Assets]
  WHERE ([AssetID] = @AssetID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAsset' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAsset
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAsset

(
  @AssetID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [AssetID],
    [OrganizationID],
    [SerialNumber],
    [Name],
    [Location],
    [Notes],
    [ProductID],
    [WarrantyExpiration],
    [AssignedTo],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [SubPartOf],
    [Status],
    [ImportID],
    [ProductVersionID],
    [NeedsIndexing]
  FROM [dbo].[Assets]
  WHERE ([AssetID] = @AssetID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAsset' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAsset
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAsset

(
  @OrganizationID int,
  @SerialNumber varchar(500),
  @Name varchar(500),
  @Location varchar(500),
  @Notes text,
  @ProductID int,
  @WarrantyExpiration datetime,
  @AssignedTo int,
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @SubPartOf int,
  @Status varchar(500),
  @ImportID varchar(500),
  @ProductVersionID int,
  @NeedsIndexing bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Assets]
  (
    [OrganizationID],
    [SerialNumber],
    [Name],
    [Location],
    [Notes],
    [ProductID],
    [WarrantyExpiration],
    [AssignedTo],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [SubPartOf],
    [Status],
    [ImportID],
    [ProductVersionID],
    [NeedsIndexing])
  VALUES (
    @OrganizationID,
    @SerialNumber,
    @Name,
    @Location,
    @Notes,
    @ProductID,
    @WarrantyExpiration,
    @AssignedTo,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @SubPartOf,
    @Status,
    @ImportID,
    @ProductVersionID,
    @NeedsIndexing)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAsset' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAsset
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAsset

(
  @AssetID int,
  @OrganizationID int,
  @SerialNumber varchar(500),
  @Name varchar(500),
  @Location varchar(500),
  @Notes text,
  @ProductID int,
  @WarrantyExpiration datetime,
  @AssignedTo int,
  @DateModified datetime,
  @ModifierID int,
  @SubPartOf int,
  @Status varchar(500),
  @ImportID varchar(500),
  @ProductVersionID int,
  @NeedsIndexing bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Assets]
  SET
    [OrganizationID] = @OrganizationID,
    [SerialNumber] = @SerialNumber,
    [Name] = @Name,
    [Location] = @Location,
    [Notes] = @Notes,
    [ProductID] = @ProductID,
    [WarrantyExpiration] = @WarrantyExpiration,
    [AssignedTo] = @AssignedTo,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [SubPartOf] = @SubPartOf,
    [Status] = @Status,
    [ImportID] = @ImportID,
    [ProductVersionID] = @ProductVersionID,
    [NeedsIndexing] = @NeedsIndexing
  WHERE ([AssetID] = @AssetID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAsset' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAsset
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAsset

(
  @AssetID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Assets]
  WHERE ([AssetID] = @AssetID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAsset' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAsset
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAsset

(
  @AssetID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [AssetID],
    [OrganizationID],
    [SerialNumber],
    [Name],
    [Location],
    [Notes],
    [ProductID],
    [WarrantyExpiration],
    [AssignedTo],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [SubPartOf],
    [Status],
    [ImportID],
    [ProductVersionID],
    [NeedsIndexing]
  FROM [dbo].[Assets]
  WHERE ([AssetID] = @AssetID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAsset' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAsset
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAsset

(
  @OrganizationID int,
  @SerialNumber varchar(500),
  @Name varchar(500),
  @Location varchar(500),
  @Notes text,
  @ProductID int,
  @WarrantyExpiration datetime,
  @AssignedTo int,
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @SubPartOf int,
  @Status varchar(500),
  @ImportID varchar(500),
  @ProductVersionID int,
  @NeedsIndexing bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Assets]
  (
    [OrganizationID],
    [SerialNumber],
    [Name],
    [Location],
    [Notes],
    [ProductID],
    [WarrantyExpiration],
    [AssignedTo],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [SubPartOf],
    [Status],
    [ImportID],
    [ProductVersionID],
    [NeedsIndexing])
  VALUES (
    @OrganizationID,
    @SerialNumber,
    @Name,
    @Location,
    @Notes,
    @ProductID,
    @WarrantyExpiration,
    @AssignedTo,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @SubPartOf,
    @Status,
    @ImportID,
    @ProductVersionID,
    @NeedsIndexing)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAsset' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAsset
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAsset

(
  @AssetID int,
  @OrganizationID int,
  @SerialNumber varchar(500),
  @Name varchar(500),
  @Location varchar(500),
  @Notes text,
  @ProductID int,
  @WarrantyExpiration datetime,
  @AssignedTo int,
  @DateModified datetime,
  @ModifierID int,
  @SubPartOf int,
  @Status varchar(500),
  @ImportID varchar(500),
  @ProductVersionID int,
  @NeedsIndexing bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Assets]
  SET
    [OrganizationID] = @OrganizationID,
    [SerialNumber] = @SerialNumber,
    [Name] = @Name,
    [Location] = @Location,
    [Notes] = @Notes,
    [ProductID] = @ProductID,
    [WarrantyExpiration] = @WarrantyExpiration,
    [AssignedTo] = @AssignedTo,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [SubPartOf] = @SubPartOf,
    [Status] = @Status,
    [ImportID] = @ImportID,
    [ProductVersionID] = @ProductVersionID,
    [NeedsIndexing] = @NeedsIndexing
  WHERE ([AssetID] = @AssetID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAsset' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAsset
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAsset

(
  @AssetID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Assets]
  WHERE ([AssetID] = @AssetID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAsset' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAsset
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAsset

(
  @AssetID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [AssetID],
    [OrganizationID],
    [SerialNumber],
    [Name],
    [Location],
    [Notes],
    [ProductID],
    [WarrantyExpiration],
    [AssignedTo],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [SubPartOf],
    [Status],
    [ImportID],
    [ProductVersionID],
    [NeedsIndexing]
  FROM [dbo].[Assets]
  WHERE ([AssetID] = @AssetID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAsset' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAsset
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAsset

(
  @OrganizationID int,
  @SerialNumber varchar(500),
  @Name varchar(500),
  @Location varchar(500),
  @Notes text,
  @ProductID int,
  @WarrantyExpiration datetime,
  @AssignedTo int,
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @SubPartOf int,
  @Status varchar(500),
  @ImportID varchar(500),
  @ProductVersionID int,
  @NeedsIndexing bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Assets]
  (
    [OrganizationID],
    [SerialNumber],
    [Name],
    [Location],
    [Notes],
    [ProductID],
    [WarrantyExpiration],
    [AssignedTo],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [SubPartOf],
    [Status],
    [ImportID],
    [ProductVersionID],
    [NeedsIndexing])
  VALUES (
    @OrganizationID,
    @SerialNumber,
    @Name,
    @Location,
    @Notes,
    @ProductID,
    @WarrantyExpiration,
    @AssignedTo,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @SubPartOf,
    @Status,
    @ImportID,
    @ProductVersionID,
    @NeedsIndexing)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAsset' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAsset
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAsset

(
  @AssetID int,
  @OrganizationID int,
  @SerialNumber varchar(500),
  @Name varchar(500),
  @Location varchar(500),
  @Notes text,
  @ProductID int,
  @WarrantyExpiration datetime,
  @AssignedTo int,
  @DateModified datetime,
  @ModifierID int,
  @SubPartOf int,
  @Status varchar(500),
  @ImportID varchar(500),
  @ProductVersionID int,
  @NeedsIndexing bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Assets]
  SET
    [OrganizationID] = @OrganizationID,
    [SerialNumber] = @SerialNumber,
    [Name] = @Name,
    [Location] = @Location,
    [Notes] = @Notes,
    [ProductID] = @ProductID,
    [WarrantyExpiration] = @WarrantyExpiration,
    [AssignedTo] = @AssignedTo,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [SubPartOf] = @SubPartOf,
    [Status] = @Status,
    [ImportID] = @ImportID,
    [ProductVersionID] = @ProductVersionID,
    [NeedsIndexing] = @NeedsIndexing
  WHERE ([AssetID] = @AssetID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAsset' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAsset
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAsset

(
  @AssetID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Assets]
  WHERE ([AssetID] = @AssetID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAsset' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAsset
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAsset

(
  @AssetID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [AssetID],
    [OrganizationID],
    [SerialNumber],
    [Name],
    [Location],
    [Notes],
    [ProductID],
    [WarrantyExpiration],
    [AssignedTo],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [SubPartOf],
    [Status],
    [ImportID],
    [ProductVersionID],
    [NeedsIndexing]
  FROM [dbo].[Assets]
  WHERE ([AssetID] = @AssetID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAsset' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAsset
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAsset

(
  @OrganizationID int,
  @SerialNumber varchar(500),
  @Name varchar(500),
  @Location varchar(500),
  @Notes text,
  @ProductID int,
  @WarrantyExpiration datetime,
  @AssignedTo int,
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @SubPartOf int,
  @Status varchar(500),
  @ImportID varchar(500),
  @ProductVersionID int,
  @NeedsIndexing bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Assets]
  (
    [OrganizationID],
    [SerialNumber],
    [Name],
    [Location],
    [Notes],
    [ProductID],
    [WarrantyExpiration],
    [AssignedTo],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [SubPartOf],
    [Status],
    [ImportID],
    [ProductVersionID],
    [NeedsIndexing])
  VALUES (
    @OrganizationID,
    @SerialNumber,
    @Name,
    @Location,
    @Notes,
    @ProductID,
    @WarrantyExpiration,
    @AssignedTo,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @SubPartOf,
    @Status,
    @ImportID,
    @ProductVersionID,
    @NeedsIndexing)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAsset' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAsset
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAsset

(
  @AssetID int,
  @OrganizationID int,
  @SerialNumber varchar(500),
  @Name varchar(500),
  @Location varchar(500),
  @Notes text,
  @ProductID int,
  @WarrantyExpiration datetime,
  @AssignedTo int,
  @DateModified datetime,
  @ModifierID int,
  @SubPartOf int,
  @Status varchar(500),
  @ImportID varchar(500),
  @ProductVersionID int,
  @NeedsIndexing bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Assets]
  SET
    [OrganizationID] = @OrganizationID,
    [SerialNumber] = @SerialNumber,
    [Name] = @Name,
    [Location] = @Location,
    [Notes] = @Notes,
    [ProductID] = @ProductID,
    [WarrantyExpiration] = @WarrantyExpiration,
    [AssignedTo] = @AssignedTo,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [SubPartOf] = @SubPartOf,
    [Status] = @Status,
    [ImportID] = @ImportID,
    [ProductVersionID] = @ProductVersionID,
    [NeedsIndexing] = @NeedsIndexing
  WHERE ([AssetID] = @AssetID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAsset' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAsset
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAsset

(
  @AssetID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Assets]
  WHERE ([AssetID] = @AssetID)
GO


