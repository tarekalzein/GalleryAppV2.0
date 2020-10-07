using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BusinessLayer
{
    /// <summary>
    /// Class that represents a slideshow and its content. Instance of this class can be sent to the SlideshowWindow to play its content.
    /// </summary>
    public class Slideshow
    {
        public ObservableCollection<SlideshowItem> SlideshowItems { get; set; }
        /// <summary>
        /// Constructor that initializes the list.
        /// </summary>
        public Slideshow()
        {
            SlideshowItems = new ObservableCollection<SlideshowItem>();
        }
    }
    /// <summary>
    /// Class of each slideshow item that adds one property to the media file: a time interval for how long would an image would be shown.
    /// </summary>
    public class SlideshowItem
    {
        public MediaFile MediaFile { get; set; }
        public int Time { get; set; }

        public FileTypes FileType { get; set; }

        public SlideshowItem(MediaFile mediaFile)
        {
            MediaFile = mediaFile;
            Time = 5;//Default time in sec.
            FileType = (mediaFile is ImageFile) ? FileTypes.Image : FileTypes.Video;
        }
    }

    public enum FileTypes
    {
        Image,
        Video
    }
}
