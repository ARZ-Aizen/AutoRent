Imports System.Data.OleDb

Namespace Data.Context
    Public NotInheritable Class DatabaseInitializer
        Private Sub New()
        End Sub

        Public Shared Sub EnsureSchema()
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
                        "[inspected_at] DATETIME, " &
                        "[inspector] VARCHAR(100), " &
                        "[fuel] VARCHAR(20), " &
                        "[findings] MEMO, " &
                        "[inspection_result] VARCHAR(20))")
                End If

                Try
                    If Not ColumnExists(connection, "Users", "full_name") Then
                        Run(connection, "ALTER TABLE [Users] ADD COLUMN [full_name] VARCHAR(100)")
                    End If
                    If Not ColumnExists(connection, "Users", "branch") Then
                        Run(connection, "ALTER TABLE [Users] ADD COLUMN [branch] VARCHAR(50)")
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
    End Class
End Namespace
