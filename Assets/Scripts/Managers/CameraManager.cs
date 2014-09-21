using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
	
	//Camera boundaries
	public int nBoundary;
	public int sBoundary;
	public int eBoundary;
	public int wBoundary;

	//Camera movement values
	public int zoomSpeed;
	public int zoomMin;
	public int zoomMax;
	public int scrollSpeed;
	public int scrollThreshold;

	public GameObject BackgroundPrefab;

	private GameManager gameManager;
	private Camera gameCamera;
	
	// Use this for initialization
	void Start () {
		gameManager = GetComponent<GameManager> ();
	}

	public void Initialize() {
		gameCamera = GameObject.Find ("Main Camera").camera;
		transform.position -= new Vector3(0.0f, 10.0f, 0.0f);
	}
	
	// Update is called once per frame
	void Update () {
		if ((gameCamera) && (gameManager.GetStage() == GameStage.Playing)) {
			// Initialize Camera translation for this frame
			Vector3 translation = Vector3.zero;
			
			
			// Zoom in or out
			float zoomDelta = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;
			Vector3 zoomTranslation = gameCamera.transform.forward * zoomSpeed * zoomDelta;
			translation += zoomTranslation;
			
			Vector3 EWTranslation = Vector3.zero;
			Vector3 NSTranslation = Vector3.zero;
			// Move Camera if player's mouse cursor reaches screen borders
			if (Input.mousePosition.x < scrollThreshold) {
				EWTranslation = Vector3.right * -scrollSpeed * Time.deltaTime;
			} else if (Input.mousePosition.x >= Screen.width - scrollThreshold) {
				EWTranslation = Vector3.right * scrollSpeed * Time.deltaTime;
			}
			if (Input.mousePosition.y < scrollThreshold) {
				NSTranslation = Vector3.forward * -scrollSpeed * Time.deltaTime;
			} else if (Input.mousePosition.y > Screen.height - scrollThreshold) {
				NSTranslation = Vector3.forward * scrollSpeed * Time.deltaTime;
			}
			translation += NSTranslation += EWTranslation;
			
			
			// Keep camera within boundaries
			//TODO: Update bounds check to account for zoom
			Vector3 desiredPosition = gameCamera.transform.position + translation;
			if (desiredPosition.x < -wBoundary || desiredPosition.x > eBoundary) {
				translation.x = 0;
			}
			if (desiredPosition.z < -sBoundary || desiredPosition.z > nBoundary) {
				translation.z = 0;
			}
			if (desiredPosition.y < zoomMin || desiredPosition.y > zoomMax) {
				translation -= zoomTranslation;
			}
			
			
			// Move camera
			gameCamera.transform.position += translation;
		}
	}

	public void SetInGameBackground() {
		GameObject backObject = (GameObject)GameObject.Instantiate(BackgroundPrefab, new Vector3(0.0f, -15.0f, 0.0f), Quaternion.identity);
		backObject.transform.parent = this.transform;
	}
}