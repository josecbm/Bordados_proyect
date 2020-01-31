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
using System.Windows.Threading;
using SpreadsheetLight;
using DocumentFormat.OpenXml;
using System.Globalization;
using System.IO;
using Bordados_proyect.Config_windows;

namespace Bordados_proyect
{
    /// <summary>
    /// Interaction logic for pagina_principal.xaml
    /// </summary>
    public partial class pagina_principal : Window
    {
        int estadoActual;
        int buscador , buscador2, idBodegaSeleccionada, vendedorActual;
        int banderaFlujoVentas  = 0;
        String bodegaSeleccionada;
        string connectionString = "SERVER=localhost;DATABASE=bordados_db;UID=root;PASSWORD=1234;";
        MySqlConnection connection;
        DataTable data_flujo_ventas = new DataTable();
        private Timer timer1;
        int banderaBusquedaDinamica;

        public pagina_principal(int vendedor)
        {
            InitializeComponent();
            vendedorActual = vendedor;

            fecha_init.SelectedDate = DateTime.Now;
            Console.WriteLine();
            connection = new MySqlConnection(connectionString);
            
            estadoActual = 0;
            banderaBusquedaDinamica = 0;
            
            showInventarioTienda();
            fillCombo_Bodegas();
            ventaDia();
            pedidosPendientesDia();
            showOrdenesPendientes();
            showFacturas();
            showVendedor();
            InitTimer();

            //DispatcherTimer dispatcherTimer = new DispatcherTimer();
            //dispatcherTimer.Tick += new EventHandler(timer1_Tick);
            //dispatcherTimer.Interval = new TimeSpan(0, 0, 6);
            //dispatcherTimer.Start();
        }
        public void InitTimer()
        {
            timer1 = new Timer();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 2000; // in miliseconds
            timer1.Start();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            showOrdenesPendientes();
            ventaDia();
            pedidosPendientesDia();
            //            showFacturas();
        }

        public void showOrdenesPendientes()
        {
            connection.Open();
            MySqlCommand cmd = new MySqlCommand("select factura.id, fecha  , usr.nombre as Vendedor , total as Total from factura" +
                " inner join usr on factura.idVendedor = usr.id" +
                " where cobro = 0; ", connection);
            DataTable dt = new DataTable();
            dt.Load(cmd.ExecuteReader());
            dt_ordenes_pend.DataContext = dt;
            
            connection.Close();
        }
        public void showVendedor()
        {
            connection.Open();

            string val = "select nombre from usr where usr.id ="+vendedorActual;
            MySqlCommand cmd = new MySqlCommand(val, connection);
            MySqlDataReader r = cmd.ExecuteReader();
            while (r.Read())
            {
                lblUsuario.Content =  r[0].ToString();
            }

            connection.Close();
        }
       
        public void ventaDia()
        {
            connection.Open();

            string val = "select sum(f.total) as total from factura f where f.fecha between \'" + DateTime.Now.ToString("yyyy-M-d") + "  00:00:00\' and  \'" + DateTime.Now.ToString("yyyy-M-d") + " 23:59:59\'  and cobro=1";
            MySqlCommand cmd = new MySqlCommand(val, connection);
            MySqlDataReader r = cmd.ExecuteReader();
            while (r.Read())
            {
                txtVentaDia.Text = "Q." + r[0].ToString();
            }
            connection.Close();

        }
        public void pedidosPendientesDia()
        {
            connection.Open();

            string val = " select count(id) from pedido where estado=0 and fecha_emision = curdate()";
            MySqlCommand cmd = new MySqlCommand(val, connection);
            MySqlDataReader r = cmd.ExecuteReader();
            while (r.Read())
            {
                txtPedidosPendientes.Text = r[0].ToString();
            }

            connection.Close();
        }
        public void showInventarioTienda()
        {
            connection.Open();

            MySqlCommand cmd2 = new MySqlCommand("select nombrePrenda as Prenda, descripcion as Talla , tipoPrenda as Tipo, nombreEmpresa as Empresa, precio as Precio,stock    from movimiento " +
                "inner join prenda on movimiento.idPrenda = prenda.id" +
                " inner join tipoprenda on prenda.idTipoPrenda = tipoprenda.id" +
                " inner join talla on prenda.idTalla = talla.id " +
                "inner join empresa on prenda.idEmpresa = empresa.id where idBodega = 1 ; ", connection);

            DataTable dt2 = new DataTable();
            dt2.Load(cmd2.ExecuteReader());

            dt_disponibilidad.DataContext = dt2;
            connection.Close();
        }
        public void showFacturas()
        {

            
            banderaFlujoVentas = 0;
            connection.Open();

            Console.WriteLine(fecha_init.Text);

            MySqlCommand cmd2;
            
            string dateHoy = DateTime.ParseExact(fecha_init.SelectedDate.Value.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
            if ((bool)checkBox_oneDate.IsChecked)
            {
                cmd2 = new MySqlCommand("select factura.id, fecha  ,  c.nombre as NombreCliente , c.nit , total as Total , usr.nombre as Vendedor  from factura" +
             " inner join usr on factura.idVendedor = usr.id " +
             " inner join cliente c on c.id = factura.idCliente" +
             " where cobro = 1 and fecha between'" + DateTime.ParseExact(fecha_init.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd") + " 00:00:00'  and '" + DateTime.ParseExact(fecha_end.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd") + " 23:59:59' ", connection);
                //
            }
            else
            {
                cmd2 = new MySqlCommand("select factura.id, fecha  ,  c.nombre as NombreCliente , c.nit , total as Total , usr.nombre as Vendedor  from factura" +
              " inner join usr on factura.idVendedor = usr.id " +
              " inner join cliente c on c.id = factura.idCliente" +
              " where cobro = 1  and fecha between '" + dateHoy + " 00:00:00' and '" + dateHoy + " 23:59:59'", connection);
            }
             


            data_flujo_ventas = new DataTable();
            data_flujo_ventas.Load(cmd2.ExecuteReader());

            dt_flujo_ventas.DataContext = data_flujo_ventas;
            connection.Close();
        }
        public void showPedidos()
        {
            banderaFlujoVentas = 1;
            connection.Open();
            MySqlCommand cmd2 = new MySqlCommand("select  c.nombre , c.telefono, p.nombrePrenda , pe.cantidad , pe.observacion , pe.fecha_emision , pe.fecha_entrega from pedido pe inner join detalle d on d.idDetalle = pe.idDetalle "+
                                     " inner join factura f on f.id = d.idFactura"+
                                     " inner join cliente c on c.id = f.idCliente"+
                                     " inner join prenda p on p.id = pe.idPrenda; ", connection);
            data_flujo_ventas = new DataTable();
            data_flujo_ventas.Load(cmd2.ExecuteReader());

            dt_flujo_ventas.DataContext = data_flujo_ventas;
            connection.Close();
        }

        public void getVentas_dia()
        {
            connection.Open();
            try
            {
                MySqlCommand cmd2 = new MySqlCommand("select * from usr;", connection);
            }
            catch(Exception ex)
            {

            }
            connection.Close();

        }
        private void BtnPower_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        private void GridBarraTitulo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            clean_environment();
            define_environment(0);
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            clean_environment();
            define_environment(1);
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            clean_environment();
            define_environment(2);
        }
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            clean_environment();
            define_environment(3);
        }
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            clean_environment();
            define_environment(4);
        }
        private void clean_environment()
        {
             if(estadoActual == 0)
            {
                dashboard.Visibility = Visibility.Hidden;

            }else if(estadoActual == 1)
            {
                ventas.Visibility = Visibility.Hidden;
            }else if(estadoActual == 2)
            {
                Console.WriteLine("borrando estado 2");
                flujo_de_ventas.Visibility = Visibility.Hidden;
            }else if(estadoActual == 3)
            {
                Console.WriteLine("borrando estado 3");
                perfil.Visibility = Visibility.Hidden;
            }
            else if (estadoActual == 4)
            {
                Console.WriteLine("borrando estado 4");
                config.Visibility = Visibility.Hidden;
            }


        }
        private void define_environment(int dash)
        {
            if (dash == 0)
            {
                estadoActual = 0;
                dashboard.Visibility = Visibility.Visible;

            }
            else if (dash == 1)
            {
                estadoActual = 1;
                ventas.Visibility = Visibility.Visible;
            }
            else if (dash == 2)
            {
                estadoActual = 2;
                flujo_de_ventas.Visibility = Visibility.Visible;
                Console.WriteLine("mostrando ");
            }
            else if (dash == 3)
            {
                estadoActual = 3;
                perfil.Visibility = Visibility.Visible;
                Console.WriteLine("mostrando ");
            }
            else if (dash == 4)
            {

                estadoActual = 4;
                config.Visibility = Visibility.Visible;
            }
        }
        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }


        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            generar_orden nuevaOrden = new generar_orden();
            nuevaOrden.Show();
        }

        private void TextBox_buscador_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            MySqlCommand cmd2;
            if (banderaBusquedaDinamica == 0)
            {
                cmd2 = new MySqlCommand("select nombrePrenda as Prenda, descripcion as Talla , tipoPrenda as Tipo, nombreEmpresa as Empresa, precio as Precio,stock    from movimiento " +
              "inner join prenda on movimiento.idPrenda = prenda.id" +
              " inner join tipoprenda on prenda.idTipoPrenda = tipoprenda.id" +
              " inner join talla on prenda.idTalla = talla.id " +
              "inner join empresa on prenda.idEmpresa = empresa.id where idBodega = 1 and  nombrePrenda like('%" + textBox_buscador.Text + "%') ; ", connection);
            }
            else if(banderaBusquedaDinamica == 1)
            {
                cmd2 = new MySqlCommand("select p.nombrePrenda , pe.cantidad , pe.fecha_emision , pe.fecha_entrega from pedido pe" +
                                                " inner join detalle d on d.idPrenda = pe.idPrenda" +
                                                " inner join prenda p on p.id = d.idPrenda where nombrePrenda like('%" + textBox_buscador.Text + "%'); ", connection);

                                                
            }
            else if(banderaBusquedaDinamica == 2)
            {
                cmd2 = new MySqlCommand("select c.nombre as Cliente , c.nit as NIT , f.total as Total , u.nombre as Vendedor , f.fecha as Fechahora from factura f" +
                                               " inner join cliente c on c.id = f.idCliente" +
                                               " inner join usr u on u.id = f.idVendedor where c.nit like('%" + textBox_buscador.Text + "%');; ", connection);

            }
            else if(banderaBusquedaDinamica == 3)
            {
                cmd2 = new MySqlCommand("select  from usr where nombre like('%" + textBox_buscador.Text + "%');", connection);
            }else
            {
                cmd2 = new MySqlCommand("select nombrePrenda as Prenda, descripcion as Talla , tipoPrenda as Tipo, nombreEmpresa as Empresa, precio as Precio,stock    from movimiento " +
                 "inner join prenda on movimiento.idPrenda = prenda.id" +
                 " inner join tipoprenda on prenda.idTipoPrenda = tipoprenda.id" +
                 " inner join talla on prenda.idTalla = talla.id " +
                 "inner join empresa on prenda.idEmpresa = empresa.id where idBodega = 1 and  nombrePrenda  like('%" + textBox_buscador.Text + "%') ; ", connection);
            }
            connection.Open();
           
            
            DataTable dt_dinamica = new DataTable();
            dt_dinamica.Load(cmd2.ExecuteReader());
            dt_disponibilidad.DataContext = dt_dinamica;
            connection.Close();
        }

        private void BtnNueva_empresa_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Esta seguro que desea continuar", "Confirmacion de peticion", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                connection.Open();
                Console.WriteLine("empresa nueva :" + textBox.Text);
                Console.WriteLine("fecha de hoy:" + DateTime.Now.ToString("M/d/yyyy"));

                MySqlCommand cmd = new MySqlCommand("INSERT INTO `bordados_db`.`empresa` (`nombreEmpresa`, `fecha`) VALUES ('" + textBox.Text + "',  str_to_date('" + DateTime.Now.ToString("M/d/yyyy") + "', '%m/%d/%Y'));", connection);

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
                textBox.Text = "";
            }
               
        }

        private void BtnNueva_talla_Click(object sender, RoutedEventArgs e)
        {

            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Esta seguro que desea continuar", "Confirmacion de peticion", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                connection.Open();
                Console.WriteLine("talla nueva :" + textBox1.Text);


                MySqlCommand cmd = new MySqlCommand("INSERT INTO `bordados_db`.`talla` (`descripcion`, `medida` ) VALUES ('" + textBox1.Text + "', '" + textBox_medida.Text + "');", connection);

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
                textBox1.Text = "";
                textBox_buscador.Text = "";
                textBox_medida.Text = "";
            }
               
        }

        private void BtnNueva_tipo_prenda_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Esta seguro que desea continuar", "Confirmacion de peticion", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                connection.Open();
                Console.WriteLine("tipo nuevo :" + textBox1.Text);


                MySqlCommand cmd = new MySqlCommand("INSERT INTO `bordados_db`.`tipoprenda` (`tipoPrenda`) VALUES ('" + textBox3.Text + "');", connection);

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
                textBox3.Text = "";
            }
               
        }

        private void BtnPrenda_Click(object sender, RoutedEventArgs e)
        {
            cargaProductos productos = new cargaProductos();
            productos.Show();
        }

        private void BtnCliente_Click(object sender, RoutedEventArgs e)
        {
            Config_windows.newCliente nuevo = new Config_windows.newCliente();
            nuevo.Show();
        }

        private void BtnInventarioTienda_Click(object sender, RoutedEventArgs e)
        {
            Config_windows.inventarios inventarios = new Config_windows.inventarios();
            inventarios.Show();
        }

        private void VerEmpresa_Click(object sender, RoutedEventArgs e)
        {
            connection.Open();
            MySqlCommand cmd = new MySqlCommand("select * from Empresa;" , connection);
            DataTable dt = new DataTable();

            dt.Load(cmd.ExecuteReader());

            dt_1.DataContext = dt;
            buscador = 1;
            connection.Close();
        }

        private void VerTalla_Click(object sender, RoutedEventArgs e)
        {
            connection.Open();
            MySqlCommand cmd = new MySqlCommand("select * from talla;" , connection);
            DataTable dt = new DataTable();

            dt.Load(cmd.ExecuteReader());

            dt_1.DataContext = dt;
            buscador = 2;
            connection.Close();
        }

        private void VerTipoPrenda_Click(object sender, RoutedEventArgs e)
        {
            connection.Open();
            MySqlCommand cmd = new MySqlCommand("select * from tipoPrenda; ", connection);
            DataTable dt = new DataTable();

            dt.Load(cmd.ExecuteReader());

            dt_1.DataContext = dt;
            buscador = 3;
            connection.Close();
        }

        private void TextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            connection.Open();
            Console.WriteLine(getCmd());
            MySqlCommand cmd2 = new MySqlCommand(getCmd(), connection);

            DataTable dt_dinamica = new DataTable();
            dt_dinamica.Load(cmd2.ExecuteReader());
            dt_1.DataContext = dt_dinamica;
            connection.Close();
        }
        
        private String getCmd() {
            String variante = "";
            if (buscador == 1)
            {
                variante = "select * from Empresa where nombreEmpresa like('%"+txtBuscador.Text+"%');";
            }
            else if (buscador == 2)
            {
                variante = "select * from talla where descripcion like('%"+txtBuscador.Text+"%');";
            }
            else if(buscador == 3)
            {
                variante = "select * from tipoPrenda where tipoPrenda like('%" + txtBuscador.Text+"%');";
            }
            return variante;

        }
        private String getCmd2()
        {
            String variante = "";
            if (buscador2 == 1)
            {
                variante = "select prenda.id, nombrePrenda as Prenda , descripcion as Talla ,  nombreEmpresa as Empresa , tipoPrenda as Prenda from prenda " +
                "inner join Talla on prenda.idTalla = Talla.id" +
                " inner join Empresa on prenda.idEmpresa = Empresa.id " +
                "inner join tipoprenda on prenda.idTipoPrenda = tipoprenda.id where nombrePrenda like('%" + txtBuscador2.Text + "%');";
            }
            else if (buscador2 == 2)
            {
                variante = "select nombre , direccion , telefono , email , nit from cliente where nombre like('%" + txtBuscador2.Text + "%');";
                Console.WriteLine(variante);
            }
            else if (buscador2 == 3)
            {
                variante = "select nombrePrenda as Prenda, descripcion as Talla , tipoPrenda as Tipo, nombreEmpresa as Empresa ,stock    from movimiento " +
                "inner join prenda on movimiento.idPrenda = prenda.id" +
                " inner join tipoprenda on prenda.idTipoPrenda = tipoprenda.id" +
                " inner join talla on prenda.idTalla = talla.id " +
                "inner join empresa on prenda.idEmpresa = empresa.id where idBodega = "+idBodegaSeleccionada+"  and nombrePrenda like('%" + txtBuscador2.Text + "%');";
            }
            return variante;

        }

        private void Btn_verPrenda_Click(object sender, RoutedEventArgs e)
        {
            connection.Open();
            MySqlCommand cmd = new MySqlCommand("select prenda.id, nombrePrenda as Prenda , descripcion as Talla ,  nombreEmpresa as Empresa , tipoPrenda as Prenda from prenda " +
                "inner join Talla on prenda.idTalla = Talla.id" +
                " inner join Empresa on prenda.idEmpresa = Empresa.id " +
                "inner join tipoprenda on prenda.idTipoPrenda = tipoprenda.id", connection);
            DataTable dt = new DataTable();

            dt.Load(cmd.ExecuteReader());

            dt_2.DataContext = dt;
            buscador2 = 1;
            connection.Close();
        }

        private void Btn_verInventario_Click(object sender, RoutedEventArgs e)
        {
            connection.Open();
            MySqlCommand cmd = new MySqlCommand("select nombrePrenda as Prenda, descripcion as Talla , tipoPrenda as Tipo, nombreEmpresa as Empresa ,stock    from movimiento " +
                "inner join prenda on movimiento.idPrenda = prenda.id" +
                " inner join tipoprenda on prenda.idTipoPrenda = tipoprenda.id" +
                " inner join talla on prenda.idTalla = talla.id " +
                "inner join empresa on prenda.idEmpresa = empresa.id where idBodega = "+idBodegaSeleccionada+" ;", connection);
            DataTable dt = new DataTable();

            dt.Load(cmd.ExecuteReader());

            dt_2.DataContext = dt;
            buscador2 = 3;
            connection.Close();
        }

        private void TextBox_KeyUp_1(object sender, System.Windows.Input.KeyEventArgs e)
        {
            connection.Open();
            Console.WriteLine(getCmd());
            MySqlCommand cmd2 = new MySqlCommand(getCmd2(), connection);

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
                Config_windows.newInventario nuevoInventarioProducto_seleccionado = new Config_windows.newInventario(row_selected[0].ToString());
                nuevoInventarioProducto_seleccionado.Show();
            }
        }

        private void Btn_verCliente_Click(object sender, RoutedEventArgs e)
        {
            connection.Open();
            MySqlCommand cmd = new MySqlCommand("select * from cliente;", connection);
            DataTable dt = new DataTable();

            dt.Load(cmd.ExecuteReader());

            dt_2.DataContext = dt;
            buscador2 = 2 ;
            connection.Close();
        }

        private void Dt_ordenes_pend_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int idFacturaSeleccionada = 0 ;
            System.Windows.Controls.DataGrid gd = (System.Windows.Controls.DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                Console.WriteLine(row_selected[0]);
                idFacturaSeleccionada = (int)row_selected[0];
                Config_windows.newFactura facturaNueva = new  Config_windows.newFactura(idFacturaSeleccionada);
                facturaNueva.Show();

            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void CheckBox_oneDate_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)checkBox_oneDate.IsChecked)
            {
                Console.WriteLine("Solo tomar en cuenta fecha Especifica");
                fecha_end.Visibility = Visibility.Visible;
            }
            else fecha_end.Visibility = Visibility.Hidden;
        }

        private void BtnFacturas_Click(object sender, RoutedEventArgs e)
        {
            showFacturas();
        }

        private void BtnPedidos_Click(object sender, RoutedEventArgs e)
        {
            showPedidos();
        }

        private void BtnReporte_Click(object sender, RoutedEventArgs e)
        {
            string datetimeActual  = DateTime.Now.ToString("yyyy-MM-ddThh_mm_ss");
            string path;
            if (banderaFlujoVentas == 0)
            {
                path =  "reporte_ventas_"+datetimeActual+".xlsx";
            }
            else if(banderaFlujoVentas == 1)
            {
                path =  "reporte_pedidos_"+datetimeActual+".xlsx";
            }
            else if (banderaFlujoVentas == 2)
            {
                path =  "reporte_observaciones_" + datetimeActual + ".xlsx";
            }
            else
            {
                path = AppDomain.CurrentDomain.BaseDirectory + "miexcel.xlsx";
                //path = AppDomain.CurrentDomain.BaseDirectory + "miexcel.xlsx";
            }
          
            connection.Open();
            SLDocument documentoExcel = new SLDocument();
            connection.Close();
            documentoExcel.ImportDataTable(1, 1, data_flujo_ventas, true);
            documentoExcel.SaveAs(path);
        }

        private void BtnObservaciones_Click(object sender, RoutedEventArgs e)
        {
            banderaFlujoVentas = 2;
            connection.Open();
            MySqlCommand cmd2;

          
                cmd2 = new MySqlCommand("select factura.id, fecha  ,  c.nombre as NombreCliente , c.nit , total as Total , factura.observacionFac  from factura" +
             " inner join usr on factura.idVendedor = usr.id " +
             " inner join cliente c on c.id = factura.idCliente" +
             " where fecha between'" + DateTime.ParseExact(fecha_init.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd") + " 00:00:00'  and '" + DateTime.ParseExact(fecha_end.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd") + " 23:59:59' ", connection);
                //
            data_flujo_ventas = new DataTable();
            data_flujo_ventas.Load(cmd2.ExecuteReader());

            dt_flujo_ventas.DataContext = data_flujo_ventas;
            connection.Close();
        }


        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("entro en boton de actualizar ordenes");
            showOrdenesPendientes();
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            banderaBusquedaDinamica = 1;
            connection.Open();

            MySqlCommand cmd2 = new MySqlCommand("select p.nombrePrenda , pe.cantidad , pe.fecha_emision , pe.fecha_entrega from pedido pe"+
                                                " inner join detalle d on d.idDetalle = pe.idDetalle " +
                                                " inner join prenda p on p.id = pe.idPrenda; ", connection);

            DataTable dt2 = new DataTable();
            dt2.Load(cmd2.ExecuteReader());

            dt_disponibilidad.DataContext = dt2;
            connection.Close();
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            banderaBusquedaDinamica = 2;
            connection.Open();
            MySqlCommand cmd2 = new MySqlCommand("select c.nombre as Cliente , c.nit as NIT , f.total as Total , u.nombre as Vendedor , f.fecha as Fechahora from factura f"+
                                                " inner join cliente c on c.id = f.idCliente"+
                                                " inner join usr u on u.id = f.idVendedor; ", connection);
            DataTable dt2 = new DataTable();
            dt2.Load(cmd2.ExecuteReader());

            dt_disponibilidad.DataContext = dt2;
            connection.Close();
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            banderaBusquedaDinamica = 0;
            showInventarioTienda();
        }

        private void dt_flujo_ventas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Controls.DataGrid gd = (System.Windows.Controls.DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                // dt_orden.Rows.Add(new object[] { 1, row_selected[1].ToString() + "-" + row_selected[0].ToString(), row_selected[5], cantidad * double.Parse(row_selected[5].ToString()), false });
                Console.WriteLine(row_selected[0].ToString());
                Console.WriteLine(row_selected[1].ToString());
                Console.WriteLine(row_selected[2].ToString());
                Console.WriteLine(row_selected[3].ToString());
                Console.WriteLine(row_selected[4].ToString());
                Console.WriteLine(row_selected[5].ToString());
                detalleCompra dc = new detalleCompra(int.Parse(row_selected[0].ToString()));
                dc.Show();
                

            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            ventaDia();
           // showOrdenesPendientes();
            
        }

        private void ComboBodega_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bodegaSeleccionada = comboBodega.Text;

            connection.Open();
            string v = "select id from bodega where nombreBodega ='" + comboBodega.SelectedItem.ToString() + "';";
            MySqlCommand cmdConsulta = new MySqlCommand("select id from bodega where nombreBodega ='" + comboBodega.SelectedItem.ToString() + "' ", connection);
            idBodegaSeleccionada = (int)cmdConsulta.ExecuteScalar();
            Console.WriteLine("Bodega seleccionada:"+comboBodega.Text +"con el id:"+idBodegaSeleccionada);
            connection.Close();
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
