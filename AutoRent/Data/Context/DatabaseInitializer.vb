Imports System.Data.OleDb
Imports System.IO

Namespace Data.Context
    Public NotInheritable Class DatabaseInitializer
        Private Sub New()
        End Sub

        Public Shared Sub EnsureSchema()
            ' Preserve the original Access database once, before removing the
            ' discontinued Inspections table. Never overwrite an older backup.
            Dim databasePath As String = AccessDatabaseConnection.GetDatabasePath()
            Dim retiredBackupPath As String = Path.Combine(Path.GetDirectoryName(databasePath), "AutoRent_Backup_Before_Inspection_Removal.accdb")
            If Not File.Exists(retiredBackupPath) Then
                File.Copy(databasePath, retiredBackupPath, False)
            End If

            Using connection = AccessDatabaseConnection.CreateConnection()
                connection.Open()

                If Not TableExists(connection, "Rates") Then
                    Run(connection, "CREATE TABLE [Rates] (" &
                        "[rate_id] COUNTER PRIMARY KEY, " &
                        "[category] VARCHAR(50) NOT NULL, " &
                        "[daily_rate] CURRENCY, " &
                        "[weekly_rate] CURRENCY, " &
                        "[monthly_rate] CURRENCY, " &
                        "[deposit] CURRENCY, " &
                        "[late_fee_per_day] CURRENCY, " &
                        "[updated_at] DATETIME)")
                    Run(connection, "CREATE UNIQUE INDEX [UX_Rates_Category] ON [Rates] ([category])")
                    SeedRates(connection)
                End If

                If Not TableExists(connection, "Vehicles") Then
                    Run(connection, "CREATE TABLE [Vehicles] (" &
                        "[vehicle_id] COUNTER PRIMARY KEY, " &
                        "[plate] VARCHAR(20) NOT NULL, " &
                        "[make] VARCHAR(50), " &
                        "[model] VARCHAR(50), " &
                        "[category] VARCHAR(50), " &
                        "[model_year] LONG, " &
                        "[mileage] LONG, " &
                        "[status] VARCHAR(20), " &
                        "[image_path] VARCHAR(255), " &
                        "[created_at] DATETIME)")
                    Run(connection, "CREATE UNIQUE INDEX [UX_Vehicles_Plate] ON [Vehicles] ([plate])")
                End If

                ' Upgrade older databases without deleting any vehicle records.
                If Not ColumnExists(connection, "Vehicles", "image_path") Then
                    Run(connection, "ALTER TABLE [Vehicles] ADD COLUMN [image_path] VARCHAR(255)")
                End If

                If Not TableExists(connection, "Maintenance") Then
                    Run(connection, "CREATE TABLE [Maintenance] (" &
                        "[maintenance_id] COUNTER PRIMARY KEY, " &
                        "[vehicle_id] LONG NOT NULL, " &
                        "[service_type] VARCHAR(60), " &
                        "[scheduled_date] DATETIME, " &
                        "[cost] CURRENCY, " &
                        "[status] VARCHAR(20), " &
                        "[notes] MEMO, " &
                        "[completed_at] DATETIME, " &
                        "[created_at] DATETIME)")
                End If

                If Not TableExists(connection, "Reservations") Then
                    Run(connection, "CREATE TABLE [Reservations] (" &
                        "[reservation_id] COUNTER PRIMARY KEY, " &
                        "[customer_name] VARCHAR(100), " &
                        "[vehicle_id] LONG, " &
                        "[pickup_date] DATETIME, " &
                        "[return_date] DATETIME, " &
                        "[total] CURRENCY, " &
                        "[status] VARCHAR(20), " &
                        "[completed_at] DATETIME, " &
                        "[created_at] DATETIME)")
                End If

                ' Preserve existing reservations while adding an actual completion timestamp.
                If Not ColumnExists(connection, "Reservations", "completed_at") Then
                    Run(connection, "ALTER TABLE [Reservations] ADD COLUMN [completed_at] DATETIME")
                End If

                ' Legacy early completions have no recorded completion time. Where a
                ' completed rental's planned return is still in the future, recognize
                ' it when this upgraded version first observes it (an estimate, not
                ' a historical timestamp). Older records retain the return-date
                ' fallback in the reporting queries.
                Using legacy = connection.CreateCommand()
                    legacy.CommandText =
                        "UPDATE [Reservations] SET [completed_at] = ? " &
                        "WHERE [status] = 'Completed' AND [completed_at] IS NULL " &
                        "AND [return_date] >= ?"
                    legacy.Parameters.Add("completedAt", OleDbType.Date).Value = DateTime.Now
                    legacy.Parameters.Add("tomorrow", OleDbType.Date).Value = DateTime.Today.AddDays(1)
                    legacy.ExecuteNonQuery()
                End Using

                If Not TableExists(connection, "Customers") Then
                    Run(connection, "CREATE TABLE [Customers] (" &
                        "[customer_id] COUNTER PRIMARY KEY, " &
                        "[full_name] VARCHAR(100) NOT NULL, " &
                        "[phone] VARCHAR(30), " &
                        "[email] VARCHAR(100), " &
                        "[license_no] VARCHAR(50), " &
                        "[created_at] DATETIME)")
                End If

                If Not TableExists(connection, "Dispatches") Then
                    Run(connection, "CREATE TABLE [Dispatches] (" &
                        "[dispatch_id] COUNTER PRIMARY KEY, " &
                        "[reservation_id] LONG NOT NULL, " &
                        "[dispatch_type] VARCHAR(10), " &
                        "[scheduled_at] DATETIME, " &
                        "[branch] VARCHAR(50), " &
                        "[status] VARCHAR(20))")
                End If
                If Not TableExists(connection, "Inspections") Then
                    Run(connection, "CREATE TABLE [Inspections] (" &
                        "[inspection_id] COUNTER PRIMARY KEY, " &
                        "[vehicle_id] LONG NOT NULL, " &
                        "[checklist_type] VARCHAR(30), " &
                        "[inspected_at] DATETIME, " &
                        "[inspector] VARCHAR(100), " &
                        "[odometer] LONG, " &
                        "[fuel] VARCHAR(20), " &
                        "[exterior_ok] YESNO, " &
                        "[tires_ok] YESNO, " &
                        "[lights_ok] YESNO, " &
                        "[interior_ok] YESNO, " &
                        "[fluids_ok] YESNO, " &
                        "[brakes_ok] YESNO, " &
                        "[findings] MEMO, " &
                        "[inspection_result] VARCHAR(20), " &
                        "[created_at] DATETIME)")
                    SeedInspections(connection)
                Else
                    ' Ensure enhanced checklist columns exist
                    Try
                        If Not ColumnExists(connection, "Inspections", "checklist_type") Then
                            Run(connection, "ALTER TABLE [Inspections] ADD COLUMN [checklist_type] VARCHAR(30)")
                        End If
                        If Not ColumnExists(connection, "Inspections", "odometer") Then
                            Run(connection, "ALTER TABLE [Inspections] ADD COLUMN [odometer] LONG")
                        End If
                        If Not ColumnExists(connection, "Inspections", "exterior_ok") Then
                            Run(connection, "ALTER TABLE [Inspections] ADD COLUMN [exterior_ok] YESNO")
                        End If
                        If Not ColumnExists(connection, "Inspections", "tires_ok") Then
                            Run(connection, "ALTER TABLE [Inspections] ADD COLUMN [tires_ok] YESNO")
                        End If
                        If Not ColumnExists(connection, "Inspections", "lights_ok") Then
                            Run(connection, "ALTER TABLE [Inspections] ADD COLUMN [lights_ok] YESNO")
                        End If
                        If Not ColumnExists(connection, "Inspections", "interior_ok") Then
                            Run(connection, "ALTER TABLE [Inspections] ADD COLUMN [interior_ok] YESNO")
                        End If
                        If Not ColumnExists(connection, "Inspections", "fluids_ok") Then
                            Run(connection, "ALTER TABLE [Inspections] ADD COLUMN [fluids_ok] YESNO")
                        End If
                        If Not ColumnExists(connection, "Inspections", "brakes_ok") Then
                            Run(connection, "ALTER TABLE [Inspections] ADD COLUMN [brakes_ok] YESNO")
                        End If
                        If Not ColumnExists(connection, "Inspections", "created_at") Then
                            Run(connection, "ALTER TABLE [Inspections] ADD COLUMN [created_at] DATETIME")
                        End If
                    Catch
                    End Try

                    ' Seed if table exists but is empty
                    Using chkCmd = connection.CreateCommand()
                        chkCmd.CommandText = "SELECT COUNT(*) FROM [Inspections]"
                        Dim cnt As Integer = Convert.ToInt32(chkCmd.ExecuteScalar())
                        If cnt = 0 Then SeedInspections(connection)
                    End Using
                End If

                ' Normalize legacy inspector names in existing records to match official staff roster
                Try
                    Run(connection, "UPDATE [Inspections] SET [inspector] = 'Adrielle Bino' WHERE [inspector] IN ('Adrielle Liwag', 'Liwag')")
                    Run(connection, "UPDATE [Inspections] SET [inspector] = 'Zake Yurtas' WHERE [inspector] IN ('Zake Magdaluyo', 'Magdaluyo')")
                    Run(connection, "UPDATE [Inspections] SET [inspector] = 'Jonathan Balboa' WHERE [inspector] IN ('Jonathan Benitez', 'Benitez')")
                    Run(connection, "UPDATE [Inspections] SET [inspector] = 'Andrei Balboa' WHERE [inspector] IN ('Andrei Belando', 'Belando', 'Justine Balboa', 'Justine')")
                Catch
                End Try

                Try
                    If Not ColumnExists(connection, "Users", "full_name") Then
                        Run(connection, "ALTER TABLE [Users] ADD COLUMN [full_name] VARCHAR(100)")
                    End If
                    If Not ColumnExists(connection, "Users", "branch") Then
                        Run(connection, "ALTER TABLE [Users] ADD COLUMN [branch] VARCHAR(50)")
                    End If
                    If Not ColumnExists(connection, "Users", "user_code") Then
                        Run(connection, "ALTER TABLE [Users] ADD COLUMN [user_code] VARCHAR(20)")
                    End If
                Catch
                End Try
            End Using
        End Sub

        Private Shared Function TableExists(connection As OleDbConnection, tableName As String) As Boolean
            Try
                Using command = connection.CreateCommand()
                    command.CommandText = $"SELECT TOP 1 * FROM [{tableName}]"
                    Using reader = command.ExecuteReader()
                        Return True
                    End Using
                End Using
            Catch ex As OleDbException
                Return False
            End Try
        End Function

        Private Shared Function ColumnExists(connection As OleDbConnection, tableName As String, columnName As String) As Boolean
            Try
                Dim schema = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, New Object() {Nothing, Nothing, tableName, columnName})
                Return schema IsNot Nothing AndAlso schema.Rows.Count > 0
            Catch ex As Exception
                Return False
            End Try
        End Function

        Private Shared Sub Run(connection As OleDbConnection, sql As String)
            Using command = connection.CreateCommand()
                command.CommandText = sql
                command.ExecuteNonQuery()
            End Using
        End Sub

        Private Shared Sub SeedRates(connection As OleDbConnection)
            Dim defaults As Object()() = {
                New Object() {"Sedan", 450D, 2700D, 10800D, 500D, 50D},
                New Object() {"SUV (Sport Utility Vehicle)", 480D, 2880D, 11520D, 500D, 50D},
                New Object() {"Hatchback", 380D, 2280D, 9120D, 400D, 40D},
                New Object() {"Van", 350D, 2100D, 8400D, 450D, 45D},
                New Object() {"MPV (Multi-Purpose Vehicle)", 360D, 2160D, 8640D, 450D, 45D},
                New Object() {"Pickup Truck", 420D, 2520D, 10080D, 450D, 45D},
                New Object() {"Sports Car", 650D, 3900D, 15600D, 800D, 80D},
                New Object() {"Luxury Car", 600D, 3600D, 14400D, 750D, 75D},
                New Object() {"Hybrid Car", 430D, 2580D, 10320D, 500D, 50D},
                New Object() {"Electric Car (EV)", 460D, 2760D, 11040D, 500D, 50D}
            }

            For Each row In defaults
                Using command = connection.CreateCommand()
                    command.CommandText = "INSERT INTO [Rates] ([category], [daily_rate], [weekly_rate], [monthly_rate], [deposit], [late_fee_per_day], [updated_at]) VALUES (?, ?, ?, ?, ?, ?, ?)"
                    command.Parameters.Add("category", OleDbType.VarWChar, 50).Value = CStr(row(0))
                    For i = 1 To 5
                        command.Parameters.Add("p" & i, OleDbType.Currency).Value = CDec(row(i))
                    Next
                    command.Parameters.Add("updated_at", OleDbType.Date).Value = DateTime.Now
                    command.ExecuteNonQuery()
                End Using
            Next
        End Sub
        Private Shared Sub SeedInspections(connection As OleDbConnection)
            Dim defaults As Object()() = {
                New Object() {65, "Post-Rental", DateTime.Parse("2026-10-22 16:45:00"), "Jerome Lumbao", 12450L, "Full", True, True, True, True, True, True, "Post-rental inspection: exterior pristine, tire tread depth 6mm, transmission smooth. Ready for next dispatch.", "Passed"},
                New Object() {11, "Pre-Rental", DateTime.Parse("2026-10-25 08:30:00"), "Jerome Lumbao", 28100L, "Full", True, True, True, True, True, True, "Pre-dispatch check passed. Exterior spotless, tire pressure at 32 PSI, cabin sanitized.", "Passed"},
                New Object() {14, "Pre-Rental", DateTime.Parse("2026-10-24 08:45:00"), "Princess Santander", 19500L, "Full", True, True, True, True, True, True, "All safety checks passed. Clean interior, fluid levels normal, spare tire & tools verified.", "Passed"},
                New Object() {46, "Pre-Rental", DateTime.Parse("2026-10-26 08:20:00"), "Andrei Balboa", 15200L, "Full", True, True, True, True, True, True, "Pre-release inspection passed. All 6 safety items verified, AC cooling properly.", "Passed"},
                New Object() {56, "Pre-Rental", DateTime.Parse("2026-10-25 08:15:00"), "Jonathan Balboa", 34800L, "Full", True, True, True, True, True, True, "Bed liner clean, cargo tie-downs inspected, all exterior lights functioning.", "Passed"},
                New Object() {30, "Pre-Rental", DateTime.Parse("2026-10-26 08:40:00"), "Liam Serrano", 22100L, "Full", True, True, True, True, True, True, "All checks passed. City hatchback prepped, tires inspected, wipers functional.", "Passed"},
                New Object() {34, "Pre-Rental", DateTime.Parse("2026-10-27 08:10:00"), "Zake Yurtas", 41200L, "Full", True, True, True, True, True, True, "14-seater cabin vacuumed, emergency exit clear, fire extinguisher verified.", "Passed"},
                New Object() {69, "Pre-Rental", DateTime.Parse("2026-10-26 09:30:00"), "Adrielle Bino", 11300L, "Full", True, True, True, True, True, True, "High-performance vehicle inspection: tire pressures optimal, brakes tested, vehicle spotless.", "Passed"},
                New Object() {13, "Post-Rental", DateTime.Parse("2026-10-11 16:20:00"), "Princess Santander", 26400L, "Full", True, True, True, True, True, True, "Return inspection complete. Minor dirt on floor mats cleaned, roadworthy.", "Passed"},
                New Object() {6, "Post-Rental", DateTime.Parse("2026-10-14 16:50:00"), "Allan Oiga", 31500L, "Full", True, True, True, True, True, True, "Post-trip inspection: clean cabin, brake response sharp, fluids checked.", "Passed"},
                New Object() {76, "Post-Rental", DateTime.Parse("2026-10-18 15:30:00"), "Liam Serrano", 18200L, "Full", True, True, True, True, True, True, "Executive handover check: luxury package intact, no dents, diagnostics normal.", "Passed"},
                New Object() {51, "Post-Rental", DateTime.Parse("2026-10-20 17:40:00"), "Andrei Balboa", 37900L, "Full", True, True, True, True, True, True, "Family MPV return inspection: child seat anchors intact, all lights working.", "Passed"},
                New Object() {100, "Post-Rental", DateTime.Parse("2026-10-22 16:35:00"), "Jonathan Balboa", 9800L, "Full", True, True, True, True, True, True, "EV battery at 92%, charging cable intact, tires inspected, glass roof clean.", "Passed"},
                New Object() {62, "Routine Maintenance", DateTime.Parse("2026-10-26 14:00:00"), "Jerome Lumbao", 16800L, "1/2", True, True, True, True, False, False, "Defects: Brakes, Fluids | Front brake pad sensor warning, low brake reservoir fluid.", "Needs Attention"},
                New Object() {21, "Routine Maintenance", DateTime.Parse("2026-10-27 10:30:00"), "Princess Santander", 48200L, "3/4", True, False, True, True, True, True, "Defects: Tires | Slow leak on rear right tire, nail embedded in outer tread block.", "Needs Attention"},
                New Object() {54, "Routine Maintenance", DateTime.Parse("2026-10-27 11:15:00"), "Adrielle Bino", 29600L, "1/2", True, True, False, True, True, True, "Defects: Lights | Left projector headlamp assembly cracked, condensation noted.", "Needs Attention"}
            }

            For Each row In defaults
                Using command = connection.CreateCommand()
                    command.CommandText = "INSERT INTO [Inspections] ([vehicle_id], [checklist_type], [inspected_at], [inspector], [odometer], [fuel], [exterior_ok], [tires_ok], [lights_ok], [interior_ok], [fluids_ok], [brakes_ok], [findings], [inspection_result], [created_at]) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)"
                    command.Parameters.Add("vid", OleDbType.Integer).Value = Convert.ToInt32(row(0))
                    command.Parameters.Add("ct", OleDbType.VarWChar, 30).Value = CStr(row(1))
                    command.Parameters.Add("ia", OleDbType.Date).Value = CDate(row(2))
                    command.Parameters.Add("insp", OleDbType.VarWChar, 100).Value = CStr(row(3))
                    command.Parameters.Add("odo", OleDbType.Integer).Value = Convert.ToInt32(row(4))
                    command.Parameters.Add("fl", OleDbType.VarWChar, 20).Value = CStr(row(5))
                    command.Parameters.Add("e_ok", OleDbType.Boolean).Value = CBool(row(6))
                    command.Parameters.Add("t_ok", OleDbType.Boolean).Value = CBool(row(7))
                    command.Parameters.Add("l_ok", OleDbType.Boolean).Value = CBool(row(8))
                    command.Parameters.Add("i_ok", OleDbType.Boolean).Value = CBool(row(9))
                    command.Parameters.Add("f_ok", OleDbType.Boolean).Value = CBool(row(10))
                    command.Parameters.Add("b_ok", OleDbType.Boolean).Value = CBool(row(11))
                    command.Parameters.Add("find", OleDbType.LongVarWChar).Value = CStr(row(12))
                    command.Parameters.Add("res", OleDbType.VarWChar, 20).Value = CStr(row(13))
                    command.Parameters.Add("created", OleDbType.Date).Value = CDate(row(2))
                    command.ExecuteNonQuery()
                End Using
            Next
        End Sub
    End Class
End Namespace
