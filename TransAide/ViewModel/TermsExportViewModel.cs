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
using TransAide.Service;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Windows;
using TransAide.Helper;

namespace TransAide.ViewModel
{
    public class TermsExportViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand CloseWindowCommand { get; set; }
        public ICommand ExportTermsCommand { get; set; }
        public ICommand SelectFileCommand { get; set; }
        public ICommand OpenFolderCommand { get; set; }

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

        public TermsExportViewModel()
        {
            CloseWindowCommand = new RelayCommand(CloseWindow);
            ExportTermsCommand = new RelayCommand(ExportTerms);
            SelectFileCommand = new RelayCommand(SelectFile);
            OpenFolderCommand = new RelayCommand(OpenFolder);
            StudioProjectFolder = GetProjectFolder();

            LoadSettings();
        }

        public void ExportTerms(object parameter)
        {
            // Export Terms
            GlossaryTermCollection glossaryTermCollection = ExportGlossaryTerms.GetTerms(SourceSegmentType);
            string requiredTerms = glossaryTermCollection.GetGlossaryTermsAsString(false);
            string forbiddenTerms = glossaryTermCollection.GetGlossaryTermsAsString(true);
            string output = "Required Terms:\n" + requiredTerms + "\n\nForbidden Terms:\n" + forbiddenTerms;
            try
            {
                File.WriteAllText(FileManager.GetFileNamePath(ExportFile, StudioProjectFolder), output, Encoding.UTF8);
                MessageBox.Show("Exported successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void CloseWindow(object parameter)
        {
            // Before window closed
            SaveSettings();
        }

        public void SelectFile(object parameter)
        {
            // Select File
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Text files (*.txt)|*.txt";
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

        public string ExportFile
        {
            get { return exportFile; }
            set
            {
                exportFile = value;
                OnPropertyChanged();
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
            TransAideTermsExportConfig config = TransAideTermsExportConfig.LoadFromFile(studioProjectFolder + "\\" + "transaide-terms-export.json") 
                ?? new TransAideTermsExportConfig();
            SetSettingsExportFile(config.TermsExportFile);
            SetSettingsSourceSegmentType(config.SourceSegmentType);
        }

        public void SetSettingsExportFile(string exportFile)
        {
            ExportFile = exportFile;
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
            TransAideTermsExportConfig config = new TransAideTermsExportConfig(ExportFile, SourceSegmentType);
            TransAideTermsExportConfig.WriteToFile(config, studioProjectFolder + "\\" + "transaide-terms-export.json");
        }
    }
}
