// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpeedTestService.cs" company="MareMare">
// Copyright © 2024 MareMare.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Net.Http.Headers;
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

    /// <summary>HTTPクライアントファクトリを表します。</summary>
    private readonly IHttpClientFactory _httpClientFactory;

    /// <summary>
    /// <see cref="SpeedTestService"/> クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="httpClientFactory">HTTPクライアントファクトリ。</param>
    public SpeedTestService(IHttpClientFactory httpClientFactory)
    {
        this._httpClientFactory = httpClientFactory;
    }

    /// <inheritdoc />
    public async Task<TestResult> RunAsync(
        SpeedTestSettings settings,
        IReadOnlyCollection<TestSpec> testSpecs,
        DateTimeOffset startedAt)
    {
        using var client = this._httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.TryAddWithoutValidation(
            "User-Agent",
            $"{VersionInfo.ApplicationName}/{VersionInfo.Version}");
        client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);

        var metadata = await SpeedTestService.FetchMetadataAsync(client).ConfigureAwait(false);

        List<Measurement> measurements = [];
        foreach (var spec in testSpecs)
        {
            var partialMeasurements = await SpeedTestService.RunTestSpecAsync(client, spec).ConfigureAwait(false);
            measurements.AddRange(partialMeasurements);
        }

        var result = measurements.ToResult(startedAt);
        var pretty = new MeasurementPretty(result);
        var testResult = new TestResult(metadata, result, pretty);
        return testResult;
    }

    /// <summary>
    /// ネットワークメタデータを非同期で取得します。
    /// </summary>
    /// <param name="client">HTTPクライアント。</param>
    /// <returns>ネットワークメタデータを表す<see cref="NetworkMetadata"/>。</returns>
    private static async Task<NetworkMetadata> FetchMetadataAsync(HttpClient client)
    {
        var url = $"{SpeedTestService._baseUrl}/{SpeedTestService._downloadUrl}0";
        var response = await client.GetAsync(new Uri(url)).ConfigureAwait(false);
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
    /// <param name="client">HTTPクライアント。</param>
    /// <param name="spec">テスト仕様を表す<see cref="TestSpec"/>。</param>
    /// <returns>測定結果のコレクション。</returns>
    private static async Task<IReadOnlyCollection<Measurement>> RunTestSpecAsync(HttpClient client, TestSpec spec)
    {
        spec.StartProgress();

        List<Measurement> measurements = [];
        for (var index = 0; index < spec.Iterations; index++)
        {
            var measurement = spec.Direction == TestDirection.Upload
                ? await MeasureUploadAsync(spec).ConfigureAwait(false)
                : await MeasureDownloadAsync(spec).ConfigureAwait(false);

            measurements.Add(measurement);
            spec.IncrementProgress();
        }

        return measurements;

        async Task<Measurement> MeasureUploadAsync(TestSpec testSpec)
        {
            var url = $"{SpeedTestService._baseUrl}/{SpeedTestService._uploadUrl}";
            var payload = new byte[testSpec.BytesSize];
            var sw = Stopwatch.StartNew();
            using var payloadBytesContent = new ByteArrayContent(payload);
            var response = await client.PostAsync(new Uri(url), payloadBytesContent).ConfigureAwait(false);
            sw.Stop();
            return MeasureTimes(testSpec, sw.Elapsed, response);
        }

        async Task<Measurement> MeasureDownloadAsync(TestSpec testSpec)
        {
            var url = $"{SpeedTestService._baseUrl}/{SpeedTestService._downloadUrl}{testSpec.BytesSize}";
            var sw = Stopwatch.StartNew();
            var response = await client.GetAsync(new Uri(url)).ConfigureAwait(false);
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
                Category: testSpec.Category,
                Direction: testSpec.Direction,
                BytesSize: testSpec.BytesSize,
                Duration: duration,
                Latency: latency,
                BitsPerSeconds: bps);
        }
    }
}
