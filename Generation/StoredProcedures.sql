IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectOrganizationEmail' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectOrganizationEmail
GO

CREATE PROCEDURE dbo.uspGeneratedSelectOrganizationEmail

(
  @OrganizationEmailID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [OrganizationEmailID],
    [OrganizationID],
    [EmailTemplateID],
    [Subject],
    [Header],
    [Footer],
    [Body],
    [IsHtml],
    [UseGlobalTemplate],
    [ProductFamilyID]
  FROM [dbo].[OrganizationEmails]
  WHERE ([OrganizationEmailID] = @OrganizationEmailID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertOrganizationEmail' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertOrganizationEmail
GO

CREATE PROCEDURE dbo.uspGeneratedInsertOrganizationEmail

(
  @OrganizationID int,
  @EmailTemplateID int,
  @Subject varchar(8000),
  @Header varchar(1000),
  @Footer varchar(1000),
  @Body varchar(MAX),
  @IsHtml bit,
  @UseGlobalTemplate bit,
  @ProductFamilyID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[OrganizationEmails]
  (
    [OrganizationID],
    [EmailTemplateID],
    [Subject],
    [Header],
    [Footer],
    [Body],
    [IsHtml],
    [UseGlobalTemplate],
    [ProductFamilyID])
  VALUES (
    @OrganizationID,
    @EmailTemplateID,
    @Subject,
    @Header,
    @Footer,
    @Body,
    @IsHtml,
    @UseGlobalTemplate,
    @ProductFamilyID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateOrganizationEmail' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateOrganizationEmail
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateOrganizationEmail

(
  @OrganizationEmailID int,
  @OrganizationID int,
  @EmailTemplateID int,
  @Subject varchar(8000),
  @Header varchar(1000),
  @Footer varchar(1000),
  @Body varchar(MAX),
  @IsHtml bit,
  @UseGlobalTemplate bit,
  @ProductFamilyID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[OrganizationEmails]
  SET
    [OrganizationID] = @OrganizationID,
    [EmailTemplateID] = @EmailTemplateID,
    [Subject] = @Subject,
    [Header] = @Header,
    [Footer] = @Footer,
    [Body] = @Body,
    [IsHtml] = @IsHtml,
    [UseGlobalTemplate] = @UseGlobalTemplate,
    [ProductFamilyID] = @ProductFamilyID
  WHERE ([OrganizationEmailID] = @OrganizationEmailID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteOrganizationEmail' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteOrganizationEmail
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteOrganizationEmail

(
  @OrganizationEmailID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[OrganizationEmails]
  WHERE ([OrganizationEmailID] = @OrganizationEmailID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectOrganizationEmail' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectOrganizationEmail
GO

CREATE PROCEDURE dbo.uspGeneratedSelectOrganizationEmail

(
  @OrganizationEmailID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [OrganizationEmailID],
    [OrganizationID],
    [EmailTemplateID],
    [Subject],
    [Header],
    [Footer],
    [Body],
    [IsHtml],
    [UseGlobalTemplate],
    [ProductFamilyID]
  FROM [dbo].[OrganizationEmails]
  WHERE ([OrganizationEmailID] = @OrganizationEmailID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertOrganizationEmail' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertOrganizationEmail
GO

CREATE PROCEDURE dbo.uspGeneratedInsertOrganizationEmail

(
  @OrganizationID int,
  @EmailTemplateID int,
  @Subject varchar(8000),
  @Header varchar(1000),
  @Footer varchar(1000),
  @Body varchar(MAX),
  @IsHtml bit,
  @UseGlobalTemplate bit,
  @ProductFamilyID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[OrganizationEmails]
  (
    [OrganizationID],
    [EmailTemplateID],
    [Subject],
    [Header],
    [Footer],
    [Body],
    [IsHtml],
    [UseGlobalTemplate],
    [ProductFamilyID])
  VALUES (
    @OrganizationID,
    @EmailTemplateID,
    @Subject,
    @Header,
    @Footer,
    @Body,
    @IsHtml,
    @UseGlobalTemplate,
    @ProductFamilyID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateOrganizationEmail' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateOrganizationEmail
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateOrganizationEmail

(
  @OrganizationEmailID int,
  @OrganizationID int,
  @EmailTemplateID int,
  @Subject varchar(8000),
  @Header varchar(1000),
  @Footer varchar(1000),
  @Body varchar(MAX),
  @IsHtml bit,
  @UseGlobalTemplate bit,
  @ProductFamilyID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[OrganizationEmails]
  SET
    [OrganizationID] = @OrganizationID,
    [EmailTemplateID] = @EmailTemplateID,
    [Subject] = @Subject,
    [Header] = @Header,
    [Footer] = @Footer,
    [Body] = @Body,
    [IsHtml] = @IsHtml,
    [UseGlobalTemplate] = @UseGlobalTemplate,
    [ProductFamilyID] = @ProductFamilyID
  WHERE ([OrganizationEmailID] = @OrganizationEmailID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteOrganizationEmail' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteOrganizationEmail
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteOrganizationEmail

(
  @OrganizationEmailID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[OrganizationEmails]
  WHERE ([OrganizationEmailID] = @OrganizationEmailID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectOrganizationEmail' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectOrganizationEmail
GO

CREATE PROCEDURE dbo.uspGeneratedSelectOrganizationEmail

(
  @OrganizationEmailID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [OrganizationEmailID],
    [OrganizationID],
    [EmailTemplateID],
    [Subject],
    [Header],
    [Footer],
    [Body],
    [IsHtml],
    [UseGlobalTemplate],
    [ProductFamilyID]
  FROM [dbo].[OrganizationEmails]
  WHERE ([OrganizationEmailID] = @OrganizationEmailID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertOrganizationEmail' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertOrganizationEmail
GO

CREATE PROCEDURE dbo.uspGeneratedInsertOrganizationEmail

(
  @OrganizationID int,
  @EmailTemplateID int,
  @Subject varchar(8000),
  @Header varchar(1000),
  @Footer varchar(1000),
  @Body varchar(MAX),
  @IsHtml bit,
  @UseGlobalTemplate bit,
  @ProductFamilyID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[OrganizationEmails]
  (
    [OrganizationID],
    [EmailTemplateID],
    [Subject],
    [Header],
    [Footer],
    [Body],
    [IsHtml],
    [UseGlobalTemplate],
    [ProductFamilyID])
  VALUES (
    @OrganizationID,
    @EmailTemplateID,
    @Subject,
    @Header,
    @Footer,
    @Body,
    @IsHtml,
    @UseGlobalTemplate,
    @ProductFamilyID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateOrganizationEmail' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateOrganizationEmail
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateOrganizationEmail

(
  @OrganizationEmailID int,
  @OrganizationID int,
  @EmailTemplateID int,
  @Subject varchar(8000),
  @Header varchar(1000),
  @Footer varchar(1000),
  @Body varchar(MAX),
  @IsHtml bit,
  @UseGlobalTemplate bit,
  @ProductFamilyID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[OrganizationEmails]
  SET
    [OrganizationID] = @OrganizationID,
    [EmailTemplateID] = @EmailTemplateID,
    [Subject] = @Subject,
    [Header] = @Header,
    [Footer] = @Footer,
    [Body] = @Body,
    [IsHtml] = @IsHtml,
    [UseGlobalTemplate] = @UseGlobalTemplate,
    [ProductFamilyID] = @ProductFamilyID
  WHERE ([OrganizationEmailID] = @OrganizationEmailID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteOrganizationEmail' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteOrganizationEmail
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteOrganizationEmail

(
  @OrganizationEmailID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[OrganizationEmails]
  WHERE ([OrganizationEmailID] = @OrganizationEmailID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectOrganizationEmail' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectOrganizationEmail
GO

CREATE PROCEDURE dbo.uspGeneratedSelectOrganizationEmail

(
  @OrganizationEmailID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [OrganizationEmailID],
    [OrganizationID],
    [EmailTemplateID],
    [Subject],
    [Header],
    [Footer],
    [Body],
    [IsHtml],
    [UseGlobalTemplate],
    [ProductFamilyID]
  FROM [dbo].[OrganizationEmails]
  WHERE ([OrganizationEmailID] = @OrganizationEmailID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertOrganizationEmail' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertOrganizationEmail
GO

CREATE PROCEDURE dbo.uspGeneratedInsertOrganizationEmail

(
  @OrganizationID int,
  @EmailTemplateID int,
  @Subject varchar(8000),
  @Header varchar(1000),
  @Footer varchar(1000),
  @Body varchar(MAX),
  @IsHtml bit,
  @UseGlobalTemplate bit,
  @ProductFamilyID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[OrganizationEmails]
  (
    [OrganizationID],
    [EmailTemplateID],
    [Subject],
    [Header],
    [Footer],
    [Body],
    [IsHtml],
    [UseGlobalTemplate],
    [ProductFamilyID])
  VALUES (
    @OrganizationID,
    @EmailTemplateID,
    @Subject,
    @Header,
    @Footer,
    @Body,
    @IsHtml,
    @UseGlobalTemplate,
    @ProductFamilyID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateOrganizationEmail' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateOrganizationEmail
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateOrganizationEmail

(
  @OrganizationEmailID int,
  @OrganizationID int,
  @EmailTemplateID int,
  @Subject varchar(8000),
  @Header varchar(1000),
  @Footer varchar(1000),
  @Body varchar(MAX),
  @IsHtml bit,
  @UseGlobalTemplate bit,
  @ProductFamilyID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[OrganizationEmails]
  SET
    [OrganizationID] = @OrganizationID,
    [EmailTemplateID] = @EmailTemplateID,
    [Subject] = @Subject,
    [Header] = @Header,
    [Footer] = @Footer,
    [Body] = @Body,
    [IsHtml] = @IsHtml,
    [UseGlobalTemplate] = @UseGlobalTemplate,
    [ProductFamilyID] = @ProductFamilyID
  WHERE ([OrganizationEmailID] = @OrganizationEmailID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteOrganizationEmail' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteOrganizationEmail
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteOrganizationEmail

(
  @OrganizationEmailID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[OrganizationEmails]
  WHERE ([OrganizationEmailID] = @OrganizationEmailID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectOrganizationEmail' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectOrganizationEmail
GO

CREATE PROCEDURE dbo.uspGeneratedSelectOrganizationEmail

(
  @OrganizationEmailID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [OrganizationEmailID],
    [OrganizationID],
    [EmailTemplateID],
    [Subject],
    [Header],
    [Footer],
    [Body],
    [IsHtml],
    [UseGlobalTemplate],
    [ProductFamilyID]
  FROM [dbo].[OrganizationEmails]
  WHERE ([OrganizationEmailID] = @OrganizationEmailID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertOrganizationEmail' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertOrganizationEmail
GO

CREATE PROCEDURE dbo.uspGeneratedInsertOrganizationEmail

(
  @OrganizationID int,
  @EmailTemplateID int,
  @Subject varchar(8000),
  @Header varchar(1000),
  @Footer varchar(1000),
  @Body varchar(MAX),
  @IsHtml bit,
  @UseGlobalTemplate bit,
  @ProductFamilyID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[OrganizationEmails]
  (
    [OrganizationID],
    [EmailTemplateID],
    [Subject],
    [Header],
    [Footer],
    [Body],
    [IsHtml],
    [UseGlobalTemplate],
    [ProductFamilyID])
  VALUES (
    @OrganizationID,
    @EmailTemplateID,
    @Subject,
    @Header,
    @Footer,
    @Body,
    @IsHtml,
    @UseGlobalTemplate,
    @ProductFamilyID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateOrganizationEmail' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateOrganizationEmail
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateOrganizationEmail

(
  @OrganizationEmailID int,
  @OrganizationID int,
  @EmailTemplateID int,
  @Subject varchar(8000),
  @Header varchar(1000),
  @Footer varchar(1000),
  @Body varchar(MAX),
  @IsHtml bit,
  @UseGlobalTemplate bit,
  @ProductFamilyID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[OrganizationEmails]
  SET
    [OrganizationID] = @OrganizationID,
    [EmailTemplateID] = @EmailTemplateID,
    [Subject] = @Subject,
    [Header] = @Header,
    [Footer] = @Footer,
    [Body] = @Body,
    [IsHtml] = @IsHtml,
    [UseGlobalTemplate] = @UseGlobalTemplate,
    [ProductFamilyID] = @ProductFamilyID
  WHERE ([OrganizationEmailID] = @OrganizationEmailID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteOrganizationEmail' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteOrganizationEmail
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteOrganizationEmail

(
  @OrganizationEmailID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[OrganizationEmails]
  WHERE ([OrganizationEmailID] = @OrganizationEmailID)
GO


