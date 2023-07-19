using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Core.Helpers;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace BLL.Abstractions.Interfaces;

public interface IUploadedFileService
{
    Task<UploadResult> AddFileAsync(IFormFile file);

    Task<DeletionResult> DeleteFileAsync(string publicId);

    Task AddUploadFileAsync(TaskFile uploadedFile);

    Task DeleteUploadFileAsync(Guid id);

    Task<TaskFile> GetByIdUploadFileAsync(Guid id);
}