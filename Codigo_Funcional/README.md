# Pizzería Bella Italia — Gestión de pedidos (WinForms + ADO.NET + DAO)

Aplicación de escritorio en **C# / .NET 10 (Windows Forms)** para registrar y administrar los pedidos de la pizzería "Bella Italia". Persiste en **SQL Server LocalDB** usando **ADO.NET puro** (`Microsoft.Data.SqlClient`) con **arquitectura en capas y patrón DAO**.

Es un trabajo final de *Construcción de Software* (Instituto Profesional Virginio Gómez): CRUD completo sobre `PizzeriaDB` con consultas parametrizadas, reglas de negocio desacopladas de la interfaz y base de datos con integridad referencial.

---

## 1. Estructura del proyecto

```
Codigo_Funcional/
├── Program.cs                       Punto de entrada ([STAThread] → FormPedido)
├── PizzeriaBellaItalia.csproj       net10.0-windows, WinForms, SqlClient 7.1.2
├── PizzeriaBellaItalia.slnx         Solución (formato .slnx)
├── Dominio/                         Capa de dominio + acceso a datos
│   ├── PedidoPizza.cs               Entidad → dbo.Pedidos
│   ├── Cliente.cs                   Entidad → dbo.Clientes
│   ├── CalculadoraPedido.cs         Reglas de negocio (static)
│   ├── PedidoDAO.cs                 CRUD de dbo.Pedidos
│   ├── ClienteDAO.cs                CRUD de dbo.Clientes
│   └── ConexionBD.cs                Cadena de conexión y prueba
├── Presentacion/                    Capa de interfaz (WinForms)
│   ├── FormPedido.cs / .Designer.cs / .resx     Pantalla de pedidos
│   └── FormClientes.cs / .Designer.cs / .resx   Pantalla de clientes
├── BDpizzeria/                      Base de datos y utilitarios
│   ├── pizzeriaDB.mdf               Archivo de datos (base en vivo)
│   ├── pizzeriaDB.ldf               Archivo de log
│   ├── comandos/
│   │   ├── BAT/                     7 .bat numerados (01…07)
│   │   ├── SQL/                     8 .sql con la misma numeración
│   │   ├── cfg.txt                  Cadena de conexión de referencia
│   │   └── README.txt               Orden de uso y portabilidad
│   └── respaldo/
│       ├── pizzeriaDB.mdf / .ldf    Copias de los originales
│       ├── verRegistrosTablas.bat/.sql   Visor de esas copias
│       └── README.txt
├── bin/, obj/, .vs/                 Artefactos de compilación / IDE
├── .gitignore / .gitattributes
└── README.md                        Este archivo

../estudio/                          Documentación numerada (00_INDICE.txt)
```

---

## 2. Arquitectura y capas

| Capa | Ubicación | Responsabilidad |
|---|---|---|
| **Presentación** | `Presentacion/` | Captura eventos, lee controles, coordina la UI y muestra resultados. Nunca arma SQL. |
| **Dominio / reglas** | `Dominio/CalculadoraPedido.cs` | Define opciones y calcula montos; no depende de controles WinForms. |
| **Entidades** | `Dominio/PedidoPizza.cs`, `Cliente.cs` | Modelos que mapean las columnas de las tablas. |
| **Datos (DAO)** | `Dominio/PedidoDAO.cs`, `ClienteDAO.cs` | Ejecuta SQL parametrizado (INSERT/SELECT/UPDATE/DELETE). |
| **Conexión** | `Dominio/ConexionBD.cs` | Provee `SqlConnection` y el utilitario `ProbarConexion()`. |

Flujo: **UI → DAO → ConexionBD → SQL Server (PizzeriaDB)**.

### Clases y métodos principales

- **`Program`** — `Main()` con `[STAThread]`, `ApplicationConfiguration.Initialize()` y `Application.Run(new FormPedido())`.
- **`PedidoPizza`** — `IdPedido`, `IdCliente` (FK), datos del cliente, configuración de pizza, 5 flags de ingredientes, flags de entrega/convenio y 7 montos calculados.
- **`Cliente`** — `IdCliente`, `Nombre`, `Telefono`.
- **`CalculadoraPedido`** — diccionarios `Tamanos` y `Masas`, clase anidada `DatosPedido` y método `Calcular(DatosPedido) → PedidoPizza`.
- **`PedidoDAO`** — `InsertarPedido`, `ObtenerPedidosDataTable`, `BuscarPedidosPorCliente`, `ObtenerPedidoPorId`, `ActualizarPedido`, `EliminarPedido`; privados `ResolverIdCliente` y `CargarParametros`.
- **`ClienteDAO`** — `ObtenerClientesDataTable`, `BuscarPorNombreYTelefono`, `insertarCliente`, `InsertarClienteConTransaccion`, `ObtenerOCrearIdCliente`, `ActualizarCliente`, `EliminarCliente`, `validaClienteSinPedidos`, `BuscarClientesDataTable`, `testInsertarCliente`, `ObtenerPedidosPorCliente`, `ObtenerClientePorId` y `EsClienteDuplicado`.
- **`ConexionBD`** — `CadenaConexion`, `ObtenerConexion()`, `ProbarConexion()`.
- **`FormPedido`** — `FormPedido_Load`, `CargarPedidos`, `ConstruirPedido`, `Calculo_Changed`, `MostrarResumen`, `vistaPedido`, `LlenaForm`, `LimpiarFormulario`, `MostrarEstado`, `ConfigurarAyuda`, `CargarClienteDesdeSeleccion` y los manejadores de los botones.
- **`FormClientes`** — `FormClientes_Load`, `CargarClientes`, `PrepararNuevoCliente`, `presentaCamposParaEdicion`, `btnGrabar_Click`, `btnElimina_Click`, `btnBuscar_Click`, `btnSelecciona_Click`, `btnNuevo_Click`, `btnListar_Click`.

---

## 3. Reglas de negocio

Implementadas en `CalculadoraPedido.Calcular()` y reutilizadas por el recálculo en vivo de la UI.

| Concepto | Regla |
|---|---|
| Tamaños | Personal 5.000 · Mediana 8.500 · Familiar 12.000 (CLP) |
| Masas | Tradicional 0 · Delgada 0 · Borde de queso +1.500 |
| Ingredientes extra | 800 CLP por cada opción activa (queso, pepperoni, champiñones, aceitunas, tocino) |
| Convenio estudiante | −10 % sobre el subtotal |
| Delivery | +2.000 CLP |
| IVA | 19 % sobre `(subtotal − descuento + delivery)` |
| Total | `neto + IVA` |

Los montos se guardan como `int` (CLP sin decimales) con truncamiento por conversión.

---

## 4. Base de datos

- **Servidor**: `(localdb)\MSSQLLocalDB`
- **Base**: `PizzeriaDB`
- **Cadena** (en `Dominio/ConexionBD.cs`):

```
Server=(localdb)\MSSQLLocalDB;Database=PizzeriaDB;Integrated Security=True;TrustServerCertificate=True;
```

### Tablas

- **`dbo.Clientes`** — `IdCliente` (PK IDENTITY), `Cliente` (NVARCHAR(100)), `Telefono` (NVARCHAR(30)) y la restricción **`UQ_Clientes_Cliente_Telefono`** (UNIQUE).
- **`dbo.Pedidos`** — `IdPedido` (PK IDENTITY), `IdCliente` (FK → Clientes, `ON UPDATE CASCADE` / `ON DELETE NO ACTION`), datos del cliente, configuración de pizza, 5 flags de ingredientes, `EsDelivery`, `AplicaConvenioEstudiante`, 7 montos calculados y `FechaCreacion` (DEFAULT `GETDATE()`).
- **Índices**: `IX_Pedidos_Cliente`, `IX_Pedidos_IdCliente`, `IX_Pedidos_FechaCreacion`.

### Decisiones de persistencia

- Todas las consultas son **parametrizadas** (`AddWithValue`) → sin inyección SQL.
- `ResolverIdCliente()` reutiliza el cliente por `(nombre, teléfono)` o lo crea; el DAO detecta `SqlException` 2601/2627 y la UI muestra «Cliente duplicado».
- `InsertarPedido` y `ActualizarPedido` usan **transacciones** y `SCOPE_IDENTITY()` para recuperar el `IdPedido`.
- Los listados usan `SqlDataAdapter`/`DataTable` (modelo desconectado); el detalle por ID usa `SqlDataReader`.
- La app se conecta por **nombre de base de datos**, no por ruta de archivo: mover la carpeta no rompe la conexión.

---

## 5. Funcionalidades de la interfaz

### FormPedido (pantalla principal)

- **Grilla**: al cargar se listan los pedidos; se ocultan `IdPedido` e `IdCliente` y se muestran `Cliente`, `Teléfono`, `Tamaño` y `Total a Pagar`.
- **Calcular/Grabar** (`btnCalcular`): valida cliente, calcula, muestra el desglose en «Detalle de Pago» y, tras confirmar, inserta el pedido.
- **Recálculo en vivo** (`Calculo_Changed`): al cambiar tamaño, masa, ingredientes, entrega o convenio, actualiza los montos (usa las mismas reglas que el grábado).
- **Buscar por cliente** (`btnBuscaPedidosXCliente`): filtra con `LIKE`. Con el campo vacío muestra una validación (no lista todo).
- **Buscar por ID** (`btnBuscarPedidoPorId`): obtiene el pedido y carga su detalle; informa si no existe.
- **Todos los pedidos** (`btnTodosLosPedidos`) y **clic en fila** (`dgvPedidos_CellClick`) → `vistaPedido()` → `LlenaForm()`.
- **Actualizar** y **Eliminar** por ID, con validación y confirmación para borrar.
- **Probar conexión** (`btnProbarConexion`): muestra la cadena y el estado.
- **Clientes** (`btnEditaClientes`): abre `FormClientes` en modal.
- **Nuevo Pedido / Salir**, ayuda al pasar el mouse por los botones y barra de estado (`lblAyuda`).

### FormClientes (ABM de clientes)

- Alta, edición (doble clic en la fila), búsqueda por nombre o teléfono y listado.
- **Validación de duplicados** por `(nombre, teléfono)` antes de grabar y por excepción SQL como respaldo.
- **Borrado protegido**: no elimina clientes que tengan pedidos asociados (`validaClienteSinPedidos`).
- **Seleccionar** (`btnSelecciona`): inyecta el cliente elegido en `FormPedido` mediante `CargarClienteDesdeSeleccion`.

---

## 6. Comandos de base de datos

Ubicados en `BDpizzeria/comandos/`, separados en `BAT/` (ejecutables) y `SQL/` (scripts invocados), con numeración coincidente.

| # | Comando | Acción |
|---|---|---|
| 01 | `creaBaseDatosP-01.bat` | Crea la BD y las tablas; aplica la migración de clientes |
| 02 | `cargaBaseDatos-02.bat` | Adjunta la BD a LocalDB y la deja en `MULTI_USER` (**paso obligatorio**) |
| 03 | `verEstadoConexion-03.bat` | Diagnóstico: instancia, estado/modo, ruta del `.mdf` y filas |
| 04 | `verTablas-04.bat` | Muestra tablas, estructura y datos |
| 05 | `respaldarBaseDatos-05.bat` | Copia `.mdf`/`.ldf` a `respaldo\`; **pide confirmación antes de sobrescribir**; no genera `.bak` |
| — | `respaldo/verRegistrosTablas.bat` | Muestra tablas y registros de las copias de `respaldo/` |
| 06 | `vaciaBaseDatos-06.bat` | Borra pedidos y clientes y reinicia los IDENTITY |
| 07 | `desconectaBase-07.bat` | Solo desadjunta la BD (no borra archivos) |

**Portabilidad:** no hay rutas absolutas. Cada `.bat` resuelve la ruta de los datos dos niveles por encima de `BAT/`, busca `sqlcmd` en el PATH y en ODBC 140–180, y crea/inicia LocalDB si hace falta. Funcionan aunque se mueva el proyecto. Ver `BDpizzeria/comandos/README.txt`.

---

## 7. Requisitos y puesta en marcha

**Requisitos**
- Windows con .NET 10 SDK y carga de trabajo *Desarrollo de escritorio .NET* (Windows Forms).
- Visual Studio 2022 reciente (o abrir directamente el `.csproj`; el `.slnx` puede no ser soportado por versiones antiguas).
- SQL Server LocalDB (`(localdb)\MSSQLLocalDB`).
- Paquete NuGet `Microsoft.Data.SqlClient` 7.1.2 (se restaura automáticamente).

**Pasos**
1. Abrir `PizzeriaBellaItalia.slnx` o `PizzeriaBellaItalia.csproj`.
2. Restaurar paquetes NuGet y compilar (`Ctrl+Shift+B`). Resultado esperado: **0 errores / 0 advertencias**.
3. En una CMD, entrar a `BDpizzeria\comandos\BAT` y ejecutar `cargaBaseDatos-02.bat`.
4. Ejecutar la aplicación (`F5` / `Ctrl+F5`).

Para verificar el entorno: `verEstadoConexion-03.bat` debe imprimir `RESULTADO: TODO OK`.

---

## 8. Notas

- El total mostrado y persistido corresponde a `PedidoPizza.TotalPagar`.
- `System.Data.SqlClient` aparece comentado en el `.csproj`; el acceso real usa `Microsoft.Data.SqlClient`.
- El `.csproj` conserva exclusiones heredadas (`FuentesYbase\**`, `ejecutablePortable\**`, `BDpizzeria\pruebaDao\**`) de versiones anteriores; no afectan la compilación.
- No se generan archivos `.bak`: los respaldos son copias de los archivos originales en `BDpizzeria\respaldo\`.
- Documentación adicional en `../estudio/`: `00_INDICE.txt` (orden de lectura), `07_ResumenCodigo.txt`, `12_flujo.txt`, `13_DIAGRAMA_FLUJO.md` / `14_DIAGRAMA_FLUJO.txt` y `15_INSTRUCCIONES_COMPILACION.txt`.
