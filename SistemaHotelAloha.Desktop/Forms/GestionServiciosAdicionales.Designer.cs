using System.Windows.Forms;
using System.Drawing;

namespace SistemaHotelAloha.Desktop.Forms
{
    partial class GestionServiciosAdicionales
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblTitulo = new Label();
            dgvServicios = new DataGridView();
            colId = new DataGridViewTextBoxColumn();
            colNombre = new DataGridViewTextBoxColumn();
            colPrecio = new DataGridViewTextBoxColumn();
            groupBox1 = new GroupBox();
            txtPrecio = new TextBox();
            label3 = new Label();
            txtNombre = new TextBox();
            label2 = new Label();
            txtId = new TextBox();
            label1 = new Label();
            btnCrear = new Button();
            btnModificar = new Button();
            btnEliminar = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvServicios).BeginInit();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblTitulo.Location = new Point(12, 9);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(254, 21);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Gestión de Servicios Adicionales";
            // 
            // dgvServicios
            // 
            dgvServicios.AllowUserToAddRows = false;
            dgvServicios.AllowUserToDeleteRows = false;
            dgvServicios.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            dgvServicios.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvServicios.Columns.AddRange(new DataGridViewColumn[] { colId, colNombre, colPrecio });
            dgvServicios.Location = new Point(12, 43);
            dgvServicios.MultiSelect = false;
            dgvServicios.Name = "dgvServicios";
            dgvServicios.ReadOnly = true;
            dgvServicios.RowTemplate.Height = 25;
            dgvServicios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvServicios.Size = new Size(540, 240);
            dgvServicios.TabIndex = 1;
            dgvServicios.SelectionChanged += dgvServicios_SelectionChanged;
            // 
            // colId
            // 
            colId.DataPropertyName = "Id";
            colId.HeaderText = "Id";
            colId.Name = "colId";
            colId.ReadOnly = true;
            colId.Width = 50;
            // 
            // colNombre
            // 
            colNombre.DataPropertyName = "Nombre";
            colNombre.HeaderText = "Nombre";
            colNombre.Name = "colNombre";
            colNombre.ReadOnly = true;
            colNombre.Width = 260;
            // 
            // colPrecio
            // 
            colPrecio.DataPropertyName = "Precio";
            colPrecio.HeaderText = "Precio";
            colPrecio.Name = "colPrecio";
            colPrecio.ReadOnly = true;
            colPrecio.Width = 120;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(txtPrecio);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(txtNombre);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(txtId);
            groupBox1.Controls.Add(label1);
            groupBox1.Location = new Point(12, 289);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(540, 95);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Datos del servicio";
            // 
            // txtPrecio
            // 
            txtPrecio.Location = new Point(370, 54);
            txtPrecio.Name = "txtPrecio";
            txtPrecio.Size = new Size(140, 23);
            txtPrecio.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(370, 36);
            label3.Name = "label3";
            label3.Size = new Size(41, 15);
            label3.TabIndex = 4;
            label3.Text = "Precio";
            // 
            // txtNombre
            // 
            txtNombre.Location = new Point(120, 54);
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(230, 23);
            txtNombre.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(120, 36);
            label2.Name = "label2";
            label2.Size = new Size(51, 15);
            label2.TabIndex = 2;
            label2.Text = "Nombre";
            // 
            // txtId
            // 
            txtId.Location = new Point(20, 54);
            txtId.Name = "txtId";
            txtId.PlaceholderText = "Auto";
            txtId.ReadOnly = true;
            txtId.Size = new Size(70, 23);
            txtId.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(20, 36);
            label1.Name = "label1";
            label1.Size = new Size(18, 15);
            label1.TabIndex = 0;
            label1.Text = "Id";
            // 
            // btnCrear
            // 
            btnCrear.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnCrear.Location = new Point(12, 390);
            btnCrear.Name = "btnCrear";
            btnCrear.Size = new Size(140, 35);
            btnCrear.TabIndex = 3;
            btnCrear.Text = "Crear servicio";
            btnCrear.UseVisualStyleBackColor = true;
            btnCrear.Click += btnCrear_Click;
            // 
            // btnModificar
            // 
            btnModificar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnModificar.Location = new Point(168, 390);
            btnModificar.Name = "btnModificar";
            btnModificar.Size = new Size(160, 35);
            btnModificar.TabIndex = 4;
            btnModificar.Text = "Modificar servicio";
            btnModificar.UseVisualStyleBackColor = true;
            btnModificar.Click += btnModificar_Click;
            // 
            // btnEliminar
            // 
            btnEliminar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnEliminar.Location = new Point(344, 390);
            btnEliminar.Name = "btnEliminar";
            btnEliminar.Size = new Size(160, 35);
            btnEliminar.TabIndex = 5;
            btnEliminar.Text = "Eliminar servicio";
            btnEliminar.UseVisualStyleBackColor = true;
            btnEliminar.Click += btnEliminar_Click;
            // 
            // GestionServiciosAdicionales
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(564, 441);
            Controls.Add(btnEliminar);
            Controls.Add(btnModificar);
            Controls.Add(btnCrear);
            Controls.Add(groupBox1);
            Controls.Add(dgvServicios);
            Controls.Add(lblTitulo);
            Name = "GestionServiciosAdicionales";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Gestión de Servicios Adicionales";
            Load += GestionServiciosAdicionales_Load;
            ((System.ComponentModel.ISupportInitialize)dgvServicios).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private Label lblTitulo;
        private DataGridView dgvServicios;
        private DataGridViewTextBoxColumn colId;
        private DataGridViewTextBoxColumn colNombre;
        private DataGridViewTextBoxColumn colPrecio;
        private GroupBox groupBox1;
        private TextBox txtPrecio;
        private Label label3;
        private TextBox txtNombre;
        private Label label2;
        private TextBox txtId;
        private Label label1;
        private Button btnCrear;
        private Button btnModificar;
        private Button btnEliminar;
    }
}
