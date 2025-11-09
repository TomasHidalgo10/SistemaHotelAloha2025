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
    public partial class UsuarioEditForm : Form
    {
        private int? _userIdForEdit;   // si edito, guardo el id para leer role_id
        private bool _rolesCargados;

        public UsuarioEditForm()
        {
            InitializeComponent();
        }

        // EDITAR: coincide con tu llamada (id, nombre, email, activo)
        public UsuarioEditForm(int id, string nombre, string email, bool activo)
        {
            InitializeComponent();
            _userIdForEdit = id;
            txtNombre.Text = nombre ?? string.Empty;
            txtEmail.Text = email ?? string.Empty;
            chkActivo.Checked = activo;
        }

        private void UsuarioEditForm_Load(object sender, EventArgs e)
        {
            CargarRoles();   // llena cbRol
            _rolesCargados = true;

            // Si estoy editando, selecciono el rol actual del usuario
            if (_userIdForEdit.HasValue)
                PrefijarRolDesdeBD(_userIdForEdit.Value);
        }

        private void CargarRoles()
        {
            try
            {
                using var cn = Db.Conn(); cn.Open();
                using var cmd = new MySqlCommand("sp_roles_listar", cn)
                { CommandType = CommandType.StoredProcedure };
                using var da = new MySqlDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);

                cbRol.DisplayMember = "nombre";
                cbRol.ValueMember = "id";
                cbRol.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar roles: {ex.Message}", "BD",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrefijarRolDesdeBD(int userId)
        {
            try
            {
                using var cn = Db.Conn(); cn.Open();
                using var cmd = new MySqlCommand("SELECT role_id FROM usuarios WHERE id=@id", cn);
                cmd.Parameters.AddWithValue("@id", userId);
                var val = cmd.ExecuteScalar();
                if (val != null && val != DBNull.Value && _rolesCargados)
                {
                    // seleccionar el role_id actual
                    cbRol.SelectedValue = Convert.ToInt32(val);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo obtener el rol del usuario #{userId}: {ex.Message}",
                    "BD", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Propiedades expuestas al form padre
        public string Nombre => txtNombre.Text.Trim();
        public string Email => txtEmail.Text.Trim();
        public bool Activo => chkActivo.Checked;
        public int RolId => Convert.ToInt32(cbRol.SelectedValue ?? 1);

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Email))
            {
                MessageBox.Show("Complete nombre y email.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cbRol.SelectedValue == null)
            {
                MessageBox.Show("Seleccione un rol.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult = DialogResult.OK;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
            => DialogResult = DialogResult.Cancel;
    }
}