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
                        "[category] VARCHAR(30) NOT NULL, " &
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
                        "[category] VARCHAR(30), " &
                        "[model_year] LONG, " &
                        "[mileage] LONG, " &
                        "[status] VARCHAR(20), " &
                        "[created_at] DATETIME)")
                    Run(connection, "CREATE UNIQUE INDEX [UX_Vehicles_Plate] ON [Vehicles] ([plate])")
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
                        "[created_at] DATETIME)")
                End If

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

        Private Shared Sub Run(connection As OleDbConnection, sql As String)
            Using command = connection.CreateCommand()
                command.CommandText = sql
                command.ExecuteNonQuery()
            End Using
        End Sub

        Private Shared Sub SeedRates(connection As OleDbConnection)
            Dim defaults As Object()() = {
                New Object() {"Economy", 40D, 240D, 900D, 150D, 15D},
                New Object() {"Compact", 50D, 300D, 1100D, 200D, 20D},
                New Object() {"SUV", 80D, 480D, 1800D, 300D, 30D},
                New Object() {"Van", 95D, 570D, 2100D, 350D, 35D},
                New Object() {"Luxury", 150D, 900D, 3300D, 600D, 60D}
            }

            For Each row In defaults
                Using command = connection.CreateCommand()
                    command.CommandText = "INSERT INTO [Rates] ([category], [daily_rate], [weekly_rate], [monthly_rate], [deposit], [late_fee_per_day], [updated_at]) VALUES (?, ?, ?, ?, ?, ?, ?)"
                    command.Parameters.Add("category", OleDbType.VarWChar, 30).Value = CStr(row(0))
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
