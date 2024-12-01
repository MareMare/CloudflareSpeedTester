// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpeedTestService.cs" company="MareMare">
// Copyright © 2024 MareMare.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.RegularExpressions;
using CloudflareSpeedTester.Models;
using CloudflareSpeedTester.Settings;

namespace CloudflareSpeedTester.Services;

/// <summary>
/// スピードテストサービスを提供するクラスです。
/// </summary>
#pragma warning disable CA1812
internal sealed class SpeedTestService : ISpeedTestService
#pragma warning restore CA1812
{
    /// <summary>ベースURLを表します。</summary>
    private const string _baseUrl = "https://speed.cloudflare.com";

    /// <summary>ダウンロードURLのパスを表します。</summary>
    private const string _downloadUrl = "__down?bytes=";

    /// <summary>アップロードURLのパスを表します。</summary>
    private const string _uploadUrl = "__up";

    /// <summary>HTTPクライアントを表します。</summary>
    private readonly HttpClient _client;

    /// <summary>テスト仕様のコレクションを表します。</summary>
    private readonly IReadOnlyCollection<TestSpec> _testSpecs;

    /// <summary>
    /// <see cref="SpeedTestService"/> クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="httpClientFactory">HTTPクライアントファクトリ。</param>
    public SpeedTestService(IHttpClientFactory httpClientFactory)
    {
        var processName = Process.GetCurrentProcess().MainModule?.ModuleName;
        var appAssemblyName = Assembly.GetExecutingAssembly().GetName();
        var userAgent = $"{Path.GetFileNameWithoutExtension(processName)}/{appAssemblyName.Version?.ToString(3)}";

        this._client = httpClientFactory.CreateClient();
        this._client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", userAgent);
        this._client.Timeout = TimeSpan.FromSeconds(10);

        this._testSpecs = SpeedTestService.CreateTestSpecs();
    }

    /// <inheritdoc />
    public void Dispose() =>
        this._client.Dispose();

    /// <inheritdoc />
    public async Task<TestResult> RunAsync(SpeedTestSettings settings)
    {
        var metadata = await this.FetchMetadataAsync().ConfigureAwait(false);

        List<Measurement> measurements = [];
        foreach (var spec in this._testSpecs)
        {
            var partialMeasurements = await this.RunTestSpecAsync(spec).ConfigureAwait(false);
            measurements.AddRange(partialMeasurements);
        }

        var result = measurements.ToResult();
        var pretty = new MeasurementPretty(result);
        var testResult = new TestResult(metadata, result, pretty);
        return testResult;
    }

    /// <summary>
    /// テスト仕様のコレクションを作成します。
    /// </summary>
    /// <returns>テスト仕様のコレクション。</returns>
    private static IReadOnlyCollection<TestSpec> CreateTestSpecs() =>
    [
        new("latency", TestCategory.Latency, TestDirection.Download, 0, 20),
        new("100 kB", TestCategory.Speed, TestDirection.Download, 100_000, 10),
        new("1 MB", TestCategory.Speed, TestDirection.Download, 1_000_000, 8),
        new("10 MB", TestCategory.Speed, TestDirection.Download, 10_000_000, 6),
        new("25 MB", TestCategory.Speed, TestDirection.Download, 25_000_000, 4),
        new("100 kB", TestCategory.Speed, TestDirection.Upload, 100_000, 8),
        new("1 MB", TestCategory.Speed, TestDirection.Upload, 1_000_000, 6),
        new("10 MB", TestCategory.Speed, TestDirection.Upload, 10_000_000, 4),
    ];

    /// <summary>
    /// ネットワークメタデータを非同期で取得します。
    /// </summary>
    /// <returns>ネットワークメタデータを表す<see cref="NetworkMetadata"/>。</returns>
    private async Task<NetworkMetadata> FetchMetadataAsync()
    {
        var url = $"{SpeedTestService._baseUrl}/{SpeedTestService._downloadUrl}0";
        var response = await this._client.GetAsync(new Uri(url)).ConfigureAwait(false);
        var headers = response.Headers;
        return new NetworkMetadata(
            City: ExtractHeaderValue(headers, "cf-meta-city", "City N/A"),
            Country: ExtractHeaderValue(headers, "cf-meta-country", "Country N/A"),
            Ip: ExtractHeaderValue(headers, "cf-meta-ip", "IP N/A"),
            Asn: ExtractHeaderValue(headers, "cf-meta-asn", "ASN N/A"),
            Colo: ExtractHeaderValue(headers, "cf-meta-colo", "Colo N/A"));

        static string ExtractHeaderValue(HttpHeaders headers, string headerName, string naValue) =>
            headers.TryGetValues(headerName, out var values)
                ? values.FirstOrDefault() ?? naValue
                : naValue;
    }

    /// <summary>
    /// 非同期操作として、テスト仕様に基づいて測定を非同期で実行します。
    /// </summary>
    /// <param name="spec">テスト仕様を表す<see cref="TestSpec"/>。</param>
    /// <returns>測定結果のコレクション。</returns>
    private async Task<IReadOnlyCollection<Measurement>> RunTestSpecAsync(TestSpec spec)
    {
        List<Measurement> measurements = [];
        for (var index = 0; index < spec.Iterations; index++)
        {
            var measurement = spec.Direction == TestDirection.Upload
                ? await MeasureUploadAsync(spec).ConfigureAwait(false)
                : await MeasureDownloadAsync(spec).ConfigureAwait(false);

            measurements.Add(measurement);
        }

        return measurements;

        async Task<Measurement> MeasureUploadAsync(TestSpec testSpec)
        {
            var url = $"{SpeedTestService._baseUrl}/{SpeedTestService._uploadUrl}";
            var payload = new byte[testSpec.Size];
            var sw = Stopwatch.StartNew();
            using var payloadBytesContent = new ByteArrayContent(payload);
            var response = await this._client.PostAsync(new Uri(url), payloadBytesContent).ConfigureAwait(false);
            sw.Stop();
            return MeasureTimes(testSpec, sw.Elapsed, response);
        }

        async Task<Measurement> MeasureDownloadAsync(TestSpec testSpec)
        {
            var url = $"{SpeedTestService._baseUrl}/{SpeedTestService._downloadUrl}{testSpec.Size}";
            var sw = Stopwatch.StartNew();
            var response = await this._client.GetAsync(new Uri(url)).ConfigureAwait(false);
            sw.Stop();
            return MeasureTimes(testSpec, sw.Elapsed, response);
        }

        static Measurement MeasureTimes(TestSpec testSpec, TimeSpan duration, HttpResponseMessage response)
        {
            var serverTiming = response.Headers.GetValues("Server-Timing").FirstOrDefault();
            var match = Regex.Match(serverTiming ?? string.Empty, @"cfRequestDuration;dur=([\d.]+)");
            var cfReqDurationText = match.Success ? match.Groups[1].Value : string.Empty;
            var cfReqDurationMilliseconds = double.TryParse(cfReqDurationText, out var value) ? value : 0;
            var latency = duration.TotalMilliseconds - cfReqDurationMilliseconds;
            if (latency < 0)
            {
                latency = 0;
            }

            var bps = testSpec.Bits / duration.TotalSeconds;
            return new Measurement(
                Spec: testSpec,
                Duration: duration,
                Latency: latency,
                BitsPerSeconds: bps);
        }
    }
}
