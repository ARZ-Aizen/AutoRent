Imports System.Data
Imports AutoRent.Common.Utilities
Imports AutoRent.Data.Context

Namespace Data.Repositories
    Public NotInheritable Class MaintenanceRepository
        Private Sub New()
        End Sub

        Public Shared ReadOnly Statuses As String() = {"Scheduled", "In Progress", "Completed", "Cancelled"}

        Public Shared Function GetAll() As DataTable
            Return Db.Query(
                "SELECT m.[maintenance_id], m.[vehicle_id], v.[plate] & ' - ' & v.[make] & ' ' & v.[model] AS [vehicle], " &
                "m.[service_type], m.[scheduled_date], m.[cost], m.[status], m.[notes], m.[completed_at] " &
                "FROM [Maintenance] AS m LEFT JOIN [Vehicles] AS v ON m.[vehicle_id] = v.[vehicle_id] " &
                "ORDER BY m.[scheduled_date] DESC")
        End Function

        Public Shared Function GetUpcoming(maxRows As Integer) As DataTable
            Return Db.Query(
                $"SELECT TOP {maxRows} v.[plate] & ' - ' & v.[make] & ' ' & v.[model] AS [vehicle], m.[service_type], m.[scheduled_date], m.[status] " &
                "FROM [Maintenance] AS m LEFT JOIN [Vehicles] AS v ON m.[vehicle_id] = v.[vehicle_id] " &
                "WHERE m.[status] IN ('Scheduled', 'In Progress') ORDER BY m.[scheduled_date]")
        End Function

        Public Shared Function Add(vehicleId As Integer, serviceType As String, scheduledDate As DateTime,
                                   cost As Decimal, status As String, notes As String) As String
            Dim issue = ValidateMaintenance(vehicleId, serviceType, cost, status)
            If issue IsNot Nothing Then Return issue
            Dim completedAt As DateTime? = If(status = "Completed", DateTime.Now, CType(Nothing, DateTime?))

            Db.Execute("INSERT INTO [Maintenance] ([vehicle_id], [service_type], [scheduled_date], [cost], [status], [notes], [completed_at], [created_at]) VALUES (?, ?, ?, ?, ?, ?, ?, ?)",
                       Db.Int(vehicleId), Db.Text(serviceType, 60), Db.Dt(scheduledDate), Db.Money(cost), Db.Text(status, 20),
                       Db.Memo(notes), Db.DtOrNull(completedAt), Db.Dt(DateTime.Now))
            SyncVehicleStatus(vehicleId)
            Return Nothing
        End Function

        Public Shared Function Update(maintenanceId As Integer, vehicleId As Integer, serviceType As String, scheduledDate As DateTime,
                                      cost As Decimal, status As String, notes As String) As String
            Dim issue = ValidateMaintenance(vehicleId, serviceType, cost, status)
            If issue IsNot Nothing Then Return issue
            Dim previous = Db.Query("SELECT [vehicle_id], [completed_at] FROM [Maintenance] WHERE [maintenance_id] = ?", Db.Int(maintenanceId))
            If previous.Rows.Count = 0 Then Return "That maintenance record no longer exists."

            Dim oldVehicleId = Convert.ToInt32(previous.Rows(0)("vehicle_id"))
            Dim completedAt As DateTime? = Nothing
            If status = "Completed" Then
                completedAt = If(IsDBNull(previous.Rows(0)("completed_at")), DateTime.Now, Convert.ToDateTime(previous.Rows(0)("completed_at")))
            End If

            Db.Execute("UPDATE [Maintenance] SET [vehicle_id] = ?, [service_type] = ?, [scheduled_date] = ?, [cost] = ?, [status] = ?, [notes] = ?, [completed_at] = ? WHERE [maintenance_id] = ?",
                       Db.Int(vehicleId), Db.Text(serviceType, 60), Db.Dt(scheduledDate), Db.Money(cost), Db.Text(status, 20),
                       Db.Memo(notes), Db.DtOrNull(completedAt), Db.Int(maintenanceId))

            SyncVehicleStatus(vehicleId)
            If oldVehicleId <> vehicleId Then SyncVehicleStatus(oldVehicleId)
            Return Nothing
        End Function

        Public Shared Function Delete(maintenanceId As Integer) As String
            Dim previous = Db.Query("SELECT [vehicle_id] FROM [Maintenance] WHERE [maintenance_id] = ?", Db.Int(maintenanceId))
            Db.Execute("DELETE FROM [Maintenance] WHERE [maintenance_id] = ?", Db.Int(maintenanceId))
            If previous.Rows.Count > 0 Then SyncVehicleStatus(Convert.ToInt32(previous.Rows(0)("vehicle_id")))
            Return Nothing
        End Function

        Private Shared Function ValidateMaintenance(vehicleId As Integer, serviceType As String,
                                                    cost As Decimal, status As String) As String
            If vehicleId <= 0 Then Return "Select a vehicle."
            If String.IsNullOrWhiteSpace(serviceType) OrElse serviceType.Trim().Length > 60 Then
                Return "Maintenance type is required and must be 60 characters or fewer."
            End If
            If cost < 0D OrElse cost > 100000000D Then Return "Maintenance cost must be a nonnegative amount."
            If Array.IndexOf(Statuses, status) < 0 Then Return "Choose a valid maintenance status."
            If Db.Count("SELECT COUNT(*) FROM [Vehicles] WHERE [vehicle_id] = ? AND [status] <> 'Retired'", Db.Int(vehicleId)) = 0 Then
                Return "This vehicle does not exist or is retired."
            End If
            ' Don't mark a vehicle in progress while someone is renting it.
            If status = "In Progress" AndAlso Db.Count(
                "SELECT COUNT(*) FROM [Reservations] WHERE [vehicle_id] = ? AND [status] = 'Active'", Db.Int(vehicleId)) > 0 Then
                Return "This vehicle is currently rented. Complete its rental before starting maintenance."
            End If
            Return Nothing
        End Function

        Private Shared Sub SyncVehicleStatus(vehicleId As Integer)
            Dim openJobs = Db.Count("SELECT COUNT(*) FROM [Maintenance] WHERE [vehicle_id] = ? AND [status] = 'In Progress'", Db.Int(vehicleId))

            If openJobs > 0 Then
                Db.Execute("UPDATE [Vehicles] SET [status] = 'Maintenance' WHERE [vehicle_id] = ? AND [status] <> 'Retired'", Db.Int(vehicleId))
            Else
                Dim hasActive = Db.Count("SELECT COUNT(*) FROM [Reservations] WHERE [vehicle_id] = ? AND [status] = 'Active'", Db.Int(vehicleId)) > 0
                Dim hasConfirmed = Db.Count("SELECT COUNT(*) FROM [Reservations] WHERE [vehicle_id] = ? AND [status] = 'Confirmed'", Db.Int(vehicleId)) > 0
                Dim restoredStatus = If(hasActive, "Rented", If(hasConfirmed, "Reserved", "Available"))
                Db.Execute("UPDATE [Vehicles] SET [status] = ? WHERE [vehicle_id] = ? AND [status] = 'Maintenance'",
                           Db.Text(restoredStatus, 20), Db.Int(vehicleId))
            End If
        End Sub
    End Class
End Namespace
