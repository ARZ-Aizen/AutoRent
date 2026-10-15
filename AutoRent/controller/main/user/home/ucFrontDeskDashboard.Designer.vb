<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ucFrontDeskDashboard
    Inherits System.Windows.Forms.UserControl

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
        Dim dgvHeaderStyle As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim dgvCellStyle As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim dgvIdStyle As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        dgvHeaderStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        dgvHeaderStyle.BackColor = Color.FromArgb(250, 249, 247)
        dgvHeaderStyle.Font = New Font("Segoe UI", 7.5!, FontStyle.Bold)
        dgvHeaderStyle.ForeColor = Color.FromArgb(100, 100, 100)
        dgvHeaderStyle.Padding = New Padding(12, 0, 0, 0)
        dgvHeaderStyle.SelectionBackColor = Color.FromArgb(250, 249, 247)
        dgvHeaderStyle.SelectionForeColor = Color.FromArgb(100, 100, 100)
        dgvHeaderStyle.WrapMode = DataGridViewTriState.[False]
        dgvCellStyle.BackColor = Color.White
        dgvCellStyle.ForeColor = Color.FromArgb(50, 50, 50)
        dgvCellStyle.Padding = New Padding(12, 0, 0, 0)
        dgvCellStyle.SelectionBackColor = Color.FromArgb(252, 245, 246)
        dgvCellStyle.SelectionForeColor = Color.FromArgb(50, 50, 50)
        dgvIdStyle.BackColor = Color.White
        dgvIdStyle.Font = New Font("Segoe UI", 8.5!, FontStyle.Bold)
        dgvIdStyle.ForeColor = Color.FromArgb(122, 0, 16)
        dgvIdStyle.Padding = New Padding(12, 0, 0, 0)
        dgvIdStyle.SelectionBackColor = Color.FromArgb(252, 245, 246)
        dgvIdStyle.SelectionForeColor = Color.FromArgb(122, 0, 16)
        Me.tlpKpi = New System.Windows.Forms.TableLayoutPanel()
        Me.cardKpi1 = New CardPanel()
        Me.pnlKpiIcon1 = New System.Windows.Forms.Panel()
        Me.lblKpiIcon1 = New System.Windows.Forms.Label()
        Me.lblKpiBadge1 = New System.Windows.Forms.Label()
        Me.lblKpiValue1 = New System.Windows.Forms.Label()
        Me.lblKpiCaption1 = New System.Windows.Forms.Label()
        Me.cardKpi2 = New CardPanel()
        Me.pnlKpiIcon2 = New System.Windows.Forms.Panel()
        Me.lblKpiIcon2 = New System.Windows.Forms.Label()
        Me.lblKpiBadge2 = New System.Windows.Forms.Label()
        Me.lblKpiValue2 = New System.Windows.Forms.Label()
        Me.lblKpiCaption2 = New System.Windows.Forms.Label()
        Me.cardKpi3 = New CardPanel()
        Me.pnlKpiIcon3 = New System.Windows.Forms.Panel()
        Me.lblKpiIcon3 = New System.Windows.Forms.Label()
        Me.lblKpiBadge3 = New System.Windows.Forms.Label()
        Me.lblKpiValue3 = New System.Windows.Forms.Label()
        Me.lblKpiCaption3 = New System.Windows.Forms.Label()
        Me.cardKpi4 = New CardPanel()
        Me.pnlKpiIcon4 = New System.Windows.Forms.Panel()
        Me.lblKpiIcon4 = New System.Windows.Forms.Label()
        Me.lblKpiBadge4 = New System.Windows.Forms.Label()
        Me.lblKpiValue4 = New System.Windows.Forms.Label()
        Me.lblKpiCaption4 = New System.Windows.Forms.Label()
        Me.tlpMid = New System.Windows.Forms.TableLayoutPanel()
        Me.cardFleet = New CardPanel()
        Me.cardReturns = New CardPanel()
        Me.lblFleetTitle = New System.Windows.Forms.Label()
        Me.lblViewInventory = New System.Windows.Forms.Label()
        Me.pnlFleetLine = New System.Windows.Forms.Panel()
        Me.tlpCats = New System.Windows.Forms.TableLayoutPanel()
        Me.cardCat1 = New CardPanel()
        Me.lblCatIcon1 = New System.Windows.Forms.Label()
        Me.lblCatTitle1 = New System.Windows.Forms.Label()
        Me.lblCatAvailName1 = New System.Windows.Forms.Label()
        Me.lblCatAvail1 = New System.Windows.Forms.Label()
        Me.pnlCatBar1 = New System.Windows.Forms.Panel()
        Me.lblCatRes1 = New System.Windows.Forms.Label()
        Me.lblCatRent1 = New System.Windows.Forms.Label()
        Me.lblCatMaint1 = New System.Windows.Forms.Label()
        Me.cardCat2 = New CardPanel()
        Me.lblCatIcon2 = New System.Windows.Forms.Label()
        Me.lblCatTitle2 = New System.Windows.Forms.Label()
        Me.lblCatAvailName2 = New System.Windows.Forms.Label()
        Me.lblCatAvail2 = New System.Windows.Forms.Label()
        Me.pnlCatBar2 = New System.Windows.Forms.Panel()
        Me.lblCatRes2 = New System.Windows.Forms.Label()
        Me.lblCatRent2 = New System.Windows.Forms.Label()
        Me.lblCatMaint2 = New System.Windows.Forms.Label()
        Me.cardCat3 = New CardPanel()
        Me.lblCatIcon3 = New System.Windows.Forms.Label()
        Me.lblCatTitle3 = New System.Windows.Forms.Label()
        Me.lblCatAvailName3 = New System.Windows.Forms.Label()
        Me.lblCatAvail3 = New System.Windows.Forms.Label()
        Me.pnlCatBar3 = New System.Windows.Forms.Panel()
        Me.lblCatRes3 = New System.Windows.Forms.Label()
        Me.lblCatRent3 = New System.Windows.Forms.Label()
        Me.lblCatMaint3 = New System.Windows.Forms.Label()
        Me.cardCat4 = New CardPanel()
        Me.lblCatIcon4 = New System.Windows.Forms.Label()
        Me.lblCatTitle4 = New System.Windows.Forms.Label()
        Me.lblCatAvailName4 = New System.Windows.Forms.Label()
        Me.lblCatAvail4 = New System.Windows.Forms.Label()
        Me.pnlCatBar4 = New System.Windows.Forms.Panel()
        Me.lblCatRes4 = New System.Windows.Forms.Label()
        Me.lblCatRent4 = New System.Windows.Forms.Label()
        Me.lblCatMaint4 = New System.Windows.Forms.Label()
        Me.lblReturnsTitle = New System.Windows.Forms.Label()
        Me.pnlReturnsLine = New System.Windows.Forms.Panel()
        Me.pnlRet1 = New System.Windows.Forms.Panel()
        Me.pnlRetLine1 = New System.Windows.Forms.Panel()
        Me.lblRetIcon1 = New System.Windows.Forms.Label()
        Me.lblRetName1 = New System.Windows.Forms.Label()
        Me.lblRetSub1 = New System.Windows.Forms.Label()
        Me.lblRetEta1 = New System.Windows.Forms.Label()
        Me.pnlRet2 = New System.Windows.Forms.Panel()
        Me.pnlRetLine2 = New System.Windows.Forms.Panel()
        Me.lblRetIcon2 = New System.Windows.Forms.Label()
        Me.lblRetName2 = New System.Windows.Forms.Label()
        Me.lblRetSub2 = New System.Windows.Forms.Label()
        Me.lblRetEta2 = New System.Windows.Forms.Label()
        Me.pnlRet3 = New System.Windows.Forms.Panel()
        Me.pnlRetLine3 = New System.Windows.Forms.Panel()
        Me.lblRetIcon3 = New System.Windows.Forms.Label()
        Me.lblRetName3 = New System.Windows.Forms.Label()
        Me.lblRetSub3 = New System.Windows.Forms.Label()
        Me.lblRetEta3 = New System.Windows.Forms.Label()
        Me.pnlRet4 = New System.Windows.Forms.Panel()
        Me.pnlRetLine4 = New System.Windows.Forms.Panel()
        Me.lblRetIcon4 = New System.Windows.Forms.Label()
        Me.lblRetName4 = New System.Windows.Forms.Label()
        Me.lblRetSub4 = New System.Windows.Forms.Label()
        Me.lblRetEta4 = New System.Windows.Forms.Label()
        Me.cardDispatch = New CardPanel()
        Me.dgvDispatch = New System.Windows.Forms.DataGridView()
        Me.colBookingId = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colCustomer = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colVehicle = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDispatch = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colReturn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colStatus = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colAction = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.pnlDispFoot = New System.Windows.Forms.Panel()
        Me.pnlFootLine = New System.Windows.Forms.Panel()
        Me.lblShowing = New System.Windows.Forms.Label()
        Me.btnPrev = New System.Windows.Forms.Button()
        Me.btnPage1 = New System.Windows.Forms.Button()
        Me.btnPage2 = New System.Windows.Forms.Button()
        Me.btnPage3 = New System.Windows.Forms.Button()
        Me.btnNext = New System.Windows.Forms.Button()
        Me.pnlDispHead = New System.Windows.Forms.Panel()
        Me.lblDispTitle = New System.Windows.Forms.Label()
        Me.pnlDispTabs = New System.Windows.Forms.Panel()
        Me.btnTabPending = New System.Windows.Forms.Button()
        Me.btnTabActive = New System.Windows.Forms.Button()
        Me.btnTabCompleted = New System.Windows.Forms.Button()
        Me.btnNewBooking = New System.Windows.Forms.Button()
        CType(Me.dgvDispatch, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tlpKpi.SuspendLayout()
        Me.cardKpi1.SuspendLayout()
        Me.pnlKpiIcon1.SuspendLayout()
        Me.cardKpi2.SuspendLayout()
        Me.pnlKpiIcon2.SuspendLayout()
        Me.cardKpi3.SuspendLayout()
        Me.pnlKpiIcon3.SuspendLayout()
        Me.cardKpi4.SuspendLayout()
        Me.pnlKpiIcon4.SuspendLayout()
        Me.tlpMid.SuspendLayout()
        Me.cardFleet.SuspendLayout()
        Me.cardReturns.SuspendLayout()
        Me.pnlFleetLine.SuspendLayout()
        Me.tlpCats.SuspendLayout()
        Me.cardCat1.SuspendLayout()
        Me.pnlCatBar1.SuspendLayout()
        Me.cardCat2.SuspendLayout()
        Me.pnlCatBar2.SuspendLayout()
        Me.cardCat3.SuspendLayout()
        Me.pnlCatBar3.SuspendLayout()
        Me.cardCat4.SuspendLayout()
        Me.pnlCatBar4.SuspendLayout()
        Me.pnlReturnsLine.SuspendLayout()
        Me.pnlRet1.SuspendLayout()
        Me.pnlRetLine1.SuspendLayout()
        Me.pnlRet2.SuspendLayout()
        Me.pnlRetLine2.SuspendLayout()
        Me.pnlRet3.SuspendLayout()
        Me.pnlRetLine3.SuspendLayout()
        Me.pnlRet4.SuspendLayout()
        Me.pnlRetLine4.SuspendLayout()
        Me.cardDispatch.SuspendLayout()
        Me.pnlDispFoot.SuspendLayout()
        Me.pnlFootLine.SuspendLayout()
        Me.pnlDispHead.SuspendLayout()
        Me.pnlDispTabs.SuspendLayout()
        Me.SuspendLayout()
        '
        'tlpKpi
        '
        Me.tlpKpi.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25.0!))
        Me.tlpKpi.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25.0!))
        Me.tlpKpi.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25.0!))
        Me.tlpKpi.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25.0!))
        Me.tlpKpi.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0!))
        Me.tlpKpi.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.tlpKpi.ColumnCount = 4
        Me.tlpKpi.Location = New Point(18, 20)
        Me.tlpKpi.Name = "tlpKpi"
        Me.tlpKpi.RowCount = 1
        Me.tlpKpi.Size = New Size(1124, 156)
        Me.tlpKpi.Controls.Add(Me.cardKpi1, 0, 0)
        Me.tlpKpi.Controls.Add(Me.cardKpi2, 1, 0)
        Me.tlpKpi.Controls.Add(Me.cardKpi3, 2, 0)
        Me.tlpKpi.Controls.Add(Me.cardKpi4, 3, 0)
        '
        'cardKpi1
        '
        Me.cardKpi1.BackColor = Color.White
        Me.cardKpi1.Dock = DockStyle.Fill
        Me.cardKpi1.Margin = New Padding(6, 0, 6, 0)
        Me.cardKpi1.Name = "cardKpi1"
        Me.cardKpi1.Size = New Size(268, 156)
        Me.cardKpi1.Controls.Add(Me.pnlKpiIcon1)
        Me.cardKpi1.Controls.Add(Me.lblKpiBadge1)
        Me.cardKpi1.Controls.Add(Me.lblKpiValue1)
        Me.cardKpi1.Controls.Add(Me.lblKpiCaption1)
        '
        'pnlKpiIcon1
        '
        Me.pnlKpiIcon1.BackColor = Color.FromArgb(248, 230, 232)
        Me.pnlKpiIcon1.Location = New Point(22, 18)
        Me.pnlKpiIcon1.Name = "pnlKpiIcon1"
        Me.pnlKpiIcon1.Size = New Size(48, 48)
        Me.pnlKpiIcon1.Controls.Add(Me.lblKpiIcon1)
        '
        'lblKpiIcon1
        '
        Me.lblKpiIcon1.AutoSize = False
        Me.lblKpiIcon1.BackColor = Color.Transparent
        Me.lblKpiIcon1.Font = New Font("Segoe UI Emoji", 15.0!)
        Me.lblKpiIcon1.ForeColor = Color.FromArgb(122, 0, 16)
        Me.lblKpiIcon1.Location = New Point(0, 0)
        Me.lblKpiIcon1.Size = New Size(48, 48)
        Me.lblKpiIcon1.Text = "🚗"
        Me.lblKpiIcon1.TextAlign = ContentAlignment.MiddleCenter
        Me.lblKpiIcon1.UseMnemonic = False
        '
        'lblKpiBadge1
        '
        Me.lblKpiBadge1.AutoSize = False
        Me.lblKpiBadge1.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.lblKpiBadge1.BackColor = Color.FromArgb(228, 244, 234)
        Me.lblKpiBadge1.Font = New Font("Segoe UI", 7.5!, FontStyle.Bold)
        Me.lblKpiBadge1.ForeColor = Color.FromArgb(30, 130, 70)
        Me.lblKpiBadge1.Location = New Point(166, 20)
        Me.lblKpiBadge1.Size = New Size(80, 22)
        Me.lblKpiBadge1.Text = "+4.2%"
        Me.lblKpiBadge1.TextAlign = ContentAlignment.MiddleCenter
        Me.lblKpiBadge1.UseMnemonic = False
        '
        'lblKpiValue1
        '
        Me.lblKpiValue1.AutoSize = True
        Me.lblKpiValue1.BackColor = Color.Transparent
        Me.lblKpiValue1.Font = New Font("Segoe UI", 28.0!, FontStyle.Bold)
        Me.lblKpiValue1.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblKpiValue1.Location = New Point(20, 74)
        Me.lblKpiValue1.Text = "42"
        Me.lblKpiValue1.UseMnemonic = False
        '
        'lblKpiCaption1
        '
        Me.lblKpiCaption1.AutoSize = True
        Me.lblKpiCaption1.BackColor = Color.Transparent
        Me.lblKpiCaption1.Font = New Font("Segoe UI", 9.5!)
        Me.lblKpiCaption1.ForeColor = Color.FromArgb(130, 130, 130)
        Me.lblKpiCaption1.Location = New Point(22, 122)
        Me.lblKpiCaption1.Text = "AVAILABLE VEHICLES"
        Me.lblKpiCaption1.UseMnemonic = False
        '
        'cardKpi2
        '
        Me.cardKpi2.BackColor = Color.White
        Me.cardKpi2.Dock = DockStyle.Fill
        Me.cardKpi2.Margin = New Padding(6, 0, 6, 0)
        Me.cardKpi2.Name = "cardKpi2"
        Me.cardKpi2.Size = New Size(268, 156)
        Me.cardKpi2.Controls.Add(Me.pnlKpiIcon2)
        Me.cardKpi2.Controls.Add(Me.lblKpiBadge2)
        Me.cardKpi2.Controls.Add(Me.lblKpiValue2)
        Me.cardKpi2.Controls.Add(Me.lblKpiCaption2)
        '
        'pnlKpiIcon2
        '
        Me.pnlKpiIcon2.BackColor = Color.FromArgb(240, 240, 240)
        Me.pnlKpiIcon2.Location = New Point(22, 18)
        Me.pnlKpiIcon2.Name = "pnlKpiIcon2"
        Me.pnlKpiIcon2.Size = New Size(48, 48)
        Me.pnlKpiIcon2.Controls.Add(Me.lblKpiIcon2)
        '
        'lblKpiIcon2
        '
        Me.lblKpiIcon2.AutoSize = False
        Me.lblKpiIcon2.BackColor = Color.Transparent
        Me.lblKpiIcon2.Font = New Font("Segoe UI Emoji", 15.0!)
        Me.lblKpiIcon2.ForeColor = Color.FromArgb(122, 0, 16)
        Me.lblKpiIcon2.Location = New Point(0, 0)
        Me.lblKpiIcon2.Size = New Size(48, 48)
        Me.lblKpiIcon2.Text = "🔑"
        Me.lblKpiIcon2.TextAlign = ContentAlignment.MiddleCenter
        Me.lblKpiIcon2.UseMnemonic = False
        '
        'lblKpiBadge2
        '
        Me.lblKpiBadge2.AutoSize = False
        Me.lblKpiBadge2.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.lblKpiBadge2.BackColor = Color.FromArgb(240, 240, 240)
        Me.lblKpiBadge2.Font = New Font("Segoe UI", 7.5!, FontStyle.Bold)
        Me.lblKpiBadge2.ForeColor = Color.FromArgb(110, 110, 110)
        Me.lblKpiBadge2.Location = New Point(166, 20)
        Me.lblKpiBadge2.Size = New Size(80, 22)
        Me.lblKpiBadge2.Text = "Stable"
        Me.lblKpiBadge2.TextAlign = ContentAlignment.MiddleCenter
        Me.lblKpiBadge2.UseMnemonic = False
        '
        'lblKpiValue2
        '
        Me.lblKpiValue2.AutoSize = True
        Me.lblKpiValue2.BackColor = Color.Transparent
        Me.lblKpiValue2.Font = New Font("Segoe UI", 28.0!, FontStyle.Bold)
        Me.lblKpiValue2.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblKpiValue2.Location = New Point(20, 74)
        Me.lblKpiValue2.Text = "128"
        Me.lblKpiValue2.UseMnemonic = False
        '
        'lblKpiCaption2
        '
        Me.lblKpiCaption2.AutoSize = True
        Me.lblKpiCaption2.BackColor = Color.Transparent
        Me.lblKpiCaption2.Font = New Font("Segoe UI", 9.5!)
        Me.lblKpiCaption2.ForeColor = Color.FromArgb(130, 130, 130)
        Me.lblKpiCaption2.Location = New Point(22, 122)
        Me.lblKpiCaption2.Text = "ACTIVE RENTALS"
        Me.lblKpiCaption2.UseMnemonic = False
        '
        'cardKpi3
        '
        Me.cardKpi3.BackColor = Color.White
        Me.cardKpi3.Dock = DockStyle.Fill
        Me.cardKpi3.Margin = New Padding(6, 0, 6, 0)
        Me.cardKpi3.Name = "cardKpi3"
        Me.cardKpi3.Size = New Size(268, 156)
        Me.cardKpi3.Controls.Add(Me.pnlKpiIcon3)
        Me.cardKpi3.Controls.Add(Me.lblKpiBadge3)
        Me.cardKpi3.Controls.Add(Me.lblKpiValue3)
        Me.cardKpi3.Controls.Add(Me.lblKpiCaption3)
        '
        'pnlKpiIcon3
        '
        Me.pnlKpiIcon3.BackColor = Color.FromArgb(253, 238, 225)
        Me.pnlKpiIcon3.Location = New Point(22, 18)
        Me.pnlKpiIcon3.Name = "pnlKpiIcon3"
        Me.pnlKpiIcon3.Size = New Size(48, 48)
        Me.pnlKpiIcon3.Controls.Add(Me.lblKpiIcon3)
        '
        'lblKpiIcon3
        '
        Me.lblKpiIcon3.AutoSize = False
        Me.lblKpiIcon3.BackColor = Color.Transparent
        Me.lblKpiIcon3.Font = New Font("Segoe UI Emoji", 15.0!)
        Me.lblKpiIcon3.ForeColor = Color.FromArgb(122, 0, 16)
        Me.lblKpiIcon3.Location = New Point(0, 0)
        Me.lblKpiIcon3.Size = New Size(48, 48)
        Me.lblKpiIcon3.Text = "📅"
        Me.lblKpiIcon3.TextAlign = ContentAlignment.MiddleCenter
        Me.lblKpiIcon3.UseMnemonic = False
        '
        'lblKpiBadge3
        '
        Me.lblKpiBadge3.AutoSize = False
        Me.lblKpiBadge3.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.lblKpiBadge3.BackColor = Color.FromArgb(253, 238, 225)
        Me.lblKpiBadge3.Font = New Font("Segoe UI", 7.5!, FontStyle.Bold)
        Me.lblKpiBadge3.ForeColor = Color.FromArgb(210, 90, 0)
        Me.lblKpiBadge3.Location = New Point(166, 20)
        Me.lblKpiBadge3.Size = New Size(80, 22)
        Me.lblKpiBadge3.Text = "High Traffic"
        Me.lblKpiBadge3.TextAlign = ContentAlignment.MiddleCenter
        Me.lblKpiBadge3.UseMnemonic = False
        '
        'lblKpiValue3
        '
        Me.lblKpiValue3.AutoSize = True
        Me.lblKpiValue3.BackColor = Color.Transparent
        Me.lblKpiValue3.Font = New Font("Segoe UI", 28.0!, FontStyle.Bold)
        Me.lblKpiValue3.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblKpiValue3.Location = New Point(20, 74)
        Me.lblKpiValue3.Text = "15"
        Me.lblKpiValue3.UseMnemonic = False
        '
        'lblKpiCaption3
        '
        Me.lblKpiCaption3.AutoSize = True
        Me.lblKpiCaption3.BackColor = Color.Transparent
        Me.lblKpiCaption3.Font = New Font("Segoe UI", 9.5!)
        Me.lblKpiCaption3.ForeColor = Color.FromArgb(130, 130, 130)
        Me.lblKpiCaption3.Location = New Point(22, 122)
        Me.lblKpiCaption3.Text = "TODAY'S BOOKINGS"
        Me.lblKpiCaption3.UseMnemonic = False
        '
        'cardKpi4
        '
        Me.cardKpi4.BackColor = Color.White
        Me.cardKpi4.Dock = DockStyle.Fill
        Me.cardKpi4.Margin = New Padding(6, 0, 6, 0)
        Me.cardKpi4.Name = "cardKpi4"
        Me.cardKpi4.Size = New Size(268, 156)
        Me.cardKpi4.Controls.Add(Me.pnlKpiIcon4)
        Me.cardKpi4.Controls.Add(Me.lblKpiBadge4)
        Me.cardKpi4.Controls.Add(Me.lblKpiValue4)
        Me.cardKpi4.Controls.Add(Me.lblKpiCaption4)
        '
        'pnlKpiIcon4
        '
        Me.pnlKpiIcon4.BackColor = Color.FromArgb(232, 240, 252)
        Me.pnlKpiIcon4.Location = New Point(22, 18)
        Me.pnlKpiIcon4.Name = "pnlKpiIcon4"
        Me.pnlKpiIcon4.Size = New Size(48, 48)
        Me.pnlKpiIcon4.Controls.Add(Me.lblKpiIcon4)
        '
        'lblKpiIcon4
        '
        Me.lblKpiIcon4.AutoSize = False
        Me.lblKpiIcon4.BackColor = Color.Transparent
        Me.lblKpiIcon4.Font = New Font("Segoe UI Emoji", 15.0!)
        Me.lblKpiIcon4.ForeColor = Color.FromArgb(122, 0, 16)
        Me.lblKpiIcon4.Location = New Point(0, 0)
        Me.lblKpiIcon4.Size = New Size(48, 48)
        Me.lblKpiIcon4.Text = "↻"
        Me.lblKpiIcon4.TextAlign = ContentAlignment.MiddleCenter
        Me.lblKpiIcon4.UseMnemonic = False
        '
        'lblKpiBadge4
        '
        Me.lblKpiBadge4.AutoSize = False
        Me.lblKpiBadge4.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.lblKpiBadge4.BackColor = Color.FromArgb(232, 240, 252)
        Me.lblKpiBadge4.Font = New Font("Segoe UI", 7.5!, FontStyle.Bold)
        Me.lblKpiBadge4.ForeColor = Color.FromArgb(30, 90, 200)
        Me.lblKpiBadge4.Location = New Point(166, 20)
        Me.lblKpiBadge4.Size = New Size(80, 22)
        Me.lblKpiBadge4.Text = "Pending"
        Me.lblKpiBadge4.TextAlign = ContentAlignment.MiddleCenter
        Me.lblKpiBadge4.UseMnemonic = False
        '
        'lblKpiValue4
        '
        Me.lblKpiValue4.AutoSize = True
        Me.lblKpiValue4.BackColor = Color.Transparent
        Me.lblKpiValue4.Font = New Font("Segoe UI", 28.0!, FontStyle.Bold)
        Me.lblKpiValue4.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblKpiValue4.Location = New Point(20, 74)
        Me.lblKpiValue4.Text = "08"
        Me.lblKpiValue4.UseMnemonic = False
        '
        'lblKpiCaption4
        '
        Me.lblKpiCaption4.AutoSize = True
        Me.lblKpiCaption4.BackColor = Color.Transparent
        Me.lblKpiCaption4.Font = New Font("Segoe UI", 9.5!)
        Me.lblKpiCaption4.ForeColor = Color.FromArgb(130, 130, 130)
        Me.lblKpiCaption4.Location = New Point(22, 122)
        Me.lblKpiCaption4.Text = "RETURNS DUE TODAY"
        Me.lblKpiCaption4.UseMnemonic = False
        '
        'tlpMid
        '
        Me.tlpMid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 66.0!))
        Me.tlpMid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 34.0!))
        Me.tlpMid.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0!))
        Me.tlpMid.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.tlpMid.ColumnCount = 2
        Me.tlpMid.Location = New Point(18, 196)
        Me.tlpMid.Name = "tlpMid"
        Me.tlpMid.RowCount = 1
        Me.tlpMid.Size = New Size(1124, 340)
        Me.tlpMid.Controls.Add(Me.cardFleet, 0, 0)
        Me.tlpMid.Controls.Add(Me.cardReturns, 1, 0)
        '
        'cardFleet
        '
        Me.cardFleet.BackColor = Color.White
        Me.cardFleet.Dock = DockStyle.Fill
        Me.cardFleet.Margin = New Padding(6, 0, 6, 0)
        Me.cardFleet.Name = "cardFleet"
        Me.cardFleet.Size = New Size(730, 340)
        Me.cardFleet.Controls.Add(Me.lblFleetTitle)
        Me.cardFleet.Controls.Add(Me.lblViewInventory)
        Me.cardFleet.Controls.Add(Me.pnlFleetLine)
        Me.cardFleet.Controls.Add(Me.tlpCats)
        '
        'cardReturns
        '
        Me.cardReturns.BackColor = Color.White
        Me.cardReturns.Dock = DockStyle.Fill
        Me.cardReturns.Margin = New Padding(6, 0, 6, 0)
        Me.cardReturns.Name = "cardReturns"
        Me.cardReturns.Size = New Size(370, 340)
        Me.cardReturns.Controls.Add(Me.lblReturnsTitle)
        Me.cardReturns.Controls.Add(Me.pnlReturnsLine)
        Me.cardReturns.Controls.Add(Me.pnlRet1)
        Me.cardReturns.Controls.Add(Me.pnlRet2)
        Me.cardReturns.Controls.Add(Me.pnlRet3)
        Me.cardReturns.Controls.Add(Me.pnlRet4)
        '
        'lblFleetTitle
        '
        Me.lblFleetTitle.AutoSize = True
        Me.lblFleetTitle.BackColor = Color.Transparent
        Me.lblFleetTitle.Font = New Font("Segoe UI", 10.5!, FontStyle.Bold)
        Me.lblFleetTitle.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblFleetTitle.Location = New Point(22, 22)
        Me.lblFleetTitle.Text = "FLEET AVAILABILITY OVERVIEW"
        Me.lblFleetTitle.UseMnemonic = False
        '
        'lblViewInventory
        '
        Me.lblViewInventory.AutoSize = True
        Me.lblViewInventory.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.lblViewInventory.BackColor = Color.Transparent
        Me.lblViewInventory.Cursor = Cursors.Hand
        Me.lblViewInventory.Font = New Font("Segoe UI", 8.5!, FontStyle.Bold)
        Me.lblViewInventory.ForeColor = Color.FromArgb(122, 0, 16)
        Me.lblViewInventory.Location = New Point(570, 25)
        Me.lblViewInventory.Text = "VIEW FULL INVENTORY"
        Me.lblViewInventory.UseMnemonic = False
        '
        'pnlFleetLine
        '
        Me.pnlFleetLine.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.pnlFleetLine.BackColor = Color.FromArgb(235, 232, 228)
        Me.pnlFleetLine.Location = New Point(1, 62)
        Me.pnlFleetLine.Name = "pnlFleetLine"
        Me.pnlFleetLine.Size = New Size(728, 1)
        '
        'tlpCats
        '
        Me.tlpCats.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25.0!))
        Me.tlpCats.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25.0!))
        Me.tlpCats.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25.0!))
        Me.tlpCats.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25.0!))
        Me.tlpCats.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0!))
        Me.tlpCats.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.tlpCats.ColumnCount = 4
        Me.tlpCats.Location = New Point(20, 86)
        Me.tlpCats.Name = "tlpCats"
        Me.tlpCats.RowCount = 1
        Me.tlpCats.Size = New Size(690, 170)
        Me.tlpCats.Controls.Add(Me.cardCat1, 0, 0)
        Me.tlpCats.Controls.Add(Me.cardCat2, 1, 0)
        Me.tlpCats.Controls.Add(Me.cardCat3, 2, 0)
        Me.tlpCats.Controls.Add(Me.cardCat4, 3, 0)
        '
        'cardCat1
        '
        Me.cardCat1.BackColor = Color.FromArgb(250, 249, 247)
        Me.cardCat1.Dock = DockStyle.Fill
        Me.cardCat1.Margin = New Padding(0, 0, 12, 0)
        Me.cardCat1.Name = "cardCat1"
        Me.cardCat1.Size = New Size(160, 170)
        Me.cardCat1.Controls.Add(Me.lblCatIcon1)
        Me.cardCat1.Controls.Add(Me.lblCatTitle1)
        Me.cardCat1.Controls.Add(Me.lblCatAvailName1)
        Me.cardCat1.Controls.Add(Me.lblCatAvail1)
        Me.cardCat1.Controls.Add(Me.pnlCatBar1)
        Me.cardCat1.Controls.Add(Me.lblCatRes1)
        Me.cardCat1.Controls.Add(Me.lblCatRent1)
        Me.cardCat1.Controls.Add(Me.lblCatMaint1)
        '
        'lblCatIcon1
        '
        Me.lblCatIcon1.AutoSize = True
        Me.lblCatIcon1.BackColor = Color.Transparent
        Me.lblCatIcon1.Font = New Font("Segoe UI Emoji", 12.0!)
        Me.lblCatIcon1.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblCatIcon1.Location = New Point(10, 14)
        Me.lblCatIcon1.Text = "🚗"
        Me.lblCatIcon1.UseMnemonic = False
        '
        'lblCatTitle1
        '
        Me.lblCatTitle1.AutoSize = False
        Me.lblCatTitle1.BackColor = Color.Transparent
        Me.lblCatTitle1.Font = New Font("Segoe UI", 9.5!)
        Me.lblCatTitle1.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblCatTitle1.Location = New Point(42, 12)
        Me.lblCatTitle1.Size = New Size(108, 38)
        Me.lblCatTitle1.Text = "Luxury Sedan"
        Me.lblCatTitle1.UseMnemonic = False
        '
        'lblCatAvailName1
        '
        Me.lblCatAvailName1.AutoSize = True
        Me.lblCatAvailName1.BackColor = Color.Transparent
        Me.lblCatAvailName1.Font = New Font("Segoe UI", 8.5!)
        Me.lblCatAvailName1.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblCatAvailName1.Location = New Point(14, 66)
        Me.lblCatAvailName1.Text = "Available"
        Me.lblCatAvailName1.UseMnemonic = False
        '
        'lblCatAvail1
        '
        Me.lblCatAvail1.AutoSize = False
        Me.lblCatAvail1.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.lblCatAvail1.BackColor = Color.Transparent
        Me.lblCatAvail1.Font = New Font("Segoe UI", 9.5!, FontStyle.Bold)
        Me.lblCatAvail1.ForeColor = Color.FromArgb(30, 130, 70)
        Me.lblCatAvail1.Location = New Point(104, 64)
        Me.lblCatAvail1.Size = New Size(42, 20)
        Me.lblCatAvail1.Text = "00"
        Me.lblCatAvail1.TextAlign = ContentAlignment.MiddleRight
        Me.lblCatAvail1.UseMnemonic = False
        '
        'pnlCatBar1
        '
        Me.pnlCatBar1.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.pnlCatBar1.BackColor = Color.FromArgb(250, 249, 247)
        Me.pnlCatBar1.Location = New Point(14, 92)
        Me.pnlCatBar1.Name = "pnlCatBar1"
        Me.pnlCatBar1.Size = New Size(132, 6)
        '
        'lblCatRes1
        '
        Me.lblCatRes1.AutoSize = True
        Me.lblCatRes1.BackColor = Color.Transparent
        Me.lblCatRes1.Font = New Font("Segoe UI", 7.5!)
        Me.lblCatRes1.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblCatRes1.Location = New Point(14, 114)
        Me.lblCatRes1.Text = "RES: 0"
        Me.lblCatRes1.UseMnemonic = False
        '
        'lblCatRent1
        '
        Me.lblCatRent1.AutoSize = True
        Me.lblCatRent1.BackColor = Color.Transparent
        Me.lblCatRent1.Font = New Font("Segoe UI", 7.5!)
        Me.lblCatRent1.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblCatRent1.Location = New Point(74, 114)
        Me.lblCatRent1.Text = "RENT: 0"
        Me.lblCatRent1.UseMnemonic = False
        '
        'lblCatMaint1
        '
        Me.lblCatMaint1.AutoSize = True
        Me.lblCatMaint1.BackColor = Color.Transparent
        Me.lblCatMaint1.Font = New Font("Segoe UI", 7.5!, FontStyle.Bold)
        Me.lblCatMaint1.ForeColor = Color.FromArgb(122, 0, 16)
        Me.lblCatMaint1.Location = New Point(14, 136)
        Me.lblCatMaint1.Text = "MAINT: 0"
        Me.lblCatMaint1.UseMnemonic = False
        '
        'cardCat2
        '
        Me.cardCat2.BackColor = Color.FromArgb(250, 249, 247)
        Me.cardCat2.Dock = DockStyle.Fill
        Me.cardCat2.Margin = New Padding(0, 0, 12, 0)
        Me.cardCat2.Name = "cardCat2"
        Me.cardCat2.Size = New Size(160, 170)
        Me.cardCat2.Controls.Add(Me.lblCatIcon2)
        Me.cardCat2.Controls.Add(Me.lblCatTitle2)
        Me.cardCat2.Controls.Add(Me.lblCatAvailName2)
        Me.cardCat2.Controls.Add(Me.lblCatAvail2)
        Me.cardCat2.Controls.Add(Me.pnlCatBar2)
        Me.cardCat2.Controls.Add(Me.lblCatRes2)
        Me.cardCat2.Controls.Add(Me.lblCatRent2)
        Me.cardCat2.Controls.Add(Me.lblCatMaint2)
        '
        'lblCatIcon2
        '
        Me.lblCatIcon2.AutoSize = True
        Me.lblCatIcon2.BackColor = Color.Transparent
        Me.lblCatIcon2.Font = New Font("Segoe UI Emoji", 12.0!)
        Me.lblCatIcon2.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblCatIcon2.Location = New Point(10, 14)
        Me.lblCatIcon2.Text = "🚙"
        Me.lblCatIcon2.UseMnemonic = False
        '
        'lblCatTitle2
        '
        Me.lblCatTitle2.AutoSize = False
        Me.lblCatTitle2.BackColor = Color.Transparent
        Me.lblCatTitle2.Font = New Font("Segoe UI", 9.5!)
        Me.lblCatTitle2.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblCatTitle2.Location = New Point(42, 12)
        Me.lblCatTitle2.Size = New Size(108, 38)
        Me.lblCatTitle2.Text = "Premium SUV"
        Me.lblCatTitle2.UseMnemonic = False
        '
        'lblCatAvailName2
        '
        Me.lblCatAvailName2.AutoSize = True
        Me.lblCatAvailName2.BackColor = Color.Transparent
        Me.lblCatAvailName2.Font = New Font("Segoe UI", 8.5!)
        Me.lblCatAvailName2.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblCatAvailName2.Location = New Point(14, 66)
        Me.lblCatAvailName2.Text = "Available"
        Me.lblCatAvailName2.UseMnemonic = False
        '
        'lblCatAvail2
        '
        Me.lblCatAvail2.AutoSize = False
        Me.lblCatAvail2.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.lblCatAvail2.BackColor = Color.Transparent
        Me.lblCatAvail2.Font = New Font("Segoe UI", 9.5!, FontStyle.Bold)
        Me.lblCatAvail2.ForeColor = Color.FromArgb(30, 130, 70)
        Me.lblCatAvail2.Location = New Point(104, 64)
        Me.lblCatAvail2.Size = New Size(42, 20)
        Me.lblCatAvail2.Text = "00"
        Me.lblCatAvail2.TextAlign = ContentAlignment.MiddleRight
        Me.lblCatAvail2.UseMnemonic = False
        '
        'pnlCatBar2
        '
        Me.pnlCatBar2.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.pnlCatBar2.BackColor = Color.FromArgb(250, 249, 247)
        Me.pnlCatBar2.Location = New Point(14, 92)
        Me.pnlCatBar2.Name = "pnlCatBar2"
        Me.pnlCatBar2.Size = New Size(132, 6)
        '
        'lblCatRes2
        '
        Me.lblCatRes2.AutoSize = True
        Me.lblCatRes2.BackColor = Color.Transparent
        Me.lblCatRes2.Font = New Font("Segoe UI", 7.5!)
        Me.lblCatRes2.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblCatRes2.Location = New Point(14, 114)
        Me.lblCatRes2.Text = "RES: 0"
        Me.lblCatRes2.UseMnemonic = False
        '
        'lblCatRent2
        '
        Me.lblCatRent2.AutoSize = True
        Me.lblCatRent2.BackColor = Color.Transparent
        Me.lblCatRent2.Font = New Font("Segoe UI", 7.5!)
        Me.lblCatRent2.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblCatRent2.Location = New Point(74, 114)
        Me.lblCatRent2.Text = "RENT: 0"
        Me.lblCatRent2.UseMnemonic = False
        '
        'lblCatMaint2
        '
        Me.lblCatMaint2.AutoSize = True
        Me.lblCatMaint2.BackColor = Color.Transparent
        Me.lblCatMaint2.Font = New Font("Segoe UI", 7.5!, FontStyle.Bold)
        Me.lblCatMaint2.ForeColor = Color.FromArgb(122, 0, 16)
        Me.lblCatMaint2.Location = New Point(14, 136)
        Me.lblCatMaint2.Text = "MAINT: 0"
        Me.lblCatMaint2.UseMnemonic = False
        '
        'cardCat3
        '
        Me.cardCat3.BackColor = Color.FromArgb(250, 249, 247)
        Me.cardCat3.Dock = DockStyle.Fill
        Me.cardCat3.Margin = New Padding(0, 0, 12, 0)
        Me.cardCat3.Name = "cardCat3"
        Me.cardCat3.Size = New Size(160, 170)
        Me.cardCat3.Controls.Add(Me.lblCatIcon3)
        Me.cardCat3.Controls.Add(Me.lblCatTitle3)
        Me.cardCat3.Controls.Add(Me.lblCatAvailName3)
        Me.cardCat3.Controls.Add(Me.lblCatAvail3)
        Me.cardCat3.Controls.Add(Me.pnlCatBar3)
        Me.cardCat3.Controls.Add(Me.lblCatRes3)
        Me.cardCat3.Controls.Add(Me.lblCatRent3)
        Me.cardCat3.Controls.Add(Me.lblCatMaint3)
        '
        'lblCatIcon3
        '
        Me.lblCatIcon3.AutoSize = True
        Me.lblCatIcon3.BackColor = Color.Transparent
        Me.lblCatIcon3.Font = New Font("Segoe UI Emoji", 12.0!)
        Me.lblCatIcon3.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblCatIcon3.Location = New Point(10, 14)
        Me.lblCatIcon3.Text = "🚐"
        Me.lblCatIcon3.UseMnemonic = False
        '
        'lblCatTitle3
        '
        Me.lblCatTitle3.AutoSize = False
        Me.lblCatTitle3.BackColor = Color.Transparent
        Me.lblCatTitle3.Font = New Font("Segoe UI", 9.5!)
        Me.lblCatTitle3.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblCatTitle3.Location = New Point(42, 12)
        Me.lblCatTitle3.Size = New Size(108, 38)
        Me.lblCatTitle3.Text = "VIP Van"
        Me.lblCatTitle3.UseMnemonic = False
        '
        'lblCatAvailName3
        '
        Me.lblCatAvailName3.AutoSize = True
        Me.lblCatAvailName3.BackColor = Color.Transparent
        Me.lblCatAvailName3.Font = New Font("Segoe UI", 8.5!)
        Me.lblCatAvailName3.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblCatAvailName3.Location = New Point(14, 66)
        Me.lblCatAvailName3.Text = "Available"
        Me.lblCatAvailName3.UseMnemonic = False
        '
        'lblCatAvail3
        '
        Me.lblCatAvail3.AutoSize = False
        Me.lblCatAvail3.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.lblCatAvail3.BackColor = Color.Transparent
        Me.lblCatAvail3.Font = New Font("Segoe UI", 9.5!, FontStyle.Bold)
        Me.lblCatAvail3.ForeColor = Color.FromArgb(30, 130, 70)
        Me.lblCatAvail3.Location = New Point(104, 64)
        Me.lblCatAvail3.Size = New Size(42, 20)
        Me.lblCatAvail3.Text = "00"
        Me.lblCatAvail3.TextAlign = ContentAlignment.MiddleRight
        Me.lblCatAvail3.UseMnemonic = False
        '
        'pnlCatBar3
        '
        Me.pnlCatBar3.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.pnlCatBar3.BackColor = Color.FromArgb(250, 249, 247)
        Me.pnlCatBar3.Location = New Point(14, 92)
        Me.pnlCatBar3.Name = "pnlCatBar3"
        Me.pnlCatBar3.Size = New Size(132, 6)
        '
        'lblCatRes3
        '
        Me.lblCatRes3.AutoSize = True
        Me.lblCatRes3.BackColor = Color.Transparent
        Me.lblCatRes3.Font = New Font("Segoe UI", 7.5!)
        Me.lblCatRes3.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblCatRes3.Location = New Point(14, 114)
        Me.lblCatRes3.Text = "RES: 0"
        Me.lblCatRes3.UseMnemonic = False
        '
        'lblCatRent3
        '
        Me.lblCatRent3.AutoSize = True
        Me.lblCatRent3.BackColor = Color.Transparent
        Me.lblCatRent3.Font = New Font("Segoe UI", 7.5!)
        Me.lblCatRent3.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblCatRent3.Location = New Point(74, 114)
        Me.lblCatRent3.Text = "RENT: 0"
        Me.lblCatRent3.UseMnemonic = False
        '
        'lblCatMaint3
        '
        Me.lblCatMaint3.AutoSize = True
        Me.lblCatMaint3.BackColor = Color.Transparent
        Me.lblCatMaint3.Font = New Font("Segoe UI", 7.5!, FontStyle.Bold)
        Me.lblCatMaint3.ForeColor = Color.FromArgb(122, 0, 16)
        Me.lblCatMaint3.Location = New Point(14, 136)
        Me.lblCatMaint3.Text = "MAINT: 0"
        Me.lblCatMaint3.UseMnemonic = False
        '
        'cardCat4
        '
        Me.cardCat4.BackColor = Color.FromArgb(250, 249, 247)
        Me.cardCat4.Dock = DockStyle.Fill
        Me.cardCat4.Margin = New Padding(0, 0, 12, 0)
        Me.cardCat4.Name = "cardCat4"
        Me.cardCat4.Size = New Size(160, 170)
        Me.cardCat4.Controls.Add(Me.lblCatIcon4)
        Me.cardCat4.Controls.Add(Me.lblCatTitle4)
        Me.cardCat4.Controls.Add(Me.lblCatAvailName4)
        Me.cardCat4.Controls.Add(Me.lblCatAvail4)
        Me.cardCat4.Controls.Add(Me.pnlCatBar4)
        Me.cardCat4.Controls.Add(Me.lblCatRes4)
        Me.cardCat4.Controls.Add(Me.lblCatRent4)
        Me.cardCat4.Controls.Add(Me.lblCatMaint4)
        '
        'lblCatIcon4
        '
        Me.lblCatIcon4.AutoSize = True
        Me.lblCatIcon4.BackColor = Color.Transparent
        Me.lblCatIcon4.Font = New Font("Segoe UI Emoji", 12.0!)
        Me.lblCatIcon4.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblCatIcon4.Location = New Point(10, 14)
        Me.lblCatIcon4.Text = "🛻"
        Me.lblCatIcon4.UseMnemonic = False
        '
        'lblCatTitle4
        '
        Me.lblCatTitle4.AutoSize = False
        Me.lblCatTitle4.BackColor = Color.Transparent
        Me.lblCatTitle4.Font = New Font("Segoe UI", 9.5!)
        Me.lblCatTitle4.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblCatTitle4.Location = New Point(42, 12)
        Me.lblCatTitle4.Size = New Size(108, 38)
        Me.lblCatTitle4.Text = "Pickup"
        Me.lblCatTitle4.UseMnemonic = False
        '
        'lblCatAvailName4
        '
        Me.lblCatAvailName4.AutoSize = True
        Me.lblCatAvailName4.BackColor = Color.Transparent
        Me.lblCatAvailName4.Font = New Font("Segoe UI", 8.5!)
        Me.lblCatAvailName4.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblCatAvailName4.Location = New Point(14, 66)
        Me.lblCatAvailName4.Text = "Available"
        Me.lblCatAvailName4.UseMnemonic = False
        '
        'lblCatAvail4
        '
        Me.lblCatAvail4.AutoSize = False
        Me.lblCatAvail4.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.lblCatAvail4.BackColor = Color.Transparent
        Me.lblCatAvail4.Font = New Font("Segoe UI", 9.5!, FontStyle.Bold)
        Me.lblCatAvail4.ForeColor = Color.FromArgb(30, 130, 70)
        Me.lblCatAvail4.Location = New Point(104, 64)
        Me.lblCatAvail4.Size = New Size(42, 20)
        Me.lblCatAvail4.Text = "00"
        Me.lblCatAvail4.TextAlign = ContentAlignment.MiddleRight
        Me.lblCatAvail4.UseMnemonic = False
        '
        'pnlCatBar4
        '
        Me.pnlCatBar4.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.pnlCatBar4.BackColor = Color.FromArgb(250, 249, 247)
        Me.pnlCatBar4.Location = New Point(14, 92)
        Me.pnlCatBar4.Name = "pnlCatBar4"
        Me.pnlCatBar4.Size = New Size(132, 6)
        '
        'lblCatRes4
        '
        Me.lblCatRes4.AutoSize = True
        Me.lblCatRes4.BackColor = Color.Transparent
        Me.lblCatRes4.Font = New Font("Segoe UI", 7.5!)
        Me.lblCatRes4.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblCatRes4.Location = New Point(14, 114)
        Me.lblCatRes4.Text = "RES: 0"
        Me.lblCatRes4.UseMnemonic = False
        '
        'lblCatRent4
        '
        Me.lblCatRent4.AutoSize = True
        Me.lblCatRent4.BackColor = Color.Transparent
        Me.lblCatRent4.Font = New Font("Segoe UI", 7.5!)
        Me.lblCatRent4.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblCatRent4.Location = New Point(74, 114)
        Me.lblCatRent4.Text = "RENT: 0"
        Me.lblCatRent4.UseMnemonic = False
        '
        'lblCatMaint4
        '
        Me.lblCatMaint4.AutoSize = True
        Me.lblCatMaint4.BackColor = Color.Transparent
        Me.lblCatMaint4.Font = New Font("Segoe UI", 7.5!, FontStyle.Bold)
        Me.lblCatMaint4.ForeColor = Color.FromArgb(122, 0, 16)
        Me.lblCatMaint4.Location = New Point(14, 136)
        Me.lblCatMaint4.Text = "MAINT: 0"
        Me.lblCatMaint4.UseMnemonic = False
        '
        'lblReturnsTitle
        '
        Me.lblReturnsTitle.AutoSize = True
        Me.lblReturnsTitle.BackColor = Color.Transparent
        Me.lblReturnsTitle.Font = New Font("Segoe UI", 10.5!, FontStyle.Bold)
        Me.lblReturnsTitle.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblReturnsTitle.Location = New Point(22, 22)
        Me.lblReturnsTitle.Text = "⏳  RETURNS TODAY"
        Me.lblReturnsTitle.UseMnemonic = False
        '
        'pnlReturnsLine
        '
        Me.pnlReturnsLine.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.pnlReturnsLine.BackColor = Color.FromArgb(235, 232, 228)
        Me.pnlReturnsLine.Location = New Point(1, 62)
        Me.pnlReturnsLine.Name = "pnlReturnsLine"
        Me.pnlReturnsLine.Size = New Size(368, 1)
        '
        'pnlRet1
        '
        Me.pnlRet1.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.pnlRet1.BackColor = Color.White
        Me.pnlRet1.Location = New Point(1, 63)
        Me.pnlRet1.Name = "pnlRet1"
        Me.pnlRet1.Size = New Size(368, 66)
        Me.pnlRet1.Controls.Add(Me.pnlRetLine1)
        Me.pnlRet1.Controls.Add(Me.lblRetIcon1)
        Me.pnlRet1.Controls.Add(Me.lblRetName1)
        Me.pnlRet1.Controls.Add(Me.lblRetSub1)
        Me.pnlRet1.Controls.Add(Me.lblRetEta1)
        '
        'pnlRetLine1
        '
        Me.pnlRetLine1.Dock = DockStyle.Bottom
        Me.pnlRetLine1.BackColor = Color.FromArgb(238, 235, 231)
        Me.pnlRetLine1.Name = "pnlRetLine1"
        Me.pnlRetLine1.Size = New Size(368, 1)
        '
        'lblRetIcon1
        '
        Me.lblRetIcon1.AutoSize = False
        Me.lblRetIcon1.BackColor = Color.FromArgb(242, 241, 239)
        Me.lblRetIcon1.Font = New Font("Segoe UI Emoji", 11.0!)
        Me.lblRetIcon1.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblRetIcon1.Location = New Point(18, 15)
        Me.lblRetIcon1.Size = New Size(36, 36)
        Me.lblRetIcon1.Text = "🚗"
        Me.lblRetIcon1.TextAlign = ContentAlignment.MiddleCenter
        Me.lblRetIcon1.UseMnemonic = False
        '
        'lblRetName1
        '
        Me.lblRetName1.AutoSize = True
        Me.lblRetName1.BackColor = Color.Transparent
        Me.lblRetName1.Font = New Font("Segoe UI", 8.5!, FontStyle.Bold)
        Me.lblRetName1.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblRetName1.Location = New Point(66, 16)
        Me.lblRetName1.Text = "VEHICLE"
        Me.lblRetName1.UseMnemonic = False
        '
        'lblRetSub1
        '
        Me.lblRetSub1.AutoSize = True
        Me.lblRetSub1.BackColor = Color.Transparent
        Me.lblRetSub1.Font = New Font("Segoe UI", 7.5!)
        Me.lblRetSub1.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblRetSub1.Location = New Point(66, 35)
        Me.lblRetSub1.Text = "Customer • 00:00"
        Me.lblRetSub1.UseMnemonic = False
        '
        'lblRetEta1
        '
        Me.lblRetEta1.AutoSize = False
        Me.lblRetEta1.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.lblRetEta1.BackColor = Color.FromArgb(232, 240, 252)
        Me.lblRetEta1.Font = New Font("Segoe UI", 7.5!, FontStyle.Bold)
        Me.lblRetEta1.ForeColor = Color.FromArgb(30, 90, 200)
        Me.lblRetEta1.Location = New Point(280, 22)
        Me.lblRetEta1.Size = New Size(72, 22)
        Me.lblRetEta1.Text = "ETA"
        Me.lblRetEta1.TextAlign = ContentAlignment.MiddleCenter
        Me.lblRetEta1.UseMnemonic = False
        '
        'pnlRet2
        '
        Me.pnlRet2.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.pnlRet2.BackColor = Color.White
        Me.pnlRet2.Location = New Point(1, 129)
        Me.pnlRet2.Name = "pnlRet2"
        Me.pnlRet2.Size = New Size(368, 66)
        Me.pnlRet2.Controls.Add(Me.pnlRetLine2)
        Me.pnlRet2.Controls.Add(Me.lblRetIcon2)
        Me.pnlRet2.Controls.Add(Me.lblRetName2)
        Me.pnlRet2.Controls.Add(Me.lblRetSub2)
        Me.pnlRet2.Controls.Add(Me.lblRetEta2)
        '
        'pnlRetLine2
        '
        Me.pnlRetLine2.Dock = DockStyle.Bottom
        Me.pnlRetLine2.BackColor = Color.FromArgb(238, 235, 231)
        Me.pnlRetLine2.Name = "pnlRetLine2"
        Me.pnlRetLine2.Size = New Size(368, 1)
        '
        'lblRetIcon2
        '
        Me.lblRetIcon2.AutoSize = False
        Me.lblRetIcon2.BackColor = Color.FromArgb(242, 241, 239)
        Me.lblRetIcon2.Font = New Font("Segoe UI Emoji", 11.0!)
        Me.lblRetIcon2.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblRetIcon2.Location = New Point(18, 15)
        Me.lblRetIcon2.Size = New Size(36, 36)
        Me.lblRetIcon2.Text = "🚗"
        Me.lblRetIcon2.TextAlign = ContentAlignment.MiddleCenter
        Me.lblRetIcon2.UseMnemonic = False
        '
        'lblRetName2
        '
        Me.lblRetName2.AutoSize = True
        Me.lblRetName2.BackColor = Color.Transparent
        Me.lblRetName2.Font = New Font("Segoe UI", 8.5!, FontStyle.Bold)
        Me.lblRetName2.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblRetName2.Location = New Point(66, 16)
        Me.lblRetName2.Text = "VEHICLE"
        Me.lblRetName2.UseMnemonic = False
        '
        'lblRetSub2
        '
        Me.lblRetSub2.AutoSize = True
        Me.lblRetSub2.BackColor = Color.Transparent
        Me.lblRetSub2.Font = New Font("Segoe UI", 7.5!)
        Me.lblRetSub2.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblRetSub2.Location = New Point(66, 35)
        Me.lblRetSub2.Text = "Customer • 00:00"
        Me.lblRetSub2.UseMnemonic = False
        '
        'lblRetEta2
        '
        Me.lblRetEta2.AutoSize = False
        Me.lblRetEta2.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.lblRetEta2.BackColor = Color.FromArgb(232, 240, 252)
        Me.lblRetEta2.Font = New Font("Segoe UI", 7.5!, FontStyle.Bold)
        Me.lblRetEta2.ForeColor = Color.FromArgb(30, 90, 200)
        Me.lblRetEta2.Location = New Point(280, 22)
        Me.lblRetEta2.Size = New Size(72, 22)
        Me.lblRetEta2.Text = "ETA"
        Me.lblRetEta2.TextAlign = ContentAlignment.MiddleCenter
        Me.lblRetEta2.UseMnemonic = False
        '
        'pnlRet3
        '
        Me.pnlRet3.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.pnlRet3.BackColor = Color.White
        Me.pnlRet3.Location = New Point(1, 195)
        Me.pnlRet3.Name = "pnlRet3"
        Me.pnlRet3.Size = New Size(368, 66)
        Me.pnlRet3.Controls.Add(Me.pnlRetLine3)
        Me.pnlRet3.Controls.Add(Me.lblRetIcon3)
        Me.pnlRet3.Controls.Add(Me.lblRetName3)
        Me.pnlRet3.Controls.Add(Me.lblRetSub3)
        Me.pnlRet3.Controls.Add(Me.lblRetEta3)
        '
        'pnlRetLine3
        '
        Me.pnlRetLine3.Dock = DockStyle.Bottom
        Me.pnlRetLine3.BackColor = Color.FromArgb(238, 235, 231)
        Me.pnlRetLine3.Name = "pnlRetLine3"
        Me.pnlRetLine3.Size = New Size(368, 1)
        '
        'lblRetIcon3
        '
        Me.lblRetIcon3.AutoSize = False
        Me.lblRetIcon3.BackColor = Color.FromArgb(242, 241, 239)
        Me.lblRetIcon3.Font = New Font("Segoe UI Emoji", 11.0!)
        Me.lblRetIcon3.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblRetIcon3.Location = New Point(18, 15)
        Me.lblRetIcon3.Size = New Size(36, 36)
        Me.lblRetIcon3.Text = "🚗"
        Me.lblRetIcon3.TextAlign = ContentAlignment.MiddleCenter
        Me.lblRetIcon3.UseMnemonic = False
        '
        'lblRetName3
        '
        Me.lblRetName3.AutoSize = True
        Me.lblRetName3.BackColor = Color.Transparent
        Me.lblRetName3.Font = New Font("Segoe UI", 8.5!, FontStyle.Bold)
        Me.lblRetName3.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblRetName3.Location = New Point(66, 16)
        Me.lblRetName3.Text = "VEHICLE"
        Me.lblRetName3.UseMnemonic = False
        '
        'lblRetSub3
        '
        Me.lblRetSub3.AutoSize = True
        Me.lblRetSub3.BackColor = Color.Transparent
        Me.lblRetSub3.Font = New Font("Segoe UI", 7.5!)
        Me.lblRetSub3.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblRetSub3.Location = New Point(66, 35)
        Me.lblRetSub3.Text = "Customer • 00:00"
        Me.lblRetSub3.UseMnemonic = False
        '
        'lblRetEta3
        '
        Me.lblRetEta3.AutoSize = False
        Me.lblRetEta3.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.lblRetEta3.BackColor = Color.FromArgb(232, 240, 252)
        Me.lblRetEta3.Font = New Font("Segoe UI", 7.5!, FontStyle.Bold)
        Me.lblRetEta3.ForeColor = Color.FromArgb(30, 90, 200)
        Me.lblRetEta3.Location = New Point(280, 22)
        Me.lblRetEta3.Size = New Size(72, 22)
        Me.lblRetEta3.Text = "ETA"
        Me.lblRetEta3.TextAlign = ContentAlignment.MiddleCenter
        Me.lblRetEta3.UseMnemonic = False
        '
        'pnlRet4
        '
        Me.pnlRet4.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.pnlRet4.BackColor = Color.White
        Me.pnlRet4.Location = New Point(1, 261)
        Me.pnlRet4.Name = "pnlRet4"
        Me.pnlRet4.Size = New Size(368, 66)
        Me.pnlRet4.Controls.Add(Me.pnlRetLine4)
        Me.pnlRet4.Controls.Add(Me.lblRetIcon4)
        Me.pnlRet4.Controls.Add(Me.lblRetName4)
        Me.pnlRet4.Controls.Add(Me.lblRetSub4)
        Me.pnlRet4.Controls.Add(Me.lblRetEta4)
        '
        'pnlRetLine4
        '
        Me.pnlRetLine4.Dock = DockStyle.Bottom
        Me.pnlRetLine4.BackColor = Color.FromArgb(238, 235, 231)
        Me.pnlRetLine4.Name = "pnlRetLine4"
        Me.pnlRetLine4.Size = New Size(368, 1)
        '
        'lblRetIcon4
        '
        Me.lblRetIcon4.AutoSize = False
        Me.lblRetIcon4.BackColor = Color.FromArgb(242, 241, 239)
        Me.lblRetIcon4.Font = New Font("Segoe UI Emoji", 11.0!)
        Me.lblRetIcon4.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblRetIcon4.Location = New Point(18, 15)
        Me.lblRetIcon4.Size = New Size(36, 36)
        Me.lblRetIcon4.Text = "🚗"
        Me.lblRetIcon4.TextAlign = ContentAlignment.MiddleCenter
        Me.lblRetIcon4.UseMnemonic = False
        '
        'lblRetName4
        '
        Me.lblRetName4.AutoSize = True
        Me.lblRetName4.BackColor = Color.Transparent
        Me.lblRetName4.Font = New Font("Segoe UI", 8.5!, FontStyle.Bold)
        Me.lblRetName4.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblRetName4.Location = New Point(66, 16)
        Me.lblRetName4.Text = "VEHICLE"
        Me.lblRetName4.UseMnemonic = False
        '
        'lblRetSub4
        '
        Me.lblRetSub4.AutoSize = True
        Me.lblRetSub4.BackColor = Color.Transparent
        Me.lblRetSub4.Font = New Font("Segoe UI", 7.5!)
        Me.lblRetSub4.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblRetSub4.Location = New Point(66, 35)
        Me.lblRetSub4.Text = "Customer • 00:00"
        Me.lblRetSub4.UseMnemonic = False
        '
        'lblRetEta4
        '
        Me.lblRetEta4.AutoSize = False
        Me.lblRetEta4.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.lblRetEta4.BackColor = Color.FromArgb(232, 240, 252)
        Me.lblRetEta4.Font = New Font("Segoe UI", 7.5!, FontStyle.Bold)
        Me.lblRetEta4.ForeColor = Color.FromArgb(30, 90, 200)
        Me.lblRetEta4.Location = New Point(280, 22)
        Me.lblRetEta4.Size = New Size(72, 22)
        Me.lblRetEta4.Text = "ETA"
        Me.lblRetEta4.TextAlign = ContentAlignment.MiddleCenter
        Me.lblRetEta4.UseMnemonic = False
        '
        'cardDispatch
        '
        Me.cardDispatch.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.cardDispatch.BackColor = Color.White
        Me.cardDispatch.Location = New Point(24, 556)
        Me.cardDispatch.Name = "cardDispatch"
        Me.cardDispatch.Size = New Size(1112, 430)
        Me.cardDispatch.Controls.Add(Me.dgvDispatch)
        Me.cardDispatch.Controls.Add(Me.pnlDispFoot)
        Me.cardDispatch.Controls.Add(Me.pnlDispHead)
        '
        'dgvDispatch
        '
        Me.dgvDispatch.Columns.AddRange(New DataGridViewColumn() {Me.colBookingId, Me.colCustomer, Me.colVehicle, Me.colDispatch, Me.colReturn, Me.colStatus, Me.colAction})
        Me.dgvDispatch.RowTemplate.Height = 70
        Me.dgvDispatch.Dock = DockStyle.Fill
        Me.dgvDispatch.AllowUserToAddRows = False
        Me.dgvDispatch.AllowUserToDeleteRows = False
        Me.dgvDispatch.AllowUserToResizeRows = False
        Me.dgvDispatch.AllowUserToResizeColumns = False
        Me.dgvDispatch.BackgroundColor = Color.White
        Me.dgvDispatch.BorderStyle = BorderStyle.None
        Me.dgvDispatch.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        Me.dgvDispatch.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        Me.dgvDispatch.ColumnHeadersDefaultCellStyle = dgvHeaderStyle
        Me.dgvDispatch.ColumnHeadersHeight = 48
        Me.dgvDispatch.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.dgvDispatch.DefaultCellStyle = dgvCellStyle
        Me.dgvDispatch.EnableHeadersVisualStyles = False
        Me.dgvDispatch.Font = New Font("Segoe UI", 9.5!)
        Me.dgvDispatch.GridColor = Color.FromArgb(235, 232, 228)
        Me.dgvDispatch.Location = New Point(0, 70)
        Me.dgvDispatch.MultiSelect = False
        Me.dgvDispatch.Name = "dgvDispatch"
        Me.dgvDispatch.ReadOnly = True
        Me.dgvDispatch.RowHeadersVisible = False
        Me.dgvDispatch.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        Me.dgvDispatch.Size = New Size(1112, 296)
        Me.dgvDispatch.TabIndex = 0
        '
        'colBookingId
        '
        Me.colBookingId.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        Me.colBookingId.FillWeight = 10.0!
        Me.colBookingId.HeaderText = "BOOKING ID"
        Me.colBookingId.Name = "colBookingId"
        Me.colBookingId.ReadOnly = True
        Me.colBookingId.SortMode = DataGridViewColumnSortMode.NotSortable
        Me.colBookingId.DefaultCellStyle = dgvIdStyle
        '
        'colCustomer
        '
        Me.colCustomer.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        Me.colCustomer.FillWeight = 17.0!
        Me.colCustomer.HeaderText = "CUSTOMER"
        Me.colCustomer.Name = "colCustomer"
        Me.colCustomer.ReadOnly = True
        Me.colCustomer.SortMode = DataGridViewColumnSortMode.NotSortable
        '
        'colVehicle
        '
        Me.colVehicle.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        Me.colVehicle.FillWeight = 17.0!
        Me.colVehicle.HeaderText = "VEHICLE"
        Me.colVehicle.Name = "colVehicle"
        Me.colVehicle.ReadOnly = True
        Me.colVehicle.SortMode = DataGridViewColumnSortMode.NotSortable
        '
        'colDispatch
        '
        Me.colDispatch.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        Me.colDispatch.FillWeight = 14.0!
        Me.colDispatch.HeaderText = "DISPATCH TIME"
        Me.colDispatch.Name = "colDispatch"
        Me.colDispatch.ReadOnly = True
        Me.colDispatch.SortMode = DataGridViewColumnSortMode.NotSortable
        '
        'colReturn
        '
        Me.colReturn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        Me.colReturn.FillWeight = 12.0!
        Me.colReturn.HeaderText = "EST. RETURN"
        Me.colReturn.Name = "colReturn"
        Me.colReturn.ReadOnly = True
        Me.colReturn.SortMode = DataGridViewColumnSortMode.NotSortable
        '
        'colStatus
        '
        Me.colStatus.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        Me.colStatus.FillWeight = 15.0!
        Me.colStatus.HeaderText = "STATUS"
        Me.colStatus.Name = "colStatus"
        Me.colStatus.ReadOnly = True
        Me.colStatus.SortMode = DataGridViewColumnSortMode.NotSortable
        '
        'colAction
        '
        Me.colAction.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        Me.colAction.FillWeight = 20.0!
        Me.colAction.HeaderText = "ACTIONS"
        Me.colAction.Name = "colAction"
        Me.colAction.ReadOnly = True
        Me.colAction.SortMode = DataGridViewColumnSortMode.NotSortable
        Me.colAction.MinimumWidth = 230
        '
        'pnlDispFoot
        '
        Me.pnlDispFoot.Dock = DockStyle.Bottom
        Me.pnlDispFoot.BackColor = Color.White
        Me.pnlDispFoot.Location = New Point(0, 366)
        Me.pnlDispFoot.Name = "pnlDispFoot"
        Me.pnlDispFoot.Size = New Size(1112, 64)
        Me.pnlDispFoot.Controls.Add(Me.pnlFootLine)
        Me.pnlDispFoot.Controls.Add(Me.lblShowing)
        Me.pnlDispFoot.Controls.Add(Me.btnPrev)
        Me.pnlDispFoot.Controls.Add(Me.btnPage1)
        Me.pnlDispFoot.Controls.Add(Me.btnPage2)
        Me.pnlDispFoot.Controls.Add(Me.btnPage3)
        Me.pnlDispFoot.Controls.Add(Me.btnNext)
        '
        'pnlFootLine
        '
        Me.pnlFootLine.Dock = DockStyle.Top
        Me.pnlFootLine.BackColor = Color.FromArgb(235, 232, 228)
        Me.pnlFootLine.Name = "pnlFootLine"
        Me.pnlFootLine.Size = New Size(1112, 1)
        '
        'lblShowing
        '
        Me.lblShowing.AutoSize = True
        Me.lblShowing.BackColor = Color.Transparent
        Me.lblShowing.Font = New Font("Segoe UI", 8.5!)
        Me.lblShowing.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblShowing.Location = New Point(22, 24)
        Me.lblShowing.Text = "Showing 3 of 15 entries for today"
        Me.lblShowing.UseMnemonic = False
        '
        'btnPrev
        '
        Me.btnPrev.FlatAppearance.BorderSize = 1
        Me.btnPrev.FlatAppearance.BorderColor = Color.FromArgb(220, 217, 212)
        Me.btnPrev.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.btnPrev.BackColor = Color.White
        Me.btnPrev.Cursor = Cursors.Hand
        Me.btnPrev.FlatStyle = FlatStyle.Flat
        Me.btnPrev.Font = New Font("Segoe UI", 9.5!)
        Me.btnPrev.ForeColor = Color.FromArgb(90, 90, 90)
        Me.btnPrev.Location = New Point(886, 15)
        Me.btnPrev.Size = New Size(34, 34)
        Me.btnPrev.Text = "‹"
        Me.btnPrev.UseMnemonic = False
        Me.btnPrev.UseVisualStyleBackColor = False
        '
        'btnPage1
        '
        Me.btnPage1.FlatAppearance.BorderSize = 1
        Me.btnPage1.FlatAppearance.BorderColor = Color.FromArgb(220, 217, 212)
        Me.btnPage1.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.btnPage1.BackColor = Color.White
        Me.btnPage1.Cursor = Cursors.Hand
        Me.btnPage1.FlatStyle = FlatStyle.Flat
        Me.btnPage1.Font = New Font("Segoe UI", 9.5!)
        Me.btnPage1.ForeColor = Color.FromArgb(90, 90, 90)
        Me.btnPage1.Location = New Point(928, 15)
        Me.btnPage1.Size = New Size(34, 34)
        Me.btnPage1.Text = "1"
        Me.btnPage1.UseMnemonic = False
        Me.btnPage1.UseVisualStyleBackColor = False
        '
        'btnPage2
        '
        Me.btnPage2.FlatAppearance.BorderSize = 1
        Me.btnPage2.FlatAppearance.BorderColor = Color.FromArgb(220, 217, 212)
        Me.btnPage2.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.btnPage2.BackColor = Color.White
        Me.btnPage2.Cursor = Cursors.Hand
        Me.btnPage2.FlatStyle = FlatStyle.Flat
        Me.btnPage2.Font = New Font("Segoe UI", 9.5!)
        Me.btnPage2.ForeColor = Color.FromArgb(90, 90, 90)
        Me.btnPage2.Location = New Point(970, 15)
        Me.btnPage2.Size = New Size(34, 34)
        Me.btnPage2.Text = "2"
        Me.btnPage2.UseMnemonic = False
        Me.btnPage2.UseVisualStyleBackColor = False
        '
        'btnPage3
        '
        Me.btnPage3.FlatAppearance.BorderSize = 1
        Me.btnPage3.FlatAppearance.BorderColor = Color.FromArgb(220, 217, 212)
        Me.btnPage3.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.btnPage3.BackColor = Color.White
        Me.btnPage3.Cursor = Cursors.Hand
        Me.btnPage3.FlatStyle = FlatStyle.Flat
        Me.btnPage3.Font = New Font("Segoe UI", 9.5!)
        Me.btnPage3.ForeColor = Color.FromArgb(90, 90, 90)
        Me.btnPage3.Location = New Point(1012, 15)
        Me.btnPage3.Size = New Size(34, 34)
        Me.btnPage3.Text = "3"
        Me.btnPage3.UseMnemonic = False
        Me.btnPage3.UseVisualStyleBackColor = False
        '
        'btnNext
        '
        Me.btnNext.FlatAppearance.BorderSize = 1
        Me.btnNext.FlatAppearance.BorderColor = Color.FromArgb(220, 217, 212)
        Me.btnNext.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.btnNext.BackColor = Color.White
        Me.btnNext.Cursor = Cursors.Hand
        Me.btnNext.FlatStyle = FlatStyle.Flat
        Me.btnNext.Font = New Font("Segoe UI", 9.5!)
        Me.btnNext.ForeColor = Color.FromArgb(90, 90, 90)
        Me.btnNext.Location = New Point(1054, 15)
        Me.btnNext.Size = New Size(34, 34)
        Me.btnNext.Text = "›"
        Me.btnNext.UseMnemonic = False
        Me.btnNext.UseVisualStyleBackColor = False
        '
        'pnlDispHead
        '
        Me.pnlDispHead.Dock = DockStyle.Top
        Me.pnlDispHead.BackColor = Color.FromArgb(38, 38, 38)
        Me.pnlDispHead.Location = New Point(0, 0)
        Me.pnlDispHead.Name = "pnlDispHead"
        Me.pnlDispHead.Size = New Size(1112, 70)
        Me.pnlDispHead.Controls.Add(Me.lblDispTitle)
        Me.pnlDispHead.Controls.Add(Me.pnlDispTabs)
        Me.pnlDispHead.Controls.Add(Me.btnNewBooking)
        '
        'lblDispTitle
        '
        Me.lblDispTitle.AutoSize = True
        Me.lblDispTitle.BackColor = Color.Transparent
        Me.lblDispTitle.Font = New Font("Segoe UI", 10.5!, FontStyle.Bold)
        Me.lblDispTitle.ForeColor = Color.White
        Me.lblDispTitle.Location = New Point(24, 25)
        Me.lblDispTitle.Text = "DAILY DISPATCH SCHEDULE"
        Me.lblDispTitle.UseMnemonic = False
        '
        'pnlDispTabs
        '
        Me.pnlDispTabs.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.pnlDispTabs.BackColor = Color.FromArgb(64, 64, 64)
        Me.pnlDispTabs.Location = New Point(690, 18)
        Me.pnlDispTabs.Name = "pnlDispTabs"
        Me.pnlDispTabs.Size = New Size(240, 34)
        Me.pnlDispTabs.Controls.Add(Me.btnTabPending)
        Me.pnlDispTabs.Controls.Add(Me.btnTabActive)
        Me.pnlDispTabs.Controls.Add(Me.btnTabCompleted)
        '
        'btnTabPending
        '
        Me.btnTabPending.FlatAppearance.BorderSize = 0
        Me.btnTabPending.BackColor = Color.White
        Me.btnTabPending.Cursor = Cursors.Hand
        Me.btnTabPending.FlatStyle = FlatStyle.Flat
        Me.btnTabPending.Font = New Font("Segoe UI", 7.5!, FontStyle.Bold)
        Me.btnTabPending.ForeColor = Color.FromArgb(38, 38, 38)
        Me.btnTabPending.Location = New Point(3, 3)
        Me.btnTabPending.Size = New Size(78, 28)
        Me.btnTabPending.Text = "PENDING"
        Me.btnTabPending.UseMnemonic = False
        Me.btnTabPending.UseVisualStyleBackColor = False
        '
        'btnTabActive
        '
        Me.btnTabActive.FlatAppearance.BorderSize = 0
        Me.btnTabActive.BackColor = Color.FromArgb(64, 64, 64)
        Me.btnTabActive.Cursor = Cursors.Hand
        Me.btnTabActive.FlatStyle = FlatStyle.Flat
        Me.btnTabActive.Font = New Font("Segoe UI", 7.5!)
        Me.btnTabActive.ForeColor = Color.FromArgb(200, 200, 200)
        Me.btnTabActive.Location = New Point(83, 3)
        Me.btnTabActive.Size = New Size(70, 28)
        Me.btnTabActive.Text = "ACTIVE"
        Me.btnTabActive.UseMnemonic = False
        Me.btnTabActive.UseVisualStyleBackColor = False
        '
        'btnTabCompleted
        '
        Me.btnTabCompleted.FlatAppearance.BorderSize = 0
        Me.btnTabCompleted.BackColor = Color.FromArgb(64, 64, 64)
        Me.btnTabCompleted.Cursor = Cursors.Hand
        Me.btnTabCompleted.FlatStyle = FlatStyle.Flat
        Me.btnTabCompleted.Font = New Font("Segoe UI", 7.5!)
        Me.btnTabCompleted.ForeColor = Color.FromArgb(200, 200, 200)
        Me.btnTabCompleted.Location = New Point(155, 3)
        Me.btnTabCompleted.Size = New Size(82, 28)
        Me.btnTabCompleted.Text = "COMPLETED"
        Me.btnTabCompleted.UseMnemonic = False
        Me.btnTabCompleted.UseVisualStyleBackColor = False
        '
        'btnNewBooking
        '
        Me.btnNewBooking.FlatAppearance.BorderSize = 0
        Me.btnNewBooking.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.btnNewBooking.BackColor = Color.FromArgb(122, 0, 16)
        Me.btnNewBooking.Cursor = Cursors.Hand
        Me.btnNewBooking.FlatStyle = FlatStyle.Flat
        Me.btnNewBooking.Font = New Font("Segoe UI", 8.0!, FontStyle.Bold)
        Me.btnNewBooking.ForeColor = Color.White
        Me.btnNewBooking.Location = New Point(950, 16)
        Me.btnNewBooking.Size = New Size(142, 38)
        Me.btnNewBooking.Text = "+   NEW BOOKING"
        Me.btnNewBooking.UseMnemonic = False
        Me.btnNewBooking.UseVisualStyleBackColor = False
        '
        'ucFrontDeskDashboard
        '
        Me.AutoScaleDimensions = New SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.AutoScroll = True
        Me.AutoScrollMinSize = New Size(900, 1010)
        Me.BackColor = Color.FromArgb(245, 244, 242)
        Me.Name = "ucFrontDeskDashboard"
        Me.Size = New Size(1160, 784)
        Me.Controls.Add(Me.tlpKpi)
        Me.Controls.Add(Me.tlpMid)
        Me.Controls.Add(Me.cardDispatch)
        CType(Me.dgvDispatch, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tlpKpi.ResumeLayout(False)
        Me.cardKpi1.ResumeLayout(False)
        Me.pnlKpiIcon1.ResumeLayout(False)
        Me.cardKpi2.ResumeLayout(False)
        Me.pnlKpiIcon2.ResumeLayout(False)
        Me.cardKpi3.ResumeLayout(False)
        Me.pnlKpiIcon3.ResumeLayout(False)
        Me.cardKpi4.ResumeLayout(False)
        Me.pnlKpiIcon4.ResumeLayout(False)
        Me.tlpMid.ResumeLayout(False)
        Me.cardFleet.ResumeLayout(False)
        Me.cardReturns.ResumeLayout(False)
        Me.pnlFleetLine.ResumeLayout(False)
        Me.tlpCats.ResumeLayout(False)
        Me.cardCat1.ResumeLayout(False)
        Me.pnlCatBar1.ResumeLayout(False)
        Me.cardCat2.ResumeLayout(False)
        Me.pnlCatBar2.ResumeLayout(False)
        Me.cardCat3.ResumeLayout(False)
        Me.pnlCatBar3.ResumeLayout(False)
        Me.cardCat4.ResumeLayout(False)
        Me.pnlCatBar4.ResumeLayout(False)
        Me.pnlReturnsLine.ResumeLayout(False)
        Me.pnlRet1.ResumeLayout(False)
        Me.pnlRetLine1.ResumeLayout(False)
        Me.pnlRet2.ResumeLayout(False)
        Me.pnlRetLine2.ResumeLayout(False)
        Me.pnlRet3.ResumeLayout(False)
        Me.pnlRetLine3.ResumeLayout(False)
        Me.pnlRet4.ResumeLayout(False)
        Me.pnlRetLine4.ResumeLayout(False)
        Me.cardDispatch.ResumeLayout(False)
        Me.pnlDispFoot.ResumeLayout(False)
        Me.pnlFootLine.ResumeLayout(False)
        Me.pnlDispHead.ResumeLayout(False)
        Me.pnlDispTabs.ResumeLayout(False)
        Me.cardKpi1.PerformLayout()
        Me.pnlKpiIcon1.PerformLayout()
        Me.cardKpi2.PerformLayout()
        Me.pnlKpiIcon2.PerformLayout()
        Me.cardKpi3.PerformLayout()
        Me.pnlKpiIcon3.PerformLayout()
        Me.cardKpi4.PerformLayout()
        Me.pnlKpiIcon4.PerformLayout()
        Me.cardFleet.PerformLayout()
        Me.cardReturns.PerformLayout()
        Me.pnlFleetLine.PerformLayout()
        Me.cardCat1.PerformLayout()
        Me.pnlCatBar1.PerformLayout()
        Me.cardCat2.PerformLayout()
        Me.pnlCatBar2.PerformLayout()
        Me.cardCat3.PerformLayout()
        Me.pnlCatBar3.PerformLayout()
        Me.cardCat4.PerformLayout()
        Me.pnlCatBar4.PerformLayout()
        Me.pnlReturnsLine.PerformLayout()
        Me.pnlRet1.PerformLayout()
        Me.pnlRetLine1.PerformLayout()
        Me.pnlRet2.PerformLayout()
        Me.pnlRetLine2.PerformLayout()
        Me.pnlRet3.PerformLayout()
        Me.pnlRetLine3.PerformLayout()
        Me.pnlRet4.PerformLayout()
        Me.pnlRetLine4.PerformLayout()
        Me.cardDispatch.PerformLayout()
        Me.pnlDispFoot.PerformLayout()
        Me.pnlFootLine.PerformLayout()
        Me.pnlDispHead.PerformLayout()
        Me.pnlDispTabs.PerformLayout()
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents tlpKpi As TableLayoutPanel
    Friend WithEvents cardKpi1 As CardPanel
    Friend WithEvents pnlKpiIcon1 As Panel
    Friend WithEvents lblKpiIcon1 As Label
    Friend WithEvents lblKpiBadge1 As Label
    Friend WithEvents lblKpiValue1 As Label
    Friend WithEvents lblKpiCaption1 As Label
    Friend WithEvents cardKpi2 As CardPanel
    Friend WithEvents pnlKpiIcon2 As Panel
    Friend WithEvents lblKpiIcon2 As Label
    Friend WithEvents lblKpiBadge2 As Label
    Friend WithEvents lblKpiValue2 As Label
    Friend WithEvents lblKpiCaption2 As Label
    Friend WithEvents cardKpi3 As CardPanel
    Friend WithEvents pnlKpiIcon3 As Panel
    Friend WithEvents lblKpiIcon3 As Label
    Friend WithEvents lblKpiBadge3 As Label
    Friend WithEvents lblKpiValue3 As Label
    Friend WithEvents lblKpiCaption3 As Label
    Friend WithEvents cardKpi4 As CardPanel
    Friend WithEvents pnlKpiIcon4 As Panel
    Friend WithEvents lblKpiIcon4 As Label
    Friend WithEvents lblKpiBadge4 As Label
    Friend WithEvents lblKpiValue4 As Label
    Friend WithEvents lblKpiCaption4 As Label
    Friend WithEvents tlpMid As TableLayoutPanel
    Friend WithEvents cardFleet As CardPanel
    Friend WithEvents cardReturns As CardPanel
    Friend WithEvents lblFleetTitle As Label
    Friend WithEvents lblViewInventory As Label
    Friend WithEvents pnlFleetLine As Panel
    Friend WithEvents tlpCats As TableLayoutPanel
    Friend WithEvents cardCat1 As CardPanel
    Friend WithEvents lblCatIcon1 As Label
    Friend WithEvents lblCatTitle1 As Label
    Friend WithEvents lblCatAvailName1 As Label
    Friend WithEvents lblCatAvail1 As Label
    Friend WithEvents pnlCatBar1 As Panel
    Friend WithEvents lblCatRes1 As Label
    Friend WithEvents lblCatRent1 As Label
    Friend WithEvents lblCatMaint1 As Label
    Friend WithEvents cardCat2 As CardPanel
    Friend WithEvents lblCatIcon2 As Label
    Friend WithEvents lblCatTitle2 As Label
    Friend WithEvents lblCatAvailName2 As Label
    Friend WithEvents lblCatAvail2 As Label
    Friend WithEvents pnlCatBar2 As Panel
    Friend WithEvents lblCatRes2 As Label
    Friend WithEvents lblCatRent2 As Label
    Friend WithEvents lblCatMaint2 As Label
    Friend WithEvents cardCat3 As CardPanel
    Friend WithEvents lblCatIcon3 As Label
    Friend WithEvents lblCatTitle3 As Label
    Friend WithEvents lblCatAvailName3 As Label
    Friend WithEvents lblCatAvail3 As Label
    Friend WithEvents pnlCatBar3 As Panel
    Friend WithEvents lblCatRes3 As Label
    Friend WithEvents lblCatRent3 As Label
    Friend WithEvents lblCatMaint3 As Label
    Friend WithEvents cardCat4 As CardPanel
    Friend WithEvents lblCatIcon4 As Label
    Friend WithEvents lblCatTitle4 As Label
    Friend WithEvents lblCatAvailName4 As Label
    Friend WithEvents lblCatAvail4 As Label
    Friend WithEvents pnlCatBar4 As Panel
    Friend WithEvents lblCatRes4 As Label
    Friend WithEvents lblCatRent4 As Label
    Friend WithEvents lblCatMaint4 As Label
    Friend WithEvents lblReturnsTitle As Label
    Friend WithEvents pnlReturnsLine As Panel
    Friend WithEvents pnlRet1 As Panel
    Friend WithEvents pnlRetLine1 As Panel
    Friend WithEvents lblRetIcon1 As Label
    Friend WithEvents lblRetName1 As Label
    Friend WithEvents lblRetSub1 As Label
    Friend WithEvents lblRetEta1 As Label
    Friend WithEvents pnlRet2 As Panel
    Friend WithEvents pnlRetLine2 As Panel
    Friend WithEvents lblRetIcon2 As Label
    Friend WithEvents lblRetName2 As Label
    Friend WithEvents lblRetSub2 As Label
    Friend WithEvents lblRetEta2 As Label
    Friend WithEvents pnlRet3 As Panel
    Friend WithEvents pnlRetLine3 As Panel
    Friend WithEvents lblRetIcon3 As Label
    Friend WithEvents lblRetName3 As Label
    Friend WithEvents lblRetSub3 As Label
    Friend WithEvents lblRetEta3 As Label
    Friend WithEvents pnlRet4 As Panel
    Friend WithEvents pnlRetLine4 As Panel
    Friend WithEvents lblRetIcon4 As Label
    Friend WithEvents lblRetName4 As Label
    Friend WithEvents lblRetSub4 As Label
    Friend WithEvents lblRetEta4 As Label
    Friend WithEvents cardDispatch As CardPanel
    Friend WithEvents dgvDispatch As DataGridView
    Friend WithEvents colBookingId As DataGridViewTextBoxColumn
    Friend WithEvents colCustomer As DataGridViewTextBoxColumn
    Friend WithEvents colVehicle As DataGridViewTextBoxColumn
    Friend WithEvents colDispatch As DataGridViewTextBoxColumn
    Friend WithEvents colReturn As DataGridViewTextBoxColumn
    Friend WithEvents colStatus As DataGridViewTextBoxColumn
    Friend WithEvents colAction As DataGridViewTextBoxColumn
    Friend WithEvents pnlDispFoot As Panel
    Friend WithEvents pnlFootLine As Panel
    Friend WithEvents lblShowing As Label
    Friend WithEvents btnPrev As Button
    Friend WithEvents btnPage1 As Button
    Friend WithEvents btnPage2 As Button
    Friend WithEvents btnPage3 As Button
    Friend WithEvents btnNext As Button
    Friend WithEvents pnlDispHead As Panel
    Friend WithEvents lblDispTitle As Label
    Friend WithEvents pnlDispTabs As Panel
    Friend WithEvents btnTabPending As Button
    Friend WithEvents btnTabActive As Button
    Friend WithEvents btnTabCompleted As Button
    Friend WithEvents btnNewBooking As Button
End Class
