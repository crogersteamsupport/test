IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectEmail' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectEmail
GO

CREATE PROCEDURE dbo.uspGeneratedSelectEmail

(
  @EmailID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [EmailID],
    [OrganizationID],
    [Description],
    [FromAddress],
    [ToAddress],
    [CCAddress],
    [BCCAddress],
    [Subject],
    [Body],
    [Attachments],
    [Size],
    [IsSuccess],
    [IsWaiting],
    [IsHtml],
    [Attempts],
    [NextAttempt],
    [DateSent],
    [LastFailedReason],
    [EmailPostID],
    [DateCreated],
    [LockProcessID]
  FROM [dbo].[Emails]
  WHERE ([EmailID] = @EmailID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertEmail' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertEmail
GO

CREATE PROCEDURE dbo.uspGeneratedInsertEmail

(
  @OrganizationID int,
  @Description varchar(250),
  @FromAddress varchar(250),
  @ToAddress varchar(8000),
  @CCAddress varchar(8000),
  @BCCAddress varchar(8000),
  @Subject nvarchar(4000),
  @Body nvarchar(MAX),
  @Attachments nvarchar(4000),
  @Size int,
  @IsSuccess bit,
  @IsWaiting bit,
  @IsHtml bit,
  @Attempts int,
  @NextAttempt datetime,
  @DateSent datetime,
  @LastFailedReason varchar(8000),
  @EmailPostID int,
  @DateCreated datetime,
  @LockProcessID varchar(250),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Emails]
  (
    [OrganizationID],
    [Description],
    [FromAddress],
    [ToAddress],
    [CCAddress],
    [BCCAddress],
    [Subject],
    [Body],
    [Attachments],
    [Size],
    [IsSuccess],
    [IsWaiting],
    [IsHtml],
    [Attempts],
    [NextAttempt],
    [DateSent],
    [LastFailedReason],
    [EmailPostID],
    [DateCreated],
    [LockProcessID])
  VALUES (
    @OrganizationID,
    @Description,
    @FromAddress,
    @ToAddress,
    @CCAddress,
    @BCCAddress,
    @Subject,
    @Body,
    @Attachments,
    @Size,
    @IsSuccess,
    @IsWaiting,
    @IsHtml,
    @Attempts,
    @NextAttempt,
    @DateSent,
    @LastFailedReason,
    @EmailPostID,
    @DateCreated,
    @LockProcessID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateEmail' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateEmail
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateEmail

(
  @EmailID int,
  @OrganizationID int,
  @Description varchar(250),
  @FromAddress varchar(250),
  @ToAddress varchar(8000),
  @CCAddress varchar(8000),
  @BCCAddress varchar(8000),
  @Subject nvarchar(4000),
  @Body nvarchar(MAX),
  @Attachments nvarchar(4000),
  @Size int,
  @IsSuccess bit,
  @IsWaiting bit,
  @IsHtml bit,
  @Attempts int,
  @NextAttempt datetime,
  @DateSent datetime,
  @LastFailedReason varchar(8000),
  @EmailPostID int,
  @LockProcessID varchar(250)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Emails]
  SET
    [OrganizationID] = @OrganizationID,
    [Description] = @Description,
    [FromAddress] = @FromAddress,
    [ToAddress] = @ToAddress,
    [CCAddress] = @CCAddress,
    [BCCAddress] = @BCCAddress,
    [Subject] = @Subject,
    [Body] = @Body,
    [Attachments] = @Attachments,
    [Size] = @Size,
    [IsSuccess] = @IsSuccess,
    [IsWaiting] = @IsWaiting,
    [IsHtml] = @IsHtml,
    [Attempts] = @Attempts,
    [NextAttempt] = @NextAttempt,
    [DateSent] = @DateSent,
    [LastFailedReason] = @LastFailedReason,
    [EmailPostID] = @EmailPostID,
    [LockProcessID] = @LockProcessID
  WHERE ([EmailID] = @EmailID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteEmail' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteEmail
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteEmail

(
  @EmailID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Emails]
  WHERE ([EmailID] = @EmailID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectEmail' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectEmail
GO

CREATE PROCEDURE dbo.uspGeneratedSelectEmail

(
  @EmailID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [EmailID],
    [OrganizationID],
    [Description],
    [FromAddress],
    [ToAddress],
    [CCAddress],
    [BCCAddress],
    [Subject],
    [Body],
    [Attachments],
    [Size],
    [IsSuccess],
    [IsWaiting],
    [IsHtml],
    [Attempts],
    [NextAttempt],
    [DateSent],
    [LastFailedReason],
    [EmailPostID],
    [DateCreated],
    [LockProcessID]
  FROM [dbo].[Emails]
  WHERE ([EmailID] = @EmailID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertEmail' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertEmail
GO

CREATE PROCEDURE dbo.uspGeneratedInsertEmail

(
  @OrganizationID int,
  @Description varchar(250),
  @FromAddress varchar(250),
  @ToAddress varchar(8000),
  @CCAddress varchar(8000),
  @BCCAddress varchar(8000),
  @Subject nvarchar(4000),
  @Body nvarchar(MAX),
  @Attachments nvarchar(4000),
  @Size int,
  @IsSuccess bit,
  @IsWaiting bit,
  @IsHtml bit,
  @Attempts int,
  @NextAttempt datetime,
  @DateSent datetime,
  @LastFailedReason varchar(8000),
  @EmailPostID int,
  @DateCreated datetime,
  @LockProcessID varchar(250),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Emails]
  (
    [OrganizationID],
    [Description],
    [FromAddress],
    [ToAddress],
    [CCAddress],
    [BCCAddress],
    [Subject],
    [Body],
    [Attachments],
    [Size],
    [IsSuccess],
    [IsWaiting],
    [IsHtml],
    [Attempts],
    [NextAttempt],
    [DateSent],
    [LastFailedReason],
    [EmailPostID],
    [DateCreated],
    [LockProcessID])
  VALUES (
    @OrganizationID,
    @Description,
    @FromAddress,
    @ToAddress,
    @CCAddress,
    @BCCAddress,
    @Subject,
    @Body,
    @Attachments,
    @Size,
    @IsSuccess,
    @IsWaiting,
    @IsHtml,
    @Attempts,
    @NextAttempt,
    @DateSent,
    @LastFailedReason,
    @EmailPostID,
    @DateCreated,
    @LockProcessID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateEmail' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateEmail
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateEmail

(
  @EmailID int,
  @OrganizationID int,
  @Description varchar(250),
  @FromAddress varchar(250),
  @ToAddress varchar(8000),
  @CCAddress varchar(8000),
  @BCCAddress varchar(8000),
  @Subject nvarchar(4000),
  @Body nvarchar(MAX),
  @Attachments nvarchar(4000),
  @Size int,
  @IsSuccess bit,
  @IsWaiting bit,
  @IsHtml bit,
  @Attempts int,
  @NextAttempt datetime,
  @DateSent datetime,
  @LastFailedReason varchar(8000),
  @EmailPostID int,
  @LockProcessID varchar(250)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Emails]
  SET
    [OrganizationID] = @OrganizationID,
    [Description] = @Description,
    [FromAddress] = @FromAddress,
    [ToAddress] = @ToAddress,
    [CCAddress] = @CCAddress,
    [BCCAddress] = @BCCAddress,
    [Subject] = @Subject,
    [Body] = @Body,
    [Attachments] = @Attachments,
    [Size] = @Size,
    [IsSuccess] = @IsSuccess,
    [IsWaiting] = @IsWaiting,
    [IsHtml] = @IsHtml,
    [Attempts] = @Attempts,
    [NextAttempt] = @NextAttempt,
    [DateSent] = @DateSent,
    [LastFailedReason] = @LastFailedReason,
    [EmailPostID] = @EmailPostID,
    [LockProcessID] = @LockProcessID
  WHERE ([EmailID] = @EmailID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteEmail' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteEmail
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteEmail

(
  @EmailID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Emails]
  WHERE ([EmailID] = @EmailID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectEmail' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectEmail
GO

CREATE PROCEDURE dbo.uspGeneratedSelectEmail

(
  @EmailID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [EmailID],
    [OrganizationID],
    [Description],
    [FromAddress],
    [ToAddress],
    [CCAddress],
    [BCCAddress],
    [Subject],
    [Body],
    [Attachments],
    [Size],
    [IsSuccess],
    [IsWaiting],
    [IsHtml],
    [Attempts],
    [NextAttempt],
    [DateSent],
    [LastFailedReason],
    [EmailPostID],
    [DateCreated],
    [LockProcessID]
  FROM [dbo].[Emails]
  WHERE ([EmailID] = @EmailID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertEmail' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertEmail
GO

CREATE PROCEDURE dbo.uspGeneratedInsertEmail

(
  @OrganizationID int,
  @Description varchar(250),
  @FromAddress varchar(250),
  @ToAddress varchar(8000),
  @CCAddress varchar(8000),
  @BCCAddress varchar(8000),
  @Subject nvarchar(4000),
  @Body nvarchar(MAX),
  @Attachments nvarchar(4000),
  @Size int,
  @IsSuccess bit,
  @IsWaiting bit,
  @IsHtml bit,
  @Attempts int,
  @NextAttempt datetime,
  @DateSent datetime,
  @LastFailedReason varchar(8000),
  @EmailPostID int,
  @DateCreated datetime,
  @LockProcessID varchar(250),
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Emails]
  (
    [OrganizationID],
    [Description],
    [FromAddress],
    [ToAddress],
    [CCAddress],
    [BCCAddress],
    [Subject],
    [Body],
    [Attachments],
    [Size],
    [IsSuccess],
    [IsWaiting],
    [IsHtml],
    [Attempts],
    [NextAttempt],
    [DateSent],
    [LastFailedReason],
    [EmailPostID],
    [DateCreated],
    [LockProcessID])
  VALUES (
    @OrganizationID,
    @Description,
    @FromAddress,
    @ToAddress,
    @CCAddress,
    @BCCAddress,
    @Subject,
    @Body,
    @Attachments,
    @Size,
    @IsSuccess,
    @IsWaiting,
    @IsHtml,
    @Attempts,
    @NextAttempt,
    @DateSent,
    @LastFailedReason,
    @EmailPostID,
    @DateCreated,
    @LockProcessID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateEmail' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateEmail
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateEmail

(
  @EmailID int,
  @OrganizationID int,
  @Description varchar(250),
  @FromAddress varchar(250),
  @ToAddress varchar(8000),
  @CCAddress varchar(8000),
  @BCCAddress varchar(8000),
  @Subject nvarchar(4000),
  @Body nvarchar(MAX),
  @Attachments nvarchar(4000),
  @Size int,
  @IsSuccess bit,
  @IsWaiting bit,
  @IsHtml bit,
  @Attempts int,
  @NextAttempt datetime,
  @DateSent datetime,
  @LastFailedReason varchar(8000),
  @EmailPostID int,
  @LockProcessID varchar(250)
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Emails]
  SET
    [OrganizationID] = @OrganizationID,
    [Description] = @Description,
    [FromAddress] = @FromAddress,
    [ToAddress] = @ToAddress,
    [CCAddress] = @CCAddress,
    [BCCAddress] = @BCCAddress,
    [Subject] = @Subject,
    [Body] = @Body,
    [Attachments] = @Attachments,
    [Size] = @Size,
    [IsSuccess] = @IsSuccess,
    [IsWaiting] = @IsWaiting,
    [IsHtml] = @IsHtml,
    [Attempts] = @Attempts,
    [NextAttempt] = @NextAttempt,
    [DateSent] = @DateSent,
    [LastFailedReason] = @LastFailedReason,
    [EmailPostID] = @EmailPostID,
    [LockProcessID] = @LockProcessID
  WHERE ([EmailID] = @EmailID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteEmail' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteEmail
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteEmail

(
  @EmailID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Emails]
  WHERE ([EmailID] = @EmailID)
GO


