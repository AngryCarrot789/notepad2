﻿using Notepad2.Preferences;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Forms = System.Windows.Forms;

namespace Notepad2.AttachedProperties
{
    /// <summary>
    /// A class for allowing horizontal scrolling on any control that has a scrollviewer 
    /// </summary>
    public static class HorizontalScrolling
    {
        public static readonly DependencyProperty UseHorizontalScrollingProperty =
            DependencyProperty.RegisterAttached(
                "UseHorizontalScrolling",
                typeof(bool),
                typeof(HorizontalScrolling),
                new PropertyMetadata(
                    new PropertyChangedCallback(OnHorizontalScrollingValueChanged)));

        public static readonly DependencyProperty ForceHorizontalScrollingProperty =
            DependencyProperty.RegisterAttached(
                "ForceHorizontalScrolling",
                typeof(bool),
                typeof(HorizontalScrolling),
                new PropertyMetadata(
                    new PropertyChangedCallback(OnHorizontalScrollingValueChanged)));

        public static readonly DependencyProperty HorizontalScrollingAmountProperty =
            DependencyProperty.RegisterAttached(
                "HorizontalScrollingAmount",
                typeof(int),
                typeof(HorizontalScrolling),
                new PropertyMetadata(Forms.SystemInformation.MouseWheelScrollLines));

        public static bool GetUseHorizontalScrollingValue(DependencyObject d)
        {
            return (bool)d.GetValue(UseHorizontalScrollingProperty);
        }

        public static void SetUseHorizontalScrollingValue(DependencyObject d, bool value)
        {
            d.SetValue(UseHorizontalScrollingProperty, value);
        }

        public static bool GetForceHorizontalScrollingValue(DependencyObject d)
        {
            return (bool)d.GetValue(ForceHorizontalScrollingProperty);
        }

        public static void SetForceHorizontalScrollingValue(DependencyObject d, bool value)
        {
            d.SetValue(ForceHorizontalScrollingProperty, value);
        }

        public static int GetHorizontalScrollingAmountValue(DependencyObject d)
        {
            return (int)d.GetValue(HorizontalScrollingAmountProperty);
        }

        public static void SetHorizontalScrollingAmountValue(DependencyObject d, int value)
        {
            d.SetValue(HorizontalScrollingAmountProperty, value);
        }

        public static void OnHorizontalScrollingValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is UIElement element)
            {
                element.PreviewMouseWheel -= OnPreviewMouseWheel;
                element.PreviewMouseWheel += OnPreviewMouseWheel;
            }

            else throw new Exception("Attached property must be used with UIElement.");
        }

        private static void OnPreviewMouseWheel(object sender, MouseWheelEventArgs args)
        {
            ScrollViewer scrollViewer = null;
            bool forceHorizontalScrolling;
            int horizontalScrollingAmount;

            if (!PreferencesG.SCROLL_HORIZONTAL_WITH_SHIFT_MOUSEWHEEL)
                return;

            if (sender is UIElement element)
            {
                DependencyObject senderDp = sender as DependencyObject;
                scrollViewer = FindDescendant<ScrollViewer>(element);
                forceHorizontalScrolling = GetForceHorizontalScrollingValue(senderDp);
                horizontalScrollingAmount = GetHorizontalScrollingAmountValue(senderDp);
                if (horizontalScrollingAmount < 1)
                    horizontalScrollingAmount = 3;

                if (!forceHorizontalScrolling && (Keyboard.Modifiers == ModifierKeys.Shift || Mouse.MiddleButton == MouseButtonState.Pressed))
                {
                    if (scrollViewer == null)
                        return;

                    if (args.Delta < 0)
                        for (int i = 0; i < horizontalScrollingAmount; i++)
                            scrollViewer.LineRight();
                    else
                        for (int i = 0; i < horizontalScrollingAmount; i++)
                            scrollViewer.LineLeft();

                    args.Handled = true;
                }

                else if (forceHorizontalScrolling)
                {
                    if (scrollViewer == null)
                        return;

                    if (args.Delta < 0)
                        for (int i = 0; i < horizontalScrollingAmount; i++)
                            scrollViewer.LineRight();
                    else
                        for (int i = 0; i < horizontalScrollingAmount; i++)
                            scrollViewer.LineLeft();

                    args.Handled = true;
                }
            }
        }

        private static T FindDescendant<T>(DependencyObject d) where T : DependencyObject
        {
            if (d == null)
                return null;

            int childCount = VisualTreeHelper.GetChildrenCount(d);

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
