using Engine.Components;
using Microsoft.Xna.Framework;
//Unify

namespace Engine
{
    public class Editor : MirrorEngine
    {
        //Option Values
        public bool grid = true;
        public bool solid = true;
        public bool opaque = true;

        //Tools
        public Tool activeTool;
        public PencilTool pencilTool;
        public FlagTool flagTool;
        public FillTool fillTool;
        public ActorTool actorTool;
        public EraserTool eraseTool;
        public OptionTool optionTool;
        public StampTool stampTool;

        //Load & Save & New
        public LoadTool loadTool;
        public SaveTool saveTool;
        public NewTool newTool;

        public bool fullscreen;

        public Editor()
        {

            resourceComponent = new GameResources(this.Content, this);
            inputComponent = new EditorInput(this);
            physicsComponent = new EditorPhysics(this);
            graphicsComponent = new EditorGraphics(this);
            audioComponent = new EditorAudio(this);

            fullscreen = false;
            Tile.noLOS = true;
            
        }

        protected override void Initialize()
        {
            
            base.Initialize();
            OnStart();
            optionTool = new OptionTool(this);
            loadTool = new LoadTool(this);
            saveTool = new SaveTool(this);
            newTool = new NewTool(this);
            pencilTool = new PencilTool(this);
            flagTool = new FlagTool(this);
            fillTool = new FillTool(this);
            actorTool = new ActorTool(this);
            eraseTool = new EraserTool(this);
            stampTool = new StampTool(this);

            EditorInput input = inputComponent as EditorInput;
            
            (input[EditorInput.EditBindings.SCROLLLOCK] as SinglePressBinding).downEvent += flipScroll;
            (input[EditorInput.EditBindings.FULLSCREEN] as SinglePressBinding).downEvent += flipFull;
            (input[EditorInput.EditBindings.ZOOMIN] as SinglePressBinding).downEvent += zoomIn;
            (input[EditorInput.EditBindings.ZOOMOUT] as SinglePressBinding).downEvent += zoomOut;
            (input[EditorInput.EditBindings.QUIT] as SinglePressBinding).downEvent += Exit;

            pencilTool.active = true;
            activeTool = pencilTool; 
            graphicsComponent.guiComponent.current.performLayout();
            graphicsComponent.guiComponent.current.performLayout();

        }
        public void zoomOut()
        {
            graphicsComponent.camera.scale /= 2;
        }
        public void zoomIn()
        {
            graphicsComponent.camera.position = Vector2.Transform(new Vector2(inputComponent.currentMouseState.X, inputComponent.currentMouseState.Y), Matrix.Invert(graphicsComponent.game2screen));
            graphicsComponent.camera.scale *= 2;
        }
        public void flipFull()
        {
            if (!(this.graphicsComponent.guiComponent.focus != null && this.graphicsComponent.guiComponent.focus is TextArea) || this.graphicsComponent.guiComponent.focus == null)
            {
                graphicsComponent.setFullscreen(!this.fullscreen);
                fullscreen = !fullscreen;
                (graphicsComponent.guiComponent as EditorGUI).egl = new GUI.EditorGUILayout(graphicsComponent.guiComponent as EditorGUI);
                (graphicsComponent.guiComponent as EditorGUI).current = (graphicsComponent.guiComponent as EditorGUI).egl;
                (graphicsComponent.guiComponent as EditorGUI).egl.Initialize();
                (graphicsComponent.guiComponent as EditorGUI).egl.pack();
            }
        }

        public void flipScroll()
        {
            if (!(this.graphicsComponent.guiComponent.focus != null && this.graphicsComponent.guiComponent.focus is TextArea) || this.graphicsComponent.guiComponent.focus == null)
                
            (world as EditorWorld).scrollLock = !(world as EditorWorld).scrollLock;
        }
        public void OnStart(string mapname = "010_Play Campaign")
        {

            if (world != null)
            {
                world.UnloadContent();
            }

            //world = new EditorWorld(this, "RealMap1.map");
            //world.Initialize();
            //world.LoadContent();
            setWorld(@"Content\Maps\"+mapname+".map");            
        }
        
        public override World loadWorld(string str)
        {
            
            World w = new EditorWorld(this, str);
            w.Initialize();
            w.LoadContent();
            return w;
        }
    }
}