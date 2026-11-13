// © 2020 Partners HealthCare Technology & Innovation Department
import { BaseEnum } from './baseEnum';

class Type extends BaseEnum {
  public Current = 1;
  public Previous = 2;
  public Future = 3;

  public constructor() {
    super();
    this.init(this);
  }
}

export const CycleStatus = Object.freeze(new Type());
