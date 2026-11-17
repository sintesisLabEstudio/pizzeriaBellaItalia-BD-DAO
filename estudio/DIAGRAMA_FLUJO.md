# Diagramación del flujo completo — Pizzería Bella Italia

Documentación del recorrido **archivo → clase → método → objeto → respuesta en pantalla** para las dos pantallas del sistema (`FormPedido` y `FormClientes`). Incluye el estado actual del código: restricción `UQ_Clientes_Cliente_Telefono` aplicada y MessageBox de cliente duplicado.

---

## 1. Mapa de archivos → capas

```mermaid
flowchart TD
    subgraph PROY["Proyectos"]
        SLNX["PizzeriaBellaItalia.slnx<br/>PizzeriaBellaItalia.csproj"]
        TEST["BDpizzeria/pruebaDao/pruebaDao.csproj<br/>arnés de pruebas DAO"]
    end

    subgraph PRE["PRESENTACIÓN — Presentacion/"]
        PROG["Program.cs<br/>Main()"]
        FPD["FormPedido.Designer.cs<br/>InitializeComponent()"]
        FP["FormPedido.cs<br/>eventos + lógica de UI"]
        FCD["FormClientes.Designer.cs<br/>InitializeComponent()"]
        FC["FormClientes.cs<br/>eventos + lógica de UI"]
    end

    subgraph DOM["DOMINIO — Dominio/"]
        CALC["CalculadoraPedido.cs<br/>Tamanos, Masas, DatosPedido, Calcular()"]
        PP["PedidoPizza.cs<br/>entidad pedido"]
        CL["Cliente.cs<br/>entidad cliente"]
        PDO["PedidoDAO.cs<br/>CRUD dbo.Pedidos"]
        CDO["ClienteDAO.cs<br/>CRUD dbo.Clientes"]
        CN["ConexionBD.cs<br/>ObtenerConexion(), ProbarConexion()"]
    end

    subgraph DAT["DATOS — BDpizzeria/"]
        S1["creaBaseDatosPizzeria.sql"]
        S2["actualizaBaseDatosClientes.sql"]
        S3["cargaBaseDatos.sql / vaciaBase.sql"]
        DB[("SQL Server LocalDB — PizzeriaDB<br/>dbo.Clientes 1:N dbo.Pedidos<br/>UQ_Clientes_Cliente_Telefono")]
    end

    SLNX --> PROG
    PROG --> FP
    FP --- FPD
    FC --- FCD
    FP --> PDO
    FP --> CDO
    FP --> CALC
    FP --> CN
    FC --> CDO
    FC --> CN
    FP -->|ShowDialog modal| FC
    FC -->|CargarClienteDesdeSeleccion| FP
    CALC --> PP
    PDO --> PP
    PDO --> CDO
    CDO --> CL
    PDO --> CN
    CDO --> CN
    CN --> DB
    S1 --> DB
    S2 --> DB
    S3 --> DB
    TEST --> PDO
    TEST --> CDO
```

---

## 2. Diagrama de clases

```mermaid
classDiagram
    class Program {
        +Main() STAThread
    }
    class FormPedido {
        -PedidoDAO pedidoDao
        -bool cargandoFormulario
        +FormPedido()
        -FormPedido_Load()
        -CargarPedidos()
        -ConstruirPedido() PedidoPizza
        -Calculo_Changed()
        -MostrarResumen(p)
        -LlenaForm(p)
        -vistaPedido(id)
        -LimpiarFormulario()
        -MostrarEstado(texto, exito)
        -ConfigurarAyuda()
        +CargarClienteDesdeSeleccion(nombre, telefono)
        -BtnCalcular_Click()
        -btnBuscaPedidosXCliente_Click()
        -btnBuscarPedidoPorId_Click()
        -btnTodosLosPedidos_Click()
        -btnActualizarPedido_Click()
        -btnEliminarPedido_Click()
        -btnProbarConexion_Click()
        -btnEditaClientes_Click()
        -BtnLimpiar_Click()
        -BtnSalir_Click()
        -dgvPedidos_CellClick()
    }
    class FormClientes {
        -ClienteDAO clienteDao
        -int idClienteSeleccionado
        +Cliente ClienteSeleccionado
        +FormClientes()
        -FormClientes_Load()
        -CargarClientes(criterio)
        -PrepararNuevoCliente()
        -presentaCamposParaEdicion()
        -btnGrabar_Click()
        -btnElimina_Click()
        -btnBuscar_Click()
        -btnSelecciona_Click()
        -btnNuevo_Click()
        -btnListar_Click()
        -dgvClientes_CellDoubleClick()
    }
    class PedidoDAO {
        +InsertarPedido(p) bool
        +ObtenerPedidosDataTable() DataTable
        +BuscarPedidosPorCliente(filtro) DataTable
        +ObtenerPedidoPorId(id) PedidoPizza
        +ActualizarPedido(p) bool
        +EliminarPedido(id) bool
        -ResolverIdCliente(nombre, telefono) int
        -CargarParametros(cmd, p)
    }
    class ClienteDAO {
        +ObtenerClientesDataTable() DataTable
        +BuscarPorNombreYTelefono(nombre, telefono) Cliente
        +insertarCliente(c) int
        +InsertarClienteConTransaccion(c, cn, tran) int
        +ObtenerOCrearIdCliente(nombre, telefono, cn, tran) int
        +ActualizarCliente(c) bool
        +EliminarCliente(id) bool
        +validaClienteSinPedidos(id) bool
        +BuscarClientesDataTable(criterio) DataTable
        +testInsertarCliente() string
        +ObtenerPedidosPorCliente(id) DataTable
        +ObtenerClientePorId(id) Cliente
        +EsClienteDuplicado(ex) bool static
    }
    class ConexionBD {
        +CadenaConexion string
        +ObtenerConexion() SqlConnection
        +ProbarConexion() bool
    }
    class CalculadoraPedido {
        +Tamanos IReadOnlyDictionary
        +Masas IReadOnlyDictionary
        +Calcular(datos) PedidoPizza
    }
    class DatosPedido {
        +Cliente, Telefono
        +Tamano, Masa
        +QuesoExtra, Pepperoni, Champinones, Aceitunas, Tocino
        +EsDelivery, AplicaConvenioEstudiante
    }
    class PedidoPizza {
        +IdPedido, IdCliente
        +Cliente, Telefono
        +TamanoNombre, PrecioBaseTamano
        +MasaNombre, RecargoMasa
        +5 flags ingredientes
        +EsDelivery, AplicaConvenioEstudiante
        +CostoIngredientes, SubtotalPizza, MontoDescuento
        +CostoDelivery, NetoAfecto, MontoIva, TotalPagar
    }
    class Cliente {
        +IdCliente
        +Nombre
        +Telefono
    }

    Program ..> FormPedido : crea y ejecuta
    FormPedido --> PedidoDAO : pedidoDao
    FormPedido --> ClienteDAO : EsClienteDuplicado
    FormPedido --> CalculadoraPedido
    FormPedido --> ConexionBD
    FormPedido ..> FormClientes : ShowDialog
    FormClientes --> FormPedido : CargarClienteDesdeSeleccion
    FormClientes --> ClienteDAO
    FormClientes --> ConexionBD
    PedidoDAO --> PedidoPizza : crea y devuelve
    PedidoDAO --> ClienteDAO : ResolverIdCliente
    PedidoDAO --> ConexionBD
    ClienteDAO --> Cliente
    ClienteDAO --> ConexionBD
    CalculadoraPedido *-- DatosPedido
    CalculadoraPedido ..> PedidoPizza : Calcular devuelve
```

---

## 3. Flujo de arranque (de `Program.cs` a la grilla)

```mermaid
sequenceDiagram
    autonumber
    participant P as Program.cs
    participant F as FormPedido
    participant D as Designer.cs
    participant C as CalculadoraPedido
    participant DAO as PedidoDAO
    participant CN as ConexionBD
    participant DB as PizzeriaDB
    participant UI as Controles

    P->>P: Main() [STAThread]
    P->>P: ApplicationConfiguration.Initialize()
    P->>F: new FormPedido()
    F->>D: InitializeComponent()
    D-->>UI: crea grpCliente, grpPizza, grpIngredientes, grpEntrega, grpResumen, dgvPedidos, lblAyuda
    D-->>UI: cablea Click, CheckedChanged, SelectedIndexChanged, CellClick, Load
    P->>F: Application.Run(form)
    UI->>F: Load → FormPedido_Load()
    F->>F: cargandoFormulario = true
    F->>C: Tamanos.Keys / Masas.Keys
    C-->>F: diccionarios
    F->>UI: cboTamano.Items = Personal / Mediana / Familiar → SelectedItem = Mediana
    F->>UI: cboMasa.Items = Traditional / Delgada / Borde de Queso → SelectedItem = Masa Tradicional
    F->>UI: grpResumen.Enabled = false; lbl*Valor = "$ 0 CLP"
    F->>F: cargandoFormulario = false; ConfigurarAyuda()
    F->>DAO: CargarPedidos() → ObtenerPedidosDataTable()
    DAO->>CN: ObtenerConexion()
    CN->>DB: SELECT con INNER JOIN dbo.Clientes ORDER BY IdPedido DESC
    DB-->>DAO: DataTable
    DAO-->>F: DataTable
    F->>UI: dgvPedidos.DataSource = DataTable
    F->>UI: oculta IdPedido e IdCliente; títulos Cliente, Teléfono, Tamaño, Total a Pagar
```

---

## 4. Flujo de cálculo en vivo (`Detalle de Pago`)

```mermaid
flowchart TD
    A["Usuario cambia cboTamano, cboMasa,<br/>chkQuesoExtra, chkPepperoni, chkChampinones,<br/>chkAceitunas, chkTocino, rdbDelivery, rdbRetiro,<br/>chkConvenioEstudiante"] --> B["SelectedIndexChanged / CheckedChanged<br/>→ Calculo_Changed()"]
    B --> C{"cargandoFormulario?"}
    C -->|sí| Z["return: evita recalcular al cargar o limpiar"]
    C -->|no| D{"grpResumen.Enabled?"}
    D -->|no| Z
    D -->|sí| E{"txtCliente vacío?"}
    E -->|sí| Z
    E -->|no| F["ConstruirPedido(): arma DatosPedido<br/>desde txtCliente, txtTelefono, cbo*, chk*, rdb*"]
    F --> G["CalculadoraPedido.Calcular(datos)"]
    G --> H["PedidoPizza con CostoIngredientes, Subtotal,<br/>Descuento 10%, Delivery 2000, IVA 19%, Total"]
    H --> I["MostrarResumen(p)"]
    I --> J["lblSubtotalValor, lblDescuentoValor,<br/>lblDeliveryValor, lblIvaValor, lblTotalValor"]
    J --> K["grpResumen.Enabled = true"]
```

**Reglas aplicadas por `Calcular()`:** tamaño (5.000 / 8.500 / 12.000) + masa (0 / 0 / 1.500) + $800 por extra; descuento 10% sobre subtotal; delivery $2.000; IVA 19%; total = neto + IVA.

---

## 5. Flujo completo: Calcular / Grabar (CREATE)

```mermaid
sequenceDiagram
    autonumber
    actor U as Usuario
    participant FP as FormPedido
    participant UI as Resumen/MessageBox/lblAyuda
    participant CALC as CalculadoraPedido
    participant PDO as PedidoDAO
    participant CDO as ClienteDAO
    participant CN as ConexionBD
    participant DB as PizzeriaDB

    U->>FP: click btnCalcular
    FP->>FP: valida txtCliente
    alt txtCliente vacío
        FP->>UI: MessageBox "Ingrese el nombre del cliente" + txtCliente.Focus()
    else válido
        FP->>FP: ConstruirPedido() lee controles
        FP->>CALC: Calcular(DatosPedido)
        CALC-->>FP: PedidoPizza con montos
        FP->>UI: MostrarResumen() → 5 labels + grpResumen.Enabled = true
        FP->>UI: MessageBox "¿Grabar pedido? Total: $..."
        alt confirma Sí
            FP->>PDO: InsertarPedido(p)
            PDO->>CDO: ResolverIdCliente(nombre, telefono)
            CDO->>DB: SELECT IdCliente WHERE Cliente y Telefono
            alt cliente existe
                DB-->>CDO: IdCliente existente
            else cliente nuevo
                CDO->>DB: INSERT Clientes + SCOPE_IDENTITY()
                DB-->>CDO: nuevo IdCliente
            end
            CDO-->>PDO: p.IdCliente
            PDO->>CN: ObtenerConexion() + BeginTransaction()
            PDO->>DB: INSERT Pedidos (21 columnas) + SELECT SCOPE_IDENTITY()
            DB-->>PDO: IdPedido generado
            PDO->>DB: COMMIT
            PDO-->>FP: true
            FP->>UI: lblAyuda verde + MessageBox "Pedido guardado"
            FP->>FP: LimpiarFormulario()
            FP->>PDO: CargarPedidos() → ObtenerPedidosDataTable()
            PDO-->>FP: DataTable actualizado
            FP->>UI: dgvPedidos.DataSource + ocultar IDs + títulos
        else SqlException 2601/2627
            DB-->>PDO: violación UQ_Clientes_Cliente_Telefono
            PDO-->>FP: excepción
            FP->>UI: lblAyuda roja + MessageBox "Cliente duplicado"
        else cancela
            FP->>FP: no inserta; el resumen queda visible
        end
    end
```

---

## 6. Flujo de búsquedas y carga de detalle (READ)

```mermaid
flowchart TD
    subgraph ENTRADAS["FormPedido — entradas"]
        B1["btnBuscaPedidosXCliente_Click<br/>lee txtCliente"]
        B2["btnBuscarPedidoPorId_Click<br/>lee txtIdPedido"]
        B3["btnTodosLosPedidos_Click"]
        B4["dgvPedidos_CellClick<br/>lee columna oculta IdPedido"]
    end

    B1 -->|"filtro vacío"| E1["lblAyuda roja + MessageBox validación + txtCliente.Focus()"]
    B1 -->|"filtro con valor"| D1["PedidoDAO.BuscarPedidosPorCliente(filtro)<br/>WHERE c.Cliente LIKE @Filtro"]
    D1 --> R1["DataTable → dgvPedidos.DataSource<br/>+ ocultar IDs + títulos + lblAyuda verde"]

    B2 --> V{"int.TryParse > 0?"}
    V -->|no| E2["MessageBox 'Ingrese un ID válido' + Focus"]
    V -->|sí| D2["PedidoDAO.ObtenerPedidoPorId(id)"]
    D2 --> D3["PedidoPizza o null"]
    D3 --> D4["DataTable temporal:<br/>IdPedido, Cliente, Telefono, TamanoNombre, TotalPagar"]
    D4 --> R2["dgvPedidos: 1 fila o vacía + títulos"]
    D3 -->|"si existe"| S1["vistaPedido(id) → ObtenerPedidoPorId → LlenaForm(p)"]
    D3 -->|"null"| E3["lblAyuda roja + MessageBox 'No se encontró'"]
    S1 --> R3["txtIdPedido, txtCliente, txtTelefono, cbo*, chk*, rdb*<br/>+ MostrarResumen(p)"]

    B3 --> D5["CargarPedidos() → ObtenerPedidosDataTable()"]
    D5 --> R4["dgvPedidos completo + títulos"]

    B4 --> S1
```

---

## 7. Flujo de UPDATE, DELETE y acciones auxiliares (FormPedido)

```mermaid
flowchart TD
    A["btnActualizarPedido_Click"] --> A1{"ID válido y txtCliente con dato?"}
    A1 -->|no| AX["MessageBox de validación"]
    A1 -->|sí| A2["ConstruirPedido() → p.IdPedido = id<br/>→ PedidoDAO.ActualizarPedido(p)"]
    A2 --> A3{"filas afectadas?"}
    A3 -->|sí| A4["MessageBox éxito + CargarPedidos() → dgv actualizado"]
    A3 -->|no| A5["MessageBox aviso"]
    A2 --> AZ["catch SqlException duplicado → lblAyuda roja + MessageBox"]

    B["btnEliminarPedido_Click"] --> B1{"ID válido y confirma Sí?"}
    B1 -->|no| BX["MessageBox validación / cancelación"]
    B1 -->|sí| B2["PedidoDAO.EliminarPedido(id)"]
    B2 --> B3["lblAyuda + MessageBox + LimpiarFormulario() + CargarPedidos()"]

    C["BtnLimpiar_Click"] --> C1["LimpiarFormulario():<br/>txt vacíos, Mediana, Tradicional, Retiro,<br/>checks false, labels $ 0, resumen deshabilitado, Focus cliente"]

    D["btnProbarConexion_Click"] --> D1["ConexionBD.ProbarConexion() → bool"]
    D1 --> D2["lblAyuda verde/roja + MessageBox con cadena y estado"]

    E["MouseEnter / MouseLeave en 9 botones"] --> E1["ConfigurarAyuda() → Vincular()<br/>→ MostrarEstado() → lblAyuda"]

    F["btnEditaClientes_Click"] --> F1["new FormClientes() + ShowDialog() modal"]

    G["btnConfirmar_Click → BtnSalir_Click"] --> G1{"confirma Sí?"}
    G1 -->|sí| G2["Close()"]
```

---

## 8. Flujo completo de `FormClientes` y su cruce a `FormPedido`

```mermaid
flowchart TD
    A["FormPedido.btnEditaClientes_Click"] --> B["new FormClientes().ShowDialog()"]
    B --> L["FormClientes_Load"]
    L --> L1["CargarClientes() → ClienteDAO.ObtenerClientesDataTable()<br/>→ dgvClientes.DataSource + encabezados"]
    L --> L2["PrepararNuevoCliente(): txt vacíos, idClienteSeleccionado=0,<br/>btnGrabar.Text = Grabar"]

    G["btnGrabar_Click<br/>cableado en el constructor"] --> G1{"txtNombre o txtTelefono vacíos?"}
    G1 -->|sí| GX["MessageBox 'Datos incompletos'"]
    G1 -->|no| G2["BuscarPorNombreYTelefono() → Cliente?"]
    G2 -->|"existe otro id"| GY["MessageBox 'Cliente duplicado'<br/>+ txtTelefono.Focus()"]
    G2 -->|"no existe"| G3{"idClienteSeleccionado = 0?"}
    G3 -->|sí| G4["ClienteDAO.insertarCliente() → SCOPE_IDENTITY()"]
    G3 -->|no| G5["ClienteDAO.ActualizarCliente() → bool"]
    G4 --> G7["MessageBox éxito → PrepararNuevoCliente() → CargarClientes()"]
    G5 --> G7
    G7 --> GZ["catch 2601/2627 → MessageBox 'Cliente duplicado'"]

    E["btnElimina_Click"] --> E1{"fila seleccionada?"}
    E1 -->|no| EX["MessageBox 'Seleccione un cliente'"]
    E1 -->|sí| E2["validaClienteSinPedidos(id) → bool"]
    E2 -->|false| EY["MessageBox 'Acción Denegada'"]
    E2 -->|true| E3{"confirma Sí?"}
    E3 -->|sí| E4["EliminarCliente(id) → MessageBox + recargar grilla"]

    D["dgvClientes.CellDoubleClick<br/>cableado en el constructor"] --> D1["presentaCamposParaEdicion():<br/>txtNombre, txtTelefono, idClienteSeleccionado,<br/>btnGrabar.Text = Actualizar"]

    N["btnNuevo_Click"] --> N1["PrepararNuevoCliente()"]
    BA["btnBuscar_Click"] --> BA1["CargarClientes(criterio)<br/>→ BuscarClientesDataTable() LIKE nombre o teléfono"]

    S["btnSelecciona_Click"] --> S1{"txtNombre y txtTelefono con datos?"}
    S1 -->|no| SX["MessageBox 'Datos incompletos' + Focus"]
    S1 -->|sí| S2{"¿FormPedido abierto?"}
    S2 -->|sí| S3["frmPedido.CargarClienteDesdeSeleccion(nombre, telefono)<br/>+ BringToFront() + this.Close()"]
    S2 -->|no| S4["new FormPedido().Show()<br/>+ CargarClienteDesdeSeleccion()"]
    S3 --> FP["FormPedido: txtCliente, txtTelefono,<br/>lblAyuda verde, txtCliente.Focus()"]
    S4 --> FP

    LI["btnListar_Click<br/>cableado en el Designer"] --> LX["PrepararNuevoCliente() + CargarClientes()<br/>→ txt limpios + dgvClientes recargada"]
```

> **Nota:** `btnListar_Click()` está cableado en `FormClientes.Designer.cs` mediante `btnListar.Click += btnListar_Click;`, igual que los otros botones del grupo.

---

## 9. Mapa de objetos → respuesta en pantalla

### FormPedido

| Componente (control) | Evento | Método en la clase | Objeto / dato devuelto | Respuesta visible |
|---|---|---|---|---|
| Form (Load) | `Load` | `FormPedido_Load` | `Dictionary.Keys`, `DataTable` | Combos cargados, resumen `$ 0 CLP`, grilla con pedidos |
| `cboTamano`, `cboMasa`, 5 `chk*`, `rdb*`, `chkConvenioEstudiante` | `SelectedIndexChanged` / `CheckedChanged` | `Calculo_Changed` → `ConstruirPedido` → `Calcular` | `DatosPedido`, `PedidoPizza` | 5 `lbl*Valor` del `grpResumen` |
| `btnCalcular` | `Click` | `BtnCalcular_Click` | `PedidoPizza`, `bool`, `DataTable` | MessageBox de total/confirmación/éxito o duplicado; resumen; grilla; limpieza; `lblAyuda` |
| `btnBuscaPedidosXCliente` | `Click` | `btnBuscaPedidosXCliente_Click` | `DataTable` | `dgvPedidos` filtrado + `lblAyuda` + validación |
| `btnBuscarPedidoPorId` | `Click` | `btnBuscarPedidoPorId_Click` + `vistaPedido`/`LlenaForm` | `PedidoPizza`, `DataTable` temporal | Grilla de 1 fila + **todos** los campos del form + resumen |
| `btnTodosLosPedidos` | `Click` | `btnTodosLosPedidos_Click` → `CargarPedidos` | `DataTable` | Grilla completa |
| `dgvPedidos` | `CellClick` | `dgvPedidos_CellClick` → `vistaPedido` → `LlenaForm` | `PedidoPizza` | Formulario y resumen cargados con el pedido elegido |
| `btnActualizarPedido` | `Click` | `btnActualizarPedido_Click` | `bool`, excepción | MessageBox de resultado/duplicado + grilla recargada |
| `btnEliminarPedido` | `Click` | `btnEliminarPedido_Click` | `bool` | MessageBox + `lblAyuda` + limpieza + grilla |
| `btnProbarConexion` | `Click` | `btnProbarConexion_Click` | `bool` | `lblAyuda` con color + MessageBox con cadena/estado |
| `btnEditaClientes` | `Click` | `btnEditaClientes_Click` | nueva ventana modal | Abre `FormClientes` |
| `btnLimpiar` | `Click` | `BtnLimpiar_Click` → `LimpiarFormulario` | — | Controles y montos reiniciados |
| `btnConfirmar` | `Click` | `BtnSalir_Click` | `DialogResult` | Cierre de la app |
| 9 botones (hover) | `MouseEnter/Leave` | `ConfigurarAyuda` → `MostrarEstado` | `string` | `lblAyuda` con texto de ayuda |

### FormClientes

| Componente (control) | Evento | Método en la clase | Objeto / dato devuelto | Respuesta visible |
|---|---|---|---|---|
| Form (Load) | `Load` | `FormClientes_Load` | `DataTable` | `dgvClientes` listado + `PrepararNuevoCliente()` |
| `btnGrabar` | `Click` (en constructor) | `btnGrabar_Click` | `Cliente?`, `int`, `bool` | MessageBox de éxito/duplicado; grilla recargada; botón vuelve a "Grabar" |
| `btnElimina` | `Click` | `btnElimina_Click` | `bool` | MessageBox "Acción Denegada"/confirmación/éxito; grilla recargada |
| `btnBuscar` | `Click` | `btnBuscar_Click` → `CargarClientes(criterio)` | `DataTable` | `dgvClientes` filtrado por nombre o teléfono |
| `btnNuevo` | `Click` | `btnNuevo_Click` → `PrepararNuevoCliente` | — | TextBox limpios, `idClienteSeleccionado = 0` |
| `btnSelecciona` | `Click` | `btnSelecciona_Click` | `string, string` | En `FormPedido`: `txtCliente`, `txtTelefono`, `lblAyuda` verde |
| `dgvClientes` | `CellDoubleClick` (en constructor) | `presentaCamposParaEdicion` | fila del `DataGridView` | TextBox cargados y `btnGrabar.Text = "Actualizar"` |
| `btnListar` | `Click` (en Designer) | `btnListar_Click` → `PrepararNuevoCliente` + `CargarClientes` | `DataTable` | TextBox limpios y `dgvClientes` recargada |

---

## 10. Modelo de datos (donde terminan todos los flujos)

```mermaid
erDiagram
    CLIENTES ||--o{ PEDIDOS : "IdCliente — FK, ON UPDATE CASCADE, ON DELETE NO ACTION"
    CLIENTES {
        int IdCliente PK
        nvarchar Cliente UK
        nvarchar Telefono UK
    }
    PEDIDOS {
        int IdPedido PK
        int IdCliente FK
        nvarchar Cliente
        nvarchar Telefono
        nvarchar TamanoNombre
        int PrecioBaseTamano
        nvarchar MasaNombre
        int RecargoMasa
        bit QuesoExtra
        bit Pepperoni
        bit Champinones
        bit Aceitunas
        bit Tocino
        bit EsDelivery
        bit AplicaConvenioEstudiante
        int CostoIngredientes
        int SubtotalPizza
        int MontoDescuento
        int CostoDelivery
        int NetoAfecto
        int MontoIva
        int TotalPagar
        datetime FechaCreacion
    }
```

`UK` en `Cliente` y `Telefono` representa la restricción `UQ_Clientes_Cliente_Telefono` aplicada a la base real.

---

## Resumen del viaje de los datos

1. **Entrada del usuario** → controles WinForms declarados en `*.Designer.cs`.
2. **Evento** → método `*_Click` / `*_Changed` en `FormPedido.cs` o `FormClientes.cs`.
3. **Transformación** → `DatosPedido` → `CalculadoraPedido.Calcular()` → **`PedidoPizza`** (objeto con los montos).
4. **Persistencia** → `PedidoDAO` / `ClienteDAO` → `ConexionBD.ObtenerConexion()` → SQL parametrizado → `PizzeriaDB`.
5. **Respuesta en pantalla** →
   - `DataTable` → `dgvPedidos` / `dgvClientes`;
   - `PedidoPizza` → `LlenaForm` + `MostrarResumen` (5 labels);
   - `bool` / excepción → `MessageBox` (incluido el de **cliente duplicado**);
   - `string` → `lblAyuda` con color verde/rojo/blanco.
