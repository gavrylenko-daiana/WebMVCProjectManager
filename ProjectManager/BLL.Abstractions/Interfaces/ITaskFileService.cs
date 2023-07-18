using CloudinaryDotNet.Actions;
using Core.Models;
using Microsoft.AspNetCore.Http;

namespace BLL.Abstractions.Interfaces;

public interface ITaskFileService : IGenericService<TaskFile>
{
    Task<UploadResult> AddFileAsync(IFormFile file);

    Task<DeletionResult> DeleteFileAsync(string url);
}