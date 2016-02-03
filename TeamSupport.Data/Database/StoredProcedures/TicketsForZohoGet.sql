SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
IF OBJECT_ID('dbo.TicketsForZohoGet','P') IS NOT NULL
	DROP PROC dbo.TicketsForZohoGet;
GO
 
CREATE PROC [dbo].[TicketsForZohoGet]
	@OrganizationID		int,
	@LastMod			datetime,
	@Url						varchar(200) = NULL
AS
BEGIN
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	SET NOCOUNT ON;
 
	IF (@Url IS NULL)
		SET @Url = 'https://app.teamsupport.com'
		
	SET @Url = @Url + '?ticketid=';
 
	WITH TicketIDs AS (
		SELECT
			t.TicketID
		FROM
			Tickets t
		WHERE
			OrganizationID = @OrganizationID
			AND DateModified > @LastMod
	)
 
	SELECT
		TicketNumber,
		@Url + CAST(tv.TicketID as varchar(30)) as [TicketUrl],
		Name,
		TicketTypeName,
		TicketSource,
		[Status],
		Severity,
		UserName AS [AssignedTo],
		Customers,
		Contacts,
		ProductName,
		ReportedVersion,
		SolvedVersion,
		GroupName,
		DateModified,
		DateCreated,
		DaysOpened,
		IsClosed,
		CloserName,
		SlaViolationTime,
		StatusPosition,
		SeverityPosition,
		IsVisibleOnPortal,
		IsKnowledgeBase,
		DateClosed,
		DaysClosed,
		CreatorName,
		ModifierName,
		HoursSpent,
		Tags,
		SlaWarningTime,
		SlaViolationHours,
		SlaWarningHours,
		MinsSinceCreated,
		DaysSinceCreated,
		DATEDIFF(mi, '1900-01-01', 
			(
				SELECT TOP 1
					timeinoldstatus
				FROM
					statushistory
				WHERE
					ticketid = tv.ticketid
					AND ISNULL(timeinoldstatus, -99) <> -99
				ORDER BY
					statuschangetime
				)
		) AS MinutesToFirstResponse
	FROM
		dbo.TicketsView tv
		INNER JOIN TicketIDs ON TicketIDs.TicketID = tv.TicketID
	ORDER BY
		TicketNumber;
 
	--OPTION (MAXDOP 1);
END