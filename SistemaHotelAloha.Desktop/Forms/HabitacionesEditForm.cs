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
    public partial class HabitacionEditForm : Form
    {
        private readonly bool _esEdicion;
        private readonly int _idEditar;

        public HabitacionEditForm()
        {
            InitializeComponent();
            _esEdicion = false;
        }

        public HabitacionEditForm(int id, string numero, int tipoId, int capacidad, decimal precio)
        {
            InitializeComponent();
            _esEdicion = true;
            _idEditar = id;

            txtNumero.Text = numero;
            numCapacidad.Value = capacidad;
            txtPrecio.Text = precio.ToString("0.##");
            // El Tipo se selecciona en Load cuando ya está cargado el combo.
            this.Load += (s, e) =>
            {
                if (cbTipo.DataSource != null)
                    cbTipo.SelectedValue = tipoId;
            };
        }

        private void HabitacionEditForm_Load(object sender, EventArgs e)
        {
            CargarTipos();
        }

        private void CargarTipos()
        {
            try
            {
                using var cn = Db.Conn(); cn.Open();
                using var cmd = new MySqlCommand("sp_tipos_habitacion_listar", cn)
                { CommandType = CommandType.StoredProcedure };
                using var da = new MySqlDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);

                cbTipo.DisplayMember = "Nombre";
                cbTipo.ValueMember = "Id";
                cbTipo.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar tipos: {ex.Message}", "BD",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string Numero => txtNumero.Text.Trim();
        public int TipoId => Convert.ToInt32(cbTipo.SelectedValue ?? 0);
        public int Capacidad => Convert.ToInt32(numCapacidad.Value);
        public decimal PrecioNoche => decimal.TryParse(txtPrecio.Text.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var d) ? d : 0m;

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Numero) || TipoId <= 0 || Capacidad <= 0 || PrecioNoche <= 0)
            {
                MessageBox.Show("Complete todos los campos.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult = DialogResult.OK;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
            => DialogResult = DialogResult.Cancel;
    }
}