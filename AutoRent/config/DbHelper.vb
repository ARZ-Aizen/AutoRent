Imports System.Data.OleDb
Imports System.IO
Imports System.Security.Cryptography

Public Module DbHelper

    Private ReadOnly DbPath As String = Path.Combine(Application.StartupPath, "AutoRent.accdb")
    Public ReadOnly Property ConnectionString As String
        Get
            Return "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & DbPath & ";"
        End Get
    End Property

    Public Function HashPassword(password As String) As String
        Dim salt(15) As Byte
        Using rng = RandomNumberGenerator.Create()
            rng.GetBytes(salt)
        End Using
        Using kdf As New Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256)
            Return Convert.ToBase64String(salt) & ":" & Convert.ToBase64String(kdf.GetBytes(32))
        End Using
    End Function

    Private Function VerifyPassword(password As String, stored As String) As Boolean
        Dim parts = stored.Split(":"c)
        If parts.Length <> 2 Then Return False
        Dim salt = Convert.FromBase64String(parts(0))
        Dim expected = Convert.FromBase64String(parts(1))
        Using kdf As New Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256)
            Dim actual = kdf.GetBytes(32)
            Dim diff As Integer = 0
            For i = 0 To expected.Length - 1
                diff = diff Or (expected(i) Xor actual(i))
            Next
            Return diff = 0
        End Using
    End Function

    ' ---------- Login ----------
    Public Function Authenticate(userInput As String, password As String,
                                 ByRef fullName As String, ByRef role As String) As Boolean
        Const sql As String =
            "SELECT PasswordHash, FullName, [Role] FROM Users " &
            "WHERE (Username = ? OR UserCode = ?) AND IsActive = True"

        Using conn As New OleDbConnection(ConnectionString)
            Using cmd As New OleDbCommand(sql, conn)
                cmd.Parameters.AddWithValue("?", userInput)
                cmd.Parameters.AddWithValue("?", userInput)
                conn.Open()
                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        If VerifyPassword(password, rd("PasswordHash").ToString()) Then
                            fullName = rd("FullName").ToString()
                            role = rd("Role").ToString()
                            Return True
                        End If
                    End If
                End Using
            End Using
        End Using
        Return False
    End Function

    ' ---------- Create a user ----------
    Public Sub CreateUser(userCode As String, username As String, password As String,
                          fullName As String, role As String, Optional isActive As Boolean = True)
        Const sql As String =
            "INSERT INTO Users (UserCode, Username, PasswordHash, FullName, [Role], IsActive) " &
            "VALUES (?, ?, ?, ?, ?, ?)"

        Using conn As New OleDbConnection(ConnectionString)
            Using cmd As New OleDbCommand(sql, conn)
                cmd.Parameters.AddWithValue("?", userCode)
                cmd.Parameters.AddWithValue("?", username)
                cmd.Parameters.AddWithValue("?", HashPassword(password))
                cmd.Parameters.AddWithValue("?", fullName)
                cmd.Parameters.AddWithValue("?", role)
                cmd.Parameters.AddWithValue("?", isActive)
                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

End Module