using System;
using System.Collections.Generic;
using System.Linq;
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
using System.IO;

namespace WpfApplication1

{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class DragDropImage : Window
    {
        Model1Container _dataDC;
        string _productID;

        public string ProductID
        {
            get { return _productID; }
            set { _productID = value; }
        }
        ObservableProductImage imagelist;
        public DragDropImage(string Product)
        {
            InitializeComponent();
            _productID = Product;
            _dataDC = new Model1Container();
            imagelist = new ObservableProductImage(_dataDC, _productID);

            ImageList.ItemsSource = imagelist;
            
        }


        private BitmapImage LoadImageFromFile(string filename)
        {
            using (var fs = File.OpenRead(filename))
            {
                var img = new BitmapImage();
                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad;
                // Downscaling to keep the memory footprint low
                img.DecodePixelWidth = (int)SystemParameters.PrimaryScreenWidth;
                img.StreamSource = fs;
                img.EndInit();
                return img;
            }
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            var data = e.Data as DataObject;
            if (data.ContainsFileDropList())
            {
                var files = data.GetFileDropList();
                foreach (string file in files)
                {
                    string filename = System.IO.Path.GetFileName(file);
                    if (imagelist.Where(S => S.Name == filename).Count() == 0)
                    {
                        ProductImageSet newImage = new ProductImageSet();
                        newImage.ProducID = _productID;
                        newImage.Path = file;
                        newImage.Name = filename;
                        Ramdom r = new Ramdom();
                        newImage.ImageID = r.RandomString(32);
                        newImage.Order = 0;
                        imagelist.Add(newImage);
                        _dataDC.ProductImageSet.AddObject(newImage);
                        _dataDC.SaveChanges();
                        string destinationFile = Properties.Settings.Default.ImagePath + "\\" + ProductID + "\\" + filename;
                        if(!System.IO.Directory.Exists(Properties.Settings.Default.ImagePath + "\\" + ProductID ))
                        {
                        System.IO.Directory.CreateDirectory(Properties.Settings.Default.ImagePath + "\\" + ProductID);
                        }
                        System.IO.File.Copy(file, destinationFile);
                        imageViewer.Source = LoadImageFromFile(destinationFile);
                        ImageList.ItemsSource = imagelist.Where(S => S.ProducID == _productID).ToList();
                    }
                    else
                    {
                        MessageBox.Show("La imagen" + filename + " ya ha sido añadida anteriormente");
                    }
                }
            }

        }

        private void BorrarMouseDown(object sender, RoutedEventArgs e)
        {
            ProductImageSet Image = new ProductImageSet();
            if(ImageList.SelectedItem!= null){
                try
                {
                    Image = ImageList.SelectedItem as ProductImageSet;

                    _dataDC.ProductImageSet.DeleteObject(Image);
                    _dataDC.SaveChanges();
                    ImageList.Items.Remove(Image);
                    System.IO.File.Delete(Image.Path);
                }
                catch(Exception ex) {
                    System.Windows.MessageBox.Show("Error Borrando la imagen " + Image.Name + ": " + ex.ToString()); 
                }
            }
        }

        private void ImageList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProductImageSet image = ImageList.SelectedItem as ProductImageSet;
            imageViewer.Source = LoadImageFromFile(image.Path);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _dataDC.SaveChanges();
        }
    }
}

