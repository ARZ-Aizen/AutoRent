Imports System.Globalization
Imports System.Security.Cryptography

Namespace Common.Utilities
    Public NotInheritable Class PasswordHasher
        Private Const FormatVersion As String = "pbkdf2-sha256"
        Private Const IterationCount As Integer = 600000
        Private Const MaximumAcceptedIterations As Integer = 2000000
        Private Const SaltSizeBytes As Integer = 16
        Private Const HashSizeBytes As Integer = 32

        Private Sub New()
        End Sub

        Public Shared Function HashPassword(password As String) As String
            If password Is Nothing Then
                Throw New ArgumentNullException(NameOf(password))
            End If

            Dim salt(SaltSizeBytes - 1) As Byte
            RandomNumberGenerator.Fill(salt)

            Dim hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, IterationCount, HashAlgorithmName.SHA256, HashSizeBytes)
            Return String.Join("$", FormatVersion, IterationCount.ToString(CultureInfo.InvariantCulture), Convert.ToBase64String(salt), Convert.ToBase64String(hash))
        End Function

        Public Shared Function VerifyPassword(password As String, encodedHash As String) As Boolean
            If password Is Nothing OrElse String.IsNullOrEmpty(encodedHash) Then
                Return False
            End If

            Dim parts = encodedHash.Split("$"c)
            If parts.Length <> 4 OrElse Not String.Equals(parts(0), FormatVersion, StringComparison.Ordinal) Then
                Return False
            End If

            Dim iterations As Integer
            If Not Integer.TryParse(parts(1), NumberStyles.None, CultureInfo.InvariantCulture, iterations) OrElse iterations < 1 OrElse iterations > MaximumAcceptedIterations Then
                Return False
            End If

            Try
                Dim salt = Convert.FromBase64String(parts(2))
                Dim expectedHash = Convert.FromBase64String(parts(3))
                If salt.Length <> SaltSizeBytes OrElse expectedHash.Length <> HashSizeBytes Then
                    Return False
                End If

                Dim actualHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, HashSizeBytes)
                Return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash)
            Catch ex As FormatException
                Return False
            End Try
        End Function
    End Class
End Namespace
