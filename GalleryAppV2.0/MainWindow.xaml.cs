﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using BusinessLayer;
using DataAccess;
using Microsoft.Win32;

namespace GalleryAppV2._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //AlbumManager albumManager = new AlbumManager();
        AlbumManager albumManager ;

        private int openAlbumIndex= -1;
        private Dictionary<MediaFile, bool> toggleHelper = new Dictionary<MediaFile, bool>(); //Helper dictionary to add toggle property/state for a media file toggle button.
        private Slideshow slideshow;


        public MainWindow()
        {
            InitializeComponent();

            MyInitialization();
        }
        /// <summary>
        /// Method to finish initialization of the application..
        /// In this step: desrialization of the saved data (from data.bin) if it exists.
        /// </summary>
        private void MyInitialization()
        {
            string errorMessage;

            //Import saved data from Data.bin file
            albumManager = SerializationHelper.Deserialize(out errorMessage);
            if(!string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(errorMessage);
            }

            slideshow = new Slideshow();

            AlbumsTv.ItemsSource = albumManager.GetAlbums();
            slideshow_datagrid.ItemsSource = slideshow.SlideshowItems;
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
                SerializationHelper.Serialize(albumManager);
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
                    SerializationHelper.Serialize(albumManager);
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
                SerializationHelper.Serialize(albumManager);

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
            //Check if an album is already open or not.
            if(openAlbumIndex!=-1 && openAlbumIndex <albumManager.Count())
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
                                            toggleHelper.Add(file, false); break;
                                        case ".wmv":
                                        case ".mp4":
                                            file = new VideoFile(fileName, "", filePath);
                                            album.MediaFiles.Add(file);
                                            toggleHelper.Add(file, false); break;
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
                                        toggleHelper.Add(file, false);
                                        break;
                                    case ".wmv":
                                    case ".mp4":
                                        //album.MediaFiles.Add(new VideoFile(fileName, "", filePath));
                                        file = new VideoFile(fileName, "", filePath);
                                        album.MediaFiles.Add(file);
                                        toggleHelper.Add(file, false);
                                        break;
                                }
                            }
                            catch
                            {
                                MessageBox.Show("Error importing file(s) ");
                            }
                        }
                    }
                    SerializationHelper.Serialize(albumManager);
                }
            }
        }
        /// <summary>
        /// Method that imports all content from an album in the album manager instance.
        /// Adds the album content to a dictionary to add a toggle property (bool) to be used later in selecting the files.
        /// </summary>
        /// <param name="index"></param>
        private void ShowAlbumContent(int index)
        {            
            Album album= albumManager.GetAlbumAtIndex(index);
            toggleHelper.Clear();
            foreach (MediaFile mediaFile in album.MediaFiles) //check if album has files before adding to dictionary
            {
                toggleHelper.Add(mediaFile, false);
            }

            ListViewContent.ItemsSource = album.MediaFiles;
            AlbumName_TextBlock.Text = album.AlbumTitle;
            AlbumDescription_textBlock.Text = album.AlbumDescription;            
        }
        /// <summary>
        /// Method to call when toggle event is raised. It switches between the toggle button state. (of the media file).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = sender as ToggleButton;
            MediaFile mediaFile = toggleButton.DataContext as MediaFile;
            if (toggleHelper[mediaFile])
                toggleHelper[mediaFile] = false;
            else
                toggleHelper[mediaFile] = true;
        }
        /// <summary>
        /// Method to call when a user chooses an album from the treeview to show its content in the ListViewContent.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AlbumsTv_treeviewitem_Selected(object sender, RoutedEventArgs e)
        {
            int index = AlbumsTv.Items.IndexOf(AlbumsTv.SelectedItem);
            openAlbumIndex = index;
            ShowAlbumContent(index);
        }
        /// <summary>
        /// Action method to add the selected (toggled) media files to the slideshow instance.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {            
            foreach(KeyValuePair<MediaFile,bool> entry in toggleHelper)
            {
                if (entry.Value)
                {
                    slideshow.SlideshowItems.Add(new SlideshowItem(entry.Key));
                }
            }

            slideshow_datagrid.ItemsSource = slideshow.SlideshowItems;

            //To un-toggle all buttons after adding items to the slideshow.
            toggleHelper = toggleHelper.ToDictionary(p => p.Key, p => false); //re-set all values to false after un-toggle.
            ListViewContent.Items.Refresh(); 
        }
        /// <summary>
        /// Method to move an item in the slideshow list (one row up at a time).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Up_Button_Click(object sender, RoutedEventArgs e)
        {
            if (slideshow_datagrid.SelectedItem != null)
            {
                int index = slideshow_datagrid.Items.IndexOf(slideshow_datagrid.SelectedItem);
                if(index!=0)
                {
                    slideshow.SlideshowItems.Move(index, index - 1);
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
            if (slideshow_datagrid.SelectedItem != null)
            {
                int index = slideshow_datagrid.Items.IndexOf(slideshow_datagrid.SelectedItem);
                if (index + 1 != slideshow.SlideshowItems.Count)
                    slideshow.SlideshowItems.Move(index, index + 1);
            }
        }
        /// <summary>
        /// Method to remove a row (slideshow item) from the data grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Remove_from_Grid_Button_Click( object sender, RoutedEventArgs e)
        {
            if(slideshow_datagrid.SelectedItem!=null)
            {
                int index = slideshow_datagrid.Items.IndexOf(slideshow_datagrid.SelectedItem);
                slideshow.SlideshowItems.RemoveAt(index);
            }
        }
        /// <summary>
        /// Method to show the slideshow content in a slideshow window (fullscreen with autoplay).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlaySlideshow_Button_Click(object sender, RoutedEventArgs e)
        {
            if(slideshow.SlideshowItems.Count >0)
            {
                SlideshowWindow slideShowPlayer = new SlideshowWindow(slideshow);
                slideShowPlayer.Show();
            }
        }
    }
}
