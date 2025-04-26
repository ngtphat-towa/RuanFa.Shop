namespace RuanFa.Shop.SharedKernel.Services;
public interface IUserContext
{
    bool IsAuthenticated { get; }
    Guid? UserId { get; }
    string? Username { get; }
}
