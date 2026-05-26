using Microsoft.EntityFrameworkCore;
using SmartStudyPlannerAI.Models.Entities;

namespace SmartStudyPlannerAI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<Exam> Exams { get; set; }
    public DbSet<StudyPlan> StudyPlans { get; set; }
    public DbSet<AIChat> AIChats { get; set; }
    public DbSet<UploadedFile> UploadedFiles { get; set; }
    public DbSet<Summary> Summaries { get; set; }
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<PasswordResetOTP> PasswordResetOTPs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuration User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(20).HasDefaultValue("Student");
        });

        // Configuration Subject
        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Subjects)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuration TaskItem
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Tasks)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(e => e.Subject)
                  .WithMany(s => s.Tasks)
                  .HasForeignKey(e => e.SubjectId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuration Exam
        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Exams)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(e => e.Subject)
                  .WithMany(s => s.Exams)
                  .HasForeignKey(e => e.SubjectId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuration StudyPlan
        modelBuilder.Entity<StudyPlan>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.StudyPlans)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuration AIChat
        modelBuilder.Entity<AIChat>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.AIChats)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuration UploadedFile
        modelBuilder.Entity<UploadedFile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.UploadedFiles)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuration Summary
        modelBuilder.Entity<Summary>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Summaries)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(e => e.UploadedFile)
                  .WithMany(f => f.Summaries)
                  .HasForeignKey(e => e.FileId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuration Quiz
        modelBuilder.Entity<Quiz>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Quizzes)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(e => e.Subject)
                  .WithMany(s => s.Quizzes)
                  .HasForeignKey(e => e.SubjectId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuration Notification
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Notifications)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuration PasswordResetOTP
        modelBuilder.Entity<PasswordResetOTP>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.PasswordResetOTPs)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}