SELECT
    Clubs.ClubName AS [Club],
    COUNT(Members.MemberId) AS [Total Members]
FROM
    Clubs, Members
WHERE
    Clubs.ClubId = Members.ClubId
GROUP BY
    Clubs.ClubName
ORDER BY
    COUNT(Members.MemberId) DESC,
    Clubs.ClubName;