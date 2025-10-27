using System;
using System.Linq;
using System.Windows.Forms;
using SistemaHotelAloha.Desktop.Data;
using SistemaHotelAloha.Desktop.Models;

namespace SistemaHotelAloha.Desktop.Forms   // ⚠️ Si tu Designer usa otro namespace, copiá ese aquí.
{
    // ⚠️ Debe ser 'partial' y coincidir exactamente con la clase del Designer:
    // mira la primera línea de GestionUsuarios.Designer.cs (namespace y nombre de clase).
    public partial class GestionUsuarios : Form
    {
        private readonly UsuarioRepository _repo = new UsuarioRepository();

        // =========================
        //  ALIAS PARA CONTROLES
        // =========================

        // DataGridView de usuarios
        private DataGridView? GridUsuarios =>
            this.Controls.Find("gridUsuarios", true).OfType<DataGridView>().FirstOrDefault()
         ?? this.Controls.Find("dgvUsuarios", true).OfType<DataGridView>().FirstOrDefault()
         ?? this.Controls.Find("dataGridView1", true).OfType<DataGridView>().FirstOrDefault()
         ?? this.Controls.OfType<DataGridView>().FirstOrDefault();

        // ComboBox de roles
        private ComboBox? CmbRol =>
            this.Controls.Find("cmbRol", true).OfType<ComboBox>().FirstOrDefault()
         ?? this.Controls.Find("cbRol", true).OfType<ComboBox>().FirstOrDefault()
         ?? this.Controls.Find("comboBox1", true).OfType<ComboBox>().FirstOrDefault()
         ?? this.Controls.OfType<ComboBox>().FirstOrDefault();

        // TextBox Nombre
        private TextBox? TxtNombre =>
            this.Controls.Find("txtNombre", true).OfType<TextBox>().FirstOrDefault()
         ?? this.Controls.Find("txtNombre2", true).OfType<TextBox>().FirstOrDefault()
         ?? this.Controls.Find("txtUserName", true).OfType<TextBox>().FirstOrDefault()
         ?? this.Controls.OfType<TextBox>().FirstOrDefault();

        // TextBox Email
        private TextBox? TxtEmail =>
            this.Controls.Find("txtEmail", true).OfType<TextBox>().FirstOrDefault()
         ?? this.Controls.Find("txtCorreo", true).OfType<TextBox>().FirstOrDefault()
         ?? this.Controls.Find("txtMail", true).OfType<TextBox>().FirstOrDefault()
         ?? this.Controls.OfType<TextBox>().Skip(1).FirstOrDefault(); // fallback: 2º textbox

        // TextBox Rol (si usas TextBox en lugar de ComboBox)
        private TextBox? TxtRol =>
            this.Controls.Find("txtRol", true).OfType<TextBox>().FirstOrDefault()
         ?? this.Controls.Find("txtRole", true).OfType<TextBox>().FirstOrDefault()
         ?? this.Controls.OfType<TextBox>().LastOrDefault();

        public GestionUsuarios()
        {
            InitializeComponent();    // ← ahora compila si NAMESPACE y CLASS coinciden con el Designer
            CargarUsuarios();
        }

        // ===============================
        //   MÉTODOS AUXILIARES
        // ===============================

        private void CargarUsuarios()
        {
            var lista = _repo.GetAll();
            if (GridUsuarios is null) return;
            GridUsuarios.DataSource = null;
            GridUsuarios.DataSource = lista;
        }

        private void LimpiarFormulario()
        {
            if (TxtNombre is not null) TxtNombre.Clear();
            if (TxtEmail is not null) TxtEmail.Clear();
            if (TxtRol is not null) TxtRol.Clear();
            GridUsuarios?.ClearSelection();
        }

        private Usuario? GetUsuarioFromInputs(bool includeId = false)
        {
            var nombre = (TxtNombre?.Text ?? "").Trim();
            var email = (TxtEmail?.Text ?? "").Trim();
            var rolSel = (CmbRol?.SelectedItem?.ToString() ?? "").Trim();
            var rolTxt = (TxtRol?.Text ?? "").Trim();
            var rol = string.IsNullOrWhiteSpace(rolSel) ? rolTxt : rolSel;

            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(rol))
            {
                MessageBox.Show("Completá nombre, email y rol.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            if (!email.Contains("@") || !email.Contains("."))
            {
                MessageBox.Show("Email inválido.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            var u = new Usuario
            {
                Nombre = nombre,
                Email = email,
                Rol = string.IsNullOrWhiteSpace(rol) ? "Usuario" : rol
            };

            if (includeId)
            {
                if (GridUsuarios?.CurrentRow?.DataBoundItem is Usuario sel)
                {
                    u.Id = sel.Id;
                }
                else
                {
                    MessageBox.Show("Seleccioná un usuario de la lista.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return null;
                }
            }

            return u;
        }

        // ===============================
        //   BOTONES CRUD
        // ===============================

        private void btnCrear_Click(object sender, EventArgs e)
        {
            var u = GetUsuarioFromInputs();
            if (u is null) return;

            try
            {
                _repo.Create(u);
                CargarUsuarios();
                LimpiarFormulario();
                MessageBox.Show("Usuario creado correctamente.",
                    "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Usuarios",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear usuario:\n\n" + ex.Message,
                    "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            var u = GetUsuarioFromInputs(includeId: true);
            if (u is null) return;

            try
            {
                _repo.Update(u);
                CargarUsuarios();
                MessageBox.Show("Usuario actualizado correctamente.",
                    "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Usuarios",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar usuario:\n\n" + ex.Message,
                    "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (GridUsuarios?.CurrentRow?.DataBoundItem is not Usuario sel)
            {
                MessageBox.Show("Seleccioná un usuario para eliminar.",
                    "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirmar = MessageBox.Show(
                $"¿Seguro que querés eliminar a {sel.Nombre}?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmar != DialogResult.Yes) return;

            try
            {
                _repo.Delete(sel.Id);
                CargarUsuarios();
                LimpiarFormulario();
                MessageBox.Show("Usuario eliminado correctamente.",
                    "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Usuarios",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar usuario:\n\n" + ex.Message,
                    "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gridUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            if (GridUsuarios?.CurrentRow?.DataBoundItem is Usuario u)
            {
                if (TxtNombre is not null) TxtNombre.Text = u.Nombre;
                if (TxtEmail is not null) TxtEmail.Text = u.Email;
                if (TxtRol is not null) TxtRol.Text = u.Rol;
            }
        }

        // ===============================
        //   HANDLERS REFERENCIADOS EN EL DESIGNER (puentes)
        // ===============================

        private void dgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            gridUsuarios_SelectionChanged(sender, e);
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            btnActualizar_Click(sender, e);
        }

        private void GestionUsuarios_Load(object sender, EventArgs e)
        {
            CargarUsuarios();
        }
    }
}