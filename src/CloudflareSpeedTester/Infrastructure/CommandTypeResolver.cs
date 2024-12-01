// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandTypeResolver.cs" company="MareMare">
// Copyright © 2024 MareMare.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Spectre.Console.Cli;

namespace CloudflareSpeedTester.Infrastructure;

/// <summary>
/// Provides a mechanism to resolve command types using a service provider.
/// Implements <see cref="ITypeResolver"/> and <see cref="IDisposable"/> interfaces.
/// </summary>
/// <param name="provider"><see cref="IServiceProvider" />.</param>
internal sealed class CommandTypeResolver(IServiceProvider provider) : ITypeResolver, IDisposable
{
    /// <summary><see cref="IServiceProvider" />。</summary>
    private readonly IServiceProvider _provider = provider ?? throw new ArgumentNullException(nameof(provider));

    /// <inheritdoc />
    public object? Resolve(Type? type) =>
        type is not null ? this._provider.GetService(type) : null;

    /// <inheritdoc />
    public void Dispose()
    {
        if (this._provider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
