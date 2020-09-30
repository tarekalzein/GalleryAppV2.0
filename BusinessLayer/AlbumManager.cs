using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BusinessLayer
{
    [Serializable]
    public class AlbumManager
    {

        //Changed to ObservableCollection to eliminate the need to update ListViews.
        ObservableCollection<Album> albumList = new ObservableCollection<Album>();
        /// <summary>
        /// Empty constructor to be used with serialization.
        /// </summary>
        public AlbumManager()
        {
            albumList = new TestClass().GetAlbums();
        }
        /// <summary>
        /// Method to retrieve a list of all albums in the current album manager.
        /// Method to retrieve a list of all albums in the current album manager.
        /// </summary>
        /// <returns>List of all albums in album manager instance.</returns>
        public ObservableCollection<Album> GetAlbums()
        {
            return albumList;
        }

        /// <summary>
        /// Method to add new album to the album manager.
        /// </summary>
        /// <param name="album">Instance of Album to be added to the list.</param>
        public void AddNewAlbum(Album album)
        {
            albumList.Add(album);
        }
        /// <summary>
        /// Remove an album from the album manager
        /// </summary>
        /// <param name="index">Index of the targeted album</param>
        public void RemoveAlbum(int index)
        {
            //todo: add check for index (out of range exception);
            albumList.RemoveAt(index);
        }
        /// <summary>
        /// Method to retreieve a single album at a specific index
        /// </summary>
        /// <param name="index">index of the desired album</param>
        /// <returns>Instance of Album</returns>
        public Album GetAlbumAtIndex(int index)
        {
            return albumList[index];
        }
        public int Count()
        {
            return albumList.Count;
        }
    }
}
