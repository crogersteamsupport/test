IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectSimpleImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectSimpleImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectSimpleImportFieldsViewItem

(

)
AS
  SET NOCOUNT OFF;
  SELECT
    [ImportFieldID],
    [TableName],
    [FieldName],
    [Alias],
    [DataType],
    [Size],
    [IsVisible],
    [IsRequired],
    [Description],
    [RefType]
  FROM [dbo].[SimpleImportFieldsView]
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertSimpleImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertSimpleImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertSimpleImportFieldsViewItem

(
  @ImportFieldID int,
  @TableName varchar(100),
  @FieldName varchar(561),
  @Alias varchar(250),
  @DataType varchar(150),
  @Size int,
  @IsVisible bit,
  @IsRequired bit,
  @Description varchar(1000),
  @RefType int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[SimpleImportFieldsView]
  (
    [ImportFieldID],
    [TableName],
    [FieldName],
    [Alias],
    [DataType],
    [Size],
    [IsVisible],
    [IsRequired],
    [Description],
    [RefType])
  VALUES (
    @ImportFieldID,
    @TableName,
    @FieldName,
    @Alias,
    @DataType,
    @Size,
    @IsVisible,
    @IsRequired,
    @Description,
    @RefType)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateSimpleImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateSimpleImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateSimpleImportFieldsViewItem

(
  @ImportFieldID int,
  @TableName varchar(100),
  @FieldName varchar(561),
  @Alias varchar(250),
  @DataType varchar(150),
  @Size int,
  @IsVisible bit,
  @IsRequired bit,
  @Description varchar(1000),
  @RefType int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[SimpleImportFieldsView]
  SET
    [ImportFieldID] = @ImportFieldID,
    [TableName] = @TableName,
    [FieldName] = @FieldName,
    [Alias] = @Alias,
    [DataType] = @DataType,
    [Size] = @Size,
    [IsVisible] = @IsVisible,
    [IsRequired] = @IsRequired,
    [Description] = @Description,
    [RefType] = @RefType
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteSimpleImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteSimpleImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteSimpleImportFieldsViewItem

(

)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[SimpleImportFieldsView]
  WH)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectSimpleImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectSimpleImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectSimpleImportFieldsViewItem

(

)
AS
  SET NOCOUNT OFF;
  SELECT
    [ImportFieldID],
    [TableName],
    [FieldName],
    [Alias],
    [DataType],
    [Size],
    [IsVisible],
    [IsRequired],
    [Description],
    [RefType]
  FROM [dbo].[SimpleImportFieldsView]
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertSimpleImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertSimpleImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertSimpleImportFieldsViewItem

(
  @ImportFieldID int,
  @TableName varchar(100),
  @FieldName varchar(561),
  @Alias varchar(250),
  @DataType varchar(150),
  @Size int,
  @IsVisible bit,
  @IsRequired bit,
  @Description varchar(1000),
  @RefType int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[SimpleImportFieldsView]
  (
    [ImportFieldID],
    [TableName],
    [FieldName],
    [Alias],
    [DataType],
    [Size],
    [IsVisible],
    [IsRequired],
    [Description],
    [RefType])
  VALUES (
    @ImportFieldID,
    @TableName,
    @FieldName,
    @Alias,
    @DataType,
    @Size,
    @IsVisible,
    @IsRequired,
    @Description,
    @RefType)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateSimpleImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateSimpleImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateSimpleImportFieldsViewItem

(
  @ImportFieldID int,
  @TableName varchar(100),
  @FieldName varchar(561),
  @Alias varchar(250),
  @DataType varchar(150),
  @Size int,
  @IsVisible bit,
  @IsRequired bit,
  @Description varchar(1000),
  @RefType int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[SimpleImportFieldsView]
  SET
    [ImportFieldID] = @ImportFieldID,
    [TableName] = @TableName,
    [FieldName] = @FieldName,
    [Alias] = @Alias,
    [DataType] = @DataType,
    [Size] = @Size,
    [IsVisible] = @IsVisible,
    [IsRequired] = @IsRequired,
    [Description] = @Description,
    [RefType] = @RefType
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteSimpleImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteSimpleImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteSimpleImportFieldsViewItem

(

)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[SimpleImportFieldsView]
  WH)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectSimpleImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectSimpleImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectSimpleImportFieldsViewItem

(

)
AS
  SET NOCOUNT OFF;
  SELECT
    [ImportFieldID],
    [TableName],
    [FieldName],
    [Alias],
    [DataType],
    [Size],
    [IsVisible],
    [IsRequired],
    [Description],
    [RefType]
  FROM [dbo].[SimpleImportFieldsView]
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertSimpleImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertSimpleImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertSimpleImportFieldsViewItem

(
  @ImportFieldID int,
  @TableName varchar(100),
  @FieldName varchar(561),
  @Alias varchar(250),
  @DataType varchar(150),
  @Size int,
  @IsVisible bit,
  @IsRequired bit,
  @Description varchar(1000),
  @RefType int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[SimpleImportFieldsView]
  (
    [ImportFieldID],
    [TableName],
    [FieldName],
    [Alias],
    [DataType],
    [Size],
    [IsVisible],
    [IsRequired],
    [Description],
    [RefType])
  VALUES (
    @ImportFieldID,
    @TableName,
    @FieldName,
    @Alias,
    @DataType,
    @Size,
    @IsVisible,
    @IsRequired,
    @Description,
    @RefType)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateSimpleImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateSimpleImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateSimpleImportFieldsViewItem

(
  @ImportFieldID int,
  @TableName varchar(100),
  @FieldName varchar(561),
  @Alias varchar(250),
  @DataType varchar(150),
  @Size int,
  @IsVisible bit,
  @IsRequired bit,
  @Description varchar(1000),
  @RefType int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[SimpleImportFieldsView]
  SET
    [ImportFieldID] = @ImportFieldID,
    [TableName] = @TableName,
    [FieldName] = @FieldName,
    [Alias] = @Alias,
    [DataType] = @DataType,
    [Size] = @Size,
    [IsVisible] = @IsVisible,
    [IsRequired] = @IsRequired,
    [Description] = @Description,
    [RefType] = @RefType
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteSimpleImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteSimpleImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteSimpleImportFieldsViewItem

(

)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[SimpleImportFieldsView]
  WH)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectSimpleImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectSimpleImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectSimpleImportFieldsViewItem

(

)
AS
  SET NOCOUNT OFF;
  SELECT
    [ImportFieldID],
    [TableName],
    [FieldName],
    [Alias],
    [DataType],
    [Size],
    [IsVisible],
    [IsRequired],
    [Description],
    [RefType]
  FROM [dbo].[SimpleImportFieldsView]
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertSimpleImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertSimpleImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertSimpleImportFieldsViewItem

(
  @ImportFieldID int,
  @TableName varchar(100),
  @FieldName varchar(561),
  @Alias varchar(250),
  @DataType varchar(150),
  @Size int,
  @IsVisible bit,
  @IsRequired bit,
  @Description varchar(1000),
  @RefType int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[SimpleImportFieldsView]
  (
    [ImportFieldID],
    [TableName],
    [FieldName],
    [Alias],
    [DataType],
    [Size],
    [IsVisible],
    [IsRequired],
    [Description],
    [RefType])
  VALUES (
    @ImportFieldID,
    @TableName,
    @FieldName,
    @Alias,
    @DataType,
    @Size,
    @IsVisible,
    @IsRequired,
    @Description,
    @RefType)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateSimpleImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateSimpleImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateSimpleImportFieldsViewItem

(
  @ImportFieldID int,
  @TableName varchar(100),
  @FieldName varchar(561),
  @Alias varchar(250),
  @DataType varchar(150),
  @Size int,
  @IsVisible bit,
  @IsRequired bit,
  @Description varchar(1000),
  @RefType int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[SimpleImportFieldsView]
  SET
    [ImportFieldID] = @ImportFieldID,
    [TableName] = @TableName,
    [FieldName] = @FieldName,
    [Alias] = @Alias,
    [DataType] = @DataType,
    [Size] = @Size,
    [IsVisible] = @IsVisible,
    [IsRequired] = @IsRequired,
    [Description] = @Description,
    [RefType] = @RefType
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteSimpleImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteSimpleImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteSimpleImportFieldsViewItem

(

)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[SimpleImportFieldsView]
  WH)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectSimpleImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectSimpleImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectSimpleImportFieldsViewItem

(

)
AS
  SET NOCOUNT OFF;
  SELECT
    [ImportFieldID],
    [TableName],
    [FieldName],
    [Alias],
    [DataType],
    [Size],
    [IsVisible],
    [IsRequired],
    [Description],
    [RefType]
  FROM [dbo].[SimpleImportFieldsView]
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertSimpleImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertSimpleImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertSimpleImportFieldsViewItem

(
  @ImportFieldID int,
  @TableName varchar(100),
  @FieldName varchar(561),
  @Alias varchar(250),
  @DataType varchar(150),
  @Size int,
  @IsVisible bit,
  @IsRequired bit,
  @Description varchar(1000),
  @RefType int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[SimpleImportFieldsView]
  (
    [ImportFieldID],
    [TableName],
    [FieldName],
    [Alias],
    [DataType],
    [Size],
    [IsVisible],
    [IsRequired],
    [Description],
    [RefType])
  VALUES (
    @ImportFieldID,
    @TableName,
    @FieldName,
    @Alias,
    @DataType,
    @Size,
    @IsVisible,
    @IsRequired,
    @Description,
    @RefType)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateSimpleImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateSimpleImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateSimpleImportFieldsViewItem

(
  @ImportFieldID int,
  @TableName varchar(100),
  @FieldName varchar(561),
  @Alias varchar(250),
  @DataType varchar(150),
  @Size int,
  @IsVisible bit,
  @IsRequired bit,
  @Description varchar(1000),
  @RefType int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[SimpleImportFieldsView]
  SET
    [ImportFieldID] = @ImportFieldID,
    [TableName] = @TableName,
    [FieldName] = @FieldName,
    [Alias] = @Alias,
    [DataType] = @DataType,
    [Size] = @Size,
    [IsVisible] = @IsVisible,
    [IsRequired] = @IsRequired,
    [Description] = @Description,
    [RefType] = @RefType
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteSimpleImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteSimpleImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteSimpleImportFieldsViewItem

(

)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[SimpleImportFieldsView]
  WH)
GO


