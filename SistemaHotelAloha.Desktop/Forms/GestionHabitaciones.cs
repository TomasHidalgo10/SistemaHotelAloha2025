using System.ComponentModel;
using System.Windows.Forms;
using SistemaHotelAloha.Desktop.Data;
using SistemaHotelAloha.Desktop.Models;

namespace SistemaHotelAloha.Desktop.Forms
{
    public partial class GestionHabitaciones : Form
    {
        private readonly HabitacionRepository _repo = new();
        private BindingList<Habitacion> _binding = null!;

        public GestionHabitaciones()
        {
            InitializeComponent();
        }

        private void GestionHabitaciones_Load(object sender, EventArgs e)
        {
            // Seed de ejemplo
            _repo.Create(new Habitacion { Numero = "101", Tipo = "Simple", Estado = "Disponible", PrecioNoche = 35000 });
            _repo.Create(new Habitacion { Numero = "202", Tipo = "Doble", Estado = "Limpieza", PrecioNoche = 55000 });

            _binding = _repo.GetAll();
            dgvHabitaciones.AutoGenerateColumns = false;
            dgvHabitaciones.DataSource = _binding;
        }

        private Habitacion? GetFromInputs(bool includeId = false)
        {
            if (string.IsNullOrWhiteSpace(txtNumero.Text) ||
                string.IsNullOrWhiteSpace(txtTipo.Text) ||
                string.IsNullOrWhiteSpace(txtEstado.Text) ||
                string.IsNullOrWhiteSpace(txtPrecioNoche.Text))
            {
                MessageBox.Show("Completá Número, Tipo, Estado y Precio por noche.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
            if (!decimal.TryParse(txtPrecioNoche.Text, out var precio))
            {
                MessageBox.Show("Precio por noche inválido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            var h = new Habitacion
            {
                Numero = txtNumero.Text.Trim(),
                Tipo = txtTipo.Text.Trim(),
                Estado = txtEstado.Text.Trim(),
                PrecioNoche = precio
            };
            if (includeId && int.TryParse(txtId.Text, out var id))
                h.Id = id;
            return h;
        }

        private void dgvHabitaciones_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvHabitaciones.CurrentRow?.DataBoundItem is Habitacion h)
            {
                txtId.Text = h.Id.ToString();
                txtNumero.Text = h.Numero;
                txtTipo.Text = h.Tipo;
                txtEstado.Text = h.Estado;
                txtPrecioNoche.Text = h.PrecioNoche.ToString("0.##");
            }
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            var h = GetFromInputs();
            if (h is null) return;
            _repo.Create(h);
            dgvHabitaciones.Refresh();
            Limpiar();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out var id))
            {
                MessageBox.Show("Seleccioná una habitación de la lista.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var h = GetFromInputs(includeId: true);
            if (h is null) return;
            h.Id = id;
            _repo.Update(h);
            dgvHabitaciones.Refresh();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out var id))
            {
                MessageBox.Show("Seleccioná una habitación de la lista.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show("¿Eliminar la habitación seleccionada?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _repo.Delete(id);
                dgvHabitaciones.Refresh();
                Limpiar();
            }
        }

        private void Limpiar()
        {
            txtId.Clear();
            txtNumero.Clear();
            txtTipo.Clear();
            txtEstado.Clear();
            txtPrecioNoche.Clear();
            txtNumero.Focus();
        }
    }
}
