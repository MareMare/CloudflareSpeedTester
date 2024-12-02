// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeasurementExtensions.cs" company="MareMare">
// Copyright © 2024 MareMare.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using CloudflareSpeedTester.Extensions;

namespace CloudflareSpeedTester.Models;

/// <summary>
/// <see cref="Measurement"/>の拡張メソッドを提供するクラスです。
/// </summary>
internal static class MeasurementExtensions
{
    /// <summary>
    /// 測定結果のコレクションを<see cref="MeasurementResult"/>に変換します。
    /// </summary>
    /// <param name="allMeasurements">すべての測定結果のコレクション。</param>
    /// <param name="startedAt">テストを開始した日時(UTC)。</param>
    /// <returns>変換された<see cref="MeasurementResult"/>。</returns>
    public static MeasurementResult ToResult(this IEnumerable<Measurement> allMeasurements, DateTimeOffset startedAt)
    {
        var measurements = allMeasurements.ToArray();

        var downloadedSpeeds = measurements.FilterMeasurements(
            TestCategory.Speed,
            TestDirection.Download,
            m => m.BitsPerSeconds);
        var uploadedSpeeds = measurements.FilterMeasurements(
            TestCategory.Speed,
            TestDirection.Upload,
            m => m.BitsPerSeconds);
        var latencies = measurements.FilterMeasurements(
            TestCategory.Latency,
            null,
            m => m.Latency);
        var downloadedLatencies = measurements.FilterMeasurements(
            TestCategory.Speed,
            TestDirection.Download,
            m => m.Latency);
        var uploadedLatencies = measurements.FilterMeasurements(
            TestCategory.Speed,
            TestDirection.Upload,
            m => m.Latency);

        var downloadedSpeed90ThPercentile = downloadedSpeeds.Percentile();
        var uploadedSpeed90ThPercentile = uploadedSpeeds.Percentile();

        var latency = latencies.Median();
        var jitter = latencies.ToJitter();

        var downloadedLatency = downloadedLatencies.Median();
        var downloadedJitter = downloadedLatencies.ToJitter();

        var uploadedLatency = uploadedLatencies.Median();
        var uploadedJitter = uploadedLatencies.ToJitter();

        var result = new MeasurementResult(
            StartedAt: startedAt,
            DownloadedSpeed: downloadedSpeed90ThPercentile,
            UploadedSpeed: uploadedSpeed90ThPercentile,
            Latency: latency,
            Jitter: jitter,
            DownloadedLatency: downloadedLatency,
            DownloadedJitter: downloadedJitter,
            UploadedLatency: uploadedLatency,
            UploadedJitter: uploadedJitter);
        return result;
    }

    /// <summary>
    /// 測定結果をフィルタリングし、指定されたカテゴリと方向に基づいて値を抽出します。
    /// </summary>
    /// <param name="measurements">測定結果のコレクション。</param>
    /// <param name="category">フィルタリングするテストのカテゴリ。</param>
    /// <param name="direction">フィルタリングするテストの方向。方向が指定されていない場合はnull。</param>
    /// <param name="selector">測定結果から値を抽出するための関数。</param>
    /// <returns>フィルタリングされた値の配列。</returns>
    private static double[] FilterMeasurements(
        this IEnumerable<Measurement> measurements,
        TestCategory category,
        TestDirection? direction,
        Func<Measurement, double> selector) =>
        measurements
            .Where(
                measurement =>
                    measurement.Category == category &&
                    (!direction.HasValue || measurement.Direction == direction))
            .Select(selector)
            .ToArray();
}
