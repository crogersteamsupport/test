IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectImportFieldsViewItem

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
    [RefType],
    [ImportMapID],
    [ImportID],
    [SourceName],
    [FieldID],
    [IsCustom]
  FROM [dbo].[ImportFieldsView]
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertImportFieldsViewItem

(
  @ImportFieldID int,
  @TableName varchar(100),
  @FieldName varchar(250),
  @Alias varchar(250),
  @DataType varchar(150),
  @Size int,
  @IsVisible bit,
  @IsRequired bit,
  @Description varchar(1000),
  @RefType int,
  @ImportMapID int,
  @ImportID int,
  @SourceName varchar(200),
  @FieldID int,
  @IsCustom bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ImportFieldsView]
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
    [RefType],
    [ImportMapID],
    [ImportID],
    [SourceName],
    [FieldID],
    [IsCustom])
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
    @RefType,
    @ImportMapID,
    @ImportID,
    @SourceName,
    @FieldID,
    @IsCustom)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateImportFieldsViewItem

(
  @ImportFieldID int,
  @TableName varchar(100),
  @FieldName varchar(250),
  @Alias varchar(250),
  @DataType varchar(150),
  @Size int,
  @IsVisible bit,
  @IsRequired bit,
  @Description varchar(1000),
  @RefType int,
  @ImportMapID int,
  @ImportID int,
  @SourceName varchar(200),
  @FieldID int,
  @IsCustom bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ImportFieldsView]
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
    [RefType] = @RefType,
    [ImportMapID] = @ImportMapID,
    [ImportID] = @ImportID,
    [SourceName] = @SourceName,
    [FieldID] = @FieldID,
    [IsCustom] = @IsCustom
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteImportFieldsViewItem

(

)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ImportFieldsView]
  WH)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectImportFieldsViewItem

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
    [RefType],
    [ImportMapID],
    [ImportID],
    [SourceName],
    [FieldID],
    [IsCustom]
  FROM [dbo].[ImportFieldsView]
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertImportFieldsViewItem

(
  @ImportFieldID int,
  @TableName varchar(100),
  @FieldName varchar(250),
  @Alias varchar(250),
  @DataType varchar(150),
  @Size int,
  @IsVisible bit,
  @IsRequired bit,
  @Description varchar(1000),
  @RefType int,
  @ImportMapID int,
  @ImportID int,
  @SourceName varchar(200),
  @FieldID int,
  @IsCustom bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ImportFieldsView]
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
    [RefType],
    [ImportMapID],
    [ImportID],
    [SourceName],
    [FieldID],
    [IsCustom])
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
    @RefType,
    @ImportMapID,
    @ImportID,
    @SourceName,
    @FieldID,
    @IsCustom)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateImportFieldsViewItem

(
  @ImportFieldID int,
  @TableName varchar(100),
  @FieldName varchar(250),
  @Alias varchar(250),
  @DataType varchar(150),
  @Size int,
  @IsVisible bit,
  @IsRequired bit,
  @Description varchar(1000),
  @RefType int,
  @ImportMapID int,
  @ImportID int,
  @SourceName varchar(200),
  @FieldID int,
  @IsCustom bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ImportFieldsView]
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
    [RefType] = @RefType,
    [ImportMapID] = @ImportMapID,
    [ImportID] = @ImportID,
    [SourceName] = @SourceName,
    [FieldID] = @FieldID,
    [IsCustom] = @IsCustom
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteImportFieldsViewItem

(

)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ImportFieldsView]
  WH)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectImportFieldsViewItem

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
    [RefType],
    [ImportMapID],
    [ImportID],
    [SourceName],
    [FieldID],
    [IsCustom]
  FROM [dbo].[ImportFieldsView]
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertImportFieldsViewItem

(
  @ImportFieldID int,
  @TableName varchar(100),
  @FieldName varchar(250),
  @Alias varchar(250),
  @DataType varchar(150),
  @Size int,
  @IsVisible bit,
  @IsRequired bit,
  @Description varchar(1000),
  @RefType int,
  @ImportMapID int,
  @ImportID int,
  @SourceName varchar(200),
  @FieldID int,
  @IsCustom bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ImportFieldsView]
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
    [RefType],
    [ImportMapID],
    [ImportID],
    [SourceName],
    [FieldID],
    [IsCustom])
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
    @RefType,
    @ImportMapID,
    @ImportID,
    @SourceName,
    @FieldID,
    @IsCustom)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateImportFieldsViewItem

(
  @ImportFieldID int,
  @TableName varchar(100),
  @FieldName varchar(250),
  @Alias varchar(250),
  @DataType varchar(150),
  @Size int,
  @IsVisible bit,
  @IsRequired bit,
  @Description varchar(1000),
  @RefType int,
  @ImportMapID int,
  @ImportID int,
  @SourceName varchar(200),
  @FieldID int,
  @IsCustom bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ImportFieldsView]
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
    [RefType] = @RefType,
    [ImportMapID] = @ImportMapID,
    [ImportID] = @ImportID,
    [SourceName] = @SourceName,
    [FieldID] = @FieldID,
    [IsCustom] = @IsCustom
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteImportFieldsViewItem

(

)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ImportFieldsView]
  WH)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectImportFieldsViewItem

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
    [RefType],
    [ImportMapID],
    [ImportID],
    [SourceName],
    [FieldID],
    [IsCustom]
  FROM [dbo].[ImportFieldsView]
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertImportFieldsViewItem

(
  @ImportFieldID int,
  @TableName varchar(100),
  @FieldName varchar(250),
  @Alias varchar(250),
  @DataType varchar(150),
  @Size int,
  @IsVisible bit,
  @IsRequired bit,
  @Description varchar(1000),
  @RefType int,
  @ImportMapID int,
  @ImportID int,
  @SourceName varchar(200),
  @FieldID int,
  @IsCustom bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ImportFieldsView]
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
    [RefType],
    [ImportMapID],
    [ImportID],
    [SourceName],
    [FieldID],
    [IsCustom])
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
    @RefType,
    @ImportMapID,
    @ImportID,
    @SourceName,
    @FieldID,
    @IsCustom)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateImportFieldsViewItem

(
  @ImportFieldID int,
  @TableName varchar(100),
  @FieldName varchar(250),
  @Alias varchar(250),
  @DataType varchar(150),
  @Size int,
  @IsVisible bit,
  @IsRequired bit,
  @Description varchar(1000),
  @RefType int,
  @ImportMapID int,
  @ImportID int,
  @SourceName varchar(200),
  @FieldID int,
  @IsCustom bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ImportFieldsView]
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
    [RefType] = @RefType,
    [ImportMapID] = @ImportMapID,
    [ImportID] = @ImportID,
    [SourceName] = @SourceName,
    [FieldID] = @FieldID,
    [IsCustom] = @IsCustom
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteImportFieldsViewItem

(

)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ImportFieldsView]
  WH)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectImportFieldsViewItem

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
    [RefType],
    [ImportMapID],
    [ImportID],
    [SourceName],
    [FieldID],
    [IsCustom]
  FROM [dbo].[ImportFieldsView]
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertImportFieldsViewItem

(
  @ImportFieldID int,
  @TableName varchar(100),
  @FieldName varchar(250),
  @Alias varchar(250),
  @DataType varchar(150),
  @Size int,
  @IsVisible bit,
  @IsRequired bit,
  @Description varchar(1000),
  @RefType int,
  @ImportMapID int,
  @ImportID int,
  @SourceName varchar(200),
  @FieldID int,
  @IsCustom bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ImportFieldsView]
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
    [RefType],
    [ImportMapID],
    [ImportID],
    [SourceName],
    [FieldID],
    [IsCustom])
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
    @RefType,
    @ImportMapID,
    @ImportID,
    @SourceName,
    @FieldID,
    @IsCustom)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateImportFieldsViewItem

(
  @ImportFieldID int,
  @TableName varchar(100),
  @FieldName varchar(250),
  @Alias varchar(250),
  @DataType varchar(150),
  @Size int,
  @IsVisible bit,
  @IsRequired bit,
  @Description varchar(1000),
  @RefType int,
  @ImportMapID int,
  @ImportID int,
  @SourceName varchar(200),
  @FieldID int,
  @IsCustom bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ImportFieldsView]
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
    [RefType] = @RefType,
    [ImportMapID] = @ImportMapID,
    [ImportID] = @ImportID,
    [SourceName] = @SourceName,
    [FieldID] = @FieldID,
    [IsCustom] = @IsCustom
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteImportFieldsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteImportFieldsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteImportFieldsViewItem

(

)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ImportFieldsView]
  WH)
GO


