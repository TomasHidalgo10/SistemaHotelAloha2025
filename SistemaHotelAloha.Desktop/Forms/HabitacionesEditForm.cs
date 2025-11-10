using MySql.Data.MySqlClient;
using SistemaHotelAloha.Desktop.Helpers;
using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;

namespace SistemaHotelAloha.Desktop.Forms
{
    public partial class HabitacionEditForm : Form
    {
        private readonly bool _esEdicion;
        private readonly int _idEditar;

        // Alta (nueva habitación)
        public HabitacionEditForm()
        {
            InitializeComponent();
            _esEdicion = false;
            _idEditar = 0;
            Text = "Nueva habitación";
            this.Load += HabitacionEditForm_Load;
        }

        // Edición
        public HabitacionEditForm(int id, string numero, int tipoId, int capacidad, decimal precio)
        {
            InitializeComponent();
            _esEdicion = true;
            _idEditar = id;
            Text = "Editar habitación";

            txtNumero.Text = numero;
            numCapacidad.Value = capacidad;
            txtPrecio.Text = precio.ToString("0.##", CultureInfo.InvariantCulture);

            this.Load += (s, e) =>
            {
                CargarTipos();
                if (cbTipo.DataSource != null)
                    cbTipo.SelectedValue = tipoId;
                // usar el flag realmente -> evita CS0414
                txtNumero.ReadOnly = _esEdicion; // por ejemplo, en edición no dejamos cambiar el número
            };
        }

        private void HabitacionEditForm_Load(object? sender, EventArgs e)
        {
            // En alta también cargamos los tipos
            if (!_esEdicion) CargarTipos();
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

        // Propiedades para leer valores ingresados
        public string Numero => txtNumero.Text.Trim();
        public int TipoId => Convert.ToInt32(cbTipo.SelectedValue ?? 0);
        public int Capacidad => Convert.ToInt32(numCapacidad.Value);

        public decimal PrecioNoche
        {
            get
            {
                var raw = (txtPrecio.Text ?? "").Trim().Replace(",", ".");
                return decimal.TryParse(
                    raw,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out var d
                ) ? d : 0m;
            }
        }

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
