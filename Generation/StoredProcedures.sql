IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectUserProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectUserProduct
GO

CREATE PROCEDURE dbo.uspGeneratedSelectUserProduct

(
  @UserProductID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [UserProductID],
    [UserID],
    [ProductID],
    [ProductVersionID],
    [IsVisibleOnPortal],
    [SupportExpiration],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID]
  FROM [dbo].[UserProducts]
  WHERE ([UserProductID] = @UserProductID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertUserProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertUserProduct
GO

CREATE PROCEDURE dbo.uspGeneratedInsertUserProduct

(
  @UserID int,
  @ProductID int,
  @ProductVersionID int,
  @IsVisibleOnPortal bit,
  @SupportExpiration datetime,
  @ImportID varchar(50),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[UserProducts]
  (
    [UserID],
    [ProductID],
    [ProductVersionID],
    [IsVisibleOnPortal],
    [SupportExpiration],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID])
  VALUES (
    @UserID,
    @ProductID,
    @ProductVersionID,
    @IsVisibleOnPortal,
    @SupportExpiration,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateUserProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateUserProduct
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateUserProduct

(
  @UserProductID int,
  @UserID int,
  @ProductID int,
  @ProductVersionID int,
  @IsVisibleOnPortal bit,
  @SupportExpiration datetime,
  @ImportID varchar(50),
  @DateModified datetime,
  @ModifierID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[UserProducts]
  SET
    [UserID] = @UserID,
    [ProductID] = @ProductID,
    [ProductVersionID] = @ProductVersionID,
    [IsVisibleOnPortal] = @IsVisibleOnPortal,
    [SupportExpiration] = @SupportExpiration,
    [ImportID] = @ImportID,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID
  WHERE ([UserProductID] = @UserProductID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteUserProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteUserProduct
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteUserProduct

(
  @UserProductID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[UserProducts]
  WHERE ([UserProductID] = @UserProductID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectUserProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectUserProduct
GO

CREATE PROCEDURE dbo.uspGeneratedSelectUserProduct

(
  @UserProductID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [UserProductID],
    [UserID],
    [ProductID],
    [ProductVersionID],
    [IsVisibleOnPortal],
    [SupportExpiration],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID]
  FROM [dbo].[UserProducts]
  WHERE ([UserProductID] = @UserProductID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertUserProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertUserProduct
GO

CREATE PROCEDURE dbo.uspGeneratedInsertUserProduct

(
  @UserID int,
  @ProductID int,
  @ProductVersionID int,
  @IsVisibleOnPortal bit,
  @SupportExpiration datetime,
  @ImportID varchar(50),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[UserProducts]
  (
    [UserID],
    [ProductID],
    [ProductVersionID],
    [IsVisibleOnPortal],
    [SupportExpiration],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID])
  VALUES (
    @UserID,
    @ProductID,
    @ProductVersionID,
    @IsVisibleOnPortal,
    @SupportExpiration,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateUserProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateUserProduct
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateUserProduct

(
  @UserProductID int,
  @UserID int,
  @ProductID int,
  @ProductVersionID int,
  @IsVisibleOnPortal bit,
  @SupportExpiration datetime,
  @ImportID varchar(50),
  @DateModified datetime,
  @ModifierID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[UserProducts]
  SET
    [UserID] = @UserID,
    [ProductID] = @ProductID,
    [ProductVersionID] = @ProductVersionID,
    [IsVisibleOnPortal] = @IsVisibleOnPortal,
    [SupportExpiration] = @SupportExpiration,
    [ImportID] = @ImportID,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID
  WHERE ([UserProductID] = @UserProductID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteUserProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteUserProduct
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteUserProduct

(
  @UserProductID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[UserProducts]
  WHERE ([UserProductID] = @UserProductID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectUserProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectUserProduct
GO

CREATE PROCEDURE dbo.uspGeneratedSelectUserProduct

(
  @UserProductID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [UserProductID],
    [UserID],
    [ProductID],
    [ProductVersionID],
    [IsVisibleOnPortal],
    [SupportExpiration],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID]
  FROM [dbo].[UserProducts]
  WHERE ([UserProductID] = @UserProductID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertUserProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertUserProduct
GO

CREATE PROCEDURE dbo.uspGeneratedInsertUserProduct

(
  @UserID int,
  @ProductID int,
  @ProductVersionID int,
  @IsVisibleOnPortal bit,
  @SupportExpiration datetime,
  @ImportID varchar(50),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[UserProducts]
  (
    [UserID],
    [ProductID],
    [ProductVersionID],
    [IsVisibleOnPortal],
    [SupportExpiration],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID])
  VALUES (
    @UserID,
    @ProductID,
    @ProductVersionID,
    @IsVisibleOnPortal,
    @SupportExpiration,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateUserProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateUserProduct
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateUserProduct

(
  @UserProductID int,
  @UserID int,
  @ProductID int,
  @ProductVersionID int,
  @IsVisibleOnPortal bit,
  @SupportExpiration datetime,
  @ImportID varchar(50),
  @DateModified datetime,
  @ModifierID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[UserProducts]
  SET
    [UserID] = @UserID,
    [ProductID] = @ProductID,
    [ProductVersionID] = @ProductVersionID,
    [IsVisibleOnPortal] = @IsVisibleOnPortal,
    [SupportExpiration] = @SupportExpiration,
    [ImportID] = @ImportID,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID
  WHERE ([UserProductID] = @UserProductID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteUserProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteUserProduct
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteUserProduct

(
  @UserProductID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[UserProducts]
  WHERE ([UserProductID] = @UserProductID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectUserProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectUserProduct
GO

CREATE PROCEDURE dbo.uspGeneratedSelectUserProduct

(
  @UserProductID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [UserProductID],
    [UserID],
    [ProductID],
    [ProductVersionID],
    [IsVisibleOnPortal],
    [SupportExpiration],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID]
  FROM [dbo].[UserProducts]
  WHERE ([UserProductID] = @UserProductID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertUserProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertUserProduct
GO

CREATE PROCEDURE dbo.uspGeneratedInsertUserProduct

(
  @UserID int,
  @ProductID int,
  @ProductVersionID int,
  @IsVisibleOnPortal bit,
  @SupportExpiration datetime,
  @ImportID varchar(50),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[UserProducts]
  (
    [UserID],
    [ProductID],
    [ProductVersionID],
    [IsVisibleOnPortal],
    [SupportExpiration],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID])
  VALUES (
    @UserID,
    @ProductID,
    @ProductVersionID,
    @IsVisibleOnPortal,
    @SupportExpiration,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateUserProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateUserProduct
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateUserProduct

(
  @UserProductID int,
  @UserID int,
  @ProductID int,
  @ProductVersionID int,
  @IsVisibleOnPortal bit,
  @SupportExpiration datetime,
  @ImportID varchar(50),
  @DateModified datetime,
  @ModifierID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[UserProducts]
  SET
    [UserID] = @UserID,
    [ProductID] = @ProductID,
    [ProductVersionID] = @ProductVersionID,
    [IsVisibleOnPortal] = @IsVisibleOnPortal,
    [SupportExpiration] = @SupportExpiration,
    [ImportID] = @ImportID,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID
  WHERE ([UserProductID] = @UserProductID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteUserProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteUserProduct
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteUserProduct

(
  @UserProductID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[UserProducts]
  WHERE ([UserProductID] = @UserProductID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectUserProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectUserProduct
GO

CREATE PROCEDURE dbo.uspGeneratedSelectUserProduct

(
  @UserProductID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [UserProductID],
    [UserID],
    [ProductID],
    [ProductVersionID],
    [IsVisibleOnPortal],
    [SupportExpiration],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID]
  FROM [dbo].[UserProducts]
  WHERE ([UserProductID] = @UserProductID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertUserProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertUserProduct
GO

CREATE PROCEDURE dbo.uspGeneratedInsertUserProduct

(
  @UserID int,
  @ProductID int,
  @ProductVersionID int,
  @IsVisibleOnPortal bit,
  @SupportExpiration datetime,
  @ImportID varchar(50),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[UserProducts]
  (
    [UserID],
    [ProductID],
    [ProductVersionID],
    [IsVisibleOnPortal],
    [SupportExpiration],
    [ImportID],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID])
  VALUES (
    @UserID,
    @ProductID,
    @ProductVersionID,
    @IsVisibleOnPortal,
    @SupportExpiration,
    @ImportID,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateUserProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateUserProduct
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateUserProduct

(
  @UserProductID int,
  @UserID int,
  @ProductID int,
  @ProductVersionID int,
  @IsVisibleOnPortal bit,
  @SupportExpiration datetime,
  @ImportID varchar(50),
  @DateModified datetime,
  @ModifierID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[UserProducts]
  SET
    [UserID] = @UserID,
    [ProductID] = @ProductID,
    [ProductVersionID] = @ProductVersionID,
    [IsVisibleOnPortal] = @IsVisibleOnPortal,
    [SupportExpiration] = @SupportExpiration,
    [ImportID] = @ImportID,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID
  WHERE ([UserProductID] = @UserProductID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteUserProduct' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteUserProduct
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteUserProduct

(
  @UserProductID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[UserProducts]
  WHERE ([UserProductID] = @UserProductID)
GO


