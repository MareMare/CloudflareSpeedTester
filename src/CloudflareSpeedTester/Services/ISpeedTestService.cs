// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISpeedTestService.cs" company="MareMare">
// Copyright © 2024 MareMare.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using CloudflareSpeedTester.Models;
using CloudflareSpeedTester.Settings;

namespace CloudflareSpeedTester.Services;

/// <summary>
/// スピードテストサービスを提供するインターフェースです。
/// </summary>
public interface ISpeedTestService : IDisposable
{
    /// <summary>
    /// 非同期操作として、スピードテストを実行します。
    /// </summary>
    /// <param name="settings">スピードテストの設定を表す<see cref="SpeedTestSettings"/>。</param>
    /// <returns>テスト結果を表す<see cref="TestResult"/>を返すタスク。</returns>
    Task<TestResult> RunAsync(SpeedTestSettings settings);
}
