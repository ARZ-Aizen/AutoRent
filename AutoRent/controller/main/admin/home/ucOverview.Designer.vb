<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ucOverview
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
        Dim dgvTsStyle As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim dgvOkStyle As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        dgvHeaderStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        dgvHeaderStyle.BackColor = Color.FromArgb(245, 244, 242)
        dgvHeaderStyle.Font = New Font("Segoe UI", 9.0!, FontStyle.Bold)
        dgvHeaderStyle.ForeColor = Color.FromArgb(70, 70, 70)
        dgvHeaderStyle.Padding = New Padding(10, 0, 0, 0)
        dgvHeaderStyle.SelectionBackColor = Color.FromArgb(245, 244, 242)
        dgvHeaderStyle.SelectionForeColor = Color.FromArgb(70, 70, 70)
        dgvHeaderStyle.WrapMode = DataGridViewTriState.[False]
        dgvCellStyle.BackColor = Color.White
        dgvCellStyle.ForeColor = Color.FromArgb(50, 50, 50)
        dgvCellStyle.Padding = New Padding(10, 0, 0, 0)
        dgvCellStyle.SelectionBackColor = Color.FromArgb(252, 245, 246)
        dgvCellStyle.SelectionForeColor = Color.FromArgb(50, 50, 50)
        dgvTsStyle.BackColor = Color.White
        dgvTsStyle.Font = New Font("Segoe UI", 9.5!, FontStyle.Bold)
        dgvTsStyle.ForeColor = Color.FromArgb(122, 0, 16)
        dgvTsStyle.Padding = New Padding(10, 0, 0, 0)
        dgvTsStyle.SelectionBackColor = Color.FromArgb(252, 245, 246)
        dgvTsStyle.SelectionForeColor = Color.FromArgb(122, 0, 16)
        dgvOkStyle.BackColor = Color.White
        dgvOkStyle.Font = New Font("Segoe UI", 9.0!, FontStyle.Bold)
        dgvOkStyle.ForeColor = Color.FromArgb(46, 102, 80)
        dgvOkStyle.Padding = New Padding(10, 0, 0, 0)
        dgvOkStyle.SelectionBackColor = Color.FromArgb(252, 245, 246)
        dgvOkStyle.SelectionForeColor = Color.FromArgb(46, 102, 80)
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.lblSubtitle = New System.Windows.Forms.Label()
        Me.btnExport = New System.Windows.Forms.Button()
        Me.tlpKpi = New System.Windows.Forms.TableLayoutPanel()
        Me.cardKpi1 = New CardPanel()
        Me.lblKpiCaption1 = New System.Windows.Forms.Label()
        Me.lblKpiIcon1 = New System.Windows.Forms.Label()
        Me.lblKpiValue1 = New System.Windows.Forms.Label()
        Me.lblKpiSub1 = New System.Windows.Forms.Label()
        Me.cardKpi2 = New CardPanel()
        Me.lblKpiCaption2 = New System.Windows.Forms.Label()
        Me.lblKpiIcon2 = New System.Windows.Forms.Label()
        Me.lblKpiValue2 = New System.Windows.Forms.Label()
        Me.lblKpiSub2 = New System.Windows.Forms.Label()
        Me.cardKpi3 = New CardPanel()
        Me.lblKpiCaption3 = New System.Windows.Forms.Label()
        Me.lblKpiIcon3 = New System.Windows.Forms.Label()
        Me.lblKpiValue3 = New System.Windows.Forms.Label()
        Me.lblKpiSub3 = New System.Windows.Forms.Label()
        Me.cardKpi4 = New CardPanel()
        Me.lblKpiCaption4 = New System.Windows.Forms.Label()
        Me.lblKpiIcon4 = New System.Windows.Forms.Label()
        Me.lblKpiValue4 = New System.Windows.Forms.Label()
        Me.lblKpiSub4 = New System.Windows.Forms.Label()
        Me.cardKpi5 = New CardPanel()
        Me.lblKpiCaption5 = New System.Windows.Forms.Label()
        Me.lblKpiIcon5 = New System.Windows.Forms.Label()
        Me.lblKpiValue5 = New System.Windows.Forms.Label()
        Me.lblKpiSub5 = New System.Windows.Forms.Label()
        Me.cardKpi6 = New CardPanel()
        Me.lblKpiCaption6 = New System.Windows.Forms.Label()
        Me.lblKpiIcon6 = New System.Windows.Forms.Label()
        Me.lblKpiValue6 = New System.Windows.Forms.Label()
        Me.lblKpiSub6 = New System.Windows.Forms.Label()
        Me.tlpMid = New System.Windows.Forms.TableLayoutPanel()
        Me.cardRevenue = New CardPanel()
        Me.cardReadiness = New CardPanel()
        Me.lblRevTitle = New System.Windows.Forms.Label()
        Me.lblRevSub = New System.Windows.Forms.Label()
        Me.lblRevDate = New System.Windows.Forms.Label()
        Me.pnlTabs = New System.Windows.Forms.Panel()
        Me.btnToday = New System.Windows.Forms.Button()
        Me.btnWeek = New System.Windows.Forms.Button()
        Me.btnMonth = New System.Windows.Forms.Button()
        Me.btnCustom = New System.Windows.Forms.Button()
        Me.pnlChart = New System.Windows.Forms.Panel()
        Me.lblCash = New System.Windows.Forms.Label()
        Me.lblOutstanding = New System.Windows.Forms.Label()
        Me.lblReadyTitle = New System.Windows.Forms.Label()
        Me.lblReadySub = New System.Windows.Forms.Label()
        Me.pnlBar = New System.Windows.Forms.Panel()
        Me.tlpStats = New System.Windows.Forms.TableLayoutPanel()
        Me.lblStatName1 = New System.Windows.Forms.Label()
        Me.lblStatVal1 = New System.Windows.Forms.Label()
        Me.lblStatName2 = New System.Windows.Forms.Label()
        Me.lblStatVal2 = New System.Windows.Forms.Label()
        Me.lblStatName3 = New System.Windows.Forms.Label()
        Me.lblStatVal3 = New System.Windows.Forms.Label()
        Me.lblStatName4 = New System.Windows.Forms.Label()
        Me.lblStatVal4 = New System.Windows.Forms.Label()
        Me.lblCatHeader = New System.Windows.Forms.Label()
        Me.tlpCat = New System.Windows.Forms.TableLayoutPanel()
        Me.lblCatName1 = New System.Windows.Forms.Label()
        Me.lblCatVal1 = New System.Windows.Forms.Label()
        Me.lblCatName2 = New System.Windows.Forms.Label()
        Me.lblCatVal2 = New System.Windows.Forms.Label()
        Me.lblCatName3 = New System.Windows.Forms.Label()
        Me.lblCatVal3 = New System.Windows.Forms.Label()
        Me.lblCatName4 = New System.Windows.Forms.Label()
        Me.lblCatVal4 = New System.Windows.Forms.Label()
        Me.cardActivity = New CardPanel()
        Me.lblActTitle = New System.Windows.Forms.Label()
        Me.btnViewAudit = New System.Windows.Forms.Button()
        Me.dgvActivity = New System.Windows.Forms.DataGridView()
        Me.colTimestamp = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colStaff = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colAction = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colRecord = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colResult = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.lblFooterOrg = New System.Windows.Forms.Label()
        CType(Me.dgvActivity, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tlpKpi.SuspendLayout()
        Me.cardKpi1.SuspendLayout()
        Me.cardKpi2.SuspendLayout()
        Me.cardKpi3.SuspendLayout()
        Me.cardKpi4.SuspendLayout()
        Me.cardKpi5.SuspendLayout()
        Me.cardKpi6.SuspendLayout()
        Me.tlpMid.SuspendLayout()
        Me.cardRevenue.SuspendLayout()
        Me.cardReadiness.SuspendLayout()
        Me.pnlTabs.SuspendLayout()
        Me.pnlChart.SuspendLayout()
        Me.pnlBar.SuspendLayout()
        Me.tlpStats.SuspendLayout()
        Me.tlpCat.SuspendLayout()
        Me.cardActivity.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = True
        Me.lblTitle.BackColor = Color.Transparent
        Me.lblTitle.Font = New Font("Segoe UI", 24.0!, FontStyle.Bold)
        Me.lblTitle.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblTitle.Location = New Point(22, 18)
        Me.lblTitle.Text = "Admin Overview Dashboard"
        Me.lblTitle.UseMnemonic = False
        '
        'lblSubtitle
        '
        Me.lblSubtitle.AutoSize = True
        Me.lblSubtitle.BackColor = Color.Transparent
        Me.lblSubtitle.Font = New Font("Segoe UI", 10.5!)
        Me.lblSubtitle.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblSubtitle.Location = New Point(25, 68)
        Me.lblSubtitle.Text = "An executive view of fleet readiness, daily performance and controlled operations."
        Me.lblSubtitle.UseMnemonic = False
        '
        'btnExport
        '
        Me.btnExport.FlatAppearance.BorderSize = 1
        Me.btnExport.FlatAppearance.BorderColor = Color.FromArgb(220, 217, 212)
        Me.btnExport.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.btnExport.BackColor = Color.White
        Me.btnExport.Cursor = Cursors.Hand
        Me.btnExport.FlatStyle = FlatStyle.Flat
        Me.btnExport.Font = New Font("Segoe UI", 10.0!, FontStyle.Bold)
        Me.btnExport.ForeColor = Color.FromArgb(38, 38, 38)
        Me.btnExport.Location = New Point(966, 26)
        Me.btnExport.Size = New Size(170, 44)
        Me.btnExport.Text = "⭳  Export snapshot"
        Me.btnExport.UseMnemonic = False
        Me.btnExport.UseVisualStyleBackColor = False
        '
        'tlpKpi
        '
        Me.tlpKpi.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 16.66667!))
        Me.tlpKpi.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 16.66667!))
        Me.tlpKpi.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 16.66667!))
        Me.tlpKpi.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 16.66667!))
        Me.tlpKpi.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 16.66667!))
        Me.tlpKpi.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 16.66667!))
        Me.tlpKpi.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0!))
        Me.tlpKpi.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.tlpKpi.ColumnCount = 6
        Me.tlpKpi.Location = New Point(18, 112)
        Me.tlpKpi.Name = "tlpKpi"
        Me.tlpKpi.RowCount = 1
        Me.tlpKpi.Size = New Size(1124, 128)
        Me.tlpKpi.Controls.Add(Me.cardKpi1, 0, 0)
        Me.tlpKpi.Controls.Add(Me.cardKpi2, 1, 0)
        Me.tlpKpi.Controls.Add(Me.cardKpi3, 2, 0)
        Me.tlpKpi.Controls.Add(Me.cardKpi4, 3, 0)
        Me.tlpKpi.Controls.Add(Me.cardKpi5, 4, 0)
        Me.tlpKpi.Controls.Add(Me.cardKpi6, 5, 0)
        '
        'cardKpi1
        '
        Me.cardKpi1.BackColor = Color.White
        Me.cardKpi1.Dock = DockStyle.Fill
        Me.cardKpi1.Margin = New Padding(6, 0, 6, 0)
        Me.cardKpi1.Name = "cardKpi1"
        Me.cardKpi1.Size = New Size(175, 128)
        Me.cardKpi1.Controls.Add(Me.lblKpiCaption1)
        Me.cardKpi1.Controls.Add(Me.lblKpiIcon1)
        Me.cardKpi1.Controls.Add(Me.lblKpiValue1)
        Me.cardKpi1.Controls.Add(Me.lblKpiSub1)
        '
        'lblKpiCaption1
        '
        Me.lblKpiCaption1.AutoSize = True
        Me.lblKpiCaption1.BackColor = Color.Transparent
        Me.lblKpiCaption1.Font = New Font("Segoe UI", 8.5!)
        Me.lblKpiCaption1.ForeColor = Color.FromArgb(70, 70, 70)
        Me.lblKpiCaption1.Location = New Point(14, 14)
        Me.lblKpiCaption1.Text = "Total Fleet Vehicles"
        Me.lblKpiCaption1.UseMnemonic = False
        '
        'lblKpiIcon1
        '
        Me.lblKpiIcon1.AutoSize = True
        Me.lblKpiIcon1.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.lblKpiIcon1.BackColor = Color.Transparent
        Me.lblKpiIcon1.Font = New Font("Segoe UI Emoji", 10.0!)
        Me.lblKpiIcon1.ForeColor = Color.FromArgb(90, 90, 90)
        Me.lblKpiIcon1.Location = New Point(146, 12)
        Me.lblKpiIcon1.Text = "🚗"
        Me.lblKpiIcon1.UseMnemonic = False
        '
        'lblKpiValue1
        '
        Me.lblKpiValue1.AutoSize = True
        Me.lblKpiValue1.BackColor = Color.Transparent
        Me.lblKpiValue1.Font = New Font("Segoe UI", 24.0!, FontStyle.Bold)
        Me.lblKpiValue1.ForeColor = Color.FromArgb(122, 0, 16)
        Me.lblKpiValue1.Location = New Point(11, 34)
        Me.lblKpiValue1.Text = "184"
        Me.lblKpiValue1.UseMnemonic = False
        '
        'lblKpiSub1
        '
        Me.lblKpiSub1.AutoSize = True
        Me.lblKpiSub1.BackColor = Color.Transparent
        Me.lblKpiSub1.Font = New Font("Segoe UI", 8.0!)
        Me.lblKpiSub1.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblKpiSub1.Location = New Point(14, 92)
        Me.lblKpiSub1.Text = "4 categories · no retired units"
        Me.lblKpiSub1.UseMnemonic = False
        '
        'cardKpi2
        '
        Me.cardKpi2.BackColor = Color.White
        Me.cardKpi2.Dock = DockStyle.Fill
        Me.cardKpi2.Margin = New Padding(6, 0, 6, 0)
        Me.cardKpi2.Name = "cardKpi2"
        Me.cardKpi2.Size = New Size(175, 128)
        Me.cardKpi2.Controls.Add(Me.lblKpiCaption2)
        Me.cardKpi2.Controls.Add(Me.lblKpiIcon2)
        Me.cardKpi2.Controls.Add(Me.lblKpiValue2)
        Me.cardKpi2.Controls.Add(Me.lblKpiSub2)
        '
        'lblKpiCaption2
        '
        Me.lblKpiCaption2.AutoSize = True
        Me.lblKpiCaption2.BackColor = Color.Transparent
        Me.lblKpiCaption2.Font = New Font("Segoe UI", 8.5!)
        Me.lblKpiCaption2.ForeColor = Color.FromArgb(70, 70, 70)
        Me.lblKpiCaption2.Location = New Point(14, 14)
        Me.lblKpiCaption2.Text = "Available Vehicles"
        Me.lblKpiCaption2.UseMnemonic = False
        '
        'lblKpiIcon2
        '
        Me.lblKpiIcon2.AutoSize = True
        Me.lblKpiIcon2.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.lblKpiIcon2.BackColor = Color.Transparent
        Me.lblKpiIcon2.Font = New Font("Segoe UI Emoji", 10.0!)
        Me.lblKpiIcon2.ForeColor = Color.FromArgb(90, 90, 90)
        Me.lblKpiIcon2.Location = New Point(146, 12)
        Me.lblKpiIcon2.Text = "✔"
        Me.lblKpiIcon2.UseMnemonic = False
        '
        'lblKpiValue2
        '
        Me.lblKpiValue2.AutoSize = True
        Me.lblKpiValue2.BackColor = Color.Transparent
        Me.lblKpiValue2.Font = New Font("Segoe UI", 24.0!, FontStyle.Bold)
        Me.lblKpiValue2.ForeColor = Color.FromArgb(122, 0, 16)
        Me.lblKpiValue2.Location = New Point(11, 34)
        Me.lblKpiValue2.Text = "42"
        Me.lblKpiValue2.UseMnemonic = False
        '
        'lblKpiSub2
        '
        Me.lblKpiSub2.AutoSize = True
        Me.lblKpiSub2.BackColor = Color.Transparent
        Me.lblKpiSub2.Font = New Font("Segoe UI", 8.0!)
        Me.lblKpiSub2.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblKpiSub2.Location = New Point(14, 92)
        Me.lblKpiSub2.Text = "+4 vs yesterday"
        Me.lblKpiSub2.UseMnemonic = False
        '
        'cardKpi3
        '
        Me.cardKpi3.BackColor = Color.White
        Me.cardKpi3.Dock = DockStyle.Fill
        Me.cardKpi3.Margin = New Padding(6, 0, 6, 0)
        Me.cardKpi3.Name = "cardKpi3"
        Me.cardKpi3.Size = New Size(175, 128)
        Me.cardKpi3.Controls.Add(Me.lblKpiCaption3)
        Me.cardKpi3.Controls.Add(Me.lblKpiIcon3)
        Me.cardKpi3.Controls.Add(Me.lblKpiValue3)
        Me.cardKpi3.Controls.Add(Me.lblKpiSub3)
        '
        'lblKpiCaption3
        '
        Me.lblKpiCaption3.AutoSize = True
        Me.lblKpiCaption3.BackColor = Color.Transparent
        Me.lblKpiCaption3.Font = New Font("Segoe UI", 8.5!)
        Me.lblKpiCaption3.ForeColor = Color.FromArgb(70, 70, 70)
        Me.lblKpiCaption3.Location = New Point(14, 14)
        Me.lblKpiCaption3.Text = "Active Rentals"
        Me.lblKpiCaption3.UseMnemonic = False
        '
        'lblKpiIcon3
        '
        Me.lblKpiIcon3.AutoSize = True
        Me.lblKpiIcon3.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.lblKpiIcon3.BackColor = Color.Transparent
        Me.lblKpiIcon3.Font = New Font("Segoe UI Emoji", 10.0!)
        Me.lblKpiIcon3.ForeColor = Color.FromArgb(90, 90, 90)
        Me.lblKpiIcon3.Location = New Point(146, 12)
        Me.lblKpiIcon3.Text = "🔑"
        Me.lblKpiIcon3.UseMnemonic = False
        '
        'lblKpiValue3
        '
        Me.lblKpiValue3.AutoSize = True
        Me.lblKpiValue3.BackColor = Color.Transparent
        Me.lblKpiValue3.Font = New Font("Segoe UI", 24.0!, FontStyle.Bold)
        Me.lblKpiValue3.ForeColor = Color.FromArgb(122, 0, 16)
        Me.lblKpiValue3.Location = New Point(11, 34)
        Me.lblKpiValue3.Text = "128"
        Me.lblKpiValue3.UseMnemonic = False
        '
        'lblKpiSub3
        '
        Me.lblKpiSub3.AutoSize = True
        Me.lblKpiSub3.BackColor = Color.Transparent
        Me.lblKpiSub3.Font = New Font("Segoe UI", 8.0!)
        Me.lblKpiSub3.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblKpiSub3.Location = New Point(14, 92)
        Me.lblKpiSub3.Text = "+6 vs yesterday"
        Me.lblKpiSub3.UseMnemonic = False
        '
        'cardKpi4
        '
        Me.cardKpi4.BackColor = Color.White
        Me.cardKpi4.Dock = DockStyle.Fill
        Me.cardKpi4.Margin = New Padding(6, 0, 6, 0)
        Me.cardKpi4.Name = "cardKpi4"
        Me.cardKpi4.Size = New Size(175, 128)
        Me.cardKpi4.Controls.Add(Me.lblKpiCaption4)
        Me.cardKpi4.Controls.Add(Me.lblKpiIcon4)
        Me.cardKpi4.Controls.Add(Me.lblKpiValue4)
        Me.cardKpi4.Controls.Add(Me.lblKpiSub4)
        '
        'lblKpiCaption4
        '
        Me.lblKpiCaption4.AutoSize = True
        Me.lblKpiCaption4.BackColor = Color.Transparent
        Me.lblKpiCaption4.Font = New Font("Segoe UI", 8.5!)
        Me.lblKpiCaption4.ForeColor = Color.FromArgb(70, 70, 70)
        Me.lblKpiCaption4.Location = New Point(14, 14)
        Me.lblKpiCaption4.Text = "Under Maintenance"
        Me.lblKpiCaption4.UseMnemonic = False
        '
        'lblKpiIcon4
        '
        Me.lblKpiIcon4.AutoSize = True
        Me.lblKpiIcon4.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.lblKpiIcon4.BackColor = Color.Transparent
        Me.lblKpiIcon4.Font = New Font("Segoe UI Emoji", 10.0!)
        Me.lblKpiIcon4.ForeColor = Color.FromArgb(90, 90, 90)
        Me.lblKpiIcon4.Location = New Point(146, 12)
        Me.lblKpiIcon4.Text = "🔧"
        Me.lblKpiIcon4.UseMnemonic = False
        '
        'lblKpiValue4
        '
        Me.lblKpiValue4.AutoSize = True
        Me.lblKpiValue4.BackColor = Color.Transparent
        Me.lblKpiValue4.Font = New Font("Segoe UI", 24.0!, FontStyle.Bold)
        Me.lblKpiValue4.ForeColor = Color.FromArgb(122, 0, 16)
        Me.lblKpiValue4.Location = New Point(11, 34)
        Me.lblKpiValue4.Text = "7"
        Me.lblKpiValue4.UseMnemonic = False
        '
        'lblKpiSub4
        '
        Me.lblKpiSub4.AutoSize = True
        Me.lblKpiSub4.BackColor = Color.Transparent
        Me.lblKpiSub4.Font = New Font("Segoe UI", 8.0!)
        Me.lblKpiSub4.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblKpiSub4.Location = New Point(14, 92)
        Me.lblKpiSub4.Text = "2 overdue services"
        Me.lblKpiSub4.UseMnemonic = False
        '
        'cardKpi5
        '
        Me.cardKpi5.BackColor = Color.White
        Me.cardKpi5.Dock = DockStyle.Fill
        Me.cardKpi5.Margin = New Padding(6, 0, 6, 0)
        Me.cardKpi5.Name = "cardKpi5"
        Me.cardKpi5.Size = New Size(175, 128)
        Me.cardKpi5.Controls.Add(Me.lblKpiCaption5)
        Me.cardKpi5.Controls.Add(Me.lblKpiIcon5)
        Me.cardKpi5.Controls.Add(Me.lblKpiValue5)
        Me.cardKpi5.Controls.Add(Me.lblKpiSub5)
        '
        'lblKpiCaption5
        '
        Me.lblKpiCaption5.AutoSize = True
        Me.lblKpiCaption5.BackColor = Color.Transparent
        Me.lblKpiCaption5.Font = New Font("Segoe UI", 8.5!)
        Me.lblKpiCaption5.ForeColor = Color.FromArgb(70, 70, 70)
        Me.lblKpiCaption5.Location = New Point(14, 14)
        Me.lblKpiCaption5.Text = "Today's Revenue"
        Me.lblKpiCaption5.UseMnemonic = False
        '
        'lblKpiIcon5
        '
        Me.lblKpiIcon5.AutoSize = True
        Me.lblKpiIcon5.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.lblKpiIcon5.BackColor = Color.Transparent
        Me.lblKpiIcon5.Font = New Font("Segoe UI Emoji", 10.0!)
        Me.lblKpiIcon5.ForeColor = Color.FromArgb(90, 90, 90)
        Me.lblKpiIcon5.Location = New Point(146, 12)
        Me.lblKpiIcon5.Text = "💵"
        Me.lblKpiIcon5.UseMnemonic = False
        '
        'lblKpiValue5
        '
        Me.lblKpiValue5.AutoSize = True
        Me.lblKpiValue5.BackColor = Color.Transparent
        Me.lblKpiValue5.Font = New Font("Segoe UI", 24.0!, FontStyle.Bold)
        Me.lblKpiValue5.ForeColor = Color.FromArgb(122, 0, 16)
        Me.lblKpiValue5.Location = New Point(11, 34)
        Me.lblKpiValue5.Text = "$1,900"
        Me.lblKpiValue5.UseMnemonic = False
        '
        'lblKpiSub5
        '
        Me.lblKpiSub5.AutoSize = True
        Me.lblKpiSub5.BackColor = Color.Transparent
        Me.lblKpiSub5.Font = New Font("Segoe UI", 8.0!)
        Me.lblKpiSub5.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblKpiSub5.Location = New Point(14, 92)
        Me.lblKpiSub5.Text = "+8.6% vs yesterday"
        Me.lblKpiSub5.UseMnemonic = False
        '
        'cardKpi6
        '
        Me.cardKpi6.BackColor = Color.White
        Me.cardKpi6.Dock = DockStyle.Fill
        Me.cardKpi6.Margin = New Padding(6, 0, 6, 0)
        Me.cardKpi6.Name = "cardKpi6"
        Me.cardKpi6.Size = New Size(175, 128)
        Me.cardKpi6.Controls.Add(Me.lblKpiCaption6)
        Me.cardKpi6.Controls.Add(Me.lblKpiIcon6)
        Me.cardKpi6.Controls.Add(Me.lblKpiValue6)
        Me.cardKpi6.Controls.Add(Me.lblKpiSub6)
        '
        'lblKpiCaption6
        '
        Me.lblKpiCaption6.AutoSize = True
        Me.lblKpiCaption6.BackColor = Color.Transparent
        Me.lblKpiCaption6.Font = New Font("Segoe UI", 8.5!)
        Me.lblKpiCaption6.ForeColor = Color.FromArgb(70, 70, 70)
        Me.lblKpiCaption6.Location = New Point(14, 14)
        Me.lblKpiCaption6.Text = "Fleet Utilization Rate"
        Me.lblKpiCaption6.UseMnemonic = False
        '
        'lblKpiIcon6
        '
        Me.lblKpiIcon6.AutoSize = True
        Me.lblKpiIcon6.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.lblKpiIcon6.BackColor = Color.Transparent
        Me.lblKpiIcon6.Font = New Font("Segoe UI Emoji", 10.0!)
        Me.lblKpiIcon6.ForeColor = Color.FromArgb(90, 90, 90)
        Me.lblKpiIcon6.Location = New Point(146, 12)
        Me.lblKpiIcon6.Text = "◔"
        Me.lblKpiIcon6.UseMnemonic = False
        '
        'lblKpiValue6
        '
        Me.lblKpiValue6.AutoSize = True
        Me.lblKpiValue6.BackColor = Color.Transparent
        Me.lblKpiValue6.Font = New Font("Segoe UI", 24.0!, FontStyle.Bold)
        Me.lblKpiValue6.ForeColor = Color.FromArgb(122, 0, 16)
        Me.lblKpiValue6.Location = New Point(11, 34)
        Me.lblKpiValue6.Text = "69.6%"
        Me.lblKpiValue6.UseMnemonic = False
        '
        'lblKpiSub6
        '
        Me.lblKpiSub6.AutoSize = True
        Me.lblKpiSub6.BackColor = Color.Transparent
        Me.lblKpiSub6.Font = New Font("Segoe UI", 8.0!)
        Me.lblKpiSub6.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblKpiSub6.Location = New Point(14, 92)
        Me.lblKpiSub6.Text = "128 / 184 · +3.3 pp"
        Me.lblKpiSub6.UseMnemonic = False
        '
        'tlpMid
        '
        Me.tlpMid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 64.0!))
        Me.tlpMid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 36.0!))
        Me.tlpMid.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0!))
        Me.tlpMid.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.tlpMid.ColumnCount = 2
        Me.tlpMid.Location = New Point(18, 256)
        Me.tlpMid.Name = "tlpMid"
        Me.tlpMid.RowCount = 1
        Me.tlpMid.Size = New Size(1124, 370)
        Me.tlpMid.Controls.Add(Me.cardRevenue, 0, 0)
        Me.tlpMid.Controls.Add(Me.cardReadiness, 1, 0)
        '
        'cardRevenue
        '
        Me.cardRevenue.BackColor = Color.White
        Me.cardRevenue.Dock = DockStyle.Fill
        Me.cardRevenue.Margin = New Padding(6, 0, 6, 0)
        Me.cardRevenue.Name = "cardRevenue"
        Me.cardRevenue.Size = New Size(707, 370)
        Me.cardRevenue.Controls.Add(Me.lblRevTitle)
        Me.cardRevenue.Controls.Add(Me.lblRevSub)
        Me.cardRevenue.Controls.Add(Me.lblRevDate)
        Me.cardRevenue.Controls.Add(Me.pnlTabs)
        Me.cardRevenue.Controls.Add(Me.pnlChart)
        Me.cardRevenue.Controls.Add(Me.lblCash)
        Me.cardRevenue.Controls.Add(Me.lblOutstanding)
        '
        'cardReadiness
        '
        Me.cardReadiness.BackColor = Color.White
        Me.cardReadiness.Dock = DockStyle.Fill
        Me.cardReadiness.Margin = New Padding(6, 0, 6, 0)
        Me.cardReadiness.Name = "cardReadiness"
        Me.cardReadiness.Size = New Size(392, 370)
        Me.cardReadiness.Controls.Add(Me.lblReadyTitle)
        Me.cardReadiness.Controls.Add(Me.lblReadySub)
        Me.cardReadiness.Controls.Add(Me.pnlBar)
        Me.cardReadiness.Controls.Add(Me.tlpStats)
        Me.cardReadiness.Controls.Add(Me.lblCatHeader)
        Me.cardReadiness.Controls.Add(Me.tlpCat)
        '
        'lblRevTitle
        '
        Me.lblRevTitle.AutoSize = True
        Me.lblRevTitle.BackColor = Color.Transparent
        Me.lblRevTitle.Font = New Font("Segoe UI", 13.0!, FontStyle.Bold)
        Me.lblRevTitle.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblRevTitle.Location = New Point(18, 14)
        Me.lblRevTitle.Text = "Revenue performance"
        Me.lblRevTitle.UseMnemonic = False
        '
        'lblRevSub
        '
        Me.lblRevSub.AutoSize = True
        Me.lblRevSub.BackColor = Color.Transparent
        Me.lblRevSub.Font = New Font("Segoe UI", 9.0!)
        Me.lblRevSub.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblRevSub.Location = New Point(19, 40)
        Me.lblRevSub.Text = "Cumulative net charges + fees − discounts · refundable deposits excluded"
        Me.lblRevSub.UseMnemonic = False
        '
        'lblRevDate
        '
        Me.lblRevDate.AutoSize = False
        Me.lblRevDate.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.lblRevDate.BackColor = Color.FromArgb(250, 228, 230)
        Me.lblRevDate.Font = New Font("Segoe UI", 8.5!, FontStyle.Bold)
        Me.lblRevDate.ForeColor = Color.FromArgb(122, 0, 16)
        Me.lblRevDate.Location = New Point(597, 20)
        Me.lblRevDate.Size = New Size(92, 24)
        Me.lblRevDate.Text = "14 Oct 2026"
        Me.lblRevDate.TextAlign = ContentAlignment.MiddleCenter
        Me.lblRevDate.UseMnemonic = False
        '
        'pnlTabs
        '
        Me.pnlTabs.BackColor = Color.FromArgb(245, 244, 242)
        Me.pnlTabs.Location = New Point(18, 72)
        Me.pnlTabs.Name = "pnlTabs"
        Me.pnlTabs.Size = New Size(470, 44)
        Me.pnlTabs.Controls.Add(Me.btnToday)
        Me.pnlTabs.Controls.Add(Me.btnWeek)
        Me.pnlTabs.Controls.Add(Me.btnMonth)
        Me.pnlTabs.Controls.Add(Me.btnCustom)
        '
        'btnToday
        '
        Me.btnToday.FlatAppearance.BorderSize = 0
        Me.btnToday.BackColor = Color.FromArgb(245, 244, 242)
        Me.btnToday.Cursor = Cursors.Hand
        Me.btnToday.FlatStyle = FlatStyle.Flat
        Me.btnToday.Font = New Font("Segoe UI", 9.5!)
        Me.btnToday.ForeColor = Color.FromArgb(90, 90, 90)
        Me.btnToday.Location = New Point(6, 5)
        Me.btnToday.Size = New Size(72, 34)
        Me.btnToday.Text = "Today"
        Me.btnToday.UseMnemonic = False
        Me.btnToday.UseVisualStyleBackColor = False
        '
        'btnWeek
        '
        Me.btnWeek.FlatAppearance.BorderSize = 0
        Me.btnWeek.BackColor = Color.FromArgb(245, 244, 242)
        Me.btnWeek.Cursor = Cursors.Hand
        Me.btnWeek.FlatStyle = FlatStyle.Flat
        Me.btnWeek.Font = New Font("Segoe UI", 9.5!)
        Me.btnWeek.ForeColor = Color.FromArgb(90, 90, 90)
        Me.btnWeek.Location = New Point(80, 5)
        Me.btnWeek.Size = New Size(104, 34)
        Me.btnWeek.Text = "Last 7 Days"
        Me.btnWeek.UseMnemonic = False
        Me.btnWeek.UseVisualStyleBackColor = False
        '
        'btnMonth
        '
        Me.btnMonth.FlatAppearance.BorderSize = 0
        Me.btnMonth.BackColor = Color.FromArgb(245, 244, 242)
        Me.btnMonth.Cursor = Cursors.Hand
        Me.btnMonth.FlatStyle = FlatStyle.Flat
        Me.btnMonth.Font = New Font("Segoe UI", 9.5!)
        Me.btnMonth.ForeColor = Color.FromArgb(90, 90, 90)
        Me.btnMonth.Location = New Point(186, 5)
        Me.btnMonth.Size = New Size(112, 34)
        Me.btnMonth.Text = "Last 30 Days"
        Me.btnMonth.UseMnemonic = False
        Me.btnMonth.UseVisualStyleBackColor = False
        '
        'btnCustom
        '
        Me.btnCustom.FlatAppearance.BorderSize = 0
        Me.btnCustom.BackColor = Color.FromArgb(245, 244, 242)
        Me.btnCustom.Cursor = Cursors.Hand
        Me.btnCustom.FlatStyle = FlatStyle.Flat
        Me.btnCustom.Font = New Font("Segoe UI", 9.5!)
        Me.btnCustom.ForeColor = Color.FromArgb(90, 90, 90)
        Me.btnCustom.Location = New Point(300, 5)
        Me.btnCustom.Size = New Size(160, 34)
        Me.btnCustom.Text = "Custom Date Range"
        Me.btnCustom.UseMnemonic = False
        Me.btnCustom.UseVisualStyleBackColor = False
        '
        'pnlChart
        '
        Me.pnlChart.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        Me.pnlChart.BackColor = Color.White
        Me.pnlChart.Location = New Point(18, 124)
        Me.pnlChart.Name = "pnlChart"
        Me.pnlChart.Size = New Size(671, 202)
        '
        'lblCash
        '
        Me.lblCash.AutoSize = True
        Me.lblCash.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        Me.lblCash.BackColor = Color.Transparent
        Me.lblCash.Font = New Font("Segoe UI", 9.5!)
        Me.lblCash.ForeColor = Color.FromArgb(80, 80, 80)
        Me.lblCash.Location = New Point(18, 336)
        Me.lblCash.Text = "Collected cash $1,750"
        Me.lblCash.UseMnemonic = False
        '
        'lblOutstanding
        '
        Me.lblOutstanding.AutoSize = False
        Me.lblOutstanding.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        Me.lblOutstanding.BackColor = Color.Transparent
        Me.lblOutstanding.Font = New Font("Segoe UI", 9.5!)
        Me.lblOutstanding.ForeColor = Color.FromArgb(80, 80, 80)
        Me.lblOutstanding.Location = New Point(359, 336)
        Me.lblOutstanding.Size = New Size(330, 20)
        Me.lblOutstanding.Text = "Outstanding $150 · not collected cash"
        Me.lblOutstanding.TextAlign = ContentAlignment.MiddleRight
        Me.lblOutstanding.UseMnemonic = False
        '
        'lblReadyTitle
        '
        Me.lblReadyTitle.AutoSize = True
        Me.lblReadyTitle.BackColor = Color.Transparent
        Me.lblReadyTitle.Font = New Font("Segoe UI", 13.0!, FontStyle.Bold)
        Me.lblReadyTitle.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblReadyTitle.Location = New Point(18, 14)
        Me.lblReadyTitle.Text = "Fleet readiness"
        Me.lblReadyTitle.UseMnemonic = False
        '
        'lblReadySub
        '
        Me.lblReadySub.AutoSize = True
        Me.lblReadySub.BackColor = Color.Transparent
        Me.lblReadySub.Font = New Font("Segoe UI", 9.0!)
        Me.lblReadySub.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblReadySub.Location = New Point(19, 40)
        Me.lblReadySub.Text = "All 184 vehicles · live status"
        Me.lblReadySub.UseMnemonic = False
        '
        'pnlBar
        '
        Me.pnlBar.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.pnlBar.BackColor = Color.White
        Me.pnlBar.Location = New Point(18, 72)
        Me.pnlBar.Name = "pnlBar"
        Me.pnlBar.Size = New Size(356, 18)
        '
        'tlpStats
        '
        Me.tlpStats.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 30.0!))
        Me.tlpStats.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 20.0!))
        Me.tlpStats.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 30.0!))
        Me.tlpStats.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 20.0!))
        Me.tlpStats.RowStyles.Add(New RowStyle(SizeType.Percent, 50.0!))
        Me.tlpStats.RowStyles.Add(New RowStyle(SizeType.Percent, 50.0!))
        Me.tlpStats.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.tlpStats.ColumnCount = 4
        Me.tlpStats.Location = New Point(18, 106)
        Me.tlpStats.Name = "tlpStats"
        Me.tlpStats.RowCount = 2
        Me.tlpStats.Size = New Size(356, 64)
        Me.tlpStats.Controls.Add(Me.lblStatName1, 0, 0)
        Me.tlpStats.Controls.Add(Me.lblStatVal1, 1, 0)
        Me.tlpStats.Controls.Add(Me.lblStatName2, 2, 0)
        Me.tlpStats.Controls.Add(Me.lblStatVal2, 3, 0)
        Me.tlpStats.Controls.Add(Me.lblStatName3, 0, 1)
        Me.tlpStats.Controls.Add(Me.lblStatVal3, 1, 1)
        Me.tlpStats.Controls.Add(Me.lblStatName4, 2, 1)
        Me.tlpStats.Controls.Add(Me.lblStatVal4, 3, 1)
        '
        'lblStatName1
        '
        Me.lblStatName1.AutoSize = False
        Me.lblStatName1.BackColor = Color.Transparent
        Me.lblStatName1.Dock = DockStyle.Fill
        Me.lblStatName1.Font = New Font("Segoe UI", 9.5!)
        Me.lblStatName1.ForeColor = Color.FromArgb(100, 100, 100)
        Me.lblStatName1.Text = "Available"
        Me.lblStatName1.TextAlign = ContentAlignment.MiddleLeft
        Me.lblStatName1.UseMnemonic = False
        '
        'lblStatVal1
        '
        Me.lblStatVal1.AutoSize = False
        Me.lblStatVal1.BackColor = Color.Transparent
        Me.lblStatVal1.Dock = DockStyle.Fill
        Me.lblStatVal1.Font = New Font("Segoe UI", 9.5!, FontStyle.Bold)
        Me.lblStatVal1.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblStatVal1.Text = "42"
        Me.lblStatVal1.TextAlign = ContentAlignment.MiddleRight
        Me.lblStatVal1.UseMnemonic = False
        '
        'lblStatName2
        '
        Me.lblStatName2.AutoSize = False
        Me.lblStatName2.BackColor = Color.Transparent
        Me.lblStatName2.Dock = DockStyle.Fill
        Me.lblStatName2.Font = New Font("Segoe UI", 9.5!)
        Me.lblStatName2.ForeColor = Color.FromArgb(100, 100, 100)
        Me.lblStatName2.Text = "Rented"
        Me.lblStatName2.TextAlign = ContentAlignment.MiddleLeft
        Me.lblStatName2.UseMnemonic = False
        '
        'lblStatVal2
        '
        Me.lblStatVal2.AutoSize = False
        Me.lblStatVal2.BackColor = Color.Transparent
        Me.lblStatVal2.Dock = DockStyle.Fill
        Me.lblStatVal2.Font = New Font("Segoe UI", 9.5!, FontStyle.Bold)
        Me.lblStatVal2.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblStatVal2.Text = "128"
        Me.lblStatVal2.TextAlign = ContentAlignment.MiddleRight
        Me.lblStatVal2.UseMnemonic = False
        '
        'lblStatName3
        '
        Me.lblStatName3.AutoSize = False
        Me.lblStatName3.BackColor = Color.Transparent
        Me.lblStatName3.Dock = DockStyle.Fill
        Me.lblStatName3.Font = New Font("Segoe UI", 9.5!)
        Me.lblStatName3.ForeColor = Color.FromArgb(100, 100, 100)
        Me.lblStatName3.Text = "Reserved"
        Me.lblStatName3.TextAlign = ContentAlignment.MiddleLeft
        Me.lblStatName3.UseMnemonic = False
        '
        'lblStatVal3
        '
        Me.lblStatVal3.AutoSize = False
        Me.lblStatVal3.BackColor = Color.Transparent
        Me.lblStatVal3.Dock = DockStyle.Fill
        Me.lblStatVal3.Font = New Font("Segoe UI", 9.5!, FontStyle.Bold)
        Me.lblStatVal3.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblStatVal3.Text = "7"
        Me.lblStatVal3.TextAlign = ContentAlignment.MiddleRight
        Me.lblStatVal3.UseMnemonic = False
        '
        'lblStatName4
        '
        Me.lblStatName4.AutoSize = False
        Me.lblStatName4.BackColor = Color.Transparent
        Me.lblStatName4.Dock = DockStyle.Fill
        Me.lblStatName4.Font = New Font("Segoe UI", 9.5!)
        Me.lblStatName4.ForeColor = Color.FromArgb(100, 100, 100)
        Me.lblStatName4.Text = "Maintenance"
        Me.lblStatName4.TextAlign = ContentAlignment.MiddleLeft
        Me.lblStatName4.UseMnemonic = False
        '
        'lblStatVal4
        '
        Me.lblStatVal4.AutoSize = False
        Me.lblStatVal4.BackColor = Color.Transparent
        Me.lblStatVal4.Dock = DockStyle.Fill
        Me.lblStatVal4.Font = New Font("Segoe UI", 9.5!, FontStyle.Bold)
        Me.lblStatVal4.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblStatVal4.Text = "7"
        Me.lblStatVal4.TextAlign = ContentAlignment.MiddleRight
        Me.lblStatVal4.UseMnemonic = False
        '
        'lblCatHeader
        '
        Me.lblCatHeader.AutoSize = True
        Me.lblCatHeader.BackColor = Color.Transparent
        Me.lblCatHeader.Font = New Font("Segoe UI", 8.0!, FontStyle.Bold)
        Me.lblCatHeader.ForeColor = Color.FromArgb(70, 70, 70)
        Me.lblCatHeader.Location = New Point(18, 186)
        Me.lblCatHeader.Text = "CATEGORY · TOTAL / RENTED"
        Me.lblCatHeader.UseMnemonic = False
        '
        'tlpCat
        '
        Me.tlpCat.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 60.0!))
        Me.tlpCat.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 40.0!))
        Me.tlpCat.RowStyles.Add(New RowStyle(SizeType.Percent, 25.0!))
        Me.tlpCat.RowStyles.Add(New RowStyle(SizeType.Percent, 25.0!))
        Me.tlpCat.RowStyles.Add(New RowStyle(SizeType.Percent, 25.0!))
        Me.tlpCat.RowStyles.Add(New RowStyle(SizeType.Percent, 25.0!))
        Me.tlpCat.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.tlpCat.ColumnCount = 2
        Me.tlpCat.Location = New Point(18, 208)
        Me.tlpCat.Name = "tlpCat"
        Me.tlpCat.RowCount = 4
        Me.tlpCat.Size = New Size(356, 144)
        Me.tlpCat.Controls.Add(Me.lblCatName1, 0, 0)
        Me.tlpCat.Controls.Add(Me.lblCatVal1, 1, 0)
        Me.tlpCat.Controls.Add(Me.lblCatName2, 0, 1)
        Me.tlpCat.Controls.Add(Me.lblCatVal2, 1, 1)
        Me.tlpCat.Controls.Add(Me.lblCatName3, 0, 2)
        Me.tlpCat.Controls.Add(Me.lblCatVal3, 1, 2)
        Me.tlpCat.Controls.Add(Me.lblCatName4, 0, 3)
        Me.tlpCat.Controls.Add(Me.lblCatVal4, 1, 3)
        '
        'lblCatName1
        '
        Me.lblCatName1.AutoSize = False
        Me.lblCatName1.BackColor = Color.Transparent
        Me.lblCatName1.Dock = DockStyle.Fill
        Me.lblCatName1.Font = New Font("Segoe UI", 9.5!)
        Me.lblCatName1.ForeColor = Color.FromArgb(100, 100, 100)
        Me.lblCatName1.Text = "Sedan"
        Me.lblCatName1.TextAlign = ContentAlignment.MiddleLeft
        Me.lblCatName1.UseMnemonic = False
        '
        'lblCatVal1
        '
        Me.lblCatVal1.AutoSize = False
        Me.lblCatVal1.BackColor = Color.Transparent
        Me.lblCatVal1.Dock = DockStyle.Fill
        Me.lblCatVal1.Font = New Font("Segoe UI", 9.5!, FontStyle.Bold)
        Me.lblCatVal1.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblCatVal1.Text = "48 / 34"
        Me.lblCatVal1.TextAlign = ContentAlignment.MiddleRight
        Me.lblCatVal1.UseMnemonic = False
        '
        'lblCatName2
        '
        Me.lblCatName2.AutoSize = False
        Me.lblCatName2.BackColor = Color.Transparent
        Me.lblCatName2.Dock = DockStyle.Fill
        Me.lblCatName2.Font = New Font("Segoe UI", 9.5!)
        Me.lblCatName2.ForeColor = Color.FromArgb(100, 100, 100)
        Me.lblCatName2.Text = "SUV"
        Me.lblCatName2.TextAlign = ContentAlignment.MiddleLeft
        Me.lblCatName2.UseMnemonic = False
        '
        'lblCatVal2
        '
        Me.lblCatVal2.AutoSize = False
        Me.lblCatVal2.BackColor = Color.Transparent
        Me.lblCatVal2.Dock = DockStyle.Fill
        Me.lblCatVal2.Font = New Font("Segoe UI", 9.5!, FontStyle.Bold)
        Me.lblCatVal2.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblCatVal2.Text = "64 / 46"
        Me.lblCatVal2.TextAlign = ContentAlignment.MiddleRight
        Me.lblCatVal2.UseMnemonic = False
        '
        'lblCatName3
        '
        Me.lblCatName3.AutoSize = False
        Me.lblCatName3.BackColor = Color.Transparent
        Me.lblCatName3.Dock = DockStyle.Fill
        Me.lblCatName3.Font = New Font("Segoe UI", 9.5!)
        Me.lblCatName3.ForeColor = Color.FromArgb(100, 100, 100)
        Me.lblCatName3.Text = "Van"
        Me.lblCatName3.TextAlign = ContentAlignment.MiddleLeft
        Me.lblCatName3.UseMnemonic = False
        '
        'lblCatVal3
        '
        Me.lblCatVal3.AutoSize = False
        Me.lblCatVal3.BackColor = Color.Transparent
        Me.lblCatVal3.Dock = DockStyle.Fill
        Me.lblCatVal3.Font = New Font("Segoe UI", 9.5!, FontStyle.Bold)
        Me.lblCatVal3.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblCatVal3.Text = "32 / 24"
        Me.lblCatVal3.TextAlign = ContentAlignment.MiddleRight
        Me.lblCatVal3.UseMnemonic = False
        '
        'lblCatName4
        '
        Me.lblCatName4.AutoSize = False
        Me.lblCatName4.BackColor = Color.Transparent
        Me.lblCatName4.Dock = DockStyle.Fill
        Me.lblCatName4.Font = New Font("Segoe UI", 9.5!)
        Me.lblCatName4.ForeColor = Color.FromArgb(100, 100, 100)
        Me.lblCatName4.Text = "Pickup"
        Me.lblCatName4.TextAlign = ContentAlignment.MiddleLeft
        Me.lblCatName4.UseMnemonic = False
        '
        'lblCatVal4
        '
        Me.lblCatVal4.AutoSize = False
        Me.lblCatVal4.BackColor = Color.Transparent
        Me.lblCatVal4.Dock = DockStyle.Fill
        Me.lblCatVal4.Font = New Font("Segoe UI", 9.5!, FontStyle.Bold)
        Me.lblCatVal4.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblCatVal4.Text = "40 / 24"
        Me.lblCatVal4.TextAlign = ContentAlignment.MiddleRight
        Me.lblCatVal4.UseMnemonic = False
        '
        'cardActivity
        '
        Me.cardActivity.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.cardActivity.BackColor = Color.White
        Me.cardActivity.Location = New Point(24, 642)
        Me.cardActivity.Name = "cardActivity"
        Me.cardActivity.Size = New Size(1112, 340)
        Me.cardActivity.Controls.Add(Me.lblActTitle)
        Me.cardActivity.Controls.Add(Me.btnViewAudit)
        Me.cardActivity.Controls.Add(Me.dgvActivity)
        '
        'lblActTitle
        '
        Me.lblActTitle.AutoSize = True
        Me.lblActTitle.BackColor = Color.Transparent
        Me.lblActTitle.Font = New Font("Segoe UI", 13.0!, FontStyle.Bold)
        Me.lblActTitle.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblActTitle.Location = New Point(18, 24)
        Me.lblActTitle.Text = "Recent administrative activity"
        Me.lblActTitle.UseMnemonic = False
        '
        'btnViewAudit
        '
        Me.btnViewAudit.FlatAppearance.BorderSize = 1
        Me.btnViewAudit.FlatAppearance.BorderColor = Color.FromArgb(220, 217, 212)
        Me.btnViewAudit.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.btnViewAudit.BackColor = Color.White
        Me.btnViewAudit.Cursor = Cursors.Hand
        Me.btnViewAudit.FlatStyle = FlatStyle.Flat
        Me.btnViewAudit.Font = New Font("Segoe UI", 10.0!, FontStyle.Bold)
        Me.btnViewAudit.ForeColor = Color.FromArgb(38, 38, 38)
        Me.btnViewAudit.Location = New Point(929, 14)
        Me.btnViewAudit.Size = New Size(165, 42)
        Me.btnViewAudit.Text = "↗  View audit logs"
        Me.btnViewAudit.UseMnemonic = False
        Me.btnViewAudit.UseVisualStyleBackColor = False
        '
        'dgvActivity
        '
        Me.dgvActivity.Columns.AddRange(New DataGridViewColumn() {Me.colTimestamp, Me.colStaff, Me.colAction, Me.colRecord, Me.colResult})
        Me.dgvActivity.RowTemplate.Height = 46
        Me.dgvActivity.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        Me.dgvActivity.AllowUserToAddRows = False
        Me.dgvActivity.AllowUserToDeleteRows = False
        Me.dgvActivity.AllowUserToResizeRows = False
        Me.dgvActivity.BackgroundColor = Color.White
        Me.dgvActivity.BorderStyle = BorderStyle.None
        Me.dgvActivity.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        Me.dgvActivity.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        Me.dgvActivity.ColumnHeadersDefaultCellStyle = dgvHeaderStyle
        Me.dgvActivity.ColumnHeadersHeight = 40
        Me.dgvActivity.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.dgvActivity.DefaultCellStyle = dgvCellStyle
        Me.dgvActivity.EnableHeadersVisualStyles = False
        Me.dgvActivity.Font = New Font("Segoe UI", 9.5!)
        Me.dgvActivity.GridColor = Color.FromArgb(235, 232, 228)
        Me.dgvActivity.Location = New Point(18, 70)
        Me.dgvActivity.MultiSelect = False
        Me.dgvActivity.Name = "dgvActivity"
        Me.dgvActivity.ReadOnly = True
        Me.dgvActivity.RowHeadersVisible = False
        Me.dgvActivity.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        Me.dgvActivity.Size = New Size(1076, 250)
        Me.dgvActivity.TabIndex = 0
        '
        'colTimestamp
        '
        Me.colTimestamp.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        Me.colTimestamp.FillWeight = 18.0!
        Me.colTimestamp.HeaderText = "Timestamp ↓"
        Me.colTimestamp.Name = "colTimestamp"
        Me.colTimestamp.ReadOnly = True
        Me.colTimestamp.SortMode = DataGridViewColumnSortMode.NotSortable
        Me.colTimestamp.DefaultCellStyle = dgvTsStyle
        '
        'colStaff
        '
        Me.colStaff.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        Me.colStaff.FillWeight = 20.0!
        Me.colStaff.HeaderText = "Staff Member"
        Me.colStaff.Name = "colStaff"
        Me.colStaff.ReadOnly = True
        Me.colStaff.SortMode = DataGridViewColumnSortMode.NotSortable
        '
        'colAction
        '
        Me.colAction.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        Me.colAction.FillWeight = 20.0!
        Me.colAction.HeaderText = "Action"
        Me.colAction.Name = "colAction"
        Me.colAction.ReadOnly = True
        Me.colAction.SortMode = DataGridViewColumnSortMode.NotSortable
        '
        'colRecord
        '
        Me.colRecord.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        Me.colRecord.FillWeight = 24.0!
        Me.colRecord.HeaderText = "Related Record"
        Me.colRecord.Name = "colRecord"
        Me.colRecord.ReadOnly = True
        Me.colRecord.SortMode = DataGridViewColumnSortMode.NotSortable
        '
        'colResult
        '
        Me.colResult.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        Me.colResult.FillWeight = 18.0!
        Me.colResult.HeaderText = "Result"
        Me.colResult.Name = "colResult"
        Me.colResult.ReadOnly = True
        Me.colResult.SortMode = DataGridViewColumnSortMode.NotSortable
        Me.colResult.DefaultCellStyle = dgvOkStyle
        '
        'lblFooterOrg
        '
        Me.lblFooterOrg.AutoSize = False
        Me.lblFooterOrg.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.lblFooterOrg.BackColor = Color.Transparent
        Me.lblFooterOrg.Font = New Font("Segoe UI", 8.0!)
        Me.lblFooterOrg.ForeColor = Color.FromArgb(140, 140, 140)
        Me.lblFooterOrg.Location = New Point(736, 1000)
        Me.lblFooterOrg.Size = New Size(400, 16)
        Me.lblFooterOrg.Text = "AutoRent · HQ Branch"
        Me.lblFooterOrg.TextAlign = ContentAlignment.MiddleRight
        Me.lblFooterOrg.UseMnemonic = False
        '
        'ucOverview
        '
        Me.AutoScaleDimensions = New SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.AutoScroll = True
        Me.AutoScrollMinSize = New Size(900, 1030)
        Me.BackColor = Color.FromArgb(245, 244, 242)
        Me.Name = "ucOverview"
        Me.Size = New Size(1160, 768)
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.lblSubtitle)
        Me.Controls.Add(Me.btnExport)
        Me.Controls.Add(Me.tlpKpi)
        Me.Controls.Add(Me.tlpMid)
        Me.Controls.Add(Me.cardActivity)
        Me.Controls.Add(Me.lblFooterOrg)
        CType(Me.dgvActivity, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tlpKpi.ResumeLayout(False)
        Me.cardKpi1.ResumeLayout(False)
        Me.cardKpi2.ResumeLayout(False)
        Me.cardKpi3.ResumeLayout(False)
        Me.cardKpi4.ResumeLayout(False)
        Me.cardKpi5.ResumeLayout(False)
        Me.cardKpi6.ResumeLayout(False)
        Me.tlpMid.ResumeLayout(False)
        Me.cardRevenue.ResumeLayout(False)
        Me.cardReadiness.ResumeLayout(False)
        Me.pnlTabs.ResumeLayout(False)
        Me.pnlChart.ResumeLayout(False)
        Me.pnlBar.ResumeLayout(False)
        Me.tlpStats.ResumeLayout(False)
        Me.tlpCat.ResumeLayout(False)
        Me.cardActivity.ResumeLayout(False)
        Me.cardKpi1.PerformLayout()
        Me.cardKpi2.PerformLayout()
        Me.cardKpi3.PerformLayout()
        Me.cardKpi4.PerformLayout()
        Me.cardKpi5.PerformLayout()
        Me.cardKpi6.PerformLayout()
        Me.cardRevenue.PerformLayout()
        Me.cardReadiness.PerformLayout()
        Me.pnlTabs.PerformLayout()
        Me.pnlChart.PerformLayout()
        Me.pnlBar.PerformLayout()
        Me.cardActivity.PerformLayout()
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents lblTitle As Label
    Friend WithEvents lblSubtitle As Label
    Friend WithEvents btnExport As Button
    Friend WithEvents tlpKpi As TableLayoutPanel
    Friend WithEvents cardKpi1 As CardPanel
    Friend WithEvents lblKpiCaption1 As Label
    Friend WithEvents lblKpiIcon1 As Label
    Friend WithEvents lblKpiValue1 As Label
    Friend WithEvents lblKpiSub1 As Label
    Friend WithEvents cardKpi2 As CardPanel
    Friend WithEvents lblKpiCaption2 As Label
    Friend WithEvents lblKpiIcon2 As Label
    Friend WithEvents lblKpiValue2 As Label
    Friend WithEvents lblKpiSub2 As Label
    Friend WithEvents cardKpi3 As CardPanel
    Friend WithEvents lblKpiCaption3 As Label
    Friend WithEvents lblKpiIcon3 As Label
    Friend WithEvents lblKpiValue3 As Label
    Friend WithEvents lblKpiSub3 As Label
    Friend WithEvents cardKpi4 As CardPanel
    Friend WithEvents lblKpiCaption4 As Label
    Friend WithEvents lblKpiIcon4 As Label
    Friend WithEvents lblKpiValue4 As Label
    Friend WithEvents lblKpiSub4 As Label
    Friend WithEvents cardKpi5 As CardPanel
    Friend WithEvents lblKpiCaption5 As Label
    Friend WithEvents lblKpiIcon5 As Label
    Friend WithEvents lblKpiValue5 As Label
    Friend WithEvents lblKpiSub5 As Label
    Friend WithEvents cardKpi6 As CardPanel
    Friend WithEvents lblKpiCaption6 As Label
    Friend WithEvents lblKpiIcon6 As Label
    Friend WithEvents lblKpiValue6 As Label
    Friend WithEvents lblKpiSub6 As Label
    Friend WithEvents tlpMid As TableLayoutPanel
    Friend WithEvents cardRevenue As CardPanel
    Friend WithEvents cardReadiness As CardPanel
    Friend WithEvents lblRevTitle As Label
    Friend WithEvents lblRevSub As Label
    Friend WithEvents lblRevDate As Label
    Friend WithEvents pnlTabs As Panel
    Friend WithEvents btnToday As Button
    Friend WithEvents btnWeek As Button
    Friend WithEvents btnMonth As Button
    Friend WithEvents btnCustom As Button
    Friend WithEvents pnlChart As Panel
    Friend WithEvents lblCash As Label
    Friend WithEvents lblOutstanding As Label
    Friend WithEvents lblReadyTitle As Label
    Friend WithEvents lblReadySub As Label
    Friend WithEvents pnlBar As Panel
    Friend WithEvents tlpStats As TableLayoutPanel
    Friend WithEvents lblStatName1 As Label
    Friend WithEvents lblStatVal1 As Label
    Friend WithEvents lblStatName2 As Label
    Friend WithEvents lblStatVal2 As Label
    Friend WithEvents lblStatName3 As Label
    Friend WithEvents lblStatVal3 As Label
    Friend WithEvents lblStatName4 As Label
    Friend WithEvents lblStatVal4 As Label
    Friend WithEvents lblCatHeader As Label
    Friend WithEvents tlpCat As TableLayoutPanel
    Friend WithEvents lblCatName1 As Label
    Friend WithEvents lblCatVal1 As Label
    Friend WithEvents lblCatName2 As Label
    Friend WithEvents lblCatVal2 As Label
    Friend WithEvents lblCatName3 As Label
    Friend WithEvents lblCatVal3 As Label
    Friend WithEvents lblCatName4 As Label
    Friend WithEvents lblCatVal4 As Label
    Friend WithEvents cardActivity As CardPanel
    Friend WithEvents lblActTitle As Label
    Friend WithEvents btnViewAudit As Button
    Friend WithEvents dgvActivity As DataGridView
    Friend WithEvents colTimestamp As DataGridViewTextBoxColumn
    Friend WithEvents colStaff As DataGridViewTextBoxColumn
    Friend WithEvents colAction As DataGridViewTextBoxColumn
    Friend WithEvents colRecord As DataGridViewTextBoxColumn
    Friend WithEvents colResult As DataGridViewTextBoxColumn
    Friend WithEvents lblFooterOrg As Label
End Class
