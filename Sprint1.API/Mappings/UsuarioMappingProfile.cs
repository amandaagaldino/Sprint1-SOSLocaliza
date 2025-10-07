using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sprint1.Domain.Entities;
using Sprint1.DTOs.Usuario;

namespace Sprint1.Mappings;

public class UsuarioMapping : IEntityTypeConfiguration<Usuario>
{

    public void Configure(EntityTypeBuilder<Usuario> b)
    {
        b.ToTable("T_SOS_USUARIO");
        
        b.HasKey(x => x.Id);
        
        b.Property(x => x.Id)
            .ValueGeneratedNever();
        
        b.Property(x => x.NomeCompleto)
            .HasMaxLength(100)
            .IsRequired();
        
        b.Property(x => x.Email)
            .HasMaxLength(100)
            .IsRequired();
        
        b.HasIndex(x => x.Email)
            .IsUnique();
        
        b.Property(x => x.Senha)
            .HasMaxLength(30)
            .IsRequired();
        
        b.Property(x => x.Cpf)
            .HasMaxLength(11)
            .IsRequired();
        
        b.HasIndex(x => x.Cpf)
            .IsUnique();
        
        b.Property(x => x.DataNascimento)
            .HasColumnType("date")
            .IsRequired();
        
        
        b.Property(x => x.DataCriacao)
            .HasColumnType("date")
            .IsRequired();

        b.Property(x => x.DataAtualizacao)
            .HasColumnType("date")
            .IsRequired();
        
        b.Property(x => x.Ativo)
            .IsRequired();
    }
}