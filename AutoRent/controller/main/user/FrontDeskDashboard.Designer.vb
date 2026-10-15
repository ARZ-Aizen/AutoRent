<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrontDeskDashboard
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
        Me.pnlActiveBar = New System.Windows.Forms.Panel()
        Me.btnMenuDashboard = New System.Windows.Forms.Button()
        Me.btnMenuFleet = New System.Windows.Forms.Button()
        Me.btnMenuCustomers = New System.Windows.Forms.Button()
        Me.btnMenuReservations = New System.Windows.Forms.Button()
        Me.btnMenuDispatch = New System.Windows.Forms.Button()
        Me.btnMenuInspection = New System.Windows.Forms.Button()
        Me.btnMenuBilling = New System.Windows.Forms.Button()
        Me.btnMenuHistory = New System.Windows.Forms.Button()
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
        Me.lblPageTitle = New System.Windows.Forms.Label()
        Me.pnlSearch = New CardPanel()
        Me.lblSearchIcon = New System.Windows.Forms.Label()
        Me.txtSearch = New System.Windows.Forms.TextBox()
        Me.lblDay = New System.Windows.Forms.Label()
        Me.lblDate = New System.Windows.Forms.Label()
        Me.lblBell = New System.Windows.Forms.Label()
        Me.lblBadge = New System.Windows.Forms.Label()
        Me.lblTopAvatar = New System.Windows.Forms.Label()
        Me.pnlSidebar.SuspendLayout()
        Me.pnlMain.SuspendLayout()
        Me.pnlMenu.SuspendLayout()
        Me.pnlUserBox.SuspendLayout()
        Me.pnlBrand.SuspendLayout()
        Me.picLogo.SuspendLayout()
        Me.pnlActiveBar.SuspendLayout()
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
        Me.pnlMenu.Controls.Add(Me.pnlActiveBar)
        Me.pnlMenu.Controls.Add(Me.btnMenuDashboard)
        Me.pnlMenu.Controls.Add(Me.btnMenuFleet)
        Me.pnlMenu.Controls.Add(Me.btnMenuCustomers)
        Me.pnlMenu.Controls.Add(Me.btnMenuReservations)
        Me.pnlMenu.Controls.Add(Me.btnMenuDispatch)
        Me.pnlMenu.Controls.Add(Me.btnMenuInspection)
        Me.pnlMenu.Controls.Add(Me.btnMenuBilling)
        Me.pnlMenu.Controls.Add(Me.btnMenuHistory)
        '
        'pnlUserBox
        '
        Me.pnlUserBox.Dock = DockStyle.Bottom
        Me.pnlUserBox.BackColor = Color.FromArgb(36, 36, 36)
        Me.pnlUserBox.Name = "pnlUserBox"
        Me.pnlUserBox.Size = New Size(240, 120)
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
        Me.pnlBrand.Size = New Size(240, 90)
        Me.pnlBrand.Controls.Add(Me.picLogo)
        Me.pnlBrand.Controls.Add(Me.lblBrandName)
        Me.pnlBrand.Controls.Add(Me.lblBrandSub)
        '
        'picLogo
        '
        Me.picLogo.BackColor = Color.FromArgb(122, 0, 16)
        Me.picLogo.Location = New Point(24, 22)
        Me.picLogo.Name = "picLogo"
        Me.picLogo.Size = New Size(44, 44)
        Me.picLogo.Controls.Add(Me.lblLogoIcon)
        '
        'lblLogoIcon
        '
        Me.lblLogoIcon.AutoSize = False
        Me.lblLogoIcon.BackColor = Color.Transparent
        Me.lblLogoIcon.Font = New Font("Segoe UI Emoji", 15.0!)
        Me.lblLogoIcon.ForeColor = Color.White
        Me.lblLogoIcon.Location = New Point(0, 0)
        Me.lblLogoIcon.Size = New Size(44, 44)
        Me.lblLogoIcon.Text = "🚗"
        Me.lblLogoIcon.TextAlign = ContentAlignment.MiddleCenter
        Me.lblLogoIcon.UseMnemonic = False
        '
        'lblBrandName
        '
        Me.lblBrandName.AutoSize = True
        Me.lblBrandName.BackColor = Color.Transparent
        Me.lblBrandName.Font = New Font("Segoe UI", 9.5!, FontStyle.Bold)
        Me.lblBrandName.ForeColor = Color.White
        Me.lblBrandName.Location = New Point(80, 26)
        Me.lblBrandName.Text = "CAR RENTAL"
        Me.lblBrandName.UseMnemonic = False
        '
        'lblBrandSub
        '
        Me.lblBrandSub.AutoSize = True
        Me.lblBrandSub.BackColor = Color.Transparent
        Me.lblBrandSub.Font = New Font("Segoe UI", 9.5!, FontStyle.Bold)
        Me.lblBrandSub.ForeColor = Color.White
        Me.lblBrandSub.Location = New Point(80, 44)
        Me.lblBrandSub.Text = "MANAGEMENT"
        Me.lblBrandSub.UseMnemonic = False
        '
        'pnlActiveBar
        '
        Me.pnlActiveBar.BackColor = Color.FromArgb(122, 0, 16)
        Me.pnlActiveBar.Location = New Point(0, 24)
        Me.pnlActiveBar.Name = "pnlActiveBar"
        Me.pnlActiveBar.Size = New Size(4, 46)
        '
        'btnMenuDashboard
        '
        Me.btnMenuDashboard.FlatAppearance.BorderSize = 0
        Me.btnMenuDashboard.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 60)
        Me.btnMenuDashboard.BackColor = Color.FromArgb(36, 36, 36)
        Me.btnMenuDashboard.Cursor = Cursors.Hand
        Me.btnMenuDashboard.FlatStyle = FlatStyle.Flat
        Me.btnMenuDashboard.Font = New Font("Segoe UI", 10.5!)
        Me.btnMenuDashboard.ForeColor = Color.FromArgb(205, 205, 205)
        Me.btnMenuDashboard.Location = New Point(14, 24)
        Me.btnMenuDashboard.Padding = New Padding(14, 0, 0, 0)
        Me.btnMenuDashboard.Size = New Size(212, 46)
        Me.btnMenuDashboard.Tag = "Dashboard"
        Me.btnMenuDashboard.Text = "◔   Dashboard"
        Me.btnMenuDashboard.TextAlign = ContentAlignment.MiddleLeft
        Me.btnMenuDashboard.UseMnemonic = False
        Me.btnMenuDashboard.UseVisualStyleBackColor = False
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
        Me.btnMenuFleet.Location = New Point(14, 76)
        Me.btnMenuFleet.Padding = New Padding(14, 0, 0, 0)
        Me.btnMenuFleet.Size = New Size(212, 46)
        Me.btnMenuFleet.Tag = "Fleet Availability"
        Me.btnMenuFleet.Text = "🚗   Fleet Availability"
        Me.btnMenuFleet.TextAlign = ContentAlignment.MiddleLeft
        Me.btnMenuFleet.UseMnemonic = False
        Me.btnMenuFleet.UseVisualStyleBackColor = False
        '
        'btnMenuCustomers
        '
        Me.btnMenuCustomers.FlatAppearance.BorderSize = 0
        Me.btnMenuCustomers.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 60)
        Me.btnMenuCustomers.BackColor = Color.FromArgb(36, 36, 36)
        Me.btnMenuCustomers.Cursor = Cursors.Hand
        Me.btnMenuCustomers.FlatStyle = FlatStyle.Flat
        Me.btnMenuCustomers.Font = New Font("Segoe UI", 10.5!)
        Me.btnMenuCustomers.ForeColor = Color.FromArgb(205, 205, 205)
        Me.btnMenuCustomers.Location = New Point(14, 128)
        Me.btnMenuCustomers.Padding = New Padding(14, 0, 0, 0)
        Me.btnMenuCustomers.Size = New Size(212, 46)
        Me.btnMenuCustomers.Tag = "Customer Management"
        Me.btnMenuCustomers.Text = "👥   Customer Management"
        Me.btnMenuCustomers.TextAlign = ContentAlignment.MiddleLeft
        Me.btnMenuCustomers.UseMnemonic = False
        Me.btnMenuCustomers.UseVisualStyleBackColor = False
        '
        'btnMenuReservations
        '
        Me.btnMenuReservations.FlatAppearance.BorderSize = 0
        Me.btnMenuReservations.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 60)
        Me.btnMenuReservations.BackColor = Color.FromArgb(36, 36, 36)
        Me.btnMenuReservations.Cursor = Cursors.Hand
        Me.btnMenuReservations.FlatStyle = FlatStyle.Flat
        Me.btnMenuReservations.Font = New Font("Segoe UI", 10.5!)
        Me.btnMenuReservations.ForeColor = Color.FromArgb(205, 205, 205)
        Me.btnMenuReservations.Location = New Point(14, 180)
        Me.btnMenuReservations.Padding = New Padding(14, 0, 0, 0)
        Me.btnMenuReservations.Size = New Size(212, 46)
        Me.btnMenuReservations.Tag = "Reservations"
        Me.btnMenuReservations.Text = "📅   Reservations"
        Me.btnMenuReservations.TextAlign = ContentAlignment.MiddleLeft
        Me.btnMenuReservations.UseMnemonic = False
        Me.btnMenuReservations.UseVisualStyleBackColor = False
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
        Me.btnMenuDispatch.Location = New Point(14, 232)
        Me.btnMenuDispatch.Padding = New Padding(14, 0, 0, 0)
        Me.btnMenuDispatch.Size = New Size(212, 46)
        Me.btnMenuDispatch.Tag = "Daily Dispatch"
        Me.btnMenuDispatch.Text = "🚚   Daily Dispatch"
        Me.btnMenuDispatch.TextAlign = ContentAlignment.MiddleLeft
        Me.btnMenuDispatch.UseMnemonic = False
        Me.btnMenuDispatch.UseVisualStyleBackColor = False
        '
        'btnMenuInspection
        '
        Me.btnMenuInspection.FlatAppearance.BorderSize = 0
        Me.btnMenuInspection.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 60)
        Me.btnMenuInspection.BackColor = Color.FromArgb(36, 36, 36)
        Me.btnMenuInspection.Cursor = Cursors.Hand
        Me.btnMenuInspection.FlatStyle = FlatStyle.Flat
        Me.btnMenuInspection.Font = New Font("Segoe UI", 10.5!)
        Me.btnMenuInspection.ForeColor = Color.FromArgb(205, 205, 205)
        Me.btnMenuInspection.Location = New Point(14, 284)
        Me.btnMenuInspection.Padding = New Padding(14, 0, 0, 0)
        Me.btnMenuInspection.Size = New Size(212, 46)
        Me.btnMenuInspection.Tag = "Vehicle Inspection"
        Me.btnMenuInspection.Text = "📋   Vehicle Inspection"
        Me.btnMenuInspection.TextAlign = ContentAlignment.MiddleLeft
        Me.btnMenuInspection.UseMnemonic = False
        Me.btnMenuInspection.UseVisualStyleBackColor = False
        '
        'btnMenuBilling
        '
        Me.btnMenuBilling.FlatAppearance.BorderSize = 0
        Me.btnMenuBilling.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 60)
        Me.btnMenuBilling.BackColor = Color.FromArgb(36, 36, 36)
        Me.btnMenuBilling.Cursor = Cursors.Hand
        Me.btnMenuBilling.FlatStyle = FlatStyle.Flat
        Me.btnMenuBilling.Font = New Font("Segoe UI", 10.5!)
        Me.btnMenuBilling.ForeColor = Color.FromArgb(205, 205, 205)
        Me.btnMenuBilling.Location = New Point(14, 336)
        Me.btnMenuBilling.Padding = New Padding(14, 0, 0, 0)
        Me.btnMenuBilling.Size = New Size(212, 46)
        Me.btnMenuBilling.Tag = "Billing & Payments"
        Me.btnMenuBilling.Text = "💳   Billing & Payments"
        Me.btnMenuBilling.TextAlign = ContentAlignment.MiddleLeft
        Me.btnMenuBilling.UseMnemonic = False
        Me.btnMenuBilling.UseVisualStyleBackColor = False
        '
        'btnMenuHistory
        '
        Me.btnMenuHistory.FlatAppearance.BorderSize = 0
        Me.btnMenuHistory.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 60)
        Me.btnMenuHistory.BackColor = Color.FromArgb(36, 36, 36)
        Me.btnMenuHistory.Cursor = Cursors.Hand
        Me.btnMenuHistory.FlatStyle = FlatStyle.Flat
        Me.btnMenuHistory.Font = New Font("Segoe UI", 10.5!)
        Me.btnMenuHistory.ForeColor = Color.FromArgb(205, 205, 205)
        Me.btnMenuHistory.Location = New Point(14, 388)
        Me.btnMenuHistory.Padding = New Padding(14, 0, 0, 0)
        Me.btnMenuHistory.Size = New Size(212, 46)
        Me.btnMenuHistory.Tag = "Rental History"
        Me.btnMenuHistory.Text = "🕘   Rental History"
        Me.btnMenuHistory.TextAlign = ContentAlignment.MiddleLeft
        Me.btnMenuHistory.UseMnemonic = False
        Me.btnMenuHistory.UseVisualStyleBackColor = False
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
        Me.lblAvatar.Location = New Point(22, 22)
        Me.lblAvatar.Size = New Size(42, 42)
        Me.lblAvatar.Text = "JS"
        Me.lblAvatar.TextAlign = ContentAlignment.MiddleCenter
        Me.lblAvatar.UseMnemonic = False
        '
        'lblUserName
        '
        Me.lblUserName.AutoSize = True
        Me.lblUserName.BackColor = Color.Transparent
        Me.lblUserName.Font = New Font("Segoe UI", 10.5!, FontStyle.Bold)
        Me.lblUserName.ForeColor = Color.White
        Me.lblUserName.Location = New Point(74, 24)
        Me.lblUserName.Text = "Julian Sterling"
        Me.lblUserName.UseMnemonic = False
        '
        'lblUserRole
        '
        Me.lblUserRole.AutoSize = True
        Me.lblUserRole.BackColor = Color.Transparent
        Me.lblUserRole.Font = New Font("Segoe UI", 7.5!)
        Me.lblUserRole.ForeColor = Color.FromArgb(170, 170, 170)
        Me.lblUserRole.Location = New Point(75, 46)
        Me.lblUserRole.Text = "DISPATCH LEAD"
        Me.lblUserRole.UseMnemonic = False
        '
        'lblSettings
        '
        Me.lblSettings.AutoSize = True
        Me.lblSettings.BackColor = Color.Transparent
        Me.lblSettings.Cursor = Cursors.Hand
        Me.lblSettings.Font = New Font("Segoe UI", 9.0!)
        Me.lblSettings.ForeColor = Color.FromArgb(200, 200, 200)
        Me.lblSettings.Location = New Point(22, 78)
        Me.lblSettings.Text = "⚙  Settings"
        Me.lblSettings.UseMnemonic = False
        '
        'lblLogout
        '
        Me.lblLogout.AutoSize = True
        Me.lblLogout.BackColor = Color.Transparent
        Me.lblLogout.Cursor = Cursors.Hand
        Me.lblLogout.Font = New Font("Segoe UI", 9.0!)
        Me.lblLogout.ForeColor = Color.FromArgb(224, 90, 100)
        Me.lblLogout.Location = New Point(22, 98)
        Me.lblLogout.Text = "➜  Logout"
        Me.lblLogout.UseMnemonic = False
        '
        'pnlContent
        '
        Me.pnlContent.Dock = DockStyle.Fill
        Me.pnlContent.BackColor = Color.FromArgb(245, 244, 242)
        Me.pnlContent.Location = New Point(0, 76)
        Me.pnlContent.Name = "pnlContent"
        Me.pnlContent.Size = New Size(1160, 784)
        '
        'pnlTopBar
        '
        Me.pnlTopBar.Dock = DockStyle.Top
        Me.pnlTopBar.BackColor = Color.White
        Me.pnlTopBar.Location = New Point(0, 0)
        Me.pnlTopBar.Name = "pnlTopBar"
        Me.pnlTopBar.Size = New Size(1160, 76)
        Me.pnlTopBar.Controls.Add(Me.pnlTopLine)
        Me.pnlTopBar.Controls.Add(Me.lblCrumbCaption)
        Me.pnlTopBar.Controls.Add(Me.lblPageTitle)
        Me.pnlTopBar.Controls.Add(Me.pnlSearch)
        Me.pnlTopBar.Controls.Add(Me.lblDay)
        Me.pnlTopBar.Controls.Add(Me.lblDate)
        Me.pnlTopBar.Controls.Add(Me.lblBell)
        Me.pnlTopBar.Controls.Add(Me.lblBadge)
        Me.pnlTopBar.Controls.Add(Me.lblTopAvatar)
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
        Me.lblCrumbCaption.Location = New Point(24, 12)
        Me.lblCrumbCaption.Text = "HOME / DASHBOARD"
        Me.lblCrumbCaption.UseMnemonic = False
        '
        'lblPageTitle
        '
        Me.lblPageTitle.AutoSize = True
        Me.lblPageTitle.BackColor = Color.Transparent
        Me.lblPageTitle.Font = New Font("Segoe UI", 17.0!, FontStyle.Bold)
        Me.lblPageTitle.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblPageTitle.Location = New Point(22, 26)
        Me.lblPageTitle.Text = "Front-Desk Dashboard"
        Me.lblPageTitle.UseMnemonic = False
        '
        'pnlSearch
        '
        Me.pnlSearch.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.pnlSearch.BackColor = Color.White
        Me.pnlSearch.Location = New Point(600, 18)
        Me.pnlSearch.Name = "pnlSearch"
        Me.pnlSearch.Size = New Size(300, 40)
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
        Me.lblSearchIcon.Size = New Size(30, 38)
        Me.lblSearchIcon.Text = "🔍"
        Me.lblSearchIcon.TextAlign = ContentAlignment.MiddleCenter
        Me.lblSearchIcon.UseMnemonic = False
        '
        'txtSearch
        '
        Me.txtSearch.BackColor = Color.White
        Me.txtSearch.BorderStyle = BorderStyle.None
        Me.txtSearch.Font = New Font("Segoe UI", 9.5!)
        Me.txtSearch.Location = New Point(42, 10)
        Me.txtSearch.Name = "txtSearch"
        Me.txtSearch.PlaceholderText = "Search Reservations, Fleet..."
        Me.txtSearch.Size = New Size(248, 18)
        '
        'lblDay
        '
        Me.lblDay.AutoSize = True
        Me.lblDay.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.lblDay.BackColor = Color.Transparent
        Me.lblDay.Font = New Font("Segoe UI", 7.5!)
        Me.lblDay.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblDay.Location = New Point(930, 20)
        Me.lblDay.Text = "TUESDAY"
        Me.lblDay.UseMnemonic = False
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.lblDate.BackColor = Color.Transparent
        Me.lblDate.Font = New Font("Segoe UI", 10.5!, FontStyle.Bold)
        Me.lblDate.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblDate.Location = New Point(930, 36)
        Me.lblDate.Text = "Oct 24, 2023"
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
        Me.lblBell.Location = New Point(1050, 26)
        Me.lblBell.Text = "🔔"
        Me.lblBell.UseMnemonic = False
        '
        'lblBadge
        '
        Me.lblBadge.AutoSize = False
        Me.lblBadge.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.lblBadge.BackColor = Color.FromArgb(122, 0, 16)
        Me.lblBadge.Font = New Font("Segoe UI", 7.0!, FontStyle.Bold)
        Me.lblBadge.ForeColor = Color.White
        Me.lblBadge.Location = New Point(1068, 20)
        Me.lblBadge.Size = New Size(16, 16)
        Me.lblBadge.Text = "3"
        Me.lblBadge.TextAlign = ContentAlignment.MiddleCenter
        Me.lblBadge.UseMnemonic = False
        '
        'lblTopAvatar
        '
        Me.lblTopAvatar.AutoSize = False
        Me.lblTopAvatar.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.lblTopAvatar.BackColor = Color.FromArgb(250, 228, 230)
        Me.lblTopAvatar.Font = New Font("Segoe UI", 9.5!, FontStyle.Bold)
        Me.lblTopAvatar.ForeColor = Color.FromArgb(122, 0, 16)
        Me.lblTopAvatar.Location = New Point(1104, 16)
        Me.lblTopAvatar.Size = New Size(42, 42)
        Me.lblTopAvatar.Text = "JS"
        Me.lblTopAvatar.TextAlign = ContentAlignment.MiddleCenter
        Me.lblTopAvatar.UseMnemonic = False
        '
        'FrontDeskDashboard
        '
        Me.AutoScaleDimensions = New SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.BackColor = Color.FromArgb(245, 244, 242)
        Me.ClientSize = New Size(1400, 860)
        Me.Font = New Font("Segoe UI", 9.0!)
        Me.MinimumSize = New Size(1100, 700)
        Me.Name = "FrontDeskDashboard"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Text = "AutoRent - Front-Desk"
        Me.Controls.Add(Me.pnlMain)
        Me.Controls.Add(Me.pnlSidebar)
        Me.pnlSidebar.ResumeLayout(False)
        Me.pnlMain.ResumeLayout(False)
        Me.pnlMenu.ResumeLayout(False)
        Me.pnlUserBox.ResumeLayout(False)
        Me.pnlBrand.ResumeLayout(False)
        Me.picLogo.ResumeLayout(False)
        Me.pnlActiveBar.ResumeLayout(False)
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
        Me.pnlActiveBar.PerformLayout()
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
    Friend WithEvents pnlActiveBar As Panel
    Friend WithEvents btnMenuDashboard As Button
    Friend WithEvents btnMenuFleet As Button
    Friend WithEvents btnMenuCustomers As Button
    Friend WithEvents btnMenuReservations As Button
    Friend WithEvents btnMenuDispatch As Button
    Friend WithEvents btnMenuInspection As Button
    Friend WithEvents btnMenuBilling As Button
    Friend WithEvents btnMenuHistory As Button
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
    Friend WithEvents lblPageTitle As Label
    Friend WithEvents pnlSearch As CardPanel
    Friend WithEvents lblSearchIcon As Label
    Friend WithEvents txtSearch As TextBox
    Friend WithEvents lblDay As Label
    Friend WithEvents lblDate As Label
    Friend WithEvents lblBell As Label
    Friend WithEvents lblBadge As Label
    Friend WithEvents lblTopAvatar As Label
End Class
