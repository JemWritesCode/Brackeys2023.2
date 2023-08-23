using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game
{
    public static class SkillDatabase
    {
        public static List<Skill> AllSkills {  get; private set; }

        public static void InitializeDatabase()
        {
            AllSkills = new List<Skill>(Resources.LoadAll<Skill>("Skills"));
        }

        public static Skill GetSkill(string id)
        {
            return AllSkills.Find(skill => skill.ID == id);
        }
    }
}
