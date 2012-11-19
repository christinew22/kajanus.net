namespace UsingCVDWithoutMap.SearchResults
{
    using ESRI.ArcGIS.Client;
    
    using Framework;

    public class SearchResultsViewModel : ViewModelBase
    {
        private GraphicCollection results;

        public SearchResultsViewModel()
        { 
        }

        public GraphicCollection Results
        {
            get
            {
                return this.results;
            }
            set
            {
                if (Equals(value, this.results))
                {
                    return;
                }
                this.results = value;
                this.NotifyOfPropertyChange(() => this.Results);
            }
        }

        protected override void LoadNavigationContext(string key)
        {
            this.Results = NavigationContext.GetContext<GraphicCollection>(key);
            
            base.LoadNavigationContext(key);
        }
    }
}
