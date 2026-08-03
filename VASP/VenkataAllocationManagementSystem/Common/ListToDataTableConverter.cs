using System.Data;
using System.Reflection;
using System.Collections.Generic;

namespace VenkataAllocationManagementSystem.Common
{
    public class ListToDataTableConverter()
    {
        public static DataTable ListToDataTableConversion<T>(List<T> items)
        {
            DataTable dt = new DataTable();
            
            if (items == null || items.Count == 0)
            {
                return dt; // Return an empty table if the list is empty
            }

            // 1. Get the properties of the anonymous type (T) from the first item
            PropertyInfo[] properties = typeof(T).GetProperties();

            // 2. Dynamically create columns based on the property names and types
            foreach (PropertyInfo prop in properties)
            {
                // System.Diagnostics.EventLog.WriteEntry("Application", $"Adding column: {prop.Name} of type {prop.PropertyType}", System.Diagnostics.EventLogEntryType.Information);
                // Use the property name as the column name
                // Use the property's type as the column type
                dt.Columns.Add(prop.Name, prop.PropertyType);
            }

            // 3. Populate rows dynamically
            foreach (T item in items)
            {
                DataRow row = dt.NewRow();
                
                // Iterate through the properties again to get the values
                for (int i = 0; i < properties.Length; i++)
                {
                    // Use GetValue to retrieve the value from the current 'item'
                    object value = properties[i].GetValue(item, null);
                    
                    // Handle DBNull for null values, as DataTable requires it
                    row[i] = value ?? DBNull.Value; 
                }
                dt.Rows.Add(row);
            }

            return dt;
        }
    }
}