using ErrorOr;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.Domain.Catalogs.Enums;
using RuanFa.Shop.SharedKernel.Models.Domains;

namespace RuanFa.Shop.Domain.Catalogs.ValueObjects;

public class ImageData : ValueObject
{
    public ImageType ImageType { get; private set; }
    public string Alt { get; private set; } = null!;
    public string Url { get; private set; } = null!;
    public string? MimeType { get; private set; }
    public int? Width { get; private set; }
    public int? Height { get; private set; }
    public long? FileSizeBytes { get; private set; }

    private ImageData() { } // For EF Core

    private ImageData(
        ImageType imageType,
        string alt,
        string url,
        string? mimeType,
        int? width = null,
        int? height = null,
        long? fileSizeBytes = null)
    {
        ImageType = imageType;
        Alt = alt;
        Url = url;
        MimeType = mimeType;
        Width = width;
        Height = height;
        FileSizeBytes = fileSizeBytes;
    }

    public static ErrorOr<ImageData> Create(
        ImageType imageType,
        string? alt,
        string? url,
        string? mimeType = null,
        int? width = null,
        int? height = null,
        long? fileSizeBytes = null)
    {
        if (!Enum.IsDefined(typeof(ImageType), imageType))
            return DomainErrors.ProductImage.InvalidImageType;

        if (string.IsNullOrWhiteSpace(alt))
            return DomainErrors.ProductImage.EmptyAltText;

        if (alt!.Length > 125)
            return DomainErrors.ProductImage.AltTextTooLong;

        if (string.IsNullOrWhiteSpace(url))
            return DomainErrors.ProductImage.EmptyImageUrl;

        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            return DomainErrors.ProductImage.InvalidImageUrl("The URL is not a valid absolute URI.");

        var uri = new Uri(url);
        if (uri.Scheme != "http" && uri.Scheme != "https")
            return DomainErrors.ProductImage.InvalidImageUrl("The URL must use HTTP or HTTPS protocol.");

        var validExtensions = new Dictionary<string, string>
        {
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".png", "image/png" },
            { ".gif", "image/gif" },
            { ".webp", "image/webp" }
        };

        var extension = validExtensions.Keys.FirstOrDefault(ext => uri.AbsolutePath.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
        if (extension == null)
            return DomainErrors.ProductImage.InvalidImageUrl("The URL must point to a supported image format (jpg, jpeg, png, gif, webp).");

        if (mimeType != null && mimeType != validExtensions[extension])
            return DomainErrors.ProductImage.InvalidMimeType;

        if (width.HasValue && width <= 0)
            return DomainErrors.ProductImage.InvalidImageDimensions;

        if (height.HasValue && height <= 0)
            return DomainErrors.ProductImage.InvalidImageDimensions;

        if (fileSizeBytes.HasValue && fileSizeBytes <= 0)
            return DomainErrors.ProductImage.InvalidFileSize;

        return new ImageData(imageType, alt, url, mimeType ?? validExtensions[extension], width, height, fileSizeBytes);
    }

    public ErrorOr<Updated> UpdateAlt(string? newAlt)
    {
        if (string.IsNullOrWhiteSpace(newAlt))
            return DomainErrors.ProductImage.EmptyAltText;

        if (newAlt!.Length > 125)
            return DomainErrors.ProductImage.AltTextTooLong;

        Alt = newAlt;
        return Result.Updated;
    }

    public ErrorOr<Updated> UpdateUrl(string? newUrl, string? mimeType = null)
    {
        if (string.IsNullOrWhiteSpace(newUrl))
            return DomainErrors.ProductImage.EmptyImageUrl;

        if (!Uri.IsWellFormedUriString(newUrl, UriKind.Absolute))
            return DomainErrors.ProductImage.InvalidImageUrl("The URL is not a valid absolute URI.");

        var uri = new Uri(newUrl);
        if (uri.Scheme != "http" && uri.Scheme != "https")
            return DomainErrors.ProductImage.InvalidImageUrl("The URL must use HTTP or HTTPS protocol.");

        var validExtensions = new Dictionary<string, string>
        {
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".png", "image/png" },
            { ".gif", "image/gif" },
            { ".webp", "image/webp" }
        };

        var extension = validExtensions.Keys.FirstOrDefault(ext => uri.AbsolutePath.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
        if (extension == null)
            return DomainErrors.ProductImage.InvalidImageUrl("The URL must point to a supported image format (jpg, jpeg, png, gif, webp).");

        if (mimeType != null && mimeType != validExtensions[extension])
            return DomainErrors.ProductImage.InvalidMimeType;

        Url = newUrl;
        MimeType = mimeType ?? validExtensions[extension];
        return Result.Updated;
    }

    public ErrorOr<Updated> UpdateMetadata(int? width, int? height, long? fileSizeBytes)
    {
        if (width.HasValue && width <= 0)
            return DomainErrors.ProductImage.InvalidImageDimensions;

        if (height.HasValue && height <= 0)
            return DomainErrors.ProductImage.InvalidImageDimensions;

        if (fileSizeBytes.HasValue && fileSizeBytes <= 0)
            return DomainErrors.ProductImage.InvalidFileSize;

        Width = width;
        Height = height;
        FileSizeBytes = fileSizeBytes;
        return Result.Updated;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return ImageType;
        yield return Alt;
        yield return Url;
        yield return MimeType;
        yield return Width;
        yield return Height;
        yield return FileSizeBytes;
    }
}
