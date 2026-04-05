DECLARE @minimumAttendance AS INT = 1   -- Change this number if you want a different attendance requirement

SELECT
    Members.MemberId AS [Member ID],
    Members.FirstName + ' ' + Members.LastName AS [Member Name],
    COUNT(Attendances.AttendanceId) AS [Attendance Since Last Grading]
FROM
    Members, Attendances
WHERE
    Members.MemberId = Attendances.MemberId
    AND Attendances.Date >
    (
        SELECT MAX(Gradings.GradingDate)
        FROM MemberGradings, Gradings
        WHERE MemberGradings.GradingId = Gradings.GradingId AND MemberGradings.MemberId = Members.MemberId
    )
GROUP BY
    Members.MemberId,
    Members.FirstName,
    Members.LastName
HAVING
    COUNT(Attendances.AttendanceId) >= @minimumAttendance
ORDER BY
    [Attendance Since Last Grading] DESC,
    Members.LastName,
    Members.FirstName;