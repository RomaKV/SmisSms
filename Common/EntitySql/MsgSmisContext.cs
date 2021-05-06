using EntitySql.Entities;
using Microsoft.EntityFrameworkCore;

namespace EntitySql
{
    public class MsgSmisContext : DbContext
    {
        private static DbContextOptions<MsgSmisContext> optionsSql;

        public MsgSmisContext() : base(optionsSql)
        {
        }

        public MsgSmisContext(DbContextOptions<MsgSmisContext> options)
            : base(options)
        {
            optionsSql = options;
        }


        public virtual DbSet<JournalSms> JournalSms { get; set; }

        public virtual DbSet<Phone> Phone { get; set; }

        public virtual DbSet<UserMsgTo> UserMsgTo { get; set; }

        public virtual DbSet<EDDSMsg> EDDSMsg { get; set; }

        public virtual DbSet<GlobalSettings> GlobalSettings { get; set; }

        public virtual DbSet<TypeStatusSmis> TypeStatusSmis { get; set; }

        public virtual DbSet<PhoneTypeStatusSmis> PhoneTypeStatusSmis { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Phone>(entity =>
            {
                entity.HasOne(d => d.UserMsgTo)
                    .WithMany(p => p.Phones)
                    .HasForeignKey(d => d.UserMsgToId)
                    .HasConstraintName("FK_Phone_MsgUserTo");
            });


            modelBuilder.Entity<JournalSms>(entity =>
            {
                entity.HasOne(d => d.Phone)
                    .WithMany(p => p.JournalSmses)
                    .HasForeignKey(d => d.PhoneId)
                    .HasConstraintName("FK_JournalSms_Phone");

                entity.HasOne(d => d.EDDSMsg)
                    .WithMany(p => p.JournalSmses)
                    .HasForeignKey(d => d.EDDSMsgId)
                    .HasConstraintName("FK_JournalSms_EDDSMsg");
            });


            modelBuilder.Entity<PhoneTypeStatusSmis>(entity =>
            {
                entity.HasKey(c => new {c.PhoneId, c.TypeStatusSmisId});

                entity.HasOne(p => p.Phone)
                    .WithMany(c => c.PhoneTypeStatusSmis)
                    .HasForeignKey(i => i.PhoneId)
                    .HasConstraintName("FK_PhoneTypeStatusSmis_Phone")
                    .IsRequired();
                entity.HasOne(p => p.TypeStatusSmis)
                    .WithMany(c => c.PhoneTypeStatusSmis)
                    .HasForeignKey(i => i.TypeStatusSmisId)
                    .HasConstraintName("FK_PhoneTypeStatusSmis_TypeStatusSmis")
                    .IsRequired();
            });


            base.OnModelCreating(modelBuilder);
        }
    }
}