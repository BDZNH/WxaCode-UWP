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

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace WxaCode
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {

        WxaCode wxaCode;
        static byte[] data;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void GetWxaCodeUnlimited(object sender, RoutedEventArgs e)
        {
            wxacodeimage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Loading.png"));
            Button button = sender as Button;
            button.IsEnabled = false;
            if (wxaCode==null)
            {
                wxaCode = new WxaCode(appid.Text, appsecret.Text);
            }
            data = null;
            data = wxaCode.getWxaCode(getwxacodeParam(), false);
            if(data!=null)
            {
                BitmapImage bitmapImage = new BitmapImage();
                using(InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                {
                    await stream.WriteAsync(data.AsBuffer());
                    stream.Seek(0);
                    await bitmapImage.SetSourceAsync(stream);
                    wxacodeimage.Source = bitmapImage;
                }
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
            if(auto_color!=null)
            {
                if(auto_color.IsOn==true)
                {
                    if(line_corlor_red!=null)
                    {
                        line_corlor_red.Value = 0;
                    }

                    if(line_corlor_green!=null)
                    {
                        line_corlor_green.Value = 0;
                    }
                    
                    if(line_corlor_blue!=null)
                    {
                        line_corlor_blue.Value = 0;
                    }
                }
            }
        }

        private string getwxacodeParam()
        {
            JObject jobject = new JObject();
            jobject.Add("scene", scene.Text);
            jobject.Add("width", (int)wxacode_width.Value);
            jobject.Add("auto_color", auto_color.IsOn);
            if (auto_color.IsOn == false)
            {
                JObject linecolor = new JObject();
                linecolor.Add("r", (int)line_corlor_red.Value);
                linecolor.Add("g", (int)line_corlor_red.Value);
                linecolor.Add("b", (int)line_corlor_red.Value);
                jobject.Add("line_color", linecolor);
            }
            jobject.Add("is_hyaline", is_hyaline.IsOn);
            return jobject.ToString();
        }

        private async void SaveWxacodeImage(object sender, RoutedEventArgs e)
        {
            if (data != null)
            {
                var savePicker = new Windows.Storage.Pickers.FileSavePicker();
                savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
                savePicker.FileTypeChoices.Add("Image", new List<string>() { ".jpg" });
                savePicker.SuggestedFileName = "WxaCode";
                Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
                if (file != null)
                {
                    await FileIO.WriteBytesAsync(file, data);
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
    }
}
