﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using EmotiPal.Views;
using MvvmHelpers;
using Xamarin.Forms;

namespace EmotiPal.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private readonly INavigation navigation;
        public MainPageViewModel(INavigation navigation)
        {
            this.navigation = navigation;
        }

        public ICommand AnalyseTextCommand
        {
            get => new Command(() =>
            {
                navigation.PushAsync(new NavigationPage(new AnalyseTextPage()));
            });
        }

        public ICommand AnalyseBodyLanguageCommand
        {
            get => new Command(() =>
            {
                navigation.PushAsync(new NavigationPage(new AnalyseBodyLanguagePage()));
               
            });
        }
    }
}
