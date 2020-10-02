using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GalleryAppV2._0
{
    /// <summary>
    /// Interaction logic for ImageFrame.xaml
    /// </summary>
    public partial class ImageFrame : Page
    {
        //Ticker to show an image for a predefined interval.
        private DispatcherTimer timer = new DispatcherTimer();
        //Delegate to notify subscribers that an image has been played 
        public delegate void ImageEventHandler(object source, EventArgs args);
        public event ImageEventHandler ImagePlayFinished;

        /// <summary>
        /// Constructor that takes parameter imagePath to use it and show the image in the frame within the timer defined by time.
        /// </summary>
        /// <param name="imagePath">String of the Image file path.</param>
        /// <param name="time">The time interval that an image should be played for</param>
        public ImageFrame(string imagePath,int time)
        {
            InitializeComponent();
            {
                timer.Interval = TimeSpan.FromSeconds(time);
                timer.Tick += new EventHandler(timer_tick);
                timer.Start();
            }
            if (!string.IsNullOrEmpty(imagePath))
                image.Source = ImagePathToSourceConverter(imagePath);
        }
        /// <summary>
        /// Ticker event: it will call the delegate to notify subcsribers about the end of the dispatcher timer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_tick(object sender, EventArgs e)
        {
            OnImagePlayFinished();
        }
        /// <summary>
        /// Convert file path string to bitmapimage.
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        private BitmapImage ImagePathToSourceConverter(string imagePath)
        {
            return new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
        }
        /// <summary>
        /// Event: to call when an imagePlay event is finished
        /// </summary>
        protected virtual void OnImagePlayFinished()
        {
            if(ImagePlayFinished != null)
            {
                ImagePlayFinished(this, EventArgs.Empty);
                timer.Stop();
            }
        }
        /// <summary>
        /// Method to abruptly stop the timer.
        /// </summary>
        public void StopTimer()
        {
            timer.Stop();
        }
    }
}
