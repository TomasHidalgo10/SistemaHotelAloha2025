using MySql.Data.MySqlClient;
using SistemaHotelAloha.AccesoDatos;
using SistemaHotelAloha.Desktop.Helpers;
using SistemaHotelAloha.Desktop.Models;
using SistemaHotelAloha.Desktop.Utils;
using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace SistemaHotelAloha.Desktop.Forms
{
    public partial class GestionHabitaciones : Form
    {
        private DataTable _dt = new();

        public GestionHabitaciones()
        {
            InitializeComponent();
        }

        private void GestionHabitaciones_Load(object sender, EventArgs e)
        {
            Cargar();
            dgv.ClearSelection();
            Habilitar(false);
            // NO establecer CurrentCell acá
        }

        // =========================
        // PASO 1: HELPERS DE FOCO
        // =========================

        // Índice de la primera columna visible del DGV
        private int FirstVisibleColumnIndex()
        {
            foreach (DataGridViewColumn c in dgv.Columns)
                if (c.Visible) return c.Index;
            return -1;
        }

        // Selecciona última fila y pone foco en una celda visible
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

        // Selecciona por ID y pone foco en una celda visible
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

        // =========================
        // Eventos básicos
        // =========================

        private void dgv_SelectionChanged(object? sender, EventArgs e)
            => Habilitar(dgv.CurrentRow != null);

        private void Habilitar(bool on)
        {
            btnCrear.Enabled = true;
            btnEditar.Enabled = on;
            btnEliminar.Enabled = on;
        }

        // =========================
        // PASO 2: Cargar grilla
        // =========================
        private void Cargar()
        {
            try
            {
                using var cn = Db.Conn(); cn.Open();
                using var cmd = new MySqlCommand("sp_habitaciones_listar", cn)
                { CommandType = CommandType.StoredProcedure };
                using var da = new MySqlDataAdapter(cmd);
                _dt = new DataTable();
                da.Fill(_dt);
                dgv.DataSource = _dt;

                // Columnas esperadas: id, numero, tipo_id, tipo, capacidad, precio_noche
                if (dgv.Columns.Contains("id"))
                    dgv.Columns["id"].Visible = false;

                // NO setear CurrentCell acá (evita foco en columna oculta)
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"MySQL: {ex.Message}", "BD",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================
        // PASO 3: Crear / Editar
        // =========================

        private void btnCrear_Click(object sender, EventArgs e)
        {
            using var dlg = new HabitacionEditForm();
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                using var cn = Db.Conn(); cn.Open();
                using var cmd = new MySqlCommand("sp_habitacion_crear", cn)
                { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.AddWithValue("@p_numero", dlg.Numero);
                cmd.Parameters.AddWithValue("@p_tipo_id", dlg.TipoId);      // ← nombre EXACTO del SP
                cmd.Parameters.AddWithValue("@p_capacidad", dlg.Capacidad);
                cmd.Parameters.AddWithValue("@p_precio_noche", dlg.PrecioNoche);

                cmd.ExecuteNonQuery(); // ← ejecutar!

                Cargar();
                SeleccionarUltimoVisible(); // ← foco en columna visible
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Crear habitación",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            var row = ((DataRowView)dgv.CurrentRow.DataBoundItem).Row;

            int id = Convert.ToInt32(row["id"]);
            string numero = row["numero"].ToString() ?? "";
            int tipoId = Convert.ToInt32(row["tipo_id"]);      // viene del listar
            int capacidad = Convert.ToInt32(row["capacidad"]);
            decimal precio = Convert.ToDecimal(row["precio_noche"]);

            using var dlg = new HabitacionEditForm(id, numero, tipoId, capacidad, precio);
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                using var cn = Db.Conn(); cn.Open();
                using var cmd = new MySqlCommand("sp_habitacion_actualizar", cn)
                { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.AddWithValue("@p_id", id);
                cmd.Parameters.AddWithValue("@p_numero", dlg.Numero);
                cmd.Parameters.AddWithValue("@p_tipo_id", dlg.TipoId);      // ← nombre EXACTO del SP
                cmd.Parameters.AddWithValue("@p_capacidad", dlg.Capacidad);
                cmd.Parameters.AddWithValue("@p_precio_noche", dlg.PrecioNoche);

                cmd.ExecuteNonQuery(); // ← ejecutar!

                Cargar();
                SeleccionarPorId(id); // ← foco en columna visible de la fila editada
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Editar habitación",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            var row = ((DataRowView)dgv.CurrentRow.DataBoundItem).Row;
            int id = Convert.ToInt32(row["id"]);

            if (MessageBox.Show($"¿Eliminar habitación #{id}?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                using var cn = Db.Conn(); cn.Open();
                using var cmd = new MySqlCommand("DELETE FROM habitaciones WHERE Id=@id", cn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                Cargar();
                SeleccionarUltimoVisible();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Eliminar habitación",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}