IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCustomerHubFeatureSetting' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCustomerHubFeatureSetting
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCustomerHubFeatureSetting

(
  @CustomerHubFeatureSettingID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [CustomerHubFeatureSettingID],
    [CustomerHubID],
    [EnableKnowledgeBase],
    [EnableProducts],
    [EnableTicketCreation],
    [EnableMyTickets],
    [EnableOrganizationTickets],
    [EnableWiki],
    [EnableTicketGroupSelection],
    [EnableTicketProductSelection],
    [EnableTicketProductVersionSelection],
    [DefaultTicketTypeID],
    [DefaultGroupTypeID],
    [EnableCustomerProductAssociation],
    [EnableChat],
    [EnableCommunity],
    [EnableScreenRecording],
    [EnableVideoRecording],
    [DateModified],
    [ModifierID],
    [EnableTicketSeverity],
    [EnableTicketSeverityModification],
    [RestrictProductVersions],
    [EnableTicketNameModification],
    [KnowledgeBaseSortTypeID],
    [CommunitySortTypeID],
    [EnableAnonymousProductAssociation],
    [EnableCustomerSpecificKB],
    [EnableCustomFieldModification],
    [EnableProductFamilyFiltering]
  FROM [dbo].[CustomerHubFeatureSettings]
  WHERE ([CustomerHubFeatureSettingID] = @CustomerHubFeatureSettingID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCustomerHubFeatureSetting' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCustomerHubFeatureSetting
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCustomerHubFeatureSetting

(
  @CustomerHubID int,
  @EnableKnowledgeBase bit,
  @EnableProducts bit,
  @EnableTicketCreation bit,
  @EnableMyTickets bit,
  @EnableOrganizationTickets bit,
  @EnableWiki bit,
  @EnableTicketGroupSelection bit,
  @EnableTicketProductSelection bit,
  @EnableTicketProductVersionSelection bit,
  @DefaultTicketTypeID int,
  @DefaultGroupTypeID int,
  @EnableCustomerProductAssociation bit,
  @EnableChat bit,
  @EnableCommunity bit,
  @EnableScreenRecording bit,
  @EnableVideoRecording bit,
  @DateModified datetime,
  @ModifierID int,
  @EnableTicketSeverity bit,
  @EnableTicketSeverityModification bit,
  @RestrictProductVersions bit,
  @EnableTicketNameModification bit,
  @KnowledgeBaseSortTypeID int,
  @CommunitySortTypeID int,
  @EnableAnonymousProductAssociation bit,
  @EnableCustomerSpecificKB bit,
  @EnableCustomFieldModification bit,
  @EnableProductFamilyFiltering bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CustomerHubFeatureSettings]
  (
    [CustomerHubID],
    [EnableKnowledgeBase],
    [EnableProducts],
    [EnableTicketCreation],
    [EnableMyTickets],
    [EnableOrganizationTickets],
    [EnableWiki],
    [EnableTicketGroupSelection],
    [EnableTicketProductSelection],
    [EnableTicketProductVersionSelection],
    [DefaultTicketTypeID],
    [DefaultGroupTypeID],
    [EnableCustomerProductAssociation],
    [EnableChat],
    [EnableCommunity],
    [EnableScreenRecording],
    [EnableVideoRecording],
    [DateModified],
    [ModifierID],
    [EnableTicketSeverity],
    [EnableTicketSeverityModification],
    [RestrictProductVersions],
    [EnableTicketNameModification],
    [KnowledgeBaseSortTypeID],
    [CommunitySortTypeID],
    [EnableAnonymousProductAssociation],
    [EnableCustomerSpecificKB],
    [EnableCustomFieldModification],
    [EnableProductFamilyFiltering])
  VALUES (
    @CustomerHubID,
    @EnableKnowledgeBase,
    @EnableProducts,
    @EnableTicketCreation,
    @EnableMyTickets,
    @EnableOrganizationTickets,
    @EnableWiki,
    @EnableTicketGroupSelection,
    @EnableTicketProductSelection,
    @EnableTicketProductVersionSelection,
    @DefaultTicketTypeID,
    @DefaultGroupTypeID,
    @EnableCustomerProductAssociation,
    @EnableChat,
    @EnableCommunity,
    @EnableScreenRecording,
    @EnableVideoRecording,
    @DateModified,
    @ModifierID,
    @EnableTicketSeverity,
    @EnableTicketSeverityModification,
    @RestrictProductVersions,
    @EnableTicketNameModification,
    @KnowledgeBaseSortTypeID,
    @CommunitySortTypeID,
    @EnableAnonymousProductAssociation,
    @EnableCustomerSpecificKB,
    @EnableCustomFieldModification,
    @EnableProductFamilyFiltering)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCustomerHubFeatureSetting' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCustomerHubFeatureSetting
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCustomerHubFeatureSetting

(
  @CustomerHubFeatureSettingID int,
  @CustomerHubID int,
  @EnableKnowledgeBase bit,
  @EnableProducts bit,
  @EnableTicketCreation bit,
  @EnableMyTickets bit,
  @EnableOrganizationTickets bit,
  @EnableWiki bit,
  @EnableTicketGroupSelection bit,
  @EnableTicketProductSelection bit,
  @EnableTicketProductVersionSelection bit,
  @DefaultTicketTypeID int,
  @DefaultGroupTypeID int,
  @EnableCustomerProductAssociation bit,
  @EnableChat bit,
  @EnableCommunity bit,
  @EnableScreenRecording bit,
  @EnableVideoRecording bit,
  @DateModified datetime,
  @ModifierID int,
  @EnableTicketSeverity bit,
  @EnableTicketSeverityModification bit,
  @RestrictProductVersions bit,
  @EnableTicketNameModification bit,
  @KnowledgeBaseSortTypeID int,
  @CommunitySortTypeID int,
  @EnableAnonymousProductAssociation bit,
  @EnableCustomerSpecificKB bit,
  @EnableCustomFieldModification bit,
  @EnableProductFamilyFiltering bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CustomerHubFeatureSettings]
  SET
    [CustomerHubID] = @CustomerHubID,
    [EnableKnowledgeBase] = @EnableKnowledgeBase,
    [EnableProducts] = @EnableProducts,
    [EnableTicketCreation] = @EnableTicketCreation,
    [EnableMyTickets] = @EnableMyTickets,
    [EnableOrganizationTickets] = @EnableOrganizationTickets,
    [EnableWiki] = @EnableWiki,
    [EnableTicketGroupSelection] = @EnableTicketGroupSelection,
    [EnableTicketProductSelection] = @EnableTicketProductSelection,
    [EnableTicketProductVersionSelection] = @EnableTicketProductVersionSelection,
    [DefaultTicketTypeID] = @DefaultTicketTypeID,
    [DefaultGroupTypeID] = @DefaultGroupTypeID,
    [EnableCustomerProductAssociation] = @EnableCustomerProductAssociation,
    [EnableChat] = @EnableChat,
    [EnableCommunity] = @EnableCommunity,
    [EnableScreenRecording] = @EnableScreenRecording,
    [EnableVideoRecording] = @EnableVideoRecording,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [EnableTicketSeverity] = @EnableTicketSeverity,
    [EnableTicketSeverityModification] = @EnableTicketSeverityModification,
    [RestrictProductVersions] = @RestrictProductVersions,
    [EnableTicketNameModification] = @EnableTicketNameModification,
    [KnowledgeBaseSortTypeID] = @KnowledgeBaseSortTypeID,
    [CommunitySortTypeID] = @CommunitySortTypeID,
    [EnableAnonymousProductAssociation] = @EnableAnonymousProductAssociation,
    [EnableCustomerSpecificKB] = @EnableCustomerSpecificKB,
    [EnableCustomFieldModification] = @EnableCustomFieldModification,
    [EnableProductFamilyFiltering] = @EnableProductFamilyFiltering
  WHERE ([CustomerHubFeatureSettingID] = @CustomerHubFeatureSettingID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCustomerHubFeatureSetting' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCustomerHubFeatureSetting
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCustomerHubFeatureSetting

(
  @CustomerHubFeatureSettingID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CustomerHubFeatureSettings]
  WHERE ([CustomerHubFeatureSettingID] = @CustomerHubFeatureSettingID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCustomerHubFeatureSetting' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCustomerHubFeatureSetting
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCustomerHubFeatureSetting

(
  @CustomerHubFeatureSettingID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [CustomerHubFeatureSettingID],
    [CustomerHubID],
    [EnableKnowledgeBase],
    [EnableProducts],
    [EnableTicketCreation],
    [EnableMyTickets],
    [EnableOrganizationTickets],
    [EnableWiki],
    [EnableTicketGroupSelection],
    [EnableTicketProductSelection],
    [EnableTicketProductVersionSelection],
    [DefaultTicketTypeID],
    [DefaultGroupTypeID],
    [EnableCustomerProductAssociation],
    [EnableChat],
    [EnableCommunity],
    [EnableScreenRecording],
    [EnableVideoRecording],
    [DateModified],
    [ModifierID],
    [EnableTicketSeverity],
    [EnableTicketSeverityModification],
    [RestrictProductVersions],
    [EnableTicketNameModification],
    [KnowledgeBaseSortTypeID],
    [CommunitySortTypeID],
    [EnableAnonymousProductAssociation],
    [EnableCustomerSpecificKB],
    [EnableCustomFieldModification],
    [EnableProductFamilyFiltering]
  FROM [dbo].[CustomerHubFeatureSettings]
  WHERE ([CustomerHubFeatureSettingID] = @CustomerHubFeatureSettingID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCustomerHubFeatureSetting' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCustomerHubFeatureSetting
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCustomerHubFeatureSetting

(
  @CustomerHubID int,
  @EnableKnowledgeBase bit,
  @EnableProducts bit,
  @EnableTicketCreation bit,
  @EnableMyTickets bit,
  @EnableOrganizationTickets bit,
  @EnableWiki bit,
  @EnableTicketGroupSelection bit,
  @EnableTicketProductSelection bit,
  @EnableTicketProductVersionSelection bit,
  @DefaultTicketTypeID int,
  @DefaultGroupTypeID int,
  @EnableCustomerProductAssociation bit,
  @EnableChat bit,
  @EnableCommunity bit,
  @EnableScreenRecording bit,
  @EnableVideoRecording bit,
  @DateModified datetime,
  @ModifierID int,
  @EnableTicketSeverity bit,
  @EnableTicketSeverityModification bit,
  @RestrictProductVersions bit,
  @EnableTicketNameModification bit,
  @KnowledgeBaseSortTypeID int,
  @CommunitySortTypeID int,
  @EnableAnonymousProductAssociation bit,
  @EnableCustomerSpecificKB bit,
  @EnableCustomFieldModification bit,
  @EnableProductFamilyFiltering bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CustomerHubFeatureSettings]
  (
    [CustomerHubID],
    [EnableKnowledgeBase],
    [EnableProducts],
    [EnableTicketCreation],
    [EnableMyTickets],
    [EnableOrganizationTickets],
    [EnableWiki],
    [EnableTicketGroupSelection],
    [EnableTicketProductSelection],
    [EnableTicketProductVersionSelection],
    [DefaultTicketTypeID],
    [DefaultGroupTypeID],
    [EnableCustomerProductAssociation],
    [EnableChat],
    [EnableCommunity],
    [EnableScreenRecording],
    [EnableVideoRecording],
    [DateModified],
    [ModifierID],
    [EnableTicketSeverity],
    [EnableTicketSeverityModification],
    [RestrictProductVersions],
    [EnableTicketNameModification],
    [KnowledgeBaseSortTypeID],
    [CommunitySortTypeID],
    [EnableAnonymousProductAssociation],
    [EnableCustomerSpecificKB],
    [EnableCustomFieldModification],
    [EnableProductFamilyFiltering])
  VALUES (
    @CustomerHubID,
    @EnableKnowledgeBase,
    @EnableProducts,
    @EnableTicketCreation,
    @EnableMyTickets,
    @EnableOrganizationTickets,
    @EnableWiki,
    @EnableTicketGroupSelection,
    @EnableTicketProductSelection,
    @EnableTicketProductVersionSelection,
    @DefaultTicketTypeID,
    @DefaultGroupTypeID,
    @EnableCustomerProductAssociation,
    @EnableChat,
    @EnableCommunity,
    @EnableScreenRecording,
    @EnableVideoRecording,
    @DateModified,
    @ModifierID,
    @EnableTicketSeverity,
    @EnableTicketSeverityModification,
    @RestrictProductVersions,
    @EnableTicketNameModification,
    @KnowledgeBaseSortTypeID,
    @CommunitySortTypeID,
    @EnableAnonymousProductAssociation,
    @EnableCustomerSpecificKB,
    @EnableCustomFieldModification,
    @EnableProductFamilyFiltering)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCustomerHubFeatureSetting' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCustomerHubFeatureSetting
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCustomerHubFeatureSetting

(
  @CustomerHubFeatureSettingID int,
  @CustomerHubID int,
  @EnableKnowledgeBase bit,
  @EnableProducts bit,
  @EnableTicketCreation bit,
  @EnableMyTickets bit,
  @EnableOrganizationTickets bit,
  @EnableWiki bit,
  @EnableTicketGroupSelection bit,
  @EnableTicketProductSelection bit,
  @EnableTicketProductVersionSelection bit,
  @DefaultTicketTypeID int,
  @DefaultGroupTypeID int,
  @EnableCustomerProductAssociation bit,
  @EnableChat bit,
  @EnableCommunity bit,
  @EnableScreenRecording bit,
  @EnableVideoRecording bit,
  @DateModified datetime,
  @ModifierID int,
  @EnableTicketSeverity bit,
  @EnableTicketSeverityModification bit,
  @RestrictProductVersions bit,
  @EnableTicketNameModification bit,
  @KnowledgeBaseSortTypeID int,
  @CommunitySortTypeID int,
  @EnableAnonymousProductAssociation bit,
  @EnableCustomerSpecificKB bit,
  @EnableCustomFieldModification bit,
  @EnableProductFamilyFiltering bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CustomerHubFeatureSettings]
  SET
    [CustomerHubID] = @CustomerHubID,
    [EnableKnowledgeBase] = @EnableKnowledgeBase,
    [EnableProducts] = @EnableProducts,
    [EnableTicketCreation] = @EnableTicketCreation,
    [EnableMyTickets] = @EnableMyTickets,
    [EnableOrganizationTickets] = @EnableOrganizationTickets,
    [EnableWiki] = @EnableWiki,
    [EnableTicketGroupSelection] = @EnableTicketGroupSelection,
    [EnableTicketProductSelection] = @EnableTicketProductSelection,
    [EnableTicketProductVersionSelection] = @EnableTicketProductVersionSelection,
    [DefaultTicketTypeID] = @DefaultTicketTypeID,
    [DefaultGroupTypeID] = @DefaultGroupTypeID,
    [EnableCustomerProductAssociation] = @EnableCustomerProductAssociation,
    [EnableChat] = @EnableChat,
    [EnableCommunity] = @EnableCommunity,
    [EnableScreenRecording] = @EnableScreenRecording,
    [EnableVideoRecording] = @EnableVideoRecording,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [EnableTicketSeverity] = @EnableTicketSeverity,
    [EnableTicketSeverityModification] = @EnableTicketSeverityModification,
    [RestrictProductVersions] = @RestrictProductVersions,
    [EnableTicketNameModification] = @EnableTicketNameModification,
    [KnowledgeBaseSortTypeID] = @KnowledgeBaseSortTypeID,
    [CommunitySortTypeID] = @CommunitySortTypeID,
    [EnableAnonymousProductAssociation] = @EnableAnonymousProductAssociation,
    [EnableCustomerSpecificKB] = @EnableCustomerSpecificKB,
    [EnableCustomFieldModification] = @EnableCustomFieldModification,
    [EnableProductFamilyFiltering] = @EnableProductFamilyFiltering
  WHERE ([CustomerHubFeatureSettingID] = @CustomerHubFeatureSettingID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCustomerHubFeatureSetting' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCustomerHubFeatureSetting
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCustomerHubFeatureSetting

(
  @CustomerHubFeatureSettingID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CustomerHubFeatureSettings]
  WHERE ([CustomerHubFeatureSettingID] = @CustomerHubFeatureSettingID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCustomerHubFeatureSetting' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCustomerHubFeatureSetting
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCustomerHubFeatureSetting

(
  @CustomerHubFeatureSettingID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [CustomerHubFeatureSettingID],
    [CustomerHubID],
    [EnableKnowledgeBase],
    [EnableProducts],
    [EnableTicketCreation],
    [EnableMyTickets],
    [EnableOrganizationTickets],
    [EnableWiki],
    [EnableTicketGroupSelection],
    [EnableTicketProductSelection],
    [EnableTicketProductVersionSelection],
    [DefaultTicketTypeID],
    [DefaultGroupTypeID],
    [EnableCustomerProductAssociation],
    [EnableChat],
    [EnableCommunity],
    [EnableScreenRecording],
    [EnableVideoRecording],
    [DateModified],
    [ModifierID],
    [EnableTicketSeverity],
    [EnableTicketSeverityModification],
    [RestrictProductVersions],
    [EnableTicketNameModification],
    [KnowledgeBaseSortTypeID],
    [CommunitySortTypeID],
    [EnableAnonymousProductAssociation],
    [EnableCustomerSpecificKB],
    [EnableCustomFieldModification],
    [EnableProductFamilyFiltering]
  FROM [dbo].[CustomerHubFeatureSettings]
  WHERE ([CustomerHubFeatureSettingID] = @CustomerHubFeatureSettingID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCustomerHubFeatureSetting' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCustomerHubFeatureSetting
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCustomerHubFeatureSetting

(
  @CustomerHubID int,
  @EnableKnowledgeBase bit,
  @EnableProducts bit,
  @EnableTicketCreation bit,
  @EnableMyTickets bit,
  @EnableOrganizationTickets bit,
  @EnableWiki bit,
  @EnableTicketGroupSelection bit,
  @EnableTicketProductSelection bit,
  @EnableTicketProductVersionSelection bit,
  @DefaultTicketTypeID int,
  @DefaultGroupTypeID int,
  @EnableCustomerProductAssociation bit,
  @EnableChat bit,
  @EnableCommunity bit,
  @EnableScreenRecording bit,
  @EnableVideoRecording bit,
  @DateModified datetime,
  @ModifierID int,
  @EnableTicketSeverity bit,
  @EnableTicketSeverityModification bit,
  @RestrictProductVersions bit,
  @EnableTicketNameModification bit,
  @KnowledgeBaseSortTypeID int,
  @CommunitySortTypeID int,
  @EnableAnonymousProductAssociation bit,
  @EnableCustomerSpecificKB bit,
  @EnableCustomFieldModification bit,
  @EnableProductFamilyFiltering bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CustomerHubFeatureSettings]
  (
    [CustomerHubID],
    [EnableKnowledgeBase],
    [EnableProducts],
    [EnableTicketCreation],
    [EnableMyTickets],
    [EnableOrganizationTickets],
    [EnableWiki],
    [EnableTicketGroupSelection],
    [EnableTicketProductSelection],
    [EnableTicketProductVersionSelection],
    [DefaultTicketTypeID],
    [DefaultGroupTypeID],
    [EnableCustomerProductAssociation],
    [EnableChat],
    [EnableCommunity],
    [EnableScreenRecording],
    [EnableVideoRecording],
    [DateModified],
    [ModifierID],
    [EnableTicketSeverity],
    [EnableTicketSeverityModification],
    [RestrictProductVersions],
    [EnableTicketNameModification],
    [KnowledgeBaseSortTypeID],
    [CommunitySortTypeID],
    [EnableAnonymousProductAssociation],
    [EnableCustomerSpecificKB],
    [EnableCustomFieldModification],
    [EnableProductFamilyFiltering])
  VALUES (
    @CustomerHubID,
    @EnableKnowledgeBase,
    @EnableProducts,
    @EnableTicketCreation,
    @EnableMyTickets,
    @EnableOrganizationTickets,
    @EnableWiki,
    @EnableTicketGroupSelection,
    @EnableTicketProductSelection,
    @EnableTicketProductVersionSelection,
    @DefaultTicketTypeID,
    @DefaultGroupTypeID,
    @EnableCustomerProductAssociation,
    @EnableChat,
    @EnableCommunity,
    @EnableScreenRecording,
    @EnableVideoRecording,
    @DateModified,
    @ModifierID,
    @EnableTicketSeverity,
    @EnableTicketSeverityModification,
    @RestrictProductVersions,
    @EnableTicketNameModification,
    @KnowledgeBaseSortTypeID,
    @CommunitySortTypeID,
    @EnableAnonymousProductAssociation,
    @EnableCustomerSpecificKB,
    @EnableCustomFieldModification,
    @EnableProductFamilyFiltering)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCustomerHubFeatureSetting' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCustomerHubFeatureSetting
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCustomerHubFeatureSetting

(
  @CustomerHubFeatureSettingID int,
  @CustomerHubID int,
  @EnableKnowledgeBase bit,
  @EnableProducts bit,
  @EnableTicketCreation bit,
  @EnableMyTickets bit,
  @EnableOrganizationTickets bit,
  @EnableWiki bit,
  @EnableTicketGroupSelection bit,
  @EnableTicketProductSelection bit,
  @EnableTicketProductVersionSelection bit,
  @DefaultTicketTypeID int,
  @DefaultGroupTypeID int,
  @EnableCustomerProductAssociation bit,
  @EnableChat bit,
  @EnableCommunity bit,
  @EnableScreenRecording bit,
  @EnableVideoRecording bit,
  @DateModified datetime,
  @ModifierID int,
  @EnableTicketSeverity bit,
  @EnableTicketSeverityModification bit,
  @RestrictProductVersions bit,
  @EnableTicketNameModification bit,
  @KnowledgeBaseSortTypeID int,
  @CommunitySortTypeID int,
  @EnableAnonymousProductAssociation bit,
  @EnableCustomerSpecificKB bit,
  @EnableCustomFieldModification bit,
  @EnableProductFamilyFiltering bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CustomerHubFeatureSettings]
  SET
    [CustomerHubID] = @CustomerHubID,
    [EnableKnowledgeBase] = @EnableKnowledgeBase,
    [EnableProducts] = @EnableProducts,
    [EnableTicketCreation] = @EnableTicketCreation,
    [EnableMyTickets] = @EnableMyTickets,
    [EnableOrganizationTickets] = @EnableOrganizationTickets,
    [EnableWiki] = @EnableWiki,
    [EnableTicketGroupSelection] = @EnableTicketGroupSelection,
    [EnableTicketProductSelection] = @EnableTicketProductSelection,
    [EnableTicketProductVersionSelection] = @EnableTicketProductVersionSelection,
    [DefaultTicketTypeID] = @DefaultTicketTypeID,
    [DefaultGroupTypeID] = @DefaultGroupTypeID,
    [EnableCustomerProductAssociation] = @EnableCustomerProductAssociation,
    [EnableChat] = @EnableChat,
    [EnableCommunity] = @EnableCommunity,
    [EnableScreenRecording] = @EnableScreenRecording,
    [EnableVideoRecording] = @EnableVideoRecording,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [EnableTicketSeverity] = @EnableTicketSeverity,
    [EnableTicketSeverityModification] = @EnableTicketSeverityModification,
    [RestrictProductVersions] = @RestrictProductVersions,
    [EnableTicketNameModification] = @EnableTicketNameModification,
    [KnowledgeBaseSortTypeID] = @KnowledgeBaseSortTypeID,
    [CommunitySortTypeID] = @CommunitySortTypeID,
    [EnableAnonymousProductAssociation] = @EnableAnonymousProductAssociation,
    [EnableCustomerSpecificKB] = @EnableCustomerSpecificKB,
    [EnableCustomFieldModification] = @EnableCustomFieldModification,
    [EnableProductFamilyFiltering] = @EnableProductFamilyFiltering
  WHERE ([CustomerHubFeatureSettingID] = @CustomerHubFeatureSettingID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCustomerHubFeatureSetting' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCustomerHubFeatureSetting
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCustomerHubFeatureSetting

(
  @CustomerHubFeatureSettingID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CustomerHubFeatureSettings]
  WHERE ([CustomerHubFeatureSettingID] = @CustomerHubFeatureSettingID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCustomerHubFeatureSetting' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCustomerHubFeatureSetting
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCustomerHubFeatureSetting

(
  @CustomerHubFeatureSettingID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [CustomerHubFeatureSettingID],
    [CustomerHubID],
    [EnableKnowledgeBase],
    [EnableProducts],
    [EnableTicketCreation],
    [EnableMyTickets],
    [EnableOrganizationTickets],
    [EnableWiki],
    [EnableTicketGroupSelection],
    [EnableTicketProductSelection],
    [EnableTicketProductVersionSelection],
    [DefaultTicketTypeID],
    [DefaultGroupTypeID],
    [EnableCustomerProductAssociation],
    [EnableChat],
    [EnableCommunity],
    [EnableScreenRecording],
    [EnableVideoRecording],
    [DateModified],
    [ModifierID],
    [EnableTicketSeverity],
    [EnableTicketSeverityModification],
    [RestrictProductVersions],
    [EnableTicketNameModification],
    [KnowledgeBaseSortTypeID],
    [CommunitySortTypeID],
    [EnableAnonymousProductAssociation],
    [EnableCustomerSpecificKB],
    [EnableCustomFieldModification],
    [EnableProductFamilyFiltering]
  FROM [dbo].[CustomerHubFeatureSettings]
  WHERE ([CustomerHubFeatureSettingID] = @CustomerHubFeatureSettingID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCustomerHubFeatureSetting' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCustomerHubFeatureSetting
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCustomerHubFeatureSetting

(
  @CustomerHubID int,
  @EnableKnowledgeBase bit,
  @EnableProducts bit,
  @EnableTicketCreation bit,
  @EnableMyTickets bit,
  @EnableOrganizationTickets bit,
  @EnableWiki bit,
  @EnableTicketGroupSelection bit,
  @EnableTicketProductSelection bit,
  @EnableTicketProductVersionSelection bit,
  @DefaultTicketTypeID int,
  @DefaultGroupTypeID int,
  @EnableCustomerProductAssociation bit,
  @EnableChat bit,
  @EnableCommunity bit,
  @EnableScreenRecording bit,
  @EnableVideoRecording bit,
  @DateModified datetime,
  @ModifierID int,
  @EnableTicketSeverity bit,
  @EnableTicketSeverityModification bit,
  @RestrictProductVersions bit,
  @EnableTicketNameModification bit,
  @KnowledgeBaseSortTypeID int,
  @CommunitySortTypeID int,
  @EnableAnonymousProductAssociation bit,
  @EnableCustomerSpecificKB bit,
  @EnableCustomFieldModification bit,
  @EnableProductFamilyFiltering bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CustomerHubFeatureSettings]
  (
    [CustomerHubID],
    [EnableKnowledgeBase],
    [EnableProducts],
    [EnableTicketCreation],
    [EnableMyTickets],
    [EnableOrganizationTickets],
    [EnableWiki],
    [EnableTicketGroupSelection],
    [EnableTicketProductSelection],
    [EnableTicketProductVersionSelection],
    [DefaultTicketTypeID],
    [DefaultGroupTypeID],
    [EnableCustomerProductAssociation],
    [EnableChat],
    [EnableCommunity],
    [EnableScreenRecording],
    [EnableVideoRecording],
    [DateModified],
    [ModifierID],
    [EnableTicketSeverity],
    [EnableTicketSeverityModification],
    [RestrictProductVersions],
    [EnableTicketNameModification],
    [KnowledgeBaseSortTypeID],
    [CommunitySortTypeID],
    [EnableAnonymousProductAssociation],
    [EnableCustomerSpecificKB],
    [EnableCustomFieldModification],
    [EnableProductFamilyFiltering])
  VALUES (
    @CustomerHubID,
    @EnableKnowledgeBase,
    @EnableProducts,
    @EnableTicketCreation,
    @EnableMyTickets,
    @EnableOrganizationTickets,
    @EnableWiki,
    @EnableTicketGroupSelection,
    @EnableTicketProductSelection,
    @EnableTicketProductVersionSelection,
    @DefaultTicketTypeID,
    @DefaultGroupTypeID,
    @EnableCustomerProductAssociation,
    @EnableChat,
    @EnableCommunity,
    @EnableScreenRecording,
    @EnableVideoRecording,
    @DateModified,
    @ModifierID,
    @EnableTicketSeverity,
    @EnableTicketSeverityModification,
    @RestrictProductVersions,
    @EnableTicketNameModification,
    @KnowledgeBaseSortTypeID,
    @CommunitySortTypeID,
    @EnableAnonymousProductAssociation,
    @EnableCustomerSpecificKB,
    @EnableCustomFieldModification,
    @EnableProductFamilyFiltering)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCustomerHubFeatureSetting' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCustomerHubFeatureSetting
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCustomerHubFeatureSetting

(
  @CustomerHubFeatureSettingID int,
  @CustomerHubID int,
  @EnableKnowledgeBase bit,
  @EnableProducts bit,
  @EnableTicketCreation bit,
  @EnableMyTickets bit,
  @EnableOrganizationTickets bit,
  @EnableWiki bit,
  @EnableTicketGroupSelection bit,
  @EnableTicketProductSelection bit,
  @EnableTicketProductVersionSelection bit,
  @DefaultTicketTypeID int,
  @DefaultGroupTypeID int,
  @EnableCustomerProductAssociation bit,
  @EnableChat bit,
  @EnableCommunity bit,
  @EnableScreenRecording bit,
  @EnableVideoRecording bit,
  @DateModified datetime,
  @ModifierID int,
  @EnableTicketSeverity bit,
  @EnableTicketSeverityModification bit,
  @RestrictProductVersions bit,
  @EnableTicketNameModification bit,
  @KnowledgeBaseSortTypeID int,
  @CommunitySortTypeID int,
  @EnableAnonymousProductAssociation bit,
  @EnableCustomerSpecificKB bit,
  @EnableCustomFieldModification bit,
  @EnableProductFamilyFiltering bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CustomerHubFeatureSettings]
  SET
    [CustomerHubID] = @CustomerHubID,
    [EnableKnowledgeBase] = @EnableKnowledgeBase,
    [EnableProducts] = @EnableProducts,
    [EnableTicketCreation] = @EnableTicketCreation,
    [EnableMyTickets] = @EnableMyTickets,
    [EnableOrganizationTickets] = @EnableOrganizationTickets,
    [EnableWiki] = @EnableWiki,
    [EnableTicketGroupSelection] = @EnableTicketGroupSelection,
    [EnableTicketProductSelection] = @EnableTicketProductSelection,
    [EnableTicketProductVersionSelection] = @EnableTicketProductVersionSelection,
    [DefaultTicketTypeID] = @DefaultTicketTypeID,
    [DefaultGroupTypeID] = @DefaultGroupTypeID,
    [EnableCustomerProductAssociation] = @EnableCustomerProductAssociation,
    [EnableChat] = @EnableChat,
    [EnableCommunity] = @EnableCommunity,
    [EnableScreenRecording] = @EnableScreenRecording,
    [EnableVideoRecording] = @EnableVideoRecording,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [EnableTicketSeverity] = @EnableTicketSeverity,
    [EnableTicketSeverityModification] = @EnableTicketSeverityModification,
    [RestrictProductVersions] = @RestrictProductVersions,
    [EnableTicketNameModification] = @EnableTicketNameModification,
    [KnowledgeBaseSortTypeID] = @KnowledgeBaseSortTypeID,
    [CommunitySortTypeID] = @CommunitySortTypeID,
    [EnableAnonymousProductAssociation] = @EnableAnonymousProductAssociation,
    [EnableCustomerSpecificKB] = @EnableCustomerSpecificKB,
    [EnableCustomFieldModification] = @EnableCustomFieldModification,
    [EnableProductFamilyFiltering] = @EnableProductFamilyFiltering
  WHERE ([CustomerHubFeatureSettingID] = @CustomerHubFeatureSettingID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCustomerHubFeatureSetting' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCustomerHubFeatureSetting
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCustomerHubFeatureSetting

(
  @CustomerHubFeatureSettingID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CustomerHubFeatureSettings]
  WHERE ([CustomerHubFeatureSettingID] = @CustomerHubFeatureSettingID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCustomerHubFeatureSetting' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCustomerHubFeatureSetting
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCustomerHubFeatureSetting

(
  @CustomerHubFeatureSettingID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [CustomerHubFeatureSettingID],
    [CustomerHubID],
    [EnableKnowledgeBase],
    [EnableProducts],
    [EnableTicketCreation],
    [EnableMyTickets],
    [EnableOrganizationTickets],
    [EnableWiki],
    [EnableTicketGroupSelection],
    [EnableTicketProductSelection],
    [EnableTicketProductVersionSelection],
    [DefaultTicketTypeID],
    [DefaultGroupTypeID],
    [EnableCustomerProductAssociation],
    [EnableChat],
    [EnableCommunity],
    [EnableScreenRecording],
    [EnableVideoRecording],
    [DateModified],
    [ModifierID],
    [EnableTicketSeverity],
    [EnableTicketSeverityModification],
    [RestrictProductVersions],
    [EnableTicketNameModification],
    [KnowledgeBaseSortTypeID],
    [CommunitySortTypeID],
    [EnableAnonymousProductAssociation],
    [EnableCustomerSpecificKB],
    [EnableCustomFieldModification],
    [EnableProductFamilyFiltering]
  FROM [dbo].[CustomerHubFeatureSettings]
  WHERE ([CustomerHubFeatureSettingID] = @CustomerHubFeatureSettingID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCustomerHubFeatureSetting' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCustomerHubFeatureSetting
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCustomerHubFeatureSetting

(
  @CustomerHubID int,
  @EnableKnowledgeBase bit,
  @EnableProducts bit,
  @EnableTicketCreation bit,
  @EnableMyTickets bit,
  @EnableOrganizationTickets bit,
  @EnableWiki bit,
  @EnableTicketGroupSelection bit,
  @EnableTicketProductSelection bit,
  @EnableTicketProductVersionSelection bit,
  @DefaultTicketTypeID int,
  @DefaultGroupTypeID int,
  @EnableCustomerProductAssociation bit,
  @EnableChat bit,
  @EnableCommunity bit,
  @EnableScreenRecording bit,
  @EnableVideoRecording bit,
  @DateModified datetime,
  @ModifierID int,
  @EnableTicketSeverity bit,
  @EnableTicketSeverityModification bit,
  @RestrictProductVersions bit,
  @EnableTicketNameModification bit,
  @KnowledgeBaseSortTypeID int,
  @CommunitySortTypeID int,
  @EnableAnonymousProductAssociation bit,
  @EnableCustomerSpecificKB bit,
  @EnableCustomFieldModification bit,
  @EnableProductFamilyFiltering bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CustomerHubFeatureSettings]
  (
    [CustomerHubID],
    [EnableKnowledgeBase],
    [EnableProducts],
    [EnableTicketCreation],
    [EnableMyTickets],
    [EnableOrganizationTickets],
    [EnableWiki],
    [EnableTicketGroupSelection],
    [EnableTicketProductSelection],
    [EnableTicketProductVersionSelection],
    [DefaultTicketTypeID],
    [DefaultGroupTypeID],
    [EnableCustomerProductAssociation],
    [EnableChat],
    [EnableCommunity],
    [EnableScreenRecording],
    [EnableVideoRecording],
    [DateModified],
    [ModifierID],
    [EnableTicketSeverity],
    [EnableTicketSeverityModification],
    [RestrictProductVersions],
    [EnableTicketNameModification],
    [KnowledgeBaseSortTypeID],
    [CommunitySortTypeID],
    [EnableAnonymousProductAssociation],
    [EnableCustomerSpecificKB],
    [EnableCustomFieldModification],
    [EnableProductFamilyFiltering])
  VALUES (
    @CustomerHubID,
    @EnableKnowledgeBase,
    @EnableProducts,
    @EnableTicketCreation,
    @EnableMyTickets,
    @EnableOrganizationTickets,
    @EnableWiki,
    @EnableTicketGroupSelection,
    @EnableTicketProductSelection,
    @EnableTicketProductVersionSelection,
    @DefaultTicketTypeID,
    @DefaultGroupTypeID,
    @EnableCustomerProductAssociation,
    @EnableChat,
    @EnableCommunity,
    @EnableScreenRecording,
    @EnableVideoRecording,
    @DateModified,
    @ModifierID,
    @EnableTicketSeverity,
    @EnableTicketSeverityModification,
    @RestrictProductVersions,
    @EnableTicketNameModification,
    @KnowledgeBaseSortTypeID,
    @CommunitySortTypeID,
    @EnableAnonymousProductAssociation,
    @EnableCustomerSpecificKB,
    @EnableCustomFieldModification,
    @EnableProductFamilyFiltering)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCustomerHubFeatureSetting' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCustomerHubFeatureSetting
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCustomerHubFeatureSetting

(
  @CustomerHubFeatureSettingID int,
  @CustomerHubID int,
  @EnableKnowledgeBase bit,
  @EnableProducts bit,
  @EnableTicketCreation bit,
  @EnableMyTickets bit,
  @EnableOrganizationTickets bit,
  @EnableWiki bit,
  @EnableTicketGroupSelection bit,
  @EnableTicketProductSelection bit,
  @EnableTicketProductVersionSelection bit,
  @DefaultTicketTypeID int,
  @DefaultGroupTypeID int,
  @EnableCustomerProductAssociation bit,
  @EnableChat bit,
  @EnableCommunity bit,
  @EnableScreenRecording bit,
  @EnableVideoRecording bit,
  @DateModified datetime,
  @ModifierID int,
  @EnableTicketSeverity bit,
  @EnableTicketSeverityModification bit,
  @RestrictProductVersions bit,
  @EnableTicketNameModification bit,
  @KnowledgeBaseSortTypeID int,
  @CommunitySortTypeID int,
  @EnableAnonymousProductAssociation bit,
  @EnableCustomerSpecificKB bit,
  @EnableCustomFieldModification bit,
  @EnableProductFamilyFiltering bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CustomerHubFeatureSettings]
  SET
    [CustomerHubID] = @CustomerHubID,
    [EnableKnowledgeBase] = @EnableKnowledgeBase,
    [EnableProducts] = @EnableProducts,
    [EnableTicketCreation] = @EnableTicketCreation,
    [EnableMyTickets] = @EnableMyTickets,
    [EnableOrganizationTickets] = @EnableOrganizationTickets,
    [EnableWiki] = @EnableWiki,
    [EnableTicketGroupSelection] = @EnableTicketGroupSelection,
    [EnableTicketProductSelection] = @EnableTicketProductSelection,
    [EnableTicketProductVersionSelection] = @EnableTicketProductVersionSelection,
    [DefaultTicketTypeID] = @DefaultTicketTypeID,
    [DefaultGroupTypeID] = @DefaultGroupTypeID,
    [EnableCustomerProductAssociation] = @EnableCustomerProductAssociation,
    [EnableChat] = @EnableChat,
    [EnableCommunity] = @EnableCommunity,
    [EnableScreenRecording] = @EnableScreenRecording,
    [EnableVideoRecording] = @EnableVideoRecording,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [EnableTicketSeverity] = @EnableTicketSeverity,
    [EnableTicketSeverityModification] = @EnableTicketSeverityModification,
    [RestrictProductVersions] = @RestrictProductVersions,
    [EnableTicketNameModification] = @EnableTicketNameModification,
    [KnowledgeBaseSortTypeID] = @KnowledgeBaseSortTypeID,
    [CommunitySortTypeID] = @CommunitySortTypeID,
    [EnableAnonymousProductAssociation] = @EnableAnonymousProductAssociation,
    [EnableCustomerSpecificKB] = @EnableCustomerSpecificKB,
    [EnableCustomFieldModification] = @EnableCustomFieldModification,
    [EnableProductFamilyFiltering] = @EnableProductFamilyFiltering
  WHERE ([CustomerHubFeatureSettingID] = @CustomerHubFeatureSettingID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCustomerHubFeatureSetting' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCustomerHubFeatureSetting
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCustomerHubFeatureSetting

(
  @CustomerHubFeatureSettingID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CustomerHubFeatureSettings]
  WHERE ([CustomerHubFeatureSettingID] = @CustomerHubFeatureSettingID)
GO


