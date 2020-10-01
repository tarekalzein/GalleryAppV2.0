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
            Time = 5;//Default time in sec.
        }
    }
}
