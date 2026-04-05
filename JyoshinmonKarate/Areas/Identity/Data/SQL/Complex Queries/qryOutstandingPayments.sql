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
    AND (Payments.Status = 0 OR Payments.Status = 2)
ORDER BY
    Payments.DateDue,
    Members.LastName,
    Members.FirstName;