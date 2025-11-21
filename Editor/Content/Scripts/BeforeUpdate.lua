-- Don't edit right in the Visual Studio, use other
function BeforeUpdateMain()
	c = project.CurrentLevel.GetCamera()
	c.UpdatePosition(0.1,0.1,0.1)
end
