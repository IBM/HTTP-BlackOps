/**
Copyright 2019 Trend Micro, Incorporated, All Rights Reserved.
SPDX-License-Identifier: Apache-2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Testing;


namespace TrafficCollector
{
    class Program
    {
        private static bool _running = true;

        static void Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("Usage: TrafficCollector <IP> <PORT> <DUMP_DIR>");
                Console.WriteLine("<IP> - The IP of the network interface that the collector API is running on.");
                Console.WriteLine("<PORT> - The PORT of the collector API server");
                Console.WriteLine("<DUMP_DIR> - The directory where the collector will dump files.");
                Console.WriteLine("<TESTS_FILE> - The location of a file containing security tests.");
                Console.WriteLine("Example: TrafficCollector 10.10.9.9 8080 C:\\Share\\TrafficDump");


                Console.WriteLine("Exit codes: 1 - No args, 2 - Invalid arguments, 3 - Cannot start server");
                Environment.ExitCode = 1;
                return;
            }

            Console.CancelKeyPress += Console_CancelKeyPress;
            string ip = args[0];
            string portString = args[1];
            int port;

            if (!int.TryParse(portString, out port))
            {
                Console.WriteLine("Invalid port.");
                Environment.ExitCode = 2;
                return;
            }

            if (!Directory.Exists(args[2]))
            {
                Console.WriteLine("Dump directory doesn't exist.");
                Environment.ExitCode = 2;
                return;
            }

            if (!File.Exists(args[3]))
            {
                Console.WriteLine("Tests file doesn't exist.");
                Environment.ExitCode = 2;
                return;
            }

            TrafficCollectorSettings.Instance.Ip = ip;
            TrafficCollectorSettings.Instance.DumpDir = args[2];
            TrafficCollectorSettings.Instance.TestFile = args[3];

            Console.WriteLine("Starting collector API at http://{0}:{1}", ip, port);
            try
            {
                CollectorAPIServer server = new CollectorAPIServer(ip, port);
                server.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            do
            {
                Thread.Sleep(1000);
            }
            while (_running);

            Console.WriteLine("Stopping....");

        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            _running = false;
        }
    }
}
