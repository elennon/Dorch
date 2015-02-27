﻿using Dorch.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Dorch.View
{
 
    public sealed partial class AddPlayer : BindablePage
    {
       
        private NavigationHelper navigationHelper;
        public static AddPlayer addPlayerPage;
        public TextBox txtNme, txtPhone;
        public Popup popup;

        public AddPlayer()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            addPlayerPage = this;
            txtPhone = txtPhoneNo; txtNme = txtPlayerName;
            popup = ppPopup;
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }
       
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

    }
}
