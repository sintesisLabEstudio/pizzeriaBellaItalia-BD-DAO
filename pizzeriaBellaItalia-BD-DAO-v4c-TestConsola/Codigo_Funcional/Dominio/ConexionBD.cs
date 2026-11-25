using System;
using System.Data;
using Microsoft.Data.SqlClient;
namespace PizzeriaBellaItalia.Dominio
{
    // Helper estático: única clase que conoce la cadena de conexión.
    // Solo los DAO la usan; la capa de presentación nunca abre conexiones.
    public static class ConexionBD
    {
        //RUTA RELATIVA
        //private static readonly string rutaBaseDatos = Path.GetFullPath(
        //    Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\Codigo_Funcional\BDpizzeria\pizzeriaDB.mdf"));

        //RUTA ABSOLUTA
        //private static readonly string rutaBaseDatos = @"C:\desarrollo\Csoft\pizzeriaBellaItalia-BD-DAO-v4\Codigo_Funcional\BDpizzeria\pizzeriaDB.mdf";

        //CADENA DE CONEXION final resultante EN BASE A LA RUTA DE LA BASE DE DATOS
        //private static readonly string cadenaConexion =
        //    $@"Server=(localdb)\MSSQLLocalDB;AttachDbFilename={rutaBaseDatos};Database=PizzeriaDB;Integrated Security=True;TrustServerCertificate=True;";
        
        // ALTERNATIVA con la instancia nombrada de SQL Server Express (servicio .\SQLEXPRESS):
        //private static readonly string cadenaConexion =
        //    @"Server=.\SQLEXPRESS;Database=PizzeriaDB;Integrated Security=True;TrustServerCertificate=True;";
        
        // LocalDB por nombre de base de datos (sin ruta de archivo).
        private static readonly string cadenaConexion =
            @"Server=(localdb)\MSSQLLocalDB;Database=PizzeriaDB;Integrated Security=True;TrustServerCertificate=True;";
        
        public static string CadenaConexion => cadenaConexion;
        // Abre y devuelve una conexión nueva. Quien la recibe debe liberarla con "using".
        public static SqlConnection ObtenerConexion()
        {
            SqlConnection conexion = new SqlConnection(cadenaConexion);
            try
            {
                conexion.Open();
                return conexion;
            }
            catch
            {
                conexion.Dispose(); // Si no abre, no queda una conexión sin liberar.
                throw;
            }
        }
        // Verifica que la base PizzeriaDB abra correctamente.
        public static bool ProbarConexion()
        {
            try
            {
                using (SqlConnection conexion = ObtenerConexion())
                    return conexion.State == ConnectionState.Open;
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"[Error de Conexión SQL]: {ex.Message}");
                return false;
            }
        }
    }
}
