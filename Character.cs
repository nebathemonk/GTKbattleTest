using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ini;

namespace battle
{
    public class Character
    {
        public string Name { get; set; }

        internal string team { get; set; }

        //artificial intelligence
        internal AI behavior;

        internal float attack {get; set;}
        internal float accuracy { get; set; }
        internal float spirit { get; set; }
        internal float defense { get; set; }
        internal float evasion { get; set; }
        internal float will { get; set; }

        internal float speed;
        internal float health;
        internal float stamina;

        internal int maxHP;
        internal int HP;
        internal int maxMP;
        internal int MP;

        internal int tempAttack;
        internal int tempAccuracy;
        internal int tempSpirit;
        internal int tempDefense;
        internal int tempEvasion;
        internal int tempWill;

        internal int tempSpeed;
        internal int tempHealth;
        internal int tempStamina;

		internal Skill defendingSkill;
		internal Skill usingSkill;
		internal Character lastAttacker;

        internal List<Skill> skills;

        internal List<KeyValuePair<string, int>> resistances;

        internal List<Status> statuses;
        internal List<Status> intrinsics;

        internal List<string> equipment;
        internal List<string> inventory;
        internal List<string> weapons;

        public Character(string charFile)
        {
            load(charFile);
        }

        public void load(string CharFile){

            IniFile newFile = new IniFile("../../Characters/"+CharFile+".char");
            //Console.WriteLine("loading character " + CharFile);
            //string charName = Path.GetFileNameWithoutExtension(newFile.path);

            Name = newFile.IniReadValue(CharFile,"name");
            team = newFile.IniReadValue(CharFile, "team");
            attack = float.Parse(newFile.IniReadValue(CharFile, "attack"));
            accuracy = float.Parse(newFile.IniReadValue(CharFile, "accuracy"));
            spirit = float.Parse(newFile.IniReadValue(CharFile, "spirit"));
            defense = float.Parse(newFile.IniReadValue(CharFile, "defense"));
            evasion = float.Parse(newFile.IniReadValue(CharFile, "evasion"));
            will = float.Parse(newFile.IniReadValue(CharFile, "will"));

            speed = float.Parse(newFile.IniReadValue(CharFile, "speed"));
            health = float.Parse(newFile.IniReadValue(CharFile, "health"));
            stamina = float.Parse(newFile.IniReadValue(CharFile, "stamina"));

            resistances = new List<KeyValuePair<string,int>>();

            resistances.Add(new KeyValuePair<string,int>(
                "fire",Convert.ToInt32(newFile.IniReadValue(CharFile,"resistFire"))));
            resistances.Add(new KeyValuePair<string, int>(
                "ice", Convert.ToInt32(newFile.IniReadValue(CharFile, "resistIce"))));
            resistances.Add(new KeyValuePair<string, int>(
                "water", Convert.ToInt32(newFile.IniReadValue(CharFile, "resistWater"))));
            resistances.Add(new KeyValuePair<string, int>(
                "earth", Convert.ToInt32(newFile.IniReadValue(CharFile, "resistEarth"))));
            resistances.Add(new KeyValuePair<string, int>(
                "wind", Convert.ToInt32(newFile.IniReadValue(CharFile, "resistWind"))));
            resistances.Add(new KeyValuePair<string, int>(
                "electric", Convert.ToInt32(newFile.IniReadValue(CharFile, "resistElectric"))));
            resistances.Add(new KeyValuePair<string, int>(
                "poison", Convert.ToInt32(newFile.IniReadValue(CharFile, "resistPoison"))));
            resistances.Add(new KeyValuePair<string, int>(
                "dark", Convert.ToInt32(newFile.IniReadValue(CharFile, "resistDark"))));
            resistances.Add(new KeyValuePair<string, int>(
                "light", Convert.ToInt32(newFile.IniReadValue(CharFile, "resistLight"))));

            char[] seperator = new char[] { ',' };

            skills = new List<Skill>();
            string skillsList = newFile.IniReadValue(CharFile, "skills");
            if (skillsList.Length > 1)
            {
                string[] newSkills = skillsList.Split(seperator, StringSplitOptions.None);
                for (int s = 0; s < newSkills.Length; s++)
                {
                    Skill newSkill = new Skill(newSkills[s]);
                    //Console.WriteLine("Creating skill " + newSkill.Name);
                    skills.Add(newSkill);
                }
            }


            intrinsics = new List<Status>();
            string intrinsicList = newFile.IniReadValue(CharFile, "intrinsics");
            if (intrinsicList.Length > 1)
            {
                string[] newIntrinsic = intrinsicList.Split(seperator, StringSplitOptions.None);
                for (int n = 0; n < newIntrinsic.Length; n++)
                {
                    //intrinsics.Add(new Status(this,newIntrinsic[n]));
                    //addStatus(newIntrinsic[n]);
                }
            }


            statuses = new List<Status>();
            string status = newFile.IniReadValue(CharFile, "status");
            if (status.Length > 1)
            {
                string[] newStatuses = status.Split(seperator, StringSplitOptions.None);
                for (int t = 0; t < newStatuses.Length; t++)
                {
                    Status newStatus = new Status(this, newStatuses[t]);
                    statuses.Add(newStatus);
                }
            }            

            equipment = new List<string>();
            string equips = newFile.IniReadValue(CharFile, "equipment");
            string[] newEquips = equips.Split(seperator, StringSplitOptions.None);
            for (int e = 0; e < newEquips.Length; e++)
            {
                equipment.Add(newEquips[e]);
            }

            weapons = new List<string>();
            string weapon = newFile.IniReadValue(CharFile, "weapons");
            string[] newWeapon = weapon.Split(seperator, StringSplitOptions.None);
            for (int w = 0; w < newWeapon.Length; w++)
            {
                weapons.Add(newWeapon[w]);
            }

            inventory = new List<string>();
            string items = newFile.IniReadValue(CharFile, "inventory");
            string[] newItems = items.Split(seperator, StringSplitOptions.None);
            for (int i = 0; i < newItems.Length; i++)
            {
                inventory.Add(newItems[i]);
            }

            checkNPC();
            setStats();
        }

        internal void checkNPC()
        {
            if (team == "npc")
            {
                behavior = new AI(this);
            }
        }

		internal List<Skill> getSkills ()
		{
			return this.skills;
		}

        internal void setStats()
        {
           /* tempAttack = Convert.ToInt32(Math.Ceiling(attack * 100));
            tempAccuracy = Convert.ToInt32(Math.Ceiling(accuracy * 100));
            tempSpirit = Convert.ToInt32(Math.Ceiling(spirit * 100));
            tempDefense = Convert.ToInt32(Math.Ceiling(defense * 100));
            tempEvasion = Convert.ToInt32(Math.Ceiling(evasion * 100));
            tempWill = Convert.ToInt32(Math.Ceiling(will * 100));
            tempSpeed = Convert.ToInt32(Math.Ceiling(speed * 100));
            tempHealth = Convert.ToInt32(Math.Ceiling(health * 100));
            tempStamina = Convert.ToInt32(Math.Ceiling(stamina * 100));*/

            tempAttack = Convert.ToInt32(Math.Ceiling(attack));
            tempAccuracy = Convert.ToInt32(Math.Ceiling(accuracy));
            tempSpirit = Convert.ToInt32(Math.Ceiling(spirit));
            tempDefense = Convert.ToInt32(Math.Ceiling(defense));
            tempEvasion = Convert.ToInt32(Math.Ceiling(evasion));
            tempWill = Convert.ToInt32(Math.Ceiling(will));
            tempSpeed = Convert.ToInt32(Math.Ceiling(speed));
            tempHealth = Convert.ToInt32(Math.Ceiling(health));
            tempStamina = Convert.ToInt32(Math.Ceiling(stamina));

            maxHP = tempHealth*100; maxMP = tempStamina*100;
            HP = maxHP; MP = maxMP;
        }

        public void activateStatus(string step)
        {
            foreach(Status s in statuses)
            {
                if (s.step == step) { s.activate(); }
                //s.turnIncrease(); 
            }
        }

        public void removeOldStatus()
        {
            for (int s = statuses.Count() - 1; s > -1; s--)
            {
				statuses[s].turnIncrease(); //update the turn counter for that status
                if (statuses[s].toBeRemove) { statuses[s].remove(); }
            }
        }

        public void checkSkills()
        {
            foreach (Skill s in skills)
            {
                s.warmCheck();
            }
        }

        public void useSkill(int snum, List<Character> target){
            //Combat.output("skill number " + snum.ToString() + " used");
            skills[snum].use(this, target);
        }

        public void rest()
        {
            double percent = Combat.rng.NextDouble();
            //d = ((d * (3.2 - .23)) + .23);
            percent = ((percent * (0.60 - 0.25)) + 0.25);
            //this line should set a min and max for the double%

            MP += (int)(maxMP * percent);
            //user rests and gains back some of their stamina
            Combat.output(Name + " rests.");
        }

        public void damage(float dmg, string element, bool percent)
        {
            int damage;

            if (percent)
            {
                damage = Convert.ToInt32(Math.Ceiling(maxHP * (dmg / 100f)));
            }
            else
            {
                damage = Convert.ToInt32(Math.Ceiling(dmg));
            }

            HP -= damage;
            Combat.output(Name+" took "+ damage.ToString() + " " + element + " damage!");
        }

        public void heal(float dmg, bool percent)
        {
            int damage;
            if (percent)
            {
                damage = Convert.ToInt32(Math.Ceiling(maxHP * (dmg / 100f)));
            }
            else { damage = Convert.ToInt32(Math.Ceiling(dmg));}

            HP += damage;
            if(HP > maxHP){ HP = maxHP; } //make sure we don't heal them past full

            Combat.output(Name + " was healed for " + damage.ToString() + "!");
        }

        public void healEnergy(float dmg, bool percent)
        {
            int damage;
            if (percent)
            {
                damage = Convert.ToInt32(Math.Ceiling(maxMP * (dmg / 100f)));
            }
            else { damage = Convert.ToInt32(Math.Ceiling(dmg)); }

            MP += damage;
            if (MP > maxMP) { MP = maxMP; }

            if (damage > -1) { Combat.output(Name + " recovered " + damage.ToString() + " energy!"); }
            else { Combat.output(Name + "lost " + (-1*damage).ToString() + " energy!"); }
        }

		public void useStamina (float stam)
		{
			int damage = Convert.ToInt32 (Math.Ceiling((stamina*100)*(stam/100f)));
			//get the characters base stamina and find the proper percentage of that
			MP -= damage;
			//take the mp away from the character
		}

		public bool checkStamina (int snum)
		{
			int stUse = Convert.ToInt32(Math.Ceiling ((skills[snum].staminaUse / 100f)));
			if (MP > stUse) 
			{
				//can use the skill
				return true;
			}
			return false;
		}

        public void addStatus(string status)
        {
            //Console.WriteLine(Name+":ADDSTATUS: " + status);
            Status nStatus = new Status(this,status);
            if (nStatus.checkCount())
            {   //check if we don't have too many stacked yet
                statuses.Add(nStatus);
            }            
            //add a status to the characters status list
        }

        public bool deathCheck()
        {
            if (HP < 1) { return true; }
            return false;
        }


    }
}
