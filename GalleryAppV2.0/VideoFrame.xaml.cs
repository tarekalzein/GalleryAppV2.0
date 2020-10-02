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

namespace GalleryAppV2._0
{
    /// <summary>
    /// Interaction logic for VideoFrame.xaml
    /// </summary>
    public partial class VideoFrame : Page
    {
        MediaPlayer mediaPlayer;
        public delegate void VideoEventHandler(object source, EventArgs args);
        public event VideoEventHandler VideoPlayFinished;
        public VideoFrame(string videoPath)
        {
            InitializeComponent();
            mediaPlayer = new MediaPlayer();
            if (!string.IsNullOrEmpty(videoPath))
                video.Source = new System.Uri(videoPath);
        }
        protected virtual void OnVideoPlayFinished()
        {
            if (VideoPlayFinished != null)
            {
                VideoPlayFinished(this, EventArgs.Empty);
            }
        }
        private void video_MediaEnded(object sender, System.Windows.RoutedEventArgs e)
        {
            OnVideoPlayFinished();
        }
    }
}
