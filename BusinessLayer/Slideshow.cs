using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BusinessLayer
{
    public class Slideshow
    {
        public ObservableCollection<SlideshowItem> SlideshowItems { get; set; }
        public Slideshow()
        {
            SlideshowItems = new ObservableCollection<SlideshowItem>();
            //SlideshowItems.Add(new SlideshowItem(new ImageFile("iamge 1", "this is an image description", "Assets/test_image.jpg")));
        }
    }

    public class SlideshowItem
    {
        public MediaFile MediaFile { get; set; }
        public int Time { get; set; }

        public SlideshowItem()
        {

        }
        public SlideshowItem(MediaFile mediaFile)
        {
            MediaFile = mediaFile;
            Time = 0;
        }
    }
}
