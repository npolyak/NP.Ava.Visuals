using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using NP.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace NP.Ava.Visuals.Behaviors.DragDrop;

public class DragManager : VMBase
{
    Control? _dragControl = null;   
    public Control? DragControl
    {
        get => _dragControl;
        internal set
        {
            if (_dragControl == value)
            {
                return;   
            }
            
            if (_dragControl != null)
            {
                _dragControl.PointerPressed -= OnPointerPressed;
            }   

            _dragControl = value;

            if (_dragControl != null)
            {
                _dragControl.PointerPressed += OnPointerPressed;
            }
        }
    }

    private Rect _dragBoundWithinDropControl = new Rect();
    public Rect DragBoundsWithinDropControl
    {
        get => _dragBoundWithinDropControl;
        init
        {
            _dragBoundWithinDropControl = value;
        }
    }

    public DragManager()
    {
        
    }


    private static IDisposable? _dragSubscription = null;

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        DragControlStartPositionInScreen = DragControl!.OriginToScreen();

        CurrentScreenPointBehavior.Capture(DragControl!, e);

        DragPointerStartPositionInScreen = CurrentScreenPointBehavior.CurrentScreenPointValue;

        _dragSubscription =
            CurrentScreenPointBehavior.CurrentScreenPoint
                                        .Subscribe(OnDragMove);

    }

    Control? _currentDropControl;
    public Control? CurrentDropControl 
    {
        get => _currentDropControl;
        private set
        {
            if (_currentDropControl.ReferenceEq(value))
            {
                return;
            }
            _currentDropControl = value;

            if (DragBoundsWithinDropControl.Width <= 0 && CurrentDropControl != null)
            {
                _dragBoundWithinDropControl = CurrentDropControl.Bounds;
            }

            OnPropertyChanged(nameof(CurrentDropControl));
        }
    }

    private void OnDragMove(PixelPoint currentPointerLocationInScreen)
    {
        var currentDropControl =
            this.DropControls
                .FirstOrDefault
                (
                    c => c.IsVisible &&
                            c.GetScreenBounds()
                             .ContainsPoint(currentPointerLocationInScreen)
                 );  


        if (currentDropControl == null && DropControls.Count > 0)
        {
            //CurrentDropControl = null;
            //Console.WriteLine("Null Current Control");
            //return;
        }

        PixelPoint pointerShiftFromOrigin = currentPointerLocationInScreen - DragPointerStartPositionInScreen;

        if (!IsDragOn)
        {
            if (pointerShiftFromOrigin.Magnitude() < MinDragShift)
            {
                // distance is too small to start drag
                // Console.WriteLine("Distance too small");
                return;
            }
            else
            {
                IsDragOn = true;

                CurrentDropControl = currentDropControl;
            }
        }

        CurrentDragShift = pointerShiftFromOrigin + InitialDragControlShift;

        CurrentDragCuePositionInScreen = DragControlStartPositionInScreen + CurrentDragShift;

        Console.WriteLine(CurrentDragCuePositionInScreen);
    }

    public ObservableCollection<Control> DropControls { get; } = 
        new ObservableCollection<Control>();

    public PixelPoint DragControlStartPositionInScreen { get; private set; }

    public PixelPoint DragPointerStartPositionInScreen { get; private set; }

    public double MinDragShift { get; init; } = PointHelper.MinimumDragDistance;


    #region IsDragOn Property
    private bool _isDragOn = false;
    public bool IsDragOn
    {
        get
        {
            return this._isDragOn;
        }
        private set
        {
            if (this._isDragOn == value)
            {
                return;
            }

            DragControl!.PointerReleased -= OnPointerReleased;

            this._isDragOn = value;
            this.OnPropertyChanged(nameof(IsDragOn));

            if (IsDragOn)
            {
                DragControl!.PointerReleased += OnPointerReleased;
            }
            else
            {
                CurrentDropControl = null;
            }
        }
    }

    #endregion IsDragOn Property

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        DropEventArgs args = new DropEventArgs(DragControl, CurrentDropControl, DropEvent);

        InitialDragControlShift = CurrentDragShift;

        IsDragOn = false;

        DragControl.RaiseEvent(args);
    }

    public static readonly RoutedEvent<DropEventArgs> DropEvent =
        RoutedEvent.Register<DragManager, DropEventArgs>
        (
            "Drop",
            RoutingStrategies.Bubble);


    #region CurrentDragShift Property
    private PixelPoint _currentDragShift;
    public PixelPoint CurrentDragShift
    {
        get
        {
            return this._currentDragShift;
        }
        private set
        {
            if (this._currentDragShift == value)
            {
                return;
            }

            this._currentDragShift = value;
            this.OnPropertyChanged(nameof(CurrentDragShift));
        }
    }
    #endregion CurrentDragShift Property


    #region CurrentDragCuePositionInScreen Property
    private PixelPoint _currentDragCuePositionInScreen;
    public PixelPoint CurrentDragCuePositionInScreen
    {
        get
        {
            return this._currentDragCuePositionInScreen;
        }
        private set
        {
            if (this._currentDragCuePositionInScreen == value)
            {
                return;
            }

            this._currentDragCuePositionInScreen = value;
            this.OnPropertyChanged(nameof(CurrentDragCuePositionInScreen));
        }
    }
    #endregion CurrentDragCuePositionInScreen Property


    #region InitialDragControlShift Property
    private PixelPoint _initialDragControlShift = new PixelPoint();
    public PixelPoint InitialDragControlShift   
    {
        get
        {
            return this._initialDragControlShift;
        }
        set
        {
            if (this._initialDragControlShift == value)
            {
                return;
            }

            this._initialDragControlShift = value;
            this.OnPropertyChanged(nameof(InitialDragControlShift));
        }
    }
    #endregion InitialDragControlShift Property

}
