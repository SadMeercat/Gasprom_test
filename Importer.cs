using Microsoft.Win32;
using System.Data;
using System.IO;
using ClosedXML.Excel;
using System.Linq;

namespace WpfApp1
{
    public class Importer
    {
        public DataTable ImportExcel(string filePath)
        {
            var dataTable = new DataTable();

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheets.First();
                bool firstRow = true;

                foreach (var row in worksheet.RowsUsed())
                {
                    if (firstRow)
                    {
                        // Добавляем колонки
                        foreach (var cell in row.Cells())
                        {
                            dataTable.Columns.Add(cell.Value.ToString());
                        }
                        firstRow = false;
                    }
                    else
                    {
                        // Добавляем строки данных
                        var dataRow = dataTable.NewRow();
                        int i = 0;
                        foreach (var cell in row.Cells(1, dataTable.Columns.Count))
                        {
                            dataRow[i] = cell.Value.ToString();
                            i++;
                        }
                        dataTable.Rows.Add(dataRow);
                    }
                }
            }

            return dataTable;
        }

            public DataTable ImportCSV(string filePath)
        {
            var dataTable = new DataTable();

            using (var reader = new StreamReader(filePath)) 
            {
                bool isFirstLine = true;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    if (isFirstLine)
                    {

                        foreach (var header in values)
                        {
                            dataTable.Columns.Add(header);
                        }
                        isFirstLine = false;
                    }
                    else
                    {
                        dataTable.Rows.Add(values);
                    }
                }
                
            }
            return dataTable;
        }
    }
}
