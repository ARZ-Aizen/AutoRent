Imports System.Data
Imports System.Globalization
Imports System.Windows

Namespace Common.Utilities
    Public NotInheritable Class ViewHelpers
        Private Sub New()
        End Sub

        Public Shared Function Str(row As DataRowView, column As String) As String
            If row Is Nothing OrElse IsDBNull(row(column)) Then Return String.Empty
            Return Convert.ToString(row(column))
        End Function

        Public Shared Function Str(row As DataRow, column As String) As String
            If row Is Nothing OrElse IsDBNull(row(column)) Then Return String.Empty
            Return Convert.ToString(row(column))
        End Function

        Public Shared Function IntOf(row As DataRowView, column As String) As Integer
            If row Is Nothing OrElse IsDBNull(row(column)) Then Return 0
            Return Convert.ToInt32(row(column))
        End Function

        Public Shared Function IntOf(row As DataRow, column As String) As Integer
            If row Is Nothing OrElse IsDBNull(row(column)) Then Return 0
            Return Convert.ToInt32(row(column))
        End Function

        Public Shared Function DecOf(row As DataRowView, column As String) As Decimal
            If row Is Nothing OrElse IsDBNull(row(column)) Then Return 0D
            Return Convert.ToDecimal(row(column))
        End Function

        Public Shared Function DecOf(row As DataRow, column As String) As Decimal
            If row Is Nothing OrElse IsDBNull(row(column)) Then Return 0D
            Return Convert.ToDecimal(row(column))
        End Function

        Public Shared Function TryParseMoney(text As String, ByRef value As Decimal) As Boolean
            Return Decimal.TryParse(If(text, "").Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, value) AndAlso value >= 0D
        End Function

        Public Shared Function TryParseWhole(text As String, ByRef value As Integer) As Boolean
            Return Integer.TryParse(If(text, "").Trim(), NumberStyles.Integer, CultureInfo.CurrentCulture, value) AndAlso value >= 0
        End Function

        Public Shared Function EscapeLike(value As String) As String
            Dim sb As New System.Text.StringBuilder()
            For Each ch In value
                Select Case ch
                    Case "'"c
                        sb.Append("''")
                    Case "["c, "]"c, "%"c, "*"c
                        sb.Append("[").Append(ch).Append("]")
                    Case Else
                        sb.Append(ch)
                End Select
            Next
            Return sb.ToString()
        End Function

        Public Shared Function Owner(element As DependencyObject) As Window
            Return Window.GetWindow(element)
        End Function
    End Class
End Namespace
