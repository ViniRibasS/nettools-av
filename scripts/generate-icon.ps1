$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName PresentationCore
Add-Type -AssemblyName WindowsBase
$taskRoot = Split-Path $PSScriptRoot -Parent
$taskDrawing = New-Object System.Windows.Media.DrawingVisual
$taskContext = $taskDrawing.RenderOpen()
$taskBackground = [Windows.Media.BrushConverter]::new().ConvertFromString('#20211F')
$taskForeground = [Windows.Media.BrushConverter]::new().ConvertFromString('#ECECE7')
$taskGold = [Windows.Media.BrushConverter]::new().ConvertFromString('#B99852')
$taskContext.PushTransform([Windows.Media.ScaleTransform]::new(2,2))
$taskContext.DrawRoundedRectangle($taskBackground,$null,[Windows.Rect]::new(0,0,128,128),22,22)
$taskPen = [Windows.Media.Pen]::new($taskForeground,9)
$taskPen.StartLineCap = $taskPen.EndLineCap = 'Round'
$taskPen.LineJoin = 'Round'
$taskContext.DrawGeometry($null,$taskPen,[Windows.Media.Geometry]::Parse('M93,101 C75,118 40,114 22,94 C2,73 9,36 30,20 C53,2 89,8 107,29 C122,48 119,79 105,87 C93,94 81,87 83,74 L91,42 M85,47 C66,30 42,44 41,64 C39,86 65,95 79,75'))
$taskOutline = [Windows.Media.Pen]::new($taskForeground,4)
$taskOutline.LineJoin = 'Round'
$taskContext.DrawGeometry($taskBackground,$taskOutline,[Windows.Media.Geometry]::Parse('M80,18 L104,24 104,32 109,34 102,58 76,51 82,28 78,26 Z'))
$taskContact = [Windows.Media.Pen]::new($taskGold,1.6)
$taskContact.StartLineCap = $taskContact.EndLineCap = 'Round'
$taskContext.DrawGeometry($null,$taskContact,[Windows.Media.Geometry]::Parse('M83,33 L80,44 M86,34 L83,45 M89,35 L86,46 M92,36 L89,47 M95,37 L92,48 M98,38 L95,49 M101,39 L98,50 M104,40 L101,51'))
$taskContext.Pop()
$taskContext.Close()
$taskBitmap = [Windows.Media.Imaging.RenderTargetBitmap]::new(256,256,96,96,[Windows.Media.PixelFormats]::Pbgra32)
$taskBitmap.Render($taskDrawing)
$taskEncoder = [Windows.Media.Imaging.PngBitmapEncoder]::new()
$taskEncoder.Frames.Add([Windows.Media.Imaging.BitmapFrame]::Create($taskBitmap))
$taskMemory = [IO.MemoryStream]::new()
$taskEncoder.Save($taskMemory)
$taskBytes = $taskMemory.ToArray()
[IO.File]::WriteAllBytes((Join-Path $taskRoot 'assets/brand/nettools-av.png'),$taskBytes)
$taskIcon = [IO.BinaryWriter]::new([IO.File]::Create((Join-Path $taskRoot 'assets/brand/nettools-av.ico')))
try {
 $taskIcon.Write([uint16]0); $taskIcon.Write([uint16]1); $taskIcon.Write([uint16]1)
 $taskIcon.Write([byte]0); $taskIcon.Write([byte]0); $taskIcon.Write([byte]0); $taskIcon.Write([byte]0)
 $taskIcon.Write([uint16]1); $taskIcon.Write([uint16]32); $taskIcon.Write([uint32]$taskBytes.Length); $taskIcon.Write([uint32]22)
 $taskIcon.Write($taskBytes)
} finally { $taskIcon.Dispose(); $taskMemory.Dispose() }

