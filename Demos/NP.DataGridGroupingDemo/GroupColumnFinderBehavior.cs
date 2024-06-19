using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.Templates;
using NP.Ava.Visuals.Behaviors.DataGridBehaviors;
using NP.Utilities;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace NP.DataGridGroupingDemo
{
    public static class GroupColumnFinderBehavior
    {
        #region GroupingColumn Attached Avalonia Property
        public static DataGridColumn GetGroupingColumn(DataGridRowGroupHeader obj)
        {
            return obj.GetValue(GroupingColumnProperty);
        }

        public static void SetGroupingColumn(DataGridRowGroupHeader obj, DataGridColumn value)
        {
            obj.SetValue(GroupingColumnProperty, value);
        }

        public static readonly AttachedProperty<DataGridColumn> GroupingColumnProperty =
            AvaloniaProperty.RegisterAttached<DataGridRowGroupHeader, DataGridRowGroupHeader, DataGridColumn>
            (
                "GroupingColumn"
            );
        #endregion GroupingColumn Attached Avalonia Property


        #region GroupCellTemplate Attached Avalonia Property
        public static DataTemplate GetGroupCellTemplate(DataGridColumn obj)
        {
            return obj.GetValue(GroupCellTemplateProperty);
        }

        public static void SetGroupCellTemplate(DataGridColumn obj, DataTemplate value)
        {
            obj.SetValue(GroupCellTemplateProperty, value);
        }

        public static readonly AttachedProperty<DataTemplate> GroupCellTemplateProperty =
            AvaloniaProperty.RegisterAttached<DataGridColumn, DataGridColumn, DataTemplate>
            (
                "GroupCellTemplate"
            );
        #endregion GroupCellTemplate Attached Avalonia Property


        static GroupColumnFinderBehavior()
        {
            DataGridRowGroupHeader.PropertyNameProperty.Changed.Subscribe(OnPropNameChanged);
        }

        private static void OnPropNameChanged(AvaloniaPropertyChangedEventArgs<string> args)
        {
            DataGridRowGroupHeader header = (DataGridRowGroupHeader) args.Sender;
            header.TrySetGroupingColumn();
        }

        private static void TrySetGroupingColumn(this DataGridRowGroupHeader rowGroupHeader)
        {
            DataGrid dataGrid = 
                rowGroupHeader.GetPropValue<DataGrid>("OwningGrid", true);

            if (dataGrid != null && rowGroupHeader.PropertyName != null)
            {
                DataGridColumn? groupingColumn = dataGrid.Columns.FirstOrDefault(c => DataGridGroupingBehavior.GetGroupingPropName(c) == rowGroupHeader.PropertyName);
                if (groupingColumn != null)
                {
                    SetGroupingColumn(rowGroupHeader, groupingColumn);
                }
            }
        }
    }
}
