using System.Windows.Forms;
namespace SistemaHotelAloha.Desktop.Forms
{
    partial class ServicioAdicionalEditForm
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblNombre;
        private ComboBox cbServicio;
        private Label lblPrecio;
        private NumericUpDown nudPrecio;
        private CheckBox chkActivo;
        private Button btnAceptar;
        private Button btnCancelar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            lblNombre = new Label();
            cbServicio = new ComboBox();
            lblPrecio = new Label();
            nudPrecio = new NumericUpDown();
            chkActivo = new CheckBox();
            btnAceptar = new Button();
            btnCancelar = new Button();

            ((System.ComponentModel.ISupportInitialize)(nudPrecio)).BeginInit();
            SuspendLayout();

            // lblNombre
            lblNombre.AutoSize = true;
            lblNombre.Location = new System.Drawing.Point(18, 18);
            lblNombre.Name = "lblNombre";
            lblNombre.Size = new System.Drawing.Size(54, 15);
            lblNombre.Text = "Nombre:";

            // cbServicio
            cbServicio.DropDownStyle = ComboBoxStyle.DropDownList; // ← NO se tipea
            cbServicio.FormattingEnabled = true;
            cbServicio.Location = new System.Drawing.Point(90, 15);
            cbServicio.Name = "cbServicio";
            cbServicio.Size = new System.Drawing.Size(240, 23);

            // lblPrecio
            lblPrecio.AutoSize = true;
            lblPrecio.Location = new System.Drawing.Point(18, 56);
            lblPrecio.Name = "lblPrecio";
            lblPrecio.Size = new System.Drawing.Size(43, 15);
            lblPrecio.Text = "Precio:";

            // nudPrecio
            nudPrecio.DecimalPlaces = 2;
            nudPrecio.Location = new System.Drawing.Point(90, 54);
            nudPrecio.Maximum = 100000000;
            nudPrecio.Minimum = 0;
            nudPrecio.Name = "nudPrecio";
            nudPrecio.Size = new System.Drawing.Size(120, 23);
            nudPrecio.ThousandsSeparator = true;

            // chkActivo
            chkActivo.AutoSize = true;
            chkActivo.Location = new System.Drawing.Point(90, 90);
            chkActivo.Name = "chkActivo";
            chkActivo.Size = new System.Drawing.Size(60, 19);
            chkActivo.Text = "Activo";
            chkActivo.UseVisualStyleBackColor = true;

            // btnAceptar
            btnAceptar.Location = new System.Drawing.Point(72, 130);
            btnAceptar.Name = "btnAceptar";
            btnAceptar.Size = new System.Drawing.Size(90, 30);
            btnAceptar.Text = "Aceptar";
            btnAceptar.UseVisualStyleBackColor = true;
            btnAceptar.Click += btnAceptar_Click;

            // btnCancelar
            btnCancelar.Location = new System.Drawing.Point(190, 130);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new System.Drawing.Size(90, 30);
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            btnCancelar.Click += btnCancelar_Click;

            // Form
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(350, 180);
            Controls.Add(btnCancelar);
            Controls.Add(btnAceptar);
            Controls.Add(chkActivo);
            Controls.Add(nudPrecio);
            Controls.Add(lblPrecio);
            Controls.Add(cbServicio);
            Controls.Add(lblNombre);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ServicioAdicionalEditForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Servicio adicional";
            Load += ServicioAdicionalEditForm_Load;

            ((System.ComponentModel.ISupportInitialize)(nudPrecio)).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}