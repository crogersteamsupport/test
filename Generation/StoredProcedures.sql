IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCustomField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCustomField
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCustomField

(
  @CustomFieldID int
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
    [ParentProductID]
  FROM [dbo].[CustomFields]
  WHERE ([CustomFieldID] = @CustomFieldID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCustomField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCustomField
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCustomField

(
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
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CustomFields]
  (
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
    [ParentProductID])
  VALUES (
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
    @ParentProductID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCustomField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCustomField
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCustomField

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
  @ParentProductID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CustomFields]
  SET
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
    [ParentProductID] = @ParentProductID
  WHERE ([CustomFieldID] = @CustomFieldID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCustomField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCustomField
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCustomField

(
  @CustomFieldID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CustomFields]
  WHERE ([CustomFieldID] = @CustomFieldID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCustomField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCustomField
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCustomField

(
  @CustomFieldID int
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
    [ParentProductID]
  FROM [dbo].[CustomFields]
  WHERE ([CustomFieldID] = @CustomFieldID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCustomField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCustomField
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCustomField

(
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
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CustomFields]
  (
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
    [ParentProductID])
  VALUES (
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
    @ParentProductID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCustomField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCustomField
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCustomField

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
  @ParentProductID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CustomFields]
  SET
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
    [ParentProductID] = @ParentProductID
  WHERE ([CustomFieldID] = @CustomFieldID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCustomField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCustomField
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCustomField

(
  @CustomFieldID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CustomFields]
  WHERE ([CustomFieldID] = @CustomFieldID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCustomField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCustomField
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCustomField

(
  @CustomFieldID int
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
    [ParentProductID]
  FROM [dbo].[CustomFields]
  WHERE ([CustomFieldID] = @CustomFieldID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCustomField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCustomField
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCustomField

(
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
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CustomFields]
  (
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
    [ParentProductID])
  VALUES (
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
    @ParentProductID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCustomField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCustomField
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCustomField

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
  @ParentProductID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CustomFields]
  SET
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
    [ParentProductID] = @ParentProductID
  WHERE ([CustomFieldID] = @CustomFieldID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCustomField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCustomField
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCustomField

(
  @CustomFieldID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CustomFields]
  WHERE ([CustomFieldID] = @CustomFieldID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCustomField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCustomField
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCustomField

(
  @CustomFieldID int
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
    [ParentProductID]
  FROM [dbo].[CustomFields]
  WHERE ([CustomFieldID] = @CustomFieldID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCustomField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCustomField
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCustomField

(
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
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CustomFields]
  (
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
    [ParentProductID])
  VALUES (
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
    @ParentProductID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCustomField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCustomField
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCustomField

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
  @ParentProductID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CustomFields]
  SET
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
    [ParentProductID] = @ParentProductID
  WHERE ([CustomFieldID] = @CustomFieldID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCustomField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCustomField
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCustomField

(
  @CustomFieldID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CustomFields]
  WHERE ([CustomFieldID] = @CustomFieldID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCustomField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCustomField
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCustomField

(
  @CustomFieldID int
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
    [ParentProductID]
  FROM [dbo].[CustomFields]
  WHERE ([CustomFieldID] = @CustomFieldID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCustomField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCustomField
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCustomField

(
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
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CustomFields]
  (
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
    [ParentProductID])
  VALUES (
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
    @ParentProductID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCustomField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCustomField
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCustomField

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
  @ParentProductID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CustomFields]
  SET
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
    [ParentProductID] = @ParentProductID
  WHERE ([CustomFieldID] = @CustomFieldID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCustomField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCustomField
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCustomField

(
  @CustomFieldID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CustomFields]
  WHERE ([CustomFieldID] = @CustomFieldID)
GO


