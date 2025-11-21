function AfterUpdateMain()
	c = project.CurrentLevel.GetCamera()
	c.UpdateRotation(0.01)
end
