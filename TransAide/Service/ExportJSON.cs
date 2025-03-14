using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
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
    public class ExportJSON
    {
        public static List<TranslationUnit> GetTranslationUnits(EditorController editorController, TextFormatEnum textFormat, SourceSegmentTypeEnum sourceSegmentType)
        {
            List<TranslationUnit> translationUnits = new List<TranslationUnit>();
            if (editorController.ActiveDocument == null)
            {
                MessageBox.Show("There is no document loaded in the editor");
                return null;
            }

            var segmentPairs = (sourceSegmentType == SourceSegmentTypeEnum.FilteredSegments) ? editorController.ActiveDocument.FilteredSegmentPairs : editorController.ActiveDocument.SegmentPairs;

            foreach (var segmentPair in segmentPairs)
            {
                if (IsSegmentToBeExported(segmentPair, sourceSegmentType))
                {
                    TranslationUnit translationUnit = new TranslationUnit();
                    translationUnit.SegmentId = segmentPair.Properties.Id.Id;
                    translationUnit.FileId = segmentPair.GetProjectFile().Id;
                    translationUnit.FileName = segmentPair.GetProjectFile().Name;
                    translationUnit.Match = segmentPair.Properties.TranslationOrigin.MatchPercent;
                    translationUnit.Status = SegmentStatusConverter(segmentPair.Properties.ConfirmationLevel, 
                        segmentPair.Properties.TranslationOrigin.MatchPercent);
                    translationUnit.Source = GetSourceText(segmentPair, textFormat);
                    translationUnit.Target = (translationUnit.Status == SegmentStatusEnum.Fuzzy || translationUnit.Status == SegmentStatusEnum.Translated)
                        ? GetTargetText(segmentPair, textFormat) : "";
                    translationUnits.Add(translationUnit);
                }
            }
            return translationUnits;
        }
        private static bool IsSegmentToBeExported(ISegmentPair segmentPair, SourceSegmentTypeEnum sourceSegmentType)
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
        private static string GetSourceText(ISegmentPair segmentPair, TextFormatEnum textFormat)
        {
            SegmentVisitor visitor = new SegmentVisitor();
            switch (textFormat)
            {
                case TextFormatEnum.PlainTextWithoutTags:
                    // return segmentPair.Source.AsSimpleText();
                    return visitor.GetPlainText(segmentPair.Source, false);
                case TextFormatEnum.TextWithTags:
                    return visitor.GetPlainText(segmentPair.Source, true);
                default:
                    return "";
            }
        }

        private static string GetTargetText(ISegmentPair segmentPair, TextFormatEnum textFormat)
        {
            SegmentVisitor visitor = new SegmentVisitor();
            switch (textFormat)
            {
                case TextFormatEnum.PlainTextWithoutTags:
                    // return segmentPair.Source.AsSimpleText();
                    return visitor.GetPlainText(segmentPair.Target, false);
                case TextFormatEnum.TextWithTags:
                    return visitor.GetPlainText(segmentPair.Target, true);
                default:
                    return "";
            }
        }

        private static SegmentStatusEnum SegmentStatusConverter (ConfirmationLevel confirmationLevel, int matchPercent)
        {
            switch (confirmationLevel)
            {
                case ConfirmationLevel.Draft:
                    return (matchPercent >= 75) ? SegmentStatusEnum.Fuzzy : SegmentStatusEnum.New;
                case ConfirmationLevel.Unspecified:
                    return SegmentStatusEnum.New;
                case ConfirmationLevel.ApprovedTranslation:
                    return SegmentStatusEnum.Translated;
                case ConfirmationLevel.Translated:
                    return SegmentStatusEnum.Translated;
                case ConfirmationLevel.ApprovedSignOff:
                    return SegmentStatusEnum.Translated;
                case ConfirmationLevel.RejectedSignOff:
                    return SegmentStatusEnum.Translated;
                case ConfirmationLevel.RejectedTranslation:
                    return SegmentStatusEnum.Translated;
                default:
                    return SegmentStatusEnum.New;
            }
        }

    }
}
