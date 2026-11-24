using System;
using System.Data;
using Microsoft.Data.SqlClient;
namespace PizzeriaBellaItalia.Dominio
{
    // DAO de dbo.Clientes: ABM, búsquedas y consultas de apoyo.
    public class ClienteDAO
    {
        // dbo.Clientes tiene la restricción UQ_Clientes_Cliente_Telefono (UNIQUE en Cliente + Telefono),
        // creada por 01_Estructura_PizzeriaDB.sql. No pueden existir dos clientes con el mismo nombre
        // y teléfono: SQL Server rechaza la operación con SqlException 2601/2627.
        // La UI detecta ese error con EsClienteDuplicado y muestra un MessageBox comprensible
        // en lugar del mensaje técnico de SQL Server.
        public static bool EsClienteDuplicado(SqlException ex) => ex.Number == 2601 || ex.Number == 2627;
        // Lista de clientes para el DataGrid.
        public DataTable ObtenerClientesDataTable()
        {
            DataTable dt = new DataTable();
            string query = "SELECT IdCliente, Cliente AS Nombre, Telefono FROM dbo.Clientes ORDER BY IdCliente DESC";
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            using (SqlDataAdapter adapter = new SqlDataAdapter(query, conexion))
            {
                adapter.Fill(dt);
            }
            return dt;
        }
        // Busca un cliente por nombre y teléfono (usado por FormClientes para detectar duplicados).
        public Cliente? BuscarPorNombreYTelefono(string nombre, string telefono)
        {
            string query = "SELECT IdCliente, Cliente AS Nombre, Telefono FROM dbo.Clientes WHERE Cliente = @Nombre AND Telefono = @Telefono";
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(query, conexion))
            {
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cmd.Parameters.AddWithValue("@Telefono", telefono);
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    return r.Read() ? LeerCliente(r) : null;
                }
            }
        }
        // Inserta un cliente y devuelve su IdCliente (0 si no se pudo obtener).
        public int InsertarCliente(Cliente c)
        {
            string query = @"INSERT INTO dbo.Clientes (Cliente, Telefono)
                             VALUES (@Nombre, @Telefono);
                             SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(query, conexion))
            {
                cmd.Parameters.AddWithValue("@Nombre", c.Nombre);
                cmd.Parameters.AddWithValue("@Telefono", c.Telefono);
                object? resultado = cmd.ExecuteScalar();
                return resultado != null && resultado != DBNull.Value ? Convert.ToInt32(resultado) : 0;
            }
        }
        // Inserta un cliente dentro de una transacción ya abierta (la conexión la administra el llamador).
        public int InsertarClienteConTransaccion(Cliente c, SqlConnection conexion, SqlTransaction tran)
        {
            string query = @"INSERT INTO dbo.Clientes (Cliente, Telefono)
                             VALUES (@Nombre, @Telefono);
                             SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using (SqlCommand cmd = new SqlCommand(query, conexion, tran))
            {
                cmd.Parameters.AddWithValue("@Nombre", c.Nombre);
                cmd.Parameters.AddWithValue("@Telefono", c.Telefono);
                object? resultado = cmd.ExecuteScalar();
                return resultado != null && resultado != DBNull.Value ? Convert.ToInt32(resultado) : 0;
            }
        }
        // Obtiene el IdCliente de nombre + teléfono o lo crea si no existe.
        // Se ejecuta dentro de la transacción del llamador (lo usa PedidoDAO al grabar o actualizar un pedido).
        public int ObtenerOCrearIdCliente(string nombre, string telefono, SqlConnection conexion, SqlTransaction tran)
        {
            string queryBuscar = "SELECT IdCliente FROM dbo.Clientes WHERE Cliente = @Nombre AND Telefono = @Telefono";
            using (SqlCommand cmd = new SqlCommand(queryBuscar, conexion, tran))
            {
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cmd.Parameters.AddWithValue("@Telefono", telefono);
                object? resultado = cmd.ExecuteScalar();
                if (resultado != null && resultado != DBNull.Value)
                {
                    return Convert.ToInt32(resultado);
                }
            }
            // Si no existe, lo inserta en la misma transacción.
            return InsertarClienteConTransaccion(new Cliente { Nombre = nombre, Telefono = telefono }, conexion, tran);
        }
        // Actualiza un cliente. dbo.Pedidos guarda una copia del nombre y teléfono, por lo que
        // ambas tablas se actualizan en una sola transacción para que no queden datos distintos.
        public bool ActualizarCliente(Cliente c)
        {
            string queryCliente = "UPDATE dbo.Clientes SET Cliente = @Nombre, Telefono = @Telefono WHERE IdCliente = @IdCliente";
            string queryPedidos = "UPDATE dbo.Pedidos SET Cliente = @Nombre, Telefono = @Telefono WHERE IdCliente = @IdCliente";
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            using (SqlTransaction tran = conexion.BeginTransaction())
            {
                try
                {
                    bool actualizado;
                    using (SqlCommand cmd = new SqlCommand(queryCliente, conexion, tran))
                    {
                        AgregarParametrosActualizacion(cmd, c);
                        actualizado = cmd.ExecuteNonQuery() > 0;
                    }
                    if (actualizado)
                    {
                        using (SqlCommand cmdPedidos = new SqlCommand(queryPedidos, conexion, tran))
                        {
                            AgregarParametrosActualizacion(cmdPedidos, c);
                            cmdPedidos.ExecuteNonQuery();
                        }
                    }
                    tran.Commit();
                    return actualizado;
                }
                catch
                {
                    Revertir(tran);
                    throw;
                }
            }
        }
        // Elimina un cliente (la FK impide borrar clientes con pedidos; la UI lo valida antes).
        public bool EliminarCliente(int id)
        {
            string query = "DELETE FROM dbo.Clientes WHERE IdCliente = @IdCliente";
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(query, conexion))
            {
                cmd.Parameters.AddWithValue("@IdCliente", id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        // Para eliminar un cliente se valida que no tenga pedidos asociados.
        public bool ValidaClienteSinPedidos(int idCliente)
        {
            string query = "SELECT COUNT(*) FROM dbo.Pedidos WHERE IdCliente = @IdCliente";
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(query, conexion))
            {
                cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                object? resultado = cmd.ExecuteScalar();
                int cantidadPedidos = resultado != null && resultado != DBNull.Value ? Convert.ToInt32(resultado) : 0;
                return cantidadPedidos == 0;
            }
        }
        // Busca clientes por nombre o teléfono (LIKE).
        public DataTable BuscarClientesDataTable(string criterio)
        {
            DataTable tabla = new DataTable();
            string query = "SELECT IdCliente, Cliente AS Nombre, Telefono FROM dbo.Clientes WHERE Cliente LIKE @Criterio OR Telefono LIKE @Criterio ORDER BY IdCliente DESC";
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(query, conexion))
            {
                cmd.Parameters.AddWithValue("@Criterio", "%" + criterio + "%");
                using (SqlDataAdapter adaptador = new SqlDataAdapter(cmd))
                {
                    adaptador.Fill(tabla);
                }
            }
            return tabla;
        }
        // Pedidos asociados a un cliente (utilitario de consulta/pruebas).
        public DataTable ObtenerPedidosPorCliente(int idCliente)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT IdPedido, IdCliente, Cliente, Telefono, TamanoNombre, TotalPagar
                             FROM dbo.Pedidos WHERE IdCliente = @IdCliente ORDER BY IdPedido DESC";
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(query, conexion))
            {
                cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }
            return dt;
        }
        // Cliente por su IdCliente (utilitario de consulta/pruebas).
        public Cliente? ObtenerClientePorId(int idCliente)
        {
            string query = "SELECT IdCliente, Cliente AS Nombre, Telefono FROM dbo.Clientes WHERE IdCliente = @IdCliente";
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(query, conexion))
            {
                cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    return r.Read() ? LeerCliente(r) : null;
                }
            }
        }
        // ----------------------- Apoyo interno -----------------------
        // Convierte la fila actual del reader (IdCliente, Nombre, Telefono) en un Cliente.
        private static Cliente LeerCliente(SqlDataReader r) => new Cliente
        {
            IdCliente = Convert.ToInt32(r["IdCliente"]),
            Nombre = r["Nombre"]?.ToString() ?? "",
            Telefono = r["Telefono"]?.ToString() ?? ""
        };
        private static void AgregarParametrosActualizacion(SqlCommand cmd, Cliente c)
        {
            cmd.Parameters.AddWithValue("@IdCliente", c.IdCliente);
            cmd.Parameters.AddWithValue("@Nombre", c.Nombre);
            cmd.Parameters.AddWithValue("@Telefono", c.Telefono);
        }
        // Rollback tolerante: si la transacción ya terminó o la conexión se perdió, no oculta el error original.
        private static void Revertir(SqlTransaction tran)
        {
            try { tran.Rollback(); }
            catch (Exception) { /* la transacción ya no está activa */ }
        }
    }
}
