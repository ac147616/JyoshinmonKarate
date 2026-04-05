DECLARE @memberID AS INT = 0  -- 0 shows all payment records and you cna also give it a specific MemberId then it will show only payment records for that member

SELECT
    Members.MemberId AS [Member ID],
    Members.FirstName + ' ' + Members.LastName AS [Member Name],
    Payments.PaymentName AS [Payment Name],
    Payments.Amount AS [Amount],
    Payments.DateDue AS [Due Date],
    Payments.PaymentMethod AS [Payment Method],
    Payments.Status AS [Payment Status]
FROM
    Payments, Members
WHERE
    Payments.MemberId = Members.MemberId
    AND (@memberID = 0 OR Members.MemberId = @memberID)
ORDER BY
    Members.LastName,
    Members.FirstName,
    Payments.DateDue;   