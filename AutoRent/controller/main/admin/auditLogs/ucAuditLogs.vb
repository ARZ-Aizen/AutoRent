' Audit Logs (read-only register + selected event details). Needs SK.vb and ucVehicleDetails.vb (FiCard).
Public Class ucAuditViewer
    Inherits UserControl

    Public Event FilterChanged()
    Public Event EventSelected(rowIndex As Integer)
    Public Event PageRequested(page As Integer)

    Public ReadOnly lblSubtitle As New Label

    ' ---- filters ----
    Public ReadOnly txtSearch As New TextBox
    Public ReadOnly cboRange As New ComboBox
    Public ReadOnly cboType As New ComboBox
    Public ReadOnly cboResult As New ComboBox

    ' ---- register ----
    Public ReadOnly lblRegisterTitle As New Label
    Public ReadOnly lblRegisterSub As New Label
    Public ReadOnly dgvEvents As New DataGridView
    Public ReadOnly lblCount As New Label
    Public ReadOnly btnPrev As New Button
    Public ReadOnly btnPage1 As New Button
    Public ReadOnly btnPage2 As New Button
    Public ReadOnly btnPage3 As New Button
    Public ReadOnly btnNext As New Button

    ' ---- details ----
    Public ReadOnly lblEvTitle As New Label
    Public ReadOnly lblEvSub As New Label
    Public ReadOnly lblEvResult As New Label
    Public ReadOnly lblActor As New Label
    Public ReadOnly lblRole As New Label
    Public ReadOnly lblRecord As New Label
    Public ReadOnly lblOccurred As New Label
    Public ReadOnly lblSource As New Label
    Public ReadOnly lblReference As New Label
    Public ReadOnly lblChange As New Label
    Public ReadOnly lblReason As New Label
    Public ReadOnly lblCtxTitle As New Label
    Public ReadOnly lblCtxNote As New Label
    Public ReadOnly lblImmutableTitle As New Label
    Public ReadOnly lblImmutableNote As New Label

    Private cFilter, cReg, cDetail As FiCard
    Private pnlCtx As Panel
    Private capSearch, capRange, capType, capResult, lblBadge, capBA As Label
    Private ReadOnly _detCaps As New List(Of Label)
    Private ReadOnly _pageBtns() As Button
    Private ReadOnly pnl As New Panel
    Private _page As Integer = 1
    Private _pages As Integer = 1

    Public Sub New()
        Me.BackColor = SK.PageBg
        _pageBtns = {btnPage1, btnPage2, btnPage3}
        BuildUI()
    End Sub

    ' ---------------- public ----------------
    Public Sub SetPaging(page As Integer, totalPages As Integer)
        _page = page : _pages = Math.Max(1, totalPages)
        Dim first As Integer = Math.Max(1, Math.Min(page - 1, _pages - 2))
        For i As Integer = 0 To 2
            Dim n As Integer = first + i
            _pageBtns(i).Text = n.ToString()
            _pageBtns(i).Visible = (n <= _pages)
            Dim active As Boolean = (n = page)
            _pageBtns(i).BackColor = If(active, SK.Maroon, Color.White)
            _pageBtns(i).ForeColor = If(active, Color.White, SK.Ink)
        Next
        btnPrev.Enabled = page > 1
        btnNext.Enabled = page < _pages
    End Sub

    ' ---------------- layout ----------------
    Private Sub Arrange()
        Dim w As Integer = Math.Max(pnl.ClientSize.Width, 900) - 48
        lblBadge.Location = New Point(24 + w - lblBadge.Width, 22)
        cFilter.SetBounds(24, 92, w, 78)
        Dim detW As Integer = 290
        Dim regW As Integer = w - detW - 16
        cReg.SetBounds(24, 184, regW, 560)
        dgvEvents.SetBounds(12, 60, regW - 24, 34 + 13 * 34)
        lblCount.SetBounds(16, 60 + 34 + 13 * 34 + 14, 300, 16)
        btnNext.Location = New Point(regW - 16 - btnNext.Width, 60 + 34 + 13 * 34 + 8)
        btnPage3.Location = New Point(btnNext.Left - 4 - btnPage3.Width, btnNext.Top)
        btnPage2.Location = New Point(btnPage3.Left - 4 - btnPage2.Width, btnNext.Top)
        btnPage1.Location = New Point(btnPage2.Left - 4 - btnPage1.Width, btnNext.Top)
        btnPrev.Location = New Point(btnPage1.Left - 4 - btnPrev.Width, btnNext.Top)

        cDetail.SetBounds(24 + regW + 16, 184, detW, 440)
        Dim iw As Integer = detW - 32
        lblEvTitle.SetBounds(16, 12, iw, 20)
        lblEvSub.SetBounds(16, 32, iw, 14)
        lblEvResult.Location = New Point(16, 52)
        Dim y As Integer = 82
        Dim vals() As Label = {lblActor, lblRole, lblRecord, lblOccurred, lblSource, lblReference}
        For i As Integer = 0 To 5
            _detCaps(i).SetBounds(16, y, 110, 16)
            vals(i).SetBounds(16 + 110, y, iw - 110, 16)
            y += 24
        Next
        capBA.SetBounds(16, y + 4, iw, 14)
        lblChange.SetBounds(16, y + 22, iw, 18)
        lblReason.SetBounds(16, y + 44, iw, 44)
        pnlCtx.SetBounds(16, y + 94, iw, 70)
        lblCtxTitle.SetBounds(10, 6, iw - 20, 16)
        lblCtxNote.SetBounds(10, 24, iw - 20, 40)
        lblImmutableTitle.SetBounds(24 + regW + 16, 184 + 440 + 12, detW, 16)
        lblImmutableNote.SetBounds(24 + regW + 16, 184 + 440 + 30, detW, 50)
        pnl.AutoScrollMinSize = New Size(900, 770)
    End Sub

    Private Sub BuildUI()
        Me.SuspendLayout()
        pnl.Dock = DockStyle.Fill : pnl.BackColor = SK.PageBg : pnl.AutoScroll = True
        Controls.Add(pnl)

        pnl.Controls.Add(SK.Cap("Audit Logs", SK.F(20, FontStyle.Bold), SK.Ink, 22, 14))
        SK.Spot(lblSubtitle, SK.F(9), SK.Muted) : lblSubtitle.SetBounds(26, 58, 700, 18)
        pnl.Controls.Add(lblSubtitle)
        lblBadge = SK.Cap("No edit or delete actions", SK.F(7.5!, FontStyle.Bold), SK.Maroon, 0, 0)
        lblBadge.AutoSize = False : lblBadge.Size = New Size(150, 20) : lblBadge.TextAlign = ContentAlignment.MiddleCenter
        lblBadge.BackColor = Color.FromArgb(250, 228, 230)
        pnl.Controls.Add(lblBadge)

        ' filters
        cFilter = New FiCard
        capSearch = SK.Cap("Search events", SK.F(8, FontStyle.Bold), SK.Ink, 0, 10)
        capRange = SK.Cap("Date range", SK.F(8, FontStyle.Bold), SK.Ink, 0, 10)
        capType = SK.Cap("Event type", SK.F(8, FontStyle.Bold), SK.Ink, 0, 10)
        capResult = SK.Cap("Result", SK.F(8, FontStyle.Bold), SK.Ink, 0, 10)
        SK.StyleText(txtSearch) : SK.StyleCombo(cboRange) : SK.StyleCombo(cboType) : SK.StyleCombo(cboResult)
        cFilter.Controls.AddRange(New Control() {capSearch, capRange, capType, capResult, txtSearch, cboRange, cboType, cboResult})
        AddHandler cFilter.Resize, Sub(s, e)
                                       Dim gap As Integer = 14
                                       Dim cw As Integer = (cFilter.Width - 36 - gap * 3) \ 4
                                       Dim x As Integer = 18
                                       For Each pair As Tuple(Of Label, Control) In New Tuple(Of Label, Control)() {
                                               Tuple.Create(Of Label, Control)(capSearch, txtSearch),
                                               Tuple.Create(Of Label, Control)(capRange, cboRange),
                                               Tuple.Create(Of Label, Control)(capType, cboType),
                                               Tuple.Create(Of Label, Control)(capResult, cboResult)}
                                           pair.Item1.Location = New Point(x, 10)
                                           pair.Item2.SetBounds(x, 34, cw, 26)
                                           x += cw + gap
                                       Next
                                   End Sub
        pnl.Controls.Add(cFilter)

        ' register
        cReg = New FiCard
        lblRegisterTitle.Text = "Event register"
        SK.Spot(lblRegisterTitle, SK.F(10.5!, FontStyle.Bold), SK.Ink) : lblRegisterTitle.SetBounds(16, 10, 400, 20)
        SK.Spot(lblRegisterSub, SK.F(8), SK.Muted) : lblRegisterSub.SetBounds(16, 31, 600, 16)
        SK.StyleGrid(dgvEvents, 34)
        SK.AddCol(dgvEvents, "colId", "Event ID", 9)
        SK.AddCol(dgvEvents, "colTime", "Time", 8)
        SK.AddCol(dgvEvents, "colUserId", "User ID", 9)
        SK.AddCol(dgvEvents, "colUser", "Username", 11)
        SK.AddCol(dgvEvents, "colRole", "Role", 13)
        SK.AddCol(dgvEvents, "colAction", "Action", 20)
        SK.AddCol(dgvEvents, "colRecord", "Affected Record", 14)
        SK.AddCol(dgvEvents, "colResult", "Result", 10)
        SK.Spot(lblCount, SK.F(8), SK.Muted)
        For Each b As Button In New Button() {btnPrev, btnPage1, btnPage2, btnPage3, btnNext}
            SK.StyleBtn(b, "", 28, 28, False)
        Next
        btnPrev.Text = "‹" : btnNext.Text = "›"
        cReg.Controls.AddRange(New Control() {lblRegisterTitle, lblRegisterSub, dgvEvents, lblCount, btnPrev, btnPage1, btnPage2, btnPage3, btnNext})
        pnl.Controls.Add(cReg)

        ' details
        cDetail = New FiCard
        SK.Spot(lblEvTitle, SK.F(10, FontStyle.Bold), SK.Ink)
        SK.Spot(lblEvSub, SK.F(8), SK.Muted)
        lblEvResult.AutoSize = True : lblEvResult.Font = SK.F(7.5!, FontStyle.Bold) : lblEvResult.Padding = New Padding(6, 2, 6, 2)
        lblEvResult.UseMnemonic = False : lblEvResult.ForeColor = Color.FromArgb(46, 110, 60) : lblEvResult.BackColor = Color.FromArgb(232, 240, 233)
        cDetail.Controls.AddRange(New Control() {lblEvTitle, lblEvSub, lblEvResult})
        Dim caps() As String = {"Actor", "Role", "Affected record", "Occurred at", "Source", "Request reference"}
        Dim vals() As Label = {lblActor, lblRole, lblRecord, lblOccurred, lblSource, lblReference}
        For i As Integer = 0 To 5
            Dim c As Label = SK.Cap(caps(i), SK.F(8), SK.Muted, 0, 0) : c.AutoSize = False
            _detCaps.Add(c) : cDetail.Controls.Add(c)
            SK.Spot(vals(i), SK.F(8, FontStyle.Bold), SK.Ink, ContentAlignment.TopRight)
            cDetail.Controls.Add(vals(i))
        Next
        capBA = SK.Cap("BEFORE → AFTER", SK.F(7.5!), SK.Muted, 0, 0) : capBA.AutoSize = False
        SK.Spot(lblChange, SK.F(8.5!, FontStyle.Bold), SK.Ink)
        SK.Spot(lblReason, SK.F(8), SK.Muted)
        pnlCtx = New Panel With {.BackColor = Color.FromArgb(240, 238, 234)}
        SK.Spot(lblCtxTitle, SK.F(8, FontStyle.Bold), SK.Muted) : SK.Spot(lblCtxNote, SK.F(7.5!), SK.Muted)
        pnlCtx.Controls.AddRange(New Control() {lblCtxTitle, lblCtxNote})
        cDetail.Controls.AddRange(New Control() {capBA, lblChange, lblReason, pnlCtx})
        pnl.Controls.Add(cDetail)
        SK.Spot(lblImmutableTitle, SK.F(8.5!, FontStyle.Bold), SK.Muted)
        SK.Spot(lblImmutableNote, SK.F(8), SK.Muted)
        pnl.Controls.AddRange(New Control() {lblImmutableTitle, lblImmutableNote})

        AddHandler pnl.Resize, Sub(s, e) Arrange()
        AddHandler txtSearch.TextChanged, Sub(s, e) RaiseEvent FilterChanged()
        For Each c As ComboBox In New ComboBox() {cboRange, cboType, cboResult}
            AddHandler c.SelectedIndexChanged, Sub(s, e) RaiseEvent FilterChanged()
        Next
        AddHandler dgvEvents.SelectionChanged, Sub(s, e)
                                                   If dgvEvents.CurrentRow IsNot Nothing Then RaiseEvent EventSelected(dgvEvents.CurrentRow.Index)
                                               End Sub
        AddHandler dgvEvents.CellPainting, Sub(s, e)
                                               If e.RowIndex >= 0 AndAlso e.ColumnIndex = dgvEvents.Columns("colResult").Index Then SK.Pill(e)
                                           End Sub
        AddHandler dgvEvents.CellFormatting, Sub(s, e)
                                                 If e.RowIndex >= 0 AndAlso e.ColumnIndex = 0 Then
                                                     e.CellStyle.ForeColor = SK.Maroon : e.CellStyle.Font = SK.F(8.5!, FontStyle.Bold)
                                                 End If
                                             End Sub
        AddHandler btnPrev.Click, Sub(s, e) RaiseEvent PageRequested(_page - 1)
        AddHandler btnNext.Click, Sub(s, e) RaiseEvent PageRequested(_page + 1)
        For Each b As Button In _pageBtns
            Dim bb As Button = b
            AddHandler bb.Click, Sub(s, e) RaiseEvent PageRequested(Convert.ToInt32(bb.Text))
        Next
        Me.ResumeLayout(False)
        Arrange()
    End Sub
End Class