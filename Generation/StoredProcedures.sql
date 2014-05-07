IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTicketLinkToJiraItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTicketLinkToJiraItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTicketLinkToJiraItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [id],
    [TicketID],
    [DateModifiedByJiraSync],
    [SyncWithJira],
    [JiraID],
    [JiraKey],
    [JiraLinkURL],
    [JiraStatus],
    [CreatorID]
  FROM [dbo].[TicketLinkToJira]
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTicketLinkToJiraItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTicketLinkToJiraItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTicketLinkToJiraItem

(
  @TicketID int,
  @DateModifiedByJiraSync datetime,
  @SyncWithJira bit,
  @JiraID int,
  @JiraKey varchar(8000),
  @JiraLinkURL varchar(8000),
  @JiraStatus varchar(8000),
  @CreatorID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TicketLinkToJira]
  (
    [TicketID],
    [DateModifiedByJiraSync],
    [SyncWithJira],
    [JiraID],
    [JiraKey],
    [JiraLinkURL],
    [JiraStatus],
    [CreatorID])
  VALUES (
    @TicketID,
    @DateModifiedByJiraSync,
    @SyncWithJira,
    @JiraID,
    @JiraKey,
    @JiraLinkURL,
    @JiraStatus,
    @CreatorID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTicketLinkToJiraItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTicketLinkToJiraItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTicketLinkToJiraItem

(
  @id int,
  @TicketID int,
  @DateModifiedByJiraSync datetime,
  @SyncWithJira bit,
  @JiraID int,
  @JiraKey varchar(8000),
  @JiraLinkURL varchar(8000),
  @JiraStatus varchar(8000)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TicketLinkToJira]
  SET
    [TicketID] = @TicketID,
    [DateModifiedByJiraSync] = @DateModifiedByJiraSync,
    [SyncWithJira] = @SyncWithJira,
    [JiraID] = @JiraID,
    [JiraKey] = @JiraKey,
    [JiraLinkURL] = @JiraLinkURL,
    [JiraStatus] = @JiraStatus
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTicketLinkToJiraItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTicketLinkToJiraItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTicketLinkToJiraItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TicketLinkToJira]
  WHERE ([id] = @id)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTicketLinkToJiraItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTicketLinkToJiraItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTicketLinkToJiraItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [id],
    [TicketID],
    [DateModifiedByJiraSync],
    [SyncWithJira],
    [JiraID],
    [JiraKey],
    [JiraLinkURL],
    [JiraStatus],
    [CreatorID]
  FROM [dbo].[TicketLinkToJira]
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTicketLinkToJiraItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTicketLinkToJiraItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTicketLinkToJiraItem

(
  @TicketID int,
  @DateModifiedByJiraSync datetime,
  @SyncWithJira bit,
  @JiraID int,
  @JiraKey varchar(8000),
  @JiraLinkURL varchar(8000),
  @JiraStatus varchar(8000),
  @CreatorID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TicketLinkToJira]
  (
    [TicketID],
    [DateModifiedByJiraSync],
    [SyncWithJira],
    [JiraID],
    [JiraKey],
    [JiraLinkURL],
    [JiraStatus],
    [CreatorID])
  VALUES (
    @TicketID,
    @DateModifiedByJiraSync,
    @SyncWithJira,
    @JiraID,
    @JiraKey,
    @JiraLinkURL,
    @JiraStatus,
    @CreatorID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTicketLinkToJiraItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTicketLinkToJiraItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTicketLinkToJiraItem

(
  @id int,
  @TicketID int,
  @DateModifiedByJiraSync datetime,
  @SyncWithJira bit,
  @JiraID int,
  @JiraKey varchar(8000),
  @JiraLinkURL varchar(8000),
  @JiraStatus varchar(8000)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TicketLinkToJira]
  SET
    [TicketID] = @TicketID,
    [DateModifiedByJiraSync] = @DateModifiedByJiraSync,
    [SyncWithJira] = @SyncWithJira,
    [JiraID] = @JiraID,
    [JiraKey] = @JiraKey,
    [JiraLinkURL] = @JiraLinkURL,
    [JiraStatus] = @JiraStatus
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTicketLinkToJiraItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTicketLinkToJiraItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTicketLinkToJiraItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TicketLinkToJira]
  WHERE ([id] = @id)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTicketLinkToJiraItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTicketLinkToJiraItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTicketLinkToJiraItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [id],
    [TicketID],
    [DateModifiedByJiraSync],
    [SyncWithJira],
    [JiraID],
    [JiraKey],
    [JiraLinkURL],
    [JiraStatus],
    [CreatorID]
  FROM [dbo].[TicketLinkToJira]
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTicketLinkToJiraItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTicketLinkToJiraItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTicketLinkToJiraItem

(
  @TicketID int,
  @DateModifiedByJiraSync datetime,
  @SyncWithJira bit,
  @JiraID int,
  @JiraKey varchar(8000),
  @JiraLinkURL varchar(8000),
  @JiraStatus varchar(8000),
  @CreatorID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TicketLinkToJira]
  (
    [TicketID],
    [DateModifiedByJiraSync],
    [SyncWithJira],
    [JiraID],
    [JiraKey],
    [JiraLinkURL],
    [JiraStatus],
    [CreatorID])
  VALUES (
    @TicketID,
    @DateModifiedByJiraSync,
    @SyncWithJira,
    @JiraID,
    @JiraKey,
    @JiraLinkURL,
    @JiraStatus,
    @CreatorID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTicketLinkToJiraItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTicketLinkToJiraItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTicketLinkToJiraItem

(
  @id int,
  @TicketID int,
  @DateModifiedByJiraSync datetime,
  @SyncWithJira bit,
  @JiraID int,
  @JiraKey varchar(8000),
  @JiraLinkURL varchar(8000),
  @JiraStatus varchar(8000)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TicketLinkToJira]
  SET
    [TicketID] = @TicketID,
    [DateModifiedByJiraSync] = @DateModifiedByJiraSync,
    [SyncWithJira] = @SyncWithJira,
    [JiraID] = @JiraID,
    [JiraKey] = @JiraKey,
    [JiraLinkURL] = @JiraLinkURL,
    [JiraStatus] = @JiraStatus
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTicketLinkToJiraItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTicketLinkToJiraItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTicketLinkToJiraItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TicketLinkToJira]
  WHERE ([id] = @id)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTicketLinkToJiraItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTicketLinkToJiraItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTicketLinkToJiraItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [id],
    [TicketID],
    [DateModifiedByJiraSync],
    [SyncWithJira],
    [JiraID],
    [JiraKey],
    [JiraLinkURL],
    [JiraStatus],
    [CreatorID]
  FROM [dbo].[TicketLinkToJira]
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTicketLinkToJiraItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTicketLinkToJiraItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTicketLinkToJiraItem

(
  @TicketID int,
  @DateModifiedByJiraSync datetime,
  @SyncWithJira bit,
  @JiraID int,
  @JiraKey varchar(8000),
  @JiraLinkURL varchar(8000),
  @JiraStatus varchar(8000),
  @CreatorID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TicketLinkToJira]
  (
    [TicketID],
    [DateModifiedByJiraSync],
    [SyncWithJira],
    [JiraID],
    [JiraKey],
    [JiraLinkURL],
    [JiraStatus],
    [CreatorID])
  VALUES (
    @TicketID,
    @DateModifiedByJiraSync,
    @SyncWithJira,
    @JiraID,
    @JiraKey,
    @JiraLinkURL,
    @JiraStatus,
    @CreatorID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTicketLinkToJiraItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTicketLinkToJiraItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTicketLinkToJiraItem

(
  @id int,
  @TicketID int,
  @DateModifiedByJiraSync datetime,
  @SyncWithJira bit,
  @JiraID int,
  @JiraKey varchar(8000),
  @JiraLinkURL varchar(8000),
  @JiraStatus varchar(8000)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TicketLinkToJira]
  SET
    [TicketID] = @TicketID,
    [DateModifiedByJiraSync] = @DateModifiedByJiraSync,
    [SyncWithJira] = @SyncWithJira,
    [JiraID] = @JiraID,
    [JiraKey] = @JiraKey,
    [JiraLinkURL] = @JiraLinkURL,
    [JiraStatus] = @JiraStatus
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTicketLinkToJiraItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTicketLinkToJiraItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTicketLinkToJiraItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TicketLinkToJira]
  WHERE ([id] = @id)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTicketLinkToJiraItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTicketLinkToJiraItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTicketLinkToJiraItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [id],
    [TicketID],
    [DateModifiedByJiraSync],
    [SyncWithJira],
    [JiraID],
    [JiraKey],
    [JiraLinkURL],
    [JiraStatus],
    [CreatorID]
  FROM [dbo].[TicketLinkToJira]
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTicketLinkToJiraItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTicketLinkToJiraItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTicketLinkToJiraItem

(
  @TicketID int,
  @DateModifiedByJiraSync datetime,
  @SyncWithJira bit,
  @JiraID int,
  @JiraKey varchar(8000),
  @JiraLinkURL varchar(8000),
  @JiraStatus varchar(8000),
  @CreatorID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TicketLinkToJira]
  (
    [TicketID],
    [DateModifiedByJiraSync],
    [SyncWithJira],
    [JiraID],
    [JiraKey],
    [JiraLinkURL],
    [JiraStatus],
    [CreatorID])
  VALUES (
    @TicketID,
    @DateModifiedByJiraSync,
    @SyncWithJira,
    @JiraID,
    @JiraKey,
    @JiraLinkURL,
    @JiraStatus,
    @CreatorID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTicketLinkToJiraItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTicketLinkToJiraItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTicketLinkToJiraItem

(
  @id int,
  @TicketID int,
  @DateModifiedByJiraSync datetime,
  @SyncWithJira bit,
  @JiraID int,
  @JiraKey varchar(8000),
  @JiraLinkURL varchar(8000),
  @JiraStatus varchar(8000)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TicketLinkToJira]
  SET
    [TicketID] = @TicketID,
    [DateModifiedByJiraSync] = @DateModifiedByJiraSync,
    [SyncWithJira] = @SyncWithJira,
    [JiraID] = @JiraID,
    [JiraKey] = @JiraKey,
    [JiraLinkURL] = @JiraLinkURL,
    [JiraStatus] = @JiraStatus
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTicketLinkToJiraItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTicketLinkToJiraItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTicketLinkToJiraItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TicketLinkToJira]
  WHERE ([id] = @id)
GO


