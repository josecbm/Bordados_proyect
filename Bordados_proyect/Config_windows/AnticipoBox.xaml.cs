using apifel4;
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
    /// Interaction logic for AnticipoBox.xaml
    /// </summary>
    public partial class AnticipoBox : Window
    {
 
        string  correo , nit , nombre , direccion;
        int facturaActual;
        double totalFacturaActual , restante , anticipo ;
        string connectionString = "SERVER=localhost;DATABASE=bordados_db;UID=root;PASSWORD=1234;";
        RequestCertificacionFel request = new RequestCertificacionFel();
        MySqlConnection connection;

        private void txtAnticipo_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                double valTotal = double.Parse(lblTotalFactura.Content.ToString());
                double anticipo = double.Parse(txtAnticipo.Text);
                lblRestante.Content = (valTotal - anticipo).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                throw;
            }
            
        }

        public AnticipoBox(int facturaActual, double totalFacturaActual , string correoFac , string nitFac , string nombreFac , string dirFac)
        {
            InitializeComponent();
            connection = new MySqlConnection(connectionString);
            this.totalFacturaActual = totalFacturaActual;
            this.facturaActual = facturaActual;
            this.correo = correoFac;
            this.nit = nitFac;
            this.nombre = nombreFac;
            this.direccion = dirFac;
            lblTotalFactura.Content = totalFacturaActual.ToString();


        }

        private void btnAceptarAnticipo_Click(object sender, RoutedEventArgs e)
        {
            string response, idDocumento;
            bool Datos_generales;
            bool Datos_emisor;
            bool Datos_receptor;
            bool Frases;
            bool Item_un_impuesto;
            bool Total_impuestos;
            bool Totales;
            bool Adenda;
            bool Agregar_adenda;
            decimal montosinIva , montoIva;
            Guid guid = Guid.NewGuid();
            Console.WriteLine(guid);
            idDocumento = guid.ToString().Substring(0, 8);

            string fechaFac = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            try
            {
                anticipo = double.Parse(txtAnticipo.Text);
                this.restante = totalFacturaActual - anticipo;                 
                connection.Open();
                montosinIva = (decimal)anticipo / 1.12m;
                montoIva = (decimal)montosinIva * 0.12m;

                string pendiente = "insert into pendientes(idFactura, anticipo , total) values (" + facturaActual + "," + anticipo + "," + totalFacturaActual + ")";
                MySqlCommand cmdPendiente = new MySqlCommand(pendiente, connection);
                cmdPendiente.ExecuteNonQuery();
                connection.Close();

                Datos_generales = request.Datos_generales("GTQ", fechaFac, "FACT", "", "");
                Datos_emisor = request.Datos_emisor("GEN", 1, "01001", "bordados.francy@gmail.com", "GT", "GUATEMALA", "GUATEMALA", "30 Av. 1-13 , zona 7 Jardines de Utatlan 1", "5347319", "CARLOS BAUTISTA", "BORDADOS FRANCY");
                Datos_receptor = request.Datos_receptor(nit, nombre, "01001", correo, "GT", "GUATEMALA", "GUATEMALA", direccion, "");
                Frases = request.Frases(1, 2);
                Frases = request.Frases(2, 1);

                Item_un_impuesto = request.Item_un_impuesto("B", "UND", "1" /*CANTIDAD*/,  "POR COMPRA DE UNIFORMES", 1, anticipo.ToString("N10")/*PRECIO X UNIDAD*/,
                    anticipo.ToString("N10")/*TOTAL SIN DESC*/, "0"/*DESCUENTO*/, anticipo.ToString("N10")/*TOTAL*/, "IVA", 1, "",
                    montosinIva.ToString("N10")/*MONTOGRAVABLE*/, montoIva.ToString("N10")/*MONTOIMPUESTO*/);

                Total_impuestos = request.total_impuestos("IVA", montoIva.ToString("N10"));
                Totales = request.Totales(anticipo.ToString("N10"));
                Adenda = request.Adendas("saldo_pendiente", restante.ToString());//Información Adicional
                                                                                  //Adenda = request.Adendas("Observaciones", "ESTA ES UNA PRUEBA");

                Agregar_adenda = request.Agregar_adendas();
                response = request.enviar_peticion_fel("BORDADOS_FRANCY"/*USUARIO*/, "F43E482EC149C5B7C475152FA265B43F"/*LLAVE*/, idDocumento/*IDENTIFICADOR DEL DOCUMENTO*/, "jcbautista95@gmail.com,jcbautista95@gmail.com"/*correo copia*/, "BORDADOS_FRANCY"/*USUARIO*/, "ed308938934731b734e9836bdb96ce57"/*llave emisor*/, true);
                Console.WriteLine(response);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message.ToString());
                throw;
            }
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
