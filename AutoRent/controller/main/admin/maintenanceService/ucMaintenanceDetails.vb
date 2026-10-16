Imports System.Drawing.Drawing2D

' Maintenance Record Details screen. All data spots are empty: fill them from your code.
' Needs ucVehicleDetails.vb in the project (it holds the FiCard class used here).
Public Class ucMaintenanceDetails
    Inherits UserControl

    Private Shared ReadOnly Maroon As Color = Color.FromArgb(122, 0, 16)
    Private Shared ReadOnly Ink As Color = Color.FromArgb(38, 38, 38)
    Private Shared ReadOnly Muted As Color = Color.FromArgb(120, 120, 120)
    Private Shared ReadOnly BorderCol As Color = Color.FromArgb(228, 225, 220)
    Private Shared ReadOnly PageBg As Color = Color.FromArgb(245, 244, 242)

    Public Event ClearForRentalClicked()

    ' ---- header ----
    Public ReadOnly lblSubtitle As New Label
    Public ReadOnly lblStatusBadge As New Label

    ' ---- vehicle & scheduling ----
    Public ReadOnly picVehicle As New PictureBox
    Public ReadOnly lblVehStatus As New Label
    Public ReadOnly lblServiceDate As New Label
    Public ReadOnly lblProvider As New Label
    Public ReadOnly lblOdoPriority As New Label
    Public ReadOnly lblScheduledBy As New Label
    Public ReadOnly lblNextDate As New Label
    Public ReadOnly lblNextOdo As New Label
    Public ReadOnly lblDispatchTitle As New Label
    Public ReadOnly lblDispatchNote As New Label

    ' ---- service summary ----
    Public ReadOnly lblSummarySub As New Label
    Public ReadOnly lblEstCost As New Label
    Public ReadOnly lblLabor As New Label
    Public ReadOnly lblParts As New Label
    Public ReadOnly lblActual As New Label
    Public ReadOnly lblReportedIssue As New Label
    Public ReadOnly lblWorkPerformed As New Label

    ' ---- clear for rental ----
    Public ReadOnly lblClearSub As New Label
    Public ReadOnly chkWorkVerified As New CheckBox
    Public ReadOnly chkNoActiveRental As New CheckBox
    Public ReadOnly chkSafetyPassed As New CheckBox
    Public ReadOnly chkClearanceApproved As New CheckBox
    Public ReadOnly lblApprovalNote As New Label
    Public ReadOnly btnClearForRental As New Button

    ' ---- repair log ----
    Public ReadOnly lblLogSub As New Label
    Public ReadOnly dgvLog As New DataGridView

    Private cVehicle, cSummary, cClear, cLog As FiCard
    Private pnlDispatch, pnlApproval As Panel
    Private ReadOnly _rightCaps As New List(Of Control)

    Public Sub New()
        Me.BackColor = PageBg
        Me.AutoScroll = True
        Me.AutoScrollMinSize = New Size(900, 820)
        BuildUI()
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

    Private Shared Sub MultiLine(l As Label, x As Integer, y As Integer, w As Integer, h As Integer)
        l.AutoSize = False : l.Font = F(8.5!) : l.ForeColor = Ink : l.BackColor = Color.Transparent
        l.SetBounds(x, y, w, h) : l.UseMnemonic = False
    End Sub

    ' keeps ctl's right edge at (card.Width * fraction) - margin
    Private Shared Sub PinAt(card As Control, ctl As Control, fraction As Double, margin As Integer)
        Dim place As Action = Sub() ctl.Left = CInt(card.Width * fraction) - margin - ctl.Width
        AddHandler card.Resize, Sub(s, e) place()
        AddHandler ctl.SizeChanged, Sub(s, e) place()
    End Sub

    Private Shared Sub StyleCheck(c As CheckBox)
        c.AutoSize = True : c.Font = F(8.5!) : c.ForeColor = Ink
        c.BackColor = Color.Transparent : c.UseMnemonic = False
    End Sub

    Private Sub AddRow(card As FiCard, caption As String, value As Label, y As Integer)
        card.Controls.Add(Cap(caption, F(8), Muted, 18, y))
        Spot(value, F(8.5!, FontStyle.Bold), Ink, 0, y - 1)
        card.Controls.Add(value)
        PinAt(card, value, 1.0, 18)
    End Sub

    ' ---------------- layout ----------------
    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e)
        ArrangeLayout()
    End Sub

    Private Sub ArrangeLayout()
        If cLog Is Nothing Then Return
        Dim w As Integer = Math.Max(ClientSize.Width, 900) - 48
        Dim rw As Integer = w - 316
        lblStatusBadge.Location = New Point(24 + w - lblStatusBadge.Width, 34)
        cVehicle.SetBounds(24, 96, 300, 410)
        pnlDispatch.SetBounds(24, 516, 300, 66)
        cSummary.SetBounds(340, 96, rw, 214)
        cClear.SetBounds(340, 326, rw, 196)
        cLog.SetBounds(24, 600, w, 200)
    End Sub

    Private Sub BuildUI()
        Me.SuspendLayout()

        ' ----- header -----
        Controls.Add(Cap("Maintenance Record Details", F(20, FontStyle.Bold), Ink, 22, 14))
        Spot(lblSubtitle, F(9), Muted, 26, 58)
        Controls.Add(lblSubtitle)
        lblStatusBadge.AutoSize = True : lblStatusBadge.Font = F(8, FontStyle.Bold)
        lblStatusBadge.ForeColor = Color.FromArgb(176, 112, 32) : lblStatusBadge.BackColor = Color.Transparent
        lblStatusBadge.UseMnemonic = False
        Controls.Add(lblStatusBadge)
        AddHandler lblStatusBadge.SizeChanged, Sub(s, e) ArrangeLayout()

        ' ----- vehicle & scheduling card -----
        cVehicle = New FiCard
        cVehicle.Controls.Add(Cap("Vehicle & scheduling", F(11, FontStyle.Bold), Ink, 18, 12))
        picVehicle.SizeMode = PictureBoxSizeMode.Zoom
        picVehicle.BackColor = Color.White
        picVehicle.SetBounds(18, 46, 264, 118)
        cVehicle.Controls.Add(picVehicle)
        AddRow(cVehicle, "Current vehicle status", lblVehStatus, 180)
        AddRow(cVehicle, "Service date / scheduled", lblServiceDate, 210)
        AddRow(cVehicle, "Service provider", lblProvider, 240)
        AddRow(cVehicle, "Odometer / priority", lblOdoPriority, 270)
        AddRow(cVehicle, "Scheduled by", lblScheduledBy, 300)
        AddRow(cVehicle, "Next service date", lblNextDate, 330)
        AddRow(cVehicle, "Next service odometer", lblNextOdo, 360)
        Controls.Add(cVehicle)

        pnlDispatch = New Panel With {.BackColor = Color.FromArgb(252, 243, 228)}
        Spot(lblDispatchTitle, F(8.5!, FontStyle.Bold), Color.FromArgb(176, 112, 32), 10, 8)
        MultiLine(lblDispatchNote, 10, 26, 280, 36)
        lblDispatchNote.Font = F(7.5!) : lblDispatchNote.ForeColor = Muted
        pnlDispatch.Controls.AddRange(New Control() {lblDispatchTitle, lblDispatchNote})
        Controls.Add(pnlDispatch)

        ' ----- service summary card -----
        cSummary = New FiCard
        cSummary.Controls.Add(Cap("Service summary", F(11, FontStyle.Bold), Ink, 18, 12))
        Spot(lblSummarySub, F(7.5!), Muted, 18, 36)
        cSummary.Controls.Add(lblSummarySub)

        cSummary.Controls.Add(Cap("Estimated cost", F(8), Muted, 18, 62))
        Spot(lblEstCost, F(8.5!, FontStyle.Bold), Ink, 0, 61) : cSummary.Controls.Add(lblEstCost)
        PinAt(cSummary, lblEstCost, 0.5, 18)
        cSummary.Controls.Add(Cap("Parts", F(8), Muted, 18, 86))
        Spot(lblParts, F(8.5!, FontStyle.Bold), Ink, 0, 85) : cSummary.Controls.Add(lblParts)
        PinAt(cSummary, lblParts, 0.5, 18)

        Dim capLabor As Label = Cap("Labor", F(8), Muted, 0, 62)
        Dim capActual As Label = Cap("Actual cost / variance", F(8), Muted, 0, 86)
        cSummary.Controls.Add(capLabor) : cSummary.Controls.Add(capActual)
        Spot(lblLabor, F(8.5!, FontStyle.Bold), Ink, 0, 61) : cSummary.Controls.Add(lblLabor)
        Spot(lblActual, F(8.5!, FontStyle.Bold), Ink, 0, 85) : cSummary.Controls.Add(lblActual)
        PinAt(cSummary, lblLabor, 1.0, 18)
        PinAt(cSummary, lblActual, 1.0, 18)

        cSummary.Controls.Add(Cap("Reported issue", F(8.5!, FontStyle.Bold), Ink, 18, 116))
        MultiLine(lblReportedIssue, 18, 134, 100, 28) : lblReportedIssue.ForeColor = Muted
        cSummary.Controls.Add(lblReportedIssue)
        cSummary.Controls.Add(Cap("Work performed", F(8.5!, FontStyle.Bold), Ink, 18, 164))
        MultiLine(lblWorkPerformed, 18, 182, 100, 28) : lblWorkPerformed.ForeColor = Muted
        cSummary.Controls.Add(lblWorkPerformed)

        AddHandler cSummary.Resize, Sub(s, e)
                                        capLabor.Location = New Point(cSummary.Width \ 2 + 10, 62)
                                        capActual.Location = New Point(cSummary.Width \ 2 + 10, 86)
                                        lblReportedIssue.Width = cSummary.Width - 36
                                        lblWorkPerformed.Width = cSummary.Width - 36
                                    End Sub
        Controls.Add(cSummary)

        ' ----- clear for rental card -----
        cClear = New FiCard
        cClear.Controls.Add(Cap("Clear for rental", F(11, FontStyle.Bold), Ink, 18, 12))
        Spot(lblClearSub, F(7.5!), Muted, 18, 36)
        cClear.Controls.Add(lblClearSub)
        chkWorkVerified.Text = "Work completed and provider sign-off verified" : chkWorkVerified.Location = New Point(18, 62)
        chkSafetyPassed.Text = "Safety inspection passed" : chkSafetyPassed.Location = New Point(18, 86)
        chkNoActiveRental.Text = "No active rental or unresolved obligation" : chkNoActiveRental.Location = New Point(0, 62)
        chkClearanceApproved.Text = "Rental clearance approved by administrator" : chkClearanceApproved.Location = New Point(0, 86)
        For Each c As CheckBox In New CheckBox() {chkWorkVerified, chkSafetyPassed, chkNoActiveRental, chkClearanceApproved}
            StyleCheck(c) : cClear.Controls.Add(c)
        Next

        pnlApproval = New Panel With {.BackColor = PageBg, .Location = New Point(18, 118)}
        pnlApproval.Height = 62
        pnlApproval.Controls.Add(Cap("Approval required before release", F(8.5!, FontStyle.Bold), Muted, 10, 8))
        MultiLine(lblApprovalNote, 10, 26, 300, 34)
        lblApprovalNote.Font = F(7.5!) : lblApprovalNote.ForeColor = Muted
        pnlApproval.Controls.Add(lblApprovalNote)
        btnClearForRental.Text = "🛡  Clear for rental" : btnClearForRental.Size = New Size(150, 36)
        btnClearForRental.FlatStyle = FlatStyle.Flat : btnClearForRental.Font = F(9, FontStyle.Bold)
        btnClearForRental.BackColor = Maroon : btnClearForRental.ForeColor = Color.White
        btnClearForRental.FlatAppearance.BorderColor = Maroon : btnClearForRental.Cursor = Cursors.Hand
        btnClearForRental.UseVisualStyleBackColor = False : btnClearForRental.UseMnemonic = False
        pnlApproval.Controls.Add(btnClearForRental)
        cClear.Controls.Add(pnlApproval)

        AddHandler cClear.Resize, Sub(s, e)
                                      chkNoActiveRental.Left = cClear.Width \ 2 + 10
                                      chkClearanceApproved.Left = cClear.Width \ 2 + 10
                                      pnlApproval.Width = cClear.Width - 36
                                      btnClearForRental.Location = New Point(pnlApproval.Width - 10 - btnClearForRental.Width, 13)
                                      lblApprovalNote.Width = pnlApproval.Width - 20 - btnClearForRental.Width - 10
                                  End Sub
        Controls.Add(cClear)
        AddHandler btnClearForRental.Click, Sub(s, e) RaiseEvent ClearForRentalClicked()

        ' ----- repair log card -----
        cLog = New FiCard
        cLog.Controls.Add(Cap("Repair log & status audit trail", F(11, FontStyle.Bold), Ink, 18, 12))
        Spot(lblLogSub, F(7.5!), Muted, 18, 36)
        cLog.Controls.Add(lblLogSub)
        StyleLogGrid()
        cLog.Controls.Add(dgvLog)
        AddHandler cLog.Resize, Sub(s, e) dgvLog.SetBounds(1, 60, cLog.Width - 2, cLog.Height - 62)
        Controls.Add(cLog)

        Me.ResumeLayout(False)
        ArrangeLayout()
    End Sub

    Private Sub StyleLogGrid()
        With dgvLog
            .BackgroundColor = Color.White : .BorderStyle = BorderStyle.None
            .CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal : .GridColor = BorderCol
            .RowHeadersVisible = False : .AllowUserToAddRows = False : .AllowUserToDeleteRows = False
            .AllowUserToResizeRows = False : .ReadOnly = True : .MultiSelect = False
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .EnableHeadersVisualStyles = False
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
            .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            .ColumnHeadersHeight = 32 : .RowTemplate.Height = 34
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            .ScrollBars = ScrollBars.Vertical
            .ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(250, 249, 247)
            .ColumnHeadersDefaultCellStyle.ForeColor = Muted
            .ColumnHeadersDefaultCellStyle.Font = F(8)
            .ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(250, 249, 247)
            .ColumnHeadersDefaultCellStyle.SelectionForeColor = Muted
            .ColumnHeadersDefaultCellStyle.Padding = New Padding(12, 0, 0, 0)
            .DefaultCellStyle.Font = F(8.5!) : .DefaultCellStyle.ForeColor = Ink
            .DefaultCellStyle.SelectionBackColor = Color.FromArgb(250, 249, 247)
            .DefaultCellStyle.SelectionForeColor = Ink
            .DefaultCellStyle.Padding = New Padding(12, 0, 0, 0)
            .Columns.Add("colTime", "Timestamp")
            .Columns.Add("colEvent", "Event / work")
            .Columns.Add("colStaff", "Staff / provider")
            .Columns.Add("colCost", "Cost")
            .Columns.Add("colResult", "Result")
        End With
        AddHandler dgvLog.CellFormatting, Sub(s, e)
                                              If e.RowIndex >= 0 AndAlso e.ColumnIndex = 0 Then
                                                  e.CellStyle.ForeColor = Maroon
                                                  e.CellStyle.Font = F(8.5!, FontStyle.Bold)
                                              End If
                                          End Sub
        AddHandler dgvLog.CellPainting, Sub(s, e)
                                            If e.RowIndex >= 0 AndAlso e.ColumnIndex = 4 Then MaintDraw.Pill(e)
                                        End Sub
    End Sub
End Class

Friend Module MaintDraw
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

    ' Paints a coloured status pill in a grid cell. Colour comes from the text.
    Public Sub Pill(e As DataGridViewCellPaintingEventArgs)
        e.Paint(e.CellBounds, DataGridViewPaintParts.Background Or DataGridViewPaintParts.Border)
        Dim txt As String = Convert.ToString(e.Value)
        If txt <> "" Then
            Dim back As Color = Color.FromArgb(238, 238, 236)
            Dim fore As Color = Color.FromArgb(100, 100, 100)
            Dim key As String = txt.ToLower()
            If key.StartsWith("partial") Then key = "partial"
            Select Case key
                Case "complete", "completed", "completed & cleared", "available", "success", "paid", "returned"
                    back = Color.FromArgb(232, 240, 233) : fore = Color.FromArgb(46, 110, 60)
                Case "dispatch blocked", "urgent", "overdue", "cancelled"
                    back = Color.FromArgb(250, 228, 230) : fore = Color.FromArgb(122, 0, 16)
                Case "high", "reserved", "partial"
                    back = Color.FromArgb(252, 240, 220) : fore = Color.FromArgb(176, 112, 32)
            End Select
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
            Using f As New Font("Segoe UI", 7.5!, FontStyle.Bold)
                Dim sz As Size = TextRenderer.MeasureText(txt, f)
                Dim r As New Rectangle(e.CellBounds.Left + 12, e.CellBounds.Top + (e.CellBounds.Height - 20) \ 2, sz.Width + 14, 20)
                Using path As GraphicsPath = RoundRect(r, 4), br As New SolidBrush(back)
                    e.Graphics.FillPath(br, path)
                End Using
                TextRenderer.DrawText(e.Graphics, txt, f, r, fore,
                    TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter Or TextFormatFlags.NoPadding)
            End Using
        End If
        e.Handled = True
    End Sub
End Module