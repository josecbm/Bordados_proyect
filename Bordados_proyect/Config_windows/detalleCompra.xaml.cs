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
using SpreadsheetLight;

namespace Bordados_proyect.Config_windows
{
    /// <summary>
    /// Interaction logic for detalleCompra.xaml
    /// </summary>
    public partial class detalleCompra : Window
    {
        int facturaActual;
        string connectionString = "SERVER=localhost;DATABASE=bordados_db;UID=root;PASSWORD=1234;";
        MySqlConnection connection;
        DataTable dt2;
        public detalleCompra(int factura)
        {
            InitializeComponent();
            connection = new MySqlConnection(connectionString);
            facturaActual = factura;
            showfacturaActual();
        }

        public void showfacturaActual()
        {
            connection.Open();

            MySqlCommand cmd2 = new MySqlCommand("select d.cantidad, p.nombrePrenda, p.id , d.precioTotal , f.fecha  from factura f " +
            "inner join detalle d on f.id = d.idFactura " +
            "inner join prenda p on d.idPrenda = p.id " +
            "where f.id =" + facturaActual + " ;", connection);

            dt2 = new DataTable();
            dt2.Load(cmd2.ExecuteReader());

            dt_reporteCompra.DataContext = dt2;
            connection.Close();
        }
        public void getVentas_dia()
        {
            connection.Open();
            try
            {
                MySqlCommand cmd2 = new MySqlCommand("select * from usr;", connection);
            }
            catch (Exception ex)
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
        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnReporteExcel_Click(object sender, RoutedEventArgs e)
        {
            string datetimeActual = DateTime.Now.ToString("yyyy-MM-ddThh_mm_ss");
            string path;
            path ="reporte_compra_" + datetimeActual + ".xlsx";
            SLDocument documentoExcel = new SLDocument();
        
            documentoExcel.ImportDataTable(1, 1, dt2, true);
            documentoExcel.SaveAs(path);
        }
    }
}
