using JyoshinmonKarate.Areas.Identity.Data;
using JyoshinmonKarate.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JyoshinmonKarate.Areas.Identity.Data;

public class JyoshinmonKarateContext : IdentityDbContext<User>
{
    public JyoshinmonKarateContext(DbContextOptions<JyoshinmonKarateContext> options)
        : base(options)
    {
    }

    public DbSet<Attendance> Attendances { get; set; }
    public DbSet<Belt> Belts { get; set; }
    public DbSet<Club> Clubs { get; set; }
    public DbSet<Grading> Gradings { get; set; }
    public DbSet<Instructor> Instructors { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<MemberGrading> MemberGradings { get; set; }
    public DbSet<Membership> Memberships { get; set; }
    public DbSet<MemberMembership> MemberMemberships { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Schedule> Schedules { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<MemberGrading>()
                .HasOne(mg => mg.BeltBefore)
                .WithMany(b => b.BeltBeforeMemberGradings)
                .HasForeignKey(mg => mg.BeltBeforeId)
                .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<MemberGrading>()
                .HasOne(mg => mg.BeltAfter)
                .WithMany(b => b.BeltAfterMemberGradings)
                .HasForeignKey(mg => mg.BeltAfterId)
                .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Schedule>()
                .HasOne(s => s.Instructor)
                .WithMany(i => i.Schedules)
                .HasForeignKey(s => s.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<MemberGrading>()
                .HasOne(mg => mg.Member)
                .WithMany(m => m.MemberGradings)
                .HasForeignKey(mg => mg.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Attendance>()
                .HasOne(a => a.Schedule)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.ScheduleId)
                .OnDelete(DeleteBehavior.Restrict);

    }
}
