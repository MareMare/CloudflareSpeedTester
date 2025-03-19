// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpeedTestCommand.cs" company="MareMare">
// Copyright © 2024 MareMare.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using CloudflareSpeedTester.Exporters;
using CloudflareSpeedTester.Models;
using CloudflareSpeedTester.Services;
using CloudflareSpeedTester.Settings;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Json;

namespace CloudflareSpeedTester.Commands;

/// <summary>
/// Cloudflareサービスを使用して速度テストを実行するコマンドを表します。
/// </summary>
/// <param name="service"><see cref="ISpeedTestService" />。</param>
// ReSharper disable once ClassNeverInstantiated.Global
#pragma warning disable CA1812
internal sealed class SpeedTestCommand(ISpeedTestService service) : AsyncCommand<SpeedTestSettings>
#pragma warning restore CA1812
{
    /// <summary><see cref="JsonSerializerOptions" /> を表します。</summary>
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true, };

    /// <summary><see cref="ISpeedTestService" /> を表します。</summary>
    private readonly ISpeedTestService _service = service;

    /// <inheritdoc />
    public override async Task<int> ExecuteAsync(CommandContext context, SpeedTestSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        var startedAt = DateTimeOffset.UtcNow;
        AnsiConsole.MarkupLine(
            $"[green]Starting [blue][link=https://speed.cloudflare.com]Cloudflare Speed Test[/][/][/] at {startedAt:yyyy-MM-ddTHH:mm:ss.fffffffzzz}");

        var testSpecs = TestSpec.CreateTestSpecs()
            .OrderBy(spec => spec.Category)
            .ThenBy(spec => spec.BytesSize)
            .ThenBy(spec => spec.Direction)
            .ToArray();

        TestResult? testResult = null;
        await AnsiConsole.Progress()
            .AutoClear(true)
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new SpinnerColumn())
            .StartAsync(
                async ctx =>
                {
                    foreach (var spec in testSpecs)
                    {
                        spec.SetProgressTask(
                            ctx.AddTask(spec.Description, autoStart: false, maxValue: spec.Iterations));
                    }

                    try
                    {
                        testResult = await this._service.RunAsync(settings, testSpecs, startedAt).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        AnsiConsole.WriteException(ex);
                        throw; // 再スローします。
                    }
                })
            .ConfigureAwait(false);

        if (settings.DisplayMetadata)
        {
            SpeedTestCommand.DisplayMetadata(testResult);
        }

        if (settings.DisplaySummary)
        {
            SpeedTestCommand.DisplaySpeedTestSummary(testResult);
        }

        if (settings.DisplayJson)
        {
            SpeedTestCommand.DisplayJsonResults(testResult);
        }

        if (!string.IsNullOrEmpty(settings.CsvFilePath))
        {
            SpeedTestCommand.SaveToCsv(testResult, settings.CsvFilePath, settings.ForceNew);
            AnsiConsole.MarkupLine($"Speed test results successfully exported to CSV file: [yellow]{settings.CsvFilePath}[/]");
        }

        if (!string.IsNullOrEmpty(settings.JsonFilePath))
        {
            SpeedTestCommand.SaveToJson(testResult, settings.JsonFilePath, settings.ForceNew);
            AnsiConsole.MarkupLine($"Speed test results successfully exported to JSON file: [yellow]{settings.JsonFilePath}[/]");
        }

        return 0;
    }

    /// <summary>
    /// メタデータを表示します。
    /// </summary>
    /// <param name="testResult">テスト結果。</param>
    private static void DisplayMetadata(TestResult? testResult)
    {
        var grid = new Grid();
        grid.AddColumn(new GridColumn().NoWrap());
        grid.AddColumn(new GridColumn().NoWrap());
        grid.AddRow("City", testResult?.Meta.City ?? string.Empty);
        grid.AddRow("Country", testResult?.Meta.Country ?? string.Empty);
        grid.AddRow("IP", MaskIpAddress(testResult?.Meta.Ip ?? string.Empty));
        grid.AddRow("Asn", testResult?.Meta.Asn ?? string.Empty);
        grid.AddRow("Colo", testResult?.Meta.Colo ?? string.Empty);
        AnsiConsole.Write(
            new Panel(grid)
                .Header("[yellow]Metadata:[/]"));
        return;

        static string MaskIpAddress(string ipAddress)
        {
            if (!IPAddress.TryParse(ipAddress, out var address))
            {
                return ipAddress; // Invalid IP address
            }

            if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                // IPv4
                return Regex.Replace(ipAddress, @"(\d+)\.(\d+)\.(\d+)\.(\d+)", "$1.$2.***.***");
            }

            if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                // IPv6
                return Regex.Replace(
                    ipAddress,
                    @"([0-9a-fA-F]{1,4}):([0-9a-fA-F]{1,4}):([0-9a-fA-F]{1,4}):([0-9a-fA-F]{1,4}):([0-9a-fA-F]{1,4}):([0-9a-fA-F]{1,4}):([0-9a-fA-F]{1,4}):([0-9a-fA-F]{1,4})",
                    "$1:$2:$3:$4:****:****:****:****");
            }

            return ipAddress; // Invalid IP address
        }
    }

    /// <summary>
    /// 速度テストの概要を表示します。
    /// </summary>
    /// <param name="testResult">テスト結果。</param>
    private static void DisplaySpeedTestSummary(TestResult? testResult)
    {
        var grid = new Grid();
        grid.AddColumn(new GridColumn().NoWrap());
        grid.AddColumn(new GridColumn().Alignment(Justify.Right));
        grid.AddRow("Download", testResult?.Pretty.DownloadedSpeed ?? string.Empty);
        grid.AddRow("Upload", testResult?.Pretty.UploadedSpeed ?? string.Empty);
        grid.AddRow("Latency", testResult?.Pretty.Latency ?? string.Empty);
        grid.AddRow("Jitter", testResult?.Pretty.Jitter ?? string.Empty);
        grid.AddRow("Downloaded Latency", testResult?.Pretty.DownloadedLatency ?? string.Empty);
        grid.AddRow("Downloaded Jitter", testResult?.Pretty.DownloadedJitter ?? string.Empty);
        grid.AddRow("Uploaded Latency", testResult?.Pretty.UploadedLatency ?? string.Empty);
        grid.AddRow("Uploaded Jitter", testResult?.Pretty.UploadedJitter ?? string.Empty);
        AnsiConsole.Write(
            new Panel(grid)
                .Header("[yellow]Speed Test Results Summary:[/]"));
    }

    /// <summary>
    /// テスト結果をJSON形式で表示します。
    /// </summary>
    /// <param name="testResult">テスト結果。</param>
    private static void DisplayJsonResults(TestResult? testResult)
    {
        var resultJson = JsonSerializer.Serialize(testResult, SpeedTestCommand._jsonOptions);
        AnsiConsole.Write(
            new Panel(new JsonText(resultJson))
                .Header("[yellow]Speed Test Results in Json:[/]")
                .Collapse());
    }

    /// <summary>
    /// テスト結果をCSVファイルとして保存します。
    /// </summary>
    /// <param name="testResult">テスト結果。</param>
    /// <param name="filePath">出力するファイルパス。</param>
    /// <param name="forceNew">上書きするかどうか。</param>
    private static void SaveToCsv(TestResult? testResult, string filePath, bool forceNew)
    {
        var measurementResult = testResult?.Result;
        if (measurementResult is not null)
        {
            var exporter = new CsvExporter();
            exporter.Export(measurementResult, filePath, forceNew);
        }
    }

    /// <summary>
    /// テスト結果をJSONファイルとして保存します。
    /// </summary>
    /// <param name="testResult">テスト結果。</param>
    /// <param name="filePath">出力するファイルパス。</param>
    /// <param name="forceNew">上書きするかどうか。</param>
    private static void SaveToJson(TestResult? testResult, string filePath, bool forceNew)
    {
        var measurementResult = testResult?.Result;
        if (measurementResult is not null)
        {
            var exporter = new JsonExporter();
            exporter.Export(measurementResult, filePath, forceNew);
        }
    }
}
