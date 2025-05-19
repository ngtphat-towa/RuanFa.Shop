using ErrorOr;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Catalogs.Products.Delete;

public sealed record DeleteProductCommand(Guid Id) : ICommand<Deleted>;

internal sealed class DeleteProductCommandHandler : ICommandHandler<DeleteProductCommand, Deleted>
{
    private readonly IApplicationDbContext _context;

    public DeleteProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<Deleted>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.FindAsync(request.Id, cancellationToken);
        if (product == null)
        {
            return DomainErrors.Product.NotFound;
        }
        // TODO: Check product in used or using the chanage correct status instead of complete deleting
        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Deleted;
    }
}
