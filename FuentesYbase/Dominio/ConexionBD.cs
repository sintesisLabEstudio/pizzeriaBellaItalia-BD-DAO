using System;
using Microsoft.Data.SqlClient;

namespace PizzeriaBellaItalia.Dominio
{
    // Centraliza la conexión a SQL Server (LocalDB/PizzeriaDB).
    public static class ConexionBD
    {
        // Cadena de conexión: servidor, BD, autenticación Windows y confianza de certificado local.
        private static readonly string cadenaConexion =
            @"Server=(localdb)\MSSQLLocalDB;Database=PizzeriaDB;Integrated Security=True;TrustServerCertificate=True;";
        // Expone la cadena para mostrar detalles en la UI.
        public static string CadenaConexion => cadenaConexion;
        // Crea y abre una conexión; el llamador debe cerrarla (usar "using").
        public static SqlConnection ObtenerConexion()
        {
            SqlConnection conexion = new SqlConnection(cadenaConexion);
            conexion.Open();    // Abre la conexión; si falla, lanza SqlException.
            return conexion;    // El llamador debe cerrar la conexión.
        }
        // Prueba la conexión: true si abre correctamente, false ante error SQL.
        public static bool ProbarConexion()
        {
            try
            {
                using (SqlConnection conexion = ObtenerConexion())
                    return conexion.State == System.Data.ConnectionState.Open;
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"[Error de Conexión SQL]: {ex.Message}");
                return false;
            }
        }
    }
}