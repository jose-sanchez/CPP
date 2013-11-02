using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
namespace WpfApplication1
{
    class startrutines
    {
        public static void Database_Backup()
        {

            try
            {
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "DB_Backup"))
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "DB_Backup");
            }
            catch
            {
                MessageBox.Show("Ha habido un error al intentar crear el directorio " + AppDomain.CurrentDomain.BaseDirectory + "DB_Backup" + " . Tome nota de este error e informe al soporte de la aplicacion");
            }
            try
            {
                if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "DB_Backup\\Camiher-" + System.DateTime.Now.Date.Day + "-" + System.DateTime.Now.Date.Month + "-" + System.DateTime.Now.Date.Year))
                {
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "Camiher.sdf", AppDomain.CurrentDomain.BaseDirectory + "DB_Backup\\Camiher-" + System.DateTime.Now.Date.Day + "-" + System.DateTime.Now.Date.Month + "-" + System.DateTime.Now.Date.Year);
                }

            }
            catch
            {
                MessageBox.Show("Ha habido un error al intentar crear el archivo" +  AppDomain.CurrentDomain.BaseDirectory + "\\DB_Backup\\Camiher" + System.DateTime.Now.Date + " . Tome nota de este error e informe al soporte de la aplicacion");


            }
        }
        
    }
}
