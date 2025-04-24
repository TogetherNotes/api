using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using WebApplicationTgtNotes.Models;

namespace WebApplicationTgtNotes.Controllers
{
    public class filesController : ApiController
    {
        private TgtNotesEntities db = new TgtNotesEntities();


        // GET: api/files/download/{app_id}/{type}
        [HttpGet]
        [Route("api/files/download/{app_id:int}/{type}")]
        public IHttpActionResult DownloadFile(int app_id, string type)
        {
            if (type != "image" && type != "audio")
            {
                return BadRequest("Tipo inválido. Usa 'image' o 'audio'.");
            }

            var rootPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
            var folder = type == "audio" ? "Audios" : "Images";
            var userFolder = Path.Combine(rootPath, folder, app_id.ToString());

            if (!Directory.Exists(userFolder))
                return NotFound();

            var filePath = Directory.GetFiles(userFolder).FirstOrDefault();
            if (filePath == null)
                return NotFound();

            var fileBytes = File.ReadAllBytes(filePath);
            var fileName = Path.GetFileName(filePath);
            var mimeType = MimeMapping.GetMimeMapping(fileName);

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(fileBytes)
            };

            result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mimeType);
            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("inline")
            {
                FileName = fileName
            };

            return ResponseMessage(result);
        }


        // PUT: api/files/{id}/{app_id}
        [HttpPut]
        [Route("api/files/{id:int}/{app_id:int}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putfiles(int id, int app_id, files files)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != files.id || app_id != files.app_id)
                return BadRequest("ID mismatch");

            var existing = await db.files.FindAsync(id, app_id);
            if (existing == null)
                return NotFound();

            // Actualitzar camps
            existing.name = files.name;
            existing.type = files.type;
            existing.date = files.date;

            try
            {
                await db.SaveChangesAsync();
                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST: api/files/upload/{app_id}
        [HttpPost]
        [Route("api/files/upload/{app_id:int}")]
        public async Task<IHttpActionResult> UploadFile(int app_id)
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                return BadRequest("El tipo de contenido no es multipart/form-data");
            }
            var rootPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
            var provider = new MultipartFormDataStreamProvider(rootPath);

            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);

                foreach (var fileData in provider.FileData)
                {
                    var originalFileName = fileData.Headers.ContentDisposition.FileName.Trim('\"');
                    var extension = Path.GetExtension(originalFileName).ToLower();

                    string folderName;
                    string fileType;

                    if (extension == ".mp3" || extension == ".wav" || extension == ".ogg")
                    {
                        folderName = "Audios";
                        fileType = "audio";
                    }
                    else if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif")
                    {
                        folderName = "Images";
                        fileType = "image";
                    }
                    else
                    {
                        return BadRequest("Formato de archivo no soportado.");
                    }

                    var userFolder = Path.Combine(rootPath, folderName, app_id.ToString());

                    if (!Directory.Exists(userFolder))
                        Directory.CreateDirectory(userFolder);

                    // Eliminar archivo anterior si existe
                    var existingFiles = Directory.GetFiles(userFolder);
                    foreach (var file in existingFiles)
                    {
                        System.IO.File.Delete(file);
                    }

                    // Guardar nuevo archivo con el nombre original 
                    var savePath = Path.Combine(userFolder, originalFileName);
                    File.Move(fileData.LocalFileName, savePath);

                    using (var db = new TgtNotesEntities())
                    {
                        // Eliminar registro anterior si existe
                        var oldFiles = db.files.Where(f => f.app_id == app_id && f.type == fileType).ToList();
                        db.files.RemoveRange(oldFiles);

                        // Crear registro nuevo
                        files newFile = new files
                        {
                            name = originalFileName,
                            type = fileType,
                            date = DateTime.Now,
                            app_id = app_id
                        };

                        db.files.Add(newFile);
                        await db.SaveChangesAsync();
                    }
                }

                return Ok("Archivo subido y reemplazado correctamente.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        // DELETE: api/files/{id}/{app_id}
        [HttpDelete]
        [Route("api/files/{id:int}/{app_id:int}")]
        [ResponseType(typeof(files))]
        public async Task<IHttpActionResult> Deletefiles(int id, int app_id)
        {
            var file = await db.files.FindAsync(id, app_id);
            if (file == null)
                return NotFound();

            db.files.Remove(file);
            await db.SaveChangesAsync();

            return Ok(file);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}