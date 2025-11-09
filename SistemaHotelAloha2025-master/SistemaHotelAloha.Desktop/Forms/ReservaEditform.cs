using MySql.Data.MySqlClient;
using SistemaHotelAloha.Desktop.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaHotelAloha.Desktop.Forms
{
    public partial class ReservaEditForm : Form
    {
        public int Id { get; }
        public int IdHabitacion => (int)cbHabitacion.SelectedValue;
        public string Huesped => txtHuesped.Text.Trim();
        public DateTime FechaDesde => dtpDesde.Value.Date;
        public DateTime FechaHasta => dtpHasta.Value.Date;

        public ReservaEditForm(int id, int idHabitacion, string huesped, DateTime desde, DateTime hasta)
        {
            InitializeComponent();
            Id = id;
            CargarHabitaciones();
            cbHabitacion.SelectedValue = idHabitacion;
            txtHuesped.Text = huesped;
            dtpDesde.Value = desde;
            dtpHasta.Value = hasta;
        }

        private void CargarHabitaciones()
        {
            using var cn = Db.Conn();
            using var cmd = new MySqlCommand("sp_habitaciones_listar", cn) { CommandType = CommandType.StoredProcedure };
            using var da = new MySqlDataAdapter(cmd);
            var dt = new DataTable(); da.Fill(dt);
            cbHabitacion.DisplayMember = "numero"; cbHabitacion.ValueMember = "id"; cbHabitacion.DataSource = dt;
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Huesped)) { MessageBox.Show("Huésped obligatorio."); txtHuesped.Focus(); return; }
            if (FechaHasta <= FechaDesde) { MessageBox.Show("‘Hasta’ debe ser posterior a ‘Desde’."); dtpHasta.Focus(); return; }
            DialogResult = DialogResult.OK; Close();
        }
        private void btnCancelar_Click(object sender, EventArgs e) { DialogResult = DialogResult.Cancel; Close(); }
    }
}