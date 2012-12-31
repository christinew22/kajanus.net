namespace AGOLIdentityManager
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Web.Script.Serialization;
    using System.Windows;

    using AGOLIdentityManager.Infrastructure;

    using ESRI.ArcGIS.Client;
    using ESRI.ArcGIS.Client.WebMap;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView
    {
        // Url to generate token to ArcGIS Online
        private const string GenerateTokenBaseUrl = @"https://www.arcgis.com/sharing/generateToken";

        // Query template that is filled with user information and attached to the query
        private const string GenerateTokenQueryTemplate = "?username={0}&password={1}&expiration={2}&TickCount={3}&client=requestip&f=json";

        // JavaScriptSerializer is provided by System.Web.Extensions assembly.
        private readonly JavaScriptSerializer serializer;

        public MainView()
        {
            this.serializer = new JavaScriptSerializer();
 
            this.InitializeComponent();

            // Preset textboxes
            this.WebMapTextBox.Text = "bdcf7f5023004c568ce810388a20e2aa";
            this.UsernameTextBox.Text = "kajanus_demo";
        }

        /// <summary>
        /// Event handler for Load WebMap. 
        /// </summary>
        private void LoadWebMapButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.IdentityManagerCheckBox.IsChecked.HasValue && this.IdentityManagerCheckBox.IsChecked.Value)
            {
                // Get secured web map using IdentityManager
                this.GetMapWithIdentityManager();
            }
            else
            {
                // Get secured web map using direct REST API call
                this.GetMapWithRestApi();
            }
        }

        /// <summary>
        /// Authenticates user and loads the WebMap using IdentityManager.
        /// </summary>
        private async void GetMapWithIdentityManager()
        {
            try
            {
                // Get credentials using IdentityManager
                var credentials = await IdentityManager.Current.GetCredentialsAsync(UsernameTextBox.Text, PasswordTextBox.Password);

                // Create Document and load given WebMap using Token from credentials
                var document = new Document { Token = credentials.Token };
                var map = await document.LoadMapAsync(this.WebMapTextBox.Text);

                // Set generated map to the UI
                this.WebMapHost.Content = map;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }

        /// <summary>
        /// Authenticates user and loads the WebMap using ArcGIS REST API directly on the authentication.
        /// </summary>
        private async void GetMapWithRestApi()
        {
            var client = new HttpClient();

            // Create complete REST call
            var arcgisComTokenUrl = GenerateTokenBaseUrl + string.Format(
                GenerateTokenQueryTemplate,
                UsernameTextBox.Text, 
                PasswordTextBox.Password, 
                30, 
                Environment.TickCount);

            try
            {
                // Send a request asynchronously continue when complete
                HttpResponseMessage response = await client.GetAsync(new Uri(arcgisComTokenUrl));

                // Check that response was successful or throw exception
                response.EnsureSuccessStatusCode();

                // Get response as a string
                var responseString = await response.Content.ReadAsStringAsync();

                // Get objects from the JSON string
                var results = this.serializer.DeserializeObject(responseString) as IDictionary<string, object>;

                var token = string.Empty;

                if (results.ContainsKey("token"))
                {
                    token = results["token"].ToString();
                }

                // Create Document and load given WebMap using Token
                var document = new Document { Token = token };
                var map = await document.LoadMapAsync(this.WebMapTextBox.Text);

                // Set generated map to the UI
                this.WebMapHost.Content = map;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }
    }
}
