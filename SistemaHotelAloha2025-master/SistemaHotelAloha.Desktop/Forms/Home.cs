using System.Windows.Forms;
using SistemaHotelAloha.Desktop.Forms;

namespace SistemaHotelAloha.Desktop.Forms
{
    public partial class Home : Form
    {
        public Home() { InitializeComponent(); }

        private void btnUsuarios_Click(object sender, EventArgs e)
        {
            using var f = new GestionUsuarios();
            f.ShowDialog(this);
        }

        private void btnHabitaciones_Click(object sender, EventArgs e)
        {
            using var f = new GestionHabitaciones();
            f.ShowDialog(this);
        }

        private void btnReservas_Click(object sender, EventArgs e)
        {
            using var f = new GestionReservas();
            f.ShowDialog(this);
        }

        private void btnServicios_Click(object sender, EventArgs e)
        {
            using var f = new GestionServiciosAdicionales();
            f.ShowDialog(this);
        }
    }
}
