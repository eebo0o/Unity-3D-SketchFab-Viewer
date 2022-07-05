using System.Collections;
using UnityEngine;

enum ViewDirection
{
    Front,
    Back,
    Top,
    Down,
    Left,
    Right
}
public class OrbitMode : MonoBehaviour
{
    #region Singlton
    public static OrbitMode Instance; 
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    #endregion

    #region Attributes

        #region Publics
            public Camera OrbiteCamera;
            public Transform PlayerCamera;
            public GameObject viewsDropDown;
            
            [Header("Rotation Settings")] 
            public float rotationSpeed = 10f;
            public float maxYRotationDegree = 89f;
           
            [Header("Zoom Settings")]
            public float zoomSpeed= 10f;
            [Header("Panning Settings")] 
            public float panningSpeed = 10;
        #endregion

        #region Privates
            private Vector2 currentRotation;
            float m_previousX = 0, m_previousY = 0;
            private bool isCameraPivot = false;
            private int doubleClicksCount = 0;
            private float doubleClicksRestCounter = 0;
            private Quaternion MainPivotRotation;
            private bool isRotationDisabled = false;
            private bool isZoomDisabled = false;
            private bool self = false;
        #endregion

    #endregion

    #region Methods

        #region MonoBehaviour Overrides
            private void Start()
            {
                OrbiteCamera.transform.LookAt(transform);
                if (PlayerCamera == null)
                {
                    self = Camera.main!=null;
                    PlayerCamera = self? Camera.main.transform: OrbiteCamera.transform;
                }
                gameObject.SetActive(false);
            }
            private void OnEnable()
            {
                TurnToOrbitView(true);
                if(viewsDropDown!=null)
                    viewsDropDown.SetActive(true);
            }
            private void OnDisable()
            {
                if(viewsDropDown!=null)
                    viewsDropDown.SetActive(false);
            }
            void Update()
            {
                if (!isCameraPivot)
                {
                    OrbiteCamera.transform.LookAt(transform);
                }
                float zoomValue = Input.GetAxis("Mouse ScrollWheel");
                if (Input.GetMouseButtonDown(0)|| Input.GetMouseButtonDown(1))
                {
                    m_previousX = Input.mousePosition.x;
                    m_previousY = Input.mousePosition.y;
                }
                if (Input.GetMouseButton(0))
                {
                    DoRotate();    
                }
                if (Input.GetMouseButton(1))
                {
                    DoPaning();
                }

                if (Input.GetMouseButtonUp(0))
                {
                    Vector3 DirectionOfTheCameraNormlized = (transform.position - OrbiteCamera.transform.position).normalized;
                    bool isTheSameDirection = Vector3.Dot(DirectionOfTheCameraNormlized, transform.forward)>=0;
                    if (isCameraPivot&& isTheSameDirection)
                    {
                        ChangePivot(false);
                    }
                    else
                    {
                        doubleClicksCount += 1;
                        if (doubleClicksCount == 2)
                        {
                            doubleClicksCount = 0;
                            doubleClicksRestCounter = 0;
                            ChangePivot(true);
                        }
                    }
                }
                if(zoomValue != 0)
                {
                    DoZooming(zoomValue);
                }
                if (doubleClicksCount > 0)
                {
                    doubleClicksRestCounter += Time.deltaTime;
                    if (doubleClicksRestCounter >= 0.5f)
                    {
                        doubleClicksCount = 0;
                        doubleClicksRestCounter = 0;
                    }
                    
                }

                if (Input.GetKeyDown(KeyCode.R))
                {
                    OrbiteCamera.transform.localPosition = new Vector3(0, 0, -25f);
                    
                }

            }
            #endregion

        #region Public Methods
            public void DisableRotationFunc(bool isDisabled)
            {
                isRotationDisabled = isDisabled;
            }

            public void DisableZoomingFunc(bool isDisabled)
            {
                isZoomDisabled = isDisabled;
            }
            
            public void TurnToOrbitView(bool isOrbitView)
            {
                StartCoroutine(LerpFromOrToPlayerCamera(!isOrbitView));
            }
            public void StartLerpToSpacifcView(int dir)
            {
                Debug.Log(dir);
                ViewDirection direction;
                switch (dir)
                {
                    case 0:
                        direction = ViewDirection.Top;
                        break;
                    case 1:
                        direction = ViewDirection.Down;
                        break;
                    case 2:
                        direction = ViewDirection.Left;
                        break;
                    case 3:
                        direction = ViewDirection.Right;
                        break;
                    case 4:
                        direction = ViewDirection.Front;
                        break;
                    case 5:
                        direction = ViewDirection.Back;
                        break;
                    default:
                        direction = ViewDirection.Front;
                        break;
                }

                StartCoroutine(LerpToSpacifcView(direction));
            }   
        #endregion

        #region Private Methods

            private void DoRotate()
            {
                if(isRotationDisabled)
                    return;
                currentRotation.x += Input.GetAxis("Mouse X") * rotationSpeed;
                currentRotation.y -= Input.GetAxis("Mouse Y") * rotationSpeed;
                currentRotation.x = Mathf.Repeat(currentRotation.x, 360);
                if (isCameraPivot)
                {
                 OrbiteCamera.transform.localRotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);   
                }
                else
                {
                    currentRotation.y = Mathf.Clamp(currentRotation.y, -maxYRotationDegree, maxYRotationDegree);
                    transform.localRotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
                }

            }
            private void DoZooming(float zoomDirection)
            {
                if(isZoomDisabled)
                    return;
                if (!isCameraPivot)
                {
                    Vector3 newCameraPosition = OrbiteCamera.transform.localPosition;
                    newCameraPosition.z += (zoomDirection * zoomSpeed);
                    OrbiteCamera.transform.localPosition = newCameraPosition;
                    if (newCameraPosition.z > 0)
                    {
                        isCameraPivot = true;
                        if(viewsDropDown!=null)
                            viewsDropDown.SetActive(false);
                    }
                }
                else
                {
                    Vector3 newCameraPosition = OrbiteCamera.transform.position;
                    // if (newCameraPosition.z<0)
                    //     zoomDirection *= -1;
                    newCameraPosition += (OrbiteCamera.transform.forward*( zoomDirection * zoomSpeed));
                    OrbiteCamera.transform.position = newCameraPosition;
                    
                }

            }
            private void DoPaning()
            {
                float deltaX = Input.mousePosition.x - m_previousX;
                float deltaY = Input.mousePosition.y - m_previousY;
                if (deltaX != 0)
                {
                    Vector3 newPaningPosition = OrbiteCamera.transform.right;
                    newPaningPosition *= (deltaX*panningSpeed*Time.deltaTime);
                    transform.position -= newPaningPosition;
                    m_previousX = Input.mousePosition.x;
                }
                if (deltaY != 0)
                {
                    Vector3 newPaningPosition = OrbiteCamera.transform.up;
                    newPaningPosition *= (deltaY*panningSpeed*Time.deltaTime);
                    transform.position -= newPaningPosition;
                    m_previousY = Input.mousePosition.y;
                }
            }
            private void ChangePivot(bool isMousePosition)
            {
                RaycastHit hit;
                Ray ray;
                if (isMousePosition)
                {
                    ray = OrbiteCamera.ScreenPointToRay(Input.mousePosition);
                }
                else
                { 
                    OrbiteCamera.transform.parent = null;
                    ray = new Ray(OrbiteCamera.transform.position, OrbiteCamera.transform.forward);
                }

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "Model")
                    {
                        StartCoroutine(LerpPivotToPosition(hit.point,isMousePosition));
                    }
                }
            }
            IEnumerator LerpPivotToPosition(Vector3 newPivotPosition,bool isMousePosition)
            {
                if (isMousePosition)
                {
                    Vector3 currentPosition = transform.position;
                    for (float t = 0; t < 1; t += (Time.deltaTime / 0.5f))
                    {
                        yield return null;
                        transform.position = Vector3.Lerp(currentPosition, newPivotPosition, t);
                    }

                    transform.position = newPivotPosition;
                }
                else
                {
                    transform.position = newPivotPosition;
                    transform.rotation = MainPivotRotation;
                    transform.forward = OrbiteCamera.transform.forward;
                    OrbiteCamera.transform.parent = transform;
                    Vector3 currentPosition = OrbiteCamera.transform.localPosition;
                    Quaternion currentCameraRotation = OrbiteCamera.transform.localRotation;
                    Vector3 TargetPosition = new Vector3(0, 0, -25);
                    for (float t = 0; t < 1; t+= (Time.deltaTime / 0.5f))
                    {
                        yield return null;
                        OrbiteCamera.transform.localPosition = Vector3.Lerp(currentPosition,TargetPosition , t);
                        OrbiteCamera.transform.localRotation = Quaternion.Lerp(currentCameraRotation, Quaternion.identity, t);
                    }
                    OrbiteCamera.transform.localPosition = TargetPosition;
                    OrbiteCamera.transform.localRotation =Quaternion.identity;
                    OrbiteCamera.transform.LookAt(transform);
                    currentRotation = new Vector2(transform.localEulerAngles.y, transform.localEulerAngles.x);
                    isCameraPivot = false;
                    if(viewsDropDown!=null)
                        viewsDropDown.SetActive(true);
                }
            }
            IEnumerator LerpFromOrToPlayerCamera(bool toPlayer)
            {
                Vector3 startPos;
                Vector3 endPos;
                Quaternion startRot;
                Quaternion endRot;
                if (toPlayer)
                {
                    startPos = OrbiteCamera.transform.position;
                    endPos = PlayerCamera.position;
                    startRot = OrbiteCamera.transform.rotation;
                    endRot = PlayerCamera.rotation; 
                    for (float t = 0; t < 1; t+=(Time.deltaTime/1f))
                    {
                        yield return null;
                        OrbiteCamera.transform.position = Vector3.Lerp(startPos, endPos, t);
                        OrbiteCamera.transform.rotation = Quaternion.Lerp(startRot, endRot, t);
                    }
                    OrbiteCamera.transform.position = endPos;
                    OrbiteCamera.transform.rotation = endRot;
                    PlayerCamera.gameObject.SetActive(true);
                    yield return null;
                    yield return null;
                    gameObject.SetActive(false);
                }
                else
                {
                    OrbiteCamera.transform.localPosition = Vector3.zero;
                    transform.position = PlayerCamera.position;
                    transform.rotation = PlayerCamera.rotation;
                    startPos = OrbiteCamera.transform.localPosition;
                    endPos = new Vector3(0, 0, -25f);
                    Quaternion orbitCenterStartLocalRot = transform.localRotation;
                    Quaternion orbitCenterEndLocalRot = Quaternion.Euler(45,transform.localEulerAngles.y,transform.localEulerAngles.z);
                    for (float t = 0; t < 1; t+=(Time.deltaTime/1f))
                    {
                        yield return null;
                        if (t < 0.5f)
                        {
                            transform.rotation = Quaternion.Lerp( orbitCenterStartLocalRot, orbitCenterEndLocalRot, t*2);    
                        }
                        else
                        {
                            transform.rotation = orbitCenterEndLocalRot;
                        }

                        OrbiteCamera.transform.localPosition = Vector3.Lerp(startPos, endPos, t);
                    }
                    currentRotation = new Vector2(transform.localEulerAngles.y, transform.localEulerAngles.x);
                    OrbiteCamera.transform.localPosition = endPos;
                    if(!self)
                        PlayerCamera.gameObject.SetActive(false);
                }
            }
            IEnumerator LerpToSpacifcView(ViewDirection direction)
            {
                Quaternion directionVector =getDirection(direction);
                Quaternion currentCenterRotation = transform.localRotation;
             
                for (float t = 0; t < 1; t+=(Time.deltaTime/0.5f))
                {
                    yield return null;
                    transform.localRotation = Quaternion.Lerp(currentCenterRotation, directionVector, t);
                }
                transform.localRotation = directionVector;
                OrbiteCamera.transform.LookAt(transform);
            }
            private Quaternion getDirection(ViewDirection direction)
            {
                Quaternion dirctioVector=Quaternion.identity;
                switch (direction)
                {
                    case ViewDirection.Front:
                        dirctioVector= Quaternion.AngleAxis(0,Vector3.up);
                        currentRotation = new Vector2(dirctioVector.eulerAngles.y, dirctioVector.eulerAngles.x);
                        break;
                    case  ViewDirection.Back:
                        dirctioVector =  Quaternion.AngleAxis(180,Vector3.up);
                        currentRotation = new Vector2(dirctioVector.eulerAngles.y, dirctioVector.eulerAngles.x);
                        break;
                    case ViewDirection.Left:
                        dirctioVector= Quaternion.AngleAxis(90,Vector3.up);
                        currentRotation = new Vector2(dirctioVector.eulerAngles.y, dirctioVector.eulerAngles.x);
                        break;
                    case ViewDirection.Right:
                        dirctioVector= Quaternion.AngleAxis(270,Vector3.up);
                        currentRotation = new Vector2(dirctioVector.eulerAngles.y, dirctioVector.eulerAngles.x);
                        break;
                    case ViewDirection.Top:
                        dirctioVector = Quaternion.AngleAxis(89,Vector3.right);
                        currentRotation = new Vector2(dirctioVector.eulerAngles.y, dirctioVector.eulerAngles.x);
                        break;
                    case ViewDirection.Down:
                        dirctioVector= Quaternion.AngleAxis(-89,Vector3.right);
                        currentRotation = new Vector2(dirctioVector.eulerAngles.y, -89);
                        break;
                  
                    default:
                        dirctioVector= Quaternion.AngleAxis(0,Vector3.up);
                        currentRotation = new Vector2(dirctioVector.eulerAngles.y, dirctioVector.eulerAngles.x);
                        break;
                }
                
                return dirctioVector;
            }

        #endregion
    #endregion
}
