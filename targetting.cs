using System;
using System.Collections.Generic;

namespace battle
{
	public partial class targetting : Gtk.Dialog
	{
		internal Skill skill;
		internal MainWindow mainform;
		internal BattleControl BC;

		internal List<Character> targetList;
		internal List<Character> finalTargets;

		internal bool partySelectMode = false;
		internal string selectedCharacter = null;

		public targetting (Skill selectedSkill, MainWindow selectedMainform)
		{
			this.Build ();

			this.Title = selectedSkill.Name;

			skill = selectedSkill;
			mainform = selectedMainform;
			BC = mainform.BC;

			targetList = new List<Character>();
			finalTargets = new List<Character>();

			checkSkill();
			setTargetNames();
		}

		public targetting (MainWindow selectedMainform)
		{
			this.Build ();

			this.Title = "Party Select:";

			partySelectMode = true;
			mainform = selectedMainform;
			BC = mainform.BC;

			//populate a list of party characters
			//allow the user to select one
			//on ok, BC.addCharacter();
			populatePartyList();
		}

		internal void populatePartyList ()
		{
			checkbutton2.Sensitive = false;
			checkbutton3.Sensitive = false;
			checkbutton4.Sensitive = false;

			if (mainform.partyCharacters.Count > 0) {
				checkbutton2.Label = mainform.partyCharacters[0];
				checkbutton2.Sensitive = true;
			}
			if (mainform.partyCharacters.Count > 1) {
				checkbutton3.Label = mainform.partyCharacters[1];
				checkbutton3.Sensitive = true;
			}
			if (mainform.partyCharacters.Count > 2) {
				checkbutton4.Label = mainform.partyCharacters[2];
				checkbutton4.Sensitive = true;
			}
		}


		internal void checkSkill ()
		{
			//set all of the possible targets for the skill
			if (skill.targetGroup == "enemies") {
				targetList = BC.NPCS;
			}
			if (skill.targetGroup == "allies") {
				targetList = BC.PCS;
			}
			if (skill.targetGroup == "self") {
				targetList.Add (BC.currentCharacter);
				checkbutton2.Active = true;
				checkbutton3.Sensitive = false;
				checkbutton4.Sensitive = false;
			}

			//if the skill hits everything, select them all
			//and disable the ability to not target them
			if (skill.targetNumber == "all") {
				checkbutton2.Active = true;
				checkbutton3.Active = true;
				checkbutton4.Active = true;

				checkbutton2.Sensitive = false;
				checkbutton3.Sensitive = false;
				checkbutton4.Sensitive = false;
			}
			if (skill.targetNumber == "all") {
				checkbutton2.Active = true;
				checkbutton3.Active = true;
				checkbutton4.Active = true;

				checkbutton2.Sensitive = false;
				checkbutton3.Sensitive = false;
				checkbutton4.Sensitive = false;
			}
		}

		private void setTargetNames ()
		{
			checkbutton2.Label = targetList [0].Name;

			if (targetList.Count > 1) {
				checkbutton3.Label = targetList[1].Name;
			}

			if (targetList.Count > 2) {
				checkbutton4.Label = targetList[2].Name;
			}
		}

		internal void finalizeTarget ()
		{
			finalTargets.Clear ();
			if (checkbutton2.Active) {
				finalTargets.Add (targetList [0]);
			}
			if (checkbutton3.Active) {
				finalTargets.Add (targetList [1]);
			}
			if(checkbutton4.Active){
				finalTargets.Add (targetList[2]);
			}

		}

		protected void OnClose (object sender, EventArgs e)
		{
			//Console.WriteLine (this.Name+" Closed");
		}

		protected void OnCheckbutton2Toggled (object sender, EventArgs e)
		{
			if (partySelectMode) {
				selectedCharacter = mainform.partyCharacters[0];
				if(checkbutton3.Active){checkbutton3.Active = false;}
				if(checkbutton4.Active){checkbutton4.Active = false;}
				return;
			}
			if (skill.targetNumber == "single") {
				if(checkbutton3.Active){checkbutton3.Active = false;}
				if(checkbutton4.Active){checkbutton4.Active = false;}
				//checkbutton2.Active = true;
			}
		}

		protected void OnCheckbutton3Toggled (object sender, EventArgs e)
		{
			if (partySelectMode) {
				selectedCharacter = mainform.partyCharacters[1];
				if(checkbutton2.Active){checkbutton2.Active = false;}
				if(checkbutton4.Active){checkbutton4.Active = false;}
				return;
			}
			if (skill.targetNumber == "single") {
				if(checkbutton2.Active){checkbutton2.Active = false;}
				if(checkbutton4.Active){checkbutton4.Active = false;}
				//checkbutton3.Active = true;
			}
		}

		protected void OnCheckbutton4Toggled (object sender, EventArgs e)
		{
			if (partySelectMode) {
				selectedCharacter = mainform.partyCharacters[2];
				if(checkbutton2.Active){checkbutton2.Active = false;}
				if(checkbutton3.Active){checkbutton3.Active = false;}
				return;
			}
			if (skill.targetNumber == "single") {
				if(checkbutton2.Active){checkbutton2.Active = false;}
				if(checkbutton3.Active){checkbutton3.Active = false;}
				//checkbutton4.Active = true;
			}
		}

		protected void OnButtonOkReleased (object sender, EventArgs e)
		{
			if (partySelectMode) {
				if (selectedCharacter == null) {
					return;
				}
				BC.addCharacter (selectedCharacter);
				OnClose ();
			}
			else {
				finalizeTarget ();
				//no targets were selected, don't close yet
				if (finalTargets.Count < 1) {
					return;
				}

				//we some targets, use em up
				mainform.BC.getTargets (skill, finalTargets);
				OnClose ();
			}
		}

		protected void OnButtonCancelReleased (object sender, EventArgs e)
		{
			OnClose ();
		}

	}
}

