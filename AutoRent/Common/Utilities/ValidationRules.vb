Imports System
Imports System.Collections.Generic
Imports System.Windows
Imports System.Globalization
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports System.Windows.Controls
Imports System.Windows.Input

Namespace Common.Utilities
    ' Shared business rules: run on save as well as in the UI.
    Public NotInheritable Class InputValidation
        Private Sub New()
        End Sub

        Public Shared Function Clean(value As String) As String
            Return Regex.Replace(If(value, "").Trim(), "\s+", " ")
        End Function

        Public Shared Function FullName(value As String) As String
            Dim name = Clean(value)
            If name.Length < 3 OrElse name.Length > 100 Then
                Return "Full name must contain 3–100 characters."
            End If
            If Not Regex.IsMatch(name, "^[\p{L}\p{M}]+(?:[ '\u2019-][\p{L}\p{M}]+)*$") Then
                Return "Use letters only in names (spaces, apostrophes and hyphens are allowed)."
            End If
            If name.Split(" "c, StringSplitOptions.RemoveEmptyEntries).Length < 2 Then
                Return "Enter both a given name and a family name."
            End If
            Return Nothing
        End Function

        Public Shared Function Username(value As String) As String
            Dim usernameValue = If(value, "").Trim()
            If Not Regex.IsMatch(usernameValue, "^[A-Za-z0-9._-]{3,32}$") Then
                Return "Username must be 3–32 characters using letters, numbers, dots, underscores or hyphens."
            End If
            Return Nothing
        End Function

        Public Shared Function PhilippineMobile(value As String) As String
            If Not Regex.IsMatch(If(value, ""), "^09[0-9]{9}$") Then
                Return "Enter an 11-digit Philippine mobile number starting with 09 (example: 09171234567)."
            End If
            Return Nothing
        End Function

        Public Shared Function EmailAddress(value As String) As String
            Dim emailValue = If(value, "").Trim()
            If emailValue = "" Then Return Nothing
            If emailValue.Length > 100 OrElse Not Regex.IsMatch(emailValue, "^[^\s@]+@[^\s@]+\.[^\s@]+$") Then
                Return "Enter a valid email address, or leave it blank."
            End If
            Return Nothing
        End Function

        Public Shared Function License(value As String) As String
            Dim licenseValue = If(value, "").Trim()
            If licenseValue = "" Then Return Nothing
            If Not Regex.IsMatch(licenseValue, "^[A-Za-z0-9-]{5,50}$") Then
                Return "Driver's license must contain 5–50 letters, numbers or hyphens."
            End If
            Return Nothing
        End Function

        Public Shared Function Plate(value As String) As String
            Dim plateValue = If(value, "").Trim()
            If Not Regex.IsMatch(plateValue, "^[A-Za-z0-9 -]{2,20}$") OrElse
               Not Regex.IsMatch(plateValue, "[A-Za-z]") OrElse
               Not Regex.IsMatch(plateValue, "[0-9]") Then
                Return "Plate number must contain letters and numbers (2–20 characters; spaces and hyphens allowed)."
            End If
            Return Nothing
        End Function

        Public Shared Function VehicleMake(value As String) As String
            Dim name = Clean(value)
            If name.Length < 2 OrElse name.Length > 50 OrElse
               Not Regex.IsMatch(name, "^[\p{L}\p{M}0-9 .&'-]+$") Then
                Return "Vehicle make must contain 2–50 letters/numbers (spaces and basic punctuation allowed)."
            End If
            Return Nothing
        End Function

        Public Shared Function VehicleModel(value As String) As String
            Dim name = Clean(value)
            If name.Length < 1 OrElse name.Length > 50 OrElse
               Not Regex.IsMatch(name, "^[\p{L}\p{M}0-9 .&'()+/-]+$") Then
                Return "Vehicle model contains unsupported characters or is longer than 50 characters."
            End If
            Return Nothing
        End Function

        Public Shared Function WholeNumber(value As Integer, label As String) As String
            If value < 0 Then Return label & " cannot be negative."
            Return Nothing
        End Function
    End Class

    ' Non-disruptive real-time typing + paste restrictions for WPF TextBoxes.
    Public NotInheritable Class InputBehavior
        Private Sub New()
        End Sub

        Private NotInheritable Class KindHolder
            Public Property Value As String
        End Class

        Private Shared ReadOnly Kinds As New ConditionalWeakTable(Of TextBox, KindHolder)()

        Public Shared Sub Attach(box As TextBox, kind As String, Optional maxLength As Integer = 0)
            If box Is Nothing Then Return
            If maxLength > 0 Then box.MaxLength = maxLength
            Dim existing As KindHolder = Nothing
            If Kinds.TryGetValue(box, existing) Then Return
            Kinds.Add(box, New KindHolder With {.Value = kind})
            AddHandler box.PreviewTextInput, AddressOf OnInput
            DataObject.AddPastingHandler(box, AddressOf OnPaste)
        End Sub

        Private Shared Function Allowed(textValue As String, kind As String) As Boolean
            Select Case kind
                Case "name"
                    Return Regex.IsMatch(textValue, "^[\p{L}\p{M} '\u2019-]*$")
                Case "digits"
                    Return Regex.IsMatch(textValue, "^[0-9]*$")
                Case "money"
                    Return Regex.IsMatch(textValue, "^[0-9]*(?:[.,][0-9]{0,2})?$")
                Case "plate"
                    Return Regex.IsMatch(textValue, "^[A-Za-z0-9 -]*$")
                Case Else
                    Return True
            End Select
        End Function

        Private Shared Function Prospective(box As TextBox, entered As String) As String
            Return box.Text.Substring(0, box.SelectionStart) & entered &
                   box.Text.Substring(box.SelectionStart + box.SelectionLength)
        End Function

        Private Shared Sub OnInput(sender As Object, e As TextCompositionEventArgs)
            Dim box = TryCast(sender, TextBox)
            If box Is Nothing Then Return
            Dim holder As KindHolder = Nothing
            If Not Kinds.TryGetValue(box, holder) Then Return
            e.Handled = Not Allowed(Prospective(box, e.Text), holder.Value)
        End Sub

        Private Shared Sub OnPaste(sender As Object, e As DataObjectPastingEventArgs)
            Dim box = TryCast(sender, TextBox)
            If box Is Nothing Then Return
            Dim holder As KindHolder = Nothing
            If Not Kinds.TryGetValue(box, holder) Then Return
            If Not e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText) Then
                e.CancelCommand()
                Return
            End If
            Dim incoming = Convert.ToString(e.SourceDataObject.GetData(DataFormats.UnicodeText))
            If (box.MaxLength > 0 AndAlso Prospective(box, incoming).Length > box.MaxLength) OrElse
               Not Allowed(Prospective(box, incoming), holder.Value) Then
                e.CancelCommand()
            End If
        End Sub
    End Class
End Namespace
