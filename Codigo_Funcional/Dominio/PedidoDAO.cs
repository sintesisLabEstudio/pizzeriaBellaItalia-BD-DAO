using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace PizzeriaBellaItalia.Dominio
{
    // DAO: acceso a datos CRUD para dbo.Pedidos.
    // Cada pedido guarda su IdCliente (FK a dbo.Clientes): antes de guardar
    // se resuelve el cliente en dbo.Clientes (se crea si no existe) y se
    // envía su IdCliente en el INSERT/UPDATE.
    public class PedidoDAO
    {
        // INSERT: guarda un pedido completo; retorna true si insertó filas.
        public bool InsertarPedido(PedidoPizza p)
        {
            //construccion de query para insertar un pedido en la base de datos
            //al final se pide el SCOPE_IDENTITY para conocer el IdPedido generado
            string query = @"INSERT INTO dbo.Pedidos 
                (IdCliente,Cliente,Telefono,TamanoNombre,PrecioBaseTamano,MasaNombre,RecargoMasa,
                 QuesoExtra,Pepperoni,Champinones,Aceitunas,Tocino,EsDelivery,AplicaConvenioEstudiante,
                 CostoIngredientes,SubtotalPizza,MontoDescuento,CostoDelivery,NetoAfecto,MontoIva,TotalPagar)
                VALUES
                (@IdCliente,@Cliente,@Telefono,@TamanoNombre,@PrecioBaseTamano,@MasaNombre,@RecargoMasa,
                 @QuesoExtra,@Pepperoni,@Champinones,@Aceitunas,@Tocino,@EsDelivery,@AplicaConvenioEstudiante,
                 @CostoIngredientes,@SubtotalPizza,@MontoDescuento,@CostoDelivery,@NetoAfecto,@MontoIva,@TotalPagar);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            //1) valida el cliente en dbo.Clientes por nombre + telefono:
            //   si ya existe se usa su IdCliente; si no, se inserta y se usa el IdCliente nuevo.
            p.IdCliente = ResolverIdCliente(p.Cliente, p.Telefono);

            //establece la conexion con la base de datos y abre la transaccion
            //(si el INSERT del pedido falla, no queda el pedido a medias)
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            using (SqlTransaction tran = conexion.BeginTransaction())
            {
                //ejecuta el comando sql con la query y la conexion
                using (SqlCommand cmd = new SqlCommand(query, conexion, tran))
                {
                    //Asigna parámetros SQL a cmd con valores del objeto p de PedidoPizza.
                    CargarParametros(cmd, p);

                    //ExecuteScalar devuelve el IdPedido generado por el INSERT
                    object resultado = cmd.ExecuteScalar();
                    bool exito = resultado != null && resultado != DBNull.Value;

                    //guarda en el objeto el IdPedido recien creado
                    if (exito) p.IdPedido = Convert.ToInt32(resultado);

                    //confirma la transaccion del pedido
                    tran.Commit();
                    return exito;
                }
            }

        }
        // SELECT: lista pedidos para grilla (incluye IdPedido oculto en UI).
        public DataTable ObtenerPedidosDataTable()
        {
            //instancia de DataTable para almacenar los resultados de la consulta
            DataTable dt = new DataTable(); //dt es una tabla para llenar una consulta

            //query para seleccionar los campos necesarios del pedido con los datos
            //del cliente que están en dbo.Clientes (relacionados por IdCliente)
            string query = @"SELECT p.IdPedido, p.IdCliente, c.Cliente, c.Telefono, p.TamanoNombre, p.TotalPagar
                             FROM dbo.Pedidos p
                             INNER JOIN dbo.Clientes c ON c.IdCliente = p.IdCliente
                             ORDER BY p.IdPedido DESC";

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
            string query = @"SELECT p.IdPedido, p.IdCliente, c.Cliente, c.Telefono, p.TamanoNombre, p.TotalPagar
                             FROM dbo.Pedidos p
                             INNER JOIN dbo.Clientes c ON c.IdCliente = p.IdCliente
                             WHERE c.Cliente LIKE @Filtro
                             ORDER BY p.IdPedido DESC";
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
            string query = @"SELECT p.IdPedido, p.IdCliente, c.Cliente, c.Telefono, p.TamanoNombre, p.PrecioBaseTamano, p.MasaNombre, p.RecargoMasa,
                                    p.QuesoExtra, p.Pepperoni, p.Champinones, p.Aceitunas, p.Tocino, p.EsDelivery, p.AplicaConvenioEstudiante,
                                    p.CostoIngredientes, p.SubtotalPizza, p.MontoDescuento, p.CostoDelivery, p.NetoAfecto, p.MontoIva, p.TotalPagar
                             FROM dbo.Pedidos p
                             INNER JOIN dbo.Clientes c ON c.IdCliente = p.IdCliente
                             WHERE p.IdPedido = @IdPedido";
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
                        IdCliente = Convert.ToInt32(r["IdCliente"]), //FK al cliente del pedido
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
                                IdCliente=@IdCliente,Cliente=@Cliente,Telefono=@Telefono,TamanoNombre=@TamanoNombre,PrecioBaseTamano=@PrecioBaseTamano,
                                MasaNombre=@MasaNombre,RecargoMasa=@RecargoMasa,QuesoExtra=@QuesoExtra,Pepperoni=@Pepperoni,
                                Champinones=@Champinones,Aceitunas=@Aceitunas,Tocino=@Tocino,EsDelivery=@EsDelivery,
                                AplicaConvenioEstudiante=@AplicaConvenioEstudiante,CostoIngredientes=@CostoIngredientes,
                                SubtotalPizza=@SubtotalPizza,MontoDescuento=@MontoDescuento,CostoDelivery=@CostoDelivery,
                                NetoAfecto=@NetoAfecto,MontoIva=@MontoIva,TotalPagar=@TotalPagar
                             WHERE IdPedido=@IdPedido";

            //1) valida el cliente en dbo.Clientes por nombre + telefono:
            //   si ya existe se usa su IdCliente; si no, se inserta y se usa el IdCliente nuevo.
            p.IdCliente = ResolverIdCliente(p.Cliente, p.Telefono);

            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            using (SqlTransaction tran = conexion.BeginTransaction())
            {
                using (SqlCommand cmd = new SqlCommand(query, conexion, tran))
                {
                    cmd.Parameters.AddWithValue("@IdPedido", p.IdPedido);

                    //carga/asigna en cmd los valores de los parámetros SQL (@IdCliente, @Cliente, etc.)
                    //tomándolos del objeto p.
                    CargarParametros(cmd, p);

                    //ejecuta la sentencia del comando (en este caso UPDATE dbo.Pedidos ...)
                    //y devuelve el número de filas afectadas
                    bool exito = cmd.ExecuteNonQuery() > 0;

                    //confirma la transaccion del pedido
                    tran.Commit();
                    return exito;
                }
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
        // Resuelve el IdCliente que se enviará en el pedido.
        // IMPORTANTE: dbo.Clientes tiene UQ_Clientes_Cliente_Telefono (UNIQUE),
        // por lo que no se pueden crear dos clientes iguales; si en una carrera
        // entre SELECT e INSERT otro proceso crea el mismo cliente, SQL Server
        // lanza SqlException 2601/2627 y la UI lo informa con un MessageBox.
        private static int ResolverIdCliente(string nombre, string telefono)
        // 1) valida el cliente en dbo.Clientes con ClienteDAO.BuscarPorNombreYTelefono.
        //    Si ya existe (mismo nombre y mismo teléfono) se usa su IdCliente y
        //    el pedido se registra tal cual con ese IdCliente.
        // 2) si no existe, se inserta el cliente en dbo.Clientes y se usa el IdCliente
        //    generado para registrar el pedido.
        {
            ClienteDAO clienteDao = new ClienteDAO();

            //1) el cliente ya está registrado: se reutiliza su IdCliente
            Cliente? cliente = clienteDao.BuscarPorNombreYTelefono(nombre, telefono);
            if (cliente != null)
                return cliente.IdCliente;

            //2) el cliente no existe: se inserta y se toma el IdCliente nuevo
            return clienteDao.insertarCliente(new Cliente { Nombre = nombre, Telefono = telefono });
        }
        //Asigna parámetros SQL desde PedidoPizza (excepto @IdPedido en UPDATE se agrega aparte).
        private static void CargarParametros(SqlCommand cmd, PedidoPizza p)
        //se ocupa en instartar y actualizar pedido
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
