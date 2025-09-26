using System.Windows.Forms;
using System.Drawing;

namespace SistemaHotelAloha.Desktop.Forms
{
    partial class GestionHabitaciones
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
            dgvHabitaciones = new DataGridView();
            colId = new DataGridViewTextBoxColumn();
            colNumero = new DataGridViewTextBoxColumn();
            colTipo = new DataGridViewTextBoxColumn();
            colEstado = new DataGridViewTextBoxColumn();
            colPrecioNoche = new DataGridViewTextBoxColumn();
            groupBox1 = new GroupBox();
            txtPrecioNoche = new TextBox();
            label5 = new Label();
            txtEstado = new TextBox();
            label4 = new Label();
            txtTipo = new TextBox();
            label3 = new Label();
            txtNumero = new TextBox();
            label2 = new Label();
            txtId = new TextBox();
            label1 = new Label();
            btnCrear = new Button();
            btnModificar = new Button();
            btnEliminar = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvHabitaciones).BeginInit();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblTitulo.Location = new Point(12, 9);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(196, 21);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Gestión de Habitaciones";
            // 
            // dgvHabitaciones
            // 
            dgvHabitaciones.AllowUserToAddRows = false;
            dgvHabitaciones.AllowUserToDeleteRows = false;
            dgvHabitaciones.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            dgvHabitaciones.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvHabitaciones.Columns.AddRange(new DataGridViewColumn[] { colId, colNumero, colTipo, colEstado, colPrecioNoche });
            dgvHabitaciones.Location = new Point(12, 43);
            dgvHabitaciones.MultiSelect = false;
            dgvHabitaciones.Name = "dgvHabitaciones";
            dgvHabitaciones.ReadOnly = true;
            dgvHabitaciones.RowTemplate.Height = 25;
            dgvHabitaciones.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHabitaciones.Size = new Size(760, 260);
            dgvHabitaciones.TabIndex = 1;
            dgvHabitaciones.SelectionChanged += dgvHabitaciones_SelectionChanged;
            // 
            // colId
            // 
            colId.DataPropertyName = "Id";
            colId.HeaderText = "Id";
            colId.Name = "colId";
            colId.ReadOnly = true;
            colId.Width = 50;
            // 
            // colNumero
            // 
            colNumero.DataPropertyName = "Numero";
            colNumero.HeaderText = "Número";
            colNumero.Name = "colNumero";
            colNumero.ReadOnly = true;
            colNumero.Width = 120;
            // 
            // colTipo
            // 
            colTipo.DataPropertyName = "Tipo";
            colTipo.HeaderText = "Tipo";
            colTipo.Name = "colTipo";
            colTipo.ReadOnly = true;
            colTipo.Width = 120;
            // 
            // colEstado
            // 
            colEstado.DataPropertyName = "Estado";
            colEstado.HeaderText = "Estado";
            colEstado.Name = "colEstado";
            colEstado.ReadOnly = true;
            colEstado.Width = 140;
            // 
            // colPrecioNoche
            // 
            colPrecioNoche.DataPropertyName = "PrecioNoche";
            colPrecioNoche.HeaderText = "Precio/Noche";
            colPrecioNoche.Name = "colPrecioNoche";
            colPrecioNoche.ReadOnly = true;
            colPrecioNoche.Width = 140;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(txtPrecioNoche);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(txtEstado);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(txtTipo);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(txtNumero);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(txtId);
            groupBox1.Controls.Add(label1);
            groupBox1.Location = new Point(12, 309);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(760, 110);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Datos de la habitación";
            // 
            // txtPrecioNoche
            // 
            txtPrecioNoche.Location = new Point(620, 63);
            txtPrecioNoche.Name = "txtPrecioNoche";
            txtPrecioNoche.Size = new Size(120, 23);
            txtPrecioNoche.TabIndex = 9;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(620, 45);
            label5.Name = "label5";
            label5.Size = new Size(84, 15);
            label5.TabIndex = 8;
            label5.Text = "Precio/Noche";
            // 
            // txtEstado
            // 
            txtEstado.Location = new Point(470, 63);
            txtEstado.Name = "txtEstado";
            txtEstado.Size = new Size(120, 23);
            txtEstado.TabIndex = 7;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(470, 45);
            label4.Name = "label4";
            label4.Size = new Size(45, 15);
            label4.TabIndex = 6;
            label4.Text = "Estado";
            // 
            // txtTipo
            // 
            txtTipo.Location = new Point(320, 63);
            txtTipo.Name = "txtTipo";
            txtTipo.Size = new Size(120, 23);
            txtTipo.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(320, 45);
            label3.Name = "label3";
            label3.Size = new Size(31, 15);
            label3.TabIndex = 4;
            label3.Text = "Tipo";
            // 
            // txtNumero
            // 
            txtNumero.Location = new Point(170, 63);
            txtNumero.Name = "txtNumero";
            txtNumero.Size = new Size(120, 23);
            txtNumero.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(170, 45);
            label2.Name = "label2";
            label2.Size = new Size(54, 15);
            label2.TabIndex = 2;
            label2.Text = "Número";
            // 
            // txtId
            // 
            txtId.Location = new Point(20, 63);
            txtId.Name = "txtId";
            txtId.PlaceholderText = "Auto";
            txtId.ReadOnly = true;
            txtId.Size = new Size(120, 23);
            txtId.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(20, 45);
            label1.Name = "label1";
            label1.Size = new Size(18, 15);
            label1.TabIndex = 0;
            label1.Text = "Id";
            // 
            // btnCrear
            // 
            btnCrear.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnCrear.Location = new Point(12, 425);
            btnCrear.Name = "btnCrear";
            btnCrear.Size = new Size(160, 35);
            btnCrear.TabIndex = 3;
            btnCrear.Text = "Crear habitación";
            btnCrear.UseVisualStyleBackColor = true;
            btnCrear.Click += btnCrear_Click;
            // 
            // btnModificar
            // 
            btnModificar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnModificar.Location = new Point(188, 425);
            btnModificar.Name = "btnModificar";
            btnModificar.Size = new Size(180, 35);
            btnModificar.TabIndex = 4;
            btnModificar.Text = "Modificar habitación";
            btnModificar.UseVisualStyleBackColor = true;
            btnModificar.Click += btnModificar_Click;
            // 
            // btnEliminar
            // 
            btnEliminar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnEliminar.Location = new Point(384, 425);
            btnEliminar.Name = "btnEliminar";
            btnEliminar.Size = new Size(180, 35);
            btnEliminar.TabIndex = 5;
            btnEliminar.Text = "Eliminar habitación";
            btnEliminar.UseVisualStyleBackColor = true;
            btnEliminar.Click += btnEliminar_Click;
            // 
            // GestionHabitaciones
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 471);
            Controls.Add(btnEliminar);
            Controls.Add(btnModificar);
            Controls.Add(btnCrear);
            Controls.Add(groupBox1);
            Controls.Add(dgvHabitaciones);
            Controls.Add(lblTitulo);
            Name = "GestionHabitaciones";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Gestión de Habitaciones";
            Load += GestionHabitaciones_Load;
            ((System.ComponentModel.ISupportInitialize)dgvHabitaciones).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private Label lblTitulo;
        private DataGridView dgvHabitaciones;
        private DataGridViewTextBoxColumn colId;
        private DataGridViewTextBoxColumn colNumero;
        private DataGridViewTextBoxColumn colTipo;
        private DataGridViewTextBoxColumn colEstado;
        private DataGridViewTextBoxColumn colPrecioNoche;
        private GroupBox groupBox1;
        private TextBox txtPrecioNoche;
        private Label label5;
        private TextBox txtEstado;
        private Label label4;
        private TextBox txtTipo;
        private Label label3;
        private TextBox txtNumero;
        private Label label2;
        private TextBox txtId;
        private Label label1;
        private Button btnCrear;
        private Button btnModificar;
        private Button btnEliminar;
    }
}
