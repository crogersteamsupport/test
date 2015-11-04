IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectNot' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectNot
GO

CREATE PROCEDURE dbo.uspGeneratedSelectNot

(
  @NoteID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [NoteID],
    [RefType],
    [RefID],
    [Title],
    [Description],
    [CreatorID],
    [ModifierID],
    [DateCreated],
    [DateModified],
    [NeedsIndexing],
    [IsAlert],
    [ImportFileID]
  FROM [dbo].[Notes]
  WHERE ([NoteID] = @NoteID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertNot' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertNot
GO

CREATE PROCEDURE dbo.uspGeneratedInsertNot

(
  @RefType int,
  @RefID int,
  @Title varchar(1000),
  @Description varchar(MAX),
  @CreatorID int,
  @ModifierID int,
  @DateCreated datetime,
  @DateModified datetime,
  @NeedsIndexing bit,
  @IsAlert bit,
  @ImportFileID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Notes]
  (
    [RefType],
    [RefID],
    [Title],
    [Description],
    [CreatorID],
    [ModifierID],
    [DateCreated],
    [DateModified],
    [NeedsIndexing],
    [IsAlert],
    [ImportFileID])
  VALUES (
    @RefType,
    @RefID,
    @Title,
    @Description,
    @CreatorID,
    @ModifierID,
    @DateCreated,
    @DateModified,
    @NeedsIndexing,
    @IsAlert,
    @ImportFileID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateNot' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateNot
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateNot

(
  @NoteID int,
  @RefType int,
  @RefID int,
  @Title varchar(1000),
  @Description varchar(MAX),
  @ModifierID int,
  @DateModified datetime,
  @NeedsIndexing bit,
  @IsAlert bit,
  @ImportFileID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Notes]
  SET
    [RefType] = @RefType,
    [RefID] = @RefID,
    [Title] = @Title,
    [Description] = @Description,
    [ModifierID] = @ModifierID,
    [DateModified] = @DateModified,
    [NeedsIndexing] = @NeedsIndexing,
    [IsAlert] = @IsAlert,
    [ImportFileID] = @ImportFileID
  WHERE ([NoteID] = @NoteID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteNot' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteNot
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteNot

(
  @NoteID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Notes]
  WHERE ([NoteID] = @NoteID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectNot' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectNot
GO

CREATE PROCEDURE dbo.uspGeneratedSelectNot

(
  @NoteID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [NoteID],
    [RefType],
    [RefID],
    [Title],
    [Description],
    [CreatorID],
    [ModifierID],
    [DateCreated],
    [DateModified],
    [NeedsIndexing],
    [IsAlert],
    [ImportFileID]
  FROM [dbo].[Notes]
  WHERE ([NoteID] = @NoteID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertNot' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertNot
GO

CREATE PROCEDURE dbo.uspGeneratedInsertNot

(
  @RefType int,
  @RefID int,
  @Title varchar(1000),
  @Description varchar(MAX),
  @CreatorID int,
  @ModifierID int,
  @DateCreated datetime,
  @DateModified datetime,
  @NeedsIndexing bit,
  @IsAlert bit,
  @ImportFileID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Notes]
  (
    [RefType],
    [RefID],
    [Title],
    [Description],
    [CreatorID],
    [ModifierID],
    [DateCreated],
    [DateModified],
    [NeedsIndexing],
    [IsAlert],
    [ImportFileID])
  VALUES (
    @RefType,
    @RefID,
    @Title,
    @Description,
    @CreatorID,
    @ModifierID,
    @DateCreated,
    @DateModified,
    @NeedsIndexing,
    @IsAlert,
    @ImportFileID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateNot' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateNot
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateNot

(
  @NoteID int,
  @RefType int,
  @RefID int,
  @Title varchar(1000),
  @Description varchar(MAX),
  @ModifierID int,
  @DateModified datetime,
  @NeedsIndexing bit,
  @IsAlert bit,
  @ImportFileID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Notes]
  SET
    [RefType] = @RefType,
    [RefID] = @RefID,
    [Title] = @Title,
    [Description] = @Description,
    [ModifierID] = @ModifierID,
    [DateModified] = @DateModified,
    [NeedsIndexing] = @NeedsIndexing,
    [IsAlert] = @IsAlert,
    [ImportFileID] = @ImportFileID
  WHERE ([NoteID] = @NoteID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteNot' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteNot
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteNot

(
  @NoteID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Notes]
  WHERE ([NoteID] = @NoteID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectNot' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectNot
GO

CREATE PROCEDURE dbo.uspGeneratedSelectNot

(
  @NoteID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [NoteID],
    [RefType],
    [RefID],
    [Title],
    [Description],
    [CreatorID],
    [ModifierID],
    [DateCreated],
    [DateModified],
    [NeedsIndexing],
    [IsAlert],
    [ImportFileID]
  FROM [dbo].[Notes]
  WHERE ([NoteID] = @NoteID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertNot' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertNot
GO

CREATE PROCEDURE dbo.uspGeneratedInsertNot

(
  @RefType int,
  @RefID int,
  @Title varchar(1000),
  @Description varchar(MAX),
  @CreatorID int,
  @ModifierID int,
  @DateCreated datetime,
  @DateModified datetime,
  @NeedsIndexing bit,
  @IsAlert bit,
  @ImportFileID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Notes]
  (
    [RefType],
    [RefID],
    [Title],
    [Description],
    [CreatorID],
    [ModifierID],
    [DateCreated],
    [DateModified],
    [NeedsIndexing],
    [IsAlert],
    [ImportFileID])
  VALUES (
    @RefType,
    @RefID,
    @Title,
    @Description,
    @CreatorID,
    @ModifierID,
    @DateCreated,
    @DateModified,
    @NeedsIndexing,
    @IsAlert,
    @ImportFileID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateNot' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateNot
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateNot

(
  @NoteID int,
  @RefType int,
  @RefID int,
  @Title varchar(1000),
  @Description varchar(MAX),
  @ModifierID int,
  @DateModified datetime,
  @NeedsIndexing bit,
  @IsAlert bit,
  @ImportFileID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Notes]
  SET
    [RefType] = @RefType,
    [RefID] = @RefID,
    [Title] = @Title,
    [Description] = @Description,
    [ModifierID] = @ModifierID,
    [DateModified] = @DateModified,
    [NeedsIndexing] = @NeedsIndexing,
    [IsAlert] = @IsAlert,
    [ImportFileID] = @ImportFileID
  WHERE ([NoteID] = @NoteID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteNot' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteNot
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteNot

(
  @NoteID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Notes]
  WHERE ([NoteID] = @NoteID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectNot' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectNot
GO

CREATE PROCEDURE dbo.uspGeneratedSelectNot

(
  @NoteID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [NoteID],
    [RefType],
    [RefID],
    [Title],
    [Description],
    [CreatorID],
    [ModifierID],
    [DateCreated],
    [DateModified],
    [NeedsIndexing],
    [IsAlert],
    [ImportFileID]
  FROM [dbo].[Notes]
  WHERE ([NoteID] = @NoteID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertNot' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertNot
GO

CREATE PROCEDURE dbo.uspGeneratedInsertNot

(
  @RefType int,
  @RefID int,
  @Title varchar(1000),
  @Description varchar(MAX),
  @CreatorID int,
  @ModifierID int,
  @DateCreated datetime,
  @DateModified datetime,
  @NeedsIndexing bit,
  @IsAlert bit,
  @ImportFileID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Notes]
  (
    [RefType],
    [RefID],
    [Title],
    [Description],
    [CreatorID],
    [ModifierID],
    [DateCreated],
    [DateModified],
    [NeedsIndexing],
    [IsAlert],
    [ImportFileID])
  VALUES (
    @RefType,
    @RefID,
    @Title,
    @Description,
    @CreatorID,
    @ModifierID,
    @DateCreated,
    @DateModified,
    @NeedsIndexing,
    @IsAlert,
    @ImportFileID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateNot' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateNot
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateNot

(
  @NoteID int,
  @RefType int,
  @RefID int,
  @Title varchar(1000),
  @Description varchar(MAX),
  @ModifierID int,
  @DateModified datetime,
  @NeedsIndexing bit,
  @IsAlert bit,
  @ImportFileID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Notes]
  SET
    [RefType] = @RefType,
    [RefID] = @RefID,
    [Title] = @Title,
    [Description] = @Description,
    [ModifierID] = @ModifierID,
    [DateModified] = @DateModified,
    [NeedsIndexing] = @NeedsIndexing,
    [IsAlert] = @IsAlert,
    [ImportFileID] = @ImportFileID
  WHERE ([NoteID] = @NoteID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteNot' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteNot
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteNot

(
  @NoteID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Notes]
  WHERE ([NoteID] = @NoteID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectNot' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectNot
GO

CREATE PROCEDURE dbo.uspGeneratedSelectNot

(
  @NoteID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [NoteID],
    [RefType],
    [RefID],
    [Title],
    [Description],
    [CreatorID],
    [ModifierID],
    [DateCreated],
    [DateModified],
    [NeedsIndexing],
    [IsAlert],
    [ImportFileID]
  FROM [dbo].[Notes]
  WHERE ([NoteID] = @NoteID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertNot' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertNot
GO

CREATE PROCEDURE dbo.uspGeneratedInsertNot

(
  @RefType int,
  @RefID int,
  @Title varchar(1000),
  @Description varchar(MAX),
  @CreatorID int,
  @ModifierID int,
  @DateCreated datetime,
  @DateModified datetime,
  @NeedsIndexing bit,
  @IsAlert bit,
  @ImportFileID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Notes]
  (
    [RefType],
    [RefID],
    [Title],
    [Description],
    [CreatorID],
    [ModifierID],
    [DateCreated],
    [DateModified],
    [NeedsIndexing],
    [IsAlert],
    [ImportFileID])
  VALUES (
    @RefType,
    @RefID,
    @Title,
    @Description,
    @CreatorID,
    @ModifierID,
    @DateCreated,
    @DateModified,
    @NeedsIndexing,
    @IsAlert,
    @ImportFileID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateNot' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateNot
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateNot

(
  @NoteID int,
  @RefType int,
  @RefID int,
  @Title varchar(1000),
  @Description varchar(MAX),
  @ModifierID int,
  @DateModified datetime,
  @NeedsIndexing bit,
  @IsAlert bit,
  @ImportFileID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Notes]
  SET
    [RefType] = @RefType,
    [RefID] = @RefID,
    [Title] = @Title,
    [Description] = @Description,
    [ModifierID] = @ModifierID,
    [DateModified] = @DateModified,
    [NeedsIndexing] = @NeedsIndexing,
    [IsAlert] = @IsAlert,
    [ImportFileID] = @ImportFileID
  WHERE ([NoteID] = @NoteID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteNot' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteNot
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteNot

(
  @NoteID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Notes]
  WHERE ([NoteID] = @NoteID)
GO


