using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using PizzeriaBellaItalia.Dominio;
namespace PizzeriaBellaItalia.Presentacion
{
    // CRUD de clientes: grabar/actualizar, eliminar, buscar, listar y seleccionar para un pedido.
    public partial class FormClientes : Form
    {
        //DECLARA
        private readonly ClienteDAO clienteDao = new ClienteDAO(); //instancia ClienteDAO vacia
        private int idClienteSeleccionado = 0; // 0 = cliente nuevo; > 0 = cliente en edición.
        //ƒ
        public FormClientes()
        {
            Console.WriteLine("\n FormClientes.cs ...inicializando");
            InitializeComponent();
            // Estos dos eventos se suscriben aquí porque el diseñador no los define
            // (los demás botones y el Load se suscriben en FormClientes.Designer.cs).
            btnGrabar.Click += btnGrabar_Click;
            dgvClientes.CellDoubleClick += dgvClientes_CellDoubleClick;
        }
        //ƒ
        private void FormClientes_Load(object? sender, EventArgs e)
        {
            Console.WriteLine("FormClientes_Load: Cargando clientes...con funcion CargarClientes()");
            CargarClientes();
            Console.WriteLine("FormClientes_Load: Preparando nuevo cliente...con funcion PrepararNuevoCliente()");
            PrepararNuevoCliente();
        }
        private void btnGrabar_Click(object? sender, EventArgs e)
        {
            Console.WriteLine("CLICK EN BOTON GRABAR CLIENTE");
            Console.WriteLine("ejecutando validaciones no nulo txtNombre.Text  y txtTelefono.Text");
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtTelefono.Text))
            {
                MessageBox.Show("Por favor, ingrese el nombre y el teléfono del cliente.", "Datos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                Console.WriteLine("crea objeto cliente");
                Cliente cliente = new Cliente
                {
                    IdCliente = idClienteSeleccionado,
                    Nombre = txtNombre.Text.Trim(),
                    Telefono = txtTelefono.Text.Trim()
                };
                Console.WriteLine($"objeto cliente con datos {cliente.IdCliente} {cliente.Nombre} {cliente.Telefono}");
                //valida duplicado
                Console.WriteLine("validando duplicado con funcion clienteDao.BuscarPorNombreYTelefono(cliente.Nombre, cliente.Telefono) ");
                Cliente? duplicado = clienteDao.BuscarPorNombreYTelefono(cliente.Nombre, cliente.Telefono);
                //si no es dupicado
                if (duplicado != null && duplicado.IdCliente != idClienteSeleccionado)
                {
                    Console.WriteLine("si cliente duplicado lanza mensaje y sale");
                    MessageBox.Show(
                        $"Ya existe un cliente con el nombre '{cliente.Nombre}' y el teléfono '{cliente.Telefono}'.\nUse otro teléfono o edite el cliente existente.",
                        "Cliente duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTelefono.Focus();
                    return;
                }
                //inserta cliente validado no repetido
                if (idClienteSeleccionado == 0) //no hay cliente seleccionado
                {
                    int idGenerado = clienteDao.InsertarCliente(cliente); //si genera id entonces grabo nuevo cliente
                    if (idGenerado > 0)
                    {
                        Console.WriteLine("se valido el cliente ahora graba con clienteDao.InsertarCliente(cliente)");
                        MessageBox.Show("Cliente guardado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                //actualiza ciente
                {
                    Console.WriteLine("cliente repetido por actualizar con clienteDao.ActualizarCliente(cliente)");
                    bool actualizado = clienteDao.ActualizarCliente(cliente);
                    if (actualizado)
                    {
                        MessageBox.Show("Cliente actualizado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                Console.WriteLine("seguimos en FormCliente.cs preparando cliente con funcion PrepararNuevoCliente()");
                //
                PrepararNuevoCliente();
                Console.WriteLine("seguimos en FormCliente.cs preparando cliente con funcion CargarClientes()");
                CargarClientes();
            }
            catch (SqlException ex) when (ClienteDAO.EsClienteDuplicado(ex))
            {
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
            Console.WriteLine("\n en FormClientes click en boton elimina");
            //valida si no hay seleccion elimina
            if (dgvClientes.CurrentRow == null || dgvClientes.CurrentRow.IsNewRow)
            {
                MessageBox.Show("Por favor, seleccione un cliente de la lista para eliminar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DataGridViewRow filaSeleccionada = dgvClientes.CurrentRow;
            Console.WriteLine($"\n en FormClientes selecciono:{dgvClientes.CurrentRow} ");
            //obtiene valores de los campos de la fila seleccionada
            int idCliente = Convert.ToInt32(filaSeleccionada.Cells["IdCliente"].Value ?? 0);
            string nombreCliente = filaSeleccionada.Cells["Nombre"].Value?.ToString() ?? "Sin nombre";
            Console.WriteLine($"\n con los valores seleccionados idcliente: {idCliente} nombrecliente: {nombreCliente}");
            //valida cliente sin pedidos
            Console.WriteLine($"\n valida clientes sin pedidos para eliminar con funcionclienteDao.ValidaClienteSinPedidos({idCliente}) ");
            if (!clienteDao.ValidaClienteSinPedidos(idCliente))
            {
                MessageBox.Show($"No es posible eliminar al cliente '{nombreCliente}' porque tiene pedidos registrados en el sistema.",
                                "Acción Denegada", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            DialogResult respuesta = MessageBox.Show($"¿Está seguro que desea eliminar al cliente '{nombreCliente}'?",
                                                     "Confirmar Eliminación",
                                                     MessageBoxButtons.YesNo,
                                                     MessageBoxIcon.Question);
            //en basa a la respuest elimina
            if (respuesta == DialogResult.Yes)
            {
                try
                {
                    Console.WriteLine($"\n desde FORM salta a clienteDao.EliminarCliente({idCliente})");
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
            // Envía el cliente indicado en los cuadros de texto al formulario de pedido.
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtTelefono.Text))
            {
                MessageBox.Show("Debe ingresar o cargar el nombre y teléfono del cliente antes de seleccionarlo.",
                                "Datos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return;
            }
            string nombre = txtNombre.Text.Trim();
            string telefono = txtTelefono.Text.Trim();
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
            btnGrabar.Text = "Grabar";
            txtNombre.Focus();
        }
        private void PresentaCamposParaEdicion()
        {
            // Pasa la fila seleccionada a los cuadros de texto y deja el formulario en modo "Actualizar".
            if (dgvClientes.CurrentRow == null || dgvClientes.CurrentRow.IsNewRow) return;
            DataGridViewRow fila = dgvClientes.CurrentRow;
            idClienteSeleccionado = Convert.ToInt32(fila.Cells["IdCliente"].Value ?? 0);
            txtNombre.Text = fila.Cells["Nombre"].Value?.ToString() ?? "";
            txtTelefono.Text = fila.Cells["Telefono"].Value?.ToString() ?? "";
            btnGrabar.Text = "Actualizar";
            txtNombre.Focus();
        }
        private void dgvClientes_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                PresentaCamposParaEdicion();
            }
        }
    }
}
