using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bordados_proyect
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string connectionString = "SERVER=localhost;DATABASE=bordados_db;UID=root;PASSWORD=1234;";
        MySqlConnection connection;
        public MainWindow()
        {

            InitializeComponent();
            connection = new MySqlConnection(connectionString);


            //dtPrueba.DataContext = dt;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            logear();
            
        }



        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                logear();
            }
        }
        public void logear()
        {
            connection.Open();
            string usr = txtUsr.Text;
            string pass = txtPass.Password;
            MySqlCommand cmd = new MySqlCommand("select id , nombre , pass from usr where usr.nombre='" + usr.Trim() + "' and usr.pass='" + pass.Trim() + "' ", connection);
            MySqlDataReader login = cmd.ExecuteReader();
            if (login.HasRows)
            {
                while (login.Read())
                {
                    Console.WriteLine(login.GetString(0));
                    Console.WriteLine(login.GetString(1));
                    Console.WriteLine(login.GetString(2));
                    pagina_principal principal = new pagina_principal(int.Parse(login.GetString(0)));
                    principal.Show();
                    this.Close();
                }
            }
            else Console.WriteLine("no se encontro el usuario en la base de datos");

            connection.Close();
        }
    }
}
