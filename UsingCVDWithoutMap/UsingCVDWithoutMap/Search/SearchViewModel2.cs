namespace UsingCVDWithoutMap.Search
{
    using System;
    using System.Linq;
    using System.Windows;

    using Caliburn.Micro;

    using ESRI.ArcGIS.Client.FeatureService;

    using UsingCVDWithoutMap.FeatureService;
    using UsingCVDWithoutMap.Framework;
    using UsingCVDWithoutMap.Resources;
    using UsingCVDWithoutMap.SearchResults;

    /// <summary>
    /// ViewModel for Search view.
    /// </summary>
    /// <remarks>
    /// Responsibility : Contains interaction logic with the Search view and application logic.
    /// Functionality  : Loads search options from the service and provides searching functionality against
    ///                  the target service.
    /// Navigation     : To SearchResults after the search is done and there were results. If no results, stay
    ///                  on the page.
    /// </remarks>
    public class SearchViewModel2 : Screen
    {
        #region Variables

        private readonly FeatureService featureService;
        private readonly INavigationService navigationService;

        private bool isInitialized;
        private bool isBusy;
        private string busyText;
        
        private CodedValueDomain routeTypes;
        private string selectedRouteType;
        private string routeTypeText;

        private CodedValueDomain routeLevels;
        private string selectedRouteLevel;
        private string routeLevelText;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchViewModel2"/> class.
        /// </summary>
        /// <remarks>
        /// Contains only the default initialization. Lazy load other stuff in the <see scref="Screen.OnActivate"/> override.
        /// </remarks>
        public SearchViewModel2(INavigationService navigationService, IFeatureServiceManager featureServiceManager)
        {
            this.navigationService = navigationService;

            this.isInitialized = false;
            this.IsBusy = true;

            // Using demo feature service published at ArcGIS Online
            this.featureService = featureServiceManager.CreateOrGetFeatureService(@"http://services.arcgis.com/4PuGhqdWG1FwH2Yk/ArcGIS/rest/services/DemoCyclingRoutesFinland/FeatureServer/0");

            this.BusyText = UiTexts.Search_Initializing;     
        }

        #endregion // Constructor

        #region // Properties

        /// <summary>
        /// Gets or sets the value indicating if the ViewModel is busy ie. initializing or searching.
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return this.isBusy;
            }
            set
            {
                if (value.Equals(this.isBusy))
                {
                    return;
                }
                this.isBusy = value;
                this.NotifyOfPropertyChange(() => this.IsBusy);
            }
        }

        /// <summary>
        /// Gets or sets the text for the busy indication
        /// </summary>
        public string BusyText
        {
            get
            {
                return this.busyText;
            }
            set
            {
                if (value == this.busyText)
                {
                    return;
                }
                this.busyText = value;
                this.NotifyOfPropertyChange(() => this.BusyText);
            }
        }

        /// <summary>
        /// Gets or sets the RouteTypes that are used.
        /// </summary>
        public CodedValueDomain RouteTypes
        {
            get
            {
                return this.routeTypes;
            }
            set
            {
                if (Equals(value, this.routeTypes))
                {
                    return;
                }
                this.routeTypes = value;
                this.NotifyOfPropertyChange(() => this.RouteTypes);
            }
        }

        /// <summary>
        /// Gets or sets the selected RouteType.
        /// </summary>
        public string SelectedRouteType 
        {
            get
            {
                return this.selectedRouteType;
            }
            set
            {
                if (value == this.selectedRouteType)
                {
                    return;
                }
                this.selectedRouteType = value;
                this.NotifyOfPropertyChange(() => this.SelectedRouteType);
            }
        }

        /// <summary>
        /// Gets or sets the text for the Type.
        /// </summary>
        public string RouteTypeText 
        {
            get
            {
                return this.routeTypeText;
            }
            set
            {
                if (value == this.routeTypeText)
                {
                    return;
                }
                this.routeTypeText = value;
                this.NotifyOfPropertyChange(() => this.RouteTypeText);
            }
        }

        /// <summary>
        /// Gets or sets the RouteLevels that are used.
        /// </summary>
        public CodedValueDomain RouteLevels
        {
            get
            {
                return this.routeLevels;
            }
            set
            {
                if (Equals(value, this.routeLevels))
                {
                    return;
                }
                this.routeLevels = value;
                this.NotifyOfPropertyChange(() => this.RouteLevels);
            }
        }

        /// <summary>
        /// Gets or sets the selected RouteLevel.
        /// </summary>
        public string SelectedRouteLevel
        {
            get
            {
                return this.selectedRouteLevel;
            }
            set
            {
                if (value == this.selectedRouteLevel)
                {
                    return;
                }
                this.selectedRouteLevel = value;
                this.NotifyOfPropertyChange(() => this.SelectedRouteLevel);
            }
        }

        /// <summary>
        /// Gets or sets the text for the Level.
        /// </summary>
        public string RouteLevelText
        {
            get
            {
                return this.routeLevelText;
            }
            set
            {
                if (value == this.routeLevelText)
                {
                    return;
                }
                this.routeLevelText = value;
                this.NotifyOfPropertyChange(() => this.RouteLevelText);
            }
        }

        #endregion  // Properties

        #region Lifecycle events

        /// <summary>
        /// Called when activating. Handles lazy loading.
        /// </summary>
        protected override void OnActivate()
        {
            if (!this.isInitialized)
            {
                this.Initialize();
            }

            base.OnActivate();
        }

        #endregion // Lifecycle events

        #region Public methods

        /// <summary>
        /// Executes the search by selected values.
        /// </summary>
        public void Search()
        {
            var whereClause = string.Empty;
            
            if (this.SelectedRouteLevel == "Any" && this.selectedRouteType == "Any")
            {
                whereClause = "1=1";
            }

            if (this.SelectedRouteLevel != "Any")
            {
                whereClause = string.Format("{0} = {1}", "Level", this.RouteLevels.CodedValues.First(x => x.Value == this.SelectedRouteLevel).Key);
            }

            if (this.selectedRouteType != "Any")
            {
                if (!string.IsNullOrEmpty(whereClause))
                {
                    whereClause += " AND ";
                }

                whereClause += string.Format("{0} = {1}", "Type", this.RouteTypes.CodedValues.First(x => x.Value == this.SelectedRouteType).Key);
            }

            //this.featureService.SearchFeatures(whereClause,
            //    x =>
            //        {
            //            if (x.Count == 0)
            //            {
            //                MessageBox.Show("No results");
            //            }
            //            else
            //            {
            //                var key = Guid.NewGuid().ToString();
            //                NavigationContext.SetContext(key, x);
            //                this.navigationService.UriFor<SearchResultsViewModel>().WithParam(prop => prop.ContextKey, key).Navigate();
            //            }
            //        },
            //    y =>
            //        {
                        
            //        });
        }

        #endregion // Public methods

        private void Initialize()
        {
            this.featureService.Initialize();
        }
    }
}
