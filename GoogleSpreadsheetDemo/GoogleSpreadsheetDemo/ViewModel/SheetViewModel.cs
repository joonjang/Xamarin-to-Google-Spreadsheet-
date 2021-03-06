﻿using GoogleSpreadsheetDemo.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Xamarin.Forms;
using static GoogleSpreadsheetDemo.SpreadsheetModel;

namespace GoogleSpreadsheetDemo.ViewModel
{
    //https://github.com/saamerm/Xamarin-GoogleSheetsDB
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer


    class SheetViewModel : INotifyPropertyChanged
    {
        string[][] setArray;
        string jsonString;
        WatchModel watchModel;

        public ICommand SubmitJsonCommand { get; }
        public ICommand GetJsonCommand { get; }

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


            // from jsonString
            string tmp = "{\"DataArray\":[[\"Week 0\",\"DAY 1\"],[\"69\",\"69\",\"420\",\"420\",\"42069\",\"6969\"],[\"69\",\"69\",\"420\",\"420\",\"42069\",\"6969\"],[\"69\",\"69\",\"420\",\"420\",\"42069\",\"6969\"],[\"69\",\"69\",\"420\",\"420\",\"42069\",\"6969\"]]}";
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
            int sheetPageNumber = 1;

            Regex regex = new Regex(@"(?<=d/)(.*)(?=/)");
            MatchCollection matches = regex.Matches(SpreadsheetUrl);
            string spreadsheetCode = matches[0].Value;
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

            FilterThroughJsonList(cellInfoList);
            PackageForWatch();
        }

        void PackageForWatch()
        {
            if (watchModel == null)
            {
                return;
            };

            var jsonString = JsonConvert.SerializeObject(watchModel);


        }

        private string userChosenDay;
        public string UserChosenDay
        {
            get => userChosenDay;
            set
            {
                userChosenDay = value;
                OnPropertyChanged();
            }
        }

        void FilterThroughJsonList(List<string> jsonList)
        {
            // todo: for debugging hardcoded the day selections
            UserChosenDay = "DAY 2";

            watchModel = new WatchModel();
            watchModel.Sets = new List<string>();
            watchModel.Workouts = new List<string>();

            //the first array will always be the week information
            watchModel.Week = jsonList[0];
            // we already know which day is chosen based off user input
            watchModel.Day = UserChosenDay;

            int i = 0;

            // master loop bool, determines if loop is iterated
            bool workoutLoop = true;

            // triggers to recording list string into the WatchModel object
            bool workoutDayFound = false;
            bool registerToObject = false;

            while (workoutLoop)
            {
                i++;
                if(jsonList[i] == UserChosenDay)
                {
                    workoutDayFound = true;
                }
                if(jsonList[i].Contains("Set") && workoutDayFound)
                {
                    registerToObject = true;
                    watchModel.Sets.Add(jsonList[i]);
                }

                if (registerToObject && !jsonList[i].Contains("Set"))
                {
                    // if the next array is DAY then a new workout is found,
                    //  then object is populated and its time to end the loop
                    if (jsonList[i].Contains("DAY"))
                    {
                        workoutDayFound = false;
                        registerToObject = false;
                        workoutLoop = false;
                        break;
                    }

                    watchModel.Workouts.Add(jsonList[i]);

                    
                }


            }



        }
        
    }

}

