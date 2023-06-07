using CsvHelper;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMM_Simulator;
public static class FileHandler
{
    public static List<MovementModel> GetDataFromCSV()
    {
        using (var reader = new StreamReader("C:\\Users\\Adam\\Documents\\CMM-Simulator\\CMM-Simulator\\MovementsData.csv"))
        using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csvReader.GetRecords<MovementModel>().ToList();

            return records;
        }
    }

    public static List<string> ReadAllNonEmptyLines(string path)
    {
        string line;
        List<string> lines = new List<string>();
        using (var reader = new StreamReader(path))
        {
            while((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line) == false)
                {
                    lines.Add(line);
                }
            }
        }
        return lines;
    }

    public static void RemoveTextOutfilFromFileLines(this List<string> fileLines)
    {
        fileLines.RemoveAll(line => line.StartsWith("TEXT/OUTFIL"));
    }

    public static void RemoveOutputsFromFileLines(this List<string> fileLines)
    {
        fileLines.RemoveAll(line => line.StartsWith("OUTPUT"));
    }

    public static void RemoveCommentsFromFileLines(this List<string> fileLines)
    {
        fileLines.RemoveAll(line => line.StartsWith("$$"));
    }
    public static void RemoveConstructedFeaturesFromFileLines(this List<string> fileLines)
    {
        int currentIndex = 0;

        while (currentIndex < fileLines.Count)
        {
            if (fileLines[currentIndex].Contains("FEAT"))
            {
                if (fileLines[currentIndex + 1].Contains("CONST"))
                {
                    fileLines.RemoveRange(currentIndex, 2);
                    currentIndex++;
                }
            }
            currentIndex++;
        }
    }

    public static void RemoveMultipleLineCode(this List<string> fileLines)
    {
        int currentIndex = 0;
        string newLine = "";

        while (currentIndex < fileLines.Count)
        {
            if (fileLines[currentIndex].EndsWith('$'))
            {
                while (fileLines[currentIndex].EndsWith('$'))
                {
                    newLine += fileLines[currentIndex].TrimEnd('$');
                    fileLines.RemoveAt(currentIndex);
                }
                newLine += fileLines[currentIndex];
                fileLines[currentIndex] = newLine;
                newLine = "";
            }
            currentIndex++;
        }
    }
}
