// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CalculationExtensions.cs" company="MareMare">
// Copyright © 2024 MareMare.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;
using System.Numerics;

namespace CloudflareSpeedTester.Extensions;

/// <summary>
/// 計算に関する拡張メソッドを提供するクラスです。
/// </summary>
internal static class CalculationExtensions
{
    /// <summary>
    /// レイテンシのコレクションからジッターを計算します。
    /// </summary>
    /// <param name="latencies">レイテンシのコレクション。</param>
    /// <returns>ジッターの平均値。要素が2つ未満の場合は <c>null</c> を返します。</returns>
    public static double? ToJitter(this IReadOnlyCollection<double> latencies)
    {
        if (latencies.Count < 2)
        {
            return null;
        }

        var aveJitter = latencies
            .Zip(
                latencies.Skip(1),
                (a, b) =>
                    Math.Abs(a - b))
            .Average();
        return aveJitter;
    }

    /// <summary>
    /// 値のコレクションから中央値を計算します。
    /// </summary>
    /// <typeparam name="T">数値型。</typeparam>
    /// <param name="values">値のコレクション。</param>
    /// <returns>中央値。要素がない場合は <c>null</c> を返します。</returns>
    public static T? Median<T>(this IReadOnlyCollection<T> values)
        where T : struct, INumber<T>
    {
        if (values.Count == 0)
        {
            return null;
        }

        var sortedValues = values.OrderBy(v => v).ToArray();
        var count = sortedValues.Length;
        if (count % 2 == 0)
        {
            // 偶数個の要素の場合、中央の2つの要素の平均を取る
            var midValue1 = sortedValues[(count / 2) - 1];
            var midValue2 = sortedValues[count / 2];
            var total = midValue1 + midValue2;
            return total / T.CreateChecked(2);
        }
        else
        {
            // 奇数個の要素の場合、中央の要素を取る
            var midValue = sortedValues[count / 2];
            return midValue;
        }
    }

    /// <summary>
    /// 値のコレクションから指定されたパーセンタイルの値を計算します。
    /// </summary>
    /// <typeparam name="T">数値型。</typeparam>
    /// <param name="values">値のコレクション。</param>
    /// <param name="percentile">パーセンタイル（0から1の間）。</param>
    /// <returns>指定されたパーセンタイルの値。</returns>
    public static T Percentile<T>(this IEnumerable<T> values, double percentile = 0.9)
        where T : struct, INumber<T>
    {
        var sequence = values.ToArray();
        if (sequence.Length == 0)
        {
            throw new ArgumentException("The sequence cannot be null or empty.");
        }

        if (percentile is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(percentile), "Percentile must be between 0 and 1.");
        }

        Array.Sort(sequence);
        var position = percentile * (sequence.Length - 1);
        var lowerIndex = (int)Math.Floor(position);
        var upperIndex = (int)Math.Ceiling(position);
        if (lowerIndex == upperIndex)
        {
            return sequence[lowerIndex];
        }

        var lowerValue = sequence[lowerIndex];
        var upperValue = sequence[upperIndex];
        var weight = T.CreateChecked(position - lowerIndex);
        return lowerValue + (weight * (upperValue - lowerValue));
    }

    /// <summary>
    /// ビット毎秒（bps）を人間が読みやすい形式に変換します。
    /// </summary>
    /// <param name="self">変換対象の値。</param>
    /// <param name="digits">小数点以下の桁数。</param>
    /// <returns>人間が読みやすい形式の文字列。</returns>
    public static string ToPrettyBps(this double? self, int digits = 2)
    {
        string[] units = ["bps", "kbps", "Mbps", "Gbps"];
        return ToPretty(units, self, digits);
    }

    /// <summary>
    /// ミリ秒を人間が読みやすい形式に変換します。
    /// </summary>
    /// <param name="self">変換対象の値。</param>
    /// <param name="digits">小数点以下の桁数。</param>
    /// <returns>人間が読みやすい形式の文字列。</returns>
    public static string ToPrettyMilliseconds(this double? self, int digits = 2)
    {
        string[] units = ["ms", "s"];
        return ToPretty(units, self, digits);
    }

    /// <summary>
    /// 指定された単位で値を人間が読みやすい形式に変換します。
    /// </summary>
    /// <param name="units">単位の配列。</param>
    /// <param name="target">変換対象の値。</param>
    /// <param name="digits">小数点以下の桁数。</param>
    /// <returns>人間が読みやすい形式の文字列。</returns>
    private static string ToPretty(string[] units, double? target, int digits = 2)
    {
        var targetValue = target ?? 0;
        if (targetValue == 0)
        {
            return ToPrettyText(0, digits, units[0]);
        }

        // mag is 0 for bps, 1 for kbps, 2, for Mbps, etc.
        var mag = (int)Math.Floor(Math.Log(targetValue, 1000));
        if (mag >= units.Length)
        {
            mag = units.Length - 1;
        }

        // Adjusted value using Math.Pow instead of bit shift
        var adjustedValue = (decimal)targetValue / (decimal)Math.Pow(1000, mag);

        return ToPrettyText(adjustedValue, digits, units[mag]);

        static string ToPrettyText(decimal valueToPretty, int decimalPlaces, string unitText)
        {
            return $"{valueToPretty.ToString($"n{decimalPlaces}", CultureInfo.InvariantCulture)} {unitText}";
        }
    }
}
