using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TransAide.Enum;
using TransAide.Model;

namespace TransAide.Service
{
    public class ImportJSON
    {
        public static int SetTargetSegmentsText(EditorController editorController,
            DestinationSegmentTypeEnum destinationSegmentType, ImportFileContentEnum importFileContent,
            ImportFromEnum importFrom, ImportToEnum importTo,
            List<TranslationUnit> translationUnits)
        {
            if (editorController.ActiveDocument == null)
            {
                MessageBox.Show("There is no document loaded in the editor");
                return 0;
            }
            if (translationUnits == null || translationUnits.Count() == 0)
            {
                MessageBox.Show("There is no text to import");
                return 0;
            }
            int targetIndex = 0;
            int translationUnitsCount = translationUnits.Count();

            IStudioDocument activeDocument = editorController.ActiveDocument;
            ISegmentPair activeSgmentPair = activeDocument.ActiveSegmentPair;

            List<int> filteredSegmentIndexes = (destinationSegmentType == DestinationSegmentTypeEnum.FilteredSegments) ?
                GetFilteredSegmentIndexes(activeDocument.SegmentPairs, activeDocument.FilteredSegmentPairs) : new List<int>();


            IEnumerable<ISegmentPair> segmentPairs = activeDocument.SegmentPairs;
            int segmentPairsCount = segmentPairs.Count();

            int startImportIndex = GetImportStartIndex(importFrom, segmentPairsCount);
            int endImportIndex = GetImportEndIndex(importFrom, importTo, startImportIndex, segmentPairsCount);

            for (var i = 0; i < segmentPairsCount; i++)
            {
                ISegmentPair segmentPair = segmentPairs.ElementAt(i);
                if (importFrom == ImportFromEnum.CurrentSegment && IsSegmentEqual(segmentPair, activeSgmentPair))
                {
                    startImportIndex = i;
                    endImportIndex = UpdateImportEndIndex(importFrom, importTo, startImportIndex, endImportIndex, segmentPairsCount);
                }

                if (IsSegmentTypeToBeImported(segmentPair, destinationSegmentType, i, filteredSegmentIndexes)
                    && IsInImportRange(i, startImportIndex, endImportIndex)
                    && IsTextToImport(translationUnitsCount, i, targetIndex, importFileContent))
                {
                    if (!segmentPair.Properties.IsLocked) { 
                        string textToImport = GetTextToImport(segmentPair.Properties.Id.Id, segmentPair.GetProjectFile().Id, translationUnits);
                        if (textToImport == "") { break; }
                        segmentPair.Target.Clear();
                        segmentPair.Target.Add(CreateSegmentTextItem(textToImport));
                        //segmentPair.Properties.TranslationOrigin = CreateTranslationOriginItem();
                        editorController.ActiveDocument.UpdateSegmentPair(segmentPair);
                    }
                    targetIndex++;
                }
            }
            return targetIndex;
        }


        private static List<int> GetFilteredSegmentIndexes(IEnumerable<ISegmentPair> allSegmentPairs, IEnumerable<ISegmentPair> filteredSegmentPairs)
        {
            List<int> indexes = new List<int>();
            int allIndex = 0;
            int filteredIndex = 0;

            foreach (var segmentPair in allSegmentPairs)
            {
                if (IsSegmentEqual(segmentPair, filteredSegmentPairs.ElementAt(filteredIndex)))
                {
                    indexes.Add(allIndex);
                    filteredIndex++;
                    allIndex++;
                    if (filteredIndex >= filteredSegmentPairs.Count())
                    {
                        break;
                    }
                }
                else
                {
                    allIndex++;
                }
            }
            return indexes;
        }


        private static int GetImportStartIndex(ImportFromEnum importFrom, int segmentPairsCount)
        {
            return (importFrom == ImportFromEnum.FirstSegment) ? 0 : segmentPairsCount + 1;
        }

        private static int GetImportEndIndex(ImportFromEnum importFrom, ImportToEnum importTo, int startImportIndex, int segmentPairsCount)
        {
            int endImportIndex = (importTo == ImportToEnum.Next50Segments && importFrom == ImportFromEnum.FirstSegment) ? startImportIndex + 50 : 0;
            endImportIndex = (importTo == ImportToEnum.Next100Segments && importFrom == ImportFromEnum.FirstSegment) ? startImportIndex + 100 : endImportIndex;
            endImportIndex = (importTo == ImportToEnum.ToEnd && importFrom == ImportFromEnum.FirstSegment) ? segmentPairsCount : endImportIndex;
            return endImportIndex;
        }

        private static int UpdateImportEndIndex(ImportFromEnum importFrom, ImportToEnum importTo, int startImportIndex, int endImportIndex, int segmentPairsCount)
        {
            endImportIndex = (importTo == ImportToEnum.Next50Segments) ? startImportIndex + 50 : endImportIndex;
            endImportIndex = (importTo == ImportToEnum.Next100Segments) ? startImportIndex + 100 : endImportIndex;
            endImportIndex = (importTo == ImportToEnum.ToEnd) ? segmentPairsCount : endImportIndex;
            return endImportIndex;
        }


        private static string GetTextToImport(string segmentId, Guid fileId, List<TranslationUnit> translationUnits)
        {
            var translationUnit = translationUnits.FirstOrDefault(tu => tu.SegmentId == segmentId && tu.FileId == fileId);
            return translationUnit?.Target ?? string.Empty;
        }


        private static bool IsSegmentEqual(ISegmentPair segmentPairA, ISegmentPair segmentPairB)
        {
            return ((segmentPairA.GetProjectFile().Id == segmentPairB.GetProjectFile().Id)
                && (segmentPairA.Properties.Id.Id == segmentPairB.Properties.Id.Id));
        }

        private static bool IsTextToImport(int translationUnitsCount, int i, int targetIndex, ImportFileContentEnum importFileContent)
        {
            switch (importFileContent)
            {
                case ImportFileContentEnum.OnlySegmentsToImport:
                    return (targetIndex < translationUnitsCount);
                case ImportFileContentEnum.AllSegments:
                    return (i < translationUnitsCount);
                default:
                    return false;
            }
        }

        private static bool IsInImportRange(int currentIndex, int startImportIndex, int endImportIndex)
        {
            return ((currentIndex >= startImportIndex) && (currentIndex < endImportIndex));
        }

        private static bool IsCurrentSegment(string activeSegmentId, Guid activeSegmentFileId, ISegmentPair segmentPair)
        {
            return segmentPair.Properties.Id.Id == activeSegmentId && segmentPair.GetProjectFile().Id == activeSegmentFileId;
        }

        private static bool IsSegmentTypeToBeImported(ISegmentPair segmentPair, DestinationSegmentTypeEnum destinationSegmentType, int i, List<int> filteredSegmentIndexes)
        {
            switch (destinationSegmentType)
            {
                case DestinationSegmentTypeEnum.UnconfirmedSegments:
                    return (segmentPair.Properties.ConfirmationLevel == ConfirmationLevel.Draft || segmentPair.Properties.ConfirmationLevel == ConfirmationLevel.Unspecified);
                case DestinationSegmentTypeEnum.UnlockedSegments:
                    return !segmentPair.Properties.IsLocked;
                case DestinationSegmentTypeEnum.AllSegments:
                    return true;
                case DestinationSegmentTypeEnum.FilteredSegments:
                    return filteredSegmentIndexes.Contains(i); ;
                default:
                    return false;
            }
        }

        private static IText CreateSegmentTextItem(string text)
        {
            IDocumentItemFactory documentItemFactory = DefaultDocumentItemFactory.CreateInstance();
            ITextProperties textProperties = documentItemFactory.PropertiesFactory.CreateTextProperties(text);
            return documentItemFactory.CreateText(textProperties);
        }

    }
}
