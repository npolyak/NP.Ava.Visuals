using Avalonia;
using Avalonia.Controls;
using System;
using NP.Utilities;
using Avalonia.Collections;
using NP.Utilities.Expressions;
using System.Collections.Generic;
using System.Linq;

namespace NP.Avalonia.Visuals.Behaviors.DataGridBehaviors
{
    public class DataGridFilteringBehavior
    {
        #region ColumnFilterText Attached Avalonia Property
        public static string GetColumnFilterText(DataGridColumnHeader obj)
        {
            return obj.GetValue(ColumnFilterTextProperty);
        }

        public static void SetColumnFilterText(DataGridColumnHeader obj, string value)
        {
            obj.SetValue(ColumnFilterTextProperty, value);
        }

        public static readonly AttachedProperty<string> ColumnFilterTextProperty =
            AvaloniaProperty.RegisterAttached<DataGridColumnHeader, DataGridColumnHeader, string>
            (
                "ColumnFilterText"
            );
        #endregion ColumnFilterText Attached Avalonia Property


        #region FilterPropName Attached Avalonia Property
        public static string GetFilterPropName(DataGridColumn obj)
        {
            return obj.GetValue(FilterPropNameProperty);
        }

        public static void SetFilterPropName(DataGridColumn obj, string value)
        {
            obj.SetValue(FilterPropNameProperty, value);
        }

        public static readonly AttachedProperty<string> FilterPropNameProperty =
            AvaloniaProperty.RegisterAttached<DataGridColumn, DataGridColumn, string>
            (
                "FilterPropName"
            );
        #endregion FilterPropName Attached Avalonia Property


        #region ColumnPropGetter Attached Avalonia Property
        public static Func<object, object>? GetColumnPropGetter(DataGridColumn obj)
        {
            return obj.GetValue(ColumnPropGetterProperty);
        }

        public static void SetColumnPropGetter(DataGridColumn obj, Func<object, object>? value)
        {
            obj.SetValue(ColumnPropGetterProperty, value);
        }

        public static readonly AttachedProperty<Func<object, object>?> ColumnPropGetterProperty =
            AvaloniaProperty.RegisterAttached<DataGridColumn, DataGridColumn, Func<object, object>?>
            (
                "ColumnPropGetter"
            );
        #endregion ColumnPropGetter Attached Avalonia Property


        #region RowDataType Attached Avalonia Property
        public static Type GetRowDataType(DataGrid obj)
        {
            return obj.GetValue(RowDataTypeProperty);
        }

        public static void SetRowDataType(DataGrid obj, Type value)
        {
            obj.SetValue(RowDataTypeProperty, value);
        }

        public static readonly AttachedProperty<Type> RowDataTypeProperty =
            AvaloniaProperty.RegisterAttached<DataGrid, DataGrid, Type>
            (
                "RowDataType"
            );
        #endregion RowDataType Attached Avalonia Property


        #region HasFilters Attached Avalonia Property
        public static bool GetHasFilters(DataGrid obj)
        {
            return obj.GetValue(HasFiltersProperty);
        }

        public static void SetHasFilters(DataGrid obj, bool value)
        {
            obj.SetValue(HasFiltersProperty, value);
        }

        public static readonly AttachedProperty<bool> HasFiltersProperty =
            AvaloniaProperty.RegisterAttached<DataGrid, DataGrid, bool>
            (
                "HasFilters",
                true
            );
        #endregion HasFilters Attached Avalonia Property


        static DataGridFilteringBehavior()
        {
            ColumnFilterTextProperty.Changed.Subscribe(OnColumnFilterTextChanged);
            FilterPropNameProperty.Changed.Subscribe(OnFilterPropNameChanged);
            RowDataTypeProperty.Changed.Subscribe(OnRowDataTypeChanged);
        }

        private static void OnRowDataTypeChanged(AvaloniaPropertyChangedEventArgs<Type> obj)
        {
            DataGrid dataGrid = (DataGrid) obj.Sender;

            dataGrid.Columns.CollectionChanged += Columns_CollectionChanged;

            SetColumnPropGettersFromPropNames(dataGrid);
        }

        private static void Columns_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach(var col in e.NewItems.Cast<DataGridColumn>())
                {
                    SetColumnPropGetterFromPropName(col, GetFilterPropName(col));
                }
            }
        }

        private static void SetColumnPropGettersFromPropNames(DataGrid dataGrid)
        {
            foreach (var col in dataGrid.Columns)
            {
                SetColumnPropGetterFromPropName(col, GetFilterPropName(col));
            }
        }

        private static void OnFilterPropNameChanged(AvaloniaPropertyChangedEventArgs<string> obj)
        {
            DataGridColumn col = (DataGridColumn) obj.Sender;

            string propName = obj.NewValue.Value;

            SetColumnPropGetterFromPropName(col, propName);
        }

        public static void SetColumnPropGetterFromPropName(DataGridColumn col, string propName)
        {
            if (propName == null)
            {
                SetColumnPropGetter(col, null);
            }
            else
            {
                DataGrid dataGrid = col.GetPropValue<DataGrid>("OwningGrid", true);

                if (dataGrid == null)
                    return;

                Type rowType = GetRowDataType(dataGrid);

                if (rowType == null)
                {
                    return;
                }

                Func<object, object> propGetter = rowType.GetUntypedCSPropertyGetterByObjType(propName);

                SetColumnPropGetter(col, propGetter);
            }
        }

        private static void OnColumnFilterTextChanged(AvaloniaPropertyChangedEventArgs<string> args)
        {
            DataGridColumnHeader header = (DataGridColumnHeader)args.Sender;

            DataGrid dataGrid = (DataGrid)header.GetPropValue("OwningGrid", true);

            BuildFilter(dataGrid);
        }


        public static void BuildFilter(DataGrid dataGrid)
        {
            DataGridCollectionView collectionView = (DataGridCollectionView)dataGrid.ItemsSource;

            List<Func<object, bool>> colFilters = new List<Func<object, bool>>();

            foreach(DataGridColumn column in dataGrid.Columns)
            {
                Func<object, object> columnPropGetter = GetColumnPropGetter(column);

                if (columnPropGetter == null)
                    continue;

                DataGridColumnHeader columnHeader = 
                    column.GetPropValue<DataGridColumnHeader>("HeaderCell", true);

                string filterVal = GetColumnFilterText(columnHeader);

                if (string.IsNullOrEmpty(filterVal))
                    continue;

                Func<object, bool> colFilter = 
                    (object obj) => columnPropGetter(obj).ToStr().Contains(filterVal, StringComparison.OrdinalIgnoreCase);

                colFilters.Add(colFilter);
            }

            if (colFilters.Count == 0)
            {
                collectionView.Filter = null;
            }
            else
            {
                collectionView.Filter = (obj) => colFilters.All(f => f(obj));
            }
        }

        #region DataGridFilterTextBoxClasses Attached Avalonia Property
        public static string GetDataGridFilterTextBoxClasses(DataGrid obj)
        {
            return obj.GetValue(DataGridFilterTextBoxClassesProperty);
        }

        public static void SetDataGridFilterTextBoxClasses(DataGrid obj, string value)
        {
            obj.SetValue(DataGridFilterTextBoxClassesProperty, value);
        }

        public static readonly AttachedProperty<string> DataGridFilterTextBoxClassesProperty =
            AvaloniaProperty.RegisterAttached<DataGrid, DataGrid, string>
            (
                "DataGridFilterTextBoxClasses"
            );
        #endregion DataGridFilterTextBoxClasses Attached Avalonia Property
    }
}
