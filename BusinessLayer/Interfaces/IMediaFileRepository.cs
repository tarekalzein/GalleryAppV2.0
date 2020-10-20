using BusinessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Interfaces
{
    public interface IMediaFileRepository : IRepository<MediaFile>
    {
        IEnumerable<MediaFile> GetMediaFilesOfAlbum(int albumID);
    }
}
