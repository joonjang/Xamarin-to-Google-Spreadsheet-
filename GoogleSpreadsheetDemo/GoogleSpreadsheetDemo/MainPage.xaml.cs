using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GoogleSpreadsheetDemo
{
    //https://github.com/saamerm/Xamarin-GoogleSheetsDB
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        string[][] setArray;
        string jsonString;
       
        public MainPage()
        {
            //listToArray = new List<string>();
            setArray = new string[5][];
            InitializeComponent();
            BindingContext = this;
        }


        async void SubmitButton_Pressed(System.Object sender, System.EventArgs e)
        {
            var client = new HttpClient();


            var model = new DataArrayModel()
            {
                DataArray = setArray
            };

            var uri = "https://script.google.com/macros/s/AKfycby2BGbNJwvzgqp4hay1CR0V3cznlND4u3Ra2-mysvdELCbO3II/exec";
            var jsonString = JsonConvert.SerializeObject(model);


            ////
            //string tmp = "{\"DataArray\":[[\"Week0\",\"DAY 1\"],[\"69\",\"69\",\"420\",\"420\",\"42069\",\"6969\"],[\"69\",\"69\",\"420\",\"420\",\"42069\",\"6969\"],[\"69\",\"69\",\"420\",\"420\",\"42069\",\"6969\"],[\"69\",\"69\",\"420\",\"420\",\"42069\",\"6969\"]]}";
            //var requestContent = new StringContent(tmp);
            ////

            var requestContent = new StringContent(jsonString);

            var result = await client.PostAsync(uri, requestContent);
            var resultContent = await result.Content.ReadAsStringAsync();

            ResponseModel response = null;
            try
            {
                response = JsonConvert.DeserializeObject<ResponseModel>(resultContent);
            }
            catch { }

            ProcessResponse(response);
        }

        private void ProcessResponse(ResponseModel responseModel)
        {
            ResultLabel.IsVisible = true;
            ResultLabel.Text = responseModel.Message;
            if (responseModel.Status == "SUCCESS")
                ResultLabel.TextColor = Color.Black;
            else
                ResultLabel.TextColor = Color.Red;
        }

        private void ArrayButton_Pressed(object sender, EventArgs e)
        {
            if(this.setArray[0] is null)
            {
                this.setArray[0] = new string[] { WeekEntry.Text, DayEntry.Text };
            }

           

           this.setArray[arrayIteration] = new string[] 
            {
                SetOneEntry.Text,
                SetTwoEntry.Text,
                SetThreeEntry.Text,
                SetFourEntry.Text,
                SetFiveEntry.Text,
                SetSixEntry.Text
            };

            ArrayIteration++;
        }

        private int arrayIteration = 1;
        public int ArrayIteration
        {
            get { return arrayIteration; }
            set
            {
                arrayIteration = value;
                OnPropertyChanged(nameof(ArrayIteration));
            }
        }

        private void SpreadsheetGetButton_Pressed(object sender, EventArgs e)
        {
            ProcessSpreadsheetUrl();
        }

        private void ProcessSpreadsheetUrl()
        {
            string spreadUrl = SpreadsheetURL.Text;
            Regex regex = new Regex(@"(?<=d/)(.*)(?=/)");
            MatchCollection matches = regex.Matches(spreadUrl);
            string spreadsheetCode = matches[0].Value;
            int sheetPageNumber = 1;
            string jsonUrl = "https://spreadsheets.google.com/feeds/cells/" + spreadsheetCode + "/" + sheetPageNumber + "/public/full?alt=json";
            using (WebClient wc = new WebClient())
            {
                jsonString = wc.DownloadString(jsonUrl);
            }
            var json = 
        }
    }
}
