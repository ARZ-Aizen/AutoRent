Imports System.Globalization
Imports System.Security.Cryptography

Namespace Common.Utilities
    Public NotInheritable Class PasswordHasher
        Private Const LegacyFormatVersion As String = "pbkdf2-sha256"
        Private Const IterationCount As Integer = 600000
        Private Const MaximumAcceptedIterations As Integer = 2000000
        Private Const SaltSizeBytes As Integer = 16
        Private Const HashSizeBytes As Integer = 32

        Private Sub New()
        End Sub

        ''' <summary>
        ''' Generates an opaque combined binary hash (16-byte random salt + 32-byte PBKDF2-SHA256 key)
        ''' encoded as a clean 64-character Base64 string.
        ''' No algorithm name, version, or iteration parameters are exposed in plain text in the database.
        ''' </summary>
        Public Shared Function HashPassword(password As String) As String
            If password Is Nothing Then
                Throw New ArgumentNullException(NameOf(password))
            End If

            Dim salt(SaltSizeBytes - 1) As Byte
            RandomNumberGenerator.Fill(salt)

            Dim hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, IterationCount, HashAlgorithmName.SHA256, HashSizeBytes)

            ' Store combined 48 bytes (16-byte salt + 32-byte derived hash) as an opaque Base64 string
            Dim payload(SaltSizeBytes + HashSizeBytes - 1) As Byte
            Buffer.BlockCopy(salt, 0, payload, 0, SaltSizeBytes)
            Buffer.BlockCopy(hash, 0, payload, SaltSizeBytes, HashSizeBytes)

            Return Convert.ToBase64String(payload)
        End Function

        ''' <summary>
        ''' Returns True if the stored hash is already in the new opaque format (64-char Base64 without plain text algorithm headers).
        ''' </summary>
        Public Shared Function IsOpaqueHash(encodedHash As String) As Boolean
            If String.IsNullOrWhiteSpace(encodedHash) Then Return False
            Try
                Dim bytes = Convert.FromBase64String(encodedHash.Trim())
                Return bytes.Length = (SaltSizeBytes + HashSizeBytes)
            Catch
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Verifies a user password against the stored hash. Supports:
        ''' 1. Primary: Opaque format (no visible algorithm headers).
        ''' 2. Backward compatibility: Previous pbkdf2-sha256$... format and legacy SHA-256.
        ''' </summary>
        Public Shared Function VerifyPassword(password As String, encodedHash As String) As Boolean
            If password Is Nothing OrElse String.IsNullOrEmpty(encodedHash) Then
                Return False
            End If

            ' 1. Primary Opaque format verification: 48 bytes (16 salt + 32 hash)
            Try
                Dim payload = Convert.FromBase64String(encodedHash.Trim())
                If payload.Length = (SaltSizeBytes + HashSizeBytes) Then
                    Dim salt(SaltSizeBytes - 1) As Byte
                    Dim expectedHash(HashSizeBytes - 1) As Byte
                    Buffer.BlockCopy(payload, 0, salt, 0, SaltSizeBytes)
                    Buffer.BlockCopy(payload, SaltSizeBytes, expectedHash, 0, HashSizeBytes)

                    Dim actualHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, IterationCount, HashAlgorithmName.SHA256, HashSizeBytes)
                    Return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash)
                End If
            Catch ex As FormatException
            End Try

            ' 2. Backward compatibility fallback: previous header format (pbkdf2-sha256$iterations$salt$hash)
            If encodedHash.StartsWith(LegacyFormatVersion & "$", StringComparison.OrdinalIgnoreCase) Then
                Dim parts = encodedHash.Split("$"c)
                If parts.Length = 4 AndAlso String.Equals(parts(0), LegacyFormatVersion, StringComparison.OrdinalIgnoreCase) Then
                    Dim iterations As Integer
                    If Integer.TryParse(parts(1), NumberStyles.None, CultureInfo.InvariantCulture, iterations) AndAlso
                       iterations >= 1 AndAlso iterations <= MaximumAcceptedIterations Then
                        Try
                            Dim salt = Convert.FromBase64String(parts(2))
                            Dim expectedHash = Convert.FromBase64String(parts(3))
                            If salt.Length = SaltSizeBytes AndAlso expectedHash.Length = HashSizeBytes Then
                                Dim actualHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, HashSizeBytes)
                                Return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash)
                            End If
                        Catch ex As FormatException
                        End Try
                    End If
                End If
            End If

            ' 3. Legacy SHA-256 fallback (raw base64)
            Try
                Using sha = SHA256.Create()
                    Dim computedBytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password))
                    Dim computedBase64 = Convert.ToBase64String(computedBytes)
                    Return CryptographicOperations.FixedTimeEquals(
                        System.Text.Encoding.UTF8.GetBytes(computedBase64),
                        System.Text.Encoding.UTF8.GetBytes(encodedHash))
                End Using
            Catch
                Return False
            End Try
        End Function
    End Class
End Namespace
