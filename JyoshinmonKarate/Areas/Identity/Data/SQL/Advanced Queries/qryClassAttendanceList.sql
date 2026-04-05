DECLARE @scheduleID AS INT = 4 -- Change this to the ScheduleId you want to check

SELECT Schedules.ScheduleId AS [Schedule ID]
	,Clubs.ClubName AS [Club]
	,Schedules.DayOfWeek AS [Class Day]
	,Schedules.LEVEL AS [Class Level]
	,Schedules.StartTime AS [Start Time]
	,Schedules.EndTime AS [End Time]
	,Members.FirstName + ' ' + Members.LastName AS [Member Name]
	,Attendances.DATE AS [Attendance Date]
FROM Attendances, Members, Schedules, Clubs
WHERE Attendances.MemberId = Members.MemberId
	AND Attendances.ScheduleId = Schedules.ScheduleId
	AND Schedules.ClubId = Clubs.ClubId
	AND Schedules.ScheduleId = @scheduleID
ORDER BY Attendances.DATE
	,Members.LastName
	,Members.FirstName;
