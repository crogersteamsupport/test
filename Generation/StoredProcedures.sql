IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectEmailAddress' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectEmailAddress
GO

CREATE PROCEDURE dbo.uspGeneratedSelectEmailAddress

(
  @Id int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [Id],
    [RefID],
    [RefType],
    [Email],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [ImportFileID]
  FROM [dbo].[EmailAddresses]
  WHERE ([Id] = @Id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertEmailAddress' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertEmailAddress
GO

CREATE PROCEDURE dbo.uspGeneratedInsertEmailAddress

(
  @RefID int,
  @RefType int,
  @Email nvarchar(1024),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @ImportFileID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[EmailAddresses]
  (
    [RefID],
    [RefType],
    [Email],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [ImportFileID])
  VALUES (
    @RefID,
    @RefType,
    @Email,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @ImportFileID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateEmailAddress' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateEmailAddress
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateEmailAddress

(
  @Id int,
  @RefID int,
  @RefType int,
  @Email nvarchar(1024),
  @DateModified datetime,
  @ModifierID int,
  @ImportFileID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[EmailAddresses]
  SET
    [RefID] = @RefID,
    [RefType] = @RefType,
    [Email] = @Email,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [ImportFileID] = @ImportFileID
  WHERE ([Id] = @Id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteEmailAddress' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteEmailAddress
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteEmailAddress

(
  @Id int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[EmailAddresses]
  WHERE ([Id] = @Id)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectEmailAddress' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectEmailAddress
GO

CREATE PROCEDURE dbo.uspGeneratedSelectEmailAddress

(
  @Id int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [Id],
    [RefID],
    [RefType],
    [Email],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [ImportFileID]
  FROM [dbo].[EmailAddresses]
  WHERE ([Id] = @Id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertEmailAddress' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertEmailAddress
GO

CREATE PROCEDURE dbo.uspGeneratedInsertEmailAddress

(
  @RefID int,
  @RefType int,
  @Email nvarchar(1024),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @ImportFileID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[EmailAddresses]
  (
    [RefID],
    [RefType],
    [Email],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [ImportFileID])
  VALUES (
    @RefID,
    @RefType,
    @Email,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @ImportFileID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateEmailAddress' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateEmailAddress
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateEmailAddress

(
  @Id int,
  @RefID int,
  @RefType int,
  @Email nvarchar(1024),
  @DateModified datetime,
  @ModifierID int,
  @ImportFileID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[EmailAddresses]
  SET
    [RefID] = @RefID,
    [RefType] = @RefType,
    [Email] = @Email,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [ImportFileID] = @ImportFileID
  WHERE ([Id] = @Id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteEmailAddress' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteEmailAddress
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteEmailAddress

(
  @Id int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[EmailAddresses]
  WHERE ([Id] = @Id)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectEmailAddress' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectEmailAddress
GO

CREATE PROCEDURE dbo.uspGeneratedSelectEmailAddress

(
  @Id int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [Id],
    [RefID],
    [RefType],
    [Email],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [ImportFileID]
  FROM [dbo].[EmailAddresses]
  WHERE ([Id] = @Id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertEmailAddress' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertEmailAddress
GO

CREATE PROCEDURE dbo.uspGeneratedInsertEmailAddress

(
  @RefID int,
  @RefType int,
  @Email nvarchar(1024),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @ImportFileID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[EmailAddresses]
  (
    [RefID],
    [RefType],
    [Email],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [ImportFileID])
  VALUES (
    @RefID,
    @RefType,
    @Email,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @ImportFileID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateEmailAddress' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateEmailAddress
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateEmailAddress

(
  @Id int,
  @RefID int,
  @RefType int,
  @Email nvarchar(1024),
  @DateModified datetime,
  @ModifierID int,
  @ImportFileID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[EmailAddresses]
  SET
    [RefID] = @RefID,
    [RefType] = @RefType,
    [Email] = @Email,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [ImportFileID] = @ImportFileID
  WHERE ([Id] = @Id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteEmailAddress' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteEmailAddress
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteEmailAddress

(
  @Id int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[EmailAddresses]
  WHERE ([Id] = @Id)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectEmailAddress' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectEmailAddress
GO

CREATE PROCEDURE dbo.uspGeneratedSelectEmailAddress

(
  @Id int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [Id],
    [RefID],
    [RefType],
    [Email],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [ImportFileID]
  FROM [dbo].[EmailAddresses]
  WHERE ([Id] = @Id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertEmailAddress' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertEmailAddress
GO

CREATE PROCEDURE dbo.uspGeneratedInsertEmailAddress

(
  @RefID int,
  @RefType int,
  @Email nvarchar(1024),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @ImportFileID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[EmailAddresses]
  (
    [RefID],
    [RefType],
    [Email],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [ImportFileID])
  VALUES (
    @RefID,
    @RefType,
    @Email,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @ImportFileID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateEmailAddress' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateEmailAddress
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateEmailAddress

(
  @Id int,
  @RefID int,
  @RefType int,
  @Email nvarchar(1024),
  @DateModified datetime,
  @ModifierID int,
  @ImportFileID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[EmailAddresses]
  SET
    [RefID] = @RefID,
    [RefType] = @RefType,
    [Email] = @Email,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [ImportFileID] = @ImportFileID
  WHERE ([Id] = @Id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteEmailAddress' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteEmailAddress
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteEmailAddress

(
  @Id int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[EmailAddresses]
  WHERE ([Id] = @Id)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectEmailAddress' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectEmailAddress
GO

CREATE PROCEDURE dbo.uspGeneratedSelectEmailAddress

(
  @Id int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [Id],
    [RefID],
    [RefType],
    [Email],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [ImportFileID]
  FROM [dbo].[EmailAddresses]
  WHERE ([Id] = @Id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertEmailAddress' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertEmailAddress
GO

CREATE PROCEDURE dbo.uspGeneratedInsertEmailAddress

(
  @RefID int,
  @RefType int,
  @Email nvarchar(1024),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @ImportFileID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[EmailAddresses]
  (
    [RefID],
    [RefType],
    [Email],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [ImportFileID])
  VALUES (
    @RefID,
    @RefType,
    @Email,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @ImportFileID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateEmailAddress' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateEmailAddress
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateEmailAddress

(
  @Id int,
  @RefID int,
  @RefType int,
  @Email nvarchar(1024),
  @DateModified datetime,
  @ModifierID int,
  @ImportFileID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[EmailAddresses]
  SET
    [RefID] = @RefID,
    [RefType] = @RefType,
    [Email] = @Email,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [ImportFileID] = @ImportFileID
  WHERE ([Id] = @Id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteEmailAddress' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteEmailAddress
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteEmailAddress

(
  @Id int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[EmailAddresses]
  WHERE ([Id] = @Id)
GO


