namespace UsingCVDWithoutMap.Search
{
    using Caliburn.Micro;
    using ESRI.ArcGIS.Client;
    using ESRI.ArcGIS.Client.FeatureService;
    using ESRI.ArcGIS.Client.Tasks;
    using Framework;
    using Resources;
    using System;
    using System.Linq;
    using System.Windows;

    /// <summary>
    /// ViewModel for Search view.
    /// </summary>
    /// <remarks>
    /// Contains interaction logic with the Search view and executes search on background.
    /// Loads search options from the service and provides searching functionality against the target service.
    /// </remarks>
    public class SearchViewModel : Screen
    {
        #region Variables

        private readonly FeatureLayer targetService;

        private CodedValueDomain routeTypes;
        private CodedValueDomain routeLevels;

        private bool isBusy;
        private string busyText;

        private string selectedRouteType;
        private string routeTypeText;

        private string selectedRouteLevel;
        private string routeLevelText;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchViewModel"/> class.
        /// </summary>
        /// <remarks>
        /// Contains only the default initialization. Lazy load other stuff in the <see scref="Screen.OnActivate"/> override.
        /// </remarks>
        public SearchViewModel()
        {
            // Create feature layer that is used to internally to work with the service
            // Note the Mode definition. This is needed to work with the FeatureLayer without giving it a Map instance!
            this.targetService = new FeatureLayer
            {
                Url = @"http://services.arcgis.com/4PuGhqdWG1FwH2Yk/ArcGIS/rest/services/DemoCyclingRoutesFinland/FeatureServer/0",
                Mode = FeatureLayer.QueryMode.Snapshot
            };

            // Set initialization events
            this.targetService.Initialized += this.InitializedCompleted;
            this.targetService.InitializationFailed += this.InitializationFailed;

            // Set update events
            this.targetService.UpdateCompleted += this.SearchUpdateCompleted;
            this.targetService.UpdateFailed += this.SearchUpdateFailed;

            // Initialize used layer
            this.targetService.Initialize();

            // Set busy indication values.
            this.IsBusy = true;
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
        /// Gets or sets the text for the Type. Using field alias.
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
        /// Gets or sets the text for the Level. Using field alias.
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

        #region Public methods

        /// <summary>
        /// Executes the search by selected values.
        /// </summary>
        public void Search()
        {
            // Set Search indication
            this.IsBusy = true;
            this.BusyText = UiTexts.Search_Searching;

            // Create Where clause for REST query
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

            // Execute search with given Where clause
            // Set query to include all fields and set Where clause
            this.targetService.OutFields = new OutFields { "*" };
            this.targetService.Where = whereClause;

            // Force update for the layer to actually execute the search
            this.targetService.Update();
        }

        #endregion // Public methods

        #region Event handlers

        /// <summary>
        /// Handler for completed event when the <see cref="FeatureLayer.Update"/> is finished.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The event arguments.</param>
        private void SearchUpdateCompleted(object sender, EventArgs args)
        {
            // Remove search indication
            this.IsBusy = false;
            this.BusyText = string.Empty;

            // Just visualize that we got something from the Service
            if (this.targetService.Graphics.Count == 0)
            {
                // Never use MessageBoxes from ViewModel but I am lazy here.
                MessageBox.Show("No results");
            }
            else
            {
                // Never use MessageBoxes from ViewModel but I am lazy here.
                MessageBox.Show(string.Format("Results found : {0}", this.targetService.Graphics.Count));
            }
        }

        /// <summary>
        /// Handler for failed event when the <see cref="FeatureLayer.Update"/> didn't succeed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The event arguments.</param>
        private void SearchUpdateFailed(object sender, TaskFailedEventArgs args)
        {
            // Never use MessageBoxes from ViewModel but I am lazy here.
            MessageBox.Show(args.Error.ToString());
        }

        /// <summary>
        /// Handler for <see cref="FeatureLayer"/> initialization failed event. 
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The event arguments.</param>
        /// <remarks>
        /// Note where the initialization error is located.
        /// </remarks>
        private void InitializationFailed(object sender, EventArgs args)
        {
            // Getting initialization error and delivering it forward
            var layer = sender as FeatureLayer;

            // Never use MessageBoxes from ViewModel but I am lazy here.
            MessageBox.Show(layer.InitializationFailure.ToString());
        }

        /// <summary>
        /// Handler for <see cref="FeatureLayer"/> initialized. Used to extract the CodedValueDomains to properties.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The event arguments.</param>
        private void InitializedCompleted(object sender, EventArgs args)
        {
            // Initialize types from the Service
            // Get alias names from the fields. In this case, we can use those names in the UI
            // since no localization issues and those aliases are provided.
            this.RouteTypeText = this.targetService.GetFieldAliasName("Type");
            this.RouteLevelText = this.targetService.GetFieldAliasName("Level");

            // Set RouteTypes. 
            // Also add "Any" to the collection for the better search experience.
            var types = this.targetService.GetDomainByName("RouteType");
            types.CodedValues.Add(-1, "Any");
            this.RouteTypes = types;

            // Set default selection to "Any".
            this.SelectedRouteType = this.RouteTypes.CodedValues.Values.ToList().Min();

            // Set RouteLevels.
            // Also add "Any" to the collection for the better search experience.
            var levels = this.targetService.GetDomainByName("RouteDifficulty");
            levels.CodedValues.Add(-1, "Any");
            this.RouteLevels = levels;

            // Set default selection to "Any".
            this.SelectedRouteLevel = this.RouteLevels.CodedValues.Values.ToList().Min();

            this.IsBusy = false;
            this.BusyText = string.Empty;
        }

        #endregion // Event handlers
    }
}
