Namespace Presentation.Views.User

    Public Class FleetItem
        Public Property DbId As Integer
        Public Property Plate As String
        Public Property Model As String
        Public Property Category As String
        Public Property DailyRate As Decimal
        Public Property Status As String

        Public ReadOnly Property Display As String
            Get
                Return Plate & " - " & Model
            End Get
        End Property

        Public Sub New(plate As String, model As String, category As String, dailyRate As Decimal, status As String)
            Me.Plate = plate
            Me.Model = model
            Me.Category = category
            Me.DailyRate = dailyRate
            Me.Status = status
        End Sub
    End Class

    Public Class CustomerItem
        Public Property DbId As Integer
        Public Property Name As String
        Public Property Phone As String
        Public Property Email As String
        Public Property License As String

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

    Public Class InspectionItem
        Public Property InspectedOn As Date
        Public Property Vehicle As String
        Public Property Inspector As String
        Public Property Fuel As String
        Public Property Findings As String
        Public Property Result As String

        Public Sub New(inspectedOn As Date, vehicle As String, inspector As String,
                       fuel As String, findings As String, result As String)
            Me.InspectedOn = inspectedOn
            Me.Vehicle = vehicle
            Me.Inspector = inspector
            Me.Fuel = fuel
            Me.Findings = findings
            Me.Result = result
        End Sub
    End Class

End Namespace