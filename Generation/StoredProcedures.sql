IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTaskAssociationsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTaskAssociationsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTaskAssociationsViewItem

(
  @ReminderID int,
  @RefID int,
  @RefType int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ReminderID],
    [RefID],
    [RefType],
    [TicketNumber],
    [TicketName],
    [User],
    [Company],
    [Group],
    [Product]
  FROM [dbo].[TaskAssociationsView]
  WHERE ([ReminderID] = @ReminderID AND [RefID] = @RefID AND [RefType] = @RefType)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTaskAssociationsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTaskAssociationsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTaskAssociationsViewItem

(
  @ReminderID int,
  @RefID int,
  @RefType int,
  @TicketNumber int,
  @TicketName nvarchar(255),
  @User nvarchar(201),
  @Company nvarchar(255),
  @Group varchar(255),
  @Product varchar(255),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TaskAssociationsView]
  (
    [ReminderID],
    [RefID],
    [RefType],
    [TicketNumber],
    [TicketName],
    [User],
    [Company],
    [Group],
    [Product])
  VALUES (
    @ReminderID,
    @RefID,
    @RefType,
    @TicketNumber,
    @TicketName,
    @User,
    @Company,
    @Group,
    @Product)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTaskAssociationsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTaskAssociationsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTaskAssociationsViewItem

(
  @ReminderID int,
  @RefID int,
  @RefType int,
  @TicketNumber int,
  @TicketName nvarchar(255),
  @User nvarchar(201),
  @Company nvarchar(255),
  @Group varchar(255),
  @Product varchar(255)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TaskAssociationsView]
  SET
    [TicketNumber] = @TicketNumber,
    [TicketName] = @TicketName,
    [User] = @User,
    [Company] = @Company,
    [Group] = @Group,
    [Product] = @Product
  WHERE ([ReminderID] = @ReminderID AND [RefID] = @RefID AND [RefType] = @RefType)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTaskAssociationsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTaskAssociationsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTaskAssociationsViewItem

(
  @ReminderID int,
  @RefID int,
  @RefType int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TaskAssociationsView]
  WHERE ([ReminderID] = @ReminderID AND [RefID] = @RefID AND [RefType] = @RefType)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTaskAssociationsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTaskAssociationsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTaskAssociationsViewItem

(
  @ReminderID int,
  @RefID int,
  @RefType int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ReminderID],
    [RefID],
    [RefType],
    [TicketNumber],
    [TicketName],
    [User],
    [Company],
    [Group],
    [Product]
  FROM [dbo].[TaskAssociationsView]
  WHERE ([ReminderID] = @ReminderID AND [RefID] = @RefID AND [RefType] = @RefType)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTaskAssociationsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTaskAssociationsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTaskAssociationsViewItem

(
  @ReminderID int,
  @RefID int,
  @RefType int,
  @TicketNumber int,
  @TicketName nvarchar(255),
  @User nvarchar(201),
  @Company nvarchar(255),
  @Group varchar(255),
  @Product varchar(255),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TaskAssociationsView]
  (
    [ReminderID],
    [RefID],
    [RefType],
    [TicketNumber],
    [TicketName],
    [User],
    [Company],
    [Group],
    [Product])
  VALUES (
    @ReminderID,
    @RefID,
    @RefType,
    @TicketNumber,
    @TicketName,
    @User,
    @Company,
    @Group,
    @Product)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTaskAssociationsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTaskAssociationsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTaskAssociationsViewItem

(
  @ReminderID int,
  @RefID int,
  @RefType int,
  @TicketNumber int,
  @TicketName nvarchar(255),
  @User nvarchar(201),
  @Company nvarchar(255),
  @Group varchar(255),
  @Product varchar(255)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TaskAssociationsView]
  SET
    [TicketNumber] = @TicketNumber,
    [TicketName] = @TicketName,
    [User] = @User,
    [Company] = @Company,
    [Group] = @Group,
    [Product] = @Product
  WHERE ([ReminderID] = @ReminderID AND [RefID] = @RefID AND [RefType] = @RefType)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTaskAssociationsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTaskAssociationsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTaskAssociationsViewItem

(
  @ReminderID int,
  @RefID int,
  @RefType int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TaskAssociationsView]
  WHERE ([ReminderID] = @ReminderID AND [RefID] = @RefID AND [RefType] = @RefType)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTaskAssociationsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTaskAssociationsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTaskAssociationsViewItem

(
  @ReminderID int,
  @RefID int,
  @RefType int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ReminderID],
    [RefID],
    [RefType],
    [TicketNumber],
    [TicketName],
    [User],
    [Company],
    [Group],
    [Product]
  FROM [dbo].[TaskAssociationsView]
  WHERE ([ReminderID] = @ReminderID AND [RefID] = @RefID AND [RefType] = @RefType)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTaskAssociationsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTaskAssociationsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTaskAssociationsViewItem

(
  @ReminderID int,
  @RefID int,
  @RefType int,
  @TicketNumber int,
  @TicketName nvarchar(255),
  @User nvarchar(201),
  @Company nvarchar(255),
  @Group varchar(255),
  @Product varchar(255),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TaskAssociationsView]
  (
    [ReminderID],
    [RefID],
    [RefType],
    [TicketNumber],
    [TicketName],
    [User],
    [Company],
    [Group],
    [Product])
  VALUES (
    @ReminderID,
    @RefID,
    @RefType,
    @TicketNumber,
    @TicketName,
    @User,
    @Company,
    @Group,
    @Product)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTaskAssociationsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTaskAssociationsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTaskAssociationsViewItem

(
  @ReminderID int,
  @RefID int,
  @RefType int,
  @TicketNumber int,
  @TicketName nvarchar(255),
  @User nvarchar(201),
  @Company nvarchar(255),
  @Group varchar(255),
  @Product varchar(255)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TaskAssociationsView]
  SET
    [TicketNumber] = @TicketNumber,
    [TicketName] = @TicketName,
    [User] = @User,
    [Company] = @Company,
    [Group] = @Group,
    [Product] = @Product
  WHERE ([ReminderID] = @ReminderID AND [RefID] = @RefID AND [RefType] = @RefType)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTaskAssociationsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTaskAssociationsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTaskAssociationsViewItem

(
  @ReminderID int,
  @RefID int,
  @RefType int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TaskAssociationsView]
  WHERE ([ReminderID] = @ReminderID AND [RefID] = @RefID AND [RefType] = @RefType)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTaskAssociationsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTaskAssociationsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTaskAssociationsViewItem

(
  @ReminderID int,
  @RefID int,
  @RefType int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ReminderID],
    [RefID],
    [RefType],
    [TicketNumber],
    [TicketName],
    [User],
    [Company],
    [Group],
    [Product]
  FROM [dbo].[TaskAssociationsView]
  WHERE ([ReminderID] = @ReminderID AND [RefID] = @RefID AND [RefType] = @RefType)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTaskAssociationsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTaskAssociationsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTaskAssociationsViewItem

(
  @ReminderID int,
  @RefID int,
  @RefType int,
  @TicketNumber int,
  @TicketName nvarchar(255),
  @User nvarchar(201),
  @Company nvarchar(255),
  @Group varchar(255),
  @Product varchar(255),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TaskAssociationsView]
  (
    [ReminderID],
    [RefID],
    [RefType],
    [TicketNumber],
    [TicketName],
    [User],
    [Company],
    [Group],
    [Product])
  VALUES (
    @ReminderID,
    @RefID,
    @RefType,
    @TicketNumber,
    @TicketName,
    @User,
    @Company,
    @Group,
    @Product)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTaskAssociationsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTaskAssociationsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTaskAssociationsViewItem

(
  @ReminderID int,
  @RefID int,
  @RefType int,
  @TicketNumber int,
  @TicketName nvarchar(255),
  @User nvarchar(201),
  @Company nvarchar(255),
  @Group varchar(255),
  @Product varchar(255)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TaskAssociationsView]
  SET
    [TicketNumber] = @TicketNumber,
    [TicketName] = @TicketName,
    [User] = @User,
    [Company] = @Company,
    [Group] = @Group,
    [Product] = @Product
  WHERE ([ReminderID] = @ReminderID AND [RefID] = @RefID AND [RefType] = @RefType)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTaskAssociationsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTaskAssociationsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTaskAssociationsViewItem

(
  @ReminderID int,
  @RefID int,
  @RefType int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TaskAssociationsView]
  WHERE ([ReminderID] = @ReminderID AND [RefID] = @RefID AND [RefType] = @RefType)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTaskAssociationsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTaskAssociationsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTaskAssociationsViewItem

(
  @ReminderID int,
  @RefID int,
  @RefType int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ReminderID],
    [RefID],
    [RefType],
    [TicketNumber],
    [TicketName],
    [User],
    [Company],
    [Group],
    [Product]
  FROM [dbo].[TaskAssociationsView]
  WHERE ([ReminderID] = @ReminderID AND [RefID] = @RefID AND [RefType] = @RefType)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTaskAssociationsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTaskAssociationsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTaskAssociationsViewItem

(
  @ReminderID int,
  @RefID int,
  @RefType int,
  @TicketNumber int,
  @TicketName nvarchar(255),
  @User nvarchar(201),
  @Company nvarchar(255),
  @Group varchar(255),
  @Product varchar(255),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TaskAssociationsView]
  (
    [ReminderID],
    [RefID],
    [RefType],
    [TicketNumber],
    [TicketName],
    [User],
    [Company],
    [Group],
    [Product])
  VALUES (
    @ReminderID,
    @RefID,
    @RefType,
    @TicketNumber,
    @TicketName,
    @User,
    @Company,
    @Group,
    @Product)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTaskAssociationsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTaskAssociationsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTaskAssociationsViewItem

(
  @ReminderID int,
  @RefID int,
  @RefType int,
  @TicketNumber int,
  @TicketName nvarchar(255),
  @User nvarchar(201),
  @Company nvarchar(255),
  @Group varchar(255),
  @Product varchar(255)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TaskAssociationsView]
  SET
    [TicketNumber] = @TicketNumber,
    [TicketName] = @TicketName,
    [User] = @User,
    [Company] = @Company,
    [Group] = @Group,
    [Product] = @Product
  WHERE ([ReminderID] = @ReminderID AND [RefID] = @RefID AND [RefType] = @RefType)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTaskAssociationsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTaskAssociationsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTaskAssociationsViewItem

(
  @ReminderID int,
  @RefID int,
  @RefType int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TaskAssociationsView]
  WHERE ([ReminderID] = @ReminderID AND [RefID] = @RefID AND [RefType] = @RefType)
GO


