﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Media;
using System.Windows.Media.Animation;
using System.ComponentModel;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region main window
        /// <summary> the constractor start the intlize consractor of the data </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// cancel the option to clik x to close the window 
        /// </summary>
        /// <param name="e">close window</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = false;
            IEnumerable<BO.DroneToList> DroneToList = from item in AccessIbl.GetDroneList()
                                                       where item.Statuses == BO.DroneStatuses.inMaintenance
                                                       select item;
            if (DroneToList.Any())
            {
                foreach (var item in DroneToList)//when we close the app we need to put all of the drones that are inmintinan to free
                                                 //becuse we dont have such area to store the in charge drones 
                {
                    AccessIbl.ReleaseDroneFromCharging(item.Id);
                }
            }
            Application.Current.Shutdown();     
        }

        // we crate an obejt that give us accses to the ibl intrface  
        public BlApi.IBL AccessIbl = BlApi.BlFactory.GetBL();

        /// <summary>
        /// Login to a manager or client interface.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Blogin_Click(object sender, RoutedEventArgs e)
        {
           
            switch (Blogin.Content)
            {
                case "כניסה כמנהל":
                    if (TBadmin.Text == "admin" && PBadminID.Password == "770")
                    {
                        new ListView(AccessIbl).ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("שם משתמש או סיסמא אינם נכונים", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    
                    break;
                case "כניסת לקוח":
                    try
                    {
                        AccessIbl.GetCustomer(int.Parse(TBuserID.Password));
                        new ClientWindow(AccessIbl, int.Parse(TBuserID.Password)).ShowDialog();
                    }
                    catch (BO.NonExistentObjectException ex)
                    {
                        MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                default:   
                    break;
            }       
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void enter_Loaded(object sender, RoutedEventArgs e)
        {
            AddOn.Opacity = 0;
            DoubleAnimation Animmation = new DoubleAnimation(0, 100, TimeSpan.FromSeconds(10.5));
            PBloding.BeginAnimation(ProgressBar.ValueProperty, Animmation);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PBloding_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(PBloding.Value == 100)
            {
                Blogin.IsEnabled = true;
                DoubleAnimation doubleAnimmation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(5));
                AddOn.BeginAnimation(Grid.OpacityProperty, doubleAnimmation);
                DoubleAnimation DSF = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(1));
                Disiaper.BeginAnimation(Grid.OpacityProperty, DSF);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BNewUser_Click(object sender, RoutedEventArgs e)
        {
            ListView listView = new ListView(AccessIbl);
            new CustomerWindow(AccessIbl, listView).Show();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TICadmin_GotFocus(object sender, RoutedEventArgs e)
        {
            Blogin.Visibility = Visibility.Visible;
            Blogin.Content = "כניסה כמנהל";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TIUser_GotFocus(object sender, RoutedEventArgs e)
        {
            Blogin.Visibility = Visibility.Visible;
            Blogin.Content = "כניסת לקוח";
        }

        private void TBadmin_KeyUp(object sender, KeyEventArgs e)
        {
            if (TBadmin.Text.Length != 0)
            {
                Blogin.IsEnabled = true;
            }
            else
            {
                Blogin.IsEnabled = false;
            }
        }

        private void TBuserID_KeyUp(object sender, KeyEventArgs e)
        {
            if (TBuserID.Password.Length != 0)
            {
                Blogin.IsEnabled = true;
            }
            else
            {
                Blogin.IsEnabled = false;
            }
        }

        private void PBadminID_KeyUp(object sender, KeyEventArgs e)
        {
            if (PBadminID.Password.Length != 0)
            {
                Blogin.IsEnabled = true;
            }
            else
            {
                Blogin.IsEnabled = false;
            }
        }

        private void PBadminID_KeyDown(object sender, KeyEventArgs e)
        {
            // take only the kyes we alowed 
            if (e.Key < Key.D0 || e.Key > Key.D9)
            {
                if (e.Key < Key.NumPad0 || e.Key > Key.NumPad9) // we want keys from the num pud too
                {
                    e.Handled = true;
                }
                else
                {
                    e.Handled = false;
                }
            }
        }

        private void TBuserID_KeyDown(object sender, KeyEventArgs e)
        {
            // take only the kyes we alowed 
            if (e.Key < Key.D0 || e.Key > Key.D9)
            {
                if (e.Key < Key.NumPad0 || e.Key > Key.NumPad9) // we want keys from the num pud too
                {
                    e.Handled = true;
                }
                else
                {
                    e.Handled = false;
                }
            }
        }
    }
}
