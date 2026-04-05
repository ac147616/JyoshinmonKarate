DECLARE @memberID AS INT = 0 -- 0 shows all members and you can also give it a specific MemberId then it will show only that member's record

SELECT Members.MemberId AS [Member ID]
	,Members.FirstName + ' ' + Members.LastName AS [Member Name]
	,Clubs.ClubName AS [Club]
	,Memberships.MembershipName AS [Membership Plan]
	,Belts.BeltName AS [Current Belt]
	,Members.STATUS AS [Member Status]
	,Members.DateJoined AS [Date Joined]
FROM Members
	,Clubs
	,Memberships
	,Belts
WHERE Members.ClubId = Clubs.ClubId
	AND Members.MembershipId = Memberships.MembershipId
	AND Members.BeltId = Belts.BeltId
	AND (
		@memberID = 0
		OR Members.MemberId = @memberID
		)
ORDER BY Clubs.ClubName
	,Members.LastName
	,Members.FirstName;