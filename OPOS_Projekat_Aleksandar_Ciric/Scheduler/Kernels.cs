﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
{
    public static class Kernels // sharpening filter
    {
        public static double[,] Laplacian
        {
            get
            {
                return new double[,]
                {
                     { 0,-1, 0 },
                     {-1, 4,-1 },
                     { 0,-1, 0 }
                };
            }
        }
    }
}
