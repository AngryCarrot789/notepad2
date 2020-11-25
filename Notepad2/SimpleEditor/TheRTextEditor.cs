﻿using SharpPad.CClipboard;
using SharpPad.Finding.TextFinding;
using SharpPad.InformationStuff;
using SharpPad.Preferences;
using SharpPad.Utilities;
using SharpPad.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace SharpPad.SimpleEditor
{
    public class TheRTextEditor : RichTextBox
    {
        //public NotepadViewModel NotepadModel
        //{
        //    get => this.DataContext as NotepadViewModel;
        //}
        //
        //public static DependencyProperty TextProperty =
        //    DependencyProperty.Register(
        //        "Text",
        //        typeof(string),
        //        typeof(TheRTextEditor),
        //        new PropertyMetadata(
        //            "", 
        //            TextPropertyChanged));
        //
        //public static DependencyProperty SelectedTextProperty =
        //    DependencyProperty.Register(
        //        "SelectedText",
        //        typeof(string),
        //        typeof(TheRTextEditor),
        //        new PropertyMetadata(
        //            "",
        //            SelectedTextPropertyChanged));
        //
        //public static DependencyProperty SelectionStartProperty =
        //    DependencyProperty.Register(
        //        "SelectionStart",
        //        typeof(int),
        //        typeof(TheRTextEditor),
        //        new PropertyMetadata(
        //            0,
        //            SelectionStartPropertyChanged));
        //
        //public static DependencyProperty SelectionLengthProperty =
        //    DependencyProperty.Register(
        //        "SelectionLength",
        //        typeof(int),
        //        typeof(TheRTextEditor),
        //        new PropertyMetadata(
        //            0,
        //            SelectionLengthPropertyChanged));
        //
        //public static DependencyProperty CaretIndexProperty =
        //    DependencyProperty.Register(
        //        "CaretIndex",
        //        typeof(int),
        //        typeof(TheRTextEditor),
        //        new PropertyMetadata(
        //            0,
        //            CaretIndexPropertyChanged));
        //
        //public string Text
        //{
        //    get => (string)GetValue(TextProperty);
        //    set => SetValue(TextProperty, value);
        //}
        //
        //public string SelectedText
        //{
        //    get => (string)GetValue(SelectedTextProperty);
        //    set => SetValue(SelectedTextProperty, value);
        //}
        //
        //public int SelectionStart
        //{
        //    get => (int)GetValue(SelectionStartProperty);
        //    set => SetValue(SelectionStartProperty, value);
        //}
        //
        //public int SelectionLength
        //{
        //    get => (int)GetValue(SelectionLengthProperty);
        //    set => SetValue(SelectionLengthProperty, value);
        //}
        //
        //public int CaretIndex
        //{
        //    get => (int)GetValue(CaretIndexProperty);
        //    set => SetValue(CaretIndexProperty, value);
        //}
        //
        //private static void TextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    if (d is TheRTextEditor editor)
        //    {
        //        if (e.NewValue is string str)
        //        {
        //            editor.Document.Blocks.Clear();
        //            editor.Document.Blocks.Add(new Paragraph(new Run(str)));
        //        }
        //    }
        //}
        //
        //private static void SelectedTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //
        //}
        //
        //private static void SelectionStartPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //
        //}
        //
        //private static void SelectionLengthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //
        //}
        //
        //private static void CaretIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //
        //}
        //
        //public TheRTextEditor()
        //{
        //
        //}
        //
        //public void Select(int start, int length)
        //{
        //    SelectionStart = start;
        //    SelectionLength = length;
        //}
        //
        //protected override void OnPreviewKeyDown(KeyEventArgs e)
        //{
        //    //Model.Notepad.LinesCounter.LinesCount = this.LineCount;
        //    if (Keyboard.IsKeyDown(Key.LeftCtrl))
        //    {
        //        if (Keyboard.IsKeyDown(Key.LeftShift))
        //        {
        //            switch (e.Key)
        //            {
        //                case Key.A:
        //                    if (!PreferencesG.CAN_SELECT_ENTIRE_LINE_CTRL_SHIFT_A)
        //                        break;
        //
        //                    SelectEntireCurrentLine();
        //                    break;
        //
        //                case Key.X:
        //                    if (!PreferencesG.CAN_CUT_ENTIRE_LINE_CTRL_X)
        //                        break;
        //                    SelectEntireCurrentLine();
        //                    SelectedText = "";
        //                    break;
        //
        //                case Key.Enter:
        //                    if (!PreferencesG.CAN_ADD_ENTIRE_LINES)
        //                        break;
        //                    AddNewLineBelowCurrentLine();
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            switch (e.Key)
        //            {
        //                case Key.Enter:
        //                    if (!PreferencesG.CAN_ADD_ENTIRE_LINES)
        //                        break;
        //                    AddNewLineAndMoveCaret();
        //                    break;
        //                case Key.X:
        //                    if (!PreferencesG.CAN_CUT_ENTIRE_LINE_CTRL_X)
        //                        break;
        //                    if (SelectedText == "")
        //                    {
        //                        SelectEntireCurrentLine();
        //                        // not altering e.Handled lets the
        //                        // textbox base cut the line automatically.
        //                    }
        //                    break;
        //                case Key.C:
        //                    if (!PreferencesG.CAN_COPY_ENTIRE_LINE_CTRL_C)
        //                        break;
        //                    if (SelectedText == "")
        //                    {
        //                        CopyEntireCurrentLine();
        //                        e.Handled = true;
        //                    }
        //                    break;
        //
        //                case Key.Up:
        //                    if (!PreferencesG.SCROLL_VERTICAL_WITH_CTRL_ARROWKEYS)
        //                        break;
        //                    LineUp(); e.Handled = true;
        //                    break;
        //                case Key.Down:
        //                    if (!PreferencesG.SCROLL_VERTICAL_WITH_CTRL_ARROWKEYS)
        //                        break;
        //                    LineDown(); e.Handled = true;
        //                    break;
        //                case Key.Left:
        //                    if (!PreferencesG.SCROLL_HORIZONTAL_WITH_CTRL_ARROWKEYS)
        //                        break;
        //                    LineLeft(); e.Handled = true;
        //                    break;
        //                case Key.Right:
        //                    if (!PreferencesG.SCROLL_HORIZONTAL_WITH_CTRL_ARROWKEYS)
        //                        break;
        //                    LineRight(); e.Handled = true;
        //                    break;
        //                // This prevents a bug with undo-ing causing the FindResults
        //                // to sort of be offset. This could be quite laggy though
        //                // with lots of text and text still in the find bit...
        //                case Key.Z:
        //                case Key.Y:
        //                    NotepadModel.Find.StartFind();
        //                    break;
        //            }
        //        }
        //    }
        //    // Cant do because holding alt apparently blocks the keydown events...
        //    //if (Keyboard.Modifiers == ModifierKeys.Alt)
        //    //{
        //    //    switch (e.Key)
        //    //    {
        //    //        case Key.Left: LineLeft(); e.Handled = true; break;
        //    //        case Key.Right: LineRight(); e.Handled = true; break;
        //    //    }
        //    //}
        //
        //    // This prevents a bug with undo-ing causing the FindResults
        //    // to sort of be offset. This could be quite laggy though
        //    // with lots of text and text still in the find bit...
        //    // this in unneeded now because it's re-done after
        //    // the find box is focused
        //    //Model.Find.StartFind();
        //}
        //
        //public void InsertText(string text, int charIndex, bool setCaretAfterText = false)
        //{
        //    SelectionStart = charIndex;
        //    SelectedText = text;
        //    SelectionLength = 0;
        //    if (setCaretAfterText)
        //        CaretIndex += text.Length;
        //}
        //
        //public void InsetNewLineAtCaret()
        //{
        //    InsertText("\n", CaretIndex, true);
        //}
        //
        //public void AddNewLineAboveCurrentLine()
        //{
        //    int startOfNewLineIndex = Text.LastIndexOf(Environment.NewLine, CaretIndex) + 1;
        //    SelectionStart = startOfNewLineIndex;
        //    SelectedText = Environment.NewLine;
        //    SelectionLength = 0;
        //    CaretIndex += 2;
        //}
        //
        ///// <summary>
        ///// 
        ///// </summary>
        //public void AddNewLineBelowCurrentLine()
        //{
        //    int startOfNewLineIndex = Text.LastIndexOf(Environment.NewLine, CaretIndex) + 1;
        //    SelectionStart = startOfNewLineIndex;
        //    int prevCaretIndex = CaretIndex;
        //    SelectedText = Environment.NewLine;
        //    SelectionLength = 0;
        //    CaretIndex = prevCaretIndex;
        //}
        //
        ///// <summary>
        ///// Set the caret index to the start of the new line 
        ///// and then inser a NewLine character
        ///// </summary>
        //public void AddNewLineAndMoveCaret()
        //{
        //    int startOfNewLineIndex = Text.LastIndexOf(Environment.NewLine, CaretIndex) + 1;
        //    int lineIndex = base.GetLineIndexFromCharacterIndex(CaretIndex);
        //    int lineLength = base.GetLineLength(lineIndex);
        //    CaretIndex = startOfNewLineIndex + lineLength + 1;
        //    SelectionStart = CaretIndex;
        //    SelectedText = Environment.NewLine;
        //    SelectionLength = 0;
        //    if (CaretIndex > 1)
        //        CaretIndex -= 1;
        //}
        //
        //public void CopyEntireCurrentLine()
        //{
        //    SelectEntireCurrentLine();
        //    string newLineText = SelectedText;
        //    CaretIndex = Text.LastIndexOf(Environment.NewLine, CaretIndex) + 1;
        //    CustomClipboard.SetTextObject(newLineText);
        //}
        //
        //public void SelectEntireCurrentLine()
        //{
        //    int start = Text.LastIndexOf(Environment.NewLine, CaretIndex) + 1;
        //    int lineIdx = GetLineIndexFromCharacterIndex(CaretIndex);
        //    int lineLength = GetLineLength(lineIdx);
        //    SelectionStart = start;
        //    SelectionLength = lineLength;
        //}
        //
        //public int GetLinesCount()
        //{
        //    return Text.Split('\n').Length;
        //}
        //
        //public void HighlightSearchResult(FindResult result, bool focusTextEditor = true)
        //{
        //    if (result.StartIndex >= 0 &&
        //        result.StartIndex < Text.Length &&
        //        result.WordLength > 0 &&
        //        result.StartIndex + result.WordLength < Text.Length)
        //    {
        //        try
        //        {
        //            if (focusTextEditor)
        //            {
        //                Focus();
        //            }
        //
        //            Select(result.StartIndex, result.WordLength);
        //            ScrollToLine(GetLineIndexFromCharacterIndex(SelectionStart));
        //
        //            //int actualLine = GetLineIndexFromCharacterIndex(result.StartIndex);
        //            //int finalLine = actualLine + ((int) Math.Round((ActualHeight / FontSize) / 2, 0) ) ;
        //            //ScrollToLine(actualLine);
        //            //Select(result.StartIndex, result.WordLength);
        //        }
        //        catch { }
        //    }
        //}
        //
        //public void ReplaceText(FindResult result, string toReplaceWith)
        //{
        //    if (!toReplaceWith.IsEmpty())
        //    {
        //        SelectionStart = result.StartIndex;
        //        SelectionLength = result.WordLength;
        //        SelectedText = toReplaceWith;
        //        //StringBuilder sb = new StringBuilder(heapText);
        //        //sb.Remove(result.StartIndex, result.WordLength);
        //        //sb.Insert(result.StartIndex, replaceWith);
        //    }
        //}
        //
        //public Rect GetCaretLocation()
        //{
        //    try
        //    {
        //        if (CaretIndex >= 0)
        //        {
        //            Rect rect = GetRectFromCharacterIndex(CaretIndex);
        //            if (double.IsInfinity(rect.X) || double.IsInfinity(rect.Y))
        //                return new Rect(0, 0, 0, 0);
        //            return rect;
        //        }
        //        else return new Rect(0, 0, 0, 0);
        //    }
        //    catch { return new Rect(0, 0, 0, 0); }
        //}
        //
        //protected override void OnMouseMove(MouseEventArgs e)
        //{
        //    //if (!this.IsFocused)
        //    //    this.Focus();
        //    base.OnMouseMove(e);
        //}
        //
        //protected override void OnMouseWheel(MouseWheelEventArgs e)
        //{
        //    //if (!this.IsFocused)
        //    //{
        //    //    this.Focus();
        //    //    base.OnMouseWheel(e);
        //    //    this.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
        //    //}
        //    //else
        //    //{
        //    base.OnMouseWheel(e);
        //    //}
        //}
        //
        ////protected override void OnLostFocus(RoutedEventArgs e)
        ////{
        ////    e.Handled = true;
        ////    //base.OnLostFocus(e);
        ////}
        //
        //public int GetCaretIndexWithinLine(int line)
        //{
        //    return CaretIndex - GetCharacterIndexFromLineIndex(line);
        //}
        //
        //protected override void OnSelectionChanged(RoutedEventArgs e)
        //{
        //    try
        //    {
        //        int line = GetLineIndexFromCharacterIndex(CaretIndex);
        //        int column = GetCaretIndexWithinLine(line);
        //
        //        NotepadModel.Line = line;
        //        NotepadModel.Column = column;
        //    }
        //    catch
        //    {
        //        Information.Show("Failed to get Line/Colum", "TextEditor");
        //    }
        //    base.OnSelectionChanged(e);
        //}
    }
}
