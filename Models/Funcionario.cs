namespace ExemploWebAppMVC.Models;
public class Funcionario
{
    public int Id { get; set; }
    public string? Nome { get; set; }
    public int Idade { get; set; }
    public int CargoId { get; set; }
    public Cargo? Cargo { get; set; }
    public int EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }
}
