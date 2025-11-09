using MySql.Data.MySqlClient;
using SistemaHotelAloha.Desktop.Data;
using SistemaHotelAloha.Desktop.Helpers;
using SistemaHotelAloha.Desktop.Models;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace SistemaHotelAloha.Desktop.Forms
{
    public partial class GestionServiciosAdicionales : Form
    {
        private DataTable _dt = new();

        public GestionServiciosAdicionales()
        {
            InitializeComponent();
        }

        private void GestionServiciosAdicionales_Load(object sender, EventArgs e)
        {
            Cargar();
            dgv.ClearSelection();
            Habilitar(false);
        }

        // ===== Helpers: foco siempre en columna visible =====
        private int FirstVisibleColumnIndex()
        {
            foreach (DataGridViewColumn c in dgv.Columns)
                if (c.Visible) return c.Index;
            return -1;
        }

        private void SeleccionarUltimoVisible()
        {
            if (dgv.Rows.Count == 0) return;
            int last = dgv.Rows.Count - 1;
            int vis = FirstVisibleColumnIndex();
            if (vis >= 0)
            {
                dgv.ClearSelection();
                dgv.Rows[last].Selected = true;
                dgv.CurrentCell = dgv.Rows[last].Cells[vis];
            }
        }

        private void SeleccionarPorId(int id)
        {
            int vis = FirstVisibleColumnIndex();
            if (vis < 0) return;

            foreach (DataGridViewRow r in dgv.Rows)
            {
                if (r.DataBoundItem is DataRowView drv && Convert.ToInt32(drv.Row["id"]) == id)
                {
                    dgv.ClearSelection();
                    r.Selected = true;
                    dgv.CurrentCell = r.Cells[vis];
                    break;
                }
            }
        }
        // =====================================================

        private void Cargar()
        {
            try
            {
                using var cn = Db.Conn(); cn.Open();
                using var cmd = new MySqlCommand("sp_servicios_listar", cn) { CommandType = CommandType.StoredProcedure };
                using var da = new MySqlDataAdapter(cmd);
                _dt = new DataTable();
                da.Fill(_dt);
                dgv.DataSource = _dt;

                // columnas esperadas: id, nombre, precio, activo
                if (dgv.Columns.Contains("id"))
                    dgv.Columns["id"].Visible = false;
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
            btnCrear.Enabled = true;
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            using var dlg = new ServicioAdicionalEditForm();
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                using var cn = Db.Conn(); cn.Open();
                using var cmd = new MySqlCommand("sp_servicio_crear", cn)
                { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.AddWithValue("@p_nombre", dlg.NombreSeleccionado);
                cmd.Parameters.AddWithValue("@p_precio", dlg.Precio);
                cmd.Parameters.AddWithValue("@p_activo", dlg.Activo);
                cmd.ExecuteNonQuery();

                Cargar();
                SeleccionarUltimoVisible();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Crear servicio", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            var row = ((DataRowView)dgv.CurrentRow.DataBoundItem).Row;

            int id = Convert.ToInt32(row["id"]);
            string nombre = row["nombre"]?.ToString() ?? "";
            decimal precio = row["precio"] == DBNull.Value ? 0m : Convert.ToDecimal(row["precio"]);
            bool activo = row.Table.Columns.Contains("activo") &&
                          (row["activo"].ToString() == "1" || row["activo"].ToString()?.ToLower() == "true");

            using var dlg = new ServicioAdicionalEditForm(id, nombre, precio, activo);
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                using var cn = Db.Conn(); cn.Open();
                using var cmd = new MySqlCommand("sp_servicio_actualizar", cn)
                { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.AddWithValue("@p_id", id);
                cmd.Parameters.AddWithValue("@p_nombre", dlg.NombreSeleccionado);
                cmd.Parameters.AddWithValue("@p_precio", dlg.Precio);
                cmd.Parameters.AddWithValue("@p_activo", dlg.Activo);
                cmd.ExecuteNonQuery();

                Cargar();
                SeleccionarPorId(id);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Editar servicio", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            var row = ((DataRowView)dgv.CurrentRow.DataBoundItem).Row;
            int id = Convert.ToInt32(row["id"]);

            if (MessageBox.Show($"¿Eliminar servicio #{id}?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                using var cn = Db.Conn(); cn.Open();
                using var cmd = new MySqlCommand("sp_servicio_eliminar", cn)
                { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@p_id", id);
                cmd.ExecuteNonQuery();

                Cargar();
                SeleccionarUltimoVisible();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Eliminar servicio", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}