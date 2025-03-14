using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TransAide.View;

namespace TransAide
{
    [Action("TransAideExportAction", typeof(EditorController), Name = "Export Segments", Icon = "arrow_blue_up_icon")]
    [ActionLayout(typeof(TransAideRibbonGroup), 11, DisplayType.Large)]
    //[Shortcut(Keys.Alt | Keys.F8)]
    public class TransAideExportAction : AbstractViewControllerAction<EditorController>
    {
        public override void Initialize()
        {
        }
        protected override void Execute()
        {
            ExportView exportView = new ExportView();
            exportView.ShowDialog();
        }
    }

    [Action("TransAideImportAction", typeof(EditorController), Name = "Import Segments", Icon = "arrow_blue_down_icon")]
    [ActionLayout(typeof(TransAideRibbonGroup), 15, DisplayType.Large)]
    //[Shortcut(Keys.Alt | Keys.F8)]
    public class TransAideImportAction : AbstractViewControllerAction<EditorController>
    {
        public override void Initialize()
        {
        }
        protected override void Execute()
        {
            ImportView importView = new ImportView();
            importView.ShowDialog();
        }
    }

    [Action("TransAideTermsExportAction", typeof(EditorController), Name = "Export Terms", Icon = "arrow_orange_up_icon")]
    [ActionLayout(typeof(TransAideRibbonGroup), 13, DisplayType.Large)]
    //[Shortcut(Keys.Alt | Keys.F8)]
    public class TransAideTermsExportAction : AbstractViewControllerAction<EditorController>
    {
        public override void Initialize()
        {
        }
        protected override void Execute()
        {
            TermsExportView termsExportView = new TermsExportView();
            termsExportView.ShowDialog();
        }
    }
   

}
