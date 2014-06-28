using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoodleEmpires.Engine.Entities
{
    /// <summary>
    /// Represents buffs on a unit to increase or decrease properties
    /// </summary>
    public struct UnitBuff
    {
        float _armorMultiplier;
        float _healthMultiplier;
        float _regenMultiplier;
        float _speedMultiplier;
        float _firespeedMultiplier;
        float _ammoRegenMultiplier;
        float _constructionMultiplier;
        float _harvestingMultiplier;

        /// <summary>
        /// Gets or sets the armor multiplier for this buff
        /// </summary>
        public float ArmorMultiplier
        {
            get { return _armorMultiplier; }
            set { _armorMultiplier = value; }
        }
        /// <summary>
        /// Gets or sets the health multiplier for this buff
        /// </summary>
        public float HealthMultiplier
        {
            get { return _healthMultiplier; }
            set { _healthMultiplier = value; }
        }
        /// <summary>
        /// Gets or sets the health regeneration multiplier for this buff
        /// </summary>
        public float RegenMultiplier
        {
            get { return _regenMultiplier; }
            set { _regenMultiplier = value; }
        }
        /// <summary>
        /// Gets or sets the speed multiplier for this buff
        /// </summary>
        public float SpeedMultiplier
        {
            get { return _speedMultiplier; }
            set { _speedMultiplier = value; }
        }
        /// <summary>
        /// Gets or sets the firing rate multiplier for this buff
        /// </summary>
        public float FireRateMultiplier
        {
            get { return _firespeedMultiplier; }
            set { _firespeedMultiplier = value; }
        }
        /// <summary>
        /// Gets or sets the ammo regeneration multiplier for this buff
        /// </summary>
        public float AmmoRegenMultiplier
        {
            get { return _ammoRegenMultiplier; }
            set { _ammoRegenMultiplier = value; }
        }
        /// <summary>
        /// Gets or sets the construction speed multiplier for this buff
        /// </summary>
        public float ConstructionMultiplier
        {
            get { return _constructionMultiplier; }
            set { _constructionMultiplier = value; }
        }
        /// <summary>
        /// Gets or sets the haversting speed multiplier for this buff
        /// </summary>
        public float HarvestingMultiplier
        {
            get { return _harvestingMultiplier; }
            set { _harvestingMultiplier = value; }
        }

        /// <summary>
        /// Creates a new blank unit buff
        /// </summary>
        private static UnitBuff GetBlankBuff()
        {
            UnitBuff buff = new UnitBuff();
            buff._armorMultiplier = 1.0f;
            buff._healthMultiplier = 1.0f;
            buff._regenMultiplier = 1.0f;
            buff._speedMultiplier = 1.0f;
            buff._firespeedMultiplier = 1.0f;
            buff._ammoRegenMultiplier = 1.0f;
            buff._constructionMultiplier = 1.0f;
            buff._harvestingMultiplier = 1.0f;
            return buff;
        }

        /// <summary>
        /// Gets an empty unit buff
        /// </summary>
        public static UnitBuff Empty
        {
            get { return GetBlankBuff();  }
        }
    }
}
