﻿#pragma checksum "D:\Code\wallhaven\wallhaven\Views\ViewWallpaper.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "3B63BDFC762962D694EAC2B9897E7FCE"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace wallhaven {
    
    
    public partial class ViewWallpaper : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.Primitives.ViewportControl Viewport;
        
        internal System.Windows.Controls.Canvas Canvas;
        
        internal System.Windows.Controls.Image SelectedImageViewer;
        
        internal System.Windows.Media.ScaleTransform xform;
        
        internal System.Windows.Controls.StackPanel GettingOriginalImageOverlay;
        
        internal System.Windows.Controls.ProgressBar TagOverlayProgressBar;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/wallhaven;component/Views/ViewWallpaper.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.Viewport = ((System.Windows.Controls.Primitives.ViewportControl)(this.FindName("Viewport")));
            this.Canvas = ((System.Windows.Controls.Canvas)(this.FindName("Canvas")));
            this.SelectedImageViewer = ((System.Windows.Controls.Image)(this.FindName("SelectedImageViewer")));
            this.xform = ((System.Windows.Media.ScaleTransform)(this.FindName("xform")));
            this.GettingOriginalImageOverlay = ((System.Windows.Controls.StackPanel)(this.FindName("GettingOriginalImageOverlay")));
            this.TagOverlayProgressBar = ((System.Windows.Controls.ProgressBar)(this.FindName("TagOverlayProgressBar")));
        }
    }
}

