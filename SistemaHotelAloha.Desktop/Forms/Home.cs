using System.Windows.Forms;

namespace SistemaHotelAloha.Desktop.Forms
{
    public partial class Home : Form
    {
        public Home()
        {
            InitializeComponent();
        }

        private void btnGestionUsuarios_Click(object sender, EventArgs e)
        {
            using var f = new GestionUsuarios();
            f.ShowDialog(this);
        }

        private void btnGestionServicios_Click(object sender, EventArgs e)
        {
            using var f = new GestionServiciosAdicionales();
            f.ShowDialog(this);
        }

        private void btnGestionHabitaciones_Click(object sender, EventArgs e)
        {
            using var f = new GestionHabitaciones();
            f.ShowDialog(this);
        }
    }
}
