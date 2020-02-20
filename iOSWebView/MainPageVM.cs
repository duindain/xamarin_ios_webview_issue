using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;

namespace iOSWebView
{
    public class MainPageVM : NotifiablePropertyModel
    {
        private object _webSource;
        public object WebSource
        {
            get => _webSource;
            set => SetProperty(ref _webSource, value);
        }

        private HtmlWebViewSource _htmlWebViewSource;
        public HtmlWebViewSource HtmlWebViewSource
        {
            get => _htmlWebViewSource;
            set
            {
                if (SetProperty(ref _htmlWebViewSource, value))
                {
                    WebSource = HtmlWebViewSource;
                }
            }
        }

        public MainPageVM()
        {
            LoadWebpage();
        }

        public void WebViewNavigating(WebNavigatingEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Navigating to {e.Url}");
        }

        private async Task<bool> CheckPermission()
        {
            var grantedPermission = false;
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                    {
                        await DisplayAlert("Need storage", "Gunna need that storage", "OK");
                    }
                    var permissions = new Permission[] { Permission.Storage };
                    var requestStatus = await CrossPermissions.Current.RequestPermissionsAsync(permissions);
                    status = requestStatus[Permission.Storage];
                }

                if (status == PermissionStatus.Granted)
                {
                    grantedPermission = true;
                }
                else if (status != PermissionStatus.Unknown)
                {
                    //location denied
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MainPageVM.CheckPermission: Permission request error? {ex.Message}");
            }
            return grantedPermission;
        }

        private Task DisplayAlert(string v1, string v2, string v3)
        {
            throw new NotImplementedException();
        }

        private async Task LoadWebpage()
        {
            if(await CheckPermission())
            {
                var localFilename = "images/downloadedimage.jpg";
                var fullPathToLocalFile = GetFullFilePath(localFilename);

                if (File.Exists(fullPathToLocalFile) == false)
                {
                    using (var httpClient = new HttpClient())
                    {
                        try
                        {
                            httpClient.Timeout = new TimeSpan(0, 0, 10);
                            var pdfStream = await httpClient.GetStreamAsync("https://picsum.photos/200");
                            System.Diagnostics.Debug.WriteLine($"MainPageVM.LoadWebpage: Downloaded image file");

                            var grantedPermissions = await CheckPermission();
                            if (grantedPermissions)
                            {
                                await SaveFileToDisk(pdfStream, fullPathToLocalFile);
                            }
                        }
                        catch (AggregateException ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"MainPageVM.LoadWebpage: Certificate error? only SSL sites are supported without additional config {ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"MainPageVM.LoadWebpage: Http error, network issue? {ex.Message}");
                        }
                    };
                }

                HtmlWebViewSource = new HtmlWebViewSource
                {
                    //BaseUrl = GetDocumentsPath(),
                    Html = $@"<html>
    <head>
        <meta name='viewport' content='width=device-width,initial-scale=1,maximum-scale=1'/>
        <link href='styles/mobile.css' rel='stylesheet' type='text/css' />
    </head>
    <body>
        <p>
            
        </p>
        <div class='container'>
            <div class='divL'>
                Relative path specified to local downloaded file here '{localFilename}'
            </div>
            <div class='divR'/>
                <img src='{localFilename}'>
            </div>
            <hr>
        </div>
        <div class='container'>
            <div class='divL'>
                Full path specfied to local downloaded file here '{fullPathToLocalFile}'
            </div>
            <div class='divR'>
                <img src='{fullPathToLocalFile}'/>
            </div>
            <hr>
        </div>
        <div class='container'>
            <div class='divL'>
                Image loaded from Individual projects (iOS) Resources (Android) Assets folders 'images/IpSumLocalSourceImage.jpg'
            </div>
            <div class='divR'>
                <img src='images/IpSumLocalSourceImage.jpg'/>
            </div>
            <hr>
        </div>
        <div class='container'>
            <div class='divL'>
                Image loaded from manifest in Forms project 'iOSWebView.ManifestResources.images.ManifestImage.jpg'
            </div>
            <div class='divR'>
                <img src='iOSWebView.ManifestResources.images.ManifestImage.jpg'/>
            </div>
            <hr>
        </div>
        <div class='container'>
            <div class='divL'>
                Remote image loaded from the web 'https://picsum.photos/200'
            </div>
            <div class='divR'>
                <img src='https://picsum.photos/200'/>
            </div>
            <hr>
        </div>
    </body>
</html>"
                };
            }
        }

        private string GetFullFilePath(string fileName)
        {
            return Path.Combine(GetDocumentsPath(), fileName);
        }

        private string GetDocumentsPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        /// <summary>
        /// Creates a subdirectory in the Environment.SpecialFolder.MyDocuments directory called pdfs if it doesnt already exist
        /// Then writes out the PDF file as bytes using .net standard c# methods no platform specifics needed
        /// </summary>
        /// <param name="pdfStream">data to save to filesystem</param>
        /// <param name="filePath">path including filename to save to</param>
        /// <returns></returns>
        private async Task<string> SaveFileToDisk(Stream pdfStream, string filePath)
        {
            var containingPath = Path.GetDirectoryName(filePath);
            if (Directory.Exists(containingPath) == false)
            {
                System.Diagnostics.Debug.WriteLine($"MainPageVM.SaveFileToDisk: Directory {containingPath} doesnt exist, creating");
                Directory.CreateDirectory(containingPath);
            }
            using (var memoryStream = new MemoryStream())
            {
                await pdfStream.CopyToAsync(memoryStream);
                File.WriteAllBytes(filePath, memoryStream.ToArray());
            }
            return filePath;
        }
    }
}
