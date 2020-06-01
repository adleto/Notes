using Notes.Mobile.Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notes.Mobile.Services.Local
{
    public interface INote
    {
        Task Add(NoteInsertModel model, bool addRaw = false);
        Task Delete(int id);
        Task Update(NoteUpdateModel model, bool updateRaw = false);
        Task<NoteGetModel> Get(int id);
        Task<List<NoteGetModel>> Get(bool getRaw = false);
        Task DeleteAll();
        Task SetKey();
        Task SetExternalId(int id, int externalId);
    }
}
