using System;
using System.Data;
using Microsoft.Data.SqlClient;
using PizzeriaBellaItalia.Dominio;

// Arnés de prueba de los DAO contra la base PizzeriaDB.
// Es independiente de los datos que haya:
//   - al empezar borra los datos de pruebas anteriores (prefijo 'PRUEBA')
//   - crea sus propios clientes de prueba (prefijo 'PRUEBA-') y los borra al final
//   - respeta los 2 clientes que deja testInsertarCliente() ('CLIENTE DE PRUEBA')
class Program
{
    static int fallos = 0;
    const string MARCA = "PRUEBA-";

    static void Check(string titulo, bool ok, string detalle = "")
    {
        Console.WriteLine((ok ? "  [OK]   " : "  [FALLA] ") + titulo + (detalle != "" ? " -> " + detalle : ""));
        if (!ok) fallos++;
    }

    static int Main()
    {
        ClienteDAO clienteDao = new ClienteDAO();
        PedidoDAO pedidoDao = new PedidoDAO();

        // 0) limpieza de pruebas anteriores, para partir siempre del mismo estado
        LimpiarDatosPrueba("%PRUEBA%");

        int pedidosAntes = Contar("SELECT COUNT(*) FROM dbo.Pedidos");
        int clientesAntes = Contar("SELECT COUNT(*) FROM dbo.Clientes");
        Console.WriteLine($"Estado inicial: {pedidosAntes} pedidos, {clientesAntes} clientes\n");

        Console.WriteLine("== 0) ClienteDAO: testInsertarCliente() ==");
        string salidaPrueba = clienteDao.testInsertarCliente();
        Check("testInsertarCliente() informa OK", salidaPrueba.StartsWith("[OK]"), salidaPrueba);
        Check("el cliente de prueba 1 quedo guardado",
              Contar("SELECT COUNT(*) FROM dbo.Clientes WHERE Cliente = 'CLIENTE DE PRUEBA 1'") == 1);
        Check("el cliente de prueba 2 quedo guardado",
              Contar("SELECT COUNT(*) FROM dbo.Clientes WHERE Cliente = 'CLIENTE DE PRUEBA 2'") == 1);

        Console.WriteLine("== 1) ClienteDAO: insertarCliente + buscar por nombre y telefono ==");
        string nombreBase = MARCA + "base-" + DateTime.Now.Ticks;
        Cliente clienteBase = new Cliente { Nombre = nombreBase, Telefono = "000" };
        int idBase = clienteDao.insertarCliente(clienteBase);
        Check("insertarCliente devuelve un IdCliente > 0", idBase > 0, "IdCliente=" + idBase);

        Cliente? encontrado = clienteDao.BuscarPorNombreYTelefono(nombreBase, "000");
        Check("el cliente insertado existe", encontrado != null);
        Check("IdCliente coincide", encontrado?.IdCliente == idBase, encontrado?.IdCliente.ToString() ?? "null");
        Check("Nombre mapeado", encontrado?.Nombre == nombreBase, encontrado?.Nombre ?? "null");
        Check("Telefono mapeado", encontrado?.Telefono == "000", encontrado?.Telefono ?? "null");
        Check("cliente inexistente devuelve null", clienteDao.BuscarPorNombreYTelefono("no existe", "000") == null);

        Console.WriteLine("== 2) ClienteDAO: listado y pedidos por cliente ==");
        DataTable clientes = clienteDao.ObtenerClientesDataTable();
        Check("lista de clientes trae filas", clientes.Rows.Count == clientesAntes + 3,
              clientes.Rows.Count + " clientes (2 de testInsertarCliente + 1 base)");
        DataTable pedidosCliente = clienteDao.ObtenerPedidosPorCliente(idBase);
        Check("pedidos del cliente de prueba", pedidosCliente.Rows.Count == 0, pedidosCliente.Rows.Count + " pedidos");
        Cliente? porId = clienteDao.ObtenerClientePorId(idBase);
        Check("cliente por IdCliente", porId?.Nombre == nombreBase, porId?.Nombre ?? "null");

        Console.WriteLine("== 3) PedidoDAO: INSERT de un pedido con cliente nuevo ==");
        PedidoPizza nuevo = new PedidoPizza
        {
            Cliente = MARCA + "cliente-" + DateTime.Now.Ticks,
            Telefono = "9999999",
            TamanoNombre = "Mediana",
            PrecioBaseTamano = 8500,
            MasaNombre = "Masa Tradicional",
            RecargoMasa = 0,
            SubtotalPizza = 8500,
            NetoAfecto = 8500,
            MontoIva = 1615,
            TotalPagar = 10115
        };
        bool insertado = pedidoDao.InsertarPedido(nuevo);
        Check("InsertarPedido devuelve true", insertado);
        Check("el DAO resolvio el IdCliente", nuevo.IdCliente > 0, "IdCliente=" + nuevo.IdCliente);
        Check("el DAO devolvio el IdPedido generado", nuevo.IdPedido > 0, "IdPedido=" + nuevo.IdPedido);

        Cliente? creado = clienteDao.BuscarPorNombreYTelefono(nuevo.Cliente, "9999999");
        Check("el cliente se registro en dbo.Clientes", creado != null);
        Check("el IdCliente del pedido es el del cliente", creado?.IdCliente == nuevo.IdCliente);

        Console.WriteLine("== 4) PedidoDAO: SELECTs ==");
        DataTable listado = pedidoDao.ObtenerPedidosDataTable();
        Check("listado de pedidos trae filas", listado.Rows.Count == pedidosAntes + 1, listado.Rows.Count + " pedidos");
        Check("el listado trae IdCliente", listado.Columns.Contains("IdCliente"));
        Check("el listado trae Cliente", listado.Columns.Contains("Cliente"));
        DataTable busqueda = pedidoDao.BuscarPedidosPorCliente(MARCA);
        Check("busqueda por cliente de prueba", busqueda.Rows.Count == 1, busqueda.Rows.Count + " filas");
        PedidoPizza? leido = pedidoDao.ObtenerPedidoPorId(nuevo.IdPedido);
        Check("ObtenerPedidoPorId devuelve el pedido", leido != null);
        Check("el pedido leido trae IdCliente", leido?.IdCliente == nuevo.IdCliente, leido?.IdCliente.ToString() ?? "null");
        Check("el pedido leido trae Cliente", leido?.Cliente == nuevo.Cliente, leido?.Cliente ?? "null");
        Check("el pedido leido trae Telefono", leido?.Telefono == "9999999", leido?.Telefono ?? "null");

        Console.WriteLine("== 5) PedidoDAO: UPDATE cambiando el nombre del cliente ==");
        PedidoPizza? paraActualizar = pedidoDao.ObtenerPedidoPorId(nuevo.IdPedido);
        paraActualizar!.Cliente = MARCA + "cliente-editado-" + DateTime.Now.Ticks;
        paraActualizar.Telefono = "8888888";
        bool actualizado = pedidoDao.ActualizarPedido(paraActualizar);
        Check("ActualizarPedido devuelve true", actualizado);
        Cliente? clienteNuevo = clienteDao.BuscarPorNombreYTelefono(paraActualizar.Cliente, "8888888");
        Check("se creo el cliente nuevo", clienteNuevo != null);
        Check("el IdCliente del pedido se actualizo", clienteNuevo?.IdCliente == paraActualizar.IdCliente,
              "pedido=" + paraActualizar.IdCliente + " cliente=" + clienteNuevo?.IdCliente);
        PedidoPizza? releido = pedidoDao.ObtenerPedidoPorId(nuevo.IdPedido);
        Check("el pedido apunta al cliente nuevo", releido?.IdCliente == clienteNuevo?.IdCliente);

        Console.WriteLine("== 6) ClienteDAO: DELETE de un cliente con pedidos (FK) ==");
        Check("no se puede borrar un cliente con pedidos", clienteDao.EliminarCliente(clienteNuevo!.IdCliente) == false);

        Console.WriteLine("== 7) Limpieza de los datos del arnes ==");
        // se borran los datos creados por el arnés (prefijo 'PRUEBA-'),
        // pero se respetan los 2 clientes que deja testInsertarCliente()
        LimpiarDatosPrueba("%" + MARCA + "%");
        Check("los pedidos quedaron como estaban", Contar("SELECT COUNT(*) FROM dbo.Pedidos") == pedidosAntes,
              Contar("SELECT COUNT(*) FROM dbo.Pedidos") + " pedidos");
        Check("solo quedan los 2 clientes de testInsertarCliente",
              Contar("SELECT COUNT(*) FROM dbo.Clientes") == clientesAntes + 2,
              Contar("SELECT COUNT(*) FROM dbo.Clientes") + " clientes");
        Check("no hay pedidos huerfanos", Contar("SELECT COUNT(*) FROM dbo.Pedidos p LEFT JOIN dbo.Clientes c ON c.IdCliente = p.IdCliente WHERE c.IdCliente IS NULL") == 0);
        Check("no quedo ningun cliente del arnes", Contar("SELECT COUNT(*) FROM dbo.Clientes WHERE Cliente LIKE '" + MARCA + "%'") == 0);

        Console.WriteLine();
        Console.WriteLine(fallos == 0 ? "RESULTADO: todas las comprobaciones OK" : "RESULTADO: " + fallos + " comprobacion(es) fallaron");
        return fallos == 0 ? 0 : 1;
    }

    // Borra pedidos y clientes cuyo nombre coincide con el patron recibido.
    static void LimpiarDatosPrueba(string patron)
    {
        using (SqlConnection c = ConexionBD.ObtenerConexion())
        {
            using (SqlCommand cmd = new SqlCommand(
                "DELETE FROM dbo.Pedidos WHERE Cliente LIKE @patron; DELETE FROM dbo.Clientes WHERE Cliente LIKE @patron;",
                c))
            {
                cmd.Parameters.AddWithValue("@patron", patron);
                cmd.ExecuteNonQuery();
            }
        }
    }

    static int Contar(string query)
    {
        using (SqlConnection c = ConexionBD.ObtenerConexion())
        using (SqlCommand cmd = new SqlCommand(query, c))
            return Convert.ToInt32(cmd.ExecuteScalar());
    }
}
