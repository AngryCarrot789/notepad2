﻿using Notepad2.Preferences;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Control = System.Windows.Controls.Control;

namespace Notepad2.Utilities
{
    public static class ScrollViewerHelper
    {
        public static readonly DependencyProperty ShiftWheelScrollsHorizontallyProperty
            = DependencyProperty.RegisterAttached("ShiftWheelScrollsHorizontally",
                typeof(bool),
                typeof(ScrollViewerHelper),
                new PropertyMetadata(
                    false,
                    UseHorizontalScrollingChangedCallback));

        private static void UseHorizontalScrollingChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = d as UIElement;

            if (element == null)
                throw new Exception("Attached property must be used with UIElement.");

            if ((bool)e.NewValue)
            {
                element.PreviewMouseWheel += OnPreviewMouseWheel;
            }
            else
            {
                element.PreviewMouseWheel -= OnPreviewMouseWheel;
            }
        }

        private static void OnPreviewMouseWheel(object sender, MouseWheelEventArgs args)
        {
            if (Keyboard.Modifiers != ModifierKeys.Shift)
                return;

            if (!PreferencesG.SCROLL_HORIZONTAL_WITH_SHIFT_MOUSEWHEEL)
                return;

            var scrollViewer = ((UIElement)sender).FindDescendant<ScrollViewer>();

            if (scrollViewer == null)
                return;

            if (args.Delta < 0)
                for (int i = 1; i <= SystemInformation.MouseWheelScrollLines; i++)
                    scrollViewer.LineRight();
            else
                for (int i = 1; i <= SystemInformation.MouseWheelScrollLines; i++)
                    scrollViewer.LineLeft();
            args.Handled = true;
        }

        public static void SetShiftWheelScrollsHorizontally(Control element, bool value)
            => element.SetValue(ShiftWheelScrollsHorizontallyProperty, value);
        public static bool GetShiftWheelScrollsHorizontally(Control element)
            => (bool)element.GetValue(ShiftWheelScrollsHorizontallyProperty);

        private static T FindDescendant<T>(this DependencyObject d) where T : DependencyObject
        {
            if (d == null)
                return null;

            var childCount = VisualTreeHelper.GetChildrenCount(d);

            for (var i = 0; i < childCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(d, i);

                T result = child as T ?? FindDescendant<T>(child);

                if (result != null)
                    return result;
            }

            return null;
        }
    }
}
