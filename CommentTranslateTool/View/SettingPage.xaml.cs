﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Workshop.ViewModel;

namespace Workshop.View
{
    /// <summary>
    /// SettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingPage : Page
    {
        public SettingPage()
        {
            InitializeComponent();
            this.Unloaded += SettingPage_Unloaded;
        }

        private void SettingPage_Unloaded(object sender, RoutedEventArgs e)
        {
            ViewModelLocator.Cleanup<SettingPageViewModel>();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var vm = this.DataContext as SettingPageViewModel;
            vm.RaiseSettingChanged();
        }
    }
}
