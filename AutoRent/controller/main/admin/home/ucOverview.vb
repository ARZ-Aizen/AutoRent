Imports System.Drawing.Drawing2D
Imports System.Reflection

Public Class ucOverview

    ' Raised when "View audit logs" is clicked. The main form switches to the Audit Logs panel.
    Public Event ViewAuditLogsRequested()

    Private ReadOnly Maroon As Color = Color.FromArgb(122, 0, 16)

    ' ---- demo data (replace with queries to your AutoRent.accdb) ----
    Private _available As Integer = 42
    Private _reserved As Integer = 7
    Private _rented As Integer = 128
    Private _maintenance As Integer = 7
    Private ReadOnly _chartPoints As New List(Of PointF)   ' X = hour of day, Y = cumulative revenue

    Private Sub ucOverview_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        EnableDoubleBuffer(pnlChart)
        EnableDoubleBuffer(pnlBar)
        lblRevDate.Text = Date.Today.ToString("dd MMM yyyy")
        LoadDashboard()
        SetActiveTab(btnToday)
    End Sub

    ' =================================================================
    '  DATA  -  replace the demo values with real queries
    ' =================================================================
    Public Sub LoadDashboard()
        ' KPI cards (index, value, sub text)
        SetKpi(1, "184", "4 categories · no retired units")
        SetKpi(2, "42", "+4 vs yesterday")
        SetKpi(3, "128", "+6 vs yesterday")
        SetKpi(4, "7", "2 overdue services")
        SetKpi(5, "$1,900", "+8.6% vs yesterday")
        SetKpi(6, "69.6%", "128 / 184 · +3.3 pp")

        ' fleet readiness
        lblStatVal1.Text = _available.ToString()
        lblStatVal2.Text = _rented.ToString()
        lblStatVal3.Text = _reserved.ToString()
        lblStatVal4.Text = _maintenance.ToString()
        lblReadySub.Text = "All " & (_available + _reserved + _rented + _maintenance) & " vehicles · live status"

        ' category rows (name, total, rented)
        SetCategory(1, "Sedan", 48, 34)
        SetCategory(2, "SUV", 64, 46)
        SetCategory(3, "Van", 32, 24)
        SetCategory(4, "Pickup", 40, 24)

        ' revenue chart
        LoadChart("Today")

        ' recent activity (timestamp, staff, action, record, result)
        dgvActivity.Rows.Clear()
        dgvActivity.Rows.Add("14 Oct · 16:42", "Alex Morgan", "Vehicle added", "VH-0184 · CAD-1212", "Success")
        dgvActivity.Rows.Add("14 Oct · 15:18", "Priya Shah", "Rate updated", "RR-004 · Weekend uplift", "Success")
        dgvActivity.Rows.Add("14 Oct · 14:05", "Daniel Reed", "Maintenance scheduled", "MT-0231 · VH-0004", "Success")
        dgvActivity.Rows.Add("14 Oct · 11:26", "Alex Morgan", "Staff account modified", "USR-0008 · Maya Chen", "Success")
    End Sub

    Private Sub SetKpi(index As Integer, value As String, subText As String)
        Controls.Find("lblKpiValue" & index, True)(0).Text = value
        Controls.Find("lblKpiSub" & index, True)(0).Text = subText
    End Sub

    Private Sub SetCategory(index As Integer, name As String, total As Integer, rented As Integer)
        Controls.Find("lblCatName" & index, True)(0).Text = name
        Controls.Find("lblCatVal" & index, True)(0).Text = total & " / " & rented
    End Sub

    Private Sub LoadChart(range As String)
        _chartPoints.Clear()
        ' demo: cumulative revenue during the day. Replace with your query for the selected range.
        _chartPoints.Add(New PointF(8.0F, 170))
        _chartPoints.Add(New PointF(9.25F, 560))
        _chartPoints.Add(New PointF(11.0F, 1190))
        _chartPoints.Add(New PointF(13.75F, 1430))
        _chartPoints.Add(New PointF(16.0F, 1900))
        _chartPoints.Add(New PointF(17.0F, 1900))
        pnlChart.Invalidate()
    End Sub

    ' =================================================================
    '  EVENTS
    ' =================================================================
    Private Sub Tab_Click(sender As Object, e As EventArgs) Handles btnToday.Click, btnWeek.Click, btnMonth.Click, btnCustom.Click
        Dim b = DirectCast(sender, Button)
        SetActiveTab(b)
        LoadChart(b.Text)
    End Sub

    Private Sub SetActiveTab(active As Button)
        For Each b In {btnToday, btnWeek, btnMonth, btnCustom}
            Dim isOn As Boolean = (b Is active)
            b.BackColor = If(isOn, Maroon, Color.FromArgb(245, 244, 242))
            b.ForeColor = If(isOn, Color.White, Color.FromArgb(90, 90, 90))
            b.Font = New Font("Segoe UI", 9.5F, If(isOn, FontStyle.Bold, FontStyle.Regular))
        Next
    End Sub

    Private Sub btnViewAudit_Click(sender As Object, e As EventArgs) Handles btnViewAudit.Click
        RaiseEvent ViewAuditLogsRequested()
    End Sub

    Private Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        MessageBox.Show("Export snapshot - add your export code here.", "Export")
    End Sub

    ' =================================================================
    '  CUSTOM DRAWING
    ' =================================================================
    ' Step chart for revenue
    Private Sub pnlChart_Paint(sender As Object, e As PaintEventArgs) Handles pnlChart.Paint
        Dim g = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias
        g.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit

        Dim plot As New Rectangle(62, 12, pnlChart.Width - 62 - 22, pnlChart.Height - 12 - 32)
        Const minH As Single = 8.0F, maxH As Single = 17.0F, maxV As Single = 2000.0F
        Dim fx = Function(hr As Single) CSng(plot.Left + (hr - minH) / (maxH - minH) * plot.Width)
        Dim fy = Function(amt As Single) CSng(plot.Bottom - amt / maxV * plot.Height)

        Using gridPen As New Pen(Color.FromArgb(232, 229, 225)),
              txtBrush As New SolidBrush(Color.FromArgb(120, 120, 120)),
              small As New Font("Segoe UI", 8.5F)

            ' horizontal grid + y labels
            For Each v In {0, 1000, 2000}
                Dim y = fy(v)
                g.DrawLine(gridPen, plot.Left, y, plot.Right, y)
                Using sf As New StringFormat With {.Alignment = StringAlignment.Far, .LineAlignment = StringAlignment.Center}
                    g.DrawString("$" & v.ToString("N0"), small, txtBrush, New RectangleF(0, y - 9, 56, 18), sf)
                End Using
            Next

            ' x labels
            For Each h In {8, 10, 12, 14, 16, 17}
                Using sf As New StringFormat With {.Alignment = StringAlignment.Center}
                    g.DrawString(h.ToString("00") & ":00", small, txtBrush, New RectangleF(fx(h) - 30, plot.Bottom + 10, 60, 18), sf)
                End Using
            Next
        End Using

        If _chartPoints.Count < 2 Then Return

        ' stepped line
        Dim pts As New List(Of PointF)
        pts.Add(New PointF(fx(_chartPoints(0).X), fy(_chartPoints(0).Y)))
        For i = 1 To _chartPoints.Count - 1
            pts.Add(New PointF(fx(_chartPoints(i).X), fy(_chartPoints(i - 1).Y)))
            pts.Add(New PointF(fx(_chartPoints(i).X), fy(_chartPoints(i).Y)))
        Next
        Using linePen As New Pen(Maroon, 2.5F)
            linePen.LineJoin = LineJoin.Miter
            g.DrawLines(linePen, pts.ToArray())
        End Using

        ' callout for the last point
        Dim last = _chartPoints(_chartPoints.Count - 1)
        Dim px = fx(last.X), py = fy(last.Y)
        Dim box As New RectangleF(px - 150, py + 8, 126, 50)
        Using b As New SolidBrush(Color.FromArgb(38, 38, 38)),
              white As New SolidBrush(Color.White),
              f1 As New Font("Segoe UI", 8.5F),
              f2 As New Font("Segoe UI", 11.0F, FontStyle.Bold)
            g.FillRectangle(b, box)
            g.DrawString(Date.Today.ToString("dd MMM") & " · 17:00", f1, white, box.Left + 10, box.Top + 6)
            g.DrawString("$" & last.Y.ToString("N2"), f2, white, box.Left + 10, box.Top + 22)
            g.FillEllipse(b, px - 4, py - 4, 8, 8)
        End Using
    End Sub

    ' Segmented fleet status bar
    Private Sub pnlBar_Paint(sender As Object, e As PaintEventArgs) Handles pnlBar.Paint
        Dim counts = {_available, _reserved, _rented, _maintenance}
        Dim colors = {Color.FromArgb(46, 102, 80), Color.FromArgb(150, 110, 50), Maroon, Color.FromArgb(140, 130, 130)}
        Dim total As Integer = Math.Max(1, counts.Sum())
        Const gap As Integer = 2
        Dim usable As Single = pnlBar.Width - gap * (counts.Length - 1)
        Dim x As Single = 0
        For i = 0 To counts.Length - 1
            Dim w As Single = usable * counts(i) / total
            Using b As New SolidBrush(colors(i))
                e.Graphics.FillRectangle(b, x, 0, w, pnlBar.Height)
            End Using
            x += w + gap
        Next
    End Sub

    Private Shared Sub EnableDoubleBuffer(c As Control)
        GetType(Control).GetProperty("DoubleBuffered", BindingFlags.Instance Or BindingFlags.NonPublic).SetValue(c, True)
    End Sub

End Class
