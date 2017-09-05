IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectDeletedTicket' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectDeletedTicket
GO

CREATE PROCEDURE dbo.uspGeneratedSelectDeletedTicket

(
  @ID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ID],
    [TicketID],
    [TicketNumber],
    [OrganizationID],
    [Name],
    [DateDeleted],
    [DeleterID]
  FROM [dbo].[DeletedTickets]
  WHERE ([ID] = @ID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertDeletedTicket' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertDeletedTicket
GO

CREATE PROCEDURE dbo.uspGeneratedInsertDeletedTicket

(
  @TicketID int,
  @TicketNumber int,
  @OrganizationID int,
  @Name nvarchar(255),
  @DateDeleted datetime,
  @DeleterID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[DeletedTickets]
  (
    [TicketID],
    [TicketNumber],
    [OrganizationID],
    [Name],
    [DateDeleted],
    [DeleterID])
  VALUES (
    @TicketID,
    @TicketNumber,
    @OrganizationID,
    @Name,
    @DateDeleted,
    @DeleterID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateDeletedTicket' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateDeletedTicket
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateDeletedTicket

(
  @ID int,
  @TicketID int,
  @TicketNumber int,
  @OrganizationID int,
  @Name nvarchar(255),
  @DateDeleted datetime,
  @DeleterID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[DeletedTickets]
  SET
    [TicketID] = @TicketID,
    [TicketNumber] = @TicketNumber,
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [DateDeleted] = @DateDeleted,
    [DeleterID] = @DeleterID
  WHERE ([ID] = @ID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteDeletedTicket' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteDeletedTicket
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteDeletedTicket

(
  @ID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[DeletedTickets]
  WHERE ([ID] = @ID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectDeletedTicket' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectDeletedTicket
GO

CREATE PROCEDURE dbo.uspGeneratedSelectDeletedTicket

(
  @ID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ID],
    [TicketID],
    [TicketNumber],
    [OrganizationID],
    [Name],
    [DateDeleted],
    [DeleterID]
  FROM [dbo].[DeletedTickets]
  WHERE ([ID] = @ID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertDeletedTicket' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertDeletedTicket
GO

CREATE PROCEDURE dbo.uspGeneratedInsertDeletedTicket

(
  @TicketID int,
  @TicketNumber int,
  @OrganizationID int,
  @Name nvarchar(255),
  @DateDeleted datetime,
  @DeleterID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[DeletedTickets]
  (
    [TicketID],
    [TicketNumber],
    [OrganizationID],
    [Name],
    [DateDeleted],
    [DeleterID])
  VALUES (
    @TicketID,
    @TicketNumber,
    @OrganizationID,
    @Name,
    @DateDeleted,
    @DeleterID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateDeletedTicket' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateDeletedTicket
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateDeletedTicket

(
  @ID int,
  @TicketID int,
  @TicketNumber int,
  @OrganizationID int,
  @Name nvarchar(255),
  @DateDeleted datetime,
  @DeleterID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[DeletedTickets]
  SET
    [TicketID] = @TicketID,
    [TicketNumber] = @TicketNumber,
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [DateDeleted] = @DateDeleted,
    [DeleterID] = @DeleterID
  WHERE ([ID] = @ID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteDeletedTicket' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteDeletedTicket
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteDeletedTicket

(
  @ID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[DeletedTickets]
  WHERE ([ID] = @ID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectDeletedTicket' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectDeletedTicket
GO

CREATE PROCEDURE dbo.uspGeneratedSelectDeletedTicket

(
  @ID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ID],
    [TicketID],
    [TicketNumber],
    [OrganizationID],
    [Name],
    [DateDeleted],
    [DeleterID]
  FROM [dbo].[DeletedTickets]
  WHERE ([ID] = @ID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertDeletedTicket' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertDeletedTicket
GO

CREATE PROCEDURE dbo.uspGeneratedInsertDeletedTicket

(
  @TicketID int,
  @TicketNumber int,
  @OrganizationID int,
  @Name nvarchar(255),
  @DateDeleted datetime,
  @DeleterID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[DeletedTickets]
  (
    [TicketID],
    [TicketNumber],
    [OrganizationID],
    [Name],
    [DateDeleted],
    [DeleterID])
  VALUES (
    @TicketID,
    @TicketNumber,
    @OrganizationID,
    @Name,
    @DateDeleted,
    @DeleterID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateDeletedTicket' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateDeletedTicket
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateDeletedTicket

(
  @ID int,
  @TicketID int,
  @TicketNumber int,
  @OrganizationID int,
  @Name nvarchar(255),
  @DateDeleted datetime,
  @DeleterID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[DeletedTickets]
  SET
    [TicketID] = @TicketID,
    [TicketNumber] = @TicketNumber,
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [DateDeleted] = @DateDeleted,
    [DeleterID] = @DeleterID
  WHERE ([ID] = @ID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteDeletedTicket' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteDeletedTicket
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteDeletedTicket

(
  @ID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[DeletedTickets]
  WHERE ([ID] = @ID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectDeletedTicket' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectDeletedTicket
GO

CREATE PROCEDURE dbo.uspGeneratedSelectDeletedTicket

(
  @ID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ID],
    [TicketID],
    [TicketNumber],
    [OrganizationID],
    [Name],
    [DateDeleted],
    [DeleterID]
  FROM [dbo].[DeletedTickets]
  WHERE ([ID] = @ID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertDeletedTicket' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertDeletedTicket
GO

CREATE PROCEDURE dbo.uspGeneratedInsertDeletedTicket

(
  @TicketID int,
  @TicketNumber int,
  @OrganizationID int,
  @Name nvarchar(255),
  @DateDeleted datetime,
  @DeleterID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[DeletedTickets]
  (
    [TicketID],
    [TicketNumber],
    [OrganizationID],
    [Name],
    [DateDeleted],
    [DeleterID])
  VALUES (
    @TicketID,
    @TicketNumber,
    @OrganizationID,
    @Name,
    @DateDeleted,
    @DeleterID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateDeletedTicket' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateDeletedTicket
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateDeletedTicket

(
  @ID int,
  @TicketID int,
  @TicketNumber int,
  @OrganizationID int,
  @Name nvarchar(255),
  @DateDeleted datetime,
  @DeleterID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[DeletedTickets]
  SET
    [TicketID] = @TicketID,
    [TicketNumber] = @TicketNumber,
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [DateDeleted] = @DateDeleted,
    [DeleterID] = @DeleterID
  WHERE ([ID] = @ID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteDeletedTicket' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteDeletedTicket
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteDeletedTicket

(
  @ID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[DeletedTickets]
  WHERE ([ID] = @ID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectDeletedTicket' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectDeletedTicket
GO

CREATE PROCEDURE dbo.uspGeneratedSelectDeletedTicket

(
  @ID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [ID],
    [TicketID],
    [TicketNumber],
    [OrganizationID],
    [Name],
    [DateDeleted],
    [DeleterID]
  FROM [dbo].[DeletedTickets]
  WHERE ([ID] = @ID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertDeletedTicket' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertDeletedTicket
GO

CREATE PROCEDURE dbo.uspGeneratedInsertDeletedTicket

(
  @TicketID int,
  @TicketNumber int,
  @OrganizationID int,
  @Name nvarchar(255),
  @DateDeleted datetime,
  @DeleterID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[DeletedTickets]
  (
    [TicketID],
    [TicketNumber],
    [OrganizationID],
    [Name],
    [DateDeleted],
    [DeleterID])
  VALUES (
    @TicketID,
    @TicketNumber,
    @OrganizationID,
    @Name,
    @DateDeleted,
    @DeleterID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateDeletedTicket' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateDeletedTicket
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateDeletedTicket

(
  @ID int,
  @TicketID int,
  @TicketNumber int,
  @OrganizationID int,
  @Name nvarchar(255),
  @DateDeleted datetime,
  @DeleterID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[DeletedTickets]
  SET
    [TicketID] = @TicketID,
    [TicketNumber] = @TicketNumber,
    [OrganizationID] = @OrganizationID,
    [Name] = @Name,
    [DateDeleted] = @DateDeleted,
    [DeleterID] = @DeleterID
  WHERE ([ID] = @ID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteDeletedTicket' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteDeletedTicket
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteDeletedTicket

(
  @ID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[DeletedTickets]
  WHERE ([ID] = @ID)
GO


