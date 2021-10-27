# Configuration file format

**Note all of these values can be changed via GUI using `C`**

| Property              | Description                        | Default |
| --------------------- | ---------------------------------- | ------- |
| Debug                 | Prints lab-specific debug info to the console   | false   |
| AutoStart             | Attempts to auto start the bot when enabled | false |
| OpenDoors             | Choose to open sealed doors or not | true    |
| AvoidExploreIfTreasure| Tries to avoid exploration paintings if a treasure vault is visible behind them | true |
| AvoidPortal           | Avoids the portal if an exploration or treasure vault is visible behind it, or if there are more paintings to reveal | true |
| ~~MaxKeys~~           | Deprecated in 2.0.0, use TreasureFilterMap instead |  |
| WatchdogMinutes       | If an action doesn't complete in this number of minutes, crash recovery is performed.  Set to '0' for no crash recovery | 10 |
| RestartFailedBattle   | Attempt to restart a battle when defeated | false |
| StopOnMasterPainting  | Automatically disables when the master painting is reached. | false |
| RestartLab            | Restarts the lab once completed | false |
| UsePotions            | Uses potions when restarting a lab | false |
| UseOldCrashRecovery   | Uses legacy (pixel based) crash recovery | false |
| UseLetheTears         | Uses lethe tears when `LetheTearsSlot` and `LetheTearsFatigue` conditions are met | false |
| LetheTearsSlot        | A 5-bit binary value in decimal form of which slots should be checked for fatigue, significant bit (right) is slot 1 | 31 (0b11111) |
| LetheTearsFatigue     | Fatigue level before using lethe tears on the units in `LetheTearsSlot` | 7 |
| UseTeleportStoneOnMasterPainting | Instead of fighting the master painting, use a teleport stone | false |
| ScreenshotRadiantPainting | Saves a PNG screenshot to the bot directory if a radiant painting is seen | false |
| EnemyBlocklistAvoidOptionOverride | Enemy blocklist priorities override the Avoid.. options | false |
| PaintingPriorityMap   | A list of key-value pairs that assign a priority to paintings for selection.  Lower priority values are preferred.  If a priority is tied, then one is chosen randomly. See painting table below for values | see below |
| TreasureFilterMap     | A list of key-value pairs that filters treasures.  A priority of 0 will skip that type of treasure, otherwise lower priority values are preferred.  If a priority is tied, then one is chosen at random. MaxKeys will filter treasures depending on the number of keys it would cost, sane values are 0, 1 or 3. See treasure table below for values | see below |
| EnemyBlocklist | A list of enemies to avoid if possible | see below |
| Timings | A list of configurable timings in milliseconds | see below |

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
| 4     | Anima Lenses, Bookmark, 5* Rat Tails, Map x2, Teleport Stone     	| 1                 | 1               |
| 3     | 6* Motes, 4* Rat Tails, Magic Key, Treasure Map, Lethe Potion     | 2                 | 1               |
| 2     | 6* Crystals, Rainbow Crystal, Rosetta Stone of Wisdom             | 0                 | 0               |
| 1     | 5* Orbs, 5* Motes                                                 | 0                 | 0               |

## Timings
| Name                 | Description                                          | Default |
| -------------------- | ---------------------------------------------------- | ------- |
| Pre-SelectPainting   | Delay before selecting a painting                    | 5000    |
| Inter-SelectPainting | Delay between selecting a painting and confirming it | 1000    |
| Pre-SelectTreasure   | Delay before selecting the first treasure            | 5000    |
| Pre-Door             | Delay before either opening or leaving a sealed door | 5000    |
| Pre-MoveOn           | Delay before moving on after most explore results    | 5000    |
| Post-Battle          | Delay before pressing skip after a battle ends       | 7000    |

## Enemy Blocklist
| Name                 | Description                      | Default Enabled |
| -------------------- | -------------------------------- | --------------- |
| Alexander            | High resistance                  | false           |
| Atomos               | Slowga, High Damage              | false           |
| Diablos              | High Damage                      | false           |
| Lani & Scarlet Hair  | Damage Sponge                    | false           |
| Lunasaurs            | Reflect                          | false           |
| Octomammoth          | Reflect                          | false           |
| Marilith             | High resistance, Blind           | false           |
