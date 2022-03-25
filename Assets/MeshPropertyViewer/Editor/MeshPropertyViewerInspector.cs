using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(MeshPropertyViewer))]
public class MeshPropertyViewerInspector : Editor {
	private MeshPropertyViewer _mTarget;

	void OnEnable() {
		this._mTarget = this.target as MeshPropertyViewer;
	}

	public override void OnInspectorGUI() {

		//============= Overlay =============
		EditorGUILayout.BeginVertical(EditorStyles.helpBox);
		EditorGUILayout.LabelField("Overlay Color", EditorStyles.toolbarButton);
		EditorGUILayout.Space();

		this._mTarget.overlay = (MeshPropertyViewer.OverlayContent)EditorGUILayout.EnumPopup("Property", this._mTarget.overlay);
		if (this._mTarget.overlay == MeshPropertyViewer.OverlayContent.Texture) {
			this._mTarget.textureUV = (MeshPropertyViewer.TextureUV)EditorGUILayout.EnumPopup("Texture UV", this._mTarget.textureUV);
			this._mTarget.textureName = EditorGUILayout.TextField("Texture Name", this._mTarget.textureName);
		}
		this._mTarget.channel = (MeshPropertyViewer.Channel)EditorGUILayout.EnumPopup("Channel", this._mTarget.channel);

		EditorGUILayout.Space();
		EditorGUILayout.EndVertical();

		//============= Direction =============
		EditorGUILayout.BeginVertical(EditorStyles.helpBox);
		EditorGUILayout.LabelField("Direction", EditorStyles.toolbarButton);
		EditorGUILayout.Space();

		this._mTarget.lineWidth = EditorGUILayout.Slider("Width", this._mTarget.lineWidth, 0.1f, 5.0f);

		EditorGUILayout.BeginHorizontal();
		this._mTarget.normalOn = EditorGUILayout.Toggle("Normal", this._mTarget.normalOn);
		this._mTarget.normalColor = EditorGUILayout.ColorField(this._mTarget.normalColor);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		this._mTarget.tangentOn = EditorGUILayout.Toggle("Tangent", this._mTarget.tangentOn);
		this._mTarget.tangentColor = EditorGUILayout.ColorField(this._mTarget.tangentColor);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();
		EditorGUILayout.EndVertical();

		// Handle multi objects
		foreach (var obj in targets) {
			if (obj == this._mTarget) continue;
			MeshPropertyViewer other = obj as MeshPropertyViewer;
			other.overlay = this._mTarget.overlay;
			other.textureUV = this._mTarget.textureUV;
			other.textureName = this._mTarget.textureName;
			other.channel = this._mTarget.channel;
			other.lineWidth = this._mTarget.lineWidth;
			other.normalOn = this._mTarget.normalOn;
			other.normalColor = this._mTarget.normalColor;
			other.tangentOn = this._mTarget.tangentOn;
			other.tangentColor = this._mTarget.tangentColor;
		}
	}
}
