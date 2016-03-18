$pictures = Get-ChildItem -Path $args * -include *.jpg, *.gif, *.JPEG, *.png
$len = $pictures.Count
for([int]$x=0;$x -lt $len;$x++){
	$filename = $pictures[$x].name
	for([int]$y=0;$y -lt 3;$y++)
	{
		for([int]$z=0;$z -lt 10;$z++){
			$filename = $filename.trimStart("$z")
		}
	}
	$filename = $filename.trimStart("_")
	Rename-Item $pictures[$x] $filename
}
