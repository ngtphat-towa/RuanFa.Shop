using FluentStorage;
using FluentStorage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using ErrorOr;
using RuanFa.Shop.Application.Common.Services;
using RuanFa.Shop.Infrastructure.Settings;
using Serilog;

namespace RuanFa.Shop.Infrastructure.Services;

public class FluentStorageService : IStorageService
{
    private readonly IBlobStorage _storage;
    private readonly StorageSettings _settings;

    public FluentStorageService(
        IOptions<StorageSettings> settings)
    {
        _settings = settings.Value;

        try
        {
            Directory.CreateDirectory(_settings.LocalPath);
            _storage = StorageFactory.Blobs.DirectoryFiles(_settings.LocalPath);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to initialize storage at path: {Path}", _settings.LocalPath);
            throw;
        }
    }

    public async Task<ErrorOr<string>> UploadFileAsync(IFormFile file, string? path = null)
    {
        if (file is null)
        {
            return Error.Validation(
                code: "File.Empty",
                description: "No file was provided");
        }

        if (file.Length == 0)
        {
            return Error.Validation(
                code: "File.Empty",
                description: "The file is empty");
        }

        if (file.Length > _settings.MaxFileSizeBytes)
        {
            return Error.Validation(
                code: "File.TooLarge",
                description: $"File size exceeds the maximum allowed size of {_settings.MaxFileSizeBytes / 1024 / 1024}MB");
        }

        string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_settings.AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            return Error.Validation(
                code: "File.InvalidType",
                description: $"File type {extension} is not allowed. Allowed types: {string.Join(", ", _settings.AllowedExtensions)}");
        }

        try
        {
            // Create path structure: yyyy/MM/dd/filename
            string dateFolder = DateTime.UtcNow.ToString("yyyy/MM/dd");
            string safeFileName = $"{DateTime.UtcNow.Ticks}_{Guid.NewGuid():N}{extension}";
            string blobPath = path != null
                ? Path.Combine(path, dateFolder, safeFileName).Replace("\\", "/")
                : Path.Combine(dateFolder, safeFileName).Replace("\\", "/");

            await using var stream = file.OpenReadStream();
            await _storage.WriteAsync(blobPath, stream);

            return Path.Combine(_settings.BaseUrl, blobPath).Replace("\\", "/");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to upload file {FileName}", file.FileName);
            return Error.Failure(
                code: "File.UploadFailed",
                description: "An error occurred while uploading the file");
        }
    }

    public async Task<ErrorOr<Success>> DeleteFileAsync(string fileUrl)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
        {
            return Error.Validation(
                code: "File.InvalidUrl",
                description: "File URL cannot be empty");
        }

        try
        {
            string blobPath = GetBlobPath(fileUrl);

            if (!await _storage.ExistsAsync(blobPath))
            {
                return Error.NotFound(
                    code: "File.NotFound",
                    description: "The specified file was not found");
            }

            await _storage.DeleteAsync(blobPath);
            return Result.Success;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to delete file at {FileUrl}", fileUrl);
            return Error.Failure(
                code: "File.DeleteFailed",
                description: "An error occurred while deleting the file");
        }
    }

    public async Task<ErrorOr<Stream>> GetFileAsync(string fileUrl)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
        {
            return Error.Validation(
                code: "File.InvalidUrl",
                description: "File URL cannot be empty");
        }

        try
        {
            string blobPath = GetBlobPath(fileUrl);

            if (!await _storage.ExistsAsync(blobPath))
            {
                return Error.NotFound(
                    code: "File.NotFound",
                    description: "The specified file was not found");
            }

            var memoryStream = new MemoryStream();
            await _storage.ReadToStreamAsync(blobPath, memoryStream);
            memoryStream.Position = 0;

            return memoryStream;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to read file at {FileUrl}", fileUrl);
            return Error.Failure(
                code: "File.ReadFailed",
                description: "An error occurred while reading the file");
        }
    }

    private string GetBlobPath(string fileUrl)
    {
        // Remove base URL and leading slash to get the blob path
        return fileUrl
            .Replace(_settings.BaseUrl, "", StringComparison.OrdinalIgnoreCase)
            .TrimStart('/');
    }
}
