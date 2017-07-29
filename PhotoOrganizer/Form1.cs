using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoOrganizer
{
    public partial class PhotoOrganizer : Form
    {
        public PhotoOrganizer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// select the source folder name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectSource_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog PhotoSourceFolder = new FolderBrowserDialog();
            string[] sourceFiles = null;
            string[] sourceFolders = null;
            string destinationFolder = string.Empty;
            DialogResult result = PhotoSourceFolder.ShowDialog();
            try
            {
                if (!string.IsNullOrWhiteSpace(PhotoSourceFolder.SelectedPath))
                {
                    sourceFiles = Directory.GetFiles(PhotoSourceFolder.SelectedPath);
                    sourceFolders = Directory.GetDirectories(PhotoSourceFolder.SelectedPath);
                    if (sourceFiles.Count() == 0 && sourceFolders.Count() == 0)
                    {
                        MessageBox.Show("No files found in the selected folder, please select another folder.");
                    }
                    else
                    {
                        txtPhotoSource.Text = PhotoSourceFolder.SelectedPath.Trim();
                    }
                }
                else
                {
                    MessageBox.Show("Please select the valid folder.");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Exception ocured, please select the source folder.");
            }
        }

        /// <summary>
        /// select the destinatoin folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectDestination_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog PhotoDestinationFolder = new FolderBrowserDialog();
            string destinationFolder = string.Empty;
            DialogResult result = PhotoDestinationFolder.ShowDialog();
            try
            {
                if (!string.IsNullOrWhiteSpace(PhotoDestinationFolder.SelectedPath))
                {
                    if (Directory.Exists(PhotoDestinationFolder.SelectedPath))
                        txtDestinationFolder.Text = PhotoDestinationFolder.SelectedPath.Trim();
                    else
                    {
                        MessageBox.Show("Please select valid Folder.");
                    }
                }
                else
                {
                    MessageBox.Show("Please select valid folder.");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Exception ocured, please select the destination folder.");
            }

        }
        /// <summary>
        /// select all the photos and videos from the source folder and copy that to the destination folder based on the montha and year.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOrganize_Click(object sender, EventArgs e)
        {
            try
            {
                lblResult.Text = string.Empty;
                string sourcePath = txtPhotoSource.Text.Trim();
                string destinationPath = txtDestinationFolder.Text.Trim();
                string mediafolder = string.Empty;
                // regular expression for extensions matching pattern
                int sourcecount = 0;
                int movedcount = 0;
                string[] sourceFolders = null;
                // stores regular-expression match result
                //Match matchResult;
                if (!string.IsNullOrEmpty(sourcePath) && !string.IsNullOrEmpty(destinationPath))
                {

                    sourceFolders = Directory.GetDirectories(sourcePath);

                    List<string> sourceFiles = new List<string>();
                    sourceFiles = Directory.GetFiles(sourcePath).ToList<string>();
                    //List<string> sourceFiles = new List<string> ();


                    DirSearch(sourcePath, ref sourceFiles);

                    sourcecount = sourceFiles.Count();
                    foreach (var item in sourceFiles)
                    {
                        if (File.Exists(item))
                        {
                            FileInfo fileinfo = new FileInfo(item);
                            //if fileinfo.Extension.Equals
                            // remove directory path from file name
                            string extension = item.Substring(item.LastIndexOf(@"\") + 1).ToUpper();
                            if (extension.Contains(".3GP") || extension.Contains(".MP4") || extension.Contains(".MOV")
                                || extension.Contains(".MOV") || extension.Contains(".AVI") || extension.Contains(".FLV")
                                || extension.Contains(".mp4") || extension.Contains(".MP4") || extension.Contains(".3gp") || extension.Contains(".3GP") || extension.Contains(".WMV")
                                || extension.Contains(".m4v") || extension.Contains(".M4V") || extension.Contains(".MPEG") || extension.Contains(".mpeg")
                                || extension.Contains(".MPG"))
                            {
                                mediafolder = "Orgnized Videos";
                            }
                            else if (extension.Contains(".JPG") || extension.Contains(".JPEG") || extension.Contains(".PNG")
                                || extension.Contains(".TIFF") || extension.Contains(".TIF"))
                            {
                                mediafolder = "Organized Pics";
                            }
                            else if (extension.Contains(".NEF"))
                            {
                                mediafolder = "Organized RAWs";
                            }
                            if (mediafolder != string.Empty)
                            {
                                //Orgnized Videos

                                string folder = fileinfo.LastWriteTime.Year.ToString().Trim();
                                int subFolder = fileinfo.LastWriteTime.Month;
                                string subFolderName = getMonthName(subFolder);
                                string destinationFolderPath = destinationPath + "\\" + mediafolder + "\\" + folder + "\\" + subFolderName;
                                if (!Directory.Exists(destinationFolderPath))
                                {
                                    Directory.CreateDirectory(destinationFolderPath);
                                }
                                string destinationfilename = destinationFolderPath + "\\" + extension;
                                Timer time = new Timer();

                                if (File.Exists(destinationfilename))
                                {
                                    Random rnd = new Random();
                                    int card = rnd.Next(52);
                                    destinationfilename = destinationFolderPath + "\\" + subFolder + subFolderName + card + extension;
                                }
                                try
                                {
                                    File.Move(item, destinationfilename);
                                    movedcount = movedcount + 1;
                                }
                                catch (IOException ex)
                                {
                                    //ignore
                                }

                            }
                        }

                    }
                    MessageBox.Show("Organize completed.");
                    lblResult.Text = "Total Files in the Source File: " + sourcecount + "\n" + "Toal Files Moved: " + movedcount;
                    lblResult.Visible = true;
                }
                else
                {
                    MessageBox.Show("Please select Source and Destination folder.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception ouccured:" + ex.Message);
            }
        }



        void DirSearch(string sDir, ref List<string> lstFilesFound)
        {
            // List<string> lstFilesFound = new List<string>();
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    foreach (string f in Directory.GetFiles(d))
                    {
                        string filename = f.ToUpper();
                        if (f.Contains(".3GP") || f.Contains(".MP4") || f.Contains(".MOV")
                               || f.Contains(".MOV") || f.Contains(".AVI") || f.Contains(".FLV")
                               || f.Contains(".mp4") || f.Contains(".MP4") || f.Contains(".3gp") || f.Contains(".3GP") || f.Contains(".WMV")
                               || f.Contains(".m4v") || f.Contains(".M4V") || f.Contains(".MPEG") || f.Contains(".mpeg")
                               || f.Contains(".MPG") || f.Contains(".MTS")
                               || f.Contains(".JPG") || f.Contains(".JPEG") || f.Contains(".PNG")
                               || f.Contains(".TIFF") || f.Contains(".TIF") || f.Contains(".jpg") || f.Contains(".jpeg") || f.Contains(".png")
                               || f.Contains(".tiff") || f.Contains(".tif")
                               || f.Contains(".NEF") || f.Contains(".nef"))
                        {

                            lstFilesFound.Add(f);
                        }

                    }
                    DirSearch(d, ref lstFilesFound);
                }

            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);

            }

        }

        /// <summary>
        /// get the month name based on the int value
        /// </summary>
        /// <param name="subFolder"></param>
        /// <returns></returns>
        public string getMonthName(int subFolder)
        {
            string subFolderName = string.Empty;
            switch (subFolder)
            {
                case 1:
                    subFolderName = "Jan";
                    break;
                case 2:
                    subFolderName = "Feb";
                    break;
                case 3:
                    subFolderName = "Mar";
                    break;
                case 4:
                    subFolderName = "Apr";
                    break;
                case 5:
                    subFolderName = "May";
                    break;
                case 6:
                    subFolderName = "Jun";
                    break;
                case 7:
                    subFolderName = "Jul";
                    break;
                case 8:
                    subFolderName = "Aug";
                    break;
                case 9:
                    subFolderName = "Sep";
                    break;
                case 10:
                    subFolderName = "Oct";
                    break;
                case 11:
                    subFolderName = "Nov";
                    break;
                case 12:
                    subFolderName = "Dec";
                    break;
                default:
                    subFolderName = "Nodata";
                    break;
            }
            return subFolderName;
        }
    }
}
