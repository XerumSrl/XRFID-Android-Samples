﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using XRFID.Sample.Server.Database;

#nullable disable

namespace XRFID.Sample.Server.Migrations
{
    [DbContext(typeof(XRFIDSampleContext))]
    [Migration("20230908121752_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.10");

            modelBuilder.Entity("XRFID.Sample.Server.Entities.LoadingUnit", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Code")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatorUserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsConsolidated")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsValid")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastModificationTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("LastModifierUserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("OrderId")
                        .HasColumnType("TEXT");

                    b.Property<string>("OrderReference")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ReaderId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Reference")
                        .HasColumnType("TEXT");

                    b.Property<int>("Sequence")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("LoadingUnits");
                });

            modelBuilder.Entity("XRFID.Sample.Server.Entities.LoadingUnitItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Code")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatorUserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Epc")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsConsolidated")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastModificationTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("LastModifierUserId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("LoadingUnitId")
                        .HasColumnType("TEXT");

                    b.Property<string>("LoadingUnitReference")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Reference")
                        .HasColumnType("TEXT");

                    b.Property<string>("SerialNumber")
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("LoadingUnitId");

                    b.ToTable("LoadingUnitItems");
                });

            modelBuilder.Entity("XRFID.Sample.Server.Entities.Movement", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Code")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatorUserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsConsolidated")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsValid")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastModificationTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("LastModifierUserId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("MissingItem")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("OrderId")
                        .HasColumnType("TEXT");

                    b.Property<string>("OrderReference")
                        .HasColumnType("TEXT");

                    b.Property<bool>("OverflowItem")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("ReaderId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Reference")
                        .HasColumnType("TEXT");

                    b.Property<int>("Sequence")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.Property<bool>("UnexpectedItem")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Movements");
                });

            modelBuilder.Entity("XRFID.Sample.Server.Entities.MovementItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<bool>("Checked")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Code")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatorUserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Epc")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("FirstRead")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("IgnoreUntil")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsConsolidated")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsValid")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastModificationTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("LastModifierUserId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastRead")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("LoadingUnitItemId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("MovementId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("PC")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("ProductId")
                        .HasColumnType("TEXT");

                    b.Property<int>("ReadsCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Reference")
                        .HasColumnType("TEXT");

                    b.Property<short>("Rssi")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SerialNumber")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Tid")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("LoadingUnitItemId");

                    b.HasIndex("MovementId");

                    b.HasIndex("ProductId");

                    b.ToTable("MovementItems");
                });

            modelBuilder.Entity("XRFID.Sample.Server.Entities.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Attrib1")
                        .HasColumnType("TEXT");

                    b.Property<string>("Attrib2")
                        .HasColumnType("TEXT");

                    b.Property<string>("Attrib3")
                        .HasColumnType("TEXT");

                    b.Property<string>("Code")
                        .HasColumnType("TEXT");

                    b.Property<int>("ContentQuantity")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatorUserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Epc")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastModificationTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("LastModifierUserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Note")
                        .HasColumnType("TEXT");

                    b.Property<string>("OrderReference")
                        .HasColumnType("TEXT");

                    b.Property<string>("Reference")
                        .HasColumnType("TEXT");

                    b.Property<string>("SerialNumber")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("XRFID.Sample.Server.Entities.Reader", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("ActiveMovementId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Code")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatorUserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Ip")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastModificationTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("LastModifierUserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("MacAddress")
                        .HasColumnType("TEXT");

                    b.Property<string>("Model")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("ReaderOS")
                        .HasColumnType("TEXT");

                    b.Property<string>("Reference")
                        .HasColumnType("TEXT");

                    b.Property<string>("SerialNumber")
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Uid")
                        .HasColumnType("TEXT");

                    b.Property<string>("Version")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Readers");
                });

            modelBuilder.Entity("XRFID.Sample.Server.Entities.LoadingUnitItem", b =>
                {
                    b.HasOne("XRFID.Sample.Server.Entities.LoadingUnit", "LoadingUnit")
                        .WithMany("LoadingUnitItems")
                        .HasForeignKey("LoadingUnitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LoadingUnit");
                });

            modelBuilder.Entity("XRFID.Sample.Server.Entities.MovementItem", b =>
                {
                    b.HasOne("XRFID.Sample.Server.Entities.LoadingUnitItem", "LoadingUnitItem")
                        .WithMany()
                        .HasForeignKey("LoadingUnitItemId");

                    b.HasOne("XRFID.Sample.Server.Entities.Movement", "Movement")
                        .WithMany("MovementItems")
                        .HasForeignKey("MovementId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("XRFID.Sample.Server.Entities.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId");

                    b.Navigation("LoadingUnitItem");

                    b.Navigation("Movement");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("XRFID.Sample.Server.Entities.LoadingUnit", b =>
                {
                    b.Navigation("LoadingUnitItems");
                });

            modelBuilder.Entity("XRFID.Sample.Server.Entities.Movement", b =>
                {
                    b.Navigation("MovementItems");
                });
#pragma warning restore 612, 618
        }
    }
}