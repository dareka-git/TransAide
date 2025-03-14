using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TransAide.Model;
using TransAide.Enum;
using TransAide.Command;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using TransAide.Helper;
using TransAide.Service;
using System.Threading;

namespace TransAide.ViewModel
{
    public class ImportViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand SelectFileCommand { get; set; }
        public ICommand CloseWindowCommand { get; set; }
        public ICommand ImportTranslationsCommand { get; set; }
        public ICommand OpenFolderCommand { get; set; }


        private ImportTypeEnum importType;
        private bool importTargetOnlyTextFile;
        private bool importTargetOnlyClipboard;
        private bool importJson;
        private ImportFromEnum importFrom;
        private bool fromCurrentSegment;
        private bool fromFirstSegment;
        private ImportToEnum importTo;
        private bool next50Segments;
        private bool next100Segments;
        private bool toEnd;
        private DestinationSegmentTypeEnum destinationSegmentType;
        private bool unconfirmedSegments;
        private bool unlockedSegments;
        private bool allSegments;
        private bool filteredSegments;
        private string importFile;
        private ImportFileContentEnum importFileContent;
        private bool fileWithOnlySegmentsToImport;
        private bool fileWithAllSegments;
        
        private string studioProjectFolder;

        public ImportViewModel()
        {
            SelectFileCommand = new RelayCommand(SelectFile);
            CloseWindowCommand = new RelayCommand(CloseWindow);
            ImportTranslationsCommand = new RelayCommand(ImportTranslations);
            OpenFolderCommand = new RelayCommand(OpenFolder);
            StudioProjectFolder = GetProjectFolder();
            LoadSettings();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string StudioProjectFolder
        {
            get { return studioProjectFolder; }
            set
            {
                studioProjectFolder = value;
                OnPropertyChanged();
            }
        }

        public bool FileWithOnlySegmentsToImport
        {
            get { return fileWithOnlySegmentsToImport; }
            set
            {
                fileWithOnlySegmentsToImport = value;
                OnPropertyChanged();
                if (value) { ImportFileContent = ImportFileContentEnum.OnlySegmentsToImport; }
            }
        }

        public bool FileWithAllSegments
        {
            get { return fileWithAllSegments; }
            set
            {
                fileWithAllSegments = value;
                OnPropertyChanged();
                if (value) { ImportFileContent = ImportFileContentEnum.AllSegments; }
            }
        }

        public ImportFileContentEnum ImportFileContent
        {
            get { return importFileContent; }
            set
            {
                importFileContent = value;
                OnPropertyChanged();
            }
        }

        public bool ImportTargetOnlyTextFile
        {
            get { return importTargetOnlyTextFile; }
            set
            {
                importTargetOnlyTextFile = value;
                OnPropertyChanged();
                if (value) { ImportType = ImportTypeEnum.TargetOnlyTextFile; }
                if (String.IsNullOrEmpty(ImportFile) && File.Exists(studioProjectFolder + "tgt.txt"))
                {
                    ImportFile = "tgt.txt";
                }
            }
        }

        public bool ImportTargetOnlyClipboard
        {
            get { return importTargetOnlyClipboard; }
            set
            {
                importTargetOnlyClipboard = value;
                OnPropertyChanged();
                if (value) { ImportType = ImportTypeEnum.TargetOnlyClipboard; }
            }
        }

        public bool ImportJson
        {
            get { return importJson; }
            set
            {
                importJson = value;
                OnPropertyChanged();
                if (value) { ImportType = ImportTypeEnum.JSONFile; }
                if (String.IsNullOrEmpty(ImportFile) && File.Exists(studioProjectFolder + "tgt.json"))
                {
                    ImportFile = "tgt.json";
                }
            }
        }

        public ImportTypeEnum ImportType
        {
            get { return importType; }
            set
            {
                importType = value;
                OnPropertyChanged();
            }
        }

        public string ImportFile
        {
            get { return importFile; }
            set
            {
                importFile = value;
                OnPropertyChanged();
            }
        }

        public bool FromCurrentSegment
        {
            get { return fromCurrentSegment; }
            set
            {
                fromCurrentSegment = value;
                OnPropertyChanged();
                if (value) { ImportFrom = ImportFromEnum.CurrentSegment; }
            }
        }

        public bool FromFirstSegment
        {
            get { return fromFirstSegment; }
            set
            {
                fromFirstSegment = value;
                OnPropertyChanged();
                if (value) { ImportFrom = ImportFromEnum.FirstSegment; }
            }
        }

        public ImportFromEnum ImportFrom
        {
            get { return importFrom; }
            set
            {
                importFrom = value;
                OnPropertyChanged();
            }
        }

        public bool Next50Segments
        {
            get { return next50Segments; }
            set
            {
                next50Segments = value;
                OnPropertyChanged();
                if (value) { ImportTo = ImportToEnum.Next50Segments; }
            }
        }

        public bool Next100Segments
        {
            get { return next100Segments; }
            set
            {
                next100Segments = value;
                OnPropertyChanged();
                OnPropertyChanged("ImportTo");
                if (value) { ImportTo = ImportToEnum.Next100Segments;}
            }
        }

        public bool ToEnd
        {
            get { return toEnd; }
            set
            {
                toEnd = value;
                OnPropertyChanged();
                if (value) { ImportTo = ImportToEnum.ToEnd;}
            }
        }

        public ImportToEnum ImportTo
        {
            get { return importTo; }
            set
            {
                importTo = value;
                OnPropertyChanged();
            }
        }

        public bool UnconfirmedSegments
        {
            get { return unconfirmedSegments; }
            set
            {
                unconfirmedSegments = value;
                OnPropertyChanged();
                if (value) { DestinationSegmentType = DestinationSegmentTypeEnum.UnconfirmedSegments; }
            }
        }

        public bool UnlockedSegments
        {
            get { return unlockedSegments; }
            set
            {
                unlockedSegments = value;
                OnPropertyChanged();
                if (value) { DestinationSegmentType = DestinationSegmentTypeEnum.UnlockedSegments; }
            }
        }

        public bool AllSegments
        {
            get { return allSegments; }
            set
            {
                allSegments = value;
                OnPropertyChanged();
                if (value) { DestinationSegmentType = DestinationSegmentTypeEnum.AllSegments; }
            }
        }

        public bool FilteredSegments
        {
            get { return filteredSegments; }
            set
            {
                filteredSegments = value;
                OnPropertyChanged();
                if (value) { DestinationSegmentType = DestinationSegmentTypeEnum.FilteredSegments; }
            }
        }

        public DestinationSegmentTypeEnum DestinationSegmentType
        {
            get { return destinationSegmentType; }
            set
            {
                destinationSegmentType = value;
                OnPropertyChanged();
            }
        }

        public void SelectFile(object parameter)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Text files (*.txt)|*.txt|JSON files (*.json)|*.json";
            dialog.InitialDirectory = studioProjectFolder;
            if (dialog.ShowDialog() == true) { ImportFile = dialog.FileName; }
        }

        public void OpenFolder(object parameter)
        {
            // Open Folder
            System.Diagnostics.Process.Start("explorer.exe", studioProjectFolder);
        }

        public void CloseWindow(object parameter)
        {
            // Before window closed
            SaveSettings();
        }

        public void ImportTranslations(object parameter)
        {
            // Import translations
            EditorController editorController = SdlTradosStudio.Application.GetController<EditorController>();
            switch (ImportType)
            {
                case ImportTypeEnum.TargetOnlyTextFile:
                    List<string> targets = FileManager.LoadTextFileLines(ImportFile);
                    int result = ImportTargetText.SetTargetSegmentsText(editorController, 
                        DestinationSegmentType, ImportFileContent, ImportFrom, ImportTo, targets);
                    string messageText = result > 0 ? "Import finished succesfully: " + result + " segments imported" : "Import finished. Nothing has been imported.";
                    MessageBox.Show(messageText, "Import Results", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case ImportTypeEnum.TargetOnlyClipboard:
                    targets = FileManager.LoadTextFromClipboard();
                    result = ImportTargetText.SetTargetSegmentsText(editorController,
                        DestinationSegmentType, ImportFileContent, ImportFrom, ImportTo, targets);
                    messageText = result > 0 ? "Import finished succesfully: " + result + " segments imported" : "Import finished. Nothing has been imported.";
                    MessageBox.Show(messageText, "Import Results", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case ImportTypeEnum.JSONFile:
                    List<TranslationUnit> translationUnits = JsonManager.LoadListFromFile<TranslationUnit>(ImportFile);
                    result = ImportJSON.SetTargetSegmentsText(editorController,
                        DestinationSegmentType, ImportFileContent, ImportFrom, ImportTo, translationUnits);
                    messageText = result > 0 ? "Import finished succesfully: " + result + " segments imported" : "Import finished. Nothing has been imported.";
                    MessageBox.Show(messageText, "Import Results", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                default:
                    break;
            }

        }

        public void LoadSettings()
        {
            TransAideImportConfig config = TransAideImportConfig.LoadFromFile(studioProjectFolder + "\\" + "transaide-import.json");
            if (config == null) { config = new TransAideImportConfig(); }
            SetSettingsImportType(config.ImportType);
            SetSettingsImportFile(config.ImportFile);
            SetSettingsImportFrom(config.ImportFrom);
            SetSettingsImportTo(config.ImportTo);
            SetSettingsDestinationSegmentType(config.DestinationSegmentType);
            SetSettingsImportFileContent(config.ImportFileContent);
        }


        public void SetSettingsImportType(ImportTypeEnum importType)
        {
            switch (importType)
            {
                case ImportTypeEnum.TargetOnlyTextFile:
                    ImportTargetOnlyTextFile = true;
                    break;
                case ImportTypeEnum.TargetOnlyClipboard:
                    ImportTargetOnlyClipboard = true;
                    break;
                case ImportTypeEnum.JSONFile:
                    ImportJson = true;
                    break;
                default:
                    break;
            }

        }

        public void SetSettingsImportFile(string importFile)
        {
            ImportFile = importFile;
        }

        public void SetSettingsImportFrom(ImportFromEnum importFrom)
        {
            switch (importFrom)
            {
                case ImportFromEnum.CurrentSegment:
                    FromCurrentSegment = true;
                    break;
                case ImportFromEnum.FirstSegment:
                    FromFirstSegment = true;
                    break;
                default:
                    break;
            }
        }

        public void SetSettingsImportTo(ImportToEnum importTo)
        {
            switch (importTo)
            {
                case ImportToEnum.Next50Segments:
                    Next50Segments = true;
                    break;
                case ImportToEnum.Next100Segments:
                    Next100Segments = true;
                    break;
                case ImportToEnum.ToEnd:
                    ToEnd = true;
                    break;
                default:
                    break;
            }
        }

        public void SetSettingsDestinationSegmentType(DestinationSegmentTypeEnum destinationSegmentType)
        {
            switch (destinationSegmentType)
            {
                case DestinationSegmentTypeEnum.UnconfirmedSegments:
                    UnconfirmedSegments = true;
                    break;
                case DestinationSegmentTypeEnum.UnlockedSegments:
                    UnlockedSegments = true;
                    break;
                case DestinationSegmentTypeEnum.AllSegments:
                    AllSegments = true;
                    break;
                case DestinationSegmentTypeEnum.FilteredSegments:
                    FilteredSegments = true;
                    break;
                default:
                    break;
            }
        }

        public void SetSettingsImportFileContent(ImportFileContentEnum importFileContent)
        {
            switch (importFileContent)
            {
                case ImportFileContentEnum.OnlySegmentsToImport:
                    FileWithOnlySegmentsToImport = true;
                    break;
                case ImportFileContentEnum.AllSegments:
                    FileWithAllSegments = true; 
                    break;
                default:
                    break;
            }
        }

        public void SaveSettings()
        {
            TransAideImportConfig config = new TransAideImportConfig(ImportType, ImportFile, ImportFrom, ImportTo, DestinationSegmentType, ImportFileContent);  
            TransAideImportConfig.WriteToFile(config, studioProjectFolder + "\\" + "transaide-import.json");
        }

        public string GetProjectFolder()
        {
            EditorController editorController = SdlTradosStudio.Application.GetController<EditorController>();
            if (editorController.ActiveDocument == null)
            {
                MessageBox.Show("There is no document loaded in the editor");
                return "";
            }
            return Path.GetDirectoryName(editorController.ActiveDocument.Project.FilePath);
        }

    }

}
