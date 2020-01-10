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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
namespace Bordados_proyect
{
    /// <summary>
    /// Interaction logic for cargaProductos.xaml
    /// </summary>
    public partial class cargaProductos : Window
    {

        string connectionString = "SERVER=localhost;DATABASE=bordados_db;UID=root;PASSWORD=1234;";
        MySqlConnection connection;
        int idTalla_nueva;
        int idEmpresa_nueva;
        int idTipo_nuevo;
        public cargaProductos()
        {
            InitializeComponent();
            connection = new MySqlConnection(connectionString);
            fillCombo_Empresa();
            fillCombo_talla();
            fillCombo_Tipo();
        
        }

        private void BtnPower_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void GridBarraTitulo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        public void fillCombo_Empresa()
        {
            MySqlCommand cmd = new MySqlCommand("SELECT nombreEmpresa From Empresa", connection);

            try
            {
                connection.Open();
                MySqlDataReader nombres_Empresa = cmd.ExecuteReader();
                while (nombres_Empresa.Read())
                {
                    Console.WriteLine("entra");
                    comboEmpresa.Items.Add(nombres_Empresa.GetString("nombreEmpresa"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            connection.Close();
        }
        public void fillCombo_talla()
        {
            MySqlCommand cmd = new MySqlCommand("SELECT descripcion From talla", connection);

            try
            {
                connection.Open();
                MySqlDataReader nombres_Empresa = cmd.ExecuteReader();
                while (nombres_Empresa.Read())
                {
                    Console.WriteLine("entra");
                    comboTalla.Items.Add(nombres_Empresa.GetString("descripcion"));
                }
            }
            catch (Exception ex)
            {

            }
            connection.Close();
        }
        public void fillCombo_Tipo()
        {
            MySqlCommand cmd = new MySqlCommand("SELECT tipoPrenda From tipoPrenda", connection);

            try
            {
                connection.Open();
                MySqlDataReader nombres_Empresa = cmd.ExecuteReader();
                while (nombres_Empresa.Read())
                {
                    comboPrenda.Items.Add(nombres_Empresa.GetString("tipoPrenda"));
                }
            }
            catch (Exception ex)
            {

            }
            connection.Close();
        }

        private void BtnAceptar_productos_Click(object sender, RoutedEventArgs e)
        {
            idEmpresa_nueva = 0;
            idTalla_nueva = 0;
            idTipo_nuevo = 0;
            try
            {
                Console.WriteLine(comboEmpresa.Text.Trim());
                Console.WriteLine(comboPrenda.Text.Trim());
                Console.WriteLine(comboTalla.Text.Trim());
                Console.WriteLine(txtCodigo.Text);

                connection.Open();
                MySqlCommand cmdConsulta = new MySqlCommand("select id from empresa where nombreEmpresa ='"+comboEmpresa.Text.Trim()+"';", connection);
                idEmpresa_nueva = (int)cmdConsulta.ExecuteScalar();

                Console.WriteLine("esto encontre en id empresa jefe ->  " + idEmpresa_nueva);
                string com = "select id from talla where descripcion='" + comboTalla.Text.Trim() + "'; ";
                MySqlCommand cmdConsult2 = new MySqlCommand(com, connection);
                idTalla_nueva = (int)cmdConsult2.ExecuteScalar();

                Console.WriteLine("esto encontre en id talla  jefe ->  " + idTalla_nueva);
                MySqlCommand cmdConsult3 = new MySqlCommand("select id from tipoprenda where tipoprenda='" + comboPrenda.Text.Trim() + "';", connection);
                idTipo_nuevo = (int)cmdConsult3.ExecuteScalar();
                Console.WriteLine("esto encontre en id tipoprenda  jefe ->  " + idTipo_nuevo);
                
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            connection.Close();

            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Esta seguro que desea continuar", "Confirmacion de peticion", System.Windows.MessageBoxButton.YesNo);

            
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                connection.Open();

                String comando = "INSERT INTO `bordados_db`.`prenda` (`id`,`nombrePrenda`, `idTalla`, `idEmpresa`, `idTipoPrenda`, `precio`, `precioCosto`) VALUES (" + int.Parse(txtCodigo.Text)+" ,'" + txt_nombrePrenda.Text + "', "+ idTalla_nueva +", " + idEmpresa_nueva + ", "+idTipo_nuevo+", " + double.Parse(txt_precio_venta.Text) + " , "+double.Parse(txt_precio.Text)+");";
                MySqlCommand cmd = new MySqlCommand(comando , connection);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("_________");
                    Console.WriteLine(ex);
                    Console.WriteLine("_________");
                }
                connection.Close();
                this.Close();
            }
        }

        private void BtnCargaMasiva_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "csv files (*.csv)|*.csv";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string selectedFileName = openFileDialog1.FileName;
                using (var reader = new StreamReader(selectedFileName))
                {

                    while (!reader.EndOfStream)
                    {
                        try
                        {
                            var line = reader.ReadLine();
                            var values = line.Split(',');
                            //Nombrelist.Add(values[0]);
                            //idtallalist.Add(values[1]);
                            //idempresalist.Add(values[2]);
                            //idtipoPrendalist.Add(values[3]);
                            //preciolist.Add(values[4]);

                            String comando = "INSERT INTO `bordados_db`.`prenda` (`nombrePrenda`, `idTalla`, `idEmpresa`, `idTipoPrenda`, `precio`) VALUES ('" + values[0] + "', " + values[1] + ", " + values[2] + ", " + values[3] + ", " + values[4] + ");";
                            MySqlCommand cmd = new MySqlCommand(comando, connection);
                            cmd.ExecuteNonQuery();

                        }
                        catch(Exception ex)
                        {
                            System.Windows.MessageBox.Show("Problemas en la insercion de datos", ex.ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
        }

        private void BtnCancelar_productos_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
