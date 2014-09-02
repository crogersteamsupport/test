IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCRMLinkError' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCRMLinkError
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCRMLinkError

(
  @CRMLinkErrorID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [CRMLinkErrorID],
    [OrganizationID],
    [CRMType],
    [Orientation],
    [ObjectType],
    [ObjectID],
    [ObjectFieldName],
    [ObjectData],
    [Exception],
    [OperationType],
    [DateCreated],
    [DateModified]
  FROM [dbo].[CRMLinkErrors]
  WHERE ([CRMLinkErrorID] = @CRMLinkErrorID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCRMLinkError' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCRMLinkError
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCRMLinkError

(
  @OrganizationID int,
  @CRMType varchar(100),
  @Orientation varchar(4),
  @ObjectType varchar(100),
  @ObjectID varchar(MAX),
  @ObjectFieldName varchar(MAX),
  @ObjectData varchar(MAX),
  @Exception varchar(MAX),
  @OperationType varchar(10),
  @DateCreated datetime,
  @DateModified datetime,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CRMLinkErrors]
  (
    [OrganizationID],
    [CRMType],
    [Orientation],
    [ObjectType],
    [ObjectID],
    [ObjectFieldName],
    [ObjectData],
    [Exception],
    [OperationType],
    [DateCreated],
    [DateModified])
  VALUES (
    @OrganizationID,
    @CRMType,
    @Orientation,
    @ObjectType,
    @ObjectID,
    @ObjectFieldName,
    @ObjectData,
    @Exception,
    @OperationType,
    @DateCreated,
    @DateModified)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCRMLinkError' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCRMLinkError
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCRMLinkError

(
  @CRMLinkErrorID int,
  @OrganizationID int,
  @CRMType varchar(100),
  @Orientation varchar(4),
  @ObjectType varchar(100),
  @ObjectID varchar(MAX),
  @ObjectFieldName varchar(MAX),
  @ObjectData varchar(MAX),
  @Exception varchar(MAX),
  @OperationType varchar(10),
  @DateModified datetime
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CRMLinkErrors]
  SET
    [OrganizationID] = @OrganizationID,
    [CRMType] = @CRMType,
    [Orientation] = @Orientation,
    [ObjectType] = @ObjectType,
    [ObjectID] = @ObjectID,
    [ObjectFieldName] = @ObjectFieldName,
    [ObjectData] = @ObjectData,
    [Exception] = @Exception,
    [OperationType] = @OperationType,
    [DateModified] = @DateModified
  WHERE ([CRMLinkErrorID] = @CRMLinkErrorID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCRMLinkError' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCRMLinkError
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCRMLinkError

(
  @CRMLinkErrorID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CRMLinkErrors]
  WHERE ([CRMLinkErrorID] = @CRMLinkErrorID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCRMLinkError' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCRMLinkError
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCRMLinkError

(
  @CRMLinkErrorID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [CRMLinkErrorID],
    [OrganizationID],
    [CRMType],
    [Orientation],
    [ObjectType],
    [ObjectID],
    [ObjectFieldName],
    [ObjectData],
    [Exception],
    [OperationType],
    [DateCreated],
    [DateModified]
  FROM [dbo].[CRMLinkErrors]
  WHERE ([CRMLinkErrorID] = @CRMLinkErrorID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCRMLinkError' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCRMLinkError
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCRMLinkError

(
  @OrganizationID int,
  @CRMType varchar(100),
  @Orientation varchar(4),
  @ObjectType varchar(100),
  @ObjectID varchar(MAX),
  @ObjectFieldName varchar(MAX),
  @ObjectData varchar(MAX),
  @Exception varchar(MAX),
  @OperationType varchar(10),
  @DateCreated datetime,
  @DateModified datetime,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CRMLinkErrors]
  (
    [OrganizationID],
    [CRMType],
    [Orientation],
    [ObjectType],
    [ObjectID],
    [ObjectFieldName],
    [ObjectData],
    [Exception],
    [OperationType],
    [DateCreated],
    [DateModified])
  VALUES (
    @OrganizationID,
    @CRMType,
    @Orientation,
    @ObjectType,
    @ObjectID,
    @ObjectFieldName,
    @ObjectData,
    @Exception,
    @OperationType,
    @DateCreated,
    @DateModified)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCRMLinkError' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCRMLinkError
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCRMLinkError

(
  @CRMLinkErrorID int,
  @OrganizationID int,
  @CRMType varchar(100),
  @Orientation varchar(4),
  @ObjectType varchar(100),
  @ObjectID varchar(MAX),
  @ObjectFieldName varchar(MAX),
  @ObjectData varchar(MAX),
  @Exception varchar(MAX),
  @OperationType varchar(10),
  @DateModified datetime
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CRMLinkErrors]
  SET
    [OrganizationID] = @OrganizationID,
    [CRMType] = @CRMType,
    [Orientation] = @Orientation,
    [ObjectType] = @ObjectType,
    [ObjectID] = @ObjectID,
    [ObjectFieldName] = @ObjectFieldName,
    [ObjectData] = @ObjectData,
    [Exception] = @Exception,
    [OperationType] = @OperationType,
    [DateModified] = @DateModified
  WHERE ([CRMLinkErrorID] = @CRMLinkErrorID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCRMLinkError' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCRMLinkError
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCRMLinkError

(
  @CRMLinkErrorID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CRMLinkErrors]
  WHERE ([CRMLinkErrorID] = @CRMLinkErrorID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCRMLinkError' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCRMLinkError
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCRMLinkError

(
  @CRMLinkErrorID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [CRMLinkErrorID],
    [OrganizationID],
    [CRMType],
    [Orientation],
    [ObjectType],
    [ObjectID],
    [ObjectFieldName],
    [ObjectData],
    [Exception],
    [OperationType],
    [DateCreated],
    [DateModified]
  FROM [dbo].[CRMLinkErrors]
  WHERE ([CRMLinkErrorID] = @CRMLinkErrorID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCRMLinkError' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCRMLinkError
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCRMLinkError

(
  @OrganizationID int,
  @CRMType varchar(100),
  @Orientation varchar(4),
  @ObjectType varchar(100),
  @ObjectID varchar(MAX),
  @ObjectFieldName varchar(MAX),
  @ObjectData varchar(MAX),
  @Exception varchar(MAX),
  @OperationType varchar(10),
  @DateCreated datetime,
  @DateModified datetime,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CRMLinkErrors]
  (
    [OrganizationID],
    [CRMType],
    [Orientation],
    [ObjectType],
    [ObjectID],
    [ObjectFieldName],
    [ObjectData],
    [Exception],
    [OperationType],
    [DateCreated],
    [DateModified])
  VALUES (
    @OrganizationID,
    @CRMType,
    @Orientation,
    @ObjectType,
    @ObjectID,
    @ObjectFieldName,
    @ObjectData,
    @Exception,
    @OperationType,
    @DateCreated,
    @DateModified)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCRMLinkError' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCRMLinkError
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCRMLinkError

(
  @CRMLinkErrorID int,
  @OrganizationID int,
  @CRMType varchar(100),
  @Orientation varchar(4),
  @ObjectType varchar(100),
  @ObjectID varchar(MAX),
  @ObjectFieldName varchar(MAX),
  @ObjectData varchar(MAX),
  @Exception varchar(MAX),
  @OperationType varchar(10),
  @DateModified datetime
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CRMLinkErrors]
  SET
    [OrganizationID] = @OrganizationID,
    [CRMType] = @CRMType,
    [Orientation] = @Orientation,
    [ObjectType] = @ObjectType,
    [ObjectID] = @ObjectID,
    [ObjectFieldName] = @ObjectFieldName,
    [ObjectData] = @ObjectData,
    [Exception] = @Exception,
    [OperationType] = @OperationType,
    [DateModified] = @DateModified
  WHERE ([CRMLinkErrorID] = @CRMLinkErrorID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCRMLinkError' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCRMLinkError
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCRMLinkError

(
  @CRMLinkErrorID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CRMLinkErrors]
  WHERE ([CRMLinkErrorID] = @CRMLinkErrorID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCRMLinkError' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCRMLinkError
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCRMLinkError

(
  @CRMLinkErrorID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [CRMLinkErrorID],
    [OrganizationID],
    [CRMType],
    [Orientation],
    [ObjectType],
    [ObjectID],
    [ObjectFieldName],
    [ObjectData],
    [Exception],
    [OperationType],
    [DateCreated],
    [DateModified]
  FROM [dbo].[CRMLinkErrors]
  WHERE ([CRMLinkErrorID] = @CRMLinkErrorID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCRMLinkError' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCRMLinkError
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCRMLinkError

(
  @OrganizationID int,
  @CRMType varchar(100),
  @Orientation varchar(4),
  @ObjectType varchar(100),
  @ObjectID varchar(MAX),
  @ObjectFieldName varchar(MAX),
  @ObjectData varchar(MAX),
  @Exception varchar(MAX),
  @OperationType varchar(10),
  @DateCreated datetime,
  @DateModified datetime,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CRMLinkErrors]
  (
    [OrganizationID],
    [CRMType],
    [Orientation],
    [ObjectType],
    [ObjectID],
    [ObjectFieldName],
    [ObjectData],
    [Exception],
    [OperationType],
    [DateCreated],
    [DateModified])
  VALUES (
    @OrganizationID,
    @CRMType,
    @Orientation,
    @ObjectType,
    @ObjectID,
    @ObjectFieldName,
    @ObjectData,
    @Exception,
    @OperationType,
    @DateCreated,
    @DateModified)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCRMLinkError' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCRMLinkError
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCRMLinkError

(
  @CRMLinkErrorID int,
  @OrganizationID int,
  @CRMType varchar(100),
  @Orientation varchar(4),
  @ObjectType varchar(100),
  @ObjectID varchar(MAX),
  @ObjectFieldName varchar(MAX),
  @ObjectData varchar(MAX),
  @Exception varchar(MAX),
  @OperationType varchar(10),
  @DateModified datetime
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CRMLinkErrors]
  SET
    [OrganizationID] = @OrganizationID,
    [CRMType] = @CRMType,
    [Orientation] = @Orientation,
    [ObjectType] = @ObjectType,
    [ObjectID] = @ObjectID,
    [ObjectFieldName] = @ObjectFieldName,
    [ObjectData] = @ObjectData,
    [Exception] = @Exception,
    [OperationType] = @OperationType,
    [DateModified] = @DateModified
  WHERE ([CRMLinkErrorID] = @CRMLinkErrorID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCRMLinkError' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCRMLinkError
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCRMLinkError

(
  @CRMLinkErrorID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CRMLinkErrors]
  WHERE ([CRMLinkErrorID] = @CRMLinkErrorID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCRMLinkError' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCRMLinkError
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCRMLinkError

(
  @CRMLinkErrorID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [CRMLinkErrorID],
    [OrganizationID],
    [CRMType],
    [Orientation],
    [ObjectType],
    [ObjectID],
    [ObjectFieldName],
    [ObjectData],
    [Exception],
    [OperationType],
    [DateCreated],
    [DateModified]
  FROM [dbo].[CRMLinkErrors]
  WHERE ([CRMLinkErrorID] = @CRMLinkErrorID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCRMLinkError' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCRMLinkError
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCRMLinkError

(
  @OrganizationID int,
  @CRMType varchar(100),
  @Orientation varchar(4),
  @ObjectType varchar(100),
  @ObjectID varchar(MAX),
  @ObjectFieldName varchar(MAX),
  @ObjectData varchar(MAX),
  @Exception varchar(MAX),
  @OperationType varchar(10),
  @DateCreated datetime,
  @DateModified datetime,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CRMLinkErrors]
  (
    [OrganizationID],
    [CRMType],
    [Orientation],
    [ObjectType],
    [ObjectID],
    [ObjectFieldName],
    [ObjectData],
    [Exception],
    [OperationType],
    [DateCreated],
    [DateModified])
  VALUES (
    @OrganizationID,
    @CRMType,
    @Orientation,
    @ObjectType,
    @ObjectID,
    @ObjectFieldName,
    @ObjectData,
    @Exception,
    @OperationType,
    @DateCreated,
    @DateModified)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCRMLinkError' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCRMLinkError
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCRMLinkError

(
  @CRMLinkErrorID int,
  @OrganizationID int,
  @CRMType varchar(100),
  @Orientation varchar(4),
  @ObjectType varchar(100),
  @ObjectID varchar(MAX),
  @ObjectFieldName varchar(MAX),
  @ObjectData varchar(MAX),
  @Exception varchar(MAX),
  @OperationType varchar(10),
  @DateModified datetime
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CRMLinkErrors]
  SET
    [OrganizationID] = @OrganizationID,
    [CRMType] = @CRMType,
    [Orientation] = @Orientation,
    [ObjectType] = @ObjectType,
    [ObjectID] = @ObjectID,
    [ObjectFieldName] = @ObjectFieldName,
    [ObjectData] = @ObjectData,
    [Exception] = @Exception,
    [OperationType] = @OperationType,
    [DateModified] = @DateModified
  WHERE ([CRMLinkErrorID] = @CRMLinkErrorID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCRMLinkError' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCRMLinkError
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCRMLinkError

(
  @CRMLinkErrorID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CRMLinkErrors]
  WHERE ([CRMLinkErrorID] = @CRMLinkErrorID)
GO


