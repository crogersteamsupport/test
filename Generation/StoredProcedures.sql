IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectRemindersViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectRemindersViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectRemindersViewItem

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
    [UserName],
    [Creator],
    [ReminderType],
    [ReminderTarget]
  FROM [dbo].[RemindersView]
  WHERE ([ReminderID] = @ReminderID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertRemindersViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertRemindersViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertRemindersViewItem

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
  @UserName nvarchar(201),
  @Creator nvarchar(201),
  @ReminderType varchar(7),
  @ReminderTarget nvarchar(459),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[RemindersView]
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
    [UserName],
    [Creator],
    [ReminderType],
    [ReminderTarget])
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
    @UserName,
    @Creator,
    @ReminderType,
    @ReminderTarget)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateRemindersViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateRemindersViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateRemindersViewItem

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
  @UserName nvarchar(201),
  @Creator nvarchar(201),
  @ReminderType varchar(7),
  @ReminderTarget nvarchar(459)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[RemindersView]
  SET
    [OrganizationID] = @OrganizationID,
    [RefType] = @RefType,
    [RefID] = @RefID,
    [Description] = @Description,
    [DueDate] = @DueDate,
    [UserID] = @UserID,
    [IsDismissed] = @IsDismissed,
    [HasEmailSent] = @HasEmailSent,
    [UserName] = @UserName,
    [Creator] = @Creator,
    [ReminderType] = @ReminderType,
    [ReminderTarget] = @ReminderTarget
  WHERE ([ReminderID] = @ReminderID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteRemindersViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteRemindersViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteRemindersViewItem

(
  @ReminderID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[RemindersView]
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


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectRemindersViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectRemindersViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectRemindersViewItem

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
    [UserName],
    [Creator],
    [ReminderType],
    [ReminderTarget]
  FROM [dbo].[RemindersView]
  WHERE ([ReminderID] = @ReminderID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertRemindersViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertRemindersViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertRemindersViewItem

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
  @UserName nvarchar(201),
  @Creator nvarchar(201),
  @ReminderType varchar(7),
  @ReminderTarget nvarchar(459),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[RemindersView]
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
    [UserName],
    [Creator],
    [ReminderType],
    [ReminderTarget])
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
    @UserName,
    @Creator,
    @ReminderType,
    @ReminderTarget)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateRemindersViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateRemindersViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateRemindersViewItem

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
  @UserName nvarchar(201),
  @Creator nvarchar(201),
  @ReminderType varchar(7),
  @ReminderTarget nvarchar(459)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[RemindersView]
  SET
    [OrganizationID] = @OrganizationID,
    [RefType] = @RefType,
    [RefID] = @RefID,
    [Description] = @Description,
    [DueDate] = @DueDate,
    [UserID] = @UserID,
    [IsDismissed] = @IsDismissed,
    [HasEmailSent] = @HasEmailSent,
    [UserName] = @UserName,
    [Creator] = @Creator,
    [ReminderType] = @ReminderType,
    [ReminderTarget] = @ReminderTarget
  WHERE ([ReminderID] = @ReminderID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteRemindersViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteRemindersViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteRemindersViewItem

(
  @ReminderID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[RemindersView]
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


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectRemindersViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectRemindersViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectRemindersViewItem

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
    [UserName],
    [Creator],
    [ReminderType],
    [ReminderTarget]
  FROM [dbo].[RemindersView]
  WHERE ([ReminderID] = @ReminderID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertRemindersViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertRemindersViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertRemindersViewItem

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
  @UserName nvarchar(201),
  @Creator nvarchar(201),
  @ReminderType varchar(7),
  @ReminderTarget nvarchar(459),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[RemindersView]
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
    [UserName],
    [Creator],
    [ReminderType],
    [ReminderTarget])
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
    @UserName,
    @Creator,
    @ReminderType,
    @ReminderTarget)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateRemindersViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateRemindersViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateRemindersViewItem

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
  @UserName nvarchar(201),
  @Creator nvarchar(201),
  @ReminderType varchar(7),
  @ReminderTarget nvarchar(459)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[RemindersView]
  SET
    [OrganizationID] = @OrganizationID,
    [RefType] = @RefType,
    [RefID] = @RefID,
    [Description] = @Description,
    [DueDate] = @DueDate,
    [UserID] = @UserID,
    [IsDismissed] = @IsDismissed,
    [HasEmailSent] = @HasEmailSent,
    [UserName] = @UserName,
    [Creator] = @Creator,
    [ReminderType] = @ReminderType,
    [ReminderTarget] = @ReminderTarget
  WHERE ([ReminderID] = @ReminderID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteRemindersViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteRemindersViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteRemindersViewItem

(
  @ReminderID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[RemindersView]
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


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectRemindersViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectRemindersViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectRemindersViewItem

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
    [UserName],
    [Creator],
    [ReminderType],
    [ReminderTarget]
  FROM [dbo].[RemindersView]
  WHERE ([ReminderID] = @ReminderID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertRemindersViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertRemindersViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertRemindersViewItem

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
  @UserName nvarchar(201),
  @Creator nvarchar(201),
  @ReminderType varchar(7),
  @ReminderTarget nvarchar(459),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[RemindersView]
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
    [UserName],
    [Creator],
    [ReminderType],
    [ReminderTarget])
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
    @UserName,
    @Creator,
    @ReminderType,
    @ReminderTarget)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateRemindersViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateRemindersViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateRemindersViewItem

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
  @UserName nvarchar(201),
  @Creator nvarchar(201),
  @ReminderType varchar(7),
  @ReminderTarget nvarchar(459)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[RemindersView]
  SET
    [OrganizationID] = @OrganizationID,
    [RefType] = @RefType,
    [RefID] = @RefID,
    [Description] = @Description,
    [DueDate] = @DueDate,
    [UserID] = @UserID,
    [IsDismissed] = @IsDismissed,
    [HasEmailSent] = @HasEmailSent,
    [UserName] = @UserName,
    [Creator] = @Creator,
    [ReminderType] = @ReminderType,
    [ReminderTarget] = @ReminderTarget
  WHERE ([ReminderID] = @ReminderID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteRemindersViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteRemindersViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteRemindersViewItem

(
  @ReminderID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[RemindersView]
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


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectRemindersViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectRemindersViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectRemindersViewItem

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
    [UserName],
    [Creator],
    [ReminderType],
    [ReminderTarget]
  FROM [dbo].[RemindersView]
  WHERE ([ReminderID] = @ReminderID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertRemindersViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertRemindersViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertRemindersViewItem

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
  @UserName nvarchar(201),
  @Creator nvarchar(201),
  @ReminderType varchar(7),
  @ReminderTarget nvarchar(459),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[RemindersView]
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
    [UserName],
    [Creator],
    [ReminderType],
    [ReminderTarget])
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
    @UserName,
    @Creator,
    @ReminderType,
    @ReminderTarget)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateRemindersViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateRemindersViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateRemindersViewItem

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
  @UserName nvarchar(201),
  @Creator nvarchar(201),
  @ReminderType varchar(7),
  @ReminderTarget nvarchar(459)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[RemindersView]
  SET
    [OrganizationID] = @OrganizationID,
    [RefType] = @RefType,
    [RefID] = @RefID,
    [Description] = @Description,
    [DueDate] = @DueDate,
    [UserID] = @UserID,
    [IsDismissed] = @IsDismissed,
    [HasEmailSent] = @HasEmailSent,
    [UserName] = @UserName,
    [Creator] = @Creator,
    [ReminderType] = @ReminderType,
    [ReminderTarget] = @ReminderTarget
  WHERE ([ReminderID] = @ReminderID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteRemindersViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteRemindersViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteRemindersViewItem

(
  @ReminderID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[RemindersView]
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


