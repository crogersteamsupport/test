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
    [NameAssignedFrom],
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
    [ModifierName],
    [ShippedFromRefType]
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
  @NameAssignedFrom varchar(255),
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
  @ShippedFromRefType int,
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
    [NameAssignedFrom],
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
    [ModifierName],
    [ShippedFromRefType])
  VALUES (
    @AssetAssignmentsID,
    @HistoryID,
    @AssetID,
    @OrganizationID,
    @ActionTime,
    @ActionDescription,
    @ShippedFrom,
    @NameAssignedFrom,
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
    @ModifierName,
    @ShippedFromRefType)

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
  @NameAssignedFrom varchar(255),
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
  @ModifierName varchar(201),
  @ShippedFromRefType int
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
    [NameAssignedFrom] = @NameAssignedFrom,
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
    [ModifierName] = @ModifierName,
    [ShippedFromRefType] = @ShippedFromRefType
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
    [NameAssignedFrom],
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
    [ModifierName],
    [ShippedFromRefType]
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
  @NameAssignedFrom varchar(255),
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
  @ShippedFromRefType int,
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
    [NameAssignedFrom],
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
    [ModifierName],
    [ShippedFromRefType])
  VALUES (
    @AssetAssignmentsID,
    @HistoryID,
    @AssetID,
    @OrganizationID,
    @ActionTime,
    @ActionDescription,
    @ShippedFrom,
    @NameAssignedFrom,
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
    @ModifierName,
    @ShippedFromRefType)

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
  @NameAssignedFrom varchar(255),
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
  @ModifierName varchar(201),
  @ShippedFromRefType int
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
    [NameAssignedFrom] = @NameAssignedFrom,
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
    [ModifierName] = @ModifierName,
    [ShippedFromRefType] = @ShippedFromRefType
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
    [NameAssignedFrom],
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
    [ModifierName],
    [ShippedFromRefType]
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
  @NameAssignedFrom varchar(255),
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
  @ShippedFromRefType int,
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
    [NameAssignedFrom],
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
    [ModifierName],
    [ShippedFromRefType])
  VALUES (
    @AssetAssignmentsID,
    @HistoryID,
    @AssetID,
    @OrganizationID,
    @ActionTime,
    @ActionDescription,
    @ShippedFrom,
    @NameAssignedFrom,
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
    @ModifierName,
    @ShippedFromRefType)

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
  @NameAssignedFrom varchar(255),
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
  @ModifierName varchar(201),
  @ShippedFromRefType int
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
    [NameAssignedFrom] = @NameAssignedFrom,
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
    [ModifierName] = @ModifierName,
    [ShippedFromRefType] = @ShippedFromRefType
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
    [NameAssignedFrom],
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
    [ModifierName],
    [ShippedFromRefType]
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
  @NameAssignedFrom varchar(255),
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
  @ShippedFromRefType int,
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
    [NameAssignedFrom],
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
    [ModifierName],
    [ShippedFromRefType])
  VALUES (
    @AssetAssignmentsID,
    @HistoryID,
    @AssetID,
    @OrganizationID,
    @ActionTime,
    @ActionDescription,
    @ShippedFrom,
    @NameAssignedFrom,
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
    @ModifierName,
    @ShippedFromRefType)

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
  @NameAssignedFrom varchar(255),
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
  @ModifierName varchar(201),
  @ShippedFromRefType int
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
    [NameAssignedFrom] = @NameAssignedFrom,
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
    [ModifierName] = @ModifierName,
    [ShippedFromRefType] = @ShippedFromRefType
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
    [NameAssignedFrom],
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
    [ModifierName],
    [ShippedFromRefType]
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
  @NameAssignedFrom varchar(255),
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
  @ShippedFromRefType int,
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
    [NameAssignedFrom],
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
    [ModifierName],
    [ShippedFromRefType])
  VALUES (
    @AssetAssignmentsID,
    @HistoryID,
    @AssetID,
    @OrganizationID,
    @ActionTime,
    @ActionDescription,
    @ShippedFrom,
    @NameAssignedFrom,
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
    @ModifierName,
    @ShippedFromRefType)

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
  @NameAssignedFrom varchar(255),
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
  @ModifierName varchar(201),
  @ShippedFromRefType int
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
    [NameAssignedFrom] = @NameAssignedFrom,
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
    [ModifierName] = @ModifierName,
    [ShippedFromRefType] = @ShippedFromRefType
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


