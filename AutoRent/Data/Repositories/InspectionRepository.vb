Imports System.Data
Imports System.Data.OleDb
Imports AutoRent.Common.Utilities
Imports AutoRent.Data.Context

Namespace Data.Repositories

    Public Class InspectionRecord
        Public Property InspectionId As Integer
        Public Property VehicleId As Integer
        Public Property VehiclePlate As String
        Public Property VehicleMake As String
        Public Property VehicleModel As String
        Public Property ChecklistType As String = "Pre-Rental"
        Public Property InspectedAt As DateTime = DateTime.Now
        Public Property Inspector As String
        Public Property Odometer As Long
        Public Property Fuel As String = "Full"
        Public Property ExteriorOk As Boolean = True
        Public Property TiresOk As Boolean = True
        Public Property LightsOk As Boolean = True
        Public Property InteriorOk As Boolean = True
        Public Property FluidsOk As Boolean = True
        Public Property BrakesOk As Boolean = True
        Public Property Findings As String = ""
        Public Property InspectionResult As String = "Passed"
        Public Property CreatedAt As DateTime = DateTime.Now

        ' ============================
        ' COMPUTED / UI DISPLAY PROPS
        ' ============================
        Public ReadOnly Property DisplayId As String
            Get
                Return $"#CK-{InspectionId:D4}"
            End Get
        End Property

        Public ReadOnly Property VehicleDisplay As String
            Get
                Return $"[{VehiclePlate}] {VehicleMake} {VehicleModel}"
            End Get
        End Property

        Public ReadOnly Property DateDisplay As String
            Get
                Return InspectedAt.ToString("MMM dd, yyyy · hh:mm tt")
            End Get
        End Property

        Public ReadOnly Property OdometerDisplay As String
            Get
                Return $"{Odometer:N0} km"
            End Get
        End Property

        Public ReadOnly Property FuelDisplay As String
            Get
                Return $"Fuel: {Fuel}"
            End Get
        End Property

        Public ReadOnly Property IsPass As Boolean
            Get
                Return String.Equals(InspectionResult, "Passed", StringComparison.OrdinalIgnoreCase)
            End Get
        End Property

        Public ReadOnly Property ResultUpper As String
            Get
                Return If(IsPass, "PASSED", "NEEDS ATTENTION")
            End Get
        End Property

        Public ReadOnly Property ResultIcon As String
            Get
                Return If(IsPass, ChrW(&HE73E), ChrW(&HE7BA))
            End Get
        End Property

        Public ReadOnly Property ResultBgColor As String
            Get
                Return If(IsPass, "#E6F4EA", "#FCE8E6")
            End Get
        End Property

        Public ReadOnly Property ResultFgColor As String
            Get
                Return If(IsPass, "#137333", "#C5221F")
            End Get
        End Property

        Public ReadOnly Property ResultBorderColor As String
            Get
                Return If(IsPass, "#CEEAD6", "#F5C2C7")
            End Get
        End Property

        Public ReadOnly Property PassedPointsCount As Integer
            Get
                Dim count As Integer = 0
                If ExteriorOk Then count += 1
                If TiresOk Then count += 1
                If LightsOk Then count += 1
                If InteriorOk Then count += 1
                If FluidsOk Then count += 1
                If BrakesOk Then count += 1
                Return count
            End Get
        End Property

        Public ReadOnly Property ScoreDisplay As String
            Get
                Return $"{PassedPointsCount}/6 OK"
            End Get
        End Property

        Public ReadOnly Property ScoreBgColor As String
            Get
                Return If(PassedPointsCount = 6, "#E8F5E9", If(PassedPointsCount >= 4, "#FFF8E1", "#FFEBEE"))
            End Get
        End Property

        Public ReadOnly Property ScoreFgColor As String
            Get
                Return If(PassedPointsCount = 6, "#2E7D32", If(PassedPointsCount >= 4, "#F57F17", "#C62828"))
            End Get
        End Property

        Public ReadOnly Property TypeBadgeBg As String
            Get
                Select Case ChecklistType
                    Case "Pre-Rental" : Return "#E8F0FE"
                    Case "Post-Rental" : Return "#FEF7E0"
                    Case Else : Return "#F1F3F4"
                End Select
            End Get
        End Property

        Public ReadOnly Property TypeBadgeFg As String
            Get
                Select Case ChecklistType
                    Case "Pre-Rental" : Return "#1A73E8"
                    Case "Post-Rental" : Return "#B06000"
                    Case Else : Return "#5F6368"
                End Select
            End Get
        End Property
    End Class

    Public Structure InspectionStats
        Public TotalChecklists As Integer
        Public PassedCount As Integer
        Public AttentionCount As Integer
        Public MaintenanceVehiclesCount As Integer
    End Structure

    Public Class FleetVehicleOption
        Public Property VehicleId As Integer
        Public Property Plate As String
        Public Property Make As String
        Public Property Model As String
        Public Property Category As String
        Public Property Mileage As Long
        Public Property Status As String

        Public ReadOnly Property DisplayText As String
            Get
                Return $"[{Plate}] {Make} {Model} ({Category}) · {Mileage:N0} km"
            End Get
        End Property

        ' Some custom ComboBox templates render an object using ToString instead of
        ' DisplayMemberPath. Always return a readable vehicle label in both places.
        Public Overrides Function ToString() As String
            Return DisplayText
        End Function
    End Class

    Public NotInheritable Class InspectionRepository
        Private Sub New()
        End Sub

        Public Shared Function GetAll(Optional search As String = Nothing,
                                       Optional resultFilter As String = Nothing,
                                       Optional typeFilter As String = Nothing) As List(Of InspectionRecord)
            Dim list As New List(Of InspectionRecord)()
            Try
                Dim sql =
                    "SELECT i.[inspection_id], i.[vehicle_id], i.[checklist_type], i.[inspected_at], " &
                    "i.[inspector], i.[odometer], i.[fuel], i.[exterior_ok], i.[tires_ok], i.[lights_ok], " &
                    "i.[interior_ok], i.[fluids_ok], i.[brakes_ok], i.[findings], i.[inspection_result], i.[created_at], " &
                    "v.[plate], v.[make], v.[model] " &
                    "FROM [Inspections] AS i " &
                    "LEFT JOIN [Vehicles] AS v ON i.[vehicle_id] = v.[vehicle_id] " &
                    "ORDER BY i.[inspected_at] DESC"

                Dim dt = Db.Query(sql)
                For Each row As DataRow In dt.Rows
                    Dim item As New InspectionRecord() With {
                        .InspectionId = Convert.ToInt32(row("inspection_id")),
                        .VehicleId = Convert.ToInt32(row("vehicle_id")),
                        .ChecklistType = If(IsDBNull(row("checklist_type")), "Pre-Rental", row("checklist_type").ToString()),
                        .InspectedAt = If(IsDBNull(row("inspected_at")), DateTime.Now, Convert.ToDateTime(row("inspected_at"))),
                        .Inspector = If(IsDBNull(row("inspector")), "Staff Inspector", row("inspector").ToString()),
                        .Odometer = If(IsDBNull(row("odometer")), 0L, Convert.ToInt64(row("odometer"))),
                        .Fuel = If(IsDBNull(row("fuel")), "Full", row("fuel").ToString()),
                        .ExteriorOk = If(IsDBNull(row("exterior_ok")), True, Convert.ToBoolean(row("exterior_ok"))),
                        .TiresOk = If(IsDBNull(row("tires_ok")), True, Convert.ToBoolean(row("tires_ok"))),
                        .LightsOk = If(IsDBNull(row("lights_ok")), True, Convert.ToBoolean(row("lights_ok"))),
                        .InteriorOk = If(IsDBNull(row("interior_ok")), True, Convert.ToBoolean(row("interior_ok"))),
                        .FluidsOk = If(IsDBNull(row("fluids_ok")), True, Convert.ToBoolean(row("fluids_ok"))),
                        .BrakesOk = If(IsDBNull(row("brakes_ok")), True, Convert.ToBoolean(row("brakes_ok"))),
                        .Findings = If(IsDBNull(row("findings")), "", row("findings").ToString()),
                        .InspectionResult = If(IsDBNull(row("inspection_result")), "Passed", row("inspection_result").ToString()),
                        .CreatedAt = If(IsDBNull(row("created_at")), DateTime.Now, Convert.ToDateTime(row("created_at"))),
                        .VehiclePlate = If(IsDBNull(row("plate")), "N/A", row("plate").ToString()),
                        .VehicleMake = If(IsDBNull(row("make")), "Vehicle", row("make").ToString()),
                        .VehicleModel = If(IsDBNull(row("model")), "", row("model").ToString())
                    }

                    ' In-memory filtration for search query, result status, and checklist type
                    Dim matchesSearch = True
                    If Not String.IsNullOrWhiteSpace(search) Then
                        Dim q = search.Trim().ToLowerInvariant()
                        matchesSearch = item.VehiclePlate.ToLowerInvariant().Contains(q) OrElse
                                        item.VehicleMake.ToLowerInvariant().Contains(q) OrElse
                                        item.VehicleModel.ToLowerInvariant().Contains(q) OrElse
                                        item.Inspector.ToLowerInvariant().Contains(q) OrElse
                                        item.DisplayId.ToLowerInvariant().Contains(q) OrElse
                                        item.Findings.ToLowerInvariant().Contains(q)
                    End If

                    Dim matchesResult = True
                    If Not String.IsNullOrWhiteSpace(resultFilter) AndAlso Not resultFilter.StartsWith("All", StringComparison.OrdinalIgnoreCase) Then
                        If resultFilter.StartsWith("Pass", StringComparison.OrdinalIgnoreCase) Then
                            matchesResult = item.IsPass
                        ElseIf resultFilter.Contains("Attention") OrElse resultFilter.Contains("Defect") Then
                            matchesResult = Not item.IsPass
                        End If
                    End If

                    Dim matchesType = True
                    If Not String.IsNullOrWhiteSpace(typeFilter) AndAlso Not typeFilter.StartsWith("All", StringComparison.OrdinalIgnoreCase) Then
                        matchesType = String.Equals(item.ChecklistType, typeFilter, StringComparison.OrdinalIgnoreCase)
                    End If

                    If matchesSearch AndAlso matchesResult AndAlso matchesType Then
                        list.Add(item)
                    End If
                Next
            Catch ex As Exception
                System.Diagnostics.Debug.WriteLine("Error in InspectionRepository.GetAll: " & ex.Message)
            End Try

            Return list
        End Function

        Public Shared Function GetById(id As Integer) As InspectionRecord
            Dim list = GetAll()
            Return list.FirstOrDefault(Function(x) x.InspectionId = id)
        End Function

        Public Shared Function GetVehicleOptions() As List(Of FleetVehicleOption)
            Dim options As New List(Of FleetVehicleOption)()
            Try
                Dim sql = "SELECT [vehicle_id], [plate], [make], [model], [category], [mileage], [status] FROM [Vehicles] ORDER BY [plate]"
                Dim dt = Db.Query(sql)
                For Each row As DataRow In dt.Rows
                    options.Add(New FleetVehicleOption() With {
                        .VehicleId = Convert.ToInt32(row("vehicle_id")),
                        .Plate = If(IsDBNull(row("plate")), "", row("plate").ToString()),
                        .Make = If(IsDBNull(row("make")), "", row("make").ToString()),
                        .Model = If(IsDBNull(row("model")), "", row("model").ToString()),
                        .Category = If(IsDBNull(row("category")), "", row("category").ToString()),
                        .Mileage = If(IsDBNull(row("mileage")), 0L, Convert.ToInt64(row("mileage"))),
                        .Status = If(IsDBNull(row("status")), "Available", row("status").ToString())
                    })
                Next
            Catch ex As Exception
                System.Diagnostics.Debug.WriteLine("Error in InspectionRepository.GetVehicleOptions: " & ex.Message)
            End Try
            Return options
        End Function

        Public Shared Function Add(record As InspectionRecord) As String
            If record Is Nothing Then Return "Invalid inspection record."
            If record.VehicleId <= 0 Then Return "Please select a vehicle to inspect."
            If String.IsNullOrWhiteSpace(record.Inspector) Then Return "Inspector name is required."
            If record.Odometer < 0 Then Return "Odometer reading cannot be negative."

            ' Determine result automatically based on the 6 checklist points
            Dim allPointsPassed = record.ExteriorOk AndAlso record.TiresOk AndAlso record.LightsOk AndAlso
                                  record.InteriorOk AndAlso record.FluidsOk AndAlso record.BrakesOk

            record.InspectionResult = If(allPointsPassed, "Passed", "Needs Attention")

            If Not allPointsPassed AndAlso String.IsNullOrWhiteSpace(record.Findings) Then
                Return "Please enter specific defect notes/findings for the unchecked safety items."
            End If

            Try
                Dim sql =
                    "INSERT INTO [Inspections] (" &
                    "[vehicle_id], [checklist_type], [inspected_at], [inspector], [odometer], [fuel], " &
                    "[exterior_ok], [tires_ok], [lights_ok], [interior_ok], [fluids_ok], [brakes_ok], " &
                    "[findings], [inspection_result], [created_at]) " &
                    "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)"

                Db.Execute(sql,
                           Db.Int(record.VehicleId),
                           Db.Text(record.ChecklistType, 30),
                           Db.Dt(record.InspectedAt),
                           Db.Text(record.Inspector, 100),
                           Db.Int(CInt(record.Odometer)),
                           Db.Text(record.Fuel, 20),
                           Db.Bool(record.ExteriorOk),
                           Db.Bool(record.TiresOk),
                           Db.Bool(record.LightsOk),
                           Db.Bool(record.InteriorOk),
                           Db.Bool(record.FluidsOk),
                           Db.Bool(record.BrakesOk),
                           Db.Memo(record.Findings),
                           Db.Text(record.InspectionResult, 20),
                           Db.Dt(DateTime.Now))

                ' Synchronize fleet status & odometer
                SyncFleetAfterInspection(record.VehicleId, record.InspectionResult, record.Odometer)
                Return Nothing
            Catch ex As Exception
                Return "Database error saving checklist: " & ex.Message
            End Try
        End Function

        Public Shared Function Update(record As InspectionRecord) As String
            If record Is Nothing Then Return "Invalid inspection record."
            If record.InspectionId <= 0 Then Return "Invalid inspection ID."
            If String.IsNullOrWhiteSpace(record.Inspector) Then Return "Inspector name is required."
            If record.Odometer < 0 OrElse record.Odometer > Integer.MaxValue Then Return "Please enter a valid odometer reading."
            Dim existing = Db.Query("SELECT [vehicle_id] FROM [Inspections] WHERE [inspection_id] = ?", Db.Int(record.InspectionId))
            If existing.Rows.Count = 0 Then Return "Checklist record no longer exists."
            If Convert.ToInt32(existing.Rows(0)("vehicle_id")) <> record.VehicleId Then
                Return "The inspected vehicle cannot be changed on an existing checklist."
            End If

            Dim allPointsPassed = record.ExteriorOk AndAlso record.TiresOk AndAlso record.LightsOk AndAlso
                                  record.InteriorOk AndAlso record.FluidsOk AndAlso record.BrakesOk

            record.InspectionResult = If(allPointsPassed, "Passed", "Needs Attention")
            If Not allPointsPassed AndAlso String.IsNullOrWhiteSpace(record.Findings) Then
                Return "Please enter specific defect notes/findings for the unchecked safety items."
            End If

            Try
                Dim sql =
                    "UPDATE [Inspections] SET " &
                    "[checklist_type] = ?, [inspector] = ?, [odometer] = ?, [fuel] = ?, " &
                    "[exterior_ok] = ?, [tires_ok] = ?, [lights_ok] = ?, [interior_ok] = ?, [fluids_ok] = ?, [brakes_ok] = ?, " &
                    "[findings] = ?, [inspection_result] = ? " &
                    "WHERE [inspection_id] = ?"

                Db.Execute(sql,
                           Db.Text(record.ChecklistType, 30),
                           Db.Text(record.Inspector, 100),
                           Db.Int(CInt(record.Odometer)),
                           Db.Text(record.Fuel, 20),
                           Db.Bool(record.ExteriorOk),
                           Db.Bool(record.TiresOk),
                           Db.Bool(record.LightsOk),
                           Db.Bool(record.InteriorOk),
                           Db.Bool(record.FluidsOk),
                           Db.Bool(record.BrakesOk),
                           Db.Memo(record.Findings),
                           Db.Text(record.InspectionResult, 20),
                           Db.Int(record.InspectionId))

                ' Editing an older checklist must not override the latest vehicle safety assessment.
                Dim latestId = Db.Scalar("SELECT TOP 1 [inspection_id] FROM [Inspections] WHERE [vehicle_id] = ? ORDER BY [inspected_at] DESC, [inspection_id] DESC", Db.Int(record.VehicleId))
                If latestId IsNot Nothing AndAlso Not IsDBNull(latestId) AndAlso Convert.ToInt32(latestId) = record.InspectionId Then
                    SyncFleetAfterInspection(record.VehicleId, record.InspectionResult, record.Odometer)
                End If
                Return Nothing
            Catch ex As Exception
                Return "Database error updating checklist: " & ex.Message
            End Try
        End Function

        Public Shared Function Delete(inspectionId As Integer) As String
            Try
                Db.Execute("DELETE FROM [Inspections] WHERE [inspection_id] = ?", Db.Int(inspectionId))
                Return Nothing
            Catch ex As Exception
                Return "Database error deleting checklist: " & ex.Message
            End Try
        End Function

        Private Shared Sub SyncFleetAfterInspection(vehicleId As Integer, result As String, newOdometer As Long)
            Dim oldStatus = VehicleStatusAuditRepository.CurrentStatus(vehicleId)
            If String.IsNullOrWhiteSpace(oldStatus) Then Throw New InvalidOperationException("Vehicle not found for checklist status synchronization.")
            ' If failed / needs attention, set vehicle status to Maintenance
            If String.Equals(result, "Needs Attention", StringComparison.OrdinalIgnoreCase) Then
                    Db.Execute("UPDATE [Vehicles] SET [status] = 'Maintenance' WHERE [vehicle_id] = ?", Db.Int(vehicleId))
                ElseIf String.Equals(result, "Passed", StringComparison.OrdinalIgnoreCase) Then
                    ' If vehicle is currently in maintenance, but passed checklist, restore to Available
                    Dim curStatus = Db.Scalar("SELECT [status] FROM [Vehicles] WHERE [vehicle_id] = ?", Db.Int(vehicleId))
                    If curStatus IsNot Nothing AndAlso String.Equals(curStatus.ToString(), "Maintenance", StringComparison.OrdinalIgnoreCase) AndAlso
                       Db.Count("SELECT COUNT(*) FROM [Maintenance] WHERE [vehicle_id] = ? AND [status] = 'In Progress'", Db.Int(vehicleId)) = 0 AndAlso
                       Db.Count("SELECT COUNT(*) FROM [Reservations] WHERE [vehicle_id] = ? AND [status] IN ('Active', 'Confirmed')", Db.Int(vehicleId)) = 0 Then
                        Db.Execute("UPDATE [Vehicles] SET [status] = 'Available' WHERE [vehicle_id] = ?", Db.Int(vehicleId))
                    End If
                End If

                ' Update vehicle mileage if new odometer reading is higher
                If newOdometer > 0 Then
                    Db.Execute("UPDATE [Vehicles] SET [mileage] = ? WHERE [vehicle_id] = ? AND ([mileage] IS NULL OR [mileage] < ?)",
                               Db.Int(CInt(newOdometer)), Db.Int(vehicleId), Db.Int(CInt(newOdometer)))
                End If
            VehicleStatusAuditRepository.RecordIfChanged(vehicleId, oldStatus,
                "Vehicle condition checklist: " & result, "Safety inspection · Odometer " & newOdometer.ToString("N0") & " km")
        End Sub

        Public Shared Function GetSummaryStats() As InspectionStats
            Dim stats As New InspectionStats()
            Try
                Dim all = GetAll()
                stats.TotalChecklists = all.Count
                stats.PassedCount = all.Where(Function(x) x.IsPass).Count()
                stats.AttentionCount = all.Where(Function(x) Not x.IsPass).Count()

                ' Count vehicles currently in maintenance
                Dim maintCount = Db.Scalar("SELECT COUNT(*) FROM [Vehicles] WHERE [status] = 'Maintenance'")
                stats.MaintenanceVehiclesCount = If(maintCount IsNot Nothing AndAlso Not IsDBNull(maintCount), Convert.ToInt32(maintCount), 0)
            Catch ex As Exception
                System.Diagnostics.Debug.WriteLine("Error in GetSummaryStats: " & ex.Message)
            End Try
            Return stats
        End Function
    End Class

End Namespace

