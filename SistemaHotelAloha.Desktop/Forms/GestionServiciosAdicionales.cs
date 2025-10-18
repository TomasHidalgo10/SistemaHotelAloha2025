using System.ComponentModel;
using System.Windows.Forms;
using SistemaHotelAloha.Desktop.Data;
using SistemaHotelAloha.Desktop.Models;

namespace SistemaHotelAloha.Desktop.Forms
{
    public partial class GestionServiciosAdicionales : Form
    {
        private readonly ServicioAdicionalRepository _repo = new();
        private BindingList<ServicioAdicional> _binding = null!;

        public GestionServiciosAdicionales()
        {
            InitializeComponent();
        }

        private void GestionServiciosAdicionales_Load(object sender, EventArgs e)
        {
            
            _repo.Create(new ServicioAdicional { Nombre = "Spa", Precio = 15000 });
            _repo.Create(new ServicioAdicional { Nombre = "Traslado", Precio = 8000 });

            _binding = _repo.GetAll();
            dgvServicios.AutoGenerateColumns = false;
            dgvServicios.DataSource = _binding;
        }

        private ServicioAdicional? GetFromInputs(bool includeId = false)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtPrecio.Text))
            {
                MessageBox.Show("Completá Nombre y Precio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
            if (!decimal.TryParse(txtPrecio.Text, out var precio))
            {
                MessageBox.Show("Precio inválido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
            var s = new ServicioAdicional
            {
                Nombre = txtNombre.Text.Trim(),
                Precio = precio
            };
            if (includeId && int.TryParse(txtId.Text, out var id))
                s.Id = id;
            return s;
        }

        private void dgvServicios_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvServicios.CurrentRow?.DataBoundItem is ServicioAdicional s)
            {
                txtId.Text = s.Id.ToString();
                txtNombre.Text = s.Nombre;
                txtPrecio.Text = s.Precio.ToString("0.##");
            }
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            var s = GetFromInputs();
            if (s is null) return;
            _repo.Create(s);
            dgvServicios.Refresh();
            Limpiar();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out var id))
            {
                MessageBox.Show("Seleccioná un ítem de la lista.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var s = GetFromInputs(includeId: true);
            if (s is null) return;
            s.Id = id;
            _repo.Update(s);
            dgvServicios.Refresh();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out var id))
            {
                MessageBox.Show("Seleccioná un ítem de la lista.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show("¿Eliminar el servicio seleccionado?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _repo.Delete(id);
                dgvServicios.Refresh();
                Limpiar();
            }
        }

        private void Limpiar()
        {
            txtId.Clear();
            txtNombre.Clear();
            txtPrecio.Clear();
            txtNombre.Focus();
        }
    }
}
