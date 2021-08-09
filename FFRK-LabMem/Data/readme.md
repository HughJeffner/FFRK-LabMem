# Data file format

## Drops (drops_battle.csv, drops.csv)
| Column            | Description                       |
| ----------------- | ----------------------------------|
| Timestamp			| Date/time in YYYY-MM-DD HH:MM     |
| Source			| Painting Id					    |
| Name				| Dropped item name					|
| Qty				| Quantity							|

## Explore Results (explore_results_v01.csv)
| Column            | Description                       |
| ----------------- | ----------------------------------|
| Timestamp			| Date/time in YYYY-MM-DD HH:MM     |
| Source			| Painting Id (always 4)		    |
| ResultType		| See below                 		|
| InsideDoor		| true if result of opening door	|

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