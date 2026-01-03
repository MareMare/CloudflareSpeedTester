// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestResult.cs" company="MareMare">
// Copyright © 2024 MareMare.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CloudflareSpeedTester.Models;

/// <summary>
/// テスト結果を表すレコードです。
/// </summary>
/// <param name="Meta">ネットワークメタデータを表す<see cref="NetworkMetadata"/>。</param>
/// <param name="Result">測定結果を表す<see cref="MeasurementResult"/>。</param>
/// <param name="Pretty">人間が読みやすい形式の測定結果を表す<see cref="MeasurementPretty"/>。</param>
internal sealed record TestResult(
    NetworkMetadata Meta,
    MeasurementResult Result,
    MeasurementPretty Pretty);
