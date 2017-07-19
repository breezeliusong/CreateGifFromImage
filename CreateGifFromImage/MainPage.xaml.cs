using Lumia.Imaging;
using Lumia.Imaging.Adjustments;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CreateGifFromImage
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var sourceFile = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync("pic.png");
            BitmapImage bitmap;
            using (var source = new StorageFileImageSource(sourceFile))
            {
                using (var renderer = new GifRenderer())
                {
                    var sources = new List<IImageProvider>();

                    for (int i = 1; i < 257; i++)
                    {
                        sources.Add(new BlurEffect(source, i));
                    }

                    renderer.Sources = sources;
                    renderer.NumberOfAnimationLoops = 0;
                    renderer.Duration = 30;

                    var buffer = await renderer.RenderAsync();

                    var filename = "myFile";
                    var storageFile = await KnownFolders.SavedPictures.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                    using (var stream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        await stream.WriteAsync(buffer);
                        bitmap = new BitmapImage();
                    FileRandomAccessStream stream1 = (FileRandomAccessStream)await storageFile.OpenAsync(FileAccessMode.Read);

                        bitmap.SetSource(stream1);
                    }

                    MyImage.Source = bitmap;
                }
            }

        }

        private static async Task<int> GetFileNameRunningNumber()
        {
            var files = await KnownFolders.SavedPictures.GetFilesAsync();
            int max = 0;
            foreach (StorageFile storageFile in files)
            {
                var pattern = "Sequence\\d+\\.gif";
                if (System.Text.RegularExpressions.Regex.IsMatch(storageFile.Name, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    max = Math.Max(max, Convert.ToInt32(storageFile.Name.Split('.')[0].Substring(8)));
                }
            }

            return max + 1;
        }
    }
}
