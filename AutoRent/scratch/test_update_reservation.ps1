$dbPath = "AutoRent\bin\Debug\net10.0-windows\Resources\Data\AutoRent.accdb.accdb"
$connStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$dbPath;"
$conn = New-Object System.Data.OleDb.OleDbConnection($connStr)
$conn.Open()

Write-Output "--- CUSTOMERS WITH / WITHOUT LICENSE ---"
$cmd = $conn.CreateCommand()
$cmd.CommandText = "SELECT customer_id, full_name, license_no FROM Customers ORDER BY customer_id"
$reader = $cmd.ExecuteReader()
while ($reader.Read()) {
    $lic = if ($reader.IsDBNull(2) -or [string]::IsNullOrWhiteSpace($reader.GetString(2))) { "NO LICENSE" } else { $reader.GetString(2) }
    Write-Output "Customer #$($reader.GetInt32(0)): $($reader.GetString(1)) -> License: $lic"
}
$reader.Close()

Write-Output ""
Write-Output "--- ACTIVE / CONFIRMED RESERVATIONS ---"
$cmd.CommandText = "SELECT reservation_id, customer_name, vehicle_id, pickup_date, return_date, total, status FROM Reservations WHERE status IN ('Active', 'Confirmed')"
$reader = $cmd.ExecuteReader()
while ($reader.Read()) {
    Write-Output "Res #$($reader.GetInt32(0)): $($reader.GetString(1)) | Veh #$($reader.GetInt32(2)) | $($reader.GetDateTime(3).ToString('yyyy-MM-dd')) to $($reader.GetDateTime(4).ToString('yyyy-MM-dd')) | P$($reader.GetDecimal(5)) | Status: $($reader.GetString(6))"
}
$reader.Close()

$conn.Close()
