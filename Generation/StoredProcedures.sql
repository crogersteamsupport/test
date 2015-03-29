IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTicketTyp' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTicketTyp
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTicketTyp

(
  @TicketTypeID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [TicketTypeID],
    [Name],
    [Description],
    [Position],
    [OrganizationID],
    [IconUrl],
    [IsVisibleOnPortal],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [ProductFamilyID]
  FROM [dbo].[TicketTypes]
  WHERE ([TicketTypeID] = @TicketTypeID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTicketTyp' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTicketTyp
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTicketTyp

(
  @Name varchar(255),
  @Description varchar(1024),
  @Position int,
  @OrganizationID int,
  @IconUrl varchar(255),
  @IsVisibleOnPortal bit,
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @ProductFamilyID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TicketTypes]
  (
    [Name],
    [Description],
    [Position],
    [OrganizationID],
    [IconUrl],
    [IsVisibleOnPortal],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [ProductFamilyID])
  VALUES (
    @Name,
    @Description,
    @Position,
    @OrganizationID,
    @IconUrl,
    @IsVisibleOnPortal,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @ProductFamilyID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTicketTyp' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTicketTyp
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTicketTyp

(
  @TicketTypeID int,
  @Name varchar(255),
  @Description varchar(1024),
  @Position int,
  @OrganizationID int,
  @IconUrl varchar(255),
  @IsVisibleOnPortal bit,
  @DateModified datetime,
  @ModifierID int,
  @ProductFamilyID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TicketTypes]
  SET
    [Name] = @Name,
    [Description] = @Description,
    [Position] = @Position,
    [OrganizationID] = @OrganizationID,
    [IconUrl] = @IconUrl,
    [IsVisibleOnPortal] = @IsVisibleOnPortal,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [ProductFamilyID] = @ProductFamilyID
  WHERE ([TicketTypeID] = @TicketTypeID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTicketTyp' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTicketTyp
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTicketTyp

(
  @TicketTypeID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TicketTypes]
  WHERE ([TicketTypeID] = @TicketTypeID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTicketTyp' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTicketTyp
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTicketTyp

(
  @TicketTypeID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [TicketTypeID],
    [Name],
    [Description],
    [Position],
    [OrganizationID],
    [IconUrl],
    [IsVisibleOnPortal],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [ProductFamilyID]
  FROM [dbo].[TicketTypes]
  WHERE ([TicketTypeID] = @TicketTypeID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTicketTyp' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTicketTyp
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTicketTyp

(
  @Name varchar(255),
  @Description varchar(1024),
  @Position int,
  @OrganizationID int,
  @IconUrl varchar(255),
  @IsVisibleOnPortal bit,
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @ProductFamilyID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TicketTypes]
  (
    [Name],
    [Description],
    [Position],
    [OrganizationID],
    [IconUrl],
    [IsVisibleOnPortal],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [ProductFamilyID])
  VALUES (
    @Name,
    @Description,
    @Position,
    @OrganizationID,
    @IconUrl,
    @IsVisibleOnPortal,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @ProductFamilyID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTicketTyp' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTicketTyp
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTicketTyp

(
  @TicketTypeID int,
  @Name varchar(255),
  @Description varchar(1024),
  @Position int,
  @OrganizationID int,
  @IconUrl varchar(255),
  @IsVisibleOnPortal bit,
  @DateModified datetime,
  @ModifierID int,
  @ProductFamilyID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TicketTypes]
  SET
    [Name] = @Name,
    [Description] = @Description,
    [Position] = @Position,
    [OrganizationID] = @OrganizationID,
    [IconUrl] = @IconUrl,
    [IsVisibleOnPortal] = @IsVisibleOnPortal,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [ProductFamilyID] = @ProductFamilyID
  WHERE ([TicketTypeID] = @TicketTypeID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTicketTyp' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTicketTyp
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTicketTyp

(
  @TicketTypeID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TicketTypes]
  WHERE ([TicketTypeID] = @TicketTypeID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTicketTyp' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTicketTyp
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTicketTyp

(
  @TicketTypeID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [TicketTypeID],
    [Name],
    [Description],
    [Position],
    [OrganizationID],
    [IconUrl],
    [IsVisibleOnPortal],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [ProductFamilyID]
  FROM [dbo].[TicketTypes]
  WHERE ([TicketTypeID] = @TicketTypeID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTicketTyp' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTicketTyp
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTicketTyp

(
  @Name varchar(255),
  @Description varchar(1024),
  @Position int,
  @OrganizationID int,
  @IconUrl varchar(255),
  @IsVisibleOnPortal bit,
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @ProductFamilyID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TicketTypes]
  (
    [Name],
    [Description],
    [Position],
    [OrganizationID],
    [IconUrl],
    [IsVisibleOnPortal],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [ProductFamilyID])
  VALUES (
    @Name,
    @Description,
    @Position,
    @OrganizationID,
    @IconUrl,
    @IsVisibleOnPortal,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @ProductFamilyID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTicketTyp' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTicketTyp
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTicketTyp

(
  @TicketTypeID int,
  @Name varchar(255),
  @Description varchar(1024),
  @Position int,
  @OrganizationID int,
  @IconUrl varchar(255),
  @IsVisibleOnPortal bit,
  @DateModified datetime,
  @ModifierID int,
  @ProductFamilyID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TicketTypes]
  SET
    [Name] = @Name,
    [Description] = @Description,
    [Position] = @Position,
    [OrganizationID] = @OrganizationID,
    [IconUrl] = @IconUrl,
    [IsVisibleOnPortal] = @IsVisibleOnPortal,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [ProductFamilyID] = @ProductFamilyID
  WHERE ([TicketTypeID] = @TicketTypeID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTicketTyp' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTicketTyp
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTicketTyp

(
  @TicketTypeID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TicketTypes]
  WHERE ([TicketTypeID] = @TicketTypeID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTicketTyp' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTicketTyp
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTicketTyp

(
  @TicketTypeID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [TicketTypeID],
    [Name],
    [Description],
    [Position],
    [OrganizationID],
    [IconUrl],
    [IsVisibleOnPortal],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [ProductFamilyID]
  FROM [dbo].[TicketTypes]
  WHERE ([TicketTypeID] = @TicketTypeID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTicketTyp' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTicketTyp
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTicketTyp

(
  @Name varchar(255),
  @Description varchar(1024),
  @Position int,
  @OrganizationID int,
  @IconUrl varchar(255),
  @IsVisibleOnPortal bit,
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @ProductFamilyID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TicketTypes]
  (
    [Name],
    [Description],
    [Position],
    [OrganizationID],
    [IconUrl],
    [IsVisibleOnPortal],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [ProductFamilyID])
  VALUES (
    @Name,
    @Description,
    @Position,
    @OrganizationID,
    @IconUrl,
    @IsVisibleOnPortal,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @ProductFamilyID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTicketTyp' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTicketTyp
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTicketTyp

(
  @TicketTypeID int,
  @Name varchar(255),
  @Description varchar(1024),
  @Position int,
  @OrganizationID int,
  @IconUrl varchar(255),
  @IsVisibleOnPortal bit,
  @DateModified datetime,
  @ModifierID int,
  @ProductFamilyID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TicketTypes]
  SET
    [Name] = @Name,
    [Description] = @Description,
    [Position] = @Position,
    [OrganizationID] = @OrganizationID,
    [IconUrl] = @IconUrl,
    [IsVisibleOnPortal] = @IsVisibleOnPortal,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [ProductFamilyID] = @ProductFamilyID
  WHERE ([TicketTypeID] = @TicketTypeID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTicketTyp' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTicketTyp
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTicketTyp

(
  @TicketTypeID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TicketTypes]
  WHERE ([TicketTypeID] = @TicketTypeID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectTicketTyp' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectTicketTyp
GO

CREATE PROCEDURE dbo.uspGeneratedSelectTicketTyp

(
  @TicketTypeID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [TicketTypeID],
    [Name],
    [Description],
    [Position],
    [OrganizationID],
    [IconUrl],
    [IsVisibleOnPortal],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [ProductFamilyID]
  FROM [dbo].[TicketTypes]
  WHERE ([TicketTypeID] = @TicketTypeID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertTicketTyp' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertTicketTyp
GO

CREATE PROCEDURE dbo.uspGeneratedInsertTicketTyp

(
  @Name varchar(255),
  @Description varchar(1024),
  @Position int,
  @OrganizationID int,
  @IconUrl varchar(255),
  @IsVisibleOnPortal bit,
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @ProductFamilyID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[TicketTypes]
  (
    [Name],
    [Description],
    [Position],
    [OrganizationID],
    [IconUrl],
    [IsVisibleOnPortal],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [ProductFamilyID])
  VALUES (
    @Name,
    @Description,
    @Position,
    @OrganizationID,
    @IconUrl,
    @IsVisibleOnPortal,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @ProductFamilyID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateTicketTyp' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateTicketTyp
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateTicketTyp

(
  @TicketTypeID int,
  @Name varchar(255),
  @Description varchar(1024),
  @Position int,
  @OrganizationID int,
  @IconUrl varchar(255),
  @IsVisibleOnPortal bit,
  @DateModified datetime,
  @ModifierID int,
  @ProductFamilyID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[TicketTypes]
  SET
    [Name] = @Name,
    [Description] = @Description,
    [Position] = @Position,
    [OrganizationID] = @OrganizationID,
    [IconUrl] = @IconUrl,
    [IsVisibleOnPortal] = @IsVisibleOnPortal,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [ProductFamilyID] = @ProductFamilyID
  WHERE ([TicketTypeID] = @TicketTypeID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteTicketTyp' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteTicketTyp
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteTicketTyp

(
  @TicketTypeID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[TicketTypes]
  WHERE ([TicketTypeID] = @TicketTypeID)
GO


