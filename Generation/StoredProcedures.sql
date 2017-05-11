IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTask' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTask
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTask

(
  @TaskID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [TaskID],
    [OrganizationID],
    [Name],
    [Description],
    [DueDate],
    [UserID],
    [IsComplete],
    [DateCompleted],
    [ParentID],
    [CreatorID],
    [DateCreated],
    [ModifierID],
    [DateModified],
    [ReminderID],
    [NeedsIndexing],
    [CompletionComment]
  FROM [dbo].[Tasks]
  WHERE ([TaskID] = @TaskID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTask' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTask
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTask

(
  @OrganizationID int,
  @Name nvarchar(1000),
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsComplete bit,
  @DateCompleted datetime,
  @ParentID int,
  @CreatorID int,
  @DateCreated datetime,
  @ModifierID int,
  @DateModified datetime,
  @ReminderID int,
  @NeedsIndexing bit,
  @CompletionComment nvarchar(4000),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Tasks]
  (
    [OrganizationID],
    [Name],
    [Description],
    [DueDate],
    [UserID],
    [IsComplete],
    [DateCompleted],
    [ParentID],
    [CreatorID],
    [DateCreated],
    [ModifierID],
    [DateModified],
    [ReminderID],
    [NeedsIndexing],
    [CompletionComment])
  VALUES (
    @OrganizationID,
    @Name,
    @Description,
    @DueDate,
    @UserID,
    @IsComplete,
    @DateCompleted,
    @ParentID,
    @CreatorID,
    @DateCreated,
    @ModifierID,
    @DateModified,
    @ReminderID,
    @NeedsIndexing,
    @CompletionComment)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTask' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTask
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTask

(
  @TaskID int,
  @OrganizationID int,
  @Name nvarchar(1000),
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsComplete bit,
  @DateCompleted datetime,
  @ParentID int,
  @ModifierID int,
  @DateModified datetime,
  @ReminderID int,
  @NeedsIndexing bit,
  @CompletionComment nvarchar(4000)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Tasks]
  SET
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [Description] = @Description,
    [DueDate] = @DueDate,
    [UserID] = @UserID,
    [IsComplete] = @IsComplete,
    [DateCompleted] = @DateCompleted,
    [ParentID] = @ParentID,
    [ModifierID] = @ModifierID,
    [DateModified] = @DateModified,
    [ReminderID] = @ReminderID,
    [NeedsIndexing] = @NeedsIndexing,
    [CompletionComment] = @CompletionComment
  WHERE ([TaskID] = @TaskID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTask' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTask
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTask

(
  @TaskID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Tasks]
  WHERE ([TaskID] = @TaskID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTasksViewItem

(

)
AS
  SET NOCOUNT OFF;
  SELECT
    [TaskID],
    [OrganizationID],
    [Name],
    [Description],
    [DueDate],
    [UserID],
    [IsComplete],
    [DateCompleted],
    [CompletionComment],
    [ParentID],
    [IsDismissed],
    [HasEmailSent],
    [ReminderDueDate],
    [TaskParentName],
    [UserName],
    [Creator],
    [CreatorID],
    [DateCreated],
    [ModifierID],
    [DateModified],
    [NeedsIndexing]
  FROM [dbo].[TasksView]
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTasksViewItem

(
  @TaskID int,
  @OrganizationID int,
  @Name nvarchar(1000),
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsComplete bit,
  @DateCompleted datetime,
  @CompletionComment nvarchar(4000),
  @ParentID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @ReminderDueDate datetime,
  @TaskParentName nvarchar(1000),
  @UserName nvarchar(201),
  @Creator nvarchar(201),
  @CreatorID int,
  @DateCreated datetime,
  @ModifierID int,
  @DateModified datetime,
  @NeedsIndexing bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TasksView]
  (
    [TaskID],
    [OrganizationID],
    [Name],
    [Description],
    [DueDate],
    [UserID],
    [IsComplete],
    [DateCompleted],
    [CompletionComment],
    [ParentID],
    [IsDismissed],
    [HasEmailSent],
    [ReminderDueDate],
    [TaskParentName],
    [UserName],
    [Creator],
    [CreatorID],
    [DateCreated],
    [ModifierID],
    [DateModified],
    [NeedsIndexing])
  VALUES (
    @TaskID,
    @OrganizationID,
    @Name,
    @Description,
    @DueDate,
    @UserID,
    @IsComplete,
    @DateCompleted,
    @CompletionComment,
    @ParentID,
    @IsDismissed,
    @HasEmailSent,
    @ReminderDueDate,
    @TaskParentName,
    @UserName,
    @Creator,
    @CreatorID,
    @DateCreated,
    @ModifierID,
    @DateModified,
    @NeedsIndexing)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTasksViewItem

(
  @TaskID int,
  @OrganizationID int,
  @Name nvarchar(1000),
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsComplete bit,
  @DateCompleted datetime,
  @CompletionComment nvarchar(4000),
  @ParentID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @ReminderDueDate datetime,
  @TaskParentName nvarchar(1000),
  @UserName nvarchar(201),
  @Creator nvarchar(201),
  @ModifierID int,
  @DateModified datetime,
  @NeedsIndexing bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TasksView]
  SET
    [TaskID] = @TaskID,
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [Description] = @Description,
    [DueDate] = @DueDate,
    [UserID] = @UserID,
    [IsComplete] = @IsComplete,
    [DateCompleted] = @DateCompleted,
    [CompletionComment] = @CompletionComment,
    [ParentID] = @ParentID,
    [IsDismissed] = @IsDismissed,
    [HasEmailSent] = @HasEmailSent,
    [ReminderDueDate] = @ReminderDueDate,
    [TaskParentName] = @TaskParentName,
    [UserName] = @UserName,
    [Creator] = @Creator,
    [ModifierID] = @ModifierID,
    [DateModified] = @DateModified,
    [NeedsIndexing] = @NeedsIndexing
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTasksViewItem

(

)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TasksView]
  WH)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTask' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTask
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTask

(
  @TaskID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [TaskID],
    [OrganizationID],
    [Name],
    [Description],
    [DueDate],
    [UserID],
    [IsComplete],
    [DateCompleted],
    [ParentID],
    [CreatorID],
    [DateCreated],
    [ModifierID],
    [DateModified],
    [ReminderID],
    [NeedsIndexing],
    [CompletionComment]
  FROM [dbo].[Tasks]
  WHERE ([TaskID] = @TaskID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTask' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTask
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTask

(
  @OrganizationID int,
  @Name nvarchar(1000),
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsComplete bit,
  @DateCompleted datetime,
  @ParentID int,
  @CreatorID int,
  @DateCreated datetime,
  @ModifierID int,
  @DateModified datetime,
  @ReminderID int,
  @NeedsIndexing bit,
  @CompletionComment nvarchar(4000),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Tasks]
  (
    [OrganizationID],
    [Name],
    [Description],
    [DueDate],
    [UserID],
    [IsComplete],
    [DateCompleted],
    [ParentID],
    [CreatorID],
    [DateCreated],
    [ModifierID],
    [DateModified],
    [ReminderID],
    [NeedsIndexing],
    [CompletionComment])
  VALUES (
    @OrganizationID,
    @Name,
    @Description,
    @DueDate,
    @UserID,
    @IsComplete,
    @DateCompleted,
    @ParentID,
    @CreatorID,
    @DateCreated,
    @ModifierID,
    @DateModified,
    @ReminderID,
    @NeedsIndexing,
    @CompletionComment)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTask' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTask
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTask

(
  @TaskID int,
  @OrganizationID int,
  @Name nvarchar(1000),
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsComplete bit,
  @DateCompleted datetime,
  @ParentID int,
  @ModifierID int,
  @DateModified datetime,
  @ReminderID int,
  @NeedsIndexing bit,
  @CompletionComment nvarchar(4000)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Tasks]
  SET
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [Description] = @Description,
    [DueDate] = @DueDate,
    [UserID] = @UserID,
    [IsComplete] = @IsComplete,
    [DateCompleted] = @DateCompleted,
    [ParentID] = @ParentID,
    [ModifierID] = @ModifierID,
    [DateModified] = @DateModified,
    [ReminderID] = @ReminderID,
    [NeedsIndexing] = @NeedsIndexing,
    [CompletionComment] = @CompletionComment
  WHERE ([TaskID] = @TaskID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTask' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTask
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTask

(
  @TaskID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Tasks]
  WHERE ([TaskID] = @TaskID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTasksViewItem

(

)
AS
  SET NOCOUNT OFF;
  SELECT
    [TaskID],
    [OrganizationID],
    [Name],
    [Description],
    [DueDate],
    [UserID],
    [IsComplete],
    [DateCompleted],
    [CompletionComment],
    [ParentID],
    [IsDismissed],
    [HasEmailSent],
    [ReminderDueDate],
    [TaskParentName],
    [UserName],
    [Creator],
    [CreatorID],
    [DateCreated],
    [ModifierID],
    [DateModified],
    [NeedsIndexing]
  FROM [dbo].[TasksView]
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTasksViewItem

(
  @TaskID int,
  @OrganizationID int,
  @Name nvarchar(1000),
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsComplete bit,
  @DateCompleted datetime,
  @CompletionComment nvarchar(4000),
  @ParentID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @ReminderDueDate datetime,
  @TaskParentName nvarchar(1000),
  @UserName nvarchar(201),
  @Creator nvarchar(201),
  @CreatorID int,
  @DateCreated datetime,
  @ModifierID int,
  @DateModified datetime,
  @NeedsIndexing bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TasksView]
  (
    [TaskID],
    [OrganizationID],
    [Name],
    [Description],
    [DueDate],
    [UserID],
    [IsComplete],
    [DateCompleted],
    [CompletionComment],
    [ParentID],
    [IsDismissed],
    [HasEmailSent],
    [ReminderDueDate],
    [TaskParentName],
    [UserName],
    [Creator],
    [CreatorID],
    [DateCreated],
    [ModifierID],
    [DateModified],
    [NeedsIndexing])
  VALUES (
    @TaskID,
    @OrganizationID,
    @Name,
    @Description,
    @DueDate,
    @UserID,
    @IsComplete,
    @DateCompleted,
    @CompletionComment,
    @ParentID,
    @IsDismissed,
    @HasEmailSent,
    @ReminderDueDate,
    @TaskParentName,
    @UserName,
    @Creator,
    @CreatorID,
    @DateCreated,
    @ModifierID,
    @DateModified,
    @NeedsIndexing)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTasksViewItem

(
  @TaskID int,
  @OrganizationID int,
  @Name nvarchar(1000),
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsComplete bit,
  @DateCompleted datetime,
  @CompletionComment nvarchar(4000),
  @ParentID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @ReminderDueDate datetime,
  @TaskParentName nvarchar(1000),
  @UserName nvarchar(201),
  @Creator nvarchar(201),
  @ModifierID int,
  @DateModified datetime,
  @NeedsIndexing bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TasksView]
  SET
    [TaskID] = @TaskID,
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [Description] = @Description,
    [DueDate] = @DueDate,
    [UserID] = @UserID,
    [IsComplete] = @IsComplete,
    [DateCompleted] = @DateCompleted,
    [CompletionComment] = @CompletionComment,
    [ParentID] = @ParentID,
    [IsDismissed] = @IsDismissed,
    [HasEmailSent] = @HasEmailSent,
    [ReminderDueDate] = @ReminderDueDate,
    [TaskParentName] = @TaskParentName,
    [UserName] = @UserName,
    [Creator] = @Creator,
    [ModifierID] = @ModifierID,
    [DateModified] = @DateModified,
    [NeedsIndexing] = @NeedsIndexing
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTasksViewItem

(

)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TasksView]
  WH)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTask' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTask
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTask

(
  @TaskID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [TaskID],
    [OrganizationID],
    [Name],
    [Description],
    [DueDate],
    [UserID],
    [IsComplete],
    [DateCompleted],
    [ParentID],
    [CreatorID],
    [DateCreated],
    [ModifierID],
    [DateModified],
    [ReminderID],
    [NeedsIndexing],
    [CompletionComment]
  FROM [dbo].[Tasks]
  WHERE ([TaskID] = @TaskID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTask' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTask
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTask

(
  @OrganizationID int,
  @Name nvarchar(1000),
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsComplete bit,
  @DateCompleted datetime,
  @ParentID int,
  @CreatorID int,
  @DateCreated datetime,
  @ModifierID int,
  @DateModified datetime,
  @ReminderID int,
  @NeedsIndexing bit,
  @CompletionComment nvarchar(4000),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Tasks]
  (
    [OrganizationID],
    [Name],
    [Description],
    [DueDate],
    [UserID],
    [IsComplete],
    [DateCompleted],
    [ParentID],
    [CreatorID],
    [DateCreated],
    [ModifierID],
    [DateModified],
    [ReminderID],
    [NeedsIndexing],
    [CompletionComment])
  VALUES (
    @OrganizationID,
    @Name,
    @Description,
    @DueDate,
    @UserID,
    @IsComplete,
    @DateCompleted,
    @ParentID,
    @CreatorID,
    @DateCreated,
    @ModifierID,
    @DateModified,
    @ReminderID,
    @NeedsIndexing,
    @CompletionComment)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTask' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTask
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTask

(
  @TaskID int,
  @OrganizationID int,
  @Name nvarchar(1000),
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsComplete bit,
  @DateCompleted datetime,
  @ParentID int,
  @ModifierID int,
  @DateModified datetime,
  @ReminderID int,
  @NeedsIndexing bit,
  @CompletionComment nvarchar(4000)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Tasks]
  SET
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [Description] = @Description,
    [DueDate] = @DueDate,
    [UserID] = @UserID,
    [IsComplete] = @IsComplete,
    [DateCompleted] = @DateCompleted,
    [ParentID] = @ParentID,
    [ModifierID] = @ModifierID,
    [DateModified] = @DateModified,
    [ReminderID] = @ReminderID,
    [NeedsIndexing] = @NeedsIndexing,
    [CompletionComment] = @CompletionComment
  WHERE ([TaskID] = @TaskID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTask' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTask
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTask

(
  @TaskID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Tasks]
  WHERE ([TaskID] = @TaskID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTasksViewItem

(

)
AS
  SET NOCOUNT OFF;
  SELECT
    [TaskID],
    [OrganizationID],
    [Name],
    [Description],
    [DueDate],
    [UserID],
    [IsComplete],
    [DateCompleted],
    [CompletionComment],
    [ParentID],
    [IsDismissed],
    [HasEmailSent],
    [ReminderDueDate],
    [TaskParentName],
    [UserName],
    [Creator],
    [CreatorID],
    [DateCreated],
    [ModifierID],
    [DateModified],
    [NeedsIndexing]
  FROM [dbo].[TasksView]
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTasksViewItem

(
  @TaskID int,
  @OrganizationID int,
  @Name nvarchar(1000),
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsComplete bit,
  @DateCompleted datetime,
  @CompletionComment nvarchar(4000),
  @ParentID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @ReminderDueDate datetime,
  @TaskParentName nvarchar(1000),
  @UserName nvarchar(201),
  @Creator nvarchar(201),
  @CreatorID int,
  @DateCreated datetime,
  @ModifierID int,
  @DateModified datetime,
  @NeedsIndexing bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TasksView]
  (
    [TaskID],
    [OrganizationID],
    [Name],
    [Description],
    [DueDate],
    [UserID],
    [IsComplete],
    [DateCompleted],
    [CompletionComment],
    [ParentID],
    [IsDismissed],
    [HasEmailSent],
    [ReminderDueDate],
    [TaskParentName],
    [UserName],
    [Creator],
    [CreatorID],
    [DateCreated],
    [ModifierID],
    [DateModified],
    [NeedsIndexing])
  VALUES (
    @TaskID,
    @OrganizationID,
    @Name,
    @Description,
    @DueDate,
    @UserID,
    @IsComplete,
    @DateCompleted,
    @CompletionComment,
    @ParentID,
    @IsDismissed,
    @HasEmailSent,
    @ReminderDueDate,
    @TaskParentName,
    @UserName,
    @Creator,
    @CreatorID,
    @DateCreated,
    @ModifierID,
    @DateModified,
    @NeedsIndexing)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTasksViewItem

(
  @TaskID int,
  @OrganizationID int,
  @Name nvarchar(1000),
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsComplete bit,
  @DateCompleted datetime,
  @CompletionComment nvarchar(4000),
  @ParentID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @ReminderDueDate datetime,
  @TaskParentName nvarchar(1000),
  @UserName nvarchar(201),
  @Creator nvarchar(201),
  @ModifierID int,
  @DateModified datetime,
  @NeedsIndexing bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TasksView]
  SET
    [TaskID] = @TaskID,
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [Description] = @Description,
    [DueDate] = @DueDate,
    [UserID] = @UserID,
    [IsComplete] = @IsComplete,
    [DateCompleted] = @DateCompleted,
    [CompletionComment] = @CompletionComment,
    [ParentID] = @ParentID,
    [IsDismissed] = @IsDismissed,
    [HasEmailSent] = @HasEmailSent,
    [ReminderDueDate] = @ReminderDueDate,
    [TaskParentName] = @TaskParentName,
    [UserName] = @UserName,
    [Creator] = @Creator,
    [ModifierID] = @ModifierID,
    [DateModified] = @DateModified,
    [NeedsIndexing] = @NeedsIndexing
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTasksViewItem

(

)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TasksView]
  WH)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTask' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTask
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTask

(
  @TaskID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [TaskID],
    [OrganizationID],
    [Name],
    [Description],
    [DueDate],
    [UserID],
    [IsComplete],
    [DateCompleted],
    [ParentID],
    [CreatorID],
    [DateCreated],
    [ModifierID],
    [DateModified],
    [ReminderID],
    [NeedsIndexing],
    [CompletionComment]
  FROM [dbo].[Tasks]
  WHERE ([TaskID] = @TaskID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTask' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTask
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTask

(
  @OrganizationID int,
  @Name nvarchar(1000),
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsComplete bit,
  @DateCompleted datetime,
  @ParentID int,
  @CreatorID int,
  @DateCreated datetime,
  @ModifierID int,
  @DateModified datetime,
  @ReminderID int,
  @NeedsIndexing bit,
  @CompletionComment nvarchar(4000),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Tasks]
  (
    [OrganizationID],
    [Name],
    [Description],
    [DueDate],
    [UserID],
    [IsComplete],
    [DateCompleted],
    [ParentID],
    [CreatorID],
    [DateCreated],
    [ModifierID],
    [DateModified],
    [ReminderID],
    [NeedsIndexing],
    [CompletionComment])
  VALUES (
    @OrganizationID,
    @Name,
    @Description,
    @DueDate,
    @UserID,
    @IsComplete,
    @DateCompleted,
    @ParentID,
    @CreatorID,
    @DateCreated,
    @ModifierID,
    @DateModified,
    @ReminderID,
    @NeedsIndexing,
    @CompletionComment)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTask' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTask
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTask

(
  @TaskID int,
  @OrganizationID int,
  @Name nvarchar(1000),
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsComplete bit,
  @DateCompleted datetime,
  @ParentID int,
  @ModifierID int,
  @DateModified datetime,
  @ReminderID int,
  @NeedsIndexing bit,
  @CompletionComment nvarchar(4000)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Tasks]
  SET
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [Description] = @Description,
    [DueDate] = @DueDate,
    [UserID] = @UserID,
    [IsComplete] = @IsComplete,
    [DateCompleted] = @DateCompleted,
    [ParentID] = @ParentID,
    [ModifierID] = @ModifierID,
    [DateModified] = @DateModified,
    [ReminderID] = @ReminderID,
    [NeedsIndexing] = @NeedsIndexing,
    [CompletionComment] = @CompletionComment
  WHERE ([TaskID] = @TaskID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTask' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTask
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTask

(
  @TaskID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Tasks]
  WHERE ([TaskID] = @TaskID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTasksViewItem

(

)
AS
  SET NOCOUNT OFF;
  SELECT
    [TaskID],
    [OrganizationID],
    [Name],
    [Description],
    [DueDate],
    [UserID],
    [IsComplete],
    [DateCompleted],
    [CompletionComment],
    [ParentID],
    [IsDismissed],
    [HasEmailSent],
    [ReminderDueDate],
    [TaskParentName],
    [UserName],
    [Creator],
    [CreatorID],
    [DateCreated],
    [ModifierID],
    [DateModified],
    [NeedsIndexing]
  FROM [dbo].[TasksView]
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTasksViewItem

(
  @TaskID int,
  @OrganizationID int,
  @Name nvarchar(1000),
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsComplete bit,
  @DateCompleted datetime,
  @CompletionComment nvarchar(4000),
  @ParentID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @ReminderDueDate datetime,
  @TaskParentName nvarchar(1000),
  @UserName nvarchar(201),
  @Creator nvarchar(201),
  @CreatorID int,
  @DateCreated datetime,
  @ModifierID int,
  @DateModified datetime,
  @NeedsIndexing bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TasksView]
  (
    [TaskID],
    [OrganizationID],
    [Name],
    [Description],
    [DueDate],
    [UserID],
    [IsComplete],
    [DateCompleted],
    [CompletionComment],
    [ParentID],
    [IsDismissed],
    [HasEmailSent],
    [ReminderDueDate],
    [TaskParentName],
    [UserName],
    [Creator],
    [CreatorID],
    [DateCreated],
    [ModifierID],
    [DateModified],
    [NeedsIndexing])
  VALUES (
    @TaskID,
    @OrganizationID,
    @Name,
    @Description,
    @DueDate,
    @UserID,
    @IsComplete,
    @DateCompleted,
    @CompletionComment,
    @ParentID,
    @IsDismissed,
    @HasEmailSent,
    @ReminderDueDate,
    @TaskParentName,
    @UserName,
    @Creator,
    @CreatorID,
    @DateCreated,
    @ModifierID,
    @DateModified,
    @NeedsIndexing)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTasksViewItem

(
  @TaskID int,
  @OrganizationID int,
  @Name nvarchar(1000),
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsComplete bit,
  @DateCompleted datetime,
  @CompletionComment nvarchar(4000),
  @ParentID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @ReminderDueDate datetime,
  @TaskParentName nvarchar(1000),
  @UserName nvarchar(201),
  @Creator nvarchar(201),
  @ModifierID int,
  @DateModified datetime,
  @NeedsIndexing bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TasksView]
  SET
    [TaskID] = @TaskID,
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [Description] = @Description,
    [DueDate] = @DueDate,
    [UserID] = @UserID,
    [IsComplete] = @IsComplete,
    [DateCompleted] = @DateCompleted,
    [CompletionComment] = @CompletionComment,
    [ParentID] = @ParentID,
    [IsDismissed] = @IsDismissed,
    [HasEmailSent] = @HasEmailSent,
    [ReminderDueDate] = @ReminderDueDate,
    [TaskParentName] = @TaskParentName,
    [UserName] = @UserName,
    [Creator] = @Creator,
    [ModifierID] = @ModifierID,
    [DateModified] = @DateModified,
    [NeedsIndexing] = @NeedsIndexing
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTasksViewItem

(

)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TasksView]
  WH)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTask' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTask
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTask

(
  @TaskID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [TaskID],
    [OrganizationID],
    [Name],
    [Description],
    [DueDate],
    [UserID],
    [IsComplete],
    [DateCompleted],
    [ParentID],
    [CreatorID],
    [DateCreated],
    [ModifierID],
    [DateModified],
    [ReminderID],
    [NeedsIndexing],
    [CompletionComment]
  FROM [dbo].[Tasks]
  WHERE ([TaskID] = @TaskID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTask' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTask
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTask

(
  @OrganizationID int,
  @Name nvarchar(1000),
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsComplete bit,
  @DateCompleted datetime,
  @ParentID int,
  @CreatorID int,
  @DateCreated datetime,
  @ModifierID int,
  @DateModified datetime,
  @ReminderID int,
  @NeedsIndexing bit,
  @CompletionComment nvarchar(4000),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Tasks]
  (
    [OrganizationID],
    [Name],
    [Description],
    [DueDate],
    [UserID],
    [IsComplete],
    [DateCompleted],
    [ParentID],
    [CreatorID],
    [DateCreated],
    [ModifierID],
    [DateModified],
    [ReminderID],
    [NeedsIndexing],
    [CompletionComment])
  VALUES (
    @OrganizationID,
    @Name,
    @Description,
    @DueDate,
    @UserID,
    @IsComplete,
    @DateCompleted,
    @ParentID,
    @CreatorID,
    @DateCreated,
    @ModifierID,
    @DateModified,
    @ReminderID,
    @NeedsIndexing,
    @CompletionComment)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTask' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTask
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTask

(
  @TaskID int,
  @OrganizationID int,
  @Name nvarchar(1000),
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsComplete bit,
  @DateCompleted datetime,
  @ParentID int,
  @ModifierID int,
  @DateModified datetime,
  @ReminderID int,
  @NeedsIndexing bit,
  @CompletionComment nvarchar(4000)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Tasks]
  SET
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [Description] = @Description,
    [DueDate] = @DueDate,
    [UserID] = @UserID,
    [IsComplete] = @IsComplete,
    [DateCompleted] = @DateCompleted,
    [ParentID] = @ParentID,
    [ModifierID] = @ModifierID,
    [DateModified] = @DateModified,
    [ReminderID] = @ReminderID,
    [NeedsIndexing] = @NeedsIndexing,
    [CompletionComment] = @CompletionComment
  WHERE ([TaskID] = @TaskID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTask' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTask
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTask

(
  @TaskID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Tasks]
  WHERE ([TaskID] = @TaskID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTasksViewItem

(

)
AS
  SET NOCOUNT OFF;
  SELECT
    [TaskID],
    [OrganizationID],
    [Name],
    [Description],
    [DueDate],
    [UserID],
    [IsComplete],
    [DateCompleted],
    [CompletionComment],
    [ParentID],
    [IsDismissed],
    [HasEmailSent],
    [ReminderDueDate],
    [TaskParentName],
    [UserName],
    [Creator],
    [CreatorID],
    [DateCreated],
    [ModifierID],
    [DateModified],
    [NeedsIndexing]
  FROM [dbo].[TasksView]
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTasksViewItem

(
  @TaskID int,
  @OrganizationID int,
  @Name nvarchar(1000),
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsComplete bit,
  @DateCompleted datetime,
  @CompletionComment nvarchar(4000),
  @ParentID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @ReminderDueDate datetime,
  @TaskParentName nvarchar(1000),
  @UserName nvarchar(201),
  @Creator nvarchar(201),
  @CreatorID int,
  @DateCreated datetime,
  @ModifierID int,
  @DateModified datetime,
  @NeedsIndexing bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TasksView]
  (
    [TaskID],
    [OrganizationID],
    [Name],
    [Description],
    [DueDate],
    [UserID],
    [IsComplete],
    [DateCompleted],
    [CompletionComment],
    [ParentID],
    [IsDismissed],
    [HasEmailSent],
    [ReminderDueDate],
    [TaskParentName],
    [UserName],
    [Creator],
    [CreatorID],
    [DateCreated],
    [ModifierID],
    [DateModified],
    [NeedsIndexing])
  VALUES (
    @TaskID,
    @OrganizationID,
    @Name,
    @Description,
    @DueDate,
    @UserID,
    @IsComplete,
    @DateCompleted,
    @CompletionComment,
    @ParentID,
    @IsDismissed,
    @HasEmailSent,
    @ReminderDueDate,
    @TaskParentName,
    @UserName,
    @Creator,
    @CreatorID,
    @DateCreated,
    @ModifierID,
    @DateModified,
    @NeedsIndexing)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTasksViewItem

(
  @TaskID int,
  @OrganizationID int,
  @Name nvarchar(1000),
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsComplete bit,
  @DateCompleted datetime,
  @CompletionComment nvarchar(4000),
  @ParentID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @ReminderDueDate datetime,
  @TaskParentName nvarchar(1000),
  @UserName nvarchar(201),
  @Creator nvarchar(201),
  @ModifierID int,
  @DateModified datetime,
  @NeedsIndexing bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TasksView]
  SET
    [TaskID] = @TaskID,
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [Description] = @Description,
    [DueDate] = @DueDate,
    [UserID] = @UserID,
    [IsComplete] = @IsComplete,
    [DateCompleted] = @DateCompleted,
    [CompletionComment] = @CompletionComment,
    [ParentID] = @ParentID,
    [IsDismissed] = @IsDismissed,
    [HasEmailSent] = @HasEmailSent,
    [ReminderDueDate] = @ReminderDueDate,
    [TaskParentName] = @TaskParentName,
    [UserName] = @UserName,
    [Creator] = @Creator,
    [ModifierID] = @ModifierID,
    [DateModified] = @DateModified,
    [NeedsIndexing] = @NeedsIndexing
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTasksViewItem

(

)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TasksView]
  WH)
GO


