// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IResultExporter.cs" company="MareMare">
// Copyright © 2024 MareMare.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using CloudflareSpeedTester.Models;

namespace CloudflareSpeedTester.Exporters;

/// <summary>
/// 結果をエクスポートするためのインターフェースです。
/// </summary>
public interface IResultExporter
{
    /// <summary>
    /// 測定結果を指定されたファイルにエクスポートします。
    /// </summary>
    /// <param name="result">エクスポートする測定結果。</param>
    /// <param name="filePath">エクスポート先のファイルパス。</param>
    /// <param name="forceNew">既存のファイルを上書きするかどうかを示すフラグ。</param>
    void Export(MeasurementResult result, string filePath, bool forceNew = false);
}
