using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using TransAide.Enum;
using TransAide.Extension;
using TransAide.Model;
using TransAide.Model.Multiterm;

namespace TransAide.Service
{
    public class ExportGlossaryTerms
    {
        //List<GlossaryTerm> glossaryTerms;
        public static GlossaryTermCollection GetTerms(SourceSegmentTypeEnum sourceSegmentType)
        {
            EditorController editorController = SdlTradosStudio.Application.GetController<EditorController>();
            if (editorController.ActiveDocument == null)
            {
                MessageBox.Show("There is no document loaded in the editor");
                return null;
            }
            ProjectsController projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
            TermbaseSettings termbaseSettings = new TermbaseSettings(projectsController, editorController);
            MultitermProvider terminologyProvider = new MultitermProvider(termbaseSettings.Path);
            GlossaryTermCollection glossaryTerms = new GlossaryTermCollection();

            var segmentPairs = (sourceSegmentType == SourceSegmentTypeEnum.FilteredSegments) ? editorController.ActiveDocument.FilteredSegmentPairs : editorController.ActiveDocument.SegmentPairs;

            foreach (var segmentPair in segmentPairs)
            {
                if (IsSegmentToBeExported(segmentPair, sourceSegmentType))
                {
                    string sourceText = GetSourceText(segmentPair, TextFormatEnum.PlainTextWithoutTags);
                    glossaryTerms.AddGlossaryTerms(SearchGlossaryTerms(sourceText, terminologyProvider, termbaseSettings));
                }
            }
            return glossaryTerms;
        }

        public static List<GlossaryTerm> SearchGlossaryTerms(string source, MultitermProvider terminologyProvider,
            TermbaseSettings termbaseSettings)
        {
            List<GlossaryTerm> glossaryTerms = new List<GlossaryTerm>();
            List<TermHit> termHits = terminologyProvider.SearchTerm(source, termbaseSettings.FuzzySearch,
                termbaseSettings.MaximumHits, termbaseSettings.SourceIndex, termbaseSettings.TargetIndex);

            foreach (TermHit termHit in termHits)
            {
                foreach (TermEntry termEntrySource in termHit.TermEntries)
                {
                    if (termEntrySource.Language.Equals(termbaseSettings.SourceIndex) && !IsForbidden(termEntrySource.TermFields))
                    {
                        List<string> forbiddenTargetTerms = new List<string>();
                        List<string> requiredTargetTerms = new List<string>();
                        
                        
                        foreach (TermEntry termEntryTarget in termHit.TermEntries)
                        {
                            if (termEntryTarget.Language.Equals(termbaseSettings.TargetIndex))
                            {
                                if (IsForbidden(termEntryTarget.TermFields))
                                {
                                    forbiddenTargetTerms.Add(termEntryTarget.Term);
                                }
                                else
                                {
                                    requiredTargetTerms.Add(termEntryTarget.Term);
                                }
                            }
                        }
                        if (forbiddenTargetTerms.Count > 0)
                        {
                            glossaryTerms.Add(new GlossaryTerm(termEntrySource.Term, forbiddenTargetTerms, true));
                        }
                        if (requiredTargetTerms.Count > 0)
                        {
                            glossaryTerms.Add(new GlossaryTerm(termEntrySource.Term, requiredTargetTerms));
                        }
                    }
                }
            }
            return glossaryTerms;
        }


        public static TermbaseConfiguration GetTermbaseConfiguration(ProjectsController projectsController)
        {
            FileBasedProject activeProject = projectsController?.CurrentProject;
            return activeProject?.GetTermbaseConfiguration();
        }

        public static string GetTermbasePath(TermbaseConfiguration termbaseConfiguration)
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


        public static bool IsTermbaseLocal(TermbaseConfiguration termbaseConfiguration)
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


        public static bool IsSegmentToBeExported(ISegmentPair segmentPair, SourceSegmentTypeEnum sourceSegmentType)
        {
            switch (sourceSegmentType)
            {
                case SourceSegmentTypeEnum.UnconfirmedSegments:
                    return (segmentPair.Properties.ConfirmationLevel == ConfirmationLevel.Draft || segmentPair.Properties.ConfirmationLevel == ConfirmationLevel.Unspecified);
                case SourceSegmentTypeEnum.UnlockedSegments:
                    return !segmentPair.Properties.IsLocked;
                case SourceSegmentTypeEnum.AllSegments:
                    return true;
                case SourceSegmentTypeEnum.FilteredSegments:
                    return true;
                default:
                    return false;
            }
        }

        public static string GetSourceText(ISegmentPair segmentPair, TextFormatEnum textFormat)
        {
            SegmentVisitor visitor = new SegmentVisitor();
            switch (textFormat)
            {
                case TextFormatEnum.PlainTextWithoutTags:
                    // return segmentPair.Source.AsSimpleText();
                    return visitor.GetPlainText(segmentPair.Source, true);
                case TextFormatEnum.TextWithTags:
                    return visitor.GetPlainText(segmentPair.Source, true);
                default:
                    return "";
            }
        }

        public static string GetTermbaseIndex(TermbaseConfiguration termbaseConfiguration, Language language)
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

        private static bool IsForbidden(List<EntryField> entryFields)
        {
            foreach (var entryField in entryFields)
            {
                if (entryField.Name.Contains("status", StringComparison.OrdinalIgnoreCase) && entryField.Value.IsForbidden())
                {
                    return true;
                }

            }
            return false;
        }

    } 
}
