using System;
using System.Windows.Forms;
using PizzeriaBellaItalia.Presentacion;
namespace PizzeriaBellaItalia;
// Punto de entrada de la aplicación WinForms.
static class Program
{
    [STAThread] // Requerido por WinForms (modelo STA).
    static void Main()
    {
        ApplicationConfiguration.Initialize(); // Inicializa configuración de la app.
        Application.Run(new FormPedido());     // Ejecuta el formulario principal.
    }
}