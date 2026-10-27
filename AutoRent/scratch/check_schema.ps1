$c = New-Object System.Data.OleDb.OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=c:\Users\allan\source\repos\AutoRent\AutoRent\Resources\Data\AutoRent.accdb.accdb")
$c.Open()
$cmd = $c.CreateCommand()
$cmd.CommandText = "SELECT TOP 1 * FROM Vehicles"
$adapter = New-Object System.Data.OleDb.OleDbDataAdapter($cmd)
$dt = New-Object System.Data.DataTable
$adapter.Fill($dt) | Out-Null
foreach ($col in $dt.Columns) {
    Write-Host ($col.ColumnName + " (" + $col.DataType.Name + ")")
}
$c.Close()
