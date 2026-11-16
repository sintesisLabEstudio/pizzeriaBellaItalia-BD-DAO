using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace PizzeriaBellaItalia.Dominio
{
    // DAO: acceso a datos CRUD para dbo.Pedidos.
    public class PedidoDAO
    {
        // INSERT: guarda un pedido completo; retorna true si insertó filas.
        public bool InsertarPedido(PedidoPizza p)
        {
            //construccion de query para insertar un pedido en la base de datos
            string query = @"INSERT INTO dbo.Pedidos 
                (Cliente,Telefono,TamanoNombre,PrecioBaseTamano,MasaNombre,RecargoMasa,
                 QuesoExtra,Pepperoni,Champinones,Aceitunas,Tocino,EsDelivery,AplicaConvenioEstudiante,
                 CostoIngredientes,SubtotalPizza,MontoDescuento,CostoDelivery,NetoAfecto,MontoIva,TotalPagar)
                VALUES
                (@Cliente,@Telefono,@TamanoNombre,@PrecioBaseTamano,@MasaNombre,@RecargoMasa,
                 @QuesoExtra,@Pepperoni,@Champinones,@Aceitunas,@Tocino,@EsDelivery,@AplicaConvenioEstudiante,
                 @CostoIngredientes,@SubtotalPizza,@MontoDescuento,@CostoDelivery,@NetoAfecto,@MontoIva,@TotalPagar)";
            
            //establece la conexion con la base de datos
            using (SqlConnection conexion = ConexionBD.ObtenerConexion()) 
            
            //ejecuta el comando sql con la query y la conexion
            using (SqlCommand cmd = new SqlCommand(query, conexion)) 
            {
                //Asigna parámetros SQL a cmd con valores del objeto p de PedidoPizza.
                CargarParametros(cmd, p);

                //retorna el cmd ejecutado y verifica si se insertaron filas
                return cmd.ExecuteNonQuery() > 0; 
            }

        }

        // SELECT: lista pedidos para grilla (incluye IdPedido oculto en UI).
        public DataTable ObtenerPedidosDataTable()
        {
            //instancia de DataTable para almacenar los resultados de la consulta
            DataTable dt = new DataTable(); //dt es una tabla para llenar una consulta

            //query para seleccionar los campos necesarios de la tabla Pedidos ordenados por IdPedido descendente
            string query = "SELECT IdPedido,Cliente,Telefono,TamanoNombre,TotalPagar FROM dbo.Pedidos ORDER BY IdPedido DESC";

            //establece la conexion con la base de datos y ejecuta la consulta
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())

            //ejecuta el comando sql con la query y la conexion
            using (SqlDataAdapter adapter = new SqlDataAdapter(query, conexion))
            {
                adapter.Fill(dt); //llena el DataTable con los resultados de la consulta dt
            }
            //retorna la tabla de datos filtrada
            return dt;
        }

        // SELECT: busca por cliente con LIKE (parámetro @Filtro).
        public DataTable BuscarPedidosPorCliente(string filtroCliente)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT IdPedido,Cliente,Telefono,TamanoNombre,TotalPagar
                             FROM dbo.Pedidos WHERE Cliente LIKE @Filtro ORDER BY IdPedido DESC";
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(query, conexion))
            {
                //parametrizacion del filtro
                cmd.Parameters.AddWithValue("@Filtro", "%" + filtroCliente + "%");

                //crea el adaptador usando el SqlCommand cmd ya configurado con cmd
                //componente de ADO.NET que actúa como puente entre la base de datos
                //y un contenedor en memoria como DataTable o DataSet.
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt); //llena ..Deja dt lleno en memoria para usarlo
                }
            }
            return dt; //retorna un DataTable?
        }

        // SELECT por ID: retorna PedidoPizza completo o null si no existe.
        public PedidoPizza? ObtenerPedidoPorId(int id)
        {
            string query = @"SELECT IdPedido,Cliente,Telefono,TamanoNombre,PrecioBaseTamano,MasaNombre,RecargoMasa,
                                    QuesoExtra,Pepperoni,Champinones,Aceitunas,Tocino,EsDelivery,AplicaConvenioEstudiante,
                                    CostoIngredientes,SubtotalPizza,MontoDescuento,CostoDelivery,NetoAfecto,MontoIva,TotalPagar
                             FROM dbo.Pedidos WHERE IdPedido=@IdPedido";
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(query, conexion))
            {
                //parametrizacion de la consulta 
                cmd.Parameters.AddWithValue("@IdPedido", id);

                //
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null; //si no hay lectura retorna null
                    return new PedidoPizza  //retorna el pedido con toda su cfg
                    {
                        IdPedido = Convert.ToInt32(r["IdPedido"]), //r permite leer el campo ?
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

        // UPDATE: modifica un pedido por IdPedido.
        public bool ActualizarPedido(PedidoPizza p)
        {
            string query = @"UPDATE dbo.Pedidos SET
                                Cliente=@Cliente,Telefono=@Telefono,TamanoNombre=@TamanoNombre,PrecioBaseTamano=@PrecioBaseTamano,
                                MasaNombre=@MasaNombre,RecargoMasa=@RecargoMasa,QuesoExtra=@QuesoExtra,Pepperoni=@Pepperoni,
                                Champinones=@Champinones,Aceitunas=@Aceitunas,Tocino=@Tocino,EsDelivery=@EsDelivery,
                                AplicaConvenioEstudiante=@AplicaConvenioEstudiante,CostoIngredientes=@CostoIngredientes,
                                SubtotalPizza=@SubtotalPizza,MontoDescuento=@MontoDescuento,CostoDelivery=@CostoDelivery,
                                NetoAfecto=@NetoAfecto,MontoIva=@MontoIva,TotalPagar=@TotalPagar
                             WHERE IdPedido=@IdPedido";
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(query, conexion))
            {
                cmd.Parameters.AddWithValue("@IdPedido", p.IdPedido);

                //carga/asigna en cmd los valores de los parámetros SQL (@Cliente, @Telefono, etc.)
                //tomándolos del objeto p.
                CargarParametros(cmd, p);

                //ejecuta la sentencia del comando (en este caso UPDATE dbo.Pedidos ...)
                //y devuelve el número de filas afectadas
                return cmd.ExecuteNonQuery() > 0;  
            }
        }

        // DELETE: elimina un pedido por IdPedido.
        public bool EliminarPedido(int id)
        {
            string query = "DELETE FROM dbo.Pedidos WHERE IdPedido=@IdPedido";
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(query, conexion))
            {
                //agrega un parámetro al comando SQL,
                //asociando el nombre @IdPedido con el valor de la variable id.
                cmd.Parameters.AddWithValue("@IdPedido", id);
                return cmd.ExecuteNonQuery() > 0; 
            }
        }

        //Asigna parámetros SQL desde PedidoPizza (excepto @IdPedido en UPDATE se agrega aparte).
        //se ocupa en instartar y actualizar pedido
        private static void CargarParametros(SqlCommand cmd, PedidoPizza p)
        {
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