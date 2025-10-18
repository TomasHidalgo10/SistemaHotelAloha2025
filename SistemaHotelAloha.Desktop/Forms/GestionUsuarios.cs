using System.ComponentModel;
using System.Windows.Forms;
using SistemaHotelAloha.Desktop.Data;
using SistemaHotelAloha.Desktop.Models;

namespace SistemaHotelAloha.Desktop.Forms
{
    public partial class GestionUsuarios : Form
    {
        private readonly UsuarioRepository _repo = new();
        private BindingList<Usuario> _binding = null!;

        public GestionUsuarios()
        {
            InitializeComponent();
        }

        private void GestionUsuarios_Load(object sender, EventArgs e)
        {
            
            _repo.Create(new Usuario { Nombre = "Admin", Email = "admin@aloha.com", Rol = "Administrador" });
            _repo.Create(new Usuario { Nombre = "Recep", Email = "recepcion@aloha.com", Rol = "Recepción" });

            _binding = _repo.GetAll();
            dgvUsuarios.AutoGenerateColumns = false;
            dgvUsuarios.DataSource = _binding;
        }

        private Usuario? GetUsuarioFromInputs(bool includeId = false)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtRol.Text))
            {
                MessageBox.Show("Completá Nombre, Email y Rol.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            var u = new Usuario
            {
                Nombre = txtNombre.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Rol = txtRol.Text.Trim()
            };

            if (includeId && int.TryParse(txtId.Text, out var id))
            {
                u.Id = id;
            }

            return u;
        }

        private void dgvUsuarios_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvUsuarios.CurrentRow?.DataBoundItem is Usuario u)
            {
                txtId.Text = u.Id.ToString();
                txtNombre.Text = u.Nombre;
                txtEmail.Text = u.Email;
                txtRol.Text = u.Rol;
            }
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            var u = GetUsuarioFromInputs();
            if (u is null) return;
            _repo.Create(u);
            dgvUsuarios.Refresh();
            LimpiarInputs();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out var id))
            {
                MessageBox.Show("Seleccioná un usuario de la lista.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var u = GetUsuarioFromInputs(includeId: true);
            if (u is null) return;
            u.Id = id;
            _repo.Update(u);
            dgvUsuarios.Refresh();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out var id))
            {
                MessageBox.Show("Seleccioná un usuario de la lista.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var r = MessageBox.Show("¿Seguro que querés eliminar el usuario seleccionado?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (r == DialogResult.Yes)
            {
                _repo.Delete(id);
                dgvUsuarios.Refresh();
                LimpiarInputs();
            }
        }

        private void LimpiarInputs()
        {
            txtId.Clear();
            txtNombre.Clear();
            txtEmail.Clear();
            txtRol.Clear();
            txtNombre.Focus();
        }
    }
}
