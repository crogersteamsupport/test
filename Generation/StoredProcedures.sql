IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectFacebookOption' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectFacebookOption
GO

CREATE PROCEDURE dbo.uspGeneratedSelectFacebookOption

(
  @OrganizationID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [OrganizationID],
    [DisplayArticles],
    [DisplayKB]
  FROM [dbo].[FacebookOptions]
  WHERE ([OrganizationID] = @OrganizationID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertFacebookOption' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertFacebookOption
GO

CREATE PROCEDURE dbo.uspGeneratedInsertFacebookOption

(
  @OrganizationID int,
  @DisplayArticles bit,
  @DisplayKB bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[FacebookOptions]
  (
    [OrganizationID],
    [DisplayArticles],
    [DisplayKB])
  VALUES (
    @OrganizationID,
    @DisplayArticles,
    @DisplayKB)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateFacebookOption' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateFacebookOption
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateFacebookOption

(
  @OrganizationID int,
  @DisplayArticles bit,
  @DisplayKB bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[FacebookOptions]
  SET
    [DisplayArticles] = @DisplayArticles,
    [DisplayKB] = @DisplayKB
  WHERE ([OrganizationID] = @OrganizationID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteFacebookOption' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteFacebookOption
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteFacebookOption

(
  @OrganizationID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[FacebookOptions]
  WHERE ([OrganizationID] = @OrganizationID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectFacebookOption' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectFacebookOption
GO

CREATE PROCEDURE dbo.uspGeneratedSelectFacebookOption

(
  @OrganizationID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [OrganizationID],
    [DisplayArticles],
    [DisplayKB]
  FROM [dbo].[FacebookOptions]
  WHERE ([OrganizationID] = @OrganizationID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertFacebookOption' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertFacebookOption
GO

CREATE PROCEDURE dbo.uspGeneratedInsertFacebookOption

(
  @OrganizationID int,
  @DisplayArticles bit,
  @DisplayKB bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[FacebookOptions]
  (
    [OrganizationID],
    [DisplayArticles],
    [DisplayKB])
  VALUES (
    @OrganizationID,
    @DisplayArticles,
    @DisplayKB)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateFacebookOption' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateFacebookOption
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateFacebookOption

(
  @OrganizationID int,
  @DisplayArticles bit,
  @DisplayKB bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[FacebookOptions]
  SET
    [DisplayArticles] = @DisplayArticles,
    [DisplayKB] = @DisplayKB
  WHERE ([OrganizationID] = @OrganizationID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteFacebookOption' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteFacebookOption
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteFacebookOption

(
  @OrganizationID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[FacebookOptions]
  WHERE ([OrganizationID] = @OrganizationID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectFacebookOption' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectFacebookOption
GO

CREATE PROCEDURE dbo.uspGeneratedSelectFacebookOption

(
  @OrganizationID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [OrganizationID],
    [DisplayArticles],
    [DisplayKB]
  FROM [dbo].[FacebookOptions]
  WHERE ([OrganizationID] = @OrganizationID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertFacebookOption' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertFacebookOption
GO

CREATE PROCEDURE dbo.uspGeneratedInsertFacebookOption

(
  @OrganizationID int,
  @DisplayArticles bit,
  @DisplayKB bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[FacebookOptions]
  (
    [OrganizationID],
    [DisplayArticles],
    [DisplayKB])
  VALUES (
    @OrganizationID,
    @DisplayArticles,
    @DisplayKB)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateFacebookOption' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateFacebookOption
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateFacebookOption

(
  @OrganizationID int,
  @DisplayArticles bit,
  @DisplayKB bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[FacebookOptions]
  SET
    [DisplayArticles] = @DisplayArticles,
    [DisplayKB] = @DisplayKB
  WHERE ([OrganizationID] = @OrganizationID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteFacebookOption' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteFacebookOption
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteFacebookOption

(
  @OrganizationID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[FacebookOptions]
  WHERE ([OrganizationID] = @OrganizationID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectFacebookOption' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectFacebookOption
GO

CREATE PROCEDURE dbo.uspGeneratedSelectFacebookOption

(
  @OrganizationID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [OrganizationID],
    [DisplayArticles],
    [DisplayKB]
  FROM [dbo].[FacebookOptions]
  WHERE ([OrganizationID] = @OrganizationID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertFacebookOption' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertFacebookOption
GO

CREATE PROCEDURE dbo.uspGeneratedInsertFacebookOption

(
  @OrganizationID int,
  @DisplayArticles bit,
  @DisplayKB bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[FacebookOptions]
  (
    [OrganizationID],
    [DisplayArticles],
    [DisplayKB])
  VALUES (
    @OrganizationID,
    @DisplayArticles,
    @DisplayKB)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateFacebookOption' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateFacebookOption
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateFacebookOption

(
  @OrganizationID int,
  @DisplayArticles bit,
  @DisplayKB bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[FacebookOptions]
  SET
    [DisplayArticles] = @DisplayArticles,
    [DisplayKB] = @DisplayKB
  WHERE ([OrganizationID] = @OrganizationID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteFacebookOption' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteFacebookOption
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteFacebookOption

(
  @OrganizationID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[FacebookOptions]
  WHERE ([OrganizationID] = @OrganizationID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectFacebookOption' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectFacebookOption
GO

CREATE PROCEDURE dbo.uspGeneratedSelectFacebookOption

(
  @OrganizationID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [OrganizationID],
    [DisplayArticles],
    [DisplayKB]
  FROM [dbo].[FacebookOptions]
  WHERE ([OrganizationID] = @OrganizationID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertFacebookOption' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertFacebookOption
GO

CREATE PROCEDURE dbo.uspGeneratedInsertFacebookOption

(
  @OrganizationID int,
  @DisplayArticles bit,
  @DisplayKB bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[FacebookOptions]
  (
    [OrganizationID],
    [DisplayArticles],
    [DisplayKB])
  VALUES (
    @OrganizationID,
    @DisplayArticles,
    @DisplayKB)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateFacebookOption' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateFacebookOption
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateFacebookOption

(
  @OrganizationID int,
  @DisplayArticles bit,
  @DisplayKB bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[FacebookOptions]
  SET
    [DisplayArticles] = @DisplayArticles,
    [DisplayKB] = @DisplayKB
  WHERE ([OrganizationID] = @OrganizationID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteFacebookOption' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteFacebookOption
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteFacebookOption

(
  @OrganizationID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[FacebookOptions]
  WHERE ([OrganizationID] = @OrganizationID)
GO


