using UnityEngine;
using UnityEditor;

[System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = true, Inherited = true)]
public class JUHeader : PropertyAttribute
{
    public string text;

    public JUHeader(string text)
    {
        this.text = text;
    }
}
[System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = true, Inherited = true)]
public class JUSubHeader : PropertyAttribute
{
    public string text;
    public JUSubHeader(string text)
    {
        this.text = text;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(JUHeader))]
public class JUHeaderDecoratorDrawer : DecoratorDrawer
{
    JUHeader header
    {
        get { return ((JUHeader)attribute); }
    }

    public override float GetHeight()
    {
        return base.GetHeight() + 5;
    }

    public override void OnGUI(Rect position)
    {
        //float lineX = (position.x + (position.width / 2)) - header.lineWidth / 2;
        float lineY = position.y + 0;
        //float lineWidth = header.lineWidth;

        var g = new GUIStyle(EditorStyles.toolbar);
        g.fontStyle = FontStyle.Bold;
        g.alignment = TextAnchor.LowerLeft;
        g.font = JUEditor.CustomEditorStyles.JUEditorFont();

        if (EditorGUIUtility.isProSkin == false)
        {
            g.normal.textColor = Color.magenta;
        }
        else
        {
            g.normal.textColor = new Color(1f, 0.5f, 0.7f);
        }

        //g.normal.textColor = new Color(1f, 0.7f, 0.5f);
        g.fontSize = 16;
        g.richText = true;
        Rect newposition = new Rect(position.x - 17, lineY, position.width + 28, position.height);
        EditorGUI.LabelField(newposition, "  " + header.text, g);
    }
}


[CustomPropertyDrawer(typeof(JUSubHeader))]
public class JUSubHeaderDecoratorDrawer : DecoratorDrawer
{
    JUSubHeader header
    {
        get { return ((JUSubHeader)attribute); }
    }

    public override float GetHeight()
    {
        return base.GetHeight() + 5;
    }
    public override void OnGUI(Rect position)
    {
        //float lineX = (position.x + (position.width / 2)) - header.lineWidth / 2;
        float lineY = position.y + 1;
        //float lineWidth = header.lineWidth;
        var g = new GUIStyle(EditorStyles.boldLabel);
        g.fontStyle = FontStyle.Bold;
        g.font = JUEditor.CustomEditorStyles.JUEditorFont();
        g.alignment = TextAnchor.MiddleLeft;

        if (EditorGUIUtility.isProSkin == false)
        {
            g.normal.textColor = Color.blue;
        }
        else
        {
            g.normal.textColor = new Color(0.4f, 0.7f, 0.9f);
        }

        //g.normal.textColor = new Color(1f, 0.7f, 0.5f);
        g.fontSize = 14;
        g.richText = true;

        Rect newposition = new Rect(position.x - 17, lineY, position.width + 19, position.height);
        EditorGUI.LabelField(newposition, "    " + header.text, g);
    }
}
#endif