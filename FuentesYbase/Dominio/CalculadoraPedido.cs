using System.Collections.Generic;

namespace PizzeriaBellaItalia.Dominio;

// Reglas de negocio y cálculo de pedidos (desacopladas de la UI).
public static class CalculadoraPedido
{
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
    public sealed class DatosPedido //sealed: indica que no se puede heredar
    {
        public string Cliente { get; set; } = "";
        public string Telefono { get; set; } = "";
        public string Tamano { get; set; } = "Mediana";
        public string Masa { get; set; } = "Masa Tradicional";
        public bool QuesoExtra { get; set; }
        public bool Pepperoni { get; set; }
        public bool Champinones { get; set; }
        public bool Aceitunas { get; set; }
        public bool Tocino { get; set; }
        public bool EsDelivery { get; set; }
        public bool AplicaConvenioEstudiante { get; set; }
    }

    // Aplica reglas y retorna un PedidoPizza con montos calculados.
    //static significa que el miembro (o la clase) pertenece al tipo en sí, no a una instancia/objeto creado con new.
    public static PedidoPizza Calcular(DatosPedido d)
    {
        int precioBase = Tamanos.TryGetValue(d.Tamano, out int pb) ? pb : Tamanos["Mediana"];
        int recargoMasa = Masas.TryGetValue(d.Masa, out int rm) ? rm : 0;

        int extras = (d.QuesoExtra ? 1 : 0) + (d.Pepperoni ? 1 : 0) +
                     (d.Champinones ? 1 : 0) + (d.Aceitunas ? 1 : 0) + (d.Tocino ? 1 : 0);
        int costoIng = extras * 800;

        int subtotal = precioBase + recargoMasa + costoIng;
        int descuento = d.AplicaConvenioEstudiante ? (int)(subtotal * 0.10) : 0;
        int delivery = d.EsDelivery ? 2000 : 0;
        int neto = subtotal - descuento + delivery;
        int iva = (int)(neto * 0.19);
        int total = neto + iva;

        return new PedidoPizza //retorna un objeto de tipo PedidoPizza con los datos calculados
        {
            Cliente = d.Cliente,
            Telefono = d.Telefono,
            TamanoNombre = d.Tamano,
            PrecioBaseTamano = precioBase,
            MasaNombre = d.Masa,
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
