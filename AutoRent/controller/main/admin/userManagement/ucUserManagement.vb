Public Class ucUserManagement
    Inherits UserControl

    ' ---- Custom Events ----
    Public Event SaveChangesRequested As EventHandler
    Public Event CancelRequested As EventHandler
    Public Event ResendInvitationRequested As EventHandler
    Public Event InitiateResetRequested As EventHandler

    ' ---- Theme Colors ----
    Private ReadOnly CardBgColor As Color = Color.White
    Private ReadOnly BorderColor As Color = Color.FromArgb(225, 225, 225)
    Private ReadOnly PrimaryRed As Color = Color.FromArgb(122, 0, 16)
    Private ReadOnly TextDark As Color = Color.FromArgb(30, 30, 30)
    Private ReadOnly TextMuted As Color = Color.FromArgb(110, 110, 110)

    ' ---- Action Buttons ----
    Public ReadOnly btnSaveUser As New Button With {.Text = "Save user changes", .Width = 140, .Height = 32, .FlatStyle = FlatStyle.Flat, .BackColor = Color.FromArgb(122, 0, 16), .ForeColor = Color.White, .Cursor = Cursors.Hand}
    Public ReadOnly btnCancel As New Button With {.Text = "Cancel", .Width = 90, .Height = 32, .FlatStyle = FlatStyle.Standard, .Cursor = Cursors.Hand}
    Public ReadOnly btnResendInvitation As New Button With {.Text = "Resend invitation", .Width = 120, .Height = 28, .FlatStyle = FlatStyle.Standard, .Cursor = Cursors.Hand}
    Public ReadOnly btnInitiateReset As New Button With {.Text = "Initiate secure reset", .Width = 130, .Height = 28, .FlatStyle = FlatStyle.Standard, .Cursor = Cursors.Hand}

    ' ---- Form Inputs (Identity & Contact) ----
    Public ReadOnly txtFullName As New TextBox With {.Font = New Font("Segoe UI", 9.5F), .BorderStyle = BorderStyle.FixedSingle}
    Public ReadOnly txtUserID As New TextBox With {.Font = New Font("Segoe UI", 9.5F), .BorderStyle = BorderStyle.FixedSingle}
    Public ReadOnly txtUsername As New TextBox With {.Font = New Font("Segoe UI", 9.5F), .BorderStyle = BorderStyle.FixedSingle}
    Public ReadOnly txtWorkEmail As New TextBox With {.Font = New Font("Segoe UI", 9.5F), .BorderStyle = BorderStyle.FixedSingle}
    Public ReadOnly txtContactPhone As New TextBox With {.Font = New Font("Segoe UI", 9.5F), .BorderStyle = BorderStyle.FixedSingle}
    Public ReadOnly cmbBranch As New ComboBox With {.Font = New Font("Segoe UI", 9.5F), .DropDownStyle = ComboBoxStyle.DropDownList}

    ' ---- Form Inputs (Role & Status) ----
    Public ReadOnly cmbAssignedRole As New ComboBox With {.Font = New Font("Segoe UI", 9.5F), .DropDownStyle = ComboBoxStyle.DropDownList}
    Public ReadOnly cmbAccountStatus As New ComboBox With {.Font = New Font("Segoe UI", 9.5F), .DropDownStyle = ComboBoxStyle.DropDownList}
    Public ReadOnly txtChangeReason As New TextBox With {.Font = New Font("Segoe UI", 9.5F), .BorderStyle = BorderStyle.FixedSingle, .Multiline = True, .Height = 44}

    ' ---- Checkboxes & Banners ----
    Public ReadOnly chkSingleUseLink As New CheckBox With {.Text = "Send single-use, time-limited email link", .Font = New Font("Segoe UI", 8.5F), .AutoSize = True, .Checked = True}
    Public ReadOnly chkRequirePasswordCreation As New CheckBox With {.Text = "Require password creation at activation", .Font = New Font("Segoe UI", 8.5F), .AutoSize = True, .Checked = True}
    Public ReadOnly chkRequireMFA As New CheckBox With {.Text = "Require MFA for privileged roles", .Font = New Font("Segoe UI", 8.5F), .AutoSize = True, .Checked = True}

    Public ReadOnly pnlBanner As New Panel With {.BackColor = Color.FromArgb(240, 248, 240), .Padding = New Padding(10), .Height = 44}
    Public ReadOnly lblBannerText As New Label With {.Font = New Font("Segoe UI", 8.5F, FontStyle.Bold), .ForeColor = Color.FromArgb(40, 120, 60), .Dock = DockStyle.Fill}

    ' ---- Metadata Labels (Right Column) ----
    Public ReadOnly lblInvitationIssued As New Label With {.Font = New Font("Segoe UI", 8.5F, FontStyle.Bold), .ForeColor = TextDark, .TextAlign = ContentAlignment.MiddleRight}
    Public ReadOnly lblInvitationStatus As New Label With {.Font = New Font("Segoe UI", 8.5F, FontStyle.Bold), .ForeColor = TextDark, .TextAlign = ContentAlignment.MiddleRight}
    Public ReadOnly lblLastPriorLogin As New Label With {.Font = New Font("Segoe UI", 8.5F, FontStyle.Bold), .ForeColor = TextDark, .TextAlign = ContentAlignment.MiddleRight}
    Public ReadOnly lblUpdatedBy As New Label With {.Font = New Font("Segoe UI", 8.5F, FontStyle.Bold), .ForeColor = TextDark, .TextAlign = ContentAlignment.MiddleRight}
    Public ReadOnly lblUpdatedAt As New Label With {.Font = New Font("Segoe UI", 8.5F, FontStyle.Bold), .ForeColor = TextDark, .TextAlign = ContentAlignment.MiddleRight}

    Public Sub New()
        Try
            Me.BackColor = Color.FromArgb(248, 249, 250)
            Me.Dock = DockStyle.Fill

            Dim root As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 1, .RowCount = 3, .Padding = New Padding(16)}
            root.RowStyles.Add(New RowStyle(SizeType.Absolute, 75))  ' Header & Banner
            root.RowStyles.Add(New RowStyle(SizeType.Percent, 100))  ' Main 2-Column Forms
            root.RowStyles.Add(New RowStyle(SizeType.Absolute, 50))  ' Footer Bar

            ' Wire up event handlers
            AddHandler btnSaveUser.Click, Sub(s, e) RaiseEvent SaveChangesRequested(Me, EventArgs.Empty)
            AddHandler btnCancel.Click, Sub(s, e) RaiseEvent CancelRequested(Me, EventArgs.Empty)
            AddHandler btnResendInvitation.Click, Sub(s, e) RaiseEvent ResendInvitationRequested(Me, EventArgs.Empty)
            AddHandler btnInitiateReset.Click, Sub(s, e) RaiseEvent InitiateResetRequested(Me, EventArgs.Empty)

            ' ==========================================
            ' ROW 0: Title, User ID & Invitation Banner
            ' ==========================================
            Dim headerPanel As New Panel With {.Dock = DockStyle.Fill}
            Dim lblTitle As New Label With {.Text = "Create / Edit User", .Font = New Font("Segoe UI", 16, FontStyle.Bold), .ForeColor = TextDark, .AutoSize = True, .Location = New Point(0, 0)}
            Dim lblSubTitle As New Label With {.Text = "USR-0008 · Maya Chen · invitation-based access setup · no stored password display", .Font = New Font("Segoe UI", 8.5F), .ForeColor = TextMuted, .AutoSize = True, .Location = New Point(0, 28)}

            Dim actionBox As New FlowLayoutPanel With {.Dock = DockStyle.Right, .AutoSize = True, .FlowDirection = FlowDirection.LeftToRight}
            btnResendInvitation.Margin = New Padding(0, 2, 0, 0)
            actionBox.Controls.Add(btnResendInvitation)

            lblBannerText.Text = "Invitation sent to maya.chen@example.com" & vbCrLf & "Demonstration success state - single-use invitation expires 18 Oct 2026, 11:26. Account activation requires secure credential setup."
            pnlBanner.Controls.Add(lblBannerText)
            AddHandler pnlBanner.Paint, AddressOf DrawBannerBorder

            ' Assemble Header
            headerPanel.Controls.Add(actionBox)
            headerPanel.Controls.Add(lblTitle)
            headerPanel.Controls.Add(lblSubTitle)

            Dim topLayout As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 1, .RowCount = 2}
            topLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 50))
            topLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 46))
            topLayout.Controls.Add(headerPanel, 0, 0)
            topLayout.Controls.Add(pnlBanner, 0, 1)
            root.Controls.Add(topLayout, 0, 0)

            ' ==========================================
            ' ROW 1: Split Form Layout (Left Forms / Right Setup & History)
            ' ==========================================
dataGridSplit:
            Dim editorSplit As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 2, .RowCount = 1, .Padding = New Padding(0, 4, 0, 4)}
            editorSplit.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 65))
            editorSplit.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 35))

            ' --- LEFT COLUMN: Identity & Role Cards ---
            Dim leftColPanel As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 1, .RowCount = 2}
            leftColPanel.RowStyles.Add(New RowStyle(SizeType.Percent, 55))
            leftColPanel.RowStyles.Add(New RowStyle(SizeType.Percent, 45))

            ' Card 1: Identity & Contact
            Dim cardIdentity As New Panel With {.Dock = DockStyle.Fill, .BackColor = CardBgColor, .Padding = New Padding(12), .Margin = New Padding(0, 0, 8, 4)}
            AddHandler cardIdentity.Paint, AddressOf DrawCardBorder
            Dim lblIdTitle As New Label With {.Text = "Identity & contact", .Dock = DockStyle.Top, .Height = 22, .Font = New Font("Segoe UI", 11, FontStyle.Bold), .ForeColor = TextDark}
            Dim lblIdSub As New Label With {.Text = "* Required - unique username validated on the server", .Dock = DockStyle.Top, .Height = 18, .Font = New Font("Segoe UI", 8.0F), .ForeColor = TextMuted}

            Dim formIdentityGrid As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 2, .RowCount = 3, .Padding = New Padding(0, 4, 0, 0)}
            formIdentityGrid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50))
            formIdentityGrid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50))

            AddFormRow(formIdentityGrid, "Full Name *", txtFullName, "User ID", txtUserID, 0)
            AddFormRow(formIdentityGrid, "Username *", txtUsername, "Work Email (optional)", txtWorkEmail, 1)
            AddFormRow(formIdentityGrid, "Contact phone", txtContactPhone, "Branch *", cmbBranch, 2)

            cardIdentity.Controls.Add(formIdentityGrid)
            cardIdentity.Controls.Add(lblIdSub)
            cardIdentity.Controls.Add(lblIdTitle)
            leftColPanel.Controls.Add(cardIdentity, 0, 0)

            ' Card 2: Role & Account Status
            Dim cardRoleStatus As New Panel With {.Dock = DockStyle.Fill, .BackColor = CardBgColor, .Padding = New Padding(12), .Margin = New Padding(0, 4, 0, 0)}
            AddHandler cardRoleStatus.Paint, AddressOf DrawCardBorder
            Dim lblRoleTitle As New Label With {.Text = "Role & account status", .Dock = DockStyle.Top, .Height = 22, .Font = New Font("Segoe UI", 11, FontStyle.Bold), .ForeColor = TextDark}

            Dim formRoleGrid As New TableLayoutPanel With {.Dock = DockStyle.Top, .Height = 55, .ColumnCount = 2, .RowCount = 1}
            formRoleGrid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50))
            formRoleGrid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50))

            Dim p1 As New FlowLayoutPanel With {.Dock = DockStyle.Fill, .FlowDirection = FlowDirection.TopDown, .WrapContents = False}
            p1.Controls.Add(New Label With {.Text = "Assigned Role *", .Font = New Font("Segoe UI", 8.5F), .ForeColor = TextMuted, .AutoSize = True})
            cmbAssignedRole.Width = 220 : p1.Controls.Add(cmbAssignedRole)

            Dim p2 As New FlowLayoutPanel With {.Dock = DockStyle.Fill, .FlowDirection = FlowDirection.TopDown, .WrapContents = False}
            p2.Controls.Add(New Label With {.Text = "Account Status *", .Font = New Font("Segoe UI", 8.5F), .ForeColor = TextMuted, .AutoSize = True})
            cmbAccountStatus.Width = 220 : p2.Controls.Add(cmbAccountStatus)

            formRoleGrid.Controls.Add(p1, 0, 0)
            formRoleGrid.Controls.Add(p2, 1, 0)

            Dim lblRoleNote As New Label With {.Text = "Available roles: System Administrator · Business Owner / Branch Manager · Front-Desk Staff · Rental Clerk · Dispatch Officer" & vbCrLf &
                                               "Role bounds are checked server-side. Only authorized administrators may assign roles.", .Font = New Font("Segoe UI", 8.0F), .ForeColor = TextMuted, .Dock = DockStyle.Top, .Height = 35}

            Dim pnlReasonBox As New FlowLayoutPanel With {.Dock = DockStyle.Bottom, .AutoSize = True, .FlowDirection = FlowDirection.TopDown, .WrapContents = False}
            pnlReasonBox.Controls.Add(New Label With {.Text = "Change reason *", .Font = New Font("Segoe UI", 8.5F), .ForeColor = TextMuted, .AutoSize = True})
            txtChangeReason.Dock = DockStyle.Fill
            pnlReasonBox.Controls.Add(txtChangeReason)

            cardRoleStatus.Controls.Add(pnlReasonBox)
            cardRoleStatus.Controls.Add(lblRoleNote)
            cardRoleStatus.Controls.Add(formRoleGrid)
            cardRoleStatus.Controls.Add(lblRoleTitle)
            leftColPanel.Controls.Add(cardRoleStatus, 0, 1)

            editorSplit.Controls.Add(leftColPanel, 0, 0)

            ' --- RIGHT COLUMN: Secure Access Setup & Account History ---
            RefactorRightColumn(editorSplit)

            root.Controls.Add(editorSplit, 0, 1)

            ' ==========================================
            ' ROW 2: Footer Bar
            ' ==========================================
            Dim footerPanel As New Panel With {.Dock = DockStyle.Fill}
            Dim lblFooterMeta As New Label With {.Text = "Protected account update - role changes and invitations are audited", .Font = New Font("Segoe UI", 8.5F), .ForeColor = TextMuted, .Dock = DockStyle.Left, .TextAlign = ContentAlignment.MiddleLeft, .AutoSize = True}

            Dim actionFooter As New FlowLayoutPanel With {.Dock = DockStyle.Right, .AutoSize = True, .FlowDirection = FlowDirection.LeftToRight}
            btnCancel.Margin = New Padding(0, 0, 8, 0)
            actionFooter.Controls.Add(btnCancel)
            actionFooter.Controls.Add(btnSaveUser)

            footerPanel.Controls.Add(lblFooterMeta)
            footerPanel.Controls.Add(actionFooter)
            root.Controls.Add(footerPanel, 0, 2)

            Me.Controls.Add(root)
        Catch ex As Exception
            MessageBox.Show("Error initializing User Management UI: " & ex.Message, "UI Exception", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub AddFormRow(tbl As TableLayoutPanel, label1 As String, c1 As Control, label2 As String, c2 As Control, row As Integer)
        Dim p1 As New FlowLayoutPanel With {.Dock = DockStyle.Fill, .FlowDirection = FlowDirection.TopDown, .WrapContents = False}
        Dim l1 As New Label With {.Text = label1, .Font = New Font("Segoe UI", 8.5F), .ForeColor = TextMuted, .AutoSize = True}
        c1.Width = 220
        p1.Controls.Add(l1)
        p1.Controls.Add(c1)

        Dim p2 As New FlowLayoutPanel With {.Dock = DockStyle.Fill, .FlowDirection = FlowDirection.TopDown, .WrapContents = False}
        Dim l2 As New Label With {.Text = label2, .Font = New Font("Segoe UI", 8.5F), .ForeColor = TextMuted, .AutoSize = True}
        c2.Width = 220
        p2.Controls.Add(l2)
        p2.Controls.Add(c2)

        tbl.Controls.Add(p1, 0, row)
        tbl.Controls.Add(p2, 1, row)
    End Sub

    Private Sub RefactorRightColumn(parentGrid As TableLayoutPanel)
VB_Label_Fix:
        Dim rightCol As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 1, .RowCount = 2}
        rightCol.RowStyles.Add(New RowStyle(SizeType.Percent, 60))
        rightCol.RowStyles.Add(New RowStyle(SizeType.Percent, 40))

        ' 1. Secure Access Setup Card
        Dim cardSecurity As New Panel With {.Dock = DockStyle.Fill, .BackColor = CardBgColor, .Padding = New Padding(12), .Margin = New Padding(8, 0, 0, 4)}
        AddHandler cardSecurity.Paint, AddressOf DrawCardBorder
        Dim lblSecTitle As New Label With {.Text = "Secure access setup", .Dock = DockStyle.Top, .Height = 22, .Font = New Font("Segoe UI", 11, FontStyle.Bold), .ForeColor = TextDark}
        Dim lblSecSub As New Label With {.Text = "Preferred method: secure invitation", .Dock = DockStyle.Top, .Height = 18, .Font = New Font("Segoe UI", 8.0F), .ForeColor = TextMuted}

        Dim secBox As New FlowLayoutPanel With {.Dock = DockStyle.Fill, .FlowDirection = FlowDirection.TopDown, .WrapContents = False, .Padding = New Padding(0, 6, 0, 0)}
        secBox.Controls.Add(chkSingleUseLink)
        secBox.Controls.Add(chkRequirePasswordCreation)
        secBox.Controls.Add(chkRequireMFA)
        secBox.Controls.Add(New Label With {.Text = "No password is shown or emailed. A temporary setup, if authorized, must require a first-login reset.", .Font = New Font("Segoe UI", 7.5F), .ForeColor = TextMuted, .Width = 320, .Height = 30})
        secBox.Controls.Add(btnInitiateReset)

        cardSecurity.Controls.Add(secBox)
        cardSecurity.Controls.Add(lblSecSub)
        cardSecurity.Controls.Add(lblSecTitle)

        ' 2. Account History Card
        Dim cardHistory As New Panel With {.Dock = DockStyle.Fill, .BackColor = CardBgColor, .Padding = New Padding(12), .Margin = New Padding(8, 4, 0, 0)}
        AddHandler cardHistory.Paint, AddressOf DrawCardBorder
        Dim lblHistTitle As New Label With {.Text = "Account history", .Dock = DockStyle.Top, .Height = 22, .Font = New Font("Segoe UI", 11, FontStyle.Bold), .ForeColor = TextDark}

        Dim histTable As New TableLayoutPanel With {.Dock = DockStyle.Top, .Height = 65, .ColumnCount = 2, .RowCount = 3}
        histTable.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 40))
        histTable.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 60))

        histTable.Controls.Add(New Label With {.Text = "Last prior login", .Font = New Font("Segoe UI", 8.5F), .ForeColor = TextMuted}, 0, 0)
        lblLastPriorLogin.Text = "14 Oct - 07:45" : histTable.Controls.Add(lblLastPriorLogin, 1, 0)

        histTable.Controls.Add(New Label With {.Text = "Updated by", .Font = New Font("Segoe UI", 8.5F), .ForeColor = TextMuted}, 0, 1)
        lblUpdatedBy.Text = "Alex Morgan" : histTable.Controls.Add(lblUpdatedBy, 1, 1)

        histTable.Controls.Add(New Label With {.Text = "Updated at", .Font = New Font("Segoe UI", 8.5F), .ForeColor = TextMuted}, 0, 2)
        lblUpdatedAt.Text = "14 Oct - 11:26" : histTable.Controls.Add(lblUpdatedAt, 1, 2)

        cardHistory.Controls.Add(histTable)
        cardHistory.Controls.Add(lblHistTitle)

        rightCol.Controls.Add(cardSecurity, 0, 0)
        rightCol.Controls.Add(cardHistory, 0, 1)
        parentGrid.Controls.Add(rightCol, 1, 0)
    End Sub

    Private Sub DrawCardBorder(sender As Object, e As PaintEventArgs)
        Dim p = DirectCast(sender, Panel)
        ControlPaint.DrawBorder(e.Graphics, p.ClientRectangle, BorderColor, ButtonBorderStyle.Solid)
    End Sub

    Private Sub DrawBannerBorder(sender As Object, e As PaintEventArgs)
        Dim p = DirectCast(sender, Panel)
        ControlPaint.DrawBorder(e.Graphics, p.ClientRectangle, Color.FromArgb(200, 230, 200), ButtonBorderStyle.Solid)
    End Sub

End Class