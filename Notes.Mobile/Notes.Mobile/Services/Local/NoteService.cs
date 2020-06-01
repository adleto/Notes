using Notes.Mobile.Helpers;
using Notes.Mobile.Models.Database;
using Notes.Mobile.Models.Requests;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Notes.Mobile.Services.Local
{
    public class NoteService : INote
    {
        private readonly SQLiteAsyncConnection _connection;
        private string _dbPath = FileAccessHelper.GetLocalFilePath("noteDbLite.db3");
        private string _key = null;

        public NoteService()
        {
            _connection = new SQLiteAsyncConnection(_dbPath);
            _connection.CreateTableAsync<Note>().Wait();
        }
        ~NoteService(){
            _connection.CloseAsync().Wait();
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
        }
        public async Task SetKey()
        {
            _key = await SecureStorage.GetAsync("secrets_key");
            if (_key == null)
            {
                await SecureStorage.SetAsync("secrets_key", SecureService.GenerateKey());
                _key = await SecureStorage.GetAsync("secrets_key");
            }
        }
        public async Task Add(NoteInsertModel model, bool addRaw = false)
        {
            try
            {
                if (_key == null) await SetKey();
                if (addRaw)
                {
                    await _connection.InsertAsync(new Note
                    {
                        Modified = DateTime.Now,
                        Text = model.Text,
                        Title = model.Title,
                        Version = 1,
                        ExternalId = model.ExternalId
                    });
                }
                else {
                    await _connection.InsertAsync(new Note { 
                        Modified = DateTime.Now,
                        Text = SecureService.EncryptString(_key, model.Text),
                        Title = SecureService.EncryptString(_key, model.Title),
                        Version = 1,
                        ExternalId = model.ExternalId
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                var entry = await _connection.GetAsync<Note>(id);
                await _connection.DeleteAsync<Note>(entry.Id);
            }
            catch(Exception ex)
            {
                throw new Exception("Database error: " +ex.Message);
            }
        }

        public async Task<NoteGetModel> Get(int id)
        {
            try
            {
                if (_key == null) await SetKey();
                var note = await _connection.GetAsync<Note>(id);
                return new NoteGetModel
                {
                    ExternalId = note.ExternalId,
                    Id = note.Id,
                    Modified = note.Modified,
                    Text = SecureService.DecryptString(_key, note.Text),
                    Title = SecureService.DecryptString(_key, note.Title)
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " +ex.Message);
            }
        }

        public async Task<List<NoteGetModel>> Get(bool getRaw = false)
        {
            try
            {
                if (_key == null) await SetKey();
                var notes = await _connection.Table<Note>().ToListAsync();
                var notesGetModel = new List<NoteGetModel>();
                foreach(var note in notes)
                {
                    if (getRaw)
                    {
                        notesGetModel.Add(new NoteGetModel
                        {
                            ExternalId = note.ExternalId,
                            Id = note.Id,
                            Modified = note.Modified,
                            Text = note.Text,
                            Title = note.Title,
                            Version = note.Version
                        });
                    }
                    else {
                        notesGetModel.Add(new NoteGetModel
                        {
                            ExternalId = note.ExternalId,
                            Id = note.Id,
                            Modified = note.Modified,
                            Text = SecureService.DecryptString(_key, note.Text),
                            Title = SecureService.DecryptString(_key, note.Title),
                            Version = note.Version
                        });
                    }
                }
                return notesGetModel;
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }
        public async Task SetExternalId(int id, int externalId)
        {
            try
            {
                if (_key == null) await SetKey();
                var note = await _connection.GetAsync<Note>(id);
                note.ExternalId = externalId;
                await _connection.UpdateAsync(note);
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }

        public async Task Update(NoteUpdateModel model, bool updateRaw = false)
        {
            try
            {
                if (_key == null) await SetKey();
                var note = await _connection.GetAsync<Note>(model.Id);
                if (updateRaw)
                {
                    note.Text = model.Text;
                    note.Title = model.Title;
                }
                else {
                    note.Text = SecureService.EncryptString(_key, model.Text);
                    note.Title = SecureService.EncryptString(_key, model.Title);
                }
                note.Modified = DateTime.Now;
                note.Version++;
                await _connection.UpdateAsync(note);
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }

        public async Task DeleteAll()
        {
            await _connection.DeleteAllAsync<Note>();
        }
    }
}
