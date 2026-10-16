' Rate Rule Editor. All data spots are empty: fill them from your code.
' Needs ucVehicleDetails.vb in the project (it holds the FiCard class used here).
Public Class ucRateConfiguration
    Inherits UserControl

    Private Shared ReadOnly Maroon As Color = Color.FromArgb(122, 0, 16)
    Private Shared ReadOnly Ink As Color = Color.FromArgb(38, 38, 38)
    Private Shared ReadOnly Muted As Color = Color.FromArgb(120, 120, 120)
    Private Shared ReadOnly BorderCol As Color = Color.FromArgb(228, 225, 220)
    Private Shared ReadOnly PageBg As Color = Color.FromArgb(245, 244, 242)

    Public Event SaveDraftClicked()
    Public Event CancelClicked()
    Public Event KeepDraftClicked()
    Public Event ReviewConfirmClicked()

    ' ---- header ----
    Public ReadOnly lblSubtitle As New Label
    Public ReadOnly lblUnsaved As New Label

    ' ---- rule definition fields ----
    Public ReadOnly txtRuleName As New TextBox
    Public ReadOnly txtRuleId As New TextBox
    Public ReadOnly cboCategory As New ComboBox
    Public ReadOnly cboOverride As New ComboBox
    Public ReadOnly cboMethod As New ComboBox
    Public ReadOnly txtRate As New TextBox
    Public ReadOnly dtpStart As New DateTimePicker
    Public ReadOnly dtpEnd As New DateTimePicker
    Public ReadOnly cboCondition As New ComboBox
    Public ReadOnly txtPrecedence As New TextBox
    Public ReadOnly cboCharges As New ComboBox
    Public ReadOnly cboState As New ComboBox
    Public ReadOnly txtReason As New TextBox
    Public ReadOnly lblDefinitionSub As New Label

    ' ---- conflict box ----
    Public ReadOnly lblConflictTitle As New Label
    Public ReadOnly lblConflict As New Label

    ' ---- example booking preview ----
    Public ReadOnly lblPreviewSub As New Label
    Public ReadOnly lblPrevVehicle As New Label
    Public ReadOnly lblPrevPeriod As New Label
    Public ReadOnly lblPrevDuration As New Label
    Public ReadOnly lblPrevBase As New Label
    Public ReadOnly lblPrevRule1Cap As New Label
    Public ReadOnly lblPrevRule1 As New Label
    Public ReadOnly lblPrevRule2Cap As New Label
    Public ReadOnly lblPrevRule2 As New Label
    Public ReadOnly lblPrevTotal As New Label
    Public ReadOnly lblPrevNote As New Label

    ' ---- confirm important rate update ----
    Public ReadOnly lblConfirmSub As New Label
    Public ReadOnly chkNewOnly As New CheckBox
    Public ReadOnly chkNoFinalized As New CheckBox
    Public ReadOnly lblPublishTitle As New Label
    Public ReadOnly lblPublishNote As New Label
    Public ReadOnly btnKeepDraft As New Button
    Public ReadOnly btnReview As New Button

    ' ---- footer ----
    Public ReadOnly lblLastModified As New Label
    Public ReadOnly btnCancel As New Button
    Public ReadOnly btnSaveDraft As New Button

    Private Class FieldItem
        Public Cap As Label
        Public Ctl As Control
        Public Y As Integer
        Public Col As Integer   ' 0 = left, 1 = right, 2 = full width
    End Class

    Private ReadOnly _fields As New List(Of FieldItem)
    Private cDef, cPrev, cConfirm, cFooter As FiCard
    Private pnlConflict, pnlPublish As Panel
    Private ReadOnly _previewCaps As New List(Of Label)

    Public Sub New()
        Me.BackColor = PageBg
        Me.AutoScroll = True
        Me.AutoScrollMinSize = New Size(900, 760)
        BuildUI()
        ClearForm()
    End Sub

    ' ---------------- public ----------------
    Public Sub ClearForm()
        txtRuleName.Text = "" : txtRate.Text = "" : txtPrecedence.Text = "" : txtReason.Text = ""
        txtRuleId.Text = "Assigned on save"
        For Each c As ComboBox In New ComboBox() {cboCategory, cboMethod, cboCondition, cboCharges, cboState}
            c.SelectedIndex = -1
        Next
        If cboOverride.Items.Count > 0 Then cboOverride.SelectedIndex = 0
        dtpStart.Value = Date.Today
        dtpEnd.Value = Date.Today.AddMonths(3)
        chkNewOnly.Checked = False : chkNoFinalized.Checked = False
        ShowConflict("", "")
    End Sub

    ''' <summary>Shows or hides the red conflict box. Pass empty text to hide it.</summary>
    Public Sub ShowConflict(title As String, message As String)
        lblConflictTitle.Text = title
        lblConflict.Text = message
        pnlConflict.Visible = (title <> "" OrElse message <> "")
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
        Dim place As Action = Sub() ctl.Left = card.Width - margin - ctl.Width
        AddHandler card.Resize, Sub(s, e) place()
        AddHandler ctl.SizeChanged, Sub(s, e) place()
    End Sub

    Private Shared Sub StyleBtn(b As Button, text As String, w As Integer, h As Integer, primary As Boolean)
        b.Text = text : b.Size = New Size(w, h) : b.FlatStyle = FlatStyle.Flat
        b.Font = F(9) : b.Cursor = Cursors.Hand : b.UseVisualStyleBackColor = False : b.UseMnemonic = False
        b.BackColor = If(primary, Maroon, Color.White)
        b.ForeColor = If(primary, Color.White, Ink)
        b.FlatAppearance.BorderColor = If(primary, Maroon, BorderCol)
        b.FlatAppearance.BorderSize = 1
    End Sub

    Private Shared Sub StyleCheck(c As CheckBox)
        c.AutoSize = True : c.Font = F(8.5!) : c.ForeColor = Ink
        c.BackColor = Color.Transparent : c.UseMnemonic = False
    End Sub

    Private Sub AddField(caption As String, ctl As Control, y As Integer, col As Integer)
        Dim fi As New FieldItem With {.Cap = Cap(caption, F(8, FontStyle.Bold), Ink, 0, 0), .Ctl = ctl, .Y = y, .Col = col}
        If TypeOf ctl Is TextBox Then
            DirectCast(ctl, TextBox).BorderStyle = BorderStyle.FixedSingle : ctl.Font = F(9)
        ElseIf TypeOf ctl Is ComboBox Then
            Dim cb = DirectCast(ctl, ComboBox)
            cb.FlatStyle = FlatStyle.Flat : cb.BackColor = Color.White : cb.Font = F(9)
            cb.DropDownStyle = ComboBoxStyle.DropDownList
        ElseIf TypeOf ctl Is DateTimePicker Then
            Dim d = DirectCast(ctl, DateTimePicker)
            d.Format = DateTimePickerFormat.Custom : d.CustomFormat = "dd MMM yyyy HH:mm" : d.Font = F(9)
        End If
        cDef.Controls.Add(fi.Cap) : cDef.Controls.Add(ctl)
        _fields.Add(fi)
    End Sub

    Private Sub AddPreviewRow(card As FiCard, caption As Label, value As Label, y As Integer)
        caption.Font = F(8) : caption.ForeColor = Muted : caption.BackColor = Color.Transparent
        caption.AutoSize = True : caption.Location = New Point(18, y) : caption.UseMnemonic = False
        card.Controls.Add(caption)
        Spot(value, F(8.5!, FontStyle.Bold), Ink, 0, y - 1)
        card.Controls.Add(value)
        PinRight(card, value, 18)
    End Sub

    ' ---------------- layout ----------------
    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e)
        ArrangeLayout()
    End Sub

    Private Sub ArrangeLayout()
        If cFooter Is Nothing Then Return
        Dim w As Integer = Math.Max(ClientSize.Width, 900) - 48
        Dim lw As Integer = w - 316
        Dim rx As Integer = 24 + lw + 16
        lblUnsaved.Location = New Point(24 + w - lblUnsaved.Width, 34)
        cDef.SetBounds(24, 96, lw, 470)
        cPrev.SetBounds(rx, 96, 300, 296)
        cConfirm.SetBounds(rx, 408, 300, 226)
        cFooter.SetBounds(24, 650, w, 60)
    End Sub

    Private Sub BuildUI()
        Me.SuspendLayout()

        ' ----- header -----
        Controls.Add(Cap("Rate Rule Editor", F(20, FontStyle.Bold), Ink, 22, 14))
        Spot(lblSubtitle, F(9), Muted, 26, 58)
        Controls.Add(lblSubtitle)
        lblUnsaved.AutoSize = True : lblUnsaved.Font = F(8, FontStyle.Bold)
        lblUnsaved.ForeColor = Color.FromArgb(176, 112, 32) : lblUnsaved.BackColor = Color.Transparent
        lblUnsaved.UseMnemonic = False
        Controls.Add(lblUnsaved)
        AddHandler lblUnsaved.SizeChanged, Sub(s, e) ArrangeLayout()

        ' ----- rule definition card -----
        cDef = New FiCard
        cDef.Controls.Add(Cap("Rule definition", F(11, FontStyle.Bold), Ink, 18, 12))
        Spot(lblDefinitionSub, F(7.5!), Muted, 18, 34)
        cDef.Controls.Add(lblDefinitionSub)

        txtRuleId.ReadOnly = True : txtRuleId.BackColor = Color.FromArgb(238, 238, 236)
        cboCategory.Items.AddRange(New Object() {"All categories", "Sedan", "SUV", "Van", "Pickup"})
        cboOverride.Items.Add("None · category-wide rule")
        cboMethod.Items.AddRange(New Object() {"Base rental multiplier", "Fixed daily rate", "Fixed add-on amount", "Percentage discount"})
        cboCondition.Items.AddRange(New Object() {"Always applies", "Pickup day is Friday, Saturday or Sunday", "Rental of 7 days or more", "Holiday period"})
        cboCharges.Items.AddRange(New Object() {"Rental base only · excludes fees and deposits", "Rental base and fees", "Fees only"})
        cboState.Items.AddRange(New Object() {"Active", "Inactive"})

        AddField("Rule name *", txtRuleName, 62, 0)
        AddField("Rule ID", txtRuleId, 62, 1)
        AddField("Vehicle category *", cboCategory, 118, 0)
        AddField("Specific vehicle override", cboOverride, 118, 1)
        AddField("Calculation method *", cboMethod, 174, 0)
        AddField("Rate / multiplier *", txtRate, 174, 1)
        AddField("Effective start *", dtpStart, 230, 0)
        AddField("Effective end *", dtpEnd, 230, 1)
        AddField("Condition *", cboCondition, 286, 0)
        AddField("Precedence *", txtPrecedence, 286, 1)
        AddField("Applicable charges", cboCharges, 342, 0)
        AddField("Rule state", cboState, 342, 1)
        AddField("Change reason *", txtReason, 398, 2)

        pnlConflict = New Panel With {.BackColor = Color.FromArgb(250, 228, 230), .Location = New Point(18, 446)}
        cDef.Controls.Add(pnlConflict)
        Spot(lblConflictTitle, F(8, FontStyle.Bold), Maroon, 8, 6)
        lblConflict.AutoSize = False : lblConflict.Font = F(7.5!) : lblConflict.ForeColor = Muted
        lblConflict.BackColor = Color.Transparent : lblConflict.UseMnemonic = False
        pnlConflict.Controls.AddRange(New Control() {lblConflictTitle, lblConflict})

        AddHandler cDef.Resize, Sub(s, e)
                                    Dim fullW As Integer = cDef.Width - 36
                                    Dim halfW As Integer = (fullW - 14) \ 2
                                    For Each fi In _fields
                                        Dim x As Integer = 18
                                        Dim wd As Integer = fullW
                                        If fi.Col = 0 Then wd = halfW
                                        If fi.Col = 1 Then x = 18 + halfW + 14 : wd = halfW
                                        fi.Cap.Location = New Point(x, fi.Y)
                                        fi.Ctl.SetBounds(x, fi.Y + 18, wd, 26)
                                    Next
                                    pnlConflict.SetBounds(18, 446, fullW, 52)
                                    lblConflict.SetBounds(8, 24, fullW - 16, 26)
                                End Sub
        Controls.Add(cDef)

        ' ----- example booking preview -----
        cPrev = New FiCard
        cPrev.Controls.Add(Cap("Example booking preview", F(11, FontStyle.Bold), Ink, 18, 12))
        Spot(lblPreviewSub, F(7.5!), Muted, 18, 34)
        cPrev.Controls.Add(lblPreviewSub)
        Dim capVeh As Label = Cap("Category / vehicle", F(8), Muted, 18, 62)
        Dim capPer As Label = Cap("Pickup → return", F(8), Muted, 18, 88)
        Dim capDur As Label = Cap("Duration", F(8), Muted, 18, 114)
        AddPreviewRow(cPrev, capVeh, lblPrevVehicle, 62)
        AddPreviewRow(cPrev, capPer, lblPrevPeriod, 88)
        AddPreviewRow(cPrev, capDur, lblPrevDuration, 114)
        AddPreviewRow(cPrev, Cap("Base", F(8), Muted, 18, 148), lblPrevBase, 148)
        AddPreviewRow(cPrev, lblPrevRule1Cap, lblPrevRule1, 172)
        AddPreviewRow(cPrev, lblPrevRule2Cap, lblPrevRule2, 196)
        cPrev.Controls.Add(New Panel With {.BackColor = BorderCol, .Location = New Point(18, 222), .Size = New Size(264, 1), .Name = "prevLine"})
        AddPreviewRow(cPrev, Cap("Preview total", F(8.5!, FontStyle.Bold), Ink, 18, 232), lblPrevTotal, 232)
        lblPrevNote.AutoSize = False : lblPrevNote.Font = F(7.5!) : lblPrevNote.ForeColor = Muted
        lblPrevNote.BackColor = Color.Transparent : lblPrevNote.UseMnemonic = False
        lblPrevNote.SetBounds(18, 256, 264, 34)
        cPrev.Controls.Add(lblPrevNote)
        AddHandler cPrev.Resize, Sub(s, e)
                                     lblPrevNote.Width = cPrev.Width - 36
                                     For Each c As Control In cPrev.Controls
                                         If c.Name = "prevLine" Then c.Width = cPrev.Width - 36
                                     Next
                                 End Sub
        Controls.Add(cPrev)

        ' ----- confirm important rate update -----
        cConfirm = New FiCard
        cConfirm.Controls.Add(Cap("Confirm important rate update", F(11, FontStyle.Bold), Ink, 18, 12))
        Spot(lblConfirmSub, F(7.5!), Muted, 18, 34)
        cConfirm.Controls.Add(lblConfirmSub)
        chkNewOnly.Text = "Apply only to new eligible bookings" : chkNewOnly.Location = New Point(18, 60)
        chkNoFinalized.Text = "Do not change finalized contracts" : chkNoFinalized.Location = New Point(18, 84)
        StyleCheck(chkNewOnly) : StyleCheck(chkNoFinalized)
        cConfirm.Controls.AddRange(New Control() {chkNewOnly, chkNoFinalized})

        pnlPublish = New Panel With {.BackColor = Color.FromArgb(252, 243, 228), .Location = New Point(18, 112), .Size = New Size(264, 62)}
        Spot(lblPublishTitle, F(8, FontStyle.Bold), Color.FromArgb(176, 112, 32), 8, 6)
        lblPublishNote.AutoSize = False : lblPublishNote.Font = F(7.5!) : lblPublishNote.ForeColor = Muted
        lblPublishNote.BackColor = Color.Transparent : lblPublishNote.UseMnemonic = False
        lblPublishNote.SetBounds(8, 24, 248, 36)
        pnlPublish.Controls.AddRange(New Control() {lblPublishTitle, lblPublishNote})
        cConfirm.Controls.Add(pnlPublish)

        StyleBtn(btnKeepDraft, "Keep draft", 90, 30, False)
        StyleBtn(btnReview, "Review & confirm", 130, 30, False)
        btnReview.ForeColor = Maroon : btnReview.FlatAppearance.BorderColor = Maroon
        btnKeepDraft.Location = New Point(18, 184)
        btnReview.Location = New Point(116, 184)
        cConfirm.Controls.AddRange(New Control() {btnKeepDraft, btnReview})
        AddHandler cConfirm.Resize, Sub(s, e) pnlPublish.Width = cConfirm.Width - 36
        Controls.Add(cConfirm)

        ' ----- footer -----
        cFooter = New FiCard
        Spot(lblLastModified, F(8), Muted, 18, 22)
        cFooter.Controls.Add(lblLastModified)
        StyleBtn(btnCancel, "Cancel", 80, 34, False)
        StyleBtn(btnSaveDraft, "💾  Save draft", 130, 34, True)
        cFooter.Controls.AddRange(New Control() {btnCancel, btnSaveDraft})
        AddHandler cFooter.Resize, Sub(s, e)
                                       btnSaveDraft.Location = New Point(cFooter.Width - 18 - btnSaveDraft.Width, 13)
                                       btnCancel.Location = New Point(btnSaveDraft.Left - 10 - btnCancel.Width, 13)
                                   End Sub
        Controls.Add(cFooter)

        AddHandler btnSaveDraft.Click, Sub(s, e) RaiseEvent SaveDraftClicked()
        AddHandler btnCancel.Click, Sub(s, e) RaiseEvent CancelClicked()
        AddHandler btnKeepDraft.Click, Sub(s, e) RaiseEvent KeepDraftClicked()
        AddHandler btnReview.Click, Sub(s, e) RaiseEvent ReviewConfirmClicked()

        Me.ResumeLayout(False)
        ArrangeLayout()
    End Sub
End Class