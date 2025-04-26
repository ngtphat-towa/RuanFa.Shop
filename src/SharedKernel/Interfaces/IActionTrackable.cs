namespace RuanFa.Shop.SharedKernel.Interfaces;

public interface IActionTrackable
{
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}
