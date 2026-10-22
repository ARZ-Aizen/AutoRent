Imports System.Windows
Imports AutoRent.Data.Repositories

Partial Public Class Application
    Private Sub Application_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
        Try
            Dim generatedPassword = UserRepository.SeedAdminIfEmpty()
            If generatedPassword IsNot Nothing Then
                MessageBox.Show(
                    $"The initial Admin account was created.{Environment.NewLine}{Environment.NewLine}Username: admin{Environment.NewLine}Temporary password: {generatedPassword}{Environment.NewLine}{Environment.NewLine}Save this password now; it will not be displayed again. Press Ctrl+C to copy this message.",
                    "Initial Admin Credentials",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information)
            End If

        Catch ex As Exception
            MessageBox.Show($"AutoRent could not initialize its user database: {ex.Message}", "Startup error", MessageBoxButton.OK, MessageBoxImage.Error)
            Shutdown()
        End Try
    End Sub
End Class
