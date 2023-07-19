using BLL.Abstractions.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Core.Helpers;
using Core.Models;
using DAL.Abstractions.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace BLL.Services;

public class UploadedFileService : GenericService<TaskFile>, IUploadedFileService
{
    private readonly Cloudinary _cloudinary;

    public UploadedFileService(IRepository<TaskFile> repository, IOptions<CloudinarySettings> config) : base(repository)
    {
        var acc = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );
        _cloudinary = new Cloudinary(acc);
    }

    public async Task<UploadResult> AddFileAsync(IFormFile file)
    {
        var uploadResult = new ImageUploadResult();

        if (file.Length > 0)
        {
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream)
            };
            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }

        return uploadResult;
    }

    public async Task<DeletionResult> DeleteFileAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);

        return await _cloudinary.DestroyAsync(deleteParams);
    }

    public async Task AddUploadFileAsync(TaskFile uploadedFile)
    {
        await Add(uploadedFile);
    }
    
    public async Task DeleteUploadFileAsync(Guid id)
    {
        await Delete(id);
    }
    
    public async Task<TaskFile> GetByIdUploadFileAsync(Guid id)
    {
        var file = await GetById(id);

        return file;
    }
}
