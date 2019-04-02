using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using TasksCommon;

namespace SchedulerModel.Entity
{
    public class BaseModel : DbContext
    {
        //public BaseModel()
        //{
        //}

        //public BaseModel(DbContextOptions<BaseModel> options)
        //    : base(options)
        //{

        //}
        SqlConn sqlServerConn = new SqlConn();
        public BaseModel(SqlConn ServerConn)
        {
            sqlServerConn = ServerConn;
        }

        public DbSet<GroupModel> Group { get; set; }
        public DbSet<LoggerModel> Logger { get; set; }
        public DbSet<UserModel> User { get; set; }
        public DbSet<TriggerModel> Trigger { get; set; }
        public DbSet<TaskModel> Task { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                if (sqlServerConn != null)
                {
                    optionsBuilder.UseSqlServer("Server=" + sqlServerConn.Host + "," + sqlServerConn.Port + "; Database=" + sqlServerConn.DataBase + ";Persist Security Info=True;User ID=" + sqlServerConn.UserName + ";password=" + sqlServerConn.Password + ";");
                }
                else
                {
                    //optionsBuilder.UseSqlServer("data source=106.75.245.241,6215;initial catalog=jyzd_2_0;user id=sa;pwd=yF3wvu1h*khPF*5k;");
                }

                //if (!optionsBuilder.IsConfigured)
                //{
                //    //配置连接字符串
                //    optionsBuilder.UseSqlServer("Server=106.75.245.241,6215; Database=jyzd_2_0;Persist Security Info=True;User ID=jiu2019ying;password=q*UrE92HL^fsZ#B4;");
                //}
                //optionsBuilder.UseSqlite($"Data Source=db.s3db");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
