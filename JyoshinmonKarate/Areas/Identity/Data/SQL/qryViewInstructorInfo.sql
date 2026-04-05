DECLARE @instructorID AS INT = 0  -- 0 shows all instructors and you can also enter a specific InstructorId to view one instructor

SELECT
    Instructors.InstructorId AS [Instructor ID],
    AspNetUsers.FirstName + ' ' + AspNetUsers.LastName AS [Instructor Name],
    Clubs.ClubName AS [Club],
    Belts.BeltName AS [Belt],
    Instructors.DateJoined AS [Date Joined]
FROM
    Instructors, AspNetUsers, Clubs, Belts
WHERE
    Instructors.UserId = AspNetUsers.Id
    AND Instructors.ClubId = Clubs.ClubId
    AND Instructors.BeltId = Belts.BeltId
    AND (@instructorID = 0 OR Instructors.InstructorId = @instructorID)
ORDER BY
    Clubs.ClubName,
    AspNetUsers.LastName;