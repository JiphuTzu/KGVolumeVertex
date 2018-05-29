using UnityEngine;
using System.Linq;

[ExecuteInEditMode, AddComponentMenu("KG/VolumeVertex")]
public class VolumeVertex : MonoBehaviour {

    [System.Serializable]
    public struct targetData
    {
        public GameObject m_targetObj;

        [ColorUsage(false, true)]
        public Color m_emissionColor;

        public float m_powItensity;
    }

	[SerializeField]
    targetData[] m_target;
	public targetData[] Target{
		get{return m_target; }
		set{ m_target = value;}
	}

	[SerializeField]
	float m_affectRange;
	public float AffectRange{
		get{return m_affectRange;}
		set{m_affectRange = value;}
	}

    [SerializeField]
    Material m_mat;

	// Update is called once per frame
	void Update () {

        if (m_mat == null)
            return;

        if (m_target == null)
            return;

        try
        {
            GetComponent<MeshRenderer>().sharedMaterial = m_mat;
        }
        catch
        {
            GetComponent<SkinnedMeshRenderer>().sharedMaterial = m_mat;
        }

         var _pos = from _s in m_target
                    select new Vector4(_s.m_targetObj.transform.position.x, 
                    _s.m_targetObj.transform.position.y, 
                    _s.m_targetObj.transform.position.z,
                    1);

        var _color = from _s in m_target
                   select _s.m_emissionColor;

        var _pow = from _s in m_target
                     select _s.m_powItensity;

        m_mat.SetVectorArray("_TargetPos", _pos.ToList());
        m_mat.SetColorArray("_TargetColor", _color.ToList());
        m_mat.SetFloatArray("_TargetPow", _pow.ToList());
        m_mat.SetInt("_TargetCount", _pos.ToList().Count());
        m_mat.SetFloat("_AffectRange",m_affectRange);

	}
}
