using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SchedulerModel.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using TasksCommon;

namespace TasksEntity.Model
{
    public class SqlContext : DbContext
    {
        public SqlContext()
        {
        }     
        public SqlContext(DbContextOptions<SqlContext> options)
            : base(options)
        {

        }
        public virtual DbSet<UserInfo> UserInfos { get; set; }
        public virtual DbSet<UserReport> UserReport { get; set; }
        public virtual DbSet<RecommendItem> RecommendItem { get; set; }
        public virtual DbSet<BnCoverageRate> BnCoverageRate { get; set; }
        public virtual DbSet<UserArticle> UserArticle { get; set; }
        public virtual DbSet<UserCareer> UserCareers { get; set; }

        public DbSet<GroupModel> Group { get; set; }
        public DbSet<LoggerModel> Logger { get; set; }
        public DbSet<UserModel> User { get; set; }
        public DbSet<TriggerModel> Trigger { get; set; }
        public DbSet<TaskModel> Task { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("data source=10.23.157.204,1433;initial catalog=jyzd_2_0;user id=jiu2019ying;pwd=q*UrE92HL^fsZ#B4;");
                //配置连接字符串
                //optionsBuilder.UseSqlServer("data source=106.75.245.241,6215;initial catalog=jyzd_2_0;user id=sa;pwd=yF3wvu1h*khPF*5k;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("User_Info");

                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.AvatarUrl)
                    .IsRequired()
                    .HasColumnName("Avatar_URL")
                    .IsUnicode(false);

                entity.Property(e => e.Birthday).HasColumnType("datetime");

                entity.Property(e => e.Bnstatus).HasColumnName("BNStatus");

                entity.Property(e => e.BusinessInfo)
                    .IsRequired()
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.CareerAuth).HasColumnName("Career_Auth");
                entity.Property(e => e.CareerType).HasColumnName("Career_Type");
                entity.Property(e => e.Company)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FieldTags)
                    .IsRequired()
                    .HasColumnName("Field_Tags")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.HopeTags)
                    .IsRequired()
                    .HasColumnName("Hope_Tags")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.IdentityAuth).HasColumnName("Identity_Auth");
                entity.Property(e => e.CareerType).HasColumnName("Career_Type");

                entity.Property(e => e.Industry)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.InviteUrl)
                    .IsRequired()
                    .HasColumnName("Invite_Url")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NickName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Position)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RegisterAt)
                    .HasColumnName("Register_At")
                    .HasColumnType("datetime");

                entity.Property(e => e.Remark)
                    .IsRequired()
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.Telphone)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateAt)
                    .HasColumnName("Update_At")
                    .HasColumnType("datetime");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });
            modelBuilder.Entity<BnCoverageRate>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("BN_Coverage_Rate");

                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .ValueGeneratedNever();

                entity.Property(e => e.NetUserCount).HasColumnName("NetUserCount");

                entity.Property(e => e.BuserCount).HasColumnName("BUserCount");

                entity.Property(e => e.Crate).HasColumnName("CRate");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            });
            modelBuilder.Entity<UserReport>(entity =>
            {
                entity.HasKey(e => e.ReportID);
                entity.ToTable("User_Report");
            });
            modelBuilder.Entity<RecommendItem>(entity =>
            {
                entity.ToTable("Recommend_Item");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.ItemId).HasColumnName("ItemID");

                entity.Property(e => e.Tags)
                    .IsRequired()
                    .IsUnicode(false);
            });
            modelBuilder.Entity<UserArticle>(entity =>
            {
                entity.ToTable("User_Article");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Content)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.Cover)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Tags)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });
            modelBuilder.Entity<UserCareer>(entity =>
            {
                entity.ToTable("User_Career");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CardUrl)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Company)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CompanyTel)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Position)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });
            modelBuilder.Entity<GroupModel>(entity =>
            {
                entity.ToTable("Scheduler_Group");

                entity.Property(e => e.Id);
            });
            modelBuilder.Entity<LoggerModel>(entity =>
            {
                entity.ToTable("Scheduler_Logger");

                entity.Property(e => e.Id);
            });
            modelBuilder.Entity<UserModel>(entity =>
            {
                entity.ToTable("Scheduler_User");

                entity.Property(e => e.Id);
            });
            modelBuilder.Entity<TriggerModel>(entity =>
            {
                entity.ToTable("Scheduler_Trigger");

                entity.Property(e => e.Id);
            });
            modelBuilder.Entity<TaskModel>(entity =>
            {
                entity.ToTable("Scheduler_Task");

                entity.Property(e => e.Id);
            });
        }
    }
}
