DECLARE @clubID AS INT = 0  -- 0 shows all clubs and you can also enter a specific ClubId to view one club

SELECT
    Clubs.ClubName AS [Club Name],
    Clubs.Address AS [Address],
    Clubs.Email AS [Email],
    Clubs.Phone AS [Phone],
    COUNT(DISTINCT Members.MemberId) AS [Total Members],
    COUNT(DISTINCT Instructors.InstructorId) AS [Total Instructors]
FROM
    Clubs, Members, Instructors
WHERE
    Clubs.ClubId = Members.ClubId
    AND Clubs.ClubId = Instructors.ClubId
    AND (@clubID = 0 OR Clubs.ClubId = @clubID)
GROUP BY
    Clubs.ClubName,
    Clubs.Address,
    Clubs.Email,
    Clubs.Phone
ORDER BY
    Clubs.ClubName;