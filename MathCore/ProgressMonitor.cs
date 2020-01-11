using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MathCore.Annotations;
// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore
{
    public class ProgressMonitor : INotifyPropertyChanged
    {
        /* ------------------------------------------------------------------------------------------ */

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] [CanBeNull] string PropertyName = null) => OnPropertyChanged(new PropertyChangedEventArgs(PropertyName));

        public event EventHandler StatusChanged;
        public event EventHandler StatusCheckerChanged;
        public event EventHandler InformationChanged;
        public event EventHandler InformationCheckerChanged;
        public event EventHandler ProgressChanged;
        public event EventHandler ProgressCheckerChanged;

        /* ------------------------------------------------------------------------------------------ */

        private double _Progress;
        private string _Information;
        private string _Status;

        private Func<string> _StatusStrFunc;
        private Func<string> _InformationStrFunc;
        private Func<double> _ProgressFunc;

        private ProgressMonitor _ConnectedMonitor;

        /* ------------------------------------------------------------------------------------------ */

        public Func<string> StatusChecker
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => _StatusStrFunc;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if(value == _StatusStrFunc) return;
                _StatusStrFunc = value;
                StatusCheckerChanged.FastStart(this);
                OnPropertyChanged();
            }
        }

        public Func<string> InformationChecker
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => _InformationStrFunc;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if(value == _InformationStrFunc) return;
                _InformationStrFunc = value;
                InformationCheckerChanged.FastStart(this);
                OnPropertyChanged();
            }
        }

        public Func<double> ProgressChecker
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => _ProgressFunc;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if(value == _ProgressFunc) return;
                _ProgressFunc = value;
                ProgressCheckerChanged.FastStart(this);
                OnPropertyChanged();
            }
        }

        /* ------------------------------------------------------------------------------------------ */

        public string Status
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                var status_f = _StatusStrFunc;
                return status_f is null ? _Status : Status = status_f();
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if(_Status == value) return;
                _Status = value;
                StatusChanged.FastStart(this);
                OnPropertyChanged();
            }
        }

        public string Information
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                var information_f = _InformationStrFunc;
                return information_f is null ? _Information : Information = information_f();
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if(_Information == value) return;
                _Information = value;
                InformationChanged.FastStart(this);
                OnPropertyChanged();
            }
        }

        public double Progress
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                var progress_f = _ProgressFunc;
                return progress_f is null ? _Progress : Progress = progress_f();
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if(Math.Abs(_Progress - value) < double.Epsilon) return;
                _Progress = value;
                ProgressChanged.FastStart(this);
                OnPropertyChanged();
            }
        }

        [NotNull]
        public string ProgressStr
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => $"{(Progress * 100).Round(2)}%";
        }

        public ProgressMonitor ConnectedMonitor
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => _ConnectedMonitor;
        }

        /* ------------------------------------------------------------------------------------------ */

        public ProgressMonitor(
            [CanBeNull] Func<double> ProgressFunc = null,
            [CanBeNull] Func<string> StatusFunc = null,
            [CanBeNull] Func<string> InformationFunc = null)
        {
            _StatusStrFunc = StatusFunc;
            _InformationStrFunc = InformationFunc;
            _ProgressFunc = ProgressFunc;
        }

        public ProgressMonitor(string Status, string Information = "", double Progress = 0)
        {
            _Status = Status;
            _Information = Information;
            _Progress = Progress;
        }

        /* ------------------------------------------------------------------------------------------ */

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Connect(ProgressMonitor Monitor)
        {
            if(ReferenceEquals(Monitor, this))
                throw new ArgumentException("Нельзя подключать монитор к себе по методам изъятия значений");

            if(_ConnectedMonitor != null) ClearEventHandlers();
            _ConnectedMonitor = Monitor;
            StatusChecker = () => Monitor.Status;
            ProgressChecker = () => Monitor.Progress;
            InformationChecker = () => Monitor.Information;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ConnectStrong([NotNull] ProgressMonitor Monitor)
        {
            if(ReferenceEquals(Monitor, this))
                throw new ArgumentException("Нельзя подключать монитор к себе по обработчикам событий");

            Disconnect();
            _ConnectedMonitor = Monitor;
            Monitor.StatusChanged += OnMonitorStatusChanged;
            Monitor.InformationChanged += OnMonitorInformationChanged;
            Monitor.ProgressChanged += OnMonitorProgressChanged;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Disconnect()
        {
            StatusChecker = null;
            ProgressChecker = null;
            InformationChecker = null;
            _ConnectedMonitor = null;
        }

        public void SetStatus(string status, double progress, string information)
        {
            Status = status;
            Progress = progress;
            Information = information;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ClearEventHandlers()
        {
            if(_ConnectedMonitor is null) return;

            _ConnectedMonitor.StatusChanged -= OnMonitorStatusChanged;
            _ConnectedMonitor.InformationChanged -= OnMonitorInformationChanged;
            _ConnectedMonitor.ProgressChanged -= OnMonitorProgressChanged;
            _ConnectedMonitor = null;
        }

        private void OnMonitorStatusChanged(object sender, EventArgs e)
        {
            var monitor = (ProgressMonitor)sender;
            Status = monitor.Status;
        }

        private void OnMonitorInformationChanged(object sender, EventArgs e) => Information = ((ProgressMonitor)sender).Information;

        private void OnMonitorProgressChanged(object sender, EventArgs e) => Progress = ((ProgressMonitor)sender).Progress;

        /* ------------------------------------------------------------------------------------------ */

        public override string ToString() => $"{Status}:({ProgressStr}) - {Information}";

        /* ------------------------------------------------------------------------------------------ */
    }
}