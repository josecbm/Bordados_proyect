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
using System.Windows.Shapes;

namespace Bordados_proyect.Config_windows
{
    /// <summary>
    /// Interaction logic for newCliente.xaml
    /// </summary>
    public partial class newCliente : Window
    {

        string connectionString = "SERVER=localhost;DATABASE=bordados_db;UID=root;PASSWORD=1234;";
        MySqlConnection connection;
        public newCliente()
        {
            InitializeComponent();
            connection = new MySqlConnection(connectionString);
            mostrarClientes();
        }
        private void mostrarClientes()
        {
            connection.Open();
            MySqlCommand cmd = new MySqlCommand("Select nombre, direccion , telefono , email , nit from cliente;" , connection);
            DataTable dt = new DataTable();

            dt.Load(cmd.ExecuteReader());
            dtClientes.DataContext = dt;
            connection.Close();
        }
        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Esta seguro que desea continuar", "Confirmacion de peticion", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                try
                {
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand("INSERT INTO `bordados_db`.`cliente` (`nombre`, `direccion`, `telefono`, `email`, `nit`) VALUES ('" + txtNombre.Text + "', '" + txtDireccion.Text + "', '" + txtTelefono.Text + "', '" + txtCorreo.Text + "', '" + txtNit.Text + "');", connection);
                    cmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {

                    System.Windows.MessageBox.Show("Problemas en la insercion de datos", ex.ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                }
                connection.Close();
            }
            txtNombre.Text = "";
            txtDireccion.Text = "";
            txtTelefono.Text = "";
            txtCorreo.Text = "";
            txtNit.Text = "";
            mostrarClientes();
        }

        private void BtnOtro_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnPower_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void GridBarraTitulo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void TextBox_buscador_KeyUp(object sender, KeyEventArgs e)
        {
            connection.Open();
            MySqlCommand cmd2 = new MySqlCommand("Select nombre, direccion , telefono , email , nit from cliente where nombre like('%" + textBox_buscador.Text + "%');", connection);

            DataTable dt_dinamica = new DataTable();
            dt_dinamica.Load(cmd2.ExecuteReader());
            dtClientes.DataContext = dt_dinamica;
            connection.Close();
        }
    }
}
