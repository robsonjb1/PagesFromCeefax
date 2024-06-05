namespace API.Magazine;

public class SpectatorCartoon(Uri CartoonUri)
{
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

    public Uri CartoonUri { get; set; } = CartoonUri;
    public string RawHtml { get; set; } = String.Empty;
    public Uri? ImageUri { get; set; }
    public string ImageBase64 {get; set; } = String.Empty;
    public string Caption { get; set; } = String.Empty;
    public bool IsValid { get; set; } = true;

#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

}
