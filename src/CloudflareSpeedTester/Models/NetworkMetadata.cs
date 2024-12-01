// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetworkMetadata.cs" company="MareMare">
// Copyright © 2024 MareMare.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CloudflareSpeedTester.Models;

/// <summary>
/// ネットワークメタデータを表すレコードです。
/// </summary>
/// <param name="Asn">自律システム番号を表します。</param>
/// <param name="City">都市名を表します。</param>
/// <param name="Colo">コロケーションを表します。</param>
/// <param name="Country">国名を表します。</param>
/// <param name="Ip">IPアドレスを表します。</param>
public sealed record NetworkMetadata(
    string Asn,
    string City,
    string Colo,
    string Country,
    string Ip)
{
    /// <inheritdoc />
    public override string ToString() =>
        $"{nameof(this.City)}: {this.City} {nameof(this.City)}: {this.Country} {nameof(this.Ip)}: {this.Ip} {nameof(this.Asn)}: {this.Asn} {nameof(this.Colo)}: {this.Colo}";
}
