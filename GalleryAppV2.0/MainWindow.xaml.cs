﻿using BusinessLayer;
using DataAccess;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace GalleryAppV2._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {      
        AlbumManager albumManager ;
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
            albumManager = new AlbumManager();
            AlbumsTv.ItemsSource = albumManager.GetAlbums();
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
                albumManager.AddNewAlbum(new Album(dialog.GetAlbumName(), dialog.GetAlbumDescription()));
            }
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
                int index = AlbumsTv.Items.IndexOf(AlbumsTv.SelectedItem);

                Album album = albumManager.GetAlbumAtIndex(index);

                NewDialog dialog = new NewDialog(album.AlbumTitle, album.AlbumDescription);
                dialog.ShowDialog();
                if (dialog.DialogResult == true)
                {
                    album.AlbumTitle = dialog.GetAlbumName();
                    album.AlbumDescription = dialog.GetAlbumDescription();

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
                int index = AlbumsTv.Items.IndexOf(AlbumsTv.SelectedItem);
                albumManager.RemoveAlbum(index);

                //TODO: When deleting all albums in treeview, last album's details are still visible: this must be fixed
                //ListViewContent.ItemsSource = new Album().MediaFiles; //Clear the ListViewContent
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
            if (openAlbumIndex != -1 && openAlbumIndex < albumManager.Count())
            {
                Album album = albumManager.GetAlbumAtIndex(openAlbumIndex);
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter =
                    "Image files (*.JPG;*.PNG)|*.JPG;*.PNG|" +
                    "Video files (*.WMV;*.MP4)|*.WMV;*.MP4|" +
                    "All supported files|*.JPG;*.PNG;*.WMV;*.MP4";
                openFileDialog.FilterIndex = 3; //Default filter is: All supported files.
                openFileDialog.Multiselect = true; //Users can select multiple media files to import at once.
                if (openFileDialog.ShowDialog() == true)
                {
                    foreach (var filePath in openFileDialog.FileNames)
                    {
                        string extension = Path.GetExtension(filePath);
                        string fileName = Path.GetFileName(filePath);

                        if (album.MediaFiles.Any(o => o.FilePath == filePath))
                        {
                            MessageBoxResult result = MessageBox.Show($"{fileName} already exists in this album, would you like to add it anyway?",
                                    "File already exists",
                                    MessageBoxButton.YesNo);
                            MediaFile file;
                            switch (result)
                            {
                                case MessageBoxResult.Yes:
                                    switch (extension)
                                    {
                                        case ".jpg":
                                        case ".png":
                                            file = new ImageFile(fileName, "", filePath);
                                            album.MediaFiles.Add(file);
                                            break;
                                        case ".wmv":
                                        case ".mp4":
                                            file = new VideoFile(fileName, "", filePath);
                                            album.MediaFiles.Add(file);
                                            break;
                                    }
                                    break;
                                case MessageBoxResult.No:
                                    //Do Nothing
                                    break;
                            }
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
                                        album.MediaFiles.Add(file);
                                        break;
                                    case ".wmv":
                                    case ".mp4":
                                        //album.MediaFiles.Add(new VideoFile(fileName, "", filePath));
                                        file = new VideoFile(fileName, "", filePath);
                                        album.MediaFiles.Add(file);
                                        break;
                                }
                            }
                            catch
                            {
                                MessageBox.Show("Error importing file(s) ");
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Method that imports all content from an album in the album manager instance and pop it to the album_datagrid.
        /// </summary>
        /// <param name="index"></param>
        private void ShowAlbumContent(int index)
        {            
            Album album= albumManager.GetAlbumAtIndex(index);
            album_datagrid.ItemsSource = album.MediaFiles;
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
            int index = AlbumsTv.Items.IndexOf(AlbumsTv.SelectedItem);
            openAlbumIndex = index;
            ShowAlbumContent(index);

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
                Album album = albumManager.GetAlbumAtIndex(openAlbumIndex);
                if(index!=0)
                {
                    album.MediaFiles.Move(index, index - 1);
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
                Album album = albumManager.GetAlbumAtIndex(openAlbumIndex);

                if (index + 1 != album.MediaFiles.Count)
                    album.MediaFiles.Move(index, index + 1);
            }
        }

        /// <summary>
        /// Method to remove a row (slideshow item) from the data grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Remove_from_Grid_Button_Click( object sender, RoutedEventArgs e)
        {
            if(album_datagrid.SelectedItem!=null)
            {
                int index = album_datagrid.Items.IndexOf(album_datagrid.SelectedItem);
                albumManager.GetAlbumAtIndex(openAlbumIndex).MediaFiles.RemoveAt(index);
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
            Album album = albumManager.GetAlbumAtIndex(openAlbumIndex);
            if (album.MediaFiles.Count >0)
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
            var psi = new ProcessStartInfo() {
                FileName = albumManager.GetAlbumAtIndex(openAlbumIndex).MediaFiles[index].FilePath,
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
            Album album = albumManager.GetAlbumAtIndex(openAlbumIndex);
            if (currentSlideshowIndex >= 0 && currentSlideshowIndex < album.MediaFiles.Count)
            {
                if(album.MediaFiles[index].PlayEnabled)
                {
                    if (album.MediaFiles[index] is ImageFile)
                    {
                        //ImageFrame imageFrame = new ImageFrame(slideshow.SlideshowItems[index].MediaFile.FilePath,slideshow.SlideshowItems[index].Time);
                        imageFrame = new ImageFrame(album.MediaFiles[index].FilePath, album.MediaFiles[index].Time);
                        imageFrame.ImagePlayFinished += OnImagePlayFinished;
                        SlideshowFrame.Content = imageFrame;
                    }
                    else if (album.MediaFiles[index] is VideoFile)
                    {
                        VideoFrame videoFrame = new VideoFrame(album.MediaFiles[index].FilePath);
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
            if (currentSlideshowIndex + 1 != albumManager.GetAlbumAtIndex(openAlbumIndex).MediaFiles.Count)
            {
                currentSlideshowIndex++;
                ShowMediaFileAtIndex(currentSlideshowIndex);
            }
            //Stop the slideshow when playing is finished.
            else if (currentSlideshowIndex + 1 == albumManager.GetAlbumAtIndex(openAlbumIndex).MediaFiles.Count)
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
            Album album = albumManager.GetAlbumAtIndex(openAlbumIndex);
            if(album.MediaFiles.All(x => x.PlayEnabled))
            {
                foreach(MediaFile mediaFile in album.MediaFiles)
                {
                    mediaFile.PlayEnabled = false;
                }    
            }
            else
            {
                foreach (MediaFile mediaFile in album.MediaFiles)
                {
                    mediaFile.PlayEnabled = true;
                }
            }
            //album_datagrid.Items.Refresh(); //Caused the app to crash if toggle button is triggered after editing rows.
            album_datagrid.ItemsSource = null;
            album_datagrid.ItemsSource = album.MediaFiles;
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
                albumManager = SerializationHelper.Deserialize(openFileDialog.FileName, out errorMessage);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    MessageBox.Show(errorMessage);
                }
                AlbumsTv.ItemsSource = albumManager.GetAlbums();

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
                SerializationHelper.Serialize(albumManager, saveFileDialog.FileName);
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
    }
}
