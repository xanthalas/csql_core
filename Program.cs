using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace csql
{
    class Program
    {
        static void Main(string[] args)
        {
            
            //In this simplistic version assume all args are the command
            var command = String.Join(" ", args);

            var connectionStringPath = Path.Combine(Environment.CurrentDirectory, "connectionString.txt");

            var connString = File.ReadAllText(connectionStringPath);

            processSQL(connString, command);
        }

        static void processSQL(string connectionString, string command)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error connecting to database using string: " + connectionString);
                Console.WriteLine("Error was: " + ex.Message);    
                return;
            }

            SqlCommand cmd = new System.Data.SqlClient.SqlCommand(command, connection);
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet returnDs = new DataSet();

            try
            {
                adapter.Fill(returnDs);
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return;
            }
            int tableCount = 1;

            foreach (DataTable table in returnDs.Tables)
            {
                if (returnDs.Tables.Count > 1)
                {
                    var rowText = (table.Rows.Count == 1) ? " row" : " rows";
                    Console.WriteLine("\nTable " + tableCount.ToString() + ": " + table.Rows.Count.ToString() + rowText);
                }
                else
                {
                    var rowText = (table.Rows.Count == 1) ? " row" : " rows";
                    Console.WriteLine("\n" + table.Rows.Count.ToString() + rowText);
                }

                outputInLineFormat(table);

                Console.WriteLine(" ");

                tableCount++;
            }

            if (returnDs.Tables.Count == 1)
            {
                Console.WriteLine("\nFinished. 1 table returned.");
            }
            else
            {
                Console.WriteLine("\nFinished. {0} tables returned.", returnDs.Tables.Count);
            }
        }

        /// <summary>
        /// Display the contents of the table in Line oriented mode
        /// </summary>
        /// <param name="table"></param>
        private static void outputInLineFormat(DataTable table)
        {
            //First find the maximum width of each column of data
            List<int> maxColumnWidth = new List<int>(table.Columns.Count);

            foreach (DataColumn col in table.Columns)
            {
                maxColumnWidth.Add(col.ColumnName.Length);
            }

            int index = 0;
            foreach (DataRow row in table.Rows)
            {
                index = 0;
                foreach (object column in row.ItemArray)
                {
                    var colLength = column.ToString().Length;
                    if (colLength > maxColumnWidth[index])
                    {
                        maxColumnWidth[index] = colLength;
                    }
                    index++;
                }
            }

            StringBuilder sbSeparator = new StringBuilder();
            int lineWidth = 0;
            foreach (var value in maxColumnWidth)
            {
                lineWidth += value + 1;
                sbSeparator.Append("+" + "".PadLeft(value, '-'));
            }

            //Now write out the actual data
            sbSeparator.Append("+");
            string separator = sbSeparator.ToString();

            Console.Write(separator + "\n");
            index = 0;
            foreach (DataColumn col in table.Columns)
            {
                Console.Write(formatColumn(col.ColumnName, maxColumnWidth[index]));
                index++;
            }

            Console.Write("|\n");
            Console.Write(separator);

            foreach (DataRow row in table.Rows)
            {
                Console.Write("\n");
                index = 0;
                foreach (object column in row.ItemArray)
                {
                    Console.Write(formatColumn(column.ToString(), maxColumnWidth[index]));
                    index++;
                }
                Console.Write("|");
            }
            Console.Write("\n");
            Console.Write(separator);
        }
        private static string formatColumn(string text, int width)
        {
            return "|" + text.PadRight(width, ' ');
        }
    }
}
