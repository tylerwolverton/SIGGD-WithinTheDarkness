using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.GUI
{
    public class EditorThumbnailContainer : ThumbnailContainer
    {

        public EditorThumbnailContainer(GUIComponent theInterface, int tilesOrActors = 0)
            : base (theInterface, tilesOrActors)
        {

        }

        public override void loadTiles()
        {
            thumbPics = guiComponent.tileEngine.resourceComponent.getTextureSet("Tiles");

        }

        public override void loadActors()
        {
            thumbPics = guiComponent.tileEngine.resourceComponent.getTextureSet("028_ActorIcons");
        }
    }
}
