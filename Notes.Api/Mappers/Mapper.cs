using AutoMapper;

namespace Notes.Api.Mappers
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Database.Entities.ApplicationUser, Models.ApplicationUser.ApplicationUserGetRequestModel>().ReverseMap();
            CreateMap<Database.Entities.Note, Models.Note.NoteGetModel>();
            CreateMap<Models.ApplicationUser.ApplicationUserUpsertModel, Database.Entities.ApplicationUser>();
        }
    }
}
