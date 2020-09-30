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
using Microsoft.Win32;

namespace GalleryAppV2._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AlbumManager albumManager = new AlbumManager();
        private int openAlbumIndex= -1;
        private Dictionary<MediaFile, bool> toggleHelper = new Dictionary<MediaFile, bool>();
        private Slideshow slideshow = new Slideshow();


        public MainWindow()
        {
            InitializeComponent();
            slideshow = new Slideshow();
            InitializeGUI();
        }

        private void InitializeGUI()
        {
            AlbumsTv.ItemsSource = albumManager.GetAlbums();
            slideshow_datagrid.ItemsSource = slideshow.SlideshowItems;
            slideshow = new Slideshow();

        }

        private void NewAlbum_Button_Click(object sender, RoutedEventArgs e)
        {
            NewDialog dialog = new NewDialog();
            dialog.ShowDialog();
            if (dialog.DialogResult == true)
            {
                albumManager.AddNewAlbum(new Album(dialog.GetFolderName(), dialog.GetFolderDescription()));
            }
        }

        private void Edit_Button_Click(object sender, RoutedEventArgs e)
        {
            int index= AlbumsTv.Items.IndexOf(AlbumsTv.SelectedItem);
            MessageBox.Show(index.ToString());            
        }

        private void import_fileDialogue_Click(object sender, RoutedEventArgs e)
        {
            if(openAlbumIndex!=-1 && openAlbumIndex <albumManager.Count())
            {
                Album album = albumManager.GetAlbumAtIndex(openAlbumIndex);
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter =
                    "Image files (*.JPG;*.PNG)|*.JPG;*.PNG|" +
                    "Video files (*.WMV;*.MP4)|*.WMV;*.MP4|" +
                    "All supported files|*.JPG;*.PNG;*.WMV;*.MP4";
                openFileDialog.FilterIndex = 3;
                openFileDialog.Multiselect = true;
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
                            switch (result)
                            {
                                case MessageBoxResult.Yes:
                                    switch (extension)
                                    {
                                        case ".jpg":
                                        case ".png":
                                            album.MediaFiles.Add(new ImageFile(fileName, "", filePath));
                                            break;
                                        case ".wmv":
                                        case ".mp4":
                                            album.MediaFiles.Add(new VideoFile(fileName, "", filePath));
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
                                switch (extension)
                                {
                                    case ".jpg":
                                    case ".png":
                                        album.MediaFiles.Add(new ImageFile(fileName, "", filePath));
                                        break;
                                    case ".wmv":
                                    case ".mp4":
                                        album.MediaFiles.Add(new VideoFile(fileName, "", filePath));
                                        break;
                                }
                            }
                            catch
                            {
                                MessageBox.Show("Error importing file(s) ");
                            }
                        }
                    }
                    //SerializationHelper.Serialize(albumManager);
                }
            }

        }

        private void ShowAlbumContent(int index)
        {            
            Album album= albumManager.GetAlbumAtIndex(index);
            foreach (MediaFile mediaFile in album.MediaFiles) //check if album has files before adding to dictionary

            {
                toggleHelper.Add(mediaFile, false);
            }
                ListViewContent.ItemsSource = album.MediaFiles;
            AlbumName_TextBlock.Text = album.AlbumTitle;
            AlbumDescription_textBlock.Text = album.AlbumDescription;
            
        }
        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = sender as ToggleButton;
            MediaFile mediaFile = toggleButton.DataContext as MediaFile;
            if (toggleHelper[mediaFile])
                toggleHelper[mediaFile] = false;
            else
                toggleHelper[mediaFile] = true;

        }
        private void AlbumsTv_treeviewitem_Selected(object sender, RoutedEventArgs e)
        {
            int index = AlbumsTv.Items.IndexOf(AlbumsTv.SelectedItem);
            openAlbumIndex = index;
            ShowAlbumContent(index);

            
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            
            foreach(KeyValuePair<MediaFile,bool> entry in toggleHelper)
            {
                if (entry.Value)
                    slideshow.SlideshowItems.Add(new SlideshowItem(entry.Key));
            }
            slideshow_datagrid.ItemsSource = slideshow.SlideshowItems;
            MessageBox.Show(slideshow.SlideshowItems.Count.ToString());

        }

        private void Up_Button_Click(object sender, RoutedEventArgs e)
        {
            SlideshowItem slideshowItem = slideshow_datagrid.SelectedItem as SlideshowItem;
            MessageBox.Show(slideshowItem.MediaFile.FileName);
        }
    }
}
