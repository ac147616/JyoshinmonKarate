using JyoshinmonKarate.Areas.Identity.Data;
using JyoshinmonKarate.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Collections.Generic;

namespace JyoshinmonKarate.Data
{
    public static class DbInitializer
    {
        public static void Initialize(JyoshinmonKarateContext context, UserManager<User> userManager)
        {
            // If already seeded, stop
            if (context.Clubs.Any())
                return;

            // -------------------------
            // CLUBS (5)
            // -------------------------
            var clubs = new List<Club>
            {
                new Club { ClubName="Central Dojo", Address="Auckland CBD", Email="central@dojo.com", Phone="0211111111" },
                new Club { ClubName="West Dojo", Address="West Auckland", Email="west@dojo.com", Phone="0212222222" },
                new Club { ClubName="North Dojo", Address="North Shore", Email="north@dojo.com", Phone="0213333333" },
                new Club { ClubName="South Dojo", Address="South Auckland", Email="south@dojo.com", Phone="0214444444" },
                new Club { ClubName="East Dojo", Address="East Auckland", Email="east@dojo.com", Phone="0215555555" }
            };
            context.Clubs.AddRange(clubs);
            context.SaveChanges();

            // -------------------------
            // BELTS (~10)
            // -------------------------
            var belts = new List<Belt>
            {
                new Belt{BeltName="White"},
                new Belt{BeltName="Yellow"},
                new Belt{BeltName="Orange"},
                new Belt{BeltName="Green"},
                new Belt{BeltName="Blue"},
                new Belt{BeltName="Purple"},
                new Belt{BeltName="Brown"},
                new Belt{BeltName="Red"},
                new Belt{BeltName="Black"},
                new Belt{BeltName="Black 1st Dan"}
            };
            context.Belts.AddRange(belts);
            context.SaveChanges();

            // -------------------------
            // MEMBERSHIPS (4)
            // -------------------------
            var memberships = new List<Membership>
            {
                new Membership{MembershipName="Junior", Cost=40, AgeGroup="Under 13"},
                new Membership{MembershipName="Teen", Cost=50, AgeGroup="13-17"},
                new Membership{MembershipName="Adult", Cost=65, AgeGroup="18+"},
                new Membership{MembershipName="Family", Cost=100, AgeGroup="All"}
            };
            context.Memberships.AddRange(memberships);
            context.SaveChanges();

            // -------------------------
            // USERS (~8)
            // -------------------------
            var users = new List<User>();

            for (int i = 1; i <= 8; i++)
            {
                var user = new User
                {
                    UserName = $"user{i}@test.com",
                    Email = $"user{i}@test.com",
                    FirstName = $"User{i}",
                    LastName = "Test",
                    IsAdmin = (i == 1)
                };

                userManager.CreateAsync(user, "Password123!").Wait();
                users.Add(user);
            }

            // -------------------------
            // INSTRUCTORS (~10)
            // -------------------------
            var instructors = new List<Instructor>();

            for (int i = 0; i < users.Count; i++)
            {
                instructors.Add(new Instructor
                {
                    UserId = users[i].Id,
                    ClubId = clubs[i % clubs.Count].ClubId,
                    BeltId = belts[8].BeltId,
                    DateJoined = DateTime.Now.AddYears(-2)
                });
            }

            context.Instructors.AddRange(instructors);
            context.SaveChanges();

            // -------------------------
            // MEMBERS (20+) MORE THAN USERS
            // -------------------------
            var members = new List<Member>();

            for (int i = 1; i <= 25; i++)
            {
                members.Add(new Member
                {
                    UserId = users[i % users.Count].Id, // multiple members per user
                    ClubId = clubs[i % clubs.Count].ClubId,
                    MembershipId = memberships[i % memberships.Count].MembershipId,
                    BeltId = belts[i % belts.Count].BeltId,
                    BeltSize = 3,
                    FirstName = $"Member{i}",
                    LastName = "Karate",
                    DateOfBirth = DateTime.Now.AddYears(-15).AddDays(i),
                    Gender = Gender.Male,
                    Weight = 60 + i,
                    Height = 150 + i,
                    DateJoined = DateTime.Now.AddMonths(-i),
                    EmergencyContactName = "Parent",
                    EmergencyContactPhone = "0219999999",
                    Status = MemberStatus.Active
                });
            }

            context.Members.AddRange(members);
            context.SaveChanges();

            // -------------------------
            // SCHEDULES (20+)
            // -------------------------
            var schedules = new List<Schedule>();

            for (int i = 1; i <= 20; i++)
            {
                schedules.Add(new Schedule
                {
                    ClubId = clubs[i % clubs.Count].ClubId,
                    InstructorId = instructors[i % instructors.Count].InstructorId,
                    Level = (Levels)(i % 3),
                    DayOfWeek = (Weekdays)(i % 7),
                    StartTime = DateTime.Today.AddHours(17),
                    EndTime = DateTime.Today.AddHours(18)
                });
            }

            context.Schedules.AddRange(schedules);
            context.SaveChanges();

            // -------------------------
            // GRADINGS (10+)
            // -------------------------
            var gradings = new List<Grading>();

            for (int i = 1; i <= 10; i++)
            {
                gradings.Add(new Grading
                {
                    ClubId = clubs[i % clubs.Count].ClubId,
                    GradingDate = DateTime.Today.AddDays(-i),
                    GradingStartTime = DateTime.Today.AddHours(10),
                    GradingEndTime = DateTime.Today.AddHours(12)
                });
            }

            context.Gradings.AddRange(gradings);
            context.SaveChanges();

            // -------------------------
            // PAYMENTS (20+)
            // -------------------------
            var payments = new List<Payment>();

            for (int i = 1; i <= 20; i++)
            {
                payments.Add(new Payment
                {
                    MemberId = members[i % members.Count].MemberId,
                    PaymentName = "Monthly Fee",
                    Amount = 50,
                    DateDue = DateTime.Today.AddDays(7),
                    PaymentMethod = PaymentMethods.CreditCard,
                    Status = PaymentStatus.Pending
                });
            }

            context.Payments.AddRange(payments);
            context.SaveChanges();

            // -------------------------
            // ATTENDANCE (20+)
            // -------------------------
            var attendances = new List<Attendance>();

            for (int i = 1; i <= 20; i++)
            {
                attendances.Add(new Attendance
                {
                    MemberId = members[i % members.Count].MemberId,
                    ScheduleId = schedules[i % schedules.Count].ScheduleId,
                    Date = DateTime.Today.AddDays(-i)
                });
            }

            context.Attendances.AddRange(attendances);
            context.SaveChanges();

            // -------------------------
            // MEMBER GRADINGS (20+)
            // -------------------------
            var memberGradings = new List<MemberGrading>();

            for (int i = 1; i <= 20; i++)
            {
                memberGradings.Add(new MemberGrading
                {
                    MemberId = members[i % members.Count].MemberId,
                    GradingId = gradings[i % gradings.Count].GradingId,
                    BeltBeforeId = belts[0].BeltId,
                    BeltAfterId = belts[1].BeltId,
                    Passed = true
                });
            }

            context.MemberGradings.AddRange(memberGradings);
            context.SaveChanges();
        }
    }
}