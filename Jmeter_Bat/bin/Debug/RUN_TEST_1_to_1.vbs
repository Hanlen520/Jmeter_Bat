arr = array("Jmeter_Bat_1")
For i=0 To UBound(arr)-LBound(arr)
    bat=".\Bat\"+arr(i)+".bat"
    wscript.createobject("wscript.shell").run "cmd /c """+bat+""""
    wscript.sleep 1000
Next