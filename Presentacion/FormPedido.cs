using System;                          // Tipos base (Exception, etc.).
using System.Data;                     // DataTable para resultados y grilla.
using System.Linq;                     // ToArray() al cargar combos desde reglas de negocio.
using System.Windows.Forms;            // WinForms, controles y MessageBox.
using PizzeriaBellaItalia.Dominio;     // PedidoPizza, PedidoDAO, ConexionBD, CalculadoraPedido.

namespace PizzeriaBellaItalia.Presentacion
{
    /// Formulario principal de gestión de pedidos.
    public partial class FormPedido : Form // "partial" se completa en FormPedido.Designer.cs.
    {
        //-------------------------- ATRIBUTOS --------------------------
        private readonly PedidoDAO pedidoDao = new PedidoDAO(); // DAO para operaciones CRUD contra dbo.Pedidos.
        private bool cargandoFormulario = false; // Evita recalcular montos mientras se llenan o limpian los controles del formulario.
        //--------------------------- GRILLA -----------------------------------------------
        // Carga todos los pedidos en la grilla (oculta IdPedido y define títulos visibles).
        private void CargarPedidos()
        {
            try
            {
                dgvPedidos.DataSource = pedidoDao.ObtenerPedidosDataTable();
                var colId = dgvPedidos.Columns["IdPedido"];
                if (colId != null) colId.Visible = false;
                var colIdCliente = dgvPedidos.Columns["IdCliente"];
                if (colIdCliente != null) colIdCliente.Visible = false;
                if (dgvPedidos.Columns["Cliente"] != null) dgvPedidos.Columns["Cliente"]!.HeaderText = "Cliente";
                if (dgvPedidos.Columns["Telefono"] != null) dgvPedidos.Columns["Telefono"]!.HeaderText = "Teléfono";
                if (dgvPedidos.Columns["TamanoNombre"] != null) dgvPedidos.Columns["TamanoNombre"]!.HeaderText = "Tamaño";
                if (dgvPedidos.Columns["TotalPagar"] != null) dgvPedidos.Columns["TotalPagar"]!.HeaderText = "Total a Pagar";
            }
            catch (Exception ex)
            {
                MostrarEstado("No se pudieron cargar los pedidos.", false);
                MessageBox.Show("No se pudieron cargar los pedidos.\n" + ex.Message, "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        // Al hacer clic en una fila, obtiene IdPedido y muestra el detalle completo.
        private void dgvPedidos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var cell = dgvPedidos.Rows[e.RowIndex].Cells["IdPedido"];
            if (cell?.Value is int id) vistaPedido(id);
            else if (int.TryParse(cell?.Value?.ToString(), out int id2)) vistaPedido(id2);
        }
        // ------------------------- CALCULOS -----------------------------------
        // Construye un PedidoPizza tomando valores de la UI y delegando el cálculo a las reglas de negocio.
        private PedidoPizza ConstruirPedido()
        {
            var datos = new CalculadoraPedido.DatosPedido
            {
                Cliente = txtCliente.Text.Trim(),
                Telefono = txtTelefono.Text.Trim(),
                Tamano = cboTamano.Text,
                Masa = cboMasa.Text,
                QuesoExtra = chkQuesoExtra.Checked,
                Pepperoni = chkPepperoni.Checked,
                Champinones = chkChampinones.Checked,
                Aceitunas = chkAceitunas.Checked,
                Tocino = chkTocino.Checked,
                EsDelivery = rdbDelivery.Checked,
                AplicaConvenioEstudiante = chkConvenioEstudiante.Checked
            };

            return CalculadoraPedido.Calcular(datos);
        }
        // Recalcula el detalle de pago cuando cambia cualquier dato que influye en los
        private void Calculo_Changed(object sender, EventArgs e)
        // montos: tamaño, masa, adicionales, servicio (Retiro/Delivery) o descuento
        // (Convenio Estudiante/IPVG). Usa las mismas reglas de negocio que
        // CalculadoraPedido.Calcular, que es la que se graba en dbo.Pedidos.
        {
            if (cargandoFormulario) return;      // la carga/limpieza del form ya fija los montos
            if (!grpResumen.Enabled) return;     // el detalle de pago se habilita con el primer cálculo
            if (string.IsNullOrWhiteSpace(txtCliente.Text)) return;

            MostrarResumen(ConstruirPedido());
        }
        //--------------------------- BOTONES -------------------------
        // Buscar por cliente: filtra con DAO (LIKE) usando el nombre ingresado.
        private void btnBuscaPedidosXCliente_Click(object sender, EventArgs e)
        {
            try
            {
                string filtro = txtCliente.Text.Trim();
                if (string.IsNullOrEmpty(filtro))
                {
                    MostrarEstado("Debe ingresar el nombre del cliente a buscar.", false);
                    MessageBox.Show("Debe ingresar el nombre del cliente.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCliente.Focus();
                    return;
                }

                dgvPedidos.DataSource = pedidoDao.BuscarPedidosPorCliente(filtro);
                var colId = dgvPedidos.Columns["IdPedido"];
                if (colId != null) colId.Visible = false;
                var colIdCliente = dgvPedidos.Columns["IdCliente"];
                if (colIdCliente != null) colIdCliente.Visible = false;
                if (dgvPedidos.Columns["Cliente"] != null) dgvPedidos.Columns["Cliente"]!.HeaderText = "Cliente";
                if (dgvPedidos.Columns["Telefono"] != null) dgvPedidos.Columns["Telefono"]!.HeaderText = "Teléfono";
                if (dgvPedidos.Columns["TamanoNombre"] != null) dgvPedidos.Columns["TamanoNombre"]!.HeaderText = "Tamaño";
                if (dgvPedidos.Columns["TotalPagar"] != null) dgvPedidos.Columns["TotalPagar"]!.HeaderText = "Total a Pagar";
                MostrarEstado("Búsqueda por cliente realizada.", true);
            }
            catch (Exception ex)
            {
                MostrarEstado("No se pudo realizar la búsqueda.", false);
                MessageBox.Show("No se pudo buscar.\n" + ex.Message, "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        // Buscar por ID: usa ObtenerPedidoPorId; si existe muestra fila y carga vista completa.
        private void btnBuscarPedidoPorId_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtIdPedido.Text.Trim(), out int id) || id <= 0)
            {
                MessageBox.Show("Ingrese un ID válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtIdPedido.Focus(); return;
            }
            try
            {
                var p = pedidoDao.ObtenerPedidoPorId(id); // Consulta el pedido completo por ID (o null si no existe).

                // Crea una tabla temporal con las columnas para mostrar el resultado en la grilla.
                var dt = new DataTable();
                dt.Columns.Add("IdPedido", typeof(int));   // ID interno (se ocultará en la UI).
                dt.Columns.Add("Cliente");                 // Dato visible.
                dt.Columns.Add("Telefono");                // Dato visible.
                dt.Columns.Add("TamanoNombre");            // Dato visible.
                dt.Columns.Add("TotalPagar", typeof(int)); // Total final visible.

                // Si se encontró el pedido, agrega una fila con sus datos.
                if (p != null) dt.Rows.Add(p.IdPedido, p.Cliente, p.Telefono, p.TamanoNombre, p.TotalPagar);

                dgvPedidos.DataSource = dt; // Enlaza la tabla a la grilla (con o sin filas).

                // Oculta IdPedido y define los títulos visibles de las columnas.
                var colId = dgvPedidos.Columns["IdPedido"];
                if (colId != null) colId.Visible = false;
                var colIdCliente = dgvPedidos.Columns["IdCliente"];
                if (colIdCliente != null) colIdCliente.Visible = false;
                if (dgvPedidos.Columns["Cliente"] != null) dgvPedidos.Columns["Cliente"]!.HeaderText = "Cliente";
                if (dgvPedidos.Columns["Telefono"] != null) dgvPedidos.Columns["Telefono"]!.HeaderText = "Teléfono";
                if (dgvPedidos.Columns["TamanoNombre"] != null) dgvPedidos.Columns["TamanoNombre"]!.HeaderText = "Tamaño";
                if (dgvPedidos.Columns["TotalPagar"] != null) dgvPedidos.Columns["TotalPagar"]!.HeaderText = "Total a Pagar";

                // Informa si no existe; si existe, carga todos los detalles en el formulario.
                if (p == null)
                {
                    MostrarEstado("No se encontró el pedido con el ID indicado.", false);
                    MessageBox.Show("No se encontró el pedido.", "Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MostrarEstado("Pedido encontrado y cargado en el formulario.", true);
                    vistaPedido(p.IdPedido);
                }
            }
            catch (Exception ex)
            {
                MostrarEstado("No se pudo buscar el pedido.", false);
                MessageBox.Show("No se pudo buscar.\n" + ex.Message, "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        // Muestra todos los pedidos.
        private void btnTodosLosPedidos_Click(object sender, EventArgs e) => CargarPedidos();
        // UPDATE: valida ID/cliente, construye pedido y llama a ActualizarPedido.
        private void btnActualizarPedido_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtIdPedido.Text.Trim(), out int id) || id <= 0)
            { MessageBox.Show("Seleccione o ingrese un ID de pedido válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (string.IsNullOrWhiteSpace(txtCliente.Text))
            { MessageBox.Show("Ingrese el nombre del cliente.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            try
            {
                var p = ConstruirPedido();
                p.IdPedido = id;
                if (pedidoDao.ActualizarPedido(p))
                { MessageBox.Show("Pedido actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information); CargarPedidos(); }
                else
                    MessageBox.Show("No se pudo actualizar el pedido.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex) { MessageBox.Show("Error al actualizar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        // DELETE: valida ID, confirma y llama a EliminarPedido.
        private void btnEliminarPedido_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtIdPedido.Text.Trim(), out int id) || id <= 0) { MessageBox.Show("Seleccione o ingrese un ID de pedido válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (MessageBox.Show("¿Está seguro de eliminar este pedido?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                if (pedidoDao.EliminarPedido(id))
                {
                    MostrarEstado("Pedido eliminado correctamente.", true);
                    MessageBox.Show("Pedido eliminado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarFormulario();
                    CargarPedidos();
                }
                else
                {
                    MostrarEstado("No se pudo eliminar el pedido.", false);
                    MessageBox.Show("No se pudo eliminar el pedido.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex) { MostrarEstado("Error al eliminar el pedido.", false); MessageBox.Show("Error al eliminar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        // Calcular/Grabar: valida, calcula y muestra montos; si confirma, inserta (CREATE) y refresca.
        private void BtnCalcular_Click(object sender, EventArgs e)
        {
            // Valida que el cliente no esté vacío.
            if (string.IsNullOrWhiteSpace(txtCliente.Text))
            {
                MessageBox.Show("Ingrese el nombre del cliente.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCliente.Focus();
                return;
            }

            //construye un objeto PedidoPizza con todos los datos y montos calculados.en pantalla
            var p = ConstruirPedido();

            //muestra los montos en la sección "Detalle de Pago" (subtotal, descuento, delivery, IVA y total)
            MostrarResumen(p);

            //si confirma, llama a InsertarPedido y refresca la grilla
            if (MessageBox.Show($"¿Grabar pedido?\n\nTotal: ${p.TotalPagar:N0} CLP", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    if (pedidoDao.InsertarPedido(p))
                    {
                        MostrarEstado("Pedido guardado exitosamente.", true);
                        MessageBox.Show("Pedido guardado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarFormulario();
                        CargarPedidos();
                    }
                }
                catch (Exception ex)
                {
                    MostrarEstado("Error al grabar el pedido.", false);
                    MessageBox.Show("Error al grabar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        // Salir: confirma y cierra el formulario.
        private void BtnSalir_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Desea salir?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) Close();
        }
        // Prueba la conexión y muestra cadena/estado.
        private void btnProbarConexion_Click(object sender, EventArgs e)
        {
            bool ok = ConexionBD.ProbarConexion();
            MostrarEstado(ok ? "Conexión exitosa." : "No se pudo conectar a la base de datos.", ok);
            string detalle = "Cadena de conexión:\n" +
                ConexionBD.CadenaConexion +
                "\n\nEstado: " +
                (ok ? "Conexión exitosa." : "No se pudo conectar.");
            MessageBox.Show(detalle, "Probar Conexión", MessageBoxButtons.OK, ok ? MessageBoxIcon.Information : MessageBoxIcon.Error);
        }
        // "Nuevo Pedido": limpia el formulario.
        private void BtnLimpiar_Click(object sender, EventArgs e) => LimpiarFormulario();
        //carga form clientes para seleccionar un cliente y cargarlo en el pedido
        private void btnEditaClientes_Click(object sender, EventArgs e)
        {
            // Creamos una instancia de FormClientes
            FormClientes frm = new FormClientes();

            // forma modal (bloquea la ventana anterior hasta cerrarla)
            frm.ShowDialog();

            // interactuar con ambas ventanas al mismo tiempo, usa:
            // frm.Show();
        }
        //---------------------------- FORMULARIO --------------------------
        // Constructor: se ejecuta al crear el formulario.
        public FormPedido()
        {
            InitializeComponent(); // Inicializa y configura todos los controles definidos en el diseñador.
        }
        // Carga y presenta todos los datos del pedido por ID.
        private void vistaPedido(int id)
        {
            try
            {
                var p = pedidoDao.ObtenerPedidoPorId(id);
                if (p == null) return;
                LlenaForm(p);
            }
            catch (Exception ex) { MostrarEstado("No se pudo cargar el detalle del pedido.", false); MessageBox.Show("No se pudo cargar el pedido.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        // Evento Load: inicializa combos desde diccionarios, resumen deshabilitado y carga pedidos.
        private void FormPedido_Load(object sender, EventArgs e)
        {
            cargandoFormulario = true;   // evita recalculos mientras se llenan los combos
            // Carga tamaños desde reglas de negocio y selecciona por defecto "Mediana".
            cboTamano.Items.Clear();
            cboTamano.Items.AddRange(CalculadoraPedido.Tamanos.Keys.ToArray());
            cboTamano.SelectedItem = "Mediana";
            // Carga masas desde reglas de negocio y selecciona por defecto "Masa Tradicional".
            cboMasa.Items.Clear();
            cboMasa.Items.AddRange(CalculadoraPedido.Masas.Keys.ToArray());
            cboMasa.SelectedItem = "Masa Tradicional";
            // Resumen de montos deshabilitado hasta calcular/ver un pedido.
            grpResumen.Enabled = false;
            lblSubtotalValor.Text = lblDescuentoValor.Text = lblDeliveryValor.Text = lblIvaValor.Text = lblTotalValor.Text = "$ 0 CLP";
            cargandoFormulario = false; //al salir ya no esta cargando form
            // Configura textos de ayuda al pasar el mouse por los botones.
            ConfigurarAyuda();
            // Carga listado inicial desde BD.
            CargarPedidos();
        }
        // Llena el formulario con los datos y montos de un pedido.
        private void LlenaForm(PedidoPizza p)
        {
            // Llena el form
            cargandoFormulario = true;   // evita recalculos mientras se asignan los controles
            txtIdPedido.Text = p.IdPedido.ToString();
            txtCliente.Text = p.Cliente; txtTelefono.Text = p.Telefono;
            cboTamano.SelectedItem = p.TamanoNombre; if (cboTamano.SelectedIndex < 0) cboTamano.Text = p.TamanoNombre;
            cboMasa.SelectedItem = p.MasaNombre; if (cboMasa.SelectedIndex < 0) cboMasa.Text = p.MasaNombre;
            chkQuesoExtra.Checked = p.QuesoExtra; chkPepperoni.Checked = p.Pepperoni; chkChampinones.Checked = p.Champinones;
            chkAceitunas.Checked = p.Aceitunas; chkTocino.Checked = p.Tocino;
            rdbDelivery.Checked = p.EsDelivery; rdbRetiro.Checked = !p.EsDelivery;
            chkConvenioEstudiante.Checked = p.AplicaConvenioEstudiante;
            cargandoFormulario = false;

            // Muestra los montos del pedido cargado (subtotal, descuento, delivery, IVA y total).
            MostrarResumen(p);
        }
        // Muestra los montos calculados en la sección "Detalle de Pago"
        private void MostrarResumen(PedidoPizza p)
        // (subtotal, descuento, delivery, IVA y total a pagar).
        {
            lblSubtotalValor.Text = $"$ {p.SubtotalPizza:N0} CLP";
            lblDescuentoValor.Text = $"- $ {p.MontoDescuento:N0} CLP";
            lblDeliveryValor.Text = $"+ $ {p.CostoDelivery:N0} CLP";
            lblIvaValor.Text = $"+ $ {p.MontoIva:N0} CLP";
            lblTotalValor.Text = $"$ {p.TotalPagar:N0} CLP";
            grpResumen.Enabled = true;
        }
        // Restablece formulario a estado inicial (nuevo pedido).
        private void LimpiarFormulario()
        {
            cargandoFormulario = true;   // evita recalculos mientras se restablecen los controles
            txtIdPedido.Clear(); txtCliente.Clear(); txtTelefono.Clear();
            if (cboTamano.Items.Count > 0) cboTamano.SelectedIndex = 1;
            if (cboMasa.Items.Count > 0) cboMasa.SelectedIndex = 0;
            chkQuesoExtra.Checked = chkPepperoni.Checked = chkChampinones.Checked = chkAceitunas.Checked = chkTocino.Checked = false;
            rdbRetiro.Checked = true; chkConvenioEstudiante.Checked = false;
            lblSubtotalValor.Text = lblDescuentoValor.Text = lblDeliveryValor.Text = lblIvaValor.Text = lblTotalValor.Text = "$ 0 CLP";
            grpResumen.Enabled = false; txtCliente.Focus();
            cargandoFormulario = false;
        }
        // Asocia textos explicativos a los botones (hover).
        private void ConfigurarAyuda()
        {
            void Vincular(Button b, string ayuda)
            {
                b.MouseEnter += (_, _) => MostrarEstado(ayuda);
                b.MouseLeave += (_, _) => MostrarEstado("Pase el mouse sobre un botón para ver ayuda.");
            }

            Vincular(btnCalcular, "Calcula los montos del pedido y permite grabarlo en la base de datos.");
            Vincular(btnLimpiar, "Limpia el formulario para ingresar un nuevo pedido.");
            Vincular(btnActualizarPedido, "Actualiza en la base de datos el pedido indicado por ID con los datos actuales.");
            Vincular(btnEliminarPedido, "Elimina de la base de datos el pedido indicado por ID (con confirmación).");
            Vincular(btnConfirmar, "Cierra la aplicación.");
            Vincular(btnProbarConexion, "Prueba la conexión con la base de datos y muestra su estado.");
            Vincular(btnBuscaPedidosXCliente, "Busca pedidos filtrando por el nombre del cliente.");
            Vincular(btnBuscarPedidoPorId, "Busca un pedido por su ID y muestra su detalle en el formulario.");
            Vincular(btnTodosLosPedidos, "Muestra todos los pedidos registrados.");
        }
        // Muestra un mensaje en el área inferior (ayuda/estado) sin reemplazar los MessageBox.
        private void MostrarEstado(string texto, bool? exito = null)
        {
            lblAyuda.Text = texto;
            lblAyuda.ForeColor = exito == true ? System.Drawing.Color.LightGreen
                : exito == false ? System.Drawing.Color.Salmon
                : System.Drawing.Color.FromArgb(230, 230, 230);
        }
        // Método público para recibir un cliente desde FormClientes
        public void CargarClienteDesdeSeleccion(string nombre, string telefono)
        {
            cargandoFormulario = true;
            txtCliente.Text = nombre;
            txtTelefono.Text = telefono;
            cargandoFormulario = false;

            MostrarEstado($"Cliente '{nombre}' cargado para el pedido.", true);
            txtCliente.Focus();
        }
    }
}
