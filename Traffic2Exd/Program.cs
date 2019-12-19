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

namespace Traffic2Exd
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: Traffic2Exd <traffic file path> <EXD file path>");
                Console.WriteLine("Supported import formats: .har, .txt");
                Console.WriteLine("If the EXD file already exists the tool will append to it.");

                Console.WriteLine("Exit codes: 1 - No args, 2 - Incorrect file path, 3 - Parsing error, 4 - Export error, 5 - Unsupported Exception.");
                Environment.ExitCode = 1;
            }
            else
            {
                string trafficFilePath = args[0];
                string exdFilePath = args[1];
                if (!File.Exists(trafficFilePath))
                {
                    Console.WriteLine("Could not find har file: '{0}'", trafficFilePath);
                    Environment.ExitCode = 2;
                }
                else
                {
                    TrafficViewerFile tvf = new TrafficViewerFile();
                    try
                    {

                        if (File.Exists(exdFilePath))
                        {
                            Console.WriteLine("EXD file {0} already exists. Appending to it.", exdFilePath);
                            ConfigurationParser exdParser = new ConfigurationParser();
                            exdParser.Parse(exdFilePath, tvf, ParsingOptions.GetDefaultProfile());
                        }


                        Console.WriteLine("Importing from '{0}'...", trafficFilePath);
                        ITrafficParser parser = null;

 
                        if (trafficFilePath.ToLower().EndsWith(".har"))
                        {
                            parser = new HarParser();
                        }
                        else if (trafficFilePath.ToLower().EndsWith(".txt"))
                        {
                            parser = new DefaultTrafficParser();
                        }
                        else {
                            Console.WriteLine("File extension is unsupported. Supported extensions/formats: .har,.txt");
                            Environment.ExitCode = 5;
                        }

                        if (parser != null)
                        {
                            parser.Parse(trafficFilePath, tvf, ParsingOptions.GetRawProfile());
                        }
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
