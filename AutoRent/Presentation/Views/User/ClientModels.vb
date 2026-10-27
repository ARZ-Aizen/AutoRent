Namespace Presentation.Views.User

    Public Class FleetItem
        Public Property DbId As Integer
        Public Property Plate As String
        Public Property Make As String
        Public Property Model As String
        Public Property Category As String
        Public Property DailyRate As Decimal
        Public Property Status As String
        Public Property ImagePath As String
        Public Property Photo As System.Windows.Media.Imaging.BitmapImage

        Public ReadOnly Property Display As String
            Get
                Return Plate & " - " & Make & " " & Model
            End Get
        End Property

        ''' <summary>Status in uppercase for the card badge.</summary>
        Public ReadOnly Property StatusUpper As String
            Get
                If String.IsNullOrWhiteSpace(Status) Then Return "AVAILABLE"
                Dim s = Status.Trim()
                If String.Equals(s, "Maintenance", StringComparison.OrdinalIgnoreCase) OrElse
                   String.Equals(s, "In Maintenance", StringComparison.OrdinalIgnoreCase) OrElse
                   String.Equals(s, "Under Maintenance", StringComparison.OrdinalIgnoreCase) Then
                    Return "IN MAINTENANCE"
                End If
                Return s.ToUpperInvariant()
            End Get
        End Property

        ''' <summary>Returns True when Photo is Nothing so a placeholder icon can be shown.</summary>
        Public ReadOnly Property HasNoPhoto As Boolean
            Get
                Return Photo Is Nothing
            End Get
        End Property

        Public Sub New(plate As String, make As String, model As String, category As String, dailyRate As Decimal, status As String, imagePath As String)
            Me.Plate = plate
            Me.Make = make
            Me.Model = model
            Me.Category = category
            Me.DailyRate = dailyRate
            Me.Status = status
            Me.ImagePath = imagePath
        End Sub
    End Class

    Public Class CustomerItem
        Public Property DbId As Integer
        Public Property Name As String
        Public Property Phone As String
        Public Property Email As String
        Public Property License As String

        Public ReadOnly Property CustomerIdDisplay As String
            Get
                Return $"CUST-{DbId:D4}"
            End Get
        End Property

        Public ReadOnly Property Initials As String
            Get
                If String.IsNullOrWhiteSpace(Name) Then Return "?"
                Dim parts = Name.Trim().Split({" "c}, StringSplitOptions.RemoveEmptyEntries)
                If parts.Length = 1 Then
                    Return parts(0).Substring(0, Math.Min(2, parts(0).Length)).ToUpperInvariant()
                Else
                    Return (parts(0)(0).ToString() & parts(parts.Length - 1)(0).ToString()).ToUpperInvariant()
                End If
            End Get
        End Property

        Public ReadOnly Property AvatarBgColor As String
            Get
                Dim colors = {"#F7E9EB", "#E6F2E7", "#E3EEF9", "#FDEBDD", "#F0E6F5"}
                Dim idx = Math.Abs(If(Name, "").GetHashCode()) Mod colors.Length
                Return colors(idx)
            End Get
        End Property

        Public ReadOnly Property AvatarFgColor As String
            Get
                Dim colors = {"#70000F", "#16A34A", "#1565C0", "#E65100", "#6A1B9A"}
                Dim idx = Math.Abs(If(Name, "").GetHashCode()) Mod colors.Length
                Return colors(idx)
            End Get
        End Property

        Public ReadOnly Property HasLicense As Boolean
            Get
                Return Not String.IsNullOrWhiteSpace(License)
            End Get
        End Property

        Public ReadOnly Property CanRent As Boolean
            Get
                Return HasLicense
            End Get
        End Property

        Public ReadOnly Property LicenseDisplay As String
            Get
                Return If(HasLicense, License, "No License")
            End Get
        End Property

        Public ReadOnly Property LicenseBadgeText As String
            Get
                Return If(HasLicense, License, "No License (Cannot Rent)")
            End Get
        End Property

        Public ReadOnly Property LicenseBadgeIcon As String
            Get
                Return If(HasLicense, ChrW(&HE73E), ChrW(&HE711)) ' Checkmark or X
            End Get
        End Property

        Public ReadOnly Property LicenseBadgeBg As String
            Get
                Return If(HasLicense, "#E6F4EA", "#FEE2E2")
            End Get
        End Property

        Public ReadOnly Property LicenseBadgeBorder As String
            Get
                Return If(HasLicense, "#CEEAD6", "#FECACA")
            End Get
        End Property

        Public ReadOnly Property LicenseBadgeFg As String
            Get
                Return If(HasLicense, "#137333", "#B91C1C")
            End Get
        End Property

        Public ReadOnly Property CanRentTooltip As String
            Get
                Return If(HasLicense, "Driver license verified on file. Customer is eligible to rent.", "No driver license on file. Customer CANNOT rent vehicles.")
            End Get
        End Property

        Public Property HasActiveReservation As Boolean

        Public ReadOnly Property StatusBadgeText As String
            Get
                If HasActiveReservation Then
                    Return "ACTIVE RENTER"
                ElseIf Not HasLicense Then
                    Return "NO LICENSE"
                Else
                    Return "REGISTERED"
                End If
            End Get
        End Property

        Public ReadOnly Property StatusBadgeBg As String
            Get
                If HasActiveReservation Then
                    Return "#E6F4EA"
                ElseIf Not HasLicense Then
                    Return "#FEF3C7"
                Else
                    Return "#F3F1ED"
                End If
            End Get
        End Property

        Public ReadOnly Property StatusBadgeFg As String
            Get
                If HasActiveReservation Then
                    Return "#137333"
                ElseIf Not HasLicense Then
                    Return "#92400E"
                Else
                    Return "#5F5B56"
                End If
            End Get
        End Property

        Public ReadOnly Property StatusDotColor As String
            Get
                If HasActiveReservation Then
                    Return "#16A34A"
                ElseIf Not HasLicense Then
                    Return "#D97706"
                Else
                    Return "#8C857B"
                End If
            End Get
        End Property

        Public ReadOnly Property HasEmail As Boolean
            Get
                Return Not String.IsNullOrWhiteSpace(Email)
            End Get
        End Property

        Public ReadOnly Property PhoneDisplay As String
            Get
                Return If(String.IsNullOrWhiteSpace(Phone), "—", Phone)
            End Get
        End Property

        Public Sub New(name As String, phone As String, email As String, license As String)
            Me.Name = name
            Me.Phone = phone
            Me.Email = email
            Me.License = license
        End Sub
    End Class

    Public Class ReservationItem
        Public Property DbId As Integer
        Public Property VehicleId As Integer
        Public Property Id As String
        Public Property Customer As String
        Public Property Vehicle As String
        Public Property Plate As String
        Public Property PickupDate As Date
        Public Property ReturnDate As Date
        Public Property Total As Decimal
        Public Property Status As String

        Public ReadOnly Property CustomerInitials As String
            Get
                If String.IsNullOrWhiteSpace(Customer) Then Return "?"
                Dim parts = Customer.Trim().Split({" "c}, StringSplitOptions.RemoveEmptyEntries)
                If parts.Length = 1 Then
                    Return parts(0).Substring(0, Math.Min(2, parts(0).Length)).ToUpperInvariant()
                Else
                    Return (parts(0)(0).ToString() & parts(parts.Length - 1)(0).ToString()).ToUpperInvariant()
                End If
            End Get
        End Property

        Public ReadOnly Property CustomerAvatarBg As String
            Get
                Dim colors = {"#F7E9EB", "#E6F2E7", "#E3EEF9", "#FDEBDD", "#F0E6F5"}
                Dim idx = Math.Abs(If(Customer, "").GetHashCode()) Mod colors.Length
                Return colors(idx)
            End Get
        End Property

        Public ReadOnly Property CustomerAvatarFg As String
            Get
                Dim colors = {"#70000F", "#16A34A", "#1565C0", "#E65100", "#6A1B9A"}
                Dim idx = Math.Abs(If(Customer, "").GetHashCode()) Mod colors.Length
                Return colors(idx)
            End Get
        End Property

        Public ReadOnly Property DateRangeDisplay As String
            Get
                Return $"{PickupDate:MMM dd, yyyy}  →  {ReturnDate:MMM dd, yyyy}"
            End Get
        End Property

        Public ReadOnly Property DurationDisplay As String
            Get
                Dim days = Math.Max(1, (ReturnDate.Date - PickupDate.Date).Days)
                Return If(days = 1, "1 day", $"{days} days")
            End Get
        End Property

        Public ReadOnly Property TotalDisplay As String
            Get
                Return $"₱{Total:N2}"
            End Get
        End Property

        Public ReadOnly Property StatusUpper As String
            Get
                Return If(String.IsNullOrWhiteSpace(Status), "CONFIRMED", Status.ToUpperInvariant())
            End Get
        End Property

        Public ReadOnly Property StatusBgColor As String
            Get
                Dim s = If(Status, "").Trim()
                If String.Equals(s, "Active", StringComparison.OrdinalIgnoreCase) Then Return "#E6F4EA"
                If String.Equals(s, "Confirmed", StringComparison.OrdinalIgnoreCase) Then Return "#E3EEF9"
                If String.Equals(s, "Cancelled", StringComparison.OrdinalIgnoreCase) Then Return "#FDF2F2"
                Return "#F3F1ED"
            End Get
        End Property

        Public ReadOnly Property StatusFgColor As String
            Get
                Dim s = If(Status, "").Trim()
                If String.Equals(s, "Active", StringComparison.OrdinalIgnoreCase) Then Return "#137333"
                If String.Equals(s, "Confirmed", StringComparison.OrdinalIgnoreCase) Then Return "#1565C0"
                If String.Equals(s, "Cancelled", StringComparison.OrdinalIgnoreCase) Then Return "#C62828"
                Return "#5F5B56"
            End Get
        End Property

        Public ReadOnly Property StatusDotColor As String
            Get
                Dim s = If(Status, "").Trim()
                If String.Equals(s, "Active", StringComparison.OrdinalIgnoreCase) Then Return "#16A34A"
                If String.Equals(s, "Confirmed", StringComparison.OrdinalIgnoreCase) Then Return "#1976D2"
                If String.Equals(s, "Cancelled", StringComparison.OrdinalIgnoreCase) Then Return "#D32F2F"
                Return "#8C857B"
            End Get
        End Property

        Public ReadOnly Property CanBeManaged As Boolean
            Get
                Dim s = If(Status, "").Trim()
                Return Not String.Equals(s, "Cancelled", StringComparison.OrdinalIgnoreCase) AndAlso
                       Not String.Equals(s, "Completed", StringComparison.OrdinalIgnoreCase)
            End Get
        End Property

        Public Sub New(id As String, customer As String, vehicle As String, plate As String,
                       pickupDate As Date, returnDate As Date, total As Decimal, status As String)
            Me.Id = id
            Me.Customer = customer
            Me.Vehicle = vehicle
            Me.Plate = plate
            Me.PickupDate = pickupDate
            Me.ReturnDate = returnDate
            Me.Total = total
            Me.Status = status
        End Sub
    End Class

    Public Class DispatchItem
        Public Property DbId As Integer
        Public Property ReservationId As Integer
        Public Property VehicleId As Integer
        Public Property Time As String
        Public Property DispatchType As String
        Public Property Customer As String
        Public Property Vehicle As String
        Public Property Plate As String
        Public Property Location As String
        Public Property Status As String

        Public ReadOnly Property CustomerInitials As String
            Get
                If String.IsNullOrWhiteSpace(Customer) Then Return "?"
                Dim parts = Customer.Trim().Split({" "c}, StringSplitOptions.RemoveEmptyEntries)
                If parts.Length = 1 Then
                    Return parts(0).Substring(0, Math.Min(2, parts(0).Length)).ToUpperInvariant()
                Else
                    Return (parts(0)(0).ToString() & parts(parts.Length - 1)(0).ToString()).ToUpperInvariant()
                End If
            End Get
        End Property

        Public ReadOnly Property CustomerAvatarBg As String
            Get
                Dim colors = {"#F7E9EB", "#E6F2E7", "#E3EEF9", "#FDEBDD", "#F0E6F5"}
                Dim idx = Math.Abs(If(Customer, "").GetHashCode()) Mod colors.Length
                Return colors(idx)
            End Get
        End Property

        Public ReadOnly Property CustomerAvatarFg As String
            Get
                Dim colors = {"#70000F", "#16A34A", "#1565C0", "#E65100", "#6A1B9A"}
                Dim idx = Math.Abs(If(Customer, "").GetHashCode()) Mod colors.Length
                Return colors(idx)
            End Get
        End Property

        Public ReadOnly Property IsPickup As Boolean
            Get
                Return String.Equals(DispatchType, "Pickup", StringComparison.OrdinalIgnoreCase)
            End Get
        End Property

        Public ReadOnly Property TypeBgColor As String
            Get
                Return If(IsPickup, "#E3EEF9", "#E6F4EA")
            End Get
        End Property

        Public ReadOnly Property TypeFgColor As String
            Get
                Return If(IsPickup, "#1565C0", "#137333")
            End Get
        End Property

        Public ReadOnly Property StatusUpper As String
            Get
                Return If(String.IsNullOrWhiteSpace(Status), "SCHEDULED", Status.ToUpperInvariant())
            End Get
        End Property

        Public ReadOnly Property StatusBgColor As String
            Get
                Return If(Status = "Completed", "#F3F1ED", "#FDEBDD")
            End Get
        End Property

        Public ReadOnly Property StatusFgColor As String
            Get
                Return If(Status = "Completed", "#5F5B56", "#E65100")
            End Get
        End Property

        Public ReadOnly Property StatusDotColor As String
            Get
                Return If(Status = "Completed", "#8C857B", "#F59E0B")
            End Get
        End Property

        Public ReadOnly Property IsPending As Boolean
            Get
                Return Not String.Equals(Status, "Completed", StringComparison.OrdinalIgnoreCase)
            End Get
        End Property

        Public Sub New(time As String, dispatchType As String, customer As String, vehicle As String,
                       plate As String, location As String, status As String)
            Me.Time = time
            Me.DispatchType = dispatchType
            Me.Customer = customer
            Me.Vehicle = vehicle
            Me.Plate = plate
            Me.Location = location
            Me.Status = status
        End Sub
    End Class


End Namespace