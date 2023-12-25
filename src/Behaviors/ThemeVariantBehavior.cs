using Avalonia;
using NP.Ava.Visuals.ThemingAndL10N;
using System;
using Avalonia.Styling;
using Avalonia.Metadata;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NP.Utilities;
using System.ComponentModel;

namespace NP.Ava.Visuals.Behaviors
{
    public class ThemeVariantBehavior
    {
        #region TheThemeVariant Attached Avalonia Property
        public static ThemeVariant GetTheThemeVariant(AvaloniaObject obj)
        {
            return obj.GetValue(TheThemeVariantProperty);
        }

        public static void SetTheThemeVariant(AvaloniaObject obj, ThemeVariant value)
        {
            obj.SetValue(TheThemeVariantProperty, value);
        }

        public static readonly AttachedProperty<ThemeVariant> TheThemeVariantProperty =
            AvaloniaProperty.RegisterAttached<ThemeVariantBehavior, AvaloniaObject, ThemeVariant>
            (
                "TheThemeVariant"
            );
        #endregion TheThemeVariant Attached Avalonia Property

        private static void OnThemeChanged(AvaloniaPropertyChangedEventArgs<ThemeVariant> eventArgs)
        {
            AvaloniaObject sender = eventArgs.Sender;

            sender.CheckThemeVariantHost();

            sender.SetCurrentValue(ThemeVariant.RequestedThemeVariantProperty, GetTheThemeVariant(sender));
        }


        #region ThemeReference Attached Avalonia Property
        public static ThemeVariantReference GetThemeReference(AvaloniaObject obj)
        {
            return obj.GetValue(ThemeReferenceProperty);
        }

        public static void SetThemeReference(AvaloniaObject obj, ThemeVariantReference value)
        {
            obj.SetValue(ThemeReferenceProperty, value);
        }

        public static readonly AttachedProperty<ThemeVariantReference> ThemeReferenceProperty =
            AvaloniaProperty.RegisterAttached<ThemeVariantBehavior, AvaloniaObject, ThemeVariantReference>
            (
                "ThemeReference"
            );
        #endregion ThemeReference Attached Avalonia Property


        #region TheThemeContainerObj Attached Avalonia Property
        private static ThemeContainerWithThemeReference? GetTheThemeContainerObj(AvaloniaObject obj)
        {
            return obj.GetValue(TheThemeContainerObjProperty);
        }

        private static void SetTheThemeContainerObj(AvaloniaObject obj, ThemeContainerWithThemeReference? value)
        {
            obj.SetValue(TheThemeContainerObjProperty, value);
        }

        private static readonly AttachedProperty<ThemeContainerWithThemeReference?> TheThemeContainerObjProperty =
            AvaloniaProperty.RegisterAttached<ThemeVariantBehavior, AvaloniaObject, ThemeContainerWithThemeReference?>
            (
                "TheThemeContainerObj"
            );
        #endregion TheThemeContainerObj Attached Avalonia Property


        static ThemeVariantBehavior()
        {
            TheThemeVariantProperty.Changed.Subscribe(OnThemeChanged);
            ThemeReferenceProperty.Changed.Subscribe(OnThemeRefChanged);
        }

        private static void OnThemeRefChanged(AvaloniaPropertyChangedEventArgs<ThemeVariantReference> args)
        {
            AvaloniaObject sender = args.Sender;

            sender.CheckThemeVariantHost();

            ThemeContainerWithThemeReference? oldThemeContainer = GetTheThemeContainerObj(sender);

            if (oldThemeContainer != null)
            {
                oldThemeContainer.PropertyChanged -= OnNotified!;
                oldThemeContainer?.Dispose();
            }

            var themeReference = GetThemeReference(sender);

            if (themeReference != null)
            {
                ThemeContainerWithThemeReference themeContainer = new ThemeContainerWithThemeReference(sender, themeReference);
                themeContainer.PropertyChanged += OnNotified!;
                SetTheThemeContainerObj(sender, themeContainer);
                themeContainer.SetTheme();
            }
            else
            {
                SetTheThemeContainerObj(sender, null);
                sender.SetCurrentValue(ThemeVariant.RequestedThemeVariantProperty, null);
            }
        }

        private static void OnNotified(object sender, PropertyChangedEventArgs e)
        {
            ThemeContainerWithThemeReference themeContainer = (ThemeContainerWithThemeReference)sender;

            if (e.PropertyName == nameof(ThemeVariantReference.TheThemeVariant))
            {
                themeContainer.SetTheme();
            }
        }

        internal class ThemeContainerWithThemeReference : INotifyPropertyChanged, IDisposable
        {
            public AvaloniaObject ObjectToApplyThemeTo { get; }

            public ThemeVariantReference ThemeReference { get; }

            public event PropertyChangedEventHandler? PropertyChanged;

            public ThemeContainerWithThemeReference(AvaloniaObject objToApplyThemeTo, ThemeVariantReference themeVariantReference)
            {
                ObjectToApplyThemeTo = objToApplyThemeTo;

                ThemeReference = themeVariantReference;

                ThemeReference.PropertyChanged += ThemeReference_PropertyChanged;
            }

            private void ThemeReference_PropertyChanged(object? sender, PropertyChangedEventArgs e)
            {
                PropertyChanged?.Invoke(this, e);
            }

            public void Dispose()
            {
                ThemeReference.PropertyChanged += ThemeReference_PropertyChanged;
            }

            public void SetTheme()
            {
                ObjectToApplyThemeTo.SetCurrentValue(ThemeVariant.RequestedThemeVariantProperty, ThemeReference.TheThemeVariant);
            }
        }
    }

    static class ThemeVariantBehaviorHelper
    {
        internal static void CheckThemeVariantHost(this AvaloniaObject sender)
        {
            if (sender is not IThemeVariantHost)
            {
                throw new Exception("Coding ERROR: ThemeVariantBehavior.TheThemeVariant attached property can only be set on an IThemeVariantHost");
            }
        }
    }
}
