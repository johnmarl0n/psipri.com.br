using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using psipri.com.br.Models;
using psipri.com.br.Models.PDM;

namespace psipri.com.br.Data
{
    /// <summary>
    /// Database context for the application, integrating Identity for security.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Table for blog posts.
        /// </summary>
        public DbSet<BlogPost> BlogPosts { get; set; }

        /// <summary>
        /// Table for dynamic site content (e.g., "About" section).
        /// </summary>
        public DbSet<SiteContent> SiteContents { get; set; }

        // --- Pingo de Mel (PDM) Tables ---
        public DbSet<PDMCategoria> PDMCategorias { get; set; }
        public DbSet<PDMProduto> PDMProdutos { get; set; }
        public DbSet<PDMReceita> PDMReceitas { get; set; }
        public DbSet<PDMReceitaItem> PDMReceitaItems { get; set; }
        public DbSet<PDMProducao> PDMProducoes { get; set; }
        public DbSet<PDMCliente> PDMClientes { get; set; }
        public DbSet<PDMVenda> PDMVendas { get; set; }
        public DbSet<PDMVendaItem> PDMVendaItems { get; set; }
        public DbSet<PDMFechamento> PDMFechamentos { get; set; }
        public DbSet<PDMFechamentoItem> PDMFechamentoItems { get; set; }
        public DbSet<PDMHistoricoPreco> PDMHistoricoPrecos { get; set; }
        public DbSet<PDMConfiguracao> PDMConfiguracoes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed initial site content if needed
            builder.Entity<SiteContent>().HasData(
                new SiteContent { Id = 1, Key = "AboutMe", Value = "<p>Sou uma psicóloga especializada em Psicologia Jurídica...</p>" }
            );

            // Configure Decimal Precisions for PDM Tables
            builder.Entity<PDMProduto>(entity =>
            {
                entity.Property(e => e.QuantidadeEstoque).HasPrecision(18, 4);
                entity.Property(e => e.EstoqueMinimo).HasPrecision(18, 4);
                entity.Property(e => e.PrecoCusto).HasPrecision(18, 2);
                entity.Property(e => e.Margem).HasPrecision(18, 2);
            });

            builder.Entity<PDMReceita>(entity =>
            {
                entity.Property(e => e.MargemLucro).HasPrecision(18, 2);
            });

            builder.Entity<PDMReceitaItem>(entity =>
            {
                entity.Property(e => e.Quantidade).HasPrecision(18, 4);
            });

            builder.Entity<PDMProducao>(entity =>
            {
                entity.Property(e => e.CustoTotalProducao).HasPrecision(18, 2);
            });

            builder.Entity<PDMVenda>(entity =>
            {
                entity.Property(e => e.ValorTotal).HasPrecision(18, 2);
                entity.Property(e => e.Desconto).HasPrecision(18, 2);
                entity.Property(e => e.ValorFinal).HasPrecision(18, 2);
            });

            builder.Entity<PDMVendaItem>(entity =>
            {
                entity.Property(e => e.PrecoUnitario).HasPrecision(18, 2);
                entity.Property(e => e.Subtotal).HasPrecision(18, 2);
            });

            builder.Entity<PDMFechamento>(entity =>
            {
                entity.Property(e => e.ValorTotalEstoque).HasPrecision(18, 2);
            });

            builder.Entity<PDMFechamentoItem>(entity =>
            {
                entity.Property(e => e.QuantidadeEstoque).HasPrecision(18, 4);
                entity.Property(e => e.PrecoCusto).HasPrecision(18, 2);
                entity.Property(e => e.ValorTotal).HasPrecision(18, 2);
            });

            builder.Entity<PDMHistoricoPreco>(entity =>
            {
                entity.Property(e => e.PrecoAnterior).HasPrecision(18, 2);
                entity.Property(e => e.PrecoNovo).HasPrecision(18, 2);
            });

            // Seed default Categories
            builder.Entity<PDMCategoria>().HasData(
                new PDMCategoria { Id = 1, Nome = "Ceras e Parafinas", Icone = "fa-fire" },
                new PDMCategoria { Id = 2, Nome = "Essências e Cheiros", Icone = "fa-flask" },
                new PDMCategoria { Id = 3, Nome = "Pavios e Suportes", Icone = "fa-grip-lines-vertical" },
                new PDMCategoria { Id = 4, Nome = "Corantes e Pigmentos", Icone = "fa-palette" },
                new PDMCategoria { Id = 5, Nome = "Embalagens e Potes", Icone = "fa-box-open" },
                new PDMCategoria { Id = 6, Nome = "Fitas e Enfeites", Icone = "fa-ribbon" },
                new PDMCategoria { Id = 7, Nome = "Outros Insumos", Icone = "fa-ellipsis-h" }
            );

            // Seed default Configuration (Margem Operacional de 15%)
            builder.Entity<PDMConfiguracao>().HasData(
                new PDMConfiguracao { Id = 1, Chave = "MargemOperacional", Valor = "15.00" }
            );
        }
    }
}
