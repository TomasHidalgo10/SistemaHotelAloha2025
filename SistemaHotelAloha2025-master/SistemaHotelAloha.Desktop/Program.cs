using SistemaHotelAloha.AccesoDatos.Infra;
using SistemaHotelAloha.Desktop.Forms;
using SistemaHotelAloha.Desktop.Utils;

namespace SistemaHotelAloha.Desktop
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.ThreadException += (s, e) =>
                MessageBox.Show(e.Exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            ApplicationConfiguration.Initialize();
            Application.Run(new Home());
        }
    }
}