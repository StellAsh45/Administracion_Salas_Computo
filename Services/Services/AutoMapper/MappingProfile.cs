using AutoMapper;
using Domain;
using Services.Models.CowModels;
using Services.Models.FarmModels;
using Services.Models.MilkModels;
using Services.Models.ModelosComputador;
using Services.Models.ModelosReporte;
using Services.Models.ModelosSala;
using Services.Models.ModelosSolicitud;
using Services.Models.ModelosUsuario;

namespace Services.Automapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            FarmMapper();
            MilkMapper();
            CowMapper();
            SalaMapper();
            ComputadorMapper();
            UsuarioMapper();
            SolicitudMapper();
            ReporteMapper();
        }

        private void MilkMapper()
        {
            CreateMap<Milk, MilkModel>().ReverseMap();
        }

        private void CowMapper()
        {
            CreateMap<Cow, CowModel>().ReverseMap();
            CreateMap<Cow, AddCowModel>().ReverseMap();
        }

        private void FarmMapper()
        {
            CreateMap<Farm, FarmModel>()
                .ForMember(dest => dest.CowCount,
                           opt => opt.MapFrom(src => src.Cows != null ? src.Cows.Count : 0))
                .ForMember(dest => dest.TotalMilkLitters, opt => opt.MapFrom(src => src.getTotalLitters()))
                .ReverseMap();

            CreateMap<Farm, AddFarmModel>().ReverseMap();
        }

        private void SalaMapper()
        {
            CreateMap<Sala, ModeloSala>().ReverseMap();
            CreateMap<Sala, AñadirModeloSala>().ReverseMap();
        }

        private void ComputadorMapper()
        {
            CreateMap<Computador, ModeloComputador>().ReverseMap();
            CreateMap<Computador, AñadirModeloComputador>().ReverseMap();
        }

        private void UsuarioMapper()
        {
            CreateMap<Usuario, ModeloUsuario>().ReverseMap();
            CreateMap<Usuario, AñadirModeloUsuario>().ReverseMap();
        }

        private void SolicitudMapper()
        {
            CreateMap<Solicitud, ModeloSolicitud>().ReverseMap();
            CreateMap<Solicitud, AñadirModeloSolicitud>().ReverseMap();
        }

        private void ReporteMapper()
        {
            CreateMap<Reporte, ModeloReporte>().ReverseMap();
            CreateMap<Reporte, AñadirModeloReporte>().ReverseMap();
        }
    }
}
