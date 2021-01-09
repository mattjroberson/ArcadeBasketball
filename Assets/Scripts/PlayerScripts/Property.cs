using Extensions;

public class Property
{
    public float Value { get; private set; }

    private Attribute attribute;
    private float min, max;

    public Property(Attribute attribute, float min, float max)
    {
        this.attribute = attribute;
        this.min = min;
        this.max = max;

        SetValue();
        attribute.onValueSet += SetValue;
    }

   private void SetValue()
    {
        Value = attribute.GetValue();
        Value = Value.Remap(Attribute.MIN, Attribute.MAX, min, max);
    }
}


