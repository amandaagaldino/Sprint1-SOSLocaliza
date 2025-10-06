using AutoMapper;
using Sprint1.Domain.Entities;
using Sprint1.DTOs.Usuario;

namespace Sprint1.Mappings;

public class UsuarioMappingProfile : Profile
{
    public UsuarioMappingProfile()
    {
        CreateMap<CreateUsuarioDto, Usuario>()
            .ConstructUsing(dto => new Usuario(
                dto.NomeCompleto,
                dto.Email,
                dto.Senha,
                dto.DataNascimento,
                dto.Cpf
            ));

        CreateMap<Usuario, UsuarioResponseDto>();
    }
}
