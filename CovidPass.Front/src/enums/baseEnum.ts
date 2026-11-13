// © 2020 Partners HealthCare Technology & Innovation Department
interface PropertyMap {
  [id: number]: string;
}

export interface EnumItem {
  id: number;
  name: string;
}

export class BaseEnum {
  private _map: PropertyMap = {};
  private _list: EnumItem[] = [];

  protected init(self: BaseEnum) {
    const entries = Object.entries(self).filter(([name]) => !name.startsWith('_'));
    this._map = entries.reduce<PropertyMap>((p, [name, id]) => {
      p[id] = this.nameConverter(id, name);
      return p;
    }, {});
    this._list = entries.map(([, id]) => ({ id, name: this.getName(+id) }));
  }

  public getName(id: number) {
    return this._map[id] != null ? this._map[id] : '???';
  }

  // You should override function in inheritor to apply custom names
  // nameConverter(id: number, name: string) {
  //   return id === this.MyField ? 'My Super Field Name' : super.nameConverter(id, name);
  // }

  protected nameConverter(id: number, name: string) {
    return name.replace(/([A-z](?=[A-Z0-9]))/g, '$1 ').replace(/([0-9](?=[A-z]))/g, '$1 ');
  }

  public get all() {
    return this._list;
  }
}

export const getEnumItemValue = (item: EnumItem) => item.id;
export const getEnumItemName = (item: EnumItem) => item.name;
