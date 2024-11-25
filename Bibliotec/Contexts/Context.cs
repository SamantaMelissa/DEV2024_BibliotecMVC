using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Bibliotec.Models;

namespace Bibliotec.Contexts
{
    // Classe Context herda de DbContext para gerenciar a interação com o banco de dados.
    public class Context : DbContext
    {
        public Context()
        {

        }
        // Construtor que permite configurar o DbContext com opções fornecidas externamente.
        public Context(DbContextOptions<Context> options) : base(options)
        {

        }

        // Método para configurar opções do contexto, como a string de conexão.
        //Este override está sobrescrevendo o método onConfiguring
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Verifica se as opções já foram configuradas; se não, configura.
            if (!optionsBuilder.IsConfigured)
            {
                // Configuração do SQL Server com a string de conexão especificada.
                // Inclui opções como segurança integrada e certificado de confiança.
                optionsBuilder.UseSqlServer("Data Source=DESKTOP-LAO5MIJ\\SQLEXPRESSTEC; Initial Catalog = Bibliotec; User Id=sa; Password=abc123; Integrated Security=true; TrustServerCertificate = true");
            }
        }

        // Propriedades que representam tabelas no banco de dados.
        // Cada DbSet corresponde a uma entidade (classe) que será mapeada para o banco.
        public DbSet<Curso> Curso { get; set; }
        public DbSet<Livro> Livro { get; set; }
        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<LivroCategoria> LivroCategoria { get; set; }
        public DbSet<LivroReserva> LivroReserva { get; set; }
        // public DbSet<LivroFavorito> LivroFavorito { get; set; }

    }
}
