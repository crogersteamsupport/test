IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectSearchStandardFilter' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectSearchStandardFilter
GO

CREATE PROCEDURE dbo.uspGeneratedSelectSearchStandardFilter

(
  @StandardFilterID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [StandardFilterID],
    [UserID],
    [Tickets],
    [KnowledgeBase],
    [Wikis],
    [Notes],
    [ProductVersions]
  FROM [dbo].[SearchStandardFilters]
  WHERE ([StandardFilterID] = @StandardFilterID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertSearchStandardFilter' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertSearchStandardFilter
GO

CREATE PROCEDURE dbo.uspGeneratedInsertSearchStandardFilter

(
  @UserID int,
  @Tickets bit,
  @KnowledgeBase bit,
  @Wikis bit,
  @Notes bit,
  @ProductVersions bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[SearchStandardFilters]
  (
    [UserID],
    [Tickets],
    [KnowledgeBase],
    [Wikis],
    [Notes],
    [ProductVersions])
  VALUES (
    @UserID,
    @Tickets,
    @KnowledgeBase,
    @Wikis,
    @Notes,
    @ProductVersions)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateSearchStandardFilter' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateSearchStandardFilter
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateSearchStandardFilter

(
  @StandardFilterID int,
  @UserID int,
  @Tickets bit,
  @KnowledgeBase bit,
  @Wikis bit,
  @Notes bit,
  @ProductVersions bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[SearchStandardFilters]
  SET
    [UserID] = @UserID,
    [Tickets] = @Tickets,
    [KnowledgeBase] = @KnowledgeBase,
    [Wikis] = @Wikis,
    [Notes] = @Notes,
    [ProductVersions] = @ProductVersions
  WHERE ([StandardFilterID] = @StandardFilterID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteSearchStandardFilter' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteSearchStandardFilter
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteSearchStandardFilter

(
  @StandardFilterID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[SearchStandardFilters]
  WHERE ([StandardFilterID] = @StandardFilterID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectSearchStandardFilter' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectSearchStandardFilter
GO

CREATE PROCEDURE dbo.uspGeneratedSelectSearchStandardFilter

(
  @StandardFilterID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [StandardFilterID],
    [UserID],
    [Tickets],
    [KnowledgeBase],
    [Wikis],
    [Notes],
    [ProductVersions]
  FROM [dbo].[SearchStandardFilters]
  WHERE ([StandardFilterID] = @StandardFilterID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertSearchStandardFilter' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertSearchStandardFilter
GO

CREATE PROCEDURE dbo.uspGeneratedInsertSearchStandardFilter

(
  @UserID int,
  @Tickets bit,
  @KnowledgeBase bit,
  @Wikis bit,
  @Notes bit,
  @ProductVersions bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[SearchStandardFilters]
  (
    [UserID],
    [Tickets],
    [KnowledgeBase],
    [Wikis],
    [Notes],
    [ProductVersions])
  VALUES (
    @UserID,
    @Tickets,
    @KnowledgeBase,
    @Wikis,
    @Notes,
    @ProductVersions)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateSearchStandardFilter' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateSearchStandardFilter
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateSearchStandardFilter

(
  @StandardFilterID int,
  @UserID int,
  @Tickets bit,
  @KnowledgeBase bit,
  @Wikis bit,
  @Notes bit,
  @ProductVersions bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[SearchStandardFilters]
  SET
    [UserID] = @UserID,
    [Tickets] = @Tickets,
    [KnowledgeBase] = @KnowledgeBase,
    [Wikis] = @Wikis,
    [Notes] = @Notes,
    [ProductVersions] = @ProductVersions
  WHERE ([StandardFilterID] = @StandardFilterID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteSearchStandardFilter' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteSearchStandardFilter
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteSearchStandardFilter

(
  @StandardFilterID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[SearchStandardFilters]
  WHERE ([StandardFilterID] = @StandardFilterID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectSearchStandardFilter' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectSearchStandardFilter
GO

CREATE PROCEDURE dbo.uspGeneratedSelectSearchStandardFilter

(
  @StandardFilterID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [StandardFilterID],
    [UserID],
    [Tickets],
    [KnowledgeBase],
    [Wikis],
    [Notes],
    [ProductVersions]
  FROM [dbo].[SearchStandardFilters]
  WHERE ([StandardFilterID] = @StandardFilterID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertSearchStandardFilter' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertSearchStandardFilter
GO

CREATE PROCEDURE dbo.uspGeneratedInsertSearchStandardFilter

(
  @UserID int,
  @Tickets bit,
  @KnowledgeBase bit,
  @Wikis bit,
  @Notes bit,
  @ProductVersions bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[SearchStandardFilters]
  (
    [UserID],
    [Tickets],
    [KnowledgeBase],
    [Wikis],
    [Notes],
    [ProductVersions])
  VALUES (
    @UserID,
    @Tickets,
    @KnowledgeBase,
    @Wikis,
    @Notes,
    @ProductVersions)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateSearchStandardFilter' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateSearchStandardFilter
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateSearchStandardFilter

(
  @StandardFilterID int,
  @UserID int,
  @Tickets bit,
  @KnowledgeBase bit,
  @Wikis bit,
  @Notes bit,
  @ProductVersions bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[SearchStandardFilters]
  SET
    [UserID] = @UserID,
    [Tickets] = @Tickets,
    [KnowledgeBase] = @KnowledgeBase,
    [Wikis] = @Wikis,
    [Notes] = @Notes,
    [ProductVersions] = @ProductVersions
  WHERE ([StandardFilterID] = @StandardFilterID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteSearchStandardFilter' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteSearchStandardFilter
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteSearchStandardFilter

(
  @StandardFilterID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[SearchStandardFilters]
  WHERE ([StandardFilterID] = @StandardFilterID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectSearchStandardFilter' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectSearchStandardFilter
GO

CREATE PROCEDURE dbo.uspGeneratedSelectSearchStandardFilter

(
  @StandardFilterID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [StandardFilterID],
    [UserID],
    [Tickets],
    [KnowledgeBase],
    [Wikis],
    [Notes],
    [ProductVersions]
  FROM [dbo].[SearchStandardFilters]
  WHERE ([StandardFilterID] = @StandardFilterID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertSearchStandardFilter' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertSearchStandardFilter
GO

CREATE PROCEDURE dbo.uspGeneratedInsertSearchStandardFilter

(
  @UserID int,
  @Tickets bit,
  @KnowledgeBase bit,
  @Wikis bit,
  @Notes bit,
  @ProductVersions bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[SearchStandardFilters]
  (
    [UserID],
    [Tickets],
    [KnowledgeBase],
    [Wikis],
    [Notes],
    [ProductVersions])
  VALUES (
    @UserID,
    @Tickets,
    @KnowledgeBase,
    @Wikis,
    @Notes,
    @ProductVersions)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateSearchStandardFilter' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateSearchStandardFilter
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateSearchStandardFilter

(
  @StandardFilterID int,
  @UserID int,
  @Tickets bit,
  @KnowledgeBase bit,
  @Wikis bit,
  @Notes bit,
  @ProductVersions bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[SearchStandardFilters]
  SET
    [UserID] = @UserID,
    [Tickets] = @Tickets,
    [KnowledgeBase] = @KnowledgeBase,
    [Wikis] = @Wikis,
    [Notes] = @Notes,
    [ProductVersions] = @ProductVersions
  WHERE ([StandardFilterID] = @StandardFilterID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteSearchStandardFilter' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteSearchStandardFilter
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteSearchStandardFilter

(
  @StandardFilterID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[SearchStandardFilters]
  WHERE ([StandardFilterID] = @StandardFilterID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectSearchStandardFilter' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectSearchStandardFilter
GO

CREATE PROCEDURE dbo.uspGeneratedSelectSearchStandardFilter

(
  @StandardFilterID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [StandardFilterID],
    [UserID],
    [Tickets],
    [KnowledgeBase],
    [Wikis],
    [Notes],
    [ProductVersions]
  FROM [dbo].[SearchStandardFilters]
  WHERE ([StandardFilterID] = @StandardFilterID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertSearchStandardFilter' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertSearchStandardFilter
GO

CREATE PROCEDURE dbo.uspGeneratedInsertSearchStandardFilter

(
  @UserID int,
  @Tickets bit,
  @KnowledgeBase bit,
  @Wikis bit,
  @Notes bit,
  @ProductVersions bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[SearchStandardFilters]
  (
    [UserID],
    [Tickets],
    [KnowledgeBase],
    [Wikis],
    [Notes],
    [ProductVersions])
  VALUES (
    @UserID,
    @Tickets,
    @KnowledgeBase,
    @Wikis,
    @Notes,
    @ProductVersions)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateSearchStandardFilter' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateSearchStandardFilter
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateSearchStandardFilter

(
  @StandardFilterID int,
  @UserID int,
  @Tickets bit,
  @KnowledgeBase bit,
  @Wikis bit,
  @Notes bit,
  @ProductVersions bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[SearchStandardFilters]
  SET
    [UserID] = @UserID,
    [Tickets] = @Tickets,
    [KnowledgeBase] = @KnowledgeBase,
    [Wikis] = @Wikis,
    [Notes] = @Notes,
    [ProductVersions] = @ProductVersions
  WHERE ([StandardFilterID] = @StandardFilterID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteSearchStandardFilter' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteSearchStandardFilter
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteSearchStandardFilter

(
  @StandardFilterID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[SearchStandardFilters]
  WHERE ([StandardFilterID] = @StandardFilterID)
GO


