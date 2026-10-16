Imports System.Drawing.Drawing2D

' Fleet Inventory > Vehicle Details layout. All data spots are empty: fill them from your code.
Public Class ucVehicleDetails
    Inherits UserControl

    Private Shared ReadOnly Maroon As Color = Color.FromArgb(122, 0, 16)
    Private Shared ReadOnly Ink As Color = Color.FromArgb(38, 38, 38)
    Private Shared ReadOnly Muted As Color = Color.FromArgb(120, 120, 120)
    Private Shared ReadOnly BorderCol As Color = Color.FromArgb(228, 225, 220)
    Private Shared ReadOnly PageBg As Color = Color.FromArgb(245, 244, 242)

    Public Event ViewHistoryRequested()

    ' ---- buttons ----
    Public ReadOnly btnChangeStatus As New Button
    Public ReadOnly btnEditVehicle As New Button
    Public ReadOnly btnViewHistory As New Button

    ' ---- data spots (set .Text) ----
    Public ReadOnly lblSubtitle As New Label
    Public ReadOnly lblVehicleName As New Label
    Public ReadOnly lblVehicleSub As New Label
    Public ReadOnly lblStatus As New Label
    Public ReadOnly pnlPhoto As New Panel
    Public ReadOnly lblRate As New Label
    Public ReadOnly lblVehicleId As New Label
    Public ReadOnly lblCategory As New Label
    Public ReadOnly lblFuel As New Label
    Public ReadOnly lblOdometer As New Label
    Public ReadOnly lblAcquired As New Label
    Public ReadOnly lblDocsStatus As New Label
    Public ReadOnly lblRegExpires As New Label
    Public ReadOnly lblInsExpires As New Label
    Public ReadOnly lblLastInspection As New Label
    Public ReadOnly lblNextService As New Label
    Public ReadOnly lblNotes As New Label
    Public ReadOnly lblTransitionNote As New Label
    Public ReadOnly dgvHistory As New DataGridView
    Public ReadOnly dgvStatus As New DataGridView

    Private cLeft, cComp, cHist, cStat As FiCard

    Public Sub New()
        Me.DoubleBuffered = True
        Me.BackColor = PageBg
        Me.AutoScroll = True
        Me.AutoScrollMinSize = New Size(900, 780)
        BuildUI()
        AddHandler btnViewHistory.Click, Sub(s, e) RaiseEvent ViewHistoryRequested()
    End Sub

    ' ---------------- helpers ----------------
    Private Shared Function F(size As Single, Optional style As FontStyle = FontStyle.Regular) As Font
        Return New Font("Segoe UI", size, style)
    End Function

    Private Shared Function Cap(text As String, f As Font, c As Color, x As Integer, y As Integer) As Label
        Return New Label With {.Text = text, .Font = f, .ForeColor = c, .BackColor = Color.Transparent,
                               .AutoSize = True, .Location = New Point(x, y), .UseMnemonic = False}
    End Function

    Private Shared Sub Spot(l As Label, f As Font, c As Color, x As Integer, y As Integer)
        l.Font = f : l.ForeColor = c : l.BackColor = Color.Transparent
        l.AutoSize = True : l.Location = New Point(x, y) : l.UseMnemonic = False
    End Sub

    Private Shared Sub PinRight(card As Control, ctl As Control, margin As Integer)
        AddHandler card.Resize, Sub(s, e) ctl.Left = card.Width - margin - ctl.Width
        AddHandler ctl.SizeChanged, Sub(s, e) ctl.Left = card.Width - margin - ctl.Width
    End Sub

    Private Shared Sub Badge(l As Label, w As Integer, back As Color, fore As Color)
        l.AutoSize = False : l.Size = New Size(w, 20)
        l.Font = F(7.5!, FontStyle.Bold) : l.BackColor = back : l.ForeColor = fore
        l.TextAlign = ContentAlignment.MiddleCenter : l.UseMnemonic = False
    End Sub

    Private Shared Sub StyleBtn(b As Button, text As String, w As Integer, h As Integer, primary As Boolean)
        b.Text = text : b.Size = New Size(w, h) : b.FlatStyle = FlatStyle.Flat
        b.Font = F(9) : b.Cursor = Cursors.Hand : b.UseVisualStyleBackColor = False : b.UseMnemonic = False
        b.BackColor = If(primary, Maroon, Color.White)
        b.ForeColor = If(primary, Color.White, Ink)
        b.FlatAppearance.BorderColor = If(primary, Maroon, BorderCol)
        b.FlatAppearance.BorderSize = 1
    End Sub

    Private Shared Sub StyleGrid(g As DataGridView, ParamArray headers() As String)
        g.BackgroundColor = Color.White : g.BorderStyle = BorderStyle.None
        g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal : g.GridColor = BorderCol
        g.RowHeadersVisible = False : g.AllowUserToAddRows = False : g.AllowUserToDeleteRows = False
        g.AllowUserToResizeRows = False : g.ReadOnly = True : g.MultiSelect = False
        g.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        g.EnableHeadersVisualStyles = False
        g.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        g.ColumnHeadersHeight = 32
        g.RowTemplate.Height = 34
        g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        g.ScrollBars = ScrollBars.Vertical
        g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(250, 249, 247)
        g.ColumnHeadersDefaultCellStyle.ForeColor = Muted
        g.ColumnHeadersDefaultCellStyle.Font = F(8)
        g.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(250, 249, 247)
        g.ColumnHeadersDefaultCellStyle.SelectionForeColor = Muted
        g.ColumnHeadersDefaultCellStyle.Padding = New Padding(12, 0, 0, 0)
        g.DefaultCellStyle.Font = F(8.5!)
        g.DefaultCellStyle.ForeColor = Ink
        g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(250, 249, 247)
        g.DefaultCellStyle.SelectionForeColor = Ink
        g.DefaultCellStyle.Padding = New Padding(12, 0, 0, 0)
        For i As Integer = 0 To headers.Length - 1
            g.Columns.Add("col" & i, headers(i))
        Next
    End Sub

    ' ---------------- layout ----------------
    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e)
        ArrangeLayout()
    End Sub

    Private Sub ArrangeLayout()
        If cStat Is Nothing Then Return
        Dim w As Integer = Math.Max(ClientSize.Width, 900) - 48
        btnEditVehicle.Location = New Point(24 + w - btnEditVehicle.Width, 20)
        btnChangeStatus.Location = New Point(btnEditVehicle.Left - 8 - btnChangeStatus.Width, 20)
        cLeft.SetBounds(24, 100, 300, 430)
        cComp.SetBounds(340, 100, w - 316, 200)
        cHist.SetBounds(340, 316, w - 316, 214)
        cStat.SetBounds(24, 546, w, 200)
    End Sub

    Private Sub BuildUI()
        Me.SuspendLayout()

        Controls.Add(Cap("Vehicle Details", F(20, FontStyle.Bold), Ink, 22, 14))
        Spot(lblSubtitle, F(9), Muted, 26, 58)
        Controls.Add(lblSubtitle)
        StyleBtn(btnChangeStatus, "Change status", 120, 34, False)
        StyleBtn(btnEditVehicle, "Edit Vehicle", 120, 34, True)
        Controls.Add(btnChangeStatus)
        Controls.Add(btnEditVehicle)

        ' ===== left card: vehicle summary =====
        cLeft = New FiCard
        Spot(lblVehicleName, F(11, FontStyle.Bold), Ink, 18, 14)
        Spot(lblVehicleSub, F(7.5!), Muted, 18, 38)
        Badge(lblStatus, 70, Color.FromArgb(232, 240, 233), Color.FromArgb(46, 110, 60))
        lblStatus.Top = 14
        pnlPhoto.BackColor = PageBg
        pnlPhoto.SetBounds(18, 64, 264, 150)
        cLeft.Controls.AddRange(New Control() {lblVehicleName, lblVehicleSub, lblStatus, pnlPhoto})
        PinRight(cLeft, lblStatus, 18)

        cLeft.Controls.Add(Cap("BASE DAILY RATE", F(7, FontStyle.Bold), Muted, 18, 236))
        Spot(lblRate, F(20, FontStyle.Bold), Maroon, 0, 222)
        cLeft.Controls.Add(lblRate)
        PinRight(cLeft, lblRate, 18)

        AddRow(cLeft, "Vehicle ID / plate", lblVehicleId, 276)
        AddRow(cLeft, "Category / seats", lblCategory, 304)
        AddRow(cLeft, "Fuel / transmission", lblFuel, 332)
        AddRow(cLeft, "Odometer", lblOdometer, 360)
        AddRow(cLeft, "Acquired", lblAcquired, 388)
        AddHandler cLeft.Resize, Sub(s, e) pnlPhoto.Width = cLeft.Width - 36
        Controls.Add(cLeft)

        ' ===== compliance card =====
        cComp = New FiCard
        cComp.Controls.Add(Cap("Compliance & operating notes", F(11, FontStyle.Bold), Ink, 18, 12))
        Badge(lblDocsStatus, 100, Color.FromArgb(232, 240, 233), Color.FromArgb(46, 110, 60))
        lblDocsStatus.Top = 12
        cComp.Controls.Add(lblDocsStatus)
        PinRight(cComp, lblDocsStatus, 18)

        cComp.Controls.Add(Cap("Registration expires", F(8), Muted, 18, 50))
        Spot(lblRegExpires, F(8.5!, FontStyle.Bold), Ink, 150, 49)
        cComp.Controls.Add(lblRegExpires)
        cComp.Controls.Add(Cap("Insurance expires", F(8), Muted, 18, 78))
        Spot(lblInsExpires, F(8.5!, FontStyle.Bold), Ink, 150, 77)
        cComp.Controls.Add(lblInsExpires)

        Dim capInsp As Label = Cap("Last inspection", F(8), Muted, 0, 50)
        Dim capNext As Label = Cap("Next service", F(8), Muted, 0, 78)
        cComp.Controls.Add(capInsp) : cComp.Controls.Add(capNext)
        Spot(lblLastInspection, F(8.5!, FontStyle.Bold), Ink, 0, 49)
        Spot(lblNextService, F(8.5!, FontStyle.Bold), Ink, 0, 77)
        cComp.Controls.Add(lblLastInspection) : cComp.Controls.Add(lblNextService)
        PinRight(cComp, lblLastInspection, 18)
        PinRight(cComp, lblNextService, 18)

        lblNotes.AutoSize = False : lblNotes.Font = F(8.5!) : lblNotes.ForeColor = Muted
        lblNotes.BackColor = Color.Transparent : lblNotes.Location = New Point(18, 106)
        cComp.Controls.Add(lblNotes)

        Dim box As New Panel With {.BackColor = PageBg, .Location = New Point(18, 148), .Height = 42}
        box.Controls.Add(Cap("Safe vehicle transitions", F(8, FontStyle.Bold), Ink, 10, 4))
        lblTransitionNote.AutoSize = False : lblTransitionNote.Font = F(7.5!) : lblTransitionNote.ForeColor = Muted
        lblTransitionNote.BackColor = Color.Transparent : lblTransitionNote.Location = New Point(10, 20)
        lblTransitionNote.Height = 20
        box.Controls.Add(lblTransitionNote)
        cComp.Controls.Add(box)

        AddHandler cComp.Resize, Sub(s, e)
                                     Dim w As Integer = cComp.Width
                                     capInsp.Left = w \ 2 + 10
                                     capNext.Left = w \ 2 + 10
                                     lblNotes.Size = New Size(w - 36, 36)
                                     box.Width = w - 36
                                     lblTransitionNote.Width = box.Width - 20
                                 End Sub
        Controls.Add(cComp)

        ' ===== rental & inspection history =====
        cHist = New FiCard
        cHist.Controls.Add(Cap("Rental & inspection history", F(11, FontStyle.Bold), Ink, 18, 12))
        cHist.Controls.Add(Cap("Latest completed records · all records retained", F(7.5!), Muted, 18, 36))
        StyleBtn(btnViewHistory, "View all history", 130, 30, False)
        btnViewHistory.Top = 12
        cHist.Controls.Add(btnViewHistory)
        PinRight(cHist, btnViewHistory, 18)
        StyleGrid(dgvHistory, "Booking / Inspection", "Customer / Inspector", "Period / Date", "Odometer", "Result")
        cHist.Controls.Add(dgvHistory)
        AddHandler cHist.Resize, Sub(s, e) dgvHistory.SetBounds(1, 62, cHist.Width - 2, cHist.Height - 64)
        Controls.Add(cHist)

        ' ===== dated status history =====
        cStat = New FiCard
        cStat.Controls.Add(Cap("Dated status history", F(11, FontStyle.Bold), Ink, 18, 12))
        cStat.Controls.Add(Cap("Immutable status audit trail", F(7.5!), Muted, 18, 36))
        StyleGrid(dgvStatus, "Timestamp", "Transition", "Authorized staff", "Reason", "Checks")
        cStat.Controls.Add(dgvStatus)
        AddHandler cStat.Resize, Sub(s, e) dgvStatus.SetBounds(1, 62, cStat.Width - 2, cStat.Height - 64)
        Controls.Add(cStat)

        Me.ResumeLayout(False)
        ArrangeLayout()
    End Sub

    Private Sub AddRow(card As FiCard, caption As String, value As Label, y As Integer)
        card.Controls.Add(Cap(caption, F(8), Muted, 18, y))
        Spot(value, F(8.5!, FontStyle.Bold), Ink, 0, y - 1)
        card.Controls.Add(value)
        PinRight(card, value, 18)
    End Sub
End Class

Friend Class FiCard
    Inherits Panel

    Public Sub New()
        SetStyle(ControlStyles.UserPaint Or ControlStyles.AllPaintingInWmPaint Or
                 ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw, True)
        Me.BackColor = Color.White
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim g As Graphics = e.Graphics
        g.Clear(Color.FromArgb(245, 244, 242))
        g.SmoothingMode = SmoothingMode.AntiAlias
        Dim r As New Rectangle(0, 0, Width - 1, Height - 1)
        Dim d As Integer = 16
        Using p As New GraphicsPath(), br As New SolidBrush(Color.White), pen As New Pen(Color.FromArgb(228, 225, 220))
            p.AddArc(r.X, r.Y, d, d, 180, 90)
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90)
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90)
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90)
            p.CloseFigure()
            g.FillPath(br, p)
            g.DrawPath(pen, p)
        End Using
    End Sub
End Class