namespace SistemaHotelAloha.Desktop
{
    internal static class Program
    {
       
        [STAThread]
        static void Main()
        {
             https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Forms.Home());
        }
    }
}