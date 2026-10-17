Imports System.Drawing.Drawing2D

' User Account Management: staff directory (list) + the Create / Edit form (ucUserForm) inside the same control.
' Needs ucVehicleDetails.vb (FiCard) and ucUserForm.vb in the project.
Public Class ucUserAccounts
    Inherits UserControl

    Private Shared ReadOnly Maroon As Color = Color.FromArgb(122, 0, 16)
    Private Shared ReadOnly Ink As Color = Color.FromArgb(38, 38, 38)
    Private Shared ReadOnly Muted As Color = Color.FromArgb(120, 120, 120)
    Private Shared ReadOnly BorderCol As Color = Color.FromArgb(228, 225, 220)
    Private Shared ReadOnly PageBg As Color = Color.FromArgb(245, 244, 242)

    Public Event CreateUserClicked()
    Public Event EditUserRequested(rowIndex As Integer)
    Public Event UserSelected(rowIndex As Integer)
    Public Event FilterChanged()
    Public Event DeactivateClicked()
    Public Event SecureResetClicked()
    Public Event ViewActivityClicked()

    ' ---- the Create / Edit form ----
    Public ReadOnly Editor As New ucUserForm

    ' ---- header ----
    Public ReadOnly btnCreate As New Button
    Public ReadOnly lblSummary As New Label

    ' ---- filters ----
    Public ReadOnly txtSearch As New TextBox
    Public ReadOnly cboRole As New ComboBox
    Public ReadOnly cboBranch As New ComboBox
    Public ReadOnly cboStatus As New ComboBox

    ' ---- directory ----
    Public ReadOnly dgvUsers As New DataGridView
    Public ReadOnly lblShowing As New Label
    Public ReadOnly lblPage As New Label

    ' ---- selected account ----
    Public ReadOnly lblSelTitle As New Label
    Public ReadOnly lblSelSub As New Label
    Public ReadOnly lblSelRole As New Label
    Public ReadOnly lblSelLastActivity As New Label
    Public ReadOnly lblSelInvitation As New Label
    Public ReadOnly lblSelAuthority As New Label
    Public ReadOnly btnDeactivate As New Button
    Public ReadOnly btnSecureReset As New Button
    Public ReadOnly btnViewActivity As New Button
    Public ReadOnly lblLifecycleTitle As New Label
    Public ReadOnly lblLifecycleNote As New Label

    Private ReadOnly pnlList As New Panel
    Private cFilter, cDir, cSel As FiCard
    Private ReadOnly _caps As New List(Of Label)
    Private lblTitle, lblDirTitle, lblBadge As Label
    Private capSearch, capRole, capBranch, capStatus As Label
    Private ReadOnly _selCaps As New List(Of Label)

    Public Sub New()
        Me.BackColor = PageBg
        BuildUI()
    End Sub

    ' ---------------- public ----------------
    Public Sub ShowList()
        Editor.Visible = False
        pnlList.Visible = True
        pnlList.BringToFront()
    End Sub

    Public Sub ShowCreate()
        Editor.SetMode(False)
        Editor.ClearForm()
        pnlList.Visible = False
        Editor.Visible = True
        Editor.BringToFront()
    End Sub

    Public Sub ShowEdit()
        Editor.SetMode(True)
        pnlList.Visible = False
        Editor.Visible = True
        Editor.BringToFront()
    End Sub

    ' ---------------- helpers ----------------
    Private Shared Function F(size As Single, Optional style As FontStyle = FontStyle.Regular) As Font
        Return New Font("Segoe UI", size, style)
    End Function

    Private Shared Function Cap(text As String, fnt As Font, c As Color, x As Integer, y As Integer) As Label
        Return New Label With {.Text = text, .Font = fnt, .ForeColor = c, .BackColor = Color.Transparent,
                               .AutoSize = True, .Location = New Point(x, y), .UseMnemonic = False}
    End Function

    Private Shared Sub Spot(l As Label, fnt As Font, c As Color, x As Integer, y As Integer)
        l.Font = fnt : l.ForeColor = c : l.BackColor = Color.Transparent
        l.AutoSize = False : l.Location = New Point(x, y) : l.UseMnemonic = False
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

    Private Shared Function RoundRect(r As Rectangle, radius As Integer) As GraphicsPath
        Dim d As Integer = radius * 2
        Dim p As New GraphicsPath()
        p.AddArc(r.X, r.Y, d, d, 180, 90)
        p.AddArc(r.Right - d, r.Y, d, d, 270, 90)
        p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90)
        p.AddArc(r.X, r.Bottom - d, d, d, 90, 90)
        p.CloseFigure()
        Return p
    End Function

    ' ---------------- layout ----------------
    Private Sub ArrangeList()
        Dim w As Integer = Math.Max(pnlList.ClientSize.Width, 900) - 48
        btnCreate.Location = New Point(24 + w - btnCreate.Width, 20)
        cFilter.SetBounds(24, 92, w, 78)
        cDir.SetBounds(24, 184, w, 468)
        dgvUsers.SetBounds(12, 52, w - 24, 8 * 40 + 34)
        lblBadge.Location = New Point(w - 16 - lblBadge.Width, 14)
        lblShowing.SetBounds(16, 52 + 8 * 40 + 34 + 12, w \ 2, 16)
        lblPage.SetBounds(w - 16 - 300, 52 + 8 * 40 + 34 + 12, 300, 16)
        lblPage.TextAlign = ContentAlignment.TopRight
        Dim selW As Integer = CInt(w * 0.62)
        cSel.SetBounds(24, 668, selW, 150)
        lblLifecycleTitle.SetBounds(24 + selW + 16, 672, w - selW - 16, 18)
        lblLifecycleNote.SetBounds(24 + selW + 16, 692, w - selW - 16, 70)
        Dim half As Integer = (selW - 32) \ 2
        lblSelRole.SetBounds(16 + 90, 52, half - 90, 16)
        lblSelInvitation.SetBounds(16 + half + 90, 52, half - 90, 16)
        lblSelLastActivity.SetBounds(16 + 90, 76, half - 90, 16)
        lblSelAuthority.SetBounds(16 + half + 90, 76, half - 90, 16)
        For i As Integer = 0 To _selCaps.Count - 1
            Dim col As Integer = i Mod 2, row As Integer = i \ 2
            _selCaps(i).SetBounds(16 + col * half, 52 + row * 24, 90, 16)
        Next
        btnDeactivate.Location = New Point(16, 108)
        btnSecureReset.Location = New Point(16 + 96, 108)
        btnViewActivity.Location = New Point(16 + 96 + 150, 108)
        pnlList.AutoScrollMinSize = New Size(900, 840)
    End Sub

    Private Sub AddCapPair(card As Control, caption As String)
        Dim l As Label = Cap(caption, F(8), Muted, 0, 0)
        l.AutoSize = False : l.Size = New Size(90, 16)
        _selCaps.Add(l)
        card.Controls.Add(l)
    End Sub

    Private Sub BuildUI()
        Me.SuspendLayout()

        Editor.Dock = DockStyle.Fill
        Editor.Visible = False
        Controls.Add(Editor)

        pnlList.Dock = DockStyle.Fill
        pnlList.BackColor = PageBg
        pnlList.AutoScroll = True
        Controls.Add(pnlList)

        ' header
        lblTitle = Cap("User Account Management", F(20, FontStyle.Bold), Ink, 22, 14)
        pnlList.Controls.Add(lblTitle)
        Spot(lblSummary, F(9), Muted, 26, 58)
        lblSummary.Size = New Size(700, 18)
        pnlList.Controls.Add(lblSummary)
        StyleBtn(btnCreate, "+  Create User", 130, 34, True)
        pnlList.Controls.Add(btnCreate)

        ' filters
        cFilter = New FiCard
        capSearch = Cap("Search staff", F(8, FontStyle.Bold), Ink, 0, 10)
        capRole = Cap("Role", F(8, FontStyle.Bold), Ink, 0, 10)
        capBranch = Cap("Branch", F(8, FontStyle.Bold), Ink, 0, 10)
        capStatus = Cap("Account status", F(8, FontStyle.Bold), Ink, 0, 10)
        txtSearch.Font = F(9) : txtSearch.BorderStyle = BorderStyle.FixedSingle
        StyleCombo(cboRole) : StyleCombo(cboBranch) : StyleCombo(cboStatus)
        cFilter.Controls.AddRange(New Control() {capSearch, capRole, capBranch, capStatus, txtSearch, cboRole, cboBranch, cboStatus})
        AddHandler cFilter.Resize, Sub(s, e)
                                       Dim gap As Integer = 14
                                       Dim cw As Integer = (cFilter.Width - 36 - gap * 3) \ 4
                                       Dim x As Integer = 18
                                       For Each pair As Tuple(Of Label, Control) In New Tuple(Of Label, Control)() {
                                               Tuple.Create(Of Label, Control)(capSearch, txtSearch),
                                               Tuple.Create(Of Label, Control)(capRole, cboRole),
                                               Tuple.Create(Of Label, Control)(capBranch, cboBranch),
                                               Tuple.Create(Of Label, Control)(capStatus, cboStatus)}
                                           pair.Item1.Location = New Point(x, 10)
                                           pair.Item2.SetBounds(x, 34, cw, 26)
                                           x += cw + gap
                                       Next
                                   End Sub
        pnlList.Controls.Add(cFilter)

        ' directory card
        cDir = New FiCard
        lblDirTitle = Cap("Staff directory", F(10.5!, FontStyle.Bold), Ink, 16, 14)
        lblBadge = Cap("Admin-only account changes", F(7.5!, FontStyle.Bold), Maroon, 0, 14)
        lblBadge.AutoSize = False : lblBadge.Size = New Size(160, 20) : lblBadge.TextAlign = ContentAlignment.MiddleCenter
        lblBadge.BackColor = Color.FromArgb(250, 228, 230)
        BuildGrid()
        Spot(lblShowing, F(8), Muted, 16, 0)
        Spot(lblPage, F(8), Muted, 0, 0)
        cDir.Controls.AddRange(New Control() {lblDirTitle, lblBadge, dgvUsers, lblShowing, lblPage})
        pnlList.Controls.Add(cDir)

        ' selected account card
        cSel = New FiCard
        Spot(lblSelTitle, F(10, FontStyle.Bold), Ink, 16, 12) : lblSelTitle.Size = New Size(500, 18)
        Spot(lblSelSub, F(8), Muted, 16, 30) : lblSelSub.Size = New Size(500, 16)
        cSel.Controls.AddRange(New Control() {lblSelTitle, lblSelSub})
        AddCapPair(cSel, "Current role") : AddCapPair(cSel, "Invitation")
        AddCapPair(cSel, "Last activity") : AddCapPair(cSel, "Account authority")
        For Each l As Label In New Label() {lblSelRole, lblSelLastActivity, lblSelInvitation, lblSelAuthority}
            Spot(l, F(8, FontStyle.Bold), Ink, 0, 0)
            l.TextAlign = ContentAlignment.TopRight
            cSel.Controls.Add(l)
        Next
        StyleBtn(btnDeactivate, "Deactivate", 90, 30, False) : btnDeactivate.ForeColor = Maroon : btnDeactivate.FlatAppearance.BorderColor = Maroon
        StyleBtn(btnSecureReset, "🔑  Secure password reset", 150, 30, False)
        StyleBtn(btnViewActivity, "View account activity", 150, 30, False)
        cSel.Controls.AddRange(New Control() {btnDeactivate, btnSecureReset, btnViewActivity})
        pnlList.Controls.Add(cSel)

        Spot(lblLifecycleTitle, F(8.5!, FontStyle.Bold), Muted, 0, 0)
        Spot(lblLifecycleNote, F(8), Muted, 0, 0)
        pnlList.Controls.AddRange(New Control() {lblLifecycleTitle, lblLifecycleNote})

        AddHandler pnlList.Resize, Sub(s, e) ArrangeList()
        AddHandler btnCreate.Click, Sub(s, e)
                                        ShowCreate()
                                        RaiseEvent CreateUserClicked()
                                    End Sub
        AddHandler Editor.CancelClicked, Sub() ShowList()
        AddHandler Editor.CreateModeClicked, Sub()
                                                 ShowCreate()
                                                 RaiseEvent CreateUserClicked()
                                             End Sub
        AddHandler btnDeactivate.Click, Sub(s, e) RaiseEvent DeactivateClicked()
        AddHandler btnSecureReset.Click, Sub(s, e) RaiseEvent SecureResetClicked()
        AddHandler btnViewActivity.Click, Sub(s, e) RaiseEvent ViewActivityClicked()
        AddHandler txtSearch.TextChanged, Sub(s, e) RaiseEvent FilterChanged()
        For Each c As ComboBox In New ComboBox() {cboRole, cboBranch, cboStatus}
            AddHandler c.SelectedIndexChanged, Sub(s, e) RaiseEvent FilterChanged()
        Next
        AddHandler dgvUsers.SelectionChanged, Sub(s, e)
                                                  If dgvUsers.CurrentRow IsNot Nothing Then RaiseEvent UserSelected(dgvUsers.CurrentRow.Index)
                                              End Sub
        AddHandler dgvUsers.CellPainting, AddressOf Grid_CellPainting
        AddHandler dgvUsers.CellFormatting, AddressOf Grid_CellFormatting
        AddHandler dgvUsers.CellMouseClick, AddressOf Grid_CellMouseClick
        AddHandler dgvUsers.CellMouseMove, AddressOf Grid_CellMouseMove

        Me.ResumeLayout(False)
        ArrangeList()
    End Sub

    ' ---------------- grid ----------------
    Private Sub BuildGrid()
        With dgvUsers
            .BackgroundColor = Color.White : .BorderStyle = BorderStyle.None
            .CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal : .GridColor = BorderCol
            .RowHeadersVisible = False : .AllowUserToAddRows = False : .AllowUserToDeleteRows = False
            .AllowUserToResizeRows = False : .ReadOnly = True : .MultiSelect = False
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .EnableHeadersVisualStyles = False
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
            .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            .ColumnHeadersHeight = 34
            .RowTemplate.Height = 40
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
        AddCol("colId", "User ID", 8)
        AddCol("colName", "Full Name", 13)
        AddCol("colUsername", "Username", 10)
        AddCol("colRole", "Role", 17)
        AddCol("colBranch", "Branch", 8)
        AddCol("colStatus", "Account Status", 12)
        AddCol("colLogin", "Last Login", 12)
        AddCol("colActions", "Actions", 18)
    End Sub

    Private Sub AddCol(colName As String, header As String, weight As Single)
        dgvUsers.Columns.Add(New DataGridViewTextBoxColumn With {.Name = colName, .HeaderText = header, .FillWeight = weight,
                                                                .SortMode = DataGridViewColumnSortMode.NotSortable})
    End Sub

    Private Sub Grid_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs)
        If e.RowIndex >= 0 AndAlso e.ColumnIndex = dgvUsers.Columns("colId").Index Then
            e.CellStyle.ForeColor = Maroon
            e.CellStyle.Font = F(8.5!, FontStyle.Bold)
        End If
    End Sub

    ' status pill + one outlined action button (its text is the cell value, e.g. "Edit · Activity")
    Private Sub Grid_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs)
        If e.RowIndex < 0 Then Return
        Dim isStatus As Boolean = (e.ColumnIndex = dgvUsers.Columns("colStatus").Index)
        Dim isActions As Boolean = (e.ColumnIndex = dgvUsers.Columns("colActions").Index)
        If Not isStatus AndAlso Not isActions Then Return

        e.Paint(e.CellBounds, DataGridViewPaintParts.Background Or DataGridViewPaintParts.Border)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        Dim txt As String = Convert.ToString(e.Value)

        If txt <> "" Then
            If isStatus Then
                Dim back As Color = Color.FromArgb(238, 238, 236)
                Dim fore As Color = Color.FromArgb(100, 100, 100)
                Select Case txt.ToLower()
                    Case "active" : back = Color.FromArgb(232, 240, 233) : fore = Color.FromArgb(46, 110, 60)
                    Case "invite pending", "invitation pending" : back = Color.FromArgb(252, 240, 220) : fore = Color.FromArgb(176, 112, 32)
                    Case "suspended" : back = Color.FromArgb(250, 228, 230) : fore = Maroon
                End Select
                Using fnt As New Font("Segoe UI", 7.5!, FontStyle.Bold)
                    Dim sz As Size = TextRenderer.MeasureText(txt, fnt)
                    Dim r As New Rectangle(e.CellBounds.Left + 8, e.CellBounds.Top + (e.CellBounds.Height - 20) \ 2, sz.Width + 14, 20)
                    Using path As GraphicsPath = RoundRect(r, 4), br As New SolidBrush(back)
                        e.Graphics.FillPath(br, path)
                    End Using
                    TextRenderer.DrawText(e.Graphics, txt, fnt, r, fore,
                        TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter Or TextFormatFlags.NoPadding)
                End Using
            Else
                Dim r As Rectangle = ActionRect(e.CellBounds.Left, e.CellBounds.Top, e.CellBounds.Height, txt)
                Using path As GraphicsPath = RoundRect(r, 4), br As New SolidBrush(Color.White),
                      pen As New Pen(Color.FromArgb(215, 212, 206)), fnt As New Font("Segoe UI", 8.5!)
                    e.Graphics.FillPath(br, path)
                    e.Graphics.DrawPath(pen, path)
                    TextRenderer.DrawText(e.Graphics, txt, fnt, r, Ink,
                        TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter Or TextFormatFlags.NoPadding)
                End Using
            End If
        End If
        e.Handled = True
    End Sub

    Private Shared Function ActionRect(left As Integer, top As Integer, cellH As Integer, txt As String) As Rectangle
        Using fnt As New Font("Segoe UI", 8.5!)
            Dim w As Integer = TextRenderer.MeasureText(txt, fnt).Width + 22
            Return New Rectangle(left + 8, top + (cellH - 26) \ 2, w, 26)
        End Using
    End Function

    Private Function OverButton(e As DataGridViewCellMouseEventArgs) As Boolean
        If e.RowIndex < 0 OrElse e.ColumnIndex <> dgvUsers.Columns("colActions").Index Then Return False
        Dim txt As String = Convert.ToString(dgvUsers.Rows(e.RowIndex).Cells(e.ColumnIndex).Value)
        If txt = "" Then Return False
        Dim r As Rectangle = ActionRect(0, 0, dgvUsers.Rows(e.RowIndex).Height, txt)
        Return r.Contains(e.X, e.Y)
    End Function

    Private Sub Grid_CellMouseMove(sender As Object, e As DataGridViewCellMouseEventArgs)
        dgvUsers.Cursor = If(OverButton(e), Cursors.Hand, Cursors.Default)
    End Sub

    Private Sub Grid_CellMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs)
        If OverButton(e) Then
            ShowEdit()
            RaiseEvent EditUserRequested(e.RowIndex)
        End If
    End Sub
End Class