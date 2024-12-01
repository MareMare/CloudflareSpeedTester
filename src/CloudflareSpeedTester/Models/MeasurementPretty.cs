// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeasurementPretty.cs" company="MareMare">
// Copyright © 2024 MareMare.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;
using CloudflareSpeedTester.Extensions;

namespace CloudflareSpeedTester.Models;

/// <summary>
/// <see cref="MeasurementResult"/>を人間が読みやすい形式に変換するクラスです。
/// </summary>
/// <param name="result">変換対象の<see cref="MeasurementResult"/>。</param>
public sealed class MeasurementPretty(MeasurementResult result)
{
    /// <summary>
    /// ダウンロード速度を人間が読みやすい形式で取得します。
    /// </summary>
    /// <value>
    /// 値を表す <see cref="string" /> 型。
    /// <para>ダウンロード速度を人間が読みやすい形式の文字列。</para>
    /// </value>
    [property: JsonPropertyName("download")]
    public string? DownloadedSpeed { get; } = result.DownloadedSpeed.ToPrettyBps();

    /// <summary>
    /// アップロード速度を人間が読みやすい形式で取得します。
    /// </summary>
    /// <value>
    /// 値を表す <see cref="string" /> 型。
    /// <para>アップロード速度を人間が読みやすい形式の文字列。</para>
    /// </value>
    [property: JsonPropertyName("upload")]
    public string? UploadedSpeed { get; } = result.UploadedSpeed.ToPrettyBps();

    /// <summary>
    /// レイテンシを人間が読みやすい形式で取得します。
    /// </summary>
    /// <value>
    /// 値を表す <see cref="string" /> 型。
    /// <para>レイテンシを人間が読みやすい形式の文字列。</para>
    /// </value>
    [property: JsonPropertyName("latency")]
    public string? Latency { get; } = result.Latency.ToPrettyMilliseconds();

    /// <summary>
    /// ジッターを人間が読みやすい形式で取得します。
    /// </summary>
    /// <value>
    /// 値を表す <see cref="string" /> 型。
    /// <para>ジッターを人間が読みやすい形式の文字列。</para>
    /// </value>
    [property: JsonPropertyName("jitter")]
    public string? Jitter { get; } = result.Jitter.ToPrettyMilliseconds();

    /// <summary>
    /// ダウンロード時のレイテンシを人間が読みやすい形式で取得します。
    /// </summary>
    /// <value>
    /// 値を表す <see cref="string" /> 型。
    /// <para>ダウンロード時のレイテンシを人間が読みやすい形式の文字列。</para>
    /// </value>
    [property: JsonPropertyName("downLoadedLatency")]
    public string? DownloadedLatency { get; } = result.DownloadedLatency.ToPrettyMilliseconds();

    /// <summary>
    /// ダウンロード時のジッターを人間が読みやすい形式で取得します。
    /// </summary>
    /// <value>
    /// 値を表す <see cref="string" /> 型。
    /// <para>ダウンロード時のジッターを人間が読みやすい形式の文字列。</para>
    /// </value>
    [property: JsonPropertyName("downLoadedJitter")]
    public string? DownloadedJitter { get; } = result.DownloadedJitter.ToPrettyMilliseconds();

    /// <summary>
    /// アップロード時のレイテンシを人間が読みやすい形式で取得します。
    /// </summary>
    /// <value>
    /// 値を表す <see cref="string" /> 型。
    /// <para>アップロード時のレイテンシを人間が読みやすい形式の文字列。</para>
    /// </value>
    [property: JsonPropertyName("upLoadedLatency")]
    public string? UploadedLatency { get; } = result.UploadedLatency.ToPrettyMilliseconds();

    /// <summary>
    /// アップロード時のジッターを人間が読みやすい形式で取得します。
    /// </summary>
    /// <value>
    /// 値を表す <see cref="string" /> 型。
    /// <para>アップロード時のジッターを人間が読みやすい形式の文字列。</para>
    /// </value>
    [property: JsonPropertyName("upLoadedJitter")]
    public string? UploadedJitter { get; } = result.UploadedJitter.ToPrettyMilliseconds();
}
