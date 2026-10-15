Imports System.ComponentModel

''' <summary>White panel with a thin light-gray border (used for all cards).</summary>
Public Class CardPanel
    Inherits Panel

    Public Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw, True)
        BackColor = Color.White
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)
        Using pen As New Pen(Color.FromArgb(225, 222, 218))
            e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1)
        End Using
    End Sub
End Class
