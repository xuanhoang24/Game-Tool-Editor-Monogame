# ECS Architecture

This project uses a simple Entity Component System (ECS) architecture.

## Structure

### Entity
- Simple container with an ID and name
- Holds components in a dictionary
- No logic, just data storage

### Components (Data)
- **TransformComponent**: Position, rotation, scale
- **MeshComponent**: Model, texture, shader
- **RotationComponent**: Rotation speed
- **OrbitComponent**: Orbit parameters (parent, speed, angle, radius)
- **TagComponent**: String tag for identification

### Systems (Logic)
- **RotationSystem**: Updates rotation based on RotationComponent
- **OrbitSystem**: Handles orbital movement around parent entities
- **RenderSystem**: Renders all entities with mesh and transform

### World
- Manages all entities and systems
- Runs system updates each frame
- Provides entity creation and lookup