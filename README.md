# csql_core
Very simple SQL Server command line client written in .net core for use in Linux.

To use: 

1. Create a file called connectionString.txt and put it in the folder you will be running csql from. The file should contain a single line in standard connection string format. For example:

    Data Source=127.0.0.1;Initial Catalog=master;Integrated Security=False;User ID=SA;Password=*****

2. Run the program and pass it the sql to execute in quotation marks. For example:

    dotnet ./csql.dll "select * from sysdatabases"


To make it easier you can set up an alias in your .bashrc as follows (adjusting the path to the csql.dll as appropriate). For example:

    alias csql="dotnet ~/csql/csql.dll"

