using System;
using System.Data;
using Microsoft.Data.SqlClient;   // Para identificar la violación de UQ_Clientes_Cliente_Telefono.
using System.Windows.Forms;
using PizzeriaBellaItalia.Dominio;

namespace PizzeriaBellaItalia.Presentacion
{
    public partial class FormClientes : Form
    {
        // ---------------- atributos -----------------------
        private readonly ClienteDAO clienteDao = new ClienteDAO();
        private int idClienteSeleccionado = 0;
        public Cliente? ClienteSeleccionado { get; private set; }
        // ----------------------- form ---------------------------
        public FormClientes()
        {
            InitializeComponent();
            btnGrabar.Click += btnGrabar_Click;
            dgvClientes.CellDoubleClick += dgvClientes_CellDoubleClick;
        }
        private void FormClientes_Load(object? sender, EventArgs e)
        {
            CargarClientes();
            PrepararNuevoCliente();
        }
        //--------------------- botones -------------------------
        private void btnGrabar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtTelefono.Text))
            {
                MessageBox.Show("Por favor, ingrese el nombre y el teléfono del cliente.", "Datos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                Cliente cliente = new Cliente
                {
                    IdCliente = idClienteSeleccionado,
                    Nombre = txtNombre.Text.Trim(),
                    Telefono = txtTelefono.Text.Trim()
                };

                // La BD exige UQ_Clientes_Cliente_Telefono: se valida antes de grabar
                // para mostrar un mensaje claro en lugar de un error de SQL Server.
                Cliente? duplicado = clienteDao.BuscarPorNombreYTelefono(cliente.Nombre, cliente.Telefono);
                if (duplicado != null && duplicado.IdCliente != idClienteSeleccionado)
                {
                    MessageBox.Show(
                        $"Ya existe un cliente con el nombre '{cliente.Nombre}' y el teléfono '{cliente.Telefono}'.\nUse otro teléfono o edite el cliente existente.",
                        "Cliente duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTelefono.Focus();
                    return;
                }

                if (idClienteSeleccionado == 0)
                {
                    // Modo Insertar (Cliente nuevo)
                    int idGenerado = clienteDao.insertarCliente(cliente);
                    if (idGenerado > 0)
                    {
                        MessageBox.Show("Cliente guardado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    // Modo Actualizar (Cliente existente)
                    bool actualizado = clienteDao.ActualizarCliente(cliente);
                    if (actualizado)
                    {
                        MessageBox.Show("Cliente actualizado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                PrepararNuevoCliente();
                CargarClientes();
            }
            catch (SqlException ex) when (ClienteDAO.EsClienteDuplicado(ex))
            {
                // Respaldo por si otra ventana crea el mismo cliente entre la validación y el INSERT.
                MessageBox.Show("Ya existe un cliente con ese nombre y teléfono. No se guardó el cambio.",
                                "Cliente duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar el cliente:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnElimina_Click(object? sender, EventArgs e)
        {
            // Asegurar que hay una fila seleccionada y que no es una fila nueva
            if (dgvClientes.CurrentRow == null || dgvClientes.CurrentRow.IsNewRow)
            {
                MessageBox.Show("Por favor, seleccione un cliente de la lista para eliminar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // filaSeleccionada contiene los campos de la fila actual del DataGridView
            DataGridViewRow filaSeleccionada = dgvClientes.CurrentRow;
            int idCliente = Convert.ToInt32(filaSeleccionada.Cells["IdCliente"].Value ?? 0);
            string nombreCliente = filaSeleccionada.Cells["Nombre"].Value?.ToString() ?? "Sin nombre";

            // Validación: no eliminar si el cliente tiene pedidos asociados
            if (!clienteDao.validaClienteSinPedidos(idCliente))
            {
                MessageBox.Show($"No es posible eliminar al cliente '{nombreCliente}' porque tiene pedidos registrados en el sistema.",
                                "Acción Denegada", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            // Confirmación de eliminación
            DialogResult respuesta = MessageBox.Show($"¿Está seguro que desea eliminar al cliente '{nombreCliente}'?",
                                                     "Confirmar Eliminación",
                                                     MessageBoxButtons.YesNo,
                                                     MessageBoxIcon.Question);

            // Si el usuario confirma, se procede a eliminar
            if (respuesta == DialogResult.Yes)
            {
                try
                {
                    if (clienteDao.EliminarCliente(idCliente))
                    {
                        MessageBox.Show("Cliente eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        PrepararNuevoCliente();
                        CargarClientes();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al intentar eliminar:\n" + ex.Message, "Error de BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnListar_Click(object? sender, EventArgs e)
        {
            PrepararNuevoCliente();
            CargarClientes();
        }
        private void btnNuevo_Click(object? sender, EventArgs e)
        {
            PrepararNuevoCliente();
        }
        private void btnSelecciona_Click(object? sender, EventArgs e)
        {
            // 1. Validar que los campos del formulario tengan datos
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtTelefono.Text))
            {
                MessageBox.Show("Debe ingresar o cargar el nombre y teléfono del cliente antes de seleccionarlo.",
                                "Datos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return;
            }

            // 2. Tomar los valores directamente desde los campos del formulario
            string nombre = txtNombre.Text.Trim();
            string telefono = txtTelefono.Text.Trim();

            // 3. Buscar si FormPedido ya está abierto
            FormPedido? frmPedido = Application.OpenForms.OfType<FormPedido>().FirstOrDefault();

            if (frmPedido != null)
            {
                frmPedido.CargarClienteDesdeSeleccion(nombre, telefono);
                frmPedido.BringToFront();
                this.Close();
            }
            else
            {
                FormPedido nuevoPedido = new FormPedido();
                nuevoPedido.Show();
                nuevoPedido.CargarClienteDesdeSeleccion(nombre, telefono);

                if (Application.OpenForms[0] == this)
                {
                    nuevoPedido.FormClosed += (s, args) => this.Close();
                    this.Hide();
                }
                else
                {
                    this.Close();
                }
            }
        }
        private void btnBuscar_Click(object? sender, EventArgs e)
        {
            string criterio = !string.IsNullOrWhiteSpace(txtNombre.Text)
                              ? txtNombre.Text.Trim()
                              : txtTelefono.Text.Trim();

            CargarClientes(criterio);
        }
        //------------------- tools --------------------------
        private void CargarClientes(string criterio = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(criterio))
                    dgvClientes.DataSource = clienteDao.ObtenerClientesDataTable();
                else
                    dgvClientes.DataSource = clienteDao.BuscarClientesDataTable(criterio);

                dgvClientes.AllowUserToAddRows = false;
                dgvClientes.ReadOnly = true;
                dgvClientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                var colNombre = dgvClientes.Columns["Nombre"];
                if (colNombre != null) colNombre.HeaderText = "Nombre Cliente";

                var colTelefono = dgvClientes.Columns["Telefono"];
                if (colTelefono != null) colTelefono.HeaderText = "Teléfono actual";
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar los clientes.\n" + ex.Message, "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void PrepararNuevoCliente()
        {
            txtNombre.Clear();
            txtTelefono.Clear();
            idClienteSeleccionado = 0;
            // Cambiamos el botón a modo inserción
            btnGrabar.Text = "Grabar";
            txtNombre.Focus();
        }
        private void presentaCamposParaEdicion()
        {
            if (dgvClientes.CurrentRow == null || dgvClientes.CurrentRow.IsNewRow) return;

            DataGridViewRow fila = dgvClientes.CurrentRow;

            // Guardamos el ID oculto para saber qué registro actualizar después
            idClienteSeleccionado = Convert.ToInt32(fila.Cells["IdCliente"].Value ?? 0);

            // Pasamos los valores a las cajas de texto
            txtNombre.Text = fila.Cells["Nombre"].Value?.ToString() ?? "";
            txtTelefono.Text = fila.Cells["Telefono"].Value?.ToString() ?? "";

            // Cambiamos el botón a modo edición
            btnGrabar.Text = "Actualizar";
            txtNombre.Focus();
        }
        // ----------------------- grid --------------------
        private void dgvClientes_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            // Valida que no haya sido en la fila de los encabezados (RowIndex < 0)
            if (e.RowIndex >= 0)
            {
                presentaCamposParaEdicion();
            }
        }
    }
}