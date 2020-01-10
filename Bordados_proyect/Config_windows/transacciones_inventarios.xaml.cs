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
    /// Interaction logic for transacciones_inventarios.xaml
    /// </summary>
    public partial class transacciones_inventarios : Window
    {
        string connectionString = "SERVER=localhost;DATABASE=bordados_db;UID=root;PASSWORD=1234;";
        MySqlConnection connection;
        string id, nombre, talla, empresa, tipo , precio, cantidadMaxima  , prendaActual;
        int idOrigen, idDestino;
        
        private void BtnMenos_Click(object sender, RoutedEventArgs e)
        {
            txtCantidad.Text = (Int32.Parse(txtCantidad.Text) - 1).ToString();
        }

        private void BtnMas_Click(object sender, RoutedEventArgs e)
        {
            txtCantidad.Text = (Int32.Parse(txtCantidad.Text) + 1).ToString();
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            connection.Open();
            String res;
            try
            {
                String consulta = "call addMovimiento(" + this.id + "," + this.idOrigen + "," + idDestino + "," + Int32.Parse(txtCantidad.Text) + ",@res,@debug);";

                MySqlParameter[] pms = new MySqlParameter[4];
                pms[0] = new MySqlParameter("idProducto", Int32.Parse(id));
                pms[1] = new MySqlParameter("idBode_origen", idOrigen);
                pms[2] = new MySqlParameter("idBode_destino", idDestino);
                pms[3] = new MySqlParameter("cantidad", Int32.Parse(txtCantidad.Text));
               
                MySqlCommand command = new MySqlCommand();
                command.Parameters.Add("@res", MySqlDbType.VarChar).Direction = System.Data.ParameterDirection.Output;
                command.Parameters.Add("@debug", MySqlDbType.VarChar).Direction = System.Data.ParameterDirection.Output;
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "addMovimiento";
                command.Connection = connection;

                command.Parameters.AddRange(pms);

                if(command.ExecuteNonQuery() == 1)
                {
                    //MessageBox.Show("yes");
                    res = Convert.ToString(command.Parameters["@res"].Value);
                    Console.WriteLine(res);
                }

            }
            catch(Exception ex)
            {

            }


            connection.Close();
            this.Close();
        }

        private void TxtCantidad_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (!txtCantidad.Text.Equals(""))
            {
              
                if (Int32.Parse(txtCantidad.Text.Trim()) > Int32.Parse(cantidadMaxima))
                {
                    MessageBox.Show("La cantidad soprepasa el stock actual.");
                    txtCantidad.Text = "";
                    return;
                }
                if (System.Text.RegularExpressions.Regex.IsMatch(txtCantidad.Text, "[^0-9]"))
                {
                    MessageBox.Show("Porfavor solo numeros.");
                    txtCantidad.Text = txtCantidad.Text.Remove(txtCantidad.Text.Length - 1);

                }
            }
            
        }

        public transacciones_inventarios(String val , int origen, int destino)
        {
            InitializeComponent();
            this.idOrigen = origen;
            this.idDestino = destino;
            this.id = val;
            connection = new MySqlConnection(connectionString);
            connection.Open();
            Console.WriteLine("Encontre esto para nuevo " + val);
            MySqlCommand com = new MySqlCommand("select nombrePrenda as Prenda, descripcion as Talla , tipoPrenda as Tipo, nombreEmpresa as Empresa ,stock    from movimiento" +
                " inner join prenda on movimiento.idPrenda = prenda.id" +
                " inner join tipoprenda on prenda.idTipoPrenda = tipoprenda.id " +
                "inner join talla on prenda.idTalla = talla.id" +
                " inner join empresa on prenda.idEmpresa = empresa.id where idBodega = 1 and prenda.id =" + val, connection);
            MySqlDataReader reader = com.ExecuteReader();
            while (reader.Read())
            {
                txtPrenda.Text = reader.GetString("Prenda");
                txtTalla.Text = reader.GetString("talla");
                txtTipo.Text = reader.GetString("Tipo");
                txtEmpresa.Text = reader.GetString("Empresa");
                cantidadMaxima = reader.GetString("stock");
                txtCantidad.Text = reader.GetString("stock");
               
            }

            connection.Close();
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
    }
}
