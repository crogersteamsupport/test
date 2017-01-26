IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTaskEmailPost' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTaskEmailPost
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTaskEmailPost

(
  @TaskEmailPostID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [TaskEmailPostID],
    [TaskEmailPostType],
    [HoldTime],
    [DateCreated],
    [ReminderID],
    [CreatorID],
    [LockProcessID],
    [OldUserID]
  FROM [dbo].[TaskEmailPosts]
  WHERE ([TaskEmailPostID] = @TaskEmailPostID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTaskEmailPost' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTaskEmailPost
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTaskEmailPost

(
  @TaskEmailPostType int,
  @HoldTime int,
  @DateCreated datetime,
  @ReminderID int,
  @CreatorID int,
  @LockProcessID varchar(250),
  @OldUserID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TaskEmailPosts]
  (
    [TaskEmailPostType],
    [HoldTime],
    [DateCreated],
    [ReminderID],
    [CreatorID],
    [LockProcessID],
    [OldUserID])
  VALUES (
    @TaskEmailPostType,
    @HoldTime,
    @DateCreated,
    @ReminderID,
    @CreatorID,
    @LockProcessID,
    @OldUserID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTaskEmailPost' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTaskEmailPost
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTaskEmailPost

(
  @TaskEmailPostID int,
  @TaskEmailPostType int,
  @HoldTime int,
  @ReminderID int,
  @LockProcessID varchar(250),
  @OldUserID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TaskEmailPosts]
  SET
    [TaskEmailPostType] = @TaskEmailPostType,
    [HoldTime] = @HoldTime,
    [ReminderID] = @ReminderID,
    [LockProcessID] = @LockProcessID,
    [OldUserID] = @OldUserID
  WHERE ([TaskEmailPostID] = @TaskEmailPostID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTaskEmailPost' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTaskEmailPost
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTaskEmailPost

(
  @TaskEmailPostID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TaskEmailPosts]
  WHERE ([TaskEmailPostID] = @TaskEmailPostID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTaskEmailPostHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTaskEmailPostHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTaskEmailPostHistoryItem

(
  @TaskEmailPostID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [TaskEmailPostID],
    [TaskEmailPostType],
    [HoldTime],
    [DateCreated],
    [ReminderID],
    [CreatorID],
    [LockProcessID],
    [OldUserID]
  FROM [dbo].[TaskEmailPostHistory]
  WHERE ([TaskEmailPostID] = @TaskEmailPostID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTaskEmailPostHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTaskEmailPostHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTaskEmailPostHistoryItem

(
  @TaskEmailPostID int,
  @TaskEmailPostType int,
  @HoldTime int,
  @DateCreated datetime,
  @ReminderID int,
  @CreatorID int,
  @LockProcessID varchar(250),
  @OldUserID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TaskEmailPostHistory]
  (
    [TaskEmailPostID],
    [TaskEmailPostType],
    [HoldTime],
    [DateCreated],
    [ReminderID],
    [CreatorID],
    [LockProcessID],
    [OldUserID])
  VALUES (
    @TaskEmailPostID,
    @TaskEmailPostType,
    @HoldTime,
    @DateCreated,
    @ReminderID,
    @CreatorID,
    @LockProcessID,
    @OldUserID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTaskEmailPostHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTaskEmailPostHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTaskEmailPostHistoryItem

(
  @TaskEmailPostID int,
  @TaskEmailPostType int,
  @HoldTime int,
  @ReminderID int,
  @LockProcessID varchar(250),
  @OldUserID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TaskEmailPostHistory]
  SET
    [TaskEmailPostType] = @TaskEmailPostType,
    [HoldTime] = @HoldTime,
    [ReminderID] = @ReminderID,
    [LockProcessID] = @LockProcessID,
    [OldUserID] = @OldUserID
  WHERE ([TaskEmailPostID] = @TaskEmailPostID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTaskEmailPostHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTaskEmailPostHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTaskEmailPostHistoryItem

(
  @TaskEmailPostID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TaskEmailPostHistory]
  WHERE ([TaskEmailPostID] = @TaskEmailPostID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTaskEmailPost' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTaskEmailPost
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTaskEmailPost

(
  @TaskEmailPostID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [TaskEmailPostID],
    [TaskEmailPostType],
    [HoldTime],
    [DateCreated],
    [ReminderID],
    [CreatorID],
    [LockProcessID],
    [OldUserID]
  FROM [dbo].[TaskEmailPosts]
  WHERE ([TaskEmailPostID] = @TaskEmailPostID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTaskEmailPost' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTaskEmailPost
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTaskEmailPost

(
  @TaskEmailPostType int,
  @HoldTime int,
  @DateCreated datetime,
  @ReminderID int,
  @CreatorID int,
  @LockProcessID varchar(250),
  @OldUserID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TaskEmailPosts]
  (
    [TaskEmailPostType],
    [HoldTime],
    [DateCreated],
    [ReminderID],
    [CreatorID],
    [LockProcessID],
    [OldUserID])
  VALUES (
    @TaskEmailPostType,
    @HoldTime,
    @DateCreated,
    @ReminderID,
    @CreatorID,
    @LockProcessID,
    @OldUserID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTaskEmailPost' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTaskEmailPost
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTaskEmailPost

(
  @TaskEmailPostID int,
  @TaskEmailPostType int,
  @HoldTime int,
  @ReminderID int,
  @LockProcessID varchar(250),
  @OldUserID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TaskEmailPosts]
  SET
    [TaskEmailPostType] = @TaskEmailPostType,
    [HoldTime] = @HoldTime,
    [ReminderID] = @ReminderID,
    [LockProcessID] = @LockProcessID,
    [OldUserID] = @OldUserID
  WHERE ([TaskEmailPostID] = @TaskEmailPostID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTaskEmailPost' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTaskEmailPost
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTaskEmailPost

(
  @TaskEmailPostID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TaskEmailPosts]
  WHERE ([TaskEmailPostID] = @TaskEmailPostID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTaskEmailPostHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTaskEmailPostHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTaskEmailPostHistoryItem

(
  @TaskEmailPostID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [TaskEmailPostID],
    [TaskEmailPostType],
    [HoldTime],
    [DateCreated],
    [ReminderID],
    [CreatorID],
    [LockProcessID],
    [OldUserID]
  FROM [dbo].[TaskEmailPostHistory]
  WHERE ([TaskEmailPostID] = @TaskEmailPostID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTaskEmailPostHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTaskEmailPostHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTaskEmailPostHistoryItem

(
  @TaskEmailPostID int,
  @TaskEmailPostType int,
  @HoldTime int,
  @DateCreated datetime,
  @ReminderID int,
  @CreatorID int,
  @LockProcessID varchar(250),
  @OldUserID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TaskEmailPostHistory]
  (
    [TaskEmailPostID],
    [TaskEmailPostType],
    [HoldTime],
    [DateCreated],
    [ReminderID],
    [CreatorID],
    [LockProcessID],
    [OldUserID])
  VALUES (
    @TaskEmailPostID,
    @TaskEmailPostType,
    @HoldTime,
    @DateCreated,
    @ReminderID,
    @CreatorID,
    @LockProcessID,
    @OldUserID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTaskEmailPostHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTaskEmailPostHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTaskEmailPostHistoryItem

(
  @TaskEmailPostID int,
  @TaskEmailPostType int,
  @HoldTime int,
  @ReminderID int,
  @LockProcessID varchar(250),
  @OldUserID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TaskEmailPostHistory]
  SET
    [TaskEmailPostType] = @TaskEmailPostType,
    [HoldTime] = @HoldTime,
    [ReminderID] = @ReminderID,
    [LockProcessID] = @LockProcessID,
    [OldUserID] = @OldUserID
  WHERE ([TaskEmailPostID] = @TaskEmailPostID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTaskEmailPostHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTaskEmailPostHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTaskEmailPostHistoryItem

(
  @TaskEmailPostID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TaskEmailPostHistory]
  WHERE ([TaskEmailPostID] = @TaskEmailPostID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTaskEmailPost' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTaskEmailPost
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTaskEmailPost

(
  @TaskEmailPostID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [TaskEmailPostID],
    [TaskEmailPostType],
    [HoldTime],
    [DateCreated],
    [ReminderID],
    [CreatorID],
    [LockProcessID],
    [OldUserID]
  FROM [dbo].[TaskEmailPosts]
  WHERE ([TaskEmailPostID] = @TaskEmailPostID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTaskEmailPost' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTaskEmailPost
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTaskEmailPost

(
  @TaskEmailPostType int,
  @HoldTime int,
  @DateCreated datetime,
  @ReminderID int,
  @CreatorID int,
  @LockProcessID varchar(250),
  @OldUserID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TaskEmailPosts]
  (
    [TaskEmailPostType],
    [HoldTime],
    [DateCreated],
    [ReminderID],
    [CreatorID],
    [LockProcessID],
    [OldUserID])
  VALUES (
    @TaskEmailPostType,
    @HoldTime,
    @DateCreated,
    @ReminderID,
    @CreatorID,
    @LockProcessID,
    @OldUserID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTaskEmailPost' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTaskEmailPost
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTaskEmailPost

(
  @TaskEmailPostID int,
  @TaskEmailPostType int,
  @HoldTime int,
  @ReminderID int,
  @LockProcessID varchar(250),
  @OldUserID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TaskEmailPosts]
  SET
    [TaskEmailPostType] = @TaskEmailPostType,
    [HoldTime] = @HoldTime,
    [ReminderID] = @ReminderID,
    [LockProcessID] = @LockProcessID,
    [OldUserID] = @OldUserID
  WHERE ([TaskEmailPostID] = @TaskEmailPostID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTaskEmailPost' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTaskEmailPost
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTaskEmailPost

(
  @TaskEmailPostID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TaskEmailPosts]
  WHERE ([TaskEmailPostID] = @TaskEmailPostID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTaskEmailPostHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTaskEmailPostHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTaskEmailPostHistoryItem

(
  @TaskEmailPostID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [TaskEmailPostID],
    [TaskEmailPostType],
    [HoldTime],
    [DateCreated],
    [ReminderID],
    [CreatorID],
    [LockProcessID],
    [OldUserID]
  FROM [dbo].[TaskEmailPostHistory]
  WHERE ([TaskEmailPostID] = @TaskEmailPostID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTaskEmailPostHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTaskEmailPostHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTaskEmailPostHistoryItem

(
  @TaskEmailPostID int,
  @TaskEmailPostType int,
  @HoldTime int,
  @DateCreated datetime,
  @ReminderID int,
  @CreatorID int,
  @LockProcessID varchar(250),
  @OldUserID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TaskEmailPostHistory]
  (
    [TaskEmailPostID],
    [TaskEmailPostType],
    [HoldTime],
    [DateCreated],
    [ReminderID],
    [CreatorID],
    [LockProcessID],
    [OldUserID])
  VALUES (
    @TaskEmailPostID,
    @TaskEmailPostType,
    @HoldTime,
    @DateCreated,
    @ReminderID,
    @CreatorID,
    @LockProcessID,
    @OldUserID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTaskEmailPostHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTaskEmailPostHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTaskEmailPostHistoryItem

(
  @TaskEmailPostID int,
  @TaskEmailPostType int,
  @HoldTime int,
  @ReminderID int,
  @LockProcessID varchar(250),
  @OldUserID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TaskEmailPostHistory]
  SET
    [TaskEmailPostType] = @TaskEmailPostType,
    [HoldTime] = @HoldTime,
    [ReminderID] = @ReminderID,
    [LockProcessID] = @LockProcessID,
    [OldUserID] = @OldUserID
  WHERE ([TaskEmailPostID] = @TaskEmailPostID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTaskEmailPostHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTaskEmailPostHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTaskEmailPostHistoryItem

(
  @TaskEmailPostID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TaskEmailPostHistory]
  WHERE ([TaskEmailPostID] = @TaskEmailPostID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTaskEmailPost' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTaskEmailPost
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTaskEmailPost

(
  @TaskEmailPostID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [TaskEmailPostID],
    [TaskEmailPostType],
    [HoldTime],
    [DateCreated],
    [ReminderID],
    [CreatorID],
    [LockProcessID],
    [OldUserID]
  FROM [dbo].[TaskEmailPosts]
  WHERE ([TaskEmailPostID] = @TaskEmailPostID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTaskEmailPost' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTaskEmailPost
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTaskEmailPost

(
  @TaskEmailPostType int,
  @HoldTime int,
  @DateCreated datetime,
  @ReminderID int,
  @CreatorID int,
  @LockProcessID varchar(250),
  @OldUserID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TaskEmailPosts]
  (
    [TaskEmailPostType],
    [HoldTime],
    [DateCreated],
    [ReminderID],
    [CreatorID],
    [LockProcessID],
    [OldUserID])
  VALUES (
    @TaskEmailPostType,
    @HoldTime,
    @DateCreated,
    @ReminderID,
    @CreatorID,
    @LockProcessID,
    @OldUserID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTaskEmailPost' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTaskEmailPost
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTaskEmailPost

(
  @TaskEmailPostID int,
  @TaskEmailPostType int,
  @HoldTime int,
  @ReminderID int,
  @LockProcessID varchar(250),
  @OldUserID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TaskEmailPosts]
  SET
    [TaskEmailPostType] = @TaskEmailPostType,
    [HoldTime] = @HoldTime,
    [ReminderID] = @ReminderID,
    [LockProcessID] = @LockProcessID,
    [OldUserID] = @OldUserID
  WHERE ([TaskEmailPostID] = @TaskEmailPostID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTaskEmailPost' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTaskEmailPost
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTaskEmailPost

(
  @TaskEmailPostID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TaskEmailPosts]
  WHERE ([TaskEmailPostID] = @TaskEmailPostID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTaskEmailPostHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTaskEmailPostHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTaskEmailPostHistoryItem

(
  @TaskEmailPostID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [TaskEmailPostID],
    [TaskEmailPostType],
    [HoldTime],
    [DateCreated],
    [ReminderID],
    [CreatorID],
    [LockProcessID],
    [OldUserID]
  FROM [dbo].[TaskEmailPostHistory]
  WHERE ([TaskEmailPostID] = @TaskEmailPostID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTaskEmailPostHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTaskEmailPostHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTaskEmailPostHistoryItem

(
  @TaskEmailPostID int,
  @TaskEmailPostType int,
  @HoldTime int,
  @DateCreated datetime,
  @ReminderID int,
  @CreatorID int,
  @LockProcessID varchar(250),
  @OldUserID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TaskEmailPostHistory]
  (
    [TaskEmailPostID],
    [TaskEmailPostType],
    [HoldTime],
    [DateCreated],
    [ReminderID],
    [CreatorID],
    [LockProcessID],
    [OldUserID])
  VALUES (
    @TaskEmailPostID,
    @TaskEmailPostType,
    @HoldTime,
    @DateCreated,
    @ReminderID,
    @CreatorID,
    @LockProcessID,
    @OldUserID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTaskEmailPostHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTaskEmailPostHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTaskEmailPostHistoryItem

(
  @TaskEmailPostID int,
  @TaskEmailPostType int,
  @HoldTime int,
  @ReminderID int,
  @LockProcessID varchar(250),
  @OldUserID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TaskEmailPostHistory]
  SET
    [TaskEmailPostType] = @TaskEmailPostType,
    [HoldTime] = @HoldTime,
    [ReminderID] = @ReminderID,
    [LockProcessID] = @LockProcessID,
    [OldUserID] = @OldUserID
  WHERE ([TaskEmailPostID] = @TaskEmailPostID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTaskEmailPostHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTaskEmailPostHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTaskEmailPostHistoryItem

(
  @TaskEmailPostID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TaskEmailPostHistory]
  WHERE ([TaskEmailPostID] = @TaskEmailPostID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTaskEmailPost' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTaskEmailPost
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTaskEmailPost

(
  @TaskEmailPostID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [TaskEmailPostID],
    [TaskEmailPostType],
    [HoldTime],
    [DateCreated],
    [ReminderID],
    [CreatorID],
    [LockProcessID],
    [OldUserID]
  FROM [dbo].[TaskEmailPosts]
  WHERE ([TaskEmailPostID] = @TaskEmailPostID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTaskEmailPost' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTaskEmailPost
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTaskEmailPost

(
  @TaskEmailPostType int,
  @HoldTime int,
  @DateCreated datetime,
  @ReminderID int,
  @CreatorID int,
  @LockProcessID varchar(250),
  @OldUserID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TaskEmailPosts]
  (
    [TaskEmailPostType],
    [HoldTime],
    [DateCreated],
    [ReminderID],
    [CreatorID],
    [LockProcessID],
    [OldUserID])
  VALUES (
    @TaskEmailPostType,
    @HoldTime,
    @DateCreated,
    @ReminderID,
    @CreatorID,
    @LockProcessID,
    @OldUserID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTaskEmailPost' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTaskEmailPost
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTaskEmailPost

(
  @TaskEmailPostID int,
  @TaskEmailPostType int,
  @HoldTime int,
  @ReminderID int,
  @LockProcessID varchar(250),
  @OldUserID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TaskEmailPosts]
  SET
    [TaskEmailPostType] = @TaskEmailPostType,
    [HoldTime] = @HoldTime,
    [ReminderID] = @ReminderID,
    [LockProcessID] = @LockProcessID,
    [OldUserID] = @OldUserID
  WHERE ([TaskEmailPostID] = @TaskEmailPostID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTaskEmailPost' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTaskEmailPost
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTaskEmailPost

(
  @TaskEmailPostID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TaskEmailPosts]
  WHERE ([TaskEmailPostID] = @TaskEmailPostID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTaskEmailPostHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTaskEmailPostHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTaskEmailPostHistoryItem

(
  @TaskEmailPostID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [TaskEmailPostID],
    [TaskEmailPostType],
    [HoldTime],
    [DateCreated],
    [ReminderID],
    [CreatorID],
    [LockProcessID],
    [OldUserID]
  FROM [dbo].[TaskEmailPostHistory]
  WHERE ([TaskEmailPostID] = @TaskEmailPostID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTaskEmailPostHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTaskEmailPostHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTaskEmailPostHistoryItem

(
  @TaskEmailPostID int,
  @TaskEmailPostType int,
  @HoldTime int,
  @DateCreated datetime,
  @ReminderID int,
  @CreatorID int,
  @LockProcessID varchar(250),
  @OldUserID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TaskEmailPostHistory]
  (
    [TaskEmailPostID],
    [TaskEmailPostType],
    [HoldTime],
    [DateCreated],
    [ReminderID],
    [CreatorID],
    [LockProcessID],
    [OldUserID])
  VALUES (
    @TaskEmailPostID,
    @TaskEmailPostType,
    @HoldTime,
    @DateCreated,
    @ReminderID,
    @CreatorID,
    @LockProcessID,
    @OldUserID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTaskEmailPostHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTaskEmailPostHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTaskEmailPostHistoryItem

(
  @TaskEmailPostID int,
  @TaskEmailPostType int,
  @HoldTime int,
  @ReminderID int,
  @LockProcessID varchar(250),
  @OldUserID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TaskEmailPostHistory]
  SET
    [TaskEmailPostType] = @TaskEmailPostType,
    [HoldTime] = @HoldTime,
    [ReminderID] = @ReminderID,
    [LockProcessID] = @LockProcessID,
    [OldUserID] = @OldUserID
  WHERE ([TaskEmailPostID] = @TaskEmailPostID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTaskEmailPostHistoryItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTaskEmailPostHistoryItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTaskEmailPostHistoryItem

(
  @TaskEmailPostID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TaskEmailPostHistory]
  WHERE ([TaskEmailPostID] = @TaskEmailPostID)
GO


