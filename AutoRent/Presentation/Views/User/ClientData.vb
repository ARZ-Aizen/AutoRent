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
                Dim make = Txt(r, "make")
                Dim model = Txt(r, "model").Trim()
                Dim year = Txt(r, "model_year").Trim()
                Dim modelDisplay = If(model <> "" AndAlso year <> "", model & " " & year, model & year)
                Dim imgPath = Txt(r, "image_path")
                Dim item As New FleetItem(Txt(r, "plate"), make, modelDisplay, Txt(r, "category"), Money(r, "daily_rate"),
                                          If(Txt(r, "status") = "", "Available", Txt(r, "status")), imgPath) With {.DbId = Num(r, "vehicle_id")}
                item.Photo = ReadVehiclePhoto(imgPath)
                list.Add(item)
            Next
            Return list
        End Function

        Private Function ReadVehiclePhoto(imagePath As String) As System.Windows.Media.Imaging.BitmapImage
            Try
                Dim fullPath = Data.Repositories.VehicleRepository.GetImageFilePath(imagePath)
                If String.IsNullOrWhiteSpace(fullPath) OrElse Not IO.File.Exists(fullPath) Then Return Nothing

                Dim bitmap As New System.Windows.Media.Imaging.BitmapImage()
                bitmap.BeginInit()
                bitmap.UriSource = New Uri(fullPath, UriKind.Absolute)
                bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad
                bitmap.DecodePixelWidth = 280
                bitmap.EndInit()
                bitmap.Freeze()
                Return bitmap
            Catch
                Return Nothing
            End Try
        End Function

        Friend Function LoadCustomers() As List(Of CustomerItem)
            Dim list As New List(Of CustomerItem)
            Dim activeCustomers As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            Try
                For Each resRow As DataRow In ClientRepository.GetReservations().Rows
                    Dim st = Txt(resRow, "status")
                    If String.Equals(st, "Confirmed", StringComparison.OrdinalIgnoreCase) OrElse
                       String.Equals(st, "Active", StringComparison.OrdinalIgnoreCase) Then
                        Dim cName = Txt(resRow, "customer_name").Trim()
                        If cName <> "" Then activeCustomers.Add(cName)
                    End If
                Next
            Catch
            End Try

            For Each r As DataRow In ClientRepository.GetCustomers().Rows
                Dim name = Txt(r, "full_name").Trim()
                Dim item As New CustomerItem(name, Txt(r, "phone"), Txt(r, "email"), Txt(r, "license_no")) With {
                    .DbId = Num(r, "customer_id"),
                    .HasActiveReservation = activeCustomers.Contains(name)
                }
                list.Add(item)
            Next
            Return list
        End Function

        Friend Function LoadReservations() As List(Of ReservationItem)
            Dim list As New List(Of ReservationItem)
            For Each r As DataRow In ClientRepository.GetReservations().Rows
                Dim id = Num(r, "reservation_id")
                list.Add(New ReservationItem("BK-" & id.ToString("0000"), Txt(r, "customer_name"), VehicleLabel(r), Txt(r, "plate"),
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

    End Module
End Namespace