using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class StartThankyouMessage
{
    static StartThankyouMessage()
    {
        if (System.IO.File.Exists(Application.dataPath + "/JU Voxel/Editor/Resources/DontShowThankYouMessage.jutps") == false)
        {
            ThankYouWindow.ShowWindow();
            System.IO.File.Create(Application.dataPath + "/JU Voxel/Editor/Resources/DontShowThankYouMessage.jutps");
        }
    }
}

public class ThankYouWindow : EditorWindow
{
    [MenuItem("JU Voxel/Thank You!")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ThankYouWindow));
        GetWindow(typeof(ThankYouWindow)).titleContent.text = "Thanks!";
        const int width = 272;
        const int height = 570;

        var x = (Screen.currentResolution.width - width) / 2;
        var y = (Screen.currentResolution.height - height) / 2;

        GetWindow<ThankYouWindow>().position = new Rect(x, y, width, height);
    }
    [MenuItem("JU Voxel/Developer AssetStore Page")]
    public static void OpenAssetStorePage()
    {
        Application.OpenURL("https://assetstore.unity.com/publishers/50337");
    }
    [MenuItem("JU Voxel/Documentation")]
    public static void OpenDocumentation()
    {
        Application.OpenURL(Application.dataPath + "/JU Voxel/JU Voxel Documentation.pdf");
    }
    [MenuItem("JU Voxel/Support Email")]
    public static void OpenSupportEmail()
    {
        Application.OpenURL("mailto:julhieciogames1@gmail.com");
    }

    public Texture2D Banner;
    private void OnGUI()
    {
        if (Banner == null)
        {
            Banner = JUEditor.CustomEditorUtilities.GetImage("Assets/JU Voxel/Textures/Icons/JU VOXEL Banner.png");
        }
        if (Banner != null)
        {
            JUEditor.CustomEditorUtilities.RenderImageWithResize(Banner, new Vector2(265, 70));
        }

        var style = new GUIStyle(EditorStyles.label);
        style.font = JUEditor.CustomEditorStyles.JUEditorFont();
        style.fontSize = 16;
        style.wordWrap = true;

        GUILayout.Label("Thanks!", JUEditor.CustomEditorStyles.Header());
        GUILayout.Label("Thank you very much for the purchase, you don't know how much is helping me!" +
            "\r\n\n I hope you enjoy my work, I am always updating and bringing new things and improvements." +
            "\r\n\n  if you have any suggestions or need help with something you can send me an email:", style);


        if (GUILayout.Button(" ✎ Open Support Email", JUEditor.CustomEditorStyles.MiniToolbarButton()))
        {
            OpenSupportEmail();
        }

        GUILayout.Space(15);

        GUILayout.Label("If you are interested in seeing my other Assets:", style);
        if (GUILayout.Button(" Assetstore Page", JUEditor.CustomEditorStyles.MiniToolbarButton()))
        {
            OpenAssetStorePage();
        }

        GUILayout.Space(15);


        GUILayout.Label("How to Start ?", JUEditor.CustomEditorStyles.Header());
        if (GUILayout.Button(" ▓ Open Documentation", JUEditor.CustomEditorStyles.MiniToolbarButton()))
        {
            OpenDocumentation();
        }

        GUILayout.Space(15);

        GUILayout.Label("My Youtube Channel:", JUEditor.CustomEditorStyles.Header());
        if (GUILayout.Button(" Julhiecio GameDev", JUEditor.CustomEditorStyles.MiniToolbarButton()))
        {
            Application.OpenURL("https://www.youtube.com/c/JulhiecioGameDev");
        }
    }
}
