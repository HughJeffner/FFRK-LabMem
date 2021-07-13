# Configuration file format

| Property              | Description                        |
| --------------------- | ---------------------------------- |
| Debug                 | Prints debug info to the console   |
| OpenDoors             | Choose to open sealed doors or not |
| AvoidExploreIfTreasure| Tries to avoid exploration paintings if a treasure vault is visible behind them |
| AvoidPortal           | Avoids the portal if an exploration or treasure vault is visible behind it |
| PaintingPriorityMap   | A list of key-value pairs that assign a priority to paintings for selection.  Lower priority values are preferred.  If a priority is tied, then one is chosen randomly. See painting table below for values |
| TreasurePriorityMap   | A list of key-value pains that assign a priority to treasures.  A value of 0 will skip that type of treasure, otherwise lower priority values are preferred.  If a priority is tied, then one is chosen at random.  See treasure table below for values |
| MaxKeys               | The maximum number of keys to use when opening treasures.  Sane values are 0, 1 or 3 |
| AppPosition           | The postion of the FFRK icon on the homescreen, in pixels.  Used for battle crash recovery |

## Painting Types
|  Id   | Type                  |
| ----- | ----------------------|
| 1.1   | Combatant (Green)     |
| 1.2   | Combatant (Orange)    |
| 1.3   | Combatant (Red)       |
| 2     | Master Painting       |
| 3     | Treasure Vault        |
| 4     | Exploration           |
| 5     | Onslaught             |
| 6     | Portal                |
| 7     | Restoration           |

## Treasure Types
| Id    | Type                                  |
| ----- | ------------------------------------- |
| 5     | Hero Equipment                        |
| 4     | Anima Lenses, Bookmark, 5* Rat Tails  |
| 3     | 6* Motes, 4* Rat Tails, Magic Key     |
| 2     | 6* Crystals, Rainbow Crystals         |
| 1     | 5* Orbs, 5* Motes                     |
