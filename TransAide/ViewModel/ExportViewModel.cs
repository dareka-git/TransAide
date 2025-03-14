using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TransAide.Enum;
using TransAide.Model;
using TransAide.Command;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Internal;
using System.Windows;
using TransAide.Helper;

namespace TransAide.ViewModel
{
    public class ExportViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand CloseWindowCommand { get; set; }
        public ICommand ExportSourceCommand { get; set; }
        public ICommand SelectFileCommand { get; set; }
        public ICommand OpenFolderCommand { get; set; }

        //public ICommand ExportFileLostFocusCommand { get; set; }

        private ExportTypeEnum exportType;
        private bool exportSourceOnlyTextFile;
        private bool exportJson;
        private TextFormatEnum textFormat;
        private bool plainTextWithoutTags;
        private bool textWithTags;
        private SourceSegmentTypeEnum sourceSegmentType;
        private bool unconfirmedSegments;
        private bool unlockedSegments;
        private bool allSegments;
        private bool filteredSegments;
        private string exportFile;

        private string studioProjectFolder;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ExportViewModel()
        {
            CloseWindowCommand = new RelayCommand(CloseWindow);
            ExportSourceCommand = new RelayCommand(ExportSource);
            SelectFileCommand = new RelayCommand(SelectFile);
            OpenFolderCommand = new RelayCommand(OpenFolder);
            //ExportFileLostFocusCommand = new RelayCommand(ExportFileLostFocus);
            StudioProjectFolder = GetProjectFolder();
            LoadSettings();

        }

        public void CloseWindow(object parameter)
        {
            // Before window closed
            SaveSettings();
        }


        public void ExportSource(object parameter)
        {
            EditorController editorController = SdlTradosStudio.Application.GetController<EditorController>();
            switch (ExportType)
            {
                case ExportTypeEnum.SourceOnlyTextFile:
                    // Export Source
                    string sourceText = Service.ExportSourceOnly.GetSourceSegmentsText(editorController, TextFormat, SourceSegmentType);
                    try
                    {
                        File.WriteAllText(FileManager.GetFileNamePath(ExportFile, StudioProjectFolder), sourceText, Encoding.UTF8);
                        MessageBox.Show("Exported successfully");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    break;
                case ExportTypeEnum.JSONFile:
                    // Export JSON
                    List<TranslationUnit> translationUnits = Service.ExportJSON.GetTranslationUnits(editorController, TextFormat, SourceSegmentType);
                    try
                    {
                        JsonManager.WriteListToFile(translationUnits, FileManager.GetFileNamePath(ExportFile, StudioProjectFolder));
                        MessageBox.Show("Exported successfully");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    break;
                default:
                    break;
            }
        }

        public void SelectFile(object parameter)
        {
            // Select File
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Text files (*.txt)|*.txt|JSON files (*.json)|*.json";
            dialog.InitialDirectory = studioProjectFolder;
            if (dialog.ShowDialog() == true) { ExportFile = dialog.FileName; }
        }

        public void OpenFolder(object parameter)
        {
            // Open Folder
            System.Diagnostics.Process.Start("explorer.exe", studioProjectFolder);
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

        public ExportTypeEnum ExportType
        {
            get { return exportType; }
            set
            {
                exportType = value;
                OnPropertyChanged();
            }
        }

        public bool ExportSourceOnlyTextFile
        {
            get { return exportSourceOnlyTextFile; }
            set
            {
                exportSourceOnlyTextFile = value;
                OnPropertyChanged();
                if (value) { 
                    ExportType = ExportTypeEnum.SourceOnlyTextFile;
                    ExportFile = "src.txt";
                }
            }
        }

        public bool ExportJson
        {
            get { return exportJson; }
            set
            {
                exportJson = value;
                OnPropertyChanged();
                if (value) { 
                    ExportType = ExportTypeEnum.JSONFile;
                    ExportFile = "translations.json";
                }
            }
        }

        public string ExportFile
        {
            get { return exportFile; }
            set
            {
                exportFile = value;
                OnPropertyChanged();
            }
        }

        public TextFormatEnum TextFormat
        {
            get { return textFormat; }
            set
            {
                textFormat = value;
                OnPropertyChanged();
            }
        }

        public bool PlainTextWithoutTags
        {
            get { return plainTextWithoutTags; }
            set
            {
                plainTextWithoutTags = value;
                OnPropertyChanged();
                if (value) { TextFormat = TextFormatEnum.PlainTextWithoutTags; }
            }
        }

        public bool TextWithTags
        {
            get { return textWithTags; }
            set
            {
                textWithTags = value;
                OnPropertyChanged();
                if (value) { TextFormat = TextFormatEnum.TextWithTags; }
            }
        }

        public SourceSegmentTypeEnum SourceSegmentType
        {
            get { return sourceSegmentType; }
            set
            {
                sourceSegmentType = value;
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
                if (value) { SourceSegmentType = SourceSegmentTypeEnum.UnconfirmedSegments; }
            }
        }

        public bool UnlockedSegments
        {
            get { return unlockedSegments; }
            set
            {
                unlockedSegments = value;
                OnPropertyChanged();
                if (value) { SourceSegmentType = SourceSegmentTypeEnum.UnlockedSegments; }
            }
        }

        public bool AllSegments
        {
            get { return allSegments; }
            set
            {
                allSegments = value;
                OnPropertyChanged();
                if (value) { SourceSegmentType = SourceSegmentTypeEnum.AllSegments; }
            }
        }

        public bool FilteredSegments
        {
            get { return filteredSegments; }
            set
            {
                filteredSegments = value;
                OnPropertyChanged();
                if (value) { SourceSegmentType = SourceSegmentTypeEnum.FilteredSegments; }
            }
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

        public void LoadSettings()
        {
            // Load settings
            TransAideExportConfig config = TransAideExportConfig.LoadFromFile(studioProjectFolder + "\\" + "transaide-export.json") ?? new TransAideExportConfig();
            SetSettingsExportType(config.ExportType);
            SetSettingsExportFile(config.ExportFile);
            SetSettingsTextFormat(config.TextFormat);
            SetSettingsSourceSegmentType(config.SourceSegmentType);
        }

        public void SetSettingsExportType(ExportTypeEnum exportType)
        {
            switch (exportType)
            {
                case ExportTypeEnum.SourceOnlyTextFile:
                    ExportSourceOnlyTextFile = true;
                    break;
                case ExportTypeEnum.JSONFile:
                    ExportJson = true;
                    break;
                default:
                    break;
            }
        }

        public void SetSettingsExportFile(string exportFile)
        {
            ExportFile = exportFile;
        }

        public void SetSettingsTextFormat(TextFormatEnum textFormat)
        {
            switch (textFormat)
            {
                case TextFormatEnum.PlainTextWithoutTags:
                    PlainTextWithoutTags = true;
                    break;
                case TextFormatEnum.TextWithTags:
                    TextWithTags = true;
                    break;
                default:
                    break;
            }
        }

        public void SetSettingsSourceSegmentType(SourceSegmentTypeEnum sourceSegmentType)
        {
            switch (sourceSegmentType)
            {
                case SourceSegmentTypeEnum.UnconfirmedSegments:
                    UnconfirmedSegments = true;
                    break;
                case SourceSegmentTypeEnum.UnlockedSegments:
                    UnlockedSegments = true;
                    break;
                case SourceSegmentTypeEnum.AllSegments:
                    AllSegments = true;
                    break;
                case SourceSegmentTypeEnum.FilteredSegments:
                    FilteredSegments = true;
                    break;
                default:
                    break;
            }
        }

        public void SaveSettings()
        {
            // Save settings
            TransAideExportConfig config = new TransAideExportConfig(ExportType, ExportFile, TextFormat, SourceSegmentType);
            TransAideExportConfig.WriteToFile(config, studioProjectFolder + "\\" + "transaide-export.json");
        }
    }
}
