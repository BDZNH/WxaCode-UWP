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

namespace WxaCode
{
    public sealed partial class HomePage : Page
    {
        private static WxaCode wxaCode;
        private static WxaCodeParams wxacodeparams;
        private static byte[] qrcode;
        public HomePage()
        {
            this.InitializeComponent();
            if(wxaCode!=null)
            {
                appid.Text = wxaCode.appid;
                appsecret.Text = wxaCode.appsecret;
            }
            if(wxacodeparams!=null)
            {
                scene.Text = wxacodeparams.scene;
                page.Text = wxacodeparams.page;
                wxacode_width.Value = wxacodeparams.width;
                auto_color.IsOn = wxacodeparams.auto_color;
                line_corlor_red.Value = wxacodeparams.line_color.r;
                line_corlor_green.Value = wxacodeparams.line_color.g;
                line_corlor_blue.Value = wxacodeparams.line_color.b;
                is_hyaline.IsOn = wxacodeparams.is_hyaline;
            }
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
            if (wxaCode == null)
            {
                wxaCode = new WxaCode();
            }
            qrcode = null;
            bool needRefresh = wxaCode.IsNeedRefreshAccesstoken(appid.Text, appsecret.Text);
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
            if (auto_color != null)
            {
                if (auto_color.IsOn == true)
                {
                    if (line_corlor_red != null)
                    {
                        line_corlor_red.Value = 0;
                    }

                    if (line_corlor_green != null)
                    {
                        line_corlor_green.Value = 0;
                    }

                    if (line_corlor_blue != null)
                    {
                        line_corlor_blue.Value = 0;
                    }
                }
            }
        }

        private string GetwxacodeParam()
        {
            JObject jobject = new JObject();
            jobject.Add("scene", scene.Text);
            jobject.Add("width", (int)wxacode_width.Value);
            jobject.Add("auto_color", auto_color.IsOn);
            if (auto_color.IsOn == false)
            {
                JObject linecolor = new JObject();
                linecolor.Add("r", (int)line_corlor_red.Value);
                linecolor.Add("g", (int)line_corlor_green.Value);
                linecolor.Add("b", (int)line_corlor_blue.Value);
                jobject.Add("line_color", linecolor);
            }
            jobject.Add("is_hyaline", is_hyaline.IsOn);
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
            if(appid.Text is null || appid.Text.Length==0)
            {
                appid.Focus(FocusState.Programmatic);
                return false;
            }
            if(appsecret.Text is null || appsecret.Text.Length==0)
            {
                appsecret.Focus(FocusState.Programmatic);
                return false;
            }
            if(scene.Text is null || scene.Text.Length == 0)
            {
                scene.Focus(FocusState.Programmatic);
                return false;
            }
            if(wxacodeparams==null)
            {
                wxacodeparams = new WxaCodeParams();
            }
            wxacodeparams.scene = scene.Text;
            wxacodeparams.width = (int)wxacode_width.Value;
            wxacodeparams.auto_color = auto_color.IsOn;
            wxacodeparams.line_color.r = (int)(line_corlor_red.Value);
            wxacodeparams.line_color.g = (int)(line_corlor_green.Value);
            wxacodeparams.line_color.b = (int)(line_corlor_blue.Value);
            wxacodeparams.is_hyaline = is_hyaline.IsOn;
            if(page.Text !=null && page.Text.Length >0)
            {
                wxacodeparams.page = page.Text;
            }
            else
            {
                wxacodeparams.page = "pages/index/index";
            }
            return true;
        }
    }
}
