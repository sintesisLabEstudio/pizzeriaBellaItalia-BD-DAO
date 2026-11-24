namespace PizzeriaBellaItalia.Dominio;
public class Cliente
{
    public int IdCliente { get; set; }        // PK (IDENTITY) de dbo.Clientes.
    public string Nombre { get; set; } = "";
    public string Telefono { get; set; } = "";
}
