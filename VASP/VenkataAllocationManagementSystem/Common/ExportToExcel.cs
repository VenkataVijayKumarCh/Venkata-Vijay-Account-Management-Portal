using System.Data;
using OfficeOpenXml;
using System.IO;
using System.ComponentModel;

namespace VenkataAllocationManagementSystem.Common
{
    public class ExportToExcel()
    {
        // public void ExportData()
        // {
        //     // Set the license context for non-commercial use
        //     ExcelPackage.LicenseContext = LicenseContext.NonCommercial; 
            
        //     // Now you can safely use EPPlus
        //     using (var package = new ExcelPackage(new FileInfo("MyFile.xlsx")))
        //     {
        //         // ... Excel code here
        //     }
        // }

        public void ExportDataTableToExcel(DataTable dataTable, string filePath)
        {
            // 1. Set the License Context
            // This is required for EPPlus 5.0 and later. Use NonCommercial for free personal/test use.
            // ExcelPackage.License = new LicenseContent(LicenseType.NonCommercial);
            ExcelPackage.License.SetNonCommercialPersonal("VASP - Venkata Allocation Management System");

            // 2. Create the Excel Package and Worksheet
            FileInfo file = new FileInfo(filePath);
            
            // Ensure the directory exists
            if (!file.Directory.Exists)
            {
                file.Directory.Create();
            }
            if(file.Exists)
            {
                file.Delete();  // If the file exists, delete it to avoid issues
                file = new FileInfo(filePath); // Recreate the FileInfo object
            }
            
            // Use a 'using' statement to ensure the package is correctly disposed of
            using (var package = new ExcelPackage(file))
            {
                // Add a new worksheet (you can change the name)
                var worksheet = package.Workbook.Worksheets.Add("DataExport");

                // 3. Load the DataTable into the Worksheet
                // This is the core step: it automatically maps all columns and rows.
                worksheet.Cells["A1"].LoadFromDataTable(dataTable, PrintHeaders: true);

                // 4. Optional: Formatting and Adjustments
                
                // Auto-fit all columns for better readability
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Make the header row bold
                worksheet.Row(1).Style.Font.Bold = true;
                
                // 5. Save the Excel file
                package.Save();
            }
            
            Console.WriteLine($"Successfully exported data to: {filePath}");
        }
    }
}