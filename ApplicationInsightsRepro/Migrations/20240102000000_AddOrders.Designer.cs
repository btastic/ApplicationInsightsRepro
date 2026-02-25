using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationInsightsRepro.Migrations
{
    [DbContext(typeof(ApplicationInsightsRepro.Data.ApplicationDbContext))]
    [Migration("20240102000000_AddOrders")]
    partial class AddOrders
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "10.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ApplicationInsightsRepro.Models.Customer", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("Email")
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnType("nvarchar(255)");

                b.Property<string>("FirstName")
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnType("nvarchar(100)");

                b.Property<string>("LastName")
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnType("nvarchar(100)");

                b.Property<string>("PhoneNumber")
                    .HasMaxLength(20)
                    .HasColumnType("nvarchar(20)");

                b.Property<DateTime?>("UpdatedAt")
                    .HasColumnType("datetime2");

                b.HasKey("Id");

                b.HasIndex("Email")
                    .IsUnique();

                b.ToTable("Customers");
            });

            modelBuilder.Entity("ApplicationInsightsRepro.Models.Order", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2");

                b.Property<int>("CustomerId")
                    .HasColumnType("int");

                b.Property<string>("Notes")
                    .HasMaxLength(500)
                    .HasColumnType("nvarchar(500)");

                b.Property<DateTime>("OrderDate")
                    .HasColumnType("datetime2");

                b.Property<string>("OrderNumber")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("nvarchar(50)");

                b.Property<string>("Status")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("nvarchar(50)");

                b.Property<decimal>("TotalAmount")
                    .HasPrecision(18, 2)
                    .HasColumnType("decimal(18,2)");

                b.Property<DateTime?>("UpdatedAt")
                    .HasColumnType("datetime2");

                b.HasKey("Id");

                b.HasIndex("CustomerId");

                b.HasIndex("OrderNumber")
                    .IsUnique();

                b.ToTable("Orders");
            });

            modelBuilder.Entity("ApplicationInsightsRepro.Models.Order", b =>
            {
                b.HasOne("ApplicationInsightsRepro.Models.Customer", "Customer")
                    .WithMany()
                    .HasForeignKey("CustomerId")
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                b.Navigation("Customer");
            });
#pragma warning restore 612, 618
        }
    }
}
