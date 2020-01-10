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

    public partial class inventarios : Window
    {
        int idBodegaOrigen , idBodegaDestino ,idBodegaActualizar = 0;

        string connectionString = "SERVER=localhost;DATABASE=bordados_db;UID=root;PASSWORD=1234;";
        MySqlConnection connection;
        public inventarios()
        {
            InitializeComponent();
            connection = new MySqlConnection(connectionString);
            fillCombo_Bodegas();
        }

        public inventarios(String val)
        {
            Console.WriteLine(val);
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
                    comboDestino.Items.Add(nombres_Empresa.GetString("nombreBodega"));
                    comboOrigen.Items.Add(nombres_Empresa.GetString("nombreBodega"));
                    comboBodega.Items.Add(nombres_Empresa.GetString("nombreBodega"));
                }
            }
            catch (Exception ex)
            {

            }
            connection.Close();
        }
        private void GridBarraTitulo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnPower_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (!comboOrigen.SelectedItem.ToString().Equals("") )
            {
                mostrarOrigen();
            }else if (!comboDestino.SelectedItem.ToString().Equals(""))
            {
                mostrarDestino();
            }
            if(comboOrigen.SelectedItem.ToString().Equals("") && comboDestino.SelectedItem.ToString().Equals(""))
            {
                MessageBox.Show("Seleccione una bodega");
            }
            
  
        }

        private void ComboOrigen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                connection.Open();
                string v = "select id from bodega where nombreBodega ='" + comboOrigen.SelectedItem.ToString() + "';";
                MySqlCommand cmdConsulta = new MySqlCommand("select id from bodega where nombreBodega ='" + comboOrigen.SelectedItem.ToString() + "' ", connection);
                idBodegaOrigen = (int)cmdConsulta.ExecuteScalar();
                Console.WriteLine("Bodega seleccionada:" + comboOrigen.Text + "con el id:" + idBodegaOrigen);

               
                    MySqlCommand cmd = new MySqlCommand("select prenda.id , nombrePrenda as Prenda, descripcion as Talla , tipoPrenda as Tipo, nombreEmpresa as Empresa ,stock    from movimiento " +
                  "inner join prenda on movimiento.idPrenda = prenda.id" +
                  " inner join tipoprenda on prenda.idTipoPrenda = tipoprenda.id" +
                  " inner join talla on prenda.idTalla = talla.id " +
                  "inner join empresa on prenda.idEmpresa = empresa.id where idBodega = " + idBodegaOrigen + "  ;", connection);
                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());
                    dt_origen.DataContext = dt;
               
                connection.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.WriteLine("cambiando seleccion origen");
        }

        private void TxtBuscadorOrigen_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                connection.Open();
                MySqlCommand cmd = new MySqlCommand("select prenda.id , nombrePrenda as Prenda, descripcion as Talla , tipoPrenda as Tipo, nombreEmpresa as Empresa ,stock    from movimiento " +
              "inner join prenda on movimiento.idPrenda = prenda.id" +
              " inner join tipoprenda on prenda.idTipoPrenda = tipoprenda.id" +
              " inner join talla on prenda.idTalla = talla.id " +
              "inner join empresa on prenda.idEmpresa = empresa.id where idBodega = " + idBodegaOrigen + "  and nombrePrenda like('%" + txtBuscadorOrigen.Text + "%') ;", connection);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                dt_origen.DataContext = dt;

                connection.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void TxtBuscadordestino_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                connection.Open();
                MySqlCommand cmd = new MySqlCommand("select prenda.id , nombrePrenda as Prenda, descripcion as Talla , tipoPrenda as Tipo, nombreEmpresa as Empresa ,stock    from movimiento " +
              "inner join prenda on movimiento.idPrenda = prenda.id" +
              " inner join tipoprenda on prenda.idTipoPrenda = tipoprenda.id" +
              " inner join talla on prenda.idTalla = talla.id " +
              "inner join empresa on prenda.idEmpresa = empresa.id where idBodega = " + idBodegaDestino + "  and nombrePrenda like('%" + txtBuscadordestino.Text + "%') ;", connection);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                dt_destino.DataContext = dt;

                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void ComboOrigen_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            mostrarOrigen();
        }
        public void mostrarOrigen()
        {
            try
            {
                connection.Open();
                string v = "select id from bodega where nombreBodega ='" + comboOrigen.SelectedItem.ToString() + "';";
                MySqlCommand cmdConsulta = new MySqlCommand("select id from bodega where nombreBodega ='" + comboOrigen.SelectedItem.ToString() + "' ", connection);
                idBodegaOrigen = (int)cmdConsulta.ExecuteScalar();
                Console.WriteLine("Bodega seleccionada:" + comboOrigen.Text + "con el id:" + idBodegaOrigen);


                MySqlCommand cmd = new MySqlCommand("select prenda.id , nombrePrenda as Prenda, descripcion as Talla , tipoPrenda as Tipo, nombreEmpresa as Empresa ,stock    from movimiento " +
              "inner join prenda on movimiento.idPrenda = prenda.id" +
              " inner join tipoprenda on prenda.idTipoPrenda = tipoprenda.id" +
              " inner join talla on prenda.idTalla = talla.id " +
              "inner join empresa on prenda.idEmpresa = empresa.id where idBodega = " + idBodegaOrigen + "  ;", connection);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                dt_origen.DataContext = dt;

                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.WriteLine("cambiando seleccion origen");
        }
        private void Dt_origen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            System.Windows.Controls.DataGrid gd = (System.Windows.Controls.DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                Console.WriteLine(row_selected[0]);
                Config_windows.transacciones_inventarios trans = new Config_windows.transacciones_inventarios(row_selected[0].ToString() , idBodegaOrigen, idBodegaDestino);
                trans.Show();               
                
            }
           
        }

        private void ComboDestino_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            mostrarDestino();
        }

        private void BtnEditarBodega_Click(object sender, RoutedEventArgs e)
        {
            Config_windows.editBodega edicionBodega = new Config_windows.editBodega(idBodegaActualizar);
            edicionBodega.Show();
        }

        private void ComboBodega_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            connection.Open();
            string v = "select id from bodega where nombreBodega ='" + comboBodega.SelectedItem.ToString() + "';";
            MySqlCommand cmdConsulta = new MySqlCommand("select id from bodega where nombreBodega ='" + comboBodega.SelectedItem.ToString() + "' ", connection);
            idBodegaActualizar = (int)cmdConsulta.ExecuteScalar();
            Console.WriteLine("Bodega por actualizar :" + comboBodega.SelectedItem.ToString() + "con el id:" + idBodegaActualizar);
            connection.Close();

        }

        public void mostrarDestino()
        {
            try
            {
                connection.Open();
                string v = "select id from bodega where nombreBodega ='" + comboDestino.SelectedItem.ToString() + "';";
                MySqlCommand cmdConsulta = new MySqlCommand("select id from bodega where nombreBodega ='" + comboDestino.SelectedItem.ToString() + "' ", connection);
                idBodegaDestino = (int)cmdConsulta.ExecuteScalar();
                Console.WriteLine("Bodega seleccionada:" + comboDestino.Text + "con el id:" + idBodegaDestino);


                MySqlCommand cmd = new MySqlCommand("select nombrePrenda as Prenda, descripcion as Talla , tipoPrenda as Tipo, nombreEmpresa as Empresa ,stock    from movimiento " +
              "inner join prenda on movimiento.idPrenda = prenda.id" +
              " inner join tipoprenda on prenda.idTipoPrenda = tipoprenda.id" +
              " inner join talla on prenda.idTalla = talla.id " +
              "inner join empresa on prenda.idEmpresa = empresa.id where idBodega = " + idBodegaDestino + "  ;", connection);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                dt_destino.DataContext = dt;

                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.WriteLine("cambiando seleccion destino");
        }
    }
}
