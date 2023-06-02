﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class Program
    {
        static void Main(string[] args)
        {
            
            ServiceHost host = new ServiceHost(typeof(FileHandler));
            host.Open();
            Console.WriteLine("Pokrenut server...");
            Console.ReadKey();
            host.Close();
            Console.WriteLine("Service is closed");
        }
    }
}
