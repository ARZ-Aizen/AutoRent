Imports System.Drawing.Drawing2D

Public Class AdminDashboard

    Private ReadOnly ActiveColor As Color = Color.FromArgb(122, 0, 16)
    Private ReadOnly NormalColor As Color = Color.FromArgb(36, 36, 36)

    Private ReadOnly _menuButtons As New List(Of Button)
    Private ReadOnly _panels As New Dictionary(Of Button, UserControl)
    Private _active As Button

    Private _fullName As String = "Alex Morgan"
    Private _role As String = "System Administrator"

    Public Sub New(fullName As String, role As String)
        Me.New()
        _fullName = fullName
        _role = role
    End Sub

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub AdminDashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' user info
        lblUserName.Text = _fullName
        lblUserRole.Text = _role.ToUpper()
        lblAvatar.Text = Initials(_fullName)
        lblTopAvatar.Text = Initials(_fullName)

        ' date
        lblDay.Text = Date.Now.ToString("dddd").ToUpper()
        lblDate.Text = Date.Now.ToString("dd MMM yyyy")

        ' round shapes
        MakeCircle(lblAvatar)
        MakeCircle(lblTopAvatar)
        MakeRounded(picLogo, 10)

        ' menu buttons
        _menuButtons.AddRange({btnMenuOverview, btnMenuFleet, btnMenuMaintenance, btnMenuRates, btnMenuRevenue,
                               btnMenuDispatch, btnMenuUsers, btnMenuAccess, btnMenuAudit, btnMenuSettings})
        For Each b In _menuButtons
            AddHandler b.Click, AddressOf MenuButton_Click
        Next

        Navigate(btnMenuOverview)
    End Sub

    ' ---------------------------------------------------------------
    '  Navigation: each menu button shows its own UserControl panel
    ' ---------------------------------------------------------------
    Private Sub MenuButton_Click(sender As Object, e As EventArgs)
        Navigate(DirectCast(sender, Button))
    End Sub

    Private Sub Navigate(btn As Button)
        If btn Is _active Then Return

        ' create the panel the first time it is opened
        If Not _panels.ContainsKey(btn) Then
            Dim uc As UserControl = CreatePanel(btn)
            uc.Dock = DockStyle.Fill
            _panels(btn) = uc
            pnlContent.Controls.Add(uc)
        End If
        _panels(btn).BringToFront()

        ' highlight the active menu item
        For Each b In _menuButtons
            Dim isActive As Boolean = (b Is btn)
            b.BackColor = If(isActive, ActiveColor, NormalColor)
            b.ForeColor = If(isActive, Color.White, Color.FromArgb(205, 205, 205))
            b.Font = New Font("Segoe UI", 10.5F, If(isActive, FontStyle.Bold, FontStyle.Regular))
            b.FlatAppearance.MouseOverBackColor = If(isActive, ActiveColor, Color.FromArgb(60, 60, 60))
        Next
        _active = btn

        Dim title As String = btn.Tag.ToString()
        lblCrumb.Text = title & " / " & If(btn Is btnMenuOverview, "Admin Overview Dashboard", title)
    End Sub

    Private Function CreatePanel(btn As Button) As UserControl
        Select Case btn.Name
            Case "btnMenuOverview"
                Dim ov As New ucOverview()
                AddHandler ov.ViewAuditLogsRequested, Sub() Navigate(btnMenuAudit)
                Return ov
            Case "btnMenuFleet" : Return New ucFleetInventory()
            Case "btnMenuMaintenance" : Return New ucMaintenance()
            Case "btnMenuRates" : Return New ucRateConfiguration()
            Case "btnMenuRevenue" : Return New ucRevenueReports()
            Case "btnMenuDispatch" : Return New ucDispatchReports()
            Case "btnMenuUsers" : Return New ucUserManagement()
            Case "btnMenuAccess" : Return New ucAccessPermissions()
            Case "btnMenuAudit" : Return New ucAuditLogs()
            Case "btnMenuSettings" : Return New ucSystemSettings()
        End Select
        Return New UserControl()
    End Function

    ' bottom-left links
    Private Sub lblSettings_Click(sender As Object, e As EventArgs) Handles lblSettings.Click
        Navigate(btnMenuSettings)
    End Sub

    Private Sub lblLogout_Click(sender As Object, e As EventArgs) Handles lblLogout.Click
        If MessageBox.Show("Do you want to log out?", "Logout", MessageBoxButtons.YesNo,
                           MessageBoxIcon.Question) = DialogResult.Yes Then
            Me.Close()
        End If
    End Sub

    ' ---------------------------------------------------------------
    '  Helpers
    ' ---------------------------------------------------------------
    Private Shared Function Initials(name As String) As String
        Dim parts = name.Split({" "c}, StringSplitOptions.RemoveEmptyEntries)
        If parts.Length = 0 Then Return "?"
        If parts.Length = 1 Then Return parts(0).Substring(0, 1).ToUpper()
        Return (parts(0).Substring(0, 1) & parts(parts.Length - 1).Substring(0, 1)).ToUpper()
    End Function

    Private Shared Sub MakeCircle(c As Control)
        Using gp As New GraphicsPath()
            gp.AddEllipse(0, 0, c.Width, c.Height)
            c.Region = New Region(gp)
        End Using
    End Sub

    Private Shared Sub MakeRounded(c As Control, radius As Integer)
        Dim d As Integer = radius * 2
        Using gp As New GraphicsPath()
            gp.AddArc(0, 0, d, d, 180, 90)
            gp.AddArc(c.Width - d, 0, d, d, 270, 90)
            gp.AddArc(c.Width - d, c.Height - d, d, d, 0, 90)
            gp.AddArc(0, c.Height - d, d, d, 90, 90)
            gp.CloseFigure()
            c.Region = New Region(gp)
        End Using
    End Sub

End Class
