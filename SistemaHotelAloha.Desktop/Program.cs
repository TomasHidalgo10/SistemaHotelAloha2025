using SistemaHotelAloha.AccesoDatos.Infra;

namespace SistemaHotelAloha.Desktop
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // 1) Asegurar BD + Tablas antes de abrir el Home
            try
            {
                DatabaseBootstrapper.EnsureCreated();
            }
            catch (Exception ex)
            {
                // Si algo falla, lo mostramos y detenemos para que sepas qué pasó
                MessageBox.Show($"Error inicializando la base de datos:\n\n{ex.Message}",
                    "ALOHA_DB", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 2) Iniciar UI
            ApplicationConfiguration.Initialize();
            Application.Run(new Forms.Home());
        }
    }
}