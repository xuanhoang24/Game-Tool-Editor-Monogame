-- Don't edit right in the Visual Studio, use other
function BeforeRenderMain()
	t = project.CurrentLevel.GetTerrain()
	t.Scale = t.Scale
end
