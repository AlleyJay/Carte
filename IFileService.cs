using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sabio.Models;
using Sabio.Models.Files;
using Sabio.Models.Requests.Files;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Sabio.Services.Interfaces
{
    public interface IFileService
    {
        int Add(FileAddRequest model, int userId);
        File Get(int id);
        Paged<File> Get(int pageIndex, int pageSize);
        Paged<File> Get(int pageIndex, int pageSize, int userId);
        Paged<File> GetByType(int pageIndex, int pageSize, int typeId);
        void Update(FileUpdateRequest model);
        Task<List<FileBase>> UploadFileAsync(List<IFormFile> files, int userId);
    }
}
