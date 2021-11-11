﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    namespace BO
    {
        public abstract class ParcelParent
        {
            public int Id { get; set; }

            public WeightCategories Weight { get; set; }

            public Priorities Prior { get; set; }

            public override string ToString()
            {
                return String.Format("ID is:{0,-8}\t Weight Categorie:{2,-8}\t Prioritie:{3,-8}"  , Id, Weight, Prior);
            }
        }
    }
}