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
using System.Text.RegularExpressions;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Index.xaml
    /// </summary>
    public partial class Index : Page
    {
        Model1Container _dataDC;
        ObservableProductSearch ProductSearch;
        Boolean Updatenotification = true;
        public Index()
        {
            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
            InitializeComponent();
             RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;

              
             Product_List.LWidth = 500;
             Product_List.LHeight = 400;
             Provider_List.LWidth = 500;
             Provider_List.LHeight = 400;
             Client_List.LWidth = 500;
             Client_List.LHeight = 400;
             _dataDC = ModelSingleton.getDataDC;
             startrutines.Database_Backup();
             ProductSearch = new ObservableProductSearch(_dataDC);
             UpdateNotificationList();

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

        private void UpdateNotificationList()
        {
            var NoticationbyProduct = (from pr in _dataDC.ProductsSet
                                       where _dataDC.NotificationSet.Select(S => S.ProductID).Contains(pr.Id)
                                       select pr).ToList();

            //ListNotification.ItemsSource = (from pr in _dataDC.ProductsSet
            //                                where _dataDC.NotificationSet.Select(S => S.ProductID).Contains(pr.Id)
            //                                select pr).ToList();
            ListNotification.ItemsSource = NoticationbyProduct;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Products p = new Products();
            //p.ShowDialog();
        }

        #region Product

        public void product_tab_focus(object sender, RoutedEventArgs e)
        {

            
        }
        #endregion

        private void MarcaFocus(object sender, RoutedEventArgs e)
        {
            cbMarca.IsDropDownOpen = true;
        }

        private void ListRequestedProduct_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListRequestedProduct.Items.Count > 0 && ListRequestedProduct.SelectedItem != null)
            {
                ProductSearched search = new ProductSearched((ProductsSet)ListRequestedProduct.SelectedItem);
                
                search.ShowDialog();
                
                if (search.Cancel)
                {
                    _dataDC.SaveChanges();
                }

            }
        }

        private void pendingRequest(object sender, RoutedEventArgs e)
        {

            ListRequestedProduct.ItemsSource = ProductSearch;
        }

        private void List_Notification(object sender, MouseButtonEventArgs e)
        {
            if (ListNotification.SelectedItem != null)
            {
                ProductsSet selectedProduct = (ProductsSet)ListNotification.SelectedItem;
                NotificationClient NCWindow = new NotificationClient(selectedProduct);
                
                NCWindow.ShowDialog();
                
            }
        }

        private void NotificationsFocus(object sender, RoutedEventArgs e)
        {


         
        }

        private void ListNotification_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        //protected  void OnPreviewTextBox(TextCompositionEventArgs e)
        //{
        //    char c = Convert.ToChar(e.Text);
        //    if (Char.IsNumber(c))
        //        e.Handled = false;
        //    else
        //        e.Handled = true;

        //    base.OnPreviewTextInput(e);
        //}
        #region validation
        private void OnPreviewTextBox(object sender, TextCompositionEventArgs e)
        {
            TextBox tb = sender as TextBox;

            if (tb != null)
            {
                char c = Convert.ToChar(e.Text);

                if (Char.IsNumber(c))
                {
                    if (tb.Text.Count()>0 && tb.Text[0] == '0')
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

        private void NotificationsLostFocus(object sender, RoutedEventArgs e)
        {
         
        }

        private void tabControl1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string[] todos = new string[] { "Todos" };
            cbProducto.ItemsSource = _dataDC.ProductsSet.Select(S => S.Producto).Distinct().Union(todos).ToList();
            ProductSearch = new ObservableProductSearch(_dataDC);
            ListRequestedProduct.ItemsSource = ProductSearch;
            //Product_List.refresh();
            UpdateNotificationList();
            
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

        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMessage = string.Format("An unhandled exception occurred: {0}", e.Exception.Message);
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
            MessageBox.Show(ModelSingleton.getDataDC.Connection.DataSource);
        }
    }


}
