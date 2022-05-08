using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ORM0Entities
{
    public partial class remote_databaseContext : DbContext
    {
        public remote_databaseContext()
        {
        }

        public remote_databaseContext(DbContextOptions<remote_databaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Additionalcustomerinformation> Additionalcustomerinformations { get; set; } = null!;
        public virtual DbSet<Article> Articles { get; set; } = null!;
        public virtual DbSet<Bill> Bills { get; set; } = null!;
        public virtual DbSet<Book> Books { get; set; } = null!;
        public virtual DbSet<Chapter> Chapters { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Page> Pages { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Database=remote_database;Uid=remote_user;Pwd=remote_password;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Additionalcustomerinformation>(entity =>
            {
                entity.ToTable("additionalcustomerinformation");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Customerlikescars).HasColumnName("customerlikescars");

                entity.Property(e => e.Customerlikescolorgreen).HasColumnName("customerlikescolorgreen");

                entity.Property(e => e.Defaultpurchasepricemultiplicator).HasColumnName("defaultpurchasepricemultiplicator");

                entity.Property(e => e.Totalarticlesaddedtoshoppingcart).HasColumnName("totalarticlesaddedtoshoppingcart");
            });

            modelBuilder.Entity<Article>(entity =>
            {
                entity.ToTable("articles");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Articlename)
                    .HasMaxLength(50)
                    .HasColumnName("articlename")
                    .IsFixedLength();

                entity.Property(e => e.Articleprice).HasColumnName("articleprice");

                entity.Property(e => e.Ishidden).HasColumnName("ishidden");
            });

            modelBuilder.Entity<Bill>(entity =>
            {
                entity.ToTable("bills");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Purchasedate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("purchasedate");

                entity.Property(e => e.Purchaseprice).HasColumnName("purchaseprice");

                entity.HasMany(d => d.FkArticles)
                    .WithMany(p => p.FkBills)
                    .UsingEntity<Dictionary<string, object>>(
                        "Billsarticle",
                        l => l.HasOne<Article>().WithMany().HasForeignKey("FkArticlesid").HasConstraintName("billsarticles_fk_articlesid_fkey"),
                        r => r.HasOne<Bill>().WithMany().HasForeignKey("FkBillsid").HasConstraintName("billsarticles_fk_billsid_fkey"),
                        j =>
                        {
                            j.HasKey("FkBillsid", "FkArticlesid").HasName("billsarticles_pkey");

                            j.ToTable("billsarticles");

                            j.IndexerProperty<int>("FkBillsid").HasColumnName("fk_billsid");

                            j.IndexerProperty<int>("FkArticlesid").HasColumnName("fk_articlesid");
                        });
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.ToTable("books");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Authorname)
                    .HasMaxLength(150)
                    .HasColumnName("authorname");

                entity.Property(e => e.Bookname)
                    .HasMaxLength(150)
                    .HasColumnName("bookname");

                entity.Property(e => e.Price).HasColumnName("price");
            });

            modelBuilder.Entity<Chapter>(entity =>
            {
                entity.ToTable("chapters");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Chaptername).HasColumnName("chaptername");

                entity.Property(e => e.FkBookid).HasColumnName("fk_bookid");

                entity.HasOne(d => d.FkBook)
                    .WithMany(p => p.Chapters)
                    .HasForeignKey(d => d.FkBookid)
                    .HasConstraintName("chapters_fk_bookid_fkey");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("customers");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.CustomerSince)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("customer_since");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .HasColumnName("email")
                    .IsFixedLength();

                entity.Property(e => e.FkAdditionalcustomerinformation).HasColumnName("fk_additionalcustomerinformation");

                entity.Property(e => e.Isvip).HasColumnName("isvip");

                entity.Property(e => e.Lastname)
                    .HasMaxLength(50)
                    .HasColumnName("lastname")
                    .IsFixedLength();

                entity.HasOne(d => d.FkAdditionalcustomerinformationNavigation)
                    .WithMany(p => p.Customers)
                    .HasForeignKey(d => d.FkAdditionalcustomerinformation)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("customers_fk_additionalcustomerinformation_fkey");
            });

            modelBuilder.Entity<Page>(entity =>
            {
                entity.ToTable("pages");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.FkChapterId).HasColumnName("fk_chapter_id");

                entity.Property(e => e.Text).HasColumnName("text");

                entity.HasOne(d => d.FkChapter)
                    .WithMany(p => p.Pages)
                    .HasForeignKey(d => d.FkChapterId)
                    .HasConstraintName("pages_fk_chapter_id_fkey");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
