using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for newInventario.xaml
    /// </summary>
    public partial class newInventario : Window
    {

        int idComboBodega;

        string connectionString = "SERVER=localhost;DATABASE=bordados_db;UID=root;PASSWORD=1234;";
        MySqlConnection connection;
        public newInventario(string value)
        {
            InitializeComponent();
            
            connection = new MySqlConnection(connectionString);
            fillCombo_Bodegas();
            connection.Open();
            
            MySqlCommand com = new MySqlCommand("select prenda.id ,nombrePrenda , descripcion , nombreEmpresa , tipoPrenda from prenda " +
                "inner join Talla on prenda.idTalla = Talla.id" +
                " inner join Empresa on prenda.idEmpresa = Empresa.id " +
                "inner join tipoprenda on prenda.idTipoPrenda = tipoprenda.id where prenda.id ="+ value, connection);
            MySqlDataReader reader = com.ExecuteReader();
            while (reader.Read())
            {
                id.Text = reader.GetString("id");
                nombre.Text = reader.GetString("nombrePrenda");
                talla.Text = reader.GetString("descripcion");
                empresa.Text = reader.GetString("nombreEmpresa");
                tipo.Text = reader.GetString("tipoPrenda");

            }
            connection.Close();
        }

      
        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void GridBarraTitulo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void BtnPower_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                idComboBodega = 0;
                connection.Open();
                MySqlCommand cmdConsulta = new MySqlCommand("select id from bodega where nombreBodega ='" + comboBodega.Text.Trim() + "' ", connection);
                idComboBodega = (int)cmdConsulta.ExecuteScalar();
                string con = "INSERT INTO `bordados_db`.`movimiento` (`idBodega`, `idAccion`, `idPrenda`, `fecha_movimiento`, `stock`) VALUES ('" + idComboBodega + "', '1','" + id.Text + "', str_to_date('" + DateTime.Now.ToString("M/d/yyyy") + "', '%m/%d/%Y'), '" + stockInicial.Text.Trim() + "');";
                Console.WriteLine(con);
                cmdConsulta = new MySqlCommand(con, connection);
                cmdConsulta.ExecuteNonQuery();
                connection.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            this.Close();
        }

        public void fillCombo_Bodegas()
        {
            MySqlCommand cmd = new MySqlCommand("SELECT nombreBodega From bodega", connection);

            try
            {
                connection.Open();
                MySqlDataReader nombres_Empresa = cmd.ExecuteReader();
                while (nombres_Empresa.Read())
                {
                    Console.WriteLine("entra");
                   
                    comboBodega.Items.Add(nombres_Empresa.GetString("nombreBodega"));
                }
            }
            catch (Exception ex)
            {

            }
            connection.Close();
        }
    }
}
