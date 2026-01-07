-- Don't edit right in the Visual Studio, use other
function AfterUpdateMain()
	c = project.CurrentLevel.GetCamera()
	c.UpdateRotation(0.0)
end
