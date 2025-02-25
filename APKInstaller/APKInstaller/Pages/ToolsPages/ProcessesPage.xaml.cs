﻿using AdvancedSharpAdbClient;
using AdvancedSharpAdbClient.DeviceCommands;
using APKInstaller.Helpers;
using APKInstaller.ViewModels.ToolsPages;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace APKInstaller.Pages.ToolsPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProcessesPage : Page
    {
        private ProcessesViewModel Provider;

        public ProcessesPage() => InitializeComponent();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Provider = new ProcessesViewModel(this);
            DataContext = Provider;
            Provider.TitleBar = TitleBar;
            ADBHelper.Monitor.DeviceChanged += OnDeviceChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ADBHelper.Monitor.DeviceChanged -= OnDeviceChanged;
        }

        private void OnDeviceChanged(object sender, DeviceDataEventArgs e) => _ = DispatcherQueue.EnqueueAsync(() => Provider.GetDevices());

        private void TitleBar_BackRequested(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AdvancedAdbClient client = new AdvancedAdbClient();
            Provider.Processes = DeviceExtensions.ListProcesses(client, Provider.devices[(sender as ComboBox).SelectedIndex]);
        }

        private async void TitleBar_RefreshEvent(object sender, RoutedEventArgs e)
        {
            TitleBar.ShowProgressRing();
            await DispatcherQueue.EnqueueAsync(() =>
            {
                Provider.GetDevices();
                TitleBar.ShowProgressRing();
                AdvancedAdbClient client = new AdvancedAdbClient();
                Provider.Processes = DeviceExtensions.ListProcesses(client, Provider.devices[DeviceComboBox.SelectedIndex]);
            });
            TitleBar.HideProgressRing();
        }

        private async void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            Provider.DeviceComboBox = sender as ComboBox;
            await DispatcherQueue.EnqueueAsync(() => Provider.GetDevices());
        }
    }
}
