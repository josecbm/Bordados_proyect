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
    public partial class newFactura : Window
    {
        string connectionString = "SERVER=localhost;DATABASE=bordados_db;UID=root;PASSWORD=1234;";
        apifel4.RequestCertificacionFel request = new RequestCertificacionFel();
        MySqlConnection connection;
        int facturaActual;
        double totalFacturaActual;
        bool banderaDescuento;
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

            DataTable dt2 = new DataTable();
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
            if (textBox.Text.Equals(""))
            {
                System.Windows.MessageBox.Show("Seleccione un cliente");
                return;
            }
            string[] cadenaCliente = textBox.Text.Split('-');
            connection.Open();
            MySqlCommand cmd = new MySqlCommand("update factura set cobro = 1 , idCliente = "+cadenaCliente[1]+"  where factura.id="+facturaActual+"" , connection);
            cmd.ExecuteNonQuery();
            connection.Close();
            this.Close();

            // esto esta pendiente 


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
            /*bool Complemento_notas;*/
            /*bool Complemento_cambiaria;*/
            /*bool Complemento_especial;*/
            /*bool Complemento_exportacion;*/

            Datos_generales = request.Datos_generales("GTQ", "2019-09-0T09:58:00-06:00", "FACT", "", "");
            Datos_emisor = request.Datos_emisor("GEN", 1, "01001", "demo@demo.com.gt", "GT", "GUATEMALA", "GUATEMALA", "CUIDAD", "5203333", "DEMO, SOCIEDAD ANONIMA", "DEMO");
            Datos_receptor = request.Datos_receptor("76365204", "Jaime Alvizures", "01001", "leyoalvizures4456@gmail.com", "GT", "GUATEMALA", "GUATEMALA", "CUIDAD", "");
            Frases = request.Frases(1, 1);
            Frases = request.Frases(2, 1);
            Item_un_impuesto = request.Item_un_impuesto("B", "UND", "1", "PRODUCTO1", 1, "120", "120", "0", "120", "IVA", 1, "", "107.14", "12.86");
            Total_impuestos = request.total_impuestos("IVA", "12.86");
            Totales = request.Totales("120");
            //Complemento_notas = request.Complemento_notas("","","","","","","","","");
            //Complemento_cambiaria = request.Complemento_cambiaria("","","");
            //Complemento_especial = request.Complemento_especial("","","","","","");
            //Complemento_exportacion = request.Complemento_exportacion("","","","","","","","","","","","","");


            Adenda = request.Adendas("Codigo_cliente", "C01");//Información Adicional
            Adenda = request.Adendas("Observaciones", "ESTA ES UNA ADENDA");

            Agregar_adenda = request.Agregar_adendas();
            //Adenda = request.Adendas("PERSONALIZADO1", "ESTA ES UNA ADENDA");
            response = request.enviar_peticion_fel("DEMO_FEL"/*USUARIO*/, "8C6439B233B4FAA6BC2E7BA8E736A173"/*LLAVE*/, "NDEBEXC1"/*IDENTIFICADOR DEL DOCUMENTO*/, "demo@demo.com.gt"/*correo copia*/, "COUNISA"/*USUARIO*/, "9242cc46a4ef81a457680f2b6f02fb51"/*llave emisor*/, true);
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
            MySqlCommand cmd = new MySqlCommand("select id , nombre from cliente", connection);
            AutoCompleteStringCollection autoCliente = new AutoCompleteStringCollection();
            MySqlDataReader clientes = cmd.ExecuteReader();
            while (clientes.Read())
            {

                data.Add(clientes.GetString("nombre") + "-" + clientes.GetString("id"));
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
