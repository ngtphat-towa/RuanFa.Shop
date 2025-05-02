using Microsoft.AspNetCore.Http;
using ErrorOr;

namespace RuanFa.Shop.Application.Common.Services;

public interface IStorageService
{
    /// <summary>
    /// Upload a file to storage
    /// </summary>
    /// <param name="file">The file to upload</param>
    /// <param name="path">Optional path/prefix for the file</param>
    /// <returns>The file URL or path</returns>
    Task<ErrorOr<string>> UploadFileAsync(IFormFile file, string? path = null);

    /// <summary>
    /// Delete a file from storage
    /// </summary>
    /// <param name="fileUrl">The URL or path of the file to delete</param>
    Task<ErrorOr<Success>> DeleteFileAsync(string fileUrl);

    /// <summary>
    /// Get a file from storage
    /// </summary>
    /// <param name="fileUrl">The URL or path of the file to get</param>
    /// <returns>The file stream</returns>
    Task<ErrorOr<Stream>> GetFileAsync(string fileUrl);
}
