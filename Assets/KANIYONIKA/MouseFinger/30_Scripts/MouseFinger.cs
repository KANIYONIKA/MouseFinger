using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;

namespace MouseFinger
{

#if UNITY_EDITOR
  [CustomEditor(typeof(MouseFinger))]
  public class MouseFingerCustomWindow : Editor
  {
    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();

      EditorGUILayout.BeginVertical(GUI.skin.box);
      {
        EditorGUILayout.HelpBox(

        "◆◆ Parameter Descriptions ◆◆\n"
        + "* IsMouseCursorVisible: If set True, the mouse cursor will be visible.\n"
        + "* KeyCodeToAppear: The MouseFinger will appear when the key set here is pressed.\n"
        + "* TimeToAppear: The time it takes to display the MouseFinger(in seconds)\n"
        + "* TimeToDisappear: The time it takes to hide the MouseFinger(in seconds)\n"
        + "* FingerDownImage: Image when clicked\n"
        + "* FingerUpImage: Image when not clicked.\n"
        + "\n"
        + "◆◆ パラメーターの説明 ◆◆\n"
        + "* IsMouseCursorVisible: Trueにするとマウスカーソルが表示されます\n"
        + "* KeyCodeToAppear: ここで設定したキーを押すとMouseFingerが表示されます\n"
        + "* TimeToAppear: MouseFingerを表示するまでにかかる時間（秒）\n"
        + "* TimeToDisappear: MouseFingerを非表示するまでにかかる時間（秒）\n"
        + "* FingerDownImage: クリックした時の画像\n"
        + "* FingerUpImage: クリックしていない時の画像\n"
        + "\n"
        + "\n"
        + "◆◆ If you want to change the image ◆◆\n"
        + "1- Replace the FingerDown and FingerUp images under Canvas_MouseFinger.\n"
        + "2- Adjust the Width, Height, and Scale of the RectTransform of FingerDown and FingerUp.\n"
        + "3- Try playing and check if the click points are appropriate. If not, adjust the Pivot of the RectTransform of FingerDown and FingerUp.\n"
        + "* Note: For subsequent size(Scale) adjustment, it is easier to adjust the ScaleFactor of Canvas_MouseFinger's CanvasScaler.\n"
        + "\n"
        + "◆◆ 画像を変更したい場合 ◆◆\n"
        + "1- Canvas_MouseFinger配下にある FingerDown と FingerUp の Image を差し替えてください。\n"
        + "2- FingerDownとFingerUp の RectTransform の Width, Height, Scale を調整してください。\n"
        + "3- 試しにPlayして、クリックポイントが適切か確認してください。ズレていたら FingerDown と FingerUp の RectTransform のPivot を調整してください。\n"
        + "*備考: 以後のサイズ（Scale）調整については Canvas_MouseFinger の CanvasScaler の ScaleFactor で調整すると楽です。"
        , MessageType.Info);
      }
      EditorGUILayout.EndVertical();
    }
  }
#endif


  public class MouseFinger : MonoBehaviour
  {
    public bool isMouseCursorVisible = true;
    //画像
    public KeyCode keyCodeToAppear = KeyCode.Q;
    public float timeToAppear = 0.15f;
    public float timeToDisappear = 0.15f;
    public Image fingerDownImage;
    public Image fingerUpImage;


    //Canvasの変数
    private Canvas canvas;
    //キャンバス内のレクトトランスフォーム
    private RectTransform canvasRect;
    //マウスの位置の最終的な格納先
    private Vector2 MousePos;
    //指を表示するか
    private bool isImageShow = false;
    //指イメージのスケール
    private Vector3 fingerUpImageScale;

    void Start()
    {
      //Canvasを取得
      canvas = gameObject.GetComponent<Canvas>();
      //canvas内にあるRectTransformをcanvasRectに入れる
      canvasRect = canvas.GetComponent<RectTransform>();
      //指イメージのスケール
      fingerUpImageScale = fingerUpImage.rectTransform.localScale;

      if (isImageShow)
      {
        fingerDownImage.enabled = true;
        fingerUpImage.enabled = true;
      }
      if (!isImageShow)
      {
        fingerDownImage.enabled = false;
        fingerUpImage.enabled = false;
      }
    }


    private void Update()
    {
      //マウスカーソル（ポインター）
      if (Cursor.visible != isMouseCursorVisible)
      {
        Cursor.visible = isMouseCursorVisible;
      }

      if (isImageShow)
      {
        if (Input.GetMouseButtonDown(0))
        {
          fingerDownImage.enabled = true;
          fingerUpImage.enabled = false;
        }

        if (Input.GetMouseButtonUp(0))
        {
          fingerDownImage.enabled = false;
          fingerUpImage.enabled = true;
        }
      }

      if (Input.GetKeyDown(keyCodeToAppear))
      {
        isImageShow = !isImageShow;

        if (isImageShow)
        {
          StartCoroutine(ScaleOverTime(timeToAppear));
        }
        if (!isImageShow)
        {
          StartCoroutine(ScaleOverTime(timeToDisappear, Act.Disappear));
        }
      }

      RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Input.mousePosition, canvas.worldCamera, out MousePos);
      fingerUpImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(MousePos.x, MousePos.y);
      fingerDownImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(MousePos.x, MousePos.y);
    }


    enum Act { Appear, Disappear }
    IEnumerator ScaleOverTime(float time, Act act = Act.Appear)
    {
      float currentTime = 0.0f;

      if (act == Act.Appear)
      {
        fingerUpImage.rectTransform.localScale = Vector3.zero;
        fingerUpImage.enabled = true;
      }

      do
      {
        Vector3 calculatedScale;
        if (act == Act.Appear)
          calculatedScale = Vector3.Lerp(Vector3.zero, fingerUpImageScale, currentTime / time);
        else
          calculatedScale = Vector3.Lerp(fingerUpImageScale, Vector3.zero, currentTime / time);

        fingerUpImage.rectTransform.localScale = calculatedScale;
        currentTime += Time.deltaTime;
        yield return null;
      } while (currentTime <= time);

      if (act == Act.Disappear)
      {
        fingerUpImage.enabled = false;
      }
    }
  }

}