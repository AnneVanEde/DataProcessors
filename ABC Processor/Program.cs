using System;
using System.IO;
using System.Collections.Generic;

namespace ABC_Processor
{
    class Program
    {
        enum Type { folk, euroVision, euroVisionCleaned, post, toAudio };

        // Set Process Type
        static Type processType;

        static List<string> newLines = new List<string>();
        static int fileIndex, songIndex;
        static string[] lines;
        static string baseName;
        static bool removeRests = false;
        static string docPath;
        static string extension;

        static int noEuroVisionSongs;
        static int noFolkSongs;
        static int[] chosenEuroVisionSongs, chosenFolkSongs;

        static void Main(string[] args)
        {
            Console.WriteLine("Welkom to The Processor!");

            Init();

            for (int i = 0; i < lines.Length; i++)
            {
                if (processType == Type.folk || processType == Type.euroVision || processType == Type.euroVisionCleaned)
                {
                    if (!PreProcessing(i)) continue;
                }
                else if (processType == Type.post)
                {
                    if(!PostProcessing(i)) continue;
                }
                else if (processType == Type.toAudio)
                {
                    ToAudio(i);
                }

                newLines.Add(lines[i]);
            }

            WriteToFile();
        }

        static void Init()
        {
            fileIndex = 1;
            songIndex = 0;
            processType = Type.post;

            noEuroVisionSongs = 247;
            noFolkSongs = 83;

            string baseString = @"D:\Anne\Documents\Uni\Sound and Music Technology\Tools\";
            extension = "abc";

            if (processType == Type.euroVision)
            {
                lines = System.IO.File.ReadAllLines(baseString + @"Parser\unprocessed\all.abc");
                baseName = "euroVision";
                docPath = baseString + @"Parser\abc";
            }
            else if (processType == Type.euroVisionCleaned)
            {
                lines = System.IO.File.ReadAllLines(baseString + @"Parser\unprocessed\all.abc");
                baseName = "euroVision";
                docPath = baseString + @"Parser\mid";
                extension = "abc";
            }
            else if (processType == Type.folk)
            {
                lines = System.IO.File.ReadAllLines(baseString + @"Parser\unprocessed\allnlb1000.abc");
                baseName = "allnlb";
                docPath = baseString + @"Parser\abc";
            }
            else if (processType == Type.post)
            {
                lines = System.IO.File.ReadAllLines(baseString + @"Parser\dataset50-50.txt");
                baseName = "50_eurovision-50_folk";
                docPath = baseString + @"Parser\processed";
            }
            else
            {
                lines = System.IO.File.ReadAllLines(baseString + @"Parser\processed\parsed_dataset1.abc");
                baseName = "audio_parsed_dataset";
                docPath = baseString + @"Parser\processed\music";
            }

            chosenEuroVisionSongs = new int[noEuroVisionSongs];
            chosenFolkSongs = new int[noFolkSongs];

            int noChosenEuro = 0, noChosenFolk = 0;
            Random rnd = new Random();

            if (processType == Type.euroVision && false)
            {
                while (noChosenEuro < noEuroVisionSongs)
                {
                StartOne:
                    int song = rnd.Next(1, 247);

                    for (int i = 0; i < noChosenEuro; i++)
                    {
                        if (chosenEuroVisionSongs[i] == song) goto StartOne;
                    }
                    chosenEuroVisionSongs[noChosenEuro++] = song;
                }

                Array.Sort(chosenEuroVisionSongs);
            }
            else if (processType == Type.folk)
            {
                while (noChosenFolk < noFolkSongs)
                {
                StartTwo:
                    int song = rnd.Next(1, 999);

                    for (int i = 0; i < noChosenFolk; i++)
                    {
                        if (chosenFolkSongs[i] == song) goto StartTwo;
                    }
                    chosenFolkSongs[noChosenFolk++] = song;
                }

                Array.Sort(chosenFolkSongs);
            }




        }

        static bool PreProcessing(int i)
        {
            if (lines[i].StartsWith('X') && newLines.Count != 0)
            {
                WriteToFile();
            }
            else if (lines[i].StartsWith('%'))
            {
                return false;
            }

            // extra checks for eurovision songs
            if (processType == Type.euroVision || processType == Type.euroVisionCleaned)
            {
                if (lines[i].StartsWith('V'))
                {
                    return false;
                }
                else if (lines[i].StartsWith('K'))
                {
                    lines[i] = lines[i].Split('%')[0];
                }

                lines[i] = String.Join("", lines[i].Split('z'));
                lines[i] = lines[i].Split('\\')[0];
                lines[i] = lines[i].Trim();

                if (lines[i] == "8|" && removeRests)
                {
                    return false;
                }
                else if (lines[i] == "8|")
                {
                    removeRests = true;
                }
                else
                {
                    removeRests = false;
                }

            }
            if (processType == Type.euroVisionCleaned)
            {
                if (lines[i].StartsWith('T'))
                {
                    baseName = lines[i].Split('/')[1].Split('.')[0];
                }
            }

            return true;
        }

        static bool PostProcessing(int i)
        {
            lines[i].Trim();

            // nice clean header
            if (lines[i].StartsWith('[') && lines[i].EndsWith(']'))
            {
                lines[i] = lines[i].Split('[')[1].Split(']')[0];

                if (lines[i].StartsWith('L'))
                {
                    lines[i] = "<s>" + lines[i];
                }

                return true;
            }
            // nice clean body
            else if (!lines[i].Contains('[') && !lines[i].Contains(']') && lines[i].Length > 0)
            {
                lines[i] += "</s>";
                return true;
            }
            // messy boddy/header
            else if (lines[i].Length > 0)
            {

                string[] subString = lines[i].Split('[');
                lines[i] = subString[0];

                for (int sub = 1; sub < subString.Length; sub++)
                {
                    if (subString[sub].Trim() == "]") continue;
                    if (subString[sub].StartsWith('L'))
                    {
                        lines[i] += "</s>@<s>" + subString[sub].Trim();
                    }
                    else if (subString[sub].StartsWith('M') || subString[sub].StartsWith('K'))
                    {
                        lines[i] += "@" + subString[sub].Trim();
                    }
                    else
                    {
                        lines[i] += subString[sub].Trim();
                    }
                }

                string[] subString1 = lines[i].Split(']');
                lines[i] = subString1[0];


                for (int sub = 1; sub < subString1.Length; sub++)
                {
                    

                    if (subString1[sub - 1].EndsWith("@ ") || subString1[sub].StartsWith(" @"))
                    {
                        lines[i] += subString1[sub].Trim();
                    }
                    else if(subString1[sub-1].StartsWith('L') || subString1[sub-1].StartsWith("@M") || subString1[sub-1].StartsWith("@K"))
                    {
                        lines[i] += "@" + subString1[sub].Trim();
                    }
                    else
                    {
                        lines[i] += subString1[sub].Trim();
                    }
                }

                string[] subString2 = lines[i].Split('@', StringSplitOptions.RemoveEmptyEntries);

                for (int sub = 0; sub < subString2.Length-1; sub++)
                {
                    if (subString2[sub].StartsWith("<s>"))
                    {
                        newLines.Add("");
                    }
                    newLines.Add(subString2[sub]);
                    
                }
                if (lines.Length > 1)
                {
                    newLines.Add(subString2[subString2.Length - 1] + "</s>");
                }

                return false;
            }
            return true;
        }

        static bool ToAudio(int i)
        {
            if (lines[i].StartsWith('<') && newLines.Count != 0)
            {
                WriteToFile();
            }

            if (lines[i].StartsWith('<'))
            {
                lines[i] = lines[i].Split('>')[1];
                newLines.Add("T:Song No. " + fileIndex);
            }
            else if (lines[i].EndsWith('>'))
            {
                lines[i] = lines[i].Split('<')[0];
            }

            return true;
        }

        static void WriteToFile()
        {
            if (processType == Type.euroVisionCleaned || processType == Type.post)
            {
                // Write the string array to a new file named "WriteLines.txt".
                using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, baseName + "." + extension)))
                {
                    foreach (string newline in newLines)
                        outputFile.WriteLine(newline);
                }
                newLines.Clear();
            }
            else
            {
                songIndex++;
                if (true)
                {
                    if (processType == Type.euroVision && fileIndex - 1 < chosenEuroVisionSongs.Length)
                    {
                        /*if (chosenEuroVisionSongs[fileIndex - 1] == songIndex)*/
                        goto End;
                    }
                    else if (processType == Type.folk && fileIndex - 1 < chosenFolkSongs.Length)
                    {
                        if (chosenFolkSongs[fileIndex - 1] == songIndex) goto End;
                    }
                    return;
                }
            End:

                // Write the string array to a new file named "WriteLines.txt".
                using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, baseName + fileIndex++ + "." + extension)))
                {
                    foreach (string newline in newLines)
                        outputFile.WriteLine(newline);
                }
                newLines.Clear();
            }


        }


    }
}
