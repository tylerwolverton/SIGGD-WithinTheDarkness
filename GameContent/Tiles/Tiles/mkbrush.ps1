$files = Get-Item *.png
$curname = ""
foreach ($file in $files) {
	if (! ($file -match "(\d{3})_(.*?)(TL|TR|BL|BR).png") ) {
		"Malformed filename $file"
	}
	$num = [int]$matches[1]
	$name = $matches[2]

	if ($name -eq $curname) {
		continue
	}

	$curname = $name

	$brush = "2 2`r`n"
	$brush += [string]$num + ' ' + ($num+1) + "`r`n" + ($num+2) + ' ' + ($num+3)
	Out-File "$name.txt" -InputObject $brush -Encoding ASCII
}