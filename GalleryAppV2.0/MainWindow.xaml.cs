using BusinessLayer;
using DataAccess;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GalleryAppV2._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UnitOfWork unitOfWork = new UnitOfWork(new GalleryAppContext());

        List<MediaFile> albumContent;

        private int openAlbumIndex= -1;
        int currentSlideshowIndex = 0;
        ImageFrame imageFrame;

        /// <summary>
        /// Default constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            //Subscribe to ESC key event. Receiver ESC button press to stop playing slideshow.
            this.PreviewKeyDown += new KeyEventHandler(EscButtonHandler);
            AlbumsTv.ItemsSource = unitOfWork.Albums.GetAll();
        }

        /// <summary>
        /// New Album Button Action. Opens the New Album Dialogue, receives the album title and description from the user
        /// Then uses them to create a new Album instance. Saves changes in data.bin with serialization.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewAlbum_Button_Click(object sender, RoutedEventArgs e)
        {
            NewDialog dialog = new NewDialog();
            dialog.ShowDialog();
            if (dialog.DialogResult == true)
            {
                unitOfWork.Albums.Add(new Album(dialog.GetAlbumName(), dialog.GetAlbumDescription()));
                unitOfWork.Complete();
            }
            AlbumsTv.ItemsSource = unitOfWork.Albums.GetAll();
        }
        /// <summary>
        /// Action method to edit the details of an album. Opens the New Album Dialog with the current title and description. Saves them to Album Manager and serializes them after validation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditAlbum_Button_Click(object sender, RoutedEventArgs e)
        {
            if(AlbumsTv.SelectedItem!=null)
            {

                int id = (AlbumsTv.SelectedItem as Album).AlbumID;

                Album album = unitOfWork.Albums.Get(id);

                NewDialog dialog = new NewDialog(album.AlbumTitle, album.AlbumDescription);
                dialog.ShowDialog();
                if (dialog.DialogResult == true)
                {
                    album.AlbumTitle = dialog.GetAlbumName();
                    album.AlbumDescription = dialog.GetAlbumDescription();

                    unitOfWork.Complete();
                    AlbumsTv.Items.Refresh(); //called because observablecollection will notify only when adding/deleting item from list, but not when changing an item's detail.
                }
            }
        }

        /// <summary>
        /// Action Method to remove an item, album, from the album manager instance.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveAlbum_Button_Click(object sender, RoutedEventArgs e)
        {
            if(AlbumsTv.SelectedItem!=null)
            {                
                unitOfWork.Albums.Remove(AlbumsTv.SelectedItem as Album);
                unitOfWork.Complete();
                AlbumsTv.ItemsSource = unitOfWork.Albums.GetAll();
                //TODO: When deleting all albums in treeview, last album's details are still visible: this must be fixed
                //ListViewContent.ItemsSource = null; //Clear the ListViewContent
                //AlbumName_TextBlock.Text = "";
                //AlbumDescription_textBlock.Text = "Create or choose an album to show its content";
            }
        }

        /// <summary>
        /// Action method for adding new media files to an album.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void import_fileDialogue_Click(object sender, RoutedEventArgs e)
        {
            //TODO: add method to prevent Duplication in code (DRY)
            //Check if an album is already open or not.
            if (openAlbumIndex != -1)
            {
                Album album = unitOfWork.Albums.Get(openAlbumIndex);
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter =
                    "Image files (*.JPG;*.PNG)|*.JPG;*.PNG|" +
                    "Video files (*.WMV;*.MP4)|*.WMV;*.MP4|" +
                    "All supported files|*.JPG;*.PNG;*.WMV;*.MP4";
                openFileDialog.FilterIndex = 3; //Default filter is: All supported fileimport_fileDialogue_Clicks.
                openFileDialog.Multiselect = true; //Users can select multiple media files to import at once.
                if (openFileDialog.ShowDialog() == true)
                {
                    foreach (var filePath in openFileDialog.FileNames)
                    {
                        string extension = Path.GetExtension(filePath);
                        string fileName = Path.GetFileName(filePath);

                        //if (album.MediaFiles.Any(o => o.FilePath == filePath))
                        if (unitOfWork.MediaFiles.GetMediaFilesOfAlbum(openAlbumIndex).Any(o => o.FilePath == filePath))
                        {
                            MessageBox.Show($"{fileName} already exists in this album");
                        }
                        else
                        {
                            try
                            {
                                MediaFile file;
                                switch (extension)
                                {
                                    case ".jpg":
                                    case ".png":
                                        //album.MediaFiles.Add(new ImageFile(fileName, "", filePath));
                                        file = new ImageFile(fileName, "", filePath);
                                        file.AlbumID = openAlbumIndex;
                                        unitOfWork.MediaFiles.Add(file);
                                        unitOfWork.Complete();

                                        break;
                                    case ".wmv":
                                    case ".mp4":
                                        //album.MediaFiles.Add(new VideoFile(fileName, "", filePath));
                                        file = new VideoFile(fileName, "", filePath);
                                        file.AlbumID = openAlbumIndex;
                                        unitOfWork.MediaFiles.Add(file);
                                        unitOfWork.Complete();
                                        break;
                                }
                            }
                            catch
                            {
                                MessageBox.Show("Error importing file(s)");
                            }
                            ShowAlbumContent(openAlbumIndex);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Method that imports all content from an album in the album manager instance and pop it to the album_datagrid.
        /// </summary>
        /// <param name="index"></param>
        private void ShowAlbumContent(int id)
        {                
            Album album= unitOfWork.Albums.Get(id);
            //album_datagrid.ItemsSource = unitOfWork.MediaFiles.GetMediaFilesOfAlbum(id).ToList();
            albumContent = unitOfWork.MediaFiles.GetMediaFilesOfAlbum(id).ToList();
            album_datagrid.ItemsSource = albumContent;
            AlbumName_TextBlock.Text = album.AlbumTitle;
            AlbumDescription_textBlock.Text = album.AlbumDescription;
        }
 
        /// <summary>
        /// Method to call when a user chooses an album from the treeview to show its content in the Album Datagrid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AlbumsTv_treeviewitem_Selected(object sender, RoutedEventArgs e)
        {
            int id = (AlbumsTv.SelectedItem as Album).AlbumID;
            openAlbumIndex = id;
            ShowAlbumContent(id);

            SlideshowFrame.Content = null;
            currentSlideshowIndex = 0;
        }

        ///// <summary>
        /// Method to move an item in the slideshow list (one row up at a time).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Up_Button_Click(object sender, RoutedEventArgs e)
        {
            if (album_datagrid.SelectedItem != null)
            {
                int index = album_datagrid.Items.IndexOf(album_datagrid.SelectedItem);
                if(index > 0)
                {
                    MediaFile item = albumContent[index];
                    albumContent.RemoveAt(index);
                    albumContent.Insert(index - 1, item);
                    album_datagrid.Items.Refresh();
                }
            }
        }

        /// <summary>
        /// Method to move an item in the slideshow list (one row down at a time).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Down_Button_Click(object sender, RoutedEventArgs e)
        {
            if (album_datagrid.SelectedItem != null)
            {
                int index = album_datagrid.Items.IndexOf(album_datagrid.SelectedItem);
                if (index + 1 != albumContent.Count)
                {
                    MediaFile item = albumContent[index];
                    albumContent.RemoveAt(index);
                    albumContent.Insert(index + 1, item);
                    album_datagrid.Items.Refresh();
                }
            }
        }
        /// <summary>
        /// Method to remove a row (slideshow item) from the data grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Remove_from_Grid_Button_Click( object sender, RoutedEventArgs e)
        {
            if (album_datagrid.SelectedItem != null)
            {
                albumContent.Remove(album_datagrid.SelectedItem as MediaFile);
                unitOfWork.MediaFiles.Remove(album_datagrid.SelectedItem as MediaFile);
                unitOfWork.Complete();
                album_datagrid.Items.Refresh();                
            }
        }

        /// <summary>
        /// Method to show the slideshow content in a slideshow window (fullscreen with autoplay).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlaySlideshow_Button_Click(object sender, RoutedEventArgs e)
        {
            currentSlideshowIndex = 0;
            if (albumContent!=null && albumContent.Count > 0)
            {
                ShowMediaFileAtIndex(currentSlideshowIndex);
            }
        }

        /// <summary>
        /// Method to open a selected media file in the windows default viewer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Play_Selected_Item_Click(object sender, RoutedEventArgs e)
        {
            int index = album_datagrid.Items.IndexOf(album_datagrid.SelectedItem);
            //Process.Start(albumManager.GetAlbumAtIndex(openAlbumIndex).MediaFiles[index].FilePath); //I don't know why it throws win32 exception
            var psi = new ProcessStartInfo()
            {
                FileName = albumContent[index].FilePath,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        /// <summary>
        /// Method to show a media file in the slideshow frame.
        /// </summary>
        /// <param name="index">index of the media file from an album.</param>
        private void ShowMediaFileAtIndex(int index)
        {           
            if (currentSlideshowIndex >= 0 && currentSlideshowIndex < albumContent.Count)
            {
                if(albumContent[index].PlayEnabled)
                {
                    if (albumContent[index] is ImageFile)
                    {
                        //ImageFrame imageFrame = new ImageFrame(slideshow.SlideshowItems[index].MediaFile.FilePath,slideshow.SlideshowItems[index].Time);
                        imageFrame = new ImageFrame(albumContent[index].FilePath, albumContent[index].Time);
                        imageFrame.ImagePlayFinished += OnImagePlayFinished;
                        SlideshowFrame.Content = imageFrame;
                    }
                    else if (albumContent[index] is VideoFile)
                    {
                        VideoFrame videoFrame = new VideoFrame(albumContent[index].FilePath);
                        videoFrame.VideoPlayFinished += OnVideoPlayFinished;
                        SlideshowFrame.Content = videoFrame;
                    }
                }
                else { PlayNext(); }                
            }
        }

        /// <summary>
        /// Event to be called when an image has been shown for a given interval
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void OnImagePlayFinished(object source, System.EventArgs args)
        {
                PlayNext();
        }

        /// <summary>
        /// Event to be called when a video has been played until finished
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void OnVideoPlayFinished(object source, EventArgs args)
        {
            PlayNext();
        }

        /// <summary>
        /// Method to show next enabled item in an album on the slideshow frame.
        /// </summary>
        private void PlayNext()
        {
            //Stop timer and nullify the instance of the image page.
            if (imageFrame != null)
            {
                imageFrame.StopTimer();
            }
            if (currentSlideshowIndex + 1 != albumContent.Count)
            {
                currentSlideshowIndex++;
                ShowMediaFileAtIndex(currentSlideshowIndex);
            }
            //Stop the slideshow when playing is finished.
            else if (currentSlideshowIndex + 1 == albumContent.Count)
                StopSlideshow();
        }

        /// <summary>
        /// Action method to navigate to next enabled media file in an album.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void next_btn_Click(object sender, RoutedEventArgs e)
        {
            PlayNext();
        }

        /// <summary>
        /// Action method to navigate to the previous enabled media file in an album.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void previous_btn_Click(object sender, RoutedEventArgs e)
        {
            if (currentSlideshowIndex != 0)
            {
                if (imageFrame != null)
                {
                    imageFrame.StopTimer();
                    imageFrame = null;
                }
                currentSlideshowIndex--;
                ShowMediaFileAtIndex(currentSlideshowIndex);
            }
        }

        /// <summary>
        /// ESC button event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EscButtonHandler(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Escape)
            {
                StopSlideshow();
            }
        }

        /// <summary>
        /// Method to stop the slideshow and the timers.
        /// </summary>
        private void StopSlideshow()
        {
            if(imageFrame!=null)
            {
                imageFrame.StopTimer();
                imageFrame = null;
            }
            currentSlideshowIndex = 0;
            SlideshowFrame.Content = null;
            MessageBox.Show("Slideshow finished playing");
        }

        /// <summary>
        /// Method to Enable/disable all items in the datagrid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Toggle_Selection(object sender, RoutedEventArgs e)
        {
            if(albumContent.All(x => x.PlayEnabled))
            {
                foreach(MediaFile mediaFile in albumContent)
                {
                    mediaFile.PlayEnabled = false;
                    unitOfWork.MediaFiles.Update(mediaFile.FileID, mediaFile);
                }               
            }
            else
            {
                foreach (MediaFile mediaFile in albumContent)
                {
                    mediaFile.PlayEnabled = true;
                    unitOfWork.MediaFiles.Update(mediaFile.FileID, mediaFile);
                }
            }
            unitOfWork.Complete();
            album_datagrid.Items.Refresh();
        }

        /// <summary>
        /// Method to show open file dialog to let user open a bin file to load data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Open_Menu_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage;

            //Import saved data from Data.bin file
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Bin file (*.bin)|*.bin";
            if(openFileDialog.ShowDialog()==true)
            {
                //albumManager = SerializationHelper.Deserialize(openFileDialog.FileName, out errorMessage);
                //if (!string.IsNullOrEmpty(errorMessage))
                //{
                //    MessageBox.Show(errorMessage);
                //}
                //AlbumsTv.ItemsSource = albumManager.GetAlbums();

            }

        }

        /// <summary>
        /// Method to show save file dialog to let user save a bin file that contains the albums.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SaveAs_Menu_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Bin file (*.bin)|*.bin";
            if(saveFileDialog.ShowDialog() == true)
            {
                //SerializationHelper.Serialize(albumManager, saveFileDialog.FileName);
            }
        }

        /// <summary>
        /// Exits the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Exit_Menu_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Ctrl+O Commmand event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Open_CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Open_Menu_Click(sender, e);
        }

        /// <summary>
        /// Ctrl+O Commmand event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveAs_CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
           SaveAs_Menu_Click(sender, e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            unitOfWork.Dispose();
        }

        private void album_datagrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            MediaFile mediaFile = album_datagrid.SelectedItem as MediaFile;
            unitOfWork.MediaFiles.Update(mediaFile.FileID, mediaFile) ;
            unitOfWork.Complete();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            MediaFile mediaFile = album_datagrid.SelectedItem as MediaFile;
            unitOfWork.MediaFiles.Update(mediaFile.FileID, mediaFile);
            unitOfWork.Complete();
        }
        private void OnSearchTextBoxTextChanged(object sender, RoutedEventArgs e)
        {
            if( searchTextBox.Text =="")
            {
                AlbumsTv.ItemsSource = unitOfWork.Albums.GetAll();
            }
            else
            {
                AlbumsTv.ItemsSource = unitOfWork.Albums.GetAll().Where(x => x.AlbumTitle.ToLower().Contains(searchTextBox.Text.ToLower()));
            }
        }
    }
}
