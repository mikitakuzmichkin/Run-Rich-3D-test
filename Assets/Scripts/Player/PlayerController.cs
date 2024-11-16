using ButchersGames;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float sensitivity;
        [SerializeField] private float maxRotationSpeed;
        [SerializeField] private float trackWidth = 3f;

        private Pathway _pathway;

        private Transform _origin;
        private float _rotation;

        private float _oldPos;

        private void Awake()
        {
            _origin = transform.parent;
        }

        public void ResetToStart(Level level)
        {
            _pathway = level.transform.GetComponentInChildren<Pathway>();
            _origin.position = level.PlayerSpawnPoint;
            _origin.rotation = Quaternion.identity;
            _rotation = 0;
            transform.localPosition = Vector3.zero;
            _pathway.Reset();
        }

        private void FixedUpdate()
        {
            OnMove();
            
            if (!GameManager.Instance.GameIsOn) return;
            var targetRotation = _pathway.GetTargetRotation(_origin.position);
            var targetDeltaRotation = targetRotation - _rotation;
            if (Mathf.Abs(targetDeltaRotation) < maxRotationSpeed) _rotation = targetRotation;
            else _rotation += Mathf.Sign(targetDeltaRotation) * maxRotationSpeed;
            _origin.rotation = Quaternion.AngleAxis(_rotation, Vector3.up);
            _origin.Translate(Time.fixedDeltaTime * speed * Vector3.forward);
        }

        public void OnMove()
        {
            var gameManager = GameManager.Instance;
            
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                _oldPos = Input.mousePosition.x ;
            }
            
            if (Input.GetMouseButton(0) == false) return;
            
            var value = (Input.mousePosition.x - _oldPos) / Screen.width ;
            
            _oldPos = Input.mousePosition.x;
#else
            if (Input.touchCount < 1) return;

            var touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                _oldPos = Input.mousePosition.x ;
            }

            if (touch.phase == TouchPhase.Moved) return;
            
            var value = (touch.position.x - _oldPos) / Screen.width;
            
            _oldPos = touch.position.x;
#endif
            
            if (gameManager.WaitingForMove && value > 0)
            {
                gameManager.StartGame();
            }
            
            if (!gameManager.GameIsOn) return;
            
            var targetDeltaPosition = value * trackWidth;
            var targetPosition = transform.localPosition.x + targetDeltaPosition;
            if (targetPosition > trackWidth / 2f)
            {
                targetDeltaPosition -= targetPosition - trackWidth / 2f;
            }

            if (targetPosition < -trackWidth / 2f)
            {
                targetDeltaPosition -= targetPosition + trackWidth / 2f;
            }
            transform.Translate(targetDeltaPosition * Vector3.right);
        }
    }
}
