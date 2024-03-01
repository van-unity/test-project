# Match3 Game Challenge - BEBOP BEE

## Overview

This repository contains the core business logic for a Match3 game challenge, organized within the `Domain` namespace to
ensure clean separation of concerns and maintainability.

## Architecture and Design

### Strategy Pattern for Extensibility

- **Match Finding**: To accommodate future enhancements and optimizations, I employ the `IMatchFinderStrategy`
  interface. This abstraction allows the integration of more efficient algorithms for match detection as they become
  available.
- **Tile Creation**: The `ITileCreatorStrategy` interface abstracts tile creation processes. This design supports the
  introduction of new tile generation algorithms and the potential inclusion of unique gameplay elements such as
  power-ups or special tiles.

### Object Pooling

- I utilize an Object Pool design pattern for efficient tile management, reducing the overhead associated with the
  frequent creation and destruction of game objects.

### Configuration and Customization

- **Tile Configuration**: The `TileConfiguration` class enables the addition and modification of tile types,
  facilitating easy expansion and customization of the game's visual and functional elements.
- **Board Settings**: Initial board dimensions and other game settings are managed via the `BoardSettings` class. This
  setup is designed for easy adjustments and future expansions to accommodate additional game configuration
  requirements.
