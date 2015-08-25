IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectImportField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectImportField
GO

CREATE PROCEDURE dbo.uspGeneratedSelectImportField

(
  @ImportFieldID int
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
    [Enabled],
    [Position]
  FROM [dbo].[ImportFields]
  WHERE ([ImportFieldID] = @ImportFieldID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertImportField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertImportField
GO

CREATE PROCEDURE dbo.uspGeneratedInsertImportField

(
  @TableName varchar(100),
  @FieldName varchar(250),
  @Alias varchar(250),
  @DataType varchar(150),
  @Size int,
  @IsVisible bit,
  @IsRequired bit,
  @Description varchar(1000),
  @RefType int,
  @Enabled bit,
  @Position int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ImportFields]
  (
    [TableName],
    [FieldName],
    [Alias],
    [DataType],
    [Size],
    [IsVisible],
    [IsRequired],
    [Description],
    [RefType],
    [Enabled],
    [Position])
  VALUES (
    @TableName,
    @FieldName,
    @Alias,
    @DataType,
    @Size,
    @IsVisible,
    @IsRequired,
    @Description,
    @RefType,
    @Enabled,
    @Position)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateImportField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateImportField
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateImportField

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
  @Enabled bit,
  @Position int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ImportFields]
  SET
    [TableName] = @TableName,
    [FieldName] = @FieldName,
    [Alias] = @Alias,
    [DataType] = @DataType,
    [Size] = @Size,
    [IsVisible] = @IsVisible,
    [IsRequired] = @IsRequired,
    [Description] = @Description,
    [RefType] = @RefType,
    [Enabled] = @Enabled,
    [Position] = @Position
  WHERE ([ImportFieldID] = @ImportFieldID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteImportField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteImportField
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteImportField

(
  @ImportFieldID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ImportFields]
  WHERE ([ImportFieldID] = @ImportFieldID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectImportField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectImportField
GO

CREATE PROCEDURE dbo.uspGeneratedSelectImportField

(
  @ImportFieldID int
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
    [Enabled],
    [Position]
  FROM [dbo].[ImportFields]
  WHERE ([ImportFieldID] = @ImportFieldID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertImportField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertImportField
GO

CREATE PROCEDURE dbo.uspGeneratedInsertImportField

(
  @TableName varchar(100),
  @FieldName varchar(250),
  @Alias varchar(250),
  @DataType varchar(150),
  @Size int,
  @IsVisible bit,
  @IsRequired bit,
  @Description varchar(1000),
  @RefType int,
  @Enabled bit,
  @Position int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ImportFields]
  (
    [TableName],
    [FieldName],
    [Alias],
    [DataType],
    [Size],
    [IsVisible],
    [IsRequired],
    [Description],
    [RefType],
    [Enabled],
    [Position])
  VALUES (
    @TableName,
    @FieldName,
    @Alias,
    @DataType,
    @Size,
    @IsVisible,
    @IsRequired,
    @Description,
    @RefType,
    @Enabled,
    @Position)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateImportField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateImportField
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateImportField

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
  @Enabled bit,
  @Position int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ImportFields]
  SET
    [TableName] = @TableName,
    [FieldName] = @FieldName,
    [Alias] = @Alias,
    [DataType] = @DataType,
    [Size] = @Size,
    [IsVisible] = @IsVisible,
    [IsRequired] = @IsRequired,
    [Description] = @Description,
    [RefType] = @RefType,
    [Enabled] = @Enabled,
    [Position] = @Position
  WHERE ([ImportFieldID] = @ImportFieldID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteImportField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteImportField
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteImportField

(
  @ImportFieldID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ImportFields]
  WHERE ([ImportFieldID] = @ImportFieldID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectImportField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectImportField
GO

CREATE PROCEDURE dbo.uspGeneratedSelectImportField

(
  @ImportFieldID int
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
    [Enabled],
    [Position]
  FROM [dbo].[ImportFields]
  WHERE ([ImportFieldID] = @ImportFieldID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertImportField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertImportField
GO

CREATE PROCEDURE dbo.uspGeneratedInsertImportField

(
  @TableName varchar(100),
  @FieldName varchar(250),
  @Alias varchar(250),
  @DataType varchar(150),
  @Size int,
  @IsVisible bit,
  @IsRequired bit,
  @Description varchar(1000),
  @RefType int,
  @Enabled bit,
  @Position int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ImportFields]
  (
    [TableName],
    [FieldName],
    [Alias],
    [DataType],
    [Size],
    [IsVisible],
    [IsRequired],
    [Description],
    [RefType],
    [Enabled],
    [Position])
  VALUES (
    @TableName,
    @FieldName,
    @Alias,
    @DataType,
    @Size,
    @IsVisible,
    @IsRequired,
    @Description,
    @RefType,
    @Enabled,
    @Position)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateImportField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateImportField
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateImportField

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
  @Enabled bit,
  @Position int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ImportFields]
  SET
    [TableName] = @TableName,
    [FieldName] = @FieldName,
    [Alias] = @Alias,
    [DataType] = @DataType,
    [Size] = @Size,
    [IsVisible] = @IsVisible,
    [IsRequired] = @IsRequired,
    [Description] = @Description,
    [RefType] = @RefType,
    [Enabled] = @Enabled,
    [Position] = @Position
  WHERE ([ImportFieldID] = @ImportFieldID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteImportField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteImportField
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteImportField

(
  @ImportFieldID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ImportFields]
  WHERE ([ImportFieldID] = @ImportFieldID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectImportField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectImportField
GO

CREATE PROCEDURE dbo.uspGeneratedSelectImportField

(
  @ImportFieldID int
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
    [Enabled],
    [Position]
  FROM [dbo].[ImportFields]
  WHERE ([ImportFieldID] = @ImportFieldID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertImportField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertImportField
GO

CREATE PROCEDURE dbo.uspGeneratedInsertImportField

(
  @TableName varchar(100),
  @FieldName varchar(250),
  @Alias varchar(250),
  @DataType varchar(150),
  @Size int,
  @IsVisible bit,
  @IsRequired bit,
  @Description varchar(1000),
  @RefType int,
  @Enabled bit,
  @Position int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ImportFields]
  (
    [TableName],
    [FieldName],
    [Alias],
    [DataType],
    [Size],
    [IsVisible],
    [IsRequired],
    [Description],
    [RefType],
    [Enabled],
    [Position])
  VALUES (
    @TableName,
    @FieldName,
    @Alias,
    @DataType,
    @Size,
    @IsVisible,
    @IsRequired,
    @Description,
    @RefType,
    @Enabled,
    @Position)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateImportField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateImportField
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateImportField

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
  @Enabled bit,
  @Position int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ImportFields]
  SET
    [TableName] = @TableName,
    [FieldName] = @FieldName,
    [Alias] = @Alias,
    [DataType] = @DataType,
    [Size] = @Size,
    [IsVisible] = @IsVisible,
    [IsRequired] = @IsRequired,
    [Description] = @Description,
    [RefType] = @RefType,
    [Enabled] = @Enabled,
    [Position] = @Position
  WHERE ([ImportFieldID] = @ImportFieldID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteImportField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteImportField
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteImportField

(
  @ImportFieldID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ImportFields]
  WHERE ([ImportFieldID] = @ImportFieldID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectImportField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectImportField
GO

CREATE PROCEDURE dbo.uspGeneratedSelectImportField

(
  @ImportFieldID int
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
    [Enabled],
    [Position]
  FROM [dbo].[ImportFields]
  WHERE ([ImportFieldID] = @ImportFieldID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertImportField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertImportField
GO

CREATE PROCEDURE dbo.uspGeneratedInsertImportField

(
  @TableName varchar(100),
  @FieldName varchar(250),
  @Alias varchar(250),
  @DataType varchar(150),
  @Size int,
  @IsVisible bit,
  @IsRequired bit,
  @Description varchar(1000),
  @RefType int,
  @Enabled bit,
  @Position int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ImportFields]
  (
    [TableName],
    [FieldName],
    [Alias],
    [DataType],
    [Size],
    [IsVisible],
    [IsRequired],
    [Description],
    [RefType],
    [Enabled],
    [Position])
  VALUES (
    @TableName,
    @FieldName,
    @Alias,
    @DataType,
    @Size,
    @IsVisible,
    @IsRequired,
    @Description,
    @RefType,
    @Enabled,
    @Position)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateImportField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateImportField
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateImportField

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
  @Enabled bit,
  @Position int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ImportFields]
  SET
    [TableName] = @TableName,
    [FieldName] = @FieldName,
    [Alias] = @Alias,
    [DataType] = @DataType,
    [Size] = @Size,
    [IsVisible] = @IsVisible,
    [IsRequired] = @IsRequired,
    [Description] = @Description,
    [RefType] = @RefType,
    [Enabled] = @Enabled,
    [Position] = @Position
  WHERE ([ImportFieldID] = @ImportFieldID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteImportField' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteImportField
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteImportField

(
  @ImportFieldID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ImportFields]
  WHERE ([ImportFieldID] = @ImportFieldID)
GO


