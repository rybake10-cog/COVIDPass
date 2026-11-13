// © 2020 Partners HealthCare Technology & Innovation Department
import { Action } from 'redux';

import { ActionType } from 'data/actionTypes';

export interface AppAction<T> extends Action<ActionType> {
  data: T;
  mod?: string;
  callBack?: (error?: any) => void;
}

export interface Dictionary<V> {
  [key: string]: V;
}
