using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Sabio.Models.Files;
using Sabio.Models.Requests.Files;
using Sabio.Services;
using Sabio.Services.Interfaces;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FileApiController : BaseApiController
    {
        private IFileService _fileService = null;
      

        private IAuthenticationService<int> _authService = null;
        public FileApiController(IFileService service
            , ILogger<FileApiController> logger
      
            , IAuthenticationService<int> authService) : base(logger)
        {
            _fileService = service;
            _authService = authService;
           
        }

        [HttpPost]
        public async Task<ActionResult<ItemsResponse<FileBase>>> UploadFile(List<IFormFile> files)
        {

            ObjectResult result = null;
          

            try
            {
                int userId = _authService.GetCurrentUserId();
                List<FileBase> res = await _fileService.UploadFileAsync(files, userId);
                ItemsResponse<FileBase> response = new ItemsResponse<FileBase>() { Items = res };
                result = StatusCode(200, response);
            }
            catch (Exception ex)    
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                result = StatusCode(500, response);
            }

            return result;
        }

        [HttpGet("paginate")]
        public ActionResult<ItemsResponse<Paged<File>>> Get(int pageIndex, int pageSize)
        {
            ActionResult result = null;

            try
            {
                Paged<File> paged = _fileService.Get(pageIndex, pageSize);

                if (paged == null)
                {
                    result = NotFound404(new ErrorResponse("Records not found"));
                }
                else
                {
                    ItemResponse<Paged<File>> response = new ItemResponse<Paged<File>>();
                    response.Item = paged;
                    result = Ok200(response);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                result = StatusCode(500, new ErrorResponse(ex.Message.ToString()));
            }

            return result;
        }

        [HttpGet("paginatetype")]
        public ActionResult<ItemsResponse<Paged<File>>> GetByType(int pageIndex, int pageSize, int fileTypeId)
        {
            ActionResult result = null;

            try
            {
                Paged<File> paged = _fileService.GetByType(pageIndex, pageSize, fileTypeId);

                if (paged == null)
                {
                    result = NotFound404(new ErrorResponse("Records not found"));
                }
                else
                {
                    ItemResponse<Paged<File>> response = new ItemResponse<Paged<File>>();
                    response.Item = paged;
                    result = Ok200(response);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                result = StatusCode(500, new ErrorResponse(ex.Message.ToString()));
            }

            return result;
        }

        [HttpPut("{id:int}")]
        public ActionResult<ItemResponse<int>> Update(FileUpdateRequest model)
        {
            _fileService.Update(model);

            SuccessResponse response = new SuccessResponse();

            return Ok(response);
        }
    }
}
