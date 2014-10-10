IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectOrganizationProductsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectOrganizationProductsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectOrganizationProductsViewItem

(
  @OrganizationProductID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [Product],
    [VersionStatus],
    [IsShipping],
    [IsDiscontinued],
    [VersionNumber],
    [ProductVersionStatusID],
    [ReleaseDate],
    [IsReleased],
    [Description],
    [OrganizationProductID],
    [OrganizationID],
    [OrganizationName],
    [ProductID],
    [ProductVersionID],
    [IsVisibleOnPortal],
    [SupportExpiration],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID]
  FROM [dbo].[OrganizationProductsView]
  WHERE ([OrganizationProductID] = @OrganizationProductID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertOrganizationProductsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertOrganizationProductsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertOrganizationProductsViewItem

(
  @Product varchar(255),
  @VersionStatus varchar(255),
  @IsShipping bit,
  @IsDiscontinued bit,
  @VersionNumber varchar(50),
  @ProductVersionStatusID int,
  @ReleaseDate datetime,
  @IsReleased bit,
  @Description varchar(MAX),
  @OrganizationProductID int,
  @OrganizationID int,
  @OrganizationName varchar(255),
  @ProductID int,
  @ProductVersionID int,
  @IsVisibleOnPortal bit,
  @SupportExpiration datetime,
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[OrganizationProductsView]
  (
    [Product],
    [VersionStatus],
    [IsShipping],
    [IsDiscontinued],
    [VersionNumber],
    [ProductVersionStatusID],
    [ReleaseDate],
    [IsReleased],
    [Description],
    [OrganizationProductID],
    [OrganizationID],
    [OrganizationName],
    [ProductID],
    [ProductVersionID],
    [IsVisibleOnPortal],
    [SupportExpiration],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID])
  VALUES (
    @Product,
    @VersionStatus,
    @IsShipping,
    @IsDiscontinued,
    @VersionNumber,
    @ProductVersionStatusID,
    @ReleaseDate,
    @IsReleased,
    @Description,
    @OrganizationProductID,
    @OrganizationID,
    @OrganizationName,
    @ProductID,
    @ProductVersionID,
    @IsVisibleOnPortal,
    @SupportExpiration,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateOrganizationProductsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateOrganizationProductsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateOrganizationProductsViewItem

(
  @Product varchar(255),
  @VersionStatus varchar(255),
  @IsShipping bit,
  @IsDiscontinued bit,
  @VersionNumber varchar(50),
  @ProductVersionStatusID int,
  @ReleaseDate datetime,
  @IsReleased bit,
  @Description varchar(MAX),
  @OrganizationProductID int,
  @OrganizationID int,
  @OrganizationName varchar(255),
  @ProductID int,
  @ProductVersionID int,
  @IsVisibleOnPortal bit,
  @SupportExpiration datetime,
  @DateModified datetime,
  @ModifierID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[OrganizationProductsView]
  SET
    [Product] = @Product,
    [VersionStatus] = @VersionStatus,
    [IsShipping] = @IsShipping,
    [IsDiscontinued] = @IsDiscontinued,
    [VersionNumber] = @VersionNumber,
    [ProductVersionStatusID] = @ProductVersionStatusID,
    [ReleaseDate] = @ReleaseDate,
    [IsReleased] = @IsReleased,
    [Description] = @Description,
    [OrganizationID] = @OrganizationID,
    [OrganizationName] = @OrganizationName,
    [ProductID] = @ProductID,
    [ProductVersionID] = @ProductVersionID,
    [IsVisibleOnPortal] = @IsVisibleOnPortal,
    [SupportExpiration] = @SupportExpiration,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID
  WHERE ([OrganizationProductID] = @OrganizationProductID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteOrganizationProductsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteOrganizationProductsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteOrganizationProductsViewItem

(
  @OrganizationProductID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[OrganizationProductsView]
  WHERE ([OrganizationProductID] = @OrganizationProductID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectOrganizationProductsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectOrganizationProductsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectOrganizationProductsViewItem

(
  @OrganizationProductID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [Product],
    [VersionStatus],
    [IsShipping],
    [IsDiscontinued],
    [VersionNumber],
    [ProductVersionStatusID],
    [ReleaseDate],
    [IsReleased],
    [Description],
    [OrganizationProductID],
    [OrganizationID],
    [OrganizationName],
    [ProductID],
    [ProductVersionID],
    [IsVisibleOnPortal],
    [SupportExpiration],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID]
  FROM [dbo].[OrganizationProductsView]
  WHERE ([OrganizationProductID] = @OrganizationProductID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertOrganizationProductsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertOrganizationProductsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertOrganizationProductsViewItem

(
  @Product varchar(255),
  @VersionStatus varchar(255),
  @IsShipping bit,
  @IsDiscontinued bit,
  @VersionNumber varchar(50),
  @ProductVersionStatusID int,
  @ReleaseDate datetime,
  @IsReleased bit,
  @Description varchar(MAX),
  @OrganizationProductID int,
  @OrganizationID int,
  @OrganizationName varchar(255),
  @ProductID int,
  @ProductVersionID int,
  @IsVisibleOnPortal bit,
  @SupportExpiration datetime,
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[OrganizationProductsView]
  (
    [Product],
    [VersionStatus],
    [IsShipping],
    [IsDiscontinued],
    [VersionNumber],
    [ProductVersionStatusID],
    [ReleaseDate],
    [IsReleased],
    [Description],
    [OrganizationProductID],
    [OrganizationID],
    [OrganizationName],
    [ProductID],
    [ProductVersionID],
    [IsVisibleOnPortal],
    [SupportExpiration],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID])
  VALUES (
    @Product,
    @VersionStatus,
    @IsShipping,
    @IsDiscontinued,
    @VersionNumber,
    @ProductVersionStatusID,
    @ReleaseDate,
    @IsReleased,
    @Description,
    @OrganizationProductID,
    @OrganizationID,
    @OrganizationName,
    @ProductID,
    @ProductVersionID,
    @IsVisibleOnPortal,
    @SupportExpiration,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateOrganizationProductsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateOrganizationProductsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateOrganizationProductsViewItem

(
  @Product varchar(255),
  @VersionStatus varchar(255),
  @IsShipping bit,
  @IsDiscontinued bit,
  @VersionNumber varchar(50),
  @ProductVersionStatusID int,
  @ReleaseDate datetime,
  @IsReleased bit,
  @Description varchar(MAX),
  @OrganizationProductID int,
  @OrganizationID int,
  @OrganizationName varchar(255),
  @ProductID int,
  @ProductVersionID int,
  @IsVisibleOnPortal bit,
  @SupportExpiration datetime,
  @DateModified datetime,
  @ModifierID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[OrganizationProductsView]
  SET
    [Product] = @Product,
    [VersionStatus] = @VersionStatus,
    [IsShipping] = @IsShipping,
    [IsDiscontinued] = @IsDiscontinued,
    [VersionNumber] = @VersionNumber,
    [ProductVersionStatusID] = @ProductVersionStatusID,
    [ReleaseDate] = @ReleaseDate,
    [IsReleased] = @IsReleased,
    [Description] = @Description,
    [OrganizationID] = @OrganizationID,
    [OrganizationName] = @OrganizationName,
    [ProductID] = @ProductID,
    [ProductVersionID] = @ProductVersionID,
    [IsVisibleOnPortal] = @IsVisibleOnPortal,
    [SupportExpiration] = @SupportExpiration,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID
  WHERE ([OrganizationProductID] = @OrganizationProductID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteOrganizationProductsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteOrganizationProductsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteOrganizationProductsViewItem

(
  @OrganizationProductID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[OrganizationProductsView]
  WHERE ([OrganizationProductID] = @OrganizationProductID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectOrganizationProductsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectOrganizationProductsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectOrganizationProductsViewItem

(
  @OrganizationProductID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [Product],
    [VersionStatus],
    [IsShipping],
    [IsDiscontinued],
    [VersionNumber],
    [ProductVersionStatusID],
    [ReleaseDate],
    [IsReleased],
    [Description],
    [OrganizationProductID],
    [OrganizationID],
    [OrganizationName],
    [ProductID],
    [ProductVersionID],
    [IsVisibleOnPortal],
    [SupportExpiration],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID]
  FROM [dbo].[OrganizationProductsView]
  WHERE ([OrganizationProductID] = @OrganizationProductID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertOrganizationProductsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertOrganizationProductsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertOrganizationProductsViewItem

(
  @Product varchar(255),
  @VersionStatus varchar(255),
  @IsShipping bit,
  @IsDiscontinued bit,
  @VersionNumber varchar(50),
  @ProductVersionStatusID int,
  @ReleaseDate datetime,
  @IsReleased bit,
  @Description varchar(MAX),
  @OrganizationProductID int,
  @OrganizationID int,
  @OrganizationName varchar(255),
  @ProductID int,
  @ProductVersionID int,
  @IsVisibleOnPortal bit,
  @SupportExpiration datetime,
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[OrganizationProductsView]
  (
    [Product],
    [VersionStatus],
    [IsShipping],
    [IsDiscontinued],
    [VersionNumber],
    [ProductVersionStatusID],
    [ReleaseDate],
    [IsReleased],
    [Description],
    [OrganizationProductID],
    [OrganizationID],
    [OrganizationName],
    [ProductID],
    [ProductVersionID],
    [IsVisibleOnPortal],
    [SupportExpiration],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID])
  VALUES (
    @Product,
    @VersionStatus,
    @IsShipping,
    @IsDiscontinued,
    @VersionNumber,
    @ProductVersionStatusID,
    @ReleaseDate,
    @IsReleased,
    @Description,
    @OrganizationProductID,
    @OrganizationID,
    @OrganizationName,
    @ProductID,
    @ProductVersionID,
    @IsVisibleOnPortal,
    @SupportExpiration,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateOrganizationProductsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateOrganizationProductsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateOrganizationProductsViewItem

(
  @Product varchar(255),
  @VersionStatus varchar(255),
  @IsShipping bit,
  @IsDiscontinued bit,
  @VersionNumber varchar(50),
  @ProductVersionStatusID int,
  @ReleaseDate datetime,
  @IsReleased bit,
  @Description varchar(MAX),
  @OrganizationProductID int,
  @OrganizationID int,
  @OrganizationName varchar(255),
  @ProductID int,
  @ProductVersionID int,
  @IsVisibleOnPortal bit,
  @SupportExpiration datetime,
  @DateModified datetime,
  @ModifierID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[OrganizationProductsView]
  SET
    [Product] = @Product,
    [VersionStatus] = @VersionStatus,
    [IsShipping] = @IsShipping,
    [IsDiscontinued] = @IsDiscontinued,
    [VersionNumber] = @VersionNumber,
    [ProductVersionStatusID] = @ProductVersionStatusID,
    [ReleaseDate] = @ReleaseDate,
    [IsReleased] = @IsReleased,
    [Description] = @Description,
    [OrganizationID] = @OrganizationID,
    [OrganizationName] = @OrganizationName,
    [ProductID] = @ProductID,
    [ProductVersionID] = @ProductVersionID,
    [IsVisibleOnPortal] = @IsVisibleOnPortal,
    [SupportExpiration] = @SupportExpiration,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID
  WHERE ([OrganizationProductID] = @OrganizationProductID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteOrganizationProductsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteOrganizationProductsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteOrganizationProductsViewItem

(
  @OrganizationProductID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[OrganizationProductsView]
  WHERE ([OrganizationProductID] = @OrganizationProductID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectOrganizationProductsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectOrganizationProductsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectOrganizationProductsViewItem

(
  @OrganizationProductID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [Product],
    [VersionStatus],
    [IsShipping],
    [IsDiscontinued],
    [VersionNumber],
    [ProductVersionStatusID],
    [ReleaseDate],
    [IsReleased],
    [Description],
    [OrganizationProductID],
    [OrganizationID],
    [OrganizationName],
    [ProductID],
    [ProductVersionID],
    [IsVisibleOnPortal],
    [SupportExpiration],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID]
  FROM [dbo].[OrganizationProductsView]
  WHERE ([OrganizationProductID] = @OrganizationProductID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertOrganizationProductsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertOrganizationProductsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertOrganizationProductsViewItem

(
  @Product varchar(255),
  @VersionStatus varchar(255),
  @IsShipping bit,
  @IsDiscontinued bit,
  @VersionNumber varchar(50),
  @ProductVersionStatusID int,
  @ReleaseDate datetime,
  @IsReleased bit,
  @Description varchar(MAX),
  @OrganizationProductID int,
  @OrganizationID int,
  @OrganizationName varchar(255),
  @ProductID int,
  @ProductVersionID int,
  @IsVisibleOnPortal bit,
  @SupportExpiration datetime,
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[OrganizationProductsView]
  (
    [Product],
    [VersionStatus],
    [IsShipping],
    [IsDiscontinued],
    [VersionNumber],
    [ProductVersionStatusID],
    [ReleaseDate],
    [IsReleased],
    [Description],
    [OrganizationProductID],
    [OrganizationID],
    [OrganizationName],
    [ProductID],
    [ProductVersionID],
    [IsVisibleOnPortal],
    [SupportExpiration],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID])
  VALUES (
    @Product,
    @VersionStatus,
    @IsShipping,
    @IsDiscontinued,
    @VersionNumber,
    @ProductVersionStatusID,
    @ReleaseDate,
    @IsReleased,
    @Description,
    @OrganizationProductID,
    @OrganizationID,
    @OrganizationName,
    @ProductID,
    @ProductVersionID,
    @IsVisibleOnPortal,
    @SupportExpiration,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateOrganizationProductsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateOrganizationProductsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateOrganizationProductsViewItem

(
  @Product varchar(255),
  @VersionStatus varchar(255),
  @IsShipping bit,
  @IsDiscontinued bit,
  @VersionNumber varchar(50),
  @ProductVersionStatusID int,
  @ReleaseDate datetime,
  @IsReleased bit,
  @Description varchar(MAX),
  @OrganizationProductID int,
  @OrganizationID int,
  @OrganizationName varchar(255),
  @ProductID int,
  @ProductVersionID int,
  @IsVisibleOnPortal bit,
  @SupportExpiration datetime,
  @DateModified datetime,
  @ModifierID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[OrganizationProductsView]
  SET
    [Product] = @Product,
    [VersionStatus] = @VersionStatus,
    [IsShipping] = @IsShipping,
    [IsDiscontinued] = @IsDiscontinued,
    [VersionNumber] = @VersionNumber,
    [ProductVersionStatusID] = @ProductVersionStatusID,
    [ReleaseDate] = @ReleaseDate,
    [IsReleased] = @IsReleased,
    [Description] = @Description,
    [OrganizationID] = @OrganizationID,
    [OrganizationName] = @OrganizationName,
    [ProductID] = @ProductID,
    [ProductVersionID] = @ProductVersionID,
    [IsVisibleOnPortal] = @IsVisibleOnPortal,
    [SupportExpiration] = @SupportExpiration,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID
  WHERE ([OrganizationProductID] = @OrganizationProductID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteOrganizationProductsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteOrganizationProductsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteOrganizationProductsViewItem

(
  @OrganizationProductID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[OrganizationProductsView]
  WHERE ([OrganizationProductID] = @OrganizationProductID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectOrganizationProductsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectOrganizationProductsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedSelectOrganizationProductsViewItem

(
  @OrganizationProductID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [Product],
    [VersionStatus],
    [IsShipping],
    [IsDiscontinued],
    [VersionNumber],
    [ProductVersionStatusID],
    [ReleaseDate],
    [IsReleased],
    [Description],
    [OrganizationProductID],
    [OrganizationID],
    [OrganizationName],
    [ProductID],
    [ProductVersionID],
    [IsVisibleOnPortal],
    [SupportExpiration],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID]
  FROM [dbo].[OrganizationProductsView]
  WHERE ([OrganizationProductID] = @OrganizationProductID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertOrganizationProductsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertOrganizationProductsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedInsertOrganizationProductsViewItem

(
  @Product varchar(255),
  @VersionStatus varchar(255),
  @IsShipping bit,
  @IsDiscontinued bit,
  @VersionNumber varchar(50),
  @ProductVersionStatusID int,
  @ReleaseDate datetime,
  @IsReleased bit,
  @Description varchar(MAX),
  @OrganizationProductID int,
  @OrganizationID int,
  @OrganizationName varchar(255),
  @ProductID int,
  @ProductVersionID int,
  @IsVisibleOnPortal bit,
  @SupportExpiration datetime,
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[OrganizationProductsView]
  (
    [Product],
    [VersionStatus],
    [IsShipping],
    [IsDiscontinued],
    [VersionNumber],
    [ProductVersionStatusID],
    [ReleaseDate],
    [IsReleased],
    [Description],
    [OrganizationProductID],
    [OrganizationID],
    [OrganizationName],
    [ProductID],
    [ProductVersionID],
    [IsVisibleOnPortal],
    [SupportExpiration],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID])
  VALUES (
    @Product,
    @VersionStatus,
    @IsShipping,
    @IsDiscontinued,
    @VersionNumber,
    @ProductVersionStatusID,
    @ReleaseDate,
    @IsReleased,
    @Description,
    @OrganizationProductID,
    @OrganizationID,
    @OrganizationName,
    @ProductID,
    @ProductVersionID,
    @IsVisibleOnPortal,
    @SupportExpiration,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateOrganizationProductsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateOrganizationProductsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateOrganizationProductsViewItem

(
  @Product varchar(255),
  @VersionStatus varchar(255),
  @IsShipping bit,
  @IsDiscontinued bit,
  @VersionNumber varchar(50),
  @ProductVersionStatusID int,
  @ReleaseDate datetime,
  @IsReleased bit,
  @Description varchar(MAX),
  @OrganizationProductID int,
  @OrganizationID int,
  @OrganizationName varchar(255),
  @ProductID int,
  @ProductVersionID int,
  @IsVisibleOnPortal bit,
  @SupportExpiration datetime,
  @DateModified datetime,
  @ModifierID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[OrganizationProductsView]
  SET
    [Product] = @Product,
    [VersionStatus] = @VersionStatus,
    [IsShipping] = @IsShipping,
    [IsDiscontinued] = @IsDiscontinued,
    [VersionNumber] = @VersionNumber,
    [ProductVersionStatusID] = @ProductVersionStatusID,
    [ReleaseDate] = @ReleaseDate,
    [IsReleased] = @IsReleased,
    [Description] = @Description,
    [OrganizationID] = @OrganizationID,
    [OrganizationName] = @OrganizationName,
    [ProductID] = @ProductID,
    [ProductVersionID] = @ProductVersionID,
    [IsVisibleOnPortal] = @IsVisibleOnPortal,
    [SupportExpiration] = @SupportExpiration,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID
  WHERE ([OrganizationProductID] = @OrganizationProductID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteOrganizationProductsViewItem' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteOrganizationProductsViewItem
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteOrganizationProductsViewItem

(
  @OrganizationProductID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[OrganizationProductsView]
  WHERE ([OrganizationProductID] = @OrganizationProductID)
GO


