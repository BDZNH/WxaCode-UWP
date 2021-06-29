using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace WxaCode
{
    public sealed partial class SettingPage : Page
    {
        ApplicationDataContainer localSettings;
        public SettingPage()
        {
            this.InitializeComponent();
            localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            initViewState();
        }

        private void SaveAppidAndAppsecret_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ToggleSwitch ss = sender as ToggleSwitch;
            localSettings.Values["saveAppidAndAppsecret"] = ss.IsOn ? true : (object)false;
        }

        private void SvaveAppAccessToken_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ToggleSwitch ss = sender as ToggleSwitch;
            localSettings.Values["saveAccesstoken"] = ss.IsOn ? true : (object)false;
        }

        private void SaveWxacodeParam_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ToggleSwitch ss = sender as ToggleSwitch;
            localSettings.Values["saveWxacodeParam"] = ss.IsOn ? true : (object)false;
        }

        private void initViewState()
        {
            if(SaveAppidAndAppsecretSwitch!=null)
            {
                SaveAppidAndAppsecretSwitch.IsOn = LocalSettings.IfSaveAppidAndAppsecret();
            }

            if(SvaveAppAccessTokenSwitch!=null)
            {
                SvaveAppAccessTokenSwitch.IsOn = LocalSettings.IfSaveAccessToken();
            }


            if(SaveWxacodeParamSwitch!=null)
            {
                SaveWxacodeParamSwitch.IsOn = LocalSettings.IfSaveWxacodeParam();
            }

            if(AppVersionTextBlock !=null)
            {
                AppVersionTextBlock.Text = GetAppVersion();
            }
        }

        public static string GetAppVersion()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            return string.Format("版本: {0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }

    }
}
