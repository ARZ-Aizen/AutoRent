' System Settings. Needs SK.vb and ucVehicleDetails.vb (FiCard).
' Every setting is a key -> control pair (see Fields). The dashboard loads / saves them.
Public Class ucSettingsPage
    Inherits UserControl

    Public Event SaveClicked()
    Public Event DiscardClicked()

    Public ReadOnly lblSubtitle As New Label
    Public ReadOnly lblUnsaved As New Label

    ' ---- agency ----
    Public ReadOnly txtAgencyName As New TextBox
    Public ReadOnly txtAgencyId As New TextBox
    Public ReadOnly txtBranchName As New TextBox
    Public ReadOnly txtBranchCode As New TextBox
    Public ReadOnly txtBranchAddress As New TextBox
    Public ReadOnly txtSupportEmail As New TextBox
    Public ReadOnly lblAgencySub As New Label

    ' ---- billing ----
    Public ReadOnly cboCurrency As New ComboBox
    Public ReadOnly cboTimezone As New ComboBox
    Public ReadOnly cboBillingRule As New ComboBox
    Public ReadOnly txtGrace As New TextBox
    Public ReadOnly txtHours As New TextBox
    Public ReadOnly txtHolidays As New TextBox
    Public ReadOnly lblBillingNote As New Label

    ' ---- documents ----
    Public ReadOnly cboContract As New ComboBox
    Public ReadOnly cboReportFormat As New ComboBox
    Public ReadOnly chkWatermark As New CheckBox

    ' ---- notifications ----
    Public ReadOnly chkMaintenance As New CheckBox
    Public ReadOnly chkOverdue As New CheckBox
    Public ReadOnly chkExpiry As New CheckBox
    Public ReadOnly chkDigest As New CheckBox
    Public ReadOnly chkSensitive As New CheckBox
    Public ReadOnly txtQuiet As New TextBox

    ' ---- confirm sensitive change ----
    Public ReadOnly lblDraft As New Label
    Public ReadOnly lblWarnTitle As New Label
    Public ReadOnly lblWarnNote As New Label
    Public ReadOnly chkReviewed As New CheckBox
    Public ReadOnly btnKeep As New Button
    Public ReadOnly btnConfirm As New Button

    ' ---- history ----
    Public ReadOnly lblVersion As New Label
    Public ReadOnly lblSavedBy As New Label
    Public ReadOnly lblSavedAt As New Label
    Public ReadOnly lblHistNote As New Label

    ' ---- footer ----
    Public ReadOnly lblFootNote As New Label
    Public ReadOnly btnDiscard As New Button
    Public ReadOnly btnSave As New Button

    ' key -> control. The keys are the SettingKey values stored in the database.
    Public ReadOnly Fields As New Dictionary(Of String, Control)
    Private ReadOnly _labels As New Dictionary(Of String, String)
    Private ReadOnly _sensitive As New HashSet(Of String)
    Private ReadOnly _baseline As New Dictionary(Of String, String)
    Private ReadOnly _caps As New Dictionary(Of Control, Label)
    Private ReadOnly _tabs As New List(Of Button)
    Private ReadOnly _tabTargets As New List(Of Control)
    Private _loading As Boolean = False

    Private cAgency, cBilling, cDoc, cNotif, cConfirm, cHist As FiCard
    Private pnlWarn As Panel
    Private lblTopTitle, lblAgencyTitle, lblBillingTitle, lblDocTitle, lblNotifTitle, lblConfirmTitle, lblHistTitle As Label
    Private lblQuietCap, capVersion, capSavedBy, capSavedAt As Label
    Private ReadOnly pnl As New Panel

    Public Sub New()
        Me.BackColor = SK.PageBg
        BuildUI()
        UpdateDirty()
    End Sub

    ' ---------------- public ----------------
    Public Sub SetValues(values As Dictionary(Of String, String))
        _loading = True
        For Each kv In Fields
            Dim v As String = If(values.ContainsKey(kv.Key), values(kv.Key), "")
            SetValue(kv.Value, v)
        Next
        _baseline.Clear()
        For Each kv In Fields
            _baseline(kv.Key) = GetValue(kv.Value)
        Next
        chkReviewed.Checked = False
        _loading = False
        UpdateDirty()
    End Sub

    Public Function GetValues() As Dictionary(Of String, String)
        Dim d As New Dictionary(Of String, String)
        For Each kv In Fields
            d(kv.Key) = GetValue(kv.Value)
        Next
        Return d
    End Function

    Public Function ChangedKeys() As List(Of String)
        Dim l As New List(Of String)
        For Each kv In Fields
            If _baseline.ContainsKey(kv.Key) AndAlso _baseline(kv.Key) <> GetValue(kv.Value) Then l.Add(kv.Key)
        Next
        Return l
    End Function

    Public Function BaselineValue(key As String) As String
        Return If(_baseline.ContainsKey(key), _baseline(key), "")
    End Function

    Public Function NeedsReview() As Boolean
        Return ChangedKeys().Any(Function(k) _sensitive.Contains(k))
    End Function

    Public Function FriendlyName(key As String) As String
        Return If(_labels.ContainsKey(key), _labels(key), key)
    End Function

    Public Sub UpdateDirty()
        If _loading Then Return
        Dim ch As List(Of String) = ChangedKeys()
        lblUnsaved.Text = If(ch.Count = 0, "", "Unsaved changes · " & ch.Count & " field" & If(ch.Count = 1, "", "s"))
        lblFootNote.Text = If(ch.Count = 0, "No unsaved changes", "Unsaved: " & String.Join(", ", ch.Select(Function(k) FriendlyName(k).ToLower())))
        btnSave.Enabled = ch.Count > 0
        btnDiscard.Enabled = ch.Count > 0
        Dim sens As List(Of String) = ch.Where(Function(k) _sensitive.Contains(k)).ToList()
        pnlWarn.Visible = sens.Count > 0
        If sens.Count > 0 Then
            Dim k As String = sens(0)
            lblDraft.Text = "Draft: " & FriendlyName(k).ToLower() & "  " & _baseline(k) & " → " & GetValue(Fields(k))
        Else
            lblDraft.Text = "No sensitive changes pending"
        End If
        btnConfirm.Enabled = sens.Count > 0
    End Sub

    ' ---------------- value helpers ----------------
    Private Shared Function GetValue(c As Control) As String
        If TypeOf c Is CheckBox Then Return If(DirectCast(c, CheckBox).Checked, "1", "0")
        Return c.Text
    End Function

    Private Shared Sub SetValue(c As Control, v As String)
        If TypeOf c Is CheckBox Then
            DirectCast(c, CheckBox).Checked = (v = "1")
        ElseIf TypeOf c Is ComboBox Then
            Dim cb As ComboBox = DirectCast(c, ComboBox)
            If v <> "" AndAlso cb.FindStringExact(v) < 0 Then cb.Items.Add(v)
            cb.SelectedIndex = If(v = "", -1, cb.FindStringExact(v))
        Else
            c.Text = v
        End If
    End Sub

    ' ---------------- build ----------------
    Private Sub Reg(key As String, label As String, c As Control, Optional sensitive As Boolean = False)
        Fields(key) = c : _labels(key) = label
        If sensitive Then _sensitive.Add(key)
        If TypeOf c Is TextBox Then
            AddHandler DirectCast(c, TextBox).TextChanged, Sub(s, e) UpdateDirty()
        ElseIf TypeOf c Is ComboBox Then
            AddHandler DirectCast(c, ComboBox).SelectedIndexChanged, Sub(s, e) UpdateDirty()
        ElseIf TypeOf c Is CheckBox Then
            AddHandler DirectCast(c, CheckBox).CheckedChanged, Sub(s, e) UpdateDirty()
        End If
    End Sub

    Private Sub AddField(card As Control, caption As String, c As Control)
        Dim lb As Label = SK.Cap(caption, SK.F(8, FontStyle.Bold), SK.Ink, 0, 0)
        lb.AutoSize = False
        _caps(c) = lb
        card.Controls.Add(lb)
        If TypeOf c Is TextBox Then SK.StyleText(DirectCast(c, TextBox))
        If TypeOf c Is ComboBox Then SK.StyleCombo(DirectCast(c, ComboBox))
        card.Controls.Add(c)
    End Sub

    Private Sub Place(c As Control, x As Integer, y As Integer, w As Integer)
        _caps(c).SetBounds(x, y, w, 16)
        c.SetBounds(x, y + 18, w, 26)
    End Sub

    Private Sub StyleCheck(k As CheckBox, text As String)
        k.Text = text : k.AutoSize = False : k.BackColor = Color.Transparent : k.Font = SK.F(8) : k.ForeColor = SK.Ink
    End Sub

    Private Sub Arrange()
        Dim w As Integer = Math.Max(pnl.ClientSize.Width, 900) - 48
        lblUnsaved.SetBounds(24 + w - 220, 20, 220, 16)
        Dim tx As Integer = 24
        For Each t As Button In _tabs
            t.Location = New Point(tx, 84)
            tx += t.Width + 4
        Next

        Dim lw As Integer = CInt((w - 16) * 0.66)
        Dim rw As Integer = w - 16 - lw
        Dim cw As Integer = (lw - 32 - 16) \ 2
        Dim y As Integer = 126

        cAgency.SetBounds(24, y, lw, 258)
        lblAgencyTitle.SetBounds(16, 10, lw - 32, 20) : lblAgencySub.SetBounds(16, 30, lw - 32, 16)
        Place(txtAgencyName, 16, 54, cw) : Place(txtAgencyId, 16 + cw + 16, 54, cw)
        Place(txtBranchName, 16, 118, cw) : Place(txtBranchCode, 16 + cw + 16, 118, cw)
        Place(txtBranchAddress, 16, 182, cw) : Place(txtSupportEmail, 16 + cw + 16, 182, cw)

        Dim by As Integer = y + 258 + 14
        cBilling.SetBounds(24, by, lw, 284)
        lblBillingTitle.SetBounds(16, 10, lw - 32, 20)
        Place(cboCurrency, 16, 40, cw) : Place(cboTimezone, 16 + cw + 16, 40, cw)
        Place(cboBillingRule, 16, 104, cw) : Place(txtGrace, 16 + cw + 16, 104, cw)
        Place(txtHours, 16, 168, cw) : Place(txtHolidays, 16 + cw + 16, 168, cw)
        lblBillingNote.SetBounds(16, 232, lw - 32, 34)

        Dim dy As Integer = by + 284 + 14
        cDoc.SetBounds(24, dy, lw, 150)
        lblDocTitle.SetBounds(16, 10, lw - 32, 20)
        Place(cboContract, 16, 40, cw) : Place(cboReportFormat, 16 + cw + 16, 40, cw)
        chkWatermark.SetBounds(16, 108, lw - 32, 20)

        ' right column
        Dim rx As Integer = 24 + lw + 16
        Dim riw As Integer = rw - 32
        cNotif.SetBounds(rx, y, rw, 228)
        lblNotifTitle.SetBounds(16, 10, riw, 20)
        Dim ky As Integer = 38
        For Each k As CheckBox In New CheckBox() {chkMaintenance, chkOverdue, chkExpiry, chkDigest, chkSensitive}
            k.SetBounds(16, ky, riw, 20) : ky += 24
        Next
        lblQuietCap.SetBounds(16, ky + 8, 100, 16)
        txtQuiet.SetBounds(riw - 90 + 16, ky + 4, 90, 24)

        Dim cy As Integer = y + 228 + 14
        cConfirm.SetBounds(rx, cy, rw, 232)
        lblConfirmTitle.SetBounds(16, 10, riw, 20)
        lblDraft.SetBounds(16, 32, riw, 16)
        pnlWarn.SetBounds(16, 54, riw, 66)
        lblWarnTitle.SetBounds(10, 6, riw - 20, 16) : lblWarnNote.SetBounds(10, 24, riw - 20, 40)
        chkReviewed.SetBounds(16, 130, riw, 20)
        btnKeep.Location = New Point(16, 162)
        btnConfirm.Location = New Point(16 + btnKeep.Width + 8, 162)

        Dim hy As Integer = cy + 232 + 14
        cHist.SetBounds(rx, hy, rw, 186)
        lblHistTitle.SetBounds(16, 10, riw, 20)
        capVersion.SetBounds(16, 40, riw \ 2, 16) : lblVersion.SetBounds(16 + riw \ 2, 40, riw - riw \ 2, 16)
        capSavedBy.SetBounds(16, 66, riw \ 2, 16) : lblSavedBy.SetBounds(16 + riw \ 2, 66, riw - riw \ 2, 16)
        capSavedAt.SetBounds(16, 92, riw \ 2, 16) : lblSavedAt.SetBounds(16 + riw \ 2, 92, riw - riw \ 2, 16)
        lblHistNote.SetBounds(16, 122, riw, 54)

        Dim fy As Integer = Math.Max(dy + 150, hy + 186) + 16
        lblFootNote.SetBounds(24, fy + 8, w - 300, 16)
        btnSave.Location = New Point(24 + w - btnSave.Width, fy)
        btnDiscard.Location = New Point(btnSave.Left - 8 - btnDiscard.Width, fy)
        pnl.AutoScrollMinSize = New Size(900, fy + 60)
    End Sub

    Private Function MakeCard(title As Label, text As String) As FiCard
        Dim c As New FiCard
        SK.Spot(title, SK.F(10.5!, FontStyle.Bold), SK.Ink)
        title.Text = text
        c.Controls.Add(title)
        pnl.Controls.Add(c)
        Return c
    End Function

    Private Sub BuildUI()
        Me.SuspendLayout()
        pnl.Dock = DockStyle.Fill : pnl.BackColor = SK.PageBg : pnl.AutoScroll = True
        Controls.Add(pnl)

        lblTopTitle = SK.Cap("System Settings", SK.F(20, FontStyle.Bold), SK.Ink, 22, 14)
        pnl.Controls.Add(lblTopTitle)
        SK.Spot(lblSubtitle, SK.F(9), SK.Muted) : lblSubtitle.SetBounds(26, 58, 800, 18)
        SK.Spot(lblUnsaved, SK.F(8, FontStyle.Bold), Color.FromArgb(176, 112, 32), ContentAlignment.TopRight)
        pnl.Controls.AddRange(New Control() {lblSubtitle, lblUnsaved})

        lblAgencyTitle = New Label : lblBillingTitle = New Label : lblDocTitle = New Label
        lblNotifTitle = New Label : lblConfirmTitle = New Label : lblHistTitle = New Label

        ' cards
        cAgency = MakeCard(lblAgencyTitle, "Agency & branch information")
        SK.Spot(lblAgencySub, SK.F(8), SK.Muted) : cAgency.Controls.Add(lblAgencySub)
        AddField(cAgency, "Agency name *", txtAgencyName) : Reg("AgencyName", "Agency name", txtAgencyName)
        AddField(cAgency, "Agency identifier", txtAgencyId) : Reg("AgencyId", "Agency identifier", txtAgencyId)
        txtAgencyId.ReadOnly = True : txtAgencyId.BackColor = Color.FromArgb(240, 238, 234)
        AddField(cAgency, "Branch name *", txtBranchName) : Reg("BranchName", "Branch name", txtBranchName)
        AddField(cAgency, "Branch code *", txtBranchCode) : Reg("BranchCode", "Branch code", txtBranchCode)
        AddField(cAgency, "Branch address *", txtBranchAddress) : Reg("BranchAddress", "Branch address", txtBranchAddress)
        AddField(cAgency, "Contact / support email", txtSupportEmail) : Reg("SupportEmail", "Support email", txtSupportEmail)

        cBilling = MakeCard(lblBillingTitle, "Billing, localization & operating rules")
        cboCurrency.Items.AddRange(New Object() {"USD", "EUR", "GBP", "PHP", "TWD"})
        cboTimezone.Items.AddRange(New Object() {"Europe/London", "Asia/Manila", "Asia/Taipei", "UTC", "America/New_York"})
        cboBillingRule.Items.AddRange(New Object() {"24-hour blocks · daily rounding", "Hourly", "Calendar day"})
        AddField(cBilling, "Currency / number format *", cboCurrency) : Reg("Currency", "Currency", cboCurrency, True)
        AddField(cBilling, "Branch timezone *", cboTimezone) : Reg("Timezone", "Branch timezone", cboTimezone, True)
        AddField(cBilling, "Rental billing rule *", cboBillingRule) : Reg("BillingRule", "Rental billing rule", cboBillingRule, True)
        AddField(cBilling, "Return grace period *", txtGrace) : Reg("GracePeriod", "Return grace period", txtGrace, True)
        AddField(cBilling, "Operating hours", txtHours) : Reg("OperatingHours", "Operating hours", txtHours, True)
        AddField(cBilling, "Holiday calendar", txtHolidays) : Reg("HolidayCalendar", "Holiday calendar", txtHolidays, True)
        SK.Spot(lblBillingNote, SK.F(7.5!), SK.Muted) : cBilling.Controls.Add(lblBillingNote)

        cDoc = MakeCard(lblDocTitle, "Document & report preferences")
        cboContract.Items.AddRange(New Object() {"Executive rental agreement · v4", "Standard rental agreement · v3"})
        cboReportFormat.Items.AddRange(New Object() {"A4 landscape · USD · ISO dates", "A4 portrait · USD · ISO dates", "Letter portrait"})
        AddField(cDoc, "Contract template", cboContract) : Reg("ContractTemplate", "Contract template", cboContract)
        AddField(cDoc, "Report format", cboReportFormat) : Reg("ReportFormat", "Report format", cboReportFormat)
        StyleCheck(chkWatermark, "Include demonstration watermark on all sample documents")
        cDoc.Controls.Add(chkWatermark) : Reg("Watermark", "Watermark", chkWatermark)

        cNotif = MakeCard(lblNotifTitle, "Notification preferences")
        StyleCheck(chkMaintenance, "Maintenance due / overdue · email + in-app") : Reg("NotifMaintenance", "Maintenance notifications", chkMaintenance)
        StyleCheck(chkOverdue, "Overdue returns · in-app + supervisor email") : Reg("NotifOverdue", "Overdue notifications", chkOverdue)
        StyleCheck(chkExpiry, "Registration / insurance expiry · 30-day reminder") : Reg("NotifExpiry", "Expiry notifications", chkExpiry)
        StyleCheck(chkDigest, "Daily revenue digest · managers only") : Reg("NotifDigest", "Revenue digest", chkDigest)
        StyleCheck(chkSensitive, "Sensitive access and rate changes · administrator") : Reg("NotifSensitive", "Sensitive change alerts", chkSensitive)
        cNotif.Controls.AddRange(New Control() {chkMaintenance, chkOverdue, chkExpiry, chkDigest, chkSensitive})
        lblQuietCap = SK.Cap("Quiet hours", SK.F(8), SK.Muted, 0, 0) : lblQuietCap.AutoSize = False
        SK.StyleText(txtQuiet) : txtQuiet.TextAlign = HorizontalAlignment.Right
        cNotif.Controls.AddRange(New Control() {lblQuietCap, txtQuiet}) : Reg("QuietHours", "Quiet hours", txtQuiet)

        cConfirm = MakeCard(lblConfirmTitle, "Confirm sensitive change")
        SK.Spot(lblDraft, SK.F(8), SK.Muted) : cConfirm.Controls.Add(lblDraft)
        pnlWarn = New Panel With {.BackColor = Color.FromArgb(252, 244, 230), .Visible = False}
        SK.Spot(lblWarnTitle, SK.F(8, FontStyle.Bold), Color.FromArgb(176, 112, 32)) : SK.Spot(lblWarnNote, SK.F(7.5!), SK.Muted)
        pnlWarn.Controls.AddRange(New Control() {lblWarnTitle, lblWarnNote})
        cConfirm.Controls.Add(pnlWarn)
        StyleCheck(chkReviewed, "I reviewed billing and timezone impact") : cConfirm.Controls.Add(chkReviewed)
        SK.StyleBtn(btnKeep, "Keep editing", 100, 28, False)
        SK.StyleBtn(btnConfirm, "Confirm change", 120, 28, False)
        btnConfirm.ForeColor = SK.Maroon : btnConfirm.FlatAppearance.BorderColor = SK.Maroon
        cConfirm.Controls.AddRange(New Control() {btnKeep, btnConfirm})

        cHist = MakeCard(lblHistTitle, "Configuration history")
        capVersion = SK.Cap("Version", SK.F(8), SK.Muted, 0, 0) : capSavedBy = SK.Cap("Last saved by", SK.F(8), SK.Muted, 0, 0)
        capSavedAt = SK.Cap("Last saved at", SK.F(8), SK.Muted, 0, 0)
        For Each cp As Label In New Label() {capVersion, capSavedBy, capSavedAt} : cp.AutoSize = False : cHist.Controls.Add(cp) : Next
        For Each v As Label In New Label() {lblVersion, lblSavedBy, lblSavedAt}
            SK.Spot(v, SK.F(8, FontStyle.Bold), SK.Ink, ContentAlignment.TopRight) : cHist.Controls.Add(v)
        Next
        SK.Spot(lblHistNote, SK.F(7.5!), SK.Muted) : cHist.Controls.Add(lblHistNote)

        SK.Spot(lblFootNote, SK.F(8), SK.Muted) : pnl.Controls.Add(lblFootNote)
        SK.StyleBtn(btnDiscard, "Discard changes", 130, 34, False)
        SK.StyleBtn(btnSave, "Save settings", 130, 34, True)
        pnl.Controls.AddRange(New Control() {btnDiscard, btnSave})

        ' tabs (scroll to the matching card)
        Dim tabNames() As String = {"Agency & branch", "Billing & localization", "Operations", "Notifications", "Documents & reports"}
        _tabTargets.AddRange(New Control() {cAgency, cBilling, cBilling, cNotif, cDoc})
        For i As Integer = 0 To tabNames.Length - 1
            Dim b As New Button
            SK.StyleBtn(b, tabNames(i), TextRenderer.MeasureText(tabNames(i), SK.F(8.5!)).Width + 28, 28, i = 0)
            b.Font = SK.F(8.5!)
            If i > 0 Then b.BackColor = Color.FromArgb(240, 238, 234) : b.FlatAppearance.BorderColor = Color.FromArgb(240, 238, 234)
            Dim idx As Integer = i
            AddHandler b.Click, Sub(s, e) SelectTab(idx)
            _tabs.Add(b) : pnl.Controls.Add(b)
        Next

        AddHandler pnl.Resize, Sub(s, e) Arrange()
        AddHandler btnSave.Click, Sub(s, e) RaiseEvent SaveClicked()
        AddHandler btnConfirm.Click, Sub(s, e) RaiseEvent SaveClicked()
        AddHandler btnDiscard.Click, Sub(s, e) RaiseEvent DiscardClicked()
        AddHandler btnKeep.Click, Sub(s, e)
                                      chkReviewed.Checked = False
                                      Dim ch = ChangedKeys()
                                      If ch.Count > 0 Then Fields(ch(0)).Focus()
                                  End Sub
        Me.ResumeLayout(False)
        Arrange()
    End Sub

    Private Sub SelectTab(idx As Integer)
        For i As Integer = 0 To _tabs.Count - 1
            Dim active As Boolean = (i = idx)
            _tabs(i).BackColor = If(active, SK.Maroon, Color.FromArgb(240, 238, 234))
            _tabs(i).ForeColor = If(active, Color.White, SK.Ink)
            _tabs(i).FlatAppearance.BorderColor = If(active, SK.Maroon, Color.FromArgb(240, 238, 234))
        Next
        pnl.ScrollControlIntoView(_tabTargets(idx))
    End Sub
End Class