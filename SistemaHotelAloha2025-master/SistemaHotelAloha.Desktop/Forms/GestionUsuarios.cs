using MySql.Data.MySqlClient;
using SistemaHotelAloha.Desktop.Data;
using SistemaHotelAloha.Desktop.Helpers;
using SistemaHotelAloha.Desktop.Models;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace SistemaHotelAloha.Desktop.Forms
{
    public partial class GestionUsuarios : Form
    {
        private DataTable _dt = new();

        public GestionUsuarios()
        {
            InitializeComponent();
        }


        private void GestionUsuarios_Load(object sender, EventArgs e)
        {
            Cargar();
            dgv.ClearSelection();
            Habilitar(false);
        }

        private void Cargar()
        {
            try
            {
                using var cn = Db.Conn(); cn.Open();
                using var cmd = new MySqlCommand("sp_usuarios_listar", cn) { CommandType = CommandType.StoredProcedure };
                using var da = new MySqlDataAdapter(cmd);
                _dt = new DataTable();
                da.Fill(_dt);
                dgv.DataSource = _dt;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"MySQL: {ex.Message}", "BD", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgv_SelectionChanged(object? sender, EventArgs e) => Habilitar(dgv.CurrentRow != null);

        private void Habilitar(bool on)
        {
            btnEditar.Enabled = on;
            btnEliminar.Enabled = on;
        }

        private void SeleccionarUltimo()
        {
            if (dgv.Rows.Count == 0) return;
            int i = dgv.Rows.Count - 1;
            dgv.Rows[i].Selected = true;
            dgv.CurrentCell = dgv.Rows[i].Cells[0];
        }

        private void SeleccionarPorId(int id)
        {
            foreach (DataGridViewRow r in dgv.Rows)
            {
                if (r.DataBoundItem is DataRowView drv && Convert.ToInt32(drv.Row["id"]) == id)
                { r.Selected = true; dgv.CurrentCell = r.Cells[0]; break; }
            }
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            using var dlg = new UsuarioEditForm();
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                using var cn = Db.Conn(); cn.Open();
                using var cmd = new MySqlCommand("sp_usuario_crear", cn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@p_nombre", dlg.Nombre);
                cmd.Parameters.AddWithValue("@p_email", dlg.Email);
                cmd.Parameters.AddWithValue("@p_activo", dlg.Activo);
                cmd.Parameters.AddWithValue("@p_role_id", dlg.RolId);
                cmd.ExecuteNonQuery();
                Cargar(); // refresca
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Crear usuario", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            var row = ((DataRowView)dgv.CurrentRow.DataBoundItem).Row;
            int id = Convert.ToInt32(row["id"]);

            using var dlg = new UsuarioEditForm(
                id,
                row["nombre"].ToString() ?? "",
                row["email"].ToString() ?? "",
                row.Table.Columns.Contains("activo") && (row["activo"].ToString() == "1" || row["activo"].ToString()?.ToLower() == "true")
            );
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                using var cn = Db.Conn(); cn.Open();
                using var cmd = new MySqlCommand("sp_usuario_actualizar", cn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@p_id", id);
                cmd.Parameters.AddWithValue("@p_nombre", dlg.Nombre);
                cmd.Parameters.AddWithValue("@p_email", dlg.Email);
                cmd.Parameters.AddWithValue("@p_activo", dlg.Activo);
                cmd.Parameters.AddWithValue("@p_role_id", dlg.RolId);
                cmd.ExecuteNonQuery();
                Cargar();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Editar usuario", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            var row = ((DataRowView)dgv.CurrentRow.DataBoundItem).Row;
            int id = Convert.ToInt32(row["id"]);

            if (MessageBox.Show($"¿Eliminar usuario #{id}?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                using var cn = Db.Conn(); cn.Open();
                using var cmd = new MySqlCommand("sp_usuario_eliminar", cn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@p_id", id);
                cmd.ExecuteNonQuery();
                Cargar();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Eliminar usuario", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
    }
}