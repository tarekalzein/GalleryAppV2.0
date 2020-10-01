using System;
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
        private Dictionary<MediaFile, bool> toggleHelper = new Dictionary<MediaFile, bool>(); //Helper dictionary to add toggle property/state for a media file toggle button.
        private Slideshow slideshow;


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
        }

        private void NewAlbum_Button_Click(object sender, RoutedEventArgs e)
        {
            NewDialog dialog = new NewDialog();
            dialog.ShowDialog();
            if (dialog.DialogResult == true)
            {
                albumManager.AddNewAlbum(new Album(dialog.GetAlbumName(), dialog.GetAlbumDescription()));
            }
        }

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

        private void RemoveAlbum_Button_Click(object sender, RoutedEventArgs e)
        {
            if(AlbumsTv.SelectedItem!=null)
            {
                int index = AlbumsTv.Items.IndexOf(AlbumsTv.SelectedItem);
                albumManager.RemoveAlbum(index);
            }
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
                    //SerializationHelper.Serialize(albumManager);
                }
            }
        }

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
        }

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

        private void Down_Button_Click(object sender, RoutedEventArgs e)
        {
            if (slideshow_datagrid.SelectedItem != null)
            {
                int index = slideshow_datagrid.Items.IndexOf(slideshow_datagrid.SelectedItem);
                if (index + 1 != slideshow.SlideshowItems.Count)
                    slideshow.SlideshowItems.Move(index, index + 1);

            }
        }
        private void Remove_from_Grid_Button_Click( object sender, RoutedEventArgs e)
        {
            if(slideshow_datagrid.SelectedItem!=null)
            {
                int index = slideshow_datagrid.Items.IndexOf(slideshow_datagrid.SelectedItem);
                slideshow.SlideshowItems.RemoveAt(index);
            }
        }
    }
}
