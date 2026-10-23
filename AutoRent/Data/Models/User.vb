Namespace Data.Models
    Public Class User
        Public Property UserId As Integer
        Public Property Username As String = String.Empty
        Public Property PasswordHash As String = String.Empty
        Public Property Role As UserRole
        Public Property IsActive As Boolean
        Public Property IsDeleted As Boolean
        Public Property LastLogin As DateTime?
        Public Property CreatedAt As DateTime
    End Class
End Namespace
