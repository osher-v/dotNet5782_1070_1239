﻿using System;
using System.Collections.Generic;
using IBL;
using IBL.BO;

namespace ConsoleUI_BL
{

    #region enums
    /// <summary>enum for the first dialog </summary>
    enum Options { Insert = 1, Update, DisplaySingle, DisplayList, EXIT }
    /// <summary> enum for InsertrOption</summary>
    enum InsertrOption { BaseStation = 1, Drone, Customer, Parcel }
    /// <summary> enum for UpdatesOption</summary>
    enum UpdatesOption { DroneUpdate=1, BaseStaitonUpdate, CustomerUpdate, InCharging, Outcharging, AssignDrone , PickUp, Deliverd }
    /// <summary>enum for DisplaySingleOption </summary>
    enum DisplaySingleOption { BaseStationView = 1, DroneDisplay, CustomerView, PackageView }
    /// <summary>enum for DisplayListOption </summary>
    enum DisplayListOption
    {
        ListOfBaseStationView = 1, ListOfDronedisplay, ListOfCustomerView,
        ListOfPackageView, ListOfFreePackageView, ListOfBaseStationsWithFreeChargSlots
    }
    /// <summary>enum for chackDistance</summary>
    enum chackDistance { BasePoint = 1, CustomerPoint }
    #endregion enums

    ///<summary> main class </summary> 
    class Program
    {
        #region fanction of main

        #region Handling insert options
        /// <summary>
        /// The function handles various addition options.
        /// </summary>
        /// <param name="dal">DalObject object that is passed as a parameter to enable the functions in the DalObject class</param>
        static public void InsertOptions(IBL.IBL bl)
        {
            Console.WriteLine(@"
Insert options:

1. Add a base station to the list of stations.
2. Add a drone to the list of existing drones.
3. Admission of a new customer to the customer list.
4. Admission of package for shipment.

Your choice:");
            int.TryParse(Console.ReadLine(), out int choice);

            switch ((InsertrOption)choice)
            {
                case InsertrOption.BaseStation:
                    int newBaseStationID, newchargsSlots;
                    string newName;
                    double newlongitude, newlatitude;

                    Console.WriteLine(@"
You have selected to add a new station.
Please enter an ID number for the new station:");
                    while (!int.TryParse(Console.ReadLine(), out newBaseStationID)) ;
                    Console.WriteLine("Next Please enter the name of the station:");
                    newName = Console.ReadLine();
                    Console.WriteLine("Next Please enter the number of charging slots at the station:");
                    while (!int.TryParse(Console.ReadLine(), out newchargsSlots)) ;
                    Console.WriteLine("Next Please enter the longitude of the station:");
                    while (!double.TryParse(Console.ReadLine(), out newlongitude)) ;
                    Console.WriteLine("Next Please enter the latitude of the station:");
                    while (!double.TryParse(Console.ReadLine(), out newlatitude)) ;

                    Location location = new Location
                    {
                        longitude = newlongitude,
                        latitude = newlatitude
                    };
                    BaseStation newbaseStation = new BaseStation
                    {
                        Id = newBaseStationID,
                        Name = newName,
                        FreeChargeSlots = newchargsSlots,
                        BaseStationLocation = location,
                        DroneInChargsList = new List<DroneInCharg>()     
                    };
                    bl.AddStation(newbaseStation);
                    break;

                case InsertrOption.Drone:
                    int newDroneID, newMaxWeight, firstChargingStation;
                    string newmodel;
                    double newBatteryLevel;

                    Console.WriteLine(@"
You have selected to add a new Drone.
Please enter an ID number for the new drone:");
                    while (!int.TryParse(Console.ReadLine(), out newDroneID)) ;
                    Console.WriteLine("Next Please enter the model of the Drone:");
                    newmodel = Console.ReadLine();
                    Console.WriteLine("Next enter the weight category of the new Drone: 0 for light, 1 for medium and 2 for heavy");
                    while (!int.TryParse(Console.ReadLine(), out newMaxWeight)) ;
                    Console.WriteLine("Next enter the charge level of the battery:");
                    while (!double.TryParse(Console.ReadLine(), out newBatteryLevel)) ;
                    Console.WriteLine("Next enter the ID of the Station to Put the drone for first charge : 0 for free, 1 for inMaintenance and 2 for busy");
                    while (!int.TryParse(Console.ReadLine(), out firstChargingStation)) ;

                    DroneToList newdrone = new DroneToList
                    {
                        Id = newDroneID,
                        Model = newmodel,
                        MaxWeight = (WeightCategories)newMaxWeight
                    };

                    bl.AddDrone(newdrone,firstChargingStation);
                    break;

                case InsertrOption.Customer:
                    int newCustomerID;
                    string newCustomerName, newPhoneNumber;
                    double newCustomerLongitude, newCustomerLatitude;

                    Console.WriteLine(@"
You have selected to add a new Customer.
Please enter an ID number for the new Customer:");
                    while (!int.TryParse(Console.ReadLine(), out newCustomerID)) ;
                    Console.WriteLine("Next Please enter the name of the customer:");
                    newCustomerName = Console.ReadLine();
                    Console.WriteLine("Next enter the phone number of the new customer:");
                    newPhoneNumber = Console.ReadLine();
                    Console.WriteLine("Next Please enter the longitude of the station:");
                    while (!double.TryParse(Console.ReadLine(), out newCustomerLongitude)) ;
                    Console.WriteLine("Next Please enter the latitude of the station:");
                    while (!double.TryParse(Console.ReadLine(), out newCustomerLatitude)) ;
                    Location customerlocation = new Location
                    {
                        longitude = newCustomerLongitude,
                        latitude = newCustomerLatitude
                    };
                    Customer newCustomer = new Customer
                    {
                        Id = newCustomerID,
                        Name = newCustomerName,
                        PhoneNumber = newPhoneNumber,
                        LocationOfCustomer=customerlocation
                    };
                    bl.AddCustomer(newCustomer);
                    break;

                case InsertrOption.Parcel:
                    int newSenderId, newTargetId, newWeight, newPriorities;
                    string senderName,reciverName;

                    Console.WriteLine(@"
You have selected to add a new Parcel.
Next Please enter the sender ID number:");
                    while (!int.TryParse(Console.ReadLine(), out newSenderId)) ;
                    //Console.WriteLine("Next Please enter the name of the customer:");
                    //senderName = Console.ReadLine();
                    Console.WriteLine("Next Please enter the target ID number:");
                    while (!int.TryParse(Console.ReadLine(), out newTargetId)) ;
                    //Console.WriteLine("Next Please enter the name of the customer:");
                    //reciverName = Console.ReadLine();
                    Console.WriteLine("Next enter the weight category of the new Parcel: 0 for free, 1 for inMaintenance and 2 for busy");
                    while (!int.TryParse(Console.ReadLine(), out newWeight)) ;
                    Console.WriteLine("Next enter the priorities of the new Parcel: 0 for regular, 1 for fast and 2 for urgent");
                    while (!int.TryParse(Console.ReadLine(), out newPriorities)) ;


                    CustomerInDelivery newSender = new CustomerInDelivery
                    {
                        Id = newSenderId,
                        //Name = senderName,
                    };
                    CustomerInDelivery newReciver = new CustomerInDelivery
                    {
                        Id = newTargetId,
                        //Name = reciverName,
                    };

                    Parcel newParcel = new Parcel
                    {
                        Sender = newSender,
                        Receiver= newReciver,
                        Weight = (WeightCategories)newWeight,
                        Prior = (Priorities)newPriorities
                    };
                    //int counterParcelSerialNumber = 
                        bl.AddParcel(newParcel);
                    break;

                default:
                    break;
            }
        }
        #endregion Handling insert options

        #region Handling update options
        /// <summary>
        /// The function handles various update options.
        /// </summary>
        /// <param name="dal">DalObject object that is passed as a parameter to enable the functions in the DalObject class</param>
        static public void UpdateOptions(IBL.IBL bl)
        {
            Console.WriteLine(@"
Update options:
1. drone        (new modal name)
2. base station (new name or new chrage slots number )
3. costomer     (new name or new phone number)
4. Assigning a package to a drone
5. Collection of a package by drone
6. Delivery package to customer
7. Sending a drone for charging at a base station
8. Release drone from charging at base station
Your choice:");
            int.TryParse(Console.ReadLine(), out int choice);

            int  droneId, baseStationId, chargeslots, customerId;
            string phoneNumber, droneName, baseName, customerName;
            DateTime time;
            switch ((UpdatesOption)choice)
            {
                case UpdatesOption.DroneUpdate:
                    Console.WriteLine("please enter drone ID for update:");
                    int.TryParse(Console.ReadLine(), out droneId);
                    Console.WriteLine("Next Please enter the new Modal name:");
                    droneName = Console.ReadLine();
                    bl.UpdateDroneName(droneId, droneName);
                    break;

                case UpdatesOption.BaseStaitonUpdate:
                    Console.WriteLine("please enter base station ID for update:");
                    int.TryParse(Console.ReadLine(), out baseStationId);
                    Console.WriteLine("Next Please enter the new base station name if not send empty line:");
                    baseName = Console.ReadLine();//אם נשלח ריק השדה לא מתעדכן
                    Console.WriteLine("please enter update for the Charge slots number:");
                    int.TryParse(Console.ReadLine(), out chargeslots);//אם נשלח ריק השדה לא מתעדכן
                    bl.UpdateBaseStaison(baseStationId, baseName, chargeslots);
                    break;

                case UpdatesOption.CustomerUpdate:
                    Console.WriteLine("please enter Customer ID for update:");
                    int.TryParse(Console.ReadLine(), out customerId);
                    Console.WriteLine("Next Please enter the new base station name if not send empty line:");
                    customerName = Console.ReadLine();//אם נשלח ריק השדה לא מתעדכן
                    Console.WriteLine("please enter update for the Charge slots number:");
                    phoneNumber = Console.ReadLine();//אם נשלח ריק השדה לא מתעדכן
                    bl.UpdateCustomer(customerId, customerName, phoneNumber);
                    break;

                case UpdatesOption.InCharging:
                    Console.WriteLine("please enter Drone ID:");
                    int.TryParse(Console.ReadLine(), out droneId);   
                    bl.SendingDroneforCharging(droneId);
                    break;

                case UpdatesOption.Outcharging:
                    Console.WriteLine("please enter Drone ID:");
                    int.TryParse(Console.ReadLine(), out droneId);
                    Console.WriteLine("Please enter the length of time the drone has been charging:");
                    DateTime.TryParse(Console.ReadLine(), out time);
                    bl.ReleaseDroneFromCharging(droneId, time);
                    break;

                case UpdatesOption.AssignDrone:          
                    Console.WriteLine("please enter Drone ID:");
                    int.TryParse(Console.ReadLine(), out droneId);
                    bl.AssignPackageToDdrone( droneId);
                    break;

                case UpdatesOption.PickUp:
                    Console.WriteLine("please enter Drone ID:");
                    int.TryParse(Console.ReadLine(), out droneId);
                    bl.PickedUpPackageByTheDrone(droneId);
                    break;

                case UpdatesOption.Deliverd:
                    Console.WriteLine("please enter Drone ID:");
                    int.TryParse(Console.ReadLine(), out droneId);
                    bl.DeliveryPackageToTheCustomer(droneId);
                    break;

                default:
                    break;
            }
        }
        #endregion Handling update options

        #region Handling display options
        /// <summary>
        /// The function handles display options.
        /// </summary>
        /// <param name="dal">DalObject object that is passed as a parameter to enable the functions in the DalObject class</param>
        static public void DisplaySingleOptions(IBL.IBL bl)
        {
            Console.WriteLine(@"
Display options(single):

1. Base station view.
2. Drone display.
3. Customer view.
4. Package view.

Your choice:");
            int.TryParse(Console.ReadLine(), out int choice);

            int idForDisplayObject;

            switch ((DisplaySingleOption)choice)
            {
                case DisplaySingleOption.BaseStationView:
                    Console.WriteLine("Insert ID number of base station:");
                    int.TryParse(Console.ReadLine(), out idForDisplayObject);

                    Console.WriteLine(bl.GetBaseStation(idForDisplayObject).ToString());
                    break;

                case DisplaySingleOption.DroneDisplay:
                    Console.WriteLine("Insert ID number of requsted drone:");
                    int.TryParse(Console.ReadLine(), out idForDisplayObject);

                    Console.WriteLine(bl.GetDrone(idForDisplayObject).ToString());
                    break;

                case DisplaySingleOption.CustomerView:
                    Console.WriteLine("Insert ID number of requsted Customer:");
                    int.TryParse(Console.ReadLine(), out idForDisplayObject);

                    Console.WriteLine(bl.GetCustomer(idForDisplayObject).ToString());
                    break;

                case DisplaySingleOption.PackageView:
                    Console.WriteLine("Insert ID number of reqused parcel:");
                    int.TryParse(Console.ReadLine(), out idForDisplayObject);

                    Console.WriteLine(bl.GetParcel(idForDisplayObject).ToString());
                    break;

                default:
                    break;
            }
        }
        #endregion Handling display options

        #region Handling the list display options
        /// <summary>
        /// The function prints the data array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listToPrint"></param>
        public static void printTheList<T>(List<T> listToPrint) 
        {
            foreach (T item in listToPrint)
            {
                Console.WriteLine(item);
            }
        }
        /// <summary>
        /// The function handles list view options.
        /// </summary>
        /// <param name="bl">DalObject object that is passed as a parameter to enable the functions in the DalObject class</param>
        static public void DisplayListOptions(IBL.IBL bl)
        {
            Console.WriteLine(@"
Display options (for the whole list):

1. Displays a list of base stations.
2. Displays the list of drone.
3. View the customer list.
4. Displays the list of packages.
5. Displays a list of packages that have not yet been assigned to the drone.
6. Display base stations with available charging stations.

Your choice:");
            int.TryParse(Console.ReadLine(), out int choice);

            switch ((DisplayListOption)choice)
            {
                case DisplayListOption.ListOfBaseStationView:
                    printTheList(bl.GetBaseStationList().ToList());
                    break;

                case DisplayListOption.ListOfDronedisplay:
                    printTheList(bl.GetDroneList().ToList());
                    break;

                case DisplayListOption.ListOfCustomerView:
                    printTheList(bl.GetCustomerList().ToList());
                    break;

                case DisplayListOption.ListOfPackageView:
                    printTheList(bl.GetParcelList().ToList());
                    break;

                case DisplayListOption.ListOfFreePackageView:
                    printTheList(bl.GetParcelWithoutDrone().ToList());
                    break;

                case DisplayListOption.ListOfBaseStationsWithFreeChargSlots:
                    printTheList(bl.GetBaseStationsWithFreeChargSlots().ToList());
                    break;

                default:
                    break;
            }

        }
        #endregion Handling the list display options

        #endregion fanction of main

        static void Main(string[] args)
        {
            IBL.IBL BLObject = new IBL.BL();
            //IDal.IDal idal = dalObject;

            int choice;
            do
            {
                Console.WriteLine(@"
choose from the following options (type the selected number): 

1. Insert options.
2. Update options.
3. Display options(singel).
4. Display options (for the whole list).
5. Calculate distance between points.
6. EXIT.
Your choice:");
                int.TryParse(Console.ReadLine(), out choice);

                switch ((Options)choice)
                {
                    case Options.Insert:
                        InsertOptions(BLObject);
                        break;

                    case Options.Update:
                        UpdateOptions(BLObject);
                        break;

                    case Options.DisplaySingle:
                        DisplaySingleOptions(BLObject);
                        break;

                    case Options.DisplayList:
                        DisplayListOptions(BLObject);
                        break;
                 
                    case Options.EXIT:
                        Console.WriteLine("Have a good day");
                        break;

                    default:
                        break;
                }
            } while (!(choice == 5));
        }
    }


}
