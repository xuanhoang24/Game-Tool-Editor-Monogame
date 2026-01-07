-- Don't edit right in the Visual Studio, use other
function BeforeUpdateMain()
	c = project.CurrentLevel.GetCamera()
	c.UpdatePosition(0.0,0.0,0.0)
end
