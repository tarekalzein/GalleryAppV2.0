using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using BusinessLayer;

namespace GalleryAppV2._0.UnitTests
{
    [TestClass]
    public class AlbumManagerTests
    {
        [TestMethod]
        public void GetAlbumAtIndex_IndexWithinRange_ReturnsAlbumInstance()
        {
            AlbumManager albumManager = new AlbumManager();
            Album album = new Album();
            albumManager.AddNewAlbum(album);

            var result = albumManager.GetAlbumAtIndex(0);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetAlbumAtIndex_IndexOutOfRange_ReturnsAlbumInstance()
        {
            AlbumManager albumManager = new AlbumManager();
            Album album = new Album();
            albumManager.AddNewAlbum(album);

            var result = albumManager.GetAlbumAtIndex(1);
            Assert.IsNull(result);
        }
        [TestMethod]
        public void RemoveAlbum_AlbumWithIndexExistInList_RemovesTheAlbum()
        {
            AlbumManager albumManager = new AlbumManager();
            Album album1 = new Album{ AlbumTitle = "album1" };
            albumManager.AddNewAlbum(album1);

            Album album2 = new Album{AlbumTitle = "album2" };
            albumManager.AddNewAlbum(album2);

            albumManager.RemoveAlbum(0);

            Assert.AreNotEqual("album1", albumManager.GetAlbumAtIndex(0));
        }



    }
}
