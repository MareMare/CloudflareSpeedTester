// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandTypeRegistrar.cs" company="MareMare">
// Copyright © 2024 MareMare.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace CloudflareSpeedTester.Infrastructure;

/// <summary>
/// Represents a registrar for command types, integrating with the dependency injection framework.
/// </summary>
internal sealed class CommandTypeRegistrar(IServiceCollection builder) : ITypeRegistrar
{
    /// <inheritdoc />
    public ITypeResolver Build() =>
        new CommandTypeResolver(builder.BuildServiceProvider());

    /// <inheritdoc />
    public void Register(Type service, Type implementation) =>
        builder.AddSingleton(service, implementation);

    /// <inheritdoc />
    public void RegisterInstance(Type service, object implementation) =>
        builder.AddSingleton(service, implementation);

    /// <inheritdoc />
    public void RegisterLazy(Type service, Func<object> func)
    {
        ArgumentNullException.ThrowIfNull(func);
        builder.AddSingleton(service, _ => func());
    }
}
