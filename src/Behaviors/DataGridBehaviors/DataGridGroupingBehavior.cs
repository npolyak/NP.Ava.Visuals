using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data.Common;
using System.Linq;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.Templates;
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
        private static MousePositionInfo GetDragInfo(this DataGridColumnHeader obj)
        {
            return obj.GetValue(DragInfoProperty);
        }

        private static void SetDragInfo(DataGridColumnHeader obj, MousePositionInfo value)
        {
            obj.SetValue(DragInfoProperty, value);
        }

        private static readonly AttachedProperty<MousePositionInfo> DragInfoProperty =
            AvaloniaProperty.RegisterAttached<DataGridColumnHeader, DataGridColumnHeader, MousePositionInfo>
            (
                "DragInfo"
            );
        #endregion DragInfo Attached Avalonia Property

        public static void RemoveGroupingColumn(DataGridColumn groupingColumn)
        {
            DataGrid dataGrid = groupingColumn.OwningGrid;

            var groupColumns = GetGroupColumns(dataGrid);

            groupColumns.Remove(groupingColumn);
        }

        public static void RemoveAllGroupingColumns(DataGrid dataGrid)
        {
            var groupColumns = GetGroupColumns(dataGrid);

            groupColumns.RemoveAllOneByOne();
        }

        #region GroupingPropName Attached Avalonia Property
        public static string GetGroupingPropName(DataGridColumn obj)
        {
            return obj.GetValue(GroupingPropNameProperty);
        }

        public static void SetGroupingPropName(DataGridColumn obj, string value)
        {
            obj.SetValue(GroupingPropNameProperty, value);
        }

        public static readonly AttachedProperty<string> GroupingPropNameProperty =
            AvaloniaProperty.RegisterAttached<DataGridColumn, DataGridColumn, string>
            (
                "GroupingPropName"
            );
        #endregion GroupingPropName Attached Avalonia Property


        #region GroupColumns Attached Avalonia Property
        public static ObservableCollection<DataGridColumn> GetGroupColumns(DataGrid obj)
        {
            return obj.GetValue(GroupColumnsProperty);
        }

        public static void SetGroupColumns(DataGrid obj, ObservableCollection<DataGridColumn> value)
        {
            obj.SetValue(GroupColumnsProperty, value);
        }

        public static readonly AttachedProperty<ObservableCollection<DataGridColumn>> GroupColumnsProperty =
            AvaloniaProperty.RegisterAttached<DataGrid, DataGrid, ObservableCollection<DataGridColumn>>
            (
                "GroupColumns"
            );
        #endregion GroupColumns Attached Avalonia Property


        #region RemoveAllGroupsButtonClasses Attached Avalonia Property
        public static string GetRemoveAllGroupsButtonClasses(Control obj)
        {
            return obj.GetValue(RemoveAllGroupsButtonClassesProperty);
        }

        public static void SetRemoveAllGroupsButtonClasses(Control obj, string value)
        {
            obj.SetValue(RemoveAllGroupsButtonClassesProperty, value);
        }

        public static readonly AttachedProperty<string> RemoveAllGroupsButtonClassesProperty =
            AvaloniaProperty.RegisterAttached<DataGrid, DataGrid, string>
            (
                "RemoveAllGroupsButtonClasses"
            );
        #endregion RemoveAllGroupsButtonClasses Attached Avalonia Property


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


        #region GroupColumnDataTemplate Attached Avalonia Property
        public static DataTemplate GetGroupColumnDataTemplate(DataGrid obj)
        {
            return obj.GetValue(GroupColumnDataTemplateProperty);
        }

        public static void SetGroupColumnDataTemplate(DataGrid obj, DataTemplate value)
        {
            obj.SetValue(GroupColumnDataTemplateProperty, value);
        }

        public static readonly AttachedProperty<DataTemplate> GroupColumnDataTemplateProperty =
            AvaloniaProperty.RegisterAttached<DataGrid, DataGrid, DataTemplate>
            (
                "GroupColumnDataTemplate"
            );
        #endregion GroupColumnDataTemplate Attached Avalonia Property


        static DataGridGroupingBehavior()
        {
            IsGroupingOnProperty.Changed.Subscribe(OnIsGroupingOnChanged);
            IsHeaderGroupingOnProperty.Changed.Subscribe(IsHeaderGroupingChanged);
        }

        private static void OnIsGroupingOnChanged(AvaloniaPropertyChangedEventArgs<bool> args)
        {
            DataGrid dataGrid = (DataGrid) args.Sender!;

            if (args.GetNewValue<bool>())
            {
                var groupColumns = new ObservableCollection<DataGridColumn>();

                groupColumns.CollectionChanged += GroupColumns_CollectionChanged;

                SetGroupColumns(dataGrid, groupColumns);
            }
            else
            {
                var groupColumns = GetGroupColumns(dataGrid);

                groupColumns.CollectionChanged -= GroupColumns_CollectionChanged;
                dataGrid.ClearValue(GroupColumnsProperty);
            }
        }

        private static void GroupColumns_CollectionChanged
        (
            object? sender, 
            NotifyCollectionChangedEventArgs e)
        {
            DataGrid? dataGrid = null;
            if (e.OldItems != null)
            {
                DataGridColumn col = e.OldItems.Cast<DataGridColumn>().FirstOrDefault()!;

                dataGrid = col.OwningGrid;
            }
            else if (e.NewItems != null)
            {
                DataGridColumn col = e.NewItems.Cast<DataGridColumn>().FirstOrDefault()!;

                dataGrid = col.OwningGrid;
            }
            else
            {
                return;
            }

            DataGridCollectionView collectionView = (DataGridCollectionView)dataGrid.ItemsSource;

            if (e.OldItems != null)
            {
                foreach (DataGridColumn col in e.OldItems)
                {
                    var groupDesc = collectionView.GroupDescriptions.FirstOrDefault(gd => gd.PropertyName == GetGroupingPropName(col));

                    collectionView.GroupDescriptions.Remove(groupDesc);
                }
            }
            if (e.NewItems != null)
            {
                int newIdx = e.NewStartingIndex;
                foreach (DataGridColumn col in e.NewItems)
                {
                    var groupDesc = collectionView.GroupDescriptions.FirstOrDefault(gd => gd.PropertyName == GetGroupingPropName(col));

                    int groupDescIdx = collectionView.GroupDescriptions.IndexOf(groupDesc);

                    if (groupDescIdx < newIdx && groupDescIdx >= 0)
                    {
                        newIdx--;
                    }

                    if (groupDesc != null)
                    {
                        collectionView.GroupDescriptions.Remove(groupDesc);
                    }

                    collectionView.GroupDescriptions.Insert(newIdx, new DataGridPathGroupDescription(GetGroupingPropName(col)));

                    newIdx++;
                }
            }
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

        private static bool CanGroup(this DataGridColumnHeader header)
        {
            DataGridColumn column = header.OwningColumn;

            return (header.IsEnabled) && (!GetGroupingPropName(column).IsStrNullOrWhiteSpace());
        }

        // returns isGrouping flag
        private static bool SetShift(this DataGridColumnHeader header, PointerEventArgs e)
        {
            DataGridColumn column = header.OwningColumn;

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

            Rect groupArea = dragContainer.GetGroupArea();

            Panel groupingPanel = dragContainer.GetGroupingPanel();

            bool isGrouping = groupArea.Contains(shift) && header.CanGroup();

            if (isGrouping)
            {
                groupingPanel.Opacity = 1;
            }
            else
            {
                groupingPanel.Opacity = 0.6;
            }

            return isGrouping;
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

        private static Grid GetGroupingDropIndicator(this Grid dragContainer)
        {
            Grid groupingPanel = (Grid)dragContainer.GetGroupingPanel();
            Grid groupingIndicator = (Grid)groupingPanel.Children.FirstOrDefault(c => c.Name == "PART_GroupingDropIndicator")!;

            return groupingIndicator;
        }

        private static void Header_PointerMoved(object? sender, PointerEventArgs e)
        {
            DataGridColumnHeader header = (DataGridColumnHeader)sender!;
            DataGrid dataGrid = header.OwningGrid;
            DataGridColumn owningColumn = header.OwningColumn;
            var dragInfo = header.GetDragInfo();
            (
                Point startDragPostion, 
                Point startDragHeaderPosition, 
                DataGridColumnCollection columnsInternal, 
                DataGridColumnHeadersPresenter columnHeaders, 
                _, 
                _, 
                _, 
                Grid dragContainer, 
                Control dropLocationIndicator) =
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

            Grid groupingIndicator = dragContainer.GetGroupingDropIndicator();

            bool isGrouping = header.SetShift(e);

            if (isGrouping)
            {
                columnHeaders.DragColumn = null;
                columnHeaders.DropLocationIndicator = null;

                (int idxToInsertAt, double middleShift) =
                    header.GetGroupingDropInfo(e);

                var shiftX = middleShift - 1d;

                (groupingIndicator.RenderTransform as TranslateTransform)!.X = shiftX;

                groupingIndicator.IsVisible = true;
            }
            else
            {
                groupingIndicator.IsVisible = false;
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

        private static Panel GetGroupingPanel(this Grid dragContainer)
        {
            Grid groupingAndColumnHeadersContainer =
                 (Grid)dragContainer.VisualParent!;

            Grid groupingPanel =
                (Grid)groupingAndColumnHeadersContainer
                            .VisualChildren
                            .FirstOrDefault(c => c.Name == "PART_GroupingPanel")!;

            return groupingPanel;
        }

        private static Rect GetGroupArea(this Grid dragContainer)
        {
            Panel groupingPanel = dragContainer.GetGroupingPanel();

            Rect groupArea = groupingPanel.ToRect();

            return groupArea;
        }

        private static void ClearEvents(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
        {
            DataGridColumnHeader header = (DataGridColumnHeader)sender!;
            header.PointerReleased -= Header_PointerReleased;
            header.PointerMoved -= Header_PointerMoved;
            var dragInfo = header.GetDragInfo();
            Grid dragContainer = dragInfo.DragIndicatorContainer;

            Grid groupingDropIndicator = dragContainer.GetGroupingDropIndicator();

            groupingDropIndicator.IsVisible = false;
        }

        private static (int idxToInsertAt, double middleShift) 
            GetGroupingDropInfo(this DataGridColumnHeader header, PointerEventArgs e)
        {
            (_, _, _, DataGridColumnHeadersPresenter columnHeaders, _, _, _, Grid dragContainer, _) =
                header.GetDragInfo().Deconstruct();

            DataGrid owningGrid = header.OwningGrid;
            Grid groupingPanel = (Grid)dragContainer.GetGroupingPanel();
            ItemsControl itemsControl =
                (ItemsControl)groupingPanel.Children.FirstOrDefault(c => c.Name == "PART_GroupHeaders")!;

            var visualItems =
                itemsControl.GetVisualDescendants().OfType<Border>().Where(b => "Header".Equals(b.Tag)).ToList();

            double mouseXPositionInGroupingPanel = e.GetPosition(groupingPanel).X;

            int idxToInsertAt = 0;
            double leftShift = 0;
            double middleShift = 0;
            double minDistance = 99999;
            Visual lastVisualItem = null;
            double rightShift = 0;
            double resultMiddleShift = 0;
            for (int i = 0; i < visualItems.Count; i++)
            {
                lastVisualItem = visualItems[i];

                rightShift = lastVisualItem.Translate(groupingPanel, new Point()).X;

                middleShift = (leftShift + rightShift) / 2d;

                double newDistance = Math.Abs(mouseXPositionInGroupingPanel - middleShift);

                if (newDistance < minDistance)
                {
                    minDistance = newDistance;
                    idxToInsertAt = i; 
                    resultMiddleShift = middleShift;
                }

                leftShift = lastVisualItem.Translate(groupingPanel, lastVisualItem.Bounds.ToPoint()).X;
            }

            if (lastVisualItem != null)
            {
                middleShift = leftShift + 5;
                double newDistance = Math.Abs(mouseXPositionInGroupingPanel - middleShift);

                if (newDistance < minDistance)
                {
                    minDistance = newDistance;
                    idxToInsertAt = visualItems.Count;
                    resultMiddleShift = middleShift;
                }
            }

            return (idxToInsertAt, resultMiddleShift);
        }

        private static void Header_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            DataGridColumnHeader header = (DataGridColumnHeader)sender!;
            ClearEvents(sender, e);

            (_, _, _, DataGridColumnHeadersPresenter columnHeaders, _, _, _, Grid dragContainer,_) =
                header.GetDragInfo().Deconstruct();

            DataGridColumn owningColumn = header.OwningColumn;
            DataGrid owningGrid = header.OwningGrid;

            bool isGrouping = header.SetShift(e);

            if (isGrouping)
            {
                columnHeaders.DragColumn = null;
                columnHeaders.DropLocationIndicator = null;

                var groupColumns = GetGroupColumns(owningGrid);

                (int idxToInsertAt, double middleShift) =
                    header.GetGroupingDropInfo(e);
                
                var columnToRemove = groupColumns.FirstOrDefault(c => GetGroupingPropName(c) == GetGroupingPropName(owningColumn));

                if (columnToRemove != null)
                {
                    int columnToRemoveIdx = groupColumns.IndexOf(columnToRemove);

                    if (columnToRemoveIdx < idxToInsertAt)
                    {
                        idxToInsertAt--;
                    }
                    groupColumns.Remove(columnToRemove);
                }

                groupColumns.Insert(idxToInsertAt, owningColumn);
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
