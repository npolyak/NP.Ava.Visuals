﻿

// (c) Nick Polyak 2021 - http://awebpros.com/
// License: MIT License (https://opensource.org/licenses/MIT)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.
//
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using NP.Utilities;
using System.Collections.Generic;

namespace NP.Avalonia.Visuals.Controls
{
    public class DataGridAttachedProps
    { 
    
    }

    public class AttachedProperties
    {
        #region RowsHeights Attached Avalonia Property
        public static Dictionary<int, GridLength> GetRowsHeights(Control obj)
        {
            return obj.GetValue(RowsHeightsProperty);
        }

        public static void SetRowsHeights(Control obj, Dictionary<int, GridLength> value)
        {
            obj.SetValue(RowsHeightsProperty, value);
        }

        public static readonly AttachedProperty<Dictionary<int, GridLength>> RowsHeightsProperty =
            AvaloniaProperty.RegisterAttached<AttachedProperties, Control, Dictionary<int, GridLength>>
            (
                "RowsHeights"
            );
        #endregion RowsHeights Attached Avalonia Property


        #region ColumnsWidths Attached Avalonia Property
        public static Dictionary<int, GridLength> GetColumnsWidths(Control obj)
        {
            return obj.GetValue(ColumnsWidthsProperty);
        }

        public static void SetColumnsWidths(Control obj, Dictionary<int, GridLength> value)
        {
            obj.SetValue(ColumnsWidthsProperty, value);
        }

        public static readonly AttachedProperty<Dictionary<int, GridLength>> ColumnsWidthsProperty =
            AvaloniaProperty.RegisterAttached<AttachedProperties, Control, Dictionary<int, GridLength>>
            (
                "ColumnsWidths"
            );
        #endregion ColumnsWidths Attached Avalonia Property


        #region MouseOverBrush Attached Avalonia Property
        public static IBrush GetMouseOverBrush(AvaloniaObject obj)
        {
            return obj.GetValue(MouseOverBrushProperty);
        }

        public static void SetMouseOverBrush(AvaloniaObject obj, IBrush value)
        {
            obj.SetValue(MouseOverBrushProperty, value);
        }

        public static readonly AttachedProperty<IBrush> MouseOverBrushProperty =
            AvaloniaProperty.RegisterAttached<AttachedProperties, Control, IBrush>
            (
                "MouseOverBrush"
            );
        #endregion MouseOverBrush Attached Avalonia Property


        #region RealBackground Attached Avalonia Property
        public static IBrush GetRealBackground(AvaloniaObject obj)
        {
            return obj.GetValue(RealBackgroundProperty);
        }

        public static void SetRealBackground(AvaloniaObject obj, IBrush value)
        {
            obj.SetValue(RealBackgroundProperty, value);
        }

        public static readonly AttachedProperty<IBrush> RealBackgroundProperty =
            AvaloniaProperty.RegisterAttached<AttachedProperties, Control, IBrush>
            (
                "RealBackground"
            );
        #endregion RealBackground Attached Avalonia Property


        #region IconData Attached Avalonia Property
        public static Geometry GetIconData(AvaloniaObject obj)
        {
            return obj.GetValue(IconDataProperty);
        }

        public static void SetIconData(AvaloniaObject obj, Geometry value)
        {
            obj.SetValue(IconDataProperty, value);
        }

        public static readonly AttachedProperty<Geometry> IconDataProperty =
            AvaloniaProperty.RegisterAttached<AttachedProperties, Control, Geometry>
            (
                "IconData"
            );
        #endregion IconData Attached Avalonia Property


        #region IconMargin Attached Avalonia Property
        public static Thickness GetIconMargin(AvaloniaObject obj)
        {
            return obj.GetValue(IconMarginProperty);
        }

        public static void SetIconMargin(AvaloniaObject obj, Thickness value)
        {
            obj.SetValue(IconMarginProperty, value);
        }

        public static readonly AttachedProperty<Thickness> IconMarginProperty =
            AvaloniaProperty.RegisterAttached<AttachedProperties, Control, Thickness>
            (
                "IconMargin"
            );
        #endregion IconMargin Attached Avalonia Property


        #region IconWidth Attached Avalonia Property
        public static double GetIconWidth(AvaloniaObject obj)
        {
            return obj.GetValue(IconWidthProperty);
        }

        public static void SetIconWidth(AvaloniaObject obj, double value)
        {
            obj.SetValue(IconWidthProperty, value);
        }

        public static readonly AttachedProperty<double> IconWidthProperty =
            AvaloniaProperty.RegisterAttached<AttachedProperties, Control, double>
            (
                "IconWidth",
                double.NaN
            );
        #endregion IconWidth Attached Avalonia Property


        #region IconHeight Attached Avalonia Property
        public static double GetIconHeight(AvaloniaObject obj)
        {
            return obj.GetValue(IconHeightProperty);
        }

        public static void SetIconHeight(AvaloniaObject obj, double value)
        {
            obj.SetValue(IconHeightProperty, value);
        }

        public static readonly AttachedProperty<double> IconHeightProperty =
            AvaloniaProperty.RegisterAttached<AttachedProperties, Control, double>
            (
                "IconHeight",
                double.NaN
            );
        #endregion IconHeight Attached Avalonia Property


        #region IconStretch Attached Avalonia Property
        public static Stretch GetIconStretch(AvaloniaObject obj)
        {
            return obj.GetValue(IconStretchProperty);
        }

        public static void SetIconStretch(AvaloniaObject obj, Stretch value)
        {
            obj.SetValue(IconStretchProperty, value);
        }

        public static readonly AttachedProperty<Stretch> IconStretchProperty =
            AvaloniaProperty.RegisterAttached<AttachedProperties, Control, Stretch>
            (
                "IconStretch",
                Stretch.Uniform
            );
        #endregion IconStretch Attached Avalonia Property


        #region IconHorizontalAlignment Attached Avalonia Property
        public static HorizontalAlignment GetIconHorizontalAlignment(Control obj)
        {
            return obj.GetValue(IconHorizontalAlignmentProperty);
        }

        public static void SetIconHorizontalAlignment(Control obj, HorizontalAlignment value)
        {
            obj.SetValue(IconHorizontalAlignmentProperty, value);
        }

        public static readonly AttachedProperty<HorizontalAlignment> IconHorizontalAlignmentProperty =
            AvaloniaProperty.RegisterAttached<AttachedProperties, Control, HorizontalAlignment>
            (
                "IconHorizontalAlignment",
                HorizontalAlignment.Center
            );
        #endregion IconHorizontalAlignment Attached Avalonia Property


        #region IconVerticalAlignment Attached Avalonia Property
        public static VerticalAlignment GetIconVerticalAlignment(AvaloniaObject obj)
        {
            return obj.GetValue(IconVerticalAlignmentProperty);
        }

        public static void SetIconVerticalAlignment(AvaloniaObject obj, VerticalAlignment value)
        {
            obj.SetValue(IconVerticalAlignmentProperty, value);
        }

        public static readonly AttachedProperty<VerticalAlignment> IconVerticalAlignmentProperty =
            AvaloniaProperty.RegisterAttached<AttachedProperties, Control, VerticalAlignment>
            (
                "IconVerticalAlignment",
                VerticalAlignment.Center
            );
        #endregion IconVerticalAlignment Attached Avalonia Property


        #region IconRow Attached Avalonia Property
        public static int GetIconRow(Control obj)
        {
            return obj.GetValue(IconRowProperty);
        }

        public static void SetIconRow(Control obj, int value)
        {
            obj.SetValue(IconRowProperty, value);
        }

        public static readonly AttachedProperty<int> IconRowProperty =
            AvaloniaProperty.RegisterAttached<AttachedProperties, Control, int>
            (
                "IconRow"
            );
        #endregion IconRow Attached Avalonia Property


        #region IconColumn Attached Avalonia Property
        public static int GetIconColumn(Control obj)
        {
            return obj.GetValue(IconColumnProperty);
        }

        public static void SetIconColumn(Control obj, int value)
        {
            obj.SetValue(IconColumnProperty, value);
        }

        public static readonly AttachedProperty<int> IconColumnProperty =
            AvaloniaProperty.RegisterAttached<AttachedProperties, Control, int>
            (
                "IconColumn"
            );
        #endregion IconColumn Attached Avalonia Property


        #region CurrentScreenPoint Attached Avalonia Property
        public static Point2D GetCurrentScreenPoint(AvaloniaObject obj)
        {
            return obj.GetValue(CurrentScreenPointProperty);
        }

        public static void SetCurrentScreenPoint(AvaloniaObject obj, Point2D value)
        {
            obj.SetValue(CurrentScreenPointProperty, value);
        }

        public static readonly AttachedProperty<Point2D> CurrentScreenPointProperty =
            AvaloniaProperty.RegisterAttached<AttachedProperties, Control, Point2D>
            (
                "CurrentScreenPoint"
            );
        #endregion CurrentScreenPoint Attached Avalonia Property


        #region HasVisibleLogicalChildren Attached Avalonia Property
        public static bool GetHasVisibleLogicalChildren(AvaloniaObject obj)
        {
            return obj.GetValue(HasVisibleLogicalChildrenProperty);
        }

        internal static void SetHasVisibleLogicalChildren(AvaloniaObject obj, bool value)
        {
            obj.SetValue(HasVisibleLogicalChildrenProperty, value);
        }

        public static readonly AttachedProperty<bool> HasVisibleLogicalChildrenProperty =
            AvaloniaProperty.RegisterAttached<AttachedProperties, Control, bool>
            (
                "HasVisibleLogicalChildren"
            );
        #endregion HasVisibleLogicalChildren Attached Avalonia Property


        #region PrimaryIconData Attached Avalonia Property
        public static Geometry GetPrimaryIconData(Control obj)
        {
            return obj.GetValue(PrimaryIconDataProperty);
        }

        public static void SetPrimaryIconData(Control obj, Geometry value)
        {
            obj.SetValue(PrimaryIconDataProperty, value);
        }

        public static readonly AttachedProperty<Geometry> PrimaryIconDataProperty =
            AvaloniaProperty.RegisterAttached<AttachedProperties, Control, Geometry>
            (
                "PrimaryIconData"
            );
        #endregion PrimaryIconData Attached Avalonia Property

        #region AlternateIconData Attached Avalonia Property
        public static Geometry GetAlternateIconData(Control obj)
        {
            return obj.GetValue(AlternateIconDataProperty);
        }

        public static void SetAlternateIconData(Control obj, Geometry value)
        {
            obj.SetValue(AlternateIconDataProperty, value);
        }

        public static readonly AttachedProperty<Geometry> AlternateIconDataProperty =
            AvaloniaProperty.RegisterAttached<AttachedProperties, Control, Geometry>
            (
                "AlternateIconData"
            );
        #endregion AlternateIconData Attached Avalonia Property

        #region UriString Attached Avalonia Property
        public static string GetUriString(AvaloniaObject obj)
        {
            return obj.GetValue(UriStringProperty);
        }

        public static void SetUriString(AvaloniaObject obj, string value)
        {
            obj.SetValue(UriStringProperty, value);
        }

        public static readonly AttachedProperty<string> UriStringProperty =
            AvaloniaProperty.RegisterAttached<AttachedProperties, Control, string>
            (
                "UriString"
            );
        #endregion UriString Attached Avalonia Property


        #region Text Attached Avalonia Property
        public static string GetText(Control obj)
        {
            return obj.GetValue(TextProperty);
        }

        public static void SetText(Control obj, string value)
        {
            obj.SetValue(TextProperty, value);
        }

        public static readonly AttachedProperty<string> TextProperty =
            AvaloniaProperty.RegisterAttached<AttachedProperties, Control, string>
            (
                "Text"
            );
        #endregion Text Attached Avalonia Property


        #region TheTextWrapping Attached Avalonia Property
        public static TextWrapping GetTheTextWrapping(Control obj)
        {
            return obj.GetValue(TheTextWrappingProperty);
        }

        public static void SetTheTextWrapping(Control obj, TextWrapping value)
        {
            obj.SetValue(TheTextWrappingProperty, value);
        }

        public static readonly AttachedProperty<TextWrapping> TheTextWrappingProperty =
            AvaloniaProperty.RegisterAttached<AttachedProperties, Control, TextWrapping>
            (
                "TheTextWrapping"
            );
        #endregion TheTextWrapping Attached Avalonia Property


        #region TheTextAlignment Attached Avalonia Property
        public static TextAlignment GetTheTextAlignment(Control obj)
        {
            return obj.GetValue(TheTextAlignmentProperty);
        }

        public static void SetTheTextAlignment(Control obj, TextAlignment value)
        {
            obj.SetValue(TheTextAlignmentProperty, value);
        }

        public static readonly AttachedProperty<TextAlignment> TheTextAlignmentProperty =
            AvaloniaProperty.RegisterAttached<AttachedProperties, Control, TextAlignment>
            (
                "TheTextAlignment"
            );
        #endregion TheTextAlignment Attached Avalonia Property


        #region TheTextTrimming Attached Avalonia Property
        public static TextTrimming GetTheTextTrimming(Control obj)
        {
            return obj.GetValue(TheTextTrimmingProperty);
        }

        public static void SetTheTextTrimming(Control obj, TextTrimming value)
        {
            obj.SetValue(TheTextTrimmingProperty, value);
        }

        public static readonly AttachedProperty<TextTrimming> TheTextTrimmingProperty =
            AvaloniaProperty.RegisterAttached<AttachedProperties, Control, TextTrimming>
            (
                "TheTextTrimming"
            );
        #endregion TheTextTrimming Attached Avalonia Property


        #region TextHorizontalAlignment Attached Avalonia Property
        public static HorizontalAlignment GetTextHorizontalAlignment(Control obj)
        {
            return obj.GetValue(TextHorizontalAlignmentProperty);
        }

        public static void SetTextHorizontalAlignment(Control obj, HorizontalAlignment value)
        {
            obj.SetValue(TextHorizontalAlignmentProperty, value);
        }

        public static readonly AttachedProperty<HorizontalAlignment> TextHorizontalAlignmentProperty =
            AvaloniaProperty.RegisterAttached<AttachedProperties, Control, HorizontalAlignment>
            (
                "TextHorizontalAlignment",
                HorizontalAlignment.Center
            );
        #endregion TextHorizontalAlignment Attached Avalonia Property


        #region TextVerticalAlignment Attached Avalonia Property
        public static VerticalAlignment GetTextVerticalAlignment(Control obj)
        {
            return obj.GetValue(TextVerticalAlignmentProperty);
        }

        public static void SetTextVerticalAlignment(Control obj, VerticalAlignment value)
        {
            obj.SetValue(TextVerticalAlignmentProperty, value);
        }

        public static readonly AttachedProperty<VerticalAlignment> TextVerticalAlignmentProperty =
            AvaloniaProperty.RegisterAttached<AttachedProperties, Control, VerticalAlignment>
            (
                "TextVerticalAlignment",
                VerticalAlignment.Center
            );
        #endregion TextVerticalAlignment Attached Avalonia Property


        #region MainPartTemplate Attached Avalonia Property
        public static ControlTemplate GetMainPartTemplate(Control obj)
        {
            return obj.GetValue(MainPartTemplateProperty);
        }

        public static void SetMainPartTemplate(Control obj, ControlTemplate value)
        {
            obj.SetValue(MainPartTemplateProperty, value);
        }

        public static readonly AttachedProperty<ControlTemplate> MainPartTemplateProperty =
            AvaloniaProperty.RegisterAttached<AttachedProperties, Control, ControlTemplate>
            (
                "MainPartTemplate"
            );
        #endregion MainPartTemplate Attached Avalonia Property
    }
}
