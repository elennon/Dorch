﻿

#pragma checksum "C:\Users\ed\Documents\Visual Studio 2013\Projects\Dorch\Dorch\View\ShowAllPlayers.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "7593609B0450E8ABBE70F3F087A23AD6"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Dorch.View
{
    partial class ShowAllPlayers : global::Dorch.Common.BindablePage
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Dorch.Converters.ItemClickedConverter ItemClickedConverter; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Dorch.Converters.BooleanToVisibilityConverter BooleanToVisibilityConverter; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Grid LayoutRoot; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Grid ContentRoot; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Image picc; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.SemanticZoom ArtistSemanticZoom; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.CommandBar appBar; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private bool _contentLoaded;

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent()
        {
            if (_contentLoaded)
                return;

            _contentLoaded = true;
            global::Windows.UI.Xaml.Application.LoadComponent(this, new global::System.Uri("ms-appx:///View/ShowAllPlayers.xaml"), global::Windows.UI.Xaml.Controls.Primitives.ComponentResourceLocation.Application);
 
            ItemClickedConverter = (global::Dorch.Converters.ItemClickedConverter)this.FindName("ItemClickedConverter");
            BooleanToVisibilityConverter = (global::Dorch.Converters.BooleanToVisibilityConverter)this.FindName("BooleanToVisibilityConverter");
            LayoutRoot = (global::Windows.UI.Xaml.Controls.Grid)this.FindName("LayoutRoot");
            ContentRoot = (global::Windows.UI.Xaml.Controls.Grid)this.FindName("ContentRoot");
            picc = (global::Windows.UI.Xaml.Controls.Image)this.FindName("picc");
            ArtistSemanticZoom = (global::Windows.UI.Xaml.Controls.SemanticZoom)this.FindName("ArtistSemanticZoom");
            appBar = (global::Windows.UI.Xaml.Controls.CommandBar)this.FindName("appBar");
        }
    }
}



