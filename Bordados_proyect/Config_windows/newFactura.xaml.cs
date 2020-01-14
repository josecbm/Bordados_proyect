using Microsoft.VisualBasic;
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
using apifel4;

namespace Bordados_proyect.Config_windows
{
    /// <summary>
    /// Interaction logic for newFactura.xaml
    /// </summary>
    /// 
    
    public partial class newFactura : Window
    {


        string connectionString = "SERVER=localhost;DATABASE=bordados_db;UID=root;PASSWORD=1234;";
        RequestCertificacionFel request = new RequestCertificacionFel();
        MySqlConnection connection;
        int facturaActual;
        double totalFacturaActual;
        bool banderaDescuento;
        DataTable dt2;
        //receptor(string IDReceptor, string NombreReceptor, string CodigoPostal, string CorreoReceptor, string Pais, string Departamento, string Municipio, string Direccion, string TipoEspecial);
        string idReceptor, nombreReceptor, correoReceptor, nitReceptor, direccionReceptor;
        public newFactura(int factura)
        {
            InitializeComponent();
            connection = new MySqlConnection(connectionString);
            facturaActual = factura;
            showFacturaDetails();
            getTotalFactura();
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
        public void showFacturaDetails()
        {
            connection.Open();

            MySqlCommand cmd2 = new MySqlCommand("select d.cantidad, p.nombrePrenda, p.id , d.precioTotal , f.fecha  from factura f "+
            "inner join detalle d on f.id = d.idFactura "+
            "inner join prenda p on d.idPrenda = p.id "+
            "where f.id ="+facturaActual+" ;", connection);

           dt2= new DataTable();
            dt2.Load(cmd2.ExecuteReader());

            dt_ordenes_pend.DataContext = dt2;
            connection.Close();
        }
        public void getTotalFactura()
        {
            connection.Open();
            MySqlCommand cmd = new MySqlCommand("select f.total  from factura f " +
           "inner join detalle d on f.id = d.idFactura " +
           "inner join prenda p on d.idPrenda = p.id " +
           "where f.id =" + facturaActual + " ;", connection);
            MySqlDataReader fA = cmd.ExecuteReader();
            while (fA.Read())
            {
                totalFacturaActual = double.Parse( fA.GetString(0));
            }
            label2.Content = totalFacturaActual.ToString();
            connection.Close();
        }

        private void BtnFacturar_Click(object sender, RoutedEventArgs e)
        {
            //rstring NombreCorto1, int CodigoUnidadGravable1, string CantidadUnidadesGravables1, string MontoGravable1, string MontoImpuesto1);
            double preciocantidad=0;
            double codigoUnidadGravable = 0;
            double cantidadUnidadGravable = 0;
            double montoGravable = 0;
            double montoImpuesto = 0;
            double totalMontoImpuesto = 0;

            int iterator = 1;
            int descuento = 0;

            string response;
            bool Datos_generales;
            bool Datos_emisor;
            bool Datos_receptor;
            bool Frases;
            bool Item_un_impuesto;
            bool Total_impuestos;
            bool Totales;
            bool Adenda;
            bool Agregar_adenda;
            bool Complemento_notas;
            bool Complemento_cambiaria;
            bool Complemento_especial;
            bool Complemento_exportacion;



            if (textBox.Text.Equals(""))
            {
                System.Windows.MessageBox.Show("Seleccione un cliente");
                return;
            }

            // buscar cliente y llengar parametros ;
            string[] cadenaCliente = textBox.Text.Split('-');
            connection.Open();
            MySqlCommand cmddataCliente = new MySqlCommand(" select id , nombre , direccion , email , nit from cliente where id = "+cadenaCliente[2]+"", connection);
            MySqlDataReader fA = cmddataCliente.ExecuteReader();
            while (fA.Read())
            {
                idReceptor = fA.GetString(0);
                nombreReceptor = fA.GetString(1);
                direccionReceptor = fA.GetString(2);
                nombreReceptor = fA.GetString(3);
                nitReceptor = fA.GetString(4);
            }
            
            connection.Close();

            connection.Open();
            MySqlCommand cmd = new MySqlCommand("update factura set cobro = 1 , idCliente = "+cadenaCliente[2]+"  where factura.id="+facturaActual+"" , connection);
            cmd.ExecuteNonQuery();
            connection.Close();
            this.Close();

            // esto esta pendiente 
            Datos_generales = request.Datos_generales("GTQ", DateTime.Now.ToString("dd/MM/yyyy"), "FACT", "", "");
            Datos_emisor = request.Datos_emisor("GEN", 1, "01001", "bordados.francy@gmail.com", "GT", "GUATEMALA", "GUATEMALA", "CUIDAD", "5347319", "BORDADOS FRANCY", "BORDADOS");
            Datos_receptor = request.Datos_receptor(nitReceptor, nombreReceptor , "01001", correoReceptor, "GT", "GUATEMALA", "GUATEMALA", direccionReceptor, "");
            Frases = request.Frases(1, 1);
            
            foreach (DataRow row in dt2.Rows)
            {

                //d.cantidad, p.nombrePrenda, p.id , d.precioTotal , f.fecha
               // Console.WriteLine(row["Valor"].ToString());
                try
                {
                    preciocantidad = 0;
                    preciocantidad = Double.Parse(row["precioTotal"].ToString()) * int.Parse(row["cantidad"].ToString());
                    montoImpuesto = preciocantidad * 0.12;
                    montoGravable = preciocantidad - montoImpuesto;
                    totalMontoImpuesto += montoImpuesto;
                }
                catch (Exception)
                {

                    throw;
                }
                //       NombreCorto1, int CodigoUnidadGravable1, string CantidadUnidadesGravables1, string MontoGravable1, string MontoImpuesto1);
                Item_un_impuesto = request.Item_un_impuesto("B", "UND", row["cantidad"].ToString(), row["nombrePrenda"].ToString(),
                                    iterator, row["precioTotal"].ToString(), preciocantidad.ToString(), descuento.ToString(), totalFacturaActual.ToString(),
                                    "IVA", iterator, row["cantidad"].ToString(), montoGravable.ToString(), montoImpuesto.ToString()) ;


                iterator++;
            }

            // totalMonto = este va a ser la sumatoria de todos los montos acumulados 
            Total_impuestos = request.total_impuestos("IVA", totalMontoImpuesto.ToString()) ;
            Totales = request.Totales(totalFacturaActual.ToString());
            //Complemento_notas = request.Complemento_notas("","","","","","","","","");
            //Complemento_cambiaria = request.Complemento_cambiaria("","","");
            //Complemento_especial = request.Complemento_especial("","","","","","");
            //Complemento_exportacion = request.Complemento_exportacion("","","","","","","","","","","","","");


            Adenda = request.Adendas("Codigo_cliente", idReceptor) ;//Información Adicional
            Adenda = request.Adendas("Observaciones", "ESTA ES UNA PRUEBA");

            Agregar_adenda = request.Agregar_adendas();
            //Adenda = request.Adendas("PERSONALIZADO1", "ESTA ES UNA ADENDA");
            response = request.enviar_peticion_fel("BORDADOS_FRANCY"/*USUARIO*/, "ed308938934731b734e9836bdb96ce57"/*LLAVE*/, "NDEBEXC1"/*IDENTIFICADOR DEL DOCUMENTO*/, "jcbautista95@gmail.com"/*correo copia*/, "BORDADOS_FRANCY"/*USUARIO*/, "F43E482EC149C5B7C475152FA265B43F"/*llave emisor*/, true);
            System.Windows.MessageBox.Show(response);


            //#########################


            /*
             
                USUARIO FIRMA:           BORDADOS_FRANCY  
                LLAVE: ed308938934731b734e9836bdb96ce57


                USUARIO:           BORDADOS_FRANCY  
                LLAVE CERTIFICACIÓN:            F43E482EC149C5B7C475152FA265B43F  
                NIT:  5347319
             */
        }
        

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            connection.Open();
            MySqlCommand cmd = new MySqlCommand("", connection);
            cmd.ExecuteNonQuery();
            connection.Close();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Config_windows.newCliente nuevoCliente = new Config_windows.newCliente();
            nuevoCliente.Show();
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
            MySqlCommand cmd = new MySqlCommand("select id , nit , nombre from cliente", connection);
            AutoCompleteStringCollection autoCliente = new AutoCompleteStringCollection();
            MySqlDataReader clientes = cmd.ExecuteReader();
            while (clientes.Read())
            {

                data.Add(clientes.GetString("nit")+ "-" + clientes.GetString("nombre") + "-" + clientes.GetString("id"));
            }
            connection.Close();
            return data;
        }

        private void TextBox_KeyUp_1(object sender, System.Windows.Input.KeyEventArgs e)
        {

        }

        private void BtnDescuento_Click(object sender, RoutedEventArgs e)
        {

            if (banderaDescuento)
            {
                System.Windows.MessageBox.Show("Ya se aplico descuento");
                return;
            }
            string message, title, defaultValue;
            object myValue;

            message = "Ingrese el porsentaje de descuento en porcentaje";

            title = "Descuento";

            defaultValue = "10";
            double operacion;

            myValue = Interaction.InputBox(message, title, defaultValue);

            if ((string)myValue == "")
            {
               
                Microsoft.VisualBasic.Interaction.MsgBox("Descuento aplicado : " + myValue.ToString(), MsgBoxStyle.OkOnly | MsgBoxStyle.Information, "Descuento");
                banderaDescuento = true;
            }
            else
            {
                Interaction.MsgBox("Descuento de " + myValue.ToString());
            }

            try
            {
                
                operacion = totalFacturaActual - (double.Parse(myValue.ToString()) / 100) * totalFacturaActual ;
            }
            catch (Exception ex)
            {

                throw;
            }

            label2.Content = operacion.ToString();
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            //boton para agregar observacion a facturas
            connection.Open();
            MySqlCommand cmd = new MySqlCommand("update factura set observacionFac='"+txtObservacion.Text+"' where factura.id="+facturaActual+"", connection);
            cmd.ExecuteNonQuery();
            connection.Close();
            txtObservacion.Text = "";
         }
    }
}
