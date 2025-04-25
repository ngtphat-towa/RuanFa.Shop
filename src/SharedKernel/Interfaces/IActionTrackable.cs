namespace RuanFa.Shop.SharedKernel.Interfaces;

public interface IActionTrackable
{
    string? CreatedBy { get; set; }
    string? UpdatedBy { get; set; }
}
