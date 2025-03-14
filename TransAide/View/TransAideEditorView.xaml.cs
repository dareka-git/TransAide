using Sdl.Desktop.IntegrationApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TransAide.View
{
    /// <summary>
    /// Interaction logic for TransAideEditorView.xaml
    /// </summary>
    public partial class TransAideEditorView : UserControl, IUIControl
    {
        public TransAideEditorView()
        {
            InitializeComponent();
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
