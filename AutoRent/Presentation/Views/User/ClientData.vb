Imports System.Data
Imports System.Windows
Imports AutoRent.Data.Repositories

Namespace Presentation.Views.User

    Friend Module ClientData

        Private Function Txt(row As DataRow, column As String) As String
            If IsDBNull(row(column)) Then Return String.Empty
            Return Convert.ToString(row(column))
        End Function

        Private Function Num(row As DataRow, column As String) As Integer
            If IsDBNull(row(column)) Then Return 0
            Return Convert.ToInt32(row(column))
        End Function

        Private Function Money(row As DataRow, column As String) As Decimal
            If IsDBNull(row(column)) Then Return 0D
            Return Convert.ToDecimal(row(column))
        End Function

        Private Function Stamp(row As DataRow, column As String) As Date
            If IsDBNull(row(column)) Then Return Date.MinValue
            Return Convert.ToDateTime(row(column))
        End Function

        Private Function VehicleName(row As DataRow) As String
            Dim parts As New List(Of String)
            For Each col In {"make", "model", "model_year"}
                Dim part = Txt(row, col).Trim()
                If part <> "" Then parts.Add(part)
            Next
            Return String.Join(" ", parts)
        End Function

        Private Function VehicleLabel(row As DataRow) As String
            Dim plate = Txt(row, "plate")
            If plate = "" Then Return "(vehicle removed)"
            Return plate & " - " & VehicleName(row)
        End Function

        Friend Sub ShowDbError(action As String, ex As Exception)
            MessageBox.Show($"Could not {action}: {ex.Message}", "AutoRent",
                            MessageBoxButton.OK, MessageBoxImage.Error)
        End Sub

        Friend Function LoadFleet() As List(Of FleetItem)
            Dim list As New List(Of FleetItem)
            For Each r As DataRow In ClientRepository.GetFleet().Rows
                list.Add(New FleetItem(Txt(r, "plate"), VehicleName(r), Txt(r, "category"), Money(r, "daily_rate"),
                                       If(Txt(r, "status") = "", "Available", Txt(r, "status"))) With {.DbId = Num(r, "vehicle_id")})
            Next
            Return list
        End Function

        Friend Function LoadCustomers() As List(Of CustomerItem)
            Dim list As New List(Of CustomerItem)
            For Each r As DataRow In ClientRepository.GetCustomers().Rows
                list.Add(New CustomerItem(Txt(r, "full_name"), Txt(r, "phone"), Txt(r, "email"), Txt(r, "license_no")) With {.DbId = Num(r, "customer_id")})
            Next
            Return list
        End Function

        Friend Function LoadReservations() As List(Of ReservationItem)
            Dim list As New List(Of ReservationItem)
            For Each r As DataRow In ClientRepository.GetReservations().Rows
                Dim id = Num(r, "reservation_id")
                list.Add(New ReservationItem("R-" & id.ToString("0000"), Txt(r, "customer_name"), VehicleLabel(r), Txt(r, "plate"),
                                             Stamp(r, "pickup_date"), Stamp(r, "return_date"), Money(r, "total"), Txt(r, "status")) With {.DbId = id, .VehicleId = Num(r, "vehicle_id")})
            Next
            Return list
        End Function

        Friend Function LoadDispatch() As List(Of DispatchItem)
            Dim list As New List(Of DispatchItem)
            For Each r As DataRow In ClientRepository.GetDispatchBoard().Rows
                Dim at = Stamp(r, "scheduled_at")
                Dim timeText = If(at.Date = Date.Today, at.ToString("hh:mm tt"), at.ToString("MMM d, hh:mm tt"))
                list.Add(New DispatchItem(timeText, Txt(r, "dispatch_type"), Txt(r, "customer_name"), VehicleName(r), Txt(r, "plate"),
                                          Txt(r, "branch"), Txt(r, "status")) With {.DbId = Num(r, "dispatch_id"), .ReservationId = Num(r, "reservation_id"), .VehicleId = Num(r, "vehicle_id")})
            Next
            Return list
        End Function

        Friend Function LoadInspections() As List(Of InspectionItem)
            Dim list As New List(Of InspectionItem)
            For Each r As DataRow In ClientRepository.GetInspections().Rows
                list.Add(New InspectionItem(Stamp(r, "inspected_at"), VehicleLabel(r), Txt(r, "inspector"), Txt(r, "fuel"),
                                            Txt(r, "findings"), Txt(r, "inspection_result")))
            Next
            Return list
        End Function

    End Module
End Namespace