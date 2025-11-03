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
    public partial class ServicioAdicionalEditForm : Form
    {
        private DataTable _catalogo = new();

        public ServicioAdicionalEditForm()
        {
            InitializeComponent();
        }

        public ServicioAdicionalEditForm(int id, string nombre, decimal precio, bool activo) : this()
        {
            // Si venís en modo edición, seteo después de cargar catálogo:
            this.Load += (s, e) =>
            {
                // Buscar nombre en catálogo
                int idx = -1;
                for (int i = 0; i < cbServicio.Items.Count; i++)
                {
                    if (cbServicio.Items[i] is DataRowView drv &&
                        string.Equals(drv["Nombre"]?.ToString(), nombre, StringComparison.OrdinalIgnoreCase))
                    { idx = i; break; }
                }
                if (idx >= 0) cbServicio.SelectedIndex = idx;
                else
                {
                    // Si no existe en catálogo (histórico), agrego temporal
                    var r = _catalogo.NewRow();
                    r["Id"] = -1;
                    r["Nombre"] = nombre;
                    r["PrecioSugerido"] = precio;
                    _catalogo.Rows.InsertAt(r, 0);
                    cbServicio.SelectedIndex = 0;
                }

                nudPrecio.Value = precio;
                chkActivo.Checked = activo;
            };
        }

        private void ServicioAdicionalEditForm_Load(object sender, EventArgs e)
        {
            CargarCatalogo();
        }

        private void CargarCatalogo()
        {
            using var cn = Db.Conn(); cn.Open();
            using var cmd = new MySqlCommand("sp_servicios_catalogo_listar", cn)
            { CommandType = CommandType.StoredProcedure };
            using var da = new MySqlDataAdapter(cmd);
            _catalogo = new DataTable();
            da.Fill(_catalogo);

            cbServicio.DisplayMember = "Nombre";
            cbServicio.ValueMember = "Id";
            cbServicio.DataSource = _catalogo;

            cbServicio.SelectedIndexChanged += (_, __) =>
            {
                if (cbServicio.SelectedItem is DataRowView drv &&
                    decimal.TryParse(drv["PrecioSugerido"]?.ToString(), out var p))
                {
                    if (p < nudPrecio.Minimum) p = nudPrecio.Minimum;
                    if (p > nudPrecio.Maximum) p = nudPrecio.Maximum;
                    nudPrecio.Value = p;     // autocompleta precio
                }
            };

            if (cbServicio.Items.Count > 0)
                cbServicio.SelectedIndex = 0;
        }

        public string NombreSeleccionado
            => (cbServicio.SelectedItem as DataRowView)?["Nombre"]?.ToString() ?? string.Empty;

        public decimal Precio => nudPrecio.Value;

        public bool Activo => chkActivo.Checked;

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NombreSeleccionado))
            {
                MessageBox.Show("Seleccione un servicio.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (Precio <= 0)
            {
                MessageBox.Show("El precio debe ser mayor que cero.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult = DialogResult.OK;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}