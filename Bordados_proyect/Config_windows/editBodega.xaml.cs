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
    /// Interaction logic for editBodega.xaml
    /// </summary>
    /// 
    
    public partial class editBodega : Window
    {
        int bodega = 0;
        int idPrendaSeleccionada = 0;

        string connectionString = "SERVER=localhost;DATABASE=bordados_db;UID=root;PASSWORD=1234;";
        MySqlConnection connection;
        public editBodega(int idBodegaActualizar)
        {
            bodega = idBodegaActualizar;
            InitializeComponent();
            connection = new MySqlConnection(connectionString);
            mostrarStockBodega();
            mostrarPrendas();

        }
        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnPower_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void GridBarraTitulo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
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
              "inner join empresa on prenda.idEmpresa = empresa.id where idBodega = " + bodega + "  and nombrePrenda like('%" + txtBuscadorOrigen.Text + "%') ;", connection);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                dt_origen.DataContext = dt;

                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        private void mostrarStockBodega()
        {
            try
            {
                connection.Open();
                MySqlCommand cmd = new MySqlCommand("select prenda.id , nombrePrenda as Prenda, descripcion as Talla , tipoPrenda as Tipo, nombreEmpresa as Empresa ,stock    from movimiento " +
              "inner join prenda on movimiento.idPrenda = prenda.id" +
              " inner join tipoprenda on prenda.idTipoPrenda = tipoprenda.id" +
              " inner join talla on prenda.idTalla = talla.id " +
              "inner join empresa on prenda.idEmpresa = empresa.id where idBodega = " + bodega + " ;", connection);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                dt_origen.DataContext = dt;

                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        private void mostrarPrendas()
        {
            connection.Open();
            MySqlCommand cmd = new MySqlCommand("select prenda.id, nombrePrenda as Prenda , descripcion as Talla ,  nombreEmpresa as Empresa , tipoPrenda as Prenda from prenda " +
                "inner join Talla on prenda.idTalla = Talla.id" +
                " inner join Empresa on prenda.idEmpresa = Empresa.id " +
                "inner join tipoprenda on prenda.idTipoPrenda = tipoprenda.id", connection);
            DataTable dt = new DataTable();

            dt.Load(cmd.ExecuteReader());

            dt_2.DataContext = dt;            
            connection.Close();
        }
        private void TextBox_KeyUp_1(object sender, System.Windows.Input.KeyEventArgs e)
        {
            connection.Open();
            MySqlCommand cmd2 = new MySqlCommand("select prenda.id, nombrePrenda as Prenda , descripcion as Talla ,  nombreEmpresa as Empresa , tipoPrenda as Prenda from prenda " +
                "inner join Talla on prenda.idTalla = Talla.id" +
                " inner join Empresa on prenda.idEmpresa = Empresa.id " +
                "inner join tipoprenda on prenda.idTipoPrenda = tipoprenda.id where nombrePrenda like('%" + txtBuscador2.Text + "%');", connection);

            DataTable dt_dinamica = new DataTable();
            dt_dinamica.Load(cmd2.ExecuteReader());

            dt_2.DataContext = dt_dinamica;
            connection.Close();
        }

        private void Dt_2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

                System.Windows.Controls.DataGrid gd = (System.Windows.Controls.DataGrid)sender;
                DataRowView row_selected = gd.SelectedItem as DataRowView;
                
                if (row_selected != null)
                {
                    Console.WriteLine(row_selected[0]);
                    idPrendaSeleccionada = (int)row_selected[0];

                }

            
        }

        private void BtnActualizar_Click(object sender, RoutedEventArgs e)
        {
            if(txtCantidad.Text == "")
            {
                MessageBox.Show("Debe seleccionar una cantidad minima");
                return;
            }
            //   if(checkBox.IsChecked && )
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Esta seguro que desea continuar", "Confirmacion de peticion", System.Windows.MessageBoxButton.YesNo);


            if (messageBoxResult == MessageBoxResult.Yes){
                if (idPrendaSeleccionada != 0)
                {
                    connection.Open();
                    //MySqlCommand cmd = new MySqlCommand("update movimiento set stock = 100 where idBodega = " + bodega + " and idPrenda = " + idPrendaSeleccionada + "", connection);
                    String consulta = "call addMovimiento(" + idPrendaSeleccionada + "," + bodega + "," + txtCantidad.Text +");";

                    MySqlParameter[] pms = new MySqlParameter[3];
                    pms[0] = new MySqlParameter("idPrendaIN", idPrendaSeleccionada);
                    pms[1] = new MySqlParameter("idBodegaIN", bodega);
                    pms[2] = new MySqlParameter("cantidad", Int32.Parse(txtCantidad.Text));
                    MySqlCommand command = new MySqlCommand();
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = "nuevoIngresoBodega";
                    command.Connection = connection;

                    command.Parameters.AddRange(pms);
                    command.ExecuteNonQuery();
                    connection.Close();


                }
                else MessageBox.Show("Seleccione una prenda");
                mostrarStockBodega();
                if (!(bool)checkBox.IsChecked)
                {
                    txtCantidad.Text = "";
                }
            }
        }

        private void TxtBuscador2_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
