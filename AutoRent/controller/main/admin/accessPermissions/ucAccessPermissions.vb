Public Class ucAccessPermissions
    Inherits UserControl

    ' ---- Custom Events ----
    Public Event ReviewChangesRequested As EventHandler
    Public Event ConfirmSensitiveChangeRequested As EventHandler
    Public Event CancelChangeRequested As EventHandler

    ' ---- Theme Colors ----
    Private ReadOnly CardBgColor As Color = Color.White
    Private ReadOnly BorderColor As Color = Color.FromArgb(225, 225, 225)
    Private ReadOnly PrimaryRed As Color = Color.FromArgb(122, 0, 16)
    Private ReadOnly TextDark As Color = Color.FromArgb(30, 30, 30)
    Private ReadOnly TextMuted As Color = Color.FromArgb(110, 110, 110)

    ' ---- Buttons ----
    Public ReadOnly btnReviewChanges As New Button With {.Text = "Review permission changes", .Width = 160, .Height = 32, .FlatStyle = FlatStyle.Flat, .BackColor = Color.FromArgb(122, 0, 16), .ForeColor = Color.White, .Cursor = Cursors.Hand}
    Public ReadOnly btnCancelChange As New Button With {.Text = "Cancel change", .Width = 110, .Height = 30, .FlatStyle = FlatStyle.Standard, .Cursor = Cursors.Hand}
    Public ReadOnly btnConfirmChange As New Button With {.Text = "Confirm sensitive change", .Width = 160, .Height = 30, .FlatStyle = FlatStyle.Standard, .Cursor = Cursors.Hand}

    ' ---- Controls ----
    Public ReadOnly dgvMatrix As New DataGridView
    Public ReadOnly txtAuditReason As New TextBox With {.Font = New Font("Segoe UI", 9.5F), .BorderStyle = BorderStyle.FixedSingle, .Multiline = True, .Height = 40}
    Public ReadOnly chkReviewBounds As New CheckBox With {.Text = "I have reviewed branch scope and permission bounds", .Font = New Font("Segoe UI", 9), .AutoSize = True, .Checked = True}

    Public Sub New()
        Try
            Me.BackColor = Color.FromArgb(248, 249, 250)
            Me.Dock = DockStyle.Fill

            Dim root As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 1, .RowCount = 3, .Padding = New Padding(16)}
            root.RowStyles.Add(New RowStyle(SizeType.Absolute, 115)) ' Header & Guardrail Banner
            root.RowStyles.Add(New RowStyle(SizeType.Percent, 55))  ' Role Permission Matrix Grid
            root.RowStyles.Add(New RowStyle(SizeType.Percent, 45))  ' Confirm Sensitive Change & Info

            ' Wire events
            AddHandler btnReviewChanges.Click, Sub(s, e) RaiseEvent ReviewChangesRequested(Me, EventArgs.Empty)
            AddHandler btnCancelChange.Click, Sub(s, e) RaiseEvent CancelChangeRequested(Me, EventArgs.Empty)
            AddHandler btnConfirmChange.Click, Sub(s, e) RaiseEvent ConfirmSensitiveChangeRequested(Me, EventArgs.Empty)

            ' ==========================================
            ' ROW 0: Header & Anti-Lockout Guardrail Banner
            ' ==========================================
            Dim headerPanel As New Panel With {.Dock = DockStyle.Fill}
            Dim lblTitle As New Label With {.Text = "Access Permissions Matrix", .Font = New Font("Segoe UI", 16, FontStyle.Bold), .ForeColor = TextDark, .AutoSize = True, .Location = New Point(0, 0)}
            Dim lblSubTitle As New Label With {.Text = "Role policy · HQ branch · changes restricted to the System Administrator", .Font = New Font("Segoe UI", 8.5F), .ForeColor = TextMuted, .AutoSize = True, .Location = New Point(0, 28)}

            btnReviewChanges.Anchor = AnchorStyles.Right Or AnchorStyles.Top
            btnReviewChanges.Top = 0
            btnReviewChanges.Left = headerPanel.Width - btnReviewChanges.Width
            headerPanel.Controls.Add(btnReviewChanges)
            headerPanel.Controls.Add(lblTitle)
            headerPanel.Controls.Add(lblSubTitle)

            ' Guardrail Banner
            Dim pnlGuardrail As New Panel With {.BackColor = Color.FromArgb(254, 242, 242), .Padding = New Padding(10), .Height = 44, .Dock = DockStyle.Bottom}
            Dim lblGuardText As New Label With {.Text = "Protected authority · anti-lockout guardrail" & vbCrLf & "Server-side authorization must validate every protected route and action. You cannot grant beyond your authority or remove critical Manage Users / Change Permissions access from the final active administrator.", .Font = New Font("Segoe UI", 8.0F), .ForeColor = Color.FromArgb(180, 40, 40), .Dock = DockStyle.Fill}
            pnlGuardrail.Controls.Add(lblGuardText)
            AddHandler pnlGuardrail.Paint, AddressOf DrawAlertBorder

            Dim topPanel As New Panel With {.Dock = DockStyle.Fill}
            topPanel.Controls.Add(pnlGuardrail)
            topPanel.Controls.Add(headerPanel)
            root.Controls.Add(topPanel, 0, 0)

            ' ==========================================
            ' ROW 1: Role Permission Matrix Card
            ' ==========================================
            Dim cardMatrix As New Panel With {.Dock = DockStyle.Fill, .BackColor = CardBgColor, .Padding = New Padding(12), .Margin = New Padding(0, 4, 0, 4)}
            AddHandler cardMatrix.Paint, AddressOf DrawCardBorder

            Dim lblMatrixTitle As New Label With {.Text = "Role permission matrix", .Dock = DockStyle.Top, .Height = 22, .Font = New Font("Segoe UI", 11, FontStyle.Bold), .ForeColor = TextDark}
            Dim lblMatrixLegend As New Label With {.Text = "Allowed = action access · Read-only = view without mutation · Restricted = no access", .Dock = DockStyle.Top, .Height = 18, .Font = New Font("Segoe UI", 8.0F), .ForeColor = TextMuted}

            FormatGrid(dgvMatrix)
            cardMatrix.Controls.Add(dgvMatrix)
            cardMatrix.Controls.Add(lblMatrixLegend)
            cardMatrix.Controls.Add(lblMatrixTitle)
            root.Controls.Add(cardMatrix, 0, 1)

            ' ==========================================
            ' ROW 2: Split Bottom Cards (Confirm Change / Auditable Info)
            ' ==========================================
            Dim bottomSplit As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 2, .RowCount = 1, .Padding = New Padding(0, 4, 0, 0)}
            bottomSplit.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 65))
            bottomSplit.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 35))

            ' Left: Confirm Sensitive Permission Change Card
            Dim cardConfirm As New Panel With {.Dock = DockStyle.Fill, .BackColor = CardBgColor, .Padding = New Padding(12), .Margin = New Padding(0, 0, 8, 0)}
            AddHandler cardConfirm.Paint, AddressOf DrawCardBorder

            Dim lblConfTitle As New Label With {.Text = "Confirm sensitive permission change", .Dock = DockStyle.Top, .Height = 22, .Font = New Font("Segoe UI", 11, FontStyle.Bold), .ForeColor = TextDark}
            Dim lblConfSub As New Label With {.Text = "Draft example · Export Reports for Dispatch Officer", .Dock = DockStyle.Top, .Height = 18, .Font = New Font("Segoe UI", 8.0F), .ForeColor = TextMuted}

            Dim pnlDiff As New FlowLayoutPanel With {.Dock = DockStyle.Top, .Height = 36, .FlowDirection = FlowDirection.LeftToRight}
            pnlDiff.Controls.Add(New Label With {.Text = "BEFORE: -- Restricted", .Font = New Font("Segoe UI", 8.5F), .ForeColor = TextMuted, .Width = 180})
            pnlDiff.Controls.Add(New Label With {.Text = "AFTER - PENDING CONFIRMATION: ● Read-only", .Font = New Font("Segoe UI", 8.5F, FontStyle.Bold), .ForeColor = TextDark, .Width = 260})

            Dim confForm As New FlowLayoutPanel With {.Dock = DockStyle.Fill, .FlowDirection = FlowDirection.TopDown, .WrapContents = False}
            confForm.Controls.Add(New Label With {.Text = "Audit reason *", .Font = New Font("Segoe UI", 8.5F), .ForeColor = TextMuted, .AutoSize = True})
            txtAuditReason.Width = 440
            confForm.Controls.Add(txtAuditReason)
            confForm.Controls.Add(New Label With {.Height = 4})
            confForm.Controls.Add(hwndCheckboxFix())
            confForm.Controls.Add(New Label With {.Height = 6})

            Dim confActions As New FlowLayoutPanel With {.AutoSize = True, .FlowDirection = FlowDirection.LeftToRight}
            btnCancelChange.Margin = New Padding(0, 0, 8, 0)
            confActions.Controls.Add(btnCancelChange)
            confActions.Controls.Add(btnConfirmChange)
            confForm.Controls.Add(confActions)

            cardConfirm.Controls.Add(confForm)
            cardConfirm.Controls.Add(pnlDiff)
            cardConfirm.Controls.Add(lblConfSub)
            cardConfirm.Controls.Add(lblConfTitle)
            bottomSplit.Controls.Add(cardConfirm, 0, 0)

            ' Right: Permission changes are auditable Card
            Dim cardAuditable As New Panel With {.Dock = DockStyle.Fill, .BackColor = CardBgColor, .Padding = New Padding(12), .Margin = New Padding(8, 0, 0, 0)}
            AddHandler cardAuditable.Paint, AddressOf DrawCardBorder

            Dim lblAuditTitle As New Label With {.Text = "Permission changes are auditable", .Dock = DockStyle.Top, .Height = 22, .Font = New Font("Segoe UI", 11, FontStyle.Bold), .ForeColor = TextDark}
            Dim lblAuditBody As New Label With {.Text = "The published matrix remains unchanged until confirmation. Before/after states, branch scope, actor and authorization context are retained. Read-only Export Reports permits report viewing, never downloadable exports.", .Font = New Font("Segoe UI", 8.5F), .ForeColor = TextMuted, .Dock = DockStyle.Fill}

            cardAuditable.Controls.Add(lblAuditBody)
            cardAuditable.Controls.Add(lblAuditTitle)
            bottomSplit.Controls.Add(cardAuditable, 1, 0)

            root.Controls.Add(bottomSplit, 0, 2)

            Me.Controls.Add(root)
        Catch ex As Exception
            MessageBox.Show("Error initializing Access Permissions UI: " & ex.Message, "UI Exception", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function hwndCheckboxFix() As CheckBox
        Return chkReviewBounds
    End Function

    Private Sub FormatGrid(dgv As DataGridView)
        dgv.Dock = DockStyle.Fill
        dgv.BackgroundColor = Color.White
        dgv.BorderStyle = BorderStyle.None
        dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        dgv.RowHeadersVisible = False
        dgv.AllowUserToAddRows = False
        dgv.ReadOnly = True
        dgv.AutoGenerateColumns = True
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250)
        dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 8.5F, FontStyle.Bold)
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = TextMuted
        dgv.EnableHeadersVisualStyles = False
    End Sub

    Private Sub DrawCardBorder(sender As Object, e As PaintEventArgs)
        Dim p = DirectCast(sender, Panel)
        ControlPaint.DrawBorder(e.Graphics, p.ClientRectangle, BorderColor, ButtonBorderStyle.Solid)
    End Sub

    Private Sub DrawAlertBorder(sender As Object, e As PaintEventArgs)
        Dim p = DirectCast(sender, Panel)
        ControlPaint.DrawBorder(e.Graphics, p.ClientRectangle, Color.FromArgb(245, 200, 200), ButtonBorderStyle.Solid)
    End Sub

End Class