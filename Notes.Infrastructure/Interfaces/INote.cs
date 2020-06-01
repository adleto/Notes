using Notes.Models.Note;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notes.Infrastructure.Interfaces
{
    public interface INote
    {
        Task<NoteGetModel> Add(NoteInsertModel model, int userId);
        Task<NoteGetModel> Update(NoteUpdateModel model, int userId);
        Task<List<NoteGetModel>> Get(int userId);
        Task Delete(int noteId, int userId);
        Task<List<NoteGetModel>> GetForPull(List<NoteSyncRequestModel> model, int userId);
        Task<List<NoteSyncRequestModel>> GetForPush(List<NoteSyncRequestModel> model, int userId);
        Task<List<NotePushedDataModel>> Push(List<NotePushModel> model, int userId);
    }
}
