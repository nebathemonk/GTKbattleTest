using System;
using System.Collections.Generic;
using Gtk;

namespace battle{

public partial class MainWindow: Gtk.Window
{	
	public BattleControl BC;
	public List<string> partyCharacters;

	internal int roundCount = 0;

	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
		partyCharacters = new List<string>();
		BC = new BattleControl(this,"TestBattle","Neba");
		
		roundCounter.Text = "Rounds: "+roundCount.ToString();
		menuButton.Clicked += MenuButton_clicked;

	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	internal void output (string outputText)
	{
		AppendText (this.Text.Buffer,(outputText));
		//this.Text.Buffer.Text += (outputText+"\n");
	}

	public void AppendTextWithoutScroll (TextBuffer buffer, string text) 
 	{
 		TextIter iter;
 		buffer.MoveMark(buffer.InsertMark, buffer.EndIter);
		if(text != null) 
 		{
 			if (text.Equals ("") == false) 
 			{				
 				iter = buffer.EndIter;
 				buffer.Insert (iter, text);
 			}
 		}
 		iter = buffer.EndIter;
 		buffer.Insert (iter, "\n");
 	}
 

 	public void AppendText (TextBuffer buffer, string text)
	{
		AppendTextWithoutScroll (buffer, text);
		while (Application.EventsPending ()) 
			Application.RunIteration ();
		Text.ScrollToMark (buffer.InsertMark, 0.4, true, 0.0, 1.0);
	}

	public void findTeams ()
		{
			//sorts out all of the combatants in BC.allcharacters
			//if they are on Player team, add them to player side for gui
			//if they are on NPC team, add them to npc side for gui

			foreach (Character c in BC.NPCS) {
				int sameCounter = 1;
				for(int b=0; b < BC.NPCS.Count; b++){
					if(c != BC.NPCS[b] && c.Name == BC.NPCS[b].Name){
						sameCounter++;
						BC.NPCS[b].Name += " "+sameCounter.ToString();
					}
				}
			}
		}

	public void setStats ()
		{
			//check all of the characters and set stat labels
			//refreshes the screen, basically, after a turn
			PC1Name.Text = BC.PCS [0].Name;
			PC1HP.Text = "HP: " + BC.PCS [0].HP + "/" + BC.PCS [0].maxHP;
			PC1MP.Text = "MP: " + BC.PCS [0].MP + "/" + BC.PCS [0].maxMP;
			PC1Status.Text = null;
			foreach (Status s in BC.PCS[0].statuses) {
				PC1Status.Text += s.Name + " ";
			}

			if (BC.PCS.Count > 1) {
				PC2Name.Text = BC.PCS [1].Name;
				PC2HP.Text = "HP: " + BC.PCS [1].HP + "/" + BC.PCS [1].maxHP;
				PC2MP.Text = "MP: " + BC.PCS [1].MP + "/" + BC.PCS [1].maxMP;
				PC2Status.Text = null;
				foreach (Status s in BC.PCS[1].statuses) {
					PC2Status.Text += s.Name + " ";
				}
			}

			if (BC.PCS.Count > 2) {
				PC3Name.Text = BC.PCS [2].Name;
				PC3HP.Text = "HP: " + BC.PCS [2].HP + "/" + BC.PCS [2].maxHP;
				PC3MP.Text = "MP: " + BC.PCS [2].MP + "/" + BC.PCS [2].maxMP;
				PC3Status.Text = null;
				foreach (Status s in BC.PCS[2].statuses) {
					PC3Status.Text += s.Name + " ";
				}
			}

			if (BC.NPCS.Count > 0) {
				NPC1Name.Text = BC.NPCS [0].Name;
				NPC1HP.Text = "HP: " + BC.NPCS [0].HP + "/" + BC.NPCS [0].maxHP;
				NPC1MP.Text = "MP: " + BC.NPCS [0].MP + "/" + BC.NPCS [0].maxMP;
				NPC1Status.Text = null;
				foreach (Status s in BC.NPCS[0].statuses) {
					NPC1Status.Text += s.Name + " ";
				}
			}

			if (BC.NPCS.Count > 1) {
				NPC2Name.Text = BC.NPCS [1].Name;
				NPC2HP.Text = "HP: " + BC.NPCS [1].HP + "/" + BC.NPCS [1].maxHP;
				NPC2MP.Text = "MP: " + BC.NPCS [1].MP + "/" + BC.NPCS [1].maxMP;
				NPC2Status.Text = null;
				foreach (Status s in BC.NPCS[1].statuses) {
					NPC2Status.Text += s.Name + " ";
				}
			}

			if (BC.NPCS.Count > 2) {
				NPC3Name.Text = BC.NPCS [2].Name;
				NPC3HP.Text = "HP: " + BC.NPCS [2].HP + "/" + BC.NPCS [2].maxHP;
				NPC3MP.Text = "MP: " + BC.NPCS [2].MP + "/" + BC.NPCS [2].maxMP;
				NPC3Status.Text = null;
				foreach (Status s in BC.NPCS[2].statuses) {
					NPC3Status.Text += s.Name + " ";
				}
			}


			//also. change the skill labels 
			setSkills();
		}

	internal void setSkills ()
		{
			skillButton1.Sensitive = false;
			skillButton2.Sensitive = false;
			skillButton3.Sensitive = false;
			skillButton4.Sensitive = false;

			if (BC.currentCharacter.checkStamina(0) && !BC.currentSkills[0].isWarm) {
				skillButton1.Sensitive = true;
			}
			skillButton1.Label = BC.currentSkills[0].Name;

			if (BC.currentCharacter.skills.Count > 1) {
				if(BC.currentCharacter.checkStamina(1) && !BC.currentSkills[1].isWarm)
				   { skillButton2.Sensitive = true;}
				skillButton2.Label = BC.currentSkills[1].Name;
			}
			if (BC.currentCharacter.skills.Count > 2 && !BC.currentSkills[2].isWarm) {
				if(BC.currentCharacter.checkStamina(2))
				   { skillButton3.Sensitive = true;}
				skillButton3.Label = BC.currentSkills[2].Name;
			}
			if (BC.currentCharacter.skills.Count > 3 && !BC.currentSkills[3].isWarm) {
				if(BC.currentCharacter.checkStamina(3))
				   { skillButton4.Sensitive = true;}
				skillButton4.Label = BC.currentSkills[3].Name;
			}
		}

	internal void MenuButton_clicked (object s, EventArgs e)
		{
			if (BC.inBattle)
			{
				//open up the menu
			} 
			else
			{
				BC.startCombat();
				menuButton.Label = "Menu";
			}
		}
	
	internal void countRound()
		{
			roundCount++;
			roundCounter.Text = "Round: "+roundCount.ToString();
		}



		protected void OnSkillButton1Released (object sender, EventArgs e)
		{
			if (BC.currentCharacter != null) {
				BC.useSkill (BC.currentCharacter, 0);
			}
			//throw new System.NotImplementedException ();
		}
		protected void OnSkillButton2Released (object sender, EventArgs e)
		{
			if(BC.currentCharacter != null){
				if (BC.currentCharacter.skills.Count > 1) {
				BC.useSkill (BC.currentCharacter, 1);
				}
			}
			//throw new System.NotImplementedException ();
		}
		protected void OnSkillButton3Released (object sender, EventArgs e)
		{
			if(BC.currentCharacter != null){
				if (BC.currentCharacter.skills.Count > 2) {
				BC.useSkill (BC.currentCharacter, 2);
				}
			}
			//throw new System.NotImplementedException ();
		}
		protected void OnSkillButton4Released (object sender, EventArgs e)
		{
			if(BC.currentCharacter != null){
				if (BC.currentCharacter.skills.Count > 3) {
				BC.useSkill (BC.currentCharacter, 3);
				}
			}
			//throw new System.NotImplementedException ();
		}


		protected void PartySelect (object sender, EventArgs e)
		{
			targetting PartyDialog = new targetting(this);
			//throw new System.NotImplementedException ();
		}





}
}
