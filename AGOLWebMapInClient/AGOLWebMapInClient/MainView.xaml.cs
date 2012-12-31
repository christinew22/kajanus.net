namespace AGOLWebMapInClient
{
    using System.Windows;

    using ESRI.ArcGIS.Client.WebMap;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView
    {
        public MainView()
        {
            InitializeComponent();

            var webMap = new Document();
            webMap.GetMapCompleted += (sender, args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ToString());
                    }

                    this.WebMapHost.Content = args.Map;
                };

            // WebMap used is the same that we created in the Esri Code Camp.
            webMap.GetMapAsync("1e7d9301457943b08034cb89abea905b");
        }
    }
}