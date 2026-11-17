namespace PizzeriaBellaItalia.Presentacion
{
    partial class FormClientes
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dgvClientes = new DataGridView();
            grpCliente = new GroupBox();
            btnListar = new Button();
            btnSelecciona = new Button();
            btnNuevo = new Button();
            btnBuscar = new Button();
            btnElimina = new Button();
            btnGrabar = new Button();
            txtTelefono = new TextBox();
            txtNombre = new TextBox();
            label2 = new Label();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvClientes).BeginInit();
            grpCliente.SuspendLayout();
            SuspendLayout();
            // 
            // dgvClientes
            // 
            dgvClientes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvClientes.Location = new Point(12, 194);
            dgvClientes.Name = "dgvClientes";
            dgvClientes.Size = new Size(368, 346);
            dgvClientes.TabIndex = 0;
            // 
            // grpCliente
            // 
            grpCliente.BackColor = SystemColors.ControlDark;
            grpCliente.Controls.Add(btnListar);
            grpCliente.Controls.Add(btnSelecciona);
            grpCliente.Controls.Add(btnNuevo);
            grpCliente.Controls.Add(btnBuscar);
            grpCliente.Controls.Add(btnElimina);
            grpCliente.Controls.Add(btnGrabar);
            grpCliente.Controls.Add(txtTelefono);
            grpCliente.Controls.Add(txtNombre);
            grpCliente.Controls.Add(label2);
            grpCliente.Controls.Add(label1);
            grpCliente.Location = new Point(12, 12);
            grpCliente.Name = "grpCliente";
            grpCliente.Size = new Size(368, 176);
            grpCliente.TabIndex = 1;
            grpCliente.TabStop = false;
            grpCliente.Text = "Datos Cliente";
            // 
            // btnListar
            // 
            btnListar.Location = new Point(261, 97);
            btnListar.Name = "btnListar";
            btnListar.Size = new Size(75, 23);
            btnListar.TabIndex = 10;
            btnListar.Text = "Listar";
            btnListar.UseVisualStyleBackColor = true;
            // 
            // btnSelecciona
            // 
            btnSelecciona.Location = new Point(195, 62);
            btnSelecciona.Name = "btnSelecciona";
            btnSelecciona.Size = new Size(158, 23);
            btnSelecciona.TabIndex = 9;
            btnSelecciona.Text = "Seleccion para un Pedido";
            btnSelecciona.UseVisualStyleBackColor = true;
            btnSelecciona.Click += btnSelecciona_Click;
            // 
            // btnNuevo
            // 
            btnNuevo.Location = new Point(261, 137);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(75, 23);
            btnNuevo.TabIndex = 8;
            btnNuevo.Text = "Nuevo";
            btnNuevo.UseVisualStyleBackColor = true;
            btnNuevo.Click += btnNuevo_Click;
            // 
            // btnBuscar
            // 
            btnBuscar.Location = new Point(276, 37);
            btnBuscar.Name = "btnBuscar";
            btnBuscar.Size = new Size(75, 23);
            btnBuscar.TabIndex = 7;
            btnBuscar.Text = "Buscar";
            btnBuscar.UseVisualStyleBackColor = true;
            btnBuscar.Click += btnBuscar_Click;
            // 
            // btnElimina
            // 
            btnElimina.Location = new Point(136, 137);
            btnElimina.Name = "btnElimina";
            btnElimina.Size = new Size(75, 23);
            btnElimina.TabIndex = 6;
            btnElimina.Text = "Eliminar";
            btnElimina.TextAlign = ContentAlignment.MiddleRight;
            btnElimina.UseVisualStyleBackColor = true;
            btnElimina.Click += btnElimina_Click;
            // 
            // btnGrabar
            // 
            btnGrabar.Location = new Point(20, 137);
            btnGrabar.Name = "btnGrabar";
            btnGrabar.Size = new Size(75, 23);
            btnGrabar.TabIndex = 4;
            btnGrabar.Text = "Grabar";
            btnGrabar.UseVisualStyleBackColor = true;
            // 
            // txtTelefono
            // 
            txtTelefono.Location = new Point(20, 98);
            txtTelefono.Name = "txtTelefono";
            txtTelefono.Size = new Size(167, 23);
            txtTelefono.TabIndex = 3;
            // 
            // txtNombre
            // 
            txtNombre.Location = new Point(20, 37);
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(250, 23);
            txtNombre.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(75, 70);
            label2.Name = "label2";
            label2.Size = new Size(52, 15);
            label2.TabIndex = 1;
            label2.Text = "Telefono";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(20, 19);
            label1.Name = "label1";
            label1.Size = new Size(107, 15);
            label1.TabIndex = 0;
            label1.Text = "Nombre Completo";
            // 
            // FormClientes
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.WindowFrame;
            ClientSize = new Size(393, 552);
            Controls.Add(grpCliente);
            Controls.Add(dgvClientes);
            Name = "FormClientes";
            Text = "Clientes";
            Load += FormClientes_Load;
            ((System.ComponentModel.ISupportInitialize)dgvClientes).EndInit();
            grpCliente.ResumeLayout(false);
            grpCliente.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dgvClientes; // <-- AQUÍ ESTABA EL ERROR (antes decía dataGridView1)
        private GroupBox grpCliente;
        private Button btnGrabar;
        private TextBox txtTelefono;
        private TextBox txtNombre;
        private Label label2;
        private Label label1;
        private Button btnBuscar;
        private Button btnElimina;
        private Button btnNuevo;
        private Button btnSelecciona;
        private Button btnListar;
    }
}