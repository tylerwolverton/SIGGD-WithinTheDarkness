using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Engine.Tiles;
using Engine.Textures;

namespace Engine.Acting.Attacks
{
    public class SlashAttack : Attack
    {

        private readonly Actor actor;

        //Attack Specific stuff
        private float damage;
        private int radius;
        private float coneWidth;
        private int thisImpulse;
        private int attackImpulse;
        
        //Direction Specific stuff
        private Vector2 direction;
        public bool active = false;
        private bool loop = false;

        private Animation.Action endAct;

        public SlashAttack(Actor actor, float damage, int radius, float coneWidth, int thisImpulse, int attackImpulse, bool loop)
            : base(actor)
        {

            this.actor = actor;
            this.damage = damage;
            this.radius = radius;
            this.coneWidth = coneWidth;
            this.thisImpulse = thisImpulse;
            this.attackImpulse = attackImpulse;
            this.loop = loop;
        }

        public void addEndAct(Animation.Action endAct)
        {
            this.endAct = endAct;
        }

        public void setAnimation(Vector2 direction, Animation currentAnim, int frame)
        {
            this.direction = direction;

            currentAnim.clearFrameActs();

            currentAnim.addFrameAct(frame, delegate { attack(); } );

            if (!loop)
            {
                currentAnim.addEndAct(delegate { active = false; });
                if (endAct != null){
                    currentAnim.addEndAct(endAct);
                }
            }
           
            currentAnim.loop = loop;
            
            actor.anim = currentAnim;

            active = true;
        }

        public void setAnimation(Vector2 direction, Animation currentAnim, Predicate<int> frames)
        {

            this.direction = direction;

            currentAnim.clearFrameActs();
            currentAnim.addPredAct(frames, delegate { attack(); } );

            if (!loop)
            {
                currentAnim.addEndAct(delegate { active = false; });
                if (endAct != null)
                {
                    currentAnim.addEndAct(endAct);
                }
            }

            currentAnim.loop = loop;
            
            actor.anim = currentAnim;

            active = true;
        }

        public override void run()
        {
            if (active)
            {
                //Actor Impulse
                Vector2 impulse = new Vector2();
                impulse = direction;
                impulse.Normalize();
                impulse *= thisImpulse;
                actor.addImpulse(impulse);
            }
        }

        public void attack()
        {

            //Determine which actors are hit
            foreach (Actor a in actor.getConeAround(radius, direction, coneWidth, null))
            {

                ILife liveAct = a as ILife;
                if (a.collisionimmunitymask != Actor.ActorCategory.friendly && a.actorcategory != Actor.ActorCategory.nocategory)
                {

                    //Damage
                    if (liveAct != null)
                    {
                        liveAct.life.life -= damage;
                    }

                    //Impulse
                    Vector2 impulse = new Vector2();
                    impulse = (a.position - actor.position)  * 2 / 3;
                    impulse.Normalize();

                    //Attack Impulse
                    impulse *= attackImpulse * a.mass / 10.0f;

                    //Attacker Speed Impulse
                    impulse += actor.velocity / 6;

                    //One side or the other Impulse
                    Vector2 offset = a.position - actor.position;
                    int dir = 0;
                    if (actor.velocity.X * offset.Y - offset.X * actor.velocity.Y > 0)
                        dir = 30;
                    else
                        dir = -30;
                    impulse += Vector2.Transform(impulse, Matrix.CreateRotationZ(MathHelper.ToRadians(dir)));

                    //End frame impulse
                    if (actor.anim.curFrame == actor.anim.frameEnd)
                    {
                        impulse *= 2f;
                    }

                    //Add Impulse!
                    a.addImpulse(impulse * 25);
                }
            }
        }

        public override void saveState(System.IO.BinaryWriter writer)
        {
            base.saveState(writer);
            writer.Write(direction.X);
            writer.Write(direction.Y);
            writer.Write(active);
        }

        public override void loadState(System.IO.BinaryReader reader)
        {
            base.loadState(reader);
            direction = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            active = reader.ReadBoolean();
        }
    }
}