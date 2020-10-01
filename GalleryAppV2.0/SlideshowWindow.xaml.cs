using System;
using System.Windows;
using System.Windows.Input;
using BusinessLayer;

namespace GalleryAppV2._0
{
    /// <summary>
    /// Interaction logic for SlideshowWindow.xaml
    /// </summary>
    public partial class SlideshowWindow : Window
    {
        /**autoplay logic is like this: 
       1- Get instance of Slideshow
       2- Loop through each media file element in it 
       3- if the element is image: send it to ImageFrame to play it with a dispatcher timer coupled with notifying event. When tick is over  notify parent window (SlideshowWindow).
          if the element is video: send it to VideoFrame and play it, when media.ended event is called, it will notify parent Window.
       4- when ImagePlayFinished or VideoEnded events are raised: continue with loop and get next item.
       **/
        ImageFrame imageFrame;
        private Slideshow slideshow;
        int currentIndex;
        /// <summary>
        /// Default constructor that takes an instance of Album to show all its content in a slideshow.
        /// </summary>
        /// <param name="a"></param>
        /// 
        public SlideshowWindow(Slideshow s)
        {
            InitializeComponent();
            slideshow = s;
            this.PreviewKeyDown += new KeyEventHandler(EscButtonHandler); //listener to ESC button to skip the slideshow and close the window
            if(slideshow.SlideshowItems.Count > 0)
            {
                currentIndex = 0;
                ShowMediaFileAtIndex(currentIndex);
            }


        }

        /// <summary>
        /// Method to send the media file to the appropriate page and subscribe to their events.
        /// </summary>
        /// <param name="index"></param>
        private void ShowMediaFileAtIndex(int index)
        {
            if (currentIndex >= 0 && currentIndex < slideshow.SlideshowItems.Count)
            {
                if(slideshow.SlideshowItems[index].MediaFile is ImageFile)
                {
                    //ImageFrame imageFrame = new ImageFrame(slideshow.SlideshowItems[index].MediaFile.FilePath,slideshow.SlideshowItems[index].Time);
                    imageFrame = new ImageFrame(slideshow.SlideshowItems[index].MediaFile.FilePath, slideshow.SlideshowItems[index].Time);
                    imageFrame.ImagePlayFinished += OnImagePlayFinished;
                    SlideshowFrame.Content = imageFrame;
                }
                else if (slideshow.SlideshowItems[index].MediaFile is VideoFile)
                    {
                    VideoFrame videoFrame= new VideoFrame(slideshow.SlideshowItems[index].MediaFile.FilePath);
                    videoFrame.VideoPlayFinished += OnVideoPlayFinished;
                    SlideshowFrame.Content = videoFrame;
                }
            }

        }
        private void OnImagePlayFinished(object source, EventArgs args)
        {
            if (slideshow.SlideshowItems[currentIndex].MediaFile is ImageFile)
                PlayNext();
        }
        private void OnVideoPlayFinished(object source, EventArgs args)
        {
            PlayNext();
        }

        private void next_btn_Click(object sender, RoutedEventArgs e)
        {
            PlayNext();
        }

        private void previous_btn_Click(object sender, RoutedEventArgs e)
        {
            if(currentIndex !=0)
            { 
                currentIndex--;
                ShowMediaFileAtIndex(currentIndex);
            }
        }
        private void PlayNext()
        {
            if (imageFrame != null)
            {
                imageFrame.StopTimer();
            }
            if (currentIndex + 1 != slideshow.SlideshowItems.Count)
            {
                currentIndex++;
                ShowMediaFileAtIndex(currentIndex);
            }
        }

        /// <summary>
        /// ESC button handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EscButtonHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }
    }
}
