IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCustomFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCustomFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCustomFieldsViewItem

(

)
AS
  SET NOCOUNT OFF;
  SELECT
    [CustomFieldID],
    [OrganizationID],
    [Name],
    [ApiFieldName],
    [RefType],
    [FieldType],
    [AuxID],
    [Position],
    [ListValues],
    [Description],
    [IsVisibleOnPortal],
    [IsFirstIndexSelect],
    [IsRequired],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [CustomFieldCategoryID],
    [IsRequiredToClose],
    [Mask],
    [ParentCustomFieldID],
    [ParentCustomValue],
    [ParentProductID],
    [ParentFieldName],
    [ParentProductName]
  FROM [dbo].[CustomFieldsView]
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCustomFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCustomFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCustomFieldsViewItem

(
  @CustomFieldID int,
  @OrganizationID int,
  @Name varchar(50),
  @ApiFieldName varchar(100),
  @RefType int,
  @FieldType int,
  @AuxID int,
  @Position int,
  @ListValues varchar(MAX),
  @Description varchar(250),
  @IsVisibleOnPortal bit,
  @IsFirstIndexSelect bit,
  @IsRequired bit,
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @CustomFieldCategoryID int,
  @IsRequiredToClose bit,
  @Mask varchar(MAX),
  @ParentCustomFieldID int,
  @ParentCustomValue varchar(MAX),
  @ParentProductID int,
  @ParentFieldName varchar(50),
  @ParentProductName varchar(255),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CustomFieldsView]
  (
    [CustomFieldID],
    [OrganizationID],
    [Name],
    [ApiFieldName],
    [RefType],
    [FieldType],
    [AuxID],
    [Position],
    [ListValues],
    [Description],
    [IsVisibleOnPortal],
    [IsFirstIndexSelect],
    [IsRequired],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [CustomFieldCategoryID],
    [IsRequiredToClose],
    [Mask],
    [ParentCustomFieldID],
    [ParentCustomValue],
    [ParentProductID],
    [ParentFieldName],
    [ParentProductName])
  VALUES (
    @CustomFieldID,
    @OrganizationID,
    @Name,
    @ApiFieldName,
    @RefType,
    @FieldType,
    @AuxID,
    @Position,
    @ListValues,
    @Description,
    @IsVisibleOnPortal,
    @IsFirstIndexSelect,
    @IsRequired,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @CustomFieldCategoryID,
    @IsRequiredToClose,
    @Mask,
    @ParentCustomFieldID,
    @ParentCustomValue,
    @ParentProductID,
    @ParentFieldName,
    @ParentProductName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCustomFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCustomFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCustomFieldsViewItem

(
  @CustomFieldID int,
  @OrganizationID int,
  @Name varchar(50),
  @ApiFieldName varchar(100),
  @RefType int,
  @FieldType int,
  @AuxID int,
  @Position int,
  @ListValues varchar(MAX),
  @Description varchar(250),
  @IsVisibleOnPortal bit,
  @IsFirstIndexSelect bit,
  @IsRequired bit,
  @DateModified datetime,
  @ModifierID int,
  @CustomFieldCategoryID int,
  @IsRequiredToClose bit,
  @Mask varchar(MAX),
  @ParentCustomFieldID int,
  @ParentCustomValue varchar(MAX),
  @ParentProductID int,
  @ParentFieldName varchar(50),
  @ParentProductName varchar(255)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CustomFieldsView]
  SET
    [CustomFieldID] = @CustomFieldID,
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [ApiFieldName] = @ApiFieldName,
    [RefType] = @RefType,
    [FieldType] = @FieldType,
    [AuxID] = @AuxID,
    [Position] = @Position,
    [ListValues] = @ListValues,
    [Description] = @Description,
    [IsVisibleOnPortal] = @IsVisibleOnPortal,
    [IsFirstIndexSelect] = @IsFirstIndexSelect,
    [IsRequired] = @IsRequired,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [CustomFieldCategoryID] = @CustomFieldCategoryID,
    [IsRequiredToClose] = @IsRequiredToClose,
    [Mask] = @Mask,
    [ParentCustomFieldID] = @ParentCustomFieldID,
    [ParentCustomValue] = @ParentCustomValue,
    [ParentProductID] = @ParentProductID,
    [ParentFieldName] = @ParentFieldName,
    [ParentProductName] = @ParentProductName
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCustomFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCustomFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCustomFieldsViewItem

(

)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CustomFieldsView]
  WH)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCustomFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCustomFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCustomFieldsViewItem

(

)
AS
  SET NOCOUNT OFF;
  SELECT
    [CustomFieldID],
    [OrganizationID],
    [Name],
    [ApiFieldName],
    [RefType],
    [FieldType],
    [AuxID],
    [Position],
    [ListValues],
    [Description],
    [IsVisibleOnPortal],
    [IsFirstIndexSelect],
    [IsRequired],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [CustomFieldCategoryID],
    [IsRequiredToClose],
    [Mask],
    [ParentCustomFieldID],
    [ParentCustomValue],
    [ParentProductID],
    [ParentFieldName],
    [ParentProductName]
  FROM [dbo].[CustomFieldsView]
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCustomFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCustomFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCustomFieldsViewItem

(
  @CustomFieldID int,
  @OrganizationID int,
  @Name varchar(50),
  @ApiFieldName varchar(100),
  @RefType int,
  @FieldType int,
  @AuxID int,
  @Position int,
  @ListValues varchar(MAX),
  @Description varchar(250),
  @IsVisibleOnPortal bit,
  @IsFirstIndexSelect bit,
  @IsRequired bit,
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @CustomFieldCategoryID int,
  @IsRequiredToClose bit,
  @Mask varchar(MAX),
  @ParentCustomFieldID int,
  @ParentCustomValue varchar(MAX),
  @ParentProductID int,
  @ParentFieldName varchar(50),
  @ParentProductName varchar(255),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CustomFieldsView]
  (
    [CustomFieldID],
    [OrganizationID],
    [Name],
    [ApiFieldName],
    [RefType],
    [FieldType],
    [AuxID],
    [Position],
    [ListValues],
    [Description],
    [IsVisibleOnPortal],
    [IsFirstIndexSelect],
    [IsRequired],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [CustomFieldCategoryID],
    [IsRequiredToClose],
    [Mask],
    [ParentCustomFieldID],
    [ParentCustomValue],
    [ParentProductID],
    [ParentFieldName],
    [ParentProductName])
  VALUES (
    @CustomFieldID,
    @OrganizationID,
    @Name,
    @ApiFieldName,
    @RefType,
    @FieldType,
    @AuxID,
    @Position,
    @ListValues,
    @Description,
    @IsVisibleOnPortal,
    @IsFirstIndexSelect,
    @IsRequired,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @CustomFieldCategoryID,
    @IsRequiredToClose,
    @Mask,
    @ParentCustomFieldID,
    @ParentCustomValue,
    @ParentProductID,
    @ParentFieldName,
    @ParentProductName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCustomFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCustomFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCustomFieldsViewItem

(
  @CustomFieldID int,
  @OrganizationID int,
  @Name varchar(50),
  @ApiFieldName varchar(100),
  @RefType int,
  @FieldType int,
  @AuxID int,
  @Position int,
  @ListValues varchar(MAX),
  @Description varchar(250),
  @IsVisibleOnPortal bit,
  @IsFirstIndexSelect bit,
  @IsRequired bit,
  @DateModified datetime,
  @ModifierID int,
  @CustomFieldCategoryID int,
  @IsRequiredToClose bit,
  @Mask varchar(MAX),
  @ParentCustomFieldID int,
  @ParentCustomValue varchar(MAX),
  @ParentProductID int,
  @ParentFieldName varchar(50),
  @ParentProductName varchar(255)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CustomFieldsView]
  SET
    [CustomFieldID] = @CustomFieldID,
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [ApiFieldName] = @ApiFieldName,
    [RefType] = @RefType,
    [FieldType] = @FieldType,
    [AuxID] = @AuxID,
    [Position] = @Position,
    [ListValues] = @ListValues,
    [Description] = @Description,
    [IsVisibleOnPortal] = @IsVisibleOnPortal,
    [IsFirstIndexSelect] = @IsFirstIndexSelect,
    [IsRequired] = @IsRequired,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [CustomFieldCategoryID] = @CustomFieldCategoryID,
    [IsRequiredToClose] = @IsRequiredToClose,
    [Mask] = @Mask,
    [ParentCustomFieldID] = @ParentCustomFieldID,
    [ParentCustomValue] = @ParentCustomValue,
    [ParentProductID] = @ParentProductID,
    [ParentFieldName] = @ParentFieldName,
    [ParentProductName] = @ParentProductName
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCustomFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCustomFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCustomFieldsViewItem

(

)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CustomFieldsView]
  WH)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCustomFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCustomFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCustomFieldsViewItem

(

)
AS
  SET NOCOUNT OFF;
  SELECT
    [CustomFieldID],
    [OrganizationID],
    [Name],
    [ApiFieldName],
    [RefType],
    [FieldType],
    [AuxID],
    [Position],
    [ListValues],
    [Description],
    [IsVisibleOnPortal],
    [IsFirstIndexSelect],
    [IsRequired],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [CustomFieldCategoryID],
    [IsRequiredToClose],
    [Mask],
    [ParentCustomFieldID],
    [ParentCustomValue],
    [ParentProductID],
    [ParentFieldName],
    [ParentProductName]
  FROM [dbo].[CustomFieldsView]
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCustomFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCustomFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCustomFieldsViewItem

(
  @CustomFieldID int,
  @OrganizationID int,
  @Name varchar(50),
  @ApiFieldName varchar(100),
  @RefType int,
  @FieldType int,
  @AuxID int,
  @Position int,
  @ListValues varchar(MAX),
  @Description varchar(250),
  @IsVisibleOnPortal bit,
  @IsFirstIndexSelect bit,
  @IsRequired bit,
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @CustomFieldCategoryID int,
  @IsRequiredToClose bit,
  @Mask varchar(MAX),
  @ParentCustomFieldID int,
  @ParentCustomValue varchar(MAX),
  @ParentProductID int,
  @ParentFieldName varchar(50),
  @ParentProductName varchar(255),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CustomFieldsView]
  (
    [CustomFieldID],
    [OrganizationID],
    [Name],
    [ApiFieldName],
    [RefType],
    [FieldType],
    [AuxID],
    [Position],
    [ListValues],
    [Description],
    [IsVisibleOnPortal],
    [IsFirstIndexSelect],
    [IsRequired],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [CustomFieldCategoryID],
    [IsRequiredToClose],
    [Mask],
    [ParentCustomFieldID],
    [ParentCustomValue],
    [ParentProductID],
    [ParentFieldName],
    [ParentProductName])
  VALUES (
    @CustomFieldID,
    @OrganizationID,
    @Name,
    @ApiFieldName,
    @RefType,
    @FieldType,
    @AuxID,
    @Position,
    @ListValues,
    @Description,
    @IsVisibleOnPortal,
    @IsFirstIndexSelect,
    @IsRequired,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @CustomFieldCategoryID,
    @IsRequiredToClose,
    @Mask,
    @ParentCustomFieldID,
    @ParentCustomValue,
    @ParentProductID,
    @ParentFieldName,
    @ParentProductName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCustomFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCustomFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCustomFieldsViewItem

(
  @CustomFieldID int,
  @OrganizationID int,
  @Name varchar(50),
  @ApiFieldName varchar(100),
  @RefType int,
  @FieldType int,
  @AuxID int,
  @Position int,
  @ListValues varchar(MAX),
  @Description varchar(250),
  @IsVisibleOnPortal bit,
  @IsFirstIndexSelect bit,
  @IsRequired bit,
  @DateModified datetime,
  @ModifierID int,
  @CustomFieldCategoryID int,
  @IsRequiredToClose bit,
  @Mask varchar(MAX),
  @ParentCustomFieldID int,
  @ParentCustomValue varchar(MAX),
  @ParentProductID int,
  @ParentFieldName varchar(50),
  @ParentProductName varchar(255)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CustomFieldsView]
  SET
    [CustomFieldID] = @CustomFieldID,
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [ApiFieldName] = @ApiFieldName,
    [RefType] = @RefType,
    [FieldType] = @FieldType,
    [AuxID] = @AuxID,
    [Position] = @Position,
    [ListValues] = @ListValues,
    [Description] = @Description,
    [IsVisibleOnPortal] = @IsVisibleOnPortal,
    [IsFirstIndexSelect] = @IsFirstIndexSelect,
    [IsRequired] = @IsRequired,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [CustomFieldCategoryID] = @CustomFieldCategoryID,
    [IsRequiredToClose] = @IsRequiredToClose,
    [Mask] = @Mask,
    [ParentCustomFieldID] = @ParentCustomFieldID,
    [ParentCustomValue] = @ParentCustomValue,
    [ParentProductID] = @ParentProductID,
    [ParentFieldName] = @ParentFieldName,
    [ParentProductName] = @ParentProductName
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCustomFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCustomFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCustomFieldsViewItem

(

)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CustomFieldsView]
  WH)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCustomFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCustomFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCustomFieldsViewItem

(

)
AS
  SET NOCOUNT OFF;
  SELECT
    [CustomFieldID],
    [OrganizationID],
    [Name],
    [ApiFieldName],
    [RefType],
    [FieldType],
    [AuxID],
    [Position],
    [ListValues],
    [Description],
    [IsVisibleOnPortal],
    [IsFirstIndexSelect],
    [IsRequired],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [CustomFieldCategoryID],
    [IsRequiredToClose],
    [Mask],
    [ParentCustomFieldID],
    [ParentCustomValue],
    [ParentProductID],
    [ParentFieldName],
    [ParentProductName]
  FROM [dbo].[CustomFieldsView]
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCustomFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCustomFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCustomFieldsViewItem

(
  @CustomFieldID int,
  @OrganizationID int,
  @Name varchar(50),
  @ApiFieldName varchar(100),
  @RefType int,
  @FieldType int,
  @AuxID int,
  @Position int,
  @ListValues varchar(MAX),
  @Description varchar(250),
  @IsVisibleOnPortal bit,
  @IsFirstIndexSelect bit,
  @IsRequired bit,
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @CustomFieldCategoryID int,
  @IsRequiredToClose bit,
  @Mask varchar(MAX),
  @ParentCustomFieldID int,
  @ParentCustomValue varchar(MAX),
  @ParentProductID int,
  @ParentFieldName varchar(50),
  @ParentProductName varchar(255),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CustomFieldsView]
  (
    [CustomFieldID],
    [OrganizationID],
    [Name],
    [ApiFieldName],
    [RefType],
    [FieldType],
    [AuxID],
    [Position],
    [ListValues],
    [Description],
    [IsVisibleOnPortal],
    [IsFirstIndexSelect],
    [IsRequired],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [CustomFieldCategoryID],
    [IsRequiredToClose],
    [Mask],
    [ParentCustomFieldID],
    [ParentCustomValue],
    [ParentProductID],
    [ParentFieldName],
    [ParentProductName])
  VALUES (
    @CustomFieldID,
    @OrganizationID,
    @Name,
    @ApiFieldName,
    @RefType,
    @FieldType,
    @AuxID,
    @Position,
    @ListValues,
    @Description,
    @IsVisibleOnPortal,
    @IsFirstIndexSelect,
    @IsRequired,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @CustomFieldCategoryID,
    @IsRequiredToClose,
    @Mask,
    @ParentCustomFieldID,
    @ParentCustomValue,
    @ParentProductID,
    @ParentFieldName,
    @ParentProductName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCustomFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCustomFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCustomFieldsViewItem

(
  @CustomFieldID int,
  @OrganizationID int,
  @Name varchar(50),
  @ApiFieldName varchar(100),
  @RefType int,
  @FieldType int,
  @AuxID int,
  @Position int,
  @ListValues varchar(MAX),
  @Description varchar(250),
  @IsVisibleOnPortal bit,
  @IsFirstIndexSelect bit,
  @IsRequired bit,
  @DateModified datetime,
  @ModifierID int,
  @CustomFieldCategoryID int,
  @IsRequiredToClose bit,
  @Mask varchar(MAX),
  @ParentCustomFieldID int,
  @ParentCustomValue varchar(MAX),
  @ParentProductID int,
  @ParentFieldName varchar(50),
  @ParentProductName varchar(255)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CustomFieldsView]
  SET
    [CustomFieldID] = @CustomFieldID,
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [ApiFieldName] = @ApiFieldName,
    [RefType] = @RefType,
    [FieldType] = @FieldType,
    [AuxID] = @AuxID,
    [Position] = @Position,
    [ListValues] = @ListValues,
    [Description] = @Description,
    [IsVisibleOnPortal] = @IsVisibleOnPortal,
    [IsFirstIndexSelect] = @IsFirstIndexSelect,
    [IsRequired] = @IsRequired,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [CustomFieldCategoryID] = @CustomFieldCategoryID,
    [IsRequiredToClose] = @IsRequiredToClose,
    [Mask] = @Mask,
    [ParentCustomFieldID] = @ParentCustomFieldID,
    [ParentCustomValue] = @ParentCustomValue,
    [ParentProductID] = @ParentProductID,
    [ParentFieldName] = @ParentFieldName,
    [ParentProductName] = @ParentProductName
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCustomFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCustomFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCustomFieldsViewItem

(

)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CustomFieldsView]
  WH)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCustomFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCustomFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCustomFieldsViewItem

(

)
AS
  SET NOCOUNT OFF;
  SELECT
    [CustomFieldID],
    [OrganizationID],
    [Name],
    [ApiFieldName],
    [RefType],
    [FieldType],
    [AuxID],
    [Position],
    [ListValues],
    [Description],
    [IsVisibleOnPortal],
    [IsFirstIndexSelect],
    [IsRequired],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [CustomFieldCategoryID],
    [IsRequiredToClose],
    [Mask],
    [ParentCustomFieldID],
    [ParentCustomValue],
    [ParentProductID],
    [ParentFieldName],
    [ParentProductName]
  FROM [dbo].[CustomFieldsView]
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCustomFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCustomFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCustomFieldsViewItem

(
  @CustomFieldID int,
  @OrganizationID int,
  @Name varchar(50),
  @ApiFieldName varchar(100),
  @RefType int,
  @FieldType int,
  @AuxID int,
  @Position int,
  @ListValues varchar(MAX),
  @Description varchar(250),
  @IsVisibleOnPortal bit,
  @IsFirstIndexSelect bit,
  @IsRequired bit,
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @CustomFieldCategoryID int,
  @IsRequiredToClose bit,
  @Mask varchar(MAX),
  @ParentCustomFieldID int,
  @ParentCustomValue varchar(MAX),
  @ParentProductID int,
  @ParentFieldName varchar(50),
  @ParentProductName varchar(255),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CustomFieldsView]
  (
    [CustomFieldID],
    [OrganizationID],
    [Name],
    [ApiFieldName],
    [RefType],
    [FieldType],
    [AuxID],
    [Position],
    [ListValues],
    [Description],
    [IsVisibleOnPortal],
    [IsFirstIndexSelect],
    [IsRequired],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [CustomFieldCategoryID],
    [IsRequiredToClose],
    [Mask],
    [ParentCustomFieldID],
    [ParentCustomValue],
    [ParentProductID],
    [ParentFieldName],
    [ParentProductName])
  VALUES (
    @CustomFieldID,
    @OrganizationID,
    @Name,
    @ApiFieldName,
    @RefType,
    @FieldType,
    @AuxID,
    @Position,
    @ListValues,
    @Description,
    @IsVisibleOnPortal,
    @IsFirstIndexSelect,
    @IsRequired,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @CustomFieldCategoryID,
    @IsRequiredToClose,
    @Mask,
    @ParentCustomFieldID,
    @ParentCustomValue,
    @ParentProductID,
    @ParentFieldName,
    @ParentProductName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCustomFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCustomFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCustomFieldsViewItem

(
  @CustomFieldID int,
  @OrganizationID int,
  @Name varchar(50),
  @ApiFieldName varchar(100),
  @RefType int,
  @FieldType int,
  @AuxID int,
  @Position int,
  @ListValues varchar(MAX),
  @Description varchar(250),
  @IsVisibleOnPortal bit,
  @IsFirstIndexSelect bit,
  @IsRequired bit,
  @DateModified datetime,
  @ModifierID int,
  @CustomFieldCategoryID int,
  @IsRequiredToClose bit,
  @Mask varchar(MAX),
  @ParentCustomFieldID int,
  @ParentCustomValue varchar(MAX),
  @ParentProductID int,
  @ParentFieldName varchar(50),
  @ParentProductName varchar(255)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CustomFieldsView]
  SET
    [CustomFieldID] = @CustomFieldID,
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [ApiFieldName] = @ApiFieldName,
    [RefType] = @RefType,
    [FieldType] = @FieldType,
    [AuxID] = @AuxID,
    [Position] = @Position,
    [ListValues] = @ListValues,
    [Description] = @Description,
    [IsVisibleOnPortal] = @IsVisibleOnPortal,
    [IsFirstIndexSelect] = @IsFirstIndexSelect,
    [IsRequired] = @IsRequired,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [CustomFieldCategoryID] = @CustomFieldCategoryID,
    [IsRequiredToClose] = @IsRequiredToClose,
    [Mask] = @Mask,
    [ParentCustomFieldID] = @ParentCustomFieldID,
    [ParentCustomValue] = @ParentCustomValue,
    [ParentProductID] = @ParentProductID,
    [ParentFieldName] = @ParentFieldName,
    [ParentProductName] = @ParentProductName
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCustomFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCustomFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCustomFieldsViewItem

(

)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CustomFieldsView]
  WH)
GO


