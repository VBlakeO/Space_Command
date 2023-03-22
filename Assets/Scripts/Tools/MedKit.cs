public class MedKit : InteractiveObject
{
    protected override void OnInteract()
    {
        base.OnInteract();

        SkillManager.m_Instance.AddMedKit(1);
        Destroy(gameObject);
    }
}
