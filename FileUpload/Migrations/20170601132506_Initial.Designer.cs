using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using FileUpload.Models;

namespace FileUpload.Migrations
{
    [DbContext(typeof(FileUploadContext))]
    [Migration("20170601132506_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("FileUpload.Models.File", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("FileContent");

                    b.Property<string>("FileName");

                    b.Property<string>("MD5");

                    b.Property<byte[]>("PreviewImage");

                    b.Property<string>("Type");

                    b.Property<DateTime>("UploadDate");

                    b.HasKey("ID");

                    b.ToTable("File");
                });
        }
    }
}
