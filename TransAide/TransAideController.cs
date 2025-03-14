using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Extensions;
using System;
using System.Collections.Generic;
using TransAide.View;
using TransAide.ViewModel;

namespace TransAide
{
    [ViewPart(
        Id = "TransAideEditorView",
        Name = "TransAide View",
        Description = "Exports and Imports segments to and from AI models",
        Icon = "transaide_icon")]
    [ViewPartLayout(typeof(EditorController), Dock = DockType.Bottom)]
    public class TransAideController : AbstractViewPartController
    {
        private TransAideEditorView control;
        private EditorController editorController;
        private ProjectsController projectsController;
        protected override IUIControl GetContentControl()
        {
            return control;
        }

        protected override void Initialize()
        {
            editorController = SdlTradosStudio.Application.GetController<EditorController>();
            projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
            //var dataContext = new EditorViewPartModel();
            var viewModel = new TransAideEditorViewModel(editorController, projectsController);
            control = new TransAideEditorView { DataContext = viewModel };
        }
    }
}
