// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpeedTestSettings.cs" company="MareMare">
// Copyright © 2024 MareMare.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel;
using Spectre.Console.Cli;

namespace CloudflareSpeedTester.Settings;

/// <summary>
/// スピードテストの設定を表すクラスです。
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class SpeedTestSettings : CommandSettings
{
    /// <summary>
    /// メタデータを表示するかどうかを取得します。
    /// </summary>
    /// <value>
    /// 値を表す <see cref="bool" /> 型。
    /// <para>メタデータを表示するかどうか。</para>
    /// </value>
    [CommandOption("--display-metadata")]
    [Description("Displays speed test metadata")]
    [DefaultValue(true)]
    public bool DisplayMetadata { get; init; }

    /// <summary>
    /// 速度テストの概要を表示するかどうかを取得します。
    /// </summary>
    /// <value>
    /// 値を表す <see cref="bool" /> 型。
    /// <para>速度テストの概要を表示するかどうか。</para>
    /// </value>
    [CommandOption("--display-summary")]
    [Description("Displays speed test summary")]
    [DefaultValue(true)]
    public bool DisplaySummary { get; init; }

    /// <summary>
    /// JSON形式の結果を表示するかどうかを取得します。
    /// </summary>
    /// <value>
    /// 値を表す <see cref="bool" /> 型。
    /// <para>JSON形式の結果を表示するかどうか。</para>
    /// </value>
    [CommandOption("--display-json")]
    [Description("Displays speed test results in JSON format")]
    [DefaultValue(false)]
    public bool DisplayJson { get; init; }

    /// <summary>
    /// 出力するCSVのファイルパスを取得します。
    /// </summary>
    /// <value>
    /// 値を表す <see cref="string" /> 型。
    /// <para>出力するCSVのファイルパス。</para>
    /// </value>
    [CommandOption("--output-csv <PATH>")]
    [Description("Specifies the file path to output speed test results in CSV format")]
    public string CsvFilePath { get; init; } = default!;

    /// <summary>
    /// 出力するJSONのファイルパスを取得します。
    /// </summary>
    /// <value>
    /// 値を表す <see cref="string" /> 型。
    /// <para>出力するJSONのファイルパス。</para>
    /// </value>
    [CommandOption("--output-json <PATH>")]
    [Description("Specifies the file path to output speed test results in JSON format")]
    public string JsonFilePath { get; init; } = default!;

    /// <summary>
    /// 既存ファイルを上書きするかどうかを示すフラグを取得します。
    /// </summary>
    /// <value>
    /// 値を表す <see cref="bool" /> 型。
    /// <para>JSON形式の結果を表示するかどうか。</para>
    /// </value>
    [CommandOption("--force-new")]
    [Description("Flag to overwrite existing files or create new ones for the specified CsvFilePath or JsonFilePath")]
    [DefaultValue(false)]
    public bool ForceNew { get; init; }

    /// <summary>
    /// HTTPクライアントのタイムアウト時間を取得または設定します。
    /// </summary>
    /// <value>
    /// 値を表す <see cref="int" /> 型。
    /// <para>HTTPクライアントのタイムアウト時間（秒）。</para>
    /// </value>
    [CommandOption("--timeout <SECONDS>")]
    [Description("Specifies the timeout duration for the HTTP client in seconds")]
    [DefaultValue(90)]
    public int TimeoutSeconds { get; init; }
}
