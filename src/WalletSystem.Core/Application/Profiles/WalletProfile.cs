using AutoMapper;
using WalletSystem.Core.Application.DTOs.Movement;
using WalletSystem.Core.Application.DTOs.Wallet;
using WalletSystem.Core.Domain.Entities;

namespace WalletSystem.Core.Application.Profiles;

public class WalletProfile : Profile
{
    public WalletProfile()
    {
        CreateMap<Wallet, WalletDto>();
        CreateMap<CreateWalletDto, Wallet>()
        .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
        .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
        .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
    }
}