namespace jcdcdev.Cloudflare.CustomListSync.IpDetection;

public sealed class IpDetector(HttpClient httpClient, ILogger<IpDetector> logger) : IIpDetector
{
    private static readonly Dictionary<IpVersion, string[]> ServiceUrls = new()
    {
        {
            IpVersion.V4,
            [
                "https://ipv4.seeip.org/",
            ]
        },
        {
            IpVersion.V6,
            [
                "https://ipv6.seeip.org"
            ]
        }
    };

    public async Task<string?> GetPublicIpAsync(IpVersion ipVersion, CancellationToken ct = default)
    {
        if (!ServiceUrls.TryGetValue(ipVersion, out var urls))
        {
            logger.LogWarning("Unknown IP version '{IpVersion}' — expected 'v4' or 'v6'", ipVersion);
            return null;
        }

        foreach (var url in urls)
        {
            ct.ThrowIfCancellationRequested();

            var startedAt = DateTimeOffset.UtcNow;
            try
            {
                var response = await httpClient.GetAsync(url, ct);
                response.EnsureSuccessStatusCode();

                var ip = (await response.Content.ReadAsStringAsync(ct)).Trim();

                var elapsed = (DateTimeOffset.UtcNow - startedAt).TotalMilliseconds;
                logger.LogInformation("Detected public {IpVersion} address {Ip} via {Url} in {ElapsedMs:F0}ms", ipVersion, ip, url, elapsed);

                if (!string.IsNullOrEmpty(ip))
                {
                    return ip;
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                var elapsed = (DateTimeOffset.UtcNow - startedAt).TotalMilliseconds;
                logger.LogWarning("IP detection via {Url} failed in {ElapsedMs:F0}ms: {Error}", url, elapsed, ex.Message);
            }
        }

        logger.LogError("All IP detection services failed for {IpVersion}", ipVersion);
        return null;
    }
}