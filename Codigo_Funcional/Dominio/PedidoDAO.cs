using System;
using System.Data;
using Microsoft.Data.SqlClient;
namespace PizzeriaBellaItalia.Dominio
{
    // DAO de dbo.Pedidos: CRUD con consultas parametrizadas.
    public class PedidoDAO
    {
        // Inserta un pedido. Resolver (u obtener) el cliente y grabar el pedido ocurren en UNA sola
        // transacción: si algo falla, no queda un cliente huérfano creado a medias.
        public bool InsertarPedido(PedidoPizza p)
        {
            string query = @"INSERT INTO dbo.Pedidos
                (IdCliente,Cliente,Telefono,TamanoNombre,PrecioBaseTamano,MasaNombre,RecargoMasa,
                 QuesoExtra,Pepperoni,Champinones,Aceitunas,Tocino,EsDelivery,AplicaConvenioEstudiante,
                 CostoIngredientes,SubtotalPizza,MontoDescuento,CostoDelivery,NetoAfecto,MontoIva,TotalPagar)
                VALUES
                (@IdCliente,@Cliente,@Telefono,@TamanoNombre,@PrecioBaseTamano,@MasaNombre,@RecargoMasa,
                 @QuesoExtra,@Pepperoni,@Champinones,@Aceitunas,@Tocino,@EsDelivery,@AplicaConvenioEstudiante,
                 @CostoIngredientes,@SubtotalPizza,@MontoDescuento,@CostoDelivery,@NetoAfecto,@MontoIva,@TotalPagar);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            using (SqlTransaction tran = conexion.BeginTransaction())
            {
                try
                {
                    p.IdCliente = ResolverIdCliente(p.Cliente, p.Telefono, conexion, tran);

                    using (SqlCommand cmd = new SqlCommand(query, conexion, tran))
                    {
                        CargarParametros(cmd, p);
                        object? resultado = cmd.ExecuteScalar();
                        bool exito = resultado != null && resultado != DBNull.Value;
                        if (exito)
                        {
                            p.IdPedido = Convert.ToInt32(resultado);
                            tran.Commit();
                        }
                        else
                        {
                            Revertir(tran);
                        }
                        return exito;
                    }
                }
                catch
                {
                    Revertir(tran);
                    throw;
                }
            }
        }
        public DataTable ObtenerPedidosDataTable()
        {
            DataTable dt = new DataTable();
            string query = @"SELECT p.IdPedido, p.IdCliente, c.Cliente, c.Telefono, p.TamanoNombre, p.TotalPagar
                             FROM dbo.Pedidos p
                             INNER JOIN dbo.Clientes c ON c.IdCliente = p.IdCliente
                             ORDER BY p.IdPedido DESC";
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            using (SqlDataAdapter adapter = new SqlDataAdapter(query, conexion))
            {
                adapter.Fill(dt);
            }
            return dt;
        }
        public DataTable BuscarPedidosPorCliente(string filtroCliente)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT p.IdPedido, p.IdCliente, c.Cliente, c.Telefono, p.TamanoNombre, p.TotalPagar
                             FROM dbo.Pedidos p
                             INNER JOIN dbo.Clientes c ON c.IdCliente = p.IdCliente
                             WHERE c.Cliente LIKE @Filtro
                             ORDER BY p.IdPedido DESC";
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(query, conexion))
            {
                cmd.Parameters.AddWithValue("@Filtro", "%" + filtroCliente + "%");
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }
            return dt;
        }
        public PedidoPizza? ObtenerPedidoPorId(int id)
        {
            string query = @"SELECT p.IdPedido, p.IdCliente, c.Cliente, c.Telefono, p.TamanoNombre, p.PrecioBaseTamano, p.MasaNombre, p.RecargoMasa,
                                    p.QuesoExtra, p.Pepperoni, p.Champinones, p.Aceitunas, p.Tocino, p.EsDelivery, p.AplicaConvenioEstudiante,
                                    p.CostoIngredientes, p.SubtotalPizza, p.MontoDescuento, p.CostoDelivery, p.NetoAfecto, p.MontoIva, p.TotalPagar
                             FROM dbo.Pedidos p
                             INNER JOIN dbo.Clientes c ON c.IdCliente = p.IdCliente
                             WHERE p.IdPedido = @IdPedido";
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(query, conexion))
            {
                cmd.Parameters.AddWithValue("@IdPedido", id);
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null;
                    return new PedidoPizza
                    {
                        IdPedido = Convert.ToInt32(r["IdPedido"]),
                        IdCliente = Convert.ToInt32(r["IdCliente"]),
                        Cliente = r["Cliente"]?.ToString() ?? "",
                        Telefono = r["Telefono"]?.ToString() ?? "",
                        TamanoNombre = r["TamanoNombre"]?.ToString() ?? "",
                        PrecioBaseTamano = Convert.ToInt32(r["PrecioBaseTamano"]),
                        MasaNombre = r["MasaNombre"]?.ToString() ?? "",
                        RecargoMasa = Convert.ToInt32(r["RecargoMasa"]),
                        QuesoExtra = Convert.ToBoolean(r["QuesoExtra"]),
                        Pepperoni = Convert.ToBoolean(r["Pepperoni"]),
                        Champinones = Convert.ToBoolean(r["Champinones"]),
                        Aceitunas = Convert.ToBoolean(r["Aceitunas"]),
                        Tocino = Convert.ToBoolean(r["Tocino"]),
                        EsDelivery = Convert.ToBoolean(r["EsDelivery"]),
                        AplicaConvenioEstudiante = Convert.ToBoolean(r["AplicaConvenioEstudiante"]),
                        CostoIngredientes = Convert.ToInt32(r["CostoIngredientes"]),
                        SubtotalPizza = Convert.ToInt32(r["SubtotalPizza"]),
                        MontoDescuento = Convert.ToInt32(r["MontoDescuento"]),
                        CostoDelivery = Convert.ToInt32(r["CostoDelivery"]),
                        NetoAfecto = Convert.ToInt32(r["NetoAfecto"]),
                        MontoIva = Convert.ToInt32(r["MontoIva"]),
                        TotalPagar = Convert.ToInt32(r["TotalPagar"])
                    };
                }
            }
        }
        // Actualiza un pedido. Si cambia el nombre o el teléfono, el pedido pasa a pertenecer al cliente
        // que corresponda (se crea si no existe). Todo en una sola transacción.
        public bool ActualizarPedido(PedidoPizza p)
        {
            string query = @"UPDATE dbo.Pedidos SET
                                IdCliente=@IdCliente,Cliente=@Cliente,Telefono=@Telefono,TamanoNombre=@TamanoNombre,PrecioBaseTamano=@PrecioBaseTamano,
                                MasaNombre=@MasaNombre,RecargoMasa=@RecargoMasa,QuesoExtra=@QuesoExtra,Pepperoni=@Pepperoni,
                                Champinones=@Champinones,Aceitunas=@Aceitunas,Tocino=@Tocino,EsDelivery=@EsDelivery,
                                AplicaConvenioEstudiante=@AplicaConvenioEstudiante,CostoIngredientes=@CostoIngredientes,
                                SubtotalPizza=@SubtotalPizza,MontoDescuento=@MontoDescuento,CostoDelivery=@CostoDelivery,
                                NetoAfecto=@NetoAfecto,MontoIva=@MontoIva,TotalPagar=@TotalPagar
                             WHERE IdPedido=@IdPedido";
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            using (SqlTransaction tran = conexion.BeginTransaction())
            {
                try
                {
                    p.IdCliente = ResolverIdCliente(p.Cliente, p.Telefono, conexion, tran);
                    using (SqlCommand cmd = new SqlCommand(query, conexion, tran))
                    {
                        cmd.Parameters.AddWithValue("@IdPedido", p.IdPedido);
                        CargarParametros(cmd, p);
                        bool exito = cmd.ExecuteNonQuery() > 0;
                        if (exito) tran.Commit();
                        else Revertir(tran); // el pedido no existe: tampoco se conserva un cliente creado de más
                        return exito;
                    }
                }
                catch
                {
                    Revertir(tran);
                    throw;
                }
            }
        }
        public bool EliminarPedido(int id)
        {
            string query = "DELETE FROM dbo.Pedidos WHERE IdPedido=@IdPedido";
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(query, conexion))
            {
                cmd.Parameters.AddWithValue("@IdPedido", id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        // Obtiene el IdCliente de nombre + teléfono o crea cliente, usando la transacción del llamador.
        private static int ResolverIdCliente(string nombre, string telefono, SqlConnection conexion, SqlTransaction tran)
        {
            return new ClienteDAO().ObtenerOCrearIdCliente(nombre, telefono, conexion, tran);
        }
        // Rollback tolerante: si la transacción ya terminó o la conexión se perdió, no oculta el error original.
        private static void Revertir(SqlTransaction tran)
        {
            try { tran.Rollback(); }
            catch (Exception) { /* la transacción ya no está activa */ }
        }
        private static void CargarParametros(SqlCommand cmd, PedidoPizza p)
        {
            cmd.Parameters.AddWithValue("@IdCliente", p.IdCliente);
            cmd.Parameters.AddWithValue("@Cliente", p.Cliente);
            cmd.Parameters.AddWithValue("@Telefono", p.Telefono);
            cmd.Parameters.AddWithValue("@TamanoNombre", p.TamanoNombre);
            cmd.Parameters.AddWithValue("@PrecioBaseTamano", p.PrecioBaseTamano);
            cmd.Parameters.AddWithValue("@MasaNombre", p.MasaNombre);
            cmd.Parameters.AddWithValue("@RecargoMasa", p.RecargoMasa);
            cmd.Parameters.AddWithValue("@QuesoExtra", p.QuesoExtra);
            cmd.Parameters.AddWithValue("@Pepperoni", p.Pepperoni);
            cmd.Parameters.AddWithValue("@Champinones", p.Champinones);
            cmd.Parameters.AddWithValue("@Aceitunas", p.Aceitunas);
            cmd.Parameters.AddWithValue("@Tocino", p.Tocino);
            cmd.Parameters.AddWithValue("@EsDelivery", p.EsDelivery);
            cmd.Parameters.AddWithValue("@AplicaConvenioEstudiante", p.AplicaConvenioEstudiante);
            cmd.Parameters.AddWithValue("@CostoIngredientes", p.CostoIngredientes);
            cmd.Parameters.AddWithValue("@SubtotalPizza", p.SubtotalPizza);
            cmd.Parameters.AddWithValue("@MontoDescuento", p.MontoDescuento);
            cmd.Parameters.AddWithValue("@CostoDelivery", p.CostoDelivery);
            cmd.Parameters.AddWithValue("@NetoAfecto", p.NetoAfecto);
            cmd.Parameters.AddWithValue("@MontoIva", p.MontoIva);
            cmd.Parameters.AddWithValue("@TotalPagar", p.TotalPagar);
        }
    }
}
