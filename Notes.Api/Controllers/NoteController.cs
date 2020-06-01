using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Notes.Infrastructure.Interfaces;
using Notes.Models.Note;

namespace Notes.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NoteController : ControllerBase
    {
        private readonly INote _noteService;

        public NoteController(INote noteService)
        {
            _noteService = noteService;
        }
        [HttpPost]
        public async Task<IActionResult> PushSyncRequest([FromBody] List<NoteSyncRequestModel> model)
        {
            try
            {
                return Ok(await _noteService.GetForPush(model, GetUserId()));
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<IActionResult> PushSync([FromBody] List<NotePushModel> model)
        {
            try
            {
                return Ok(await _noteService.Push(model, GetUserId()));
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<IActionResult> PullSync([FromBody] List<NoteSyncRequestModel> model)
        {
            try
            {
                return Ok(await _noteService.GetForPull(model, GetUserId()));
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetNotes()
        {
            try
            {
                return Ok(await _noteService.Get(GetUserId()));
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateNote([FromBody] NoteUpdateModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception("Model not valid.");
                }
                return Ok(await _noteService.Update(model, GetUserId()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPatch]
        public async Task<IActionResult> UpdateNote([FromBody] List<NoteUpdateModel> listModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception("Model not valid.");
                }
                var list = new List<NoteGetModel>();
                foreach (var item in listModel)
                {
                    var updatedItem = await _noteService.Update(item, GetUserId());
                    list.Add(updatedItem);
                }
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddNote([FromBody] NoteInsertModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception("Model not valid.");
                }
                return Ok(await _noteService.Add(model, GetUserId()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddNotes([FromBody] List<NoteInsertModel> listModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception("Model not valid.");
                }
                var list = new List<NoteGetModel>();
                foreach (var item in listModel)
                {
                    var insertedItem = await _noteService.Add(item, GetUserId());
                    list.Add(insertedItem);
                }
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{noteId}")]
        public async Task<IActionResult> RemoveNote(int noteId)
        {
            try
            {
                await _noteService.Delete(noteId, GetUserId());
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        private int GetUserId()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            return int.Parse(identity.FindFirst("UserId").Value);
        }
    }
}
