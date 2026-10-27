$dbs = @(
    "c:\Users\allan\source\repos\AutoRent\AutoRent\Resources\Data\AutoRent.accdb.accdb",
    "c:\Users\allan\source\repos\AutoRent\AutoRent\bin\Debug\net10.0-windows\Resources\Data\AutoRent.accdb.accdb"
)

foreach ($db in $dbs) {
    Write-Host "===================================="
    Write-Host "Verifying: $db"
    $conn = New-Object System.Data.OleDb.OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$db")
    $conn.Open()

    $staffCmd = $conn.CreateCommand()
    $staffCmd.CommandText = "SELECT full_name FROM Users"
    $rStaff = $staffCmd.ExecuteReader()
    $staffList = New-Object System.Collections.Generic.List[string]
    while ($rStaff.Read()) {
        $staffList.Add($rStaff["full_name"].ToString())
    }
    $rStaff.Close()
    Write-Host ("Staff count: " + $staffList.Count + " -> " + ($staffList -join ", "))

    $custCmd = $conn.CreateCommand()
    $custCmd.CommandText = "SELECT full_name FROM Customers"
    $rCust = $custCmd.ExecuteReader()
    $custList = New-Object System.Collections.Generic.List[string]
    $overlap = New-Object System.Collections.Generic.List[string]
    while ($rCust.Read()) {
        $cN = $rCust["full_name"].ToString()
        $custList.Add($cN)
        if ($staffList.Contains($cN)) {
            $overlap.Add($cN)
        }
    }
    $rCust.Close()
    Write-Host ("Customers count: " + $custList.Count)
    if ($overlap.Count -gt 0) {
        Write-Host ("ERROR! Overlap between Staff and Customers: " + ($overlap -join ", "))
    } else {
        Write-Host "SUCCESS: Zero overlap between Staff and Customers!"
    }

    $resOverlap = New-Object System.Collections.Generic.List[string]
    $resCmd = $conn.CreateCommand()
    $resCmd.CommandText = "SELECT reservation_id, customer_name, total, status FROM Reservations"
    $rRes = $resCmd.ExecuteReader()
    $resCount = 0
    $rev = 0.0
    while ($rRes.Read()) {
        $resCount++
        $rC = $rRes["customer_name"].ToString()
        if ($staffList.Contains($rC)) {
            $resOverlap.Add($rC)
        }
        if ($rRes["status"].ToString() -eq "Completed") {
            $rev += [double]$rRes["total"]
        }
    }
    $rRes.Close()
    Write-Host ("Reservations count: " + $resCount + ", Completed Revenue: PHP " + $rev)
    if ($resOverlap.Count -gt 0) {
        Write-Host ("ERROR! Staff in Reservations: " + ($resOverlap -join ", "))
    } else {
        Write-Host "SUCCESS: Zero staff names in Reservations!"
    }

    $maintCmd = $conn.CreateCommand()
    $maintCmd.CommandText = "SELECT SUM(cost) FROM Maintenance WHERE status = 'Completed'"
    $exp = $maintCmd.ExecuteScalar()
    Write-Host ("Completed Maintenance Expenses: PHP " + $exp)

    $conn.Close()
}
