// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="MareMare">
// Copyright © 2024 MareMare.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using CloudflareSpeedTester.Commands;
using CloudflareSpeedTester.Infrastructure;
using CloudflareSpeedTester.Models;
using CloudflareSpeedTester.Services;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Cli.Help;
using Spectre.Console.Rendering;

var registrations = new ServiceCollection();
registrations.AddHttpClient();
registrations.AddTransient<ISpeedTestService, SpeedTestService>();

// Create a type registrar and register any dependencies.
// A type registrar is an adapter for a DI framework.
var registrar = new CommandTypeRegistrar(registrations);

// Create a new command app with the registrar
// and run it with the provided arguments.
var app = new CommandApp<SpeedTestCommand>(registrar);

app.Configure(
    config =>
    {
        config.SetHelpProvider(new CustomHelpProvider(config.Settings));
    });
return app.Run(args);

/// <summary>
/// Provides custom help functionality for command-line applications.
/// </summary>
#pragma warning disable SA1649
file sealed class CustomHelpProvider : HelpProvider
#pragma warning restore SA1649
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomHelpProvider"/> class with the specified command application settings.
    /// </summary>
    /// <param name="settings">The settings for the command application.</param>
    public CustomHelpProvider(ICommandAppSettings settings)
        : base(settings)
    {
    }

    /// <inheritdoc />
    public override IEnumerable<IRenderable> GetHeader(ICommandModel model, ICommandInfo? command) =>
    [
        new Markup("Simple Speed Test CLI via [bold blue][link=https://speed.cloudflare.com]Cloudflare Speed Test[/][/]."),
        Text.NewLine,
        new Markup($"version: [yellow]{VersionInfo.Version}[/]"),
        Text.NewLine,
        Text.NewLine,
    ];
}
