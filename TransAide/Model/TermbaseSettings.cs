using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TransAide.Model
{
    public class TermbaseSettings
    {
        public string Path { get; set; }
        public bool IsLocal { get; set; }
        public string SourceIndex { get; set; }
        public string TargetIndex { get; set; }
        public int MaximumHits { get; set; }
        public bool FuzzySearch { get; set; }

        public TermbaseSettings()
        {
            ProjectsController projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
            TermbaseConfiguration termbaseConfiguration = GetTermbaseConfiguration(projectsController);
            EditorController editorController = SdlTradosStudio.Application.GetController<EditorController>();
            IStudioDocument activeDocument = editorController?.ActiveDocument;
            Language sourceLanguage = activeDocument.ActiveFile?.SourceFile.Language;
            Language targetLanguage = activeDocument.ActiveFile?.Language;
            MaximumHits = 20;
            FuzzySearch = true;
            Path = GetTermbasePath(termbaseConfiguration);
            IsLocal = IsTermbaseLocal(termbaseConfiguration);
            SourceIndex = GetTermbaseIndex(termbaseConfiguration, sourceLanguage);
            TargetIndex = GetTermbaseIndex(termbaseConfiguration, targetLanguage);
        }

        public TermbaseSettings(ProjectsController projectsController, EditorController editorController)
        {
            TermbaseConfiguration termbaseConfiguration = GetTermbaseConfiguration(projectsController);
            IStudioDocument activeDocument = editorController?.ActiveDocument;
            Language sourceLanguage = activeDocument.ActiveFile?.SourceFile.Language;
            Language targetLanguage = activeDocument.ActiveFile?.Language;
            MaximumHits = 20;
            FuzzySearch = true;
            Path = GetTermbasePath(termbaseConfiguration);
            IsLocal = IsTermbaseLocal(termbaseConfiguration);
            SourceIndex = GetTermbaseIndex(termbaseConfiguration, sourceLanguage);
            TargetIndex = GetTermbaseIndex(termbaseConfiguration, targetLanguage);
        }


        public TermbaseSettings(Language sourceLanguage, Language targetLanguage)
        {
            ProjectsController projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
            TermbaseConfiguration termbaseConfiguration = GetTermbaseConfiguration(projectsController);
            MaximumHits = 20;
            FuzzySearch = true;
            Path = GetTermbasePath(termbaseConfiguration);
            IsLocal = IsTermbaseLocal(termbaseConfiguration);
            SourceIndex = GetTermbaseIndex(termbaseConfiguration, sourceLanguage);
            TargetIndex = GetTermbaseIndex(termbaseConfiguration, targetLanguage);
        }

        private string GetTermbaseIndex(TermbaseConfiguration termbaseConfiguration, Language language)
        {
            List<TermbaseLanguageIndex> termbaseIndexes = termbaseConfiguration.LanguageIndexes;
            if (termbaseIndexes.Any())
            {
                var termbaseIndex = termbaseIndexes.FirstOrDefault(t => t.ProjectLanguage.CultureInfo.Name.Equals(language.CultureInfo.Name));
                if (termbaseIndex != null)
                {
                    return termbaseIndex.TermbaseIndex;
                }
            }
            return string.Empty;
        }

        public TermbaseConfiguration GetTermbaseConfiguration(ProjectsController projectsController)
        {
            FileBasedProject activeProject = projectsController?.CurrentProject;
            return activeProject?.GetTermbaseConfiguration();
        }

        private string GetTermbasePath(TermbaseConfiguration termbaseConfiguration)
        {
            string termbaseSettingsXml = termbaseConfiguration?.Termbases.FirstOrDefault()?.SettingsXML;
            if (!string.IsNullOrEmpty(termbaseSettingsXml))
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(termbaseSettingsXml);
                XmlNodeList xnListPath = xml.SelectNodes("/TermbaseSettings/Path");
                if (xnListPath?.Count > 0)
                {
                    return xnListPath[0].InnerText;
                }
            }
            return string.Empty;
        }

        private bool IsTermbaseLocal(TermbaseConfiguration termbaseConfiguration)
        {
            string termbaseSettingsXml = termbaseConfiguration?.Termbases.FirstOrDefault()?.SettingsXML;
            if (!string.IsNullOrEmpty(termbaseSettingsXml))
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(termbaseSettingsXml);
                XmlNodeList xnListLocal = xml.SelectNodes("/TermbaseSettings/Local");
                if (xnListLocal?.Count > 0)
                {
                    return bool.Parse(xnListLocal[0].InnerText);
                }
            }
            return false;
        }
    }
}
