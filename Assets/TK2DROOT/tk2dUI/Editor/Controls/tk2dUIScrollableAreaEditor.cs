using UnityEngine;
using UnityEditor;
using System.Collections;

[CanEditMultipleObjects]
[CustomEditor(typeof(tk2dUIScrollableArea))]
public class tk2dUIScrollableAreaEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeInspector();
        base.OnInspectorGUI();

        GUILayout.Label("Tools", EditorStyles.boldLabel);
        if (GUILayout.Button("Calculate content length")) {
            tk2dUIScrollableArea scrollableArea = (tk2dUIScrollableArea)target;

            Undo.RegisterUndo(scrollableArea, "Content length changed");
            Bounds b = tk2dUIItemBoundsHelper.GetRendererBoundsInChildren( scrollableArea.contentContainer.transform, scrollableArea.contentContainer.transform );
            float contentSize = (scrollableArea.scrollAxes == tk2dUIScrollableArea.Axes.XAxis) ? b.size.x : b.size.y;
            scrollableArea.ContentLength = contentSize * 1.02f; // 5% more
            EditorUtility.SetDirty(scrollableArea);
        }
    }

    public void OnSceneGUI()
    {
        bool wasChange=false;
        tk2dUIScrollableArea scrollableArea = (tk2dUIScrollableArea)target;
        bool isYAxis = scrollableArea.scrollAxes== tk2dUIScrollableArea.Axes.YAxis;

        // Get rescaled transforms
        Matrix4x4 m = scrollableArea.transform.localToWorldMatrix;
        Vector3 up = m.MultiplyVector(Vector3.up);
        Vector3 right = m.MultiplyVector(Vector3.right);

        float newVisibleAreaLength = tk2dUIControlsHelperEditor.DrawLengthHandles("Visible Area Length", scrollableArea.VisibleAreaLength,scrollableArea.transform.position, isYAxis? -up:right, Color.red,isYAxis?.2f:-.2f, 0, .05f);
        if (newVisibleAreaLength != scrollableArea.VisibleAreaLength)
        {
            Undo.RegisterUndo(scrollableArea, "Visible area changed");
            scrollableArea.VisibleAreaLength = newVisibleAreaLength;
            wasChange = true;
        }

        float newContentLength = tk2dUIControlsHelperEditor.DrawLengthHandles("Content Length", scrollableArea.ContentLength, scrollableArea.transform.position, isYAxis ? -up : right, Color.blue, isYAxis ? .2f : -.2f, isYAxis?.4f:-.4f, .1f);
        if (newContentLength != scrollableArea.ContentLength)
        {
            Undo.RegisterUndo(scrollableArea, "Content length changed");
            scrollableArea.ContentLength = newContentLength;
            wasChange = true;
        }

        if (wasChange)
        {
            EditorUtility.SetDirty(scrollableArea);
        }
    }

}
