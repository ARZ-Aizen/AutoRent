Imports System.Data
Imports System.Data.OleDb
Imports AutoRent.Data.Context

Namespace Data.Repositories
    ' Stores real before/after status transitions, never fabricated historical changes.
    ' For workflows that already own an OleDb transaction, use the transactional overload.
    Public NotInheritable Class VehicleStatusAuditRepository
        Private Sub New()
        End Sub

        Public Shared Function CurrentStatus(vehicleId As Integer) As String
            Dim value = Db.Scalar("SELECT [status] FROM [Vehicles] WHERE [vehicle_id] = ?", Db.Int(vehicleId))
            Return If(value Is Nothing OrElse IsDBNull(value), "", Convert.ToString(value).Trim())
        End Function

        Public Shared Function CurrentStatus(connection As OleDbConnection, tx As OleDbTransaction, vehicleId As Integer) As String
            Using cmd = connection.CreateCommand()
                cmd.Transaction = tx
                cmd.CommandText = "SELECT [status] FROM [Vehicles] WHERE [vehicle_id] = ?"
                cmd.Parameters.Add(Db.Int(vehicleId))
                Dim value = cmd.ExecuteScalar()
                Return If(value Is Nothing OrElse IsDBNull(value), "", Convert.ToString(value).Trim())
            End Using
        End Function

        Private Shared Function StaffName() As String
            Dim app = TryCast(System.Windows.Application.Current, Global.AutoRent.Application)
            If app IsNot Nothing AndAlso app.CurrentUser IsNot Nothing Then
                Dim userName = app.CurrentUser.DisplayName
                If Not String.IsNullOrWhiteSpace(userName) Then Return userName.Trim()
            End If
            Return "System (user not recorded)"
        End Function

        Public Shared Sub RecordIfChanged(vehicleId As Integer, oldStatus As String, reason As String, checks As String)
            Dim newStatus = CurrentStatus(vehicleId)
            If String.IsNullOrWhiteSpace(newStatus) OrElse
               String.Equals(oldStatus, newStatus, StringComparison.OrdinalIgnoreCase) Then Return
            Db.Execute("INSERT INTO [VehicleStatusEvents] ([vehicle_id], [changed_at], [old_status], [new_status], [staff_name], [reason], [checks]) VALUES (?, ?, ?, ?, ?, ?, ?)",
                       Db.Int(vehicleId), Db.Dt(DateTime.Now), Db.Text(oldStatus, 30), Db.Text(newStatus, 30),
                       Db.Text(StaffName(), 100), Db.Memo(reason), Db.Memo(checks))
        End Sub

        Public Shared Sub RecordIfChanged(connection As OleDbConnection, tx As OleDbTransaction,
                                          vehicleId As Integer, oldStatus As String, reason As String, checks As String)
            Dim newStatus = CurrentStatus(connection, tx, vehicleId)
            If String.IsNullOrWhiteSpace(newStatus) OrElse
               String.Equals(oldStatus, newStatus, StringComparison.OrdinalIgnoreCase) Then Return
            Using cmd = connection.CreateCommand()
                cmd.Transaction = tx
                cmd.CommandText = "INSERT INTO [VehicleStatusEvents] ([vehicle_id], [changed_at], [old_status], [new_status], [staff_name], [reason], [checks]) VALUES (?, ?, ?, ?, ?, ?, ?)"
                cmd.Parameters.Add(Db.Int(vehicleId))
                cmd.Parameters.Add(Db.Dt(DateTime.Now))
                cmd.Parameters.Add(Db.Text(oldStatus, 30))
                cmd.Parameters.Add(Db.Text(newStatus, 30))
                cmd.Parameters.Add(Db.Text(StaffName(), 100))
                cmd.Parameters.Add(Db.Memo(reason))
                cmd.Parameters.Add(Db.Memo(checks))
                cmd.ExecuteNonQuery()
            End Using
        End Sub

        Public Shared Sub RecordRegistration(vehicleId As Integer, plate As String)
            Db.Execute("INSERT INTO [VehicleStatusEvents] ([vehicle_id], [changed_at], [old_status], [new_status], [staff_name], [reason], [checks]) VALUES (?, ?, ?, ?, ?, ?, ?)",
                       Db.Int(vehicleId), Db.Dt(DateTime.Now), Db.Text("Fleet Entry", 30), Db.Text("Available", 30),
                       Db.Text(StaffName(), 100), Db.Memo("Vehicle registered · Plate " & plate),
                       Db.Memo("Initial vehicle registration"))
        End Sub

        Public Shared Function GetHistory(vehicleId As Integer) As DataTable
            Return Db.Query("SELECT [event_id], [changed_at], [old_status], [new_status], [staff_name], [reason], [checks] " &
                            "FROM [VehicleStatusEvents] WHERE [vehicle_id] = ? ORDER BY [changed_at] DESC, [event_id] DESC",
                            Db.Int(vehicleId))
        End Function
    End Class
End Namespace
