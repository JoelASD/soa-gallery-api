﻿// <auto-generated />
using System;
using ConsoleApp.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SOAImageGalleryAPI.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.4")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("SOAImageGalleryAPI.Models.Comment", b =>
                {
                    b.Property<string>("CommentId")
                        .HasColumnType("text");

                    b.Property<string>("CommentId1")
                        .HasColumnType("text");

                    b.Property<string>("CommentParentID")
                        .HasColumnType("text");

                    b.Property<string>("CommentText")
                        .HasColumnType("text");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("ImageID")
                        .HasColumnType("text");

                    b.Property<DateTime>("Updated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("UserID")
                        .HasColumnType("text");

                    b.HasKey("CommentId");

                    b.HasIndex("CommentId1");

                    b.HasIndex("ImageID");

                    b.HasIndex("UserID");

                    b.ToTable("Comment");
                });

            modelBuilder.Entity("SOAImageGalleryAPI.Models.Image", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("ImageFile")
                        .HasColumnType("text");

                    b.Property<string>("ImageTitle")
                        .HasColumnType("text");

                    b.Property<DateTime>("Updated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("UserID")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserID");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("SOAImageGalleryAPI.Models.User", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("Updated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("UserName")
                        .HasColumnType("text");

                    b.Property<string>("UserPassword")
                        .HasColumnType("text");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SOAImageGalleryAPI.Models.UserHasFavourite", b =>
                {
                    b.Property<string>("FavouriteID")
                        .HasColumnType("text");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("ImageID")
                        .HasColumnType("text");

                    b.Property<DateTime>("Updated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("UserID")
                        .HasColumnType("text");

                    b.HasKey("FavouriteID");

                    b.HasIndex("ImageID");

                    b.HasIndex("UserID");

                    b.ToTable("UserHasFavourite");
                });

            modelBuilder.Entity("SOAImageGalleryAPI.Models.Vote", b =>
                {
                    b.Property<string>("VoteId")
                        .HasColumnType("text");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("ImageID")
                        .HasColumnType("text");

                    b.Property<DateTime>("Updated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("UserID")
                        .HasColumnType("text");

                    b.Property<int>("Voted")
                        .HasColumnType("integer");

                    b.HasKey("VoteId");

                    b.HasIndex("ImageID");

                    b.HasIndex("UserID");

                    b.ToTable("Vote");
                });

            modelBuilder.Entity("SOAImageGalleryAPI.Models.Comment", b =>
                {
                    b.HasOne("SOAImageGalleryAPI.Models.Comment", null)
                        .WithMany("Comments")
                        .HasForeignKey("CommentId1");

                    b.HasOne("SOAImageGalleryAPI.Models.Image", "Image")
                        .WithMany()
                        .HasForeignKey("ImageID");

                    b.HasOne("SOAImageGalleryAPI.Models.User", "User")
                        .WithMany("Comments")
                        .HasForeignKey("UserID");

                    b.Navigation("Image");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SOAImageGalleryAPI.Models.Image", b =>
                {
                    b.HasOne("SOAImageGalleryAPI.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SOAImageGalleryAPI.Models.UserHasFavourite", b =>
                {
                    b.HasOne("SOAImageGalleryAPI.Models.Image", "Image")
                        .WithMany("Favourites")
                        .HasForeignKey("ImageID");

                    b.HasOne("SOAImageGalleryAPI.Models.User", "User")
                        .WithMany("Favourites")
                        .HasForeignKey("UserID");

                    b.Navigation("Image");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SOAImageGalleryAPI.Models.Vote", b =>
                {
                    b.HasOne("SOAImageGalleryAPI.Models.Image", "Image")
                        .WithMany("Votes")
                        .HasForeignKey("ImageID");

                    b.HasOne("SOAImageGalleryAPI.Models.User", "User")
                        .WithMany("Votes")
                        .HasForeignKey("UserID");

                    b.Navigation("Image");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SOAImageGalleryAPI.Models.Comment", b =>
                {
                    b.Navigation("Comments");
                });

            modelBuilder.Entity("SOAImageGalleryAPI.Models.Image", b =>
                {
                    b.Navigation("Favourites");

                    b.Navigation("Votes");
                });

            modelBuilder.Entity("SOAImageGalleryAPI.Models.User", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Favourites");

                    b.Navigation("Votes");
                });
#pragma warning restore 612, 618
        }
    }
}