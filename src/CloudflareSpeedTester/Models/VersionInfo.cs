// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionInfo.cs" company="MareMare">
// Copyright © 2024 MareMare.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Reflection;

namespace CloudflareSpeedTester.Models;

/// <summary>
/// アプリケーションのバージョンと名前に関する情報を提供します。
/// </summary>
internal static class VersionInfo
{
    /// <summary>
    /// アプリケーション名を取得します。
    /// </summary>
    /// <value>
    /// 値を表す <see cref="string" /> 型。
    /// <para>アプリケーション名。</para>
    /// </value>
    public static string ApplicationName { get; } = Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule?.ModuleName) ?? string.Empty;

    /// <summary>
    /// バージョン名を取得します。
    /// </summary>
    /// <value>
    /// 値を表す <see cref="string" /> 型。
    /// <para>バージョン名。</para>
    /// </value>
    public static string Version { get; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? string.Empty;
}
