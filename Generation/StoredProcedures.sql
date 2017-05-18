IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTicketLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [id],
    [TicketID],
    [DateModifiedByTFSSync],
    [SyncWithTFS],
    [TFSID],
    [TFSTitle],
    [TFSURL],
    [TFSState],
    [CreatorID],
    [CrmLinkID]
  FROM [dbo].[TicketLinkToTFS]
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTicketLinkToTFSItem

(
  @TicketID int,
  @DateModifiedByTFSSync datetime,
  @SyncWithTFS bit,
  @TFSID int,
  @TFSTitle varchar(8000),
  @TFSURL varchar(8000),
  @TFSState varchar(8000),
  @CreatorID int,
  @CrmLinkID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TicketLinkToTFS]
  (
    [TicketID],
    [DateModifiedByTFSSync],
    [SyncWithTFS],
    [TFSID],
    [TFSTitle],
    [TFSURL],
    [TFSState],
    [CreatorID],
    [CrmLinkID])
  VALUES (
    @TicketID,
    @DateModifiedByTFSSync,
    @SyncWithTFS,
    @TFSID,
    @TFSTitle,
    @TFSURL,
    @TFSState,
    @CreatorID,
    @CrmLinkID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTicketLinkToTFSItem

(
  @id int,
  @TicketID int,
  @DateModifiedByTFSSync datetime,
  @SyncWithTFS bit,
  @TFSID int,
  @TFSTitle varchar(8000),
  @TFSURL varchar(8000),
  @TFSState varchar(8000),
  @CrmLinkID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TicketLinkToTFS]
  SET
    [TicketID] = @TicketID,
    [DateModifiedByTFSSync] = @DateModifiedByTFSSync,
    [SyncWithTFS] = @SyncWithTFS,
    [TFSID] = @TFSID,
    [TFSTitle] = @TFSTitle,
    [TFSURL] = @TFSURL,
    [TFSState] = @TFSState,
    [CrmLinkID] = @CrmLinkID
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTicketLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TicketLinkToTFS]
  WHERE ([id] = @id)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectActionLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [id],
    [ActionID],
    [DateModifiedByTFSSync],
    [TFSID]
  FROM [dbo].[ActionLinkToTFS]
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertActionLinkToTFSItem

(
  @ActionID int,
  @DateModifiedByTFSSync datetime,
  @TFSID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ActionLinkToTFS]
  (
    [ActionID],
    [DateModifiedByTFSSync],
    [TFSID])
  VALUES (
    @ActionID,
    @DateModifiedByTFSSync,
    @TFSID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateActionLinkToTFSItem

(
  @id int,
  @ActionID int,
  @DateModifiedByTFSSync datetime,
  @TFSID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ActionLinkToTFS]
  SET
    [ActionID] = @ActionID,
    [DateModifiedByTFSSync] = @DateModifiedByTFSSync,
    [TFSID] = @TFSID
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteActionLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ActionLinkToTFS]
  WHERE ([id] = @id)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTicketLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [id],
    [TicketID],
    [DateModifiedByTFSSync],
    [SyncWithTFS],
    [TFSID],
    [TFSTitle],
    [TFSURL],
    [TFSState],
    [CreatorID],
    [CrmLinkID]
  FROM [dbo].[TicketLinkToTFS]
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTicketLinkToTFSItem

(
  @TicketID int,
  @DateModifiedByTFSSync datetime,
  @SyncWithTFS bit,
  @TFSID int,
  @TFSTitle varchar(8000),
  @TFSURL varchar(8000),
  @TFSState varchar(8000),
  @CreatorID int,
  @CrmLinkID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TicketLinkToTFS]
  (
    [TicketID],
    [DateModifiedByTFSSync],
    [SyncWithTFS],
    [TFSID],
    [TFSTitle],
    [TFSURL],
    [TFSState],
    [CreatorID],
    [CrmLinkID])
  VALUES (
    @TicketID,
    @DateModifiedByTFSSync,
    @SyncWithTFS,
    @TFSID,
    @TFSTitle,
    @TFSURL,
    @TFSState,
    @CreatorID,
    @CrmLinkID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTicketLinkToTFSItem

(
  @id int,
  @TicketID int,
  @DateModifiedByTFSSync datetime,
  @SyncWithTFS bit,
  @TFSID int,
  @TFSTitle varchar(8000),
  @TFSURL varchar(8000),
  @TFSState varchar(8000),
  @CrmLinkID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TicketLinkToTFS]
  SET
    [TicketID] = @TicketID,
    [DateModifiedByTFSSync] = @DateModifiedByTFSSync,
    [SyncWithTFS] = @SyncWithTFS,
    [TFSID] = @TFSID,
    [TFSTitle] = @TFSTitle,
    [TFSURL] = @TFSURL,
    [TFSState] = @TFSState,
    [CrmLinkID] = @CrmLinkID
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTicketLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TicketLinkToTFS]
  WHERE ([id] = @id)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectActionLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [id],
    [ActionID],
    [DateModifiedByTFSSync],
    [TFSID]
  FROM [dbo].[ActionLinkToTFS]
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertActionLinkToTFSItem

(
  @ActionID int,
  @DateModifiedByTFSSync datetime,
  @TFSID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ActionLinkToTFS]
  (
    [ActionID],
    [DateModifiedByTFSSync],
    [TFSID])
  VALUES (
    @ActionID,
    @DateModifiedByTFSSync,
    @TFSID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateActionLinkToTFSItem

(
  @id int,
  @ActionID int,
  @DateModifiedByTFSSync datetime,
  @TFSID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ActionLinkToTFS]
  SET
    [ActionID] = @ActionID,
    [DateModifiedByTFSSync] = @DateModifiedByTFSSync,
    [TFSID] = @TFSID
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteActionLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ActionLinkToTFS]
  WHERE ([id] = @id)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTicketLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [id],
    [TicketID],
    [DateModifiedByTFSSync],
    [SyncWithTFS],
    [TFSID],
    [TFSTitle],
    [TFSURL],
    [TFSState],
    [CreatorID],
    [CrmLinkID]
  FROM [dbo].[TicketLinkToTFS]
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTicketLinkToTFSItem

(
  @TicketID int,
  @DateModifiedByTFSSync datetime,
  @SyncWithTFS bit,
  @TFSID int,
  @TFSTitle varchar(8000),
  @TFSURL varchar(8000),
  @TFSState varchar(8000),
  @CreatorID int,
  @CrmLinkID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TicketLinkToTFS]
  (
    [TicketID],
    [DateModifiedByTFSSync],
    [SyncWithTFS],
    [TFSID],
    [TFSTitle],
    [TFSURL],
    [TFSState],
    [CreatorID],
    [CrmLinkID])
  VALUES (
    @TicketID,
    @DateModifiedByTFSSync,
    @SyncWithTFS,
    @TFSID,
    @TFSTitle,
    @TFSURL,
    @TFSState,
    @CreatorID,
    @CrmLinkID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTicketLinkToTFSItem

(
  @id int,
  @TicketID int,
  @DateModifiedByTFSSync datetime,
  @SyncWithTFS bit,
  @TFSID int,
  @TFSTitle varchar(8000),
  @TFSURL varchar(8000),
  @TFSState varchar(8000),
  @CrmLinkID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TicketLinkToTFS]
  SET
    [TicketID] = @TicketID,
    [DateModifiedByTFSSync] = @DateModifiedByTFSSync,
    [SyncWithTFS] = @SyncWithTFS,
    [TFSID] = @TFSID,
    [TFSTitle] = @TFSTitle,
    [TFSURL] = @TFSURL,
    [TFSState] = @TFSState,
    [CrmLinkID] = @CrmLinkID
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTicketLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TicketLinkToTFS]
  WHERE ([id] = @id)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectActionLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [id],
    [ActionID],
    [DateModifiedByTFSSync],
    [TFSID]
  FROM [dbo].[ActionLinkToTFS]
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertActionLinkToTFSItem

(
  @ActionID int,
  @DateModifiedByTFSSync datetime,
  @TFSID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ActionLinkToTFS]
  (
    [ActionID],
    [DateModifiedByTFSSync],
    [TFSID])
  VALUES (
    @ActionID,
    @DateModifiedByTFSSync,
    @TFSID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateActionLinkToTFSItem

(
  @id int,
  @ActionID int,
  @DateModifiedByTFSSync datetime,
  @TFSID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ActionLinkToTFS]
  SET
    [ActionID] = @ActionID,
    [DateModifiedByTFSSync] = @DateModifiedByTFSSync,
    [TFSID] = @TFSID
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteActionLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ActionLinkToTFS]
  WHERE ([id] = @id)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTicketLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [id],
    [TicketID],
    [DateModifiedByTFSSync],
    [SyncWithTFS],
    [TFSID],
    [TFSTitle],
    [TFSURL],
    [TFSState],
    [CreatorID],
    [CrmLinkID]
  FROM [dbo].[TicketLinkToTFS]
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTicketLinkToTFSItem

(
  @TicketID int,
  @DateModifiedByTFSSync datetime,
  @SyncWithTFS bit,
  @TFSID int,
  @TFSTitle varchar(8000),
  @TFSURL varchar(8000),
  @TFSState varchar(8000),
  @CreatorID int,
  @CrmLinkID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TicketLinkToTFS]
  (
    [TicketID],
    [DateModifiedByTFSSync],
    [SyncWithTFS],
    [TFSID],
    [TFSTitle],
    [TFSURL],
    [TFSState],
    [CreatorID],
    [CrmLinkID])
  VALUES (
    @TicketID,
    @DateModifiedByTFSSync,
    @SyncWithTFS,
    @TFSID,
    @TFSTitle,
    @TFSURL,
    @TFSState,
    @CreatorID,
    @CrmLinkID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTicketLinkToTFSItem

(
  @id int,
  @TicketID int,
  @DateModifiedByTFSSync datetime,
  @SyncWithTFS bit,
  @TFSID int,
  @TFSTitle varchar(8000),
  @TFSURL varchar(8000),
  @TFSState varchar(8000),
  @CrmLinkID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TicketLinkToTFS]
  SET
    [TicketID] = @TicketID,
    [DateModifiedByTFSSync] = @DateModifiedByTFSSync,
    [SyncWithTFS] = @SyncWithTFS,
    [TFSID] = @TFSID,
    [TFSTitle] = @TFSTitle,
    [TFSURL] = @TFSURL,
    [TFSState] = @TFSState,
    [CrmLinkID] = @CrmLinkID
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTicketLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TicketLinkToTFS]
  WHERE ([id] = @id)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectActionLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [id],
    [ActionID],
    [DateModifiedByTFSSync],
    [TFSID]
  FROM [dbo].[ActionLinkToTFS]
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertActionLinkToTFSItem

(
  @ActionID int,
  @DateModifiedByTFSSync datetime,
  @TFSID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ActionLinkToTFS]
  (
    [ActionID],
    [DateModifiedByTFSSync],
    [TFSID])
  VALUES (
    @ActionID,
    @DateModifiedByTFSSync,
    @TFSID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateActionLinkToTFSItem

(
  @id int,
  @ActionID int,
  @DateModifiedByTFSSync datetime,
  @TFSID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ActionLinkToTFS]
  SET
    [ActionID] = @ActionID,
    [DateModifiedByTFSSync] = @DateModifiedByTFSSync,
    [TFSID] = @TFSID
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteActionLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ActionLinkToTFS]
  WHERE ([id] = @id)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTicketLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [id],
    [TicketID],
    [DateModifiedByTFSSync],
    [SyncWithTFS],
    [TFSID],
    [TFSTitle],
    [TFSURL],
    [TFSState],
    [CreatorID],
    [CrmLinkID]
  FROM [dbo].[TicketLinkToTFS]
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTicketLinkToTFSItem

(
  @TicketID int,
  @DateModifiedByTFSSync datetime,
  @SyncWithTFS bit,
  @TFSID int,
  @TFSTitle varchar(8000),
  @TFSURL varchar(8000),
  @TFSState varchar(8000),
  @CreatorID int,
  @CrmLinkID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TicketLinkToTFS]
  (
    [TicketID],
    [DateModifiedByTFSSync],
    [SyncWithTFS],
    [TFSID],
    [TFSTitle],
    [TFSURL],
    [TFSState],
    [CreatorID],
    [CrmLinkID])
  VALUES (
    @TicketID,
    @DateModifiedByTFSSync,
    @SyncWithTFS,
    @TFSID,
    @TFSTitle,
    @TFSURL,
    @TFSState,
    @CreatorID,
    @CrmLinkID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTicketLinkToTFSItem

(
  @id int,
  @TicketID int,
  @DateModifiedByTFSSync datetime,
  @SyncWithTFS bit,
  @TFSID int,
  @TFSTitle varchar(8000),
  @TFSURL varchar(8000),
  @TFSState varchar(8000),
  @CrmLinkID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TicketLinkToTFS]
  SET
    [TicketID] = @TicketID,
    [DateModifiedByTFSSync] = @DateModifiedByTFSSync,
    [SyncWithTFS] = @SyncWithTFS,
    [TFSID] = @TFSID,
    [TFSTitle] = @TFSTitle,
    [TFSURL] = @TFSURL,
    [TFSState] = @TFSState,
    [CrmLinkID] = @CrmLinkID
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTicketLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTicketLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTicketLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TicketLinkToTFS]
  WHERE ([id] = @id)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectActionLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [id],
    [ActionID],
    [DateModifiedByTFSSync],
    [TFSID]
  FROM [dbo].[ActionLinkToTFS]
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertActionLinkToTFSItem

(
  @ActionID int,
  @DateModifiedByTFSSync datetime,
  @TFSID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[ActionLinkToTFS]
  (
    [ActionID],
    [DateModifiedByTFSSync],
    [TFSID])
  VALUES (
    @ActionID,
    @DateModifiedByTFSSync,
    @TFSID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateActionLinkToTFSItem

(
  @id int,
  @ActionID int,
  @DateModifiedByTFSSync datetime,
  @TFSID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[ActionLinkToTFS]
  SET
    [ActionID] = @ActionID,
    [DateModifiedByTFSSync] = @DateModifiedByTFSSync,
    [TFSID] = @TFSID
  WHERE ([id] = @id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteActionLinkToTFSItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteActionLinkToTFSItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteActionLinkToTFSItem

(
  @id int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[ActionLinkToTFS]
  WHERE ([id] = @id)
GO


