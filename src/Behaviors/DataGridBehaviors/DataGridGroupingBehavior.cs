using System;
using System.Linq;
using Avalonia;
using Avalonia.Automation.Provider;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using NP.Utilities;

namespace NP.Ava.Visuals.Behaviors.DataGridBehaviors
{
    public static class DataGridGroupingBehavior
    {
        class MousePositionInfo
        {
            public Point StartDragMousePosition { get; }
            public Point StartHeaderPosition { get; }
            public DataGridColumnCollection ColumnsInternals { get; }
            public DataGridColumnHeadersPresenter ColumnHeaders { get; }

            public double CellsWidth { get; }
            public double FrozenColumnsWidth { get; }
            public ScrollBar HorizontalScrollBar { get; }

            public bool IsDraggingHeader { get; private set; } = false;

            public Grid DragIndicatorContainer { get; private set; }

            public Control DropLocationIndicator { get; set; }

            public MousePositionInfo
            (
                Point startDragMousePosition, 
                Point startHeaderPosition,
                DataGridColumnCollection columnsInternals, 
                DataGridColumnHeadersPresenter columnHeaders,
                double cellWidth,
                double frozenColumnsWidth,
                ScrollBar horizontalScrollBar,
                Grid dragIndicatorContainer)
            {
                StartDragMousePosition = startDragMousePosition;
                StartHeaderPosition = startHeaderPosition;
                ColumnsInternals = columnsInternals;
                ColumnHeaders = columnHeaders;
                CellsWidth = cellWidth;
                FrozenColumnsWidth = frozenColumnsWidth;
                HorizontalScrollBar = horizontalScrollBar;
                DragIndicatorContainer = dragIndicatorContainer;
            }

            public (Point startDragPostion,
                    Point startHeaderPosition,
                    DataGridColumnCollection columnInternal, 
                    DataGridColumnHeadersPresenter columnHeaders,
                    double cellsWidth,
                    double frozenCellsWidth,
                    ScrollBar horizontalScrollBar,
                    Grid dragIndicatorContainer,
                    Control dropLocationIndicator
                ) Deconstruct()
            {
                return 
                    (StartDragMousePosition, 
                     StartHeaderPosition,
                     ColumnsInternals, 
                     ColumnHeaders, 
                     CellsWidth, 
                     FrozenColumnsWidth, 
                     HorizontalScrollBar,
                     DragIndicatorContainer,
                     DropLocationIndicator);
            }

            public void SetDragging()
            {
                IsDraggingHeader = true;
            }
        }


        #region DragInfo Attached Avalonia Property
        private static MousePositionInfo GetDragInfo(this Control obj)
        {
            return obj.GetValue(DragInfoProperty);
        }

        private static void SetDragInfo(Control obj, MousePositionInfo value)
        {
            obj.SetValue(DragInfoProperty, value);
        }

        private static readonly AttachedProperty<MousePositionInfo> DragInfoProperty =
            AvaloniaProperty.RegisterAttached<DataGridColumnHeader, DataGridColumnHeader, MousePositionInfo>
            (
                "DragInfo"
            );
        #endregion DragInfo Attached Avalonia Property


        private enum DragMode
        {
            None = 0,
            MouseDown = 1,
            Drag = 2,
            Resize = 3,
            Reorder = 4
        }

        private const int DATAGRIDCOLUMNHEADER_columnsDragTreshold = 5;
        private const int DATAGRIDCOLUMNHEADER_resizeRegionWidth = 5;

        #region IsGroupingOn Attached Avalonia Property
        public static bool GetIsGroupingOn(Control obj)
        {
            return obj.GetValue(IsGroupingOnProperty);
        }

        public static void SetIsGroupingOn(Control obj, bool value)
        {
            obj.SetValue(IsGroupingOnProperty, value);
        }

        public static readonly AttachedProperty<bool> IsGroupingOnProperty =
            AvaloniaProperty.RegisterAttached<DataGrid, DataGrid, bool>
            (
                "IsGroupingOn",
                false
            );
        #endregion IsGroupingOn Attached Avalonia Property


        #region IsHeaderGroupingOn Attached Avalonia Property
        public static bool GetIsHeaderGroupingOn(Control obj)
        {
            return obj.GetValue(IsHeaderGroupingOnProperty);
        }

        public static void SetIsHeaderGroupingOn(Control obj, bool value)
        {
            obj.SetValue(IsHeaderGroupingOnProperty, value);
        }

        public static readonly AttachedProperty<bool> IsHeaderGroupingOnProperty =
            AvaloniaProperty.RegisterAttached<DataGridColumnHeader, DataGridColumnHeader, bool>
            (
                "IsHeaderGroupingOn"
            );
        #endregion IsHeaderGroupingOn Attached Avalonia Property

        static DataGridGroupingBehavior()
        {
            IsHeaderGroupingOnProperty.Changed.Subscribe(IsHeaderGroupingChanged);
        }

        private static void IsHeaderGroupingChanged(AvaloniaPropertyChangedEventArgs<bool> args)
        {
            DataGridColumnHeader header = (DataGridColumnHeader) args.Sender;

            if (args.NewValue == true)
            {
                header.PointerPressed += Header_PointerPressed;
            }
            else
            {
                header.PointerPressed -= Header_PointerPressed;
            }
        }

        private static void Header_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            DataGridColumnHeader header = (DataGridColumnHeader)sender!;

            DataGridColumn currentColumn = header.OwningColumn;
            DataGrid dataGrid = header.OwningGrid;

            if (currentColumn is DataGridFillerColumn)
            {
                return;
            }

            DataGridColumnHeadersPresenter columnHeaders =
                dataGrid.FindDescendantOfType<DataGridColumnHeadersPresenter>()!;

            DataGridColumnCollection columnsInternal =
                dataGrid.GetPropValue<DataGridColumnCollection>("ColumnsInternal", true);

            DataGridColumn previousCollumn = columnsInternal.GetPreviousVisibleNonFillerColumn(currentColumn);

            DragMode dragMode = (DragMode)header.GetType().GetStaticFieldValue("_dragMode", true);

            Point mousePositionWithinHeader = e.GetPosition(header);


            Grid headersAndGroupsContainer =
                (Grid)columnHeaders
                    .GetSelfAndVisualAncestors()
                    .FirstOrDefault(g => g.Name == "PART_GroupingAndColumnHeadersContainer")!;

            Grid dragIndicatorContainer =
                (Grid) headersAndGroupsContainer.GetVisualDescendants().FirstOrDefault(d => d.Name == "PART_DragIndicatorContainer")!;

            Point startMousePositionWithinDragContainer = header.Translate(dragIndicatorContainer, mousePositionWithinHeader);
            Point startHeaderPositionWithinDragContainer = header.Translate(dragIndicatorContainer, new Point());

            double distanceFromLeft = mousePositionWithinHeader.X;
            double distanceFromRight = header.Bounds.Width - distanceFromLeft;

            if (dragMode == DragMode.MouseDown)
            {
                if (distanceFromLeft < DATAGRIDCOLUMNHEADER_resizeRegionWidth)
                {
                    return;
                }
                else if (distanceFromRight < DATAGRIDCOLUMNHEADER_resizeRegionWidth && previousCollumn != null)
                {
                    return;
                }
            }

            e.Pointer.Capture(header);

            double cellsWidth = dataGrid.GetPropValue<double>("CellsWidth", true);

            double frozenCellsWidth = columnsInternal.GetVisibleFrozenEdgedColumnsWidth();
            ScrollBar horizontalScrollBar = dataGrid.GetPropValue<ScrollBar>("HorizontalScrollBar", true);

            MousePositionInfo mousePositionInfo = 
                new MousePositionInfo
                (
                    startMousePositionWithinDragContainer, 
                    startHeaderPositionWithinDragContainer,
                    columnsInternal, 
                    columnHeaders,
                    cellsWidth,
                    frozenCellsWidth,
                    horizontalScrollBar,
                    dragIndicatorContainer);

            SetDragInfo(header, mousePositionInfo);

            header.AddHandler(Control.PointerReleasedEvent, ClearEvents, Avalonia.Interactivity.RoutingStrategies.Tunnel, true);
            header.PointerMoved += Header_PointerMoved;
        }

        private static void StartReorder(this DataGridColumnHeader header, PointerEventArgs e)
        {
            DataGrid dataGrid = header.OwningGrid;
            if (dataGrid == null || !header.IsEnabled)
            {
                return;
            }

            var dragIndicator = new DataGridColumnHeader
            {
                IsEnabled = false,
                Content = header.Content,
                OwningColumn = header.OwningColumn, 
                Background = new SolidColorBrush(Colors.LightGray),
                ContentTemplate = header.ContentTemplate,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Padding = new Thickness(3d, 0, 3d, 3d),
                VerticalContentAlignment = VerticalAlignment.Center,
                RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative),
                RenderTransform = new TranslateTransform()
            };

            if (dataGrid.ColumnHeaderTheme is { } columnHeaderTheme)
            {
                dragIndicator.SetValue(DataGridColumnHeader.ThemeProperty, columnHeaderTheme, BindingPriority.Template);
            }

            Control dropLocationIndicator = dataGrid.DropLocationIndicatorTemplate?.Build()!;

            // If the user didn't style the dropLocationIndicator's Height, default to the column header's height
            if (dropLocationIndicator != null && double.IsNaN(dropLocationIndicator.Height) && dropLocationIndicator is Control element)
            {
                element.Height = header.Bounds.Height;
            }

            // pass the caret's data template to the user for modification
            DataGridColumnReorderingEventArgs columnReorderingEventArgs = new DataGridColumnReorderingEventArgs(header.OwningColumn)
            {
                DropLocationIndicator = dropLocationIndicator,
                DragIndicator = dragIndicator
            };

            dataGrid.CallMethodExtras("OnColumnReordering", true, false, columnReorderingEventArgs);
            if (columnReorderingEventArgs.Cancel)
            {
                return;
            }

            var dragInfo = header.GetDragInfo();

            (Point startDragMousePosition, Point startDragHeaderPosition, DataGridColumnCollection columnInternal, DataGridColumnHeadersPresenter columnHeaders, _, _, _, Grid dragContainer, _) =
                dragInfo.Deconstruct();

            dragIndicator.Width = header.Bounds.Width;

            var visual = header.GetVisualDescendants().FirstOrDefault(d => d.Name == "PART_Header");

            dragIndicator.Height = visual!.Bounds.Height * 1.3;

            dragContainer.Children.Add(dragIndicator);

            header.SetShift(e);

            dragInfo.DropLocationIndicator = columnReorderingEventArgs.DropLocationIndicator!;

            columnHeaders.DropLocationIndicator = columnReorderingEventArgs.DropLocationIndicator;

            columnHeaders.DragColumn = header.OwningColumn;

            // If the user didn't style the dragIndicator's Width, default it to the column header's width
            if (double.IsNaN(dragIndicator.Width))
            {
                dragIndicator.Width = header.Bounds.Width;
            }
        }

        public static Point GetShift(this DataGridColumnHeader header)
        {
            (Point startDragMousePosition, Point startDragHeaderPosition, _, _, _, _, _, Grid dragContainer, _) =
                header.GetDragInfo().Deconstruct();

            DataGridColumnHeader dragIndicator = (DataGridColumnHeader)dragContainer.Children.FirstOrDefault()!;

            var transform = (TranslateTransform)dragIndicator.RenderTransform!;

            return new Point(transform.X, transform.Y);
        }

        private static void SetShift(this DataGridColumnHeader header, PointerEventArgs e)
        {
            (Point startDragMousePosition, Point startDragHeaderPosition, _, _, _, _, _, Grid dragContainer, _) =
                header.GetDragInfo().Deconstruct();

            Point currentMousePosition = e.GetPosition(dragContainer);

            Point shift = startDragHeaderPosition.Add(currentMousePosition.Subtract(startDragMousePosition));

            DataGridColumnHeader dragIndicator = (DataGridColumnHeader) dragContainer.Children.FirstOrDefault()!;

            var dragIndicatorBounds = dragIndicator.Bounds.ToPoint();
            var dragContainerBounds = dragContainer.Bounds.ToPoint();

            Point maxShift = dragContainerBounds.Subtract(dragIndicatorBounds); 

            if (shift.X < 0)
                shift = shift.WithX(0);

            if (shift.Y < 0)
                shift = shift.WithY(0);

            if (shift.X > maxShift.X)
            {
                shift = shift.WithX(maxShift.X);
            }    

            if (shift.Y > maxShift.Y)
            {
                shift = shift.WithY(maxShift.Y);
            }

            ((TranslateTransform)dragIndicator.RenderTransform!).X = shift.X;
            ((TranslateTransform)dragIndicator.RenderTransform!).Y = shift.Y;
        }

        /// <summary>
        /// Returns the column against whose top-left the reordering caret should be positioned
        /// </summary>
        /// <param name="mousePositionHeaders">Mouse position within the ColumnHeadersPresenter</param>
        /// <param name="scroll">Whether or not to scroll horizontally when a column is dragged out of bounds</param>
        /// <param name="scrollAmount">If scroll is true, returns the horizontal amount that was scrolled</param>
        /// <returns></returns>
        private  static DataGridColumn GetReorderingTargetColumn
        (
            this DataGridColumnHeader header,
            Point mousePositionHeaders, 
            bool scroll, 
            out double scrollAmount)
        {
            var owningGrid = header.OwningGrid;
            var owningColumn = header.OwningColumn;

            (
                Point startDragPostion, 
                Point startDragHeaderPosition,
                DataGridColumnCollection columnsInternal, 
                DataGridColumnHeadersPresenter columnHeaders, 
                double cellsWidth, 
                double frozenColumnsWidth, 
                ScrollBar horizontalScrollBar,
                Grid dragIndicatorContainer,
                Control dropLocationIndicator) =
                header.GetDragInfo().Deconstruct();

            scrollAmount = 0;
            double leftEdge = columnsInternal.RowGroupSpacerColumn.IsRepresented ? columnsInternal.RowGroupSpacerColumn.ActualWidth : 0;
            double rightEdge = cellsWidth;
            if (owningColumn.IsFrozen)
            {
                rightEdge = Math.Min(rightEdge, frozenColumnsWidth);
            }
            else if (owningGrid.FrozenColumnCount > 0)
            {
                leftEdge = frozenColumnsWidth;
            }

            if (mousePositionHeaders.X < leftEdge)
            {
                if (scroll &&
                    horizontalScrollBar != null &&
                    horizontalScrollBar.IsVisible &&
                    horizontalScrollBar.Value > 0)
                {
                    double newVal = mousePositionHeaders.X - leftEdge;
                    scrollAmount = Math.Min(newVal, horizontalScrollBar.Value);
                    owningGrid.CallMethodExtras("UpdateHorizontalOffset", true, false, scrollAmount + horizontalScrollBar.Value);
                }
                mousePositionHeaders = mousePositionHeaders.WithX(leftEdge);
            }
            else if (mousePositionHeaders.X >= rightEdge)
            {
                if (scroll &&
                    horizontalScrollBar != null &&
                    horizontalScrollBar.IsVisible &&
                    horizontalScrollBar.Value < horizontalScrollBar.Maximum)
                {
                    double newVal = mousePositionHeaders.X - rightEdge;
                    scrollAmount = Math.Min(newVal, horizontalScrollBar.Maximum - horizontalScrollBar.Value);
                    owningGrid.CallMethodExtras("UpdateHorizontalOffset", true, false, scrollAmount + horizontalScrollBar.Value);
                }
                mousePositionHeaders = mousePositionHeaders.WithX(rightEdge - 1);
            }

            foreach (DataGridColumn column in columnsInternal.GetDisplayedColumns())
            {
                Point mousePosition = columnHeaders.Translate(column.HeaderCell, mousePositionHeaders);
                double columnMiddle = column.HeaderCell.Bounds.Width / 2;
                if (mousePosition.X >= 0 && mousePosition.X <= columnMiddle)
                {
                    return column;
                }
                else if (mousePosition.X > columnMiddle && mousePosition.X < column.HeaderCell.Bounds.Width)
                {
                    return columnsInternal.GetNextVisibleColumn(column);
                }
            }

            return null;
        }

        private static void Header_PointerMoved(object? sender, PointerEventArgs e)
        {
            DataGridColumnHeader header = (DataGridColumnHeader)sender!;
            DataGrid dataGrid = header.OwningGrid;
            DataGridColumn owningColumn = header.OwningColumn;
            var dragInfo = header.GetDragInfo();
            (Point startDragPostion, Point startDragHeaderPosition, DataGridColumnCollection columnsInternal, DataGridColumnHeadersPresenter columnHeaders, _, _, _, Grid dragIndicatorContainer, Control dropLocationIndicator) =
                dragInfo.Deconstruct();

            if (columnHeaders == null)
                return;

            Point mousePositionHeaders = e.GetPosition(columnHeaders);

            var distanceFromStart = Math.Abs(mousePositionHeaders.X - startDragPostion.X);
            var isDistanceFromStartSmall = distanceFromStart < 5;

            if (!dragInfo.IsDraggingHeader)
            {
                if (isDistanceFromStartSmall)
                {
                    return;
                }
                else //(!isDistanceFromStartSmall)
                {
                    header.RemoveHandler(Control.PointerReleasedEvent, ClearEvents);
                    header.AddHandler(Control.PointerReleasedEvent, Header_PointerReleased, Avalonia.Interactivity.RoutingStrategies.Tunnel, true);
                    if (columnHeaders.DragColumn == null)
                    {
                        header.StartReorder(e);
                    }
                    dragInfo.SetDragging();
                }
            }

            header.SetShift(e);

            Point shift = header.GetShift();

            Rect groupArea = dragIndicatorContainer.GetGroupArea();

            if (groupArea.Contains(shift))
            {
                columnHeaders.DragColumn = null;
                columnHeaders.DropLocationIndicator = null;
                columnHeaders.DropLocationIndicatorOffset = -1000;
            }
            else
            {
                columnHeaders.DragColumn = header.OwningColumn;
                columnHeaders.DropLocationIndicator = dropLocationIndicator;
                // Find header we're hovering over
                DataGridColumn targetColumn =
                    header.GetReorderingTargetColumn(mousePositionHeaders, !header.OwningColumn.IsFrozen, out double scrollAmount);

                columnHeaders.DragIndicatorOffset = mousePositionHeaders.X - startDragPostion.X + scrollAmount;

                columnHeaders.InvalidateArrange();

                Point targetPosition = new Point(0, 0);
                if (targetColumn == null || targetColumn == columnsInternal.FillerColumn || targetColumn.IsFrozen != owningColumn.IsFrozen)
                {
                    targetColumn =
                        columnsInternal.GetLastColumn(
                            isVisible: true,
                            isFrozen: owningColumn.IsFrozen,
                            isReadOnly: null);
                    targetPosition = targetColumn.HeaderCell.Translate(columnHeaders, targetPosition);

                    targetPosition = targetPosition.WithX(targetPosition.X + targetColumn.ActualWidth);
                }
                else
                {
                    targetPosition = targetColumn.HeaderCell.Translate(columnHeaders, targetPosition);
                }
                columnHeaders.DropLocationIndicatorOffset = targetPosition.X - scrollAmount;
            }
        }

        private static Rect GetGroupArea(this Grid dragContainer)
        {
            Grid groupingAndColumnHeadersContainer =
                (Grid)dragContainer.VisualParent!;

            Grid groupingPanel =
                (Grid)groupingAndColumnHeadersContainer
                            .VisualChildren
                            .FirstOrDefault(c => c.Name == "PART_GroupingPanel")!;

            Rect groupArea = groupingPanel.ToRect();

            return groupArea;
        }

        private static void ClearEvents(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
        {
            DataGridColumnHeader header = (DataGridColumnHeader)sender!;
            header.PointerReleased -= Header_PointerReleased;
            header.PointerMoved -= Header_PointerMoved;
        }

        private static void Header_PointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
        {
            DataGridColumnHeader header = (DataGridColumnHeader)sender!;
            ClearEvents(sender, e);

            (_, _, _, DataGridColumnHeadersPresenter columnHeaders, _, _, _, Grid dragContainer,_) =
                header.GetDragInfo().Deconstruct();

            DataGridColumn owningColumn = header.OwningColumn;
            DataGrid owningGrid = header.OwningGrid;

            header.SetShift(e);

            Point shift = header.GetShift();

            Rect groupArea = dragContainer.GetGroupArea();

            if (groupArea.Contains(shift))
            {
                columnHeaders.DragColumn = null;
                columnHeaders.DropLocationIndicatorOffset = -1000;
            }
            else
            {
                columnHeaders.DragColumn = owningColumn;
                Point mousePosition = e.GetPosition(header);

                Point mousePositionHeaders = header.Translate(columnHeaders, mousePosition);

                int targetIndex = header.GetReorderingTargetDisplayIndex(mousePositionHeaders);

                if ((!owningColumn.IsFrozen && targetIndex >= owningGrid.FrozenColumnCount)
                       || (owningColumn.IsFrozen && targetIndex < owningGrid.FrozenColumnCount))
                {
                    owningColumn.DisplayIndex = targetIndex;

                    DataGridColumnEventArgs ea = new DataGridColumnEventArgs(owningColumn);
                    owningGrid.CallMethodExtras("OnColumnReordered", true, false, ea);
                }
            }


            dragContainer.Children.Clear();

            e.Pointer.Capture(null);
            columnHeaders.DragColumn = null;
            columnHeaders.DropLocationIndicator = null;

            header.ClearValue(DragInfoProperty);

            e.Handled = true;
        }

        private static int GetReorderingTargetDisplayIndex(this DataGridColumnHeader header, Point mousePositionHeaders)
        {
            DataGridColumn targetColumn = header.GetReorderingTargetColumn(mousePositionHeaders, false /*scroll*/, out double scrollAmount);
            if (targetColumn != null)
            {
                return targetColumn.DisplayIndex > header.OwningColumn.DisplayIndex ? targetColumn.DisplayIndex - 1 : targetColumn.DisplayIndex;
            }
            else
            {
                return header.OwningGrid.Columns.Count - 1;
            }
        }

    }
}
