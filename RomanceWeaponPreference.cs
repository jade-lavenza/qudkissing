using System;
using System.Collections.Generic;
using XRL.Core;
using XRL.UI;
using XRL.World;
using XRL.World.Encounters;
using Qud.API;
using System.Linq;

namespace XRL.World.Parts
{
	[Serializable]
	public class acegiak_WeaponPreference : acegiak_RomancePreference
	{
        string wantedType = "Cudgel";
        float amount = 0;
        acegiak_Romancable Romancable = null;

        string ExampleName = "Club";
        List<string> tales = new List<string>();


        Dictionary<string, string> verbs = new Dictionary<string, string>()
        {
            { "Cudgel", "bashing" },
            { "ShortBlades", "stabbing" },
            { "LongBlades", "slashing" },
            { "Axe", "cleaving" }
        };

        Dictionary<string, string> presentable = new Dictionary<string, string>()
        {
            { "Cudgel", "cudgel" },
            { "ShortBlades", "short blade" },
            { "LongBlades", "long blade" },
            { "Axe", "axe" }
        };


        public acegiak_WeaponPreference(acegiak_Romancable romancable){
            GameObject sample = GameObjectFactory.Factory.CreateSampleObject(EncountersAPI.GetARandomDescendentOf("MeleeWeapon"));
            this.wantedType =  sample.GetPart<MeleeWeapon>().Skill;
            this.ExampleName = sample.ShortDisplayName;
            Romancable = romancable;

            Random r = new Random();
            amount = (float)(r.NextDouble()*2-0.9);
        }

        public acegiak_RomancePreferenceResult GiftRecieve(GameObject from, GameObject gift){
            float retamount = 0;
            string retexplain = "";
            if(gift.GetPart<MeleeWeapon>() != null && gift.GetPart<MeleeWeapon>().Skill == wantedType){
                return new acegiak_RomancePreferenceResult(amount,(amount >= 0 ?"&Glikes&Y the ":"&rdislikes&Y the ")+gift.pRender.DisplayName+"&Y.");
            }
            return null;
        }



        public acegiak_RomanceChatNode BuildNode(acegiak_RomanceChatNode node){
            string bodytext = "whoah";

			Random r = new Random();
            float g = (float)r.NextDouble();
            bool haskey = false;
            foreach(var item in verbs){
                if(item.Key == wantedType){
                    haskey = true;
                    break;
                }
            }

            if(g<0.3 && haskey){
                bodytext = "Do you ever think about just "+verbs[wantedType]+" people?";
                node.AddChoice("yeahcleave","Oh yes, quite often.",amount>0?"Oh good. I thought I was the only one.":"Really? That is troubling.",amount>0?1:-1);
                node.AddChoice("nahcleave","No, that sounds bad.",amount>0?"Oh, I guess it is. Sorry.":"It does, doesn't it? How scary!",amount>0?-1:1);
            }else if(g<0.6 && haskey){
                bodytext = "How do you like to slay your enemies?";
                foreach(var item in verbs){
                    if(item.Key == wantedType){
                        node.AddChoice(item.Key,"I like "+item.Value+" them with a "+presentable[item.Key]+".",amount>0?"Me too!":"That's quite violent, isn't it?",amount>0?1:-1);
                    }else{
                        node.AddChoice(item.Key,"I like "+item.Value+" them with a "+presentable[item.Key]+".",amount>0?"That sounds unpleasant.":"That's quite violent, isn't it?",amount>0?1:-1);
                    }
                }
                node.AddChoice("notmelee","I prefer to keep them at a distance.",amount>0?"That sounds cowardly.":"That sounds very wise.",amount>0?-1:1);
            }else{
                bodytext = "Do you have any interesting weapons?";
                Inventory part2 = XRLCore.Core.Game.Player.Body.GetPart<Inventory>();
                int c = 0;
                int s = 0;
                foreach(GameObject GO in part2.GetObjects())
                {
                    MeleeWeapon mw = null;
                    mw = GO.GetPart<MeleeWeapon>();
                    if(GO.GetBlueprint().InheritsFrom("MeleeWeapon") && mw != null){
                        if(mw.Skill == wantedType){
                            node.AddChoice("weapon"+c.ToString(),"I have this "+GO.DisplayName+".",amount>0?"Wow, that's very interesting!":"Oh, is that all?",amount>0?1:-1);
                            s++;
                        }else{
                            node.AddChoice("weapon"+c.ToString(),"I have this "+GO.DisplayName+".",amount>0?"Oh, is that all?":"Hmm, that seems dangerous.",amount>0?0:-1);
                            s++;
                        }
                    }
                    if(s>5){
                        break;
                    }
                    MissileWeapon rw = null;
                    rw = GO.GetPart<MissileWeapon>();
                    if(rw != null && rw.Skill != null){
                        node.AddChoice("weapon"+c.ToString(),"I have this "+GO.DisplayName+"&Y.",amount>0?"Oh, is that all?":"Hmm, that seems dangerous.",amount>0?0:-1);
                        s++;
                    }
                    if(s>5){
                        break;
                    }
                    c++;
                    
                }
                node.AddChoice("noweapons","Not really, no.",amount>0?"That's a pity.":"That's sensible. Weapons are dangerous.",amount>0?-1:1);
            }

            if(Romancable != null){
                node.Text = node.Text+"\n\n"+Romancable.GetStory();
            }
            node.Text = node.Text+"\n\n"+bodytext;

            return node;
        }


        public string GetStory(){
            Random r = new Random();
            if(tales.Count < 3){
                List<string> Stories = null;
                if(amount>0){
                    Stories = new List<string>(new string[] {
                        "Once, I had a dream about a ==sample== and then the next day I saw a rainbow.",
                        "I really love ==typeverb== my enemies.",
                        "I think I could probably make a ==sample==.",
                        "I just think ==type==s are kind of neat.",
                        "You look like the kind of person that might carry a ==type==.",
                        "My friend used to carry a ==type==."
                    });
                }else{
                    Stories = new List<string>(new string[] {
                        "Once, I had a dream about a ==sample== and then the next day I got hit with a rock.",
                        "I worry about people attacking me with a ==type==.",
                        "A "+GameObjectFactory.Factory.CreateSampleObject(EncountersAPI.GetARandomDescendentOf("Creature")).ShortDisplayName+" once attacked me with a ==type==",
                        "I just don't feel saf around ==type==s.",
                        "You look like the kind of person that might carry a ==type==.",
                        "My greatest enemy used to carry a ==type==."
                    });
                }
                tales.Add(Stories[r.Next(0,Stories.Count-1)].Replace("==type==",presentable[wantedType]).Replace("==typeverb==",verbs[wantedType]).Replace("==sample==",ExampleName));
            }
              


            return tales[r.Next(0,tales.Count-1)];


        }



    }
}