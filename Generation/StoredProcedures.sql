IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTasksViewItem

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
    [IsDismissed],
    [HasEmailSent],
    [ReminderDueDate],
    [TaskParentName],
    [UserName],
    [Creator],
    [CreatorID],
    [DateCreated],
    [ModifierID],
    [DateModified]
  FROM [dbo].[TasksView]
  WHERE ([TaskID] = @TaskID)
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
    [DateModified])
  VALUES (
    @TaskID,
    @OrganizationID,
    @Name,
    @Description,
    @DueDate,
    @UserID,
    @IsComplete,
    @DateCompleted,
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
    @DateModified)

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
  @ParentID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @ReminderDueDate datetime,
  @TaskParentName nvarchar(1000),
  @UserName nvarchar(201),
  @Creator nvarchar(201),
  @ModifierID int,
  @DateModified datetime
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TasksView]
  SET
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [Description] = @Description,
    [DueDate] = @DueDate,
    [UserID] = @UserID,
    [IsComplete] = @IsComplete,
    [DateCompleted] = @DateCompleted,
    [ParentID] = @ParentID,
    [IsDismissed] = @IsDismissed,
    [HasEmailSent] = @HasEmailSent,
    [ReminderDueDate] = @ReminderDueDate,
    [TaskParentName] = @TaskParentName,
    [UserName] = @UserName,
    [Creator] = @Creator,
    [ModifierID] = @ModifierID,
    [DateModified] = @DateModified
  WHERE ([TaskID] = @TaskID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTasksViewItem

(
  @TaskID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TasksView]
  WHERE ([TaskID] = @TaskID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTasksViewItem

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
    [IsDismissed],
    [HasEmailSent],
    [ReminderDueDate],
    [TaskParentName],
    [UserName],
    [Creator],
    [CreatorID],
    [DateCreated],
    [ModifierID],
    [DateModified]
  FROM [dbo].[TasksView]
  WHERE ([TaskID] = @TaskID)
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
    [DateModified])
  VALUES (
    @TaskID,
    @OrganizationID,
    @Name,
    @Description,
    @DueDate,
    @UserID,
    @IsComplete,
    @DateCompleted,
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
    @DateModified)

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
  @ParentID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @ReminderDueDate datetime,
  @TaskParentName nvarchar(1000),
  @UserName nvarchar(201),
  @Creator nvarchar(201),
  @ModifierID int,
  @DateModified datetime
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TasksView]
  SET
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [Description] = @Description,
    [DueDate] = @DueDate,
    [UserID] = @UserID,
    [IsComplete] = @IsComplete,
    [DateCompleted] = @DateCompleted,
    [ParentID] = @ParentID,
    [IsDismissed] = @IsDismissed,
    [HasEmailSent] = @HasEmailSent,
    [ReminderDueDate] = @ReminderDueDate,
    [TaskParentName] = @TaskParentName,
    [UserName] = @UserName,
    [Creator] = @Creator,
    [ModifierID] = @ModifierID,
    [DateModified] = @DateModified
  WHERE ([TaskID] = @TaskID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTasksViewItem

(
  @TaskID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TasksView]
  WHERE ([TaskID] = @TaskID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTasksViewItem

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
    [IsDismissed],
    [HasEmailSent],
    [ReminderDueDate],
    [TaskParentName],
    [UserName],
    [Creator],
    [CreatorID],
    [DateCreated],
    [ModifierID],
    [DateModified]
  FROM [dbo].[TasksView]
  WHERE ([TaskID] = @TaskID)
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
    [DateModified])
  VALUES (
    @TaskID,
    @OrganizationID,
    @Name,
    @Description,
    @DueDate,
    @UserID,
    @IsComplete,
    @DateCompleted,
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
    @DateModified)

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
  @ParentID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @ReminderDueDate datetime,
  @TaskParentName nvarchar(1000),
  @UserName nvarchar(201),
  @Creator nvarchar(201),
  @ModifierID int,
  @DateModified datetime
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TasksView]
  SET
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [Description] = @Description,
    [DueDate] = @DueDate,
    [UserID] = @UserID,
    [IsComplete] = @IsComplete,
    [DateCompleted] = @DateCompleted,
    [ParentID] = @ParentID,
    [IsDismissed] = @IsDismissed,
    [HasEmailSent] = @HasEmailSent,
    [ReminderDueDate] = @ReminderDueDate,
    [TaskParentName] = @TaskParentName,
    [UserName] = @UserName,
    [Creator] = @Creator,
    [ModifierID] = @ModifierID,
    [DateModified] = @DateModified
  WHERE ([TaskID] = @TaskID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTasksViewItem

(
  @TaskID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TasksView]
  WHERE ([TaskID] = @TaskID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTasksViewItem

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
    [IsDismissed],
    [HasEmailSent],
    [ReminderDueDate],
    [TaskParentName],
    [UserName],
    [Creator],
    [CreatorID],
    [DateCreated],
    [ModifierID],
    [DateModified]
  FROM [dbo].[TasksView]
  WHERE ([TaskID] = @TaskID)
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
    [DateModified])
  VALUES (
    @TaskID,
    @OrganizationID,
    @Name,
    @Description,
    @DueDate,
    @UserID,
    @IsComplete,
    @DateCompleted,
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
    @DateModified)

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
  @ParentID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @ReminderDueDate datetime,
  @TaskParentName nvarchar(1000),
  @UserName nvarchar(201),
  @Creator nvarchar(201),
  @ModifierID int,
  @DateModified datetime
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TasksView]
  SET
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [Description] = @Description,
    [DueDate] = @DueDate,
    [UserID] = @UserID,
    [IsComplete] = @IsComplete,
    [DateCompleted] = @DateCompleted,
    [ParentID] = @ParentID,
    [IsDismissed] = @IsDismissed,
    [HasEmailSent] = @HasEmailSent,
    [ReminderDueDate] = @ReminderDueDate,
    [TaskParentName] = @TaskParentName,
    [UserName] = @UserName,
    [Creator] = @Creator,
    [ModifierID] = @ModifierID,
    [DateModified] = @DateModified
  WHERE ([TaskID] = @TaskID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTasksViewItem

(
  @TaskID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TasksView]
  WHERE ([TaskID] = @TaskID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTasksViewItem

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
    [IsDismissed],
    [HasEmailSent],
    [ReminderDueDate],
    [TaskParentName],
    [UserName],
    [Creator],
    [CreatorID],
    [DateCreated],
    [ModifierID],
    [DateModified]
  FROM [dbo].[TasksView]
  WHERE ([TaskID] = @TaskID)
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
    [DateModified])
  VALUES (
    @TaskID,
    @OrganizationID,
    @Name,
    @Description,
    @DueDate,
    @UserID,
    @IsComplete,
    @DateCompleted,
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
    @DateModified)

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
  @ParentID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @ReminderDueDate datetime,
  @TaskParentName nvarchar(1000),
  @UserName nvarchar(201),
  @Creator nvarchar(201),
  @ModifierID int,
  @DateModified datetime
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TasksView]
  SET
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [Description] = @Description,
    [DueDate] = @DueDate,
    [UserID] = @UserID,
    [IsComplete] = @IsComplete,
    [DateCompleted] = @DateCompleted,
    [ParentID] = @ParentID,
    [IsDismissed] = @IsDismissed,
    [HasEmailSent] = @HasEmailSent,
    [ReminderDueDate] = @ReminderDueDate,
    [TaskParentName] = @TaskParentName,
    [UserName] = @UserName,
    [Creator] = @Creator,
    [ModifierID] = @ModifierID,
    [DateModified] = @DateModified
  WHERE ([TaskID] = @TaskID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTasksViewItem

(
  @TaskID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TasksView]
  WHERE ([TaskID] = @TaskID)
GO


