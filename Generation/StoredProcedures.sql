IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectNotesViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectNotesViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectNotesViewItem

(
  @NoteID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [NoteID],
    [RefType],
    [RefID],
    [Title],
    [Description],
    [CreatorID],
    [ModifierID],
    [DateModified],
    [DateCreated],
    [NeedsIndexing],
    [CreatorName],
    [ModifierName],
    [ParentOrganizationID],
    [OrganizationName],
    [ContactName]
  FROM [dbo].[NotesView]
  WHERE ([NoteID] = @NoteID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertNotesViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertNotesViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertNotesViewItem

(
  @NoteID int,
  @RefType int,
  @RefID int,
  @Title varchar(1000),
  @Description varchar(MAX),
  @CreatorID int,
  @ModifierID int,
  @DateModified datetime,
  @DateCreated datetime,
  @NeedsIndexing bit,
  @CreatorName varchar(201),
  @ModifierName varchar(201),
  @ParentOrganizationID int,
  @OrganizationName varchar(255),
  @ContactName varchar(201),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[NotesView]
  (
    [NoteID],
    [RefType],
    [RefID],
    [Title],
    [Description],
    [CreatorID],
    [ModifierID],
    [DateModified],
    [DateCreated],
    [NeedsIndexing],
    [CreatorName],
    [ModifierName],
    [ParentOrganizationID],
    [OrganizationName],
    [ContactName])
  VALUES (
    @NoteID,
    @RefType,
    @RefID,
    @Title,
    @Description,
    @CreatorID,
    @ModifierID,
    @DateModified,
    @DateCreated,
    @NeedsIndexing,
    @CreatorName,
    @ModifierName,
    @ParentOrganizationID,
    @OrganizationName,
    @ContactName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateNotesViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateNotesViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateNotesViewItem

(
  @NoteID int,
  @RefType int,
  @RefID int,
  @Title varchar(1000),
  @Description varchar(MAX),
  @ModifierID int,
  @DateModified datetime,
  @NeedsIndexing bit,
  @CreatorName varchar(201),
  @ModifierName varchar(201),
  @ParentOrganizationID int,
  @OrganizationName varchar(255),
  @ContactName varchar(201)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[NotesView]
  SET
    [RefType] = @RefType,
    [RefID] = @RefID,
    [Title] = @Title,
    [Description] = @Description,
    [ModifierID] = @ModifierID,
    [DateModified] = @DateModified,
    [NeedsIndexing] = @NeedsIndexing,
    [CreatorName] = @CreatorName,
    [ModifierName] = @ModifierName,
    [ParentOrganizationID] = @ParentOrganizationID,
    [OrganizationName] = @OrganizationName,
    [ContactName] = @ContactName
  WHERE ([NoteID] = @NoteID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteNotesViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteNotesViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteNotesViewItem

(
  @NoteID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[NotesView]
  WHERE ([NoteID] = @NoteID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectNotesViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectNotesViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectNotesViewItem

(
  @NoteID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [NoteID],
    [RefType],
    [RefID],
    [Title],
    [Description],
    [CreatorID],
    [ModifierID],
    [DateModified],
    [DateCreated],
    [NeedsIndexing],
    [CreatorName],
    [ModifierName],
    [ParentOrganizationID],
    [OrganizationName],
    [ContactName]
  FROM [dbo].[NotesView]
  WHERE ([NoteID] = @NoteID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertNotesViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertNotesViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertNotesViewItem

(
  @NoteID int,
  @RefType int,
  @RefID int,
  @Title varchar(1000),
  @Description varchar(MAX),
  @CreatorID int,
  @ModifierID int,
  @DateModified datetime,
  @DateCreated datetime,
  @NeedsIndexing bit,
  @CreatorName varchar(201),
  @ModifierName varchar(201),
  @ParentOrganizationID int,
  @OrganizationName varchar(255),
  @ContactName varchar(201),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[NotesView]
  (
    [NoteID],
    [RefType],
    [RefID],
    [Title],
    [Description],
    [CreatorID],
    [ModifierID],
    [DateModified],
    [DateCreated],
    [NeedsIndexing],
    [CreatorName],
    [ModifierName],
    [ParentOrganizationID],
    [OrganizationName],
    [ContactName])
  VALUES (
    @NoteID,
    @RefType,
    @RefID,
    @Title,
    @Description,
    @CreatorID,
    @ModifierID,
    @DateModified,
    @DateCreated,
    @NeedsIndexing,
    @CreatorName,
    @ModifierName,
    @ParentOrganizationID,
    @OrganizationName,
    @ContactName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateNotesViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateNotesViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateNotesViewItem

(
  @NoteID int,
  @RefType int,
  @RefID int,
  @Title varchar(1000),
  @Description varchar(MAX),
  @ModifierID int,
  @DateModified datetime,
  @NeedsIndexing bit,
  @CreatorName varchar(201),
  @ModifierName varchar(201),
  @ParentOrganizationID int,
  @OrganizationName varchar(255),
  @ContactName varchar(201)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[NotesView]
  SET
    [RefType] = @RefType,
    [RefID] = @RefID,
    [Title] = @Title,
    [Description] = @Description,
    [ModifierID] = @ModifierID,
    [DateModified] = @DateModified,
    [NeedsIndexing] = @NeedsIndexing,
    [CreatorName] = @CreatorName,
    [ModifierName] = @ModifierName,
    [ParentOrganizationID] = @ParentOrganizationID,
    [OrganizationName] = @OrganizationName,
    [ContactName] = @ContactName
  WHERE ([NoteID] = @NoteID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteNotesViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteNotesViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteNotesViewItem

(
  @NoteID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[NotesView]
  WHERE ([NoteID] = @NoteID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectNotesViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectNotesViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectNotesViewItem

(
  @NoteID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [NoteID],
    [RefType],
    [RefID],
    [Title],
    [Description],
    [CreatorID],
    [ModifierID],
    [DateModified],
    [DateCreated],
    [NeedsIndexing],
    [CreatorName],
    [ModifierName],
    [ParentOrganizationID],
    [OrganizationName],
    [ContactName]
  FROM [dbo].[NotesView]
  WHERE ([NoteID] = @NoteID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertNotesViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertNotesViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertNotesViewItem

(
  @NoteID int,
  @RefType int,
  @RefID int,
  @Title varchar(1000),
  @Description varchar(MAX),
  @CreatorID int,
  @ModifierID int,
  @DateModified datetime,
  @DateCreated datetime,
  @NeedsIndexing bit,
  @CreatorName varchar(201),
  @ModifierName varchar(201),
  @ParentOrganizationID int,
  @OrganizationName varchar(255),
  @ContactName varchar(201),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[NotesView]
  (
    [NoteID],
    [RefType],
    [RefID],
    [Title],
    [Description],
    [CreatorID],
    [ModifierID],
    [DateModified],
    [DateCreated],
    [NeedsIndexing],
    [CreatorName],
    [ModifierName],
    [ParentOrganizationID],
    [OrganizationName],
    [ContactName])
  VALUES (
    @NoteID,
    @RefType,
    @RefID,
    @Title,
    @Description,
    @CreatorID,
    @ModifierID,
    @DateModified,
    @DateCreated,
    @NeedsIndexing,
    @CreatorName,
    @ModifierName,
    @ParentOrganizationID,
    @OrganizationName,
    @ContactName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateNotesViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateNotesViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateNotesViewItem

(
  @NoteID int,
  @RefType int,
  @RefID int,
  @Title varchar(1000),
  @Description varchar(MAX),
  @ModifierID int,
  @DateModified datetime,
  @NeedsIndexing bit,
  @CreatorName varchar(201),
  @ModifierName varchar(201),
  @ParentOrganizationID int,
  @OrganizationName varchar(255),
  @ContactName varchar(201)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[NotesView]
  SET
    [RefType] = @RefType,
    [RefID] = @RefID,
    [Title] = @Title,
    [Description] = @Description,
    [ModifierID] = @ModifierID,
    [DateModified] = @DateModified,
    [NeedsIndexing] = @NeedsIndexing,
    [CreatorName] = @CreatorName,
    [ModifierName] = @ModifierName,
    [ParentOrganizationID] = @ParentOrganizationID,
    [OrganizationName] = @OrganizationName,
    [ContactName] = @ContactName
  WHERE ([NoteID] = @NoteID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteNotesViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteNotesViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteNotesViewItem

(
  @NoteID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[NotesView]
  WHERE ([NoteID] = @NoteID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectNotesViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectNotesViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectNotesViewItem

(
  @NoteID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [NoteID],
    [RefType],
    [RefID],
    [Title],
    [Description],
    [CreatorID],
    [ModifierID],
    [DateModified],
    [DateCreated],
    [NeedsIndexing],
    [CreatorName],
    [ModifierName],
    [ParentOrganizationID],
    [OrganizationName],
    [ContactName]
  FROM [dbo].[NotesView]
  WHERE ([NoteID] = @NoteID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertNotesViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertNotesViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertNotesViewItem

(
  @NoteID int,
  @RefType int,
  @RefID int,
  @Title varchar(1000),
  @Description varchar(MAX),
  @CreatorID int,
  @ModifierID int,
  @DateModified datetime,
  @DateCreated datetime,
  @NeedsIndexing bit,
  @CreatorName varchar(201),
  @ModifierName varchar(201),
  @ParentOrganizationID int,
  @OrganizationName varchar(255),
  @ContactName varchar(201),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[NotesView]
  (
    [NoteID],
    [RefType],
    [RefID],
    [Title],
    [Description],
    [CreatorID],
    [ModifierID],
    [DateModified],
    [DateCreated],
    [NeedsIndexing],
    [CreatorName],
    [ModifierName],
    [ParentOrganizationID],
    [OrganizationName],
    [ContactName])
  VALUES (
    @NoteID,
    @RefType,
    @RefID,
    @Title,
    @Description,
    @CreatorID,
    @ModifierID,
    @DateModified,
    @DateCreated,
    @NeedsIndexing,
    @CreatorName,
    @ModifierName,
    @ParentOrganizationID,
    @OrganizationName,
    @ContactName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateNotesViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateNotesViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateNotesViewItem

(
  @NoteID int,
  @RefType int,
  @RefID int,
  @Title varchar(1000),
  @Description varchar(MAX),
  @ModifierID int,
  @DateModified datetime,
  @NeedsIndexing bit,
  @CreatorName varchar(201),
  @ModifierName varchar(201),
  @ParentOrganizationID int,
  @OrganizationName varchar(255),
  @ContactName varchar(201)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[NotesView]
  SET
    [RefType] = @RefType,
    [RefID] = @RefID,
    [Title] = @Title,
    [Description] = @Description,
    [ModifierID] = @ModifierID,
    [DateModified] = @DateModified,
    [NeedsIndexing] = @NeedsIndexing,
    [CreatorName] = @CreatorName,
    [ModifierName] = @ModifierName,
    [ParentOrganizationID] = @ParentOrganizationID,
    [OrganizationName] = @OrganizationName,
    [ContactName] = @ContactName
  WHERE ([NoteID] = @NoteID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteNotesViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteNotesViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteNotesViewItem

(
  @NoteID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[NotesView]
  WHERE ([NoteID] = @NoteID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectNotesViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectNotesViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectNotesViewItem

(
  @NoteID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [NoteID],
    [RefType],
    [RefID],
    [Title],
    [Description],
    [CreatorID],
    [ModifierID],
    [DateModified],
    [DateCreated],
    [NeedsIndexing],
    [CreatorName],
    [ModifierName],
    [ParentOrganizationID],
    [OrganizationName],
    [ContactName]
  FROM [dbo].[NotesView]
  WHERE ([NoteID] = @NoteID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertNotesViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertNotesViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertNotesViewItem

(
  @NoteID int,
  @RefType int,
  @RefID int,
  @Title varchar(1000),
  @Description varchar(MAX),
  @CreatorID int,
  @ModifierID int,
  @DateModified datetime,
  @DateCreated datetime,
  @NeedsIndexing bit,
  @CreatorName varchar(201),
  @ModifierName varchar(201),
  @ParentOrganizationID int,
  @OrganizationName varchar(255),
  @ContactName varchar(201),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[NotesView]
  (
    [NoteID],
    [RefType],
    [RefID],
    [Title],
    [Description],
    [CreatorID],
    [ModifierID],
    [DateModified],
    [DateCreated],
    [NeedsIndexing],
    [CreatorName],
    [ModifierName],
    [ParentOrganizationID],
    [OrganizationName],
    [ContactName])
  VALUES (
    @NoteID,
    @RefType,
    @RefID,
    @Title,
    @Description,
    @CreatorID,
    @ModifierID,
    @DateModified,
    @DateCreated,
    @NeedsIndexing,
    @CreatorName,
    @ModifierName,
    @ParentOrganizationID,
    @OrganizationName,
    @ContactName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateNotesViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateNotesViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateNotesViewItem

(
  @NoteID int,
  @RefType int,
  @RefID int,
  @Title varchar(1000),
  @Description varchar(MAX),
  @ModifierID int,
  @DateModified datetime,
  @NeedsIndexing bit,
  @CreatorName varchar(201),
  @ModifierName varchar(201),
  @ParentOrganizationID int,
  @OrganizationName varchar(255),
  @ContactName varchar(201)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[NotesView]
  SET
    [RefType] = @RefType,
    [RefID] = @RefID,
    [Title] = @Title,
    [Description] = @Description,
    [ModifierID] = @ModifierID,
    [DateModified] = @DateModified,
    [NeedsIndexing] = @NeedsIndexing,
    [CreatorName] = @CreatorName,
    [ModifierName] = @ModifierName,
    [ParentOrganizationID] = @ParentOrganizationID,
    [OrganizationName] = @OrganizationName,
    [ContactName] = @ContactName
  WHERE ([NoteID] = @NoteID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteNotesViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteNotesViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteNotesViewItem

(
  @NoteID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[NotesView]
  WHERE ([NoteID] = @NoteID)
GO


