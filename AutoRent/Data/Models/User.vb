Imports System.Security.Cryptography

Namespace Data.Models
    Public Class User
        Public Property UserId As Integer
        Public Property Username As String = String.Empty
        Public Property FullName As String = String.Empty
        Public Property Branch As String = "HQ"
        Public Property PasswordHash As String = String.Empty
        Public Property Role As UserRole
        Public Property RoleName As String = String.Empty
        Public Property IsActive As Boolean
        Public Property IsDeleted As Boolean
        Public Property LastLogin As DateTime?
        Public Property CreatedAt As DateTime

        Public ReadOnly Property UserCode As String
            Get
                If UserId <= 0 Then Return "USR-00000"
                Dim rawBytes = System.Text.Encoding.UTF8.GetBytes($"AutoRent_Secure_Salt_{UserId}")
                Using sha = SHA256.Create()
                    Dim hash = sha.ComputeHash(rawBytes)
                    Dim val = BitConverter.ToUInt32(hash, 0)
                    Dim codeNum = 10000L + (CLng(val) Mod 90000L)
                    Return $"USR-{codeNum}"
                End Using
            End Get
        End Property

        Public ReadOnly Property DisplayName As String
            Get
                If Not String.IsNullOrWhiteSpace(FullName) Then Return FullName
                Return Username
            End Get
        End Property

        Public ReadOnly Property DisplayRole As String
            Get
                If Not String.IsNullOrWhiteSpace(RoleName) Then
                    Select Case RoleName.Trim().ToLowerInvariant()
                        Case "admin": Return "System Administrator"
                        Case "user": Return "Rental Clerk"
                        Case Else: Return RoleName
                    End Select
                End If
                If Role = UserRole.Admin Then Return "System Administrator"
                Return "Rental Clerk"
            End Get
        End Property

        Public ReadOnly Property StatusBadge As String
            Get
                If Not IsActive Then Return "Deactivated"
                If Not LastLogin.HasValue Then Return "Invite pending"
                Return "Active"
            End Get
        End Property

        Public ReadOnly Property LastLoginFormatted As String
            Get
                If Not LastLogin.HasValue Then Return "Never"
                Return LastLogin.Value.ToString("dd MMM · HH:mm")
            End Get
        End Property

        Public ReadOnly Property ActionText As String
            Get
                If StatusBadge = "Invite pending" Then Return "Edit · Resend Invite"
                If StatusBadge = "Deactivated" Then Return "Reactivate · Activity"
                Return "Edit · Activity"
            End Get
        End Property

        Public ReadOnly Property InvitationText As String
            Get
                If CreatedAt <> DateTime.MinValue Then
                    Return $"Issued {CreatedAt:dd MMM · HH:mm}"
                End If
                Return "Issued on account creation"
            End Get
        End Property

        Public ReadOnly Property LastActivityText As String
            Get
                If LastLogin.HasValue Then
                    Return $"Authenticated · {LastLogin.Value:dd MMM, HH:mm}"
                End If
                Return "Account created · Pending first sign-in"
            End Get
        End Property

        Public ReadOnly Property AccountAuthority As String
            Get
                If Role = UserRole.Admin OrElse RoleName.Equals("Admin", StringComparison.OrdinalIgnoreCase) Then
                    Return $"All branches ({Branch})"
                End If
                Return $"{Branch} branch only"
            End Get
        End Property

        Public ReadOnly Property StatusSubtitle As String
            Get
                If StatusBadge = "Invite pending" Then
                    Return $"{UserCode} · secure invitation pending after account update"
                ElseIf StatusBadge = "Deactivated" Then
                    Return $"{UserCode} · account deactivated · credentials suspended"
                End If
                Return $"{UserCode} · active account verified in good standing"
            End Get
        End Property

        Public ReadOnly Property user_code As String
            Get
                Return UserCode
            End Get
        End Property

        Public ReadOnly Property full_name As String
            Get
                Return DisplayName
            End Get
        End Property

        Public ReadOnly Property display_role As String
            Get
                Return DisplayRole
            End Get
        End Property

        Public ReadOnly Property status_badge As String
            Get
                Return StatusBadge
            End Get
        End Property

        Public ReadOnly Property last_login_formatted As String
            Get
                Return LastLoginFormatted
            End Get
        End Property

        Public ReadOnly Property action_text As String
            Get
                Return ActionText
            End Get
        End Property

        Public ReadOnly Property user_id As Integer
            Get
                Return UserId
            End Get
        End Property
    End Class
End Namespace
