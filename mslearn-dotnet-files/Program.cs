using Newtonsoft.Json;
using System.Text;

// Get project directories
var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");
var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);

// Find all input sales files (ignore output files)
var salesFiles = FindFiles(storesDirectory);

// Generate the report
var report = GenerateSalesReport(salesFiles);

// Write the report to file
File.WriteAllText(Path.Combine(salesTotalDir, "salesReport.txt"), report);

// ---------------------- FUNCTIONS ----------------------

IEnumerable<string> FindFiles(string folderName)
{
    // Include only .json files and ignore any output file like 'salestotals.json'
    return Directory
        .EnumerateFiles(folderName, "*.json", SearchOption.AllDirectories)
        .Where(f => Path.GetFileName(f).ToLower() != "salestotals.json");
}

string GenerateSalesReport(IEnumerable<string> salesFiles)
{
    var report = new StringBuilder();
    double totalSales = 0;
    var fileTotals = new Dictionary<string, double>();

    report.AppendLine("Sales Summary");
    report.AppendLine("----------------------------");

    foreach (var file in salesFiles)
    {
        string salesJson = File.ReadAllText(file);
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);
        double fileTotal = data?.Total ?? 0;

        // Use store folder name as label if available
        string storeName = Path.GetFileName(Path.GetDirectoryName(file) ?? file);
        fileTotals[storeName] = fileTotal;

        totalSales += fileTotal;
    }

    report.AppendLine($" Total Sales: {totalSales:C}");
    report.AppendLine();
    report.AppendLine(" Details:");

    foreach (var entry in fileTotals)
    {
        report.AppendLine($"  {entry.Key}: {entry.Value:C}");
    }

    return report.ToString();
}

// ---------------------- DATA RECORD ----------------------
record SalesData(double Total);