Imports System.Drawing.Drawing2D

' Fleet Inventory register (list). Clicking "View" on a row opens ucVehicleDetails inside this control.
' Needs ucVehicleDetails.vb (also in your project, it holds the FiCard class used here).
Public Class ucFleetInventory
    Inherits UserControl

    Private Shared ReadOnly Maroon As Color = Color.FromArgb(122, 0, 16)
    Private Shared ReadOnly Ink As Color = Color.FromArgb(38, 38, 38)
    Private Shared ReadOnly Muted As Color = Color.FromArgb(120, 120, 120)
    Private Shared ReadOnly BorderCol As Color = Color.FromArgb(228, 225, 220)
    Private Shared ReadOnly PageBg As Color = Color.FromArgb(245, 244, 242)

    Public Event ViewVehicleRequested(rowIndex As Integer)
    Public Event EditVehicleRequested(rowIndex As Integer)
    Public Event AddVehicleClicked()
    Public Event ExportClicked()

    ' ---- the vehicle details screen (shown after clicking View) ----
    Public ReadOnly Details As New ucVehicleDetails

    ' ---- the Add / Edit vehicle form ----
    Public ReadOnly Editor As New ucVehicleForm

    ' ---- buttons ----
    Public ReadOnly btnExport As New Button
    Public ReadOnly btnAdd As New Button
    Public ReadOnly btnPrev As New Button
    Public ReadOnly btnPage1 As New Button
    Public ReadOnly btnPage2 As New Button
    Public ReadOnly btnPage3 As New Button
    Public ReadOnly btnNext As New Button

    ' ---- filters ----
    Public ReadOnly txtSearch As New TextBox
    Public ReadOnly cboCategory As New ComboBox
    Public ReadOnly cboStatus As New ComboBox
    Public ReadOnly cboSort As New ComboBox

    ' ---- data spots ----
    Public ReadOnly lblSummary As New Label
    Public ReadOnly lblRegisterSub As New Label
    Public ReadOnly lblTotal As New Label
    Public ReadOnly lblShowing As New Label
    Public ReadOnly lblProtectedNote As New Label
    Public ReadOnly dgvFleet As New DataGridView

    Private ReadOnly pnlRegister As New Panel
    Private cFilter, cReg As FiCard
    Private capSearch, capCategory, capStatus, capSort As Label

    Public Sub New()
        Me.BackColor = PageBg
        BuildUI()
    End Sub

    ' ---------------- public ----------------
    Public Sub ShowDetails()
        pnlRegister.Visible = False
        Editor.Visible = False
        Details.Visible = True
        Details.BringToFront()
    End Sub

    Public Sub ShowRegister()
        Details.Visible = False
        Editor.Visible = False
        pnlRegister.Visible = True
        pnlRegister.BringToFront()
    End Sub

    Public Sub ShowAddForm()
        Editor.SetMode(False)
        Editor.ClearForm()
        pnlRegister.Visible = False
        Details.Visible = False
        Editor.Visible = True
        Editor.BringToFront()
    End Sub

    Public Sub ShowEditForm()
        Editor.SetMode(True)
        pnlRegister.Visible = False
        Details.Visible = False
        Editor.Visible = True
        Editor.BringToFront()
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

    Private Shared Sub StyleBtn(b As Button, text As String, w As Integer, h As Integer, primary As Boolean)
        b.Text = text : b.Size = New Size(w, h) : b.FlatStyle = FlatStyle.Flat
        b.Font = F(9) : b.Cursor = Cursors.Hand : b.UseVisualStyleBackColor = False : b.UseMnemonic = False
        b.BackColor = If(primary, Maroon, Color.White)
        b.ForeColor = If(primary, Color.White, Ink)
        b.FlatAppearance.BorderColor = If(primary, Maroon, BorderCol)
        b.FlatAppearance.BorderSize = 1
    End Sub

    Private Shared Sub StyleCombo(c As ComboBox)
        c.DropDownStyle = ComboBoxStyle.DropDownList
        c.FlatStyle = FlatStyle.Flat
        c.Font = F(9)
        c.BackColor = Color.White
    End Sub

    ' ---------------- layout ----------------
    Private Sub ArrangeRegister()
        Dim w As Integer = Math.Max(pnlRegister.ClientSize.Width, 900) - 48
        btnAdd.Location = New Point(24 + w - btnAdd.Width, 20)
        btnExport.Location = New Point(btnAdd.Left - 8 - btnExport.Width, 20)
        cFilter.SetBounds(24, 96, w, 78)
        cReg.SetBounds(24, 190, w, 520)
        lblProtectedNote.Width = w
        lblProtectedNote.Left = 24
    End Sub

    Private Sub BuildUI()
        Me.SuspendLayout()

        Details.Dock = DockStyle.Fill
        Details.Visible = False
        Controls.Add(Details)

        Editor.Dock = DockStyle.Fill
        Editor.Visible = False
        Controls.Add(Editor)

        pnlRegister.Dock = DockStyle.Fill
        pnlRegister.BackColor = PageBg
        pnlRegister.AutoScroll = True
        pnlRegister.AutoScrollMinSize = New Size(900, 800)
        Controls.Add(pnlRegister)

        ' ----- header -----
        pnlRegister.Controls.Add(Cap("Fleet Inventory", F(20, FontStyle.Bold), Ink, 22, 14))
        Spot(lblSummary, F(9), Muted, 26, 58)
        pnlRegister.Controls.Add(lblSummary)
        StyleBtn(btnExport, "Authorized export", 140, 34, False)
        StyleBtn(btnAdd, "+  Add Vehicle", 130, 34, True)
        pnlRegister.Controls.Add(btnExport)
        pnlRegister.Controls.Add(btnAdd)

        ' ----- filter card -----
        cFilter = New FiCard
        capSearch = Cap("Search fleet", F(8, FontStyle.Bold), Ink, 0, 10)
        capCategory = Cap("Category", F(8, FontStyle.Bold), Ink, 0, 10)
        capStatus = Cap("Current status", F(8, FontStyle.Bold), Ink, 0, 10)
        capSort = Cap("Sort by", F(8, FontStyle.Bold), Ink, 0, 10)
        txtSearch.Font = F(9) : txtSearch.BorderStyle = BorderStyle.FixedSingle
        StyleCombo(cboCategory) : StyleCombo(cboStatus) : StyleCombo(cboSort)
        cFilter.Controls.AddRange(New Control() {capSearch, capCategory, capStatus, capSort,
                                                 txtSearch, cboCategory, cboStatus, cboSort})
        AddHandler cFilter.Resize, Sub(s, e)
                                       Dim gap As Integer = 14
                                       Dim cw As Integer = (cFilter.Width - 36 - gap * 3) \ 4
                                       Dim x As Integer = 18
                                       For Each pair In New Tuple(Of Label, Control)() {
                                               Tuple.Create(Of Label, Control)(capSearch, txtSearch),
                                               Tuple.Create(Of Label, Control)(capCategory, cboCategory),
                                               Tuple.Create(Of Label, Control)(capStatus, cboStatus),
                                               Tuple.Create(Of Label, Control)(capSort, cboSort)}
                                           pair.Item1.Location = New Point(x, 10)
                                           pair.Item2.SetBounds(x, 34, cw, 28)
                                           x += cw + gap
                                       Next
                                   End Sub
        pnlRegister.Controls.Add(cFilter)

        ' ----- register card -----
        cReg = New FiCard
        cReg.Controls.Add(Cap("Vehicle register", F(11, FontStyle.Bold), Ink, 18, 12))
        Spot(lblRegisterSub, F(7.5!), Muted, 18, 36)
        cReg.Controls.Add(lblRegisterSub)

        lblTotal.AutoSize = False : lblTotal.Size = New Size(70, 20)
        lblTotal.Font = F(7.5!, FontStyle.Bold) : lblTotal.ForeColor = Maroon
        lblTotal.BackColor = Color.FromArgb(250, 228, 230)
        lblTotal.TextAlign = ContentAlignment.MiddleCenter : lblTotal.UseMnemonic = False
        lblTotal.Top = 14
        cReg.Controls.Add(lblTotal)

        BuildGrid()
        cReg.Controls.Add(dgvFleet)

        Spot(lblShowing, F(8), Muted, 18, 0)
        cReg.Controls.Add(lblShowing)

        StyleBtn(btnPrev, "‹", 28, 28, False)
        StyleBtn(btnPage1, "1", 28, 28, True)
        StyleBtn(btnPage2, "2", 28, 28, False)
        StyleBtn(btnPage3, "3", 28, 28, False)
        StyleBtn(btnNext, "›", 28, 28, False)
        For Each b As Button In New Button() {btnPrev, btnPage1, btnPage2, btnPage3, btnNext}
            b.Font = F(8.5!) : b.Padding = New Padding(0)
            cReg.Controls.Add(b)
        Next

        AddHandler cReg.Resize, Sub(s, e)
                                    Dim w As Integer = cReg.Width
                                    lblTotal.Left = w - 18 - lblTotal.Width
                                    dgvFleet.SetBounds(1, 62, w - 2, cReg.Height - 62 - 56)
                                    Dim fy As Integer = cReg.Height - 42
                                    lblShowing.Top = fy + 6
                                    Dim x As Integer = w - 18 - 28
                                    For Each b As Button In New Button() {btnNext, btnPage3, btnPage2, btnPage1, btnPrev}
                                        b.Location = New Point(x, fy)
                                        x -= 34
                                    Next
                                End Sub
        pnlRegister.Controls.Add(cReg)

        ' ----- protected note -----
        pnlRegister.Controls.Add(Cap("Protected fleet actions", F(8.5!, FontStyle.Bold), Muted, 24, 722))
        lblProtectedNote.AutoSize = False : lblProtectedNote.Font = F(8) : lblProtectedNote.ForeColor = Muted
        lblProtectedNote.BackColor = Color.Transparent : lblProtectedNote.Location = New Point(24, 742)
        lblProtectedNote.Height = 30 : lblProtectedNote.UseMnemonic = False
        pnlRegister.Controls.Add(lblProtectedNote)

        AddHandler pnlRegister.Resize, Sub(s, e) ArrangeRegister()
        AddHandler btnAdd.Click, Sub(s, e)
                                     ShowAddForm()
                                     RaiseEvent AddVehicleClicked()
                                 End Sub
        AddHandler Details.btnEditVehicle.Click, Sub(s, e) ShowEditForm()
        AddHandler Editor.BackClicked, Sub() ShowRegister()
        AddHandler Editor.CancelClicked, Sub() ShowRegister()
        AddHandler Editor.KeepActiveClicked, Sub() ShowRegister()
        AddHandler btnExport.Click, Sub(s, e) RaiseEvent ExportClicked()
        AddHandler dgvFleet.CellPainting, AddressOf Grid_CellPainting
        AddHandler dgvFleet.CellFormatting, AddressOf Grid_CellFormatting
        AddHandler dgvFleet.CellMouseClick, AddressOf Grid_CellMouseClick
        AddHandler dgvFleet.CellMouseMove, AddressOf Grid_CellMouseMove

        Me.ResumeLayout(False)
        ArrangeRegister()
    End Sub

    ' ---------------- grid ----------------
    Private Sub BuildGrid()
        With dgvFleet
            .BackgroundColor = Color.White : .BorderStyle = BorderStyle.None
            .CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal : .GridColor = BorderCol
            .RowHeadersVisible = False : .AllowUserToAddRows = False : .AllowUserToDeleteRows = False
            .AllowUserToResizeRows = False : .ReadOnly = True : .MultiSelect = False
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .EnableHeadersVisualStyles = False
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
            .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            .ColumnHeadersHeight = 34
            .RowTemplate.Height = 44
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            .ScrollBars = ScrollBars.Vertical
            .ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(250, 249, 247)
            .ColumnHeadersDefaultCellStyle.ForeColor = Muted
            .ColumnHeadersDefaultCellStyle.Font = F(8)
            .ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(250, 249, 247)
            .ColumnHeadersDefaultCellStyle.SelectionForeColor = Muted
            .ColumnHeadersDefaultCellStyle.Padding = New Padding(12, 0, 0, 0)
            .DefaultCellStyle.Font = F(8.5!)
            .DefaultCellStyle.ForeColor = Ink
            .DefaultCellStyle.SelectionBackColor = Color.FromArgb(250, 249, 247)
            .DefaultCellStyle.SelectionForeColor = Ink
            .DefaultCellStyle.Padding = New Padding(12, 0, 0, 0)
        End With

        AddCol("colId", "Vehicle ID", 9)
        Dim photo As New DataGridViewImageColumn With {
            .Name = "colPhoto", .HeaderText = "Vehicle Photo", .FillWeight = 11,
            .ImageLayout = DataGridViewImageCellLayout.Zoom}
        photo.DefaultCellStyle.NullValue = Nothing
        dgvFleet.Columns.Add(photo)
        AddCol("colPlate", "Plate Number", 10)
        AddCol("colModel", "Make and Model", 14)
        AddCol("colYear", "Year", 6)
        AddCol("colCategory", "Category", 8)
        AddCol("colTransmission", "Transmission", 10)
        AddCol("colRate", "Daily Rate", 8)
        AddCol("colStatus", "Current Status", 12)
        AddCol("colActions", "Actions", 12)
    End Sub

    Private Sub AddCol(name As String, header As String, weight As Single)
        Dim c As New DataGridViewTextBoxColumn With {.Name = name, .HeaderText = header, .FillWeight = weight,
                                                    .SortMode = DataGridViewColumnSortMode.NotSortable}
        dgvFleet.Columns.Add(c)
    End Sub

    Private Sub Grid_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs)
        If e.RowIndex >= 0 AndAlso e.ColumnIndex = dgvFleet.Columns("colId").Index Then
            e.CellStyle.ForeColor = Maroon
            e.CellStyle.Font = F(8.5!, FontStyle.Bold)
        End If
    End Sub

    ' pill for status, two outlined buttons for actions
    Private Sub Grid_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs)
        If e.RowIndex < 0 Then Return
        Dim isStatus As Boolean = (e.ColumnIndex = dgvFleet.Columns("colStatus").Index)
        Dim isActions As Boolean = (e.ColumnIndex = dgvFleet.Columns("colActions").Index)
        If Not isStatus AndAlso Not isActions Then Return

        e.Paint(e.CellBounds, DataGridViewPaintParts.Background Or DataGridViewPaintParts.Border)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias

        If isStatus Then
            Dim txt As String = Convert.ToString(e.Value)
            If txt <> "" Then
                Dim back As Color = Color.FromArgb(238, 238, 236)
                Dim fore As Color = Color.FromArgb(100, 100, 100)
                Select Case txt
                    Case "Available" : back = Color.FromArgb(232, 240, 233) : fore = Color.FromArgb(46, 110, 60)
                    Case "Reserved" : back = Color.FromArgb(252, 240, 220) : fore = Color.FromArgb(176, 112, 32)
                    Case "Under Maintenance" : back = Color.FromArgb(250, 228, 230) : fore = Maroon
                End Select
                Using f As New Font("Segoe UI", 7.5!, FontStyle.Bold)
                    Dim sz As Size = TextRenderer.MeasureText(txt, f)
                    Dim r As New Rectangle(e.CellBounds.Left + 12, e.CellBounds.Top + (e.CellBounds.Height - 20) \ 2, sz.Width + 14, 20)
                    Using path As GraphicsPath = FleetDraw.RoundRect(r, 4), br As New SolidBrush(back)
                        e.Graphics.FillPath(br, path)
                    End Using
                    TextRenderer.DrawText(e.Graphics, txt, f, r, fore,
                        TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter Or TextFormatFlags.NoPadding)
                End Using
            End If
        Else
            Dim y As Integer = e.CellBounds.Top + (e.CellBounds.Height - 26) \ 2
            DrawBtn(e.Graphics, New Rectangle(e.CellBounds.Left + 12, y, 48, 26), "View")
            DrawBtn(e.Graphics, New Rectangle(e.CellBounds.Left + 68, y, 44, 26), "Edit")
        End If
        e.Handled = True
    End Sub

    Private Shared Sub DrawBtn(g As Graphics, r As Rectangle, text As String)
        Using path As GraphicsPath = FleetDraw.RoundRect(r, 4), br As New SolidBrush(Color.White),
              pen As New Pen(Color.FromArgb(215, 212, 206)), f As New Font("Segoe UI", 8.5!)
            g.FillPath(br, path)
            g.DrawPath(pen, path)
            TextRenderer.DrawText(g, text, f, r, Color.FromArgb(38, 38, 38),
                TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter Or TextFormatFlags.NoPadding)
        End Using
    End Sub

    Private Function OverButton(e As DataGridViewCellMouseEventArgs) As Integer
        ' 1 = View, 2 = Edit, 0 = none
        If e.RowIndex < 0 OrElse e.ColumnIndex <> dgvFleet.Columns("colActions").Index Then Return 0
        Dim top As Integer = (dgvFleet.Rows(e.RowIndex).Height - 26) \ 2
        If e.Y < top OrElse e.Y > top + 26 Then Return 0
        If e.X >= 12 AndAlso e.X <= 60 Then Return 1
        If e.X >= 68 AndAlso e.X <= 112 Then Return 2
        Return 0
    End Function

    Private Sub Grid_CellMouseMove(sender As Object, e As DataGridViewCellMouseEventArgs)
        dgvFleet.Cursor = If(OverButton(e) <> 0, Cursors.Hand, Cursors.Default)
    End Sub

    Private Sub Grid_CellMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs)
        Select Case OverButton(e)
            Case 1
                ShowDetails()
                RaiseEvent ViewVehicleRequested(e.RowIndex)
            Case 2
                ShowEditForm()
                RaiseEvent EditVehicleRequested(e.RowIndex)
        End Select
    End Sub
End Class

Friend Module FleetDraw
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
End Module