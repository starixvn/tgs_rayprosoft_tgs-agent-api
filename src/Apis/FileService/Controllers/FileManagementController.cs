using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TGS.Apis.FileService.Controllers
{
    [Route("api/fs")]
    [ApiController]
    public class FileManagementController : ControllerBase
    {
        [HttpPost("{*path}")]
        public void UploadFile(string path, IFormFile file)
        {

        }
    }
}
