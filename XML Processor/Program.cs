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

        // Change before distribution
        static string baseString = @"D:\Anne\Documents\Uni\Sound and Music Technology\Data\jSymbolic Data Output\";

        static string[] functionOptions = new string[] { "-extract_jsym", "-print_all_txt", "-print_inf_txt", "-print_val_txt", "-change_basedir", "-print_all_csv", "-extract_div" };

        static List<string> newLines = new List<string>();
        static string[] lines;

        static List<List<string>>[] featureValues;
        static int[] featureLength;
        static List<string> songNames = new List<string>();
        static List<Feature> featInfo = new List<Feature>();
        static int noSongs, noFeatures;

        static IDictionary<string, int>[] featureDicts;

        static int listIndex = 0;
        static int fileIndex = 1;

        static void Main(string[] args)
        {
            Init();

            while (true)
            {
                //Console.Write("<XMLProcessor>");
                Console.Write(baseString + ">");
                string opResult = "";
                string[] inputParameters = Console.ReadLine().Split("-p ");
                string param = "";
                if (inputParameters.Length > 1)
                    param = inputParameters[1].Trim();

                switch (inputParameters[0].Trim())
                {
                    case "-h":
                        PrintHelp();
                        break;
                    case "-extract_jsym":
                        opResult += ExtractJSymbolic(param);
                        break;
                    case "-extract_div":
                        opResult += ExtractDiversity(param, "feature_diversity");
                        break;
                    case "-print_all_txt":
                        opResult += ParseAllToTXT(param, "feature_info_values");
                        break;
                    case "-print_inf_txt":
                        opResult += ParseInfoToTXT(param, "feature_info");
                        break;
                    case "-print_val_txt":
                        opResult += ParseValuesToTXT(param, "feature_values");
                        break;
                    case "-print_all_csv":
                        opResult += ParseAllToCSV(param, "feature_info_values");
                        break;
                    case "-change_basedir":
                        baseString = @"" + param;
                        opResult += "Successful - Directory changed";
                        break;
                    default:
                        Console.WriteLine("Function Option not recognized. Input -h for help.");
                        break;
                }

                if (opResult != "") Console.WriteLine("====== " + opResult + " ======");

            }

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
            Console.WriteLine(functionOptions[6] + "\t[-p feature_diversity]" + " \t\t\t\t\t  :   Extract and Print Feature Diversity");
            Console.WriteLine(functionOptions[1] + "\t[-p feature_info_values]" + " \t\t\t\t  :   Print Feature info and values to TXT");
            Console.WriteLine(functionOptions[2] + "\t[-p feature_info]" + " \t\t\t\t\t  :   Print Feature info to TXT");
            Console.WriteLine(functionOptions[3] + "\t[-p feature_values]" + " \t\t\t\t\t  :   Print Feature values to TXT");
            Console.WriteLine(functionOptions[5] + "\t[-p feature_info_values]" + " \t\t\t\t  :   Print Feature info and values to CSV");
            Console.WriteLine(functionOptions[4] + "\t[-p C:/]" + " \t\t\t\t\t\t  :   View and change base directory");
            Console.WriteLine("");

            Console.WriteLine("Leave parameter part ([-p ...]) away for default values as show above");
            Console.WriteLine("NB: filenames or directories with spaces must be enclosed in \"\".");
            Console.WriteLine("#####################################################################################################################");
            Console.WriteLine("");
        }

        static string ExtractJSymbolic(string parameter)
        {
            string paramOne = "", paramTwo = "";
            if (parameter != "")
            {
                string[] parameters = parameter.Split(' ');
                if (parameters.Length < 2)
                {
                    return "Failed - Parameters invalid";
                }
                paramOne = parameters[0];
                paramTwo = parameters[1];
            }
            string resultOne = RetrieveFeaturesJSymbolic(@"" + paramOne, @"feature_definitions.xml");
            string resultTwo = RetrieveValuesJSymbolic(@"" + paramTwo, @"extracted_feature_values.xml");


            return resultOne + " ======\n====== " + resultTwo;
        }

        static string RetrieveFeaturesJSymbolic(string paramName, string defaultName)
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
            noFeatures = featInfo.Count;
            featureValues = new List<List<string>>[noFeatures];
            featureLength = new int[noFeatures];

            for (int x = 0; x < noFeatures; x++)
            {
                featureValues[x] = new List<List<string>>();
                featureLength[x] = -1;
            }

            return "Successful - Feature Information Retrieved";
        }

        static string RetrieveValuesJSymbolic(string paramName, string defaultName)
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
                    featureValues[featureIndex].Add(new List<string>());

                    lines[i] = lines[i].Split('>')[1].Split('<')[0];
                }
                else if (lines[i].Contains("<v>"))
                {
                    // value of feature
                    lines[i] = lines[i].Split('>')[1].Split('<')[0];
                    featureValues[featureIndex][featureLength[featureIndex]].Add(lines[i]);
                }
                else
                {
                    continue;
                }
            }

            noSongs = songNames.Count;
            return "Successful - Feature Values Retrieved";
        }

        static string ParseAllToTXT(string paramName, string defaultName)
        {
            string fileName = (paramName == "" ? defaultName : paramName);

            // Parse To Printable
            for (int x = 0; x < noFeatures; x++)
            {
                newLines.Add(featInfo[x].name);
                newLines.Add(featInfo[x].description);
                newLines.Add(featInfo[x].dimensions.ToString());

                for (int y = 0; y < featureValues[x].Count; y++)
                {
                    newLines.Add(songNames[y]);
                    for (int z = 0; z < featureValues[x][y].Count; z++)
                    {
                        newLines.Add(featureValues[x][y][z]);
                    }
                }
            }

            WriteToFile(fileName, "txt");
            return "Successful - Results written to '" + fileName + ".txt'";
        }

        static string ParseInfoToTXT(string paramName, string defaultName)
        {
            string fileName = (paramName == "" ? defaultName : paramName);

            // Parse To Printable
            for (int x = 0; x < noFeatures; x++)
            {
                newLines.Add(featInfo[x].name);
                newLines.Add(featInfo[x].description);
                newLines.Add(featInfo[x].dimensions.ToString());
            }

            WriteToFile(fileName, "txt");
            return "Successful - Results written to '" + fileName + ".txt'";
        }

        static string ParseValuesToTXT(string paramName, string defaultName)
        {
            string fileName = (paramName == "" ? defaultName : paramName);

            // Parse To Printable
            for (int x = 0; x < noFeatures; x++)
            {

                for (int y = 0; y < featureValues[x].Count; y++)
                {
                    newLines.Add(songNames[y]);
                    for (int z = 0; z < featureValues[x][y].Count; z++)
                    {
                        newLines.Add(featureValues[x][y][z]);
                    }
                }
            }

            WriteToFile(fileName, "txt");
            return "Successful - Results written to '" + fileName + ".txt'";
        }

        static string ParseAllToCSV(string paramName, string defaultName)
        {
            string fileName = (paramName == "" ? defaultName : paramName);

            int songID = 1;
            string nextLine = "\"Song ID\",\"Song Name\"";

            //Header of Table
            for (int i = 0; i < featInfo.Count; i++)
            {
                nextLine += ",\"" + featInfo[i].name + "\"";
            }

            newLines.Add(nextLine);
            
            for (int y = 0; y < noSongs; y++)
            {
                nextLine = "\"" + songID++.ToString() + "\",\"" + songNames[y] + "\"";
                for (int x = 0; x < noFeatures; x++)
                {
                    if (featureValues[x][y].Count == 1)
                    {
                        nextLine += ",\"\"\"" + featureValues[x][y][0] + "\"\"\"";
                    }
                    else
                    {
                        nextLine += ",\"" + featureValues[x][y][0];
                        for (int z = 1; z < featureValues[x][y].Count; z++)
                        {
                            nextLine += "," + featureValues[x][y][z];
                        }
                        nextLine += "\"";

                    }

                }

                newLines.Add(nextLine);
            }

            WriteToFile(fileName, "csv");
            return "Successful - Results written to '" + fileName + ".csv'";
        }

        static string ExtractDiversity(string paramName, string defaultName)
        {
            string resultOne = RetrieveDiversity();
            string resultTwo = ParseDiversityToTXT(paramName == "" ? defaultName : paramName);

            return resultOne + " ======\n====== " + resultTwo;
        }

        static string RetrieveDiversity()
        {
            featureDicts = new Dictionary<string, int>[noFeatures];

            for (int x = 0; x < noFeatures; x++)
            {
                featureDicts[x] = new Dictionary<string, int>();
                for (int y = 0; y < featureValues[x].Count; y++)
                {
                    // Prepare Feature Value(s)
                    string featureValue = featureValues[x][y][0];
                    for (int z = 1; z < featureValues[x][y].Count; z++)
                    {
                        featureValue += ", " + featureValues[x][y][z];
                    }

                    AddToDictionnary(featureDicts[x], featureValue);

                }
            }

            return "Successful - Diversity Retrieved";
        }

        static string ParseDiversityToTXT(string fileName)
        {

            // Parse To Printable
            for (int x = 0; x < noFeatures; x++)
            {
                newLines.Add(featInfo[x].name);
                newLines.Add("No. Different Values: " + featureDicts[x].Count.ToString());

                foreach (KeyValuePair<string, int> item in featureDicts[x])
                {
                    newLines.Add(item.Value + "x " + item.Key);
                }
            }

            WriteToFile(fileName, "txt");
            return "Successful - Results written to '" + fileName + ".txt'";
        }

        static void AddToDictionnary(IDictionary<string, int> dict, string key)
        {
            int result;
            if (dict.TryGetValue(key, out result))
            {
                dict[key] = result + 1;
            }
            else
            {
                dict.Add(key, 1);
            }
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

