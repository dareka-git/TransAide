using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Implementation;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.MultiTerm.TMO.Interop;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TransAide.Enum;

namespace TransAide.Service
{
    public class ImportTargetOnly
    {
        public static int SetTargetSegmentsText(EditorController editorController, 
            DestinationSegmentTypeEnum destinationSegmentType, ImportFileContentEnum importFileContent,
            ImportFromEnum importFrom, ImportToEnum importTo,
            List<string> targets)
        {
            if (editorController.ActiveDocument == null)
            {
                MessageBox.Show("There is no document loaded in the editor");
                return 0;
            }
            if (targets == null || targets.Count() == 0)
            {
                MessageBox.Show("There is no text to import");
                return 0;
            }
            int targetIndex = 0;
            string activeSgmentPairId = editorController.ActiveDocument.ActiveSegmentPair.Properties.Id.Id;
            Guid activeSgmentPairFileId = editorController.ActiveDocument.ActiveSegmentPair.GetProjectFile().Id;
            int targetsCount = targets.Count();
            
            int segmentPairsCount = (destinationSegmentType == DestinationSegmentTypeEnum.FilteredSegments && importFileContent == ImportFileContentEnum.OnlySegmentsToImport) 
                        ? editorController.ActiveDocument.FilteredSegmentPairsCount : editorController.ActiveDocument.SegmentPairs.Count();
            var segmentPairs = (destinationSegmentType == DestinationSegmentTypeEnum.FilteredSegments && importFileContent == ImportFileContentEnum.OnlySegmentsToImport) 
                        ? editorController.ActiveDocument.FilteredSegmentPairs : editorController.ActiveDocument.SegmentPairs;
            int startImportIndex = (importFrom == ImportFromEnum.FirstSegment) ? 0 : segmentPairsCount + 1;
            int endImportIndex = (importTo == ImportToEnum.Next50Segments && importFrom == ImportFromEnum.FirstSegment) ? startImportIndex + 50 : 0;
            endImportIndex = (importTo == ImportToEnum.Next100Segments && importFrom == ImportFromEnum.FirstSegment) ? startImportIndex + 100 : endImportIndex;
            endImportIndex = (importTo == ImportToEnum.ToEnd && importFrom == ImportFromEnum.FirstSegment) ? segmentPairsCount : endImportIndex;

            List<Tuple<int, int>> filteredSegmentIndexes = (destinationSegmentType == DestinationSegmentTypeEnum.FilteredSegments && importFileContent == ImportFileContentEnum.OnlySegmentsToImport)
                        ? GetFilteredSegmentIndexes(editorController.ActiveDocument.SegmentPairs, editorController.ActiveDocument.FilteredSegmentPairs) : new List<Tuple<int, int>>();


            for (var i = 0; i < segmentPairsCount; i++)
            {
                ISegmentPair segmentPair = segmentPairs.ElementAt(i);
                if (importFrom == ImportFromEnum.CurrentSegment && IsCurrentSegment(activeSgmentPairId, activeSgmentPairFileId, segmentPair))
                {
                    startImportIndex = i;
                    //

                    endImportIndex = (importTo == ImportToEnum.Next50Segments) ? startImportIndex + 50 : endImportIndex;
                    endImportIndex = (importTo == ImportToEnum.Next100Segments) ? startImportIndex + 100 : endImportIndex;
                    endImportIndex = (importTo == ImportToEnum.ToEnd) ? segmentPairsCount : endImportIndex;
                }
                // uzupełnić
                if (IsSegmentTypeToBeImported(segmentPair, destinationSegmentType) 
                    && IsInImportRange(i, startImportIndex, endImportIndex)
                    && IsTextToImport(targetsCount, i, targetIndex, importFileContent))
                {
                    segmentPair.Target.Clear();
                    string textToImport = (importFileContent == ImportFileContentEnum.OnlySegmentsToImport) ? targets[targetIndex] : targets[i];
                    segmentPair.Target.Add(CreateSegmentTextItem(textToImport));
                    editorController.ActiveDocument.UpdateSegmentPair(segmentPair);
                    targetIndex++;
                }
            }
            return targetIndex;
        }

        private static bool IsFilteredSegment(int allSegmentIndex, List<Tuple<int, int>>  filteredSegmentIndexes)
        {
            foreach (var index in filteredSegmentIndexes)
            {
                if (index.Item1 == allSegmentIndex)
                {
                    return true;
                }
            }
            return false;
        }

        private static List<Tuple<int, int>> GetFilteredSegmentIndexes(IEnumerable<ISegmentPair> allSegmentPairs, IEnumerable<ISegmentPair> filteredSegmentPairs)
        {
            List<Tuple<int, int>> indexes = new List<Tuple<int, int>>();
            int allIndex = 0;
            int filteredIndex = 0;

            foreach (var segmentPair in allSegmentPairs)
            {
                if (CompareSegmentPair(segmentPair, filteredSegmentPairs.ElementAt(filteredIndex)))
                {
                    indexes.Add(new Tuple<int, int>(allIndex, filteredIndex));
                    filteredIndex++;
                    allIndex++;
                }
                else
                {
                    allIndex++;
                }
            }
            return indexes;
        }


        private static bool CompareSegmentPair(ISegmentPair segmentPairA, ISegmentPair segmentPairB)
        {
            return segmentPairA.Equals(segmentPairB);
        }

        private static bool IsTextToImport(int targetsCount, int i, int targetIndex, ImportFileContentEnum importFileContent)
        {
            switch (importFileContent)
            {
                case ImportFileContentEnum.OnlySegmentsToImport:
                    return (targetIndex < targetsCount) ? true : false;
                case ImportFileContentEnum.AllSegments:
                    return (i < targetsCount) ? true : false;
                default:
                    return false;
            }
        }

        private static bool IsInImportRange(int currentIndex, int startImportIndex, int endImportIndex)
        {
            return ((currentIndex >= startImportIndex) && (currentIndex < endImportIndex)) ? true : false;
        }

        private static bool IsCurrentSegment(string activeSegmentId, Guid activeSegmentFileId, ISegmentPair segmentPair)
        {
            return segmentPair.Properties.Id.Id == activeSegmentId && segmentPair.GetProjectFile().Id == activeSegmentFileId;
        }

        private static bool IsSegmentTypeToBeImported(ISegmentPair segmentPair, DestinationSegmentTypeEnum destinationSegmentType)
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
                    return true;
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
