using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHotelAloha.Desktop.Utils
{
    public static class UiCatalogBinder
    {
        /// Reemplaza un TextBox por un ComboBox DropDownList, en el mismo lugar,
        /// lo bindea a un DataTable (catálogo) y retorna el ComboBox creado.
        public static ComboBox ReplaceTextBoxWithCombo(
            Control parent,
            TextBox textBoxToReplace,
            DataTable catalog,
            string displayMember,
            string valueMember,
            string? selectedTextOrId = null
        )
        {
            var combo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Left = textBoxToReplace.Left,
                Top = textBoxToReplace.Top,
                Width = textBoxToReplace.Width,
                Height = textBoxToReplace.Height,
                TabIndex = textBoxToReplace.TabIndex,
                DataSource = catalog,
                DisplayMember = displayMember,
                ValueMember = valueMember
            };

            // Si venía un valor (texto o id) lo intentamos seleccionar
            if (!string.IsNullOrWhiteSpace(selectedTextOrId))
            {
                // 1) por Id
                if (int.TryParse(selectedTextOrId, out var id))
                {
                    combo.SelectedValue = id;
                }
                else
                {
                    // 2) por texto
                    var row = catalog.AsEnumerable()
                        .FirstOrDefault(r => string.Equals(
                            r.Field<string>(displayMember),
                            selectedTextOrId,
                            StringComparison.OrdinalIgnoreCase));
                    if (row is not null) combo.SelectedValue = row.Field<int>(valueMember);
                }
            }

            // Insertar el combo y ocultar el TextBox
            parent.Controls.Add(combo);
            textBoxToReplace.Visible = false;

            return combo;
        }

        /// Agrega una DataGridViewComboBoxColumn vinculada a un catálogo
        public static DataGridViewComboBoxColumn AddComboColumn(
            DataGridView grid,
            string dataPropertyName, string headerText,
            DataTable catalog, string displayMember, string valueMember, int width = 140)
        {
            var col = new DataGridViewComboBoxColumn
            {
                DataPropertyName = dataPropertyName,
                HeaderText = headerText,
                DataSource = catalog,
                DisplayMember = displayMember,
                ValueMember = valueMember,
                FlatStyle = FlatStyle.Flat,
                Width = width
            };
            grid.Columns.Add(col);
            return col;
        }
    }
}
