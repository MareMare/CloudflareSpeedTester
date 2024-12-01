// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestSpec.cs" company="MareMare">
// Copyright © 2024 MareMare.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CloudflareSpeedTester.Models;

/// <summary>
/// テストの仕様を表すレコードです。
/// </summary>
/// <param name="Name">テストの名前を表します。</param>
/// <param name="Category">テストのカテゴリを表す<see cref="TestCategory"/>。</param>
/// <param name="Direction">テストの方向を表す<see cref="TestDirection"/>。</param>
/// <param name="Size">テストデータのサイズを表します。</param>
/// <param name="Iterations">テストの反復回数を表します。</param>
internal sealed record TestSpec(
    string Name,
    TestCategory Category,
    TestDirection Direction,
    int Size,
    int Iterations)
{
    /// <summary>
    /// テストデータのビット単位のサイズを取得します。
    /// </summary>
    /// <value>
    /// 値を表す <see cref="int" /> 型。
    /// <para>テストデータのビット単位のサイズ。</para>
    /// </value>
    public int Bits => this.Size * 8;
}
