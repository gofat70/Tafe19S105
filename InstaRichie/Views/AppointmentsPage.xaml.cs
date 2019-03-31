﻿// **************************************************************************
//Start Finance - An to manage your personal finances.
//Copyright(C) 2016  Jijo Bose

//Start Finance is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//Start Finance is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with Start Finance.If not, see<http://www.gnu.org/licenses/>.
// ***************************************************************************

using SQLite.Net;
using StartFinance.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppointmentsPage : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public AppointmentsPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            Results();
        }

        public void Results()
        {
            // Creating table
            conn.CreateTable<Appointments>();

            /// Refresh Data
            var query = conn.Table<Appointments>();
            AppointmentsListView.ItemsSource = query.ToList();
        }

        private async void AddData(object sender, RoutedEventArgs e)
        {
            try
            {
                if (EventNameText.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("No Event Name entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (LocationText.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("No Location entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.Insert(new Appointments()
                    {
                        EventName = EventNameText.Text,
                        Location = LocationText.Text,
                        EventDate = EventDatePick.Date.DateTime,
                        StartTime = StartTimePick.Time,
                        EndTime = EndTimePick.Time
                    });
                    Results();
                }
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the Event Name or entered an invalid data", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("A Similar Event Name already exists, Try a different name", "Oops..!");
                    await dialog.ShowAsync();
                }
            }
        }

        private async void DeleteAppointment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string AccSelection = ((Appointments)AppointmentsListView.SelectedItem).EventName;
                if (AccSelection == "")
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<Appointments>();
                    var query1 = conn.Table<Appointments>();
                    var query3 = conn.Query<Appointments>("DELETE FROM Appointments WHERE EventName ='" + AccSelection + "'");
                    AppointmentsListView.ItemsSource = query1.ToList();
                }
            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                await dialog.ShowAsync();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
            SaveIcon.Visibility = Visibility.Collapsed;
            EditIcon.Visibility = Visibility.Collapsed;
        }

        private void AppointmentsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditIcon.Visibility = Visibility.Visible;
            DeleteIcon.Visibility = Visibility.Collapsed;
        }

        private void EditData(object sender, RoutedEventArgs e)
        {
            SaveIcon.Visibility = Visibility.Visible;
            AddIcon.Visibility = Visibility.Collapsed;
        }

        private async void SaveData(object sender, RoutedEventArgs e)
        {
            {
                try
                {
                    string AccSelection = ((Appointments)AppointmentsListView.SelectedItem).EventName;
                    if (AccSelection == "")
                    {
                        MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                        await dialog.ShowAsync();
                    }
                    else
                    {
                        conn.CreateTable<Appointments>();
                        var query1 = conn.Table<Appointments>();
                        var query3 = conn.Query<Appointments>("UPDATE FROM Appointments WHERE EventName ='" + AccSelection + "'");
                        AppointmentsListView.ItemsSource = query1.ToList();
                    }
                }
                catch (NullReferenceException)
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
            }
        }
    }
}
