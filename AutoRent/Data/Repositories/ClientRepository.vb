Imports System.Data
Imports System.Data.OleDb
Imports AutoRent.Data.Context

Namespace Data.Repositories
    ''' <summary>Data access for the Client Portal (fleet board, customers, reservations, dispatch, inspections).</summary>
    Public NotInheritable Class ClientRepository
        Private Sub New()
        End Sub

        ' ---------- Fleet ----------

        Public Shared Function GetFleet() As DataTable
            Return Db.Query(
                "SELECT v.[vehicle_id], v.[plate], v.[make], v.[model], v.[model_year], v.[category], v.[status], r.[daily_rate] " &
                "FROM [Vehicles] AS v LEFT JOIN [Rates] AS r ON v.[category] = r.[category] " &
                "WHERE v.[status] <> 'Retired' ORDER BY v.[plate]")
        End Function

        ' ---------- Customers ----------

        Public Shared Function GetCustomers() As DataTable
            Return Db.Query("SELECT [customer_id], [full_name], [phone], [email], [license_no] FROM [Customers] ORDER BY [full_name]")
        End Function

        Public Shared Function AddCustomer(name As String, phone As String, email As String, license As String) As String
            If Db.Count("SELECT COUNT(*) FROM [Customers] WHERE [full_name] = ? AND [phone] = ?", Db.Text(name, 100), Db.Text(phone, 30)) > 0 Then
                Return "A customer with the same name and phone already exists."
            End If

            Db.Execute("INSERT INTO [Customers] ([full_name], [phone], [email], [license_no], [created_at]) VALUES (?, ?, ?, ?, ?)",
                       Db.Text(name, 100), Db.Text(phone, 30), Db.Text(email, 100), Db.Text(license, 50), Db.Dt(DateTime.Now))
            Return Nothing
        End Function

        Public Shared Function DeleteCustomer(customerId As Integer, customerName As String) As String
            If Db.Count("SELECT COUNT(*) FROM [Reservations] WHERE [customer_name] = ? AND ([status] = 'Confirmed' OR [status] = 'Active')", Db.Text(customerName, 100)) > 0 Then
                Return $"{customerName} still has an open reservation and cannot be deleted."
            End If

            Db.Execute("DELETE FROM [Customers] WHERE [customer_id] = ?", Db.Int(customerId))
            Return Nothing
        End Function

        ' ---------- Reservations ----------

        Public Shared Function GetReservations() As DataTable
            Return Db.Query(
                "SELECT r.[reservation_id], r.[customer_name], r.[vehicle_id], r.[pickup_date], r.[return_date], r.[total], r.[status], " &
                "v.[plate], v.[make], v.[model], v.[model_year] " &
                "FROM [Reservations] AS r LEFT JOIN [Vehicles] AS v ON r.[vehicle_id] = v.[vehicle_id] " &
                "ORDER BY r.[reservation_id] DESC")
        End Function

        ''' <summary>Creates the reservation, its pickup and return dispatches, and marks the vehicle Reserved, all or nothing.</summary>
        Public Shared Function CreateReservation(customerName As String, vehicleId As Integer, pickup As Date, dropoff As Date, total As Decimal) As String
            Using connection = AccessDatabaseConnection.CreateConnection()
                connection.Open()
                Using tx = connection.BeginTransaction()
                    If Scalar(connection, tx, "SELECT COUNT(*) FROM [Vehicles] WHERE [vehicle_id] = ? AND [status] = 'Available'", Db.Int(vehicleId)) = 0 Then
                        tx.Rollback()
                        Return "That vehicle is no longer available."
                    End If

                    Run(connection, tx, "INSERT INTO [Reservations] ([customer_name], [vehicle_id], [pickup_date], [return_date], [total], [status], [created_at]) VALUES (?, ?, ?, ?, ?, ?, ?)",
                        Db.Text(customerName, 100), Db.Int(vehicleId), Db.Dt(pickup), Db.Dt(dropoff), Db.Money(total), Db.Text("Confirmed", 20), Db.Dt(DateTime.Now))

                    Dim reservationId As Integer
                    Using idCommand = connection.CreateCommand()
                        idCommand.Transaction = tx
                        idCommand.CommandText = "SELECT @@IDENTITY"
                        reservationId = Convert.ToInt32(idCommand.ExecuteScalar())
                    End Using

                    Const insertDispatch As String = "INSERT INTO [Dispatches] ([reservation_id], [dispatch_type], [scheduled_at], [branch], [status]) VALUES (?, ?, ?, ?, ?)"
                    Run(connection, tx, insertDispatch, Db.Int(reservationId), Db.Text("Pickup", 10), Db.Dt(pickup.Date.AddHours(9)), Db.Text("Main Branch", 50), Db.Text("Scheduled", 20))
                    Run(connection, tx, insertDispatch, Db.Int(reservationId), Db.Text("Return", 10), Db.Dt(dropoff.Date.AddHours(17)), Db.Text("Main Branch", 50), Db.Text("Scheduled", 20))

                    Run(connection, tx, "UPDATE [Vehicles] SET [status] = 'Reserved' WHERE [vehicle_id] = ?", Db.Int(vehicleId))
                    tx.Commit()
                End Using
            End Using
            Return Nothing
        End Function

        ''' <summary>Sets a reservation to Cancelled or Completed, frees the vehicle and closes any open dispatches.</summary>
        Public Shared Sub CloseReservation(reservationId As Integer, vehicleId As Integer, newStatus As String)
            Dim dispatchStatus = If(newStatus = "Completed", "Completed", "Cancelled")

            Using connection = AccessDatabaseConnection.CreateConnection()
                connection.Open()
                Using tx = connection.BeginTransaction()
                    Run(connection, tx, "UPDATE [Reservations] SET [status] = ? WHERE [reservation_id] = ?", Db.Text(newStatus, 20), Db.Int(reservationId))
                    Run(connection, tx, "UPDATE [Vehicles] SET [status] = 'Available' WHERE [vehicle_id] = ? AND ([status] = 'Reserved' OR [status] = 'Rented')", Db.Int(vehicleId))
                    Run(connection, tx, "UPDATE [Dispatches] SET [status] = ? WHERE [reservation_id] = ? AND [status] = 'Scheduled'", Db.Text(dispatchStatus, 20), Db.Int(reservationId))
                    tx.Commit()
                End Using
            End Using
        End Sub

        ' ---------- Dispatch ----------

        ''' <summary>Today's dispatches plus anything from earlier days that is still scheduled.</summary>
        Public Shared Function GetDispatchBoard() As DataTable
            Dim today = DateTime.Today
            Return Db.Query(
                "SELECT d.[dispatch_id], d.[reservation_id], d.[dispatch_type], d.[scheduled_at], d.[branch], d.[status], " &
                "r.[customer_name], r.[vehicle_id], v.[plate], v.[make], v.[model], v.[model_year] " &
                "FROM ([Dispatches] AS d LEFT JOIN [Reservations] AS r ON d.[reservation_id] = r.[reservation_id]) " &
                "LEFT JOIN [Vehicles] AS v ON r.[vehicle_id] = v.[vehicle_id] " &
                "WHERE d.[status] <> 'Cancelled' AND d.[scheduled_at] < ? AND (d.[status] = 'Scheduled' OR d.[scheduled_at] >= ?) " &
                "ORDER BY d.[scheduled_at]",
                Db.Dt(today.AddDays(1)), Db.Dt(today))
        End Function

        ''' <summary>Pickup: reservation becomes Active and the vehicle Rented. Return: reservation Completed and the vehicle Available.</summary>
        Public Shared Sub CompleteDispatch(dispatchId As Integer, reservationId As Integer, vehicleId As Integer, dispatchType As String)
            Using connection = AccessDatabaseConnection.CreateConnection()
                connection.Open()
                Using tx = connection.BeginTransaction()
                    Run(connection, tx, "UPDATE [Dispatches] SET [status] = 'Completed' WHERE [dispatch_id] = ?", Db.Int(dispatchId))

                    If dispatchType = "Pickup" Then
                        Run(connection, tx, "UPDATE [Reservations] SET [status] = 'Active' WHERE [reservation_id] = ?", Db.Int(reservationId))
                        Run(connection, tx, "UPDATE [Vehicles] SET [status] = 'Rented' WHERE [vehicle_id] = ?", Db.Int(vehicleId))
                    Else
                        Run(connection, tx, "UPDATE [Reservations] SET [status] = 'Completed' WHERE [reservation_id] = ?", Db.Int(reservationId))
                        Run(connection, tx, "UPDATE [Vehicles] SET [status] = 'Available' WHERE [vehicle_id] = ? AND [status] <> 'Maintenance'", Db.Int(vehicleId))
                    End If
                    tx.Commit()
                End Using
            End Using
        End Sub

        ' ---------- Inspections ----------

        Public Shared Function GetInspections() As DataTable
            Return Db.Query(
                "SELECT TOP 200 i.[inspected_at], i.[inspector], i.[fuel], i.[findings], i.[inspection_result], v.[plate], v.[make], v.[model], v.[model_year] " &
                "FROM [Inspections] AS i LEFT JOIN [Vehicles] AS v ON i.[vehicle_id] = v.[vehicle_id] " &
                "ORDER BY i.[inspected_at] DESC")
        End Function

        Public Shared Sub AddInspection(vehicleId As Integer, inspector As String, fuel As String, findings As String, passed As Boolean)
            Using connection = AccessDatabaseConnection.CreateConnection()
                connection.Open()
                Using tx = connection.BeginTransaction()
                    Run(connection, tx, "INSERT INTO [Inspections] ([vehicle_id], [inspected_at], [inspector], [fuel], [findings], [inspection_result]) VALUES (?, ?, ?, ?, ?, ?)",
                        Db.Int(vehicleId), Db.Dt(DateTime.Now), Db.Text(inspector, 100), Db.Text(fuel, 20), Db.Memo(findings), Db.Text(If(passed, "Pass", "Needs Attention"), 20))
                    If Not passed Then
                        Run(connection, tx, "UPDATE [Vehicles] SET [status] = 'Maintenance' WHERE [vehicle_id] = ?", Db.Int(vehicleId))
                    End If
                    tx.Commit()
                End Using
            End Using
        End Sub

        ' ---------- helpers ----------

        Private Shared Sub Run(connection As OleDbConnection, tx As OleDbTransaction, sql As String, ParamArray args() As OleDbParameter)
            Using command = connection.CreateCommand()
                command.Transaction = tx
                command.CommandText = sql
                If args IsNot Nothing AndAlso args.Length > 0 Then command.Parameters.AddRange(args)
                command.ExecuteNonQuery()
            End Using
        End Sub

        Private Shared Function Scalar(connection As OleDbConnection, tx As OleDbTransaction, sql As String, ParamArray args() As OleDbParameter) As Integer
            Using command = connection.CreateCommand()
                command.Transaction = tx
                command.CommandText = sql
                If args IsNot Nothing AndAlso args.Length > 0 Then command.Parameters.AddRange(args)
                Dim result = command.ExecuteScalar()
                If result Is Nothing OrElse IsDBNull(result) Then Return 0
                Return Convert.ToInt32(result)
            End Using
        End Function
    End Class
End Namespace
