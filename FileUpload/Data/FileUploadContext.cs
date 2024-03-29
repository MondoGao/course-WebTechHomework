﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FileUpload.Models
{
    public class FileUploadContext : DbContext
    {
        public FileUploadContext (DbContextOptions<FileUploadContext> options)
            : base(options)
        {
        }

        public DbSet<FileUpload.Models.File> File { get; set; }
        public DbSet<FileUpload.Models.Order> Order { get; set; }
    }
}
