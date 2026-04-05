DECLARE @memberID AS INT = 0  -- 0 shows all grading records and you can also give it a specific MemberId then it will show only grading records for that member

SELECT
    Members.MemberId AS [Member ID],
    Members.FirstName + ' ' + Members.LastName AS [Member Name],
    Gradings.GradingDate AS [Grading Date],
    BeltBefore.BeltName AS [Belt Before],
    BeltAfter.BeltName AS [Belt After],
    MemberGradings.Passed AS [Passed]
FROM
    MemberGradings, Members, Gradings, Belts BeltBefore, Belts BeltAfter
WHERE
    MemberGradings.MemberId = Members.MemberId
    AND MemberGradings.GradingId = Gradings.GradingId
    AND MemberGradings.BeltBeforeId = BeltBefore.BeltId
    AND MemberGradings.BeltAfterId = BeltAfter.BeltId
    AND (@memberID = 0 OR Members.MemberId = @memberID)
ORDER BY
    Members.LastName,
    Members.FirstName,
    Gradings.GradingDate;