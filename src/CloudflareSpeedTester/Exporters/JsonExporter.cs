// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonExporter.cs" company="MareMare">
// Copyright © 2024 MareMare.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json;
using CloudflareSpeedTester.Models;

namespace CloudflareSpeedTester.Exporters;

/// <summary>
/// 測定結果をJSONファイルにエクスポートするクラスです。
/// </summary>
internal sealed class JsonExporter : IResultExporter
{
    /// <summary><see cref="JsonSerializerOptions" /> を表します。</summary>
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true, };

    /// <inheritdoc />
    public void Export(MeasurementResult result, string filePath, bool forceNew = false)
    {
        // フォルダが存在しない場合は新規作成します。
        var directory = Path.GetDirectoryName(filePath) ?? string.Empty;
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // forceNew が true の場合、既存ファイルを削除して新規作成します。
        var fileExists = File.Exists(filePath);
        if (forceNew && fileExists)
        {
            File.Delete(filePath);
            fileExists = false;
        }

        List<MeasurementResult> results;
        if (fileExists)
        {
            // 既存の JSON ファイルを読み込みます。
            var jsonString = File.ReadAllText(filePath);
            results = JsonSerializer.Deserialize<List<MeasurementResult>>(jsonString) ?? new List<MeasurementResult>();
        }
        else
        {
            results = [];
        }

        results.Add(result);

        // リストを JSON 形式でファイルに書き込みます。
        var updatedJsonString = JsonSerializer.Serialize(results, this._jsonOptions);
        File.WriteAllText(filePath, updatedJsonString);
    }
}
