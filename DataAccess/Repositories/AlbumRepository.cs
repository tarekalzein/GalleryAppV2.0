using BusinessLayer;
using BusinessLayer.Interfaces;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess
{
    public class AlbumRepository : Repository<Album>, IAlbumRepository
    {
        public AlbumRepository(GalleryAppContext context)
            : base (context)
        {
        }

    }
}
