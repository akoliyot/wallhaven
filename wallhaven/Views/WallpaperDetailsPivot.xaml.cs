using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using wallhaven.ViewModels;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace wallhaven
{
    public partial class WallpaperDetailsPivot : PhoneApplicationPage
    {
        bool tagsInitialized = false;

        public static WallpaperDetailsPivot instace { get; private set; }

        public WallpaperDetailsPivot()
        {
            InitializeComponent();
            Loaded += WallpaperDetailsPivot_Loaded;
            BuildLocalizedApplicationBar();
            instace = this;
        }

        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();
            ApplicationBar = ((ApplicationBar)Application.Current.Resources["ImagePivotAppBar"]);
        }

        async void WallpaperDetailsPivot_Loaded(object sender, RoutedEventArgs e)
        {
            if (!tagsInitialized)
            {
                App.wallHavenModel.SelectedWallpaper.GenerateImagePageUri(App.wallHavenModel.SelectedWallpaper.LazyLoadImageUri);
                await App.wallHavenModel.SelectedWallpaper.GetImageProperties();
                SetupGridAndTextBlock(App.wallHavenModel.SelectedWallpaper.Tags);
                SetPropertyValues();

                GettingTagsOverlay.Visibility = Visibility.Collapsed;

                if (App.wallHavenModel.SelectedWallpaper.Tags.Count == 0)
                {
                    TagNotFoundOverlay.Visibility = Visibility.Visible;
                }
                tagsInitialized = true;
            }
        }

        private void SetPropertyValues()
        {
            Resolution.Text = App.wallHavenModel.SelectedWallpaper.Property["resolution"];
            Size.Text = App.wallHavenModel.SelectedWallpaper.Property["size"];
            Category.Text = App.wallHavenModel.SelectedWallpaper.Property["category"];
            Views.Text = App.wallHavenModel.SelectedWallpaper.Property["views"];
            Favorites.Text = App.wallHavenModel.SelectedWallpaper.Property["favorites"];
            UploadedBy.Text = App.wallHavenModel.SelectedWallpaper.Property["uploadedBy"];
            Added.Text = App.wallHavenModel.SelectedWallpaper.Property["added"];
            Source.Text = App.wallHavenModel.SelectedWallpaper.Property["source"];
        }

        private void SetupGridAndTextBlock(List<string> tags)
        {
            // Initialize Grid and TextBlock collections
            // The Grids and TextBlocks are used specifically for styling purposes. 
            List<Grid> GridCollection = new List<Grid>();
            List<TextBlock> TextBlockCollection = new List<TextBlock>();

            // Add the required number of respective items to Grid and TextBlock collection
            for (int i = 0; i < tags.Count; i++)
            {
                GridCollection.Add(new Grid());
                TextBlockCollection.Add(new TextBlock());
            }

            // Set the attributes for each TextBlock
            for (int i = 0; i < TextBlockCollection.Count; i++)
            {
                TextBlockCollection[i].Text = tags[i].ToString();
                TextBlockCollection[i].Foreground = GetColorFromHexa("#94db94");
                TextBlockCollection[i].Padding = new Thickness(10, 10, 10, 10);
                TextBlockCollection[i].FontSize = 28;
                // TextBlockCollection[i].MouseLeftButtonDown += TagTextBlock_MouseLeftButtonDown;
                TextBlockCollection[i].MouseLeftButtonUp += TagTextBlock_MouseLeftButtonUp;
            }

            // Set the attributes for each Grid
            foreach (var item in GridCollection)
            {
                item.Background = GetColorFromHexa("#293033");
                item.Margin = new Thickness(0, 10, 10, 0);
            }

            AddChildrenToHierarchy(GridCollection, TextBlockCollection);

        }

        void TagTextBlock_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MessageBox.Show("You clicked a tag!");

            var selectedTagTextBlock = sender as TextBlock;
            string tag = selectedTagTextBlock.Text;

            MessageBox.Show(tag);
        }

        private void AddChildrenToHierarchy(List<Grid> GridCollection, List<TextBlock> TextBlockCollection)
        {           
            // Assign one TextBlock item to one Grid item
            for (int i = 0; i < GridCollection.Count; i++)
            {
                GridCollection[i].Children.Add(TextBlockCollection[i]);
            }

            // Add each Grid to the WrapPanel
            foreach (var item in GridCollection)
            {
                TagsWrapPanel.Children.Add(item);
            }
        }

        public SolidColorBrush GetColorFromHexa(string hexaColor)
        {
            byte R = Convert.ToByte(hexaColor.Substring(1, 2), 16);
            byte G = Convert.ToByte(hexaColor.Substring(3, 2), 16);
            byte B = Convert.ToByte(hexaColor.Substring(5, 2), 16);
            SolidColorBrush scb = new SolidColorBrush(Color.FromArgb(0xFF, R, G, B));
            return scb;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Uri SelectedImageLazyUri = App.wallHavenModel.SelectedWallpaper.LazyLoadImageUri;
            SelectionImageControl.Source = new BitmapImage(App.wallHavenModel.SelectedWallpaper.LazyLoadImageUri);
            // You can use this to get the Url of the full image.
            // App.wallHavenModel.SelectedWallpaper.ConvertToOriginalImageUri(SelectedImageLazyUri);
        }

        private void SelectionImageControl_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MessageBox.Show("You have double tapped the image.");

            string navTo =
                "/Views/ViewWallpaper.xaml";

            NavigationService.Navigate(new Uri(navTo, UriKind.RelativeOrAbsolute));
        }
    }
}