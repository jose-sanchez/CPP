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
using System.Text.RegularExpressions;
namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Machine.xaml
    /// </summary>
    public partial class ProductSearched : Window
    {
        private Boolean _cancel=true;

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
            }
            }
        }


        public ProductSearched(ProductsSet product)
        {
            InitializeComponent();
           
            _product = product;
            _dataDC = ModelSingleton.getDataDC;

            product.Descripcion = "";

            cbProducto.ItemsSource = _dataDC.ProductsSet.Select(S => S.Producto).Distinct().ToList();
            cbModelo.ItemsSource = _dataDC.ProductsSet.Select(S => S.Modelo).Distinct().ToList();
            cbMarca.ItemsSource = _dataDC.ProductsSet.Select(S => S.Marca).Distinct().ToList();
            this.DataContext = product;
            
            for ( int añoindex = 1900; añoindex < 2025; añoindex++)
            {

                cbYear.Items.Add(añoindex);
            }
            if (product.Año != null){
                cbYear.SelectedItem = product.Año;
            }
            _product.PrivateDescription = "";
            _product.Kilometer = 0;
            _product.Hours = 0;

        }



        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (tbProducto.Text != null && tbProducto.Text != "")
            {
                if (tbModelo.Text == "" || tbModelo.Text == null)
                {
                    _product.Modelo = "";
                }
                else _product.Modelo = tbModelo.Text;
                if (tbMarca.Text == "" || tbMarca.Text == null)
                {
                    _product.Marca = "";
                }
                else _product.Marca = tbMarca.Text;
                
               
                _product.Producto = tbProducto.Text;
                if (_product.Cantidad == null)
                {
                    _product.Cantidad = 1;
                }
                if (_product.Peso == null)
                {
                    _product.Peso = 0;
                }
                if (_product.Potencia == null)
                {
                    _product.Potencia = 0;
                }
                if (_product.Precio == null || _product.Precio == 0)
                {
                    _product.Precio = int.MaxValue;
                }
                if (_product.Año == null)
                {
                    _product.Año = 1925;
                }


                _cancel = false;
                this.Close();
            }
            else MessageBox.Show("Seleccione un producto");
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

    }
}
