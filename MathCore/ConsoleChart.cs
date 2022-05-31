﻿#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace MathCore;

public sealed class ConsoleChart
{
    private const int __ValuesYOffset0 = 3;

    private readonly int _Width;
    private readonly int _Height;
    private readonly TextWriter _Writer;

    public string? YValuesFormat { get; set; } = "f2";

    public IFormatProvider? YFormatProvider { get; set; }

    public string YAxeName { get; set; } = "y";

    public string XAxeName { get; set; } = "x";

    public char PointSymbol { get; set; } = '*';

    public bool PlotAxeX { get; set; } = true;
    public bool PlotAxeY { get; set; } = true;

    public char AxeXChar { get; set; } = '─';
    public char AxeYChar { get; set; } = '│';

    public char AxeCrossChar { get; set; } = '┼';

    public char AxeXArrowChar { get; set; } = '>'; //'→'
    public char AxeYArrowChar { get; set; } = '↑';

    public ConsoleChart(int Width, int Height, TextWriter Writer)
    {
        _Width = Width;
        _Height = Height;
        _Writer = Writer;
    }

    public void Plot(IReadOnlyList<double> XX, IReadOnlyList<double> YY)
    {
        if (XX is null) throw new ArgumentNullException(nameof(XX));
        if (YY is null) throw new ArgumentNullException(nameof(YY));

        if (XX.Count != YY.Count) throw new InvalidOperationException("Размеры коллекций не совпадают");
        if (XX.Count == 0) throw new InvalidOperationException("Число точек не должно быть равно 0");

        var x_interval = XX.GetMinMax();
        var y_interval = YY.GetMinMax();

        var x_min = x_interval.Min;
        var y_max = y_interval.Max;

        var axe_x_char = AxeXChar;
        var axe_y_char = AxeYChar;
        var axe_cross_char = AxeCrossChar;

        var values_rows_count = _Height - __ValuesYOffset0;
        var dy = y_interval.Length / (values_rows_count - 1);

        var row_str = new string[values_rows_count];
        var f_values_format = YValuesFormat;
        var formatter = YFormatProvider ?? CultureInfo.InvariantCulture;
        for (var i = 0; i < values_rows_count; i++)
        {
            var y0 = y_max - (i + 0.5) * dy;
            row_str[i] = f_values_format is null
                ? y0.ToString(formatter)
                : y0.ToString(f_values_format, formatter);
        }

        var y_max_label = f_values_format is null
            ? y_interval.Max.ToString(formatter)
            : y_interval.Max.ToString(f_values_format, formatter);

        var y_min_label = f_values_format is null
            ? y_interval.Min.ToString(formatter)
            : y_interval.Min.ToString(f_values_format, formatter);

        var y_axe_label_width = Math.Max(row_str.Max(l => l.Length), Math.Max(y_max_label.Length, y_min_label.Length));

        for (var i = 0; i < values_rows_count; i++)
            if (row_str[i].Length < y_axe_label_width)
                row_str[i] = row_str[i].PadLeft(y_axe_label_width);

        if (y_max_label.Length < y_axe_label_width)
            y_max_label = y_max_label.PadLeft(y_axe_label_width);

        if (y_min_label.Length < y_axe_label_width)
            y_min_label = y_min_label.PadLeft(y_axe_label_width);

        var writer = _Writer;

        writer.Write(y_max_label);
        writer.Write(AxeYArrowChar);
        writer.WriteLine(YAxeName);

        var x_axe_values_width_offset = y_max_label.Length + 2 + XAxeName.Length;
        var values_cols_count = _Width - x_axe_values_width_offset;
        var dx = x_interval / (values_cols_count - 1);

        var buffer_lines = new char[values_rows_count][];
        var x_axes_not_plotet = PlotAxeX;
        var axe_x_index = -1;
        var plot_y_axe = PlotAxeY;
        var axe_y_index = -1;
        for (var i = 0; i < values_rows_count; i++)
        {
            var buffer_line = new char[values_cols_count];
            buffer_lines[i] = buffer_line;

            var y0 = y_max - (i + 0.5) * dy;

            if (x_axes_not_plotet && Math.Abs(y0) < dy)
            {
                x_axes_not_plotet = false;
                axe_x_index = i;
                var axe_char = AxeXChar;
                var axe_x_not_plotted = plot_y_axe;
                for (var j = 0; j < values_cols_count; j++)
                {
                    var x0 = x_min + (j + 0.5) * dx;
                    if (axe_x_not_plotted && Math.Abs(x0) < dx)
                    {
                        buffer_line[j] = axe_cross_char;
                        axe_x_not_plotted = false;
                        if (axe_y_index < 0) axe_y_index = j;
                    }
                    else
                        buffer_line[j] = axe_char;
                }
            }
            else
            {
                var axe_x_not_plotted = plot_y_axe;
                for (var j = 0; j < values_cols_count; j++)
                {
                    var x0 = x_min + (j + 0.5) * dx;
                    if (axe_x_not_plotted && Math.Abs(x0) < dx)
                    {
                        buffer_line[j] = axe_y_char;
                        axe_x_not_plotted = false;
                        if (axe_y_index < 0) axe_y_index = j;
                    }
                    else
                        buffer_line[j] = ' ';
                }
            }
        }

        var points = XX.Zip(YY, (x, y) => (x, y));
        var symbol = PointSymbol;
        foreach (var (x, y) in points)
        {
            var row = (int)Math.Round((y_max - y) / dy);
            var col = (int)Math.Round((x - x_min) / dx);

            buffer_lines[row][col] = symbol;
        }

        for (var i = 0; i < values_rows_count; i++)
        {
            if (i == axe_x_index)
                writer.WriteLine($"{row_str[i]}{AxeCrossChar}{new string(buffer_lines[i]).TrimEnd(' ')}");
            else
                writer.WriteLine($"{row_str[i]}{axe_y_char}{new string(buffer_lines[i]).TrimEnd(' ')}");
        }

        writer.Write(y_min_label);
        writer.Write(axe_cross_char);
        var down_axe = new char[values_cols_count];
        for (var i = 0; i < values_cols_count; i++)
            down_axe[i] = i == axe_y_index ? axe_cross_char : axe_x_char;
        writer.Write(new string(down_axe));
        writer.Write(AxeXArrowChar);
        //writer.Write('→');
        writer.WriteLine(XAxeName);

        var x_max_label = f_values_format is null
            ? x_interval.Max.ToString(formatter)
            : x_interval.Max.ToString(f_values_format, formatter);

        var x_min_label = f_values_format is null
            ? x_interval.Min.ToString(formatter)
            : x_interval.Min.ToString(f_values_format, formatter);

        var x_000_label = f_values_format is null
            ? x_interval.Middle.ToString(formatter)
            : x_interval.Middle.ToString(f_values_format, formatter);

        writer.Write(new string(' ', y_axe_label_width));
        writer.Write(axe_y_char);
        writer.Write(x_min_label);
        writer.Write(new string(' ', values_cols_count / 2 - x_min_label.Length - x_000_label.Length / 2));
        writer.Write(x_000_label);
        writer.Write(new string(' ', values_cols_count / 2 - x_000_label.Length / 2 - x_max_label.Length));
        writer.WriteLine(x_max_label);
    }
}
