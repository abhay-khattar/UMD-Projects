using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OTSv9.Data;
using OTSv9.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OTSv9.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class FileUploadController : Controller
    {

        private readonly ApplicationDbContext context;
        //private readonly UserManager<ApplicationUser> _userManager;
        public FileUploadController(ApplicationDbContext context)
        {
            this.context = context;
        }


        public async Task<IActionResult> Index()
        {
            var fileuploadViewModel = await LoadAllFiles();
            ViewBag.Message = TempData["Message"];
            return View(fileuploadViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UploadToFileSystem(List<IFormFile> files, string description, string pname)
        {
            try
            {
                foreach (var file in files)
                {
                    //var userName = await _userManager.GetUserNameAsync(user);
                    var doctorname = HttpContext.User.Identity.Name;
                    var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\Files\\" + doctorname);
                    bool basePathExists = System.IO.Directory.Exists(basePath);
                    if (!basePathExists) Directory.CreateDirectory(basePath);
                    var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    var filePath = Path.Combine(basePath, file.FileName);
                    var extension = Path.GetExtension(file.FileName);
                    
                    if (extension == ".pdf")
                    {
                        if (!System.IO.File.Exists(filePath))
                        {
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                            var fileModel = new FileOnFileSystemModel
                            {

                                CreatedOn = DateTime.UtcNow,
                                DoctorUserName = doctorname,
                                PatientUserName = pname,
                                Extension = extension,
                                FileName = fileName,
                                FileType = file.ContentType,
                                Description = description,
                                FilePath = filePath
                            };
                            context.FileOnFileSystemModel.Add(fileModel);
                            context.SaveChanges();
                        }
                        else
                        {
                            TempData["Message"] = "File already exists.";
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        TempData["Message"] = "Extension Invalid. ";
                        return RedirectToAction("Index");
                    }


                }

                TempData["Message"] = "File successfully uploaded to File System.";
                return RedirectToAction("Index");
            }
            catch
            {
                //UseExceptionHandler("/error");
                TempData["Message"] = "There was an error.";
                return RedirectToAction("Index");
            }
        }


        private async Task<FileOnFileSystemModel> LoadAllFiles()
        {
            var viewModel = new FileOnFileSystemModel();
            viewModel.FilesOnFileSystem = await context.FileOnFileSystemModel.ToListAsync();
            //context.FileOnFileSystemModel.FindAsync()
            return viewModel;
        }

        public async Task<IActionResult> DownloadFileFromFileSystem(int id)
        {
            //var doctorname = HttpContext.User.Identity.Name;
            var file = await context.FileOnFileSystemModel.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (file == null) return null;
            var memory = new MemoryStream();
            using (var stream = new FileStream(file.FilePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, file.FileType, file.FileName + file.Extension);
        }

        //public async Task<IActionResult> DownloadFileFromFileSystem(int id)
        //{
        //    var file = await context.FileOnFileSystemModel.Where(x => x.Id == id).FirstOrDefaultAsync();
        //    if (file == null) return null;
        //    var memory = new MemoryStream();
        //    using (var stream = new FileStream(file.FilePath, FileMode.Open))
        //    {
        //        await stream.CopyToAsync(memory);
        //    }
        //    memory.Position = 0;
        //    return File(memory, file.FileName);
        //}
        public async Task<IActionResult> DeleteFileFromFileSystem(int id)
        {
            var file = await context.FileOnFileSystemModel.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (file == null) return null;
            if (System.IO.File.Exists(file.FilePath))
            {
                System.IO.File.Delete(file.FilePath);
            }
            context.FileOnFileSystemModel.Remove(file);
            context.SaveChanges();
            TempData["Message"] = $"Removed {file.FileName + file.Extension} successfully from File System.";
            return RedirectToAction("Index");
        }
    }
}
