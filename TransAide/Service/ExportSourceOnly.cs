using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TransAide.Enum;

namespace TransAide.Service
{
    public class ExportSourceOnly
    {
        public static string GetSourceSegmentsText(EditorController editorController, TextFormatEnum textFormat, SourceSegmentTypeEnum sourceSegmentType)
        {
            string sourceText = "";
            if (editorController.ActiveDocument == null)
            {
                MessageBox.Show("There is no document loaded in the editor");
                return "";
            }

            var segmentPairs = (sourceSegmentType == SourceSegmentTypeEnum.FilteredSegments) ? editorController.ActiveDocument.FilteredSegmentPairs : editorController.ActiveDocument.SegmentPairs;

            foreach (var segmentPair in segmentPairs)
            {
                sourceText += IsSegmentToBeExported(segmentPair, sourceSegmentType) ? GetSourceText(segmentPair, textFormat) + Environment.NewLine : "";
            }

            return sourceText;
        }

        private static bool IsSegmentToBeExported (ISegmentPair segmentPair, SourceSegmentTypeEnum sourceSegmentType)
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

        private static string GetSourceText (ISegmentPair segmentPair, TextFormatEnum textFormat)
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
    }
}
