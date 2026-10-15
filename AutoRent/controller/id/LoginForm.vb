Public Class LoginForm

    Private Sub LoginForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
    End Sub

    Private Sub lblEye_Click(sender As Object, e As EventArgs) Handles lblEye.Click
        txtPassword.UseSystemPasswordChar = Not txtPassword.UseSystemPasswordChar
        lblEye.ForeColor = If(txtPassword.UseSystemPasswordChar, Color.Silver, Color.FromArgb(122, 0, 16))
    End Sub

    Private Sub btnSignIn_Click(sender As Object, e As EventArgs) Handles btnSignIn.Click
        If txtUsername.Text.Trim() = "" OrElse txtPassword.Text = "" Then
            MessageBox.Show("Please enter your username and password.", "Login",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            Dim fullName As String = "", role As String = ""
            If DbHelper.Authenticate(txtUsername.Text.Trim(), txtPassword.Text, fullName, role) Then
                Dim dash As Form
                If role.Trim().ToLower() = "admin" Then
                    dash = New AdminDashboard(fullName, role)
                Else
                    dash = New FrontDeskDashboard(fullName, role)
                End If
                AddHandler dash.FormClosed, Sub()
                                                txtPassword.Clear()
                                                Me.Show()
                                            End Sub
                dash.Show()
                Me.Hide()
            Else
                MessageBox.Show("Invalid username/user ID or password.", "Login failed",
                                MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtPassword.Clear()
                txtPassword.Focus()
            End If
        Catch ex As Exception
            MessageBox.Show("Database error: " & ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub lnkForgot_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lnkForgot.LinkClicked
        MessageBox.Show("Please contact your administrator to reset your password.", "Forgot Password")
    End Sub

    Private Sub LoginForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Enter Then btnSignIn.PerformClick()
    End Sub

End Class