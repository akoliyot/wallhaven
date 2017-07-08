using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net.Http;
using System.Diagnostics;

namespace wallhaven.ViewModels
{
    public class Wallpaper : INotifyPropertyChanged
    {
        // public string Title { get; set; }

        public string Header { get; set; }

        // Example of LazyLoadImage url
        // http://alpha.wallhaven.cc/wallpapers/thumb/small/th-193946.jpg
        public Uri LazyLoadImageUri { get; set; }

        // Example of full image url
        // http://wallpapers.wallhaven.cc/wallpapers/full/wallhaven-193946.jpg || .png
        // http://wallpapers.wallhaven.cc/wallpapers/full/wallhaven-142098.png
        public Uri OriginalImageUri { get; set; }

        // Example of Image page Url
        // http://alpha.wallhaven.cc/wallpaper/193946
        public Uri ImagePageUri { get; set; }

        public int CurrentPage { get; set; }



        public ObservableCollection<Wallpaper> WallpaperCollection { get; set; }

        public List<string> Tags { get; set; }

        public Dictionary<string, string> Property { get; set; }

        public Wallpaper()
        {
            CurrentPage = 1;
            WallpaperCollection = new ObservableCollection<Wallpaper>();

            // Strange Behaviour
            // If I initialize Tags and Property here, then the second time I click on a particular wallpaper 
            // that has tags or properties (hint: that's all of them) the code breaks (Debugger.Break())
            // Why does that happen? And why does it only break when I click on the same image? 
            Tags = new List<string>();
            Property = new Dictionary<string, string>();
        }

        // Other properties
        public string Resolution { get; set; }


        public async Task GetImageProperties()
        {
            Tags.Clear();
            Property.Clear();

            // GetOriginalImageUri() 
            
            // Getting the source code of the HTML page
            string htmlSourceCode;
            using (var client = new HttpClient())
            {
                if (this.ImagePageUri == null)
                {
                    // System.ArgumentException
                    // Debug.WriteLine was used since if 'Break on System.ArgumentException' was not enabled then the Output 
                    // window would just show: 
                    // "A first chance exception of type 'System.ArgumentException' occurred in wallhaven.DLL"

                    Debug.WriteLine("ImagePageUri cannot be null while calling GenerateOriginalImageUri(Uri LazyLoadImageUri)");
                    throw new ArgumentException("ImagePageUri cannot be null while calling GenerateOriginalImageUri(Uri LazyLoadImageUri)");
                }
                else
                {
                    htmlSourceCode = await client.GetStringAsync(Convert.ToString(ImagePageUri));
                }
                     
            }
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlSourceCode);


            // Getting the original image url
            var data = document.DocumentNode.SelectSingleNode("//img[starts-with(@id, 'wallpaper')]");
            string fullUrl = "http:" + Convert.ToString(data.Attributes["src"].Value); 
            this.OriginalImageUri = new Uri(fullUrl);





            // GetImageProperties

            // To check if there are any tags. Sometimes they are null
            // This variable is named 'x' because it doesn't server any other purpose. It's just used in this one context. 
            var x = document.DocumentNode.SelectNodes("//a[starts-with(@class, 'tagname')]");

            if (x != null)
            {
                foreach (var listItem in document.DocumentNode.SelectNodes("//a[starts-with(@class, 'tagname')]"))
                {
                    if (listItem != null)
                    {
                        Tags.Add(listItem.InnerHtml.ToString());
                    }
                }
            }

            // Getting the properties
            // 1. Resolution
            string resolution = document.DocumentNode.SelectSingleNode(".//*[@id='showcase-sidebar']/div/div[1]/div[2]/dl//dd[position()=1]").InnerHtml.Trim();
            Property.Add("resolution", resolution);

            // 2. Size
            string size = document.DocumentNode.SelectSingleNode(".//*[@id='showcase-sidebar']/div/div[1]/div[2]/dl//dd[position()=2]").InnerHtml.Trim();
            Property.Add("size", size);

            // 3. Category
            string category = document.DocumentNode.SelectSingleNode(".//*[@id='showcase-sidebar']/div/div[1]/div[2]/dl//dd[position()=3]").InnerHtml.Trim();
            Property.Add("category", category);

            // 4. Views
            string views = document.DocumentNode.SelectSingleNode(".//*[@id='showcase-sidebar']/div/div[1]/div[2]/dl//dd[position()=4]").InnerHtml.Trim();
            Property.Add("views", views);

            // 5. Favorites
            var favorites = document.DocumentNode.SelectSingleNode(".//*[@id='showcase-sidebar']/div/div[1]/div[2]/dl//dd[position()=5]/a");
            if (favorites != null)
                Property.Add("favorites", favorites.InnerHtml.Trim());
            else
                Property.Add("favorites", "0");

            // 6. Uploaded By
            var uploadedBy = document.DocumentNode.SelectSingleNode(".//*[@id='showcase-sidebar']/div/div[1]/div[2]/dl//dd[position()=6]/a");
            if (uploadedBy != null)
                Property.Add("uploadedBy", uploadedBy.InnerHtml.Trim());
            else
                Property.Add("uploadedBy", "deleted");

            // 7. Added
            string added = document.DocumentNode.SelectSingleNode(".//*[@id='showcase-sidebar']/div/div[1]/div[2]/dl//dd/time").InnerHtml.Trim();
            Property.Add("added", added);

            // 8. Source
            var source = document.DocumentNode.SelectSingleNode(".//*[@id='showcase-sidebar']/div/div[1]/div[2]/dl//dd[last()]/a");
            if (source != null)
            {
                string href = source.Attributes["href"].Value;
                Property.Add("source", href);
            }
            else
                Property.Add("source", "unknown");

        }

        

        public void GenerateImagePageUri(Uri LazyLoadImageUri)
        {
            // Get the file name of the wallpaper

            // [Sample] lazyUrl: http://alpha.wallhaven.cc/wallpapers/thumb/small/th-198149.jpg
            string lazyUrl = Convert.ToString(LazyLoadImageUri);

            // [Sample] wallpaperFileName: 198149.jpg
            string wallpaperFileName = lazyUrl.Split('-').Last();

            // Get the ID of the wallpaper from the file name of the wallpaper
            // [Sample] wallpaperID: 198149
            var wallpaperID = wallpaperFileName.Split('.')[0];

            // Generating the url for the image page.
            // [Sample] imagePageUrl: http://alpha.wallhaven.cc/wallpaper/198149
            string imagePageUrl = "http://alpha.wallhaven.cc/wallpaper/" + wallpaperID;

            this.ImagePageUri = new Uri(imagePageUrl);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
