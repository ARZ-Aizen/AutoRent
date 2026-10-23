Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media

Namespace AutoRent
    Partial Public Class AlertDialog
        Inherits Window

        Public Sub New(message As String, title As String, iconText As String, iconForeground As Brush, iconBackground As Brush, isConfirmation As Boolean, showCopyButton As Boolean, owner As Window)
            InitializeComponent()

            Me.WindowStyle = System.Windows.WindowStyle.None
            Me.AllowsTransparency = True
            Me.Background = Brushes.Transparent
            Dim dialogCard = TryCast(Content, Border)
            If dialogCard IsNot Nothing Then dialogCard.Margin = New Thickness(0)

            Me.Title = title
            HeaderText.Text = title
            MessageText.Text = message
            Me.IconText.Text = iconText
            Me.IconText.Foreground = iconForeground
            IconBadge.Background = iconBackground

            If owner Is Nothing Then
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            Else
                Me.Owner = owner
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            End If

            If isConfirmation Then
                PrimaryButton.Content = "Yes"
                CancelButton.Visibility = Visibility.Visible
                AddHandler CancelButton.Click, AddressOf CancelButton_Click
            End If

            If showCopyButton Then
                CopyButton.Visibility = Visibility.Visible
                AddHandler CopyButton.Click, AddressOf CopyButton_Click
            End If

            AddHandler PrimaryButton.Click, AddressOf PrimaryButton_Click
        End Sub

        Private Sub CopyButton_Click(sender As Object, e As RoutedEventArgs)
            Clipboard.SetText(MessageText.Text)
            CopyButton.Content = "Copied"
        End Sub

        Private Sub PrimaryButton_Click(sender As Object, e As RoutedEventArgs)
            DialogResult = True
        End Sub

        Private Sub CancelButton_Click(sender As Object, e As RoutedEventArgs)
            DialogResult = False
        End Sub
    End Class
End Namespace
