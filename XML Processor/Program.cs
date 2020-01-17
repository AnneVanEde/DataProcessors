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
                Console.Write("<XMLProcessor>");
                string opSuccess = "";
                string[] inputParameters = Console.ReadLine().Split(' ');

                switch (inputParameters[0])
                {
                    case "-h":
                        PrintHelp();
                        break;
                    case "-extract_jsym":
                        opSuccess += RetrieveFeaturesXML();
                        opSuccess += RetrieveValuesXML();
                        break;
                    case "-print_all_xml":
                        opSuccess += ParseAllToXML();
                        break;
                    case "-print_inf_xml":
                        opSuccess += ParseInfoToXML();
                        break;
                    case "-print_val_xml":
                        opSuccess += ParseValuesToXML();
                        break;
                    case "-print_all_csv":
                        opSuccess += ParseAllToCSV();
                        break;
                    case "-change_basedir":
                        string line = baseString;
                        while (line != "")
                        {
                            Console.WriteLine("The current base directory is:");
                            Console.WriteLine("  " + baseString);
                            Console.WriteLine("If you want to change this directory, give directory. If not press \"Enter\".");
                            Console.Write("<XMLProcessor>");
                            if ((line = Console.ReadLine()) != "")
                                baseString = @"" + line;
                        }
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
            Console.WriteLine("######################################################################");
            Console.WriteLine("usage                :   [-function_option]");
            Console.WriteLine("-h                   :   Help");
            Console.WriteLine("");

            Console.WriteLine("Function options:");
            Console.WriteLine(functionOptions[0] + "        :   Extract features");
            Console.WriteLine(functionOptions[1] + "       :   Print Feature info and values to XML");
            Console.WriteLine(functionOptions[2] + "       :   Print Feature info to XML");
            Console.WriteLine(functionOptions[3] + "       :   Print Feature values to XML");
            Console.WriteLine(functionOptions[5] + "       :   Print Feature info and values to CSV");
            Console.WriteLine(functionOptions[4] + "      :   View and change base directory");
            Console.WriteLine("######################################################################");
            Console.WriteLine("");
        }

        static string RetrieveFeaturesXML()
        {
            lines = System.IO.File.ReadAllLines(baseString + @"feature_definitions.xml");
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

        static string RetrieveValuesXML()
        {
            lines = System.IO.File.ReadAllLines(baseString + @"extracted_feature_values.xml");
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

        static string ParseAllToXML()
        {
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

            WriteToFile("feature_info_values", "xml");
            return "Results written to 'feature_info_values.xml'";
        }

        static string ParseInfoToXML()
        {
            // Parse To Printable
            for (int x = 0; x < longFeatures.Length; x++)
            {
                newLines.Add(featInfo[x].name);
                newLines.Add(featInfo[x].description);
                newLines.Add(featInfo[x].dimensions.ToString());
            }

            WriteToFile("feature_info", "xml");
            return "Results written to 'feature_info.xml'";
        }

        static string ParseValuesToXML()
        {
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

            WriteToFile("feature_values", "xml");
            return "Results written to 'feature_values.xml'";
        }

        static string ParseAllToCSV()
        {
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
                    else { 
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

            WriteToFile("feature_info_values", "csv");
            return "Results written to 'feature_info_values.csv'";
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

