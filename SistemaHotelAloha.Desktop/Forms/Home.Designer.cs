using System.Windows.Forms;
using System.Drawing;
namespace SistemaHotelAloha.Desktop.Forms
{
    partial class Home
    {
        private System.ComponentModel.IContainer components = null;
        private Button btnUsuarios;
        private Button btnHabitaciones;
        private Button btnReservas;
        private Button btnServicios;

        protected override void Dispose(bool disposing)
        { if (disposing && (components != null)) components.Dispose(); base.Dispose(disposing); }

        private void InitializeComponent()
        {
            this.btnUsuarios = new System.Windows.Forms.Button();
            this.btnHabitaciones = new System.Windows.Forms.Button();
            this.btnReservas = new System.Windows.Forms.Button();
            this.btnServicios = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnUsuarios
            // 
            this.btnUsuarios.Location = new System.Drawing.Point(24, 22);
            this.btnUsuarios.Name = "btnUsuarios";
            this.btnUsuarios.Size = new System.Drawing.Size(220, 42);
            this.btnUsuarios.Text = "Gestión de Usuarios";
            this.btnUsuarios.UseVisualStyleBackColor = true;
            this.btnUsuarios.Click += new System.EventHandler(this.btnUsuarios_Click);
            // 
            // btnHabitaciones
            // 
            this.btnHabitaciones.Location = new System.Drawing.Point(24, 78);
            this.btnHabitaciones.Name = "btnHabitaciones";
            this.btnHabitaciones.Size = new System.Drawing.Size(220, 42);
            this.btnHabitaciones.Text = "Gestión de Habitaciones";
            this.btnHabitaciones.UseVisualStyleBackColor = true;
            this.btnHabitaciones.Click += new System.EventHandler(this.btnHabitaciones_Click);
            // 
            // btnReservas
            // 
            this.btnReservas.Location = new System.Drawing.Point(24, 134);
            this.btnReservas.Name = "btnReservas";
            this.btnReservas.Size = new System.Drawing.Size(220, 42);
            this.btnReservas.Text = "Gestión de Reservas";
            this.btnReservas.UseVisualStyleBackColor = true;
            this.btnReservas.Click += new System.EventHandler(this.btnReservas_Click);
            // 
            // btnServicios
            // 
            this.btnServicios.Location = new System.Drawing.Point(24, 190);
            this.btnServicios.Name = "btnServicios";
            this.btnServicios.Size = new System.Drawing.Size(220, 42);
            this.btnServicios.Text = "Servicios Adicionales";
            this.btnServicios.UseVisualStyleBackColor = true;
            this.btnServicios.Click += new System.EventHandler(this.btnServicios_Click);
            // 
            // Home
            // 
            this.ClientSize = new System.Drawing.Size(270, 248);
            this.Controls.Add(this.btnServicios);
            this.Controls.Add(this.btnReservas);
            this.Controls.Add(this.btnHabitaciones);
            this.Controls.Add(this.btnUsuarios);
            this.Name = "Home";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Aloha Desktop";
            this.ResumeLayout(false);
        }
    }
}