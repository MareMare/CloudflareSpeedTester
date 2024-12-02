// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestSpec.cs" company="MareMare">
// Copyright © 2024 MareMare.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Spectre.Console;

namespace CloudflareSpeedTester.Models;

/// <summary>
/// テスト仕様を表すレコードです。
/// </summary>
/// <param name="Name">テストの名前を表します。</param>
/// <param name="Category">テストのカテゴリを表す<see cref="TestCategory"/>。</param>
/// <param name="Direction">テストの方向を表す<see cref="TestDirection"/>。</param>
/// <param name="BytesSize">テストデータのサイズを表します。</param>
/// <param name="Iterations">テストの反復回数を表します。</param>
internal sealed record TestSpec(
    string Name,
    TestCategory Category,
    TestDirection Direction,
    int BytesSize,
    int Iterations)
{
    /// <summary><see cref="Spectre.Console.ProgressTask"/> を表します。</summary>
    private ProgressTask? _progressTask;

    /// <summary>
    /// テストデータのビット単位のサイズを取得します。
    /// </summary>
    /// <value>
    /// 値を表す <see cref="int" /> 型。
    /// <para>テストデータのビット単位のサイズ。</para>
    /// </value>
    public int Bits => this.BytesSize * 8;

    /// <summary>
    /// テスト仕様の説明を取得します。
    /// </summary>
    /// <value>
    /// 値を表す <see cref="string" /> 型。
    /// <para>テスト仕様の説明。</para>
    /// </value>
    public string Description =>
        this.Category == TestCategory.Latency ? this.Name : $"{this.Direction} {this.Name}";

    /// <summary>
    /// テスト仕様のコレクションを作成します。
    /// </summary>
    /// <returns>テスト仕様のコレクション。</returns>
    public static IReadOnlyCollection<TestSpec> CreateTestSpecs() =>
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
    /// 進捗タスクを設定します。
    /// </summary>
    /// <param name="progressTask"><see cref="Spectre.Console.ProgressTask"/>。</param>
    public void SetProgressTask(ProgressTask progressTask) => this._progressTask = progressTask;

    /// <summary>
    /// 進捗を開始します。
    /// </summary>
    public void StartProgress() => this._progressTask?.StartTask();

    /// <summary>
    /// 進捗をインクリメントします。
    /// </summary>
    public void IncrementProgress() => this._progressTask?.Increment(1d);
}
