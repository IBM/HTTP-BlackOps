/**
Copyright 2019 Trend Micro, Incorporated, All Rights Reserved.
SPDX-License-Identifier: Apache-2.0
 */
using System;
using System.IO;
using TrafficViewerSDK;
using TrafficViewerSDK.Importers;
using TrafficViewerSDK.Exporters;
using TrafficViewerSDK.Options;

namespace Har2Exd
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: Har2Exd <HAR file path> <EXD file path>");
                Console.WriteLine("Exit codes: 1 - No args, 2 - Incorrect har path, 3 - Parsing error, 4 - Export error.");
                Environment.ExitCode = 1;
            }
            else
            {
                string harFilePath = args[0];
                string exdFilePath = args[1];
                if (!File.Exists(harFilePath))
                {
                    Console.WriteLine("Could not find har file: '{0}'", harFilePath);
                    Environment.ExitCode = 2;
                }
                else
                {
                    TrafficViewerFile tvf = new TrafficViewerFile();
                    try
                    {
                        Console.WriteLine("Importing from '{0}'...", harFilePath);
                        ITrafficParser harParser = new HarParser();

                        harParser.Parse(harFilePath, tvf, ParsingOptions.GetDefaultProfile());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Parsing exception: '{0}'", ex.Message);
                        Environment.ExitCode = 3;
                    }
                    //now export

                    try
                    {
                        Console.WriteLine("Exporting to '{0}'...", exdFilePath);
                        var exporter = new ManualExploreExporter();
                        exporter.Export(tvf, new FileStream(exdFilePath, FileMode.Create, FileAccess.ReadWrite));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Export exception: '{0}'", ex.Message);
                        Environment.ExitCode = 4;
                    }
                    tvf.Close(false);
                    Console.WriteLine("Done.");
                }

            }
        }
    }
}
