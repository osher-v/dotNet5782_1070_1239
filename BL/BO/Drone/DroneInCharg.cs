﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    namespace BO
    {
        public class DroneInCharg
        {
            public int Id { get; set; }
            public double BatteryStatus { get; set; }

            public override string ToString()
            {
                return string.Format("id is:{0,-8}\t Battery Status is:{1,5}\t",Id,BatteryStatus);
            }

        }
    }
}