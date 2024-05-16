using ExemploWebAppMVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace ExemploWebAppMVC.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Cargo> Cargos { get; set; }
    }
}

