using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace NP.Avalonia.Visuals.Behaviors
{
    public static class ProcessControllerBehavior
    {
        #region ProcessExePath Attached Avalonia Property
        public static string GetProcessExePath(AvaloniaObject obj)
        {
            return obj.GetValue(ProcessExePathProperty);
        }

        public static void SetProcessExePath(AvaloniaObject obj, string value)
        {
            obj.SetValue(ProcessExePathProperty, value);
        }

        public static readonly AttachedProperty<string> ProcessExePathProperty =
            AvaloniaProperty.RegisterAttached<Control, AvaloniaObject, string>
            (
                "ProcessExePath"
            );
        #endregion ProcessExePath Attached Avalonia Property


        #region ProcInitInfo Attached Avalonia Property
        public static ProcessInitInfo GetProcInitInfo(AvaloniaObject obj)
        {
            return obj.GetValue(ProcInitInfoProperty);
        }

        public static void SetProcInitInfo(AvaloniaObject obj, ProcessInitInfo value)
        {
            obj.SetValue(ProcInitInfoProperty, value);
        }

        public static readonly AttachedProperty<ProcessInitInfo> ProcInitInfoProperty =
            AvaloniaProperty.RegisterAttached<Control, AvaloniaObject, ProcessInitInfo>
            (
                "ProcInitInfo"
            );
        #endregion ProcInitInfo Attached Avalonia Property


        #region MultiPlatformProcInitInfo Attached Avalonia Property
        public static MultiPlatformProcessInitInfo GetMultiPlatformProcInitInfo(AvaloniaObject obj)
        {
            return obj.GetValue(MultiPlatformProcInitInfoProperty);
        }

        public static void SetMultiPlatformProcInitInfo(AvaloniaObject obj, MultiPlatformProcessInitInfo value)
        {
            obj.SetValue(MultiPlatformProcInitInfoProperty, value);
        }

        public static readonly AttachedProperty<MultiPlatformProcessInitInfo> MultiPlatformProcInitInfoProperty =
            AvaloniaProperty.RegisterAttached<Control, AvaloniaObject, MultiPlatformProcessInitInfo>
            (
                "MultiPlatformProcInitInfo"
            );
        #endregion MultiPlatformProcInitInfo Attached Avalonia Property



        #region TheProcess Attached Avalonia Property
        public static Process? GetTheProcess(AvaloniaObject obj)
        {
            return obj.GetValue(TheProcessProperty);
        }

        private static void SetTheProcess(AvaloniaObject obj, Process? value)
        {
            obj.SetValue(TheProcessProperty, value);
        }

        public static readonly AttachedProperty<Process?> TheProcessProperty =
            AvaloniaProperty.RegisterAttached<Control, AvaloniaObject, Process?>
            (
                "TheProcess"
            );
        #endregion TheProcess Attached Avalonia Property


        #region MainWindowHandle Attached Avalonia Property
        public static IntPtr GetMainWindowHandle(AvaloniaObject obj)
        {
            return obj.GetValue(MainWindowHandleProperty);
        }

        public static void SetMainWindowHandle(AvaloniaObject obj, IntPtr value)
        {
            obj.SetValue(MainWindowHandleProperty, value);
        }

        public static readonly AttachedProperty<IntPtr> MainWindowHandleProperty =
            AvaloniaProperty.RegisterAttached<AvaloniaObject, AvaloniaObject, IntPtr>
            (
                "MainWindowHandle",
                IntPtr.Zero
            );
        #endregion MainWindowHandle Attached Avalonia Property

        static ProcessControllerBehavior()
        {
            ProcessExePathProperty.Changed.Subscribe(OnStartProcesPathPropertyChanged);

            ProcInitInfoProperty.Changed.Subscribe(OnProcInitInfoPropChanged);

            MultiPlatformProcInitInfoProperty.Changed.Subscribe(OnMultiPlatformProcInitInfoChanged);

            TheProcessProperty.Changed.Subscribe(OnProcessChanged);
        }

        private static void OnMultiPlatformProcInitInfoChanged(this AvaloniaPropertyChangedEventArgs<MultiPlatformProcessInitInfo> changeArgs)
        {
            var sender = (AvaloniaObject)changeArgs.Sender;

            var mpProcInitInfo = changeArgs.NewValue.Value;

            sender.OnMultiPlatformProcInitInfoChangedWithExtraParams(mpProcInitInfo);
        }

        public static void OnMultiPlatformProcInitInfoChangedWithExtraParams
        (
            this AvaloniaObject sender, 
            MultiPlatformProcessInitInfo? mpProcInitInfo,
            params string[] extraParams
        )
        {
            ProcessInitInfo? procInitInfo = null;

            if (OperatingSystem.IsWindows())
            {
                procInitInfo = mpProcInitInfo.WindowsProcInitInfo;
            }
            else if (OperatingSystem.IsLinux())
            {
                procInitInfo = mpProcInitInfo.LinuxProcInitInfo;
            }
            else if (OperatingSystem.IsMacOS())
            {
                procInitInfo = mpProcInitInfo.MacProcInitInfo;
            }


            if (extraParams != null && extraParams.Length > 0)
            {
                procInitInfo.Args.InsertRange(procInitInfo.InsertIdx.Value, extraParams);
            }

            sender.SetProcFromIniInfo(procInitInfo);
        }

        private static void OnProcInitInfoPropChanged(AvaloniaPropertyChangedEventArgs<ProcessInitInfo> changeArgs)
        {
            var sender = (AvaloniaObject)changeArgs.Sender;

            var procInitInfo = changeArgs.NewValue.Value;

            sender.SetProcFromIniInfo(procInitInfo);
        }

        private static void SetProcFromIniInfo(this AvaloniaObject sender, ProcessInitInfo? procInitInfo)
        {
            if (procInitInfo?.ExePath == null)
            {
                SetTheProcess(sender, null);
                return;
            }

            ProcessStartInfo processStartInfo = new ProcessStartInfo(procInitInfo.ExePath, string.Join(' ', procInitInfo.Args));
            
            if (procInitInfo.WorkingDir != null)
            {
                processStartInfo.WorkingDirectory = procInitInfo.WorkingDir;
            }

            Process p = Process.Start(processStartInfo)!;

            SetTheProcess(sender, p);
        }

        private static void OnProcessChanged(AvaloniaPropertyChangedEventArgs<Process?> obj)
        {
            Process? oldProcess = obj.OldValue.Value;

            oldProcess?.DestroyProcess();

            Control sender = (Control) obj.Sender;

            Process? p = obj.NewValue.Value;

            //await Task.Delay(3000);

            if (p != null)
            {

                void OnProcessExited(object? procObj, EventArgs e)
                {
                    Process proc = (Process) procObj!;

                    proc.Exited -= OnProcessExited;

                    if (GetTheProcess(sender) != null)
                    {
                        SetTheProcess(sender, null);
                    }
                }

                p.Exited += OnProcessExited;
            }
        }

        private static void OnStartProcesPathPropertyChanged(AvaloniaPropertyChangedEventArgs<string> changeArgs)
        {
            var sender = (AvaloniaObject) changeArgs.Sender;

            string exePath = changeArgs.NewValue.Value;

            ProcessInitInfo processInitInfo = new ProcessInitInfo { ExePath = exePath };

            sender.SetProcFromIniInfo(processInitInfo);
        }

        public static void DestroyProcess(this Process? p)
        {
            if (p == null)
                return;

            p.Kill();

            p.Dispose();
        }

        public static void DestroyProcess(this AvaloniaObject avaloniaObject)
        {
            Process? p = GetTheProcess(avaloniaObject);

            if (p != null)
            {
                p.DestroyProcess();
            }
        }
    }

    public class ProcessInitInfo
    {
        // if there is an extra property or properties
        // e.g. WindowHostId, InsertIdx specifies where
        // to insert it/them. This is an optional property
        public int? InsertIdx { get; set; }

        public string? ExePath { get; set; }

        // directory to start the executable in
        public string? WorkingDir { get; set; }

        public List<string> Args { get; } = new List<string>();
    }

    public class MultiPlatformProcessInitInfo
    {
        public ProcessInitInfo? DefaultInitInfo { get; set; }

        private ProcessInitInfo? _windowsProcInitInfo;
        public ProcessInitInfo? WindowsProcInitInfo
        {
            get => _windowsProcInitInfo ?? DefaultInitInfo;
            set => _windowsProcInitInfo = value;
        }

        private ProcessInitInfo? _linuxProcInitInfo;
        public ProcessInitInfo? LinuxProcInitInfo 
        {
            get => _linuxProcInitInfo ?? DefaultInitInfo;
            set => _linuxProcInitInfo = value;
        }

        private ProcessInitInfo? _macProcInitInfo;
        public ProcessInitInfo? MacProcInitInfo 
        {
            get => _macProcInitInfo ?? DefaultInitInfo;
            set => _macProcInitInfo = value;
        }
    }
}
