using MySql.Data.MySqlClient;
using SistemaHotelAloha.AccesoDatos;
using SistemaHotelAloha.AccesoDatos.Models;
using SistemaHotelAloha.Desktop.Forms;
using SistemaHotelAloha.Desktop.Helpers;
using SistemaHotelAloha.Desktop.Infra;
using SistemaHotelAloha.Desktop.Utils;
using System;
using System.Configuration; // para leer App.config
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaHotelAloha.Desktop.Forms
{
    public partial class GestionReservas : Form
    {
        private DataTable _dt = new();

        // Nombre real de la columna de la tabla 'reservas' que referencia la habitación
        // Lo detectamos en runtime: "IdHabitacion" o "id_habitacion"
        private string _colReservaHabitacion = "IdHabitacion";

        private readonly ReservasAdoRepository _repo;

        // Toolbar y controles de reporte
        private readonly Panel pnlToolbar = new();
        private readonly FlowLayoutPanel flow = new();
        private readonly DateTimePicker dtpMesAnio = new();
        private readonly CheckBox chkIncluirCanceladas = new();
        private readonly Button btnExportarPdf = new();

        public GestionReservas()
        {
            InitializeComponent();

            // connectionString local (no queda campo “asignado y no usado”)
            var connString = ConfigurationManager
                .ConnectionStrings["DefaultConnection"]
                .ConnectionString;

            _repo = new ReservasAdoRepository(connString);
        }

        private void GestionReservas_Load(object sender, EventArgs e)
        {
            DetectarColumnaHabitacion();
            Cargar();
            dgv.ClearSelection();
            Habilitar(false);

            // IMPORTANTE: crear barra superior ANTES de ajustar Dock del dgv
            ConfigurarToolbarReporte();

            // Aseguramos que el dgv quede “Fill” y el panel quede arriba visible
            dgv.Dock = DockStyle.Fill;
            pnlToolbar.BringToFront();
        }

        // ---------------------------------------------
        // Detecta si la columna es IdHabitacion o id_habitacion
        // ---------------------------------------------
        private void DetectarColumnaHabitacion()
        {
            try
            {
                using var cn = Db.Conn(); cn.Open();
                using var cmd = new MySqlCommand(@"
                    SELECT COLUMN_NAME
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME   = 'reservas'
                      AND COLUMN_NAME IN ('IdHabitacion','id_habitacion')
                    LIMIT 1;", cn);

                var name = cmd.ExecuteScalar() as string;
                if (!string.IsNullOrEmpty(name))
                    _colReservaHabitacion = name; // guarda el real
            }
            catch
            {
                // Si falla, dejamos el default "IdHabitacion"
            }
        }

        // ---------------------------------------------
        // Barra superior con DateTimePicker, Check y Botón PDF
        // ---------------------------------------------
        private void ConfigurarToolbarReporte()
        {
            // Panel superior
            pnlToolbar.Height = 48;
            pnlToolbar.Dock = DockStyle.Top;
            pnlToolbar.Padding = new Padding(8, 8, 8, 8);
            Controls.Add(pnlToolbar);

            // Flow para colocar controles en línea
            flow.Dock = DockStyle.Fill;
            flow.FlowDirection = FlowDirection.LeftToRight;
            flow.WrapContents = false;
            flow.AutoSize = false;
            pnlToolbar.Controls.Add(flow);

            // === DateTimePicker: modo calendario + límites ===
            var hoy = DateTime.Today;
            dtpMesAnio.ShowUpDown = false; // calendario desplegable
            dtpMesAnio.Format = DateTimePickerFormat.Custom;
            dtpMesAnio.CustomFormat = "MMMM yyyy";
            dtpMesAnio.MinDate = new DateTime(2000, 1, 1);
            dtpMesAnio.MaxDate = new DateTime(hoy.Year, hoy.Month, DateTime.DaysInMonth(hoy.Year, hoy.Month));
            dtpMesAnio.Value = new DateTime(hoy.Year, hoy.Month, 1);
            dtpMesAnio.Width = 160;
            dtpMesAnio.ValueChanged += (s, e) =>
            {
                // “Snap” al primer día del mes
                var v = dtpMesAnio.Value;
                dtpMesAnio.Value = new DateTime(v.Year, v.Month, 1);
            };

            // Check
            chkIncluirCanceladas.Text = "Incluir canceladas";
            chkIncluirCanceladas.AutoSize = true;
            chkIncluirCanceladas.Margin = new Padding(12, 10, 0, 0);

            // Botón Exportar
            btnExportarPdf.Text = "Exportar PDF (mes)";
            btnExportarPdf.AutoSize = true;
            btnExportarPdf.Margin = new Padding(12, 8, 0, 0);
            btnExportarPdf.Click += BtnExportarPdf_Click;

            // Agregar al flow
            flow.Controls.Add(dtpMesAnio);
            flow.Controls.Add(chkIncluirCanceladas);
            flow.Controls.Add(btnExportarPdf);
        }

        private void Cargar()
        {
            try
            {
                using var cn = Db.Conn(); cn.Open();
                using var cmd = new MySqlCommand("sp_reservas_listar", cn)
                { CommandType = CommandType.StoredProcedure };
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
            btnModificar.Enabled = on;
            btnEliminar.Enabled = on;
        }

        private void SeleccionarPorId(int id)
        {
            foreach (DataGridViewRow r in dgv.Rows)
            {
                if (r.DataBoundItem is DataRowView drv && Convert.ToInt32(drv.Row["id"]) == id)
                { r.Selected = true; dgv.CurrentCell = r.Cells[0]; break; }
            }
        }

        // ---------------------------------------------
        // Valida solapamiento en la misma habitación
        // ---------------------------------------------
        private bool HaySolapamiento(int idHabitacion, DateTime desde, DateTime hasta, int idReservaEditar)
        {
            // Usamos la columna real detectada
            string sql = $@"
                SELECT COUNT(1)
                FROM reservas r
                WHERE r.{_colReservaHabitacion} = @idHab
                  AND r.Id <> @idRes
                  AND NOT (@hasta <= r.FechaDesde OR @desde >= r.FechaHasta);";

            using var cn = Db.Conn(); cn.Open();
            using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@idHab", idHabitacion);
            cmd.Parameters.AddWithValue("@idRes", idReservaEditar);
            cmd.Parameters.AddWithValue("@desde", desde.Date);
            cmd.Parameters.AddWithValue("@hasta", hasta.Date);
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        // Helper para leer un int por nombre con fallback de alias
        private static int GetInt(DataRow row, params string[] names)
        {
            foreach (var n in names)
            {
                if (row.Table.Columns.Contains(n) && row[n] != DBNull.Value)
                    return Convert.ToInt32(row[n]);
            }
            return 0;
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            var row = ((DataRowView)dgv.CurrentRow.DataBoundItem).Row;

            int id = Convert.ToInt32(row["id"]);
            // soporta alias "id_habitacion" que suele devolver el SP y también posibles columnas reales
            int idHabitacion = GetInt(row, "id_habitacion", "IdHabitacion", "idHabitacion");
            DateTime desde = Convert.ToDateTime(row["fecha_desde"]);
            DateTime hasta = Convert.ToDateTime(row["fecha_hasta"]);

            using var dlg = new ReservaEditForm(id, idHabitacion, "", desde, hasta);
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            // Validación de solapamiento
            if (HaySolapamiento(dlg.IdHabitacion, dlg.FechaDesde, dlg.FechaHasta, id))
            {
                MessageBox.Show("La habitación ya está reservada en ese rango.", "Conflicto de fechas",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using var cn = Db.Conn(); cn.Open();
                using var cmd = new MySqlCommand("sp_reserva_actualizar", cn)
                { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@p_id", id);
                cmd.Parameters.AddWithValue("@p_id_habitacion", dlg.IdHabitacion);
                cmd.Parameters.AddWithValue("@p_fecha_desde", dlg.FechaDesde);
                cmd.Parameters.AddWithValue("@p_fecha_hasta", dlg.FechaHasta);

                // ejecutar UPDATE
                cmd.ExecuteNonQuery();

                Cargar();
                SeleccionarPorId(id);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Modificar reserva", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            var row = ((DataRowView)dgv.CurrentRow.DataBoundItem).Row;
            int id = Convert.ToInt32(row["id"]);

            if (MessageBox.Show($"¿Cancelar reserva #{id}?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                using var cn = Db.Conn(); cn.Open();
                using var cmd = new MySqlCommand("sp_reserva_cancelar", cn)
                { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@p_id", id);
                cmd.ExecuteNonQuery();
                Cargar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Cancelar reserva", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ---------------------------------------------
        // Exportar a PDF (mes)
        // ---------------------------------------------
        private async void BtnExportarPdf_Click(object? sender, EventArgs e)
        {
            try
            {
                btnExportarPdf.Enabled = false;

                var periodo = dtpMesAnio.Value;
                int anio = periodo.Year;
                int mes = periodo.Month;
                bool incluirCanceladas = chkIncluirCanceladas.Checked; // si luego lo usás dentro del PDF

                // 1) Traemos el reporte mensual (lista de ReporteMensualDto)
                var lista = await Task.Run(() => _repo.ObtenerReporteMensual(anio, mes));

                if (lista.Count == 0)
                {
                    MessageBox.Show("No hay reservas para el período seleccionado.",
                        "Reporte", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 2) Mapeamos al tipo que espera tu exportador (ReservaReporteDto)
                //    IMPORTANTE: no asignamos 'x.Reservas' (suele ser int) a una List<>
                var dataExport = lista.Select(x => new ReservaReporteDto
                {
                    Fecha = x.Dia,
                    // Si necesitás la cantidad, agregá una propiedad Cantidad al DTO y usala aquí.
                    // Cantidad = x.Reservas,
                    Total = x.Importe // <- reemplaza a 'Importe'
                }).ToList();

                using var sfd = new SaveFileDialog()
                {
                    Title = "Guardar reporte de reservas",
                    Filter = "PDF (*.pdf)|*.pdf",
                    FileName = $"Reservas_{anio}_{mes:00}.pdf",
                    OverwritePrompt = true
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    await Task.Run(() =>
                        // Tu helper acepta IEnumerable<ReservaReporteDto>
                        PdfReportes.ExportarReservasMensuales(
                            dataExport,
                            new DateTime(anio, mes, 1),
                            sfd.FileName
                        )
                    );

                    MessageBox.Show("PDF generado correctamente.",
                        "Reporte", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar PDF: {ex.Message}", "Reporte",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnExportarPdf.Enabled = true;
            }
        }
    }
}
