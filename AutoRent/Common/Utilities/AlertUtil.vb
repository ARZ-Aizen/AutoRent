Imports System.Windows
Imports System.Windows.Media

Namespace Common.Utilities
    Public NotInheritable Class AlertUtil
        Private Const DefaultTitle As String = "AutoRent"

        Private Sub New()
        End Sub

        Public Shared Sub ShowInformation(message As String, Optional title As String = DefaultTitle, Optional owner As Window = Nothing)
            ShowAlert(message, title, "i", Brushes.DodgerBlue, New SolidColorBrush(Color.FromRgb(232, 242, 255)), False, False, owner)
        End Sub

        Public Shared Sub ShowInformationWithCopy(message As String, Optional title As String = DefaultTitle, Optional owner As Window = Nothing)
            ShowAlert(message, title, "i", Brushes.DodgerBlue, New SolidColorBrush(Color.FromRgb(232, 242, 255)), False, True, owner)
        End Sub

        Public Shared Sub ShowSuccess(message As String, Optional title As String = DefaultTitle, Optional owner As Window = Nothing)
            ShowAlert(message, title, "✓", Brushes.SeaGreen, New SolidColorBrush(Color.FromRgb(232, 247, 239)), False, False, owner)
        End Sub

        Public Shared Sub ShowWarning(message As String, Optional title As String = DefaultTitle, Optional owner As Window = Nothing)
            ShowAlert(message, title, "!", Brushes.DarkOrange, New SolidColorBrush(Color.FromRgb(255, 245, 225)), False, False, owner)
        End Sub

        Public Shared Sub ShowError(message As String, Optional title As String = DefaultTitle, Optional owner As Window = Nothing)
            ShowAlert(message, title, "×", Brushes.Firebrick, New SolidColorBrush(Color.FromRgb(253, 235, 235)), False, False, owner)
        End Sub

        Public Shared Function Confirm(message As String, Optional title As String = DefaultTitle, Optional owner As Window = Nothing) As Boolean
            Return ShowAlert(message, title, "?", Brushes.Maroon, New SolidColorBrush(Color.FromRgb(247, 233, 235)), True, False, owner)
        End Function

        Private Shared Function ShowAlert(message As String, title As String, iconText As String, iconForeground As Brush, iconBackground As Brush, isConfirmation As Boolean, showCopyButton As Boolean, owner As Window) As Boolean
            If String.IsNullOrWhiteSpace(message) Then
                Throw New ArgumentException("A message is required.", NameOf(message))
            End If

            If String.IsNullOrWhiteSpace(title) Then
                title = DefaultTitle
            End If

            Dim dialog As New Global.AutoRent.AutoRent.AlertDialog(message, title, iconText, iconForeground, iconBackground, isConfirmation, showCopyButton, owner)
            Return dialog.ShowDialog().GetValueOrDefault(False)
        End Function
    End Class
End Namespace
