using System.Windows.Forms;
using System.Drawing;
namespace SistemaHotelAloha.Desktop.Forms
{
    partial class Home
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnGestionUsuarios = new Button();
            btnGestionServicios = new Button();
            btnGestionHabitaciones = new Button();
            SuspendLayout();
            // 
            // btnGestionUsuarios
            // 
            btnGestionUsuarios.Location = new Point(30, 30);
            btnGestionUsuarios.Name = "btnGestionUsuarios";
            btnGestionUsuarios.Size = new Size(240, 50);
            btnGestionUsuarios.TabIndex = 0;
            btnGestionUsuarios.Text = "Gestión de Usuarios";
            btnGestionUsuarios.UseVisualStyleBackColor = true;
            btnGestionUsuarios.Click += btnGestionUsuarios_Click;
            // 
            // btnGestionServicios
            // 
            btnGestionServicios.Location = new Point(30, 100);
            btnGestionServicios.Name = "btnGestionServicios";
            btnGestionServicios.Size = new Size(240, 50);
            btnGestionServicios.TabIndex = 1;
            btnGestionServicios.Text = "Gestión de Servicios Adicionales";
            btnGestionServicios.UseVisualStyleBackColor = true;
            btnGestionServicios.Click += btnGestionServicios_Click;
            // 
            // btnGestionHabitaciones
            // 
            btnGestionHabitaciones.Location = new Point(30, 170);
            btnGestionHabitaciones.Name = "btnGestionHabitaciones";
            btnGestionHabitaciones.Size = new Size(240, 50);
            btnGestionHabitaciones.TabIndex = 2;
            btnGestionHabitaciones.Text = "Gestión de Habitaciones";
            btnGestionHabitaciones.UseVisualStyleBackColor = true;
            btnGestionHabitaciones.Click += btnGestionHabitaciones_Click;
            // 
            // Home
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(304, 261);
            Controls.Add(btnGestionHabitaciones);
            Controls.Add(btnGestionServicios);
            Controls.Add(btnGestionUsuarios);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "Home";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Home - Hotel ALOHA";
            ResumeLayout(false);
        }

        #endregion

        private Button btnGestionUsuarios;
        private Button btnGestionServicios;
        private Button btnGestionHabitaciones;
    }
}
