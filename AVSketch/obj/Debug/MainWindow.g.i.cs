﻿#pragma checksum "..\..\MainWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "78F1734D5FB8ACDA591A5B7237189AFB2C82C1FF99C32E263C5DC3582342CAA4"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using AVSketch;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace AVSketch {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid mainGrid;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button pan_selector;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button shape_selector;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button line_selector;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button text_selector;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button transform_selector;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button delete_selector;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button colour_selector;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image imageContainer;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/AVSketch;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 8 "..\..\MainWindow.xaml"
            ((AVSketch.MainWindow)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            
            #line 8 "..\..\MainWindow.xaml"
            ((AVSketch.MainWindow)(target)).SizeChanged += new System.Windows.SizeChangedEventHandler(this.Window_SizeChanged);
            
            #line default
            #line hidden
            
            #line 8 "..\..\MainWindow.xaml"
            ((AVSketch.MainWindow)(target)).TextInput += new System.Windows.Input.TextCompositionEventHandler(this.Window_TextInput);
            
            #line default
            #line hidden
            
            #line 8 "..\..\MainWindow.xaml"
            ((AVSketch.MainWindow)(target)).PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.Window_PreviewKeyDown);
            
            #line default
            #line hidden
            
            #line 8 "..\..\MainWindow.xaml"
            ((AVSketch.MainWindow)(target)).PreviewKeyUp += new System.Windows.Input.KeyEventHandler(this.Window_PreviewKeyUp);
            
            #line default
            #line hidden
            return;
            case 2:
            this.mainGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            this.pan_selector = ((System.Windows.Controls.Button)(target));
            
            #line 27 "..\..\MainWindow.xaml"
            this.pan_selector.Click += new System.Windows.RoutedEventHandler(this.Pan_selector_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.shape_selector = ((System.Windows.Controls.Button)(target));
            
            #line 28 "..\..\MainWindow.xaml"
            this.shape_selector.Click += new System.Windows.RoutedEventHandler(this.Shape_selector_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.line_selector = ((System.Windows.Controls.Button)(target));
            
            #line 29 "..\..\MainWindow.xaml"
            this.line_selector.Click += new System.Windows.RoutedEventHandler(this.Line_selector_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.text_selector = ((System.Windows.Controls.Button)(target));
            
            #line 30 "..\..\MainWindow.xaml"
            this.text_selector.Click += new System.Windows.RoutedEventHandler(this.Text_selector_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.transform_selector = ((System.Windows.Controls.Button)(target));
            
            #line 31 "..\..\MainWindow.xaml"
            this.transform_selector.Click += new System.Windows.RoutedEventHandler(this.Transform_selector_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.delete_selector = ((System.Windows.Controls.Button)(target));
            
            #line 32 "..\..\MainWindow.xaml"
            this.delete_selector.Click += new System.Windows.RoutedEventHandler(this.Delete_selector_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.colour_selector = ((System.Windows.Controls.Button)(target));
            
            #line 33 "..\..\MainWindow.xaml"
            this.colour_selector.Click += new System.Windows.RoutedEventHandler(this.Colour_selector_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            this.imageContainer = ((System.Windows.Controls.Image)(target));
            
            #line 39 "..\..\MainWindow.xaml"
            this.imageContainer.MouseMove += new System.Windows.Input.MouseEventHandler(this.Image_MouseMove);
            
            #line default
            #line hidden
            
            #line 39 "..\..\MainWindow.xaml"
            this.imageContainer.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.ImageContainer_MouseLeftButtonDown);
            
            #line default
            #line hidden
            
            #line 39 "..\..\MainWindow.xaml"
            this.imageContainer.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.ImageContainer_MouseLeftButtonUp);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

