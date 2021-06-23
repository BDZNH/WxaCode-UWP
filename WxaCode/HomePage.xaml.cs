using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json.Linq;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.UI;

namespace WxaCode
{
    public sealed partial class HomePage : Page
    {
        private SolidColorBrush lineColorBrush = new SolidColorBrush();
        private WxaCode wxaCode = WxaCode.GetInstance();
        private WxaCodeParams wxacodeparams = WxaCodeParams.GetInstance();
        private static byte[] qrcode;
        public HomePage()
        {
            this.InitializeComponent();
            AppidTextBlock.Text = wxaCode.appid;
            AppsecretTextBlock.Text = wxaCode.appsecret;
            WxacodeSceneTextBlock.Text = wxacodeparams.scene;
            WxacodePageTextBlock.Text = wxacodeparams.page;
            WxacodeWidthSlider.Value = wxacodeparams.width;
            WxacodeAutoColorSwitch.IsOn = wxacodeparams.auto_color;
            WxacodeLineColorRedSlider.Value = wxacodeparams.line_color.r;
            WxacodeLineColorGreenSlier.Value = wxacodeparams.line_color.g;
            WxacodeLineColorBlueSlider.Value = wxacodeparams.line_color.b;
            WxacodeIsHyalineSwitch.IsOn = wxacodeparams.is_hyaline;
            if(qrcode!=null)
            {
                SetByteArrayAsImageSource(qrcode);
            }
        }

        private async void GetWxaCodeUnlimited(object sender, RoutedEventArgs e)
        {
            if(!CheckArgValid())
            {
                return;
            }

            wxacodeimage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Loading.png"));
            Button button = sender as Button;
            button.IsEnabled = false;

            qrcode = null;
            bool needRefresh = wxaCode.IsNeedRefreshAccesstoken(AppidTextBlock.Text, AppsecretTextBlock.Text);
            qrcode = wxaCode.GetWxaCodeUnlimited(GetwxacodeParam(), needRefresh);
            if (qrcode != null)
            {
                SetByteArrayAsImageSource(qrcode);
            }
            else
            {
                var dialog = new MessageDialog(wxaCode.lasstErrorMsg, "好像发生了一些错误");
                dialog.Commands.Add(new UICommand("确定", cmd => { }, commandId: 0));
                await dialog.ShowAsync();
            }
            button.IsEnabled = true;
        }

        private void ToggleAutoColorSwitch(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;

            WxaCodeParams.GetInstance().auto_color = toggleSwitch.IsOn;
            if (WxacodeAutoColorSwitch.IsOn == true)
            {
                if (WxacodeLineColorRedSlider != null)
                {
                    WxacodeLineColorRedSlider.Value = 0;
                    wxacodeparams.line_color.r = (int)WxacodeLineColorRedSlider.Value;
                }

                if (WxacodeLineColorGreenSlier != null)
                {
                    WxacodeLineColorGreenSlier.Value = 0;
                    wxacodeparams.line_color.g = (int)WxacodeLineColorGreenSlier.Value;
                }

                if (WxacodeLineColorBlueSlider != null)
                {
                    WxacodeLineColorBlueSlider.Value = 0;
                    wxacodeparams.line_color.b = (int)WxacodeLineColorBlueSlider.Value;
                }
            }
            UpdateLineColorPreview();
        }

        private string GetwxacodeParam()
        {
            JObject jobject = new JObject();
            jobject.Add("scene", AppsecretTextBlock.Text);
            if (WxacodePageTextBlock.Text != null && WxacodePageTextBlock.Text.Length > 0)
            {
                jobject.Add("page", WxacodePageTextBlock.Text);
            }
            jobject.Add("width", (int)WxacodeWidthSlider.Value);
            jobject.Add("auto_color", WxacodeAutoColorSwitch.IsOn);
            if (WxacodeAutoColorSwitch.IsOn == false)
            {
                JObject linecolor = new JObject();
                linecolor.Add("r", (int)WxacodeLineColorRedSlider.Value);
                linecolor.Add("g", (int)WxacodeLineColorGreenSlier.Value);
                linecolor.Add("b", (int)WxacodeLineColorBlueSlider.Value);
                jobject.Add("line_color", linecolor);
            }
            jobject.Add("is_hyaline", WxacodeIsHyalineSwitch.IsOn);
            return jobject.ToString();
        }

        private async void SaveWxacodeImage(object sender, RoutedEventArgs e)
        {
            if (qrcode != null)
            {
                var savePicker = new Windows.Storage.Pickers.FileSavePicker();
                savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
                savePicker.FileTypeChoices.Add("Image", new List<string>() { ".jpg" });
                savePicker.SuggestedFileName = "WxaCode";
                Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
                if (file != null)
                {
                    await FileIO.WriteBytesAsync(file, qrcode);
                    Windows.Storage.Provider.FileUpdateStatus status = await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                    if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                    {
                        var dialog = new MessageDialog("保存成功", "保存小程序码");
                        dialog.Commands.Add(new UICommand("确定", cmd => { }, commandId: 0));
                        await dialog.ShowAsync();
                    }
                    else
                    {
                        var dialog = new MessageDialog("保存失败", "保存小程序码");
                        dialog.Commands.Add(new UICommand("确定", cmd => { }, commandId: 0));
                        await dialog.ShowAsync();
                    }
                }
            }
            else
            {
                var dialog = new MessageDialog("您还没有获取过小程序码", "保存小程序码");
                dialog.Commands.Add(new UICommand("确定", cmd => { }, commandId: 0));
                await dialog.ShowAsync();
            }
        }

        public async void SetByteArrayAsImageSource(byte[] data)
        {
            BitmapImage bitmapImage = new BitmapImage();
            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                await stream.WriteAsync(data.AsBuffer());
                stream.Seek(0);
                await bitmapImage.SetSourceAsync(stream);
                wxacodeimage.Source = bitmapImage;
            }
        }

        private bool CheckArgValid()
        {
            if(AppidTextBlock.Text is null || AppidTextBlock.Text.Length==0)
            {
                AppidTextBlock.Focus(FocusState.Programmatic);
                return false;
            }
            if(AppsecretTextBlock.Text is null || AppsecretTextBlock.Text.Length==0)
            {
                AppsecretTextBlock.Focus(FocusState.Programmatic);
                return false;
            }
            if(WxacodeSceneTextBlock.Text is null || WxacodeSceneTextBlock.Text.Length == 0)
            {
                WxacodeSceneTextBlock.Focus(FocusState.Programmatic);
                return false;
            }

            wxacodeparams.scene = WxacodeSceneTextBlock.Text;
            wxacodeparams.width = (int)WxacodeWidthSlider.Value;
            wxacodeparams.auto_color = WxacodeAutoColorSwitch.IsOn;
            wxacodeparams.line_color.r = (int)WxacodeLineColorRedSlider.Value;
            wxacodeparams.line_color.g = (int)WxacodeLineColorGreenSlier.Value;
            wxacodeparams.line_color.b = (int)WxacodeLineColorBlueSlider.Value;
            wxacodeparams.is_hyaline = WxacodeIsHyalineSwitch.IsOn;
            if(WxacodePageTextBlock.Text !=null && WxacodePageTextBlock.Text.Length >0)
            {
                wxacodeparams.page = WxacodePageTextBlock.Text;
            }
            else
            {
                wxacodeparams.page = "pages/index/index";
            }
            return true;
        }

        private void WxacodeWidthSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if(WxacodeWidthSlider!=null)
            {
                WxaCodeParams.GetInstance().width = (int)WxacodeWidthSlider.Value;
            }
            
        }

        private void WxacodeIsHyalineSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if(WxacodeIsHyalineSwitch!=null)
            {
                WxaCodeParams.GetInstance().is_hyaline = WxacodeIsHyalineSwitch.IsOn;
            }
        }

        private void WxacodeLineColorRedSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider slider = sender as Slider;
            WxaCodeParams.GetInstance().line_color.r = (int)slider.Value;
            UpdateLineColorPreview();
        }

        private void WxacodeLineColorGreenSlier_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider slider = sender as Slider;
            WxaCodeParams.GetInstance().line_color.g = (int)slider.Value;
            UpdateLineColorPreview();
        }

        private void WxacodeLineColorBlueSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider slider = sender as Slider;
            WxaCodeParams.GetInstance().line_color.b = (int)slider.Value;
            UpdateLineColorPreview();
        }

        private void UpdateLineColorPreview()
        {
            byte r = (byte)WxaCodeParams.GetInstance().line_color.r;
            byte g = (byte)WxaCodeParams.GetInstance().line_color.g;
            byte b = (byte)WxaCodeParams.GetInstance().line_color.b;
            lineColorBrush.Color = Color.FromArgb(255, r, g, b);
            ColorPreview.Fill = lineColorBrush;
        }
    }
}
