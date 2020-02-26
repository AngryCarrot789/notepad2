## WARNING! I THINK THE LAST UPDATE HAD LIGHT THEME ON SO DONT GET BLINDED! 
click the theme menu at the top bar, click dark and enjoy the darker theme :)
# Notepad # (Notepad Sharp, good name tbh)

Window's notepad is blindingly light themed, so i decided to make my own dark theme version. It's fairly robust but it works

## Some 'code-level' info about it
the items on the left are a separate control (NotepadItem or something), and their datacontext is a FileItemViewModel which contains a DocumentModel (containing path, filename, text, etc) and FormatModel (fontsize, wrapping, etc). When you select an item, it sets the main notepad view's datacontext as the item's datacontext you selected. 

## other things
it's completely opensource so you can edit it and stuff. would be nice if you credited me if you post it somewhere else (or dont... not too bothered tbh) :)
