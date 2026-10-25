Imports System.Data
Imports System.Data.OleDb

Namespace Data.Context
    ''' <summary>
    ''' Small helper around OleDb so repositories stay short.
    ''' Parameters are positional (Access/OleDb ignores names), so pass them in the same order as the "?" in the SQL.
    ''' </summary>
    Public NotInheritable Class Db
        Private Sub New()
        End Sub

        ' ---------- Parameter factories (blank / Nothing becomes NULL) ----------

        Public Shared Function Text(value As String, Optional size As Integer = 255) As OleDbParameter
            Dim p As New OleDbParameter("p", OleDbType.VarWChar, size)
            If String.IsNullOrWhiteSpace(value) Then
                p.Value = DBNull.Value
            Else
                p.Value = value.Trim()
            End If
            Return p
        End Function

        Public Shared Function Memo(value As String) As OleDbParameter
            Dim p As New OleDbParameter("p", OleDbType.LongVarWChar)
            If String.IsNullOrWhiteSpace(value) Then
                p.Value = DBNull.Value
            Else
                p.Value = value.Trim()
            End If
            Return p
        End Function

        Public Shared Function Int(value As Integer) As OleDbParameter
            Return New OleDbParameter("p", OleDbType.Integer) With {.Value = value}
        End Function

        Public Shared Function IntOrNull(value As Integer?) As OleDbParameter
            Dim p As New OleDbParameter("p", OleDbType.Integer)
            If value.HasValue Then
                p.Value = value.Value
            Else
                p.Value = DBNull.Value
            End If
            Return p
        End Function

        Public Shared Function Money(value As Decimal) As OleDbParameter
            Return New OleDbParameter("p", OleDbType.Currency) With {.Value = value}
        End Function

        Public Shared Function Dt(value As DateTime) As OleDbParameter
            Return New OleDbParameter("p", OleDbType.Date) With {.Value = value}
        End Function

        Public Shared Function DtOrNull(value As DateTime?) As OleDbParameter
            Dim p As New OleDbParameter("p", OleDbType.Date)
            If value.HasValue Then
                p.Value = value.Value
            Else
                p.Value = DBNull.Value
            End If
            Return p
        End Function

        Public Shared Function Bool(value As Boolean) As OleDbParameter
            Return New OleDbParameter("p", OleDbType.Boolean) With {.Value = value}
        End Function

        ' ---------- Execution ----------

        Public Shared Function Query(sql As String, ParamArray args() As OleDbParameter) As DataTable
            Using connection = AccessDatabaseConnection.CreateConnection()
                connection.Open()
                Using command = connection.CreateCommand()
                    command.CommandText = sql
                    If args IsNot Nothing AndAlso args.Length > 0 Then command.Parameters.AddRange(args)
                    Using adapter As New OleDbDataAdapter(command)
                        Dim table As New DataTable()
                        adapter.Fill(table)
                        Return table
                    End Using
                End Using
            End Using
        End Function

        Public Shared Function Execute(sql As String, ParamArray args() As OleDbParameter) As Integer
            Using connection = AccessDatabaseConnection.CreateConnection()
                connection.Open()
                Using command = connection.CreateCommand()
                    command.CommandText = sql
                    If args IsNot Nothing AndAlso args.Length > 0 Then command.Parameters.AddRange(args)
                    Return command.ExecuteNonQuery()
                End Using
            End Using
        End Function

        Public Shared Function Scalar(sql As String, ParamArray args() As OleDbParameter) As Object
            Using connection = AccessDatabaseConnection.CreateConnection()
                connection.Open()
                Using command = connection.CreateCommand()
                    command.CommandText = sql
                    If args IsNot Nothing AndAlso args.Length > 0 Then command.Parameters.AddRange(args)
                    Return command.ExecuteScalar()
                End Using
            End Using
        End Function

        Public Shared Function Count(sql As String, ParamArray args() As OleDbParameter) As Integer
            Dim result = Scalar(sql, args)
            If result Is Nothing OrElse IsDBNull(result) Then Return 0
            Return Convert.ToInt32(result)
        End Function

        Public Shared Function Sum(sql As String, ParamArray args() As OleDbParameter) As Decimal
            Dim result = Scalar(sql, args)
            If result Is Nothing OrElse IsDBNull(result) Then Return 0D
            Return Convert.ToDecimal(result)
        End Function
    End Class
End Namespace
