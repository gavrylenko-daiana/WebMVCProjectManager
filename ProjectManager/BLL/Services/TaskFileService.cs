using BLL.Abstractions.Interfaces;
using Core.Models;
using DAL.Abstractions.Interfaces;

namespace BLL.Services;

public class TaskFileService : GenericService<TaskFile>, ITaskFileService
{
    public TaskFileService(IRepository<TaskFile> repository) : base(repository)
    {
    }

    public async Task<TaskFile> AddNewFileAsync(string inputPathFile)
    {
        try
        {
            var fileTask = new TaskFile()
            {
                UploadedFiles = inputPathFile
            };
            await Add(fileTask);

            return fileTask;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task UpdateTaskFile(TaskFile taskFile)
    {
        try
        {
            await Update(taskFile.Id, taskFile);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}