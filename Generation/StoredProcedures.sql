IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectImport' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectImport
GO

CREATE PROCEDURE dbo.uspGeneratedSelectImport

(
  @ImportID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ImportID],
    [FileName],
    [OrganizationID],
    [ImportGUID],
    [RefType],
    [AuxID],
    [IsDone],
    [IsRunning],
    [IsDeleted],
    [NeedsDeleted],
    [TotalRows],
    [CompletedRows],
    [DateStarted],
    [DateEnded],
    [DateCreated],
    [CreatorID],
    [IsRolledBack]
  FROM [dbo].[Imports]
  WHERE ([ImportID] = @ImportID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertImport' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertImport
GO

CREATE PROCEDURE dbo.uspGeneratedInsertImport

(
  @FileName varchar(255),
  @OrganizationID int,
  @ImportGUID uniqueidentifier,
  @RefType int,
  @AuxID int,
  @IsDone bit,
  @IsRunning bit,
  @IsDeleted bit,
  @NeedsDeleted bit,
  @TotalRows int,
  @CompletedRows int,
  @DateStarted datetime,
  @DateEnded datetime,
  @DateCreated datetime,
  @CreatorID int,
  @IsRolledBack bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Imports]
  (
    [FileName],
    [OrganizationID],
    [ImportGUID],
    [RefType],
    [AuxID],
    [IsDone],
    [IsRunning],
    [IsDeleted],
    [NeedsDeleted],
    [TotalRows],
    [CompletedRows],
    [DateStarted],
    [DateEnded],
    [DateCreated],
    [CreatorID],
    [IsRolledBack])
  VALUES (
    @FileName,
    @OrganizationID,
    @ImportGUID,
    @RefType,
    @AuxID,
    @IsDone,
    @IsRunning,
    @IsDeleted,
    @NeedsDeleted,
    @TotalRows,
    @CompletedRows,
    @DateStarted,
    @DateEnded,
    @DateCreated,
    @CreatorID,
    @IsRolledBack)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateImport' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateImport
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateImport

(
  @ImportID int,
  @FileName varchar(255),
  @OrganizationID int,
  @ImportGUID uniqueidentifier,
  @RefType int,
  @AuxID int,
  @IsDone bit,
  @IsRunning bit,
  @IsDeleted bit,
  @NeedsDeleted bit,
  @TotalRows int,
  @CompletedRows int,
  @DateStarted datetime,
  @DateEnded datetime,
  @IsRolledBack bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Imports]
  SET
    [FileName] = @FileName,
    [OrganizationID] = @OrganizationID,
    [ImportGUID] = @ImportGUID,
    [RefType] = @RefType,
    [AuxID] = @AuxID,
    [IsDone] = @IsDone,
    [IsRunning] = @IsRunning,
    [IsDeleted] = @IsDeleted,
    [NeedsDeleted] = @NeedsDeleted,
    [TotalRows] = @TotalRows,
    [CompletedRows] = @CompletedRows,
    [DateStarted] = @DateStarted,
    [DateEnded] = @DateEnded,
    [IsRolledBack] = @IsRolledBack
  WHERE ([ImportID] = @ImportID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteImport' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteImport
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteImport

(
  @ImportID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Imports]
  WHERE ([ImportID] = @ImportID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectImport' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectImport
GO

CREATE PROCEDURE dbo.uspGeneratedSelectImport

(
  @ImportID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ImportID],
    [FileName],
    [OrganizationID],
    [ImportGUID],
    [RefType],
    [AuxID],
    [IsDone],
    [IsRunning],
    [IsDeleted],
    [NeedsDeleted],
    [TotalRows],
    [CompletedRows],
    [DateStarted],
    [DateEnded],
    [DateCreated],
    [CreatorID],
    [IsRolledBack]
  FROM [dbo].[Imports]
  WHERE ([ImportID] = @ImportID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertImport' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertImport
GO

CREATE PROCEDURE dbo.uspGeneratedInsertImport

(
  @FileName varchar(255),
  @OrganizationID int,
  @ImportGUID uniqueidentifier,
  @RefType int,
  @AuxID int,
  @IsDone bit,
  @IsRunning bit,
  @IsDeleted bit,
  @NeedsDeleted bit,
  @TotalRows int,
  @CompletedRows int,
  @DateStarted datetime,
  @DateEnded datetime,
  @DateCreated datetime,
  @CreatorID int,
  @IsRolledBack bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Imports]
  (
    [FileName],
    [OrganizationID],
    [ImportGUID],
    [RefType],
    [AuxID],
    [IsDone],
    [IsRunning],
    [IsDeleted],
    [NeedsDeleted],
    [TotalRows],
    [CompletedRows],
    [DateStarted],
    [DateEnded],
    [DateCreated],
    [CreatorID],
    [IsRolledBack])
  VALUES (
    @FileName,
    @OrganizationID,
    @ImportGUID,
    @RefType,
    @AuxID,
    @IsDone,
    @IsRunning,
    @IsDeleted,
    @NeedsDeleted,
    @TotalRows,
    @CompletedRows,
    @DateStarted,
    @DateEnded,
    @DateCreated,
    @CreatorID,
    @IsRolledBack)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateImport' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateImport
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateImport

(
  @ImportID int,
  @FileName varchar(255),
  @OrganizationID int,
  @ImportGUID uniqueidentifier,
  @RefType int,
  @AuxID int,
  @IsDone bit,
  @IsRunning bit,
  @IsDeleted bit,
  @NeedsDeleted bit,
  @TotalRows int,
  @CompletedRows int,
  @DateStarted datetime,
  @DateEnded datetime,
  @IsRolledBack bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Imports]
  SET
    [FileName] = @FileName,
    [OrganizationID] = @OrganizationID,
    [ImportGUID] = @ImportGUID,
    [RefType] = @RefType,
    [AuxID] = @AuxID,
    [IsDone] = @IsDone,
    [IsRunning] = @IsRunning,
    [IsDeleted] = @IsDeleted,
    [NeedsDeleted] = @NeedsDeleted,
    [TotalRows] = @TotalRows,
    [CompletedRows] = @CompletedRows,
    [DateStarted] = @DateStarted,
    [DateEnded] = @DateEnded,
    [IsRolledBack] = @IsRolledBack
  WHERE ([ImportID] = @ImportID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteImport' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteImport
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteImport

(
  @ImportID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Imports]
  WHERE ([ImportID] = @ImportID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectImport' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectImport
GO

CREATE PROCEDURE dbo.uspGeneratedSelectImport

(
  @ImportID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ImportID],
    [FileName],
    [OrganizationID],
    [ImportGUID],
    [RefType],
    [AuxID],
    [IsDone],
    [IsRunning],
    [IsDeleted],
    [NeedsDeleted],
    [TotalRows],
    [CompletedRows],
    [DateStarted],
    [DateEnded],
    [DateCreated],
    [CreatorID],
    [IsRolledBack]
  FROM [dbo].[Imports]
  WHERE ([ImportID] = @ImportID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertImport' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertImport
GO

CREATE PROCEDURE dbo.uspGeneratedInsertImport

(
  @FileName varchar(255),
  @OrganizationID int,
  @ImportGUID uniqueidentifier,
  @RefType int,
  @AuxID int,
  @IsDone bit,
  @IsRunning bit,
  @IsDeleted bit,
  @NeedsDeleted bit,
  @TotalRows int,
  @CompletedRows int,
  @DateStarted datetime,
  @DateEnded datetime,
  @DateCreated datetime,
  @CreatorID int,
  @IsRolledBack bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Imports]
  (
    [FileName],
    [OrganizationID],
    [ImportGUID],
    [RefType],
    [AuxID],
    [IsDone],
    [IsRunning],
    [IsDeleted],
    [NeedsDeleted],
    [TotalRows],
    [CompletedRows],
    [DateStarted],
    [DateEnded],
    [DateCreated],
    [CreatorID],
    [IsRolledBack])
  VALUES (
    @FileName,
    @OrganizationID,
    @ImportGUID,
    @RefType,
    @AuxID,
    @IsDone,
    @IsRunning,
    @IsDeleted,
    @NeedsDeleted,
    @TotalRows,
    @CompletedRows,
    @DateStarted,
    @DateEnded,
    @DateCreated,
    @CreatorID,
    @IsRolledBack)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateImport' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateImport
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateImport

(
  @ImportID int,
  @FileName varchar(255),
  @OrganizationID int,
  @ImportGUID uniqueidentifier,
  @RefType int,
  @AuxID int,
  @IsDone bit,
  @IsRunning bit,
  @IsDeleted bit,
  @NeedsDeleted bit,
  @TotalRows int,
  @CompletedRows int,
  @DateStarted datetime,
  @DateEnded datetime,
  @IsRolledBack bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Imports]
  SET
    [FileName] = @FileName,
    [OrganizationID] = @OrganizationID,
    [ImportGUID] = @ImportGUID,
    [RefType] = @RefType,
    [AuxID] = @AuxID,
    [IsDone] = @IsDone,
    [IsRunning] = @IsRunning,
    [IsDeleted] = @IsDeleted,
    [NeedsDeleted] = @NeedsDeleted,
    [TotalRows] = @TotalRows,
    [CompletedRows] = @CompletedRows,
    [DateStarted] = @DateStarted,
    [DateEnded] = @DateEnded,
    [IsRolledBack] = @IsRolledBack
  WHERE ([ImportID] = @ImportID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteImport' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteImport
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteImport

(
  @ImportID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Imports]
  WHERE ([ImportID] = @ImportID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectImport' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectImport
GO

CREATE PROCEDURE dbo.uspGeneratedSelectImport

(
  @ImportID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ImportID],
    [FileName],
    [OrganizationID],
    [ImportGUID],
    [RefType],
    [AuxID],
    [IsDone],
    [IsRunning],
    [IsDeleted],
    [NeedsDeleted],
    [TotalRows],
    [CompletedRows],
    [DateStarted],
    [DateEnded],
    [DateCreated],
    [CreatorID],
    [IsRolledBack]
  FROM [dbo].[Imports]
  WHERE ([ImportID] = @ImportID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertImport' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertImport
GO

CREATE PROCEDURE dbo.uspGeneratedInsertImport

(
  @FileName varchar(255),
  @OrganizationID int,
  @ImportGUID uniqueidentifier,
  @RefType int,
  @AuxID int,
  @IsDone bit,
  @IsRunning bit,
  @IsDeleted bit,
  @NeedsDeleted bit,
  @TotalRows int,
  @CompletedRows int,
  @DateStarted datetime,
  @DateEnded datetime,
  @DateCreated datetime,
  @CreatorID int,
  @IsRolledBack bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Imports]
  (
    [FileName],
    [OrganizationID],
    [ImportGUID],
    [RefType],
    [AuxID],
    [IsDone],
    [IsRunning],
    [IsDeleted],
    [NeedsDeleted],
    [TotalRows],
    [CompletedRows],
    [DateStarted],
    [DateEnded],
    [DateCreated],
    [CreatorID],
    [IsRolledBack])
  VALUES (
    @FileName,
    @OrganizationID,
    @ImportGUID,
    @RefType,
    @AuxID,
    @IsDone,
    @IsRunning,
    @IsDeleted,
    @NeedsDeleted,
    @TotalRows,
    @CompletedRows,
    @DateStarted,
    @DateEnded,
    @DateCreated,
    @CreatorID,
    @IsRolledBack)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateImport' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateImport
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateImport

(
  @ImportID int,
  @FileName varchar(255),
  @OrganizationID int,
  @ImportGUID uniqueidentifier,
  @RefType int,
  @AuxID int,
  @IsDone bit,
  @IsRunning bit,
  @IsDeleted bit,
  @NeedsDeleted bit,
  @TotalRows int,
  @CompletedRows int,
  @DateStarted datetime,
  @DateEnded datetime,
  @IsRolledBack bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Imports]
  SET
    [FileName] = @FileName,
    [OrganizationID] = @OrganizationID,
    [ImportGUID] = @ImportGUID,
    [RefType] = @RefType,
    [AuxID] = @AuxID,
    [IsDone] = @IsDone,
    [IsRunning] = @IsRunning,
    [IsDeleted] = @IsDeleted,
    [NeedsDeleted] = @NeedsDeleted,
    [TotalRows] = @TotalRows,
    [CompletedRows] = @CompletedRows,
    [DateStarted] = @DateStarted,
    [DateEnded] = @DateEnded,
    [IsRolledBack] = @IsRolledBack
  WHERE ([ImportID] = @ImportID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteImport' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteImport
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteImport

(
  @ImportID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Imports]
  WHERE ([ImportID] = @ImportID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectImport' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectImport
GO

CREATE PROCEDURE dbo.uspGeneratedSelectImport

(
  @ImportID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ImportID],
    [FileName],
    [OrganizationID],
    [ImportGUID],
    [RefType],
    [AuxID],
    [IsDone],
    [IsRunning],
    [IsDeleted],
    [NeedsDeleted],
    [TotalRows],
    [CompletedRows],
    [DateStarted],
    [DateEnded],
    [DateCreated],
    [CreatorID],
    [IsRolledBack]
  FROM [dbo].[Imports]
  WHERE ([ImportID] = @ImportID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertImport' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertImport
GO

CREATE PROCEDURE dbo.uspGeneratedInsertImport

(
  @FileName varchar(255),
  @OrganizationID int,
  @ImportGUID uniqueidentifier,
  @RefType int,
  @AuxID int,
  @IsDone bit,
  @IsRunning bit,
  @IsDeleted bit,
  @NeedsDeleted bit,
  @TotalRows int,
  @CompletedRows int,
  @DateStarted datetime,
  @DateEnded datetime,
  @DateCreated datetime,
  @CreatorID int,
  @IsRolledBack bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Imports]
  (
    [FileName],
    [OrganizationID],
    [ImportGUID],
    [RefType],
    [AuxID],
    [IsDone],
    [IsRunning],
    [IsDeleted],
    [NeedsDeleted],
    [TotalRows],
    [CompletedRows],
    [DateStarted],
    [DateEnded],
    [DateCreated],
    [CreatorID],
    [IsRolledBack])
  VALUES (
    @FileName,
    @OrganizationID,
    @ImportGUID,
    @RefType,
    @AuxID,
    @IsDone,
    @IsRunning,
    @IsDeleted,
    @NeedsDeleted,
    @TotalRows,
    @CompletedRows,
    @DateStarted,
    @DateEnded,
    @DateCreated,
    @CreatorID,
    @IsRolledBack)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateImport' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateImport
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateImport

(
  @ImportID int,
  @FileName varchar(255),
  @OrganizationID int,
  @ImportGUID uniqueidentifier,
  @RefType int,
  @AuxID int,
  @IsDone bit,
  @IsRunning bit,
  @IsDeleted bit,
  @NeedsDeleted bit,
  @TotalRows int,
  @CompletedRows int,
  @DateStarted datetime,
  @DateEnded datetime,
  @IsRolledBack bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Imports]
  SET
    [FileName] = @FileName,
    [OrganizationID] = @OrganizationID,
    [ImportGUID] = @ImportGUID,
    [RefType] = @RefType,
    [AuxID] = @AuxID,
    [IsDone] = @IsDone,
    [IsRunning] = @IsRunning,
    [IsDeleted] = @IsDeleted,
    [NeedsDeleted] = @NeedsDeleted,
    [TotalRows] = @TotalRows,
    [CompletedRows] = @CompletedRows,
    [DateStarted] = @DateStarted,
    [DateEnded] = @DateEnded,
    [IsRolledBack] = @IsRolledBack
  WHERE ([ImportID] = @ImportID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteImport' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteImport
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteImport

(
  @ImportID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Imports]
  WHERE ([ImportID] = @ImportID)
GO


