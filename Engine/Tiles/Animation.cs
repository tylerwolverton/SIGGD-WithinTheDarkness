using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Engine.Tiles
{
    public class Animation
    {
        // Action types
        public delegate void Action(int frame);
        
        // An action tied to a specific frame
        private class FrameAct
        {
            public readonly int frame;
            public event Action action;

            public FrameAct(int frame) {
                this.frame = frame;
            }

            public void run(int frame) {
                if (this.frame == frame) {
                    action(frame);
                }
            }
        }

        // An action with a predicate
        private class PredAct
        {            
            public readonly Predicate<int> pred;
            public event Action action;

            public PredAct(Predicate<int> pred) {
                this.pred = pred;
            }

            public void run(int frame) {
                if (pred(frame)) {
                    action(frame);
                }
            }
        }

        public int curFrame { get; set; }
        public bool isNewFrame { get; set; }
        public bool finished { get; private set; }

        public int frameBegin { get; private set; }
        public int frameEnd { get; private set; }
        public float ticksPerFrame { get; set; }
        private float ticksSinceLast = 0;

        public bool loop { get; set; }
        public int xoffset { get; private set; }
        public int yoffset { get; private set; }

        // Actions tied to events:
        private List<FrameAct> frameActs;
        private List<PredAct> predActs;
        private int curFrameAct;  // The index of the most recently triggered frame action.

        public Animation(int frameBegin, int frameEnd, float ticksPerFrame, bool loop, int xoffset = 0, int yoffset = 0)
        {

            this.frameBegin = frameBegin;
            this.frameEnd = frameEnd;
            this.ticksPerFrame = ticksPerFrame;
            this.loop = loop;
            this.xoffset = xoffset;
            this.yoffset = yoffset;

            reset();
        }

        public void run()
        {
            if (finished)
                return;

            if (ticksSinceLast >= ticksPerFrame)
            {

                if (curFrame >= frameEnd)
                {
                    if (loop)
                    {
                        curFrame = frameBegin;
                        curFrameAct = 0;
                        isNewFrame = true;
                    }
                    else
                    {
                        finished = true;
                    }
                }
                else
                {
                    curFrame++;
                    isNewFrame = true;
                }

                ticksSinceLast -= ticksPerFrame;
            }

            if (isNewFrame)
            {
                // Consider Frame Actions
                if (frameActs != null) {
                    int len = frameActs.Count;
                    while (curFrameAct < len && frameActs[curFrameAct].frame < curFrame) {  // Seek to find the next frame act
                        curFrameAct++;
                    }

                    while (curFrameAct < len && frameActs[curFrameAct].frame == curFrame)
                    {
                        frameActs[curFrameAct].run(curFrame); // Try it
                        curFrameAct++;
                    }

                    if (curFrameAct >= len)
                        curFrameAct = 0;
                }

                // Consider Predicate Actions
                if (predActs != null) {
                    foreach (PredAct pAct in predActs) {
                        pAct.run(curFrame);  // Try all predicate actions
                    }
                }

                isNewFrame = false;
            }

            ticksSinceLast++;
        }

        public void reset()
        {
            this.curFrameAct = 0;
            curFrame = frameBegin;
            ticksSinceLast = 0;
            isNewFrame = true;
            finished = false;
        }

        public void clearFrameActs()
        {

            if(frameActs != null)
                frameActs.Clear();
            if(predActs != null)
                predActs.Clear();
        }

        public void addFrameAct(int frame, Action action) {
            FrameAct fAct = null;

            if (frame < frameBegin || frame > frameEnd)
                throw new Exception("addFrameAct: Frame out of bounds.");

            if (action == null)
                throw new Exception("addFrameAct: Null action");

            if (frameActs == null) {
                frameActs = new List<FrameAct>();
            } else {
                fAct = frameActs.Find((a) => (a.frame == frame));                
            }

            if (fAct == null) {
                fAct = new FrameAct(frame);
                fAct.action += action;
                frameActs.Add(fAct);
            }
            else {
                fAct.action += action;
            }
        }

        // Convenience functions

        public void addBeginAct(Action action)
        {
            addFrameAct(frameBegin, action);
        }

        public void addEndAct(Action action)
        {
            addFrameAct(frameEnd, action);
        }

        public void addPredAct(Predicate<int> pred, Action action) {
            PredAct pAct = null;

            if (pred == null)
                throw new Exception("addPredAct: Null predicate");

            if (action == null)
                throw new Exception("addPredAct: Null action");

            if (predActs == null) {
                predActs = new List<PredAct>();
            } else {
                pAct = predActs.Find((a) => (a.pred.Equals(pred)));
            }

            if (pAct == null) {
                pAct = new PredAct(pred);
                pAct.action += action;
                predActs.Add(pAct);
            } else {
                pAct.action += action;
            }
        }
    }
}
