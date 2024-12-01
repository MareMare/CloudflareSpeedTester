// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Enums.cs" company="MareMare">
// Copyright © 2024 MareMare.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CloudflareSpeedTester.Models;

/// <summary>
/// テストのカテゴリを表す列挙型です。
/// </summary>
internal enum TestCategory
{
    /// <summary>レイテンシを表します。</summary>
    Latency,

    /// <summary>スピードを表します。</summary>
    Speed,
}

/// <summary>
/// テストの方向を表す列挙型です。
/// </summary>
internal enum TestDirection
{
    /// <summary>ダウンロードを表します。</summary>
    Download,

    /// <summary>アップロードを表します。</summary>
    Upload,
}
