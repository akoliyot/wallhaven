using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using wallhaven.Resources;
using System.Net.Http;
using HtmlAgilityPack;
using wallhaven.ViewModels;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Input;
using System.IO.IsolatedStorage;
using System.Windows.Controls.Primitives;

namespace wallhaven
{
    public partial class MainPage : PhoneApplicationPage
    {
        private string SearchTerm;
        bool ListPickerInitialized = false;

        List<string> filters;
        List<string> displayCriteria;

        // The static constructor is used so that I can access controls in MainPage from App.xaml 
        public static MainPage instance { get; private set; }

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
            BuildLocalizedApplicationBar();
            instance = this;
            
        }

        async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.wallHavenModel.IsDataLoaded)
            {
                SetDefaultStyles();

                RestoreUserPreferences();

                await App.wallHavenModel.DownloadWallpapers("latest");
                await App.wallHavenModel.DownloadWallpapers("random");                
                LatestWallpapers.ItemsSource = App.wallHavenModel.Latest.WallpaperCollection;
                RandomWallpapers.ItemsSource = App.wallHavenModel.Random.WallpaperCollection;

                
                
                ListPickerInitialized = true;
                App.wallHavenModel.IsDataLoaded = true;
            }
        }

        public void SetDefaultStyles()
        {
            // Setting styles where necessary. 
            SearchTextBox.Style = App.Current.Resources["SearchTextBoxStyle"] as Style;
        }

        public void RestoreUserPreferences()
        {
            filters = new List<string>();
            displayCriteria = new List<string>();

            GetToggleButtonAndListPickerNames();

            if (!(UserPreference.IsPreferenceInitialized()))
                UserPreference.SetDefaultPreferences(filters, displayCriteria);

            RestoreFilterStatus();
            RestoreListPickerStatus();
        }

        private void GetToggleButtonAndListPickerNames()
        {
            // This helps to create IsolatedStorageSettings variables with the same name as the controls on the xaml page. 
            foreach (UIElement gridElement in MainStackPanel.Children)
            {
                if (gridElement is Grid)
                {
                    foreach (UIElement toggleButtonElement in ((Grid)gridElement).Children)
                    {
                        if (toggleButtonElement is ToggleButton)
                            filters.Add(((ToggleButton)toggleButtonElement).Name.ToString());
                    }
                }
            }

            // This helps to create IsolatedStorageSettings variables with the same name as the controls on the xaml page. 
            foreach (UIElement listPicker in MainStackPanel.Children)
            {
                if (listPicker is ListPicker)
                    displayCriteria.Add(((ListPicker)listPicker).Name.ToString());
            }
        }

        private void RestoreFilterStatus()
        {
            foreach (var filter in filters)
            {
                string filterStatus = IsolatedStorageSettings.ApplicationSettings[filter] as string;
                if (filterStatus == "checked")
                {
                    // Assigning xFilter.IsChecked = true calls the PurityFilter_Checked event handler.
                    switch (filter)
                    {
                        case "GeneralFilter":
                            GeneralFilter.IsChecked = true;
                            break;

                        case "AnimeFilter":
                            AnimeFilter.IsChecked = true;
                            break;

                        case "PeopleFilter":
                            PeopleFilter.IsChecked = true;
                            break;

                        case "SFWFilter":
                            SFWFilter.IsChecked = true;
                            break;

                        case "SketchyFilter":
                            SketchyFilter.IsChecked = true;
                            break;

                        case "NSFWFilter":
                            NSFWFilter.IsChecked = true;
                            break;
                    }
                }
                else if (filterStatus == "unchecked")
                {
                    switch (filter)
                    {
                        case "GeneralFilter":
                            GeneralFilter.IsChecked = false;
                            GeneralFilter.Style = App.Current.Resources["ToggleButtonUnchecked"] as Style;
                            break;

                        case "AnimeFilter":
                            AnimeFilter.IsChecked = false;
                            AnimeFilter.Style = App.Current.Resources["ToggleButtonUnchecked"] as Style;
                            break;

                        case "PeopleFilter":
                            PeopleFilter.IsChecked = false;
                            PeopleFilter.Style = App.Current.Resources["ToggleButtonUnchecked"] as Style;
                            break;

                        case "SFWFilter":
                            // The below statement does not call the PurityFilter_Unchecked event handler. 
                            // The code for the styles have to be repeated. 
                            SFWFilter.IsChecked = false;
                            SFWFilter.Style = App.Current.Resources["ToggleButtonUnchecked"] as Style;
                            break;

                        case "SketchyFilter":
                            SketchyFilter.IsChecked = false;
                            SketchyFilter.Style = App.Current.Resources["ToggleButtonUnchecked"] as Style;
                            break;

                        case "NSFWFilter":
                            NSFWFilter.IsChecked = false;
                            NSFWFilter.Style = App.Current.Resources["ToggleButtonUnchecked"] as Style;
                            break;
                    }
                }
            }
        }

        private void RestoreListPickerStatus()
        {
            foreach (var displayCriterion in displayCriteria)
            {
                switch (displayCriterion)
                {
                    case "SortBy":
                        foreach (ListPickerItem item in SortBy.Items)
                        {
                            if (item.Name.ToString().ToLower() == (IsolatedStorageSettings.ApplicationSettings[displayCriterion]).ToString().ToLower())
                            {
                                MessageBox.Show("Setting is: " + (IsolatedStorageSettings.ApplicationSettings[displayCriterion]).ToString().ToLower());
                                SortBy.SelectedItem = item;
                                break;
                            }
                        }
                        break;

                    case "OrderBy":
                        foreach (ListPickerItem item in OrderBy.Items)
                        {
                            if (item.Name.ToString().ToLower() == (IsolatedStorageSettings.ApplicationSettings[displayCriterion]).ToString().ToLower())
                            {
                                MessageBox.Show("Setting is: " + (IsolatedStorageSettings.ApplicationSettings[displayCriterion]).ToString().ToLower());
                                OrderBy.SelectedItem = item;
                                break;
                            }
                        }
                        break;
                }
            }
        }

        private void LongListSelector_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (!App.wallHavenModel.IsCurrentlyDownloading)
            {
                var longListSelector = sender as LongListSelector;
                string wallpaperType = longListSelector.Name.ToString();

                var wallpaper = e.Container.Content as Wallpaper;
                var wallpaperIndex = longListSelector.ItemsSource.IndexOf(wallpaper);

                var wallpaperCount = longListSelector.ItemsSource.Count;

                //MessageBox.Show(wallpaperCount.ToString());
                //MessageBox.Show(App.wallHavenModel.Latest.WallpaperCollection.Count.ToString());

                LoadMoreWallpapers(wallpaperType, wallpaperIndex, wallpaperCount);
            }
            
        }

        private async Task LoadMoreWallpapers(string wallpaperType, int wallpaperIndex, int wallpaperCount)
        {

            if (wallpaperIndex >= (wallpaperCount - 1))
            {
                //MessageBox.Show("Loading next set of images");
                switch (wallpaperType)
                {
                    case "LatestWallpapers":

                        if (App.listRefreshed == false)
                        {
                            // MessageBox.Show("Loading the next set of latest items");
                            await App.wallHavenModel.DownloadWallpapers("latest");
                        }
                        else
                        {
                            await App.wallHavenModel.DownloadWallpapers("latestWithSearchPreferences");
                        }
                        break;


                    case "RandomWallpapers":

                        // MessageBox.Show("Loading the next set of random items");
                        await App.wallHavenModel.DownloadWallpapers("random");
                        break;

                    case "SearchResults":

                        // MessageBox.Show("Load the next page here");
                        await App.wallHavenModel.DownloadWallpapers("search", SearchTerm);
                        break;
                }
            }
        }

        private void MainPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((Pivot)sender).SelectedIndex)
            {
                case 0:
                    ApplicationBar = ((ApplicationBar)Application.Current.Resources["LatestImagesPivotAppBar"]);
                    break;

                case 1:
                    ApplicationBar = ((ApplicationBar)Application.Current.Resources["RandomImagesPivotAppBar"]);
                    break;

                case 2:
                    ApplicationBar = ((ApplicationBar)Application.Current.Resources["SearchPivotAppBar"]);
                    break;

                //case 3:
                //    ApplicationBar = ((ApplicationBar)Application.Current.Resources["SettingsPivot"]);
                //    break;

                //default:
                //    //ApplicationBar.IsVisible = false;
                //    break;
            }
        }

        //async void appBarRefreshButton_Click(object sender, EventArgs e)
        //{
        //    App.wallHavenModel.Random.WallpaperCollection.Clear();
        //    App.wallHavenModel.Random.CurrentPage = 1;
        //    await App.wallHavenModel.DownloadWallpapers("random");
        //}

        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();
            
            // ApplicationBar.IsVisible = false;
        }

        private void LongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selector = sender as LongListSelector;
            if(selector.SelectedItem == null)
                return;

            App.wallHavenModel.SelectedWallpaper = selector.SelectedItem as Wallpaper;

            string navTo =
                "/Views/WallpaperDetailsPivot.xaml";

            NavigationService.Navigate(new Uri(navTo, UriKind.RelativeOrAbsolute));

            selector.SelectedItem = null;
            MessageBox.Show("Clicked!");
        }

        private void Image_Opened(object sender, RoutedEventArgs e)
        {
            Image img = sender as Image;

            if (img == null)
            {
                return;
            }

            Storyboard s = new Storyboard();

            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.To = 1.0;
            doubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(500));

            Storyboard.SetTarget(doubleAnimation, img);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(OpacityProperty));

            s.Children.Add(doubleAnimation);
            s.Begin();
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Style = App.Current.Resources["SearchTextBoxGotFocusStyle"] as Style;
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Style = App.Current.Resources["SearchTextBoxStyle"] as Style;
        }

        private async void SearchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textBox = sender as TextBox;
                SearchTerm = textBox.Text;
                SearchPivot.Focus();

                // Reset the search image collection to ensure the new collection is loaded correctly
                App.wallHavenModel.SearchResult.WallpaperCollection.Clear();
                App.wallHavenModel.SearchResult.CurrentPage = 1;
                await App.wallHavenModel.DownloadWallpapers("search", SearchTerm);

                SearchResults.ItemsSource = App.wallHavenModel.SearchResult.WallpaperCollection;
                
                MessageBox.Show("Complete");
            }
        }

        private void Filter_Checked(object sender, RoutedEventArgs e)
        {
            var purityToggleButton = sender as ToggleButton;
            switch (purityToggleButton.Name.ToString())
            {
                case "GeneralFilter":
                    GeneralFilter.Style = App.Current.Resources["ToggleButtonChecked"] as Style;
                    UserPreference.UpdatePreference(purityToggleButton.Name.ToString(), "checked");
                    break;

                case "AnimeFilter":
                    AnimeFilter.Style = App.Current.Resources["ToggleButtonChecked"] as Style;
                    UserPreference.UpdatePreference(purityToggleButton.Name.ToString(), "checked");
                    break;

                case "PeopleFilter":
                    PeopleFilter.Style = App.Current.Resources["ToggleButtonChecked"] as Style;
                    UserPreference.UpdatePreference(purityToggleButton.Name.ToString(), "checked");
                    break;

                case "SFWFilter":
                    SFWFilter.Style = App.Current.Resources["SFWFilterChecked"] as Style;
                    UserPreference.UpdatePreference(purityToggleButton.Name.ToString(), "checked");
                    break;

                case "SketchyFilter":
                    SketchyFilter.Style = App.Current.Resources["SketchyFilterChecked"] as Style;
                    // Changes the first argument to use a variable instead of a literal string. 
                    UserPreference.UpdatePreference(purityToggleButton.Name.ToString(), "checked");
                    break;

                case "NSFWFilter":
                    NSFWFilter.Style = App.Current.Resources["NSFWFilterChecked"] as Style;
                    UserPreference.UpdatePreference(purityToggleButton.Name.ToString(), "checked");
                    break;
                    
            }
        }

        private void Filter_Unchecked(object sender, RoutedEventArgs e)
        {
            var purityToggleButton = sender as ToggleButton;
            purityToggleButton.Style = App.Current.Resources["ToggleButtonUnchecked"] as Style;
            UserPreference.UpdatePreference(purityToggleButton.Name.ToString(), "unchecked");
        }

        private void ImageDisplayCriteria_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (ListPickerInitialized)
            {
                ListPicker listPicker = sender as ListPicker;
                ListPickerItem selectedItem = listPicker.SelectedItem as ListPickerItem;

                string name = listPicker.Name.ToString();
                string value = selectedItem.Name.ToString().ToLower();
                UserPreference.UpdatePreference(name, value);
            }

            
        }
    }
}