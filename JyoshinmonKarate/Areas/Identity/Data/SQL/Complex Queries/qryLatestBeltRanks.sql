SELECT
    Members.MemberId AS [Member ID],
    Members.FirstName + ' ' + Members.LastName AS [Member Name],
    Clubs.ClubName AS [Club],
    Belts.BeltName AS [Current Belt],
    Members.Status AS [Member Status]
FROM
    Members, Belts, Clubs
WHERE
    Members.BeltId = Belts.BeltId
    AND Members.ClubId = Clubs.ClubId
ORDER BY
    Clubs.ClubName,
    Belts.BeltName,
    Members.LastName,
    Members.FirstName;