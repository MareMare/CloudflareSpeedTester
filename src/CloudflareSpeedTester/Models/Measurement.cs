// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Measurement.cs" company="MareMare">
// Copyright © 2024 MareMare.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CloudflareSpeedTester.Models;

/// <summary>
/// テストの測定結果を表すレコードです。
/// </summary>
/// <param name="Category">テストのカテゴリを表す<see cref="TestCategory"/>。</param>
/// <param name="Direction">テストの方向を表す<see cref="TestDirection"/>。</param>
/// <param name="BytesSize">テストデータのサイズを表します。</param>
/// <param name="Duration">テストの実行時間を表す<see cref="TimeSpan"/>。</param>
/// <param name="Latency">レイテンシを表す値。</param>
/// <param name="BitsPerSeconds">ビット毎秒を表す値。</param>
internal sealed record Measurement(
    TestCategory Category,
    TestDirection Direction,
    int BytesSize,
    TimeSpan Duration,
    double Latency,
    double BitsPerSeconds);
