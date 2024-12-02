// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvExporter.cs" company="MareMare">
// Copyright © 2024 MareMare.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;
using CloudflareSpeedTester.Models;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace CloudflareSpeedTester.Exporters;

/// <summary>
/// 測定結果をCSVファイルにエクスポートするクラスです。
/// </summary>
internal sealed class CsvExporter : IResultExporter
{
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

        using var writer = new StreamWriter(filePath, append: true);
        using var csv = new CsvWriter(
            writer,
            new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                // ヘッダーを追加するかどうかを制御します
                HasHeaderRecord = !fileExists,
            });

        // DateTimeOffset 用のカスタムコンバータを登録します。
        csv.Context.TypeConverterCache.AddConverter<DateTimeOffset>(new IsoDateTimeOffsetConverter());

        if (!fileExists)
        {
            csv.WriteHeader<MeasurementResult>();
            csv.NextRecord();
        }

        csv.WriteRecord(result);
        csv.NextRecord();
    }

    /// <summary>
    /// DateTimeOffset 用のカスタムコンバータを表します。
    /// </summary>
    private sealed class IsoDateTimeOffsetConverter : DefaultTypeConverter
    {
        /// <inheritdoc />
        public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
        {
            if (value is DateTimeOffset dateTimeOffset)
            {
                return dateTimeOffset.ToString("o", CultureInfo.InvariantCulture); // "o" はISO 8601形式
            }

            return base.ConvertToString(value, row, memberMapData);
        }
    }
}
