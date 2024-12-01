// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpeedTestCommand.cs" company="MareMare">
// Copyright © 2024 MareMare.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json;
using CloudflareSpeedTester.Services;
using CloudflareSpeedTester.Settings;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CloudflareSpeedTester.Commands;

/// <summary>
/// Cloudflareサービスを使用して速度テストを実行するコマンドを表します。
/// </summary>
/// <param name="service"><see cref="ISpeedTestService" />。</param>
public sealed class SpeedTestCommand(ISpeedTestService service) : AsyncCommand<SpeedTestSettings>
{
    /// <summary><see cref="ISpeedTestService" /> を表します。</summary>
    private readonly ISpeedTestService _service = service;

    /// <summary><see cref="JsonSerializerOptions" /> を表します。</summary>
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true, };

    /// <inheritdoc />
    public override async Task<int> ExecuteAsync(CommandContext context, SpeedTestSettings settings)
    {
        var testResult = await this._service.RunAsync(settings).ConfigureAwait(false);
        var resultJson = JsonSerializer.Serialize(testResult, this._jsonOptions);
        AnsiConsole.MarkupLine(resultJson);
        return 0;
    }
}
