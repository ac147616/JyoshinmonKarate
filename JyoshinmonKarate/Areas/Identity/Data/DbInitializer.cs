using JyoshinmonKarate.Areas.Identity.Data;
using JyoshinmonKarate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JyoshinmonKarate.Data
{
    public static class DbInitializer
    {

        public static void Initialize(JyoshinmonKarateContext context, UserManager<User> userManager)
        {
            context.Database.Migrate();

            if (context.Clubs.Any())
            {
                return;
            }

            var random = new Random(42);

            // CLUBS
            var clubs = new List<Club>
            {
                new Club { ClubName = "Central Dojo", Address = "123 Queen Street, Auckland", Email = "central@jyoshinmon.org", Phone = "0211000001" },
                new Club { ClubName = "West Dojo", Address = "45 Great North Road, Auckland", Email = "west@jyoshinmon.org", Phone = "0211000002" },
                new Club { ClubName = "North Dojo", Address = "78 Lake Road, Auckland", Email = "north@jyoshinmon.org", Phone = "0211000003" },
                new Club { ClubName = "South Dojo", Address = "22 Manukau Road, Auckland", Email = "south@jyoshinmon.org", Phone = "0211000004" },
                new Club { ClubName = "East Dojo", Address = "10 Ti Rakau Drive, Auckland", Email = "east@jyoshinmon.org", Phone = "0211000005" }
            };
            context.Clubs.AddRange(clubs);
            context.SaveChanges();

            // BELTS
            var belts = new List<Belt>
            {
                new Belt { BeltName = "White" },
                new Belt { BeltName = "Yellow" },
                new Belt { BeltName = "Orange" },
                new Belt { BeltName = "Green" },
                new Belt { BeltName = "Blue" },
                new Belt { BeltName = "Purple" },
                new Belt { BeltName = "Brown 3rd Kyu" },
                new Belt { BeltName = "Brown 2nd Kyu" },
                new Belt { BeltName = "Brown 1st Kyu" },
                new Belt { BeltName = "Black" }
            };
            context.Belts.AddRange(belts);
            context.SaveChanges();

            // MEMBERSHIPS
            var memberships = new List<Membership>
            {
                new Membership { MembershipName = "Junior Plan", Cost = 40.00m, AgeGroup = "Under 13" },
                new Membership { MembershipName = "Teen Plan", Cost = 50.00m, AgeGroup = "13-17" },
                new Membership { MembershipName = "Adult Plan", Cost = 65.00m, AgeGroup = "18+" },
                new Membership { MembershipName = "Family Plan", Cost = 110.00m, AgeGroup = "All Ages" }
            };
            context.Memberships.AddRange(memberships);
            context.SaveChanges();

            // USERS
            var users = new List<User>();

            for (int i = 1; i <= 12; i++)
            {
                var user = new User
                {
                    UserName = $"user{i}@test.com",
                    Email = $"user{i}@test.com",
                    FirstName = $"User{i}",
                    LastName = $"Manager{i}",
                    PhoneNumber = $"02177{i:0000}",
                    EmailConfirmed = true,
                    IsAdmin = i <= 3
                };

                var result = userManager.CreateAsync(user, "Password123!").Result;
                if (result.Succeeded)
                {
                    users.Add(user);
                }
            }

            // Refresh users from DB just in case
            users = context.Users.OrderBy(u => u.UserName).ToList();

            // INSTRUCTORS
            var instructors = new List<Instructor>();
            for (int i = 0; i < 12; i++)
            {
                instructors.Add(new Instructor
                {
                    UserId = users[i].Id,
                    ClubId = clubs[i % clubs.Count].ClubId,
                    BeltId = belts[8 + (i % 2)].BeltId,
                    DateJoined = DateTime.Today.AddYears(-(2 + (i % 6))).AddDays(i * -15)
                });
            }
            context.Instructors.AddRange(instructors);
            context.SaveChanges();

            // MEMBERS
            var firstNames = new[]
            {
                "Ava","Liam","Noah","Mia","Sophia","Lucas","Ethan","Ella","Olivia","Leo",
                "Isla","Aria","Grace","Jack","Emily","James","Charlotte","Amelia","Henry","Ruby",
                "Daniel","Sophie","Benjamin","Chloe","Mason","Evie","Samuel","Zoe","Nathan","Lucy",
                "Caleb","Hannah","Max","Layla","Ryan","Nina","Adam","Emma","Tyler","Bella",
                "Oscar","Harper","David","Aaliyah","Matthew","Lily","Isaac","Georgia","Finn","Maya"
            };

            var lastNames = new[]
            {
                "Singh","Smith","Patel","Kim","Lee","Brown","Taylor","Wilson","Chen","Khan",
                "Martin","Walker","Nguyen","Hall","Young","White","Scott","Green","King","Moore"
            };

            var emergencyNames = new[]
            {
                "Parent One","Parent Two","Mum","Dad","Guardian","Auntie","Uncle","Older Sister","Older Brother","Family Friend"
            };

            var members = new List<Member>();
            for (int i = 0; i < 50; i++)
            {
                var age = 8 + (i % 30);
                var dob = DateTime.Today.AddYears(-age).AddDays(-(i * 7 % 365));
                var gender = (Gender)(i % 3);
                var status = i % 10 == 0 ? MemberStatus.Suspended :
                             i % 6 == 0 ? MemberStatus.Inactive :
                             MemberStatus.Active;

                members.Add(new Member
                {
                    UserId = users[i % users.Count].Id,
                    ClubId = clubs[i % clubs.Count].ClubId,
                    BeltId = belts[i % 7].BeltId,
                    BeltSize = 2 + (i % 8),
                    FirstName = firstNames[i],
                    LastName = lastNames[i % lastNames.Length],
                    DateOfBirth = dob,
                    Gender = gender,
                    Weight = 30 + (i % 70),
                    Height = 120 + (i % 70),
                    DateJoined = DateTime.Today.AddMonths(-(1 + (i % 24))).AddDays(-(i % 20)),
                    EmergencyContactName = emergencyNames[i % emergencyNames.Length],
                    EmergencyContactPhone = $"02288{i:0000}",
                    Status = status
                });
            }
            context.Members.AddRange(members);
            context.SaveChanges();

            // SCHEDULES
            var schedules = new List<Schedule>();
            for (int i = 0; i < 50; i++)
            {
                var startHour = 16 + (i % 4); // 4pm to 7pm
                var durationHours = (i % 2 == 0) ? 1 : 2;

                schedules.Add(new Schedule
                {
                    ClubId = clubs[i % clubs.Count].ClubId,
                    InstructorId = instructors[i % instructors.Count].InstructorId,
                    Level = (Levels)(i % 3),
                    DayOfWeek = (Weekdays)(i % 7),
                    StartTime = DateTime.Today.AddHours(startHour),
                    EndTime = DateTime.Today.AddHours(startHour + durationHours)
                });
            }
            context.Schedules.AddRange(schedules);
            context.SaveChanges();

            // GRADINGS
            var gradings = new List<Grading>();
            for (int i = 0; i < 20; i++)
            {
                var gradingDate = DateTime.Today.AddDays(-(i * 14 + 3));
                var startHour = 9 + (i % 3);

                gradings.Add(new Grading
                {
                    ClubId = clubs[i % clubs.Count].ClubId,
                    GradingDate = gradingDate,
                    GradingStartTime = gradingDate.AddHours(startHour),
                    GradingEndTime = gradingDate.AddHours(startHour + 2)
                });
            }
            context.Gradings.AddRange(gradings);
            context.SaveChanges();

            // PAYMENTS
            var paymentNames = new[]
            {
                "Monthly Fee",
                "Grading Fee",
                "Uniform Fee",
                "Registration Fee",
                "Seminar Fee"
            };

            var payments = new List<Payment>();
            for (int i = 0; i < 50; i++)
            {
                var status = i % 5 == 0 ? PaymentStatus.Failed :
                             i % 3 == 0 ? PaymentStatus.Completed :
                             PaymentStatus.Pending;

                decimal amount = paymentNames[i % paymentNames.Length] switch
                {
                    "Monthly Fee" => 40 + (i % 4) * 5,
                    "Grading Fee" => 35 + (i % 3) * 10,
                    "Uniform Fee" => 75,
                    "Registration Fee" => 25,
                    _ => 20 + (i % 5) * 5
                };

                payments.Add(new Payment
                {
                    MemberId = members[i % members.Count].MemberId,
                    PaymentName = paymentNames[i % paymentNames.Length],
                    Amount = amount,
                    DateDue = DateTime.Today.AddDays(-10 + i),
                    PaymentMethod = (PaymentMethods)(i % 4),
                    Status = status
                });
            }
            context.Payments.AddRange(payments);
            context.SaveChanges();

            // ATTENDANCES
            var attendances = new List<Attendance>();
            for (int i = 0; i < 50; i++)
            {
                attendances.Add(new Attendance
                {
                    MemberId = members[i % members.Count].MemberId,
                    ScheduleId = schedules[i % schedules.Count].ScheduleId,
                    Date = DateTime.Today.AddDays(-(i % 30))
                });
            }
            context.Attendances.AddRange(attendances);
            context.SaveChanges();

            // MEMBER GRADINGS
            var memberGradings = new List<MemberGrading>();
            for (int i = 0; i < 50; i++)
            {
                var beforeIndex = i % 8;
                var afterIndex = beforeIndex + (i % 4 == 0 ? 0 : 1); // some same belt, most move up

                if (afterIndex >= belts.Count)
                {
                    afterIndex = belts.Count - 1;
                }

                var passed = i % 5 != 0;

                if (!passed)
                {
                    afterIndex = beforeIndex;
                }

                memberGradings.Add(new MemberGrading
                {
                    GradingId = gradings[i % gradings.Count].GradingId,
                    MemberId = members[i % members.Count].MemberId,
                    BeltBeforeId = belts[beforeIndex].BeltId,
                    BeltAfterId = belts[afterIndex].BeltId,
                    Passed = passed
                });
            }
            context.MemberGradings.AddRange(memberGradings);
            context.SaveChanges();
        }
    }
}