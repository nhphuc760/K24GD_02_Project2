using System;
using UnityEngine;

public class AnimalEvent 
{
    public event Action<FarmAnimal> onBuyAnimal;
    public void BuyAnimal(FarmAnimal animal)
    {
        onBuyAnimal?.Invoke(animal);
    }
    public event Action<ProductDataSO> onGetProduct;
    public void OnGetProduct(ProductDataSO sO)
    {
        onGetProduct?.Invoke(sO);
    }
}
