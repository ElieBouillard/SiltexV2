using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocalHealthController : PlayerHealthController
{
    public override void Death()
    {
        base.Death();
        GetComponent<PlayerIdentity>().Initialize(false);
        NetworkManager.Instance.ClientMessage.SendOnDeath();
    }
}
