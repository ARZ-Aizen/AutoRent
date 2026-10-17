Imports System.Drawing.Drawing2D

' Shared helpers for ucPermissionMatrix, ucAuditViewer and ucSettingsPage.
Friend Module SK
    Public ReadOnly Maroon As Color = Color.FromArgb(122, 0, 16)
    Public ReadOnly Ink As Color = Color.FromArgb(38, 38, 38)
    Public ReadOnly Muted As Color = Color.FromArgb(120, 120, 120)
    Public ReadOnly Line As Color = Color.FromArgb(228, 225, 220)
    Public ReadOnly PageBg As Color = Color.FromArgb(245, 244, 242)

    Public Function F(size As Single, Optional style As FontStyle = FontStyle.Regular) As Font
        Return New Font("Segoe UI", size, style)
    End Function

    Public Function Cap(text As String, fnt As Font, c As Color, x As Integer, y As Integer) As Label
        Return New Label With {.Text = text, .Font = fnt, .ForeColor = c, .BackColor = Color.Transparent,
                               .AutoSize = True, .Location = New Point(x, y), .UseMnemonic = False}
    End Function

    Public Sub Spot(l As Label, fnt As Font, c As Color, Optional align As ContentAlignment = ContentAlignment.TopLeft)
        l.Font = fnt : l.ForeColor = c : l.BackColor = Color.Transparent
        l.AutoSize = False : l.UseMnemonic = False : l.TextAlign = align
    End Sub

    Public Sub StyleBtn(b As Button, text As String, w As Integer, h As Integer, primary As Boolean)
        b.Text = text : b.Size = New Size(w, h) : b.FlatStyle = FlatStyle.Flat
        b.Font = F(9) : b.Cursor = Cursors.Hand : b.UseVisualStyleBackColor = False : b.UseMnemonic = False
        b.BackColor = If(primary, Maroon, Color.White)
        b.ForeColor = If(primary, Color.White, Ink)
        b.FlatAppearance.BorderColor = If(primary, Maroon, Line)
        b.FlatAppearance.BorderSize = 1
    End Sub

    Public Sub StyleCombo(c As ComboBox)
        c.DropDownStyle = ComboBoxStyle.DropDownList
        c.FlatStyle = FlatStyle.Flat
        c.Font = F(9)
        c.BackColor = Color.White
    End Sub

    Public Sub StyleText(t As TextBox)
        t.Font = F(9) : t.BorderStyle = BorderStyle.FixedSingle
    End Sub

    Public Function RoundRect(r As Rectangle, radius As Integer) As GraphicsPath
        Dim d As Integer = radius * 2
        Dim p As New GraphicsPath()
        p.AddArc(r.X, r.Y, d, d, 180, 90)
        p.AddArc(r.Right - d, r.Y, d, d, 270, 90)
        p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90)
        p.AddArc(r.X, r.Bottom - d, d, d, 90, 90)
        p.CloseFigure()
        Return p
    End Function

    Public Sub StyleGrid(g As DataGridView, rowH As Integer)
        With g
            .BackgroundColor = Color.White : .BorderStyle = BorderStyle.None
            .CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal : .GridColor = Line
            .RowHeadersVisible = False : .AllowUserToAddRows = False : .AllowUserToDeleteRows = False
            .AllowUserToResizeRows = False : .ReadOnly = True : .MultiSelect = False
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .EnableHeadersVisualStyles = False
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
            .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            .ColumnHeadersHeight = 34
            .RowTemplate.Height = rowH
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            .ScrollBars = ScrollBars.Vertical
            .ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(250, 249, 247)
            .ColumnHeadersDefaultCellStyle.ForeColor = Muted
            .ColumnHeadersDefaultCellStyle.Font = F(8)
            .ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(250, 249, 247)
            .ColumnHeadersDefaultCellStyle.SelectionForeColor = Muted
            .ColumnHeadersDefaultCellStyle.Padding = New Padding(8, 0, 0, 0)
            .DefaultCellStyle.Font = F(8.5!)
            .DefaultCellStyle.ForeColor = Ink
            .DefaultCellStyle.SelectionBackColor = Color.FromArgb(250, 249, 247)
            .DefaultCellStyle.SelectionForeColor = Ink
            .DefaultCellStyle.Padding = New Padding(8, 0, 0, 0)
        End With
    End Sub

    Public Sub AddCol(g As DataGridView, colName As String, header As String, weight As Single)
        g.Columns.Add(New DataGridViewTextBoxColumn With {.Name = colName, .HeaderText = header, .FillWeight = weight,
                                                         .SortMode = DataGridViewColumnSortMode.NotSortable})
    End Sub

    ' coloured pill painted inside a grid cell (colour comes from the text)
    Public Sub Pill(e As DataGridViewCellPaintingEventArgs)
        e.Paint(e.CellBounds, DataGridViewPaintParts.Background Or DataGridViewPaintParts.Border)
        Dim txt As String = Convert.ToString(e.Value)
        If txt <> "" Then
            Dim back As Color = Color.FromArgb(238, 238, 236)
            Dim fore As Color = Color.FromArgb(100, 100, 100)
            Select Case txt.ToLower()
                Case "success", "active", "authorized - success"
                    back = Color.FromArgb(232, 240, 233) : fore = Color.FromArgb(46, 110, 60)
                Case "denied", "failed", "error"
                    back = Color.FromArgb(250, 228, 230) : fore = Maroon
            End Select
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
            Using fnt As New Font("Segoe UI", 7.5!, FontStyle.Bold)
                Dim sz As Size = TextRenderer.MeasureText(txt, fnt)
                Dim r As New Rectangle(e.CellBounds.Left + 8, e.CellBounds.Top + (e.CellBounds.Height - 20) \ 2, sz.Width + 14, 20)
                Using path As GraphicsPath = RoundRect(r, 4), br As New SolidBrush(back)
                    e.Graphics.FillPath(br, path)
                End Using
                TextRenderer.DrawText(e.Graphics, txt, fnt, r, fore,
                    TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter Or TextFormatFlags.NoPadding)
            End Using
        End If
        e.Handled = True
    End Sub
End Module