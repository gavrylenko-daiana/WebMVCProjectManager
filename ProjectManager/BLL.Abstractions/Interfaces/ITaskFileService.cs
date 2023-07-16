using Core.Models;

namespace BLL.Abstractions.Interfaces;

public interface ITaskFileService : IGenericService<TaskFile>
{
    Task<TaskFile> AddNewFileAsync(string inputPathFile);

    Task UpdateTaskFile(TaskFile taskFile);
}