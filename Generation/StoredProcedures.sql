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
    [LockProcessID]
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
    [LockProcessID])
  VALUES (
    @TaskEmailPostType,
    @HoldTime,
    @DateCreated,
    @ReminderID,
    @CreatorID,
    @LockProcessID)

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
  @LockProcessID varchar(250)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TaskEmailPosts]
  SET
    [TaskEmailPostType] = @TaskEmailPostType,
    [HoldTime] = @HoldTime,
    [ReminderID] = @ReminderID,
    [LockProcessID] = @LockProcessID
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
    [LockProcessID]
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
    [LockProcessID])
  VALUES (
    @TaskEmailPostType,
    @HoldTime,
    @DateCreated,
    @ReminderID,
    @CreatorID,
    @LockProcessID)

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
  @LockProcessID varchar(250)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TaskEmailPosts]
  SET
    [TaskEmailPostType] = @TaskEmailPostType,
    [HoldTime] = @HoldTime,
    [ReminderID] = @ReminderID,
    [LockProcessID] = @LockProcessID
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
    [LockProcessID]
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
    [LockProcessID])
  VALUES (
    @TaskEmailPostType,
    @HoldTime,
    @DateCreated,
    @ReminderID,
    @CreatorID,
    @LockProcessID)

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
  @LockProcessID varchar(250)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TaskEmailPosts]
  SET
    [TaskEmailPostType] = @TaskEmailPostType,
    [HoldTime] = @HoldTime,
    [ReminderID] = @ReminderID,
    [LockProcessID] = @LockProcessID
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
    [LockProcessID]
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
    [LockProcessID])
  VALUES (
    @TaskEmailPostType,
    @HoldTime,
    @DateCreated,
    @ReminderID,
    @CreatorID,
    @LockProcessID)

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
  @LockProcessID varchar(250)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TaskEmailPosts]
  SET
    [TaskEmailPostType] = @TaskEmailPostType,
    [HoldTime] = @HoldTime,
    [ReminderID] = @ReminderID,
    [LockProcessID] = @LockProcessID
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
    [LockProcessID]
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
    [LockProcessID])
  VALUES (
    @TaskEmailPostType,
    @HoldTime,
    @DateCreated,
    @ReminderID,
    @CreatorID,
    @LockProcessID)

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
  @LockProcessID varchar(250)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TaskEmailPosts]
  SET
    [TaskEmailPostType] = @TaskEmailPostType,
    [HoldTime] = @HoldTime,
    [ReminderID] = @ReminderID,
    [LockProcessID] = @LockProcessID
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


