using jcdcdev.Cloudflare.CustomListSync.Configuration;
using Xunit;

namespace jcdcdev.Cloudflare.CustomListSync.Tests;

public sealed class AppOptionsTests
{
    [Fact]
    public void Defaults_HaveExpectedValues()
    {
        var opts = new AppOptions();

        Assert.Equal(string.Empty, opts.ApiToken);
        Assert.Equal(string.Empty, opts.AccountId);
        Assert.Equal(string.Empty, opts.ListIds);
        Assert.Equal(300, opts.SyncIntervalSeconds);
        Assert.False(opts.KeepOrphans);
    }

    [Fact]
    public void ParsedListIds_SplitsCommaSeparated()
    {
        var opts = new AppOptions { ListIds = "aaa, bbb,ccc" };

        Assert.Equal(["aaa", "bbb", "ccc"], opts.ParsedListIds);
    }

    [Fact]
    public void ParsedListIds_EmptyWhenNoIds()
    {
        var opts = new AppOptions();

        Assert.Empty(opts.ParsedListIds);
    }

    [Fact]
    public void ParsedListIds_TrimsAndIgnoresEmpty()
    {
        var opts = new AppOptions { ListIds = " , aa , , bb , " };

        Assert.Equal(["aa", "bb"], opts.ParsedListIds);
    }

    [Fact]
    public void SyncIntervalSeconds_CanBeOverridden()
    {
        var opts = new AppOptions { SyncIntervalSeconds = 60 };

        Assert.Equal(60, opts.SyncIntervalSeconds);
    }

    [Fact]
    public void KeepOrphans_CanBeSetToTrue()
    {
        var opts = new AppOptions { KeepOrphans = true };

        Assert.True(opts.KeepOrphans);
    }
}
