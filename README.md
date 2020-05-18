# SharpPad
Window's notepad is blindingly light themed, so i decided to make my own dark theme version with tabs and many other (probably useless) features. It uses WPF and MVVM (no MVVM Light or any nuget packages, i did it all myself). It's fairly robust, but it works. 
BTW, if you want to, you could improve the "messyness" of the code, cus i just splat things randomly into classes and never clean them up (most of the times). And, i dont think this program 100% follow the MVVM standards (maybe 90%), but who cares anyway ;)
Here's a snapshot of the program, showing the infolist at the bottom, multiple NotepadListItems and loads of text.
![](sharpPadPicture.png)

## Info on how to use this program
NotepadListItems are the things on the left of the program, containing stuff.
- Starting from the top, the titlebar. it does everything you'd expect.
- There's menus below that (file, theme, etc). File allows opening/saving/closing of NotepadListItems. Theme allows you to change the application theme (dark/light). Windows lets you open the selected NotepadListItems in another window. Help... shows help :)
- Below, is some buttons. New File, Open File, Save, Save as, Close selected NotepadListItem, Close all NotepadListItems, Cut, Copy, Paste, Undo, Redo, Print selected file and find (but it's broken).
- Below that, for the NotepadListItem, the Font, FontSize, Thingy... Normal/Italic/Oblique, Normal/Bold/ExtraBold, and textdecorations like underlining and strikethrough, and finally text wrapping (hides horizontal scrollbar)
- Then you have the list to the left, containing NotepadListItem, aka files. Those NotepadListItem have a name, Saved/Unsaved tag, Character count and filesize in kb. There's a close button to the very right top, which will close/remove the NotepadListItem. There's also a grip on the left of the item; click and move your mouse to start dragging the item. this allows you to drag the file like you would in file explorer. At the bottom of the list is a button to clear all items. it has the same functionality as the button at the top.
- To the right is the TextBox, or the text input. This is where all the text goes. Right click for a menu of standard functions, like Copy/Paste/Cut/Undo/Redo. Below the TextBox is the ammount you've zoomed in/out (hold ctrl and scroll to zoom), which is actually just the fontsize :/, to the right is the FileName (in a textbox so you can change it), and next is the file's path (also editable).
- Finally below is the Information list, containing a list of info regarding things that have happened in the program (like Info, Statuses and errors). An example of info is creating a new file, opening one, saving one, Draging the NotepadListItem, or breaking the program somehow because i forgot to check if the file exists when saving or something lol

## Latest Updates
- Also fixed DragDropping slightly. Before, you could drag the entire NotepadListItems. Now, you have to drag the grip (on the left of the item). This is makes sure you dont accidentally start dragging by accident
- Added ability to add extensions to file, or change the existing extension with another. can do this by right clicking the NotepadListItems and at the bottom is a dropdown menu with a list of extensions. Nifty feature tbh, but kinda useless as you could just rename the file. oh well.
- Added a list at the bottom for errors, alerts and information the programs throws. Click the expander to show it. Also added an orange/red ribbon to the NotepadListItems when the file size goes over 100kb/250kb, because at those points the program might lag.
- Added even more colourful icons, better than the other ones
- Added a Drag and Drop feature to the NotepadListItems. you can drag them to a folder, and it creates a file with the specified extension at that location. can also be used to transfer NotepadListItems between different windows
- Added a light theme, and also colourful themes too (4 themes in total: colourful light and dark, and normal light and dark)
- Can open selected notepad in another window (bugs may occour, so be careful with it)
- Auto-prompts you to save unsaved/edited work/files when you exit. Click no to just exit
- Added ability to change fonts/fontsizes/wrapping, etc. affects all of the text in the selected NotepadListItems because this isn't a rich text editor. 

## Errors:
I added a feature to open the selected notepad in another window. It's a bit broken. Try not to use it; closing the original mainwindow closes all other windows because of some error i couldnt fix. and some other things too. I added MainWindow to the titlebar, so that you know which window is which.

## Some 'code-level' info about it
the items on the left are a separate control (NotepadListItems, or NotepadListItems), and their DataContext is a FileItemViewModel which contains a DocumentModel (containing path, filename, text, etc) and FormatModel (fontsize, wrapping, etc). When you select an item, it sets the main notepad view's DataContext as the selected NotepadListItems's DataContext. and through binding, it updates the view accordingly.

## other things
it's completely opensource so you can edit it and stuff. would be nice if you credited me if you post it somewhere else (or dont... not too bothered tbh) :)
