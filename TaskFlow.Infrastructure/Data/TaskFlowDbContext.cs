using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Data
{
    public class TaskFlowDbContext : DbContext
    {
        public TaskFlowDbContext(DbContextOptions<TaskFlowDbContext> options) : base(options)
        {
        }
        public DbSet<Company> Companies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Company>()
                .HasIndex(c => c.Name)
                .IsUnique();

            modelBuilder.Entity<Team>()
                 .HasOne(t => t.Company)
                 .WithMany(c => c.Teams)
                 .HasForeignKey(t => t.CompanyId)
                 .OnDelete(DeleteBehavior.Restrict);
            
            // Constraint: Team name must be unique within a company
            modelBuilder.Entity<Team>()
                .HasIndex(t => new { t.CompanyId, t.Name })
                .IsUnique();

            modelBuilder.Entity<User>()
                 .HasOne(u => u.Company)
                 .WithMany(c => c.Users)
                 .HasForeignKey(u => u.CompanyId)
                 .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TeamMember>()
                .HasOne(tm => tm.User)
                .WithMany(u => u.TeamMembers)
                .HasForeignKey(tm => tm.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TeamMember>()
                .HasOne(tm => tm.Team)
                .WithMany(t => t.TeamMembers)
                .HasForeignKey(tm => tm.TeamId)
                .OnDelete(DeleteBehavior.Restrict);
            //constraint
            modelBuilder.Entity<TeamMember>()
                .HasIndex(tm => new { tm.UserId, tm.TeamId })
                .IsUnique();

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Team)
                .WithMany(t => t.Projects)
                .HasForeignKey(p => p.TeamId)
                .OnDelete(DeleteBehavior.Restrict);

            // Active project names must be unique within a team.
            // Soft-deleted projects do not participate in the unique constraint.
            modelBuilder.Entity<Project>()
                .HasIndex(p => new { p.TeamId, p.Name })
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            modelBuilder.Entity<TaskItem>()
                .HasOne(ti => ti.Project)
                .WithMany(p => p.TaskItems)
                .HasForeignKey(ti => ti.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.AssignedUser)
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(t => t.AssignedUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
