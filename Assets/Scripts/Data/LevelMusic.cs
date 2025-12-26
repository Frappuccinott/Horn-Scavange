using System.Collections.Generic;
using UnityEngine;

public class LevelMusic : MonoBehaviour
{
    [Header("Level Music Pool")]
    [Tooltip("Bu level'da çalacak müzikler:")]
    public List<AudioClip> musicClips;
}
