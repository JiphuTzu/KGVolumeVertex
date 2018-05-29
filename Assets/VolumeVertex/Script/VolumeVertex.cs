using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public struct targetData
{
    [SerializeField]
    GameObject m_targetObj;
    public GameObject targetObj
    {
        get { return m_targetObj; }
        set { m_targetObj = value; }
    }

    [SerializeField, ColorUsage(false, true)]
    Color m_emissionColor;
    public Color emissionColor
    {
        get { return m_emissionColor; }
        set { m_emissionColor = value; }
    }

    [SerializeField]
    float m_powItensity;
    public float powItensity
    {
        get { return m_powItensity; }
        set { m_powItensity = value; }
    }
}


[ExecuteInEditMode, AddComponentMenu("KG/VolumeVertex")]
public class VolumeVertex : MonoBehaviour {

    [SerializeField]
    List<targetData> m_target = new List<targetData>();
	public List<targetData> Target
    {
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
                    select new Vector4(_s.targetObj.transform.position.x, 
                    _s.targetObj.transform.position.y, 
                    _s.targetObj.transform.position.z,
                    1);

        var _color = from _s in m_target
                   select _s.emissionColor;

        var _pow = from _s in m_target
                     select _s.powItensity;

        m_mat.SetVectorArray("_TargetPos", _pos.ToList());
        m_mat.SetColorArray("_TargetColor", _color.ToList());
        m_mat.SetFloatArray("_TargetPow", _pow.ToList());
        m_mat.SetInt("_TargetCount", _pos.ToList().Count());
        m_mat.SetFloat("_AffectRange",m_affectRange);

	}
}
