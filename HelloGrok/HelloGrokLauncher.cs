// ----------------------------------------------------------------------
//  Copyright (C) 2006-2012 Numenta Inc. All rights reserved.
//
//  The information and source code contained herein is the
//  exclusive property of Numenta Inc. No part of this software
//  may be used, reproduced, stored or distributed in any form,
//  without explicit written authorization from Numenta Inc.
// ----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloGrok
{
    public class HelloGrokLauncher
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter 1 to run the initial HelloGrok application,");
            Console.WriteLine("Or, press 2 to run Hello Grok Part 2");
            Console.WriteLine("Or, press 3 to run Hello Anomaly");
            string Input = Console.ReadLine();
            bool Continue = true;

            while (Continue)
            switch (Input)
            {
                case "1":
                    Continue = false;
                    HelloGrok.RunHelloGrok();
                    break;
                case "2":
                    Continue = false;
                    HelloGrokPart2.RunHelloGrokPart2();
                    break;
                case "3":
                    HelloAnomaly.RunHelloAnomaly();
                    Continue = false;
                    break;
                default:
                    Console.WriteLine("Invalid input, please try again.");
                    break;
            }

            Console.WriteLine("Please press any key to close");
            Console.ReadLine();
        }
    }
}
