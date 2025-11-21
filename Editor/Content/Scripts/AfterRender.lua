-- Don't edit right in the Visual Studio, use other
function AfterRenderMain()
	l = project.CurrentLevel.GetLight()
	l.SetColor(255,0,0)
end
