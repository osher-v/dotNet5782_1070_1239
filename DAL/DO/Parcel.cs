﻿
using System;
using IDAL.DO;
using DalObject;

namespace IDAL
{
    namespace DO
    {
        public struct Parcel
        {
            public int Id { get; set; }

            public int SenderId { get; set; }

            public int TargetId { get; set; }

            public WeightCategories Weight { get; set; }
            public Priorities Priority { get; set; }//
            public int DroneId { get; set; }

            public DateTime Requested { get; set; }
            public DateTime Scheduled { get; set; }
            public DateTime PickedUp { get; set; }
            public DateTime Delivered { get; set; }
            public override string ToString()
            {
                return string.Format("ID is: {0}\t sender ID is: {1}\t target ID: {2}\t" +
                    "parcel Weight: {3}\t  Priority: {4}\t  drone id: {5}\t" +
                    "Request started in: {6}\t  Scheduled: {7}\t" +
                    "pick up time at: {8}\t  arivel time at: {9}\t"
                    , Id, SenderId, TargetId, Weight, Priority, DroneId, Requested, Scheduled, PickedUp, Delivered);
            }
        }
    }

}





