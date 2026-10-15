<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class LoginForm
    Inherits System.Windows.Forms.Form

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.picBanner = New System.Windows.Forms.PictureBox()
        Me.lblBannerIcon = New System.Windows.Forms.Label()
        Me.lblBannerTitle = New System.Windows.Forms.Label()
        Me.lblBannerSub = New System.Windows.Forms.Label()
        Me.lblHeadline1 = New System.Windows.Forms.Label()
        Me.lblHeadline2 = New System.Windows.Forms.Label()
        Me.lblTagline = New System.Windows.Forms.Label()
        Me.pnlRight = New System.Windows.Forms.Panel()
        Me.lblLogo = New System.Windows.Forms.Label()
        Me.lblBrand = New System.Windows.Forms.Label()
        Me.lblBrandSub = New System.Windows.Forms.Label()
        Me.pnlDivider = New System.Windows.Forms.Panel()
        Me.lblWelcome = New System.Windows.Forms.Label()
        Me.lblSignInMsg = New System.Windows.Forms.Label()
        Me.lblUserCaption = New System.Windows.Forms.Label()
        Me.pnlUser = New System.Windows.Forms.Panel()
        Me.lblUserIcon = New System.Windows.Forms.Label()
        Me.txtUsername = New System.Windows.Forms.TextBox()
        Me.lblPassCaption = New System.Windows.Forms.Label()
        Me.pnlPass = New System.Windows.Forms.Panel()
        Me.lblPassIcon = New System.Windows.Forms.Label()
        Me.txtPassword = New System.Windows.Forms.TextBox()
        Me.lblEye = New System.Windows.Forms.Label()
        Me.chkRemember = New System.Windows.Forms.CheckBox()
        Me.lnkForgot = New System.Windows.Forms.LinkLabel()
        Me.btnSignIn = New System.Windows.Forms.Button()
        Me.lblFooter = New System.Windows.Forms.Label()
        CType(Me.picBanner, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.picBanner.SuspendLayout()
        Me.pnlRight.SuspendLayout()
        Me.pnlUser.SuspendLayout()
        Me.pnlPass.SuspendLayout()
        Me.SuspendLayout()
        '
        'picBanner  (PICTURE PLACEHOLDER - set the Image property here)
        '
        Me.picBanner.BackColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(48, Byte), Integer), CType(CType(54, Byte), Integer))
        Me.picBanner.Controls.Add(Me.lblBannerIcon)
        Me.picBanner.Controls.Add(Me.lblBannerTitle)
        Me.picBanner.Controls.Add(Me.lblBannerSub)
        Me.picBanner.Controls.Add(Me.lblHeadline1)
        Me.picBanner.Controls.Add(Me.lblHeadline2)
        Me.picBanner.Controls.Add(Me.lblTagline)
        Me.picBanner.Dock = System.Windows.Forms.DockStyle.Fill
        Me.picBanner.Location = New System.Drawing.Point(0, 0)
        Me.picBanner.Name = "picBanner"
        Me.picBanner.Size = New System.Drawing.Size(640, 620)
        Me.picBanner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picBanner.TabIndex = 0
        Me.picBanner.TabStop = False
        '
        'lblBannerIcon
        '
        Me.lblBannerIcon.AutoSize = True
        Me.lblBannerIcon.BackColor = System.Drawing.Color.Transparent
        Me.lblBannerIcon.Font = New System.Drawing.Font("Segoe UI Emoji", 20.0!)
        Me.lblBannerIcon.ForeColor = System.Drawing.Color.White
        Me.lblBannerIcon.Location = New System.Drawing.Point(40, 40)
        Me.lblBannerIcon.Name = "lblBannerIcon"
        Me.lblBannerIcon.Text = ChrW(&HD83D) & ChrW(&HDE97)
        '
        'lblBannerTitle
        '
        Me.lblBannerTitle.AutoSize = True
        Me.lblBannerTitle.BackColor = System.Drawing.Color.Transparent
        Me.lblBannerTitle.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.lblBannerTitle.ForeColor = System.Drawing.Color.White
        Me.lblBannerTitle.Location = New System.Drawing.Point(95, 44)
        Me.lblBannerTitle.Name = "lblBannerTitle"
        Me.lblBannerTitle.Text = "AutoRent"
        '
        'lblBannerSub
        '
        Me.lblBannerSub.AutoSize = True
        Me.lblBannerSub.BackColor = System.Drawing.Color.Transparent
        Me.lblBannerSub.Font = New System.Drawing.Font("Segoe UI", 8.0!)
        Me.lblBannerSub.ForeColor = System.Drawing.Color.Gainsboro
        Me.lblBannerSub.Location = New System.Drawing.Point(96, 66)
        Me.lblBannerSub.Name = "lblBannerSub"
        Me.lblBannerSub.Text = "CAR RENTAL MANAGEMENT SYSTEM"
        '
        'lblHeadline1
        '
        Me.lblHeadline1.AutoSize = True
        Me.lblHeadline1.BackColor = System.Drawing.Color.Transparent
        Me.lblHeadline1.Font = New System.Drawing.Font("Segoe UI Light", 36.0!)
        Me.lblHeadline1.ForeColor = System.Drawing.Color.White
        Me.lblHeadline1.Location = New System.Drawing.Point(40, 380)
        Me.lblHeadline1.Name = "lblHeadline1"
        Me.lblHeadline1.Text = "Precision in"
        '
        'lblHeadline2
        '
        Me.lblHeadline2.AutoSize = True
        Me.lblHeadline2.BackColor = System.Drawing.Color.Transparent
        Me.lblHeadline2.Font = New System.Drawing.Font("Segoe UI", 36.0!)
        Me.lblHeadline2.ForeColor = System.Drawing.Color.White
        Me.lblHeadline2.Location = New System.Drawing.Point(40, 440)
        Me.lblHeadline2.Name = "lblHeadline2"
        Me.lblHeadline2.Text = "Every Journey."
        '
        'lblTagline
        '
        Me.lblTagline.BackColor = System.Drawing.Color.Transparent
        Me.lblTagline.Font = New System.Drawing.Font("Segoe UI Light", 12.0!)
        Me.lblTagline.ForeColor = System.Drawing.Color.Gainsboro
        Me.lblTagline.Location = New System.Drawing.Point(44, 520)
        Me.lblTagline.Name = "lblTagline"
        Me.lblTagline.Size = New System.Drawing.Size(460, 50)
        Me.lblTagline.Text = "A smarter way to manage fleet operations, daily dispatch, and vehicle inspections."
        '
        'pnlRight
        '
        Me.pnlRight.BackColor = System.Drawing.Color.FromArgb(CType(CType(245, Byte), Integer), CType(CType(244, Byte), Integer), CType(CType(242, Byte), Integer))
        Me.pnlRight.Controls.Add(Me.lblLogo)
        Me.pnlRight.Controls.Add(Me.lblBrand)
        Me.pnlRight.Controls.Add(Me.lblBrandSub)
        Me.pnlRight.Controls.Add(Me.pnlDivider)
        Me.pnlRight.Controls.Add(Me.lblWelcome)
        Me.pnlRight.Controls.Add(Me.lblSignInMsg)
        Me.pnlRight.Controls.Add(Me.lblUserCaption)
        Me.pnlRight.Controls.Add(Me.pnlUser)
        Me.pnlRight.Controls.Add(Me.lblPassCaption)
        Me.pnlRight.Controls.Add(Me.pnlPass)
        Me.pnlRight.Controls.Add(Me.chkRemember)
        Me.pnlRight.Controls.Add(Me.lnkForgot)
        Me.pnlRight.Controls.Add(Me.btnSignIn)
        Me.pnlRight.Controls.Add(Me.lblFooter)
        Me.pnlRight.Dock = System.Windows.Forms.DockStyle.Right
        Me.pnlRight.Location = New System.Drawing.Point(640, 0)
        Me.pnlRight.Name = "pnlRight"
        Me.pnlRight.Size = New System.Drawing.Size(460, 620)
        Me.pnlRight.TabIndex = 1
        '
        'lblLogo
        '
        Me.lblLogo.Font = New System.Drawing.Font("Segoe UI Emoji", 26.0!)
        Me.lblLogo.ForeColor = System.Drawing.Color.FromArgb(CType(CType(122, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(16, Byte), Integer))
        Me.lblLogo.Location = New System.Drawing.Point(0, 30)
        Me.lblLogo.Name = "lblLogo"
        Me.lblLogo.Size = New System.Drawing.Size(460, 56)
        Me.lblLogo.Text = ChrW(&HD83D) & ChrW(&HDE97)
        Me.lblLogo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblBrand
        '
        Me.lblBrand.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Bold)
        Me.lblBrand.ForeColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
        Me.lblBrand.Location = New System.Drawing.Point(0, 92)
        Me.lblBrand.Name = "lblBrand"
        Me.lblBrand.Size = New System.Drawing.Size(460, 20)
        Me.lblBrand.Text = "AutoRent"
        Me.lblBrand.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblBrandSub
        '
        Me.lblBrandSub.Font = New System.Drawing.Font("Segoe UI", 7.5!)
        Me.lblBrandSub.ForeColor = System.Drawing.Color.Gray
        Me.lblBrandSub.Location = New System.Drawing.Point(0, 112)
        Me.lblBrandSub.Name = "lblBrandSub"
        Me.lblBrandSub.Size = New System.Drawing.Size(460, 18)
        Me.lblBrandSub.Text = "C A R  R E N T A L  M A N A G E M E N T  S Y S T E M"
        Me.lblBrandSub.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'pnlDivider
        '
        Me.pnlDivider.BackColor = System.Drawing.Color.Gainsboro
        Me.pnlDivider.Location = New System.Drawing.Point(200, 145)
        Me.pnlDivider.Name = "pnlDivider"
        Me.pnlDivider.Size = New System.Drawing.Size(60, 1)
        '
        'lblWelcome
        '
        Me.lblWelcome.Font = New System.Drawing.Font("Segoe UI", 22.0!)
        Me.lblWelcome.ForeColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
        Me.lblWelcome.Location = New System.Drawing.Point(0, 170)
        Me.lblWelcome.Name = "lblWelcome"
        Me.lblWelcome.Size = New System.Drawing.Size(460, 50)
        Me.lblWelcome.Text = "Welcome Back"
        Me.lblWelcome.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblSignInMsg
        '
        Me.lblSignInMsg.Font = New System.Drawing.Font("Segoe UI", 10.5!)
        Me.lblSignInMsg.ForeColor = System.Drawing.Color.Gray
        Me.lblSignInMsg.Location = New System.Drawing.Point(0, 222)
        Me.lblSignInMsg.Name = "lblSignInMsg"
        Me.lblSignInMsg.Size = New System.Drawing.Size(460, 26)
        Me.lblSignInMsg.Text = "Sign in to access your workspace."
        Me.lblSignInMsg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblUserCaption
        '
        Me.lblUserCaption.AutoSize = True
        Me.lblUserCaption.Font = New System.Drawing.Font("Segoe UI", 8.0!, System.Drawing.FontStyle.Bold)
        Me.lblUserCaption.ForeColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer))
        Me.lblUserCaption.Location = New System.Drawing.Point(40, 280)
        Me.lblUserCaption.Name = "lblUserCaption"
        Me.lblUserCaption.Text = "USERNAME OR USER ID"
        '
        'pnlUser
        '
        Me.pnlUser.BackColor = System.Drawing.Color.White
        Me.pnlUser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnlUser.Controls.Add(Me.lblUserIcon)
        Me.pnlUser.Controls.Add(Me.txtUsername)
        Me.pnlUser.Location = New System.Drawing.Point(40, 302)
        Me.pnlUser.Name = "pnlUser"
        Me.pnlUser.Size = New System.Drawing.Size(380, 50)
        '
        'lblUserIcon
        '
        Me.lblUserIcon.Font = New System.Drawing.Font("Segoe UI Emoji", 11.0!)
        Me.lblUserIcon.ForeColor = System.Drawing.Color.Silver
        Me.lblUserIcon.Location = New System.Drawing.Point(8, 0)
        Me.lblUserIcon.Name = "lblUserIcon"
        Me.lblUserIcon.Size = New System.Drawing.Size(34, 48)
        Me.lblUserIcon.Text = ChrW(&HD83D) & ChrW(&HDC64)
        Me.lblUserIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'txtUsername
        '
        Me.txtUsername.BackColor = System.Drawing.Color.White
        Me.txtUsername.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtUsername.Font = New System.Drawing.Font("Segoe UI", 11.0!)
        Me.txtUsername.Location = New System.Drawing.Point(50, 13)
        Me.txtUsername.Name = "txtUsername"
        Me.txtUsername.PlaceholderText = "Enter your username or user ID"
        Me.txtUsername.Size = New System.Drawing.Size(315, 20)
        Me.txtUsername.TabIndex = 0
        '
        'lblPassCaption
        '
        Me.lblPassCaption.AutoSize = True
        Me.lblPassCaption.Font = New System.Drawing.Font("Segoe UI", 8.0!, System.Drawing.FontStyle.Bold)
        Me.lblPassCaption.ForeColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer))
        Me.lblPassCaption.Location = New System.Drawing.Point(40, 372)
        Me.lblPassCaption.Name = "lblPassCaption"
        Me.lblPassCaption.Text = "PASSWORD"
        '
        'pnlPass
        '
        Me.pnlPass.BackColor = System.Drawing.Color.White
        Me.pnlPass.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnlPass.Controls.Add(Me.lblPassIcon)
        Me.pnlPass.Controls.Add(Me.txtPassword)
        Me.pnlPass.Controls.Add(Me.lblEye)
        Me.pnlPass.Location = New System.Drawing.Point(40, 394)
        Me.pnlPass.Name = "pnlPass"
        Me.pnlPass.Size = New System.Drawing.Size(380, 50)
        '
        'lblPassIcon
        '
        Me.lblPassIcon.Font = New System.Drawing.Font("Segoe UI Emoji", 11.0!)
        Me.lblPassIcon.ForeColor = System.Drawing.Color.Silver
        Me.lblPassIcon.Location = New System.Drawing.Point(8, 0)
        Me.lblPassIcon.Name = "lblPassIcon"
        Me.lblPassIcon.Size = New System.Drawing.Size(34, 48)
        Me.lblPassIcon.Text = ChrW(&HD83D) & ChrW(&HDD12)
        Me.lblPassIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'txtPassword
        '
        Me.txtPassword.BackColor = System.Drawing.Color.White
        Me.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtPassword.Font = New System.Drawing.Font("Segoe UI", 11.0!)
        Me.txtPassword.Location = New System.Drawing.Point(50, 13)
        Me.txtPassword.Name = "txtPassword"
        Me.txtPassword.PlaceholderText = "Enter your password"
        Me.txtPassword.Size = New System.Drawing.Size(280, 20)
        Me.txtPassword.TabIndex = 1
        Me.txtPassword.UseSystemPasswordChar = True
        '
        'lblEye
        '
        Me.lblEye.Cursor = System.Windows.Forms.Cursors.Hand
        Me.lblEye.Font = New System.Drawing.Font("Segoe UI Emoji", 11.0!)
        Me.lblEye.ForeColor = System.Drawing.Color.Silver
        Me.lblEye.Location = New System.Drawing.Point(338, 0)
        Me.lblEye.Name = "lblEye"
        Me.lblEye.Size = New System.Drawing.Size(34, 48)
        Me.lblEye.Text = ChrW(&HD83D) & ChrW(&HDC41)
        Me.lblEye.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'chkRemember
        '
        Me.chkRemember.AutoSize = True
        Me.chkRemember.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.chkRemember.ForeColor = System.Drawing.Color.DimGray
        Me.chkRemember.Location = New System.Drawing.Point(40, 464)
        Me.chkRemember.Name = "chkRemember"
        Me.chkRemember.TabIndex = 2
        Me.chkRemember.Text = "Remember Me"
        '
        'lnkForgot
        '
        Me.lnkForgot.ActiveLinkColor = System.Drawing.Color.Red
        Me.lnkForgot.AutoSize = True
        Me.lnkForgot.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.lnkForgot.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline
        Me.lnkForgot.LinkColor = System.Drawing.Color.FromArgb(CType(CType(122, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(16, Byte), Integer))
        Me.lnkForgot.Location = New System.Drawing.Point(327, 465)
        Me.lnkForgot.Name = "lnkForgot"
        Me.lnkForgot.TabIndex = 3
        Me.lnkForgot.TabStop = True
        Me.lnkForgot.Text = "Forgot Password?"
        '
        'btnSignIn
        '
        Me.btnSignIn.BackColor = System.Drawing.Color.FromArgb(CType(CType(122, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(16, Byte), Integer))
        Me.btnSignIn.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnSignIn.FlatAppearance.BorderSize = 0
        Me.btnSignIn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSignIn.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold)
        Me.btnSignIn.ForeColor = System.Drawing.Color.White
        Me.btnSignIn.Location = New System.Drawing.Point(40, 505)
        Me.btnSignIn.Name = "btnSignIn"
        Me.btnSignIn.Size = New System.Drawing.Size(380, 52)
        Me.btnSignIn.TabIndex = 4
        Me.btnSignIn.Text = "S I G N   I N   " & ChrW(&H2192)
        Me.btnSignIn.UseVisualStyleBackColor = False
        '
        'lblFooter
        '
        Me.lblFooter.Font = New System.Drawing.Font("Segoe UI", 7.0!)
        Me.lblFooter.ForeColor = System.Drawing.Color.Silver
        Me.lblFooter.Location = New System.Drawing.Point(0, 585)
        Me.lblFooter.Name = "lblFooter"
        Me.lblFooter.Size = New System.Drawing.Size(460, 18)
        Me.lblFooter.Text = "A U T H O R I Z E D   P E R S O N N E L   O N L Y"
        Me.lblFooter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LoginForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1100, 620)
        Me.Controls.Add(Me.picBanner)
        Me.Controls.Add(Me.pnlRight)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.Name = "LoginForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "AutoRent - Login"
        CType(Me.picBanner, System.ComponentModel.ISupportInitialize).EndInit()
        Me.picBanner.ResumeLayout(False)
        Me.picBanner.PerformLayout()
        Me.pnlRight.ResumeLayout(False)
        Me.pnlRight.PerformLayout()
        Me.pnlUser.ResumeLayout(False)
        Me.pnlUser.PerformLayout()
        Me.pnlPass.ResumeLayout(False)
        Me.pnlPass.PerformLayout()
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents picBanner As PictureBox
    Friend WithEvents lblBannerIcon As Label
    Friend WithEvents lblBannerTitle As Label
    Friend WithEvents lblBannerSub As Label
    Friend WithEvents lblHeadline1 As Label
    Friend WithEvents lblHeadline2 As Label
    Friend WithEvents lblTagline As Label
    Friend WithEvents pnlRight As Panel
    Friend WithEvents lblLogo As Label
    Friend WithEvents lblBrand As Label
    Friend WithEvents lblBrandSub As Label
    Friend WithEvents pnlDivider As Panel
    Friend WithEvents lblWelcome As Label
    Friend WithEvents lblSignInMsg As Label
    Friend WithEvents lblUserCaption As Label
    Friend WithEvents pnlUser As Panel
    Friend WithEvents lblUserIcon As Label
    Friend WithEvents txtUsername As TextBox
    Friend WithEvents lblPassCaption As Label
    Friend WithEvents pnlPass As Panel
    Friend WithEvents lblPassIcon As Label
    Friend WithEvents txtPassword As TextBox
    Friend WithEvents lblEye As Label
    Friend WithEvents chkRemember As CheckBox
    Friend WithEvents lnkForgot As LinkLabel
    Friend WithEvents btnSignIn As Button
    Friend WithEvents lblFooter As Label
End Class