// © 2020 Partners HealthCare Technology & Innovation Department
class Counter {
  private _value: number = 0;

  public get next() {
    return ++this._value;
  }
}

export const counter = new Counter();
