Imports System.Drawing.Drawing2D

' Access Permissions Matrix. Needs SK.vb and ucVehicleDetails.vb (FiCard).
Public Class ucPermissionMatrix
    Inherits UserControl

    Public Event ReviewClicked()
    Public Event CellClicked(rowIndex As Integer, colIndex As Integer)
    Public Event ConfirmClicked()
    Public Event CancelChangeClicked()

    ' ---- header / banner ----
    Public ReadOnly lblSubtitle As New Label
    Public ReadOnly btnReview As New Button
    Public ReadOnly lblGuardTitle As New Label
    Public ReadOnly lblGuardNote As New Label

    ' ---- matrix ----
    Public ReadOnly lblMatrixTitle As New Label
    Public ReadOnly lblMatrixSub As New Label
    Public ReadOnly lblLegend As New Label
    Public ReadOnly dgvMatrix As New DataGridView

    ' ---- confirm card ----
    Public ReadOnly lblDraftTitle As New Label
    Public ReadOnly lblDraftSub As New Label
    Public ReadOnly lblBefore As New Label
    Public ReadOnly lblAfter As New Label
    Public ReadOnly txtReason As New TextBox
    Public ReadOnly chkReviewed As New CheckBox
    Public ReadOnly btnCancel As New Button
    Public ReadOnly btnConfirm As New Button
    Public ReadOnly lblAuditTitle As New Label
    Public ReadOnly lblAuditNote As New Label

    Private cGuard As Panel
    Private cMatrix, cConfirm As FiCard
    Private capBefore, capAfter, capReason As Label
    Private lblPageTitle As Label
    Private ReadOnly pnl As New Panel

    Public Sub New()
        Me.BackColor = SK.PageBg
        BuildUI()
    End Sub

    ' ---------------- public ----------------
    Public Sub SetRoles(roles As IEnumerable(Of String))
        dgvMatrix.Columns.Clear()
        SK.AddCol(dgvMatrix, "colPerm", "Permission", 16)
        Dim i As Integer = 0
        For Each r As String In roles
            SK.AddCol(dgvMatrix, "colRole" & i, r, 16)
            i += 1
        Next
    End Sub

    Public Sub ShowDraft(subtitle As String, before As String, after As String)
        lblDraftSub.Text = subtitle
        lblBefore.Text = before
        lblAfter.Text = after & " · pending confirmation"
    End Sub

    Public Sub ClearDraft()
        lblDraftSub.Text = "" : lblBefore.Text = "" : lblAfter.Text = ""
        txtReason.Text = "" : chkReviewed.Checked = False
    End Sub

    Public Sub FocusDraft()
        pnl.ScrollControlIntoView(cConfirm)
        txtReason.Focus()
    End Sub

    ' ---------------- layout ----------------
    Private Sub Arrange()
        Dim w As Integer = Math.Max(pnl.ClientSize.Width, 900) - 48
        btnReview.Location = New Point(24 + w - btnReview.Width, 18)
        cGuard.SetBounds(24, 88, w, 50)
        lblGuardTitle.SetBounds(14, 6, w - 28, 18)
        lblGuardNote.SetBounds(14, 26, w - 28, 18)

        cMatrix.SetBounds(24, 152, w, 70 + 34 + 11 * 34 + 14)
        lblLegend.SetBounds(w - 16 - 320, 16, 320, 16)
        dgvMatrix.SetBounds(12, 60, w - 24, 34 + 11 * 34)

        Dim y As Integer = 152 + cMatrix.Height + 16
        Dim cw As Integer = CInt(w * 0.58)
        cConfirm.SetBounds(24, y, cw, 196)
        lblDraftTitle.SetBounds(16, 10, cw - 32, 20)
        lblDraftSub.SetBounds(16, 30, cw - 32, 16)
        Dim half As Integer = (cw - 32) \ 2
        capBefore.SetBounds(16, 54, half, 14) : capAfter.SetBounds(16 + half, 54, half, 14)
        lblBefore.SetBounds(16, 70, half, 16) : lblAfter.SetBounds(16 + half, 70, half, 16)
        capReason.SetBounds(16, 96, 200, 16)
        txtReason.SetBounds(16, 114, cw - 32, 24)
        chkReviewed.SetBounds(16, 142, cw - 32, 20)
        btnCancel.Location = New Point(16, 166)
        btnConfirm.Location = New Point(16 + btnCancel.Width + 8, 166)
        lblAuditTitle.SetBounds(24 + cw + 16, y + 4, w - cw - 16, 18)
        lblAuditNote.SetBounds(24 + cw + 16, y + 24, w - cw - 16, 70)
        pnl.AutoScrollMinSize = New Size(900, y + 196 + 30)
    End Sub

    Private Sub BuildUI()
        Me.SuspendLayout()
        pnl.Dock = DockStyle.Fill : pnl.BackColor = SK.PageBg : pnl.AutoScroll = True
        Controls.Add(pnl)

        lblPageTitle = SK.Cap("Access Permissions Matrix", SK.F(20, FontStyle.Bold), SK.Ink, 22, 14)
        pnl.Controls.Add(lblPageTitle)
        SK.Spot(lblSubtitle, SK.F(9), SK.Muted) : lblSubtitle.SetBounds(26, 58, 800, 18)
        pnl.Controls.Add(lblSubtitle)
        SK.StyleBtn(btnReview, "Review permission changes", 200, 36, True)
        pnl.Controls.Add(btnReview)

        ' guardrail banner
        cGuard = New Panel With {.BackColor = Color.FromArgb(250, 232, 234)}
        SK.Spot(lblGuardTitle, SK.F(8.5!, FontStyle.Bold), SK.Maroon)
        SK.Spot(lblGuardNote, SK.F(8), SK.Muted)
        cGuard.Controls.AddRange(New Control() {lblGuardTitle, lblGuardNote})
        pnl.Controls.Add(cGuard)

        ' matrix card
        cMatrix = New FiCard
        SK.Spot(lblMatrixTitle, SK.F(10.5!, FontStyle.Bold), SK.Ink) : lblMatrixTitle.SetBounds(16, 10, 500, 20)
        SK.Spot(lblMatrixSub, SK.F(8), SK.Muted) : lblMatrixSub.SetBounds(16, 31, 600, 16)
        SK.Spot(lblLegend, SK.F(8), SK.Muted, ContentAlignment.TopRight)
        lblLegend.Text = "✓ Allowed     ● Read-only     — Restricted"
        SK.StyleGrid(dgvMatrix, 34)
        dgvMatrix.Cursor = Cursors.Hand
        cMatrix.Controls.AddRange(New Control() {lblMatrixTitle, lblMatrixSub, lblLegend, dgvMatrix})
        pnl.Controls.Add(cMatrix)

        ' confirm card
        cConfirm = New FiCard
        SK.Spot(lblDraftTitle, SK.F(10.5!, FontStyle.Bold), SK.Ink) : lblDraftTitle.Text = "Confirm sensitive permission change"
        SK.Spot(lblDraftSub, SK.F(8), SK.Muted)
        capBefore = SK.Cap("BEFORE", SK.F(7.5!), SK.Muted, 0, 0) : capAfter = SK.Cap("AFTER · PENDING CONFIRMATION", SK.F(7.5!), SK.Muted, 0, 0)
        capBefore.AutoSize = False : capAfter.AutoSize = False
        capAfter.Text = "AFTER"
        SK.Spot(lblBefore, SK.F(8.5!), SK.Ink) : SK.Spot(lblAfter, SK.F(8.5!), SK.Ink)
        capReason = SK.Cap("Audit reason *", SK.F(8, FontStyle.Bold), SK.Ink, 0, 0) : capReason.AutoSize = False
        SK.StyleText(txtReason)
        chkReviewed.Text = "I have reviewed branch scope and permission bounds"
        chkReviewed.AutoSize = False : chkReviewed.BackColor = Color.Transparent : chkReviewed.Font = SK.F(8)
        SK.StyleBtn(btnCancel, "Cancel change", 110, 28, False)
        SK.StyleBtn(btnConfirm, "Confirm sensitive change", 170, 28, False)
        btnConfirm.ForeColor = SK.Maroon : btnConfirm.FlatAppearance.BorderColor = SK.Maroon
        cConfirm.Controls.AddRange(New Control() {lblDraftTitle, lblDraftSub, capBefore, capAfter, lblBefore, lblAfter,
                                                  capReason, txtReason, chkReviewed, btnCancel, btnConfirm})
        pnl.Controls.Add(cConfirm)

        SK.Spot(lblAuditTitle, SK.F(8.5!, FontStyle.Bold), SK.Muted)
        SK.Spot(lblAuditNote, SK.F(8), SK.Muted)
        pnl.Controls.AddRange(New Control() {lblAuditTitle, lblAuditNote})

        AddHandler pnl.Resize, Sub(s, e) Arrange()
        AddHandler btnReview.Click, Sub(s, e) RaiseEvent ReviewClicked()
        AddHandler btnConfirm.Click, Sub(s, e) RaiseEvent ConfirmClicked()
        AddHandler btnCancel.Click, Sub(s, e) RaiseEvent CancelChangeClicked()
        AddHandler dgvMatrix.CellPainting, AddressOf Grid_CellPainting
        AddHandler dgvMatrix.CellMouseClick, Sub(s, e)
                                                 If e.RowIndex >= 0 AndAlso e.ColumnIndex > 0 Then RaiseEvent CellClicked(e.RowIndex, e.ColumnIndex)
                                             End Sub
        Me.ResumeLayout(False)
        Arrange()
    End Sub

    ' ---------------- grid painting ----------------
    Private Sub Grid_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs)
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return
        Dim txt As String = Convert.ToString(e.Value)
        Dim pending As Boolean = (Convert.ToString(dgvMatrix.Rows(e.RowIndex).Cells(e.ColumnIndex).Tag) = "pending")
        e.Paint(e.CellBounds, DataGridViewPaintParts.Border)
        Using br As New SolidBrush(If(pending, Color.FromArgb(253, 244, 225), Color.White))
            e.Graphics.FillRectangle(br, New Rectangle(e.CellBounds.Left, e.CellBounds.Top, e.CellBounds.Width, e.CellBounds.Height - 1))
        End Using
        Dim flags As TextFormatFlags = TextFormatFlags.VerticalCenter Or TextFormatFlags.Left Or TextFormatFlags.NoPadding
        Dim r As New Rectangle(e.CellBounds.Left + 8, e.CellBounds.Top, e.CellBounds.Width - 8, e.CellBounds.Height)
        If e.ColumnIndex = 0 Then
            TextRenderer.DrawText(e.Graphics, txt, SK.F(8.5!, FontStyle.Bold), r, SK.Maroon, flags)
        Else
            Select Case txt
                Case "Allowed" : TextRenderer.DrawText(e.Graphics, "✓ Allowed", SK.F(8), r, Color.FromArgb(46, 110, 60), flags)
                Case "Read-only" : TextRenderer.DrawText(e.Graphics, "● Read-only", SK.F(8), r, Color.FromArgb(90, 90, 90), flags)
                Case Else : TextRenderer.DrawText(e.Graphics, "— Restricted", SK.F(8), r, SK.Muted, flags)
            End Select
        End If
        e.Handled = True
    End Sub
End Class