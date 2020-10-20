using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;
using BusinessLayer;

namespace DataAccess
{
    public class GalleryAppContext : DbContext
    {
        public GalleryAppContext()
        {

        }
        public virtual DbSet<Album> Albums{ get; set; }
        public virtual DbSet<MediaFile> MediaFiles { get; set; }
    }
}
