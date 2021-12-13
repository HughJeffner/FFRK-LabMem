using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FFRK_LabMem.Data.Counters;

namespace FFRK_LabMem.Data
{
    class CounterInference
    {
        public static int InferRarity(DropCategory category, string name)
        {

            // Motes - First character is a digit (star in name)
            if (category == DropCategory.SPHERE_MATERIAL && char.IsDigit(name[0]))
            {
                return int.Parse(name[0].ToString());
            }

            // Crystals/Orbs
            if (category == DropCategory.ABILITY_MATERIAL)
            {
                // Crystals are 6*
                if (name.EndsWith("Crystal")) return 6;

                // Orbs
                if (name.EndsWith("Orb"))
                {
                    if (name.StartsWith("Major")) return 5;
                    if (name.StartsWith("Greater")) return 4;
                    if (name.StartsWith("Lesser")) return 2;
                    if (name.StartsWith("Minor")) return 1;
                    return 3;
                }
            }

            // Upgrade materials
            if (category == DropCategory.EQUIPMENT_SP_MATERIAL)
            {
                if (name.EndsWith("Crystal")) return 6;
                if (name.StartsWith("Giant")) return 5;
                if (name.StartsWith("Large")) return 4;
                if (name.StartsWith("Small")) return 2;
                if (name.StartsWith("Tiny")) return 1;
                return 3;

            }

            // Tails
            if (category == DropCategory.HISTORIA_CRYSTAL_ENHANCEMENT_MATERIAL)
            {
                if (name.StartsWith("Huge")) return 5;
                if (name.StartsWith("Large")) return 4;
                if (name.StartsWith("Medium")) return 3;
                if (name.StartsWith("Small")) return 2;
                return 1; // Does not exist?
            }

            // Eggs
            if (category == DropCategory.GROW_EGG)
            {
                if (name.StartsWith("Major")) return 5;
                if (name.StartsWith("Greater")) return 4;
                if (name.StartsWith("Lesser")) return 2;
                if (name.StartsWith("Minor")) return 1;
                return 3;
            }

            // Arcana
            if (category == DropCategory.BEAST_FOOD)
            {
                if (name.StartsWith("Major")) return 5;
                if (name.StartsWith("Greater")) return 4;
                if (name.StartsWith("Lesser")) return 2;
                if (name.StartsWith("Minor")) return 1;  // Does not exist?
                return 3;
            }

            return 0;

        }

        public static DropCategory? InferCategory(string imagePath)
        {
            if (imagePath.Contains("labyrinth_item")) return DropCategory.LABYRINTH_ITEM;
            if (imagePath.Contains("common_item")) return DropCategory.COMMON;
            if (imagePath.Contains("sphere_material")) return DropCategory.SPHERE_MATERIAL;
            if (imagePath.Contains("ability_material")) return DropCategory.ABILITY_MATERIAL;
            if (imagePath.Contains("equipment_sp_material")) return DropCategory.EQUIPMENT_SP_MATERIAL;
            if (imagePath.Contains("historia_crystal_enhancement_material")) return DropCategory.HISTORIA_CRYSTAL_ENHANCEMENT_MATERIAL;
            if (imagePath.Contains("grow_egg")) return DropCategory.GROW_EGG;
            if (imagePath.Contains("beast_food")) return DropCategory.BEAST_FOOD;
            if (imagePath.Contains("equipment")) return DropCategory.EQUIPMENT;
            return null;
        }
    }
}
