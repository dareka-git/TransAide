using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.ProjectAutomation.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using TransAide.View;
using TransAide.Command;
using System.Windows;
using System.IO;
using System.Collections.ObjectModel;
using TransAide.Model;
using TransAide.Helper;

namespace TransAide.ViewModel
{
    public class TransAideEditorViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand OpenImportWindowCommand { get; set; }
        public ICommand OpenExportSegmentsWindowCommand { get; set; }
        public ICommand OpenExportTermsWindowCommand { get; set; }
        public ICommand OpenFolderCommand { get; set; }
        public ICommand SelectJsonFileCommand { get; set; }
        public ICommand AttachJsonFileCommand { get; set; }
        public ICommand DetachJsonFileCommand { get; set; }
        public ICommand ApplyTargetCommand { get; set; }
        public ICommand OpenHomePageCommand { get; set; }

        public ObservableCollection<TranslationUnit> TranslationUnits;

        private EditorController editorController;
        private ProjectsController projectsController;
        private IStudioDocument activeDocument;
        private ISegmentPair activeSegment;

        
        private TranslationUnit tu; 
        private string studioProjectFolder;
        private string jsonFile;
        private bool isFileAttached;
        private string fileStatusBarLabel;

        private string source;
        private string target;
        private string appTitle;


        public TransAideEditorViewModel(EditorController editorController, ProjectsController projectsController)
        {
            ObservableCollection<TranslationUnit> TranslationUnits = new ObservableCollection<TranslationUnit>();
            OpenImportWindowCommand = new RelayCommand(OpenImportWindow);
            OpenExportSegmentsWindowCommand = new RelayCommand(OpenExportSegmentsWindow);
            OpenExportTermsWindowCommand = new RelayCommand(OpenExportTermsWindow);
            OpenFolderCommand = new RelayCommand(OpenFolder);
            OpenHomePageCommand = new RelayCommand(OpenHomePage);

            SelectJsonFileCommand = new RelayCommand(SelectJsonFile);
            AttachJsonFileCommand = new RelayCommand(AttachJsonFile);
            DetachJsonFileCommand = new RelayCommand(DetachJsonFile);
            ApplyTargetCommand = new RelayCommand(ApplyTarget);


            AppTitle = "Trans AIde";
            this.editorController = editorController;
            this.projectsController = projectsController;
            IsFileAttached = false;

            this.editorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;
            
            SetActiveDocument(this.editorController.ActiveDocument);
            StudioProjectFolder = GetProjectFolder();

            Source = this.editorController?.ActiveDocument?.GetActiveSegmentPair().Source.ToString();
            Target = this.editorController?.ActiveDocument?.GetActiveSegmentPair().Target.ToString();
        }


        public string FileStatusBarLabel
        {
            get { return fileStatusBarLabel; }
            set
            {
                fileStatusBarLabel = value;
                OnPropertyChanged();
            }
        }


        public bool IsFileAttached
        {
            get { return isFileAttached; }
            set
            {
                isFileAttached = value;
                OnPropertyChanged();
            }
        }

        public TranslationUnit Tu
        {
            get { return tu; }
            set
            {
                tu = value;
                OnPropertyChanged();
            }
        }

        public string JsonFile
        {
            get { return jsonFile; }
            set
            {
                jsonFile = value;
                OnPropertyChanged();
            }
        }

        public string Target
        {
            get { return target; }
            set
            {
                target = value;
                OnPropertyChanged();
            }
        }

        public string Source
        {
            get { return source; }
            set
            {
                source = value;
                OnPropertyChanged();
            }
        }
        public string AppTitle
        {
            get { return appTitle; }
            set
            {
                appTitle = value;
                OnPropertyChanged();
            }
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

        public void OpenHomePage(object parameter)
        {
            // Open Home Page
            try { 
                System.Diagnostics.Process.Start("https://posteditacat.xyz/en/");
            }
            catch
            {
                MessageBox.Show("Error opening the home page.");
            }
        }

        public void ApplyTarget(object parameter)
        {
            // Apply Target
            if (IsFileAttached && activeSegment != null)
            {
                Service.UpdateTargetSegment.ApplyTarget(editorController, Tu.Target);
            }
        }

        public void SelectJsonFile(object parameter)
        {
            // Open Json File
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".json";
            dlg.Filter = "Json Files (*.json)|*.json";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                JsonFile = dlg.FileName;
                IsFileAttached = false;
                FileStatusBarLabel = GetFileStatusBarLabel();
            }
        }

        public void AttachJsonFile(object parameter)
        {
            // Attach Json File
            if (JsonFile != null && File.Exists(JsonFile))
            {
                TranslationUnits = JsonManager.LoadFromFile<ObservableCollection<TranslationUnit>>(JsonFile);
                IsFileAttached = true;
                FileStatusBarLabel = GetFileStatusBarLabel();
            }
        }

        public void DetachJsonFile(object parameter)
        {
            // Detach Json File
            TranslationUnits.Clear();
            IsFileAttached = false;
            FileStatusBarLabel = GetFileStatusBarLabel();
        }


        public void OpenExportSegmentsWindow(object parameter)
        {
            // Open Export Window
            ExportView exportView = new ExportView();
            exportView.ShowDialog();
        }

        public void OpenExportTermsWindow(object parameter)
        {
            // Open Export Terms Window
            TermsExportView termsExportView = new TermsExportView();
            termsExportView.ShowDialog();
        }

        public void OpenImportWindow(object parameter)
        {
            // Open Import Window
            ImportView importView = new ImportView();
            importView.ShowDialog();
        }

        public void OpenFolder(object parameter)
        {
            // Open Folder
            System.Diagnostics.Process.Start("explorer.exe", studioProjectFolder);
        }

        private void EditorController_ActiveDocumentChanged(object sender, DocumentEventArgs e)
        {
            SetActiveDocument(e.Document);
        }

        private void SetActiveDocument (IStudioDocument doc)
        {
            //IStudioDocument doc = e.Document;
            if (activeDocument != null)
            {
                activeDocument.ActiveSegmentChanged -= ActiveDocument_ActiveSegmentChanged;
            }
            activeDocument = doc;
            studioProjectFolder = GetProjectFolder(activeDocument);
            if (activeDocument != null)
            {
                activeDocument.ActiveSegmentChanged += ActiveDocument_ActiveSegmentChanged;
                //activeDocument.ContentChanged += ActiveDocument_ContentChanged;
               
            }
        }

        private void ActiveDocument_ActiveSegmentChanged(object sender, EventArgs e)
        {
            activeSegment = activeDocument?.GetActiveSegmentPair();
            Target = activeSegment?.Target.ToString();
            Source = activeSegment?.Source.ToString();
            if (IsFileAttached && activeSegment != null)
            {
                Tu = FindTranslationUnit(activeSegment.Properties.Id.Id, activeSegment.GetProjectFile().Id);
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string GetProjectFolder()
        {
            return Path.GetDirectoryName(editorController?.ActiveDocument?.Project.FilePath);
        }

        private string GetFileStatusBarLabel()
        {
            string status = (IsFileAttached) ? "ATTACHED" : "DETACHED";
            return JsonFile + " : " + status;
        }
            
        private string GetProjectFolder(IStudioDocument activeDocument)
        {
            return Path.GetDirectoryName(activeDocument?.Project.FilePath);
        }

        private TranslationUnit FindTranslationUnit(string segmentId, Guid fileGuid)
        {
            return TranslationUnits.FirstOrDefault(tu => tu.SegmentId == segmentId && tu.FileId == fileGuid);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
