using GoogleSpreadsheetDemo.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using static GoogleSpreadsheetDemo.SpreadsheetModel;
using Entry = GoogleSpreadsheetDemo.SpreadsheetModel.Entry;

namespace GoogleSpreadsheetDemo
{
    //https://github.com/saamerm/Xamarin-GoogleSheetsDB
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
       
        public MainPage()
        {
            BindingContext = new SheetViewModel();
            InitializeComponent();
            
        }

    }
}
