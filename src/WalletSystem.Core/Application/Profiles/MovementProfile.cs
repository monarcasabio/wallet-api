using AutoMapper;
using WalletSystem.Core.Application.DTOs.Movement;
using WalletSystem.Core.Domain.Entities;

namespace WalletSystem.Core.Application.Profiles;

public class MovementProfile : Profile
{
    public MovementProfile()
    {
        CreateMap<Movement, MovementDto>();
        CreateMap<CreateMovementDto, Movement>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
    }
}