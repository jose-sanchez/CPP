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
using Microsoft.Windows.Controls;
using System.Windows.Controls.Input;
using PicViewer;
using System.IO;
using System.Text.RegularExpressions;
namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Machine.xaml
    /// </summary>
    public partial class Machine : Window
    {
        Boolean _cancel = true;
        Boolean _viewOnly = false;

        public Boolean ViewOnly
        {
            get { return _viewOnly; }
            set { _viewOnly = value;
            if (value) 
            {
                GridProduct.IsEnabled = false;
            
            }
            }
        }
            
        public Boolean Cancel
        {
            get { return _cancel; }
            set { _cancel = value; }
        }
        Boolean isnew;
        Model1Container _dataDC;
        ProductsSet _product;
        public Boolean _new
        {
            get { return isnew; }
            set { isnew = value;
            if (this.isnew)
            {
                Ramdom r = new Ramdom();
                _product.Id = r.RandomString(32);
                _product.Descripcion = "";
                _product.PrivateDescription = "";
            }
            }
        }
        

        public Machine(ProductsSet product)
        {
            InitializeComponent();
            
            _product = product;
            _dataDC = ModelSingleton.getDataDC;
            string productPath = Properties.Settings.Default.ImagePath.ToString() + "\\" + _product.Id;
            if (!Directory.Exists(productPath))
            {
                System.IO.Directory.CreateDirectory(productPath);
            }
            ProveedorSet p;
            if (product.Proveedor_ID != null && product.Proveedor_ID !="")
            {
                p = (ProveedorSet)_dataDC.ProveedorSet.Where(S => S.Id == product.Proveedor_ID).ToList()[0];
            }
            else {
            p = new ProveedorSet();
            }

            if (product.Descripcion != "" && product.Descripcion != null) { tbdescription.Text = product.Descripcion; }
        
            
            cbProducto.ItemsSource = _dataDC.ProductsSet.Select(S => S.Producto).Distinct().ToList();
            cbModelo.ItemsSource = _dataDC.ProductsSet.Select(S => S.Modelo).Distinct().ToList();
            cbMarca.ItemsSource = _dataDC.ProductsSet.Select(S => S.Marca).Distinct().ToList();
            this.DataContext = product;
            tbProvider.ItemsSource = _dataDC.ProveedorSet.Select(S=> S ).ToList();
            tbProvider.SelectedItem = p ;

            ClientList.ItemsSource = from c in _dataDC.ClientSet
                                     where _dataDC.SaleSet.Where(S => S.Product_ID == _product.Id).Select(S => S.Client_ID).Contains(c.Id)
                                     select c;

            for (int añoindex = 1900; añoindex < 2025; añoindex++)
            {

                cbyear.Items.Add(añoindex);
            }
        }



        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if(validateFields()){
            ProveedorSet p;
            _product.Marca = tbMarca.Text;
            _product.Modelo = tbModelo.Text;
            _product.Producto = tbProducto.Text;
            if (tbPotency.Text == null || tbPotency.Text == "") { _product.Potencia = 0; }
            if (tbPeso.Text == null || tbPeso.Text =="") { _product.Peso = 0; }
            if (tbProvider.SelectedIndex >= 0)
            {

                p = (ProveedorSet)tbProvider.SelectedItem;
                _product.Proveedor_ID = p.Id;
            }
            else _product.Proveedor_ID = "";
            _cancel = false;
            this.Close();
            }
        }

        private Boolean validateFields() 
        {
            if (tbProducto.Text=="")
            {
                MessageBox.Show("Rellene el campo Producto");
                return false;
            }
            if (tbMarca.Text == "")
            {
                MessageBox.Show("Rellene el campo Marca");
                return false;
            }
            if (tbModelo.Text == "")
            {
                MessageBox.Show("Rellene el campo Modelo");
                return false;
            }
            if (cbyear.SelectedItem== null)
            {
                MessageBox.Show("Rellene el campo Año");
                return false;
            }
            if (cbprecio.Text == "")
            {
                MessageBox.Show("Rellene el campo Precio");
                return false;
            }
            if (tbCantidad.Text == "")
            {
                MessageBox.Show("Rellene el campo Cantidad");
                return false;
            }
            if (tbHours.Text == "" && tbHours.Text == null)
            {
                _product.Hours = 0;
            }
            if (tbkilometer.Text == "" && tbkilometer.Text == null)
            {
                _product.Kilometer = 0;
            }
            if (tbdescription.Text == "" && tbdescription.Text == null) {
                _product.Descripcion = "";
            }
            return true;
        }

        private void cbMarca_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tbMarca.Text = cbMarca.SelectedItem.ToString();
        }

        private void cbModelo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tbModelo.Text = cbModelo.SelectedItem.ToString();

        }

        private void cbProducto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tbProducto.Text = cbProducto.SelectedItem.ToString();
        }

        private void ImageView_Click(object sender, RoutedEventArgs e)
        {
            string productPath = Properties.Settings.Default.ImagePath.ToString() + "\\" +_product.Id ;

            PicViewer.MainWindow ImageGallery = new PicViewer.MainWindow();
            ImageGallery.FolderPath = productPath;
           
            ImageGallery.ShowDialog();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DragDropImage AddImageWindow = new DragDropImage(_product.Id);
            
            AddImageWindow.ShowDialog();
            


        }
        #region event treatment
        private void ClientList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ClientList.Items.Count > 0 && ClientList.SelectedItem!= null)
            {
                String file = ClientList.SelectedItem.ToString();
                ClientSet p = (ClientSet)ClientList.SelectedItem;
                Client clientdetails = new Client(p);
                clientdetails._new = false;
            
                clientdetails.ShowDialog();
               
                if (!clientdetails.Cancel) _dataDC.SaveChanges();
            }
        }
        #endregion

        #region validation
        private void OnPreviewTextBox(object sender, TextCompositionEventArgs e)
        {
            TextBox tb = sender as TextBox;

            if (tb != null)
            {
                char c = Convert.ToChar(e.Text);

                if (Char.IsNumber(c))
                {
                    if (tb.Text.Count() > 0 && tb.Text[0] == '0')
                    {
                        e.Handled = true;
                    }
                    else { e.Handled = false; }
                }
                else
                    e.Handled = true;
            }

            base.OnPreviewTextInput(e);
        }

        private static bool IsEmailAllowed(string text)
        {
            bool blnValidEmail = true;
            Regex regEMail = new Regex(@"^[a-zA-Z][\w\.-]{2,28}[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$");
            if (text.Length > 0)
            {
                blnValidEmail = regEMail.IsMatch(text);
            }

            return blnValidEmail;
        }



        private void txtEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                if (IsEmailAllowed(tb.Text.Trim()) == false)
                {
                    e.Handled = true;
                    MessageBox.Show("El correo no es un correo valido", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    tb.Focus();
                }
            }
        }
        #endregion

        private void btViewProvider_Click(object sender, RoutedEventArgs e)
        {
            if( tbProvider.SelectedItem!=null){
            ProveedorSet p = tbProvider.SelectedItem as ProveedorSet;
            Provider providerdetails = new Provider(p);
            providerdetails._new = false;
           
            providerdetails.ShowDialog();
            
            if (!providerdetails.Cancel) _dataDC.SaveChanges();
            }
        }



    }
}
