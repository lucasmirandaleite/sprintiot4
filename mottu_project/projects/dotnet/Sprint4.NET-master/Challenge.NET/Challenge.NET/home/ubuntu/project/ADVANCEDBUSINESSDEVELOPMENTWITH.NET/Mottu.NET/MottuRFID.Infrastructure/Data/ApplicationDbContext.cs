using Microsoft.EntityFrameworkCore;
using MottuRFID.Domain.Entities;

namespace MottuRFID.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Moto> Motos { get; set; }
        public DbSet<Filial> Filiais { get; set; }
        public DbSet<PontoLeitura> PontosLeitura { get; set; }
        public DbSet<LeituraRFID> LeiturasRFID { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração da entidade Moto
            modelBuilder.Entity<Moto>(entity =>
            {
                entity.ToTable("MOTOS");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                entity.Property(e => e.Placa).HasColumnName("PLACA").IsRequired().HasMaxLength(10);
                entity.Property(e => e.Modelo).HasColumnName("MODELO").IsRequired().HasMaxLength(100);
                entity.Property(e => e.Cor).HasColumnName("COR").HasMaxLength(50);
                entity.Property(e => e.NumeroSerie).HasColumnName("NUMERO_SERIE").HasMaxLength(50);
                entity.Property(e => e.TagRFID).HasColumnName("TAG_RFID").IsRequired().HasMaxLength(50);
                entity.Property(e => e.FilialId).HasColumnName("FILIAL_ID");
                entity.Property(e => e.PontoLeituraAtualId).HasColumnName("PONTO_LEITURA_ATUAL_ID");
                entity.Property(e => e.UltimaAtualizacao).HasColumnName("ULTIMA_ATUALIZACAO");
                entity.Property(e => e.Ativa).HasColumnName("ATIVA");

                entity.HasOne(e => e.Filial)
                    .WithMany(f => f.Motos)
                    .HasForeignKey(e => e.FilialId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.PontoLeituraAtual)
                    .WithMany(p => p.MotosAtuais)
                    .HasForeignKey(e => e.PontoLeituraAtualId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configuração da entidade Filial
            modelBuilder.Entity<Filial>(entity =>
            {
                entity.ToTable("FILIAIS");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                entity.Property(e => e.Nome).HasColumnName("NOME").IsRequired().HasMaxLength(100);
                entity.Property(e => e.Endereco).HasColumnName("ENDERECO").HasMaxLength(200);
                entity.Property(e => e.Cidade).HasColumnName("CIDADE").HasMaxLength(100);
                entity.Property(e => e.Estado).HasColumnName("ESTADO").HasMaxLength(50);
                entity.Property(e => e.Pais).HasColumnName("PAIS").HasMaxLength(50);
                entity.Property(e => e.CodigoFilial).HasColumnName("CODIGO_FILIAL").IsRequired().HasMaxLength(20);
            });

            // Configuração da entidade PontoLeitura
            modelBuilder.Entity<PontoLeitura>(entity =>
            {
                entity.ToTable("PONTOS_LEITURA");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                entity.Property(e => e.Nome).HasColumnName("NOME").IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descricao).HasColumnName("DESCRICAO").HasMaxLength(200);
                entity.Property(e => e.Localizacao).HasColumnName("LOCALIZACAO").HasMaxLength(100);
                entity.Property(e => e.FilialId).HasColumnName("FILIAL_ID");
                entity.Property(e => e.PosicaoX).HasColumnName("POSICAO_X");
                entity.Property(e => e.PosicaoY).HasColumnName("POSICAO_Y");
                entity.Property(e => e.Ativo).HasColumnName("ATIVO");

                entity.HasOne(e => e.Filial)
                    .WithMany(f => f.PontosLeitura)
                    .HasForeignKey(e => e.FilialId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuração da entidade LeituraRFID
            modelBuilder.Entity<LeituraRFID>(entity =>
            {
                entity.ToTable("LEITURAS_RFID");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                entity.Property(e => e.TagRFID).HasColumnName("TAG_RFID").IsRequired().HasMaxLength(50);
                entity.Property(e => e.MotoId).HasColumnName("MOTO_ID");
                entity.Property(e => e.PontoLeituraId).HasColumnName("PONTO_LEITURA_ID");
                entity.Property(e => e.DataHoraLeitura).HasColumnName("DATA_HORA_LEITURA");
                entity.Property(e => e.Observacao).HasColumnName("OBSERVACAO").HasMaxLength(200);

                entity.HasOne(e => e.Moto)
                    .WithMany(m => m.Leituras)
                    .HasForeignKey(e => e.MotoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.PontoLeitura)
                    .WithMany(p => p.Leituras)
                    .HasForeignKey(e => e.PontoLeituraId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
