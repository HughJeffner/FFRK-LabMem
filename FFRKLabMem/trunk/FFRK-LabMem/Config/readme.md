# Configuration file format

| Property              | Description                        | Default |
| --------------------- | ---------------------------------- | ------- |
| Debug                 | Prints debug info to the console   | false   |
| OpenDoors             | Choose to open sealed doors or not | true    |
| AvoidExploreIfTreasure| Tries to avoid exploration paintings if a treasure vault is visible behind them | true |
| AvoidPortal           | Avoids the portal if an exploration or treasure vault is visible behind it | true |
| PaintingPriorityMap   | A list of key-value pairs that assign a priority to paintings for selection.  Lower priority values are preferred.  If a priority is tied, then one is chosen randomly. See painting table below for values | see below |
| TreasurePriorityMap   | A list of key-value pairs that assign a priority to treasures.  A value of 0 will skip that type of treasure, otherwise lower priority values are preferred.  If a priority is tied, then one is chosen at random.  See treasure table below for values | see below |
| MaxKeys               | The maximum number of keys to use when opening treasures.  Sane values are 0, 1 or 3 | 3 |
| AppPosition           | The postion of the FFRK icon on the homescreen, in pixels.  Used for battle crash recovery | 213, 432 |
| BattleWatchdogMinutes | If a battle doesn't complete in this number of minutes, crash recovery is performed.  Set to '0' for no crash recovery | 10 |

## Painting Types
|  Id   | Type                  | Default Priority  |
| ----- | ----------------------| ----------------- |
| 1.1   | Combatant (Green)     | 8                 |
| 1.2   | Combatant (Orange)    | 7                 |
| 1.3   | Combatant (Red)       | 6                 |
| 2     | Master Painting       | 9                 |
| 3     | Treasure Vault        | 1                 |
| 4     | Exploration           | 2                 |
| 5     | Onslaught             | 4                 |
| 6     | Portal                | 5                 |
| 7     | Restoration           | 3                 |

## Treasure Types
| Id    | Type                                                              | Default Priority  |
| ----- | ----------------------------------------------------------------- | ----------------- |
| 5     | Hero Equipment                                                    | 1                 |
| 4     | Anima Lenses, Bookmark, 5* Rat Tails, Map x2, Teleport Stone     	| 1                 |
| 3     | 6* Motes, 4* Rat Tails, Magic Key, Treasure Map, Lethe Potion     | 1                 |
| 2     | 6* Crystals, Rainbow Crystal, Rosetta Stone of Wisdom             | 0                 |
| 1     | 5* Orbs, 5* Motes                                                 | 0                 |
