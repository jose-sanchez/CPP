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
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for BuyProduct.xaml
    /// </summary>
    public partial class BuyProduct : Window
    {
        SaleSet _sale;
        private Boolean _cancel = false;
        public Boolean Cancel
        {
            get { return _cancel; }
            set { _cancel = value; }
        }

        
        public BuyProduct(SaleSet sale)
        {
            InitializeComponent();
            _sale = sale;
            tbPrecioCliente.Text = sale.PriceforClient.ToString();

        }

        private void btBuy_Click(object sender, RoutedEventArgs e)
        {
            if (tbPrecioFinal.Text != "" && tbPrecioFinal.Text != null && tbPrecioFinal.Text != "0")
            {
                _sale.FinalPrice = Convert.ToInt32(tbPrecioFinal.Text);
                _cancel = false;
                Close();
            }
            else { 
                MessageBox.Show("Rellene el campo Precio Final");
                _cancel = true;
            }
        }

    }
}
