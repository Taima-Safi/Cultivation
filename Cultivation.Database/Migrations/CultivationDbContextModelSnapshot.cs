﻿// <auto-generated />
using System;
using Cultivation.Database.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Cultivation.Database.Migrations
{
    [DbContext(typeof(CultivationDbContext))]
    partial class CultivationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Cultivation.Database.Model.ClientModel", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("CodePhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsLocal")
                        .HasColumnType("bit");

                    b.Property<bool>("IsValid")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Client");
                });

            modelBuilder.Entity("Cultivation.Database.Model.ColorModel", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsValid")
                        .HasColumnType("bit");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Color");
                });

            modelBuilder.Entity("Cultivation.Database.Model.CuttingColorModel", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("ColorId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<long>("CuttingId")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsValid")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ColorId");

                    b.HasIndex("CuttingId");

                    b.ToTable("CuttingColor");
                });

            modelBuilder.Entity("Cultivation.Database.Model.CuttingLandModel", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<long>("CuttingColorId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsValid")
                        .HasColumnType("bit");

                    b.Property<long>("LandId")
                        .HasColumnType("bigint");

                    b.Property<long>("Quantity")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CuttingColorId");

                    b.HasIndex("LandId");

                    b.ToTable("CuttingLand");
                });

            modelBuilder.Entity("Cultivation.Database.Model.CuttingModel", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<int>("Age")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsValid")
                        .HasColumnType("bit");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Cutting");
                });

            modelBuilder.Entity("Cultivation.Database.Model.FertilizerLandModel", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<long>("CuttingLandId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<long>("FertilizerId")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsValid")
                        .HasColumnType("bit");

                    b.Property<double>("Quantity")
                        .HasColumnType("float");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CuttingLandId");

                    b.HasIndex("FertilizerId");

                    b.ToTable("FertilizerLand");
                });

            modelBuilder.Entity("Cultivation.Database.Model.FertilizerModel", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("File")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsValid")
                        .HasColumnType("bit");

                    b.Property<string>("NPK")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PublicTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Fertilizer");
                });

            modelBuilder.Entity("Cultivation.Database.Model.FlowerModel", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<long>("CuttingLandId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsValid")
                        .HasColumnType("bit");

                    b.Property<double>("Long")
                        .HasColumnType("float");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Worker")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CuttingLandId");

                    b.ToTable("Flower");
                });

            modelBuilder.Entity("Cultivation.Database.Model.FlowerStoreModel", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<double>("FlowerLong")
                        .HasColumnType("float");

                    b.Property<bool>("IsValid")
                        .HasColumnType("bit");

                    b.Property<int>("RemainedCount")
                        .HasColumnType("int");

                    b.Property<int>("TotalCount")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("FlowerStore");
                });

            modelBuilder.Entity("Cultivation.Database.Model.InsecticideLandModel", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<long>("CuttingLandId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<long>("InsecticideId")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsValid")
                        .HasColumnType("bit");

                    b.Property<double>("Liter")
                        .HasColumnType("float");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("Quantity")
                        .HasColumnType("float");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CuttingLandId");

                    b.HasIndex("InsecticideId");

                    b.ToTable("InsecticideLand");
                });

            modelBuilder.Entity("Cultivation.Database.Model.InsecticideModel", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsValid")
                        .HasColumnType("bit");

                    b.Property<string>("PublicTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Insecticide");
                });

            modelBuilder.Entity("Cultivation.Database.Model.LandModel", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsValid")
                        .HasColumnType("bit");

                    b.Property<string>("Location")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("ParentId")
                        .HasColumnType("bigint");

                    b.Property<double>("Size")
                        .HasColumnType("float");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("Land");
                });

            modelBuilder.Entity("Cultivation.Database.Model.OrderDetailModel", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<long>("FlowerStoreId")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsValid")
                        .HasColumnType("bit");

                    b.Property<long>("OrderId")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("FlowerStoreId");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderDetail");
                });

            modelBuilder.Entity("Cultivation.Database.Model.OrderModel", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime?>("BoughtDate")
                        .HasColumnType("datetime2");

                    b.Property<long>("ClientId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsBought")
                        .HasColumnType("bit");

                    b.Property<bool>("IsValid")
                        .HasColumnType("bit");

                    b.Property<string>("Number")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("Order");
                });

            modelBuilder.Entity("Cultivation.Database.Model.CuttingColorModel", b =>
                {
                    b.HasOne("Cultivation.Database.Model.ColorModel", "Color")
                        .WithMany("CuttingColors")
                        .HasForeignKey("ColorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Cultivation.Database.Model.CuttingModel", "Cutting")
                        .WithMany("CuttingColors")
                        .HasForeignKey("CuttingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Color");

                    b.Navigation("Cutting");
                });

            modelBuilder.Entity("Cultivation.Database.Model.CuttingLandModel", b =>
                {
                    b.HasOne("Cultivation.Database.Model.CuttingColorModel", "CuttingColor")
                        .WithMany("CuttingLands")
                        .HasForeignKey("CuttingColorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Cultivation.Database.Model.LandModel", "Land")
                        .WithMany("CuttingLands")
                        .HasForeignKey("LandId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CuttingColor");

                    b.Navigation("Land");
                });

            modelBuilder.Entity("Cultivation.Database.Model.FertilizerLandModel", b =>
                {
                    b.HasOne("Cultivation.Database.Model.CuttingLandModel", "CuttingLand")
                        .WithMany("FertilizerLands")
                        .HasForeignKey("CuttingLandId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Cultivation.Database.Model.FertilizerModel", "Fertilizer")
                        .WithMany("FertilizerLands")
                        .HasForeignKey("FertilizerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CuttingLand");

                    b.Navigation("Fertilizer");
                });

            modelBuilder.Entity("Cultivation.Database.Model.FlowerModel", b =>
                {
                    b.HasOne("Cultivation.Database.Model.CuttingLandModel", "CuttingLand")
                        .WithMany("Flowers")
                        .HasForeignKey("CuttingLandId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CuttingLand");
                });

            modelBuilder.Entity("Cultivation.Database.Model.InsecticideLandModel", b =>
                {
                    b.HasOne("Cultivation.Database.Model.CuttingLandModel", "CuttingLand")
                        .WithMany("InsecticideLands")
                        .HasForeignKey("CuttingLandId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Cultivation.Database.Model.InsecticideModel", "Insecticide")
                        .WithMany("InsecticideLands")
                        .HasForeignKey("InsecticideId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CuttingLand");

                    b.Navigation("Insecticide");
                });

            modelBuilder.Entity("Cultivation.Database.Model.LandModel", b =>
                {
                    b.HasOne("Cultivation.Database.Model.LandModel", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("Cultivation.Database.Model.OrderDetailModel", b =>
                {
                    b.HasOne("Cultivation.Database.Model.FlowerStoreModel", "FlowerStore")
                        .WithMany("OrderDetails")
                        .HasForeignKey("FlowerStoreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Cultivation.Database.Model.OrderModel", "Order")
                        .WithMany("OrderDetails")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FlowerStore");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("Cultivation.Database.Model.OrderModel", b =>
                {
                    b.HasOne("Cultivation.Database.Model.ClientModel", "Client")
                        .WithMany("Orders")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");
                });

            modelBuilder.Entity("Cultivation.Database.Model.ClientModel", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("Cultivation.Database.Model.ColorModel", b =>
                {
                    b.Navigation("CuttingColors");
                });

            modelBuilder.Entity("Cultivation.Database.Model.CuttingColorModel", b =>
                {
                    b.Navigation("CuttingLands");
                });

            modelBuilder.Entity("Cultivation.Database.Model.CuttingLandModel", b =>
                {
                    b.Navigation("FertilizerLands");

                    b.Navigation("Flowers");

                    b.Navigation("InsecticideLands");
                });

            modelBuilder.Entity("Cultivation.Database.Model.CuttingModel", b =>
                {
                    b.Navigation("CuttingColors");
                });

            modelBuilder.Entity("Cultivation.Database.Model.FertilizerModel", b =>
                {
                    b.Navigation("FertilizerLands");
                });

            modelBuilder.Entity("Cultivation.Database.Model.FlowerStoreModel", b =>
                {
                    b.Navigation("OrderDetails");
                });

            modelBuilder.Entity("Cultivation.Database.Model.InsecticideModel", b =>
                {
                    b.Navigation("InsecticideLands");
                });

            modelBuilder.Entity("Cultivation.Database.Model.LandModel", b =>
                {
                    b.Navigation("Children");

                    b.Navigation("CuttingLands");
                });

            modelBuilder.Entity("Cultivation.Database.Model.OrderModel", b =>
                {
                    b.Navigation("OrderDetails");
                });
#pragma warning restore 612, 618
        }
    }
}
