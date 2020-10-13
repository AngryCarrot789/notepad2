﻿using Notepad2.CClipboard;
using Notepad2.Finding;
using Notepad2.Finding.TextFinding;
using Notepad2.Preferences;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;

namespace Notepad2.SimpleEditor
{
    public class TextEditor : TextBox
    {
        public TextEditor()
        {

        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (Keyboard.IsKeyDown(Key.LeftShift))
                {
                    switch (e.Key)
                    {
                        case Key.A:
                            if (!PreferencesG.CAN_SELECT_ENTIRE_LINE_CTRL_SHIFT_A)
                                break;

                            SelectEntireCurrentLine();
                            break;

                        case Key.Delete:
                            SelectEntireCurrentLine();
                            SelectedText = "";
                            break;

                        case Key.Enter:
                            if (!PreferencesG.CAN_ADD_ENTIRE_LINES)
                                break;
                            AddNewLineBelowCurrentLine();
                            break;
                    }
                }
                else
                {
                    switch (e.Key)
                    {
                        case Key.Enter:
                            if (!PreferencesG.CAN_ADD_ENTIRE_LINES)
                                break;
                            AddNewLineAndMoveCaret();
                            break;
                        case Key.X:
                            if (!PreferencesG.CAN_CUT_ENTIRE_LINE_CTRL_X)
                                break;
                            if (SelectedText == "")
                            {
                                SelectEntireCurrentLine();
                                // not altering e.Handled lets the
                                // textbox base cut the line automatically.
                            }
                            break;
                        case Key.C:
                            if (!PreferencesG.CAN_COPY_ENTIRE_LINE_CTRL_C)
                                break;
                            if (SelectedText == "")
                            {
                                CopyEntireCurrentLine();
                                e.Handled = true;
                            }
                            break;

                        case Key.Up:
                            if (!PreferencesG.SCROLL_VERTICAL_WITH_CTRL_ARROWKEYS)
                                break;
                            LineUp(); e.Handled = true;
                            break;
                        case Key.Down:
                            if (!PreferencesG.SCROLL_VERTICAL_WITH_CTRL_ARROWKEYS)
                                break;
                            LineDown(); e.Handled = true;
                            break;
                        case Key.Left:
                            if (!PreferencesG.SCROLL_HORIZONTAL_WITH_CTRL_ARROWKEYS)
                                break;
                            LineLeft(); e.Handled = true;
                            break;
                        case Key.Right:
                            if (!PreferencesG.SCROLL_HORIZONTAL_WITH_CTRL_ARROWKEYS)
                                break;
                            LineRight(); e.Handled = true;
                            break;
                    }
                }
            }
            // Cant do because holding alt apparently blocks the keydown events...
            //if (Keyboard.Modifiers == ModifierKeys.Alt)
            //{
            //    switch (e.Key)
            //    {
            //        case Key.Left: LineLeft(); e.Handled = true; break;
            //        case Key.Right: LineRight(); e.Handled = true; break;
            //    }
            //}
        }

        public void InsertText(string text, int charIndex, bool setCaretAfterText = false)
        {
            SelectionStart = charIndex;
            SelectedText = text;
            SelectionLength = 0;
            if (setCaretAfterText)
                CaretIndex += text.Length;
        }

        public void InsetNewLineAtCaret()
        {
            InsertText("\n", CaretIndex, true);
        }

        public void AddNewLineAboveCurrentLine()
        {
            int startOfNewLineIndex = Text.LastIndexOf(Environment.NewLine, CaretIndex) + 1;
            SelectionStart = startOfNewLineIndex;
            SelectedText = Environment.NewLine;
            SelectionLength = 0;
            CaretIndex += 2;
        }

        /// <summary>
        /// 
        /// </summary>
        public void AddNewLineBelowCurrentLine()
        {
            int startOfNewLineIndex = Text.LastIndexOf(Environment.NewLine, CaretIndex) + 1;
            SelectionStart = startOfNewLineIndex;
            int prevCaretIndex = CaretIndex;
            SelectedText = Environment.NewLine;
            SelectionLength = 0;
            CaretIndex = prevCaretIndex;
        }

        /// <summary>
        /// Set the caret index to the start of the new line 
        /// and then inser a NewLine character
        /// </summary>
        public void AddNewLineAndMoveCaret()
        {
            int startOfNewLineIndex = Text.LastIndexOf(Environment.NewLine, CaretIndex) + 1;
            int lineIndex = base.GetLineIndexFromCharacterIndex(CaretIndex);
            int lineLength = base.GetLineLength(lineIndex);
            CaretIndex = startOfNewLineIndex + lineLength + 1;
            SelectionStart = CaretIndex;
            SelectedText = Environment.NewLine;
            SelectionLength = 0;
            if (CaretIndex > 1)
                CaretIndex -= 1;
        }

        public void CopyEntireCurrentLine()
        {
            SelectEntireCurrentLine();
            string newLineText = SelectedText;
            CaretIndex = Text.LastIndexOf(Environment.NewLine, CaretIndex) + 1;
            CustomClipboard.SetTextObject(newLineText);
        }

        public void SelectEntireCurrentLine()
        {
            int start = Text.LastIndexOf(Environment.NewLine, CaretIndex) + 1;
            int lineIdx = GetLineIndexFromCharacterIndex(CaretIndex);
            int lineLength = GetLineLength(lineIdx);
            SelectionStart = start;
            SelectionLength = lineLength;
        }

        public int GetLinesCount()
        {
            return Text.Split('\n').Length;
        }

        public void HighlightSearchResult(FindResult result, bool focusTextEditor = true)
        {
            if (result.StartIndex >= 0 &&
                result.StartIndex < Text.Length &&
                result.WordLength > 0 &&
                result.StartIndex + result.WordLength < Text.Length)
            {
                try
                {
                    if (focusTextEditor)
                    {
                        Focus();
                    }

                    Select(result.StartIndex, result.WordLength);
                    ScrollToLine(GetLineIndexFromCharacterIndex(SelectionStart));

                    //int actualLine = GetLineIndexFromCharacterIndex(result.StartIndex);
                    //int finalLine = actualLine + ((int) Math.Round((ActualHeight / FontSize) / 2, 0) ) ;
                    //ScrollToLine(actualLine);
                    //Select(result.StartIndex, result.WordLength);
                }
                catch { }
            }
        }

        public void ReplaceText(FindResult result, string toReplaceWith)
        {
            SelectionStart = result.StartIndex;
            SelectionLength = result.WordLength;
            SelectedText = toReplaceWith;
            //StringBuilder sb = new StringBuilder(heapText);
            //sb.Remove(result.StartIndex, result.WordLength);
            //sb.Insert(result.StartIndex, replaceWith);
        }

        public Rect GetCaretLocation()
        {
            try
            {
                if (CaretIndex >= 0)
                {
                    Rect rect = GetRectFromCharacterIndex(CaretIndex);
                    if (double.IsInfinity(rect.X) || double.IsInfinity(rect.Y))
                        return new Rect(0, 0, 0, 0);
                    return rect;
                }
                else return new Rect(0, 0, 0, 0);
            }
            catch { return new Rect(0, 0, 0, 0); }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            //if (!this.IsFocused)
            //    this.Focus();
            base.OnMouseMove(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            //if (!this.IsFocused)
            //{
            //    this.Focus();
            //    base.OnMouseWheel(e);
            //    this.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
            //}
            //else
            //{
                base.OnMouseWheel(e);
            //}
        }

        //protected override void OnLostFocus(RoutedEventArgs e)
        //{
        //    e.Handled = true;
        //    //base.OnLostFocus(e);
        //}
    }
}
