﻿using PrismSample.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;
using Prism.Regions;

namespace PrismSample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Container.Resolve<IRegionManager>().RequestNavigate("ContentRegion", nameof(SamplePage));
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<SamplePage>(nameof(SamplePage));
        }
    }
}
