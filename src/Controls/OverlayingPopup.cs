using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;

namespace NP.Avalonia.Visuals.Controls
{
    public class OverlayingPopup : Popup
    {
        #region Content Styled Avalonia Property
        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly StyledProperty<object> ContentProperty =
            AvaloniaProperty.Register<OverlayingPopup, object>
            (
                nameof(Content)
            );
        #endregion Content Styled Avalonia Property


        #region ContentTemplate Styled Avalonia Property
        public IDataTemplate ContentTemplate
        {
            get { return GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        public static readonly StyledProperty<IDataTemplate> ContentTemplateProperty =
            AvaloniaProperty.Register<OverlayingPopup, IDataTemplate>
            (
                nameof(ContentTemplate)
            );
        #endregion ContentTemplate Styled Avalonia Property


        #region HorizontalContentAlignment Styled Avalonia Property
        public HorizontalAlignment HorizontalContentAlignment
        {
            get { return GetValue(HorizontalContentAlignmentProperty); }
            set { SetValue(HorizontalContentAlignmentProperty, value); }
        }

        public static readonly StyledProperty<HorizontalAlignment> HorizontalContentAlignmentProperty =
            AvaloniaProperty.Register<OverlayingPopup, HorizontalAlignment>
            (
                nameof(HorizontalContentAlignment)
            );
        #endregion HorizontalContentAlignment Styled Avalonia Property


        #region VerticalContentAlignment Styled Avalonia Property
        public VerticalAlignment VerticalContentAlignment
        {
            get { return GetValue(VerticalContentAlignmentProperty); }
            set { SetValue(VerticalContentAlignmentProperty, value); }
        }

        public static readonly StyledProperty<VerticalAlignment> VerticalContentAlignmentProperty =
            AvaloniaProperty.Register<OverlayingPopup, VerticalAlignment>
            (
                nameof(VerticalContentAlignment)
            );
        #endregion VerticalContentAlignment Styled Avalonia Property


        #region Background Styled Avalonia Property
        public IBrush Background
        {
            get { return GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        public static readonly StyledProperty<IBrush> BackgroundProperty =
            AvaloniaProperty.Register<OverlayingPopup, IBrush>
            (
                nameof(Background)
            );
        #endregion Background Styled Avalonia Property


        #region Padding Styled Avalonia Property
        public Thickness Padding
        {
            get { return GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        public static readonly StyledProperty<Thickness> PaddingProperty =
            AvaloniaProperty.Register<OverlayingPopup, Thickness>
            (
                nameof(Padding)
            );
        #endregion Padding Styled Avalonia Property
    }
}
