using jcdcdev.Cloudflare.CustomListSync;
using jcdcdev.Cloudflare.CustomListSync.Cloudflare;
using jcdcdev.Cloudflare.CustomListSync.Config;
using jcdcdev.Cloudflare.CustomListSync.Configuration;
using jcdcdev.Cloudflare.CustomListSync.IpDetection;
using jcdcdev.Cloudflare.CustomListSync.ListSync;
using Polly;
using Polly.Extensions.Http;

var builder = Host.CreateApplicationBuilder(args);

var apiToken = builder.Configuration.GetValue("CF_API_TOKEN", string.Empty);
var accountId = builder.Configuration.GetValue("CF_ACCOUNT_ID", string.Empty);
var listIds = builder.Configuration.GetValue("CF_LIST_IDS", string.Empty);
var syncIntervalSeconds = builder.Configuration.GetValue("CF_SYNC_INTERVAL_SECONDS", 300);
var keepOrphans = builder.Configuration.GetValue("CF_KEEP_ORPHANS", false);
var entries = builder.Configuration.GetValue("CF_ENTRIES", string.Empty);
var filePath = builder.Configuration.GetValue("CF_FILE_PATH", string.Empty);

var options = new AppOptions
{
    ApiToken = apiToken,
    AccountId = accountId,
    ListIds = listIds,
    SyncIntervalSeconds = syncIntervalSeconds,
    KeepOrphans = keepOrphans,
    Entries = entries,
    FilePath = filePath
};

builder.Services
    .AddOptions<AppOptions>()
    .Configure(opts =>
    {
        opts.ApiToken = options.ApiToken;
        opts.AccountId = options.AccountId;
        opts.ListIds = options.ListIds;
        opts.SyncIntervalSeconds = options.SyncIntervalSeconds;
        opts.KeepOrphans = options.KeepOrphans;
        opts.Entries = options.Entries;
        opts.FilePath = options.FilePath;
    })
    .ValidateDataAnnotations()
    .Validate(x =>
    {
        if (string.IsNullOrWhiteSpace(x.FilePath))
        {
            return true;
        }

        return File.Exists(x.FilePath);
    }, "CSV file path is invalid or file does not exist.")
    .Validate(x => !string.IsNullOrWhiteSpace(x.Entries),
        "No entries found."
    );


var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
        onRetry: (outcome, timespan, attempt, _) => { });

builder.Services.AddHttpClient<ICloudflareApiClient, CloudflareApiClient>(client =>
    {
        client.BaseAddress = new Uri("https://api.cloudflare.com/client/v4/");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {options.ApiToken}");
    })
    .AddPolicyHandler(retryPolicy);

builder.Services.AddHttpClient<IIpDetector, IpDetector>(client => { client.Timeout = TimeSpan.FromSeconds(5); });
builder.Services.AddSingleton<ConfigProviderResolver>();
builder.Services.AddSingleton<IListSyncer, ListSyncer>();

builder.Services.AddHostedService<SyncWorker>();

var host = builder.Build();
host.Run();