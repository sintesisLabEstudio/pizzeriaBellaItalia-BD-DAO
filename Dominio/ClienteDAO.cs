using System;
using System.Data;
using Microsoft.Data.SqlClient; // SOLUCIÓN 1: Esto elimina todos los errores rojos de Sql.

namespace PizzeriaBellaItalia.Dominio
{
    public class ClienteDAO
    {
        // 1. Obtener lista de clientes para el DataGrid
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
        // 2. Buscar un cliente por Nombre y Teléfono (usado por PedidoDAO)
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
                    if (r.Read())
                    {
                        return new Cliente
                        {
                            IdCliente = Convert.ToInt32(r["IdCliente"]),
                            Nombre = r["Nombre"]?.ToString() ?? "",
                            Telefono = r["Telefono"]?.ToString() ?? ""
                        };
                    }
                }
            }
            return null;
        }
        // 3. Insertar cliente normal
        public int insertarCliente(Cliente c)
        {
            string query = @"INSERT INTO dbo.Clientes (Cliente, Telefono) 
                             VALUES (@Nombre, @Telefono);
                             SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(query, conexion))
            {
                cmd.Parameters.AddWithValue("@Nombre", c.Nombre);
                cmd.Parameters.AddWithValue("@Telefono", c.Telefono);

                // SOLUCIÓN 2: object? y validación de null
                object? resultado = cmd.ExecuteScalar();
                return resultado != null && resultado != DBNull.Value ? Convert.ToInt32(resultado) : 0;
            }
        }
        // 4. Insertar cliente con Transacción 
        public int InsertarClienteConTransaccion(Cliente c, SqlConnection conexion, SqlTransaction tran)
        {
            string query = @"INSERT INTO dbo.Clientes (Cliente, Telefono) 
                             VALUES (@Nombre, @Telefono);
                             SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlCommand cmd = new SqlCommand(query, conexion, tran))
            {
                cmd.Parameters.AddWithValue("@Nombre", c.Nombre);
                cmd.Parameters.AddWithValue("@Telefono", c.Telefono);

                // SOLUCIÓN 2: object? y validación de null
                object? resultado = cmd.ExecuteScalar();
                return resultado != null && resultado != DBNull.Value ? Convert.ToInt32(resultado) : 0;
            }
        }
        // 5. Obtener o Crear Cliente (usado en transacciones)
        public int ObtenerOCrearIdCliente(string nombre, string telefono, SqlConnection conexion, SqlTransaction tran)
        {
            string queryBuscar = "SELECT IdCliente FROM dbo.Clientes WHERE Cliente = @Nombre AND Telefono = @Telefono";
            using (SqlCommand cmd = new SqlCommand(queryBuscar, conexion, tran))
            {
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cmd.Parameters.AddWithValue("@Telefono", telefono);

                // SOLUCIÓN 2: object? y validación de null
                object? resultado = cmd.ExecuteScalar();
                if (resultado != null && resultado != DBNull.Value)
                {
                    return Convert.ToInt32(resultado);
                }
            }

            // Si no existe, lo inserta
            return InsertarClienteConTransaccion(new Cliente { Nombre = nombre, Telefono = telefono }, conexion, tran);
        }
        // 6. Actualizar cliente (CRUD)
        public bool ActualizarCliente(Cliente c)
        {
            string query = "UPDATE dbo.Clientes SET Cliente = @Nombre, Telefono = @Telefono WHERE IdCliente = @IdCliente";
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(query, conexion))
            {
                cmd.Parameters.AddWithValue("@IdCliente", c.IdCliente);
                cmd.Parameters.AddWithValue("@Nombre", c.Nombre);
                cmd.Parameters.AddWithValue("@Telefono", c.Telefono);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        // 7. Eliminar cliente (CRUD)
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
        // 8 para la eliminacion correcta de clientes, se valida si el cliente tiene pedidos asociados.
        public bool validaClienteSinPedidos(int idCliente)
        //Si tiene pedidos, no se permite eliminarlo.
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
        // 9. Buscar clientes por nombre o teléfono (DataTable)
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
    }
}