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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Linq;

namespace WpfApplication1
{
	/// <summary>
	/// Interaction logic for UserControl1.xaml
	/// </summary>
	public partial class UCClientList : UserControl
	{




        
               
        
        private static Model1Container _dataDC;
        ObservableClient lcl;
        private string name="";

        public string FirstName
        {
            get { return name; }
            set { name = value; filter(); }
        }
   
                private string surname="";

        public string Surname
        {
            get { return surname; }
            set { surname = value; filter(); }
        }
         private string email="";
        public string Email
        {
            get { return email; }
            set { email = value; filter(); }
        }

        private string phone="";
        public string Phone
        {
            get { return phone; }
            set { phone = value; filter(); }
        }

        public double LWidth
        {
            get { return clientSet1ViewSource.Width; }
            set
            {
                clientSet1ViewSource.Width = value;
            }
        }
        public double LHeight
        {
            get { return clientSet1ViewSource.Height; }
            set
            {
                clientSet1ViewSource.Height = value;
            }
        }


        public UCClientList()
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
              
                InitializeComponent();
                _dataDC = ModelSingleton.getDataDC;
                lcl = new ObservableClient(_dataDC);
               
                this.DataContext = lcl;
                clientSet1ViewSource.ItemsSource = lcl;
            }

        }
    



        private void ProductsSetListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void clientSetListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Editar();
        }

        private void Editar()
        {
            if (clientSet1ViewSource.Items.Count > 0 && clientSet1ViewSource.SelectedItem !=null)
            {
                String file = clientSet1ViewSource.SelectedItem.ToString();
                ClientSet p = (ClientSet)clientSet1ViewSource.SelectedItem;
                Client clientdetails = new Client(p);
                clientdetails._new = false;
               
                clientdetails.ShowDialog();
              
                if (!clientdetails.Cancel) _dataDC.SaveChanges();
            }
        }

        private void MenuItem_MouseDown(object sender, RoutedEventArgs e)
        {
            //provmenu.PlacementTarget = this;
            //provmenu.IsOpen = true;

            ClientSet p = new ClientSet();
       
            Client clientdetails = new Client(p);
            clientdetails._new = true;
          
            clientdetails.ShowDialog();
           
            if (clientdetails._new && !clientdetails.Cancel)
            {
                lcl.Add(p);
                _dataDC.ClientSet.AddObject(p);
                _dataDC.SaveChanges();
                filter();
            }
        }
        private void filter(){

            try
            {
                clientSet1ViewSource.ItemsSource = lcl.Where(S =>
                S.Name.ToLower().Contains(name.ToLower())
                && S.Surname.ToLower().Contains(surname.ToLower())
                && S.PhoneMain.Contains(phone)
                && S.PhoneSecond.Contains(email)

                );
            }
            catch (Exception e)
            {
                MessageBox.Show(" Ha ocurrido un error en la funcion filter del modulo UCClientList - " + e.ToString());
               
            }
            
       }

        private void clientSet1ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void EditarMouseDown(object sender, RoutedEventArgs e)

        {
            
            Editar();
        }

        private void BorrarMouseDown(object sender, RoutedEventArgs e)
        {
            if (clientSet1ViewSource.SelectedItem != null)
            {
            string ClientID = (clientSet1ViewSource.SelectedItem as ClientSet).Id;
            if (_dataDC.ProductsSet.Where(S => S.Proveedor_ID == ClientID).Count() > 0) {

                if (MessageBox.Show("Este cliente tiene busquedas activas, si borra el cliente se borraran sus de busquedas de productos ¿Desea Continuar?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    //do no stuff
                }
                else
                {
                    DeleteClient(ClientID);
                }
            }
            else
            {
                DeleteClient(ClientID);
            }
            }
        }

         private void DeleteClient(string ClientID)
         {
             foreach (ProductsSet item in _dataDC.ProductsSet.Where(S => S.Proveedor_ID == ClientID && S.Enbusca == "True"))            
             {
                 _dataDC.ProductsSet.DeleteObject(item);
             }
            _dataDC.ClientSet.DeleteObject( _dataDC.ClientSet.First(S => S.Id == ClientID));
            _dataDC.SaveChanges();
        
         }

         private void Editar_Click(object sender, RoutedEventArgs e)
         {
             Editar();
         }

   
       
	}

}