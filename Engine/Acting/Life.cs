using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Engine.Tiles;

namespace Engine.Acting
{

    // Liferface
    public interface ILife
    {
        Life life { get; }
    }

    public class Life
    {
        Actor a;

        // TYPES
        public delegate void DeathEventHandler();
        public delegate void LifeChangeHandler(float oldLife);

        // MEMBERS
        public bool isGod;

        public float maxlife;
        public float life
        {
            get { return _life; }

            set
            {
                if (isGod && value < _life)
                    return;

                float oldLife = _life;

                _life = value;

                if (_life < 0)
                    _life = 0;
                if (_life > maxlife)
                    _life = maxlife;

                if (_life != oldLife)
                {
                    if (lifeChangingEvent != null)
                    {
                        lifeChangingEvent(oldLife);
                    }

                    if (dead)
                    {
                        a.active = false;
                        a.actorcategory = Actor.ActorCategory.nocategory;

                        if (deathEvent != null)
                        {
                            deathEvent();
                        }
                    }
                }
            }
        }

        public event DeathEventHandler deathEvent;
        public event LifeChangeHandler lifeChangingEvent;

        public bool dead
        {
            get
            {
                return life <= 0f;
            }
        }

        static public void collisionDamage(Actor a, Actor target, int damage, float impulse = 250)
        {
            ILife liveA = target as ILife;
            ILife liveB = a as ILife;

            if (liveA == null || liveA.life.dead || liveB.life.dead)
                return;

            if (target != null && !liveA.life.isGod && ((a.damageimmunitymask & target.actorcategory) == 0))
            {
                Vector2 dir = new Vector2(target.position.X - a.position.X, target.position.Y - a.position.Y);
                dir.Normalize();
                dir *= impulse;
                target.addImpulse(dir);
                liveA.life.life -= damage;
            }
        }

        static public void basicDamage(Actor a, Actor target, int damage, float impulse = 250)
        {
            ILife liveA = target as ILife;
            ILife liveB = a as ILife;

            if (liveA == null || liveA.life.dead || liveB.life.dead)
                return;

            if (target != null && !liveA.life.isGod && ((a.damageimmunitymask & target.actorcategory) == 0))
            {
                liveA.life.life -= damage;
            }
        }


        // PRIVATE MEMBERS
        private float _life;

        // Constructor
        public Life(Actor a, int maxlife)
        {
            this.a = a;
            this.life = this.maxlife = maxlife;
        }

        //State saving
        public void saveState(System.IO.BinaryWriter writer)
        {
            writer.Write(_life);
            writer.Write(maxlife);
            writer.Write(isGod);
        }

        public void loadState(System.IO.BinaryReader reader)
        {
            _life = reader.ReadSingle();
            maxlife = reader.ReadSingle();
            isGod = reader.ReadBoolean();
        }
    }
}
