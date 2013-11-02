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

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for UCProviderList.xaml
    /// </summary>
    public partial class UCProviderList : UserControl
    {



               
        ObservableProveedor lpr;
        private static Model1Container _dataDC;
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
            get { return providerSet1ViewSource.Width; }
            set
            {
                providerSet1ViewSource.Width = value;
            }
        }
        public double LHeight
        {
            get { return providerSet1ViewSource.Height; }
            set
            {
                providerSet1ViewSource.Height = value;
            }
        }



        public UCProviderList()
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                _dataDC = ModelSingleton.getDataDC;
                InitializeComponent();
                lpr = new ObservableProveedor(_dataDC);
                
                this.DataContext = lpr;
                providerSet1ViewSource.ItemsSource = lpr;
            }

        }
    



        private void ProductsSetListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void proveedorSetListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (providerSet1ViewSource.Items.Count > 0 && providerSet1ViewSource.SelectedItem != null)
            {
                String file = providerSet1ViewSource.SelectedItem.ToString();
                ProveedorSet p = (ProveedorSet)providerSet1ViewSource.SelectedItem;
                Provider providerdetails = new Provider(p);
                providerdetails._new = false;
              
                providerdetails.ShowDialog();
              
                if ( !providerdetails.Cancel) _dataDC.SaveChanges();
              
            }
        }

        private void MenuItem_MouseDown(object sender, RoutedEventArgs e)
        {
            //provmenu.PlacementTarget = this;
            //provmenu.IsOpen = true;

            ProveedorSet p = new ProveedorSet();
       
            Provider providerdetails = new Provider(p);
            providerdetails._new = true;
           
            providerdetails.ShowDialog();
           
            if( !providerdetails.Cancel){
                if (providerdetails._new )
                {
                    lpr.Add(p);
                    _dataDC.ProveedorSet.AddObject(p);
                    _dataDC.SaveChanges();
                    
                }
                else lpr.Add(p);
                filter();
            }
        }
        private void filter(){


       providerSet1ViewSource.ItemsSource = lpr.Where(S =>
       S.Nombre.ToLower().Contains(name.ToLower())
       && S.Apellido.ToLower().Contains(surname.ToLower())
       && S.TelefonoM.Contains(phone)
       && S.Email.Contains(email)
       );
       }

    }
    
}



    

