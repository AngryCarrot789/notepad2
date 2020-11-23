﻿using SharpPad.FileExplorer;
using SharpPad.Interfaces;
using SharpPad.Utilities;
using System;
using System.IO;
using System.Windows.Input;

namespace SharpPad.Notepad.FileProperties
{
    public class FilePropertiesViewModel : BaseViewModel
    {
        private string _fileName;
        private string _fileNameNoExtension;
        private string _filePath;
        private long _fileSize;
        private long _fileSizeDisk;
        private string _dateCreated;
        private string _dateModified;
        private string _dateAccessed;
        private bool _attributeIsReadOnly;
        private bool _attributeIsHidden;

        #region Info

        public string FileName
        {
            get => _fileName;
            set => RaisePropertyChanged(ref _fileName, value);
        }

        public string FileNameWithoutExtension
        {
            get => _fileNameNoExtension;
            set => RaisePropertyChanged(ref _fileNameNoExtension, value);
        }

        public string FilePath
        {
            get => _filePath;
            set => RaisePropertyChanged(ref _filePath, value);
        }

        public long FileSize
        {
            get => _fileSize;
            set => RaisePropertyChanged(ref _fileSize, value);
        }

        public long FileSizeDisk
        {
            get => _fileSizeDisk;
            set => RaisePropertyChanged(ref _fileSizeDisk, value);
        }

        public string DateCreated
        {
            get => _dateCreated;
            set => RaisePropertyChanged(ref _dateCreated, value);
        }

        public string DateModified
        {
            get => _dateModified;
            set => RaisePropertyChanged(ref _dateModified, value);
        }

        public string DateAccessed
        {
            get => _dateAccessed;
            set => RaisePropertyChanged(ref _dateAccessed, value);
        }

        public bool IsReadOnlyAttribute
        {
            get => _attributeIsReadOnly;
            set => RaisePropertyChanged(ref _attributeIsReadOnly, value);
        }

        public bool IsHiddenAttribute
        {
            get => _attributeIsHidden;
            set => RaisePropertyChanged(ref _attributeIsHidden, value);
        }

        #endregion

        public IView PropertiesView { get; }

        public ICommand CloseViewCommand { get; }
        public ICommand ShowAdditionalInfoCommand { get; }
        public ICommand RefreshInfoCommand { get; }

        public FilePropertiesViewModel(IView view)
        {
            PropertiesView = view;

            CloseViewCommand = new Command(Hide);
            ShowAdditionalInfoCommand = new Command(ShowAdditionalInfo);
            RefreshInfoCommand = new Command(RefreshInfo);
        }

        public void Show()
        {
            PropertiesView?.ShowView();
        }

        public void Hide()
        {
            PropertiesView?.HideView();
        }

        public void FetchProperties(string path)
        {
            if (path.IsFile())
            {
                FileInfo info = new FileInfo(path);
                FileAttributes attributes = File.GetAttributes(path);

                FileName = info.Name;
                FileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
                FilePath = info.FullName;
                FileSize = info.Length;
                FileSizeDisk = FilePropertiesHelper.GetFileSizeOnDisk(path);
                DateCreated = info.CreationTime.ToString();
                DateModified = info.LastWriteTime.ToString();
                DateAccessed = info.LastAccessTime.ToString();
                IsReadOnlyAttribute = (attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
                IsHiddenAttribute = (attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
            }
        }

        public void FetchFromDocument(DocumentViewModel model)
        {
            FileName = model.FileName;
            FileNameWithoutExtension = Path.GetFileNameWithoutExtension(model.FileName);
            FilePath = model.FilePath;
            FileSize = Convert.ToInt64(model.Text.Length);
            FileSizeDisk = 0;
            DateCreated = "Unavaliable";
            DateModified = "Unavaliable";
            DateAccessed = "Unavaliable";
        }

        public void RefreshInfo()
        {
            FetchProperties(FilePath);
        }

        public void ShowAdditionalInfo()
        {
            if (FilePath.IsFile())
            {
                FilePropertiesHelper.ShowFileProperties(FilePath);
            }
        }
    }
}
