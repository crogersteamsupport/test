IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCompanyParentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCompanyParentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCompanyParentsViewItem

(
  @ChildID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ChildID],
    [ParentID],
    [ParentName]
  FROM [dbo].[CompanyParentsView]
  WHERE ([ChildID] = @ChildID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCompanyParentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCompanyParentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCompanyParentsViewItem

(
  @ChildID int,
  @ParentID int,
  @ParentName nvarchar(255),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CompanyParentsView]
  (
    [ChildID],
    [ParentID],
    [ParentName])
  VALUES (
    @ChildID,
    @ParentID,
    @ParentName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCompanyParentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCompanyParentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCompanyParentsViewItem

(
  @ChildID int,
  @ParentID int,
  @ParentName nvarchar(255)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CompanyParentsView]
  SET
    [ParentID] = @ParentID,
    [ParentName] = @ParentName
  WHERE ([ChildID] = @ChildID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCompanyParentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCompanyParentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCompanyParentsViewItem

(
  @ChildID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CompanyParentsView]
  WHERE ([ChildID] = @ChildID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCompanyParentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCompanyParentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCompanyParentsViewItem

(
  @ChildID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ChildID],
    [ParentID],
    [ParentName]
  FROM [dbo].[CompanyParentsView]
  WHERE ([ChildID] = @ChildID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCompanyParentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCompanyParentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCompanyParentsViewItem

(
  @ChildID int,
  @ParentID int,
  @ParentName nvarchar(255),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CompanyParentsView]
  (
    [ChildID],
    [ParentID],
    [ParentName])
  VALUES (
    @ChildID,
    @ParentID,
    @ParentName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCompanyParentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCompanyParentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCompanyParentsViewItem

(
  @ChildID int,
  @ParentID int,
  @ParentName nvarchar(255)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CompanyParentsView]
  SET
    [ParentID] = @ParentID,
    [ParentName] = @ParentName
  WHERE ([ChildID] = @ChildID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCompanyParentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCompanyParentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCompanyParentsViewItem

(
  @ChildID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CompanyParentsView]
  WHERE ([ChildID] = @ChildID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCompanyParentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCompanyParentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCompanyParentsViewItem

(
  @ChildID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ChildID],
    [ParentID],
    [ParentName]
  FROM [dbo].[CompanyParentsView]
  WHERE ([ChildID] = @ChildID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCompanyParentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCompanyParentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCompanyParentsViewItem

(
  @ChildID int,
  @ParentID int,
  @ParentName nvarchar(255),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CompanyParentsView]
  (
    [ChildID],
    [ParentID],
    [ParentName])
  VALUES (
    @ChildID,
    @ParentID,
    @ParentName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCompanyParentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCompanyParentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCompanyParentsViewItem

(
  @ChildID int,
  @ParentID int,
  @ParentName nvarchar(255)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CompanyParentsView]
  SET
    [ParentID] = @ParentID,
    [ParentName] = @ParentName
  WHERE ([ChildID] = @ChildID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCompanyParentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCompanyParentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCompanyParentsViewItem

(
  @ChildID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CompanyParentsView]
  WHERE ([ChildID] = @ChildID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCompanyParentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCompanyParentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCompanyParentsViewItem

(
  @ChildID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ChildID],
    [ParentID],
    [ParentName]
  FROM [dbo].[CompanyParentsView]
  WHERE ([ChildID] = @ChildID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCompanyParentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCompanyParentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCompanyParentsViewItem

(
  @ChildID int,
  @ParentID int,
  @ParentName nvarchar(255),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CompanyParentsView]
  (
    [ChildID],
    [ParentID],
    [ParentName])
  VALUES (
    @ChildID,
    @ParentID,
    @ParentName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCompanyParentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCompanyParentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCompanyParentsViewItem

(
  @ChildID int,
  @ParentID int,
  @ParentName nvarchar(255)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CompanyParentsView]
  SET
    [ParentID] = @ParentID,
    [ParentName] = @ParentName
  WHERE ([ChildID] = @ChildID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCompanyParentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCompanyParentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCompanyParentsViewItem

(
  @ChildID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CompanyParentsView]
  WHERE ([ChildID] = @ChildID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCompanyParentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCompanyParentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCompanyParentsViewItem

(
  @ChildID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ChildID],
    [ParentID],
    [ParentName]
  FROM [dbo].[CompanyParentsView]
  WHERE ([ChildID] = @ChildID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCompanyParentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCompanyParentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCompanyParentsViewItem

(
  @ChildID int,
  @ParentID int,
  @ParentName nvarchar(255),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CompanyParentsView]
  (
    [ChildID],
    [ParentID],
    [ParentName])
  VALUES (
    @ChildID,
    @ParentID,
    @ParentName)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCompanyParentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCompanyParentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCompanyParentsViewItem

(
  @ChildID int,
  @ParentID int,
  @ParentName nvarchar(255)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CompanyParentsView]
  SET
    [ParentID] = @ParentID,
    [ParentName] = @ParentName
  WHERE ([ChildID] = @ChildID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCompanyParentsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCompanyParentsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCompanyParentsViewItem

(
  @ChildID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CompanyParentsView]
  WHERE ([ChildID] = @ChildID)
GO


