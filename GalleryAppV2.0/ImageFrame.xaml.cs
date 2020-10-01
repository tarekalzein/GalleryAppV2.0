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
        private DispatcherTimer timer = new DispatcherTimer();
        public delegate void ImageEventHandler(object source, EventArgs args);
        public event ImageEventHandler ImagePlayFinished;


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

        void timer_tick(object sender, EventArgs e)
        {
            OnImagePlayFinished();
        }

        private BitmapImage ImagePathToSourceConverter(string imagePath)
        {
            return new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
        }

        protected virtual void OnImagePlayFinished()
        {
            if(ImagePlayFinished != null)
            {
                ImagePlayFinished(this, EventArgs.Empty);
                timer.Stop();
            }
        }

        public void StopTimer()
        {
            timer.Stop();
        }
    }
}
