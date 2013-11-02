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
    /// Interaction logic for ListSelectProduct.xaml
    /// </summary>
    public partial class ListSelectProduct : Window
    {
        String _clientID;

        public String ClientID
        {
            get { return _clientID; }
            set { _clientID = value; }
        }
        Boolean isnew;
        List< ProductsSet> addedlist;
        List<SaleSet> AddSaleList;
        Model1Container _dataDC;

        ObservableSale SaleList;
        List<SaleListItem> ItemList;
        public Boolean _new
        {
            get { return isnew; }
            set
            {
                isnew = value;
                //if (this.isnew)
                //{
                //    Ramdom r = new Ramdom();
                //    _product.Id = r.RandomString(32);
                //}
            }
        }
        public ListSelectProduct(String ClientID)
        {
            InitializeComponent();
            Product_List.Width = 342;
            Product_List.IsfilterMode =false;
            
            _dataDC = ModelSingleton.getDataDC;
            _clientID = ClientID;
            addedlist = new List <ProductsSet>();
            SaleList = new ObservableSale(_dataDC);
            AddSaleList = new List<SaleSet>();
            ItemList = new  List<SaleListItem>();
            Product_List.MouseDoubleClick += AddProduct;
            string[] todos = new string[] { "Todos" };
            cbProducto.ItemsSource = _dataDC.ProductsSet.Select(S => S.Producto).Distinct().Union(todos).ToList();
            cbMarca.ItemsSource = _dataDC.ProductsSet.Select(S => S.Marca).Distinct().Union(todos).ToList();
            cbModelo.ItemsSource = _dataDC.ProductsSet.Select(S => S.Modelo).Distinct().Union(todos).ToList();
            int añoindex;
            for (añoindex = 1900; añoindex < 2025; añoindex++)
            {

                cbfromyear.Items.Add(añoindex);
            }
        }
        protected void AddProduct(object sender, EventArgs e)
        {
            UCProductList s = (UCProductList) sender;
            if (s.ProductsSetListView.SelectedItem != null)
            {
                ProductsSet productSelected = (ProductsSet)s.ProductsSetListView.SelectedItem;
                if (_dataDC.SaleSet.Where(S => S.Client_ID == _clientID &&
                                               S.Product_ID == productSelected.Id).Count()==0)
                {
                    SaleListItem Item = new SaleListItem();
                    Item.Name = productSelected.Producto + " " + productSelected.Marca + " " + productSelected.Modelo;
                    Item.ProviderPrice = productSelected.Precio;
                    Item.ClientPrice = 0;
                    Item.Profit = 0;
                    Item.SendEmail = false;
                    Item.ProductID = productSelected.Id;
                    ItemList.Add(Item);
                    addlist.ItemsSource = ItemList.ToList();
                }
                else MessageBox.Show("Este producto ya ha sido previamente solicitado por este cliente,vuelva a la ventana de cliente para visualizar o modificar los detalles de la solicitud anterior");
            }
            
             
        }
        private void save_Click(object sender, RoutedEventArgs e){

            foreach (SaleListItem item in addlist.Items)
            {
                SaleSet newSale = new SaleSet();
                Ramdom r = new Ramdom();
                newSale.Id = r.RandomString(32);
                newSale.Client_ID = ClientID;
                newSale.Product_ID = item.ProductID;
                newSale.PriceforClient =  Convert.ToInt32(item.ClientPrice);
                newSale.PriceClientOffered = 0;
                newSale.FinalPrice = 0;
                newSale.LastEmailDate = new DateTime(1900, 1,1);
                if (item.SendEmail) {
                    if (Email.SendEmail(ClientID, item.ProductID, item.ClientPrice))
                    {
                        newSale.LastEmailDate = DateTime.Now;
                    }
                    else {
                        
                        MessageBox.Show("El mensaje con informacion sobre el producto " +
                            item.Name + " no pudo ser enviado");
                    }
                }
                _dataDC.SaleSet.AddObject(newSale);

            }
            _dataDC.SaveChanges();
            Close();
       
        }


        private void myList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (addlist.SelectedItem != null)
            {
                object o = addlist.SelectedItem;
                ListViewItem lvi = (ListViewItem)addlist.ItemContainerGenerator.ContainerFromItem(o);
                TextBox tb = FindByName("myBox", lvi) as TextBox;

                if (tb != null)
                    tb.Dispatcher.BeginInvoke(new Func<bool>(tb.Focus));
            }
        }

        private FrameworkElement FindByName(string name, FrameworkElement root)
        {
            Stack<FrameworkElement> tree = new Stack<FrameworkElement>();
            tree.Push(root);

            while (tree.Count > 0)
            {
                FrameworkElement current = tree.Pop();
                if (current.Name == name)
                    return current;

                int count = VisualTreeHelper.GetChildrenCount(current);
                for (int i = 0; i < count; ++i)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(current, i);
                    if (child is FrameworkElement)
                        tree.Push((FrameworkElement)child);
                }
            }

            return null;
        }

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

        private void cbProducto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string[] todos = new string[] { "Todos" };
            cbMarca.ItemsSource = _dataDC.ProductsSet.Where(S => S.Producto == cbProducto.SelectedValue).Select(S => S.Marca).Distinct().Union(todos).ToList();
        }

        private void cbMarca_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string[] todos = new string[] { "Todos" };
            cbModelo.ItemsSource = _dataDC.ProductsSet.Where(S => S.Marca == cbMarca.SelectedValue).Select(S => S.Modelo).Distinct().Union(todos).ToList();
        }

 
    }
    class SaleListItem {     
        public String Name { get; set; }
        public int ClientPrice { get; set; }
        public int ProviderPrice { get; set; }
        public int Profit { get; set; }
        public Boolean SendEmail { get; set; }
        public String ProductID { get; set; }
    }
}
