IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTaskAssociation' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTaskAssociation
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTaskAssociation

(
  @ReminderID int,
  @RefID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ReminderID],
    [RefID],
    [RefType],
    [CreatorID],
    [DateCreated]
  FROM [dbo].[TaskAssociations]
  WHERE ([ReminderID] = @ReminderID AND [RefID] = @RefID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTaskAssociation' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTaskAssociation
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTaskAssociation

(
  @ReminderID int,
  @RefID int,
  @RefType int,
  @CreatorID int,
  @DateCreated datetime,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TaskAssociations]
  (
    [ReminderID],
    [RefID],
    [RefType],
    [CreatorID],
    [DateCreated])
  VALUES (
    @ReminderID,
    @RefID,
    @RefType,
    @CreatorID,
    @DateCreated)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTaskAssociation' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTaskAssociation
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTaskAssociation

(
  @ReminderID int,
  @RefID int,
  @RefType int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TaskAssociations]
  SET
    [RefType] = @RefType
  WHERE ([ReminderID] = @ReminderID AND [RefID] = @RefID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTaskAssociation' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTaskAssociation
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTaskAssociation

(
  @ReminderID int,
  @RefID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TaskAssociations]
  WHERE ([ReminderID] = @ReminderID AND [RefID] = @RefID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTaskAssociation' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTaskAssociation
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTaskAssociation

(
  @ReminderID int,
  @RefID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ReminderID],
    [RefID],
    [RefType],
    [CreatorID],
    [DateCreated]
  FROM [dbo].[TaskAssociations]
  WHERE ([ReminderID] = @ReminderID AND [RefID] = @RefID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTaskAssociation' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTaskAssociation
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTaskAssociation

(
  @ReminderID int,
  @RefID int,
  @RefType int,
  @CreatorID int,
  @DateCreated datetime,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TaskAssociations]
  (
    [ReminderID],
    [RefID],
    [RefType],
    [CreatorID],
    [DateCreated])
  VALUES (
    @ReminderID,
    @RefID,
    @RefType,
    @CreatorID,
    @DateCreated)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTaskAssociation' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTaskAssociation
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTaskAssociation

(
  @ReminderID int,
  @RefID int,
  @RefType int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TaskAssociations]
  SET
    [RefType] = @RefType
  WHERE ([ReminderID] = @ReminderID AND [RefID] = @RefID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTaskAssociation' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTaskAssociation
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTaskAssociation

(
  @ReminderID int,
  @RefID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TaskAssociations]
  WHERE ([ReminderID] = @ReminderID AND [RefID] = @RefID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTaskAssociation' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTaskAssociation
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTaskAssociation

(
  @ReminderID int,
  @RefID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ReminderID],
    [RefID],
    [RefType],
    [CreatorID],
    [DateCreated]
  FROM [dbo].[TaskAssociations]
  WHERE ([ReminderID] = @ReminderID AND [RefID] = @RefID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTaskAssociation' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTaskAssociation
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTaskAssociation

(
  @ReminderID int,
  @RefID int,
  @RefType int,
  @CreatorID int,
  @DateCreated datetime,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TaskAssociations]
  (
    [ReminderID],
    [RefID],
    [RefType],
    [CreatorID],
    [DateCreated])
  VALUES (
    @ReminderID,
    @RefID,
    @RefType,
    @CreatorID,
    @DateCreated)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTaskAssociation' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTaskAssociation
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTaskAssociation

(
  @ReminderID int,
  @RefID int,
  @RefType int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TaskAssociations]
  SET
    [RefType] = @RefType
  WHERE ([ReminderID] = @ReminderID AND [RefID] = @RefID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTaskAssociation' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTaskAssociation
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTaskAssociation

(
  @ReminderID int,
  @RefID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TaskAssociations]
  WHERE ([ReminderID] = @ReminderID AND [RefID] = @RefID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTaskAssociation' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTaskAssociation
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTaskAssociation

(
  @ReminderID int,
  @RefID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ReminderID],
    [RefID],
    [RefType],
    [CreatorID],
    [DateCreated]
  FROM [dbo].[TaskAssociations]
  WHERE ([ReminderID] = @ReminderID AND [RefID] = @RefID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTaskAssociation' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTaskAssociation
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTaskAssociation

(
  @ReminderID int,
  @RefID int,
  @RefType int,
  @CreatorID int,
  @DateCreated datetime,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TaskAssociations]
  (
    [ReminderID],
    [RefID],
    [RefType],
    [CreatorID],
    [DateCreated])
  VALUES (
    @ReminderID,
    @RefID,
    @RefType,
    @CreatorID,
    @DateCreated)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTaskAssociation' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTaskAssociation
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTaskAssociation

(
  @ReminderID int,
  @RefID int,
  @RefType int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TaskAssociations]
  SET
    [RefType] = @RefType
  WHERE ([ReminderID] = @ReminderID AND [RefID] = @RefID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTaskAssociation' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTaskAssociation
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTaskAssociation

(
  @ReminderID int,
  @RefID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TaskAssociations]
  WHERE ([ReminderID] = @ReminderID AND [RefID] = @RefID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTaskAssociation' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTaskAssociation
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTaskAssociation

(
  @ReminderID int,
  @RefID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ReminderID],
    [RefID],
    [RefType],
    [CreatorID],
    [DateCreated]
  FROM [dbo].[TaskAssociations]
  WHERE ([ReminderID] = @ReminderID AND [RefID] = @RefID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTaskAssociation' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTaskAssociation
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTaskAssociation

(
  @ReminderID int,
  @RefID int,
  @RefType int,
  @CreatorID int,
  @DateCreated datetime,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TaskAssociations]
  (
    [ReminderID],
    [RefID],
    [RefType],
    [CreatorID],
    [DateCreated])
  VALUES (
    @ReminderID,
    @RefID,
    @RefType,
    @CreatorID,
    @DateCreated)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTaskAssociation' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTaskAssociation
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTaskAssociation

(
  @ReminderID int,
  @RefID int,
  @RefType int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TaskAssociations]
  SET
    [RefType] = @RefType
  WHERE ([ReminderID] = @ReminderID AND [RefID] = @RefID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTaskAssociation' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTaskAssociation
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTaskAssociation

(
  @ReminderID int,
  @RefID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TaskAssociations]
  WHERE ([ReminderID] = @ReminderID AND [RefID] = @RefID)
GO


