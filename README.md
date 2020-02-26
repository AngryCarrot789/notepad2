# Notepad # (Notepad Sharp, good name tbh)
Window's notepad is blindingly light themed, so i decided to make my own dark theme version. It's fairly robust but it works

## Errors:
I added a feature to open the selected notepad in another window. It's a bit broken, and by a bit i mean very broken. Try not to use it; it basically resets the theme you're using when you open the program again, and closing the original mainwindow closes all other windows because of some error i couldnt fix because of a vs2017 bug apparently, and some other things too.

## Some 'code-level' info about it
the items on the left are a separate control (NotepadItem or something), and their datacontext is a FileItemViewModel which contains a DocumentModel (containing path, filename, text, etc) and FormatModel (fontsize, wrapping, etc). When you select an item, it sets the main notepad view's datacontext as the item's datacontext you selected. 

## other things
it's completely opensource so you can edit it and stuff. would be nice if you credited me if you post it somewhere else (or dont... not too bothered tbh) :)
