using System.Collections.Generic;

namespace PizzeriaBellaItalia.Dominio;

// Reglas de negocio y cálculo de pedidos (desacopladas de la UI).
public static class CalculadoraPedido
{
    private const string TamanoPorDefecto = "Mediana";
    private const string MasaPorDefecto = "Masa Tradicional";
    // Opciones de tamaño: nombre -> precio base (CLP).
    public static readonly IReadOnlyDictionary<string, int> Tamanos = new Dictionary<string, int>
    {
        { "Personal", 5000 },
        { "Mediana", 8500 },
        { "Familiar", 12000 }
    };
    // Opciones de masa: nombre -> recargo (CLP).
    public static readonly IReadOnlyDictionary<string, int> Masas = new Dictionary<string, int>
    {
        { "Masa Tradicional", 0 },
        { "Masa Delgada", 0 },
        { "Borde de Queso", 1500 }
    };
    // Datos de entrada para el cálculo (sin dependencia de controles de UI).
    public sealed class DatosPedido // sealed: indica que no se puede heredar
    {
        public string Cliente { get; set; } = "";
        public string Telefono { get; set; } = "";
        public string Tamano { get; set; } = TamanoPorDefecto;
        public string Masa { get; set; } = MasaPorDefecto;
        public bool QuesoExtra { get; set; }
        public bool Pepperoni { get; set; }
        public bool Champinones { get; set; }
        public bool Aceitunas { get; set; }
        public bool Tocino { get; set; }
        public bool EsDelivery { get; set; }
        public bool AplicaConvenioEstudiante { get; set; }
    }
    // Aplica reglas y retorna un PedidoPizza con montos calculados.
    public static PedidoPizza Calcular(DatosPedido d)
    {
        // Si el tamaño o la masa no existen, se usa el valor por defecto y se guarda ese nombre,
        // para que el nombre persistido siempre coincida con el precio aplicado.
        string tamano = Tamanos.ContainsKey(d.Tamano) ? d.Tamano : TamanoPorDefecto;
        string masa = Masas.ContainsKey(d.Masa) ? d.Masa : MasaPorDefecto;
        int precioBase = Tamanos[tamano];
        int recargoMasa = Masas[masa];
        int extras = (d.QuesoExtra ? 1 : 0) + (d.Pepperoni ? 1 : 0) +
                     (d.Champinones ? 1 : 0) + (d.Aceitunas ? 1 : 0) + 
                     (d.Tocino ? 1 : 0);
        int costoIng = extras * 800;
        int subtotal = precioBase + recargoMasa + costoIng;
        int descuento = d.AplicaConvenioEstudiante ? (int)(subtotal * 0.10) : 0;
        int delivery = d.EsDelivery ? 2000 : 0;
        int neto = subtotal - descuento + delivery;
        int iva = (int)(neto * 0.19);
        int total = neto + iva;
        return new PedidoPizza // retorna un objeto de tipo PedidoPizza con los datos calculados
        {
            Cliente = d.Cliente,
            Telefono = d.Telefono,
            TamanoNombre = tamano,
            PrecioBaseTamano = precioBase,
            MasaNombre = masa,
            RecargoMasa = recargoMasa,
            QuesoExtra = d.QuesoExtra,
            Pepperoni = d.Pepperoni,
            Champinones = d.Champinones,
            Aceitunas = d.Aceitunas,
            Tocino = d.Tocino,
            EsDelivery = d.EsDelivery,
            AplicaConvenioEstudiante = d.AplicaConvenioEstudiante,
            CostoIngredientes = costoIng,
            SubtotalPizza = subtotal,
            MontoDescuento = descuento,
            CostoDelivery = delivery,
            NetoAfecto = neto,
            MontoIva = iva,
            TotalPagar = total
        };
    }
}
