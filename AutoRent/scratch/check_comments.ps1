$vb = Get-Content 'AutoRent\Presentation\Views\User\ReservationsView.xaml.vb'
$vbComments = @()
for ($i = 0; $i -lt $vb.Count; $i++) {
    if ($vb[$i].TrimStart().StartsWith("'")) {
        $vbComments += "$($i+1): $($vb[$i])"
    }
}
Write-Output "VB Comments found: $($vbComments.Count)"
$vbComments

$xaml = Get-Content 'AutoRent\Presentation\Views\User\ReservationsView.xaml'
$xamlComments = @()
for ($i = 0; $i -lt $xaml.Count; $i++) {
    if ($xaml[$i].Contains("<!--")) {
        $xamlComments += "$($i+1): $($xaml[$i])"
    }
}
Write-Output "XAML Comments found: $($xamlComments.Count)"
$xamlComments
