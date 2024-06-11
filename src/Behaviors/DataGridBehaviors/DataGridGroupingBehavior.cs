using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.VisualTree;
using NP.Utilities;

namespace NP.Ava.Visuals.Behaviors.DataGridBehaviors
{
    public static class DataGridGroupingBehavior
    {
        class MousePositionInfo
        {
            public Point StartDragMousePosition { get; }
            public DataGridColumnCollection ColumnsInternals { get; }
            public DataGridColumnHeadersPresenter ColumnHeaders { get; }

            public double CellsWidth { get; }
            public double FrozenColumnsWidth { get; }
            public ScrollBar HorizontalScrollBar { get; }

            public bool IsDraggingHeader { get; private set; } = false;

            public MousePositionInfo
            (
                Point startDragMousePosition, 
                DataGridColumnCollection columnsInternals, 
                DataGridColumnHeadersPresenter columnHeaders,
                double cellWidth,
                double frozenColumnsWidth,
                ScrollBar horizontalScrollBar)
            {
                StartDragMousePosition = startDragMousePosition;
                ColumnsInternals = columnsInternals;
                ColumnHeaders = columnHeaders;
                CellsWidth = cellWidth;
                FrozenColumnsWidth = frozenColumnsWidth;
                HorizontalScrollBar = horizontalScrollBar;
            }

            public (Point startDragPostion, 
                    DataGridColumnCollection columnInternal, 
                    DataGridColumnHeadersPresenter columnHeaders,
                    double cellsWidth,
                    double frozenCellsWidth,
                    ScrollBar horizontalScrollBar) Deconstruct()
            {
                return (StartDragMousePosition, ColumnsInternals, ColumnHeaders, CellsWidth, FrozenColumnsWidth, HorizontalScrollBar);
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

            Point mousePosition = e.GetPosition(header);

            Point lastMousePositionHeaders = header.Translate(columnHeaders, mousePosition);

            double distanceFromLeft = mousePosition.X;
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
                    lastMousePositionHeaders, 
                    columnsInternal, 
                    columnHeaders,
                    cellsWidth,
                    frozenCellsWidth,
                    horizontalScrollBar);

            SetDragInfo(header, mousePositionInfo);

            header.AddHandler(Control.PointerReleasedEvent, ClearEvents, Avalonia.Interactivity.RoutingStrategies.Tunnel, true);
            header.PointerMoved += Header_PointerMoved;
            //header.PointerReleased += Header_PointerReleased;
        }

        private static void StartReorder(this DataGridColumnHeader header)
        {
            DataGrid dataGrid = header.OwningGrid;
            if (dataGrid == null || !header.IsEnabled)
            {
                return;
            }

            var dragIndicator = new DataGridColumnHeader
            {
                OwningColumn = header.OwningColumn,
                IsEnabled = false,
                Content = header.Content,
                ContentTemplate = header.ContentTemplate
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

            (Point startDragPostion, DataGridColumnCollection columnInternal, DataGridColumnHeadersPresenter columnHeaders, _, _, _) =
                header.GetDragInfo().Deconstruct();

            columnHeaders.DragColumn = header.OwningColumn;
            columnHeaders.DragIndicator = dragIndicator;
            columnHeaders.DropLocationIndicator = columnReorderingEventArgs.DropLocationIndicator;

            // If the user didn't style the dragIndicator's Width, default it to the column header's width
            if (double.IsNaN(dragIndicator.Width))
            {
                dragIndicator.Width = header.Bounds.Width;
            }

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
                DataGridColumnCollection columnsInternal, 
                DataGridColumnHeadersPresenter columnHeaders, 
                double cellsWidth, 
                double frozenColumnsWidth, 
                ScrollBar horizontalScrollBar) =
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

        private static void Header_PointerMoved(object? sender, Avalonia.Input.PointerEventArgs e)
        {
            DataGridColumnHeader header = (DataGridColumnHeader)sender!;
            DataGrid dataGrid = header.OwningGrid;
            DataGridColumn owningColumn = header.OwningColumn;
            var dragInfo = header.GetDragInfo();
            (Point startDragPostion, DataGridColumnCollection columnsInternal, DataGridColumnHeadersPresenter columnHeaders, _, _, _) =
                dragInfo.Deconstruct();

            if (columnHeaders == null)
                return;

            Point mousePosition = e.GetPosition(header);
            Point mousePositionHeaders = e.GetPosition(columnHeaders);

            var distanceFromStart = Math.Abs(mousePositionHeaders.X - startDragPostion.X);
            var isDistanceFromStartSmall = distanceFromStart < 5;

            if (!dragInfo.IsDraggingHeader)
            {
                if (isDistanceFromStartSmall)
                {
                    return;
                }
                else if (!isDistanceFromStartSmall)
                {
                    header.RemoveHandler(Control.PointerReleasedEvent, ClearEvents);
                    header.AddHandler(Control.PointerReleasedEvent, Header_PointerReleased, Avalonia.Interactivity.RoutingStrategies.Tunnel, true);
                    if (columnHeaders.DragColumn == null)
                    {
                        header.StartReorder();
                    }
                    dragInfo.SetDragging();
                }
            }

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

            (_, _, DataGridColumnHeadersPresenter columnHeaders, _, _, _) =
                header.GetDragInfo().Deconstruct();

            DataGridColumn owningColumn = header.OwningColumn;
            DataGrid owningGrid = header.OwningGrid;

            Point mousePosition = e.GetPosition(header);

            Point mousePositionHeaders = header.Translate(columnHeaders, mousePosition);

            int targetIndex = header.GetReorderingTargetDisplayIndex(mousePositionHeaders);

            if ( (!owningColumn.IsFrozen && targetIndex >= owningGrid.FrozenColumnCount)
                   || (owningColumn.IsFrozen && targetIndex < owningGrid.FrozenColumnCount))
            {
                owningColumn.DisplayIndex = targetIndex;

                DataGridColumnEventArgs ea = new DataGridColumnEventArgs(owningColumn);
                owningGrid.CallMethodExtras("OnColumnReordered", true, false, ea);
            }


            e.Pointer.Capture(null);
            columnHeaders.DragColumn = null;
            columnHeaders.DragIndicator = null;
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
