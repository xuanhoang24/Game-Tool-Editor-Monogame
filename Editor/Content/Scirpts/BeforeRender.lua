function BeforeRenderMain()
	t = project.CurrentLevel.GetTerrain()
	t.Scale = t.Scale - 0.001
end
