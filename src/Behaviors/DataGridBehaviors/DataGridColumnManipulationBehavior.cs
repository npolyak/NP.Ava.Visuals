using Avalonia;
using Avalonia.Controls;
using NP.Concepts.Behaviors;
using NP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NP.Avalonia.Visuals.Behaviors.DataGridBehaviors
{
    public static class DataGridColumnManipulationBehavior
    {

        #region IsOn Attached Avalonia Property
        public static bool GetIsOn(DataGrid obj)
        {
            return obj.GetValue(IsOnProperty);
        }

        public static void SetIsOn(DataGrid obj, bool value)
        {
            obj.SetValue(IsOnProperty, value);
        }

        public static readonly AttachedProperty<bool> IsOnProperty =
            AvaloniaProperty.RegisterAttached<DataGrid, DataGrid, bool>
            (
                "IsOn"
            );
        #endregion IsOn Attached Avalonia Property


        #region ColumnManipulationBehavior Attached Avalonia Property
        public static BehaviorsDisposable<IEnumerable<DataGridColumn>> GetColumnManipulationBehavior(DataGrid obj)
        {
            return obj.GetValue(ColumnManipulationBehaviorProperty);
        }

        public static void SetColumnManipulationBehavior(DataGrid obj, BehaviorsDisposable<IEnumerable<DataGridColumn>> value)
        {
            obj.SetValue(ColumnManipulationBehaviorProperty, value);
        }

        public static readonly AttachedProperty<BehaviorsDisposable<IEnumerable<DataGridColumn>>> ColumnManipulationBehaviorProperty =
            AvaloniaProperty.RegisterAttached<DataGrid, DataGrid, BehaviorsDisposable<IEnumerable<DataGridColumn>>>
            (
                "ColumnManipulationBehavior"
            );
        #endregion ColumnManipulationBehavior Attached Avalonia Property


        static DataGridColumnManipulationBehavior()
        {
            IsOnProperty.Changed.Subscribe(OnIsOnChanged);
        }

        private static void OnIsOnChanged(AvaloniaPropertyChangedEventArgs<bool> args)
        {
            DataGrid dataGrid = (DataGrid)args.Sender;

            if (args.NewValue.Value)
            {
                SetColumnManipulationBehavior(dataGrid, dataGrid.Columns.AddBehavior(OnColumnAdded));
            }
        }

        private static void OnColumnAdded(DataGridColumn col)
        {
            DataGridColumnHeader header = col.GetPropValue<DataGridColumnHeader>("HeaderCell", true);

            SetColumn(header, col);
        }

        #region Column Attached Avalonia Property
        public static DataGridColumn GetColumn(DataGridColumnHeader obj)
        {
            return obj.GetValue(ColumnProperty);
        }

        public static void SetColumn(DataGridColumnHeader obj, DataGridColumn value)
        {
            obj.SetValue(ColumnProperty, value);
        }

        public static readonly AttachedProperty<DataGridColumn> ColumnProperty =
            AvaloniaProperty.RegisterAttached<DataGridColumnHeader, DataGridColumnHeader, DataGridColumn>
            (
                "Column"
            );
        #endregion Column Attached Avalonia Property


        #region CanRemoveColumn Attached Avalonia Property
        public static bool GetCanRemoveColumn(DataGridColumn column)
        {
            return column.GetValue(CanRemoveColumnProperty);
        }

        public static void SetCanRemoveColumn(DataGridColumn column, bool value)
        {
            column.SetValue(CanRemoveColumnProperty, value);
        }

        public static readonly AttachedProperty<bool> CanRemoveColumnProperty =
            AvaloniaProperty.RegisterAttached<DataGridColumn, DataGridColumn, bool>
            (
                "CanRemoveColumn",
                true
            );
        #endregion CanRemoveColumn Attached Avalonia Property


        public static void RemoveColumn(this DataGridColumn column)
        {
            column.IsVisible = false;
        }

        public static DataGridLengthConverter TheDataGridLengthConverter { get; } =
            new DataGridLengthConverter();

        public static void SaveDataGridLayoutToFile(this DataGrid dataGrid, string fileName)
        {
            var colSerializationData = 
                dataGrid
                    .Columns
                        .OrderBy(col => col.DisplayIndex)
                        .Select
                        (col => new ColumnSerializationData 
                                {
                                    IsVisible = col.IsVisible, 
                                    WidthStr = TheDataGridLengthConverter.ConvertToString(col.Width),
                                    HeaderId = col.Header?.ToStr()
                                }).ToArray();

            XmlSerializationUtils.SerializeToFile(colSerializationData, fileName);
        }

        public static void RestoreDataGridLayoutFromFile(this DataGrid dataGrid, string fileName)
        {
            ColumnSerializationData[] colSerializationData = 
                XmlSerializationUtils.DeserializeFromFile<ColumnSerializationData[]>(fileName);

            colSerializationData
                .DoForEach
                (
                    (col, idx) =>
                    {
                        DataGridColumn gridCol = 
                            dataGrid.Columns.Single(dataGridCol => dataGridCol.Header?.ToString() == col.HeaderId);

                        gridCol.IsVisible = col.IsVisible;
                        gridCol.DisplayIndex = idx;
                        gridCol.Width = (DataGridLength)TheDataGridLengthConverter.ConvertFromString(col.WidthStr);
                    });
        }
    }
}
