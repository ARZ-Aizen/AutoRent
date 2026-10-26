Imports System.Data
Imports AutoRent.Data.Context

Namespace Data.Repositories
    Public NotInheritable Class VehicleRepository
        Private Sub New()
        End Sub

        Public Shared ReadOnly Statuses As String() = {"Available", "Reserved", "Rented", "Maintenance", "Retired"}

        Public Shared Function GetAll() As DataTable
            Return Db.Query(
                "SELECT v.[vehicle_id], v.[plate], v.[make], v.[model], v.[category], v.[model_year], v.[mileage], v.[status], r.[daily_rate] " &
                "FROM [Vehicles] AS v LEFT JOIN [Rates] AS r ON v.[category] = r.[category] " &
                "ORDER BY v.[plate]")
        End Function

        Public Shared Function GetPickList() As DataTable
            Return Db.Query("SELECT [vehicle_id], [plate] & ' - ' & [make] & ' ' & [model] AS [display] FROM [Vehicles] WHERE [status] <> 'Retired' ORDER BY [plate]")
        End Function

        Public Shared Function Add(plate As String, make As String, model As String, category As String,
                                   modelYear As Integer?, mileage As Integer, status As String) As String
            plate = plate.Trim().ToUpperInvariant()
            If PlateExists(plate, 0) Then Return $"A vehicle with plate {plate} already exists."

            Db.Execute("INSERT INTO [Vehicles] ([plate], [make], [model], [category], [model_year], [mileage], [status], [created_at]) VALUES (?, ?, ?, ?, ?, ?, ?, ?)",
                       Db.Text(plate, 20), Db.Text(make, 50), Db.Text(model, 50), Db.Text(category, 30),
                       Db.IntOrNull(modelYear), Db.Int(mileage), Db.Text(status, 20), Db.Dt(DateTime.Now))
            Return Nothing
        End Function

        Public Shared Function Update(vehicleId As Integer, plate As String, make As String, model As String, category As String,
                                      modelYear As Integer?, mileage As Integer, status As String) As String
            plate = plate.Trim().ToUpperInvariant()
            If PlateExists(plate, vehicleId) Then Return $"Another vehicle already uses plate {plate}."

            Db.Execute("UPDATE [Vehicles] SET [plate] = ?, [make] = ?, [model] = ?, [category] = ?, [model_year] = ?, [mileage] = ?, [status] = ? WHERE [vehicle_id] = ?",
                       Db.Text(plate, 20), Db.Text(make, 50), Db.Text(model, 50), Db.Text(category, 30),
                       Db.IntOrNull(modelYear), Db.Int(mileage), Db.Text(status, 20), Db.Int(vehicleId))
            Return Nothing
        End Function

        Public Shared Function Delete(vehicleId As Integer) As String
            Dim uses = Db.Count("SELECT COUNT(*) FROM [Maintenance] WHERE [vehicle_id] = ?", Db.Int(vehicleId)) +
                       Db.Count("SELECT COUNT(*) FROM [Reservations] WHERE [vehicle_id] = ?", Db.Int(vehicleId))
            If uses > 0 Then
                Return "This vehicle has maintenance or reservation history, so it can't be deleted. Set its status to Retired instead."
            End If

            Db.Execute("DELETE FROM [Vehicles] WHERE [vehicle_id] = ?", Db.Int(vehicleId))
            Return Nothing
        End Function

        Private Shared Function PlateExists(plate As String, excludeId As Integer) As Boolean
            Return Db.Count("SELECT COUNT(*) FROM [Vehicles] WHERE [plate] = ? AND [vehicle_id] <> ?", Db.Text(plate, 20), Db.Int(excludeId)) > 0
        End Function
    End Class
End Namespace
