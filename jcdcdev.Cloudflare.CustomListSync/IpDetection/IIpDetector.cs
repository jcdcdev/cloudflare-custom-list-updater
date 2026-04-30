namespace jcdcdev.Cloudflare.CustomListSync.IpDetection;

public interface IIpDetector
{
    Task<string?> GetPublicIpAsync(IpVersion ipVersion, CancellationToken ct = default);
}