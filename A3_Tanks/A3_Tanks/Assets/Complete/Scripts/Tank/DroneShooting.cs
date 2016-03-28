using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace Complete
{
    public class DroneShooting : NetworkBehaviour
    {       
        public Rigidbody2D m_Shell;                   // Prefab of the shell.       
        public AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
        public AudioClip m_ChargingClip;            // Audio that plays when each shot is charging up.
        public AudioClip m_FireClip;                // Audio that plays when each shot is fired.
        public float m_MinLaunchForce = 15f;        // The force given to the shell if the fire button is not held.
        public float m_MaxLaunchForce = 30f;        // The force given to the shell if the fire button is held for the max charge time.
        public float m_MaxChargeTime = 0.75f;       // How long the shell can charge for before it is fired at max force.


        private float m_CurrentLaunchForce;         // The force that will be given to the shell when the fire button is released.
        private float m_ChargeSpeed;                // How fast the launch force increases, based on the max charge time.
        private bool m_Fired;                       // Whether or not the shell has been launched with this button press.
       

        private void Update ()
        {
            if (!isLocalPlayer)
            {
                return;
            }
        }

        public void Fire()
        {
            if (!isLocalPlayer)
            {
                return;
            }
            CmdFire();
        }

        [Command]
        private void CmdFire ()
        {
            // Set the fired flag so only Fire is only called once.
            m_Fired = true;

            // Create an instance of the shell and store a reference to it's rigidbody.
            Rigidbody2D shellInstance =
                Instantiate (m_Shell, transform.position, transform.rotation) as Rigidbody2D;

            // Set the shell's velocity to the launch force in the fire position's forward direction.
            shellInstance.velocity = m_CurrentLaunchForce * transform.forward;

            NetworkServer.Spawn(shellInstance.gameObject);        

            // Change the clip to the firing clip and play it.
            m_ShootingAudio.clip = m_FireClip;
            m_ShootingAudio.Play ();

            // Reset the launch force.  This is a precaution in case of missing button events.
            m_CurrentLaunchForce = m_MinLaunchForce;
        }
    }
}