using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wallhaven.ViewModels
{
    class UserPreference
    {
        // The variables are set to public so that I can get them through reflection. 
        public static IsolatedStorageSettings Settings { get; set; }

        public static string SortByTitle { get; set; }

        // Purity Filters
        static string GeneralFilterStatus { get; set; }
        static string AnimeFilterStatus { get; set; }
        static string PeopleFilterStatus { get; set; }
        static string SFWFilterStatus { get; set; }
        static string SketchyFilterStatus { get; set; }
        static string NSFWFilterStatus { get; set; }

        // ImageDisplayCriteria
        static string SortBy { get; set; }
        static string OrderBy { get; set; }

        static UserPreference()
        {
            // Default purity filters
            GeneralFilterStatus = "checked";
            AnimeFilterStatus = "checked";
            PeopleFilterStatus = "checked";
            SFWFilterStatus = "checked";
            SketchyFilterStatus = "unchecked";
            NSFWFilterStatus = "unchecked";

            // Default ImageDisplayCriteria
            // The value assigned have to match the name given for the ListPickerItem
            SortBy = "dateadded";
            OrderBy = "descending";            

            Settings = IsolatedStorageSettings.ApplicationSettings;
        }

        public static void SetDefaultPreferences(List<string> filters, List<string> displayCriteria)
        {
            // Save the settings
            Settings.Add("IsPreferenceInitialized", "true");
            foreach (var filter in filters)
            {
                // I'm still not sure if this is the best way to do this, if not, I don't know what is.
                // One alternative is to use reflection and use a loop with multiple enumerators to assign
                // the control to it's default value - This would cause problems that would be hard to debug 
                // if the order of the UI elements are changed. 

                // The names of the "cases" should be the same as the name of the controls on the Settings pivot. 
                switch (filter)
                {
                    case "GeneralFilter":
                        Settings.Add(filter, GeneralFilterStatus);
                        break;

                    case "AnimeFilter":
                        Settings.Add(filter, AnimeFilterStatus);
                        break;

                    case "PeopleFilter":
                        Settings.Add(filter, PeopleFilterStatus);
                        break;

                    case "SFWFilter":
                        Settings.Add(filter, SFWFilterStatus);
                        break;

                    case "SketchyFilter":
                        Settings.Add(filter, SketchyFilterStatus);
                        break;

                    case "NSFWFilter":
                        Settings.Add(filter, NSFWFilterStatus);
                        break;
                }
            }

            foreach (var displayCriterion in displayCriteria)
            {
                // The names of the "cases" should be the same as the name of the controls on the Settings pivot. 
                switch (displayCriterion)
                {
                    case "SortBy":
                        Settings.Add(displayCriterion, SortBy);
                        break;

                    case "OrderBy":
                        Settings.Add(displayCriterion, OrderBy);
                        break;
                }
            }
        }

        public static bool IsPreferenceInitialized()
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("IsPreferenceInitialized"))
            {
                return true;
            }
            return false;
        }

        public static void UpdatePreference(string filter, string status)
        {
            Settings[filter] = status;
            Settings.Save();
        }

        public static string GenerateSearchUrl(string searchTerm = "")
        {
            string url = "http://alpha.wallhaven.cc/search?";

            if (searchTerm != "")
            {
                url = url + "q=" + searchTerm + "&";
            }

            

            //adding categories filter to the url
            if (Settings["GeneralFilter"].ToString() == "checked")
            {
                url += "categories=1";
            }
            else
            {
                url += "categories=0";
            }

            if (Settings["AnimeFilter"].ToString() == "checked")
            {
                url += "1";
            }
            else
            {
                url += "0";
            }

            if (Settings["PeopleFilter"].ToString() == "checked")
            {
                url += "1";
            }
            else
            {
                url += "0";
            }

            //adding purity filter to the url
            if (Settings["SFWFilter"].ToString() == "checked")
            {
                url += "&purity=1";
            }
            else
            {
                url += "&purity=0";
            }

            if (Settings["SketchyFilter"].ToString() == "checked")
            {
                url += "1";
            }
            else
            {
                url += "0";
            }

            if (Settings["NSFWFilter"].ToString() == "checked")
            {
                url += "1";
            }
            else
            {
                url += "0";
            }

            //adding sorting to the url
            // Relevance, Date, Views, Favorites
            if (Settings["SortBy"].ToString() == "relevance")
            {
                url += "&sorting=relevance";
                UserPreference.SortByTitle = "relevance";
            }
            else if (Settings["SortBy"].ToString() == "dateadded")
            {
                url += "&sorting=date_added";

                if (Settings["OrderBy"].ToString() == "descending")
                    UserPreference.SortByTitle = "latest";
                else
                    UserPreference.SortByTitle = "date added";

                
            }
            else if (Settings["SortBy"].ToString() == "views")
            {
                url += "&sorting=views";
                UserPreference.SortByTitle = "views";
            }
            else if (Settings["SortBy"].ToString() == "favorites")
            {
                url += "&sorting=favorites";
                UserPreference.SortByTitle = "favorites";
            }

            //adding orderBy to the url
            if (Settings["OrderBy"].ToString() == "descending")
            {
                url += "&order=desc";
            }
            else if (Settings["OrderBy"].ToString() == "ascending")
            {
                url += "&order=asc";
            }

            //constructing the rest of the url
            url += "&page=";

            App.listRefreshed = true;
            return url;

        }
    }
}
