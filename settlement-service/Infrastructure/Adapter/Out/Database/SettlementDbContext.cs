using Microsoft.EntityFrameworkCore;
using SettlementService.Domain.Model;

namespace SettlementService.Infrastructure.Adapter.Out.Database;

// DbContext é a classe base do EF Core. Equivalente a configurar o EntityManager/Session do Hibernate.
public class SettlementDbContext : DbContext
{
    // O DbSet representa a tabela no banco. É por ele que faremos o equivalente ao JpaRepository.
    public DbSet<Settlement> Settlements { get; set; }

    // Construtor: Recebe as credenciais/conexão do Program.cs e passa para a classe "pai" (base)
    public SettlementDbContext(DbContextOptions<SettlementDbContext> options) : base(options)
    {
    }

    // O equivalente ao "Fluent API". Onde configuramos as colunas sem sujar a entidade.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Settlement>(entity =>
        {
            entity.ToTable("Settlements"); // Nome da tabela
            
            entity.HasKey(s => s.Id); // Define a Chave Primária (@Id)
            
            entity.Property(s => s.PixTransactionId)
                  .IsRequired()
                  .HasMaxLength(100); // @Column(nullable = false, length = 100)
            
            entity.Property(s => s.Amount)
                  .HasPrecision(18, 2); // BigDecimal mapeado corretamente para dinheiro (18 casas, 2 decimais)
            
            // Converte o Enum do C# para texto puro no banco (Pending, Processed)
            entity.Property(s => s.Status)
                  .HasConversion<string>(); 
        });
    }
}