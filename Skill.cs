using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ini;

namespace battle
{
    public class Skill
    {

        //public string skillName;
        public string Name;
        public string targetGroup; //enemies, allies, self
        public string targetNumber;//single, all

        public string description; //displayed on mouseover
        public string brand;    //offense, defense, support used by AI to determine
                                //which skill to use

        public bool freeAction; //doesn't end the turn

        public int coolDown; //how many rounds it takes to cool off
        public bool isWarm; //can't be used if true
        public int warmCount; //the count of how many rounds it has been warm

        public int accuracy; //how likely the skill hits
        public int criticalRatio; //the chance to do a critical hit
        public float criticalDamage; //how much to multiply damage by

        //for resistances and determing the type of the attack
        public string element;			    //fire, ice, etc
        public string method;				//melee, physical, ranged, magic
        public string damageType;			//piercing, slashing, blunt


        //public int attackNumber; //# of attacks, obsolete in new system
        public int minDamage;   //determined for better damage previewing
        public int maxDamage;
        public string attackStat;   //the stat used for attack, probably obsolete
        public string defenseStat;  //also obsolete

        public int staminaUse;  //how much stamina is used by the skill, eventually a %

        public Character user;
        public List<Character> targets;

        public string functions;

        public Skill(string newSkill)
        {
           // Console.WriteLine("opening skill file for " + newSkill);
            IniFile newFile = new IniFile("../../Skills/" + newSkill + ".skil");
            loadSkill(newFile);
        }

        void loadSkill(IniFile currentFile)
        {
            string skillName = System.IO.Path.GetFileNameWithoutExtension(currentFile.path);
            //TBName.Text = currentFile.IniReadValue(skillName, "name");

            Name = currentFile.IniReadValue(skillName, "name");
            targetGroup = currentFile.IniReadValue(skillName, "targetGroup");
            targetNumber = currentFile.IniReadValue(skillName, "targets");
            description = currentFile.IniReadValue(skillName, "desc");
            brand = currentFile.IniReadValue(skillName, "brand");

            staminaUse = Convert.ToInt32(currentFile.IniReadValue(skillName, "staminaUse"));
            freeAction = Convert.ToBoolean(currentFile.IniReadValue(skillName, "freeAction"));
            isWarm = Convert.ToBoolean(currentFile.IniReadValue(skillName, "isWarm"));
            coolDown = Convert.ToInt32(currentFile.IniReadValue(skillName, "coolDown"));
            accuracy = Convert.ToInt32(currentFile.IniReadValue(skillName, "hitChance"));
            criticalRatio = Convert.ToInt32(currentFile.IniReadValue(skillName, "criticalRatio"));
            criticalDamage = float.Parse(currentFile.IniReadValue(skillName, "criticalDamage"));
            //attackNumber = Convert.ToInt32(currentFile.IniReadValue(skillName, "attackNumber"));
            element = currentFile.IniReadValue(skillName, "element");
			method = currentFile.IniReadValue (skillName,"method");
			damageType = currentFile.IniReadValue (skillName,"damageType");

            functions = currentFile.IniReadValue(skillName, "function");

            string[] commands = functions.Split(new string[] { ";" }, System.StringSplitOptions.None);
            for (int i = 0; i < commands.Length; i++)
            {
                string[] args = commands[i].Split(new string[] { "," }, System.StringSplitOptions.None);
                if (args[0].Equals("attack"))
                { //this was put in for display purposes of demo damage
                    minDamage += int.Parse(args[1]);
                    maxDamage += int.Parse(args[2]);
                }
            }

        }
        internal void scriptRead(string input)
        {
            //input = "walk,5;attackUp,Anger,1";
            string[] commands = input.Split(new string[] { ";" }, System.StringSplitOptions.None);
            bool statusContinue = true;

            for (int i = 0; i < commands.Length; i++)
            {
                string[] args = commands[i].Split(new string[] { "," }, System.StringSplitOptions.None);

                if (args[0].Equals("attack"))
                {
                    attack(int.Parse(args[1]), int.Parse(args[2]), args[3]);
                }
                if (args[0].Equals("addStatus"))
                {
                    if (statusContinue)
                    {	//if this is false, the statuses before had an endflag so we stop looking for statuses to add
                        statusContinue = addStatus(args[1], int.Parse(args[2]), int.Parse(args[3]), args[4], Convert.ToBoolean(args[5]));
                    }
                }
                if (args[0].Equals("addTurn"))
                {
                    //somehow add another turn of the user into the TC?
                }
                if (args[0].Equals("alwaysCrit"))
                {
                    //change the skills accuracy so that it is a critical hit
                }
                if (args[0].Equals("alwaysHit"))
                {
                    //chance the skill so that it hits, but is not necessarily critical
                }
                if (args[0].Equals("createItem"))
                {
                }
                if (args[0].Equals("changeSet"))
                {
                }
                if (args[0].Equals("heal"))
                {
                    heal(int.Parse(args[1]), args[2], args[3]);
                }
                if (args[0].Equals("ignoreArmor"))
                {
                }
                if (args[0].Equals("removeBuffs"))
                {
                }
                if (args[0].Equals("removeDebuffs"))
                {
                }
                if (args[0].Equals("removeStatus"))
                {
                }
                if (args[0].Equals("useAmmo"))
                {
                    //remove the selected status,
                    //attack
                    //repeat until all of the status are depleted
                }

                /*
                 addStatus(status,#ofstacks,chance%,target,endFlag)
addTurn(#ofTurns,target)
alwaysCrit()
alwaysHit()
attack(#dmgPerDie,statToUse,target)
changeSet(skill1,skill2,skill3,skill4)
createItem(item,chance%,target,endFlag)
heal(#toheal%,Health/Stamina,target)
ignoreArmor(#pointsOfArmor)
removeBuffs(chance%,target)
removeDebuffs(chance%,target)
removeStatus(status,chance%,target)
useAmmo(status) */
                //if(args[0].Equals("run"))
                //{
                //	doRunCode(int.parse(args[1]),args[2],..);
                //}
                /*etc etc check args[0] for the function you want to call and every following index is a variable to pass*/
            }
        }

        private bool addStatus(string status, int stacks, int chance, string target, bool endFlag)
        {
            if (target == "enemies")
            {
                foreach (Character c in targets)
                {
                    if (c.team != user.team)
                    {
                        if (Combat.rollCheck(chance, 101))
                        {
                            c.addStatus(status);
                        }
                    }
                }
                if (endFlag) { return false; }
                else return true;
            }
            if (target == "allies")
            {
                foreach (Character c in targets)
                {
                    if (c.team == user.team && c != user)
                    {//use on allies and not self
                        if (Combat.rollCheck(chance, 101))
                        {
                            c.addStatus(status);
                        }
                    }
                }
                if (endFlag) { return false; }
                else return true;
            }
            if (target == "self")
            {
                if (Combat.rollCheck(chance, 101))
                {
                    user.addStatus(status);
                }
                if (endFlag) { return false; }
                else return true;
            }
            return true;
        }

        private void heal(int percent, string whatToHeal, string targetGroup)
        {
            if (whatToHeal == "health")
            {
                switch (targetGroup)
                {
                    case "enemies":
                        foreach (Character c in targets)
                        {
                            if (c.team != user.team)
                            {
                                c.heal(percent, true);
                            }
                        }
                        break;
                    case "allies":
                        foreach (Character c in targets)
                        {
                            if (c.team == user.team && c != user)
                            {//use on both allies and not self
                                c.heal(percent, true);
                            }
                        }
                        break;
                    case "self":
                        foreach (Character c in targets)
                        {
                            if (c == user)
                            {
                                c.heal(percent, true);
                            }
                        }
                        break;
                }
            }
            if(whatToHeal == "stamina")
            {
                switch (targetGroup)
                {
                    case "enemies":
                        foreach (Character c in targets)
                        {
                            if (c.team != user.team)
                            {
                                c.healEnergy(percent, true);
                            }
                        }
                        break;
                    case "allies":
                        foreach (Character c in targets)
                        {
                            if (c.team == user.team && c != user)
                            {//use on both allies and not self
                                c.healEnergy(percent, true);
                            }
                        }
                        break;
                    case "self":
                        foreach (Character c in targets)
                        {
                            if (c == user)
                            {
                                c.healEnergy(percent, true);
                            }
                        }
                        break;
                }
            }
        }

        private void attack(int min, int max, string targetGroup)
        {
                switch (targetGroup)
                {
                    case "enemies":
                        foreach (Character c in targets)
                        {
                            int AC = accuracyCheck();
                            if (c.team != user.team && AC > 0)
                            {
								c.defendingSkill = this;
								c.lastAttacker = user;
                                Combat.output(user.Name + " hits " + c.Name + " with " + Name);
                                float dmg = getDamage();
                                if (AC == 2) { dmg = criticalHit(dmg); } //critical hit
                                c.damage(dmg, element, false);
                            }
                            else { Combat.output(user.Name + " missed " + c.Name + " with " + Name); }
                        }
                        break;

                    case "allies":
                        foreach (Character c in targets)
                        {
                            int AC = accuracyCheck();
                            if (c.team == user.team && c != user && AC > 0)
                            {
								c.defendingSkill = this;
								c.lastAttacker = user;
                                Combat.output(user.Name + " hits " + c.Name + " with " + Name);
                                float dmg = getDamage();
                                if (AC == 2) { dmg = criticalHit(dmg); } //critical hit
                                c.damage(dmg, element, false);
                            }
                            else { Combat.output(user.Name + " missed " + c.Name + " with " + Name); }
                        }
                        break;

                    case "self":
                        foreach (Character c in targets)
                        {
                            int AC = accuracyCheck();
                            if (c == user && AC > 0)
                            {
                                Combat.output(user.Name + " hits themselves with " + Name);
                                float dmg = getDamage();
                                if (AC == 2) { dmg = criticalHit(dmg); }
                                c.damage(dmg, element, false);
                            }
                        }
                        break;

                    default:
                        Combat.output("What happened here? Attack no targets?");
                        break;
                }
        }

        public bool use(Character skillUser, List<Character> target)
        {
            user = skillUser;
			user.usingSkill = this;
            targets = target;

            if (this.isWarm)
            {
                return false;
            }

            if (useStamina()) //enough stamina, use skill
            {
                if (functions.Length > 0)
                {
                    scriptRead(functions);
                }

                if (coolDown > 0)
                {
                    isWarm = true;
                    warmCount = coolDown;
                }

                return true;
            }
            //skill wasn't used, return false
            return false;
        }

        public bool warmCheck()
        {
            if (warmCount == 1)
            {
                warmCount = 0;
                isWarm = false;
                return true;
            }
            else { warmCount--; return false; }
        }

        bool useStamina()
        {
            float staminaTotal = 0f;
            staminaTotal = user.stamina * 100f;
            staminaTotal = staminaTotal * (staminaUse / 100f);
            //turn the staminatotal into a percent of the users base stamina
            //drain that staminatotal from their current stamina
            if (user.MP >= staminaTotal)
            {
                //user.healEnergy((staminaTotal * -1), false);
				user.useStamina(staminaUse);
                return true;
            }
            Combat.output(user.Name + " doesn't have enough stamina to use " + Name);
            return false;//not enough mp, return false

        }

        int accuracyCheck()
        {
            int check = 0;
            float targetNum = ((user.tempAccuracy + accuracy)/targets[0].tempEvasion)*100f;
            if (Combat.rollCheck((int)targetNum, 101)) { check = 1; }
            if (Combat.rollCheck((int)criticalRatio, 101)) { check = 2; }
            //roll for accuracy, return 1 if hit, 2 if crit            
            return check;
        }

        float getDamage()
        {
           // float totalDamage = 0f;
            List<int> damageDice = Combat.rollDice(user.tempAttack, 10);

            int successes = Combat.countSuccesses(damageDice,targets[0].tempDefense);
            //int dmg = Combat.rng.Next(minDamage, maxDamage + 1);
            float firstDamage = successes * (Combat.rng.Next(minDamage, maxDamage + 1));
            float secondDamage = (firstDamage * user.tempAttack * user.tempAttack) / (user.tempAttack * targets[0].tempDefense);
            //roll for total damage - defense
            return secondDamage;
        }

        float criticalHit(float dmg)
        {
            Combat.output("SMAAAAASH!");
            float totalDamage = dmg * criticalDamage;
            return totalDamage;
        }

        float resistDamage(float dmg)
        {
            float totalDamage = 0f;
            int resistance = 0;
            //get string of skills elemental type
            foreach (KeyValuePair<string, int> resist in targets[0].resistances)
            {
                if (resist.Key == element)
                {
                    resistance = resist.Value;
                    //get the value of the resistance of the target
                }
            }
            totalDamage = dmg + (dmg * (resistance/100f));
            //check all of the targets resistances, edit dmg accordingly
            return totalDamage;
        }

    } //end skill class

}
