/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using UnityEngine;

namespace TanksMP
{
    /// <summary>
    /// Camera script for following the player or a different target transform.
    /// Extended with ability to hide certain layers (e.g. UI) while in "follow mode".
    /// </summary>
    public class FollowTarget : MonoBehaviour
    {
        /// <summary>
        /// Added by: Jilmer John Cariaso
        /// </summary>
        public Vector3 position = new Vector3(0, 10, -15);

        /// <summary>
        /// Added by: Jilmer John Cariaso
        /// </summary>
        public Vector3 rotation = new Vector3(15, 0, 0);

        /// <summary>
        /// The camera target to follow.
        /// Automatically picked up in LateUpdate().
        /// </summary>
        public Transform target;
        
        /// <summary>
        /// Layers to hide after calling HideMask().
        /// </summary>
        public LayerMask respawnMask;

        /// <summary>
        /// The clamped distance in the x-z plane to the target.
        /// </summary>
        public float distance = 10.0f;
        
        /// <summary>
        /// The clamped height the camera should be above the target.
        /// </summary>
        public float height = 5.0f;

        private Camera cam;
        
        private Transform camTransform;

        public Camera Cam { get => cam; }

        public Transform CamTransform { get => camTransform; }
        
        
        //initialize variables
        void Start()
        {
            cam = GetComponent<Camera>();
            camTransform = transform;

            //the AudioListener for this scene is not attached directly to this camera,
            //but to a separate gameobject parented to the camera. This is because the
            //camera is usually positioned above the player, however the AudioListener
            //should consider audio clips from the position of the player in 3D space.
            //so here we position the AudioListener child object at the target position.
            //Remark: parenting the AudioListener to the player doesn't work, because
            //it gets disabled on death and therefore stops playing sounds completely
            Transform listener = GetComponentInChildren<AudioListener>().transform;
            listener.position = transform.position + transform.forward * distance;
        }


        /* ORIGINAL
        //position the camera in every frame
        void LateUpdate()
        {
            //cancel if we don't have a target
            if (!target)
                return;

            //convert the camera's transform angle into a rotation
            Quaternion currentRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

            //set the position of the camera on the x-z plane to:
            //distance units behind the target, height units above the target
            Vector3 pos = target.position;
            pos -= currentRotation * Vector3.forward * Mathf.Abs(distance);
            pos.y = target.position.y + Mathf.Abs(height);
            transform.position = pos;

            //look at the target
            transform.LookAt(target);

            //clamp distance
            transform.position = target.position - (transform.forward * Mathf.Abs(distance));
        }*/

        /* MODIFIED
        //position the camera in every frame
        void LateUpdate()
        {
            //cancel if we don't have a target
            if (!target)
                return;

            //convert the camera's transform angle into a rotation
            Quaternion currentRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

            //set the position of the camera on the x-z plane to:
            //distance units behind the target, height units above the target
            Vector3 pos = target.position;
            pos -= currentRotation * Vector3.forward * Mathf.Abs(distance);
            pos.y = Mathf.Abs(height);
            transform.position = pos;

            //look at the target
            transform.LookAt(target);

            transform.rotation = Quaternion.Euler(50, transform.eulerAngles.y, transform.eulerAngles.z);

            //clamp distance
            transform.position = target.position - (transform.forward * Mathf.Abs(distance));

            transform.position = new Vector3(transform.position.x, height, transform.position.z);
        }*/

        /* 3RD PERSON */
        void LateUpdate()
        {
            //cancel if we don't have a target
            if (!target)
                return;

            //transform.parent = target;

            var shakeDeg = 0;//Mathf.Clamp(target.eulerAngles.x, -5, 5);

            var angleDeg = target.eulerAngles.y;



            transform.rotation = Quaternion.Euler(new Vector3(shakeDeg, angleDeg, 0) + rotation);

            var angleRad = transform.eulerAngles.y * Mathf.Deg2Rad;

            transform.position = target.position + new Vector3(Mathf.Sin(angleRad) * position.z, position.y, Mathf.Cos(angleRad) * position.z);

            
        }


        /// <summary>
        /// Culls the specified layers of 'respawnMask' by the camera.
        /// </summary>
        public void HideMask(bool shouldHide)
        {
            if(shouldHide) cam.cullingMask &= ~respawnMask;
            else cam.cullingMask |= respawnMask;
        }
    }
}