$pictures = Get-ChildItem -Path $args * -include *.jpg, *.gif, *.JPEG, *.png| Sort-Object name
$len = $pictures.Count
$change = $pictures
$count = 0
$exists = 0
$num = false
for([int]$x=0;$x -lt $len;$x++){
	$filename = $pictures[$x].name
	for([int]$z=0;($z -lt 10) -and (!$num) ;$z++){
		$num = $filename.StartsWith("$z")
	}
	if($num){
		$exists++
	}
	else{
		$change[$count] = $pictures[$x]
		$count++
	}
	$num = false
}
for([int]$x=$exists;$x -lt $len;$x++){
	$filename = $change[$x].name
	if($x -lt 10){
		$filename = "0"+"0"+$x+"_"+$filename
	}
	elseif($x -lt 100){
		$filename = "0"+$x+"_"+$filename
	}
	else{
		$filename = $x+"_"+$filename
	}
	Rename-Item $pictures[$x] $filename
}