using UnityEngine;

public partial class Player
{
    #region Drawing
    private bool bDrawing = false;
    private bool bSheathing = false;
    private bool bEquipped = false;
    [SerializeField]
    private bool bHaveItem = false;

    private void UpdateDrawing()
    {
        if (Input.GetButtonDown("Sword") == false)
            return;

        //if (bHaveItem == false)
        //    return;

        if (bDrawing == true)
            return;

        if (bSheathing == true)
            return;

        if (swordPrefab != null)
        {
            swordDestination = Instantiate<GameObject>(swordPrefab, holsterTransform);
            sword = swordDestination.GetComponent<Sword>();
        }


        if (bEquipped == false)
        {
            bDrawing = true;
            animator.SetBool("IsEquip", bDrawing);

            return;
        }

        
        bSheathing = true;
        animator.SetBool("IsUnequip", bSheathing);
    }

    private void Begin_Equip()
    {
        swordDestination.transform.parent.DetachChildren();
        swordDestination.transform.position = Vector3.zero;
        swordDestination.transform.rotation = Quaternion.identity;
        swordDestination.transform.localScale = Vector3.one;

        swordDestination.transform.SetParent(handTransform, false);
    }

    private void End_Equip()
    {
        bEquipped = true;

        bDrawing = false;
        animator.SetBool("IsEquip", false);
    }

    private void Begin_Unequip()
    {
        swordDestination.transform.parent.DetachChildren();
        swordDestination.transform.position = Vector3.zero;
        swordDestination.transform.rotation = Quaternion.identity;
        swordDestination.transform.localScale = Vector3.one;

        swordDestination.transform.SetParent(holsterTransform, false);
    }

    private void End_Unequip()
    {
        bEquipped = false;

        bSheathing = false;
        animator.SetBool("IsUnequip", false);
    }
    #endregion

    #region Attacking
    private bool bAttacking = false;

    private bool bComboEnable;
    private bool bComboExist;
    
    private int comboIndex;
    public int ComboIndex => comboIndex;

    private void UpdateAttacking()
    {
        if (Input.GetButtonDown("Attack") == false)
            return;

        //if (bDrawing || bSheathing)
        //    return;

        //if (bEquipped == false)
        //    return;

        if(bComboEnable)
        {
            bComboEnable = false;
            bComboExist = true;

            return;
        }

        if (bAttacking == true)
            return;


        bCanMove = false;

        bAttacking = true;
        animator.SetBool("IsAttacking", true);
        //animator.SetTrigger("Attacking");
    }


    private void Begin_Combo()
    {
        bComboEnable = true;
    }

    private void End_Combo()
    {
        bComboEnable = false;
    }

    private void Begin_Attack()
    {
        if (bComboExist == false)
            return;

        
        bComboExist = false;

        comboIndex++;
        animator.SetTrigger("NextCombo");
    }

    private void End_Attack()
    {
        bCanMove = true;
        bAttacking = false;
        animator.SetBool("IsAttacking", false);

        comboIndex = 0;
    }

    private void Begin_Collision()
    {
        sword.Begin_Collision();
    }

    private void End_Collision()
    {
        sword.End_Collision();
    }
    #endregion
}