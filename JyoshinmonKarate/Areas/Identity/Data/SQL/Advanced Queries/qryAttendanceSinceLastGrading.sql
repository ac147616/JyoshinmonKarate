DECLARE @memberID AS INT = 3 -- Change this to the MemberId you want to check

SELECT Members.MemberId AS [Member ID]
	,Members.FirstName + ' ' + Members.LastName AS [Member Name]
	,Attendances.DATE AS [Attendance Date]
	,Schedules.DayOfWeek AS [Class Day]
	,Schedules.LEVEL AS [Class Level]
	,Schedules.StartTime AS [Start Time]
	,Schedules.EndTime AS [End Time]
FROM Attendances, Members, Schedules
WHERE Attendances.MemberId = Members.MemberId
	AND Attendances.ScheduleId = Schedules.ScheduleId
	AND Members.MemberId = @memberID
	AND Attendances.DATE > (
		SELECT MAX(Gradings.GradingDate)
		FROM MemberGradings
			,Gradings
		WHERE MemberGradings.GradingId = Gradings.GradingId AND MemberGradings.MemberId = @memberID)
ORDER BY Attendances.DATE;
