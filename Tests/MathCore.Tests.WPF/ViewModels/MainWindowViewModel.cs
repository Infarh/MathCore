using System.IO;
using System.Windows.Input;
using System.Windows.Markup;

using MathCore.Interpolation;
using MathCore.WPF;
using MathCore.WPF.Commands;
using MathCore.WPF.ViewModels;

using OxyPlot;

namespace MathCore.Tests.WPF.ViewModels;

[MarkupExtensionReturnType(typeof(MainWindowViewModel))]
public class MainWindowViewModel() : TitledViewModel("SAR")
{
    private InterpolatorNDLinear? _Interpolator = null;

    #region Command - LoadDataCommand

    private ICommand? _LoadDataCommand;

    public ICommand LoadDataCommand => _LoadDataCommand ??=
        Command.New(
            OnLoadDataCommandExecuted,
            CanLoadDataCommandExecute);

    private bool CanLoadDataCommandExecute(object? p) => true;

    private void OnLoadDataCommandExecuted(object? p)
    {
        FileInfo? file = null;
        switch (p)
        {
            default:

                var dialog = FileDialogEx
                    .OpenFile("Файл данных интерполятора")
                    .AddFilter("CSV", "*.csv", "*.zip")
                    .AddFilterAllFiles();

                file = dialog.GetFileInfo();

               

                break;

            case FileInfo { Exists: true } f:
                file = f;
                break;

            case string path when File.Exists(path):
                file = new(path);
                break;
        }

        if (file is not { Exists: true })
            return;

        var interpolator = InterpolatorNDLinear.LoadCSV(file);

        _Interpolator = interpolator;

        OnPropertyChanged(nameof(ValueSAR));
        OnGraphChanged();
    }

    #endregion

    #region Command - CalculateValueCommand

    private ICommand? _CalculateValueCommand;

    public ICommand CalculateValueCommand => _CalculateValueCommand ??=
        Command.New(
            OnCalculateValueCommandExecuted,
            CanCalculateValueCommandExecute);

    private bool CanCalculateValueCommandExecute() => _Interpolator is not null;

    private void OnCalculateValueCommandExecuted() => OnPropertyChanged(nameof(ValueSAR));
    #endregion

    #region property ValueT : double - dT

    /// <Summary>dT</Summary>
    private double _ValueT = 0;

    /// <Summary>dT</Summary>
    public double ValueT { get => _ValueT; set => SetValue(ref _ValueT, value, v => v is >= 0 and <= 20).Then(OnGraphChanged); }

    #endregion

    #region property ValueTGraph : bool - График по dT

    /// <Summary>График по dT</Summary>
    private bool _ValueTGraph;

    /// <Summary>График по dT</Summary>
    public bool ValueTGraph
    {
        get => _ValueTGraph;
        set
        {
            if (!Set(ref _ValueTGraph, value)) return;
            if (!value) return;
            //ValueTGraph = false;
            ValueHGraph = false;
            ValueGGraph = false;
            ValueMGraph = false;
            OnGraphChanged();
        }
    }

    #endregion

    #region property ValueH : double - H

    /// <Summary>H</Summary>
    private double _ValueH = 10668;

    /// <Summary>H</Summary>
    public double ValueH { get => _ValueH; set => SetValue(ref _ValueH, value).Then(OnGraphChanged); }

    #endregion

    #region property ValueHGraph : bool - График по H

    /// <Summary>График по H</Summary>
    private bool _ValueHGraph;

    /// <Summary>График по H</Summary>
    public bool ValueHGraph
    {
        get => _ValueHGraph;
        set
        {
            if (!Set(ref _ValueHGraph, value)) return;
            if (!value) return;
            ValueTGraph = false;
            //ValueHGraph = false;
            ValueGGraph = false;
            ValueMGraph = false;
            OnGraphChanged();
        }
    }

    #endregion

    #region property ValueG : double - G

    /// <Summary>G</Summary>
    private double _ValueG = 28000;

    /// <Summary>G</Summary>
    public double ValueG { get => _ValueG; set => SetValue(ref _ValueG, value).Then(OnGraphChanged); }

    #endregion

    #region property ValueGGraph : bool - График по G

    /// <Summary>График по G</Summary>
    private bool _ValueGGraph;

    /// <Summary>График по G</Summary>
    public bool ValueGGraph
    {
        get => _ValueGGraph;
        set
        {
            if (!Set(ref _ValueGGraph, value)) return;
            if (!value) return;
            ValueTGraph = false;
            ValueHGraph = false;
            //ValueGGraph = false;
            ValueMGraph = false;
            OnGraphChanged();
        }
    }

    #endregion

    #region property ValueM : double - M

    /// <Summary>M</Summary>
    private double _ValueM = 0.45;

    /// <Summary>M</Summary>
    public double ValueM { get => _ValueM; set => SetValue(ref _ValueM, value).Then(OnGraphChanged); }

    #endregion

    #region property ValueMGraph : bool - График по M

    /// <Summary>График по M</Summary>
    private bool _ValueMGraph = true;

    /// <Summary>График по M</Summary>
    public bool ValueMGraph
    {
        get => _ValueMGraph;
        set
        {
            if (!Set(ref _ValueMGraph, value)) return;
            if (!value) return;
            ValueTGraph = false;
            ValueHGraph = false;
            ValueGGraph = false;
            //ValueMGraph = false;
            OnGraphChanged();
        }
    }

    #endregion

    #region property ValueSAR : double - SAR

    /// <Summary>SAR</Summary>
    [DependencyOn(nameof(ValueT))]
    [DependencyOn(nameof(ValueH))]
    [DependencyOn(nameof(ValueG))]
    [DependencyOn(nameof(ValueM))]
    public double ValueSAR
    {
        get
        {
            if (_Interpolator is null)
                return double.NaN;

            var value = _Interpolator[_ValueT, _ValueH, _ValueG, _ValueM];

            return value;
        }
    }

    #endregion

    #region property GraphSAR : IEnumerable<PointF> - График SAR

    private volatile int _OnGraphChangedSyncIndex;
    private volatile int _Timeout = 10;
    private async void OnGraphChanged()
    {
        if (_Interpolator is not { } interpolator) return;
        if (!_ValueTGraph && !_ValueGGraph && !_ValueHGraph && !_ValueMGraph)
        {
            ArgumentName = "-";
            GraphSAR = null;
            return;
        }

        var time = Environment.TickCount;
        _OnGraphChangedSyncIndex = time;

        await Task.Delay(_Timeout).ConfigureAwait(false);

        if (_OnGraphChangedSyncIndex != time) return;

        var start_time_ticks = Environment.TickCount64;
        try
        {
            var points = new List<DataPoint>();
            GraphSAR = null;
            if (_ValueTGraph)
            {
                for (var t = 0.0; t <= 20; t += 0.05)
                    points.Add(new(t, interpolator[t, _ValueH, _ValueG, _ValueM]));
                ArgumentName = "dT";
                GraphSAR = points;
                return;
            }

            if (_ValueHGraph)
            {
                for (var h = 10668.0; h <= 12192; h += 15.0)
                    points.Add(new(h, interpolator[_ValueT, h, _ValueG, _ValueM]));
                ArgumentName = "H";
                GraphSAR = points;
                return;
            }

            if (_ValueGGraph)
            {
                for (var g = 28000.0; g <= 52000; g += 10.0)
                    points.Add(new(g, interpolator[_ValueT, _ValueH, g, _ValueM]));
                ArgumentName = "G";
                GraphSAR = points;
                return;
            }

            if (_ValueMGraph)
            {
                for (var m = 0.45; m <= 0.82; m += 0.005)
                    points.Add(new(m, interpolator[_ValueT, _ValueH, _ValueG, m]));
                ArgumentName = "Mach";
                GraphSAR = points;
                return;
            }
        }
        finally
        {
            var end_time_ticks = Environment.TickCount64;
            var delta_ms = TimeSpan
                .FromTicks(end_time_ticks - start_time_ticks)
                .TotalMilliseconds;

            var new_timeout = _Timeout + (delta_ms - _Timeout) / 10;
            _Timeout = Math.Max(0, (int)new_timeout);
        }
    }

    /// <Summary>График SAR</Summary>
    private IEnumerable<DataPoint>? _GraphSAR;

    /// <Summary>График SAR</Summary>
    public IEnumerable<DataPoint>? GraphSAR
    {
        get => _GraphSAR;
        private set => Set(ref _GraphSAR, value);
    }

    #endregion


    #region property ArgumentName : string - название аргумента графика

    /// <Summary>название аргумента графика</Summary>
    private string _ArgumentName = "-";

    /// <Summary>название аргумента графика</Summary>
    public string ArgumentName { get => _ArgumentName; private set => Set(ref _ArgumentName!, value); }

    #endregion


}
