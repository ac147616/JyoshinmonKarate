DECLARE @memberID AS INT = 0 -- 0 shows all attendance records and you can give it a specific MemberId so it shows attendance records for that member only

SELECT Members.MemberId AS [Member ID]
	,Members.FirstName + ' ' + Members.LastName AS [Member Name]
	,Clubs.ClubName AS [Club]
	,Attendances.DATE AS [Attendance Date]
	,Schedules.DayOfWeek AS [Class Day]
	,Schedules.LEVEL AS [Class Level]
	,Schedules.StartTime AS [Start Time]
	,Schedules.EndTime AS [End Time]
FROM Attendances, Members, Schedules, Clubs
WHERE Attendances.MemberId = Members.MemberId
	AND Attendances.ScheduleId = Schedules.ScheduleId
	AND Members.ClubId = Clubs.ClubId
	AND (@memberID = 0 OR Members.MemberId = @memberID)
ORDER BY Members.LastName
	,Members.FirstName
	,Attendances.DATE;
