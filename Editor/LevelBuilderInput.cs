using UnityEngine;

public class LevelBuilderInput
{
    private readonly LevelBuilderWindow w;

    public LevelBuilderInput(LevelBuilderWindow window)
    {
        w = window;
    }

    public void Handle(Event e)
    {
        if (e.type != EventType.KeyDown)
            return;

        if (e.keyCode == KeyCode.R)
        {
            w.Rotation += 90f;

            if (w.Rotation >= 360f)
                w.Rotation = 0f;

            w.Repaint();
            e.Use();
        }

        if (e.keyCode == KeyCode.E)
        {
            w.ToggleErase();
            w.Repaint();
            e.Use();
        }
    }
}