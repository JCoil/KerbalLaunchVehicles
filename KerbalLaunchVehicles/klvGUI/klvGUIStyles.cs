using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerbalLaunchVehicles.klvGUI
{
    internal static class klvGUIStyles
    {
        private static GUIStyle _tabButton;
        private static GUIStyle _tabButtonActive;
        private static GUIStyle _tabButtonDisabled;
        private static GUIStyle _standardButton;
        private static GUIStyle _standardLabel;
        private static GUIStyle _headerLabel;
        private static GUIStyle _titleLabel;
        private static GUIStyle _panelLabel;
        private static GUIStyle _warningButton;
        private static GUIStyle _warningLabel;
        private static GUIStyle _disabledButton;
        private static GUIStyle _correctButton;
        private static GUIStyle _standardTextBox;
        private static GUIStyle _freeTextBox;
        private static GUIStyle _highlightHeader;
        private static GUIStyle _highlightBody;
        private static GUIStyle _highlightBox;
        private static GUIStyle _standardWindow;
        private static GUIStyle _popupWindow;
        private static GUIStyle _divider;

        private static Color colorHighlight = new Color(0.75f, 0.75f, 0.75f);
        private static Color colorActive = new Color(0.65f, 0.65f, 0.75f);
        private static Color colorWarning = Color.yellow;
        private static Color colorDisabled = Color.gray;
        private static Color colorCorrect = Color.green;

        private static Color colorTab = new Color(0.8f, 0.8f, 0.8f);
        private static Color colorTabActive = new Color(1f, 1f, 1f);
        private static Color colorTabDisabled = new Color(0.4f, 0.4f, 0.4f);

        internal static List<GUIStyle> AllStyles;

        internal static void Initialise()
        {
            // Initialise and register all styles
            AllStyles = new List<GUIStyle>();
            AllStyles.Add(TabButton);
            AllStyles.Add(TabButtonActive);
            AllStyles.Add(TabButtonDisabled);
            AllStyles.Add(StandardButton);
            AllStyles.Add(StandardLabel) ;
            AllStyles.Add(HeaderLabel);
            AllStyles.Add(TitleLabel);
            AllStyles.Add(PanelLabel);
            AllStyles.Add(WarningButton);
            AllStyles.Add(WarningLabel);
            AllStyles.Add(DisabledButton);
            AllStyles.Add(CorrectButton);
            AllStyles.Add(StandardTextBox);
            AllStyles.Add(FreeTextBox);
            AllStyles.Add(HighlightHeader);
            AllStyles.Add(HighlightBody);
            AllStyles.Add(HighlightBox);
            AllStyles.Add(PopupWindow);

            AdjustFontSize(initialFontSize);
            initialFontSize = 0;
        }


        //Tab Styles
        public static GUIStyle TabButton
        {
            get
            {
                if (_tabButton == null)
                {
                    _tabButton = new GUIStyle(GUI.skin.button);
                    _tabButton.normal.textColor = colorTab;
                    _tabButton.fontSize = 14;
                }
                return _tabButton;
            }
        }
        public static GUIStyle TabButtonActive
        {
            get
            {
                if (_tabButtonActive == null)
                {
                    _tabButtonActive = new GUIStyle(GUI.skin.button);
                    _tabButtonActive.normal.textColor = colorTabActive;
                    _tabButtonActive.fontSize = 14;
                }
                return _tabButtonActive;
            }
        }
        public static GUIStyle TabButtonDisabled
        {
            get
            {
                if (_tabButtonDisabled == null)
                {
                    _tabButtonDisabled = SimpleFilledStyle(GUI.skin.button, colorTabDisabled, colorTabDisabled, colorTabDisabled);
                    _tabButtonDisabled.fontSize = 14;
                }
                return _tabButtonDisabled;
            }
        }


        //Label Styles

        public static GUIStyle StandardLabel
        {
            get
            {
                if (_standardLabel == null)
                {
                    _standardLabel = SimpleLabelStyle(GUI.skin.label, Color.white, Color.white, Color.white);
                    _standardLabel.alignment = TextAnchor.MiddleLeft;
                    _standardLabel.fontSize = 14;
                }
                return _standardLabel;
            }
        }

        public static GUIStyle TitleLabel
        {
            get
            {
                if (_titleLabel == null)
                {
                    _titleLabel = SimpleLabelStyle(GUI.skin.label, Color.white, Color.white, Color.white);
                    _titleLabel.alignment = TextAnchor.MiddleLeft;
                    _titleLabel.fontSize = 18;
                }
                return _titleLabel;
            }
        }

        public static GUIStyle HeaderLabel
        {
            get
            {
                if (_headerLabel == null)
                {
                    _headerLabel = SimpleLabelStyle(GUI.skin.label, Color.white, Color.white, Color.white);
                    _headerLabel.alignment = TextAnchor.MiddleLeft;
                    _headerLabel.fontSize = 16;
                }
                return _headerLabel;
            }
        }

        public static GUIStyle WarningLabel
        {
            get
            {
                if (_warningLabel == null)
                {
                    _warningLabel = SimpleLabelStyle(GUI.skin.label, colorWarning, colorWarning, colorWarning);
                    _warningLabel.alignment = TextAnchor.MiddleLeft;
                    _warningLabel.fontSize = 14;
                }
                return _warningLabel;
            }
        }

        public static GUIStyle PanelLabel
        {
            get
            {
                if (_panelLabel == null)
                {
                    _panelLabel = SimpleLabelStyle(GUI.skin.label, Color.white, Color.white, Color.white);
                    _panelLabel.alignment = TextAnchor.UpperLeft;
                    _panelLabel.fontSize = 13;
                }
                return _panelLabel;
            }
        }

        //Button Styles

        public static GUIStyle StandardButton
        {
            get
            {
                if (_standardButton == null)
                {
                    _standardButton = new GUIStyle(GUI.skin.button);
                    _standardButton.fontSize = 14;
                }
                return _standardButton;
            }
        }

        public static GUIStyle WarningButton
        {
            get
            {
                if (_warningButton == null)
                {
                    _warningButton = new GUIStyle(GUI.skin.button);
                    _warningButton.normal.textColor = colorWarning;
                    _warningButton.fontSize = 14;
                }
                return _warningButton;
            }
        }

        public static GUIStyle DisabledButton
        {
            get
            {
                if (_disabledButton == null)
                {
                    _disabledButton = SimpleFilledStyle(GUI.skin.button, colorTabDisabled, colorTabDisabled, colorTabDisabled);
                    _disabledButton.fontSize = 14;
                }
                return _disabledButton;
            }
        }

        public static GUIStyle CorrectButton
        {
            get
            {
                if (_correctButton == null)
                {
                    _correctButton = new GUIStyle(GUI.skin.button);
                    _correctButton.normal.textColor = colorCorrect;
                    _correctButton.fontSize = 14;
                }
                return _correctButton;
            }
        }

        //Textbox Styles

        public static GUIStyle StandardTextBox
        {
            get
            {
                if (_standardTextBox == null)
                {
                    _standardTextBox = new GUIStyle(GUI.skin.textField);
                    _standardTextBox.alignment = TextAnchor.MiddleLeft;
                    _standardTextBox.fontSize = 13;
                }
                return _standardTextBox;
            }
        }

        public static GUIStyle FreeTextBox
        {
            get
            {
                if (_freeTextBox == null)
                {
                    _freeTextBox = new GUIStyle(GUI.skin.textField);
                    _freeTextBox.alignment = TextAnchor.UpperLeft;
                    _freeTextBox.wordWrap = true;
                    _freeTextBox.fontSize = 14;
                }
                return _freeTextBox;
            }
        }

        //Node Styles

        public static GUIStyle HighlightHeader
        {
            get
            {
                if (_highlightHeader == null)
                {
                    _highlightHeader = SimpleLabelStyle(GUI.skin.label, Color.white, colorHighlight, colorActive);
                    _highlightHeader.fontSize = 14;
                }
                return _highlightHeader;
            }
        }

        public static GUIStyle HighlightBody
        {
            get
            {
                if (_highlightBody == null)
                {
                    _highlightBody = SimpleLabelStyle(GUI.skin.label, Color.white, colorHighlight, Color.white);
                    _highlightBody.fontSize = 14;
                }
                return _highlightBody;
            }
        }
        public static GUIStyle HighlightBox
        {
            get
            {
                if (_highlightBox == null)
                {
                    _highlightBox = SimpleFilledStyle(GUI.skin.box, Color.grey, Color.white, Color.white);
                    _highlightBox.alignment = TextAnchor.MiddleCenter;
                    _highlightBox.fontSize = 18;
                }
                return _highlightBox;
            }
        }

        //Other styles

        public static GUIStyle StandardWindow
        {
            get
            {
                if (_standardWindow == null)
                {
                    _standardWindow = new GUIStyle(GUI.skin.window);
                    _standardWindow.fontSize = 14;
                }
                return _standardWindow;
            }
        }
        public static GUIStyle PopupWindow
        {
            get
            {
                if (_popupWindow == null)
                {
                    _popupWindow = new GUIStyle(GUI.skin.textArea);
                    _popupWindow.fontSize = 14;
                }
                return _popupWindow;
            }
        }

        public static GUIStyle Divider
        {
            get
            {
                if (_divider == null)
                {
                    _divider = new GUIStyle(GUI.skin.box);
                    _divider.border = new RectOffset(0, 0, 0, 0);
                    _divider.fontSize = 0;
                }
                return _divider;
            }
        }

        //Style Factory

        private static GUIStyle SimpleFilledStyle(GUIStyle style, Color cNormal, Color cHover, Color cActive)
        {
            GUIStyle newStyle = new GUIStyle(style);
            newStyle.normal.textColor = cNormal;

            newStyle.hover.textColor = cHover;
            newStyle.hover.background = newStyle.normal.background;
            newStyle.active.textColor = cActive;
            newStyle.active.background = newStyle.normal.background;

            return newStyle;
        }

        private static GUIStyle SimpleLabelStyle(GUIStyle style, Color cNormal, Color cHover, Color cActive)
        {
            GUIStyle newStyle = new GUIStyle(style);
            newStyle.normal.textColor = cNormal;

            Texture2D t = new Texture2D(1, 1);
            t.SetPixel(0, 0, new Color(0, 0, 0, 0));
            t.Apply();

            newStyle.hover.textColor = cHover;
            newStyle.hover.background = t;
            newStyle.active.textColor = cActive;
            newStyle.active.background = t;

            return newStyle;
        }


        // Font sizing
        internal static int fontSizeRelative { get; private set; }
        private static int initialFontSize;
        internal static int fontSizeMax = 4;
        internal static int fontSizeMin = -2;

        internal static void SetInitialFontSize(int size)
        {
            initialFontSize = size.Limit(fontSizeMax, fontSizeMin);
        }

        internal static void AdjustFontSize(int size)
        {
            if (fontSizeRelative + size <= fontSizeMax && fontSizeRelative + size >= fontSizeMin)
            {
                foreach (var style in AllStyles)
                {
                    style.fontSize += size;
                }
                fontSizeRelative += size;
            }
        }
    }
}
