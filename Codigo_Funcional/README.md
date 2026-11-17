# PizzeriaBellaItalia (WinForms + SQL Server LocalDB)

Aplicación de escritorio en C# (Windows Forms, .NET 10) para la gestión de pedidos de la pizzería "Bella Italia", con persistencia en **SQL Server LocalDB** mediante **ADO.NET** (`Microsoft.Data.SqlClient`) y arquitectura en capas.

## Estructura del proyecto

- `Presentacion/`
  - `FormPedido.cs`, `FormPedido.Designer.cs`: interfaz principal (ingreso, cálculo, grilla y acciones CRUD).
- `Dominio/`
  - `PedidoPizza.cs`: entidad/modelo que mapea los datos de un pedido (`dbo.Pedidos`).
  - `PedidoDAO.cs`: acceso a datos (CRUD) con consultas SQL parametrizadas.
  - `CalculadoraPedido.cs`: reglas de negocio/cálculo (tamaños, masas, extras, descuentos, delivery, IVA) desacopladas de la UI.
  - `ConexionBD.cs`: cadena y obtención/prueba de conexión a la base de datos.
- `BDpizzeria/`: scripts y utilitarios para crear/cargar/desconectar la base (`creaBaseDatosP.bat/.sql`, `cargaBaseDatos.bat/.sql`, `desconectaBase.bat/.sql`, archivos `.mdf/.ldf/.bak`, `cfg.txt`).
- `flujo.txt`: descripción del flujo de acciones por botón.
- `Program.cs`: punto de entrada de la aplicación.

## Base de datos

- **Motor/servidor**: `(localdb)\MSSQLLocalDB`
- **Base de datos**: `PizzeriaDB`
- **Tabla principal**: `dbo.Pedidos` (tabla única)

Campos principales: `IdPedido` (IDENTITY, PK), datos de cliente (`Cliente`, `Telefono`), configuración (`TamanoNombre`, `PrecioBaseTamano`, `MasaNombre`, `RecargoMasa`), ingredientes (flags), entrega/promoción (`EsDelivery`, `AplicaConvenioEstudiante`), montos calculados (`CostoIngredientes`, `SubtotalPizza`, `MontoDescuento`, `CostoDelivery`, `NetoAfecto`, `MontoIva`, `TotalPagar`) y `FechaCreacion` (auditoría).

> La cadena de conexión está definida en `Dominio/ConexionBD.cs`. Ajustarla si tu instancia/ruta de LocalDB es diferente.
> Los scripts de `BDpizzeria` contienen rutas absolutas del proyecto; actualízalas si mueves la carpeta.

## Funcionalidades (UI)

- **Grilla de pedidos**: al iniciar se cargan los pedidos (`ObtenerPedidosDataTable`). Se muestran las columnas visibles `Cliente`, `Teléfono`, `Tamaño` y `Total a Pagar` (títulos definidos en la UI); `IdPedido` se mantiene oculto pero disponible para selección interna.
- **Calcular / Grabar** (`btnCalcular`): valida datos mínimos, la UI arma los datos y delega el cálculo a `CalculadoraPedido.Calcular(...)` (tamaño/masa, extras, delivery, convenio, IVA), muestra el resumen y, tras confirmar, inserta el pedido (`PedidoDAO.InsertarPedido`).
- **Buscar por cliente** (`btnBuscaPedidosXCliente`): filtra por nombre con `LIKE` (`BuscarPedidosPorCliente`).
- **Buscar por ID** (`btnBuscarPedidoPorId`): obtiene un pedido por `IdPedido` (`ObtenerPedidoPorId`) y muestra su detalle.
- **Todos los pedidos** (`btnTodosLosPedidos`): recarga el listado completo.
- **Clic en fila**: al seleccionar una fila se obtiene el `IdPedido` y se cargan todos los detalles en el formulario (`vistaPedido` → `LlenaForm`).
- **Actualizar pedido** (`btnActualizarPedido`): recalcula desde la UI y actualiza el pedido indicado por ID (`ActualizarPedido`).
- **Eliminar pedido** (`btnEliminarPedido`): elimina previa confirmación (`EliminarPedido`).
- **Probar conexión** (`btnProbarConexion`): verifica conectividad y muestra cadena/estado (`ConexionBD.ProbarConexion`).
- **Nuevo pedido / Limpiar** (`btnLimpiar`) y **Salir** (`btnConfirmar`): restablecen el formulario o cierran la aplicación.
- **Apariencia**: colores/estilos e íconos de botones definidos directamente en `FormPedido.Designer.cs`.

## Arquitectura

- **Presentación** (`Presentacion`): captura eventos, lee controles, coordina la UI y muestra resultados (`LlenaForm` carga datos/montos en pantalla).
- **Reglas/Cálculo** (`Dominio/CalculadoraPedido.cs`): define opciones y calcula montos del pedido, desacoplado de controles WinForms.
- **Datos/DAO** (`Dominio/PedidoDAO.cs`): ejecuta operaciones parametrizadas sobre `dbo.Pedidos` (INSERT, SELECT/listado, SELECT por cliente/ID, UPDATE, DELETE).
- **Conexión** (`Dominio/ConexionBD.cs`): provee `SqlConnection` y utilitario de prueba.

## Requisitos

- Windows con soporte para .NET 10 y Windows Forms.
- SQL Server LocalDB (`(localdb)\MSSQLLocalDB`) con la base `PizzeriaDB` creada/cargada (usar scripts en `BDpizzeria`).
- Paquete NuGet: `Microsoft.Data.SqlClient`.

## Notas

- El total mostrado y persistido corresponde a `PedidoPizza.TotalPagar`.
- `System.Data.SqlClient` puede aparecer comentado en el `.csproj`; el acceso a datos utiliza `Microsoft.Data.SqlClient`.
- `BDpizzeria`: los scripts `.sql` y `.bat` usan rutas absolutas a esta carpeta; son correctos para la ruta del proyecto, pero deben actualizarse si se mueve la solución.
- Ver `flujo.txt` para el detalle paso a paso de cada acción.
