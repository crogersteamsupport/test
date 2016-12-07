IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectReminder' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectReminder
GO

CREATE PROCEDURE dbo.uspGeneratedSelectReminder

(
  @ReminderID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ReminderID],
    [OrganizationID],
    [RefType],
    [RefID],
    [Description],
    [DueDate],
    [UserID],
    [IsDismissed],
    [HasEmailSent],
    [CreatorID],
    [DateCreated],
    [TaskName],
    [TaskDueDate],
    [TaskIsComplete],
    [TaskDateCompleted],
    [TaskParentID]
  FROM [dbo].[Reminders]
  WHERE ([ReminderID] = @ReminderID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertReminder' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertReminder
GO

CREATE PROCEDURE dbo.uspGeneratedInsertReminder

(
  @OrganizationID int,
  @RefType int,
  @RefID int,
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @CreatorID int,
  @DateCreated datetime,
  @TaskName nvarchar(1000),
  @TaskDueDate datetime,
  @TaskIsComplete bit,
  @TaskDateCompleted datetime,
  @TaskParentID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Reminders]
  (
    [OrganizationID],
    [RefType],
    [RefID],
    [Description],
    [DueDate],
    [UserID],
    [IsDismissed],
    [HasEmailSent],
    [CreatorID],
    [DateCreated],
    [TaskName],
    [TaskDueDate],
    [TaskIsComplete],
    [TaskDateCompleted],
    [TaskParentID])
  VALUES (
    @OrganizationID,
    @RefType,
    @RefID,
    @Description,
    @DueDate,
    @UserID,
    @IsDismissed,
    @HasEmailSent,
    @CreatorID,
    @DateCreated,
    @TaskName,
    @TaskDueDate,
    @TaskIsComplete,
    @TaskDateCompleted,
    @TaskParentID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateReminder' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateReminder
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateReminder

(
  @ReminderID int,
  @OrganizationID int,
  @RefType int,
  @RefID int,
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @TaskName nvarchar(1000),
  @TaskDueDate datetime,
  @TaskIsComplete bit,
  @TaskDateCompleted datetime,
  @TaskParentID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Reminders]
  SET
    [OrganizationID] = @OrganizationID,
    [RefType] = @RefType,
    [RefID] = @RefID,
    [Description] = @Description,
    [DueDate] = @DueDate,
    [UserID] = @UserID,
    [IsDismissed] = @IsDismissed,
    [HasEmailSent] = @HasEmailSent,
    [TaskName] = @TaskName,
    [TaskDueDate] = @TaskDueDate,
    [TaskIsComplete] = @TaskIsComplete,
    [TaskDateCompleted] = @TaskDateCompleted,
    [TaskParentID] = @TaskParentID
  WHERE ([ReminderID] = @ReminderID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteReminder' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteReminder
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteReminder

(
  @ReminderID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Reminders]
  WHERE ([ReminderID] = @ReminderID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTasksViewItem

(

)
AS
  SET NOCOUNT OFF;
  SELECT
    [ReminderID],
    [OrganizationID],
    [RefType],
    [RefID],
    [Description],
    [DueDate],
    [UserID],
    [IsDismissed],
    [HasEmailSent],
    [CreatorID],
    [DateCreated],
    [TaskName],
    [TaskDueDate],
    [TaskIsComplete],
    [TaskDateCompleted],
    [TaskParentID],
    [TaskParentName],
    [UserName],
    [Creator]
  FROM [dbo].[TasksView]
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTasksViewItem

(
  @ReminderID int,
  @OrganizationID int,
  @RefType int,
  @RefID int,
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @CreatorID int,
  @DateCreated datetime,
  @TaskName nvarchar(1000),
  @TaskDueDate datetime,
  @TaskIsComplete bit,
  @TaskDateCompleted datetime,
  @TaskParentID int,
  @TaskParentName nvarchar(1000),
  @UserName nvarchar(201),
  @Creator nvarchar(201),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TasksView]
  (
    [ReminderID],
    [OrganizationID],
    [RefType],
    [RefID],
    [Description],
    [DueDate],
    [UserID],
    [IsDismissed],
    [HasEmailSent],
    [CreatorID],
    [DateCreated],
    [TaskName],
    [TaskDueDate],
    [TaskIsComplete],
    [TaskDateCompleted],
    [TaskParentID],
    [TaskParentName],
    [UserName],
    [Creator])
  VALUES (
    @ReminderID,
    @OrganizationID,
    @RefType,
    @RefID,
    @Description,
    @DueDate,
    @UserID,
    @IsDismissed,
    @HasEmailSent,
    @CreatorID,
    @DateCreated,
    @TaskName,
    @TaskDueDate,
    @TaskIsComplete,
    @TaskDateCompleted,
    @TaskParentID,
    @TaskParentName,
    @UserName,
    @Creator)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTasksViewItem

(
  @ReminderID int,
  @OrganizationID int,
  @RefType int,
  @RefID int,
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @TaskName nvarchar(1000),
  @TaskDueDate datetime,
  @TaskIsComplete bit,
  @TaskDateCompleted datetime,
  @TaskParentID int,
  @TaskParentName nvarchar(1000),
  @UserName nvarchar(201),
  @Creator nvarchar(201)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TasksView]
  SET
    [ReminderID] = @ReminderID,
    [OrganizationID] = @OrganizationID,
    [RefType] = @RefType,
    [RefID] = @RefID,
    [Description] = @Description,
    [DueDate] = @DueDate,
    [UserID] = @UserID,
    [IsDismissed] = @IsDismissed,
    [HasEmailSent] = @HasEmailSent,
    [TaskName] = @TaskName,
    [TaskDueDate] = @TaskDueDate,
    [TaskIsComplete] = @TaskIsComplete,
    [TaskDateCompleted] = @TaskDateCompleted,
    [TaskParentID] = @TaskParentID,
    [TaskParentName] = @TaskParentName,
    [UserName] = @UserName,
    [Creator] = @Creator
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


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectReminder' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectReminder
GO

CREATE PROCEDURE dbo.uspGeneratedSelectReminder

(
  @ReminderID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ReminderID],
    [OrganizationID],
    [RefType],
    [RefID],
    [Description],
    [DueDate],
    [UserID],
    [IsDismissed],
    [HasEmailSent],
    [CreatorID],
    [DateCreated],
    [TaskName],
    [TaskDueDate],
    [TaskIsComplete],
    [TaskDateCompleted],
    [TaskParentID]
  FROM [dbo].[Reminders]
  WHERE ([ReminderID] = @ReminderID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertReminder' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertReminder
GO

CREATE PROCEDURE dbo.uspGeneratedInsertReminder

(
  @OrganizationID int,
  @RefType int,
  @RefID int,
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @CreatorID int,
  @DateCreated datetime,
  @TaskName nvarchar(1000),
  @TaskDueDate datetime,
  @TaskIsComplete bit,
  @TaskDateCompleted datetime,
  @TaskParentID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Reminders]
  (
    [OrganizationID],
    [RefType],
    [RefID],
    [Description],
    [DueDate],
    [UserID],
    [IsDismissed],
    [HasEmailSent],
    [CreatorID],
    [DateCreated],
    [TaskName],
    [TaskDueDate],
    [TaskIsComplete],
    [TaskDateCompleted],
    [TaskParentID])
  VALUES (
    @OrganizationID,
    @RefType,
    @RefID,
    @Description,
    @DueDate,
    @UserID,
    @IsDismissed,
    @HasEmailSent,
    @CreatorID,
    @DateCreated,
    @TaskName,
    @TaskDueDate,
    @TaskIsComplete,
    @TaskDateCompleted,
    @TaskParentID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateReminder' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateReminder
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateReminder

(
  @ReminderID int,
  @OrganizationID int,
  @RefType int,
  @RefID int,
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @TaskName nvarchar(1000),
  @TaskDueDate datetime,
  @TaskIsComplete bit,
  @TaskDateCompleted datetime,
  @TaskParentID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Reminders]
  SET
    [OrganizationID] = @OrganizationID,
    [RefType] = @RefType,
    [RefID] = @RefID,
    [Description] = @Description,
    [DueDate] = @DueDate,
    [UserID] = @UserID,
    [IsDismissed] = @IsDismissed,
    [HasEmailSent] = @HasEmailSent,
    [TaskName] = @TaskName,
    [TaskDueDate] = @TaskDueDate,
    [TaskIsComplete] = @TaskIsComplete,
    [TaskDateCompleted] = @TaskDateCompleted,
    [TaskParentID] = @TaskParentID
  WHERE ([ReminderID] = @ReminderID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteReminder' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteReminder
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteReminder

(
  @ReminderID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Reminders]
  WHERE ([ReminderID] = @ReminderID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTasksViewItem

(

)
AS
  SET NOCOUNT OFF;
  SELECT
    [ReminderID],
    [OrganizationID],
    [RefType],
    [RefID],
    [Description],
    [DueDate],
    [UserID],
    [IsDismissed],
    [HasEmailSent],
    [CreatorID],
    [DateCreated],
    [TaskName],
    [TaskDueDate],
    [TaskIsComplete],
    [TaskDateCompleted],
    [TaskParentID],
    [TaskParentName],
    [UserName],
    [Creator]
  FROM [dbo].[TasksView]
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTasksViewItem

(
  @ReminderID int,
  @OrganizationID int,
  @RefType int,
  @RefID int,
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @CreatorID int,
  @DateCreated datetime,
  @TaskName nvarchar(1000),
  @TaskDueDate datetime,
  @TaskIsComplete bit,
  @TaskDateCompleted datetime,
  @TaskParentID int,
  @TaskParentName nvarchar(1000),
  @UserName nvarchar(201),
  @Creator nvarchar(201),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TasksView]
  (
    [ReminderID],
    [OrganizationID],
    [RefType],
    [RefID],
    [Description],
    [DueDate],
    [UserID],
    [IsDismissed],
    [HasEmailSent],
    [CreatorID],
    [DateCreated],
    [TaskName],
    [TaskDueDate],
    [TaskIsComplete],
    [TaskDateCompleted],
    [TaskParentID],
    [TaskParentName],
    [UserName],
    [Creator])
  VALUES (
    @ReminderID,
    @OrganizationID,
    @RefType,
    @RefID,
    @Description,
    @DueDate,
    @UserID,
    @IsDismissed,
    @HasEmailSent,
    @CreatorID,
    @DateCreated,
    @TaskName,
    @TaskDueDate,
    @TaskIsComplete,
    @TaskDateCompleted,
    @TaskParentID,
    @TaskParentName,
    @UserName,
    @Creator)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTasksViewItem

(
  @ReminderID int,
  @OrganizationID int,
  @RefType int,
  @RefID int,
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @TaskName nvarchar(1000),
  @TaskDueDate datetime,
  @TaskIsComplete bit,
  @TaskDateCompleted datetime,
  @TaskParentID int,
  @TaskParentName nvarchar(1000),
  @UserName nvarchar(201),
  @Creator nvarchar(201)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TasksView]
  SET
    [ReminderID] = @ReminderID,
    [OrganizationID] = @OrganizationID,
    [RefType] = @RefType,
    [RefID] = @RefID,
    [Description] = @Description,
    [DueDate] = @DueDate,
    [UserID] = @UserID,
    [IsDismissed] = @IsDismissed,
    [HasEmailSent] = @HasEmailSent,
    [TaskName] = @TaskName,
    [TaskDueDate] = @TaskDueDate,
    [TaskIsComplete] = @TaskIsComplete,
    [TaskDateCompleted] = @TaskDateCompleted,
    [TaskParentID] = @TaskParentID,
    [TaskParentName] = @TaskParentName,
    [UserName] = @UserName,
    [Creator] = @Creator
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


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectReminder' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectReminder
GO

CREATE PROCEDURE dbo.uspGeneratedSelectReminder

(
  @ReminderID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ReminderID],
    [OrganizationID],
    [RefType],
    [RefID],
    [Description],
    [DueDate],
    [UserID],
    [IsDismissed],
    [HasEmailSent],
    [CreatorID],
    [DateCreated],
    [TaskName],
    [TaskDueDate],
    [TaskIsComplete],
    [TaskDateCompleted],
    [TaskParentID]
  FROM [dbo].[Reminders]
  WHERE ([ReminderID] = @ReminderID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertReminder' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertReminder
GO

CREATE PROCEDURE dbo.uspGeneratedInsertReminder

(
  @OrganizationID int,
  @RefType int,
  @RefID int,
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @CreatorID int,
  @DateCreated datetime,
  @TaskName nvarchar(1000),
  @TaskDueDate datetime,
  @TaskIsComplete bit,
  @TaskDateCompleted datetime,
  @TaskParentID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Reminders]
  (
    [OrganizationID],
    [RefType],
    [RefID],
    [Description],
    [DueDate],
    [UserID],
    [IsDismissed],
    [HasEmailSent],
    [CreatorID],
    [DateCreated],
    [TaskName],
    [TaskDueDate],
    [TaskIsComplete],
    [TaskDateCompleted],
    [TaskParentID])
  VALUES (
    @OrganizationID,
    @RefType,
    @RefID,
    @Description,
    @DueDate,
    @UserID,
    @IsDismissed,
    @HasEmailSent,
    @CreatorID,
    @DateCreated,
    @TaskName,
    @TaskDueDate,
    @TaskIsComplete,
    @TaskDateCompleted,
    @TaskParentID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateReminder' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateReminder
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateReminder

(
  @ReminderID int,
  @OrganizationID int,
  @RefType int,
  @RefID int,
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @TaskName nvarchar(1000),
  @TaskDueDate datetime,
  @TaskIsComplete bit,
  @TaskDateCompleted datetime,
  @TaskParentID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Reminders]
  SET
    [OrganizationID] = @OrganizationID,
    [RefType] = @RefType,
    [RefID] = @RefID,
    [Description] = @Description,
    [DueDate] = @DueDate,
    [UserID] = @UserID,
    [IsDismissed] = @IsDismissed,
    [HasEmailSent] = @HasEmailSent,
    [TaskName] = @TaskName,
    [TaskDueDate] = @TaskDueDate,
    [TaskIsComplete] = @TaskIsComplete,
    [TaskDateCompleted] = @TaskDateCompleted,
    [TaskParentID] = @TaskParentID
  WHERE ([ReminderID] = @ReminderID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteReminder' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteReminder
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteReminder

(
  @ReminderID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Reminders]
  WHERE ([ReminderID] = @ReminderID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTasksViewItem

(

)
AS
  SET NOCOUNT OFF;
  SELECT
    [ReminderID],
    [OrganizationID],
    [RefType],
    [RefID],
    [Description],
    [DueDate],
    [UserID],
    [IsDismissed],
    [HasEmailSent],
    [CreatorID],
    [DateCreated],
    [TaskName],
    [TaskDueDate],
    [TaskIsComplete],
    [TaskDateCompleted],
    [TaskParentID],
    [TaskParentName],
    [UserName],
    [Creator]
  FROM [dbo].[TasksView]
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTasksViewItem

(
  @ReminderID int,
  @OrganizationID int,
  @RefType int,
  @RefID int,
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @CreatorID int,
  @DateCreated datetime,
  @TaskName nvarchar(1000),
  @TaskDueDate datetime,
  @TaskIsComplete bit,
  @TaskDateCompleted datetime,
  @TaskParentID int,
  @TaskParentName nvarchar(1000),
  @UserName nvarchar(201),
  @Creator nvarchar(201),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TasksView]
  (
    [ReminderID],
    [OrganizationID],
    [RefType],
    [RefID],
    [Description],
    [DueDate],
    [UserID],
    [IsDismissed],
    [HasEmailSent],
    [CreatorID],
    [DateCreated],
    [TaskName],
    [TaskDueDate],
    [TaskIsComplete],
    [TaskDateCompleted],
    [TaskParentID],
    [TaskParentName],
    [UserName],
    [Creator])
  VALUES (
    @ReminderID,
    @OrganizationID,
    @RefType,
    @RefID,
    @Description,
    @DueDate,
    @UserID,
    @IsDismissed,
    @HasEmailSent,
    @CreatorID,
    @DateCreated,
    @TaskName,
    @TaskDueDate,
    @TaskIsComplete,
    @TaskDateCompleted,
    @TaskParentID,
    @TaskParentName,
    @UserName,
    @Creator)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTasksViewItem

(
  @ReminderID int,
  @OrganizationID int,
  @RefType int,
  @RefID int,
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @TaskName nvarchar(1000),
  @TaskDueDate datetime,
  @TaskIsComplete bit,
  @TaskDateCompleted datetime,
  @TaskParentID int,
  @TaskParentName nvarchar(1000),
  @UserName nvarchar(201),
  @Creator nvarchar(201)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TasksView]
  SET
    [ReminderID] = @ReminderID,
    [OrganizationID] = @OrganizationID,
    [RefType] = @RefType,
    [RefID] = @RefID,
    [Description] = @Description,
    [DueDate] = @DueDate,
    [UserID] = @UserID,
    [IsDismissed] = @IsDismissed,
    [HasEmailSent] = @HasEmailSent,
    [TaskName] = @TaskName,
    [TaskDueDate] = @TaskDueDate,
    [TaskIsComplete] = @TaskIsComplete,
    [TaskDateCompleted] = @TaskDateCompleted,
    [TaskParentID] = @TaskParentID,
    [TaskParentName] = @TaskParentName,
    [UserName] = @UserName,
    [Creator] = @Creator
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


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectReminder' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectReminder
GO

CREATE PROCEDURE dbo.uspGeneratedSelectReminder

(
  @ReminderID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ReminderID],
    [OrganizationID],
    [RefType],
    [RefID],
    [Description],
    [DueDate],
    [UserID],
    [IsDismissed],
    [HasEmailSent],
    [CreatorID],
    [DateCreated],
    [TaskName],
    [TaskDueDate],
    [TaskIsComplete],
    [TaskDateCompleted],
    [TaskParentID]
  FROM [dbo].[Reminders]
  WHERE ([ReminderID] = @ReminderID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertReminder' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertReminder
GO

CREATE PROCEDURE dbo.uspGeneratedInsertReminder

(
  @OrganizationID int,
  @RefType int,
  @RefID int,
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @CreatorID int,
  @DateCreated datetime,
  @TaskName nvarchar(1000),
  @TaskDueDate datetime,
  @TaskIsComplete bit,
  @TaskDateCompleted datetime,
  @TaskParentID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Reminders]
  (
    [OrganizationID],
    [RefType],
    [RefID],
    [Description],
    [DueDate],
    [UserID],
    [IsDismissed],
    [HasEmailSent],
    [CreatorID],
    [DateCreated],
    [TaskName],
    [TaskDueDate],
    [TaskIsComplete],
    [TaskDateCompleted],
    [TaskParentID])
  VALUES (
    @OrganizationID,
    @RefType,
    @RefID,
    @Description,
    @DueDate,
    @UserID,
    @IsDismissed,
    @HasEmailSent,
    @CreatorID,
    @DateCreated,
    @TaskName,
    @TaskDueDate,
    @TaskIsComplete,
    @TaskDateCompleted,
    @TaskParentID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateReminder' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateReminder
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateReminder

(
  @ReminderID int,
  @OrganizationID int,
  @RefType int,
  @RefID int,
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @TaskName nvarchar(1000),
  @TaskDueDate datetime,
  @TaskIsComplete bit,
  @TaskDateCompleted datetime,
  @TaskParentID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Reminders]
  SET
    [OrganizationID] = @OrganizationID,
    [RefType] = @RefType,
    [RefID] = @RefID,
    [Description] = @Description,
    [DueDate] = @DueDate,
    [UserID] = @UserID,
    [IsDismissed] = @IsDismissed,
    [HasEmailSent] = @HasEmailSent,
    [TaskName] = @TaskName,
    [TaskDueDate] = @TaskDueDate,
    [TaskIsComplete] = @TaskIsComplete,
    [TaskDateCompleted] = @TaskDateCompleted,
    [TaskParentID] = @TaskParentID
  WHERE ([ReminderID] = @ReminderID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteReminder' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteReminder
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteReminder

(
  @ReminderID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Reminders]
  WHERE ([ReminderID] = @ReminderID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTasksViewItem

(

)
AS
  SET NOCOUNT OFF;
  SELECT
    [ReminderID],
    [OrganizationID],
    [RefType],
    [RefID],
    [Description],
    [DueDate],
    [UserID],
    [IsDismissed],
    [HasEmailSent],
    [CreatorID],
    [DateCreated],
    [TaskName],
    [TaskDueDate],
    [TaskIsComplete],
    [TaskDateCompleted],
    [TaskParentID],
    [TaskParentName],
    [UserName],
    [Creator]
  FROM [dbo].[TasksView]
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTasksViewItem

(
  @ReminderID int,
  @OrganizationID int,
  @RefType int,
  @RefID int,
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @CreatorID int,
  @DateCreated datetime,
  @TaskName nvarchar(1000),
  @TaskDueDate datetime,
  @TaskIsComplete bit,
  @TaskDateCompleted datetime,
  @TaskParentID int,
  @TaskParentName nvarchar(1000),
  @UserName nvarchar(201),
  @Creator nvarchar(201),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TasksView]
  (
    [ReminderID],
    [OrganizationID],
    [RefType],
    [RefID],
    [Description],
    [DueDate],
    [UserID],
    [IsDismissed],
    [HasEmailSent],
    [CreatorID],
    [DateCreated],
    [TaskName],
    [TaskDueDate],
    [TaskIsComplete],
    [TaskDateCompleted],
    [TaskParentID],
    [TaskParentName],
    [UserName],
    [Creator])
  VALUES (
    @ReminderID,
    @OrganizationID,
    @RefType,
    @RefID,
    @Description,
    @DueDate,
    @UserID,
    @IsDismissed,
    @HasEmailSent,
    @CreatorID,
    @DateCreated,
    @TaskName,
    @TaskDueDate,
    @TaskIsComplete,
    @TaskDateCompleted,
    @TaskParentID,
    @TaskParentName,
    @UserName,
    @Creator)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTasksViewItem

(
  @ReminderID int,
  @OrganizationID int,
  @RefType int,
  @RefID int,
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @TaskName nvarchar(1000),
  @TaskDueDate datetime,
  @TaskIsComplete bit,
  @TaskDateCompleted datetime,
  @TaskParentID int,
  @TaskParentName nvarchar(1000),
  @UserName nvarchar(201),
  @Creator nvarchar(201)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TasksView]
  SET
    [ReminderID] = @ReminderID,
    [OrganizationID] = @OrganizationID,
    [RefType] = @RefType,
    [RefID] = @RefID,
    [Description] = @Description,
    [DueDate] = @DueDate,
    [UserID] = @UserID,
    [IsDismissed] = @IsDismissed,
    [HasEmailSent] = @HasEmailSent,
    [TaskName] = @TaskName,
    [TaskDueDate] = @TaskDueDate,
    [TaskIsComplete] = @TaskIsComplete,
    [TaskDateCompleted] = @TaskDateCompleted,
    [TaskParentID] = @TaskParentID,
    [TaskParentName] = @TaskParentName,
    [UserName] = @UserName,
    [Creator] = @Creator
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


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectReminder' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectReminder
GO

CREATE PROCEDURE dbo.uspGeneratedSelectReminder

(
  @ReminderID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ReminderID],
    [OrganizationID],
    [RefType],
    [RefID],
    [Description],
    [DueDate],
    [UserID],
    [IsDismissed],
    [HasEmailSent],
    [CreatorID],
    [DateCreated],
    [TaskName],
    [TaskDueDate],
    [TaskIsComplete],
    [TaskDateCompleted],
    [TaskParentID]
  FROM [dbo].[Reminders]
  WHERE ([ReminderID] = @ReminderID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertReminder' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertReminder
GO

CREATE PROCEDURE dbo.uspGeneratedInsertReminder

(
  @OrganizationID int,
  @RefType int,
  @RefID int,
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @CreatorID int,
  @DateCreated datetime,
  @TaskName nvarchar(1000),
  @TaskDueDate datetime,
  @TaskIsComplete bit,
  @TaskDateCompleted datetime,
  @TaskParentID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Reminders]
  (
    [OrganizationID],
    [RefType],
    [RefID],
    [Description],
    [DueDate],
    [UserID],
    [IsDismissed],
    [HasEmailSent],
    [CreatorID],
    [DateCreated],
    [TaskName],
    [TaskDueDate],
    [TaskIsComplete],
    [TaskDateCompleted],
    [TaskParentID])
  VALUES (
    @OrganizationID,
    @RefType,
    @RefID,
    @Description,
    @DueDate,
    @UserID,
    @IsDismissed,
    @HasEmailSent,
    @CreatorID,
    @DateCreated,
    @TaskName,
    @TaskDueDate,
    @TaskIsComplete,
    @TaskDateCompleted,
    @TaskParentID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateReminder' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateReminder
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateReminder

(
  @ReminderID int,
  @OrganizationID int,
  @RefType int,
  @RefID int,
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @TaskName nvarchar(1000),
  @TaskDueDate datetime,
  @TaskIsComplete bit,
  @TaskDateCompleted datetime,
  @TaskParentID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Reminders]
  SET
    [OrganizationID] = @OrganizationID,
    [RefType] = @RefType,
    [RefID] = @RefID,
    [Description] = @Description,
    [DueDate] = @DueDate,
    [UserID] = @UserID,
    [IsDismissed] = @IsDismissed,
    [HasEmailSent] = @HasEmailSent,
    [TaskName] = @TaskName,
    [TaskDueDate] = @TaskDueDate,
    [TaskIsComplete] = @TaskIsComplete,
    [TaskDateCompleted] = @TaskDateCompleted,
    [TaskParentID] = @TaskParentID
  WHERE ([ReminderID] = @ReminderID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteReminder' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteReminder
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteReminder

(
  @ReminderID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Reminders]
  WHERE ([ReminderID] = @ReminderID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTasksViewItem

(

)
AS
  SET NOCOUNT OFF;
  SELECT
    [ReminderID],
    [OrganizationID],
    [RefType],
    [RefID],
    [Description],
    [DueDate],
    [UserID],
    [IsDismissed],
    [HasEmailSent],
    [CreatorID],
    [DateCreated],
    [TaskName],
    [TaskDueDate],
    [TaskIsComplete],
    [TaskDateCompleted],
    [TaskParentID],
    [TaskParentName],
    [UserName],
    [Creator]
  FROM [dbo].[TasksView]
  WH)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTasksViewItem

(
  @ReminderID int,
  @OrganizationID int,
  @RefType int,
  @RefID int,
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @CreatorID int,
  @DateCreated datetime,
  @TaskName nvarchar(1000),
  @TaskDueDate datetime,
  @TaskIsComplete bit,
  @TaskDateCompleted datetime,
  @TaskParentID int,
  @TaskParentName nvarchar(1000),
  @UserName nvarchar(201),
  @Creator nvarchar(201),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TasksView]
  (
    [ReminderID],
    [OrganizationID],
    [RefType],
    [RefID],
    [Description],
    [DueDate],
    [UserID],
    [IsDismissed],
    [HasEmailSent],
    [CreatorID],
    [DateCreated],
    [TaskName],
    [TaskDueDate],
    [TaskIsComplete],
    [TaskDateCompleted],
    [TaskParentID],
    [TaskParentName],
    [UserName],
    [Creator])
  VALUES (
    @ReminderID,
    @OrganizationID,
    @RefType,
    @RefID,
    @Description,
    @DueDate,
    @UserID,
    @IsDismissed,
    @HasEmailSent,
    @CreatorID,
    @DateCreated,
    @TaskName,
    @TaskDueDate,
    @TaskIsComplete,
    @TaskDateCompleted,
    @TaskParentID,
    @TaskParentName,
    @UserName,
    @Creator)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTasksViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTasksViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTasksViewItem

(
  @ReminderID int,
  @OrganizationID int,
  @RefType int,
  @RefID int,
  @Description nvarchar(4000),
  @DueDate datetime,
  @UserID int,
  @IsDismissed bit,
  @HasEmailSent bit,
  @TaskName nvarchar(1000),
  @TaskDueDate datetime,
  @TaskIsComplete bit,
  @TaskDateCompleted datetime,
  @TaskParentID int,
  @TaskParentName nvarchar(1000),
  @UserName nvarchar(201),
  @Creator nvarchar(201)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TasksView]
  SET
    [ReminderID] = @ReminderID,
    [OrganizationID] = @OrganizationID,
    [RefType] = @RefType,
    [RefID] = @RefID,
    [Description] = @Description,
    [DueDate] = @DueDate,
    [UserID] = @UserID,
    [IsDismissed] = @IsDismissed,
    [HasEmailSent] = @HasEmailSent,
    [TaskName] = @TaskName,
    [TaskDueDate] = @TaskDueDate,
    [TaskIsComplete] = @TaskIsComplete,
    [TaskDateCompleted] = @TaskDateCompleted,
    [TaskParentID] = @TaskParentID,
    [TaskParentName] = @TaskParentName,
    [UserName] = @UserName,
    [Creator] = @Creator
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


