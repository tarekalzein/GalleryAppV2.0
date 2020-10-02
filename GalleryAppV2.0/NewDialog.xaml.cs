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
using System.Windows.Shapes;

namespace GalleryAppV2._0
{
    /// <summary>
    /// Interaction logic for NewDialog.xaml
    /// </summary>
    public partial class NewDialog : Window
    {
        /// <summary>
        /// Contructor to open dialog to create a new album
        /// </summary>
        public NewDialog()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Constructor that takes details from an album instance to edit it.
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="oldDescription"></param>
        public NewDialog(string oldName,string oldDescription)
        {
            InitializeComponent();
            AlbumName_txtbox.Text = oldName;
            AlbumDescription_txtbox.Text = oldDescription;
        }
        /// <summary>
        /// Action method to call when pressing the ok button in the dialog. returns true as dialog result.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ok_Button_Click(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(AlbumName_txtbox.Text))
            {
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("Folder name can't be empty");
            }
        }
        /// <summary>
        /// Method to return false when user presses the cancel button in the dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        /// <summary>
        /// Method to get the Album title from the dialog.
        /// </summary>
        /// <returns></returns>
        public string GetAlbumName()
        {
            return AlbumName_txtbox.Text;
        }
        /// <summary>
        /// Method to get the Album description from the dialog.
        /// </summary>
        /// <returns></returns>
        public string GetAlbumDescription()
        {
            if (!string.IsNullOrEmpty(AlbumDescription_txtbox.Text))
                return AlbumDescription_txtbox.Text;
            else
                return ""; //empty description
        }
    }
}
