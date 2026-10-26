Imports System.Windows
Imports AutoRent.Common.Utilities
Imports AutoRent.Data.Context
Imports AutoRent.Data.Models
Imports AutoRent.Data.Repositories

Partial Public Class Application
    Public Property CurrentUser As User

    Private Sub Application_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
        Try
            DatabaseInitializer.EnsureSchema()

            Dim generatedPassword = UserRepository.SeedAdminIfEmpty()
            If generatedPassword IsNot Nothing Then
                AlertUtil.ShowInformationWithCopy(
                    $"The initial Admin account was created.{Environment.NewLine}{Environment.NewLine}Username: admin{Environment.NewLine}Temporary password: {generatedPassword}{Environment.NewLine}{Environment.NewLine}Save this password now; it will not be displayed again.",
                    "Initial Admin Credentials")
            End If

        Catch ex As Exception
            AlertUtil.ShowError($"AutoRent could not initialize its user database: {ex.Message}", "Startup error")
            Shutdown()
        End Try
    End Sub

    Private Sub Application_Exit(sender As Object, e As ExitEventArgs) Handles Me.Exit
        Try
            If CurrentUser IsNot Nothing Then
                UserRepository.RecordLogout(CurrentUser.UserId)
            End If
        Catch ex As Exception
        End Try
    End Sub
End Class
