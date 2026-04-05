DECLARE @userID AS NVARCHAR(450) = '' -- Leave this as '' to show all users or you can put a specific user ID to show only one user.

SELECT AspNetUsers.Id AS [User ID]
	,AspNetUsers.UserName AS [Username]
	,AspNetUsers.FirstName AS [First Name]
	,AspNetUsers.LastName AS [Last Name]
	,AspNetUsers.Email AS [Email]
	,AspNetUsers.PhoneNumber AS [Phone Number]
	,AspNetUsers.IsAdmin AS [Admin User]
FROM AspNetUsers
WHERE @userID = ''
	OR AspNetUsers.Id = @userID
ORDER BY AspNetUsers.FirstName
	,AspNetUsers.LastName;