# SharpPad
Window's notepad is blindingly light themed, so i decided to make my own dark theme version with tabs. It uses WPF and MVVM (no mvvm light, i did it all myself). It's fairly robust, but it works. 

## Some 'code-level' info about it
the items on the left are a separate control (NotepadListitems) , and their datacontext is a FileItemViewModel which contains a DocumentModel (containing path, filename, text, etc) and FormatModel (fontsize, wrapping, etc). When you select an item, it sets the main notepad view's datacontext as the item's datacontext you selected. and through binding, it updates the view accordingly.

## Errors:
I added a feature to open the selected notepad in another window. It's a bit broken, and by a bit i mean very broken. Try not to use it; it basically resets the theme you're using when you open the program again, and closing the original mainwindow closes all other windows because of some error i couldnt fix because of a vs2017 bug apparently, and some other things too. there's a very long paragraph when you hover over the button to do it (the orange one).
However, i saw that if you close all the "extra notepad windows" first before closing the mainwindow it seems to stop that theme bug from happening. But that's time consuming so overall, dont use (yet... might fix soon)

## Latest Updates
- Added light theme
- Can open selected notepad in another window (VERY BROKEN, DONT USE. BREAKS THINGS). Update: I disabled this as it's too broken 
- Auto-prompts you to save unsaved/edited work/files when you exit. Click no to just exit
- More colourful icons that stand out better in both the light and dark theme
- Improved some other things ive forgotten about :/

## other things
it's completely opensource so you can edit it and stuff. would be nice if you credited me if you post it somewhere else (or dont... not too bothered tbh) :)
