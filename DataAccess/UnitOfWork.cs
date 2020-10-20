using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessLayer.Interfaces;
using DataAccess.Repositories;

namespace DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GalleryAppContext _context;

        public UnitOfWork(GalleryAppContext context)
        {
            _context = context;
            Albums = new AlbumRepository(_context);
            MediaFiles = new MediaFileRepository(_context);
        }

        public IAlbumRepository Albums { get; private set; }

        public IMediaFileRepository MediaFiles { get; private set; }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

    }
}
