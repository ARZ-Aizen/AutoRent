' Add Vehicle / Edit Vehicle form (one control, two modes). All fields are empty.
' Needs ucVehicleDetails.vb in the project (it holds the FiCard class used here).
Public Class ucVehicleForm
    Inherits UserControl

    Private Shared ReadOnly Maroon As Color = Color.FromArgb(122, 0, 16)
    Private Shared ReadOnly Ink As Color = Color.FromArgb(38, 38, 38)
    Private Shared ReadOnly Muted As Color = Color.FromArgb(120, 120, 120)
    Private Shared ReadOnly BorderCol As Color = Color.FromArgb(228, 225, 220)
    Private Shared ReadOnly PageBg As Color = Color.FromArgb(245, 244, 242)

    Public Event SaveClicked()
    Public Event CancelClicked()
    Public Event BackClicked()
    Public Event KeepActiveClicked()
    Public Event RetireConfirmed()

    <System.ComponentModel.Browsable(False), System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
    Public Property IsEditMode As Boolean = False

    <System.ComponentModel.Browsable(False), System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
    Public Property PhotoPath As String = ""

    ' ---- header ----
    Private ReadOnly lblTitle As New Label
    Public ReadOnly lblSubtitle As New Label
    Public ReadOnly lblStatusBadge As New Label
    Public ReadOnly btnBack As New Button

    ' ---- fields ----
    Public ReadOnly txtVehicleId As New TextBox
    Public ReadOnly txtPlate As New TextBox
    Public ReadOnly cboYear As New ComboBox
    Public ReadOnly cboMake As New ComboBox
    Public ReadOnly txtModel As New TextBox
    Public ReadOnly cboCategory As New ComboBox
    Public ReadOnly txtColor As New TextBox
    Public ReadOnly cboFuel As New ComboBox
    Public ReadOnly cboTransmission As New ComboBox
    Public ReadOnly txtSeats As New TextBox
    Public ReadOnly txtOdometer As New TextBox
    Public ReadOnly dtpAcquired As New DateTimePicker
    Public ReadOnly txtRate As New TextBox
    Public ReadOnly dtpRegExpiry As New DateTimePicker
    Public ReadOnly dtpInsExpiry As New DateTimePicker
    Public ReadOnly txtNotes As New TextBox

    ' ---- hints under fields (set .Text) ----
    Public ReadOnly lblHintId As New Label
    Public ReadOnly lblHintPlate As New Label
    Public ReadOnly lblHintOdometer As New Label
    Public ReadOnly lblHintRate As New Label
    Public ReadOnly lblHintInsurance As New Label

    ' ---- photo ----
    Public ReadOnly picVehicle As New PictureBox
    Public ReadOnly btnPhoto As New Button
    Public ReadOnly lblPhotoSub As New Label
    Public ReadOnly lblPhotoHint As New Label

    ' ---- edit mode: retire card ----
    Public ReadOnly lblRetireSub As New Label
    Public ReadOnly chkNoRental As New CheckBox
    Public ReadOnly chkNoObligations As New CheckBox
    Public ReadOnly lblRetireWarning As New Label
    Public ReadOnly btnKeepActive As New Button
    Public ReadOnly btnConfirmRetire As New Button

    ' ---- add mode: checklist card ----
    Public ReadOnly chkVerifyId As New CheckBox
    Public ReadOnly chkVerifyDocs As New CheckBox
    Public ReadOnly chkReviewRate As New CheckBox
    Public ReadOnly lblSaveErrorTitle As New Label
    Public ReadOnly lblSaveError As New Label

    ' ---- footer ----
    Private ReadOnly lblAuditTitle As New Label
    Public ReadOnly lblAuditNote As New Label
    Public ReadOnly lblFooterNote As New Label
    Public ReadOnly btnCancel As New Button
    Public ReadOnly btnSave As New Button

    Private Class FieldItem
        Public Cap As Label
        Public Ctl As Control
        Public Hint As Label
        Public Row As Integer
        Public Col As Integer
        Public Span As Integer = 1
    End Class

    Private ReadOnly _fields As New List(Of FieldItem)
    Private cInfo, cPhoto, cRetire, cSave, cFooter As FiCard
    Private pnlRetireWarn, pnlSaveError As Panel

    Public Sub New()
        Me.BackColor = PageBg
        Me.AutoScroll = True
        Me.AutoScrollMinSize = New Size(900, 720)
        BuildUI()
        SetMode(False)
        ClearForm()
    End Sub

    ' ---------------- public ----------------
    Public Sub SetMode(isEdit As Boolean)
        IsEditMode = isEdit
        lblTitle.Text = If(isEdit, "Edit Vehicle", "Add Vehicle")
        lblStatusBadge.Visible = isEdit
        btnBack.Visible = Not isEdit
        cRetire.Visible = isEdit
        cSave.Visible = Not isEdit
        btnSave.Text = If(isEdit, "✓  Update Vehicle", "✓  Save Vehicle")
        btnPhoto.Text = If(isEdit, "Replace photo", "Upload vehicle photo")
        lblAuditTitle.Text = If(isEdit, "All changes are attributed and auditable", "Authorized vehicle creation")
        If Not isEdit Then
            txtVehicleId.Text = "Assigned on save"
        End If
    End Sub

    Public Sub ClearForm()
        For Each t As TextBox In New TextBox() {txtPlate, txtModel, txtColor, txtSeats, txtOdometer, txtRate, txtNotes}
            t.Text = ""
        Next
        For Each c As ComboBox In New ComboBox() {cboYear, cboMake, cboCategory, cboFuel, cboTransmission}
            c.SelectedIndex = -1
            c.Text = ""
        Next
        dtpAcquired.Value = Date.Today : dtpAcquired.Checked = False
        dtpRegExpiry.Value = Date.Today
        dtpInsExpiry.Value = Date.Today
        For Each k As CheckBox In New CheckBox() {chkNoRental, chkNoObligations, chkVerifyId, chkVerifyDocs, chkReviewRate}
            k.Checked = False
        Next
        picVehicle.Image = Nothing
        PhotoPath = ""
        lblSaveError.Text = ""
        lblSaveErrorTitle.Text = ""
        pnlSaveError.Visible = False
        txtVehicleId.Text = If(IsEditMode, "", "Assigned on save")
    End Sub

    ' ---------------- helpers ----------------
    Private Shared Function F(size As Single, Optional style As FontStyle = FontStyle.Regular) As Font
        Return New Font("Segoe UI", size, style)
    End Function

    Private Shared Function Cap(text As String, f As Font, c As Color, x As Integer, y As Integer) As Label
        Return New Label With {.Text = text, .Font = f, .ForeColor = c, .BackColor = Color.Transparent,
                               .AutoSize = True, .Location = New Point(x, y), .UseMnemonic = False}
    End Function

    Private Shared Sub Spot(l As Label, f As Font, c As Color, x As Integer, y As Integer)
        l.Font = f : l.ForeColor = c : l.BackColor = Color.Transparent
        l.AutoSize = True : l.Location = New Point(x, y) : l.UseMnemonic = False
    End Sub

    Private Shared Sub StyleBtn(b As Button, text As String, w As Integer, h As Integer, primary As Boolean)
        b.Text = text : b.Size = New Size(w, h) : b.FlatStyle = FlatStyle.Flat
        b.Font = F(9) : b.Cursor = Cursors.Hand : b.UseVisualStyleBackColor = False : b.UseMnemonic = False
        b.BackColor = If(primary, Maroon, Color.White)
        b.ForeColor = If(primary, Color.White, Ink)
        b.FlatAppearance.BorderColor = If(primary, Maroon, BorderCol)
        b.FlatAppearance.BorderSize = 1
    End Sub

    Private Shared Sub StyleCheck(c As CheckBox)
        c.AutoSize = True : c.Font = F(8.5!) : c.ForeColor = Ink
        c.BackColor = Color.Transparent : c.UseMnemonic = False
    End Sub

    Private Sub AddField(caption As String, ctl As Control, hint As Label, row As Integer, col As Integer, Optional span As Integer = 1)
        Dim fi As New FieldItem With {.Cap = Cap(caption, F(8, FontStyle.Bold), Ink, 0, 0), .Ctl = ctl, .Hint = hint,
                                      .Row = row, .Col = col, .Span = span}
        If hint IsNot Nothing Then Spot(hint, F(7.5!), Muted, 0, 0)
        If TypeOf ctl Is TextBox Then
            DirectCast(ctl, TextBox).BorderStyle = BorderStyle.FixedSingle
            ctl.Font = F(9)
        ElseIf TypeOf ctl Is ComboBox Then
            Dim cb = DirectCast(ctl, ComboBox)
            cb.FlatStyle = FlatStyle.Flat
            cb.BackColor = Color.White
            cb.Font = F(9)
            If cb IsNot cboMake Then cb.DropDownStyle = ComboBoxStyle.DropDownList
        ElseIf TypeOf ctl Is DateTimePicker Then
            Dim d = DirectCast(ctl, DateTimePicker)
            d.Format = DateTimePickerFormat.Custom
            d.CustomFormat = "dd MMM yyyy"
            d.Font = F(9)
        End If
        cInfo.Controls.Add(fi.Cap)
        cInfo.Controls.Add(ctl)
        If hint IsNot Nothing Then cInfo.Controls.Add(hint)
        _fields.Add(fi)
    End Sub

    ' ---------------- layout ----------------
    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e)
        ArrangeLayout()
    End Sub

    Private Sub ArrangeLayout()
        If cFooter Is Nothing Then Return
        Dim w As Integer = Math.Max(ClientSize.Width, 900) - 48
        Dim leftW As Integer = w - 316
        Dim rx As Integer = 24 + leftW + 16
        btnBack.Location = New Point(24 + w - btnBack.Width, 22)
        lblStatusBadge.Location = New Point(24 + w - lblStatusBadge.Width, 34)
        cInfo.SetBounds(24, 96, leftW, 452)
        cPhoto.SetBounds(rx, 96, 300, 262)
        cRetire.SetBounds(rx, 374, 300, 220)
        cSave.SetBounds(rx, 374, 300, 220)
        lblAuditTitle.Location = New Point(24, 566)
        lblAuditNote.Location = New Point(24, 584)
        cFooter.SetBounds(24, 624, w, 64)
    End Sub

    Private Sub BuildUI()
        Me.SuspendLayout()

        ' ----- header -----
        Spot(lblTitle, F(20, FontStyle.Bold), Ink, 22, 14)
        Spot(lblSubtitle, F(9), Muted, 26, 58)
        lblStatusBadge.AutoSize = False : lblStatusBadge.Size = New Size(80, 20)
        lblStatusBadge.Font = F(8, FontStyle.Bold) : lblStatusBadge.ForeColor = Color.FromArgb(46, 110, 60)
        lblStatusBadge.BackColor = Color.Transparent : lblStatusBadge.TextAlign = ContentAlignment.MiddleRight
        StyleBtn(btnBack, "←  Back to inventory", 170, 34, False)
        Controls.AddRange(New Control() {lblTitle, lblSubtitle, lblStatusBadge, btnBack})

        ' ----- vehicle information card -----
        cInfo = New FiCard
        cInfo.Controls.Add(Cap("Vehicle information", F(11, FontStyle.Bold), Ink, 18, 12))
        cInfo.Controls.Add(Cap("* Required fields · identity, specifications and compliance", F(7.5!), Muted, 18, 36))

        txtVehicleId.ReadOnly = True : txtVehicleId.BackColor = Color.FromArgb(238, 238, 236)
        For y As Integer = Date.Now.Year + 1 To 1990 Step -1
            cboYear.Items.Add(y.ToString())
        Next
        cboCategory.Items.AddRange(New Object() {"Sedan", "SUV", "Van", "Pickup"})
        cboFuel.Items.AddRange(New Object() {"Petrol", "Diesel", "Hybrid", "Electric"})
        cboTransmission.Items.AddRange(New Object() {"Automatic", "Manual"})

        AddField("Vehicle ID", txtVehicleId, lblHintId, 0, 0)
        AddField("Plate Number *", txtPlate, lblHintPlate, 0, 1)
        AddField("Year *", cboYear, Nothing, 0, 2)
        AddField("Make *", cboMake, Nothing, 1, 0)
        AddField("Model *", txtModel, Nothing, 1, 1)
        AddField("Vehicle Category *", cboCategory, Nothing, 1, 2)
        AddField("Color", txtColor, Nothing, 2, 0)
        AddField("Fuel Type *", cboFuel, Nothing, 2, 1)
        AddField("Transmission *", cboTransmission, Nothing, 2, 2)
        AddField("Seating Capacity *", txtSeats, Nothing, 3, 0)
        AddField("Odometer Reading (km) *", txtOdometer, lblHintOdometer, 3, 1)
        AddField("Acquisition Date (optional)", dtpAcquired, Nothing, 3, 2)
        dtpAcquired.ShowCheckBox = True
        AddField("Base Daily Rate (USD) *", txtRate, lblHintRate, 4, 0)
        AddField("Registration Expiration *", dtpRegExpiry, Nothing, 4, 1)
        AddField("Insurance Expiration *", dtpInsExpiry, lblHintInsurance, 4, 2)
        AddField("Vehicle Notes", txtNotes, Nothing, 5, 0, 3)

        AddHandler cInfo.Resize, Sub(s, e)
                                     Dim colW As Integer = (cInfo.Width - 36 - 28) \ 3
                                     For Each fi In _fields
                                         Dim x As Integer = 18 + fi.Col * (colW + 14)
                                         Dim y As Integer = 62 + fi.Row * 66
                                         Dim wd As Integer = colW * fi.Span + 14 * (fi.Span - 1)
                                         fi.Cap.Location = New Point(x, y)
                                         fi.Ctl.SetBounds(x, y + 18, wd, 26)
                                         If fi.Hint IsNot Nothing Then fi.Hint.Location = New Point(x, y + 46)
                                     Next
                                 End Sub
        Controls.Add(cInfo)

        ' ----- photo card -----
        cPhoto = New FiCard
        cPhoto.Controls.Add(Cap("Vehicle photo", F(11, FontStyle.Bold), Ink, 18, 12))
        Spot(lblPhotoSub, F(7.5!), Muted, 18, 36)
        cPhoto.Controls.Add(lblPhotoSub)
        picVehicle.SizeMode = PictureBoxSizeMode.Zoom
        picVehicle.BackColor = Color.White
        picVehicle.SetBounds(18, 58, 264, 120)
        cPhoto.Controls.Add(picVehicle)
        StyleBtn(btnPhoto, "Upload vehicle photo", 264, 30, False)
        btnPhoto.Location = New Point(18, 190)
        cPhoto.Controls.Add(btnPhoto)
        lblPhotoHint.AutoSize = False : lblPhotoHint.Font = F(7.5!) : lblPhotoHint.ForeColor = Muted
        lblPhotoHint.BackColor = Color.Transparent : lblPhotoHint.SetBounds(18, 226, 264, 30)
        lblPhotoHint.UseMnemonic = False
        cPhoto.Controls.Add(lblPhotoHint)
        Controls.Add(cPhoto)

        ' ----- edit mode: retire card -----
        cRetire = New FiCard
        cRetire.Controls.Add(Cap("Retire vehicle?", F(11, FontStyle.Bold), Ink, 18, 12))
        Spot(lblRetireSub, F(7.5!), Muted, 18, 36)
        cRetire.Controls.Add(lblRetireSub)
        chkNoRental.Text = "No active rental or reserved booking" : chkNoRental.Location = New Point(18, 62)
        chkNoObligations.Text = "No unresolved financial or safety obligations" : chkNoObligations.Location = New Point(18, 86)
        StyleCheck(chkNoRental) : StyleCheck(chkNoObligations)
        cRetire.Controls.AddRange(New Control() {chkNoRental, chkNoObligations})

        pnlRetireWarn = New Panel With {.BackColor = Color.FromArgb(252, 243, 228), .Location = New Point(18, 114), .Size = New Size(264, 58)}
        pnlRetireWarn.Controls.Add(Cap("History is retained permanently", F(8, FontStyle.Bold), Color.FromArgb(176, 112, 32), 8, 6))
        lblRetireWarning.AutoSize = False : lblRetireWarning.Font = F(7.5!) : lblRetireWarning.ForeColor = Muted
        lblRetireWarning.BackColor = Color.Transparent : lblRetireWarning.SetBounds(8, 24, 248, 32)
        lblRetireWarning.UseMnemonic = False
        pnlRetireWarn.Controls.Add(lblRetireWarning)
        cRetire.Controls.Add(pnlRetireWarn)

        StyleBtn(btnKeepActive, "Keep active", 90, 28, False)
        StyleBtn(btnConfirmRetire, "Confirm retirement", 140, 28, False)
        btnConfirmRetire.ForeColor = Maroon : btnConfirmRetire.FlatAppearance.BorderColor = Maroon
        btnKeepActive.Location = New Point(18, 182)
        btnConfirmRetire.Location = New Point(116, 182)
        cRetire.Controls.AddRange(New Control() {btnKeepActive, btnConfirmRetire})
        Controls.Add(cRetire)

        ' ----- add mode: checklist card -----
        cSave = New FiCard
        cSave.Controls.Add(Cap("Before you save", F(11, FontStyle.Bold), Ink, 18, 12))
        chkVerifyId.Text = "Verify unique vehicle ID and plate" : chkVerifyId.Location = New Point(18, 44)
        chkVerifyDocs.Text = "Confirm valid registration and insurance" : chkVerifyDocs.Location = New Point(18, 68)
        chkReviewRate.Text = "Review base rate and vehicle category" : chkReviewRate.Location = New Point(18, 92)
        StyleCheck(chkVerifyId) : StyleCheck(chkVerifyDocs) : StyleCheck(chkReviewRate)
        cSave.Controls.AddRange(New Control() {chkVerifyId, chkVerifyDocs, chkReviewRate})

        pnlSaveError = New Panel With {.BackColor = Color.FromArgb(250, 228, 230), .Location = New Point(18, 124), .Size = New Size(264, 56)}
        Spot(lblSaveErrorTitle, F(8, FontStyle.Bold), Maroon, 8, 6)
        lblSaveError.AutoSize = False : lblSaveError.Font = F(7.5!) : lblSaveError.ForeColor = Muted
        lblSaveError.BackColor = Color.Transparent : lblSaveError.SetBounds(8, 24, 248, 30)
        lblSaveError.UseMnemonic = False
        pnlSaveError.Controls.AddRange(New Control() {lblSaveErrorTitle, lblSaveError})
        cSave.Controls.Add(pnlSaveError)
        Controls.Add(cSave)

        ' ----- audit note + footer -----
        Spot(lblAuditTitle, F(8.5!, FontStyle.Bold), Muted, 24, 566)
        Spot(lblAuditNote, F(8), Muted, 24, 584)
        Controls.AddRange(New Control() {lblAuditTitle, lblAuditNote})

        cFooter = New FiCard
        Spot(lblFooterNote, F(8), Muted, 18, 24)
        cFooter.Controls.Add(lblFooterNote)
        StyleBtn(btnCancel, "Cancel", 80, 34, False)
        StyleBtn(btnSave, "✓  Update Vehicle", 150, 34, True)
        cFooter.Controls.AddRange(New Control() {btnCancel, btnSave})
        AddHandler cFooter.Resize, Sub(s, e)
                                       btnSave.Location = New Point(cFooter.Width - 18 - btnSave.Width, 15)
                                       btnCancel.Location = New Point(btnSave.Left - 10 - btnCancel.Width, 15)
                                   End Sub
        Controls.Add(cFooter)

        AddHandler btnBack.Click, Sub(s, e) RaiseEvent BackClicked()
        AddHandler btnCancel.Click, Sub(s, e) RaiseEvent CancelClicked()
        AddHandler btnSave.Click, Sub(s, e) RaiseEvent SaveClicked()
        AddHandler btnKeepActive.Click, Sub(s, e) RaiseEvent KeepActiveClicked()
        AddHandler btnConfirmRetire.Click, Sub(s, e) RaiseEvent RetireConfirmed()
        AddHandler btnPhoto.Click, AddressOf ChoosePhoto

        Me.ResumeLayout(False)
        ArrangeLayout()
    End Sub

    ' shows/hides the red message box in "Before you save"
    Public Sub ShowSaveError(title As String, message As String)
        lblSaveErrorTitle.Text = title
        lblSaveError.Text = message
        pnlSaveError.Visible = (title <> "" OrElse message <> "")
    End Sub

    Private Sub ChoosePhoto(sender As Object, e As EventArgs)
        Using dlg As New OpenFileDialog With {.Filter = "Images|*.jpg;*.jpeg;*.png;*.webp;*.bmp", .Title = "Choose vehicle photo"}
            If dlg.ShowDialog() = DialogResult.OK Then
                Using img As Image = Image.FromFile(dlg.FileName)
                    picVehicle.Image = New Bitmap(img)
                End Using
                PhotoPath = dlg.FileName
            End If
        End Using
    End Sub
End Class