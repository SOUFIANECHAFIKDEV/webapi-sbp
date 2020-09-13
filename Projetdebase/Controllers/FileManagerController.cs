using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InovaFileManager;
using Microsoft.AspNetCore.Mvc;
using ProjetBase.Businnes.Models;
using Serilog;

namespace Projetdebase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileManagerController : ControllerBase
    {
        private readonly FileManager fileManager;

        public FileManagerController()
        {
            this.fileManager = new FileManager();
        }

        [Route("Create")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] List<FileManagerModel> fileManagerModels)
        {
            try
            {
                var error = false;
                fileManagerModels.ForEach(file =>
                {
                    error = !fileManager.Save(file.base64, file.name);
                });
                return Ok(!error);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> Get([FromRoute] string name)
        {
            try
            {
                return Ok(new { data = fileManager.Get(name) });
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> Delete([FromRoute] string name)
        {
            try
            {
                return Ok(fileManager.Delete(name));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }

        [Route("DeleteFiles")]
        [HttpPost]
        public async Task<IActionResult> DeleteFiles([FromRoute] List<string> filesnames)
        {
            try
            {
                return Ok(fileManager.DeleteFiles(filesnames));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }
    }
}