﻿using BO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace PL
{
    /// <summary>
    /// Interaction logic for DroneWindow.xaml.
    /// This window covers a drone interface.
    /// </summary>
    public partial class DroneWindow : Window
    {
        ///Access object to the BL class.
        public BlApi.IBL AccessIbl;
        /// <summary> the calling window, becuse we want to use it here </summary> 
        private ListView listWindow;
        /// <summary> a bool to help us disable the x bootun  </summary>
        public bool ClosingWindow { get; private set; } = true;

        #region drone to add
        /// <summary>
        /// consractor for add drone option 
        /// </summary>
        /// <param name="bl"></param>
        /// <param name="_DroneListWindow"></param>
        public DroneWindow(BlApi.IBL bl, ListView _DroneListWindow)
        {
            InitializeComponent();
            //set the Width & Height for this window
            Width = 440;
            Height = 540;
            addDrone.Visibility = Visibility.Visible;
            // set the bl obeject and the drone list window that we got from the priviose window
            AccessIbl = bl;
            listWindow = _DroneListWindow;
            // the combobox use it to show the Weight Categories
            TBWeight.ItemsSource = Enum.GetValues(typeof(WeightCategories));
            // the combobox use it to show the BaseStation ID
            BaseStationID.ItemsSource = AccessIbl.GetBaseStationList(x => x.FreeChargeSlots > 0);
            BaseStationID.DisplayMemberPath = "Id";
        }

        /// <summary>
        /// A function that sends the new drone and adds it to the data after tests in the logical layer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendToBl_Click(object sender, RoutedEventArgs e)
        {
            // If all the fields are full
            if (TBModel.Text.Length != 0 && TBID.Text.Length != 0 && BaseStationID.SelectedItem != null && TBWeight.SelectedItem != null)
            {
                DroneToList newdrone = new DroneToList
                {
                    Id = int.Parse(TBID.Text),
                    Model = TBModel.Text,
                    MaxWeight = (WeightCategories)TBWeight.SelectedIndex,
                };
                // try to add the drone if fals return a MessageBox
                try
                {
                    AccessIbl.AddDrone(newdrone, ((BaseStationsToList)BaseStationID.SelectedItem).Id);
                    MessageBoxResult result = MessageBox.Show("The operation was successful", "info", MessageBoxButton.OK, MessageBoxImage.Information);
                    switch (result)
                    {
                        case MessageBoxResult.OK:
                            newdrone = AccessIbl.GetDroneList().ToList().Find(i => i.Id == newdrone.Id);
                            listWindow.DroneToLists.Add(newdrone); //Observer update
                            //Handles required window updates
                            listWindow.IsEnabled = true;
                            ClosingWindow = false;
                            Close();
                            break;
                        default:
                            break;
                    }
                }
                // Area responsible for error capture
                catch (AddAnExistingObjectException ex)
                {
                    MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    TBID.Text = "";
                    TBID.BorderBrush = Brushes.Red; //bonus 
                }
                catch (NonExistentObjectException ex)
                {
                    MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    TBID.Text = "";
                    TBID.BorderBrush = Brushes.Red;//bonus 
                }
                catch (NoFreeChargingStations ex)
                {
                    MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (NonExistentEnumException ex)
                {
                    MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("נא ודאו שכל השדות מלאים", "!שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        #region keybord events
        /// <summary>
        /// disable the non numbers keys
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TBID_KeyDown(object sender, KeyEventArgs e)
        {
            if (TBID.BorderBrush != Brushes.Gray)
            {
                TBID.BorderBrush = Brushes.Gray;
            }
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

        /// <summary>
        /// set the color back to gray
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TBModel_KeyDown(object sender, KeyEventArgs e)
        { 
            if (TBID.BorderBrush != Brushes.Gray) 
            {
                TBID.BorderBrush = Brushes.Gray;
            }
        }
        #endregion

        #region close
        /// <summary>
        /// cancel the option to clik x to close the window 
        /// </summary>
        /// <param name="e">close window</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = ClosingWindow;
        }    
        /// <summary>
        /// to aloow closing again but just in the spcific close boutoon 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bclose_Click(object sender, RoutedEventArgs e)
        {
            listWindow.IsEnabled = true;
            ClosingWindow = false;
            Close();
        }
        #endregion close

        #endregion

        #region drone in operations
        //hold the drone that we send for update or work on 
        public Drone MyDrone;
        //indexe of the drone how chosse by doubly click
        public int indexDrone;
        //Update to the package window
        ParcelWindow parcelWindow;

        /// <summary>
        /// constractor for acction staet  And updates the views accordingly
        /// </summary>
        /// <param name="bl">accses to ibl</param>
        /// <param name="_DroneListWindow">the call window</param>
        /// <param name="id">the drone id that chosen</param>
        /// <param name="_indexDrone">/indexe of the drone in the list</param>
        public DroneWindow(BlApi.IBL bl, ListView _DroneListWindow, DroneToList droneTo, int _indexDrone, ParcelWindow _parcelWindow = null)
        {
            InitializeComponent();
            // open the grid for the user
            updateDrone.Visibility = Visibility.Visible;
            //save the variables that we get in the fanction
            indexDrone = _indexDrone;
            AccessIbl = bl;
            listWindow = _DroneListWindow;
            parcelWindow = _parcelWindow;
            //to conect the binding to set the value of my drone to the proprtis
            MyDrone = bl.GetDrone(droneTo.Id);
            DataContext = MyDrone;
            //set the color of the battry
            if (MyDrone.BatteryStatus < 50)
            {
                if (MyDrone.BatteryStatus > 10)
                {
                    PBbatr.Foreground = Brushes.YellowGreen;
                }
                else
                {
                    PBbatr.Foreground = Brushes.Red;
                }
            }
            //The switch checks the drone's status value and opens buttons 
            switch ((DroneStatuses)MyDrone.Statuses)
            {
                case DroneStatuses.free:
                    BSendToCharge.Visibility = Visibility.Visible;
                    BAssignPackage.Visibility = Visibility.Visible;
                    break;

                case DroneStatuses.inMaintenance:
                    BReleaseDrone.Visibility = Visibility.Visible;
                    BAssignPackage.Visibility = Visibility.Visible;
                    BAssignPackage.IsEnabled = false;
                    break;

                case DroneStatuses.busy:
                    GRIDparcelInDelivery.Visibility = Visibility.Visible;
                    TBnotAssigned.Visibility = Visibility.Hidden;
                    //check the status to open the right button
                    if (MyDrone.Delivery.OnTheWayToTheDestination)
                    {
                        BDeliveryPackage.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        BPickedUp.Visibility = Visibility.Visible;
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// the fanction update the modal of the drone And updates the views accordingly 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BModalUpdate_Click(object sender, RoutedEventArgs e)
        {
            AccessIbl.UpdateDroneName(MyDrone.Id, TBmodel.Text);
            MessageBoxResult result = MessageBox.Show("The operation was successful", "info", MessageBoxButton.OK, MessageBoxImage.Information);
            switch (result)
            {
                case MessageBoxResult.OK:
                    BModalUpdate.IsEnabled = false;
                    listWindow.StatusDroneSelectorChanged();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// the fanction send the drone to charge And updates the views accordingly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BSendToCharge_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AccessIbl.SendingDroneforCharging(MyDrone.Id);

                MessageBoxResult result = MessageBox.Show("The operation was successful", "info", MessageBoxButton.OK, MessageBoxImage.Information);
                switch (result)
                {
                    case MessageBoxResult.OK:
                        listWindow.StatusDroneSelectorChanged();

                        //Update the list observer
                        int IdOfBaseStation = AccessIbl.GetBaseCharge(MyDrone.Id);
                        int indexOfBaseStationInTheObservable = listWindow.BaseStationToLists.IndexOf(listWindow.BaseStationToLists.First(x => x.Id == IdOfBaseStation));
                        listWindow.BaseStationToLists[indexOfBaseStationInTheObservable] = AccessIbl.GetBaseStationList().First(x => x.Id == IdOfBaseStation);//Update the list observer

                        //to conecct the binding to set the value of my drone to the proprtis
                        MyDrone = AccessIbl.GetDrone(MyDrone.Id);
                        DataContext = MyDrone;
                        //display update
                        BSendToCharge.Visibility = Visibility.Hidden;
                        BReleaseDrone.Visibility = Visibility.Visible;
                        BAssignPackage.IsEnabled = false;
                        break;
                    default:
                        break;
                }
            }
            catch (TheDroneCanNotBeSentForCharging ex)
            {
                MessageBox.Show(ex.Message, "info");
            }
        }
        /// <summary>
        ///  release the drone from charge And updates the views accordingly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BReleaseDrone_Click(object sender, RoutedEventArgs e)
        {
            int IdOfBaseStation = AccessIbl.GetBaseCharge(MyDrone.Id);//Saving the station tax for the purpose of updating the list of stations on line 350

            AccessIbl.ReleaseDroneFromCharging(MyDrone.Id); //no need for try becuse we chose drone from the list and the exption for non exsit item cant be trown 

            MessageBoxResult result = MessageBox.Show("The operation was successful", "info", MessageBoxButton.OK, MessageBoxImage.Information);//לטלפל בX
            switch (result)
            {
                case MessageBoxResult.OK:
                    listWindow.StatusDroneSelectorChanged();

                    //Update the list observer
                    int indexOfBaseStationInTheObservable = listWindow.BaseStationToLists.IndexOf(listWindow.BaseStationToLists.First(x => x.Id == IdOfBaseStation));
                    listWindow.BaseStationToLists[indexOfBaseStationInTheObservable] = AccessIbl.GetBaseStationList().First(x => x.Id == IdOfBaseStation);

                    //to conecct the binding to set the value of my drone to the proprtis
                    MyDrone = AccessIbl.GetDrone(MyDrone.Id);
                    DataContext = MyDrone;

                    BSendToCharge.Visibility = Visibility.Visible;
                    BReleaseDrone.Visibility = Visibility.Hidden;
                    BAssignPackage.IsEnabled = true;

                    //battery colors.
                    if (MyDrone.BatteryStatus < 50)
                    {
                        if (MyDrone.BatteryStatus > 10)
                        {
                            PBbatr.Foreground = Brushes.YellowGreen;
                        }
                        else
                        {
                            PBbatr.Foreground = Brushes.Red;
                        }
                    }
                    else //MyDrone.BatteryStatus > 50
                    {
                        PBbatr.Foreground = Brushes.LimeGreen;

                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        ///  Assign Package to the drone And updates the views accordingly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BAssignPackage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AccessIbl.AssignPackageToDdrone(MyDrone.Id);

                MessageBoxResult result = MessageBox.Show("The operation was successful", "info", MessageBoxButton.OK, MessageBoxImage.Information);
                switch (result)
                {
                    case MessageBoxResult.OK:
                        listWindow.StatusDroneSelectorChanged();

                        //to conecct the binding to set the value of my drone to the proprtis
                        MyDrone = AccessIbl.GetDrone(MyDrone.Id);
                        DataContext = MyDrone;

                        //update list of parcels
                        int indexOfParcelInTheObservable = listWindow.ParcelToLists.IndexOf(listWindow.ParcelToLists.First(x => x.Id == MyDrone.Delivery.Id));
                        listWindow.ParcelToLists[indexOfParcelInTheObservable] = AccessIbl.GetParcelList().First(x => x.Id == MyDrone.Delivery.Id);
                        // disply update affter the oppertion
                        BSendToCharge.IsEnabled = false;
                        BAssignPackage.Visibility = Visibility.Hidden;
                        BPickedUp.Visibility = Visibility.Visible;
                        GRIDparcelInDelivery.Visibility = Visibility.Visible;
                        TBnotAssigned.Visibility = Visibility.Hidden;
                        break;
                    default:
                        break;
                }
            }
            catch (NoSuitablePsrcelWasFoundToBelongToTheDrone ex)
            {
                MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (DroneCantBeAssigend ex)
            {
                MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Picked Up the parcel And updates the views accordingly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BPickedUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AccessIbl.PickedUpPackageByTheDrone(MyDrone.Id);

                MessageBoxResult result = MessageBox.Show("The operation was successful", "info", MessageBoxButton.OK, MessageBoxImage.Information);
                switch (result)
                {
                    case MessageBoxResult.OK:
                        listWindow.StatusDroneSelectorChanged();

                        //to conecct the binding to set the value of my drone to the proprtis
                        MyDrone = AccessIbl.GetDrone(MyDrone.Id);
                        DataContext = MyDrone;

                        // Update the list of packages
                        int indexOfParcelInTheObservable = listWindow.ParcelToLists.IndexOf(listWindow.ParcelToLists.First(x => x.Id == MyDrone.Delivery.Id));
                        listWindow.ParcelToLists[indexOfParcelInTheObservable] = AccessIbl.GetParcelList().First(x => x.Id == MyDrone.Delivery.Id);

                        // Sender update in customer list
                        int indexOfSenderCustomerInTheObservable = listWindow.CustomerToLists.IndexOf(listWindow.CustomerToLists.First(x => x.Id == MyDrone.Delivery.Sender.Id));
                        listWindow.CustomerToLists[indexOfSenderCustomerInTheObservable] = AccessIbl.GetCustomerList().First(x => x.Id == MyDrone.Delivery.Sender.Id);

                        // Recipient update in the customer list
                        int indexOfReceiverCustomerInTheObservable = listWindow.CustomerToLists.IndexOf(listWindow.CustomerToLists.First(x => x.Id == MyDrone.Delivery.Receiver.Id));
                        listWindow.CustomerToLists[indexOfReceiverCustomerInTheObservable] = AccessIbl.GetCustomerList().First(x => x.Id == MyDrone.Delivery.Receiver.Id);

                        BPickedUp.Visibility = Visibility.Hidden;
                        BDeliveryPackage.Visibility = Visibility.Visible;

                        if (parcelWindow != null)//Update the skimmer location and battery if a skimmer window opens through a package
                        {
                            parcelWindow.UpdateChangesFromDroneWindow();
                        }
                        //set the battry color 
                        if (MyDrone.BatteryStatus < 50)
                        {
                            if (MyDrone.BatteryStatus > 10)
                            {
                                PBbatr.Foreground = Brushes.YellowGreen;
                            }
                            else
                            {
                                PBbatr.Foreground = Brushes.Red;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (NonExistentObjectException ex)
            {
                MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (UnableToCollectParcel ex)
            {
                MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// The function handles the delivery of a package to the customer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BDeliveryPackage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int IdOfDeliveryInMyDrone = MyDrone.Delivery.Id;
                int IdOfSenderCustomerInMyDrone = MyDrone.Delivery.Sender.Id;
                int IdOfReceiverCustomerInMyDrone = MyDrone.Delivery.Receiver.Id;

                AccessIbl.DeliveryPackageToTheCustomer(MyDrone.Id);

                MessageBoxResult result = MessageBox.Show("The operation was successful", "info", MessageBoxButton.OK, MessageBoxImage.Information);
                switch (result)
                {
                    case MessageBoxResult.OK:
                        listWindow.StatusDroneSelectorChanged();

                        //to conecct the binding to set the value of my drone to the proprtis
                        MyDrone = AccessIbl.GetDrone(MyDrone.Id);
                        DataContext = MyDrone;

                        // Update the list of packages
                        int indexOfParcelInTheObservable = listWindow.ParcelToLists.IndexOf(listWindow.ParcelToLists.First(x => x.Id == IdOfDeliveryInMyDrone));
                        listWindow.ParcelToLists[indexOfParcelInTheObservable] = AccessIbl.GetParcelList().First(x => x.Id == IdOfDeliveryInMyDrone);

                        // Sender update in customer list
                        int indexOfSenderCustomerInTheObservable = listWindow.CustomerToLists.IndexOf(listWindow.CustomerToLists.First(x => x.Id == IdOfSenderCustomerInMyDrone));
                        listWindow.CustomerToLists[indexOfSenderCustomerInTheObservable] = AccessIbl.GetCustomerList().First(x => x.Id == IdOfSenderCustomerInMyDrone);

                        // Recipient update in the customer list
                        int indexOfReceiverCustomerInTheObservable = listWindow.CustomerToLists.IndexOf(listWindow.CustomerToLists.First(x => x.Id == IdOfReceiverCustomerInMyDrone));
                        listWindow.CustomerToLists[indexOfReceiverCustomerInTheObservable] = AccessIbl.GetCustomerList().First(x => x.Id == IdOfReceiverCustomerInMyDrone);
                        //  disply update affter the oppertion
                        BDeliveryPackage.Visibility = Visibility.Hidden;
                        BAssignPackage.Visibility = Visibility.Visible;
                        BSendToCharge.Visibility = Visibility.Visible;
                        BSendToCharge.IsEnabled = true;
                        GRIDparcelInDelivery.Visibility = Visibility.Hidden;
                        TBnotAssigned.Visibility = Visibility.Visible;

                        if (parcelWindow != null)// Update the skimmer location and battery if a skimmer window opens through a package
                        {
                            parcelWindow.UpdateChangesFromDroneWindow();
                            ClosingWindow = false;
                            Close();
                        }
                        //set the battry color 
                        if (MyDrone.BatteryStatus < 50)
                        {
                            if (MyDrone.BatteryStatus > 10)
                            {
                                PBbatr.Foreground = Brushes.YellowGreen;
                            }
                            else
                            {
                                PBbatr.Foreground = Brushes.Red;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (DeliveryCannotBeMade ex)
            {
                MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        ///  prevent the user from sending empty string
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TBmodel_KeyUp(object sender, KeyEventArgs e)
        {

            if (TBmodel.Text.Length != 0)
            {
                if (!BModalUpdate.IsEnabled)
                    BModalUpdate.IsEnabled = true;
            }
            else
            {
                BModalUpdate.IsEnabled = false;
            }
        }


        /// <summary>
        /// close drone window And updates the views accordingly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BClose1_Click(object sender, RoutedEventArgs e)
        {
            if (Bsimoltor.Visibility == Visibility.Visible)
            {
                listWindow.IsEnabled = true;//allowd to use drone window list again
                ClosingWindow = false;
                Close();
            }
            else if (BstopSimoltor.Visibility == Visibility.Visible)
            {
                BstopSimoltor.Visibility = Visibility.Hidden;
                DroneSimultor.CancelAsync();
                Cursor = Cursors.Wait;
            }
        }
        #endregion drone in operations  






        /// <summary>
        /// ///////////////////////////////////////////////////////Simultor//////////////////////////////////////////////
        /// </summary>

        #region Simultor


        internal BackgroundWorker DroneSimultor; //defining the process(Worker).

        //internal bool isTimeRun;

        /// <summary>
        /// The function creates the process.
        /// </summary>
        private void Simultor()
        {
            DroneSimultor = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
            DroneSimultor.DoWork += DroneSimultor_DoWork; //Operation function.
            DroneSimultor.ProgressChanged += DroneSimultor_ProgressChanged; //changed function.

            DroneSimultor.RunWorkerCompleted += DroneSimultor_RunWorkerCompleted;
        }

        private void DroneSimultor_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            if (BstopSimoltor.Visibility == Visibility.Visible)
            {
                Cursor = Cursors.Arrow;

                BstopSimoltor.Visibility = Visibility.Hidden;
                BstopSimoltor.IsEnabled = true;

                Bsimoltor.Visibility = Visibility.Visible;

                TBmodel.IsEnabled = true;
            }
            else
            {
                listWindow.IsEnabled = true;//allowd to use drone window list again
                ClosingWindow = false;
                Close();
            }
            //The switch checks the drone's status value and opens buttons 
            switch ((DroneStatuses)MyDrone.Statuses)
            {
                case DroneStatuses.free:
                    BSendToCharge.Visibility = Visibility.Visible;
                    BAssignPackage.Visibility = Visibility.Visible;
                    BSendToCharge.IsEnabled = true;
                    break;

                case DroneStatuses.inMaintenance:
                    BReleaseDrone.Visibility = Visibility.Visible;
                    BAssignPackage.Visibility = Visibility.Visible;
                    BAssignPackage.IsEnabled = false;
                    break;

                case DroneStatuses.busy:
                    GRIDparcelInDelivery.Visibility = Visibility.Visible;
                    TBnotAssigned.Visibility = Visibility.Hidden;

                    //check the status to open the right button
                    if (MyDrone.Delivery.OnTheWayToTheDestination)
                    {
                        BDeliveryPackage.Visibility = Visibility.Visible;
                        BSendToCharge.Visibility = Visibility.Visible;
                        BSendToCharge.IsEnabled = false;
                    }
                    else
                    {
                        BPickedUp.Visibility = Visibility.Visible;
                        BSendToCharge.Visibility = Visibility.Visible;
                        BSendToCharge.IsEnabled = false;
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// The function handles in case the user selects the automatic process button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bsimoltor_Click(object sender, RoutedEventArgs e)
        {
            //isTimeRun = true;
            listWindow.IsEnabled = true;

            Simultor(); //call to function who creates the process.

            DroneSimultor.RunWorkerAsync(); //Run the process.

            //Hiding the other buttons in the background.
            BSendToCharge.Visibility = Visibility.Hidden;
            BReleaseDrone.Visibility = Visibility.Hidden;
            BAssignPackage.Visibility = Visibility.Hidden;
            BPickedUp.Visibility = Visibility.Hidden;
            BDeliveryPackage.Visibility = Visibility.Hidden;

            //Hiding the automatic process button and opening a manually process button.
            Bsimoltor.Visibility = Visibility.Hidden;
            BstopSimoltor.Visibility = Visibility.Visible;

            TBmodel.IsEnabled = false; //to prevent modal changing
        }
        //int to help us save the ID.
        private int IdOfDeliveryInMyDrone;
        private int IdOfSenderCustomerInMyDrone;
        private int IdOfReceiverCustomerInMyDrone;

        /// <summary>
        /// The function handles the display when changes made in the process are received.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DroneSimultor_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //to update conect the binding to set the value of my drone to the proprtis.
            MyDrone = AccessIbl.GetDrone(MyDrone.Id);
            DataContext = MyDrone;

            listWindow.StatusDroneSelectorChanged(); //update the List of drones.

            // to find the index when the fanc need to find in the observer collaction and update.
            int indexOfParcelInTheObservable;
            int indexOfSenderCustomerInTheObservable;
            int indexOfReceiverCustomerInTheObservable;

            //switch betwen drone status and according to that update the display.
            switch (MyDrone.Statuses)
            {
                case DroneStatuses.free:
                    if (GRIDparcelInDelivery.Visibility == Visibility.Visible) //the drone is free cuse he just done (we know that becuse the grid is opend) it is affter deliverd.
                    {
                        //update the parcels list
                        indexOfParcelInTheObservable = listWindow.ParcelToLists.IndexOf(listWindow.ParcelToLists.First(x => x.Id == IdOfDeliveryInMyDrone));
                        listWindow.ParcelToLists[indexOfParcelInTheObservable] = AccessIbl.GetParcelList().First(x => x.Id == IdOfDeliveryInMyDrone);

                        //update spasice customer in the Customer list (sender)
                        indexOfSenderCustomerInTheObservable = listWindow.CustomerToLists.IndexOf(listWindow.CustomerToLists.First(x => x.Id == IdOfSenderCustomerInMyDrone));
                        listWindow.CustomerToLists[indexOfSenderCustomerInTheObservable] = AccessIbl.GetCustomerList().First(x => x.Id == IdOfSenderCustomerInMyDrone);

                        //update the reciver
                        indexOfReceiverCustomerInTheObservable = listWindow.CustomerToLists.IndexOf(listWindow.CustomerToLists.First(x => x.Id == IdOfReceiverCustomerInMyDrone));
                        listWindow.CustomerToLists[indexOfReceiverCustomerInTheObservable] = AccessIbl.GetCustomerList().First(x => x.Id == IdOfReceiverCustomerInMyDrone);

                        //display changes for thois stage
                        GRIDparcelInDelivery.Visibility = Visibility.Hidden;
                        TBnotAssigned.Visibility = Visibility.Visible;
                    }
                    else //the drone is in a free state that has come out of charge and not like before (not affter deliver).
                    {
                        listWindow.BaseStationToLists.Clear();
                        List<BO.BaseStationsToList> baseStations1 = AccessIbl.GetBaseStationList().ToList();
                        foreach (var item in baseStations1)
                        {
                            listWindow.BaseStationToLists.Add(item);
                        }
                    }

                    break;

                case DroneStatuses.inMaintenance:
                    listWindow.BaseStationToLists.Clear();
                    List<BO.BaseStationsToList> baseStations = AccessIbl.GetBaseStationList().ToList();
                    foreach (var item in baseStations)
                    {
                        listWindow.BaseStationToLists.Add(item);
                    }
                    break;

                case DroneStatuses.busy:
                    IdOfDeliveryInMyDrone = MyDrone.Delivery.Id;
                    IdOfSenderCustomerInMyDrone = MyDrone.Delivery.Sender.Id;
                    IdOfReceiverCustomerInMyDrone = MyDrone.Delivery.Receiver.Id;

                    if (AccessIbl.GetParcel(MyDrone.Delivery.Id).PickedUp == null)
                    {
                        IdOfDeliveryInMyDrone = MyDrone.Delivery.Id;
                        IdOfSenderCustomerInMyDrone = MyDrone.Delivery.Sender.Id;
                        IdOfReceiverCustomerInMyDrone = MyDrone.Delivery.Receiver.Id;

                        //update list of parcels
                        indexOfParcelInTheObservable = listWindow.ParcelToLists.IndexOf(listWindow.ParcelToLists.First(x => x.Id == MyDrone.Delivery.Id));
                        listWindow.ParcelToLists[indexOfParcelInTheObservable] = AccessIbl.GetParcelList().First(x => x.Id == MyDrone.Delivery.Id);

                        GRIDparcelInDelivery.Visibility = Visibility.Visible;
                        TBnotAssigned.Visibility = Visibility.Hidden;
                    }
                    else if (AccessIbl.GetParcel(MyDrone.Delivery.Id).Delivered == null)
                    {
                        //update the parcels list
                        indexOfParcelInTheObservable = listWindow.ParcelToLists.IndexOf(listWindow.ParcelToLists.First(x => x.Id == MyDrone.Delivery.Id));
                        listWindow.ParcelToLists[indexOfParcelInTheObservable] = AccessIbl.GetParcelList().First(x => x.Id == MyDrone.Delivery.Id);
                        //update spasice customer in the Customer list (sender)
                        indexOfSenderCustomerInTheObservable = listWindow.CustomerToLists.IndexOf(listWindow.CustomerToLists.First(x => x.Id == MyDrone.Delivery.Sender.Id));
                        listWindow.CustomerToLists[indexOfSenderCustomerInTheObservable] = AccessIbl.GetCustomerList().First(x => x.Id == MyDrone.Delivery.Sender.Id);
                        //update the reciver
                        indexOfReceiverCustomerInTheObservable = listWindow.CustomerToLists.IndexOf(listWindow.CustomerToLists.First(x => x.Id == MyDrone.Delivery.Receiver.Id));
                        listWindow.CustomerToLists[indexOfReceiverCustomerInTheObservable] = AccessIbl.GetCustomerList().First(x => x.Id == MyDrone.Delivery.Receiver.Id);
                    }
                    break;

                default:
                    break;
            }

            //battery colors.
            if (MyDrone.BatteryStatus < 50)
            {
                if (MyDrone.BatteryStatus > 10)
                {
                    PBbatr.Foreground = Brushes.YellowGreen;
                }
                else
                {
                    PBbatr.Foreground = Brushes.Red;
                }
            }
            else //MyDrone.BatteryStatus > 50
            {
                PBbatr.Foreground = Brushes.LimeGreen;

            }
        }

        private void DroneSimultor_DoWork(object sender, DoWorkEventArgs e)
        {
            AccessIbl.sim(MyDrone.Id, ReportProgressInSimultor, IsTimeRun);
        }

        private void BstopSimoltor_Click(object sender, RoutedEventArgs e)
        {
            DroneSimultor.CancelAsync();
            BstopSimoltor.IsEnabled = false;
            Cursor = Cursors.Wait;
        }

        public void ReportProgressInSimultor()
        {
            DroneSimultor.ReportProgress(0);
        }

        public bool IsTimeRun()
        {
            return DroneSimultor.CancellationPending;
        }
        #endregion

    }
}
