IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAssetHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAssetHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAssetHistoryItem

(
  @HistoryID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [HistoryID],
    [AssetID],
    [OrganizationID],
    [ActionTime],
    [ActionDescription],
    [ShippedFrom],
    [ShippedTo],
    [TrackingNumber],
    [ShippingMethod],
    [ReferenceNum],
    [Comments],
    [DateCreated],
    [Actor],
    [RefType],
    [DateModified],
    [ModifierID]
  FROM [dbo].[AssetHistory]
  WHERE ([HistoryID] = @HistoryID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAssetHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAssetHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAssetHistoryItem

(
  @AssetID int,
  @OrganizationID int,
  @ActionTime datetime,
  @ActionDescription varchar(500),
  @ShippedFrom int,
  @ShippedTo int,
  @TrackingNumber varchar(200),
  @ShippingMethod varchar(200),
  @ReferenceNum varchar(200),
  @Comments text,
  @DateCreated datetime,
  @Actor int,
  @RefType int,
  @DateModified datetime,
  @ModifierID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[AssetHistory]
  (
    [AssetID],
    [OrganizationID],
    [ActionTime],
    [ActionDescription],
    [ShippedFrom],
    [ShippedTo],
    [TrackingNumber],
    [ShippingMethod],
    [ReferenceNum],
    [Comments],
    [DateCreated],
    [Actor],
    [RefType],
    [DateModified],
    [ModifierID])
  VALUES (
    @AssetID,
    @OrganizationID,
    @ActionTime,
    @ActionDescription,
    @ShippedFrom,
    @ShippedTo,
    @TrackingNumber,
    @ShippingMethod,
    @ReferenceNum,
    @Comments,
    @DateCreated,
    @Actor,
    @RefType,
    @DateModified,
    @ModifierID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAssetHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAssetHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAssetHistoryItem

(
  @HistoryID int,
  @AssetID int,
  @OrganizationID int,
  @ActionTime datetime,
  @ActionDescription varchar(500),
  @ShippedFrom int,
  @ShippedTo int,
  @TrackingNumber varchar(200),
  @ShippingMethod varchar(200),
  @ReferenceNum varchar(200),
  @Comments text,
  @Actor int,
  @RefType int,
  @DateModified datetime,
  @ModifierID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[AssetHistory]
  SET
    [AssetID] = @AssetID,
    [OrganizationID] = @OrganizationID,
    [ActionTime] = @ActionTime,
    [ActionDescription] = @ActionDescription,
    [ShippedFrom] = @ShippedFrom,
    [ShippedTo] = @ShippedTo,
    [TrackingNumber] = @TrackingNumber,
    [ShippingMethod] = @ShippingMethod,
    [ReferenceNum] = @ReferenceNum,
    [Comments] = @Comments,
    [Actor] = @Actor,
    [RefType] = @RefType,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID
  WHERE ([HistoryID] = @HistoryID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAssetHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAssetHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAssetHistoryItem

(
  @HistoryID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[AssetHistory]
  WHERE ([HistoryID] = @HistoryID)
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
    [ProductVersionID]
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
    [ProductVersionID])
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
    @ProductVersionID)

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
  @ProductVersionID int
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
    [ProductVersionID] = @ProductVersionID
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


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAssetHistoryViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAssetHistoryViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAssetHistoryViewItem

(
  @HistoryID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [HistoryID],
    [AssetID],
    [OrganizationID],
    [ActionTime],
    [ActionDescription],
    [ShippedFrom],
    [ShippedTo],
    [NameAssignedTo],
    [TrackingNumber],
    [ShippingMethod],
    [ReferenceNum],
    [Comments],
    [DateCreated],
    [Actor],
    [ActorName],
    [RefType],
    [DateModified],
    [ModifierID],
    [ModifierName]
  FROM [dbo].[AssetHistoryView]
  WHERE ([HistoryID] = @HistoryID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAssetHistoryViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAssetHistoryViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAssetHistoryViewItem

(
  @HistoryID int,
  @AssetID int,
  @OrganizationID int,
  @ActionTime datetime,
  @ActionDescription varchar(500),
  @ShippedFrom int,
  @ShippedTo int,
  @NameAssignedTo varchar(255),
  @TrackingNumber varchar(200),
  @ShippingMethod varchar(200),
  @ReferenceNum varchar(200),
  @Comments text,
  @DateCreated datetime,
  @Actor int,
  @ActorName varchar(201),
  @RefType int,
  @DateModified datetime,
  @ModifierID int,
  @ModifierName varchar(201),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[AssetHistoryView]
  (
    [HistoryID],
    [AssetID],
    [OrganizationID],
    [ActionTime],
    [ActionDescription],
    [ShippedFrom],
    [ShippedTo],
    [NameAssignedTo],
    [TrackingNumber],
    [ShippingMethod],
    [ReferenceNum],
    [Comments],
    [DateCreated],
    [Actor],
    [ActorName],
    [RefType],
    [DateModified],
    [ModifierID],
    [ModifierName])
  VALUES (
    @HistoryID,
    @AssetID,
    @OrganizationID,
    @ActionTime,
    @ActionDescription,
    @ShippedFrom,
    @ShippedTo,
    @NameAssignedTo,
    @TrackingNumber,
    @ShippingMethod,
    @ReferenceNum,
    @Comments,
    @DateCreated,
    @Actor,
    @ActorName,
    @RefType,
    @DateModified,
    @ModifierID,
    @ModifierName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAssetHistoryViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAssetHistoryViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAssetHistoryViewItem

(
  @HistoryID int,
  @AssetID int,
  @OrganizationID int,
  @ActionTime datetime,
  @ActionDescription varchar(500),
  @ShippedFrom int,
  @ShippedTo int,
  @NameAssignedTo varchar(255),
  @TrackingNumber varchar(200),
  @ShippingMethod varchar(200),
  @ReferenceNum varchar(200),
  @Comments text,
  @Actor int,
  @ActorName varchar(201),
  @RefType int,
  @DateModified datetime,
  @ModifierID int,
  @ModifierName varchar(201)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[AssetHistoryView]
  SET
    [AssetID] = @AssetID,
    [OrganizationID] = @OrganizationID,
    [ActionTime] = @ActionTime,
    [ActionDescription] = @ActionDescription,
    [ShippedFrom] = @ShippedFrom,
    [ShippedTo] = @ShippedTo,
    [NameAssignedTo] = @NameAssignedTo,
    [TrackingNumber] = @TrackingNumber,
    [ShippingMethod] = @ShippingMethod,
    [ReferenceNum] = @ReferenceNum,
    [Comments] = @Comments,
    [Actor] = @Actor,
    [ActorName] = @ActorName,
    [RefType] = @RefType,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [ModifierName] = @ModifierName
  WHERE ([HistoryID] = @HistoryID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAssetHistoryViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAssetHistoryViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAssetHistoryViewItem

(
  @HistoryID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[AssetHistoryView]
  WHERE ([HistoryID] = @HistoryID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAssetAssignment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAssetAssignment
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAssetAssignment

(
  @AssetAssignmentsID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [AssetAssignmentsID],
    [HistoryID]
  FROM [dbo].[AssetAssignments]
  WHERE ([AssetAssignmentsID] = @AssetAssignmentsID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAssetAssignment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAssetAssignment
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAssetAssignment

(
  @HistoryID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[AssetAssignments]
  (
    [HistoryID])
  VALUES (
    @HistoryID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAssetAssignment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAssetAssignment
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAssetAssignment

(
  @AssetAssignmentsID int,
  @HistoryID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[AssetAssignments]
  SET
    [HistoryID] = @HistoryID
  WHERE ([AssetAssignmentsID] = @AssetAssignmentsID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAssetAssignment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAssetAssignment
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAssetAssignment

(
  @AssetAssignmentsID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[AssetAssignments]
  WHERE ([AssetAssignmentsID] = @AssetAssignmentsID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAssetAssignmentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAssetAssignmentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAssetAssignmentsViewItem

(
  @AssetAssignmentsID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [AssetAssignmentsID],
    [HistoryID],
    [AssetID],
    [OrganizationID],
    [ActionTime],
    [ActionDescription],
    [ShippedFrom],
    [ShippedTo],
    [NameAssignedTo],
    [TrackingNumber],
    [ShippingMethod],
    [ReferenceNum],
    [Comments],
    [DateCreated],
    [Actor],
    [ActorName],
    [RefType],
    [DateModified],
    [ModifierID],
    [ModifierName]
  FROM [dbo].[AssetAssignmentsView]
  WHERE ([AssetAssignmentsID] = @AssetAssignmentsID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAssetAssignmentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAssetAssignmentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAssetAssignmentsViewItem

(
  @AssetAssignmentsID int,
  @HistoryID int,
  @AssetID int,
  @OrganizationID int,
  @ActionTime datetime,
  @ActionDescription varchar(500),
  @ShippedFrom int,
  @ShippedTo int,
  @NameAssignedTo varchar(255),
  @TrackingNumber varchar(200),
  @ShippingMethod varchar(200),
  @ReferenceNum varchar(200),
  @Comments text,
  @DateCreated datetime,
  @Actor int,
  @ActorName varchar(201),
  @RefType int,
  @DateModified datetime,
  @ModifierID int,
  @ModifierName varchar(201),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[AssetAssignmentsView]
  (
    [AssetAssignmentsID],
    [HistoryID],
    [AssetID],
    [OrganizationID],
    [ActionTime],
    [ActionDescription],
    [ShippedFrom],
    [ShippedTo],
    [NameAssignedTo],
    [TrackingNumber],
    [ShippingMethod],
    [ReferenceNum],
    [Comments],
    [DateCreated],
    [Actor],
    [ActorName],
    [RefType],
    [DateModified],
    [ModifierID],
    [ModifierName])
  VALUES (
    @AssetAssignmentsID,
    @HistoryID,
    @AssetID,
    @OrganizationID,
    @ActionTime,
    @ActionDescription,
    @ShippedFrom,
    @ShippedTo,
    @NameAssignedTo,
    @TrackingNumber,
    @ShippingMethod,
    @ReferenceNum,
    @Comments,
    @DateCreated,
    @Actor,
    @ActorName,
    @RefType,
    @DateModified,
    @ModifierID,
    @ModifierName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAssetAssignmentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAssetAssignmentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAssetAssignmentsViewItem

(
  @AssetAssignmentsID int,
  @HistoryID int,
  @AssetID int,
  @OrganizationID int,
  @ActionTime datetime,
  @ActionDescription varchar(500),
  @ShippedFrom int,
  @ShippedTo int,
  @NameAssignedTo varchar(255),
  @TrackingNumber varchar(200),
  @ShippingMethod varchar(200),
  @ReferenceNum varchar(200),
  @Comments text,
  @Actor int,
  @ActorName varchar(201),
  @RefType int,
  @DateModified datetime,
  @ModifierID int,
  @ModifierName varchar(201)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[AssetAssignmentsView]
  SET
    [HistoryID] = @HistoryID,
    [AssetID] = @AssetID,
    [OrganizationID] = @OrganizationID,
    [ActionTime] = @ActionTime,
    [ActionDescription] = @ActionDescription,
    [ShippedFrom] = @ShippedFrom,
    [ShippedTo] = @ShippedTo,
    [NameAssignedTo] = @NameAssignedTo,
    [TrackingNumber] = @TrackingNumber,
    [ShippingMethod] = @ShippingMethod,
    [ReferenceNum] = @ReferenceNum,
    [Comments] = @Comments,
    [Actor] = @Actor,
    [ActorName] = @ActorName,
    [RefType] = @RefType,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [ModifierName] = @ModifierName
  WHERE ([AssetAssignmentsID] = @AssetAssignmentsID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAssetAssignmentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAssetAssignmentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAssetAssignmentsViewItem

(
  @AssetAssignmentsID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[AssetAssignmentsView]
  WHERE ([AssetAssignmentsID] = @AssetAssignmentsID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAssetHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAssetHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAssetHistoryItem

(
  @HistoryID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [HistoryID],
    [AssetID],
    [OrganizationID],
    [ActionTime],
    [ActionDescription],
    [ShippedFrom],
    [ShippedTo],
    [TrackingNumber],
    [ShippingMethod],
    [ReferenceNum],
    [Comments],
    [DateCreated],
    [Actor],
    [RefType],
    [DateModified],
    [ModifierID]
  FROM [dbo].[AssetHistory]
  WHERE ([HistoryID] = @HistoryID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAssetHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAssetHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAssetHistoryItem

(
  @AssetID int,
  @OrganizationID int,
  @ActionTime datetime,
  @ActionDescription varchar(500),
  @ShippedFrom int,
  @ShippedTo int,
  @TrackingNumber varchar(200),
  @ShippingMethod varchar(200),
  @ReferenceNum varchar(200),
  @Comments text,
  @DateCreated datetime,
  @Actor int,
  @RefType int,
  @DateModified datetime,
  @ModifierID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[AssetHistory]
  (
    [AssetID],
    [OrganizationID],
    [ActionTime],
    [ActionDescription],
    [ShippedFrom],
    [ShippedTo],
    [TrackingNumber],
    [ShippingMethod],
    [ReferenceNum],
    [Comments],
    [DateCreated],
    [Actor],
    [RefType],
    [DateModified],
    [ModifierID])
  VALUES (
    @AssetID,
    @OrganizationID,
    @ActionTime,
    @ActionDescription,
    @ShippedFrom,
    @ShippedTo,
    @TrackingNumber,
    @ShippingMethod,
    @ReferenceNum,
    @Comments,
    @DateCreated,
    @Actor,
    @RefType,
    @DateModified,
    @ModifierID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAssetHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAssetHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAssetHistoryItem

(
  @HistoryID int,
  @AssetID int,
  @OrganizationID int,
  @ActionTime datetime,
  @ActionDescription varchar(500),
  @ShippedFrom int,
  @ShippedTo int,
  @TrackingNumber varchar(200),
  @ShippingMethod varchar(200),
  @ReferenceNum varchar(200),
  @Comments text,
  @Actor int,
  @RefType int,
  @DateModified datetime,
  @ModifierID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[AssetHistory]
  SET
    [AssetID] = @AssetID,
    [OrganizationID] = @OrganizationID,
    [ActionTime] = @ActionTime,
    [ActionDescription] = @ActionDescription,
    [ShippedFrom] = @ShippedFrom,
    [ShippedTo] = @ShippedTo,
    [TrackingNumber] = @TrackingNumber,
    [ShippingMethod] = @ShippingMethod,
    [ReferenceNum] = @ReferenceNum,
    [Comments] = @Comments,
    [Actor] = @Actor,
    [RefType] = @RefType,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID
  WHERE ([HistoryID] = @HistoryID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAssetHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAssetHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAssetHistoryItem

(
  @HistoryID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[AssetHistory]
  WHERE ([HistoryID] = @HistoryID)
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
    [ProductVersionID]
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
    [ProductVersionID])
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
    @ProductVersionID)

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
  @ProductVersionID int
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
    [ProductVersionID] = @ProductVersionID
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


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAssetHistoryViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAssetHistoryViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAssetHistoryViewItem

(
  @HistoryID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [HistoryID],
    [AssetID],
    [OrganizationID],
    [ActionTime],
    [ActionDescription],
    [ShippedFrom],
    [ShippedTo],
    [NameAssignedTo],
    [TrackingNumber],
    [ShippingMethod],
    [ReferenceNum],
    [Comments],
    [DateCreated],
    [Actor],
    [ActorName],
    [RefType],
    [DateModified],
    [ModifierID],
    [ModifierName]
  FROM [dbo].[AssetHistoryView]
  WHERE ([HistoryID] = @HistoryID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAssetHistoryViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAssetHistoryViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAssetHistoryViewItem

(
  @HistoryID int,
  @AssetID int,
  @OrganizationID int,
  @ActionTime datetime,
  @ActionDescription varchar(500),
  @ShippedFrom int,
  @ShippedTo int,
  @NameAssignedTo varchar(255),
  @TrackingNumber varchar(200),
  @ShippingMethod varchar(200),
  @ReferenceNum varchar(200),
  @Comments text,
  @DateCreated datetime,
  @Actor int,
  @ActorName varchar(201),
  @RefType int,
  @DateModified datetime,
  @ModifierID int,
  @ModifierName varchar(201),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[AssetHistoryView]
  (
    [HistoryID],
    [AssetID],
    [OrganizationID],
    [ActionTime],
    [ActionDescription],
    [ShippedFrom],
    [ShippedTo],
    [NameAssignedTo],
    [TrackingNumber],
    [ShippingMethod],
    [ReferenceNum],
    [Comments],
    [DateCreated],
    [Actor],
    [ActorName],
    [RefType],
    [DateModified],
    [ModifierID],
    [ModifierName])
  VALUES (
    @HistoryID,
    @AssetID,
    @OrganizationID,
    @ActionTime,
    @ActionDescription,
    @ShippedFrom,
    @ShippedTo,
    @NameAssignedTo,
    @TrackingNumber,
    @ShippingMethod,
    @ReferenceNum,
    @Comments,
    @DateCreated,
    @Actor,
    @ActorName,
    @RefType,
    @DateModified,
    @ModifierID,
    @ModifierName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAssetHistoryViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAssetHistoryViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAssetHistoryViewItem

(
  @HistoryID int,
  @AssetID int,
  @OrganizationID int,
  @ActionTime datetime,
  @ActionDescription varchar(500),
  @ShippedFrom int,
  @ShippedTo int,
  @NameAssignedTo varchar(255),
  @TrackingNumber varchar(200),
  @ShippingMethod varchar(200),
  @ReferenceNum varchar(200),
  @Comments text,
  @Actor int,
  @ActorName varchar(201),
  @RefType int,
  @DateModified datetime,
  @ModifierID int,
  @ModifierName varchar(201)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[AssetHistoryView]
  SET
    [AssetID] = @AssetID,
    [OrganizationID] = @OrganizationID,
    [ActionTime] = @ActionTime,
    [ActionDescription] = @ActionDescription,
    [ShippedFrom] = @ShippedFrom,
    [ShippedTo] = @ShippedTo,
    [NameAssignedTo] = @NameAssignedTo,
    [TrackingNumber] = @TrackingNumber,
    [ShippingMethod] = @ShippingMethod,
    [ReferenceNum] = @ReferenceNum,
    [Comments] = @Comments,
    [Actor] = @Actor,
    [ActorName] = @ActorName,
    [RefType] = @RefType,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [ModifierName] = @ModifierName
  WHERE ([HistoryID] = @HistoryID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAssetHistoryViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAssetHistoryViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAssetHistoryViewItem

(
  @HistoryID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[AssetHistoryView]
  WHERE ([HistoryID] = @HistoryID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAssetAssignment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAssetAssignment
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAssetAssignment

(
  @AssetAssignmentsID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [AssetAssignmentsID],
    [HistoryID]
  FROM [dbo].[AssetAssignments]
  WHERE ([AssetAssignmentsID] = @AssetAssignmentsID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAssetAssignment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAssetAssignment
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAssetAssignment

(
  @HistoryID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[AssetAssignments]
  (
    [HistoryID])
  VALUES (
    @HistoryID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAssetAssignment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAssetAssignment
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAssetAssignment

(
  @AssetAssignmentsID int,
  @HistoryID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[AssetAssignments]
  SET
    [HistoryID] = @HistoryID
  WHERE ([AssetAssignmentsID] = @AssetAssignmentsID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAssetAssignment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAssetAssignment
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAssetAssignment

(
  @AssetAssignmentsID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[AssetAssignments]
  WHERE ([AssetAssignmentsID] = @AssetAssignmentsID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAssetAssignmentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAssetAssignmentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAssetAssignmentsViewItem

(
  @AssetAssignmentsID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [AssetAssignmentsID],
    [HistoryID],
    [AssetID],
    [OrganizationID],
    [ActionTime],
    [ActionDescription],
    [ShippedFrom],
    [ShippedTo],
    [NameAssignedTo],
    [TrackingNumber],
    [ShippingMethod],
    [ReferenceNum],
    [Comments],
    [DateCreated],
    [Actor],
    [ActorName],
    [RefType],
    [DateModified],
    [ModifierID],
    [ModifierName]
  FROM [dbo].[AssetAssignmentsView]
  WHERE ([AssetAssignmentsID] = @AssetAssignmentsID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAssetAssignmentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAssetAssignmentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAssetAssignmentsViewItem

(
  @AssetAssignmentsID int,
  @HistoryID int,
  @AssetID int,
  @OrganizationID int,
  @ActionTime datetime,
  @ActionDescription varchar(500),
  @ShippedFrom int,
  @ShippedTo int,
  @NameAssignedTo varchar(255),
  @TrackingNumber varchar(200),
  @ShippingMethod varchar(200),
  @ReferenceNum varchar(200),
  @Comments text,
  @DateCreated datetime,
  @Actor int,
  @ActorName varchar(201),
  @RefType int,
  @DateModified datetime,
  @ModifierID int,
  @ModifierName varchar(201),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[AssetAssignmentsView]
  (
    [AssetAssignmentsID],
    [HistoryID],
    [AssetID],
    [OrganizationID],
    [ActionTime],
    [ActionDescription],
    [ShippedFrom],
    [ShippedTo],
    [NameAssignedTo],
    [TrackingNumber],
    [ShippingMethod],
    [ReferenceNum],
    [Comments],
    [DateCreated],
    [Actor],
    [ActorName],
    [RefType],
    [DateModified],
    [ModifierID],
    [ModifierName])
  VALUES (
    @AssetAssignmentsID,
    @HistoryID,
    @AssetID,
    @OrganizationID,
    @ActionTime,
    @ActionDescription,
    @ShippedFrom,
    @ShippedTo,
    @NameAssignedTo,
    @TrackingNumber,
    @ShippingMethod,
    @ReferenceNum,
    @Comments,
    @DateCreated,
    @Actor,
    @ActorName,
    @RefType,
    @DateModified,
    @ModifierID,
    @ModifierName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAssetAssignmentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAssetAssignmentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAssetAssignmentsViewItem

(
  @AssetAssignmentsID int,
  @HistoryID int,
  @AssetID int,
  @OrganizationID int,
  @ActionTime datetime,
  @ActionDescription varchar(500),
  @ShippedFrom int,
  @ShippedTo int,
  @NameAssignedTo varchar(255),
  @TrackingNumber varchar(200),
  @ShippingMethod varchar(200),
  @ReferenceNum varchar(200),
  @Comments text,
  @Actor int,
  @ActorName varchar(201),
  @RefType int,
  @DateModified datetime,
  @ModifierID int,
  @ModifierName varchar(201)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[AssetAssignmentsView]
  SET
    [HistoryID] = @HistoryID,
    [AssetID] = @AssetID,
    [OrganizationID] = @OrganizationID,
    [ActionTime] = @ActionTime,
    [ActionDescription] = @ActionDescription,
    [ShippedFrom] = @ShippedFrom,
    [ShippedTo] = @ShippedTo,
    [NameAssignedTo] = @NameAssignedTo,
    [TrackingNumber] = @TrackingNumber,
    [ShippingMethod] = @ShippingMethod,
    [ReferenceNum] = @ReferenceNum,
    [Comments] = @Comments,
    [Actor] = @Actor,
    [ActorName] = @ActorName,
    [RefType] = @RefType,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [ModifierName] = @ModifierName
  WHERE ([AssetAssignmentsID] = @AssetAssignmentsID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAssetAssignmentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAssetAssignmentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAssetAssignmentsViewItem

(
  @AssetAssignmentsID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[AssetAssignmentsView]
  WHERE ([AssetAssignmentsID] = @AssetAssignmentsID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAssetHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAssetHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAssetHistoryItem

(
  @HistoryID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [HistoryID],
    [AssetID],
    [OrganizationID],
    [ActionTime],
    [ActionDescription],
    [ShippedFrom],
    [ShippedTo],
    [TrackingNumber],
    [ShippingMethod],
    [ReferenceNum],
    [Comments],
    [DateCreated],
    [Actor],
    [RefType],
    [DateModified],
    [ModifierID]
  FROM [dbo].[AssetHistory]
  WHERE ([HistoryID] = @HistoryID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAssetHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAssetHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAssetHistoryItem

(
  @AssetID int,
  @OrganizationID int,
  @ActionTime datetime,
  @ActionDescription varchar(500),
  @ShippedFrom int,
  @ShippedTo int,
  @TrackingNumber varchar(200),
  @ShippingMethod varchar(200),
  @ReferenceNum varchar(200),
  @Comments text,
  @DateCreated datetime,
  @Actor int,
  @RefType int,
  @DateModified datetime,
  @ModifierID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[AssetHistory]
  (
    [AssetID],
    [OrganizationID],
    [ActionTime],
    [ActionDescription],
    [ShippedFrom],
    [ShippedTo],
    [TrackingNumber],
    [ShippingMethod],
    [ReferenceNum],
    [Comments],
    [DateCreated],
    [Actor],
    [RefType],
    [DateModified],
    [ModifierID])
  VALUES (
    @AssetID,
    @OrganizationID,
    @ActionTime,
    @ActionDescription,
    @ShippedFrom,
    @ShippedTo,
    @TrackingNumber,
    @ShippingMethod,
    @ReferenceNum,
    @Comments,
    @DateCreated,
    @Actor,
    @RefType,
    @DateModified,
    @ModifierID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAssetHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAssetHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAssetHistoryItem

(
  @HistoryID int,
  @AssetID int,
  @OrganizationID int,
  @ActionTime datetime,
  @ActionDescription varchar(500),
  @ShippedFrom int,
  @ShippedTo int,
  @TrackingNumber varchar(200),
  @ShippingMethod varchar(200),
  @ReferenceNum varchar(200),
  @Comments text,
  @Actor int,
  @RefType int,
  @DateModified datetime,
  @ModifierID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[AssetHistory]
  SET
    [AssetID] = @AssetID,
    [OrganizationID] = @OrganizationID,
    [ActionTime] = @ActionTime,
    [ActionDescription] = @ActionDescription,
    [ShippedFrom] = @ShippedFrom,
    [ShippedTo] = @ShippedTo,
    [TrackingNumber] = @TrackingNumber,
    [ShippingMethod] = @ShippingMethod,
    [ReferenceNum] = @ReferenceNum,
    [Comments] = @Comments,
    [Actor] = @Actor,
    [RefType] = @RefType,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID
  WHERE ([HistoryID] = @HistoryID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAssetHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAssetHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAssetHistoryItem

(
  @HistoryID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[AssetHistory]
  WHERE ([HistoryID] = @HistoryID)
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
    [ProductVersionID]
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
    [ProductVersionID])
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
    @ProductVersionID)

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
  @ProductVersionID int
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
    [ProductVersionID] = @ProductVersionID
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


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAssetHistoryViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAssetHistoryViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAssetHistoryViewItem

(
  @HistoryID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [HistoryID],
    [AssetID],
    [OrganizationID],
    [ActionTime],
    [ActionDescription],
    [ShippedFrom],
    [ShippedTo],
    [NameAssignedTo],
    [TrackingNumber],
    [ShippingMethod],
    [ReferenceNum],
    [Comments],
    [DateCreated],
    [Actor],
    [ActorName],
    [RefType],
    [DateModified],
    [ModifierID],
    [ModifierName]
  FROM [dbo].[AssetHistoryView]
  WHERE ([HistoryID] = @HistoryID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAssetHistoryViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAssetHistoryViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAssetHistoryViewItem

(
  @HistoryID int,
  @AssetID int,
  @OrganizationID int,
  @ActionTime datetime,
  @ActionDescription varchar(500),
  @ShippedFrom int,
  @ShippedTo int,
  @NameAssignedTo varchar(255),
  @TrackingNumber varchar(200),
  @ShippingMethod varchar(200),
  @ReferenceNum varchar(200),
  @Comments text,
  @DateCreated datetime,
  @Actor int,
  @ActorName varchar(201),
  @RefType int,
  @DateModified datetime,
  @ModifierID int,
  @ModifierName varchar(201),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[AssetHistoryView]
  (
    [HistoryID],
    [AssetID],
    [OrganizationID],
    [ActionTime],
    [ActionDescription],
    [ShippedFrom],
    [ShippedTo],
    [NameAssignedTo],
    [TrackingNumber],
    [ShippingMethod],
    [ReferenceNum],
    [Comments],
    [DateCreated],
    [Actor],
    [ActorName],
    [RefType],
    [DateModified],
    [ModifierID],
    [ModifierName])
  VALUES (
    @HistoryID,
    @AssetID,
    @OrganizationID,
    @ActionTime,
    @ActionDescription,
    @ShippedFrom,
    @ShippedTo,
    @NameAssignedTo,
    @TrackingNumber,
    @ShippingMethod,
    @ReferenceNum,
    @Comments,
    @DateCreated,
    @Actor,
    @ActorName,
    @RefType,
    @DateModified,
    @ModifierID,
    @ModifierName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAssetHistoryViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAssetHistoryViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAssetHistoryViewItem

(
  @HistoryID int,
  @AssetID int,
  @OrganizationID int,
  @ActionTime datetime,
  @ActionDescription varchar(500),
  @ShippedFrom int,
  @ShippedTo int,
  @NameAssignedTo varchar(255),
  @TrackingNumber varchar(200),
  @ShippingMethod varchar(200),
  @ReferenceNum varchar(200),
  @Comments text,
  @Actor int,
  @ActorName varchar(201),
  @RefType int,
  @DateModified datetime,
  @ModifierID int,
  @ModifierName varchar(201)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[AssetHistoryView]
  SET
    [AssetID] = @AssetID,
    [OrganizationID] = @OrganizationID,
    [ActionTime] = @ActionTime,
    [ActionDescription] = @ActionDescription,
    [ShippedFrom] = @ShippedFrom,
    [ShippedTo] = @ShippedTo,
    [NameAssignedTo] = @NameAssignedTo,
    [TrackingNumber] = @TrackingNumber,
    [ShippingMethod] = @ShippingMethod,
    [ReferenceNum] = @ReferenceNum,
    [Comments] = @Comments,
    [Actor] = @Actor,
    [ActorName] = @ActorName,
    [RefType] = @RefType,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [ModifierName] = @ModifierName
  WHERE ([HistoryID] = @HistoryID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAssetHistoryViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAssetHistoryViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAssetHistoryViewItem

(
  @HistoryID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[AssetHistoryView]
  WHERE ([HistoryID] = @HistoryID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAssetAssignment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAssetAssignment
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAssetAssignment

(
  @AssetAssignmentsID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [AssetAssignmentsID],
    [HistoryID]
  FROM [dbo].[AssetAssignments]
  WHERE ([AssetAssignmentsID] = @AssetAssignmentsID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAssetAssignment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAssetAssignment
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAssetAssignment

(
  @HistoryID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[AssetAssignments]
  (
    [HistoryID])
  VALUES (
    @HistoryID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAssetAssignment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAssetAssignment
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAssetAssignment

(
  @AssetAssignmentsID int,
  @HistoryID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[AssetAssignments]
  SET
    [HistoryID] = @HistoryID
  WHERE ([AssetAssignmentsID] = @AssetAssignmentsID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAssetAssignment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAssetAssignment
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAssetAssignment

(
  @AssetAssignmentsID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[AssetAssignments]
  WHERE ([AssetAssignmentsID] = @AssetAssignmentsID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAssetAssignmentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAssetAssignmentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAssetAssignmentsViewItem

(
  @AssetAssignmentsID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [AssetAssignmentsID],
    [HistoryID],
    [AssetID],
    [OrganizationID],
    [ActionTime],
    [ActionDescription],
    [ShippedFrom],
    [ShippedTo],
    [NameAssignedTo],
    [TrackingNumber],
    [ShippingMethod],
    [ReferenceNum],
    [Comments],
    [DateCreated],
    [Actor],
    [ActorName],
    [RefType],
    [DateModified],
    [ModifierID],
    [ModifierName]
  FROM [dbo].[AssetAssignmentsView]
  WHERE ([AssetAssignmentsID] = @AssetAssignmentsID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAssetAssignmentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAssetAssignmentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAssetAssignmentsViewItem

(
  @AssetAssignmentsID int,
  @HistoryID int,
  @AssetID int,
  @OrganizationID int,
  @ActionTime datetime,
  @ActionDescription varchar(500),
  @ShippedFrom int,
  @ShippedTo int,
  @NameAssignedTo varchar(255),
  @TrackingNumber varchar(200),
  @ShippingMethod varchar(200),
  @ReferenceNum varchar(200),
  @Comments text,
  @DateCreated datetime,
  @Actor int,
  @ActorName varchar(201),
  @RefType int,
  @DateModified datetime,
  @ModifierID int,
  @ModifierName varchar(201),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[AssetAssignmentsView]
  (
    [AssetAssignmentsID],
    [HistoryID],
    [AssetID],
    [OrganizationID],
    [ActionTime],
    [ActionDescription],
    [ShippedFrom],
    [ShippedTo],
    [NameAssignedTo],
    [TrackingNumber],
    [ShippingMethod],
    [ReferenceNum],
    [Comments],
    [DateCreated],
    [Actor],
    [ActorName],
    [RefType],
    [DateModified],
    [ModifierID],
    [ModifierName])
  VALUES (
    @AssetAssignmentsID,
    @HistoryID,
    @AssetID,
    @OrganizationID,
    @ActionTime,
    @ActionDescription,
    @ShippedFrom,
    @ShippedTo,
    @NameAssignedTo,
    @TrackingNumber,
    @ShippingMethod,
    @ReferenceNum,
    @Comments,
    @DateCreated,
    @Actor,
    @ActorName,
    @RefType,
    @DateModified,
    @ModifierID,
    @ModifierName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAssetAssignmentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAssetAssignmentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAssetAssignmentsViewItem

(
  @AssetAssignmentsID int,
  @HistoryID int,
  @AssetID int,
  @OrganizationID int,
  @ActionTime datetime,
  @ActionDescription varchar(500),
  @ShippedFrom int,
  @ShippedTo int,
  @NameAssignedTo varchar(255),
  @TrackingNumber varchar(200),
  @ShippingMethod varchar(200),
  @ReferenceNum varchar(200),
  @Comments text,
  @Actor int,
  @ActorName varchar(201),
  @RefType int,
  @DateModified datetime,
  @ModifierID int,
  @ModifierName varchar(201)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[AssetAssignmentsView]
  SET
    [HistoryID] = @HistoryID,
    [AssetID] = @AssetID,
    [OrganizationID] = @OrganizationID,
    [ActionTime] = @ActionTime,
    [ActionDescription] = @ActionDescription,
    [ShippedFrom] = @ShippedFrom,
    [ShippedTo] = @ShippedTo,
    [NameAssignedTo] = @NameAssignedTo,
    [TrackingNumber] = @TrackingNumber,
    [ShippingMethod] = @ShippingMethod,
    [ReferenceNum] = @ReferenceNum,
    [Comments] = @Comments,
    [Actor] = @Actor,
    [ActorName] = @ActorName,
    [RefType] = @RefType,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [ModifierName] = @ModifierName
  WHERE ([AssetAssignmentsID] = @AssetAssignmentsID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAssetAssignmentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAssetAssignmentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAssetAssignmentsViewItem

(
  @AssetAssignmentsID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[AssetAssignmentsView]
  WHERE ([AssetAssignmentsID] = @AssetAssignmentsID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAssetHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAssetHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAssetHistoryItem

(
  @HistoryID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [HistoryID],
    [AssetID],
    [OrganizationID],
    [ActionTime],
    [ActionDescription],
    [ShippedFrom],
    [ShippedTo],
    [TrackingNumber],
    [ShippingMethod],
    [ReferenceNum],
    [Comments],
    [DateCreated],
    [Actor],
    [RefType],
    [DateModified],
    [ModifierID]
  FROM [dbo].[AssetHistory]
  WHERE ([HistoryID] = @HistoryID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAssetHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAssetHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAssetHistoryItem

(
  @AssetID int,
  @OrganizationID int,
  @ActionTime datetime,
  @ActionDescription varchar(500),
  @ShippedFrom int,
  @ShippedTo int,
  @TrackingNumber varchar(200),
  @ShippingMethod varchar(200),
  @ReferenceNum varchar(200),
  @Comments text,
  @DateCreated datetime,
  @Actor int,
  @RefType int,
  @DateModified datetime,
  @ModifierID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[AssetHistory]
  (
    [AssetID],
    [OrganizationID],
    [ActionTime],
    [ActionDescription],
    [ShippedFrom],
    [ShippedTo],
    [TrackingNumber],
    [ShippingMethod],
    [ReferenceNum],
    [Comments],
    [DateCreated],
    [Actor],
    [RefType],
    [DateModified],
    [ModifierID])
  VALUES (
    @AssetID,
    @OrganizationID,
    @ActionTime,
    @ActionDescription,
    @ShippedFrom,
    @ShippedTo,
    @TrackingNumber,
    @ShippingMethod,
    @ReferenceNum,
    @Comments,
    @DateCreated,
    @Actor,
    @RefType,
    @DateModified,
    @ModifierID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAssetHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAssetHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAssetHistoryItem

(
  @HistoryID int,
  @AssetID int,
  @OrganizationID int,
  @ActionTime datetime,
  @ActionDescription varchar(500),
  @ShippedFrom int,
  @ShippedTo int,
  @TrackingNumber varchar(200),
  @ShippingMethod varchar(200),
  @ReferenceNum varchar(200),
  @Comments text,
  @Actor int,
  @RefType int,
  @DateModified datetime,
  @ModifierID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[AssetHistory]
  SET
    [AssetID] = @AssetID,
    [OrganizationID] = @OrganizationID,
    [ActionTime] = @ActionTime,
    [ActionDescription] = @ActionDescription,
    [ShippedFrom] = @ShippedFrom,
    [ShippedTo] = @ShippedTo,
    [TrackingNumber] = @TrackingNumber,
    [ShippingMethod] = @ShippingMethod,
    [ReferenceNum] = @ReferenceNum,
    [Comments] = @Comments,
    [Actor] = @Actor,
    [RefType] = @RefType,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID
  WHERE ([HistoryID] = @HistoryID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAssetHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAssetHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAssetHistoryItem

(
  @HistoryID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[AssetHistory]
  WHERE ([HistoryID] = @HistoryID)
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
    [ProductVersionID]
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
    [ProductVersionID])
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
    @ProductVersionID)

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
  @ProductVersionID int
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
    [ProductVersionID] = @ProductVersionID
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


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAssetHistoryViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAssetHistoryViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAssetHistoryViewItem

(
  @HistoryID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [HistoryID],
    [AssetID],
    [OrganizationID],
    [ActionTime],
    [ActionDescription],
    [ShippedFrom],
    [ShippedTo],
    [NameAssignedTo],
    [TrackingNumber],
    [ShippingMethod],
    [ReferenceNum],
    [Comments],
    [DateCreated],
    [Actor],
    [ActorName],
    [RefType],
    [DateModified],
    [ModifierID],
    [ModifierName]
  FROM [dbo].[AssetHistoryView]
  WHERE ([HistoryID] = @HistoryID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAssetHistoryViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAssetHistoryViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAssetHistoryViewItem

(
  @HistoryID int,
  @AssetID int,
  @OrganizationID int,
  @ActionTime datetime,
  @ActionDescription varchar(500),
  @ShippedFrom int,
  @ShippedTo int,
  @NameAssignedTo varchar(255),
  @TrackingNumber varchar(200),
  @ShippingMethod varchar(200),
  @ReferenceNum varchar(200),
  @Comments text,
  @DateCreated datetime,
  @Actor int,
  @ActorName varchar(201),
  @RefType int,
  @DateModified datetime,
  @ModifierID int,
  @ModifierName varchar(201),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[AssetHistoryView]
  (
    [HistoryID],
    [AssetID],
    [OrganizationID],
    [ActionTime],
    [ActionDescription],
    [ShippedFrom],
    [ShippedTo],
    [NameAssignedTo],
    [TrackingNumber],
    [ShippingMethod],
    [ReferenceNum],
    [Comments],
    [DateCreated],
    [Actor],
    [ActorName],
    [RefType],
    [DateModified],
    [ModifierID],
    [ModifierName])
  VALUES (
    @HistoryID,
    @AssetID,
    @OrganizationID,
    @ActionTime,
    @ActionDescription,
    @ShippedFrom,
    @ShippedTo,
    @NameAssignedTo,
    @TrackingNumber,
    @ShippingMethod,
    @ReferenceNum,
    @Comments,
    @DateCreated,
    @Actor,
    @ActorName,
    @RefType,
    @DateModified,
    @ModifierID,
    @ModifierName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAssetHistoryViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAssetHistoryViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAssetHistoryViewItem

(
  @HistoryID int,
  @AssetID int,
  @OrganizationID int,
  @ActionTime datetime,
  @ActionDescription varchar(500),
  @ShippedFrom int,
  @ShippedTo int,
  @NameAssignedTo varchar(255),
  @TrackingNumber varchar(200),
  @ShippingMethod varchar(200),
  @ReferenceNum varchar(200),
  @Comments text,
  @Actor int,
  @ActorName varchar(201),
  @RefType int,
  @DateModified datetime,
  @ModifierID int,
  @ModifierName varchar(201)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[AssetHistoryView]
  SET
    [AssetID] = @AssetID,
    [OrganizationID] = @OrganizationID,
    [ActionTime] = @ActionTime,
    [ActionDescription] = @ActionDescription,
    [ShippedFrom] = @ShippedFrom,
    [ShippedTo] = @ShippedTo,
    [NameAssignedTo] = @NameAssignedTo,
    [TrackingNumber] = @TrackingNumber,
    [ShippingMethod] = @ShippingMethod,
    [ReferenceNum] = @ReferenceNum,
    [Comments] = @Comments,
    [Actor] = @Actor,
    [ActorName] = @ActorName,
    [RefType] = @RefType,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [ModifierName] = @ModifierName
  WHERE ([HistoryID] = @HistoryID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAssetHistoryViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAssetHistoryViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAssetHistoryViewItem

(
  @HistoryID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[AssetHistoryView]
  WHERE ([HistoryID] = @HistoryID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAssetAssignment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAssetAssignment
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAssetAssignment

(
  @AssetAssignmentsID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [AssetAssignmentsID],
    [HistoryID]
  FROM [dbo].[AssetAssignments]
  WHERE ([AssetAssignmentsID] = @AssetAssignmentsID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAssetAssignment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAssetAssignment
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAssetAssignment

(
  @HistoryID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[AssetAssignments]
  (
    [HistoryID])
  VALUES (
    @HistoryID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAssetAssignment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAssetAssignment
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAssetAssignment

(
  @AssetAssignmentsID int,
  @HistoryID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[AssetAssignments]
  SET
    [HistoryID] = @HistoryID
  WHERE ([AssetAssignmentsID] = @AssetAssignmentsID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAssetAssignment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAssetAssignment
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAssetAssignment

(
  @AssetAssignmentsID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[AssetAssignments]
  WHERE ([AssetAssignmentsID] = @AssetAssignmentsID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAssetAssignmentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAssetAssignmentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAssetAssignmentsViewItem

(
  @AssetAssignmentsID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [AssetAssignmentsID],
    [HistoryID],
    [AssetID],
    [OrganizationID],
    [ActionTime],
    [ActionDescription],
    [ShippedFrom],
    [ShippedTo],
    [NameAssignedTo],
    [TrackingNumber],
    [ShippingMethod],
    [ReferenceNum],
    [Comments],
    [DateCreated],
    [Actor],
    [ActorName],
    [RefType],
    [DateModified],
    [ModifierID],
    [ModifierName]
  FROM [dbo].[AssetAssignmentsView]
  WHERE ([AssetAssignmentsID] = @AssetAssignmentsID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAssetAssignmentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAssetAssignmentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAssetAssignmentsViewItem

(
  @AssetAssignmentsID int,
  @HistoryID int,
  @AssetID int,
  @OrganizationID int,
  @ActionTime datetime,
  @ActionDescription varchar(500),
  @ShippedFrom int,
  @ShippedTo int,
  @NameAssignedTo varchar(255),
  @TrackingNumber varchar(200),
  @ShippingMethod varchar(200),
  @ReferenceNum varchar(200),
  @Comments text,
  @DateCreated datetime,
  @Actor int,
  @ActorName varchar(201),
  @RefType int,
  @DateModified datetime,
  @ModifierID int,
  @ModifierName varchar(201),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[AssetAssignmentsView]
  (
    [AssetAssignmentsID],
    [HistoryID],
    [AssetID],
    [OrganizationID],
    [ActionTime],
    [ActionDescription],
    [ShippedFrom],
    [ShippedTo],
    [NameAssignedTo],
    [TrackingNumber],
    [ShippingMethod],
    [ReferenceNum],
    [Comments],
    [DateCreated],
    [Actor],
    [ActorName],
    [RefType],
    [DateModified],
    [ModifierID],
    [ModifierName])
  VALUES (
    @AssetAssignmentsID,
    @HistoryID,
    @AssetID,
    @OrganizationID,
    @ActionTime,
    @ActionDescription,
    @ShippedFrom,
    @ShippedTo,
    @NameAssignedTo,
    @TrackingNumber,
    @ShippingMethod,
    @ReferenceNum,
    @Comments,
    @DateCreated,
    @Actor,
    @ActorName,
    @RefType,
    @DateModified,
    @ModifierID,
    @ModifierName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAssetAssignmentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAssetAssignmentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAssetAssignmentsViewItem

(
  @AssetAssignmentsID int,
  @HistoryID int,
  @AssetID int,
  @OrganizationID int,
  @ActionTime datetime,
  @ActionDescription varchar(500),
  @ShippedFrom int,
  @ShippedTo int,
  @NameAssignedTo varchar(255),
  @TrackingNumber varchar(200),
  @ShippingMethod varchar(200),
  @ReferenceNum varchar(200),
  @Comments text,
  @Actor int,
  @ActorName varchar(201),
  @RefType int,
  @DateModified datetime,
  @ModifierID int,
  @ModifierName varchar(201)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[AssetAssignmentsView]
  SET
    [HistoryID] = @HistoryID,
    [AssetID] = @AssetID,
    [OrganizationID] = @OrganizationID,
    [ActionTime] = @ActionTime,
    [ActionDescription] = @ActionDescription,
    [ShippedFrom] = @ShippedFrom,
    [ShippedTo] = @ShippedTo,
    [NameAssignedTo] = @NameAssignedTo,
    [TrackingNumber] = @TrackingNumber,
    [ShippingMethod] = @ShippingMethod,
    [ReferenceNum] = @ReferenceNum,
    [Comments] = @Comments,
    [Actor] = @Actor,
    [ActorName] = @ActorName,
    [RefType] = @RefType,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [ModifierName] = @ModifierName
  WHERE ([AssetAssignmentsID] = @AssetAssignmentsID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAssetAssignmentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAssetAssignmentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAssetAssignmentsViewItem

(
  @AssetAssignmentsID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[AssetAssignmentsView]
  WHERE ([AssetAssignmentsID] = @AssetAssignmentsID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAssetHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAssetHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAssetHistoryItem

(
  @HistoryID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [HistoryID],
    [AssetID],
    [OrganizationID],
    [ActionTime],
    [ActionDescription],
    [ShippedFrom],
    [ShippedTo],
    [TrackingNumber],
    [ShippingMethod],
    [ReferenceNum],
    [Comments],
    [DateCreated],
    [Actor],
    [RefType],
    [DateModified],
    [ModifierID]
  FROM [dbo].[AssetHistory]
  WHERE ([HistoryID] = @HistoryID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAssetHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAssetHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAssetHistoryItem

(
  @AssetID int,
  @OrganizationID int,
  @ActionTime datetime,
  @ActionDescription varchar(500),
  @ShippedFrom int,
  @ShippedTo int,
  @TrackingNumber varchar(200),
  @ShippingMethod varchar(200),
  @ReferenceNum varchar(200),
  @Comments text,
  @DateCreated datetime,
  @Actor int,
  @RefType int,
  @DateModified datetime,
  @ModifierID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[AssetHistory]
  (
    [AssetID],
    [OrganizationID],
    [ActionTime],
    [ActionDescription],
    [ShippedFrom],
    [ShippedTo],
    [TrackingNumber],
    [ShippingMethod],
    [ReferenceNum],
    [Comments],
    [DateCreated],
    [Actor],
    [RefType],
    [DateModified],
    [ModifierID])
  VALUES (
    @AssetID,
    @OrganizationID,
    @ActionTime,
    @ActionDescription,
    @ShippedFrom,
    @ShippedTo,
    @TrackingNumber,
    @ShippingMethod,
    @ReferenceNum,
    @Comments,
    @DateCreated,
    @Actor,
    @RefType,
    @DateModified,
    @ModifierID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAssetHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAssetHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAssetHistoryItem

(
  @HistoryID int,
  @AssetID int,
  @OrganizationID int,
  @ActionTime datetime,
  @ActionDescription varchar(500),
  @ShippedFrom int,
  @ShippedTo int,
  @TrackingNumber varchar(200),
  @ShippingMethod varchar(200),
  @ReferenceNum varchar(200),
  @Comments text,
  @Actor int,
  @RefType int,
  @DateModified datetime,
  @ModifierID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[AssetHistory]
  SET
    [AssetID] = @AssetID,
    [OrganizationID] = @OrganizationID,
    [ActionTime] = @ActionTime,
    [ActionDescription] = @ActionDescription,
    [ShippedFrom] = @ShippedFrom,
    [ShippedTo] = @ShippedTo,
    [TrackingNumber] = @TrackingNumber,
    [ShippingMethod] = @ShippingMethod,
    [ReferenceNum] = @ReferenceNum,
    [Comments] = @Comments,
    [Actor] = @Actor,
    [RefType] = @RefType,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID
  WHERE ([HistoryID] = @HistoryID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAssetHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAssetHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAssetHistoryItem

(
  @HistoryID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[AssetHistory]
  WHERE ([HistoryID] = @HistoryID)
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
    [ProductVersionID]
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
    [ProductVersionID])
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
    @ProductVersionID)

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
  @ProductVersionID int
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
    [ProductVersionID] = @ProductVersionID
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


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAssetHistoryViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAssetHistoryViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAssetHistoryViewItem

(
  @HistoryID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [HistoryID],
    [AssetID],
    [OrganizationID],
    [ActionTime],
    [ActionDescription],
    [ShippedFrom],
    [ShippedTo],
    [NameAssignedTo],
    [TrackingNumber],
    [ShippingMethod],
    [ReferenceNum],
    [Comments],
    [DateCreated],
    [Actor],
    [ActorName],
    [RefType],
    [DateModified],
    [ModifierID],
    [ModifierName]
  FROM [dbo].[AssetHistoryView]
  WHERE ([HistoryID] = @HistoryID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAssetHistoryViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAssetHistoryViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAssetHistoryViewItem

(
  @HistoryID int,
  @AssetID int,
  @OrganizationID int,
  @ActionTime datetime,
  @ActionDescription varchar(500),
  @ShippedFrom int,
  @ShippedTo int,
  @NameAssignedTo varchar(255),
  @TrackingNumber varchar(200),
  @ShippingMethod varchar(200),
  @ReferenceNum varchar(200),
  @Comments text,
  @DateCreated datetime,
  @Actor int,
  @ActorName varchar(201),
  @RefType int,
  @DateModified datetime,
  @ModifierID int,
  @ModifierName varchar(201),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[AssetHistoryView]
  (
    [HistoryID],
    [AssetID],
    [OrganizationID],
    [ActionTime],
    [ActionDescription],
    [ShippedFrom],
    [ShippedTo],
    [NameAssignedTo],
    [TrackingNumber],
    [ShippingMethod],
    [ReferenceNum],
    [Comments],
    [DateCreated],
    [Actor],
    [ActorName],
    [RefType],
    [DateModified],
    [ModifierID],
    [ModifierName])
  VALUES (
    @HistoryID,
    @AssetID,
    @OrganizationID,
    @ActionTime,
    @ActionDescription,
    @ShippedFrom,
    @ShippedTo,
    @NameAssignedTo,
    @TrackingNumber,
    @ShippingMethod,
    @ReferenceNum,
    @Comments,
    @DateCreated,
    @Actor,
    @ActorName,
    @RefType,
    @DateModified,
    @ModifierID,
    @ModifierName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAssetHistoryViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAssetHistoryViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAssetHistoryViewItem

(
  @HistoryID int,
  @AssetID int,
  @OrganizationID int,
  @ActionTime datetime,
  @ActionDescription varchar(500),
  @ShippedFrom int,
  @ShippedTo int,
  @NameAssignedTo varchar(255),
  @TrackingNumber varchar(200),
  @ShippingMethod varchar(200),
  @ReferenceNum varchar(200),
  @Comments text,
  @Actor int,
  @ActorName varchar(201),
  @RefType int,
  @DateModified datetime,
  @ModifierID int,
  @ModifierName varchar(201)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[AssetHistoryView]
  SET
    [AssetID] = @AssetID,
    [OrganizationID] = @OrganizationID,
    [ActionTime] = @ActionTime,
    [ActionDescription] = @ActionDescription,
    [ShippedFrom] = @ShippedFrom,
    [ShippedTo] = @ShippedTo,
    [NameAssignedTo] = @NameAssignedTo,
    [TrackingNumber] = @TrackingNumber,
    [ShippingMethod] = @ShippingMethod,
    [ReferenceNum] = @ReferenceNum,
    [Comments] = @Comments,
    [Actor] = @Actor,
    [ActorName] = @ActorName,
    [RefType] = @RefType,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [ModifierName] = @ModifierName
  WHERE ([HistoryID] = @HistoryID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAssetHistoryViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAssetHistoryViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAssetHistoryViewItem

(
  @HistoryID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[AssetHistoryView]
  WHERE ([HistoryID] = @HistoryID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAssetAssignment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAssetAssignment
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAssetAssignment

(
  @AssetAssignmentsID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [AssetAssignmentsID],
    [HistoryID]
  FROM [dbo].[AssetAssignments]
  WHERE ([AssetAssignmentsID] = @AssetAssignmentsID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAssetAssignment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAssetAssignment
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAssetAssignment

(
  @HistoryID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[AssetAssignments]
  (
    [HistoryID])
  VALUES (
    @HistoryID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAssetAssignment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAssetAssignment
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAssetAssignment

(
  @AssetAssignmentsID int,
  @HistoryID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[AssetAssignments]
  SET
    [HistoryID] = @HistoryID
  WHERE ([AssetAssignmentsID] = @AssetAssignmentsID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAssetAssignment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAssetAssignment
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAssetAssignment

(
  @AssetAssignmentsID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[AssetAssignments]
  WHERE ([AssetAssignmentsID] = @AssetAssignmentsID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAssetAssignmentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAssetAssignmentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAssetAssignmentsViewItem

(
  @AssetAssignmentsID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [AssetAssignmentsID],
    [HistoryID],
    [AssetID],
    [OrganizationID],
    [ActionTime],
    [ActionDescription],
    [ShippedFrom],
    [ShippedTo],
    [NameAssignedTo],
    [TrackingNumber],
    [ShippingMethod],
    [ReferenceNum],
    [Comments],
    [DateCreated],
    [Actor],
    [ActorName],
    [RefType],
    [DateModified],
    [ModifierID],
    [ModifierName]
  FROM [dbo].[AssetAssignmentsView]
  WHERE ([AssetAssignmentsID] = @AssetAssignmentsID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAssetAssignmentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAssetAssignmentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAssetAssignmentsViewItem

(
  @AssetAssignmentsID int,
  @HistoryID int,
  @AssetID int,
  @OrganizationID int,
  @ActionTime datetime,
  @ActionDescription varchar(500),
  @ShippedFrom int,
  @ShippedTo int,
  @NameAssignedTo varchar(255),
  @TrackingNumber varchar(200),
  @ShippingMethod varchar(200),
  @ReferenceNum varchar(200),
  @Comments text,
  @DateCreated datetime,
  @Actor int,
  @ActorName varchar(201),
  @RefType int,
  @DateModified datetime,
  @ModifierID int,
  @ModifierName varchar(201),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[AssetAssignmentsView]
  (
    [AssetAssignmentsID],
    [HistoryID],
    [AssetID],
    [OrganizationID],
    [ActionTime],
    [ActionDescription],
    [ShippedFrom],
    [ShippedTo],
    [NameAssignedTo],
    [TrackingNumber],
    [ShippingMethod],
    [ReferenceNum],
    [Comments],
    [DateCreated],
    [Actor],
    [ActorName],
    [RefType],
    [DateModified],
    [ModifierID],
    [ModifierName])
  VALUES (
    @AssetAssignmentsID,
    @HistoryID,
    @AssetID,
    @OrganizationID,
    @ActionTime,
    @ActionDescription,
    @ShippedFrom,
    @ShippedTo,
    @NameAssignedTo,
    @TrackingNumber,
    @ShippingMethod,
    @ReferenceNum,
    @Comments,
    @DateCreated,
    @Actor,
    @ActorName,
    @RefType,
    @DateModified,
    @ModifierID,
    @ModifierName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAssetAssignmentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAssetAssignmentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAssetAssignmentsViewItem

(
  @AssetAssignmentsID int,
  @HistoryID int,
  @AssetID int,
  @OrganizationID int,
  @ActionTime datetime,
  @ActionDescription varchar(500),
  @ShippedFrom int,
  @ShippedTo int,
  @NameAssignedTo varchar(255),
  @TrackingNumber varchar(200),
  @ShippingMethod varchar(200),
  @ReferenceNum varchar(200),
  @Comments text,
  @Actor int,
  @ActorName varchar(201),
  @RefType int,
  @DateModified datetime,
  @ModifierID int,
  @ModifierName varchar(201)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[AssetAssignmentsView]
  SET
    [HistoryID] = @HistoryID,
    [AssetID] = @AssetID,
    [OrganizationID] = @OrganizationID,
    [ActionTime] = @ActionTime,
    [ActionDescription] = @ActionDescription,
    [ShippedFrom] = @ShippedFrom,
    [ShippedTo] = @ShippedTo,
    [NameAssignedTo] = @NameAssignedTo,
    [TrackingNumber] = @TrackingNumber,
    [ShippingMethod] = @ShippingMethod,
    [ReferenceNum] = @ReferenceNum,
    [Comments] = @Comments,
    [Actor] = @Actor,
    [ActorName] = @ActorName,
    [RefType] = @RefType,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [ModifierName] = @ModifierName
  WHERE ([AssetAssignmentsID] = @AssetAssignmentsID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAssetAssignmentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAssetAssignmentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAssetAssignmentsViewItem

(
  @AssetAssignmentsID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[AssetAssignmentsView]
  WHERE ([AssetAssignmentsID] = @AssetAssignmentsID)
GO


