SELECT
    Instructors.InstructorId AS [Instructor ID],
    AspNetUsers.FirstName + ' ' + AspNetUsers.LastName AS [Instructor Name],
    Clubs.ClubName AS [Club],
    Schedules.Level AS [Class Level],
    Schedules.DayOfWeek AS [Class Day],
    Schedules.StartTime AS [Start Time],
    Schedules.EndTime AS [End Time]
FROM
    Instructors, AspNetUsers, Schedules, Clubs
WHERE
    Instructors.UserId = AspNetUsers.Id
    AND Schedules.InstructorId = Instructors.InstructorId
    AND Schedules.ClubId = Clubs.ClubId
ORDER BY
    AspNetUsers.LastName,
    AspNetUsers.FirstName,
    Schedules.DayOfWeek,
    Schedules.StartTime;