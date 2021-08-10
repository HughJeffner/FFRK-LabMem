# Data file format

## Drops (drops_battle.csv, drops.csv)
| Column            | Description                       |
| ----------------- | ----------------------------------|
| Timestamp			| Date/time in YYYY-MM-DD HH:MM     |
| Floor				| Floor number						|
| Source			| Painting Id					    |
| Name				| Dropped item name					|
| Qty				| Quantity							|

## Explore Results (explores_v01.csv)
| Column            | Description                       |
| ----------------- | ----------------------------------|
| Timestamp			| Date/time in YYYY-MM-DD HH:MM     |
| Floor				| Floor number						|
| Source			| Painting Id (always 4)		    |
| ResultType		| See below                 		|
| InsideDoor		| 1 if result of opening door		|

## Treasures (treasures_v01.csv)
| Column            | Description                       |
| ----------------- | ----------------------------------|
| Timestamp			| Date/time in YYYY-MM-DD HH:MM     |
| Floor				| Floor number						|
| Source			| Painting Id (always 4)		    |
| Index				| Zero-based index of treasure 		|
| Treasure			| See below							|

## Types

### Explore Result Types
| Id	| Description	|
| ----- | ------------- |
| 1		| Nothing		|
| 2		| Normal Drop	|
| 3		| Lab Item Drop	|
| 4		| Battle		|
| 5		| Restore		|
| 6		| Onslaught		|
| 7		| Door			|
| 8		| Portal		|
| 9		| Treasure		|
| 10	| Fatigue		|

### Painting Types
| Id    | Type                  |
| ----- | ----------------------|
| 1     | Combatant				|
| 2     | Master Painting       | 
| 3     | Treasure Vault        |
| 4     | Exploration           |
| 5     | Onslaught             |
| 6     | Portal                |
| 7     | Restoration           |

### Treasure Types
| Id     | Type                                                              |
| ------ | ----------------------------------------------------------------- |
| 5xxxx  | Hero Equipment                                                    |
| 4xxxx  | Anima Lenses, Bookmark, 5* Rat Tails, Map x2, Teleport Stone      |
| 3xxxx  | 6* Motes, 4* Rat Tails, Magic Key, Treasure Map, Lethe Potion     |
| 2xxxx  | 6* Crystals, Rainbow Crystal, Rosetta Stone of Wisdom             |
| 1xxxx  | 5* Orbs, 5* Motes                                                 |