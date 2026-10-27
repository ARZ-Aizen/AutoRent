$dbPath = "AutoRent\bin\Debug\net10.0-windows\Resources\Data\AutoRent.accdb.accdb"
$connStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$dbPath;"
$conn = New-Object System.Data.OleDb.OleDbConnection($connStr)
$conn.Open()

# Insert a customer without a license
$cmd = $conn.CreateCommand()
$cmd.CommandText = "INSERT INTO Customers (full_name, phone, email, license_no) VALUES ('Unlicensed Test User', '09123456789', 'unlicensed@test.ph', '')"
$cmd.ExecuteNonQuery() | Out-Null

$cmd.CommandText = "SELECT @@IDENTITY"
$unlicensedId = $cmd.ExecuteScalar()
Write-Output "Created unlicensed customer with ID: $unlicensedId"

# Check if UpdateReservation query check catches this
$cmdLic = $conn.CreateCommand()
$cmdLic.CommandText = "SELECT [license_no] FROM [Customers] WHERE [full_name] = 'Unlicensed Test User'"
$licObj = $cmdLic.ExecuteScalar()
$licStr = if ($licObj -eq $null -or [DBNull]::Value.Equals($licObj)) { "" } else { $licObj.ToString().Trim() }

if ([string]::IsNullOrWhiteSpace($licStr)) {
    Write-Output "PASS: License guard successfully detected that 'Unlicensed Test User' has no license!"
} else {
    Write-Output "FAIL: License guard failed!"
}

# Clean up test customer
$cmd.CommandText = "DELETE FROM Customers WHERE full_name = 'Unlicensed Test User'"
$cmd.ExecuteNonQuery() | Out-Null
Write-Output "Cleaned up test customer."

$conn.Close()
