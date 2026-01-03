// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeasurementResult.cs" company="MareMare">
// Copyright © 2024 MareMare.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace CloudflareSpeedTester.Models;

/// <summary>
/// 測定結果を表すレコードです。
/// </summary>
/// <param name="StartedAt">テストを開始した日時(UTC)。</param>
/// <param name="DownloadedSpeed">ダウンロード速度を表す値。</param>
/// <param name="UploadedSpeed">アップロード速度を表す値。</param>
/// <param name="Latency">レイテンシを表す値。</param>
/// <param name="Jitter">ジッターを表す値。</param>
/// <param name="DownloadedLatency">ダウンロード時のレイテンシを表す値。</param>
/// <param name="DownloadedJitter">ダウンロード時のジッターを表す値。</param>
/// <param name="UploadedLatency">アップロード時のレイテンシを表す値。</param>
/// <param name="UploadedJitter">アップロード時のジッターを表す値。</param>
internal sealed record MeasurementResult(
    [property: JsonPropertyName("startedAt")]
    DateTimeOffset StartedAt,
    [property: JsonPropertyName("download")]
    double? DownloadedSpeed,
    [property: JsonPropertyName("upload")] double? UploadedSpeed,
    [property: JsonPropertyName("latency")]
    double? Latency,
    [property: JsonPropertyName("jitter")] double? Jitter,
    [property: JsonPropertyName("downLoadedLatency")]
    double? DownloadedLatency,
    [property: JsonPropertyName("downLoadedJitter")]
    double? DownloadedJitter,
    [property: JsonPropertyName("upLoadedLatency")]
    double? UploadedLatency,
    [property: JsonPropertyName("upLoadedJitter")]
    double? UploadedJitter);
