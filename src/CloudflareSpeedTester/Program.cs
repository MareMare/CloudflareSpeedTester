// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="MareMare">
// Copyright © 2024 MareMare.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using CloudflareSpeedTester.Commands;
using CloudflareSpeedTester.Infrastructure;
using CloudflareSpeedTester.Services;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

var registrations = new ServiceCollection();
registrations.AddHttpClient();
registrations.AddTransient<ISpeedTestService, SpeedTestService>();

// Create a type registrar and register any dependencies.
// A type registrar is an adapter for a DI framework.
var registrar = new CommandTypeRegistrar(registrations);

// Create a new command app with the registrar
// and run it with the provided arguments.
var app = new CommandApp<SpeedTestCommand>(registrar);
return app.Run(args);
