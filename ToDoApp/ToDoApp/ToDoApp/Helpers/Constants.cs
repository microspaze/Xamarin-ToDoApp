﻿using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace ToDoApp.Helpers
{
    public static class Constants
    {
        public static ObservableCollection<string> AddOptions = new ObservableCollection<string>() {
            "task",
            "list"
        };

        public static ObservableCollection<Color> ListColorList = new ObservableCollection<Color>() {
            Color.FromHex("#F9371C"),
            Color.FromHex("#F97C1C"),
            Color.FromHex("#F9C81C"),
            Color.FromHex("#41D0B6"),
            Color.FromHex("#2CADF6"),
            Color.FromHex("#6562FC"),
        };
    }
}
