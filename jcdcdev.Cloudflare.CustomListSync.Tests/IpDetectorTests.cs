using jcdcdev.Cloudflare.CustomListSync.IpDetection;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace jcdcdev.Cloudflare.CustomListSync.Tests;

public sealed class IpDetectorTests
{
    [Fact]
    public async Task Detects_ipv4_from_first_service()
    {
        var detector = new IpDetector(new HttpClient(), NullLogger<IpDetector>.Instance);
        var ip = await detector.GetPublicIpAsync(IpVersion.V4);

        Assert.NotNull(ip);
    }
}