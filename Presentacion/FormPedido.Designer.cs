namespace PizzeriaBellaItalia.Presentacion; // Namespace de la capa de presentación.

partial class FormPedido : Form
{
    private System.ComponentModel.IContainer components = null;

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
    /// Método requerido por el Diseñador: crea instancias de controles, establece sus propiedades
    /// (posición, tamaño, texto, etc.), arma la jerarquía visual y conecta eventos.
    /// No debe modificarse manualmente con el editor de código salvo casos controlados.
    /// </summary>
    private void InitializeComponent()
    {
        DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
        DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
        grpCliente = new GroupBox();
        btnEditaClientes = new Button();
        btnBuscarPedidoPorId = new Button();
        txtIdPedido = new TextBox();
        lblIdPedido = new Label();
        btnBuscaPedidosXCliente = new Button();
        txtTelefono = new TextBox();
        txtCliente = new TextBox();
        lblTelefono = new Label();
        lblNombre = new Label();
        btnTodosLosPedidos = new Button();
        grpPizza = new GroupBox();
        cboMasa = new ComboBox();
        lblMasa = new Label();
        cboTamano = new ComboBox();
        lblTamano = new Label();
        grpIngredientes = new GroupBox();
        chkTocino = new CheckBox();
        chkPepperoni = new CheckBox();
        chkAceitunas = new CheckBox();
        chkChampinones = new CheckBox();
        chkQuesoExtra = new CheckBox();
        grpEntrega = new GroupBox();
        chkConvenioEstudiante = new CheckBox();
        rdbDelivery = new RadioButton();
        rdbRetiro = new RadioButton();
        lblSubtotalValor = new Label();
        lblIvaValor = new Label();
        lblTotalValor = new Label();
        lblDeliveryValor = new Label();
        lblDescuentoValor = new Label();
        grpResumen = new GroupBox();
        lblDescuentoNombre = new Label();
        lblDeliveryNombre = new Label();
        lblIvaNombre = new Label();
        lblTotalNombre = new Label();
        lblSubtotalNombre = new Label();
        btnCalcular = new Button();
        btnLimpiar = new Button();
        btnActualizarPedido = new Button();
        btnEliminarPedido = new Button();
        btnConfirmar = new Button();
        btnProbarConexion = new Button();
        dgvPedidos = new DataGridView();
        lblAyuda = new Label();
        grpCliente.SuspendLayout();
        grpPizza.SuspendLayout();
        grpIngredientes.SuspendLayout();
        grpEntrega.SuspendLayout();
        grpResumen.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvPedidos).BeginInit();
        SuspendLayout();
        // 
        // grpCliente
        // 
        grpCliente.BackColor = Color.FromArgb(45, 45, 48);
        grpCliente.Controls.Add(btnEditaClientes);
        grpCliente.Controls.Add(btnBuscarPedidoPorId);
        grpCliente.Controls.Add(txtIdPedido);
        grpCliente.Controls.Add(lblIdPedido);
        grpCliente.Controls.Add(btnBuscaPedidosXCliente);
        grpCliente.Controls.Add(txtTelefono);
        grpCliente.Controls.Add(txtCliente);
        grpCliente.Controls.Add(lblTelefono);
        grpCliente.Controls.Add(lblNombre);
        grpCliente.ForeColor = Color.FromArgb(230, 230, 230);
        grpCliente.Location = new Point(12, 2);
        grpCliente.Margin = new Padding(3, 2, 3, 2);
        grpCliente.Name = "grpCliente";
        grpCliente.Padding = new Padding(3, 2, 3, 2);
        grpCliente.Size = new Size(321, 158);
        grpCliente.TabIndex = 0;
        grpCliente.TabStop = false;
        grpCliente.Text = "Datos del Cliente";
        // 
        // btnEditaClientes
        // 
        btnEditaClientes.BackColor = Color.FromArgb(63, 63, 70);
        btnEditaClientes.FlatAppearance.BorderColor = Color.FromArgb(70, 70, 70);
        btnEditaClientes.FlatStyle = FlatStyle.Flat;
        btnEditaClientes.ForeColor = Color.GhostWhite;
        btnEditaClientes.Location = new Point(13, 123);
        btnEditaClientes.Margin = new Padding(3, 2, 3, 2);
        btnEditaClientes.Name = "btnEditaClientes";
        btnEditaClientes.Size = new Size(292, 23);
        btnEditaClientes.TabIndex = 8;
        btnEditaClientes.Text = "Edición Clientes";
        btnEditaClientes.UseVisualStyleBackColor = false;
        btnEditaClientes.Click += btnEditaClientes_Click;
        // 
        // btnBuscarPedidoPorId
        // 
        btnBuscarPedidoPorId.BackColor = Color.FromArgb(63, 63, 70);
        btnBuscarPedidoPorId.FlatAppearance.BorderColor = Color.FromArgb(70, 70, 70);
        btnBuscarPedidoPorId.FlatStyle = FlatStyle.Flat;
        btnBuscarPedidoPorId.ForeColor = Color.Gold;
        btnBuscarPedidoPorId.Location = new Point(125, 96);
        btnBuscarPedidoPorId.Margin = new Padding(3, 2, 3, 2);
        btnBuscarPedidoPorId.Name = "btnBuscarPedidoPorId";
        btnBuscarPedidoPorId.Size = new Size(180, 23);
        btnBuscarPedidoPorId.TabIndex = 7;
        btnBuscarPedidoPorId.Text = "# Buscar pedido por ID";
        btnBuscarPedidoPorId.UseVisualStyleBackColor = false;
        btnBuscarPedidoPorId.Click += btnBuscarPedidoPorId_Click;
        // 
        // txtIdPedido
        // 
        txtIdPedido.BackColor = Color.FromArgb(37, 37, 38);
        txtIdPedido.BorderStyle = BorderStyle.FixedSingle;
        txtIdPedido.ForeColor = Color.FromArgb(230, 230, 230);
        txtIdPedido.Location = new Point(40, 96);
        txtIdPedido.Margin = new Padding(3, 2, 3, 2);
        txtIdPedido.Name = "txtIdPedido";
        txtIdPedido.Size = new Size(70, 23);
        txtIdPedido.TabIndex = 6;
        // 
        // lblIdPedido
        // 
        lblIdPedido.AutoSize = true;
        lblIdPedido.BackColor = Color.Transparent;
        lblIdPedido.ForeColor = Color.FromArgb(230, 230, 230);
        lblIdPedido.Location = new Point(13, 99);
        lblIdPedido.Name = "lblIdPedido";
        lblIdPedido.Size = new Size(21, 15);
        lblIdPedido.TabIndex = 5;
        lblIdPedido.Text = "ID:";
        // 
        // btnBuscaPedidosXCliente
        // 
        btnBuscaPedidosXCliente.BackColor = Color.FromArgb(63, 63, 70);
        btnBuscaPedidosXCliente.FlatAppearance.BorderColor = Color.FromArgb(70, 70, 70);
        btnBuscaPedidosXCliente.FlatStyle = FlatStyle.Flat;
        btnBuscaPedidosXCliente.ForeColor = Color.Gold;
        btnBuscaPedidosXCliente.Location = new Point(10, 67);
        btnBuscaPedidosXCliente.Margin = new Padding(3, 2, 3, 2);
        btnBuscaPedidosXCliente.Name = "btnBuscaPedidosXCliente";
        btnBuscaPedidosXCliente.Size = new Size(295, 23);
        btnBuscaPedidosXCliente.TabIndex = 4;
        btnBuscaPedidosXCliente.Text = "🔍 Buscar pedido por cliente";
        btnBuscaPedidosXCliente.UseVisualStyleBackColor = false;
        btnBuscaPedidosXCliente.Click += btnBuscaPedidosXCliente_Click;
        // 
        // txtTelefono
        // 
        txtTelefono.BackColor = Color.FromArgb(37, 37, 38);
        txtTelefono.BorderStyle = BorderStyle.FixedSingle;
        txtTelefono.ForeColor = Color.FromArgb(230, 230, 230);
        txtTelefono.Location = new Point(195, 40);
        txtTelefono.Margin = new Padding(3, 2, 3, 2);
        txtTelefono.Name = "txtTelefono";
        txtTelefono.Size = new Size(110, 23);
        txtTelefono.TabIndex = 3;
        // 
        // txtCliente
        // 
        txtCliente.BackColor = Color.FromArgb(37, 37, 38);
        txtCliente.BorderStyle = BorderStyle.FixedSingle;
        txtCliente.ForeColor = Color.FromArgb(230, 230, 230);
        txtCliente.Location = new Point(10, 39);
        txtCliente.Margin = new Padding(3, 2, 3, 2);
        txtCliente.Name = "txtCliente";
        txtCliente.Size = new Size(179, 23);
        txtCliente.TabIndex = 2;
        // 
        // lblTelefono
        // 
        lblTelefono.AutoSize = true;
        lblTelefono.BackColor = Color.Transparent;
        lblTelefono.ForeColor = Color.FromArgb(230, 230, 230);
        lblTelefono.Location = new Point(196, 20);
        lblTelefono.Name = "lblTelefono";
        lblTelefono.Size = new Size(55, 15);
        lblTelefono.TabIndex = 1;
        lblTelefono.Text = "Telefono:";
        // 
        // lblNombre
        // 
        lblNombre.AutoSize = true;
        lblNombre.BackColor = Color.Transparent;
        lblNombre.ForeColor = Color.FromArgb(230, 230, 230);
        lblNombre.Location = new Point(22, 20);
        lblNombre.Name = "lblNombre";
        lblNombre.Size = new Size(54, 15);
        lblNombre.TabIndex = 0;
        lblNombre.Text = "Nombre:";
        // 
        // btnTodosLosPedidos
        // 
        btnTodosLosPedidos.BackColor = Color.FromArgb(63, 63, 70);
        btnTodosLosPedidos.FlatAppearance.BorderColor = Color.FromArgb(70, 70, 70);
        btnTodosLosPedidos.FlatStyle = FlatStyle.Flat;
        btnTodosLosPedidos.ForeColor = Color.LightSteelBlue;
        btnTodosLosPedidos.Location = new Point(490, 400);
        btnTodosLosPedidos.Margin = new Padding(3, 2, 3, 2);
        btnTodosLosPedidos.Name = "btnTodosLosPedidos";
        btnTodosLosPedidos.Size = new Size(180, 23);
        btnTodosLosPedidos.TabIndex = 8;
        btnTodosLosPedidos.Text = "≡ Todos los pedidos";
        btnTodosLosPedidos.UseVisualStyleBackColor = false;
        btnTodosLosPedidos.Click += btnTodosLosPedidos_Click;
        // 
        // grpPizza
        // 
        grpPizza.BackColor = Color.FromArgb(45, 45, 48);
        grpPizza.Controls.Add(cboMasa);
        grpPizza.Controls.Add(lblMasa);
        grpPizza.Controls.Add(cboTamano);
        grpPizza.Controls.Add(lblTamano);
        grpPizza.ForeColor = Color.FromArgb(230, 230, 230);
        grpPizza.Location = new Point(339, 54);
        grpPizza.Margin = new Padding(3, 2, 3, 2);
        grpPizza.Name = "grpPizza";
        grpPizza.Padding = new Padding(3, 2, 3, 2);
        grpPizza.Size = new Size(345, 106);
        grpPizza.TabIndex = 1;
        grpPizza.TabStop = false;
        grpPizza.Text = "Tamaño y Masa:";
        // 
        // cboMasa
        // 
        cboMasa.BackColor = Color.FromArgb(37, 37, 38);
        cboMasa.FlatStyle = FlatStyle.Flat;
        cboMasa.ForeColor = Color.FromArgb(230, 230, 230);
        cboMasa.FormattingEnabled = true;
        cboMasa.Location = new Point(71, 54);
        cboMasa.Margin = new Padding(3, 2, 3, 2);
        cboMasa.Name = "cboMasa";
        cboMasa.Size = new Size(250, 23);
        cboMasa.TabIndex = 4;
        cboMasa.SelectedIndexChanged += Calculo_Changed;
        // 
        // lblMasa
        // 
        lblMasa.AutoSize = true;
        lblMasa.BackColor = Color.Transparent;
        lblMasa.ForeColor = Color.FromArgb(230, 230, 230);
        lblMasa.Location = new Point(5, 57);
        lblMasa.Name = "lblMasa";
        lblMasa.Size = new Size(38, 15);
        lblMasa.TabIndex = 3;
        lblMasa.Text = "Masa:";
        // 
        // cboTamano
        // 
        cboTamano.BackColor = Color.FromArgb(37, 37, 38);
        cboTamano.FlatStyle = FlatStyle.Flat;
        cboTamano.ForeColor = Color.FromArgb(230, 230, 230);
        cboTamano.FormattingEnabled = true;
        cboTamano.Location = new Point(71, 28);
        cboTamano.Margin = new Padding(3, 2, 3, 2);
        cboTamano.Name = "cboTamano";
        cboTamano.Size = new Size(250, 23);
        cboTamano.TabIndex = 2;
        cboTamano.SelectedIndexChanged += Calculo_Changed;
        // 
        // lblTamano
        // 
        lblTamano.AutoSize = true;
        lblTamano.BackColor = Color.Transparent;
        lblTamano.ForeColor = Color.FromArgb(230, 230, 230);
        lblTamano.Location = new Point(5, 31);
        lblTamano.Name = "lblTamano";
        lblTamano.Size = new Size(52, 15);
        lblTamano.TabIndex = 1;
        lblTamano.Text = "Tamaño:";
        // 
        // grpIngredientes
        // 
        grpIngredientes.BackColor = Color.FromArgb(45, 45, 48);
        grpIngredientes.Controls.Add(chkTocino);
        grpIngredientes.Controls.Add(chkPepperoni);
        grpIngredientes.Controls.Add(chkAceitunas);
        grpIngredientes.Controls.Add(chkChampinones);
        grpIngredientes.Controls.Add(chkQuesoExtra);
        grpIngredientes.ForeColor = Color.FromArgb(230, 230, 230);
        grpIngredientes.Location = new Point(430, 175);
        grpIngredientes.Margin = new Padding(3, 2, 3, 2);
        grpIngredientes.Name = "grpIngredientes";
        grpIngredientes.Padding = new Padding(3, 2, 3, 2);
        grpIngredientes.Size = new Size(158, 135);
        grpIngredientes.TabIndex = 3;
        grpIngredientes.TabStop = false;
        grpIngredientes.Text = "Adicionales ($800 c/u)";
        // 
        // chkTocino
        // 
        chkTocino.AutoSize = true;
        chkTocino.BackColor = Color.FromArgb(45, 45, 48);
        chkTocino.ForeColor = Color.FromArgb(230, 230, 230);
        chkTocino.Location = new Point(9, 112);
        chkTocino.Margin = new Padding(3, 2, 3, 2);
        chkTocino.Name = "chkTocino";
        chkTocino.Size = new Size(61, 19);
        chkTocino.TabIndex = 5;
        chkTocino.Text = "Tocino";
        chkTocino.UseVisualStyleBackColor = true;
        chkTocino.CheckedChanged += Calculo_Changed;
        // 
        // chkPepperoni
        // 
        chkPepperoni.AutoSize = true;
        chkPepperoni.BackColor = Color.FromArgb(45, 45, 48);
        chkPepperoni.ForeColor = Color.FromArgb(230, 230, 230);
        chkPepperoni.Location = new Point(9, 44);
        chkPepperoni.Margin = new Padding(3, 2, 3, 2);
        chkPepperoni.Name = "chkPepperoni";
        chkPepperoni.Size = new Size(80, 19);
        chkPepperoni.TabIndex = 4;
        chkPepperoni.Text = "Pepperoni";
        chkPepperoni.UseVisualStyleBackColor = true;
        chkPepperoni.CheckedChanged += Calculo_Changed;
        // 
        // chkAceitunas
        // 
        chkAceitunas.AutoSize = true;
        chkAceitunas.BackColor = Color.FromArgb(45, 45, 48);
        chkAceitunas.ForeColor = Color.FromArgb(230, 230, 230);
        chkAceitunas.Location = new Point(9, 89);
        chkAceitunas.Margin = new Padding(3, 2, 3, 2);
        chkAceitunas.Name = "chkAceitunas";
        chkAceitunas.Size = new Size(78, 19);
        chkAceitunas.TabIndex = 2;
        chkAceitunas.Text = "Aceitunas";
        chkAceitunas.UseVisualStyleBackColor = true;
        chkAceitunas.CheckedChanged += Calculo_Changed;
        // 
        // chkChampinones
        // 
        chkChampinones.AutoSize = true;
        chkChampinones.BackColor = Color.FromArgb(45, 45, 48);
        chkChampinones.ForeColor = Color.FromArgb(230, 230, 230);
        chkChampinones.Location = new Point(9, 67);
        chkChampinones.Margin = new Padding(3, 2, 3, 2);
        chkChampinones.Name = "chkChampinones";
        chkChampinones.Size = new Size(100, 19);
        chkChampinones.TabIndex = 1;
        chkChampinones.Text = "Champiñones";
        chkChampinones.UseVisualStyleBackColor = true;
        chkChampinones.CheckedChanged += Calculo_Changed;
        // 
        // chkQuesoExtra
        // 
        chkQuesoExtra.AutoSize = true;
        chkQuesoExtra.BackColor = Color.FromArgb(45, 45, 48);
        chkQuesoExtra.ForeColor = Color.FromArgb(230, 230, 230);
        chkQuesoExtra.Location = new Point(9, 22);
        chkQuesoExtra.Margin = new Padding(3, 2, 3, 2);
        chkQuesoExtra.Name = "chkQuesoExtra";
        chkQuesoExtra.Size = new Size(89, 19);
        chkQuesoExtra.TabIndex = 0;
        chkQuesoExtra.Text = "Queso Extra";
        chkQuesoExtra.UseVisualStyleBackColor = true;
        chkQuesoExtra.CheckedChanged += Calculo_Changed;
        // 
        // grpEntrega
        // 
        grpEntrega.BackColor = Color.FromArgb(45, 45, 48);
        grpEntrega.Controls.Add(chkConvenioEstudiante);
        grpEntrega.Controls.Add(rdbDelivery);
        grpEntrega.Controls.Add(rdbRetiro);
        grpEntrega.ForeColor = Color.FromArgb(230, 230, 230);
        grpEntrega.Location = new Point(12, 164);
        grpEntrega.Margin = new Padding(3, 2, 3, 2);
        grpEntrega.Name = "grpEntrega";
        grpEntrega.Padding = new Padding(3, 2, 3, 2);
        grpEntrega.Size = new Size(321, 80);
        grpEntrega.TabIndex = 4;
        grpEntrega.TabStop = false;
        grpEntrega.Text = "Servicio y Descuento";
        // 
        // chkConvenioEstudiante
        // 
        chkConvenioEstudiante.AutoSize = true;
        chkConvenioEstudiante.BackColor = Color.FromArgb(45, 45, 48);
        chkConvenioEstudiante.ForeColor = Color.FromArgb(230, 230, 230);
        chkConvenioEstudiante.Location = new Point(15, 52);
        chkConvenioEstudiante.Margin = new Padding(3, 2, 3, 2);
        chkConvenioEstudiante.Name = "chkConvenioEstudiante";
        chkConvenioEstudiante.Size = new Size(226, 19);
        chkConvenioEstudiante.TabIndex = 2;
        chkConvenioEstudiante.Text = "Convenio Estudiante/IPVG (10% Dcto)";
        chkConvenioEstudiante.UseVisualStyleBackColor = true;
        chkConvenioEstudiante.CheckedChanged += Calculo_Changed;
        // 
        // rdbDelivery
        // 
        rdbDelivery.AutoSize = true;
        rdbDelivery.BackColor = Color.FromArgb(45, 45, 48);
        rdbDelivery.ForeColor = Color.FromArgb(230, 230, 230);
        rdbDelivery.Location = new Point(145, 29);
        rdbDelivery.Margin = new Padding(3, 2, 3, 2);
        rdbDelivery.Name = "rdbDelivery";
        rdbDelivery.Size = new Size(119, 19);
        rdbDelivery.TabIndex = 1;
        rdbDelivery.Text = "Delivery ($2.000.-)";
        rdbDelivery.UseVisualStyleBackColor = true;
        rdbDelivery.CheckedChanged += Calculo_Changed;
        // 
        // rdbRetiro
        // 
        rdbRetiro.AutoSize = true;
        rdbRetiro.BackColor = Color.FromArgb(45, 45, 48);
        rdbRetiro.Checked = true;
        rdbRetiro.ForeColor = Color.FromArgb(230, 230, 230);
        rdbRetiro.Location = new Point(15, 29);
        rdbRetiro.Margin = new Padding(3, 2, 3, 2);
        rdbRetiro.Name = "rdbRetiro";
        rdbRetiro.Size = new Size(103, 19);
        rdbRetiro.TabIndex = 0;
        rdbRetiro.TabStop = true;
        rdbRetiro.Text = "Retiro en Local";
        rdbRetiro.UseVisualStyleBackColor = true;
        rdbRetiro.CheckedChanged += Calculo_Changed;
        // 
        // lblSubtotalValor
        // 
        lblSubtotalValor.AutoSize = true;
        lblSubtotalValor.BackColor = Color.Transparent;
        lblSubtotalValor.ForeColor = Color.FromArgb(230, 230, 230);
        lblSubtotalValor.Location = new Point(223, 26);
        lblSubtotalValor.Name = "lblSubtotalValor";
        lblSubtotalValor.Size = new Size(43, 15);
        lblSubtotalValor.TabIndex = 5;
        lblSubtotalValor.Text = "$0 CLP";
        // 
        // lblIvaValor
        // 
        lblIvaValor.AutoSize = true;
        lblIvaValor.BackColor = Color.Transparent;
        lblIvaValor.ForeColor = Color.FromArgb(230, 230, 230);
        lblIvaValor.Location = new Point(222, 93);
        lblIvaValor.Name = "lblIvaValor";
        lblIvaValor.Size = new Size(43, 15);
        lblIvaValor.TabIndex = 6;
        lblIvaValor.Text = "$0 CLP";
        // 
        // lblTotalValor
        // 
        lblTotalValor.AutoSize = true;
        lblTotalValor.BackColor = Color.Transparent;
        lblTotalValor.ForeColor = Color.FromArgb(230, 230, 230);
        lblTotalValor.Location = new Point(221, 122);
        lblTotalValor.Name = "lblTotalValor";
        lblTotalValor.Size = new Size(43, 15);
        lblTotalValor.TabIndex = 7;
        lblTotalValor.Text = "$0 CLP";
        // 
        // lblDeliveryValor
        // 
        lblDeliveryValor.AutoSize = true;
        lblDeliveryValor.BackColor = Color.Transparent;
        lblDeliveryValor.ForeColor = Color.FromArgb(230, 230, 230);
        lblDeliveryValor.Location = new Point(223, 71);
        lblDeliveryValor.Name = "lblDeliveryValor";
        lblDeliveryValor.Size = new Size(43, 15);
        lblDeliveryValor.TabIndex = 8;
        lblDeliveryValor.Text = "$0 CLP";
        // 
        // lblDescuentoValor
        // 
        lblDescuentoValor.AutoSize = true;
        lblDescuentoValor.BackColor = Color.Transparent;
        lblDescuentoValor.ForeColor = Color.FromArgb(230, 230, 230);
        lblDescuentoValor.Location = new Point(223, 47);
        lblDescuentoValor.Name = "lblDescuentoValor";
        lblDescuentoValor.Size = new Size(43, 15);
        lblDescuentoValor.TabIndex = 9;
        lblDescuentoValor.Text = "$0 CLP";
        // 
        // grpResumen
        // 
        grpResumen.BackColor = Color.FromArgb(45, 45, 48);
        grpResumen.Controls.Add(lblDescuentoNombre);
        grpResumen.Controls.Add(lblDeliveryNombre);
        grpResumen.Controls.Add(lblIvaNombre);
        grpResumen.Controls.Add(lblTotalNombre);
        grpResumen.Controls.Add(lblSubtotalNombre);
        grpResumen.Controls.Add(lblSubtotalValor);
        grpResumen.Controls.Add(lblDescuentoValor);
        grpResumen.Controls.Add(lblIvaValor);
        grpResumen.Controls.Add(lblDeliveryValor);
        grpResumen.Controls.Add(lblTotalValor);
        grpResumen.ForeColor = Color.FromArgb(230, 230, 230);
        grpResumen.Location = new Point(12, 248);
        grpResumen.Margin = new Padding(3, 2, 3, 2);
        grpResumen.Name = "grpResumen";
        grpResumen.Padding = new Padding(3, 2, 3, 2);
        grpResumen.Size = new Size(321, 145);
        grpResumen.TabIndex = 10;
        grpResumen.TabStop = false;
        grpResumen.Text = "Detalle de Pago";
        // 
        // lblDescuentoNombre
        // 
        lblDescuentoNombre.AutoSize = true;
        lblDescuentoNombre.BackColor = Color.Transparent;
        lblDescuentoNombre.ForeColor = Color.FromArgb(230, 230, 230);
        lblDescuentoNombre.Location = new Point(10, 47);
        lblDescuentoNombre.Name = "lblDescuentoNombre";
        lblDescuentoNombre.Size = new Size(99, 15);
        lblDescuentoNombre.TabIndex = 15;
        lblDescuentoNombre.Text = "Descuento (10%):";
        // 
        // lblDeliveryNombre
        // 
        lblDeliveryNombre.AutoSize = true;
        lblDeliveryNombre.BackColor = Color.Transparent;
        lblDeliveryNombre.ForeColor = Color.FromArgb(230, 230, 230);
        lblDeliveryNombre.Location = new Point(10, 71);
        lblDeliveryNombre.Name = "lblDeliveryNombre";
        lblDeliveryNombre.Size = new Size(98, 15);
        lblDeliveryNombre.TabIndex = 14;
        lblDeliveryNombre.Text = "Recargo Delivery:";
        // 
        // lblIvaNombre
        // 
        lblIvaNombre.AutoSize = true;
        lblIvaNombre.BackColor = Color.Transparent;
        lblIvaNombre.ForeColor = Color.FromArgb(230, 230, 230);
        lblIvaNombre.Location = new Point(10, 93);
        lblIvaNombre.Name = "lblIvaNombre";
        lblIvaNombre.Size = new Size(101, 15);
        lblIvaNombre.TabIndex = 13;
        lblIvaNombre.Text = "IVA Chileno (19%)";
        // 
        // lblTotalNombre
        // 
        lblTotalNombre.AutoSize = true;
        lblTotalNombre.BackColor = Color.Transparent;
        lblTotalNombre.ForeColor = Color.FromArgb(230, 230, 230);
        lblTotalNombre.Location = new Point(10, 122);
        lblTotalNombre.Name = "lblTotalNombre";
        lblTotalNombre.Size = new Size(93, 15);
        lblTotalNombre.TabIndex = 12;
        lblTotalNombre.Text = "TOTAL A PAGAR:";
        // 
        // lblSubtotalNombre
        // 
        lblSubtotalNombre.AutoSize = true;
        lblSubtotalNombre.BackColor = Color.Transparent;
        lblSubtotalNombre.ForeColor = Color.FromArgb(230, 230, 230);
        lblSubtotalNombre.Location = new Point(10, 26);
        lblSubtotalNombre.Name = "lblSubtotalNombre";
        lblSubtotalNombre.Size = new Size(83, 15);
        lblSubtotalNombre.TabIndex = 10;
        lblSubtotalNombre.Text = "Subtotal Pizza:";
        // 
        // btnCalcular
        // 
        btnCalcular.BackColor = Color.FromArgb(63, 63, 70);
        btnCalcular.FlatAppearance.BorderColor = Color.FromArgb(70, 70, 70);
        btnCalcular.FlatStyle = FlatStyle.Flat;
        btnCalcular.ForeColor = Color.LimeGreen;
        btnCalcular.Location = new Point(420, 324);
        btnCalcular.Margin = new Padding(3, 2, 3, 2);
        btnCalcular.Name = "btnCalcular";
        btnCalcular.Size = new Size(177, 48);
        btnCalcular.TabIndex = 11;
        btnCalcular.Text = "✔ Calcular Total / Grabar";
        btnCalcular.UseVisualStyleBackColor = false;
        btnCalcular.Click += BtnCalcular_Click;
        // 
        // btnLimpiar
        // 
        btnLimpiar.BackColor = Color.FromArgb(63, 63, 70);
        btnLimpiar.FlatAppearance.BorderColor = Color.FromArgb(70, 70, 70);
        btnLimpiar.FlatStyle = FlatStyle.Flat;
        btnLimpiar.ForeColor = Color.White;
        btnLimpiar.Location = new Point(12, 402);
        btnLimpiar.Margin = new Padding(3, 2, 3, 2);
        btnLimpiar.Name = "btnLimpiar";
        btnLimpiar.Size = new Size(144, 22);
        btnLimpiar.TabIndex = 12;
        btnLimpiar.Text = "＋ Nuevo Pedido";
        btnLimpiar.UseVisualStyleBackColor = false;
        btnLimpiar.Click += BtnLimpiar_Click;
        // 
        // btnActualizarPedido
        // 
        btnActualizarPedido.BackColor = Color.FromArgb(63, 63, 70);
        btnActualizarPedido.FlatAppearance.BorderColor = Color.FromArgb(70, 70, 70);
        btnActualizarPedido.FlatStyle = FlatStyle.Flat;
        btnActualizarPedido.ForeColor = Color.SkyBlue;
        btnActualizarPedido.Location = new Point(163, 400);
        btnActualizarPedido.Margin = new Padding(3, 2, 3, 2);
        btnActualizarPedido.Name = "btnActualizarPedido";
        btnActualizarPedido.Size = new Size(144, 26);
        btnActualizarPedido.TabIndex = 13;
        btnActualizarPedido.Text = "✎ Actualizar Pedido";
        btnActualizarPedido.UseVisualStyleBackColor = false;
        btnActualizarPedido.Click += btnActualizarPedido_Click;
        // 
        // btnEliminarPedido
        // 
        btnEliminarPedido.BackColor = Color.FromArgb(63, 63, 70);
        btnEliminarPedido.FlatAppearance.BorderColor = Color.FromArgb(70, 70, 70);
        btnEliminarPedido.FlatStyle = FlatStyle.Flat;
        btnEliminarPedido.ForeColor = Color.Orange;
        btnEliminarPedido.Location = new Point(324, 400);
        btnEliminarPedido.Margin = new Padding(3, 2, 3, 2);
        btnEliminarPedido.Name = "btnEliminarPedido";
        btnEliminarPedido.Size = new Size(144, 24);
        btnEliminarPedido.TabIndex = 14;
        btnEliminarPedido.Text = "✖ Eliminar Pedido";
        btnEliminarPedido.UseVisualStyleBackColor = false;
        btnEliminarPedido.Click += btnEliminarPedido_Click;
        // 
        // btnConfirmar
        // 
        btnConfirmar.BackColor = Color.FromArgb(63, 63, 70);
        btnConfirmar.FlatAppearance.BorderColor = Color.FromArgb(70, 70, 70);
        btnConfirmar.FlatStyle = FlatStyle.Flat;
        btnConfirmar.ForeColor = Color.LightGray;
        btnConfirmar.Location = new Point(548, 11);
        btnConfirmar.Margin = new Padding(3, 2, 3, 2);
        btnConfirmar.Name = "btnConfirmar";
        btnConfirmar.Size = new Size(112, 22);
        btnConfirmar.TabIndex = 15;
        btnConfirmar.Text = "\u23fb Salir";
        btnConfirmar.UseVisualStyleBackColor = false;
        btnConfirmar.Click += BtnSalir_Click;
        // 
        // btnProbarConexion
        // 
        btnProbarConexion.BackColor = Color.FromArgb(63, 63, 70);
        btnProbarConexion.FlatAppearance.BorderColor = Color.FromArgb(70, 70, 70);
        btnProbarConexion.FlatStyle = FlatStyle.Flat;
        btnProbarConexion.ForeColor = Color.LightGreen;
        btnProbarConexion.Location = new Point(370, 8);
        btnProbarConexion.Margin = new Padding(3, 2, 3, 2);
        btnProbarConexion.Name = "btnProbarConexion";
        btnProbarConexion.Size = new Size(146, 25);
        btnProbarConexion.TabIndex = 17;
        btnProbarConexion.Text = "● Probar Conexion";
        btnProbarConexion.UseVisualStyleBackColor = false;
        btnProbarConexion.Click += btnProbarConexion_Click;
        // 
        // dgvPedidos
        // 
        dgvPedidos.AllowUserToAddRows = false;
        dgvPedidos.AllowUserToDeleteRows = false;
        dgvPedidos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvPedidos.BackgroundColor = Color.FromArgb(45, 45, 48);
        dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
        dataGridViewCellStyle1.BackColor = Color.FromArgb(55, 55, 60);
        dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F);
        dataGridViewCellStyle1.ForeColor = Color.FromArgb(230, 230, 230);
        dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
        dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
        dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
        dgvPedidos.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
        dgvPedidos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
        dataGridViewCellStyle2.BackColor = Color.FromArgb(37, 37, 38);
        dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
        dataGridViewCellStyle2.ForeColor = Color.FromArgb(230, 230, 230);
        dataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(70, 90, 120);
        dataGridViewCellStyle2.SelectionForeColor = Color.White;
        dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
        dgvPedidos.DefaultCellStyle = dataGridViewCellStyle2;
        dgvPedidos.EnableHeadersVisualStyles = false;
        dgvPedidos.GridColor = Color.FromArgb(70, 70, 70);
        dgvPedidos.Location = new Point(12, 433);
        dgvPedidos.MultiSelect = false;
        dgvPedidos.Name = "dgvPedidos";
        dgvPedidos.ReadOnly = true;
        dgvPedidos.RowHeadersVisible = false;
        dgvPedidos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvPedidos.Size = new Size(672, 236);
        dgvPedidos.TabIndex = 16;
        dgvPedidos.CellClick += dgvPedidos_CellClick;
        // 
        // lblAyuda
        // 
        lblAyuda.BackColor = Color.FromArgb(45, 45, 48);
        lblAyuda.BorderStyle = BorderStyle.FixedSingle;
        lblAyuda.ForeColor = Color.FromArgb(230, 230, 230);
        lblAyuda.Location = new Point(12, 672);
        lblAyuda.Name = "lblAyuda";
        lblAyuda.Padding = new Padding(6, 4, 6, 4);
        lblAyuda.Size = new Size(672, 38);
        lblAyuda.TabIndex = 18;
        lblAyuda.Text = "Pase el mouse sobre un botón para ver ayuda.";
        lblAyuda.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // FormPedido
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(30, 30, 30);
        ClientSize = new Size(696, 721);
        Controls.Add(lblAyuda);
        Controls.Add(btnTodosLosPedidos);
        Controls.Add(btnProbarConexion);
        Controls.Add(dgvPedidos);
        Controls.Add(btnConfirmar);
        Controls.Add(btnEliminarPedido);
        Controls.Add(btnActualizarPedido);
        Controls.Add(btnCalcular);
        Controls.Add(btnLimpiar);
        Controls.Add(grpResumen);
        Controls.Add(grpEntrega);
        Controls.Add(grpIngredientes);
        Controls.Add(grpPizza);
        Controls.Add(grpCliente);
        ForeColor = Color.FromArgb(230, 230, 230);
        Margin = new Padding(3, 2, 3, 2);
        Name = "FormPedido";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Sistema de Pedidos - Pizzería Bella Italia (v3.0) Jorge Vera V.";
        Load += FormPedido_Load;
        grpCliente.ResumeLayout(false);
        grpCliente.PerformLayout();
        grpPizza.ResumeLayout(false);
        grpPizza.PerformLayout();
        grpIngredientes.ResumeLayout(false);
        grpIngredientes.PerformLayout();
        grpEntrega.ResumeLayout(false);
        grpEntrega.PerformLayout();
        grpResumen.ResumeLayout(false);
        grpResumen.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvPedidos).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private GroupBox grpCliente;
    private TextBox txtTelefono;
    private TextBox txtCliente;
    private Label lblTelefono;
    private Label lblNombre;
    private Button btnBuscaPedidosXCliente;
    private Label lblIdPedido;
    private TextBox txtIdPedido;
    private Button btnBuscarPedidoPorId;
    private Button btnTodosLosPedidos;
    private GroupBox grpPizza;
    private Label lblTamano;
    private ComboBox cboMasa;
    private Label lblMasa;
    private ComboBox cboTamano;
    private GroupBox grpIngredientes;
    private CheckBox chkTocino;
    private CheckBox chkPepperoni;
    private CheckBox chkAceitunas;
    private CheckBox chkChampinones;
    private CheckBox chkQuesoExtra;
    private GroupBox grpEntrega;
    private RadioButton rdbDelivery;
    private RadioButton rdbRetiro;
    private CheckBox chkConvenioEstudiante;
    private Label lblSubtotalValor;
    private Label lblIvaValor;
    private Label lblTotalValor;
    private Label lblDeliveryValor;
    private Label lblDescuentoValor;
    private GroupBox grpResumen;
    private Label lblDescuentoNombre;
    private Label lblDeliveryNombre;
    private Label lblIvaNombre;
    private Label lblTotalNombre;
    private Label lblSubtotalNombre;
    private Button btnCalcular;
    private Button btnLimpiar;
    private Button btnActualizarPedido;
    private Button btnEliminarPedido;
    private Button btnConfirmar;
    private Button btnProbarConexion;
    private DataGridView dgvPedidos;
    private Label lblAyuda;
    private Button btnEditaClientes;
}
