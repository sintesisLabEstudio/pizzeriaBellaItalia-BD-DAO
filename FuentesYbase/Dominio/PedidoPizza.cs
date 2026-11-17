namespace PizzeriaBellaItalia.Dominio;

// Entidad/modelo de pedido; sus propiedades mapean las columnas de dbo.Pedidos.
public class PedidoPizza
{
    public int IdPedido { get; set; }                         // PK (IDENTITY).

    // Relación con el cliente (FK a dbo.Clientes.IdCliente).
    public int IdCliente { get; set; }                        // FK (obligatoria en la BD).

    // Datos del cliente.
    public string Cliente { get; set; } = "";
    public string Telefono { get; set; } = "";

    // Configuración de pizza.
    public string TamanoNombre { get; set; } = "Mediana";
    public int PrecioBaseTamano { get; set; } = 8500;
    public string MasaNombre { get; set; } = "Masa Tradicional";
    public int RecargoMasa { get; set; } = 0;

    // Ingredientes adicionales (flags).
    public bool QuesoExtra { get; set; }
    public bool Pepperoni { get; set; }
    public bool Champinones { get; set; }
    public bool Aceitunas { get; set; }
    public bool Tocino { get; set; }

    // Entrega y promoción.
    public bool EsDelivery { get; set; }
    public bool AplicaConvenioEstudiante { get; set; }

    // Montos calculados y persistidos.
    public int CostoIngredientes { get; set; }
    public int SubtotalPizza { get; set; }
    public int MontoDescuento { get; set; }
    public int CostoDelivery { get; set; }
    public int NetoAfecto { get; set; }
    public int MontoIva { get; set; }
    public int TotalPagar { get; set; }   
}