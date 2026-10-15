<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ucRentalHistory
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
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.lblSubtitle = New System.Windows.Forms.Label()
        Me.cardBody = New CardPanel()
        Me.lblHint = New System.Windows.Forms.Label()
        Me.cardBody.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = True
        Me.lblTitle.BackColor = Color.Transparent
        Me.lblTitle.Font = New Font("Segoe UI", 24.0!, FontStyle.Bold)
        Me.lblTitle.ForeColor = Color.FromArgb(38, 38, 38)
        Me.lblTitle.Location = New Point(22, 18)
        Me.lblTitle.Text = "Rental History"
        Me.lblTitle.UseMnemonic = False
        '
        'lblSubtitle
        '
        Me.lblSubtitle.AutoSize = True
        Me.lblSubtitle.BackColor = Color.Transparent
        Me.lblSubtitle.Font = New Font("Segoe UI", 10.5!)
        Me.lblSubtitle.ForeColor = Color.FromArgb(120, 120, 120)
        Me.lblSubtitle.Location = New Point(25, 68)
        Me.lblSubtitle.Text = "Past rentals by customer, vehicle and date."
        Me.lblSubtitle.UseMnemonic = False
        '
        'cardBody
        '
        Me.cardBody.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        Me.cardBody.BackColor = Color.White
        Me.cardBody.Location = New Point(24, 112)
        Me.cardBody.Name = "cardBody"
        Me.cardBody.Size = New Size(1112, 620)
        Me.cardBody.Controls.Add(Me.lblHint)
        '
        'lblHint
        '
        Me.lblHint.AutoSize = True
        Me.lblHint.BackColor = Color.Transparent
        Me.lblHint.Font = New Font("Segoe UI", 11.0!)
        Me.lblHint.ForeColor = Color.FromArgb(150, 150, 150)
        Me.lblHint.Location = New Point(24, 24)
        Me.lblHint.Text = "Design the Rental History controls here."
        Me.lblHint.UseMnemonic = False
        '
        'ucRentalHistory
        '
        Me.AutoScaleDimensions = New SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.BackColor = Color.FromArgb(245, 244, 242)
        Me.Name = "ucRentalHistory"
        Me.Size = New Size(1160, 784)
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.lblSubtitle)
        Me.Controls.Add(Me.cardBody)
        Me.cardBody.ResumeLayout(False)
        Me.cardBody.PerformLayout()
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents lblTitle As Label
    Friend WithEvents lblSubtitle As Label
    Friend WithEvents cardBody As CardPanel
    Friend WithEvents lblHint As Label
End Class
