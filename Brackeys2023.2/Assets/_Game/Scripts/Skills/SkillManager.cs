using JadePhoenix.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game
{
    public class SkillManager : PersistentSingleton<SkillManager>
    {
        protected override void Awake()
        {
            base.Awake();
            SkillDatabase.InitializeDatabase();
        }
    }
}
