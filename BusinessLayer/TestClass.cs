using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BusinessLayer
{
    /// <summary>
    /// Test class to generate some albums for testing purpose.
    /// </summary>
    public class TestClass
    {
        ObservableCollection<Album> list = new ObservableCollection<Album>();
        public TestClass()
        {
            list.Add(new Album("Album 1", "Description of Album 1"));
            list.Add(new Album("Album 2", "Description of Album 2"));
            list.Add(new Album("Album 3", "Description of Album 3"));
            list.Add(new Album("Album 4", "Description of Album 4"));
            list.Add(new Album("Album 5", "Description of Album 5"));
            list.Add(new Album("Album 6", "Description of Album 6"));
            list.Add(new Album("Album 7", "Description of Album 7"));
            list.Add(new Album("Album 8", "Description of Album 8"));
            list.Add(new Album("Album 9", "Description of Album 9"));

            list[0].MediaFiles.Add(new ImageFile("iamge 1", "this is an image description", "Assets/test_image.jpg"));
        }

        public ObservableCollection<Album> GetAlbums()
        {
            return list;
        }
    }
}
