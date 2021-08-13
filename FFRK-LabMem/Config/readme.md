# Configuration file format

| Property              | Description                        | Default |
| --------------------- | ---------------------------------- | ------- |
| Debug                 | Prints debug info to the console   | false   |
| OpenDoors             | Choose to open sealed doors or not | true    |
| AvoidExploreIfTreasure| Tries to avoid exploration paintings if a treasure vault is visible behind them | true |
| AvoidPortal           | Avoids the portal if an exploration or treasure vault is visible behind it, or if there are more paintings to reveal | true |
| PaintingPriorityMap   | A list of key-value pairs that assign a priority to paintings for selection.  Lower priority values are preferred.  If a priority is tied, then one is chosen randomly. See painting table below for values | see below |
| TreasureFilterMap     | A list of key-value pairs that filters treasures.  A priority of 0 will skip that type of treasure, otherwise lower priority values are preferred.  If a priority is tied, then one is chosen at random. MaxKeys will filter treasures depending on the number of keys it would cost, sane values are 0, 1 or 3. See treasure table below for values | see below |
| MaxKeys               | Default number of max keys to use if no matches in TreasureFilterMap. Sane values are 0, 1 or 3 | 1 |
| WatchdogMinutes       | If an action doesn't complete in this number of minutes, crash recovery is performed.  Set to '0' for no crash recovery | 10 |
| RestartFailedBattle   | Attempt to restart a battle when defeated | false |
| StopOnMasterPainting  | Automatically disables when the master painting is reached. | false |

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
| Id    | Type                                                              | Default Priority  | Default MaxKeys |
| ----- | ----------------------------------------------------------------- | ----------------- | --------------- |
| 5     | Hero Equipment                                                    | 1                 | 1               |
| 4     | Anima Lenses, Bookmark, 5* Rat Tails, Map x2, Teleport Stone     	| 1                 | 3               |
| 3     | 6* Motes, 4* Rat Tails, Magic Key, Treasure Map, Lethe Potion     | 2                 | 1               |
| 2     | 6* Crystals, Rainbow Crystal, Rosetta Stone of Wisdom             | 0                 | 0               |
| 1     | 5* Orbs, 5* Motes                                                 | 0                 | 0               |
