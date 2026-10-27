Add-Type -AssemblyName System.Data

$dbs = @(
    "c:\Users\allan\source\repos\AutoRent\AutoRent\Resources\Data\AutoRent.accdb.accdb",
    "c:\Users\allan\source\repos\AutoRent\AutoRent\bin\Debug\net10.0-windows\Resources\Data\AutoRent.accdb.accdb"
)

function Hash-Password([string]$password) {
    $sha256 = [System.Security.Cryptography.SHA256]::Create()
    $bytes = [System.Text.Encoding]::UTF8.GetBytes($password)
    $hash = $sha256.ComputeHash($bytes)
    return [System.Convert]::ToBase64String($hash)
}

foreach ($db in $dbs) {
    if (-not (Test-Path $db)) { continue }
    Write-Host "Synchronizing: $db"
    $conn = New-Object System.Data.OleDb.OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$db")
    $conn.Open()

    $cmd = $conn.CreateCommand()
    $cmd.CommandText = "DELETE FROM [Dispatches]"
    $cmd.ExecuteNonQuery() | Out-Null

    $cmd.CommandText = "DELETE FROM [Reservations]"
    $cmd.ExecuteNonQuery() | Out-Null

    $cmd.CommandText = "DELETE FROM [Customers]"
    $cmd.ExecuteNonQuery() | Out-Null

    $cmd.CommandText = "DELETE FROM [Inspections]"
    $cmd.ExecuteNonQuery() | Out-Null

    $cmd.CommandText = "DELETE FROM [Maintenance]"
    $cmd.ExecuteNonQuery() | Out-Null

    $staffUsers = @(
        @{ User = "admin"; Name = "Allan Poe"; Role = "Administrator"; Branch = "Makati Central Depot - Ayala"; Active = $true },
        @{ User = "jerome"; Name = "Jerome Lumbao"; Role = "Fleet Specialist"; Branch = "NAIA Terminal 3 Hub - Pasay"; Active = $true },
        @{ User = "princess"; Name = "Princess Santander"; Role = "Operations Clerk"; Branch = "BGC Taguig Branch - 5th Ave"; Active = $true },
        @{ User = "allan"; Name = "Allan Oiga"; Role = "Fleet Auditor"; Branch = "Makati Central Depot - Ayala"; Active = $true },
        @{ User = "liam"; Name = "Liam Serrano"; Role = "Inspector"; Branch = "Quezon City North EDSA Depot"; Active = $true },
        @{ User = "andrei"; Name = "Andrei Balboa"; Role = "Rental Staff"; Branch = "NAIA Terminal 3 Hub - Pasay"; Active = $true },
        @{ User = "zake"; Name = "Zake Yurtas"; Role = "Rental Staff"; Branch = "BGC Taguig Branch - 5th Ave"; Active = $true },
        @{ User = "adrielle"; Name = "Adrielle Bino"; Role = "Rental Staff"; Branch = "Cebu IT Park Hub - Lahug"; Active = $true },
        @{ User = "jonathan"; Name = "Jonathan Balboa"; Role = "Manager"; Branch = "Clark Airport Station - Pampanga"; Active = $true }
    )

    $defaultHash = Hash-Password "AutoRent@2026"

    foreach ($u in $staffUsers) {
        $chk = $conn.CreateCommand()
        $chk.CommandText = "SELECT COUNT(*) FROM [Users] WHERE [username] = ?"
        $chk.Parameters.Add("u", [System.Data.OleDb.OleDbType]::VarWChar, 100).Value = $u.User
        $exists = [int]$chk.ExecuteScalar()

        if ($exists -eq 0) {
            $ins = $conn.CreateCommand()
            $ins.CommandText = "INSERT INTO [Users] ([username], [full_name], [branch], [password_hash], [role], [is_active], [is_deleted], [created_at]) VALUES (?, ?, ?, ?, ?, ?, ?, ?)"
            $ins.Parameters.Add("u", [System.Data.OleDb.OleDbType]::VarWChar, 100).Value = $u.User
            $ins.Parameters.Add("fn", [System.Data.OleDb.OleDbType]::VarWChar, 100).Value = $u.Name
            $ins.Parameters.Add("br", [System.Data.OleDb.OleDbType]::VarWChar, 50).Value = $u.Branch
            $ins.Parameters.Add("ph", [System.Data.OleDb.OleDbType]::VarWChar, 255).Value = $defaultHash
            $ins.Parameters.Add("ro", [System.Data.OleDb.OleDbType]::VarWChar, 50).Value = $u.Role
            $ins.Parameters.Add("act", [System.Data.OleDb.OleDbType]::Boolean).Value = $u.Active
            $ins.Parameters.Add("del", [System.Data.OleDb.OleDbType]::Boolean).Value = $false
            $ins.Parameters.Add("ca", [System.Data.OleDb.OleDbType]::Date).Value = [DateTime]::Now
            $ins.ExecuteNonQuery() | Out-Null
        } else {
            $upd = $conn.CreateCommand()
            $upd.CommandText = "UPDATE [Users] SET [full_name] = ?, [branch] = ?, [role] = ?, [is_active] = ?, [is_deleted] = False WHERE [username] = ?"
            $upd.Parameters.Add("fn", [System.Data.OleDb.OleDbType]::VarWChar, 100).Value = $u.Name
            $upd.Parameters.Add("br", [System.Data.OleDb.OleDbType]::VarWChar, 50).Value = $u.Branch
            $upd.Parameters.Add("ro", [System.Data.OleDb.OleDbType]::VarWChar, 50).Value = $u.Role
            $upd.Parameters.Add("act", [System.Data.OleDb.OleDbType]::Boolean).Value = $u.Active
            $upd.Parameters.Add("u", [System.Data.OleDb.OleDbType]::VarWChar, 100).Value = $u.User
            $upd.ExecuteNonQuery() | Out-Null
        }
    }

    $customers = @(
        @{ Name = "Maria Cristina Santos"; Phone = "0917-842-1920"; Email = "maria.santos@gmail.com"; License = "N01-19-038291" },
        @{ Name = "Juan Carlo Dela Cruz"; Phone = "0920-514-8832"; Email = "juan.delacruz@yahoo.com"; License = "N02-20-084729" },
        @{ Name = "Danica Mae Sotto"; Phone = "0945-812-3341"; Email = "danica.sotto@gmail.com"; License = "C01-21-029384" },
        @{ Name = "Gabriel Reyes"; Phone = "0998-331-7765"; Email = "gabriel.reyes@gmail.com"; License = "N03-22-049182" },
        @{ Name = "Patricia Nicole Lim"; Phone = "0918-629-9943"; Email = "patricia.lim@gmail.com"; License = "N01-23-087654" },
        @{ Name = "Rafael Dominic Aquino"; Phone = "0927-445-6612"; Email = "rafael.aquino@outlook.com"; License = "D12-22-048291" },
        @{ Name = "Bea Alonzo Evangelista"; Phone = "0916-789-5521"; Email = "bea.evangelista@gmail.com"; License = "N01-24-039182" },
        @{ Name = "Miguel Luis Tan"; Phone = "0999-881-4433"; Email = "miguel.tan@gmail.com"; License = "N02-21-073819" },
        @{ Name = "Katrina Anne Morales"; Phone = "0915-224-8890"; Email = "katrina.morales@yahoo.com"; License = "N03-23-018274" },
        @{ Name = "Christian Dave Ramos"; Phone = "0928-990-3321"; Email = "christian.ramos@gmail.com"; License = "N01-24-098231" },
        @{ Name = "Alyssa Marie Valenzuela"; Phone = "0917-334-1189"; Email = "alyssa.valenzuela@gmail.com"; License = "N04-20-091823" },
        @{ Name = "Joshua Paolo Dizon"; Phone = "0921-778-4456"; Email = "joshua.dizon@yahoo.com"; License = "C02-22-038192" },
        @{ Name = "Stephanie Grace Co"; Phone = "0946-229-8877"; Email = "stephanie.co@gmail.com"; License = "N01-21-049182" },
        @{ Name = "Kenneth Ryan Garcia"; Phone = "0997-665-2211"; Email = "kenneth.garcia@outlook.com"; License = "N03-23-084719" },
        @{ Name = "Michelle Andrea Roxas"; Phone = "0919-443-7788"; Email = "michelle.roxas@gmail.com"; License = "N02-24-019283" },
        @{ Name = "Clarisse Joyce Bernardo"; Phone = "0926-554-9900"; Email = "clarisse.bernardo@gmail.com"; License = "N01-22-076123" },
        @{ Name = "Ferdinand Marcos Reyes"; Phone = "0918-771-3344"; Email = "ferdinand.reyes@yahoo.com"; License = "N03-20-058291" },
        @{ Name = "Anthony Paul David"; Phone = "0932-889-1122"; Email = "anthony.david@gmail.com"; License = "C01-23-039184" }
    )

    foreach ($cItem in $customers) {
        $ins = $conn.CreateCommand()
        $ins.CommandText = "INSERT INTO [Customers] ([full_name], [phone], [email], [license_no], [created_at]) VALUES (?, ?, ?, ?, ?)"
        $ins.Parameters.Add("fn", [System.Data.OleDb.OleDbType]::VarWChar, 100).Value = $cItem.Name
        $ins.Parameters.Add("ph", [System.Data.OleDb.OleDbType]::VarWChar, 30).Value = $cItem.Phone
        $ins.Parameters.Add("em", [System.Data.OleDb.OleDbType]::VarWChar, 100).Value = $cItem.Email
        $ins.Parameters.Add("lic", [System.Data.OleDb.OleDbType]::VarWChar, 50).Value = $cItem.License
        $ins.Parameters.Add("ca", [System.Data.OleDb.OleDbType]::Date).Value = [DateTime]::Now.AddDays(-30)
        $ins.ExecuteNonQuery() | Out-Null
    }

    $reservations = @(
        @{ Customer = "Bea Alonzo Evangelista"; VehId = 65; Pickup = "2026-10-18 09:00:00"; Return = "2026-10-22 17:00:00"; Total = 2600.00; Status = "Completed"; CompletedAt = "2026-10-22 16:30:00"; Branch = "BGC Taguig Branch - 5th Ave"; PkStatus = "Completed"; RetStatus = "Completed" },
        @{ Customer = "Stephanie Grace Co"; VehId = 13; Pickup = "2026-10-06 09:00:00"; Return = "2026-10-11 17:00:00"; Total = 2400.00; Status = "Completed"; CompletedAt = "2026-10-11 15:45:00"; Branch = "BGC Taguig Branch - 5th Ave"; PkStatus = "Completed"; RetStatus = "Completed" },
        @{ Customer = "Kenneth Ryan Garcia"; VehId = 6; Pickup = "2026-10-10 09:00:00"; Return = "2026-10-14 17:00:00"; Total = 1800.00; Status = "Completed"; CompletedAt = "2026-10-14 16:15:00"; Branch = "Makati Central Depot - Ayala"; PkStatus = "Completed"; RetStatus = "Completed" },
        @{ Customer = "Michelle Andrea Roxas"; VehId = 76; Pickup = "2026-10-14 09:00:00"; Return = "2026-10-18 17:00:00"; Total = 2400.00; Status = "Completed"; CompletedAt = "2026-10-18 14:50:00"; Branch = "BGC Taguig Branch - 5th Ave"; PkStatus = "Completed"; RetStatus = "Completed" },
        @{ Customer = "Clarisse Joyce Bernardo"; VehId = 51; Pickup = "2026-10-16 09:00:00"; Return = "2026-10-20 17:00:00"; Total = 1440.00; Status = "Completed"; CompletedAt = "2026-10-20 17:10:00"; Branch = "Quezon City North EDSA Depot"; PkStatus = "Completed"; RetStatus = "Completed" },
        @{ Customer = "Ferdinand Marcos Reyes"; VehId = 100; Pickup = "2026-10-18 09:00:00"; Return = "2026-10-22 17:00:00"; Total = 1840.00; Status = "Completed"; CompletedAt = "2026-10-22 16:05:00"; Branch = "BGC Taguig Branch - 5th Ave"; PkStatus = "Completed"; RetStatus = "Completed" },
        @{ Customer = "Anthony Paul David"; VehId = 4; Pickup = "2026-10-20 09:00:00"; Return = "2026-10-24 17:00:00"; Total = 1800.00; Status = "Completed"; CompletedAt = "2026-10-24 16:20:00"; Branch = "Makati Central Depot - Ayala"; PkStatus = "Completed"; RetStatus = "Completed" },

        @{ Customer = "Maria Cristina Santos"; VehId = 11; Pickup = "2026-10-25 09:00:00"; Return = "2026-10-29 17:00:00"; Total = 1800.00; Status = "Active"; CompletedAt = $null; Branch = "NAIA Terminal 3 Hub - Pasay"; PkStatus = "Completed"; RetStatus = "Scheduled" },
        @{ Customer = "Juan Carlo Dela Cruz"; VehId = 14; Pickup = "2026-10-24 09:00:00"; Return = "2026-10-29 17:00:00"; Total = 2400.00; Status = "Active"; CompletedAt = $null; Branch = "BGC Taguig Branch - 5th Ave"; PkStatus = "Completed"; RetStatus = "Scheduled" },
        @{ Customer = "Danica Mae Sotto"; VehId = 46; Pickup = "2026-10-26 09:00:00"; Return = "2026-10-30 17:00:00"; Total = 1440.00; Status = "Active"; CompletedAt = $null; Branch = "Makati Central Depot - Ayala"; PkStatus = "Completed"; RetStatus = "Scheduled" },
        @{ Customer = "Gabriel Reyes"; VehId = 56; Pickup = "2026-10-25 09:00:00"; Return = "2026-10-28 17:00:00"; Total = 1260.00; Status = "Active"; CompletedAt = $null; Branch = "Clark Airport Station - Pampanga"; PkStatus = "Completed"; RetStatus = "Scheduled" },
        @{ Customer = "Patricia Nicole Lim"; VehId = 30; Pickup = "2026-10-26 09:00:00"; Return = "2026-10-28 17:00:00"; Total = 760.00; Status = "Active"; CompletedAt = $null; Branch = "Quezon City North EDSA Depot"; PkStatus = "Completed"; RetStatus = "Scheduled" },
        @{ Customer = "Rafael Dominic Aquino"; VehId = 34; Pickup = "2026-10-27 09:00:00"; Return = "2026-10-31 17:00:00"; Total = 1400.00; Status = "Active"; CompletedAt = $null; Branch = "NAIA Terminal 3 Hub - Pasay"; PkStatus = "Completed"; RetStatus = "Scheduled" },
        @{ Customer = "Miguel Luis Tan"; VehId = 69; Pickup = "2026-10-26 10:00:00"; Return = "2026-10-30 18:00:00"; Total = 2600.00; Status = "Active"; CompletedAt = $null; Branch = "BGC Taguig Branch - 5th Ave"; PkStatus = "Completed"; RetStatus = "Scheduled" },

        @{ Customer = "Katrina Anne Morales"; VehId = 5; Pickup = "2026-10-28 09:00:00"; Return = "2026-11-02 17:00:00"; Total = 2250.00; Status = "Confirmed"; CompletedAt = $null; Branch = "BGC Taguig Branch - 5th Ave"; PkStatus = "Scheduled"; RetStatus = "Scheduled" },
        @{ Customer = "Christian Dave Ramos"; VehId = 16; Pickup = "2026-10-29 09:00:00"; Return = "2026-11-03 17:00:00"; Total = 2400.00; Status = "Confirmed"; CompletedAt = $null; Branch = "Makati Central Depot - Ayala"; PkStatus = "Scheduled"; RetStatus = "Scheduled" },
        @{ Customer = "Alyssa Marie Valenzuela"; VehId = 59; Pickup = "2026-10-30 09:00:00"; Return = "2026-11-04 17:00:00"; Total = 2100.00; Status = "Confirmed"; CompletedAt = $null; Branch = "Clark Airport Station - Pampanga"; PkStatus = "Scheduled"; RetStatus = "Scheduled" },
        @{ Customer = "Joshua Paolo Dizon"; VehId = 85; Pickup = "2026-11-01 09:00:00"; Return = "2026-11-05 17:00:00"; Total = 1720.00; Status = "Confirmed"; CompletedAt = $null; Branch = "Cebu IT Park Hub - Lahug"; PkStatus = "Scheduled"; RetStatus = "Scheduled" },

        @{ Customer = "Clarisse Joyce Bernardo"; VehId = 25; Pickup = "2026-10-12 09:00:00"; Return = "2026-10-15 17:00:00"; Total = 1350.00; Status = "Cancelled"; CompletedAt = $null; Branch = "NAIA Terminal 3 Hub - Pasay"; PkStatus = "Cancelled"; RetStatus = "Cancelled" }
    )

    foreach ($res in $reservations) {
        $ins = $conn.CreateCommand()
        $ins.CommandText = "INSERT INTO [Reservations] ([customer_name], [vehicle_id], [pickup_date], [return_date], [total], [status], [completed_at], [created_at]) VALUES (?, ?, ?, ?, ?, ?, ?, ?)"
        $ins.Parameters.Add("cn", [System.Data.OleDb.OleDbType]::VarWChar, 100).Value = $res.Customer
        $ins.Parameters.Add("vid", [System.Data.OleDb.OleDbType]::Integer).Value = [int]$res.VehId
        $ins.Parameters.Add("pud", [System.Data.OleDb.OleDbType]::Date).Value = [DateTime]::Parse($res.Pickup)
        $ins.Parameters.Add("red", [System.Data.OleDb.OleDbType]::Date).Value = [DateTime]::Parse($res.Return)
        $ins.Parameters.Add("tot", [System.Data.OleDb.OleDbType]::Currency).Value = [decimal]$res.Total
        $ins.Parameters.Add("st", [System.Data.OleDb.OleDbType]::VarWChar, 20).Value = $res.Status
        if ($res.CompletedAt -ne $null) {
            $ins.Parameters.Add("ca", [System.Data.OleDb.OleDbType]::Date).Value = [DateTime]::Parse($res.CompletedAt)
        } else {
            $ins.Parameters.Add("ca", [System.Data.OleDb.OleDbType]::Date).Value = [DBNull]::Value
        }
        $ins.Parameters.Add("created", [System.Data.OleDb.OleDbType]::Date).Value = [DateTime]::Parse($res.Pickup).AddDays(-3)
        $ins.ExecuteNonQuery() | Out-Null

        $idCmd = $conn.CreateCommand()
        $idCmd.CommandText = "SELECT @@IDENTITY"
        $resId = [int]$idCmd.ExecuteScalar()

        $dispPk = $conn.CreateCommand()
        $dispPk.CommandText = "INSERT INTO [Dispatches] ([reservation_id], [dispatch_type], [scheduled_at], [branch], [status]) VALUES (?, ?, ?, ?, ?)"
        $dispPk.Parameters.Add("rid", [System.Data.OleDb.OleDbType]::Integer).Value = $resId
        $dispPk.Parameters.Add("dt", [System.Data.OleDb.OleDbType]::VarWChar, 10).Value = "Pickup"
        $dispPk.Parameters.Add("sa", [System.Data.OleDb.OleDbType]::Date).Value = [DateTime]::Parse($res.Pickup)
        $dispPk.Parameters.Add("br", [System.Data.OleDb.OleDbType]::VarWChar, 50).Value = $res.Branch
        $dispPk.Parameters.Add("st", [System.Data.OleDb.OleDbType]::VarWChar, 20).Value = $res.PkStatus
        $dispPk.ExecuteNonQuery() | Out-Null

        $dispRet = $conn.CreateCommand()
        $dispRet.CommandText = "INSERT INTO [Dispatches] ([reservation_id], [dispatch_type], [scheduled_at], [branch], [status]) VALUES (?, ?, ?, ?, ?)"
        $dispRet.Parameters.Add("rid", [System.Data.OleDb.OleDbType]::Integer).Value = $resId
        $dispRet.Parameters.Add("dt", [System.Data.OleDb.OleDbType]::VarWChar, 10).Value = "Return"
        $dispRet.Parameters.Add("sa", [System.Data.OleDb.OleDbType]::Date).Value = [DateTime]::Parse($res.Return)
        $dispRet.Parameters.Add("br", [System.Data.OleDb.OleDbType]::VarWChar, 50).Value = $res.Branch
        $dispRet.Parameters.Add("st", [System.Data.OleDb.OleDbType]::VarWChar, 20).Value = $res.RetStatus
        $dispRet.ExecuteNonQuery() | Out-Null
    }

    $resetVeh = $conn.CreateCommand()
    $resetVeh.CommandText = "UPDATE [Vehicles] SET [status] = 'Available'"
    $resetVeh.ExecuteNonQuery() | Out-Null

    $rentedVehs = @(11, 14, 46, 56, 30, 34, 69)
    foreach ($v in $rentedVehs) {
        $u = $conn.CreateCommand()
        $u.CommandText = "UPDATE [Vehicles] SET [status] = 'Rented' WHERE [vehicle_id] = $v"
        $u.ExecuteNonQuery() | Out-Null
    }

    $reservedVehs = @(5, 16, 59, 85)
    foreach ($v in $reservedVehs) {
        $u = $conn.CreateCommand()
        $u.CommandText = "UPDATE [Vehicles] SET [status] = 'Reserved' WHERE [vehicle_id] = $v"
        $u.ExecuteNonQuery() | Out-Null
    }

    $maintVehs = @(62, 21, 54)
    foreach ($v in $maintVehs) {
        $u = $conn.CreateCommand()
        $u.CommandText = "UPDATE [Vehicles] SET [status] = 'Maintenance' WHERE [vehicle_id] = $v"
        $u.ExecuteNonQuery() | Out-Null
    }

    $inspections = @(
        @{ VehId = 65; Date = "2026-10-22 16:45:00"; Inspector = "Jerome Lumbao"; Fuel = "Full"; Findings = "Post-rental inspection: exterior pristine, tire tread depth 6mm, transmission smooth. Ready for next dispatch."; Result = "Passed" },
        @{ VehId = 11; Date = "2026-10-25 08:30:00"; Inspector = "Jerome Lumbao"; Fuel = "Full"; Findings = "Pre-dispatch check passed. Exterior spotless, tire pressure at 32 PSI, cabin sanitized."; Result = "Passed" },
        @{ VehId = 14; Date = "2026-10-24 08:45:00"; Inspector = "Princess Santander"; Fuel = "Full"; Findings = "All safety checks passed. Clean interior, fluid levels normal, spare tire & tools verified."; Result = "Passed" },
        @{ VehId = 46; Date = "2026-10-26 08:20:00"; Inspector = "Andrei Balboa"; Fuel = "Full"; Findings = "Pre-release inspection passed. All 6 safety items verified, AC cooling properly."; Result = "Passed" },
        @{ VehId = 56; Date = "2026-10-25 08:15:00"; Inspector = "Jonathan Balboa"; Fuel = "Full"; Findings = "Bed liner clean, cargo tie-downs inspected, all exterior lights functioning."; Result = "Passed" },
        @{ VehId = 30; Date = "2026-10-26 08:40:00"; Inspector = "Liam Serrano"; Fuel = "Full"; Findings = "All checks passed. City hatchback prepped, tires inspected, wipers functional."; Result = "Passed" },
        @{ VehId = 34; Date = "2026-10-27 08:10:00"; Inspector = "Zake Yurtas"; Fuel = "Full"; Findings = "14-seater cabin vacuumed, emergency exit clear, fire extinguisher verified."; Result = "Passed" },
        @{ VehId = 69; Date = "2026-10-26 09:30:00"; Inspector = "Adrielle Bino"; Fuel = "Full"; Findings = "High-performance vehicle inspection: tire pressures optimal, brakes tested, vehicle spotless."; Result = "Passed" },
        @{ VehId = 13; Date = "2026-10-11 16:20:00"; Inspector = "Princess Santander"; Fuel = "Full"; Findings = "Return inspection complete. Minor dirt on floor mats cleaned, roadworthy."; Result = "Passed" },
        @{ VehId = 6;  Date = "2026-10-14 16:50:00"; Inspector = "Allan Oiga"; Fuel = "Full"; Findings = "Post-trip inspection: clean cabin, brake response sharp, fluids checked."; Result = "Passed" },
        @{ VehId = 76; Date = "2026-10-18 15:30:00"; Inspector = "Liam Serrano"; Fuel = "Full"; Findings = "Executive handover check: luxury package intact, no dents, diagnostics normal."; Result = "Passed" },
        @{ VehId = 51; Date = "2026-10-20 17:40:00"; Inspector = "Andrei Balboa"; Fuel = "Full"; Findings = "Family MPV return inspection: child seat anchors intact, all lights working."; Result = "Passed" },
        @{ VehId = 100; Date = "2026-10-22 16:35:00"; Inspector = "Jonathan Balboa"; Fuel = "Full"; Findings = "EV battery at 92%, charging cable intact, tires inspected, glass roof clean."; Result = "Passed" },
        @{ VehId = 62; Date = "2026-10-26 14:00:00"; Inspector = "Jerome Lumbao"; Fuel = "1/2"; Findings = "Defects: Brakes, Fluids | Front brake pad sensor warning, low brake reservoir fluid."; Result = "Needs Attention" },
        @{ VehId = 21; Date = "2026-10-27 10:30:00"; Inspector = "Princess Santander"; Fuel = "3/4"; Findings = "Defects: Tires | Slow leak on rear right tire, nail embedded in outer tread block."; Result = "Needs Attention" },
        @{ VehId = 54; Date = "2026-10-27 11:15:00"; Inspector = "Adrielle Bino"; Fuel = "1/2"; Findings = "Defects: Lights | Left projector headlamp assembly cracked, condensation noted."; Result = "Needs Attention" }
    )

    foreach ($insp in $inspections) {
        $ins = $conn.CreateCommand()
        $ins.CommandText = "INSERT INTO [Inspections] ([vehicle_id], [inspected_at], [inspector], [fuel], [findings], [inspection_result]) VALUES (?, ?, ?, ?, ?, ?)"
        $ins.Parameters.Add("vid", [System.Data.OleDb.OleDbType]::Integer).Value = [int]$insp.VehId
        $ins.Parameters.Add("ia", [System.Data.OleDb.OleDbType]::Date).Value = [DateTime]::Parse($insp.Date)
        $ins.Parameters.Add("insp", [System.Data.OleDb.OleDbType]::VarWChar, 100).Value = $insp.Inspector
        $ins.Parameters.Add("fl", [System.Data.OleDb.OleDbType]::VarWChar, 20).Value = $insp.Fuel
        $ins.Parameters.Add("find", [System.Data.OleDb.OleDbType]::LongVarWChar).Value = $insp.Findings
        $ins.Parameters.Add("res", [System.Data.OleDb.OleDbType]::VarWChar, 20).Value = $insp.Result
        $ins.ExecuteNonQuery() | Out-Null
    }

    $maintenance = @(
        @{ VehId = 65; Type = "Executive Sports Car PMS & Brake Inspection"; Date = "2026-10-12 10:00:00"; Cost = 6800.00; Status = "Completed"; Notes = "BMW AutoHaus BGC - High-performance synthetic oil change and multi-point safety inspection."; Comp = "2026-10-12 14:30:00" },
        @{ VehId = 11; Type = "10,000 km PMS & Synthetic Oil Change"; Date = "2026-10-08 09:00:00"; Cost = 3500.00; Status = "Completed"; Notes = "Toyota Manila Bay Service Center - OEM oil filter and fully synthetic motor oil replaced. Road tested."; Comp = "2026-10-08 15:30:00" },
        @{ VehId = 14; Type = "Front & Rear Brake Pad Replacement"; Date = "2026-10-14 10:00:00"; Cost = 4800.00; Status = "Completed"; Notes = "Bendix heavy duty ceramic brake pads installed at BGC Quick-Stop. Rotor discs inspected."; Comp = "2026-10-14 14:20:00" },
        @{ VehId = 6;  Type = "Tire Rotation & High-Speed Wheel Alignment"; Date = "2026-10-19 09:30:00"; Cost = 1650.00; Status = "Completed"; Notes = "Yokohama Tire Center Makati - all 4 wheels balanced and aligned to OEM specs."; Comp = "2026-10-19 12:00:00" },
        @{ VehId = 34; Type = "Battery Replacement (Motolite Gold 12V)"; Date = "2026-10-21 11:00:00"; Cost = 5400.00; Status = "Completed"; Notes = "Replaced with brand new Motolite Gold maintenance-free battery with 24-month warranty card."; Comp = "2026-10-21 13:15:00" },
        @{ VehId = 62; Type = "Brake System Bleeding & Sensor Replacement"; Date = "2026-10-27 09:00:00"; Cost = 7500.00; Status = "In Progress"; Notes = "Porsche Center Philippines - Brembo DOT 5.1 fluid flush and electronic pad wear sensor replacement."; Comp = $null },
        @{ VehId = 21; Type = "Tire Patch & Valve Stem Overhaul"; Date = "2026-10-27 13:00:00"; Cost = 850.00; Status = "In Progress"; Notes = "Shell AutoServ Makati - mushroom patch repair on rear right tire, leak test passed."; Comp = $null },
        @{ VehId = 54; Type = "Headlight Assembly Replacement"; Date = "2026-10-29 09:00:00"; Cost = 6200.00; Status = "Scheduled"; Notes = "Isuzu Pasig - OEM bi-LED headlamp housing scheduled for warranty replacement."; Comp = $null }
    )

    foreach ($m in $maintenance) {
        $ins = $conn.CreateCommand()
        $ins.CommandText = "INSERT INTO [Maintenance] ([vehicle_id], [service_type], [scheduled_date], [cost], [status], [notes], [completed_at], [created_at]) VALUES (?, ?, ?, ?, ?, ?, ?, ?)"
        $ins.Parameters.Add("vid", [System.Data.OleDb.OleDbType]::Integer).Value = [int]$m.VehId
        $ins.Parameters.Add("st", [System.Data.OleDb.OleDbType]::VarWChar, 60).Value = $m.Type
        $ins.Parameters.Add("sd", [System.Data.OleDb.OleDbType]::Date).Value = [DateTime]::Parse($m.Date)
        $ins.Parameters.Add("co", [System.Data.OleDb.OleDbType]::Currency).Value = [decimal]$m.Cost
        $ins.Parameters.Add("sta", [System.Data.OleDb.OleDbType]::VarWChar, 20).Value = $m.Status
        $ins.Parameters.Add("no", [System.Data.OleDb.OleDbType]::LongVarWChar).Value = $m.Notes
        if ($m.Comp -ne $null) {
            $ins.Parameters.Add("ca", [System.Data.OleDb.OleDbType]::Date).Value = [DateTime]::Parse($m.Comp)
        } else {
            $ins.Parameters.Add("ca", [System.Data.OleDb.OleDbType]::Date).Value = [DBNull]::Value
        }
        $ins.Parameters.Add("created", [System.Data.OleDb.OleDbType]::Date).Value = [DateTime]::Parse($m.Date).AddDays(-3)
        $ins.ExecuteNonQuery() | Out-Null
    }

    Write-Host "Successfully synchronized $db"
    $conn.Close()
}
