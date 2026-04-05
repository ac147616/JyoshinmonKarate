DECLARE @clubID AS INT = 0 -- 0 shows all class schedules and you can also give it a specific ClubId and it will show only schedules for that club

SELECT Schedules.ScheduleId AS [Schedule ID]
	,Clubs.ClubName AS [Club]
	,AspNetUsers.FirstName + ' ' + AspNetUsers.LastName AS [Instructor Name]
	,Schedules.LEVEL AS [Level]
	,Schedules.DayOfWeek AS [Day of Week]
	,Schedules.StartTime AS [Start Time]
	,Schedules.EndTime AS [End Time]
FROM Schedules, Clubs, Instructors, AspNetUsers
WHERE Schedules.ClubId = Clubs.ClubId
	AND Schedules.InstructorId = Instructors.InstructorId
	AND Instructors.UserId = AspNetUsers.Id
	AND (@clubID = 0 OR Schedules.ClubId = @clubID)
ORDER BY Clubs.ClubName
	,Schedules.DayOfWeek
	,Schedules.StartTime;