using System;
using System.IO;
using System.Collections.Generic;

namespace XML_Processor
{
    class Program
    {
        public struct Feature
        {
            public string name;
            public string description;
            public int dimensions;

            public Feature(string _name, string _description, int _dimensions)
            {
                name = _name;
                description = _description;
                dimensions = _dimensions;
            }
        }

        static List<string> newLines = new List<string>();
        static int listIndex = 0;
        static int fileIndex = 1;
        static string[] lines;
        static string baseString = @"D:\Anne\Documents\Uni\Sound and Music Technology\Data\jSymbolic Data Output\";

        static List<List<string>>[] longFeatures;
        static int[] featureLength;

        static List<string> songNames = new List<string>();
        static List<Feature> featInfo = new List<Feature>();

        static string[] functionOptions = new string[] { "-extract_jsym", "-print_all_xml", "-print_inf_xml", "-print_val_xml", "-change_basedir", "-print_all_csv" };


        static void Main(string[] args)
        {
            Init();

            while (true)
            {
                //Console.Write("<XMLProcessor>");
                Console.Write(baseString + ">");
                string opSuccess = "";
                string[] inputParameters = Console.ReadLine().Split("-p");
                string param = "", paramOne = "", paramTwo = "";
                if (inputParameters.Length > 1)
                    param = inputParameters[1].Trim();

                switch (inputParameters[0].Trim())
                {
                    case "-h":
                        PrintHelp();
                        break;
                    case "-extract_jsym":
                        if(param != "")
                        {
                            string[] parameters = param.Split(' ');
                            paramOne = parameters[0];
                            paramTwo = parameters[1];
                        }
                        opSuccess += RetrieveFeaturesXML(@"" + paramOne, @"feature_definitions.xml");
                        opSuccess += RetrieveValuesXML(@"" + paramTwo, @"extracted_feature_values.xml");
                        break;
                    case "-print_all_xml":
                        opSuccess += ParseAllToXML(param, "feature_info_values");
                        break;
                    case "-print_inf_xml":
                        opSuccess += ParseInfoToXML(param, "feature_info");
                        break;
                    case "-print_val_xml":
                        opSuccess += ParseValuesToXML(param, "feature_values");
                        break;
                    case "-print_all_csv":
                        opSuccess += ParseAllToCSV(param, "feature_info_values");
                        break;
                    case "-change_basedir":
                        baseString = @"" + param;
                        opSuccess += "Directory changed";
                        break;
                    default:
                        Console.WriteLine("Function Option not recognized. Input -h for help.");
                        break;
                }

                if (opSuccess != "") Console.WriteLine("====== Finished - " + opSuccess + " ======");

            }




            //WriteToFile("extracted_feature_values_cleaned", "xml");
        }

        static void Init()
        {
            Console.WriteLine("########################################");
            Console.WriteLine("#   Welkom to The Feature Processor!   #");
            Console.WriteLine("#        Made by: Anne van Ede         #");
            Console.WriteLine("# ------------------------------------ #");
            Console.WriteLine("#  This tool can extract feature info  #");
            Console.WriteLine("#    from XML files (and CSV files)    #");
            Console.WriteLine("########################################");
            Console.WriteLine("");
        }

        static void PrintHelp()
        {
            Console.WriteLine("");
            Console.WriteLine("#####################################################################################################################");
            Console.WriteLine("usage                :   [-function_option] [-p parameter parameter]");
            Console.WriteLine("-h                   :   Help");
            Console.WriteLine("");

            Console.WriteLine("Function options:");
            Console.WriteLine(functionOptions[0] + "\t[-p feature_definitions.xml extracted_feature_values.xml]" + " :   Extract features");
            Console.WriteLine(functionOptions[1] + "\t[-p feature_info_values]" + " \t\t\t\t  :   Print Feature info and values to XML");
            Console.WriteLine(functionOptions[2] + "\t[-p feature_info]" + " \t\t\t\t\t  :   Print Feature info to XML");
            Console.WriteLine(functionOptions[3] + "\t[-p feature_values]" + " \t\t\t\t\t  :   Print Feature values to XML");
            Console.WriteLine(functionOptions[5] + "\t[-p feature_info_values]" + " \t\t\t\t  :   Print Feature info and values to CSV");
            Console.WriteLine(functionOptions[4] + "\t[-p C:/]" + " \t\t\t\t\t\t  :   View and change base directory");
            Console.WriteLine("");

            Console.WriteLine("Leave parameter part ([-p ...]) away for default values as show above");
            Console.WriteLine("NB: filenames or directories with spaces must be enclosed in \"\".");
            Console.WriteLine("#####################################################################################################################");
            Console.WriteLine("");
        }

        static string RetrieveFeaturesXML(string paramName, string defaultName)
        {
            string fileName = (paramName == "" ? defaultName : paramName);

            lines = System.IO.File.ReadAllLines(baseString + fileName);
            Feature temp = new Feature("", "", 0);

            for (int i = 0; i < lines.Length; i++)
            {

                if (lines[i].Contains("<name>"))
                {
                    // name of next feature
                    temp.name = lines[i].Split('>')[1].Split('<')[0];
                }
                else if (lines[i].Contains("<description>"))
                {
                    // description of feature
                    temp.description = lines[i].Split('>')[1].Split('<')[0];
                }
                else if (lines[i].Contains("<parallel_dimensions>"))
                {
                    // number of dimensions of feature
                    temp.dimensions = Int32.Parse(lines[i].Split('>')[1].Split('<')[0]);
                }
                else if (lines[i].Contains("</feature>"))
                {
                    // end of feature
                    featInfo.Add(temp);
                    temp = new Feature("", "", 0);
                }
                else
                {
                    continue;
                }
            }

            longFeatures = new List<List<string>>[featInfo.Count];
            featureLength = new int[longFeatures.Length];

            for (int x = 0; x < longFeatures.Length; x++)
            {
                longFeatures[x] = new List<List<string>>();
                featureLength[x] = -1;
            }

            return "Feature Information Retrieved - ";
        }

        static string RetrieveValuesXML(string paramName, string defaultName)
        {
            string fileName = (paramName == "" ? defaultName : paramName);

            lines = System.IO.File.ReadAllLines(baseString + fileName);
            int featureIndex = -1;

            for (int i = 0; i < lines.Length; i++)
            {

                if (lines[i].Contains("<data_set_id>"))
                {
                    // new song file name
                    lines[i] = lines[i].Substring(lines[i].LastIndexOf('\\') + 2).Split('<')[0];
                    songNames.Add(lines[i]);
                    featureIndex = -1;
                }
                else if (lines[i].Contains("<name>"))
                {
                    // name of next feature
                    featureIndex++;
                    featureLength[featureIndex]++;
                    longFeatures[featureIndex].Add(new List<string>());

                    lines[i] = lines[i].Split('>')[1].Split('<')[0];
                }
                else if (lines[i].Contains("<v>"))
                {
                    // value of feature
                    lines[i] = lines[i].Split('>')[1].Split('<')[0];
                    longFeatures[featureIndex][featureLength[featureIndex]].Add(lines[i]);
                }
                else
                {
                    continue;
                }
            }


            return "Feature Values Retrieved";
        }

        static string ParseAllToXML(string paramName, string defaultName)
        {
            string fileName = (paramName == "" ? defaultName : paramName);

            // Parse To Printable
            for (int x = 0; x < longFeatures.Length; x++)
            {
                newLines.Add(featInfo[x].name);
                newLines.Add(featInfo[x].description);
                newLines.Add(featInfo[x].dimensions.ToString());

                for (int y = 0; y < longFeatures[x].Count; y++)
                {
                    newLines.Add(songNames[y]);
                    for (int z = 0; z < longFeatures[x][y].Count; z++)
                    {
                        newLines.Add(longFeatures[x][y][z]);
                    }
                }
            }

            WriteToFile(fileName, "xml");
            return "Results written to '" + fileName + ".xml'";
        }

        static string ParseInfoToXML(string paramName, string defaultName)
        {
            string fileName = (paramName == "" ? defaultName : paramName);

            // Parse To Printable
            for (int x = 0; x < longFeatures.Length; x++)
            {
                newLines.Add(featInfo[x].name);
                newLines.Add(featInfo[x].description);
                newLines.Add(featInfo[x].dimensions.ToString());
            }

            WriteToFile(fileName, "xml");
            return "Results written to '" + fileName + ".xml'";
        }

        static string ParseValuesToXML(string paramName, string defaultName)
        {
            string fileName = (paramName == "" ? defaultName : paramName);

            // Parse To Printable
            for (int x = 0; x < longFeatures.Length; x++)
            {

                for (int y = 0; y < longFeatures[x].Count; y++)
                {
                    newLines.Add(songNames[y]);
                    for (int z = 0; z < longFeatures[x][y].Count; z++)
                    {
                        newLines.Add(longFeatures[x][y][z]);
                    }
                }
            }

            WriteToFile(fileName, "xml");
            return "Results written to '" + fileName + ".xml'";
        }

        static string ParseAllToCSV(string paramName, string defaultName)
        {
            string fileName = (paramName == "" ? defaultName : paramName);

            int songID = 1;
            string nextLine = "Song ID,Song Name";

            //Header of Table
            for (int i = 0; i < featInfo.Count; i++)
            {
                nextLine += "," + featInfo[i].name;
            }

            newLines.Add(nextLine);

            int noSongs = songNames.Count;

            for (int y = 0; y < noSongs; y++)
            {
                nextLine = songID++.ToString() + "," + songNames[y];
                for (int x = 0; x < longFeatures.Length; x++)
                {
                    if (longFeatures[x][y].Count == 1)
                    {
                        nextLine += "," + longFeatures[x][y][0];
                    }
                    else
                    {
                        nextLine += ",\"" + longFeatures[x][y][0];
                        for (int z = 1; z < longFeatures[x][y].Count; z++)
                        {
                            nextLine += "," + longFeatures[x][y][z];
                        }
                        nextLine += "\"";

                    }

                }

                newLines.Add(nextLine);
            }

            WriteToFile(fileName, "xml");
            return "Results written to '" + fileName + ".xml'";
        }

        static void WriteToFile(string baseName, string extension, string index = "")
        {
            // Write the string array to a new file named "WriteLines.txt".
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(baseString, baseName + index + "." + extension)))
            {
                foreach (string newline in newLines)
                    outputFile.WriteLine(newline);
            }
            newLines.Clear();
        }



    }
}

