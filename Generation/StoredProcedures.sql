IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTaskLog' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTaskLog
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTaskLog

(
  @TaskLogID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [TaskLogID],
    [TaskID],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID]
  FROM [dbo].[TaskLogs]
  WHERE ([TaskLogID] = @TaskLogID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTaskLog' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTaskLog
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTaskLog

(
  @TaskID int,
  @Description varchar(1000),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TaskLogs]
  (
    [TaskID],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID])
  VALUES (
    @TaskID,
    @Description,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTaskLog' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTaskLog
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTaskLog

(
  @TaskLogID int,
  @TaskID int,
  @Description varchar(1000),
  @DateModified datetime,
  @ModifierID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TaskLogs]
  SET
    [TaskID] = @TaskID,
    [Description] = @Description,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID
  WHERE ([TaskLogID] = @TaskLogID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTaskLog' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTaskLog
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTaskLog

(
  @TaskLogID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TaskLogs]
  WHERE ([TaskLogID] = @TaskLogID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTaskLog' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTaskLog
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTaskLog

(
  @TaskLogID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [TaskLogID],
    [TaskID],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID]
  FROM [dbo].[TaskLogs]
  WHERE ([TaskLogID] = @TaskLogID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTaskLog' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTaskLog
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTaskLog

(
  @TaskID int,
  @Description varchar(1000),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TaskLogs]
  (
    [TaskID],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID])
  VALUES (
    @TaskID,
    @Description,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTaskLog' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTaskLog
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTaskLog

(
  @TaskLogID int,
  @TaskID int,
  @Description varchar(1000),
  @DateModified datetime,
  @ModifierID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TaskLogs]
  SET
    [TaskID] = @TaskID,
    [Description] = @Description,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID
  WHERE ([TaskLogID] = @TaskLogID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTaskLog' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTaskLog
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTaskLog

(
  @TaskLogID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TaskLogs]
  WHERE ([TaskLogID] = @TaskLogID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTaskLog' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTaskLog
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTaskLog

(
  @TaskLogID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [TaskLogID],
    [TaskID],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID]
  FROM [dbo].[TaskLogs]
  WHERE ([TaskLogID] = @TaskLogID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTaskLog' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTaskLog
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTaskLog

(
  @TaskID int,
  @Description varchar(1000),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TaskLogs]
  (
    [TaskID],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID])
  VALUES (
    @TaskID,
    @Description,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTaskLog' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTaskLog
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTaskLog

(
  @TaskLogID int,
  @TaskID int,
  @Description varchar(1000),
  @DateModified datetime,
  @ModifierID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TaskLogs]
  SET
    [TaskID] = @TaskID,
    [Description] = @Description,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID
  WHERE ([TaskLogID] = @TaskLogID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTaskLog' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTaskLog
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTaskLog

(
  @TaskLogID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TaskLogs]
  WHERE ([TaskLogID] = @TaskLogID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTaskLog' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTaskLog
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTaskLog

(
  @TaskLogID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [TaskLogID],
    [TaskID],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID]
  FROM [dbo].[TaskLogs]
  WHERE ([TaskLogID] = @TaskLogID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTaskLog' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTaskLog
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTaskLog

(
  @TaskID int,
  @Description varchar(1000),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TaskLogs]
  (
    [TaskID],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID])
  VALUES (
    @TaskID,
    @Description,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTaskLog' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTaskLog
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTaskLog

(
  @TaskLogID int,
  @TaskID int,
  @Description varchar(1000),
  @DateModified datetime,
  @ModifierID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TaskLogs]
  SET
    [TaskID] = @TaskID,
    [Description] = @Description,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID
  WHERE ([TaskLogID] = @TaskLogID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTaskLog' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTaskLog
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTaskLog

(
  @TaskLogID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TaskLogs]
  WHERE ([TaskLogID] = @TaskLogID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTaskLog' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTaskLog
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTaskLog

(
  @TaskLogID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [TaskLogID],
    [TaskID],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID]
  FROM [dbo].[TaskLogs]
  WHERE ([TaskLogID] = @TaskLogID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTaskLog' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTaskLog
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTaskLog

(
  @TaskID int,
  @Description varchar(1000),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TaskLogs]
  (
    [TaskID],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID])
  VALUES (
    @TaskID,
    @Description,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTaskLog' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTaskLog
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTaskLog

(
  @TaskLogID int,
  @TaskID int,
  @Description varchar(1000),
  @DateModified datetime,
  @ModifierID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TaskLogs]
  SET
    [TaskID] = @TaskID,
    [Description] = @Description,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID
  WHERE ([TaskLogID] = @TaskLogID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTaskLog' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTaskLog
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTaskLog

(
  @TaskLogID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TaskLogs]
  WHERE ([TaskLogID] = @TaskLogID)
GO


