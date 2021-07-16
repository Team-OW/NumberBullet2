namespace Treevel.Common.Entities
{
    public enum EBottleAttributeType
    {
        Dark,
        Reverse,
        Ghost,
        Life,
    }

    public static class BottleAttributeTypeExtension
    {
        /// <summary>
        /// レイヤー内の描画順序を取得する
        /// 0番目はBottleの描画順序なので、Attributeの順序は1番以降
        /// </summary>
        public static int GetOrderInLayer(this EBottleAttributeType type)
        {
            switch (type) {
                case EBottleAttributeType.Dark:
                    return 10;
                case EBottleAttributeType.Reverse:
                    return 20;
                case EBottleAttributeType.Ghost:
                    return 30;
                case EBottleAttributeType.Life:
                    return 40;
                default:
                    return 0;
            }
        }
    }
}
