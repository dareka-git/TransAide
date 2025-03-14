﻿using System;
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
using TransAide.ViewModel;

namespace TransAide.View
{
    /// <summary>
    /// Interaction logic for TermsExportView.xaml
    /// </summary>
    public partial class TermsExportView : Window
    {
        public TermsExportView()
        {
            InitializeComponent();
            this.DataContext = new TermsExportViewModel();
        }
    }
}
