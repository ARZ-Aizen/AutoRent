Add-Type -AssemblyName System.Data

$dbs = @(
    "c:\Users\allan\source\repos\AutoRent\AutoRent\Resources\Data\AutoRent.accdb.accdb",
    "c:\Users\allan\source\repos\AutoRent\AutoRent\bin\Debug\net10.0-windows\Resources\Data\AutoRent.accdb.accdb"
)

foreach ($db in $dbs) {
    if (-not (Test-Path $db)) { continue }
    $conn = New-Object System.Data.OleDb.OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$db")
    $conn.Open()

    $chk = $conn.CreateCommand()
    $chk.CommandText = "SELECT COUNT(*) FROM [Reservations] WHERE [vehicle_id] = 65"
    $hasRes = [int]$chk.ExecuteScalar()

    if ($hasRes -eq 0) {
        $ins = $conn.CreateCommand()
        $ins.CommandText = "INSERT INTO [Reservations] ([customer_name], [vehicle_id], [pickup_date], [return_date], [total], [status], [completed_at], [created_at]) VALUES (?, ?, ?, ?, ?, ?, ?, ?)"
        $ins.Parameters.Add("cn", [System.Data.OleDb.OleDbType]::VarWChar, 100).Value = "Andrei Balboa"
        $ins.Parameters.Add("vid", [System.Data.OleDb.OleDbType]::Integer).Value = 65
        $ins.Parameters.Add("pud", [System.Data.OleDb.OleDbType]::Date).Value = [DateTime]::Parse("2026-10-18 09:00:00")
        $ins.Parameters.Add("red", [System.Data.OleDb.OleDbType]::Date).Value = [DateTime]::Parse("2026-10-22 17:00:00")
        $ins.Parameters.Add("tot", [System.Data.OleDb.OleDbType]::Currency).Value = [decimal]2600.00
        $ins.Parameters.Add("st", [System.Data.OleDb.OleDbType]::VarWChar, 20).Value = "Completed"
        $ins.Parameters.Add("ca", [System.Data.OleDb.OleDbType]::Date).Value = [DateTime]::Parse("2026-10-22 16:30:00")
        $ins.Parameters.Add("created", [System.Data.OleDb.OleDbType]::Date).Value = [DateTime]::Parse("2026-10-15 14:00:00")
        $ins.ExecuteNonQuery() | Out-Null

        $idCmd = $conn.CreateCommand()
        $idCmd.CommandText = "SELECT @@IDENTITY"
        $resId = [int]$idCmd.ExecuteScalar()

        $dispPk = $conn.CreateCommand()
        $dispPk.CommandText = "INSERT INTO [Dispatches] ([reservation_id], [dispatch_type], [scheduled_at], [branch], [status]) VALUES (?, ?, ?, ?, ?)"
        $dispPk.Parameters.Add("rid", [System.Data.OleDb.OleDbType]::Integer).Value = $resId
        $dispPk.Parameters.Add("dt", [System.Data.OleDb.OleDbType]::VarWChar, 10).Value = "Pickup"
        $dispPk.Parameters.Add("sa", [System.Data.OleDb.OleDbType]::Date).Value = [DateTime]::Parse("2026-10-18 09:00:00")
        $dispPk.Parameters.Add("br", [System.Data.OleDb.OleDbType]::VarWChar, 50).Value = "BGC Taguig Branch - 5th Ave"
        $dispPk.Parameters.Add("st", [System.Data.OleDb.OleDbType]::VarWChar, 20).Value = "Completed"
        $dispPk.ExecuteNonQuery() | Out-Null

        $dispRet = $conn.CreateCommand()
        $dispRet.CommandText = "INSERT INTO [Dispatches] ([reservation_id], [dispatch_type], [scheduled_at], [branch], [status]) VALUES (?, ?, ?, ?, ?)"
        $dispRet.Parameters.Add("rid", [System.Data.OleDb.OleDbType]::Integer).Value = $resId
        $dispRet.Parameters.Add("dt", [System.Data.OleDb.OleDbType]::VarWChar, 10).Value = "Return"
        $dispRet.Parameters.Add("sa", [System.Data.OleDb.OleDbType]::Date).Value = [DateTime]::Parse("2026-10-22 16:30:00")
        $dispRet.Parameters.Add("br", [System.Data.OleDb.OleDbType]::VarWChar, 50).Value = "BGC Taguig Branch - 5th Ave"
        $dispRet.Parameters.Add("st", [System.Data.OleDb.OleDbType]::VarWChar, 20).Value = "Completed"
        $dispRet.ExecuteNonQuery() | Out-Null
    }

    $chkIns = $conn.CreateCommand()
    $chkIns.CommandText = "SELECT COUNT(*) FROM [Inspections] WHERE [vehicle_id] = 65"
    $hasIns = [int]$chkIns.ExecuteScalar()

    if ($hasIns -eq 0) {
        $insCmd = $conn.CreateCommand()
        $insCmd.CommandText = "INSERT INTO [Inspections] ([vehicle_id], [inspected_at], [inspector], [fuel], [findings], [inspection_result]) VALUES (?, ?, ?, ?, ?, ?)"
        $insCmd.Parameters.Add("vid", [System.Data.OleDb.OleDbType]::Integer).Value = 65
        $insCmd.Parameters.Add("ia", [System.Data.OleDb.OleDbType]::Date).Value = [DateTime]::Parse("2026-10-22 16:45:00")
        $insCmd.Parameters.Add("insp", [System.Data.OleDb.OleDbType]::VarWChar, 100).Value = "Jerome Lumbao"
        $insCmd.Parameters.Add("fl", [System.Data.OleDb.OleDbType]::VarWChar, 20).Value = "Full"
        $insCmd.Parameters.Add("find", [System.Data.OleDb.OleDbType]::LongVarWChar).Value = "Post-rental inspection: exterior spotless, tire tread depth 6mm, dual-clutch transmission shifts smoothly."
        $insCmd.Parameters.Add("res", [System.Data.OleDb.OleDbType]::VarWChar, 20).Value = "Passed"
        $insCmd.ExecuteNonQuery() | Out-Null
    }

    $chkMaint = $conn.CreateCommand()
    $chkMaint.CommandText = "SELECT COUNT(*) FROM [Maintenance] WHERE [vehicle_id] = 65"
    $hasMaint = [int]$chkMaint.ExecuteScalar()

    if ($hasMaint -eq 0) {
        $mCmd = $conn.CreateCommand()
        $mCmd.CommandText = "INSERT INTO [Maintenance] ([vehicle_id], [service_type], [scheduled_date], [cost], [status], [notes], [completed_at], [created_at]) VALUES (?, ?, ?, ?, ?, ?, ?, ?)"
        $mCmd.Parameters.Add("vid", [System.Data.OleDb.OleDbType]::Integer).Value = 65
        $mCmd.Parameters.Add("st", [System.Data.OleDb.OleDbType]::VarWChar, 60).Value = "Executive Sports Car PMS & Brake Inspection"
        $mCmd.Parameters.Add("sd", [System.Data.OleDb.OleDbType]::Date).Value = [DateTime]::Parse("2026-10-12 10:00:00")
        $mCmd.Parameters.Add("co", [System.Data.OleDb.OleDbType]::Currency).Value = [decimal]6800.00
        $mCmd.Parameters.Add("sta", [System.Data.OleDb.OleDbType]::VarWChar, 20).Value = "Completed"
        $mCmd.Parameters.Add("no", [System.Data.OleDb.OleDbType]::LongVarWChar).Value = "BMW AutoHaus BGC - High-performance synthetic oil change and multi-point safety inspection."
        $mCmd.Parameters.Add("ca", [System.Data.OleDb.OleDbType]::Date).Value = [DateTime]::Parse("2026-10-12 14:30:00")
        $mCmd.Parameters.Add("created", [System.Data.OleDb.OleDbType]::Date).Value = [DateTime]::Parse("2026-10-09 11:00:00")
        $mCmd.ExecuteNonQuery() | Out-Null
    }

    $conn.Close()
    Write-Host "Seeded Vehicle 65 data for $db"
}
