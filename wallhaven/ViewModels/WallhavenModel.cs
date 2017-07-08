using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace wallhaven.ViewModels
{
    public class WallhavenModel
    {
        public Wallpaper Latest { get; set; }
        public Wallpaper Random { get; set; }
        public Wallpaper SelectedWallpaper { get; set; }
        public Wallpaper SearchResult { get; set; }

        public bool IsDataLoaded { get; set; }
        public bool IsCurrentlyDownloading { get; set; }

        public WallhavenModel()
        {
            Latest = new Wallpaper();
            Random = new Wallpaper();
            SearchResult = new Wallpaper();
            IsCurrentlyDownloading = false;
        }

        public async Task DownloadWallpapers(string category, string searchTerm = "")
        {
            string url;

            switch (category)
            {
                case "latest":
                    url = "http://alpha.wallhaven.cc/latest?page=";
                    Latest = await AddImagesToCollection(url, Latest);
                    break;

                case "latestWithSearchPreferences":
                    url = UserPreference.GenerateSearchUrl();
                    Latest = await AddImagesToCollection(url, Latest);
                    break;

                case "random":
                    url = "http://alpha.wallhaven.cc/random?page=";
                    Random = await AddImagesToCollection(url, Random);
                    break;

                case "search":
                    // Returns search results with the default image settings (as seen wallhaven search results from the homepage).
                    //url = "http://alpha.wallhaven.cc/search?q=" + searchTerm + "&page=";

                    url = UserPreference.GenerateSearchUrl(searchTerm);
                    SearchResult = await AddImagesToCollection(url, SearchResult);
                    break;
            }
        }

        //public async Task DownloadWallpapers(string category)
        //{
        //    string url;

        //    switch (category)
        //    {
        //        case "latest":
        //            url = "http://alpha.wallhaven.cc/latest?page=";
        //            Latest = await AddImagesToCollection(url, Latest);
        //            break;

        //        case "latestWithSearchPreferences":
        //            url = UserPreference.GenerateSearchUrl();
        //            Latest = await AddImagesToCollection(url, Latest);
        //            break;

        //        case "random":
        //            url = "http://alpha.wallhaven.cc/random?page=";
        //            Random = await AddImagesToCollection(url, Random);
        //            break;

        //    }
        //}

        //public async Task Search(string searchTerm)
        //{
        //    // Example of search Url: http://alpha.wallhaven.cc/search?q=Emma&page=1
        //    string searchUri = "http://alpha.wallhaven.cc/search?q=" + searchTerm + "&page=";
        //    SearchResult = await AddImagesToCollection(searchUri, SearchResult);

        //}

        public async Task<Wallpaper> AddImagesToCollection(string url, Wallpaper wallpaper)
        {
            IsCurrentlyDownloading = true;

            //url = "http://alpha.wallhaven.cc/search?q=Liam Neeson&categories=100&purity=100&sorting=date_added&order=desc&page=1";
            //url = "http://alpha.wallhaven.cc/search?q=Futurama&categories=010&purity=100&sorting=date_added&order=desc&page=1";

            string htmlSourceCode = "";
            using (var client = new HttpClient())
            {
                htmlSourceCode = await client.GetStringAsync(url + wallpaper.CurrentPage);
            }

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlSourceCode);

            var x = document.DocumentNode.SelectNodes("//img[starts-with(@class, 'lazyload')]");

            if (x != null)
            {
                foreach (var listItem in document.DocumentNode.SelectNodes("//img[starts-with(@class, 'lazyload')]"))
                {
                    wallpaper.WallpaperCollection.Add(new Wallpaper { LazyLoadImageUri = new Uri(listItem.Attributes["data-src"].Value, UriKind.RelativeOrAbsolute) });
                }
            }

            wallpaper.CurrentPage++;

            IsCurrentlyDownloading = false;
            return wallpaper;
        }

        

    }
}
