using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ini;

namespace battle
{
    public class Status
    {
        public string Name { get; set; }
        public string type;	//boon, ailment, intrinsic
        public string step;	//when it activates during a turn

        public string description;

        public bool toBeRemove = false;

        public Character owner;

        public int stack { get; set; }	//how many can be stacked
        public int count { get; set; }	//how many are currently on character

        public int length { get; set; }	//how long it will last, in turns
        public int turns { get; set; }	//how many turns it has been around

        internal string functions;

        public int activateChance;
		public int activationsPerRound;
		public int activations;

        public Status(Character newOwner, string newStatus)
        {
            owner = newOwner;
            IniFile newFile = new IniFile("../../Status/" + newStatus + ".stat");

			activations = 0;
            //Console.WriteLine("Loading status " + newStatus);
            loadStatus(newFile);
        }

        public void loadStatus(IniFile currentFile)
        {
            string statusName = System.IO.Path.GetFileNameWithoutExtension(currentFile.path);
           // Console.WriteLine("setting up status " + statusName);
            //use path operation to get the section name of the status
            Name = currentFile.IniReadValue(statusName, "name");
            step = currentFile.IniReadValue(statusName, "step");
            type = currentFile.IniReadValue(statusName, "type");

			activationsPerRound = Convert.ToInt32(currentFile.IniReadValue(statusName,"activatesPerRound"));
			//if activations per round is 0, then no limit

            description = currentFile.IniReadValue(statusName, "desc");


            functions = currentFile.IniReadValue(statusName, "function");

            string[] commands = functions.Split(new string[] { ";" }, System.StringSplitOptions.None);

            activateChance = Convert.ToInt32(currentFile.IniReadValue(statusName, "activateChance"));
            stack = Convert.ToInt32(currentFile.IniReadValue(statusName, "stack"));
            length = Convert.ToInt32(currentFile.IniReadValue(statusName, "length"));


        }

        public virtual void added()
        {
           //procs to run when a status is first added
        }

        public void readFunction()
        {
            string[] commands = functions.Split(new string[] { ";" }, System.StringSplitOptions.None);
            for (int i = 0; i < commands.Length; i++)
            {
                string[] args = commands[i].Split(new string[] { "," }, System.StringSplitOptions.None);
                if (args[0] == "harm")
                {
                    harm(Convert.ToInt32(args[1]), args[2]);
                }
                if (args[0] == "heal")
                {
                    heal(Convert.ToInt32(args[1]), bool.Parse(args[2]));
                }
                if (args[0] == "drain")
                {
                    energy(Convert.ToInt32(args[1]), args[2], bool.Parse(args[3]));
                }
				if(args[0]=="counter")
				{
					react(args[1],args[2]);
				}
            }
            /*
              absorbDamage(#absorbed, from skillType)
        activate(skillName or skillType)
        addArmor(# to add, vs skillType)
        addDamage(% to increase, of skillType)
        addResist(# to change, resistance)
        addStat(# to change, stat to change)
        addStatus(% chance,status to add,target,onEnd?)*
        energy(#to energy, element, asPercentage) 
        evasion(% chance, to evade skillType)
        Harm(% to harm, elemnt) acts as percentage
        Heal(% to heal, asPercentage)
        protect(% chance to protect, from skillType)
        response(skill to use, target)
             */
        }

        public virtual void activate ()
		{
			if (activationsPerRound != 0 && activations == activationsPerRound)
			{//already activated ths around, don't do it
				return;
			}

            if (Combat.rollCheck(activateChance, 101))
            {		//status activates
                readFunction();
				activations++;
            }
            else
            {
                return;
            }
        }

		void react (string reactTo, string reactWith)
		{
			//check if the defending skill is the type to reactto
			//find a skill in the owner of type to reactwith
			//use that skill on the characters attacker
			if (owner.defendingSkill.method == reactTo)
			{
			Combat.output(owner.Name+" reacting to "+owner.lastAttacker.Name+"'s "+owner.defendingSkill.Name);
				for(int s = 0; s < owner.skills.Count; s++)
				{
					if(owner.skills[s].method == reactWith)
					{
						List<Character> target = new List<Character>();
						target.Add(owner.lastAttacker);
						owner.useSkill(s,target);
						return;
					}
				}
			}
		}

        void harm(int percent, string element){
            owner.damage(percent, element, true);
            //Combat.output(Name+" Activates on " + owner.Name);
        }

        void heal(int amount, bool asPercent)
        {
            //bool percent = bool.Parse(asPercent);
            owner.heal(amount, asPercent);
        }

        void energy(int energy, string element, bool asPercent)
        {
            owner.healEnergy(energy, asPercent);
            //energy(#to energy, element, asPercentage)
        }

        void boost(string stat, int percent)
        {
            //adds to the owners stat by a percentage
        }

        public void remove()
        {
            //incase there is a function to run here
            owner.statuses.Remove(this);
        }

        public void turnIncrease ()
		{
			if (type == "Intrinsic")
			{
				//intrinsics don't time out
				return;
			}
            if (turns == length)
            {
                toBeRemove = true;
                Console.WriteLine(Name + " is removing itself from " + owner.Name);
                return;
            }
            else
            {
                turns++;
            }
        }

        public bool checkCount()
        {
            int sCount = 0;
            foreach (Status s in owner.statuses)
            {
                if (s.Name == this.Name)
                {
                    sCount++;
                }
            }

            if (sCount < stack)
            { //there was room to add another count of status
                return true;
            }
            return false; //too many status of same type, return false
        }


    }

}
