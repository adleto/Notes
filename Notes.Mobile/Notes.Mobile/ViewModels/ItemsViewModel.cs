using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;
using Notes.Mobile.Views;
using Notes.Mobile.Models.Requests;
using Notes.Mobile.Services.External;
using Xamarin.Essentials;
using System.Collections.Generic;

namespace Notes.Mobile.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        public ObservableCollection<NoteGetModel> Notes { get; set; }
        public Command LoadNotesCommand { get; set; }
        private ApiService _noteService = null;

        public ItemsViewModel()
        {
            if (Preferences.ContainsKey("serverUrl"))
            {
                _noteService = new ApiService(Preferences.Get("serverUrl", ""), "Note");
            }
            Title = "Browse Notes";
            Notes = new ObservableCollection<NoteGetModel>();
            LoadNotesCommand = new Command(async () => await ExecuteLoadNotesCommand());

            MessagingCenter.Subscribe<CloudSyncPage>(this, "LogedIn", (obj) => _noteService = new ApiService(Preferences.Get("serverUrl", ""), "Note"));

            MessagingCenter.Subscribe<NewNotePage, NoteInsertModel>(this, "AddNote", async (obj, note) =>
            {
                try
                {
                    var newNote = note as NoteInsertModel;
                    if (string.IsNullOrEmpty(newNote.Title))
                    {
                        newNote.Title = "#";
                    }
                    await NoteService.Add(newNote);
                }
                catch(Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
                }
            });

            MessagingCenter.Subscribe<NoteDetailPage, NoteUpdateModel>(this, "UpdateNote", async (obj, note) =>
            {
                try
                {
                    var updatedNote = note as NoteUpdateModel;
                    await NoteService.Update(updatedNote);
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
                }
            });
        }

        public async Task ExecuteLoadNotesCommand()
        {
            IsBusy = true;

            try
            {
                Notes.Clear();
                var notes = await NoteService.Get();
                foreach (var note in notes)
                {
                    Notes.Add(note);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task DeleteNote(NoteGetModel note)
        {
            try {
                await NoteService.Delete(note.Id);
                Notes.Remove(note);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                await Application.Current.MainPage.DisplayAlert("Error", "Note already deleted.", "OK");
            }
        }
        private List<Notes.Models.Note.NoteSyncRequestModel> PrepareSyncRequest()
        {
            var model = new List<Notes.Models.Note.NoteSyncRequestModel>();
            foreach (var n in Notes)
            {
                if (n.ExternalId != null)
                {
                    model.Add(new Notes.Models.Note.NoteSyncRequestModel
                    {
                        Id = (int)n.ExternalId,
                        Version = n.Version
                    });
                }
            }
            return model;
        }
        public async Task Push()
        {
            try
            {
                var model = PrepareSyncRequest();
                var requestWithNoteDataToSend = await _noteService.Post<List<Notes.Models.Note.NoteSyncRequestModel>>(model, "PushSyncRequest");

                var localNotes = await NoteService.Get(true);
                var listToPush = new List<Notes.Models.Note.NotePushModel>();
                foreach (var ln in localNotes)
                {
                    if (ln.ExternalId == null)
                    {
                        listToPush.Add(new Notes.Models.Note.NotePushModel
                        {
                            Modified = ln.Modified,
                            Text = ln.Text,
                            Title = ln.Title,
                            Version = ln.Version,
                            LocalId = ln.Id
                        });
                        continue;
                    }
                    foreach (var r in requestWithNoteDataToSend)
                    {
                        if(r.Id == ln.ExternalId)
                        {
                            listToPush.Add(new Notes.Models.Note.NotePushModel
                            {
                                Id = ln.ExternalId,
                                Modified = ln.Modified,
                                Text = ln.Text,
                                Title = ln.Title,
                                Version = ln.Version,
                                LocalId = ln.Id
                            });
                            break;
                        }
                    }
                }

                // These are only newly inserted notes, not those that only required update. NEW in external database ONLY
                var pushedNewItems = await _noteService.Post<List<Notes.Models.Note.NotePushedDataModel>>(listToPush, "PushSync");
                foreach(var pni in pushedNewItems)
                {
                    foreach(var ln in localNotes)
                    {
                        if(pni.DatabaseLocalId == ln.Id)
                        {
                            await NoteService.SetExternalId(ln.Id, pni.DatabaseExternalId);
                            break;
                        }
                    }
                }
            }
            catch { }
        }
        public async Task Pull()
        {
            try
            {
                var model = PrepareSyncRequest();
                var notesFromServer = await _noteService.Post<List<Notes.Models.Note.NoteGetModel>>(model, "PullSync");

                foreach(var n in notesFromServer)
                {
                    bool contained = false;
                    foreach(var localNote in Notes)
                    {
                        if(localNote.ExternalId == n.Id)
                        {
                            contained = true;
                            await NoteService.Update(new NoteUpdateModel { 
                                Id = localNote.Id,
                                Modified = n.Modified,
                                Text = n.Text,
                                Title = n.Title
                            }, true);
                            break;
                        }
                    }
                    if (!contained)
                    {
                        await NoteService.Add(new NoteInsertModel
                        {
                            Text = n.Text,
                            Title = n.Title,
                            ExternalId = n.Id,
                            Modified = n.Modified
                        }, true);
                    }
                }
            }
            catch { }
        }
        //public async Task Sync()
        //{
        //    await Push();
        //    await Pull();
        //}
    }
}