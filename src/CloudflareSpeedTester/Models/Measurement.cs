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
/// <param name="Spec">テストの仕様を表す<see cref="TestSpec"/>。</param>
/// <param name="Duration">テストの実行時間を表す<see cref="TimeSpan"/>。</param>
/// <param name="Latency">レイテンシを表す値。</param>
/// <param name="BitsPerSeconds">ビット毎秒を表す値。</param>
internal sealed record Measurement(
    TestSpec Spec,
    TimeSpan Duration,
    double Latency,
    double BitsPerSeconds);
