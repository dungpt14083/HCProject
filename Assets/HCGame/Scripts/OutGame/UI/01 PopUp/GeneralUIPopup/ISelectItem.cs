using System;

public interface ISelectItem<TData>
{
    void OnShow(Action<TData> onClick);
    void OnSelectFilter(TData info);
}
