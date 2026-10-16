<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class AdminDashboard
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
        Me.pnlSidebar = New System.Windows.Forms.Panel()
        Me.pnlMain = New System.Windows.Forms.Panel()
        Me.pnlMenu = New System.Windows.Forms.Panel()
        Me.pnlUserBox = New System.Windows.Forms.Panel()
        Me.pnlBrand = New System.Windows.Forms.Panel()
        Me.picLogo = New System.Windows.Forms.Panel()
        Me.lblLogoIcon = New System.Windows.Forms.Label()
        Me.lblBrandName = New System.Windows.Forms.Label()
        Me.lblBrandSub = New System.Windows.Forms.Label()
        Me.lblAdminCaption = New System.Windows.Forms.Label()
        Me.btnMenuOverview = New System.Windows.Forms.Button()
        Me.btnMenuFleet = New System.Windows.Forms.Button()
        Me.btnMenuMaintenance = New System.Windows.Forms.Button()
        Me.btnMenuRates = New System.Windows.Forms.Button()
        Me.btnMenuRevenue = New System.Windows.Forms.Button()
        Me.btnMenuDispatch = New System.Windows.Forms.Button()
        Me.btnMenuUsers = New System.Windows.Forms.Button()
        Me.btnMenuAccess = New System.Windows.Forms.Button()
        Me.btnMenuAudit = New System.Windows.Forms.Button()
        Me.btnMenuSettings = New System.Windows.Forms.Button()
        Me.pnlUserLine = New System.Windows.Forms.Panel()
        Me.lblAvatar = New System.Windows.Forms.Label()
        Me.lblUserName = New System.Windows.Forms.Label()
        Me.lblUserRole = New System.Windows.Forms.Label()
        Me.lblSettings = New System.Windows.Forms.Label()
        Me.lblLogout = New System.Windows.Forms.Label()
        Me.pnlContent = New System.Windows.Forms.Panel()
        Me.pnlTopBar = New System.Windows.Forms.Panel()
        Me.pnlTopLine = New System.Windows.Forms.Panel()
        Me.lblCrumbCaption = New System.Windows.Forms.Label()
        Me.lblCrumb = New System.Windows.Forms.Label()
        Me.pnlSearch = New CardPanel()
        Me.lblSearchIcon = New System.Windows.Forms.Label()
        Me.txtSearch = New System.Windows.Forms.TextBox()
        Me.lblDay = New System.Windows.Forms.Label()
        Me.lblDate = New System.Windows.Forms.Label()
        Me.lblBell = New System.Windows.Forms.Label()
        Me.lblTopAvatar = New System.Windows.Forms.Label()
        Me.lblChevron = New System.Windows.Forms.Label()
        Me.pnlSidebar.SuspendLayout()
        Me.pnlMain.SuspendLayout()
        Me.pnlMenu.SuspendLayout()
        Me.pnlUserBox.SuspendLayout()
        Me.pnlBrand.SuspendLayout()
        Me.picLogo.SuspendLayout()
        Me.pnlUserLine.SuspendLayout()
        Me.pnlContent.SuspendLayout()
        Me.pnlTopBar.SuspendLayout()
        Me.pnlTopLine.SuspendLayout()
        Me.pnlSearch.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlSidebar
        '
        Me.pnlSidebar.Dock = DockStyle.Left
        Me.pnlSidebar.BackColor = Color.FromArgb(36, 36, 36)
        Me.pnlSidebar.Location = New Point(0, 0)
        Me.pnlSidebar.Name = "pnlSidebar"
        Me.pnlSidebar.Size = New Size(240, 860)
        Me.pnlSidebar.Controls.Add(Me.pnlMenu)
        Me.pnlSidebar.Controls.Add(Me.pnlUserBox)
        Me.pnlSidebar.Controls.Add(Me.pnlBrand)
        '
        'pnlMain
        '
        Me.pnlMain.Dock = DockStyle.Fill
        Me.pnlMain.BackColor = Color.FromArgb(245, 244, 242)
        Me.pnlMain.Location = New Point(240, 0)
        Me.pnlMain.Name = "pnlMain"
        Me.pnlMain.Size = New Size(1160, 860)
        Me.pnlMain.Controls.Add(Me.pnlContent)
        Me.pnlMain.Controls.Add(Me.pnlTopBar)
        '
        'pnlMenu
        '
        Me.pnlMenu.Dock = DockStyle.Fill
        Me.pnlMenu.BackColor = Color.FromArgb(36, 36, 36)
        Me.pnlMenu.Name = "pnlMenu"
        Me.pnlMenu.Controls.Add(Me.lblAdminCaption)
        Me.pnlMenu.Controls.Add(Me.btnMenuOverview)
        Me.pnlMenu.Controls.Add(Me.btnMenuFleet)
        Me.pnlMenu.Controls.Add(Me.btnMenuMaintenance)
        Me.pnlMenu.Controls.Add(Me.btnMenuRates)
        Me.pnlMenu.Controls.Add(Me.btnMenuRevenue)
        Me.pnlMenu.Controls.Add(Me.btnMenuDispatch)
        Me.pnlMenu.Controls.Add(Me.btnMenuUsers)
        Me.pnlMenu.Controls.Add(Me.btnMenuAccess)
        Me.pnlMenu.Controls.Add(Me.btnMenuAudit)
        Me.pnlMenu.Controls.Add(Me.btnMenuSettings)
        '
        'pnlUserBox
        '
        Me.pnlUserBox.Dock = DockStyle.Bottom
        Me.pnlUserBox.BackColor = Color.FromArgb(36, 36, 36)
        Me.pnlUserBox.Name = "pnlUserBox"
        Me.pnlUserBox.Size = New Size(240, 104)
        Me.pnlUserBox.Controls.Add(Me.pnlUserLine)
        Me.pnlUserBox.Controls.Add(Me.lblAvatar)
        Me.pnlUserBox.Controls.Add(Me.lblUserName)
        Me.pnlUserBox.Controls.Add(Me.lblUserRole)
        Me.pnlUserBox.Controls.Add(Me.lblSettings)
        Me.pnlUserBox.Controls.Add(Me.lblLogout)
        '
        'pnlBrand
        '
        Me.pnlBrand.Dock = DockStyle.Top
        Me.pnlBrand.BackColor = Color.FromArgb(36, 36, 36)
        Me.pnlBrand.Name = "pnlBrand"
        Me.pnlBrand.Size = New Size(240, 96)
        Me.pnlBrand.Controls.Add(Me.picLogo)
        Me.pnlBrand.Controls.Add(Me.lblBrandName)
        Me.pnlBrand.Controls.Add(Me.lblBrandSub)
        '
        'picLogo
        '
        Me.picLogo.BackColor = Color.FromArgb(122, 0, 16)
        Me.picLogo.Location = New Point(24, 24)
        Me.picLogo.Name = "picLogo"
        Me.picLogo.Size = New Size(46, 46)
        Me.picLogo.Controls.Add(Me.lblLogoIcon)
        '
        'lblLogoIcon
        '
        Me.lblLogoIcon.AutoSize = False
        Me.lblLogoIcon.BackColor = Color.Transparent
        Me.lblLogoIcon.Font = New Font("Segoe UI Emoji", 16.0!)
        Me.lblLogoIcon.ForeColor = Color.White
        Me.lblLogoIcon.Location = New Point(0, 0)
        Me.lblLogoIcon.Size = New Size(46, 46)
        Me.lblLogoIcon.Text = "🚗"
        Me.lblLogoIcon.TextAlign = ContentAlignment.MiddleCenter
        Me.lblLogoIcon.UseMnemonic = False
        '
        'lblBrandName
        '
        Me.lblBrandName.AutoSize = True
        Me.lblBrandName.BackColor = Color.Transparent
        Me.lblBrandName.Font = New Font("Segoe UI", 11.0!, FontStyle.Bold)
        Me.lblBrandName.ForeColor = Color.White
        Me.lblBrandName.Location = New Point(82, 26)
        Me.lblBrandName.Text = "CAR RENTAL"
        Me.lblBrandName.UseMnemonic = False
        '
        'lblBrandSub
        '
        Me.lblBrandSub.AutoSize = True
        Me.lblBrandSub.BackColor = Color.Transparent
        Me.lblBrandSub.Font = New Font("Segoe UI", 7.5!)
        Me.lblBrandSub.ForeColor = Color.FromArgb(170, 170, 170)
        Me.lblBrandSub.Location = New Point(83, 50)
        Me.lblBrandSub.Text = "MANAGEMENT SYSTEM"
        Me.lblBrandSub.UseMnemonic = False
        '
        'lblAdminCaption
        '
        Me.lblAdminCaption.AutoSize = True
        Me.lblAdminCaption.BackColor = Color.Transparent
        Me.lblAdminCaption.Font = New Font("Segoe UI", 8.0!, FontStyle.Bold)
        Me.lblAdminCaption.ForeColor = Color.FromArgb(190, 190, 190)
        Me.lblAdminCaption.Location = New Point(24, 16)
        Me.lblAdminCaption.Text = "ADMINISTRATION"
        Me.lblAdminCaption.UseMnemonic = False
        '
        'btnMenuOverview
        '
        Me.btnMenuOverview.FlatAppearance.BorderSize = 0
        Me.btnMenuOverview.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 60)
        Me.btnMenuOverview.BackColor = Color.FromArgb(36, 36, 36)
        Me.btnMenuOverview.Cursor = Cursors.Hand
        Me.btnMenuOverview.FlatStyle = FlatStyle.Flat
        Me.btnMenuOverview.Font = New Font("Segoe UI", 10.5!)
        Me.btnMenuOverview.ForeColor = Color.FromArgb(205, 205, 205)
        Me.btnMenuOverview.Location = New Point(14, 44)
        Me.btnMenuOverview.Padding = New Padding(14, 0, 0, 0)
        Me.btnMenuOverview.Size = New Size(212, 46)
        Me.btnMenuOverview.Tag = "Overview Dashboard"
        Me.btnMenuOverview.Text = "▦  Overview Dashboard"
        Me.btnMenuOverview.TextAlign = ContentAlignment.MiddleLeft
        Me.btnMenuOverview.UseMnemonic = False
        Me.btnMenuOverview.UseVisualStyleBackColor = False
        '
        'btnMenuFleet
        '
        Me.btnMenuFleet.FlatAppearance.BorderSize = 0
        Me.btnMenuFleet.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 60)
        Me.btnMenuFleet.BackColor = Color.FromArgb(36, 36, 36)
        Me.btnMenuFleet.Cursor = Cursors.Hand
        Me.btnMenuFleet.FlatStyle = FlatStyle.Flat
        Me.btnMenuFleet.Font = New Font("Segoe UI", 10.5!)
        Me.btnMenuFleet.ForeColor = Color.FromArgb(205, 205, 205)
        Me.btnMenuFleet.Location = New Point(14, 94)
        Me.btnMenuFleet.Padding = New Padding(14, 0, 0, 0)
        Me.btnMenuFleet.Size = New Size(212, 46)
        Me.btnMenuFleet.Tag = "Fleet Inventory"
        Me.btnMenuFleet.Text = "🚗  Fleet Inventory"
        Me.btnMenuFleet.TextAlign = ContentAlignment.MiddleLeft
        Me.btnMenuFleet.UseMnemonic = False
        Me.btnMenuFleet.UseVisualStyleBackColor = False
        '
        'btnMenuMaintenance
        '
        Me.btnMenuMaintenance.FlatAppearance.BorderSize = 0
        Me.btnMenuMaintenance.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 60)
        Me.btnMenuMaintenance.BackColor = Color.FromArgb(36, 36, 36)
        Me.btnMenuMaintenance.Cursor = Cursors.Hand
        Me.btnMenuMaintenance.FlatStyle = FlatStyle.Flat
        Me.btnMenuMaintenance.Font = New Font("Segoe UI", 10.5!)
        Me.btnMenuMaintenance.ForeColor = Color.FromArgb(205, 205, 205)
        Me.btnMenuMaintenance.Location = New Point(14, 144)
        Me.btnMenuMaintenance.Padding = New Padding(14, 0, 0, 0)
        Me.btnMenuMaintenance.Size = New Size(212, 46)
        Me.btnMenuMaintenance.Tag = "Maintenance & Service"
        Me.btnMenuMaintenance.Text = "🔧  Maintenance & Service"
        Me.btnMenuMaintenance.TextAlign = ContentAlignment.MiddleLeft
        Me.btnMenuMaintenance.UseMnemonic = False
        Me.btnMenuMaintenance.UseVisualStyleBackColor = False
        '
        'btnMenuRates
        '
        Me.btnMenuRates.FlatAppearance.BorderSize = 0
        Me.btnMenuRates.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 60)
        Me.btnMenuRates.BackColor = Color.FromArgb(36, 36, 36)
        Me.btnMenuRates.Cursor = Cursors.Hand
        Me.btnMenuRates.FlatStyle = FlatStyle.Flat
        Me.btnMenuRates.Font = New Font("Segoe UI", 10.5!)
        Me.btnMenuRates.ForeColor = Color.FromArgb(205, 205, 205)
        Me.btnMenuRates.Location = New Point(14, 194)
        Me.btnMenuRates.Padding = New Padding(14, 0, 0, 0)
        Me.btnMenuRates.Size = New Size(212, 46)
        Me.btnMenuRates.Tag = "Rate Configuration"
        Me.btnMenuRates.Text = "🏷  Rate Configuration"
        Me.btnMenuRates.TextAlign = ContentAlignment.MiddleLeft
        Me.btnMenuRates.UseMnemonic = False
        Me.btnMenuRates.UseVisualStyleBackColor = False
        '
        'btnMenuRevenue
        '
        Me.btnMenuRevenue.FlatAppearance.BorderSize = 0
        Me.btnMenuRevenue.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 60)
        Me.btnMenuRevenue.BackColor = Color.FromArgb(36, 36, 36)
        Me.btnMenuRevenue.Cursor = Cursors.Hand
        Me.btnMenuRevenue.FlatStyle = FlatStyle.Flat
        Me.btnMenuRevenue.Font = New Font("Segoe UI", 10.5!)
        Me.btnMenuRevenue.ForeColor = Color.FromArgb(205, 205, 205)
        Me.btnMenuRevenue.Location = New Point(14, 244)
        Me.btnMenuRevenue.Padding = New Padding(14, 0, 0, 0)
        Me.btnMenuRevenue.Size = New Size(212, 46)
        Me.btnMenuRevenue.Tag = "Revenue Reports"
        Me.btnMenuRevenue.Text = "📈  Revenue Reports"
        Me.btnMenuRevenue.TextAlign = ContentAlignment.MiddleLeft
        Me.btnMenuRevenue.UseMnemonic = False
        Me.btnMenuRevenue.UseVisualStyleBackColor = False
        '
        'btnMenuDispatch
        '
        Me.btnMenuDispatch.FlatAppearance.BorderSize = 0
        Me.btnMenuDispatch.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 60)
        Me.btnMenuDispatch.BackColor = Color.FromArgb(36, 36, 36)
        Me.btnMenuDispatch.Cursor = Cursors.Hand
        Me.btnMenuDispatch.FlatStyle = FlatStyle.Flat
        Me.btnMenuDispatch.Font = New Font("Segoe UI", 10.5!)
        Me.btnMenuDispatch.ForeColor = Color.FromArgb(205, 205, 205)
        Me.btnMenuDispatch.Location = New Point(14, 294)
        Me.btnMenuDispatch.Padding = New Padding(14, 0, 0, 0)
        Me.btnMenuDispatch.Size = New Size(212, 46)
        Me.btnMenuDispatch.Tag = "Dispatch Reports"
        Me.btnMenuDispatch.Text = "🚚  Dispatch Reports"
        Me.btnMenuDispatch.TextAlign = ContentAlignment.MiddleLeft
        Me.btnMenuDispatch.UseMnemonic = False
        Me.btnMenuDispatch.UseVisualStyleBackColor = False
        '
        'btnMenuUsers
        '
        Me.btnMenuUsers.FlatAppearance.BorderSize = 0
        Me.btnMenuUsers.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 60)
        Me.btnMenuUsers.BackColor = Color.FromArgb(36, 36, 36)
        Me.btnMenuUsers.Cursor = Cursors.Hand
        Me.btnMenuUsers.FlatStyle = FlatStyle.Flat
        Me.btnMenuUsers.Font = New Font("Segoe UI", 10.5!)
        Me.btnMenuUsers.ForeColor = Color.FromArgb(205, 205, 205)
        Me.btnMenuUsers.Location = New Point(14, 344)
        Me.btnMenuUsers.Padding = New Padding(14, 0, 0, 0)
        Me.btnMenuUsers.Size = New Size(212, 46)
        Me.btnMenuUsers.Tag = "User Management"
        Me.btnMenuUsers.Text = "👥  User Management"
        Me.btnMenuUsers.TextAlign = ContentAlignment.MiddleLeft
        Me.btnMenuUsers.UseMnemonic = False
        Me.btnMenuUsers.UseVisualStyleBackColor = False
        '
        'btnMenuAccess
        '
        Me.btnMenuAccess.FlatAppearance.BorderSize = 0
        Me.btnMenuAccess.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 60)
        Me.btnMenuAccess.BackColor = Color.FromArgb(36, 36, 36)
        Me.btnMenuAccess.Cursor = Cursors.Hand
        Me.btnMenuAccess.FlatStyle = FlatStyle.Flat
        Me.btnMenuAccess.Font = New Font("Segoe UI", 10.5!)
        Me.btnMenuAccess.ForeColor = Color.FromArgb(205, 205, 205)
        Me.btnMenuAccess.Location = New Point(14, 394)
        Me.btnMenuAccess.Padding = New Padding(14, 0, 0, 0)
        Me.btnMenuAccess.Size = New Size(212, 46)
        Me.btnMenuAccess.Tag = "Access Permissions"
        Me.btnMenuAccess.Text = "🛡  Access Permissions"
        Me.btnMenuAccess.TextAlign = ContentAlignment.MiddleLeft
        Me.btnMenuAccess.UseMnemonic = False
        Me.btnMenuAccess.UseVisualStyleBackColor = False
        '
        'btnMenuAudit
        '
        Me.btnMenuAudit.FlatAppearance.BorderSize = 0
        Me.btnMenuAudit.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 60)
        Me.btnMenuAudit.BackColor = Color.FromArgb(36, 36, 36)
        Me.btnMenuAudit.Cursor = Cursors.Hand
        Me.btnMenuAudit.FlatStyle = FlatStyle.Flat
        Me.btnMenuAudit.Font = New Font("Segoe UI", 10.5!)
        Me.btnMenuAudit.ForeColor = Color.FromArgb(205, 205, 205)
        Me.btnMenuAudit.Location = New Point(14, 444)
        Me.btnMenuAudit.Padding = New Padding(14, 0, 0, 0)
        Me.btnMenuAudit.Size = New Size(212, 46)
        Me.btnMenuAudit.Tag = "Audit Logs"
        Me.btnMenuAudit.Text = "📋  Audit Logs"
        Me.btnMenuAudit.TextAlign = ContentAlignment.MiddleLeft
        Me.btnMenuAudit.UseMnemonic = False
        Me.btnMenuAudit.UseVisualStyleBackColor = False
        '
        'btnMenuSettings
        '
        Me.btnMenuSettings.FlatAppearance.BorderSize = 0
        Me.btnMenuSettings.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 60)
        Me.btnMenuSettings.BackColor = Color.FromArgb(36, 36, 36)
        Me.btnMenuSettings.Cursor = Cursors.Hand
        Me.btnMenuSettings.FlatStyle = FlatStyle.Flat
        Me.btnMenuSettings.Font = New Font("Segoe UI", 10.5!)
        Me.btnMenuSettings.ForeColor = Color.FromArgb(205, 205, 205)
        Me.btnMenuSettings.Location = New Point(14, 494)
        Me.btnMenuSettings.Padding = New Padding(14, 0, 0, 0)
        Me.btnMenuSettings.Size = New Size(212, 46)
        Me.btnMenuSettings.Tag = "System Settings"
        Me.btnMenuSettings.Text = "⚙  System Settings"
        Me.btnMenuSettings.TextAlign = ContentAlignment.MiddleLeft
        Me.btnMenuSettings.UseMnemonic = False
        Me.btnMenuSettings.UseVisualStyleBackColor = False
        '
        'pnlUserLine
        '
        Me.pnlUserLine.Dock = DockStyle.Top
        Me.pnlUserLine.BackColor = Color.FromArgb(64, 64, 64)
        Me.pnlUserLine.Name = "pnlUserLine"
        Me.pnlUserLine.Size = New Size(240, 1)
        '
        'lblAvatar
        '
        Me.lblAvatar.AutoSize = False
        Me.lblAvatar.BackColor = Color.FromArgb(80, 80, 80)
        Me.lblAvatar.Font = New Font("Segoe UI", 10.0!, FontStyle.Bold)
        Me.lblAvatar.ForeColor = Color.White
        Me.lblAvatar.Location = New Point(22, 18)
        Me.lblAvatar.Size = New Size(42, 42)
        Me.lblAvatar.Text = "AM"
        Me.lblAvatar.TextAlign = ContentAlignment.MiddleCenter
        Me.lblAvatar.UseMnemonic = False
        '
        'lblUserName
        '
        Me.lblUserName.AutoSize = True
        Me.lblUserName.BackColor = Color.Transparent
        Me.lblUserName.Font = New Font("Segoe UI", 10.5!, FontStyle.Bold)
        Me.lblUserName.ForeColor = Color.White
        Me.lblUserName.Location = New Point(74, 20)
        Me.lblUserName.Text = "Alex Morgan"
        Me.lblUserName.UseMnemonic = False
        '
        'lblUserRole
        '
        Me.lblUserRole.AutoSize = True
        Me.lblUserRole.BackColor = Color.Transparent
        Me.lblUserRole.Font = New Font("Segoe UI", 7.5!)
        Me.lblUserRole.ForeColor = Color.FromArgb(170, 170, 170)
        Me.lblUserRole.Location = New Point(75, 42)
        Me.lblUserRole.Text = "SYSTEM ADMINISTRATOR"
        Me.lblUserRole.UseMnemonic = False
        '
        'lblSettings
        '
        Me.lblSettings.AutoSize = True
        Me.lblSettings.BackColor = Color.Transparent
        Me.lblSettings.Cursor = Cursors.Hand
        Me.lblSettings.Font = New Font("Segoe UI", 9.0!)
        Me.lblSettings.ForeColor = Color.FromArgb(200, 200, 200)
        Me.lblSettings.Location = New Point(22, 72)
        Me.lblSettings.Text = "⚙  Settings"
        Me.lblSettings.UseMnemonic = False
        '
        'lblLogout
        '
        Me.lblLogout.AutoSize = True
        Me.lblLogout.BackColor = Color.Transparent
        Me.lblLogout.Cursor = Cursors.Hand
        Me.lblLogout.Font = New Font("Segoe UI", 9.0!)
        Me.lblLogout.ForeColor = Color.FromArgb(200, 200, 200)
        Me.lblLogout.Location = New Point(150, 72)
        Me.lblLogout.Text = "➜  Logout"
        Me.lblLogout.UseMnemonic = False
        '
        'pnlTopBar
        '
        Me.pnlTopBar.Dock = DockStyle.Top
        Me.pnlTopBar.BackColor = Color.White
        Me.pnlTopBar.Location = New Point(0, 0)
        Me.pnlTopBar.Name = "pnlTopBar"
        Me.pnlTopBar.Size = New Size(1160, 92)
        Me.pnlTopBar.Controls.Add(Me.pnlTopLine)
        Me.pnlTopBar.Controls.Add(Me.lblCrumbCaption)
        Me.pnlTopBar.Controls.Add(Me.lblCrumb)
        Me.pnlTopBar.Controls.Add(Me.pnlSearch)
        Me.pnlTopBar.Controls.Add(Me.lblDay)
        Me.pnlTopBar.Controls.Add(Me.lblDate)
        Me.pnlTopBar.Controls.Add(Me.lblBell)
        Me.pnlTopBar.Controls.Add(Me.lblTopAvatar)
        Me.pnlTopBar.Controls.Add(Me.lblChevron)
        '
        '
        'pnlContent
        '
        Me.pnlContent.BackColor = Color.FromArgb(248, 249, 250)
        Me.pnlContent.Dock = DockStyle.Fill
        Me.pnlContent.Location = New Point(0, 92)
        Me.pnlContent.Name = "pnlContent"
        Me.pnlContent.Size = New Size(1160, 768)
        Me.pnlContent.TabIndex = 1
        Me.pnlContent.Visible = True
        '
        'pnlTopLine
        '
        Me.pnlTopLine.Dock = DockStyle.Bottom
        Me.pnlTopLine.BackColor = Color.FromArgb(228, 225, 220)
        Me.pnlTopLine.Name = "pnlTopLine"
        Me.pnlTopLine.Size = New Size(1160, 1)
        '
        'lblCrumbCaption
        '
        Me.lblCrumbCaption.AutoSize = True
        Me.lblCrumbCaption.BackColor = Color.Transparent
        Me.lblCrumbCaption.Font = New Font("Segoe UI", 7.5!)
        Me.lblCrumbCaption.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblCrumbCaption.Location = New Point(24, 24)
        Me.lblCrumbCaption.Text = "MANAGEMENT & CONFIGURATION"
        Me.lblCrumbCaption.UseMnemonic = False
        '
        'lblCrumb
        '
        Me.lblCrumb.AutoSize = True
        Me.lblCrumb.BackColor = Color.Transparent
        Me.lblCrumb.Font = New Font("Segoe UI", 10.0!, FontStyle.Bold)
        Me.lblCrumb.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblCrumb.Location = New Point(24, 42)
        Me.lblCrumb.Text = "Overview Dashboard / Admin Overview Dashboard"
        Me.lblCrumb.UseMnemonic = False
        '
        'pnlSearch
        '
        Me.pnlSearch.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.pnlSearch.BackColor = Color.White
        Me.pnlSearch.Location = New Point(600, 22)
        Me.pnlSearch.Name = "pnlSearch"
        Me.pnlSearch.Size = New Size(280, 44)
        Me.pnlSearch.Controls.Add(Me.lblSearchIcon)
        Me.pnlSearch.Controls.Add(Me.txtSearch)
        '
        'lblSearchIcon
        '
        Me.lblSearchIcon.AutoSize = False
        Me.lblSearchIcon.BackColor = Color.Transparent
        Me.lblSearchIcon.Font = New Font("Segoe UI Emoji", 10.0!)
        Me.lblSearchIcon.ForeColor = Color.FromArgb(150, 150, 150)
        Me.lblSearchIcon.Location = New Point(8, 0)
        Me.lblSearchIcon.Size = New Size(30, 42)
        Me.lblSearchIcon.Text = "🔍"
        Me.lblSearchIcon.TextAlign = ContentAlignment.MiddleCenter
        Me.lblSearchIcon.UseMnemonic = False
        '
        'txtSearch
        '
        Me.txtSearch.BackColor = Color.White
        Me.txtSearch.BorderStyle = BorderStyle.None
        Me.txtSearch.Font = New Font("Segoe UI", 10.0!)
        Me.txtSearch.Location = New Point(42, 12)
        Me.txtSearch.Name = "txtSearch"
        Me.txtSearch.PlaceholderText = "Search records..."
        Me.txtSearch.Size = New Size(228, 19)
        '
        'lblDay
        '
        Me.lblDay.AutoSize = True
        Me.lblDay.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.lblDay.BackColor = Color.Transparent
        Me.lblDay.Font = New Font("Segoe UI", 7.5!)
        Me.lblDay.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblDay.Location = New Point(905, 26)
        Me.lblDay.Text = "WEDNESDAY"
        Me.lblDay.UseMnemonic = False
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.lblDate.BackColor = Color.Transparent
        Me.lblDate.Font = New Font("Segoe UI", 11.0!, FontStyle.Bold)
        Me.lblDate.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblDate.Location = New Point(905, 42)
        Me.lblDate.Text = "14 Oct 2026"
        Me.lblDate.UseMnemonic = False
        '
        'lblBell
        '
        Me.lblBell.AutoSize = True
        Me.lblBell.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.lblBell.BackColor = Color.Transparent
        Me.lblBell.Cursor = Cursors.Hand
        Me.lblBell.Font = New Font("Segoe UI Emoji", 13.0!)
        Me.lblBell.ForeColor = Color.FromArgb(110, 110, 110)
        Me.lblBell.Location = New Point(1028, 28)
        Me.lblBell.Text = "🔔"
        Me.lblBell.UseMnemonic = False
        '
        'lblTopAvatar
        '
        Me.lblTopAvatar.AutoSize = False
        Me.lblTopAvatar.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.lblTopAvatar.BackColor = Color.FromArgb(250, 228, 230)
        Me.lblTopAvatar.Font = New Font("Segoe UI", 9.5!, FontStyle.Bold)
        Me.lblTopAvatar.ForeColor = Color.FromArgb(122, 0, 16)
        Me.lblTopAvatar.Location = New Point(1076, 24)
        Me.lblTopAvatar.Size = New Size(40, 40)
        Me.lblTopAvatar.Text = "AM"
        Me.lblTopAvatar.TextAlign = ContentAlignment.MiddleCenter
        Me.lblTopAvatar.UseMnemonic = False
        '
        'lblChevron
        '
        Me.lblChevron.AutoSize = True
        Me.lblChevron.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.lblChevron.BackColor = Color.Transparent
        Me.lblChevron.Font = New Font("Segoe UI", 10.0!)
        Me.lblChevron.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblChevron.Location = New Point(1124, 34)
        Me.lblChevron.Text = "▾"
        Me.lblChevron.UseMnemonic = False
        '
        'AdminDashboard
        '
        Me.AutoScaleDimensions = New SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.BackColor = Color.FromArgb(245, 244, 242)
        Me.ClientSize = New Size(1400, 860)
        Me.Font = New Font("Segoe UI", 9.0!)
        Me.MinimumSize = New Size(1100, 700)
        Me.Name = "AdminDashboard"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Text = "AutoRent - Admin Dashboard"
        Me.Controls.Add(Me.pnlMain)
        Me.Controls.Add(Me.pnlSidebar)
        Me.pnlSidebar.ResumeLayout(False)
        Me.pnlMain.ResumeLayout(False)
        Me.pnlMenu.ResumeLayout(False)
        Me.pnlUserBox.ResumeLayout(False)
        Me.pnlBrand.ResumeLayout(False)
        Me.picLogo.ResumeLayout(False)
        Me.pnlUserLine.ResumeLayout(False)
        Me.pnlContent.ResumeLayout(False)
        Me.pnlTopBar.ResumeLayout(False)
        Me.pnlTopLine.ResumeLayout(False)
        Me.pnlSearch.ResumeLayout(False)
        Me.pnlSidebar.PerformLayout()
        Me.pnlMain.PerformLayout()
        Me.pnlMenu.PerformLayout()
        Me.pnlUserBox.PerformLayout()
        Me.pnlBrand.PerformLayout()
        Me.picLogo.PerformLayout()
        Me.pnlUserLine.PerformLayout()
        Me.pnlContent.PerformLayout()
        Me.pnlTopBar.PerformLayout()
        Me.pnlTopLine.PerformLayout()
        Me.pnlSearch.PerformLayout()
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents pnlSidebar As Panel
    Friend WithEvents pnlMain As Panel
    Friend WithEvents pnlMenu As Panel
    Friend WithEvents pnlUserBox As Panel
    Friend WithEvents pnlBrand As Panel
    Friend WithEvents picLogo As Panel
    Friend WithEvents lblLogoIcon As Label
    Friend WithEvents lblBrandName As Label
    Friend WithEvents lblBrandSub As Label
    Friend WithEvents lblAdminCaption As Label
    Friend WithEvents btnMenuOverview As Button
    Friend WithEvents btnMenuFleet As Button
    Friend WithEvents btnMenuMaintenance As Button
    Friend WithEvents btnMenuRates As Button
    Friend WithEvents btnMenuRevenue As Button
    Friend WithEvents btnMenuDispatch As Button
    Friend WithEvents btnMenuUsers As Button
    Friend WithEvents btnMenuAccess As Button
    Friend WithEvents btnMenuAudit As Button
    Friend WithEvents btnMenuSettings As Button
    Friend WithEvents pnlUserLine As Panel
    Friend WithEvents lblAvatar As Label
    Friend WithEvents lblUserName As Label
    Friend WithEvents lblUserRole As Label
    Friend WithEvents lblSettings As Label
    Friend WithEvents lblLogout As Label
    Friend WithEvents pnlContent As Panel
    Friend WithEvents pnlTopBar As Panel
    Friend WithEvents pnlTopLine As Panel
    Friend WithEvents lblCrumbCaption As Label
    Friend WithEvents lblCrumb As Label
    Friend WithEvents pnlSearch As CardPanel
    Friend WithEvents lblSearchIcon As Label
    Friend WithEvents txtSearch As TextBox
    Friend WithEvents lblDay As Label
    Friend WithEvents lblDate As Label
    Friend WithEvents lblBell As Label
    Friend WithEvents lblTopAvatar As Label
    Friend WithEvents lblChevron As Label
End Class