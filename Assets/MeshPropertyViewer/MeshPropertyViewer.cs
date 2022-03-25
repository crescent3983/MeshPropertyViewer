using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class MeshPropertyViewer : MonoBehaviour {

	#region Defines
	private const string _SHADER_NAME = "Hidden/Debug/MeshPropertyViewer";
	private const string _LINE_WIDTH_PROPERTY = "_LineWidth";
	private const string _NORMAL_COLOR_PROPERTY = "_NormalColor";
	private const string _TANGENT_COLOR_PROPERTY = "_TangentColor";

	public enum OverlayContent {
		None,
		Color,
		Normal,
		Tangent,
		Texture,
		UV0,
		UV1,
		UV2,
		UV3,
		UV4,
		UV5,
		UV6,
		UV7,
	}

	public enum TextureUV {
		UV0,
		UV1,
		UV2,
		UV3,
		UV4,
		UV5,
		UV6,
		UV7,
	}

	public enum Channel {
		R,
		G,
		B,
		A,
		RGB,
		RGBA
	}

	private static readonly Dictionary<OverlayContent, string> shaderOverlayKeywords = new Dictionary<OverlayContent, string> {
		[OverlayContent.Normal] = "_VERTEX_NORMAL",
		[OverlayContent.Tangent] = "_VERTEX_TANGENT",
		[OverlayContent.Texture] = "_VERTEX_TEXTURE",
		[OverlayContent.UV0] = "_VERTEX_UV0",
		[OverlayContent.UV1] = "_VERTEX_UV1",
		[OverlayContent.UV2] = "_VERTEX_UV2",
		[OverlayContent.UV3] = "_VERTEX_UV3",
		[OverlayContent.UV4] = "_VERTEX_UV4",
		[OverlayContent.UV5] = "_VERTEX_UV5",
		[OverlayContent.UV6] = "_VERTEX_UV6",
		[OverlayContent.UV7] = "_VERTEX_UV7",
	};

	private static readonly Dictionary<TextureUV, string> shaderTextureUVKeywords = new Dictionary<TextureUV, string> {
		[TextureUV.UV1] = "_TEXTURE_UV1",
		[TextureUV.UV2] = "_TEXTURE_UV2",
		[TextureUV.UV3] = "_TEXTURE_UV3",
		[TextureUV.UV4] = "_TEXTURE_UV4",
		[TextureUV.UV5] = "_TEXTURE_UV5",
		[TextureUV.UV6] = "_TEXTURE_UV6",
		[TextureUV.UV7] = "_TEXTURE_UV7",
	};

	private static readonly Dictionary<Channel, string> shaderChannelKeywords = new Dictionary<Channel, string> {
		[Channel.R] = "_COLOR_R",
		[Channel.G] = "_COLOR_G",
		[Channel.B] = "_COLOR_B",
		[Channel.A] = "_COLOR_A",
		[Channel.RGB] = "_COLOR_RGB",
	};
	#endregion

	private Material _mat;
	private CommandBuffer _cmdBuffer;
	[SerializeField] private OverlayContent _overlay = OverlayContent.None;
	[SerializeField] private TextureUV _textureUV = TextureUV.UV0;
	[SerializeField] private Channel _channel = Channel.RGB;
	[SerializeField] private float _lineWidth = 1.0f;
	[SerializeField] private bool _normalOn = false;
	[SerializeField] private Color _normalColor = Color.yellow;
	[SerializeField] private bool _tangentOn = false;
	[SerializeField] private Color _tangentColor = Color.magenta;

	#region Properties
	public Material mat {
		get {
			if (this._mat == null) {
				this._mat = new Material(Shader.Find(_SHADER_NAME));
				if (shaderOverlayKeywords.ContainsKey(this._overlay)) {
					this.mat.EnableKeyword(shaderOverlayKeywords[this._overlay]);
				}
				if (shaderTextureUVKeywords.ContainsKey(this._textureUV)) {
					this.mat.EnableKeyword(shaderTextureUVKeywords[this._textureUV]);
				}
				if (shaderChannelKeywords.ContainsKey(this._channel)) {
					this.mat.EnableKeyword(shaderChannelKeywords[this._channel]);
				}
				this.mat.SetFloat(_LINE_WIDTH_PROPERTY, this._lineWidth);
				this.mat.SetColor(_NORMAL_COLOR_PROPERTY, this._normalColor);
				this.mat.SetColor(_TANGENT_COLOR_PROPERTY, this._tangentColor);
			}
			return this._mat;
		}
	}

	public OverlayContent overlay {
		get {
			return this._overlay;
		}
		set {
			if(this._overlay == value) {
				return;
			}
			this._overlay = value;
			foreach(var item in shaderOverlayKeywords) {
				if(item.Key == this._overlay) {
					this.mat.EnableKeyword(item.Value);
				}
				else {
					this.mat.DisableKeyword(item.Value);
				}
			}
			this.Repaint();
		}
	}
	public TextureUV textureUV {
		get {
			return this._textureUV;
		}
		set {
			if (this._textureUV == value) {
				return;
			}
			this._textureUV = value;
			foreach (var item in shaderTextureUVKeywords) {
				if (item.Key == this._textureUV) {
					this.mat.EnableKeyword(item.Value);
				}
				else {
					this.mat.DisableKeyword(item.Value);
				}
			}
			this.Repaint();
		}
	}
	public Channel channel {
		get {
			return this._channel;
		}
		set {
			if (this._channel == value) {
				return;
			}
			this._channel = value;
			foreach (var item in shaderChannelKeywords) {
				if (item.Key == this._channel) {
					this.mat.EnableKeyword(item.Value);
				}
				else {
					this.mat.DisableKeyword(item.Value);
				}
			}
			this.Repaint();
		}
	}
	public string textureName = "_MainTex";

	public float lineWidth {
		get {
			return this._lineWidth;
		}
		set {
			if (this._lineWidth == value) {
				return;
			}
			this._lineWidth = value;
			this.mat.SetFloat(_LINE_WIDTH_PROPERTY, this._lineWidth);
			this.Repaint();
		}
	}

	public bool normalOn {
		get {
			return this._normalOn;
		}
		set {
			if (this._normalOn == value) {
				return;
			}
			this._normalOn = value;
			this.Repaint();
		}
	}

	public Color normalColor {
		get {
			return this._normalColor;
		}
		set {
			if (this._normalColor == value) {
				return;
			}
			this._normalColor = value;
			this.mat.SetColor(_NORMAL_COLOR_PROPERTY, this._normalColor);
			this.Repaint();
		}
	}

	public bool tangentOn {
		get {
			return this._tangentOn;
		}
		set {
			if (this._tangentOn == value) {
				return;
			}
			this._tangentOn = value;
			this.Repaint();
		}
	}

	public Color tangentColor {
		get {
			return this._tangentColor;
		}
		set {
			if (this._tangentColor == value) {
				return;
			}
			this._tangentColor = value;
			this.mat.SetColor(_TANGENT_COLOR_PROPERTY, this._tangentColor);
			this.Repaint();
		}
	}
	#endregion

	public void Repaint() {
#if UNITY_EDITOR
		UnityEditor.SceneView.RepaintAll();
#endif
	}

	void OnRenderObject() {
#if UNITY_EDITOR
		if (UnityEditor.Selection.gameObjects.Contains(this.gameObject)) {
			this.DrawContent();
		}
#endif
	}

	private void DrawContent() {
		if (this.overlay == OverlayContent.None && !this._normalOn && !this.tangentOn) return;

		if (this._cmdBuffer == null) {
			this._cmdBuffer = new CommandBuffer();
		}
		this._cmdBuffer.Clear();

		Renderer[] renderers = this.GetComponentsInChildren<Renderer>();
		foreach(Renderer renderer in renderers) {
			int count = 0;
			if (renderer is MeshRenderer) {
				count = renderer.gameObject.GetComponent<MeshFilter>().sharedMesh.subMeshCount;
			}
			else if (renderer is SkinnedMeshRenderer) {
				count = ((SkinnedMeshRenderer)renderer).sharedMesh.subMeshCount;
			}
			for(int i = 0; i < count; i++) {
				if(this.overlay == OverlayContent.Texture) {
					Texture tex = null;
					Vector2 offset = Vector2.zero;
					Vector2 scale = Vector2.one;
					Material mat = new Material(this.mat);
					Material srcMat = renderer.sharedMaterials[i];
					if (srcMat && srcMat.HasProperty(this.textureName)) {
						tex = srcMat.GetTexture(this.textureName);
						offset = srcMat.GetTextureOffset(this.textureName);
						scale = srcMat.GetTextureScale(this.textureName);
					}
					mat.mainTexture = tex;
					mat.mainTextureOffset = offset;
					mat.mainTextureScale = scale;
					this._cmdBuffer.DrawRenderer(renderer, mat, i, 0);
				}
				else if(this.overlay != OverlayContent.None) {
					this._cmdBuffer.DrawRenderer(renderer, this.mat, i, 0);
				}

				if(this._normalOn) this._cmdBuffer.DrawRenderer(renderer, this.mat, i, 1);
				if(this._tangentOn) this._cmdBuffer.DrawRenderer(renderer, this.mat, i, 2);
			}
		}
		Graphics.ExecuteCommandBuffer(this._cmdBuffer);
	}
}
