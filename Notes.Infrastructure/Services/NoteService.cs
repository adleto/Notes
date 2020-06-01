using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Notes.Database;
using Notes.Database.Entities;
using Notes.Infrastructure.Interfaces;
using Notes.Models.Note;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notes.Infrastructure.Services
{
    public class NoteService : INote
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        public NoteService(Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<NoteGetModel> Add(NoteInsertModel model, int userId)
        {
            try {
                var note = new Note
                {
                    ApplicationUserId = userId,
                    Created = DateTime.Now,
                    Text = model.Text,
                    Title = model.Title,
                    Modified = model.Modified,
                    Version = model.Version
                };
                _context.Add(note);
                await _context.SaveChangesAsync();
                return _mapper.Map<NoteGetModel>(note);
            }
            catch
            {
                throw new Exception("Incorrect user selected");
            }
        }

        public async Task Delete(int noteId, int userId)
        {
            var note = _context.Note.Find(noteId);
            if (note != null)
            {
                if(note.ApplicationUserId != userId)
                {
                    throw new Exception("Note does not belong to the user.");
                }
                _context.Remove(note);
                await _context.SaveChangesAsync();
            }
            else { 
                throw new Exception("Id does not exist.");
            }
        }

        public async Task<List<NoteGetModel>> Get(int userId)
        {
            try {
                return _mapper.Map<List<NoteGetModel>>(await _context.Note
                    .Where(n => n.ApplicationUserId == userId)
                    .ToListAsync());
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<NoteGetModel>> GetForPull(List<NoteSyncRequestModel> model, int userId)
        {
            try {
                var notes = await _context.Note
                    .Where(n => n.ApplicationUserId == userId)
                    .ToListAsync();
                var notesToReturn = new List<Note>();
                foreach(var n in notes)
                {
                    bool contained = false;
                    foreach(var request in model)
                    {
                        if(request.Id == n.Id)
                        {
                            if(n.Version != request.Version)
                            {
                                notesToReturn.Add(n);
                            }
                            contained = true;
                            break;
                        }
                    }
                    if (!contained)
                    {
                        notesToReturn.Add(n);
                    }
                }
                return _mapper.Map<List<NoteGetModel>>(notesToReturn);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<NoteSyncRequestModel>> GetForPush(List<NoteSyncRequestModel> model, int userId)
        {
            try {
                var notes = await _context.Note
                    .Where(n => n.ApplicationUserId == userId)
                    .ToListAsync();
                var notesToRequest = new List<NoteSyncRequestModel>();

                bool atLeastOneChange = false;
                foreach (var n in notes)
                {
                    bool contained = false;
                    foreach (var request in model)
                    {
                        if (request.Id == n.Id)
                        {
                            if(n.Version != request.Version)
                            {
                                notesToRequest.Add(request);
                            }
                            contained = true;
                            break;
                        }
                    }
                    if (!contained)
                    {
                        _context.Remove(n);
                        atLeastOneChange = true;
                    }
                }
                if (atLeastOneChange) {
                    await _context.SaveChangesAsync();
                }
                return notesToRequest;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<NotePushedDataModel>> Push(List<NotePushModel> model, int userId)
        {
            try {
                var IPushedThis = new List<NotePushedDataModel>();
                foreach (var m in model)
                {
                    if(m.Id == null)
                    {
                        NoteGetModel item = null;
                        item = await this.Add(new NoteInsertModel
                        {
                            Modified = m.Modified,
                            Text = m.Text,
                            Title = m.Title,
                            Version = m.Version
                        }, userId);
                        IPushedThis.Add(new NotePushedDataModel
                        {
                            DatabaseExternalId = item.Id,
                            DatabaseLocalId = m.LocalId
                        });
                    }
                    else
                    {
                        await this.Update(new NoteUpdateModel
                        {
                            Id = (int)m.Id,
                            Modified = m.Modified,
                            Text = m.Text,
                            Title = m.Title,
                            Version = (int)m.Version
                        }, userId);
                    }
                }
                return IPushedThis;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
    }

        public async Task<NoteGetModel> Update(NoteUpdateModel model, int userId)
        {
            var note = _context.Note.Find(model.Id);
            if (note != null) 
            {
                if(note.ApplicationUserId != userId)
                {
                    throw new Exception("Note does not belong to the user.");
                }
                note.Text = model.Text;
                note.Title = model.Title;
                note.Modified = model.Modified;
                note.Version++;
                await _context.SaveChangesAsync();

                return _mapper.Map<NoteGetModel>(note);
            }
            throw new Exception("Note does not exist.");
        }
    }
}
