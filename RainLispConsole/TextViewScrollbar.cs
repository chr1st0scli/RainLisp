using Terminal.Gui;

namespace RainLispConsole
{
    // This code is based on the official example given on how to add scrollbars to a text view.
    // See UICatalog, Editor scenario at Terminal GUI project.
    internal class TextViewScrollbar : ScrollBarView
    {
        private readonly TextView _textView;

        public TextViewScrollbar(TextView textView) : base(textView, true)
        {
            _textView = textView ?? throw new ArgumentNullException(nameof(textView));

            ChangedPosition += ScrollbarChangedPosition;
            OtherScrollBarView.ChangedPosition += OtherScrollbarChangedPosition;
            VisibleChanged += ScrollbarVisibleChanged;
            OtherScrollBarView.VisibleChanged += OtherScrollbarVisibleChanged;

            textView.DrawContent += TextViewDrawContent;
        }

        private void ScrollbarChangedPosition()
        {
            _textView.TopRow = Position;
            if (_textView.TopRow != Position)
            {
                Position = _textView.TopRow;
            }
            _textView.SetNeedsDisplay();
        }

        private void OtherScrollbarChangedPosition()
        {
            _textView.LeftColumn = OtherScrollBarView.Position;
            if (_textView.LeftColumn != OtherScrollBarView.Position)
            {
                OtherScrollBarView.Position = _textView.LeftColumn;
            }
            _textView.SetNeedsDisplay();
        }

        private void ScrollbarVisibleChanged()
        {
            if (Visible && _textView.RightOffset == 0)
            {
                _textView.RightOffset = 1;
            }
            else if (!Visible && _textView.RightOffset == 1)
            {
                _textView.RightOffset = 0;
            }
        }

        private void OtherScrollbarVisibleChanged()
        {
            if (OtherScrollBarView.Visible && _textView.BottomOffset == 0)
            {
                _textView.BottomOffset = 1;
            }
            else if (!OtherScrollBarView.Visible && _textView.BottomOffset == 1)
            {
                _textView.BottomOffset = 0;
            }
        }

        private void TextViewDrawContent(Rect obj)
        {
            Size = _textView.Lines;
            Position = _textView.TopRow;
            if (OtherScrollBarView != null)
            {
                OtherScrollBarView.Size = _textView.Maxlength;
                OtherScrollBarView.Position = _textView.LeftColumn;
            }
            LayoutSubviews();
            Refresh();
        }
    }
}
