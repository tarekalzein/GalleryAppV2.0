using BusinessLayer;
using BusinessLayer.Interfaces;
using BusinessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DataAccess.Repositories
{
    public class MediaFileRepository : Repository<MediaFile>, IMediaFileRepository
    {
        public MediaFileRepository(GalleryAppContext context)
            : base (context)
        {
        }

        public IEnumerable<MediaFile> GetMediaFilesOfAlbum(int albumID)
        {
            return GalleryAppContext.MediaFiles.Where(t => t.AlbumID == albumID);                         
        }

        public GalleryAppContext GalleryAppContext
        {
            get { return Context as GalleryAppContext; }
        }
    }
}
