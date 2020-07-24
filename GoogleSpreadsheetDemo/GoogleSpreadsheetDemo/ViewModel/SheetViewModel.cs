using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using static GoogleSpreadsheetDemo.SpreadsheetModel;

namespace GoogleSpreadsheetDemo.ViewModel
{
    class SheetViewModel : INotifyPropertyChanged
    {
        string[][] setArray;
        string jsonString;

        public Command SubmitJsonCommand { get; }
        public Command GetJsonCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public SheetViewModel()
        {
            setArray = new string[5][];
            SubmitJsonCommand = new Command(ApplyJsonToSheet);
            GetJsonCommand = new Command(GetSpreadsheetJson);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        async void ApplyJsonToSheet()
        {
            var client = new HttpClient();


            var model = new DataArrayModel()
            {
                DataArray = setArray
            };

            // todo: for debugging where this Json data is sent to google sheet, make it user entered url
            var uri = "https://script.google.com/macros/s/AKfycby2BGbNJwvzgqp4hay1CR0V3cznlND4u3Ra2-mysvdELCbO3II/exec";
            var jsonString = JsonConvert.SerializeObject(model);


            //
            string tmp = "{\"DataArray\":[[\"Week0\",\"DAY 1\"],[\"69\",\"69\",\"420\",\"420\",\"42069\",\"6969\"],[\"69\",\"69\",\"420\",\"420\",\"42069\",\"6969\"],[\"69\",\"69\",\"420\",\"420\",\"42069\",\"6969\"],[\"69\",\"69\",\"420\",\"420\",\"42069\",\"6969\"]]}";
            var requestContent = new StringContent(tmp);
            //

            //var requestContent = new StringContent(jsonString);

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

        private bool resultResponseBool = false;
        public bool ResultResponseBool
        {
            get => resultResponseBool;
            set
            {
                resultResponseBool = value;
                OnPropertyChanged();
            }
        }

        private string resultResponseText;
        public string ResultResponseText
        {
            get => resultResponseText;
            set
            {
                resultResponseText = value;
                OnPropertyChanged();
            }
        }

        private void ProcessResponse(ResponseModel responseModel)
        {
            ResultResponseBool = true;
            ResultResponseText = responseModel.Message;
        }


        private string spreadsheetUrl;
        public string SpreadsheetUrl
        {
            get => spreadsheetUrl;
            set
            {
                spreadsheetUrl = value;
                OnPropertyChanged();
            }
        }

        








        //private void ArrayButton_Pressed(object sender, EventArgs e)
        //{
        //    if (this.setArray[0] is null)
        //    {
        //        this.setArray[0] = new string[] { WeekEntry.Text, DayEntry.Text };
        //    }



        //    this.setArray[arrayIteration] = new string[]
        //        {
        //    SetOneEntry.Text,
        //    SetTwoEntry.Text,
        //    SetThreeEntry.Text,
        //    SetFourEntry.Text,
        //    SetFiveEntry.Text,
        //    SetSixEntry.Text
        //        };

        //    ArrayIteration++;
        //}

        //private int arrayIteration = 1;

        

        //public int ArrayIteration
        //{
        //    get { return arrayIteration; }
        //    set
        //    {
        //        arrayIteration = value;
        //        OnPropertyChanged(nameof(ArrayIteration));
        //    }
        //}

        private void GetSpreadsheetJson()
        {
            // todo: for debugging, make text user input and saved by preference
            SpreadsheetUrl = "https://docs.google.com/spreadsheets/d/1XWQNN76FJgt3X_213zwqblrOu2eSI0Tss1Zt1jPNLi0/edit#gid=524439697";

            Regex regex = new Regex(@"(?<=d/)(.*)(?=/)");
            MatchCollection matches = regex.Matches(SpreadsheetUrl);
            string spreadsheetCode = matches[0].Value;
            int sheetPageNumber = 1;
            string jsonUrl = "https://spreadsheets.google.com/feeds/cells/" + spreadsheetCode + "/" + sheetPageNumber + "/public/full?alt=json";
            using (WebClient wc = new WebClient())
            {
                jsonString = wc.DownloadString(jsonUrl);
            }
            Root jsonObject = JsonConvert.DeserializeObject<Root>(jsonString);

            List<string> cellInfoList = new List<string>();
            foreach (var cell in jsonObject.Feed.Entry)
            {
                cellInfoList.Add(cell.Content.T);

            }

        }
        
    }

}

