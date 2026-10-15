Imports System.Drawing.Drawing2D
Imports System.Reflection

Public Class ucFrontDeskDashboard

    Public Event ViewFullInventoryRequested()
    Public Event NewBookingRequested()

    Private ReadOnly Maroon As Color = Color.FromArgb(122, 0, 16)
    Private ReadOnly Ink As Color = Color.FromArgb(38, 38, 38)
    Private ReadOnly Green As Color = Color.FromArgb(30, 130, 70)

    Private Sub ucFrontDeskDashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        EnableDoubleBuffer(dgvDispatch)
        For i = 1 To 4
            MakeRounded(Controls.Find("pnlKpiIcon" & i, True)(0), 10)
            MakeRounded(Controls.Find("lblKpiBadge" & i, True)(0), 4)
            MakeRounded(Controls.Find("lblRetEta" & i, True)(0), 4)
            MakeRounded(Controls.Find("lblRetIcon" & i, True)(0), 8)
        Next
        LoadDashboard()
        SetDispatchTab(btnTabPending)
        SetPage(btnPage1)
    End Sub

    ' =================================================================
    '  DATA  -  demo values, replace with queries to AutoRent.accdb
    ' =================================================================
    Public Sub LoadDashboard()
        ' KPI cards
        SetText("lblKpiValue", 1, "42")
        SetText("lblKpiValue", 2, "128")
        SetText("lblKpiValue", 3, "15")
        SetText("lblKpiValue", 4, "08")

        ' fleet categories: index, available, reserved, rented, maintenance
        SetCategory(1, 12, 4, 18, 2)
        SetCategory(2, 8, 6, 22, 1)
        SetCategory(3, 5, 2, 12, 0)
        SetCategory(4, 17, 3, 10, 4)

        ' returns today: index, vehicle, customer, time, eta text
        SetReturn(1, "BENTLEY CONTINENTAL GT", "Marcus Thorne • 14:00", "ETA 15m", True)
        SetReturn(2, "MERCEDES S-CLASS", "Elena Rodriguez • 15:30", "In 1h 45m", False)
        SetReturn(3, "RANGE ROVER VOGUE", "Arthur Pendragon • 16:15", "In 2h 30m", False)
        SetReturn(4, "TESLA MODEL S PLAID", "Sarah Jenkins • 17:00", "In 3h 15m", False)

        ' dispatch schedule (two-line cells are split with a line feed)
        dgvDispatch.Rows.Clear()
        dgvDispatch.Rows.Add("#CR-98421", "Vivienne Westwood" & vbLf & "PREMIUM MEMBER",
                             "Porsche Panamera GTS" & vbLf & "ABC-1234 • RED",
                             "Today, 09:30 AM" & vbLf & "READY FOR PICKUP", "Oct 27, 10:00 AM", "AWAITING CHECK-OUT", "CHECK OUT")
        dgvDispatch.Rows.Add("#CR-98425", "Sebastian Vance" & vbLf & "STANDARD CLIENT",
                             "BMW X7 xDrive40i" & vbLf & "XYZ-7890 • BLACK",
                             "Today, 11:00 AM" & vbLf & "CLEANING IN PROGRESS", "Oct 26, 02:00 PM", "IN PREPARATION", "CHECK OUT")
        dgvDispatch.Rows.Add("#CR-98428", "Clarissa Thorne" & vbLf & "GOLD TIER",
                             "Audi A8 L60 TFSI" & vbLf & "QWE-5544 • SILVER",
                             "Today, 12:45 PM" & vbLf & "INSPECTION COMPLETE", "Oct 30, 09:00 AM", "AWAITING CHECK-OUT", "CHECK OUT")
    End Sub

    Private Sub SetText(prefix As String, index As Integer, value As String)
        Controls.Find(prefix & index, True)(0).Text = value
    End Sub

    Private Sub SetCategory(i As Integer, available As Integer, reserved As Integer, rented As Integer, maint As Integer)
        SetText("lblCatAvail", i, available.ToString("00"))
        SetText("lblCatRes", i, "RES: " & reserved)
        SetText("lblCatRent", i, "RENT: " & rented)
        SetText("lblCatMaint", i, "MAINT: " & maint)
        Dim total As Integer = Math.Max(1, available + reserved + rented + maint)
        Dim bar = Controls.Find("pnlCatBar" & i, True)(0)
        bar.Tag = CSng(available / total)
        bar.Invalidate()
    End Sub

    Private Sub SetReturn(i As Integer, vehicle As String, subText As String, eta As String, highlight As Boolean)
        SetText("lblRetName", i, vehicle)
        SetText("lblRetSub", i, subText)
        Dim lbl = Controls.Find("lblRetEta" & i, True)(0)
        lbl.Text = eta
        lbl.BackColor = If(highlight, Color.FromArgb(232, 240, 252), Color.FromArgb(242, 241, 239))
        lbl.ForeColor = If(highlight, Color.FromArgb(30, 90, 200), Color.FromArgb(120, 120, 120))
    End Sub

    ' =================================================================
    '  EVENTS
    ' =================================================================
    Private Sub lblViewInventory_Click(sender As Object, e As EventArgs) Handles lblViewInventory.Click
        RaiseEvent ViewFullInventoryRequested()
    End Sub

    Private Sub btnNewBooking_Click(sender As Object, e As EventArgs) Handles btnNewBooking.Click
        RaiseEvent NewBookingRequested()
    End Sub

    Private Sub DispatchTab_Click(sender As Object, e As EventArgs) Handles btnTabPending.Click, btnTabActive.Click, btnTabCompleted.Click
        SetDispatchTab(DirectCast(sender, Button))
        ' TODO: reload dgvDispatch with Pending / Active / Completed bookings
    End Sub

    Private Sub Page_Click(sender As Object, e As EventArgs) Handles btnPage1.Click, btnPage2.Click, btnPage3.Click
        SetPage(DirectCast(sender, Button))
        ' TODO: load the selected page of bookings
    End Sub

    Private Sub SetDispatchTab(active As Button)
        For Each b In {btnTabPending, btnTabActive, btnTabCompleted}
            Dim isOn As Boolean = (b Is active)
            b.BackColor = If(isOn, Color.White, Color.FromArgb(64, 64, 64))
            b.ForeColor = If(isOn, Ink, Color.FromArgb(200, 200, 200))
            b.Font = New Font("Segoe UI", 7.5F, If(isOn, FontStyle.Bold, FontStyle.Regular))
        Next
    End Sub

    Private Sub SetPage(active As Button)
        For Each b In {btnPage1, btnPage2, btnPage3}
            Dim isOn As Boolean = (b Is active)
            b.BackColor = If(isOn, Ink, Color.White)
            b.ForeColor = If(isOn, Color.White, Color.FromArgb(90, 90, 90))
        Next
    End Sub

    ' =================================================================
    '  CUSTOM DRAWING - category bars
    ' =================================================================
    Private Sub CatBar_Paint(sender As Object, e As PaintEventArgs) Handles pnlCatBar1.Paint, pnlCatBar2.Paint, pnlCatBar3.Paint, pnlCatBar4.Paint
        Dim p = DirectCast(sender, Panel)
        Dim fraction As Single = If(p.Tag IsNot Nothing, CSng(p.Tag), 0.0F)
        Dim g = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias
        Using track As New SolidBrush(Color.FromArgb(230, 228, 224)),
              fill As New SolidBrush(Green)
            g.FillRectangle(track, 0, 0, p.Width, p.Height)
            g.FillRectangle(fill, 0, 0, CInt(p.Width * fraction), p.Height)
        End Using
    End Sub

    ' =================================================================
    '  CUSTOM DRAWING - dispatch grid cells
    ' =================================================================
    Private Function ActionRects(b As Rectangle) As Rectangle()
        Dim cy As Integer = b.Top + b.Height \ 2
        Dim eye As New Rectangle(b.Left + 12, cy - 15, 30, 30)
        Dim btn As New Rectangle(b.Left + 52, cy - 17, 100, 34)
        Dim more As New Rectangle(b.Left + 162, cy - 15, 30, 30)
        Return {eye, btn, more}
    End Function

    Private Sub dgvDispatch_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) Handles dgvDispatch.CellPainting
        If e.RowIndex < 0 Then Return
        Dim idx As Integer = e.ColumnIndex
        If idx <> colCustomer.Index AndAlso idx <> colVehicle.Index AndAlso idx <> colDispatch.Index _
           AndAlso idx <> colStatus.Index AndAlso idx <> colAction.Index Then Return

        e.Paint(e.CellBounds, DataGridViewPaintParts.Background Or DataGridViewPaintParts.Border Or DataGridViewPaintParts.SelectionBackground)
        Dim g = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias
        g.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit
        Dim txt As String = Convert.ToString(e.Value)
        Dim b As Rectangle = e.CellBounds

        If idx = colCustomer.Index Then
            DrawTwoLine(g, b, txt, 60, Color.FromArgb(150, 150, 150), True)

        ElseIf idx = colVehicle.Index Then
            DrawTwoLine(g, b, txt, 14, Color.FromArgb(150, 150, 150), False)

        ElseIf idx = colDispatch.Index Then
            Dim second As Color = Color.FromArgb(30, 130, 70)
            If txt.ToUpper().Contains("CLEANING") Then second = Color.FromArgb(30, 90, 200)
            DrawTwoLine(g, b, txt, 14, second, False)

        ElseIf idx = colStatus.Index Then
            Dim bg As Color = Color.FromArgb(255, 236, 214), fg As Color = Color.FromArgb(200, 100, 0)
            If txt.ToUpper().Contains("PREPARATION") Then
                bg = Color.FromArgb(222, 234, 252) : fg = Color.FromArgb(30, 90, 200)
            End If
            Using f As New Font("Segoe UI", 7.5F, FontStyle.Bold)
                Dim w As Integer = CInt(g.MeasureString(txt, f).Width) + 28
                Dim r As New Rectangle(b.Left + 10, b.Top + (b.Height - 28) \ 2, w, 28)
                FillRound(g, r, 14, bg)
                Using br As New SolidBrush(fg), sf As New StringFormat With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center}
                    g.DrawString(txt, f, br, r, sf)
                End Using
            End Using

        ElseIf idx = colAction.Index Then
            Dim rects = ActionRects(b)
            Dim disabled As Boolean = Convert.ToString(dgvDispatch.Rows(e.RowIndex).Cells(colStatus.Index).Value).ToUpper().Contains("PREPARATION")
            Using sf As New StringFormat With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center}
                ' eye + more boxes
                For Each i In {0, 2}
                    FillRound(g, rects(i), 6, Color.FromArgb(247, 246, 244))
                    Using pen As New Pen(Color.FromArgb(220, 217, 212))
                        g.DrawPath(pen, RoundPath(rects(i), 6))
                    End Using
                Next
                Using f As New Font("Segoe UI Emoji", 9.0F), br As New SolidBrush(Color.FromArgb(90, 90, 90))
                    g.DrawString("👁", f, br, rects(0), sf)
                    g.DrawString("⋮", f, br, rects(2), sf)
                End Using
                ' check-out button
                FillRound(g, rects(1), 6, If(disabled, Color.FromArgb(226, 225, 223), Maroon))
                Using f As New Font("Segoe UI", 7.5F, FontStyle.Bold), br As New SolidBrush(If(disabled, Color.FromArgb(160, 160, 160), Color.White))
                    g.DrawString("CHECK OUT", f, br, rects(1), sf)
                End Using
            End Using
        End If

        e.Handled = True
    End Sub

    Private Sub dgvDispatch_CellMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgvDispatch.CellMouseClick
        If e.RowIndex < 0 OrElse e.ColumnIndex <> colAction.Index Then Return
        Dim cell As Rectangle = dgvDispatch.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, False)
        Dim pt As New Point(cell.Left + e.X, cell.Top + e.Y)
        Dim rects = ActionRects(cell)
        Dim id As String = Convert.ToString(dgvDispatch.Rows(e.RowIndex).Cells(colBookingId.Index).Value)

        If rects(0).Contains(pt) Then
            MessageBox.Show("View booking " & id, "Booking")
        ElseIf rects(1).Contains(pt) Then
            If Convert.ToString(dgvDispatch.Rows(e.RowIndex).Cells(colStatus.Index).Value).ToUpper().Contains("PREPARATION") Then Return
            ' TODO: open the check-out / inspection workflow for this booking
            MessageBox.Show("Check out booking " & id, "Check out")
        ElseIf rects(2).Contains(pt) Then
            MessageBox.Show("More actions for " & id, "Booking")
        End If
    End Sub

    Private Sub DrawTwoLine(g As Graphics, b As Rectangle, text As String, leftPad As Integer, secondColor As Color, avatar As Boolean)
        Dim parts = text.Split(ControlChars.Lf)
        Dim l1 As String = parts(0)
        Dim l2 As String = If(parts.Length > 1, parts(1), "")
        Using f1 As New Font("Segoe UI", 9.5F, If(avatar, FontStyle.Regular, FontStyle.Regular)),
              f2 As New Font("Segoe UI", 7.5F),
              b1 As New SolidBrush(Ink), b2 As New SolidBrush(secondColor),
              sf As New StringFormat With {.Trimming = StringTrimming.EllipsisCharacter, .FormatFlags = StringFormatFlags.NoWrap}

            Dim y As Integer = b.Top + (b.Height - (f1.Height + f2.Height)) \ 2
            Dim w As Integer = b.Right - (b.Left + leftPad) - 6
            g.DrawString(l1, f1, b1, New RectangleF(b.Left + leftPad, y, w, f1.Height), sf)
            g.DrawString(l2, f2, b2, New RectangleF(b.Left + leftPad, y + f1.Height, w, f2.Height), sf)

            If avatar Then
                Dim c As New Rectangle(b.Left + 12, b.Top + (b.Height - 38) \ 2, 38, 38)
                g.FillEllipse(Brushes.Gainsboro, c)
                Dim parts2 = l1.Split({" "c}, StringSplitOptions.RemoveEmptyEntries)
                Dim ini As String = parts2(0).Substring(0, 1) & If(parts2.Length > 1, parts2(parts2.Length - 1).Substring(0, 1), "")
                Using fi As New Font("Segoe UI", 9.0F, FontStyle.Bold), bi As New SolidBrush(Maroon),
                      sfc As New StringFormat With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center}
                    g.DrawString(ini.ToUpper(), fi, bi, c, sfc)
                End Using
            End If
        End Using
    End Sub

    ' =================================================================
    '  Helpers
    ' =================================================================
    Private Shared Function RoundPath(r As Rectangle, radius As Integer) As GraphicsPath
        Dim d As Integer = radius * 2
        Dim gp As New GraphicsPath()
        gp.AddArc(r.Left, r.Top, d, d, 180, 90)
        gp.AddArc(r.Right - d, r.Top, d, d, 270, 90)
        gp.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90)
        gp.AddArc(r.Left, r.Bottom - d, d, d, 90, 90)
        gp.CloseFigure()
        Return gp
    End Function

    Private Shared Sub FillRound(g As Graphics, r As Rectangle, radius As Integer, color As Color)
        Using gp = RoundPath(r, radius), br As New SolidBrush(color)
            g.FillPath(br, gp)
        End Using
    End Sub

    Private Shared Sub MakeRounded(c As Control, radius As Integer)
        Using gp = RoundPath(New Rectangle(0, 0, c.Width, c.Height), radius)
            c.Region = New Region(gp)
        End Using
    End Sub

    Private Shared Sub EnableDoubleBuffer(c As Control)
        GetType(Control).GetProperty("DoubleBuffered", BindingFlags.Instance Or BindingFlags.NonPublic).SetValue(c, True)
    End Sub

End Class
