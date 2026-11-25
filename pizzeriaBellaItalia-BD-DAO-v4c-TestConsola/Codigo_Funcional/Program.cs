using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PizzeriaBellaItalia.Presentacion;
namespace PizzeriaBellaItalia;
// Punto de entrada de la aplicación WinForms.
static class Program
{
    const uint SWP_NOZORDER = 0x0004;
    const uint SWP_NOACTIVATE = 0x0010;
    const int SW_RESTORE = 9;

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint flags);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int cmdShow);

    [STAThread] // Requerido por WinForms (modelo STA).
    static void Main()
    {
        ApplicationConfiguration.Initialize(); // Inicializa configuración de la app.

        var form = new FormPedido();
        form.StartPosition = FormStartPosition.Manual;

        bool ubicando = false;

        // Formulario a la izquierda, consola a la derecha (media pantalla cada uno).
        void DividirPantalla()
        {
            if (ubicando) return;
            ubicando = true;
            try
            {
                var area = Screen.FromHandle(form.Handle).WorkingArea;
                int mitad = area.Width / 2;

                SetWindowPos(form.Handle, IntPtr.Zero, area.Left, area.Top,
                             mitad, area.Height, SWP_NOZORDER);

                ColocarConsola(area.Left + mitad, area.Top,
                               area.Width - mitad, area.Height);
            }
            finally { ubicando = false; }
        }

        // Si el usuario mueve o redimensiona la app, la consola sigue a la derecha.
        void MantenerConsolaALaDerecha()
        {
            if (ubicando) return;
            ubicando = true;
            try
            {
                var area = Screen.FromHandle(form.Handle).WorkingArea;
                int x = form.Right;
                ColocarConsola(x, form.Top,
                               Math.Max(area.Right - x, 300), form.Height);
            }
            finally { ubicando = false; }
        }

        void ColocarConsola(int x, int y, int ancho, int alto)
        {
            IntPtr consola = GetConsoleWindow();
            if (consola == IntPtr.Zero) return;
            ShowWindow(consola, SW_RESTORE);
            SetWindowPos(consola, IntPtr.Zero, x, y, ancho, alto,
                         SWP_NOZORDER | SWP_NOACTIVATE);
        }

        form.Shown += (_, _) => DividirPantalla();
        form.Move += (_, _) => MantenerConsolaALaDerecha();
        form.Resize += (_, _) => MantenerConsolaALaDerecha();

        Application.Run(form);     // Ejecuta el formulario principal.
    }
}
