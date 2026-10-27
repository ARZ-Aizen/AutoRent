$dbPath = "c:\Users\allan\source\repos\AutoRent\AutoRent\bin\Debug\net10.0-windows\Resources\Data\AutoRent.accdb.accdb"
$conn = New-Object System.Data.OleDb.OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$dbPath")
$conn.Open()

$cmd = $conn.CreateCommand()
$cmd.CommandText = "SELECT vehicle_id, plate, make, model, category, status, mileage FROM Vehicles WHERE vehicle_id = 65"
$reader = $cmd.ExecuteReader()
while ($reader.Read()) {
    Write-Host ("Vehicle: " + $reader["vehicle_id"] + " | " + $reader["plate"] + " | " + $reader["make"] + " " + $reader["model"] + " | " + $reader["category"] + " | " + $reader["status"] + " | " + $reader["mileage"])
}
$reader.Close()

$cmd.CommandText = "SELECT * FROM Reservations WHERE vehicle_id = 65"
$r2 = $cmd.ExecuteReader()
Write-Host "Reservations for 65:"
while ($r2.Read()) {
    Write-Host ($r2["reservation_id"].ToString() + " | " + $r2["customer_name"].ToString() + " | " + $r2["pickup_date"].ToString() + " | " + $r2["return_date"].ToString() + " | " + $r2["status"].ToString())
}
$r2.Close()

$cmd.CommandText = "SELECT * FROM Inspections WHERE vehicle_id = 65"
$r3 = $cmd.ExecuteReader()
Write-Host "Inspections for 65:"
while ($r3.Read()) {
    Write-Host ($r3["inspection_id"].ToString() + " | " + $r3["inspector"].ToString() + " | " + $r3["inspected_at"].ToString() + " | " + $r3["inspection_result"].ToString())
}
$r3.Close()

$conn.Close()
