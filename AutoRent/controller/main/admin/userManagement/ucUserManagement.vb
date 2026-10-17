' Create / Edit User screen. All data spots are empty. Needs ucVehicleDetails.vb (FiCard) in the project.
Public Class ucUserForm
    Inherits UserControl

    Private Shared ReadOnly Maroon As Color = Color.FromArgb(122, 0, 16)
    Private Shared ReadOnly Ink As Color = Color.FromArgb(38, 38, 38)
    Private Shared ReadOnly Muted As Color = Color.FromArgb(120, 120, 120)
    Private Shared ReadOnly BorderCol As Color = Color.FromArgb(228, 225, 220)
    Private Shared ReadOnly PageBg As Color = Color.FromArgb(245, 244, 242)

    Public Event SaveClicked()
    Public Event CancelClicked()
    Public Event ResendInvitationClicked()
    Public Event SecureResetClicked()
    Public Event CreateModeClicked()
    Public Event EditModeClicked()

    <System.ComponentModel.Browsable(False), System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
    Public Property IsEditMode As Boolean = False

    <System.ComponentModel.Browsable(False), System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
    Public Property AccessMethod As String = "Secure invitation"

    ' ---- header ----
    Private ReadOnly lblTitle As New Label
    Public ReadOnly lblSubtitle As New Label
    Public ReadOnly btnCreateUser As New Button
    Public ReadOnly btnEditUser As New Button

    ' ---- banner (hidden until ShowBanner) ----
    Private ReadOnly pnlBanner As New Panel
    Public ReadOnly lblBannerTitle As New Label
    Public ReadOnly lblBannerText As New Label
    Public ReadOnly btnResend As New Button

    ' ---- identity ----
    Public ReadOnly txtFullName As New TextBox
    Public ReadOnly txtUserId As New TextBox
    Public ReadOnly txtUsername As New TextBox
    Public ReadOnly txtEmail As New TextBox
    Public ReadOnly txtPhone As New TextBox
    Public ReadOnly cboBranch As New ComboBox
    Public ReadOnly lblHintUsername As New Label
    Public ReadOnly lblHintEmail As New Label
    Public ReadOnly lblHintPhone As New Label

    ' ---- role & status ----
    Public ReadOnly cboRole As New ComboBox
    Public ReadOnly cboAccountStatus As New ComboBox
    Public ReadOnly lblRolesNote As New Label
    Public ReadOnly lblBoundsTitle As New Label
    Public ReadOnly lblBoundsNote As New Label
    Public ReadOnly txtReason As New TextBox

    ' ---- secure access ----
    Public ReadOnly btnSecure As New Button
    Public ReadOnly btnTemporary As New Button
    Public ReadOnly chkSingleUse As New CheckBox
    Public ReadOnly chkPasswordOnActivation As New CheckBox
    Public ReadOnly chkRequireMfa As New CheckBox
    Public ReadOnly lblAccessNote As New Label
    Public ReadOnly lblIssued As New Label
    Public ReadOnly lblIssuedValue As New Label
    Public ReadOnly lblInviteStatus As New Label
    Public ReadOnly lblInviteStatusValue As New Label
    Public ReadOnly btnSecureReset As New Button

    ' ---- account history ----
    Public ReadOnly lblLastLoginValue As New Label
    Public ReadOnly lblUpdatedByValue As New Label
    Public ReadOnly lblUpdatedAtValue As New Label
    Public ReadOnly lblHistoryNote As New Label

    ' ---- footer ----
    Public ReadOnly lblFooterNote As New Label
    Public ReadOnly btnCancel As New Button
    Public ReadOnly btnSave As New Button

    Private cId, cRole, cAccess, cHist, cFoot As FiCard
    Private pnlBounds As Panel
    Private ReadOnly _caps As New Dictionary(Of Control, Label)

    Public Sub New()
        Me.BackColor = PageBg
        Me.AutoScroll = True
        Me.AutoScrollMinSize = New Size(900, 760)
        BuildUI()
        SetMode(False)
        ClearForm()
    End Sub

    ' ---------------- public ----------------
    Public Sub SetMode(isEdit As Boolean)
        IsEditMode = isEdit
        StyleTab(btnEditUser, isEdit)
        StyleTab(btnCreateUser, Not isEdit)
        txtUserId.Text = If(isEdit, txtUserId.Text, "Assigned on save")
        btnResend.Visible = isEdit
        cHist.Visible = isEdit
        btnSave.Text = If(isEdit, "✓  Save user changes", "✓  Create user")
        DoLayout()
    End Sub

    Public Sub ClearForm()
        For Each t As TextBox In New TextBox() {txtFullName, txtUsername, txtEmail, txtPhone, txtReason}
            t.Text = ""
        Next
        txtUserId.Text = If(IsEditMode, "", "Assigned on save")
        For Each c As ComboBox In New ComboBox() {cboBranch, cboRole, cboAccountStatus}
            c.SelectedIndex = -1 : c.Text = ""
        Next
        chkSingleUse.Checked = False : chkPasswordOnActivation.Checked = False : chkRequireMfa.Checked = False
        SelectMethod("Secure invitation")
        For Each l As Label In New Label() {lblIssuedValue, lblInviteStatusValue, lblLastLoginValue, lblUpdatedByValue, lblUpdatedAtValue,
                                            lblSubtitle, lblAccessNote, lblHistoryNote, lblRolesNote, lblBoundsNote, lblBoundsTitle}
            l.Text = ""
        Next
        pnlBanner.Visible = False
        DoLayout()
    End Sub

    Public Sub ShowBanner(title As String, text As String)
        lblBannerTitle.Text = title
        lblBannerText.Text = text
        pnlBanner.Visible = True
        DoLayout()
    End Sub

    Public Sub SelectMethod(method As String)
        AccessMethod = method
        StyleToggle(btnSecure, method = "Secure invitation")
        StyleToggle(btnTemporary, method = "Temporary setup")
    End Sub

    ' ---------------- build ----------------
    Private Function MkLabel(size As Single, bold As Boolean, col As Color) As Label
        Return New Label With {.AutoSize = False, .UseMnemonic = False, .BackColor = Color.Transparent, .ForeColor = col,
                               .Font = New Font("Segoe UI", size, If(bold, FontStyle.Bold, FontStyle.Regular))}
    End Function

    Private Sub StyleBox(c As Control)
        c.Font = New Font("Segoe UI", 9)
        c.ForeColor = Ink
        If TypeOf c Is TextBox Then DirectCast(c, TextBox).BorderStyle = BorderStyle.FixedSingle
        If TypeOf c Is ComboBox Then
            Dim cb As ComboBox = DirectCast(c, ComboBox)
            cb.DropDownStyle = ComboBoxStyle.DropDownList
            cb.FlatStyle = FlatStyle.Flat
            cb.BackColor = Color.White
        End If
    End Sub

    Private Sub StyleBtn(b As Button, primary As Boolean)
        b.FlatStyle = FlatStyle.Flat
        b.Cursor = Cursors.Hand
        b.UseVisualStyleBackColor = False
        b.Font = New Font("Segoe UI", 9, FontStyle.Bold)
        If primary Then
            b.BackColor = Maroon : b.ForeColor = Color.White : b.FlatAppearance.BorderColor = Maroon
        Else
            b.BackColor = Color.White : b.ForeColor = Ink : b.FlatAppearance.BorderColor = BorderCol
        End If
    End Sub

    Private Sub StyleTab(b As Button, active As Boolean)
        StyleBtn(b, active)
        b.Font = New Font("Segoe UI", 8.5, If(active, FontStyle.Bold, FontStyle.Regular))
        If Not active Then b.FlatAppearance.BorderSize = 0
    End Sub

    Private Sub StyleToggle(b As Button, active As Boolean)
        StyleBtn(b, active)
        b.Font = New Font("Segoe UI", 8.5, If(active, FontStyle.Bold, FontStyle.Regular))
        If Not active Then b.BackColor = Color.FromArgb(240, 238, 234) : b.FlatAppearance.BorderColor = Color.FromArgb(240, 238, 234)
    End Sub

    Private Sub AddField(card As Control, caption As String, ctl As Control)
        Dim lb As Label = MkLabel(8, True, Ink)
        lb.Text = caption
        _caps(ctl) = lb
        card.Controls.Add(lb)
        StyleBox(ctl)
        card.Controls.Add(ctl)
    End Sub

    Private Sub Hint(card As Control, lb As Label)
        lb.AutoSize = False : lb.BackColor = Color.Transparent : lb.ForeColor = Muted
        lb.Font = New Font("Segoe UI", 7.5) : lb.UseMnemonic = False
        card.Controls.Add(lb)
    End Sub

    Private Sub BuildUI()
        ' header
        lblTitle.Text = "Create / Edit User"
        lblTitle.AutoSize = False : lblTitle.BackColor = Color.Transparent : lblTitle.ForeColor = Ink
        lblTitle.Font = New Font("Segoe UI", 17, FontStyle.Bold)
        lblSubtitle.AutoSize = False : lblSubtitle.BackColor = Color.Transparent : lblSubtitle.ForeColor = Muted
        lblSubtitle.Font = New Font("Segoe UI", 8.5) : lblSubtitle.UseMnemonic = False
        btnCreateUser.Text = "Create user" : btnEditUser.Text = "Edit user"
        AddHandler btnCreateUser.Click, Sub() RaiseEvent CreateModeClicked()
        AddHandler btnEditUser.Click, Sub() RaiseEvent EditModeClicked()
        Me.Controls.AddRange(New Control() {lblTitle, lblSubtitle, btnCreateUser, btnEditUser})

        ' banner
        pnlBanner.BackColor = Color.FromArgb(232, 244, 236)
        lblBannerTitle.AutoSize = False : lblBannerTitle.BackColor = Color.Transparent : lblBannerTitle.ForeColor = Color.FromArgb(30, 100, 60)
        lblBannerTitle.Font = New Font("Segoe UI", 8.5, FontStyle.Bold) : lblBannerTitle.UseMnemonic = False
        lblBannerText.AutoSize = False : lblBannerText.BackColor = Color.Transparent : lblBannerText.ForeColor = Color.FromArgb(70, 110, 85)
        lblBannerText.Font = New Font("Segoe UI", 7.5) : lblBannerText.UseMnemonic = False
        btnResend.Text = "Resend invitation" : StyleBtn(btnResend, False)
        AddHandler btnResend.Click, Sub() RaiseEvent ResendInvitationClicked()
        pnlBanner.Controls.AddRange(New Control() {lblBannerTitle, lblBannerText, btnResend})
        Me.Controls.Add(pnlBanner)

        ' identity card
        cId = New FiCard
        Dim t1 As Label = MkLabel(10.5, True, Ink) : t1.Text = "Identity & contact" : t1.SetBounds(16, 14, 300, 20)
        Dim t1s As Label = MkLabel(7.5, False, Muted) : t1s.Text = "* Required" : t1s.SetBounds(16, 34, 300, 16)
        cId.Controls.AddRange(New Control() {t1, t1s})
        AddField(cId, "Full Name *", txtFullName)
        AddField(cId, "User ID", txtUserId) : txtUserId.ReadOnly = True : txtUserId.BackColor = Color.FromArgb(240, 238, 234)
        AddField(cId, "Username *", txtUsername)
        AddField(cId, "Work Email (optional)", txtEmail)
        AddField(cId, "Contact phone", txtPhone)
        AddField(cId, "Branch *", cboBranch)
        Hint(cId, lblHintUsername) : Hint(cId, lblHintEmail) : Hint(cId, lblHintPhone)
        Me.Controls.Add(cId)

        ' role card
        cRole = New FiCard
        Dim t2 As Label = MkLabel(10.5, True, Ink) : t2.Text = "Role & account status" : t2.SetBounds(16, 14, 300, 20)
        cRole.Controls.Add(t2)
        AddField(cRole, "Assigned Role *", cboRole)
        AddField(cRole, "Account Status *", cboAccountStatus)
        Hint(cRole, lblRolesNote)
        pnlBounds = New Panel With {.BackColor = Color.FromArgb(240, 238, 234)}
        lblBoundsTitle.AutoSize = False : lblBoundsTitle.BackColor = Color.Transparent : lblBoundsTitle.ForeColor = Ink
        lblBoundsTitle.Font = New Font("Segoe UI", 8, FontStyle.Bold) : lblBoundsTitle.UseMnemonic = False
        lblBoundsNote.AutoSize = False : lblBoundsNote.BackColor = Color.Transparent : lblBoundsNote.ForeColor = Muted
        lblBoundsNote.Font = New Font("Segoe UI", 7.5) : lblBoundsNote.UseMnemonic = False
        pnlBounds.Controls.AddRange(New Control() {lblBoundsTitle, lblBoundsNote})
        cRole.Controls.Add(pnlBounds)
        AddField(cRole, "Change reason *", txtReason)
        Me.Controls.Add(cRole)

        ' secure access card
        cAccess = New FiCard
        Dim t3 As Label = MkLabel(10.5, True, Ink) : t3.Text = "Secure access setup" : t3.SetBounds(16, 14, 300, 20)
        cAccess.Controls.Add(t3)
        btnSecure.Text = "Secure invitation" : btnTemporary.Text = "Temporary setup"
        AddHandler btnSecure.Click, Sub() SelectMethod("Secure invitation")
        AddHandler btnTemporary.Click, Sub() SelectMethod("Temporary setup")
        chkSingleUse.Text = "Send single-use, time-limited email link"
        chkPasswordOnActivation.Text = "Require password creation at activation"
        chkRequireMfa.Text = "Require MFA for privileged roles"
        For Each k As CheckBox In New CheckBox() {chkSingleUse, chkPasswordOnActivation, chkRequireMfa}
            k.AutoSize = False : k.BackColor = Color.Transparent : k.ForeColor = Ink : k.Font = New Font("Segoe UI", 8)
            cAccess.Controls.Add(k)
        Next
        Hint(cAccess, lblAccessNote)
        lblIssued.Text = "Invitation issued" : lblInviteStatus.Text = "Invitation status"
        For Each l As Label In New Label() {lblIssued, lblInviteStatus}
            l.AutoSize = False : l.BackColor = Color.Transparent : l.ForeColor = Muted : l.Font = New Font("Segoe UI", 8)
            cAccess.Controls.Add(l)
        Next
        For Each l As Label In New Label() {lblIssuedValue, lblInviteStatusValue}
            l.AutoSize = False : l.BackColor = Color.Transparent : l.ForeColor = Ink : l.Font = New Font("Segoe UI", 8, FontStyle.Bold)
            l.TextAlign = ContentAlignment.TopRight : l.UseMnemonic = False
            cAccess.Controls.Add(l)
        Next
        btnSecureReset.Text = "🔑  Initiate secure reset" : StyleBtn(btnSecureReset, False)
        AddHandler btnSecureReset.Click, Sub() RaiseEvent SecureResetClicked()
        cAccess.Controls.AddRange(New Control() {btnSecure, btnTemporary, btnSecureReset})
        Me.Controls.Add(cAccess)

        ' history card
        cHist = New FiCard
        Dim t4 As Label = MkLabel(10.5, True, Ink) : t4.Text = "Account history" : t4.SetBounds(16, 14, 300, 20)
        cHist.Controls.Add(t4)
        For Each pair In New Tuple(Of String, Label)() {Tuple.Create("Last prior login", lblLastLoginValue),
                                                       Tuple.Create("Updated by", lblUpdatedByValue),
                                                       Tuple.Create("Updated at", lblUpdatedAtValue)}
            Dim cp As Label = MkLabel(8, False, Muted) : cp.Text = pair.Item1
            cp.Tag = "histcap"
            cHist.Controls.Add(cp)
            pair.Item2.AutoSize = False : pair.Item2.BackColor = Color.Transparent : pair.Item2.ForeColor = Ink
            pair.Item2.Font = New Font("Segoe UI", 8, FontStyle.Bold) : pair.Item2.TextAlign = ContentAlignment.TopRight
            pair.Item2.UseMnemonic = False
            cHist.Controls.Add(pair.Item2)
        Next
        Hint(cHist, lblHistoryNote)
        Me.Controls.Add(cHist)

        ' footer
        cFoot = New FiCard
        lblFooterNote.AutoSize = False : lblFooterNote.BackColor = Color.Transparent : lblFooterNote.ForeColor = Muted
        lblFooterNote.Font = New Font("Segoe UI", 8) : lblFooterNote.UseMnemonic = False
        btnCancel.Text = "Cancel" : StyleBtn(btnCancel, False)
        btnSave.Text = "✓  Create user" : StyleBtn(btnSave, True)
        AddHandler btnCancel.Click, Sub() RaiseEvent CancelClicked()
        AddHandler btnSave.Click, Sub() RaiseEvent SaveClicked()
        cFoot.Controls.AddRange(New Control() {lblFooterNote, btnCancel, btnSave})
        Me.Controls.Add(cFoot)

        AddHandler Me.Resize, Sub(s, e) DoLayout()
    End Sub

    ' ---------------- layout ----------------
    Private Sub Place(ctl As Control, x As Integer, y As Integer, w As Integer)
        Dim lb As Label = _caps(ctl)
        lb.SetBounds(x, y, w, 16)
        ctl.SetBounds(x, y + 18, w, 28)
    End Sub

    Private Sub DoLayout()
        If cId Is Nothing Then Return
        Dim m As Integer = 24
        Dim w As Integer = Math.Max(Me.ClientSize.Width, 900) - m * 2
        Dim gap As Integer = 16
        Dim leftW As Integer = CInt((w - gap) * 0.64)
        Dim rightW As Integer = w - gap - leftW

        lblTitle.SetBounds(m, 14, 500, 34)
        lblSubtitle.SetBounds(m, 50, w - 260, 18)
        btnEditUser.SetBounds(m + w - 84, 20, 84, 28)
        btnCreateUser.SetBounds(m + w - 84 - 90, 20, 84, 28)

        Dim y As Integer = 80
        If pnlBanner.Visible Then
            pnlBanner.SetBounds(m, y, w, 48)
            lblBannerTitle.SetBounds(12, 6, w - 220, 18)
            lblBannerText.SetBounds(12, 26, w - 220, 16)
            btnResend.SetBounds(w - 150, 10, 138, 28)
            y += 58
        End If

        ' left column
        Dim iw As Integer = leftW - 32
        Dim cw As Integer = (iw - 16) \ 2
        cId.SetBounds(m, y, leftW, 282)
        Place(txtFullName, 16, 56, cw) : Place(txtUserId, 16 + cw + 16, 56, cw)
        Place(txtUsername, 16, 130, cw) : Place(txtEmail, 16 + cw + 16, 130, cw)
        lblHintUsername.SetBounds(16, 180, cw, 14) : lblHintEmail.SetBounds(16 + cw + 16, 180, cw, 14)
        Place(txtPhone, 16, 204, cw) : Place(cboBranch, 16 + cw + 16, 204, cw)
        lblHintPhone.SetBounds(16, 254, cw, 14)

        cRole.SetBounds(m, y + 282 + gap, leftW, 262)
        Place(cboRole, 16, 46, cw) : Place(cboAccountStatus, 16 + cw + 16, 46, cw)
        lblRolesNote.SetBounds(16, 96, iw, 14)
        pnlBounds.SetBounds(16, 118, iw, 62)
        lblBoundsTitle.SetBounds(12, 8, iw - 24, 16)
        lblBoundsNote.SetBounds(12, 26, iw - 24, 30)
        Place(txtReason, 16, 192, iw)

        ' right column
        Dim rx As Integer = m + leftW + gap
        Dim riw As Integer = rightW - 32
        cAccess.SetBounds(rx, y, rightW, 300)
        btnSecure.SetBounds(16, 44, 110, 26) : btnTemporary.SetBounds(130, 44, 110, 26)
        chkSingleUse.SetBounds(16, 82, riw, 20)
        chkPasswordOnActivation.SetBounds(16, 104, riw, 20)
        chkRequireMfa.SetBounds(16, 126, riw, 20)
        lblAccessNote.SetBounds(16, 152, riw, 46)
        lblIssued.SetBounds(16, 206, riw \ 2, 16) : lblIssuedValue.SetBounds(16 + riw \ 2, 206, riw - riw \ 2, 16)
        lblInviteStatus.SetBounds(16, 232, riw \ 2, 16) : lblInviteStatusValue.SetBounds(16 + riw \ 2, 232, riw - riw \ 2, 16)
        btnSecureReset.SetBounds(16, 260, 170, 30)

        cHist.SetBounds(rx, y + 300 + gap, rightW, 168)
        Dim ry As Integer = 44
        Dim vals() As Label = {lblLastLoginValue, lblUpdatedByValue, lblUpdatedAtValue}
        Dim idx As Integer = 0
        For Each c As Control In cHist.Controls
            If TypeOf c Is Label AndAlso Convert.ToString(c.Tag) = "histcap" Then
                c.SetBounds(16, ry, riw \ 2, 16)
                vals(idx).SetBounds(16 + riw \ 2, ry, riw - riw \ 2, 16)
                ry += 26 : idx += 1
            End If
        Next
        lblHistoryNote.SetBounds(16, ry + 4, riw, 40)

        ' footer
        Dim footY As Integer = y + 282 + gap + 262 + gap
        cFoot.SetBounds(m, footY, w, 60)
        lblFooterNote.SetBounds(16, 20, w - 340, 18)
        btnSave.SetBounds(w - 16 - 170, 14, 170, 32)
        btnCancel.SetBounds(w - 16 - 170 - 10 - 90, 14, 90, 32)

        Me.AutoScrollMinSize = New Size(900, footY + 60 + 24)
    End Sub
End Class