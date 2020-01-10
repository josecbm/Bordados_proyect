using Bordados_proyect.beans;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace Bordados_proyect
{
    /// <summary>
    /// Interaction logic for generar_orden.xaml
    /// </summary>
    /// 

    public partial class generar_orden : Window
    {
        int estadoActual;
        string connectionString = "SERVER=localhost;DATABASE=bordados_db;UID=root;PASSWORD=1234;";
        string empresaSeleccionada = "";
        string tallaSeleccionada = "";
        string tipoSeleccionado = "";
        int cantidad = 1;
        double Total = 0;
        DataTable dt_orden;
        MySqlConnection connection;
        public generar_orden()
        {
            InitializeComponent();
            dt_orden = new DataTable();
            connection = new MySqlConnection(connectionString);
            fillCombo_Empresa();
            fillCombo_talla();
            fillCombo_Tipo();
            filldt_prendas();
            fillingOrderTable();
            fillAutoComplete();

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
                   
                    comboEmpresa.Items.Add(nombres_Empresa.GetString("nombreEmpresa"));
                }
            }
            catch (Exception ex)
            {

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

        public void filldt_prendas()
        {
            MySqlCommand cmd = new MySqlCommand("select Prenda.id,nombrePrenda as Prenda, descripcion as Talla , tipoPrenda as Tipo, nombreEmpresa as Empresa ,prenda.precio    from movimiento " +
                "inner join prenda on movimiento.idPrenda = prenda.id" +
                " inner join tipoprenda on prenda.idTipoPrenda = tipoprenda.id" +
                " inner join talla on prenda.idTalla = talla.id " +
                "inner join empresa on prenda.idEmpresa = empresa.id where idBodega = 1 ;", connection);

            try
            {
                connection.Open();
                DataTable dt = new DataTable();

                dt.Load(cmd.ExecuteReader());
                Console.WriteLine("entro en dt");
                dt_prendas_bodega_tienda.DataContext = dt;
            }
            catch (Exception e)
            {

            }
            connection.Close();
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


        private void ComboEmpresa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            empresaSeleccionada = comboEmpresa.SelectedItem.ToString().Trim();
            getTablaBodega();
        }
        private void getTablaBodega()
        {
            connection.Open();
            DataTable dt = new DataTable();
            if (empresaSeleccionada.Length > 3 && tallaSeleccionada.Length > 2 && tipoSeleccionado.Length > 3)
            {
                MySqlCommand cmd = new MySqlCommand("select Prenda.id,nombrePrenda as Prenda, descripcion as Talla , tipoPrenda as Tipo, nombreEmpresa as Empresa ,prenda.precio    from movimiento " +
                "inner join prenda on movimiento.idPrenda = prenda.id" +
                " inner join tipoprenda on prenda.idTipoPrenda = tipoprenda.id" +
                " inner join talla on prenda.idTalla = talla.id " +
                "inner join empresa on prenda.idEmpresa = empresa.id where idBodega = 1 and empresa.nombreEmpresa = '" + empresaSeleccionada
                + "' and talla.descripcion =' " + tallaSeleccionada + "' and tipoprenda.tipoprenda = '" + tipoSeleccionado + "'", connection);

                dt.Load(cmd.ExecuteReader());
            } else if (empresaSeleccionada.Length > 3 && tallaSeleccionada.Length > 2 && tipoSeleccionado.Length < 3)
            {
                MySqlCommand cmd = new MySqlCommand("select Prenda.id,nombrePrenda as Prenda, descripcion as Talla , tipoPrenda as Tipo, nombreEmpresa as Empresa ,prenda.precio    from movimiento " +
               "inner join prenda on movimiento.idPrenda = prenda.id" +
               " inner join tipoprenda on prenda.idTipoPrenda = tipoprenda.id" +
               " inner join talla on prenda.idTalla = talla.id " +
               "inner join empresa on prenda.idEmpresa = empresa.id where idBodega = 1 and empresa.nombreEmpresa = '" + empresaSeleccionada
               + "' and talla.descripcion = '" + tallaSeleccionada + "'", connection);
                dt.Load(cmd.ExecuteReader());
            }
            else if (empresaSeleccionada.Length > 3 && tallaSeleccionada.Length < 2 && tipoSeleccionado.Length > 3)
            {
                MySqlCommand cmd = new MySqlCommand("select Prenda.id,nombrePrenda as Prenda, descripcion as Talla , tipoPrenda as Tipo, nombreEmpresa as Empresa ,prenda.precio    from movimiento " +
               "inner join prenda on movimiento.idPrenda = prenda.id" +
               " inner join tipoprenda on prenda.idTipoPrenda = tipoprenda.id" +
               " inner join talla on prenda.idTalla = talla.id " +
               "inner join empresa on prenda.idEmpresa = empresa.id where idBodega = 1 and empresa.nombreEmpresa = '" + empresaSeleccionada
               + "' and tipoprenda.tipoprenda = '" + tipoSeleccionado + "'", connection);
                dt.Load(cmd.ExecuteReader());
            }
            else if (empresaSeleccionada.Length > 3 && tallaSeleccionada.Length < 2 && tipoSeleccionado.Length < 3)
            {
                MySqlCommand cmd = new MySqlCommand("select Prenda.id,nombrePrenda as Prenda, descripcion as Talla , tipoPrenda as Tipo, nombreEmpresa as Empresa ,prenda.precio    from movimiento " +
               "inner join prenda on movimiento.idPrenda = prenda.id" +
               " inner join tipoprenda on prenda.idTipoPrenda = tipoprenda.id" +
               " inner join talla on prenda.idTalla = talla.id " +
               "inner join empresa on prenda.idEmpresa = empresa.id where idBodega = 1 and empresa.nombreEmpresa = '" + empresaSeleccionada
               + "'", connection);
                dt.Load(cmd.ExecuteReader());
            }
            else if (empresaSeleccionada.Length < 3 && tallaSeleccionada.Length > 2 && tipoSeleccionado.Length > 3)
            {
                MySqlCommand cmd = new MySqlCommand("select Prenda.id,nombrePrenda as Prenda, descripcion as Talla , tipoPrenda as Tipo, nombreEmpresa as Empresa ,prenda.precio    from movimiento " +
               "inner join prenda on movimiento.idPrenda = prenda.id" +
               " inner join tipoprenda on prenda.idTipoPrenda = tipoprenda.id" +
               " inner join talla on prenda.idTalla = talla.id " +
               "inner join empresa on prenda.idEmpresa = empresa.id where idBodega = 1 and talla.descripcion ='" + tallaSeleccionada
               + "' and tipoprenda.tipoprenda ='" + tipoSeleccionado + "'", connection);
                dt.Load(cmd.ExecuteReader());
            }
            else if (empresaSeleccionada.Length < 3 && tallaSeleccionada.Length > 2 && tipoSeleccionado.Length < 3)
            {
                MySqlCommand cmd = new MySqlCommand("select Prenda.id,nombrePrenda as Prenda, descripcion as Talla , tipoPrenda as Tipo, nombreEmpresa as Empresa ,prenda.precio    from movimiento " +
               "inner join prenda on movimiento.idPrenda = prenda.id" +
               " inner join tipoprenda on prenda.idTipoPrenda = tipoprenda.id" +
               " inner join talla on prenda.idTalla = talla.id " +
               "inner join empresa on prenda.idEmpresa = empresa.id where idBodega = 1 and talla.descripcion ='" + tallaSeleccionada
               + "'", connection);
                dt.Load(cmd.ExecuteReader());
            }
            else if (empresaSeleccionada.Length < 3 && tallaSeleccionada.Length < 2 && tipoSeleccionado.Length > 3)
            {
                MySqlCommand cmd = new MySqlCommand("select Prenda.id,nombrePrenda as Prenda, descripcion as Talla , tipoPrenda as Tipo, nombreEmpresa as Empresa ,prenda.precio    from movimiento " +
               "inner join prenda on movimiento.idPrenda = prenda.id" +
               " inner join tipoprenda on prenda.idTipoPrenda = tipoprenda.id" +
               " inner join talla on prenda.idTalla = talla.id " +
               "inner join empresa on prenda.idEmpresa = empresa.id where idBodega = 1 and tipoprenda.tipoprenda ='"
               + tipoSeleccionado + "'", connection);
                dt.Load(cmd.ExecuteReader());
            }
            dt_prendas_bodega_tienda.DataContext = dt;
            connection.Close();
        }

        private void ComboTalla_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tallaSeleccionada = comboTalla.SelectedItem.ToString().Trim();
            getTablaBodega();
        }

        private void ComboPrenda_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tipoSeleccionado = comboPrenda.SelectedItem.ToString().Trim();
            getTablaBodega();
        }

        private void fillingOrderTable (){
            dt_orden.Columns.Add("Cantidad", typeof(int));
            dt_orden.Columns.Add("Descripcion", typeof(string));
            dt_orden.Columns.Add("Precio Unidad", typeof(double));
            dt_orden.Columns.Add("Valor", typeof(double));
            dt_orden.Columns.Add("pedido", typeof(bool));
            dt_tablaResumen.DataContext = dt_orden;
        }
        private void Dt_prendas_bodega_tienda_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            System.Windows.Controls.DataGrid gd = (System.Windows.Controls.DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;

            if (row_selected != null)
            {
                dt_orden.Rows.Add(new object[] { 1 , row_selected[1].ToString()+"-"+row_selected[0].ToString(), row_selected[5], cantidad *double.Parse(row_selected[5].ToString()) , false });
            }
            if (dt_orden.Rows.Count > 0)
            {
                sumarTotal();
               
            }

            dt_tablaResumen.DataContext = dt_orden;
           

        }

        private void Dt_prendas_bodega_tienda_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            
        }

        private void Dt_tablaResumen_CurrentCellChanged(object sender, EventArgs e)
        {
            
            Console.WriteLine("hubo un cambio");
            //Console.WriteLine(dt_tablaResumen.CurrentColumn.Header.ToString() + ":") ;
            foreach(DataRow row in dt_orden.Rows)
            {
               // Console.WriteLine(row["Cantidad"].ToString());
                row["Valor"] = double.Parse(row["Cantidad"].ToString()) * double.Parse(row["Precio Unidad"].ToString());
            }
            sumarTotal();
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {

          //  dt_orden.Items.Remove(dt_tablaResumen.SelectedItem);
        }

        private void TxtBuscador_KeyUp_1(object sender, System.Windows.Input.KeyEventArgs e)
        {
            connection.Open();
            MySqlCommand cmd = new MySqlCommand("select Prenda.id,nombrePrenda as Prenda, descripcion as Talla , tipoPrenda as Tipo, nombreEmpresa as Empresa ,prenda.precio    from movimiento " +
                "inner join prenda on movimiento.idPrenda = prenda.id" +
                " inner join tipoprenda on prenda.idTipoPrenda = tipoprenda.id" +
                " inner join talla on prenda.idTalla = talla.id " +
                "inner join empresa on prenda.idEmpresa = empresa.id where idBodega = 1 and Prenda.id,nombrePrenda like('%" + txtBuscador.Text + "%')", connection);
            DataTable dt = new DataTable();
            dt.Load(cmd.ExecuteReader());
            dt_prendas_bodega_tienda.DataContext = dt;
            connection.Close();
        }
        private void sumarTotal()
        {
            Total = 0;
            foreach (DataRow row in dt_orden.Rows)
            {
                Console.WriteLine(row["Valor"].ToString());
                Total += double.Parse(row["Valor"].ToString());
            }
            txtTotal.Text = Total.ToString();
        }
        public void fillAutoComplete()
        {
            connection.Open();
            MySqlCommand cmd = new MySqlCommand("select id , nombre from cliente", connection);
            AutoCompleteStringCollection autoCliente = new AutoCompleteStringCollection();
            MySqlDataReader clientes = cmd.ExecuteReader();
            while (clientes.Read())
            {
                comboTalla.Items.Add(clientes.GetString("id"));
                autoCliente.Add(clientes.GetString("id") + "-" + clientes.GetString("nombre"));
            }
            connection.Close();
        }

        private void BtnFacturar_Click(object sender, RoutedEventArgs e)
        {
            int lastFactura = 0;
            int lastDetalle = 0;
            string[] cadenaCliente;
            int idCliente = 0;
            double totalFactura;
            int vendedor= 0;

            int cantidad;
            // descripcion tiene el id del producto separado por guion
            string[] descripcion ;
            double precioUnidad;
            double valorTotal;
            bool banderaPedido;
            if (textBox.Text.Equals(""))
            {
                System.Windows.MessageBox.Show("Creando orden para despachar");
            }
            else {
                try
                {
                    cadenaCliente = textBox.Text.Split('-');
                    idCliente = int.Parse(cadenaCliente[1]);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Compruebe cliente");
                    throw;
                }
            }




            try
            {
                MySqlCommand cmd;
                //obtenemos el id del cliente 
                
                //obtenermos la sumatoria total 
                //double no int OJO
                totalFactura = int.Parse(txtTotal.Text);
                DateTime time = DateTime.Now;
                //obtenemos el vendedor que por el momento sera estatico 
                //ACA VA USUARIO O VENDEDOR
                vendedor = 1;
                connection.Open();
                string fechaHora = time.ToString(@"dd/MM/yyyy hh:mm:ss tt", new System.Globalization.CultureInfo("en-US"));
                if (textBox.Text.Equals(""))
                {
                    cmd= new MySqlCommand("insert into factura(fecha,idVendedor,total,cobro) values(str_to_date('" + fechaHora + "', '%d/%m/%Y %h:%i:%s %p') , " + vendedor.ToString()  + "," + totalFactura.ToString() + ",0)", connection);
                }else cmd = new MySqlCommand("insert into factura(fecha,idVendedor,idCliente,total,cobro) values(str_to_date('" + fechaHora + "', '%d/%m/%Y %h:%i:%s %p') , " + vendedor.ToString()+","+ idCliente.ToString() + "," + totalFactura.ToString() + ",1)", connection);
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            try
            {
                //obtenemos la ultima factura para ser procesados con su detalle correspondiente
                
                connection.Open();
                MySqlCommand cmd2 = new MySqlCommand("select max(id) from factura ", connection);
                MySqlDataReader idLastFactura = cmd2.ExecuteReader();

                while (idLastFactura.Read())
                {
                    lastFactura = int.Parse(idLastFactura.GetString(0));
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("_________");
                Console.WriteLine(ex);
                Console.WriteLine("_________");
                throw;
            }

            foreach (DataRow row in dt_orden.Rows)
            {
                cantidad = (int)row[0];
                // descripcion tiene el id del producto separado por guion
                descripcion = row[1].ToString().Split('-');
                precioUnidad = (double)row[2];
                valorTotal = (double)row[3];
                banderaPedido = (bool)row[4];


                try
                {

                    if (lastFactura != 0)
                    {
                        connection.Open();
                        MySqlParameter[] pms = new MySqlParameter[5];
                        pms[0] = new MySqlParameter("idfactura", lastFactura);
                        pms[1] = new MySqlParameter("quantity", cantidad);
                        pms[2] = new MySqlParameter("precioUnitario", valorTotal);
                        pms[3] = new MySqlParameter("idPrendaDebitar",Int32.Parse(descripcion[1]));
                        pms[4] = new MySqlParameter("idBodegaDebitar", 1);
                        MySqlCommand command = new MySqlCommand();

                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "addDetalleFactura";
                        command.Connection = connection;
                        command.Parameters.Add("@res", MySqlDbType.VarChar).Direction = System.Data.ParameterDirection.Output;
                        command.Parameters.AddRange(pms);
                        if (command.ExecuteNonQuery() == 1)
                        {
                            //MessageBox.Show("yes"); 
                            Console.WriteLine(Convert.ToString(command.Parameters["@res"].Value));
                            if (Convert.ToString(command.Parameters["@res"].Value).Equals("1"))
                            {
                                System.Windows.MessageBox.Show("Revisar Inventario en tienda ");
                            }
                        }
                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("_________");
                    Console.WriteLine(ex);
                    Console.WriteLine("_________");
                    throw;
                }
                if (banderaPedido)
                {

                    //obtener el ultimo detalle 
                    connection.Open();
                    MySqlCommand cmdLastid = new MySqlCommand("select max(id) from factura ", connection);
                    MySqlDataReader idLastDetalle = cmdLastid.ExecuteReader();

                    while (idLastDetalle.Read())
                    {
                        lastDetalle = int.Parse(idLastDetalle.GetString(0));
                    }
                    connection.Close();

                    connection.Open();
                    string insertPedido = "insert into pedido(idDetalle , idPrenda , cantidad, observacion , fecha_emision , estado) " +
                        "values(" + lastDetalle + "," + descripcion[1] + "," + cantidad + ",'pendiente de entrega',str_to_date('" + DateTime.Now.ToString("M/d/yyyy") + "', '%m/%d/%Y'))";
                    MySqlCommand cmd3 = new MySqlCommand(insertPedido, connection);

                    connection.Close();
                }
                else
                {
                    Console.WriteLine("no pedido");
                }
            }


            this.Close();
        }



        private void TextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
           

            bool found = false;
            var border = (resultStack.Parent as ScrollViewer).Parent as Border;
            var data = updateClientSearch();

            string query = (sender as System.Windows.Controls.TextBox).Text;

            if (query.Length == 0)
            {
                // Clear   
                resultStack.Children.Clear();
                border.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                border.Visibility = System.Windows.Visibility.Visible;
            }

            // Clear the list   
            resultStack.Children.Clear();

            // Add the result   
            foreach (var obj in data)
            {
                if (obj.ToLower().StartsWith(query.ToLower()))
                {
                    // The word starts with this... Autocomplete must work   
                    addItem(obj);
                    found = true;
                }
            }

            if (!found)
            {
                resultStack.Children.Add(new TextBlock() { Text = "No results found." });
            }

        }
        private void addItem(string text)
        {
            TextBlock block = new TextBlock();

            // Add the text   
            block.Text = text;

            // A little style...   
            block.Margin = new Thickness(2, 3, 2, 3);
            block.Cursor = System.Windows.Input.Cursors.Hand;

            // Mouse events   
            block.MouseLeftButtonUp += (sender, e) =>
            {
                textBox.Text = (sender as TextBlock).Text;
            };

            block.MouseEnter += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.PeachPuff;
            };

            block.MouseLeave += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.Transparent;
            };

            // Add to the panel   
            resultStack.Children.Add(block);
        }
        public List<string> updateClientSearch()
        {
            List<string> data = new List<string>();
            connection.Open();
            MySqlCommand cmd = new MySqlCommand("select id , nombre from cliente", connection);
            AutoCompleteStringCollection autoCliente = new AutoCompleteStringCollection();
            MySqlDataReader clientes = cmd.ExecuteReader();
            while (clientes.Read())           {

                data.Add(clientes.GetString("nombre") + "-" + clientes.GetString("id"));
            }
            connection.Close();
            return data;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Config_windows.newCliente nuevoCliente = new Config_windows.newCliente();
            nuevoCliente.Show();

        }
    }
}
