using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransAide.Service
{
    public class UpdateTargetSegment
    {
        public static void ApplyTarget(EditorController editorController, string newTargetText)
        {
            IStudioDocument activeDocument = editorController.ActiveDocument;
            ISegmentPair activeSgmentPair = activeDocument.ActiveSegmentPair;
            
            ITranslationOrigin currentTranslationOrigin = activeSgmentPair.Properties.TranslationOrigin;
            ITranslationOrigin newTranslationOrigin = CreateTranslationOriginItem("TransAIde", DefaultTranslationOrigin.Unknown, 49);
            newTranslationOrigin.OriginBeforeAdaptation = currentTranslationOrigin;
            
            activeSgmentPair.Target.Clear();
            activeSgmentPair.Target.Add(CreateSegmentTextItem(newTargetText));
            activeSgmentPair.Properties.TranslationOrigin = newTranslationOrigin;
            
            editorController.ActiveDocument.UpdateSegmentPair(activeSgmentPair);
        }

        private static IText CreateSegmentTextItem(string text)
        {
            IDocumentItemFactory documentItemFactory = DefaultDocumentItemFactory.CreateInstance();
            ITextProperties textProperties = documentItemFactory.PropertiesFactory.CreateTextProperties(text);
            return documentItemFactory.CreateText(textProperties);
        }

        private static ITranslationOrigin CreateTranslationOriginItem(string originSystem, string originType, byte matchPercent)
        {
            IDocumentItemFactory documentItemFactory = DefaultDocumentItemFactory.CreateInstance();
            ISegmentPairProperties segmentPairProperties = documentItemFactory.CreateSegmentPairProperties();
            ITranslationOrigin translationOrigin = documentItemFactory.CreateTranslationOrigin();
            translationOrigin.OriginSystem = originSystem;
            translationOrigin.OriginType = originType;
            translationOrigin.MatchPercent = matchPercent;
            return translationOrigin;
        }
    }
}
