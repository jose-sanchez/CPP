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
    /// Interaction logic for NotificationClient.xaml
    /// </summary>
    public partial class NotificationClient : Window
    {
        Model1Container _dataDC = ModelSingleton.getDataDC;
        private ProductsSet _product;
        List<NotificationListItem> Notification;
        public NotificationClient(ProductsSet Product)
        {
            InitializeComponent();
            _product = Product;
            var productList = from c in _dataDC.ProductsSet
                              where _dataDC.NotificationSet.Where(S => S.ProductID == _product.Id).Select(S => S.SearchID).Contains(c.Id)
                              select c;


            Notification = (from n in productList
                                                       join cl in _dataDC.ClientSet on n.Proveedor_ID equals cl.Id
                                                       select new NotificationListItem
                                                       {
                                                           Name = cl.Name + " " + cl.Surname,
                                                           ProviderPrice = 0,
                                                           ClientPrice = 0,
                                                           Profit = 0,
                                                           SendEmail = false,
                                                           RequestProduct = false,
                                                           DeleteSearch = false,
                                                           ClientID = cl.Id,
                                                           SearchID = n.Id

                                                       }).ToList();
            foreach (NotificationListItem item in Notification)
            {
                item.ProviderPrice = _product.Precio;
            }
            UpdateAddList();
            

        }

        private void UpdateAddList()
        {
 


            addlist.ItemsSource = Notification.ToList();
        }



        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            foreach (NotificationListItem NTC in addlist.Items) {
                Boolean EmailSent = false;
                if (NTC.SendEmail)
                {
                    EmailSent = Email.SendEmail(NTC.ClientID, _product.Id, NTC.ClientPrice);
                }
                if (NTC.RequestProduct) {
                    SaleSet newSale = new SaleSet();
                    Ramdom r = new Ramdom();
                    newSale.Id = r.RandomString(32);
                    newSale.Client_ID = NTC.ClientID;
                    newSale.Product_ID = _product.Id;
                    newSale.PriceforClient = Convert.ToInt32(NTC.ClientPrice);
                    newSale.PriceClientOffered = 0;
                    newSale.FinalPrice = 0;
                    if (EmailSent)
                    {
                        newSale.LastEmailDate = DateTime.Now;
                    }
                    else
                    {
                        newSale.LastEmailDate = new DateTime(1900, 1, 1);
                    }
                    _dataDC.SaleSet.AddObject(newSale);
                }
                if (NTC.DeleteSearch) {
                    _dataDC.ProductsSet.DeleteObject(_dataDC.ProductsSet.First(S => S.Id == NTC.SearchID));
                }
                if (NTC.DeleteSearch || NTC.RequestProduct) { 
                _dataDC.NotificationSet.DeleteObject(_dataDC.NotificationSet.First(S=>S.ProductID==_product.Id
                                                                                    && S.SearchID == NTC.SearchID));
                }
                _dataDC.SaveChanges();
            }
            Close();
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

        private void TextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (addlist.SelectedItem != null)
            {
                NotificationListItem item = addlist.SelectedItem as NotificationListItem;
                Notification.First(S => S.ClientID == item.ClientID).Profit =
                  Convert.ToInt32(item.ClientPrice) - Convert.ToInt32(item.ProviderPrice);
                Notification.First(S => S.ClientID == item.ClientID).ClientPrice = item.ClientPrice;
                UpdateAddList();
            }
        }

    }
    class NotificationListItem
    {

        public String Name { get; set; }
        public int ClientPrice { get; set; }
        public int ProviderPrice { get; set; }
        public int Profit { get; set; }
        public Boolean SendEmail { get; set; }
        public Boolean RequestProduct { get; set; }
        public Boolean DeleteSearch { get; set; }
        public String ClientID { get; set; }
        public String SearchID { get; set; }


    }
}
