using System.Windows.Input;
using System.Windows.Markup;

using MathCore.Interpolation;
using MathCore.WPF;
using MathCore.WPF.Commands;
using MathCore.WPF.ViewModels;

using Microsoft.Win32;

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

    private bool CanLoadDataCommandExecute() => true;

    private void OnLoadDataCommandExecuted()
    {
        var dialog = FileDialogEx
            .OpenFile("Файл данных интерполятора")
            .AddFilter("CSV", "*.csv", "*.zip")
            .AddFilterAllFiles();

        if (dialog.GetFileInfo() is not { Exists: true } file)
            return;

        var interpolator = InterpolatorNDLinear.LoadCSV(file);

        _Interpolator = interpolator;

        OnPropertyChanged(nameof(ValueSAR));
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
    public double ValueT { get => _ValueT; set => Set(ref _ValueT, value); }

    #endregion

    #region property ValueH : double - H

    /// <Summary>H</Summary>
    private double _ValueH = 10668;

    /// <Summary>H</Summary>
    public double ValueH { get => _ValueH; set => Set(ref _ValueH, value); }

    #endregion

    #region property ValueG : double - G

    /// <Summary>G</Summary>
    private double _ValueG = 28000;

    /// <Summary>G</Summary>
    public double ValueG { get => _ValueG; set => Set(ref _ValueG, value); }

    #endregion

    #region property ValueM : double - M

    /// <Summary>M</Summary>
    private double _ValueM = 0.45;

    /// <Summary>M</Summary>
    public double ValueM { get => _ValueM; set => Set(ref _ValueM, value); }

    #endregion

    #region property ValueSAR : double - SAR

    /// <Summary>SAR</Summary>
    private double _ValueSAR;

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
}
