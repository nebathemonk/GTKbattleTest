using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace battle
{
    class AI
    {
        //ferocity (all out attack, somewhat defense, etc)
        //type of skills (boosts, attacks, etc)
        //who they attack (weakest, strongest, magic users, etc)
        //how likely they follow the best course of action
        enum Ferocity { Calm, Mild, Balanced, Fierce, Extreme };
        enum Target { Weakest, Strongest, MagicUser, Any };
        enum Natural { Offense, Defense, Support, Balanced };
        enum Intelligence { Chaotic, Low, Normal, High, Mastermind};

        Ferocity intensity;
        Target perferredTarget;
        Intelligence intelligence;
        Natural state;

        int defensePull;
        int offensePull;
        int supportPull;


        Skill skillToUse;
        List<Character> targets;

        Character owner;

        BattleControl BC;

        public AI(Character IAM)
        {
            intensity = Ferocity.Balanced;
            perferredTarget = Target.Any;
            state = Natural.Balanced;
            intelligence = Intelligence.Normal;
            owner = IAM;
        }

        public void startAITurn(BattleControl Battle)
        {
            //Console.WriteLine("starting AI turn.");
            BC = Battle;

            targets = new List<Character>();

            bool skillFound = false;
            do
            {
                skillFound = findSkill();
            } while (!skillFound);
            
            BC.findTurn();
        }

        bool findSkill()
        {
            if(!checkStamina())
            {
                //didn't have enough stamina to do shit, just rest instead
                owner.rest();
                return true;
            }

            if (!intelligencePull())
            {   //chaos didn't overtake them, use pulls
                determinePulls();
                checkPulls();
                do
                {   //keep trying to figure out the skill until we do
                    decideSkill();
                } while (!testSkill());


                if (skillToUse.targetNumber == "all")
                {   //skill just effects all of a side, so no need to determine target
                    switch (skillToUse.targetGroup)
                    {
                        case "enemies":
                            targets.AddRange(BC.PCS);
                            break;
                        case "allies":
                            targets.AddRange(BC.NPCS);
                            break;
                        case "self":
                            targets.Add(owner);
                            break;
                    }
                    if (skillToUse.use(owner, targets)) { return true; }
                }
                else
                {
                    if (skillToUse.use(owner, findTarget())) { return true; }
                }
            }

            else
            {   //Chaos!! just use random shit on random guys!
                skillToUse = randomTurn();
                if (skillToUse.use(owner, randomTarget())) { return true; }
            }
            //all failed, return false
            return false;
        }

        bool checkStamina()
        {
            bool notEnough = true;
            if (owner.MP < (owner.maxMP / 20))
            { //if the owner has less than 1/20th of stamina return false
                notEnough = false;
            }
            return notEnough;
        }

        public void determinePulls()
        {
            defensePull = 0;
            offensePull = 0;
            supportPull = 0;

            getStatePull(); //adds to the pull of the natural state
            teamSize();
            checkTeamStatus();
            checkStatus();

            offensePull += ferocity();
            defensePull += myHealth();
            supportPull += teammatesHealth();
            offensePull += enemiesHealth();
            //my stamina left

           // Console.WriteLine(owner.Name+"'s offense pull is "+offensePull.ToString());
           // Console.WriteLine(owner.Name + "'s defense pull is " + defensePull.ToString());
           // Console.WriteLine(owner.Name + "'s support pull is " + supportPull.ToString());

            //checkPulls();
        }

        public bool intelligencePull()
        {
            bool chaos = false;

            switch(intelligence){
                case Intelligence.Chaotic:
                    chaos = Combat.rollCheck(60, 101);
                    //sixty percent chance to make random move
                    break;
                case Intelligence.Low:
                    chaos = Combat.rollCheck(50, 101);
                    break;
                case Intelligence.Normal:
                    chaos = Combat.rollCheck(35, 101);
                    break;
                case Intelligence.High:
                    chaos = Combat.rollCheck(15, 101);
                    break;
                case Intelligence.Mastermind:
                    chaos = Combat.rollCheck(5, 101);
                    break;
                default:
                    chaos = false;
                    break;
            }
            Console.WriteLine(owner.Name + " chaos set to " + chaos.ToString());
            return chaos;
        }

        public Skill randomTurn()
        {
            Skill randomSkill;
            //chaos took over the character and they ignore pulls
            int rng = Combat.rng.Next(0, (owner.skills.Count));
            randomSkill = owner.skills[rng];
            return randomSkill;
        }

        public List<Character> randomTarget()
        {
         List<Character> finalTarget = new List<Character>();

            if (skillToUse.targetNumber == "all")
            {   //skill just effects all of a side, so no need to determine target
                switch (skillToUse.targetGroup)
                {
                    case "enemies":
                        finalTarget.AddRange(BC.PCS);
                        break;
                    case "allies":
                        finalTarget.AddRange(BC.NPCS);
                        break;
                    case "self":
                        finalTarget.Add(owner);
                        break;
                }
                return finalTarget;
            }
         List<Character> possibleTargets = new List<Character>();
         if (skillToUse.targetGroup == "enemies") { possibleTargets.AddRange(BC.PCS); }
         if (skillToUse.targetGroup == "allies") { possibleTargets.AddRange(BC.NPCS); }
         if (skillToUse.targetGroup == "self") { possibleTargets.Add(owner); }

        int randTarget = Combat.rng.Next(0, (possibleTargets.Count));
        finalTarget.Add(possibleTargets[randTarget]);

        return finalTarget;
        }

        public string checkPulls()
        {
            if (offensePull > defensePull && offensePull > supportPull) { return "offense"; }
            else if (defensePull > offensePull && defensePull > supportPull) { return "defense"; }
            else if (supportPull > defensePull && supportPull > offensePull) { return "support";}
            else
            {
                switch (state)
                {
                    case Natural.Offense: return "offense";
                    case Natural.Defense: return "defense";
                    case Natural.Support: return "support";
                    default: return "offense";
                }
            }
        }

        public void getStatePull()
        {
            if (state == Natural.Offense) { offensePull++; }
            if (state == Natural.Defense) { defensePull++; }
            if (state == Natural.Support) { supportPull++; }

            switch (Combat.rng.Next(1, 4))
            {   //throw a lil spice in there randomly
                case 1:
                    offensePull++;
                    break;
                case 2:
                    defensePull++;
                    break;
                case 3:
                    supportPull++;
                    break;
                default:
                    break;
            }
        }
        public int ferocity()
        {
            //Calm 0, Mild 1, Balanced 2, Fierce 3, Extreme 4
            return (int)intensity;
        }

        public int myHealth()
        {
            int healthPull = 0;
            if (owner.HP < (owner.maxHP * 0.75))
            {
                healthPull++;
            }
            if (owner.HP < (owner.maxHP * 50))
            {
                healthPull++;
            }
            if (owner.HP < (owner.maxHP * 25))
            {//low health, in bottom 1/4. Higher chance to be defensive.
                healthPull++;
            }
            return healthPull;
        }

        public int teammatesHealth()
        {
            int healthPull = 0;
            foreach (Character teammate in BC.NPCS)
            {
                if (teammate.HP < (teammate.maxHP * 0.33))
                {
                    healthPull++;
                }
            }
            return healthPull;
        }

        public int enemiesHealth()
        {
            int offensePull = 0;
            foreach (Character enemy in BC.PCS)
            {
                if (enemy.HP < (enemy.maxHP * 0.50)) { offensePull++; }
                if (enemy.HP < (enemy.maxHP * 0.33)) { offensePull++; }
            }
            return offensePull; 
        }

        public void teamSize()
        {
            if (BC.NPCS.Count < BC.PCS.Count)
            {
                //there are fewer teammates left than enemies be defensive
                defensePull++;
            }
            else
            {
                offensePull++;
            }
        }

        public void checkTeamStatus()
        {
            int debuffs = 0;
            foreach (Character c in BC.NPCS)
            {
                foreach (Status s in c.statuses)
                {
                    if (s.type == "Curse")
                    {
                        debuffs++;
                    }
                    if (s.type == "Boon")
                    {
                        debuffs--;
                    }
                }
            }
            if (debuffs > 0)
            {
                supportPull++;
            }
            if (debuffs < 0)
            {
                offensePull++;
            }
        }

        public void checkStatus()
        {
            //look at all of the statuses and determine if we
            //need to use a support/defense to get rid of debuffs
            //or maybe need to attack because we have lots of buffs already
            int buffs = 0; int debuffs = 0;
            foreach (Status s in owner.statuses)
            {
                if (s.type == "Boon")
                {
                    buffs++;
                }
                if (s.type == "curse")
                {
                    debuffs++;
                }
            }
            if (buffs > debuffs) { offensePull++; }
            if (debuffs > buffs) { defensePull++; }
        }

        public List<Character> findTarget()
        {
            List<Character> possibleTargets = new List<Character>();
            if (skillToUse.targetGroup == "enemies") { possibleTargets.AddRange(BC.PCS); }
            if (skillToUse.targetGroup == "allies") { possibleTargets.AddRange(BC.NPCS); }
            if (skillToUse.targetGroup == "self") { possibleTargets.Add(owner); }

            List<Character> finalTarget = new List<Character>();

            switch (perferredTarget)
            {
                case Target.Any:
                    int randTarget = Combat.rng.Next(0, (possibleTargets.Count));
                    finalTarget.Add(possibleTargets[randTarget]);
                    break;
                case Target.Weakest:
                    findWeakest(possibleTargets);
                    break;
                case Target.Strongest:
                    findStrongest(possibleTargets);
                    break;
                case Target.MagicUser:
                    findMagicUser(possibleTargets);
                    break;
            }
            return finalTarget;
        }

        public Character findWeakest(List<Character> targets)
        {
            Character target;

            targets.Sort(delegate(Character p1, Character p2)
            {
                return p1.HP.CompareTo(p2.HP);
            }); //returns the lowest current hp in the group

            target = targets[0];
            return target;
        }

        public Character findStrongest(List<Character> targets)
        {
            Character target;

            targets.Sort(delegate(Character p1, Character p2)
            {
                return p2.tempAttack.CompareTo(p1.tempAttack);
            }); //returns the highest tempattack in the group

            target = targets[0];
            return target;
        }

        public Character findMagicUser(List<Character> targets)
        {
            Character target;

            targets.Sort(delegate(Character p1, Character p2)
            {
                return p1.tempSpirit.CompareTo(p2.tempSpirit);
            }); //returns the highest tempspirit in the group

            target = targets[0];
            return target;
        }

        public void decideSkill()
        {
            List<Skill> possibleSkills = new List<Skill>();
            //Skill skillToUse;

            foreach (Skill s in owner.skills)
            {
                if (s.brand == checkPulls()) { possibleSkills.Add(s); }
            }

            if (possibleSkills.Count < 1)
            {   //no possible skills found; find a totally random one
                skillToUse = randomTurn();
            }

            int randomSkill = Combat.rng.Next(0, (possibleSkills.Count));
            skillToUse = possibleSkills[randomSkill];
        }

        public bool testSkill()
        {
            //make sure that we have enough stamina to use skill
            //make sure that the skill is not in cooldown
            //make srue that there is a legal target for the skill
            //etc..
            if (skillToUse.isWarm) { return false; }
            return true;
        }


    }
}
